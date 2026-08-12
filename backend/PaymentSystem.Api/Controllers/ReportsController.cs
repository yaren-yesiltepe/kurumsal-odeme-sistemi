using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Api.Services;

namespace PaymentSystem.Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(IPaymentService paymentService, ILogger<ReportsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [HttpGet("daily-volume")]
    public async Task<IActionResult> GetDailyVolume([FromQuery] int daysBack = 30)
    {
        if (daysBack is < 1 or > 365) daysBack = 30;

        try
        {
            var rows = await _paymentService.GetDailyVolumeAsync(daysBack);
            return Ok(rows);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Daily volume report failed for daysBack={DaysBack}", daysBack);
            return StatusCode(500, new { error = "Report generation failed." });
        }
    }
}
