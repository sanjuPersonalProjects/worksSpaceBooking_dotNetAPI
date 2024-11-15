using Microsoft.AspNetCore.Mvc;

using System.Data;
using System.Data.SqlClient;
using WorkSpaceBooking1.Model;

namespace WorkSpaceBooking1.AdminModule.Contollers
{

    [Route("api/[controller]")]
    [ApiController]

    public class GraphController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public GraphController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [HttpGet]
        public IActionResult GetBookedCountsByDateRange(DateTime startDate, DateTime endDate)
        {
            List<BookingCountDto> bookingCounts = new List<BookingCountDto>();

            string connectionString = _configuration.GetConnectionString("YourDatabaseConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetBookedCountsByDateRange", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters for the stored procedure
                    command.Parameters.Add("@StartDate", SqlDbType.Date).Value = startDate;
                    command.Parameters.Add("@EndDate", SqlDbType.Date).Value = endDate;

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            BookingCountDto bookingCount = new BookingCountDto
                            {
                                BookingDate = (DateTime)reader["bookingDate"],
                                MorningCount = reader.IsDBNull(reader.GetOrdinal("MorningCount")) ? 0 : (int)reader["MorningCount"],
                                AfternoonCount = reader.IsDBNull(reader.GetOrdinal("AfternoonCount")) ? 0 : (int)reader["AfternoonCount"]
                            };

                            bookingCounts.Add(bookingCount);
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }
            }

            return Ok(bookingCounts);
        }

    }
}
