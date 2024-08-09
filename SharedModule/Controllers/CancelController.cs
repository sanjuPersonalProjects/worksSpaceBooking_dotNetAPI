using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WorkSpaceBooking.Models;

namespace WorkSpaceBooking1.SharedModule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancelController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CancelController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("CancelBooking")]
        public IActionResult CancelBooking([FromBody] CancelRequest requestData)
        {
            try
            {
                // Validate requestData
                if (requestData == null || requestData.BookingId <= 0)
                {
                    return BadRequest("Invalid booking data");
                }

                string connectionString = _configuration.GetConnectionString("YourDatabaseConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("CancelBooking", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Set the parameter for the stored procedure.
                        command.Parameters.Add(new SqlParameter("@bookingId", requestData.BookingId));

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                // Return a JSON response
                var response = new { success = true, message = "Booking successfully cancelled" };
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                return StatusCode(500, new { success = false, message = "An error occurred while cancelling the booking", error = ex.Message });
            }
        }

    }
}
