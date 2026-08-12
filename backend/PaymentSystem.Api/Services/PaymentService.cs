using Dapper;
using PaymentSystem.Api.Data;
using PaymentSystem.Api.Models;

namespace PaymentSystem.Api.Services;

public class PaymentService : IPaymentService
{
    private readonly ISqlConnectionFactory _connFactory;
    private readonly ILogger<PaymentService> _logger;
    private static readonly Random _rng = new();

    public PaymentService(ISqlConnectionFactory connFactory, ILogger<PaymentService> logger)
    {
        _connFactory = connFactory;
        _logger = logger;
    }

    public async Task<Payment> ProcessAsync(CreatePaymentRequest req)
    {
        // gercek bir gateway yok, burada işlemi mock authorization ile simule ediyoruz
        var (status, failReason) = SimulateAuthorization(req);
        var last4 = req.CardNumber.Length >= 4
            ? req.CardNumber[^4..]
            : req.CardNumber.PadLeft(4, '0');

        var payment = new Payment
        {
            ReferenceNo = $"TXN{DateTime.UtcNow:yyMMdd}{_rng.Next(1000, 9999)}",
            MerchantName = req.MerchantName.Trim(),
            CardType = req.CardType.ToUpperInvariant(),
            Last4 = last4,
            Amount = req.Amount,
            Currency = string.IsNullOrWhiteSpace(req.Currency) ? "TRY" : req.Currency.ToUpperInvariant(),
            Status = status,
            FailReason = failReason,
            CreatedAt = DateTime.UtcNow
        };

        const string sql = @"
            INSERT INTO dbo.Payments (ReferenceNo, MerchantName, CardType, Last4, Amount, Currency, Status, FailReason, CreatedAt)
            OUTPUT INSERTED.Id
            VALUES (@ReferenceNo, @MerchantName, @CardType, @Last4, @Amount, @Currency, @Status, @FailReason, @CreatedAt);";

        using var conn = _connFactory.Create();
        try
        {
            payment.Id = await conn.ExecuteScalarAsync<int>(sql, payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Payment insert failed for merchant {Merchant}", payment.MerchantName);
            throw;
        }

        return payment;
    }

    public async Task<IEnumerable<Payment>> GetRecentAsync(int take, string? status)
    {
        var sql = @"
            SELECT TOP (@Take) Id, ReferenceNo, MerchantName, CardType, Last4, Amount, Currency, Status, FailReason, CreatedAt
            FROM dbo.Payments
            WHERE (@Status IS NULL OR Status = @Status)
            ORDER BY CreatedAt DESC;";

        using var conn = _connFactory.Create();
        return await conn.QueryAsync<Payment>(sql, new { Take = take, Status = status });
    }

    public async Task<Payment?> GetByIdAsync(int id)
    {
        const string sql = "SELECT * FROM dbo.Payments WHERE Id = @Id;";
        using var conn = _connFactory.Create();
        return await conn.QueryFirstOrDefaultAsync<Payment>(sql, new { Id = id });
    }

    public async Task<IEnumerable<DailyVolumeRow>> GetDailyVolumeAsync(int daysBack)
    {
        using var conn = _connFactory.Create();
        return await conn.QueryAsync<DailyVolumeRow>(
            "dbo.usp_GetDailyVolumeReport",
            new { DaysBack = daysBack },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    private static (string status, string? failReason) SimulateAuthorization(CreatePaymentRequest req)
    {
        // basit mock kural seti - gercek issuer/acquirer davranisini taklit ediyor
        if (req.Amount > 50000)
            return ("FAILED", "Limit exceeded");

        var roll = _rng.Next(1, 101);
        return roll switch
        {
            <= 88 => ("SUCCESS", null),
            <= 95 => ("FAILED", "Insufficient funds"),
            <= 98 => ("FAILED", "3D Secure timeout"),
            _ => ("PENDING", null)
        };
    }
}
