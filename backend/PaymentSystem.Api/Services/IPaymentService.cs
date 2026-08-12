using PaymentSystem.Api.Models;

namespace PaymentSystem.Api.Services;

public interface IPaymentService
{
    Task<Payment> ProcessAsync(CreatePaymentRequest req);
    Task<IEnumerable<Payment>> GetRecentAsync(int take, string? status);
    Task<Payment?> GetByIdAsync(int id);
    Task<IEnumerable<DailyVolumeRow>> GetDailyVolumeAsync(int daysBack);
}
