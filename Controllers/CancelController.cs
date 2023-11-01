using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using WorkSpaceBooking.Models;

namespace WorkSpaceBooking1.Controllers
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

        [HttpPut("CancelBooking")]
        public IActionResult CancelBooking([FromBody]int bookingId)
        {
            string connectionString = _configuration.GetConnectionString("YourDatabaseConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("CancelBooking", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Set the parameter for the stored procedure.
                    command.Parameters.Add(new SqlParameter("@bookingId", bookingId) );

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            return Ok("Booking Cancelled");
        }
    }
}
