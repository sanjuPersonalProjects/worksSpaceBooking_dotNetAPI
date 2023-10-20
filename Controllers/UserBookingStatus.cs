using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using WorkSpaceBooking1.Model;
using Microsoft.Extensions.Configuration;

namespace WorkSpaceBooking1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserBookingStatus : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UserBookingStatus(IConfiguration configuration) // Corrected constructor name
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetBookingData(DateTime bookingDate, string bookingTime, string bookedRoom, int? employeeId = null)
        {
            try
            {
                List<UserBookingStatusDTO> bookings = new List<UserBookingStatusDTO>();

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("YourDatabaseConnection")))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand("GetWorkspaceStatus", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@bookingDate", bookingDate);
                        cmd.Parameters.AddWithValue("@bookingTime", bookingTime);
                        cmd.Parameters.AddWithValue("@bookedRoom", bookedRoom);
                        cmd.Parameters.AddWithValue("@employeeId", employeeId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserBookingStatusDTO booking = new UserBookingStatusDTO
                                {
                                    Workspace = reader.GetString(0),
                                    Status = reader.GetString(1)
                                };
                                bookings.Add(booking);
                            }
                        }
                    }
                }

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
