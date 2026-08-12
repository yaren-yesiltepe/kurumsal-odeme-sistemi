namespace PaymentSystem.Api.Models;

public class Payment
{
    public int Id { get; set; }
    public string ReferenceNo { get; set; } = string.Empty;
    public string MerchantName { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
    public string Last4 { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    public string Status { get; set; } = string.Empty;
    public string? FailReason { get; set; }
    public DateTime CreatedAt { get; set; }
}
