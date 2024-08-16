using Microsoft.AspNetCore.Mvc;
using WorkSpaceBooking1.AdminModule.Services;

namespace WorkSpaceBooking1.AdminModule.Contollers
{
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class ChartsController : ControllerBase
    {
        private readonly IChartsService _ChartService;



        public ChartsController(IChartsService ChartService)
        {
            _ChartService = ChartService;
        }
        


        [HttpGet("genderCharts")]
        public async Task<IActionResult> GetBookingsByGenderChart([FromQuery] string date)
        {

            if (!DateOnly.TryParse(date, out DateOnly parsedDate))
            {
                return BadRequest("Invalid date format. Please use YYYY-MM-DD.");
            }
            var result = await _ChartService.GetBookingsByGenderChartAsync(parsedDate);
            if (result == null)
            {
                return NotFound();
            }
            
            return Ok(result);
        }
    }

}
