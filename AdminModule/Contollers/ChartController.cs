using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WorkSpaceBooking1.AdminModule.Services;

namespace WorkSpaceBooking1.AdminModule.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChartsController : ControllerBase
    {
        private readonly IChartsService _chartService;
        private readonly ILogger<ChartsController> _logger;

        public ChartsController(IChartsService chartService, ILogger<ChartsController> logger)
        {
            _chartService = chartService;
            _logger = logger;
        }

        [HttpGet("genderCharts")]
        public async Task<IActionResult> GetBookingsByGenderChart([FromQuery] string date)
        {
            _logger.LogInformation("Received request to get bookings by gender chart for date: {Date}", date);

            if (!DateOnly.TryParse(date, out DateOnly parsedDate))
            {
                _logger.LogWarning("Invalid date format received: {Date}", date);
                return BadRequest("Invalid date format. Please use YYYY-MM-DD.");
            }

            var result = await _chartService.GetBookingsByGenderChartAsync(parsedDate);

            if (result == null)
            {
                _logger.LogWarning("No bookings found for date: {Date}", parsedDate);
                return NotFound();
            }

            _logger.LogInformation("Successfully retrieved bookings by gender chart for date: {Date}", parsedDate);
            return Ok(result);
        }
    }
}
