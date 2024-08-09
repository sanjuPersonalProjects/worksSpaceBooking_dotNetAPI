using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WorkSpaceBooking.Models;
using WorkSpaceBooking1.Model;

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
        [Authorize(Policy = "UserPolicy")]
        [HttpGet("upcomingBookingsForUser/{employeeId}")]
        public IActionResult GetUpcomingBookingsForUser(int employeeId)
        {
            string connectionString = _configuration.GetConnectionString("YourDatabaseConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("upcomingBookingsForuser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@employeeid", SqlDbType.Int) { Value = employeeId });

                    List<BookingDetails> bookings = new List<BookingDetails>();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BookingDetails booking = new BookingDetails
                            {
                                BookingId = reader["BookingId"] as int? ?? 0,
                                BookingDate = reader["BookingDate"] as DateTime? ?? DateTime.MinValue,
                                BookingTime = reader["BookingTime"] as string,
                                BookedRoom = reader["BookedRoom"] as string,
                                EmployeeId = reader["EmployeeId"] as int? ?? 0,
                                EmployeeName = reader["EmployeeName"] as string,
                                BookedWorkspace = reader["bookedWorkspace"] as string,
                                Status = reader["Status"] as string,

                            };



                            bookings.Add(booking);
                        }
                    }

                    return Ok(bookings);
                }
            }
        }
    }
}

