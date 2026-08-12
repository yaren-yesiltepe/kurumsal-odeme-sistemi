using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Api.Models;
using PaymentSystem.Api.Services;

namespace PaymentSystem.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _paymentService.ProcessAsync(req);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while processing payment");
            return StatusCode(500, new { error = "Payment could not be processed." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetRecent([FromQuery] int take = 50, [FromQuery] string? status = null)
    {
        if (take is < 1 or > 500) take = 50;
        var items = await _paymentService.GetRecentAsync(take, status);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        return payment is null ? NotFound() : Ok(payment);
    }
}
