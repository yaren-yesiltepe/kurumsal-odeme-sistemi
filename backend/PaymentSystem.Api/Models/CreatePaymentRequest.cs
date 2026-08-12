using System.ComponentModel.DataAnnotations;

namespace PaymentSystem.Api.Models;

public class CreatePaymentRequest
{
    [Required, MaxLength(120)]
    public string MerchantName { get; set; } = string.Empty;

    [Required]
    public string CardNumber { get; set; } = string.Empty; // mock, we only keep last4

    [Required]
    public string CardType { get; set; } = string.Empty; // VISA / MASTERCARD / TROY / AMEX

    [Range(0.01, 1000000)]
    public decimal Amount { get; set; }

    public string Currency { get; set; } = "TRY";
}
