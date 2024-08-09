using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WorkSpaceBooking1.AdminModule.Models;

namespace WorkSpaceBooking1.AdminModule.Contollers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingDetailsWithFiltersForAdminController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public BookingDetailsWithFiltersForAdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookingDetailsWithFilters(DateTime startDate, DateTime endDate, string? roomFilter = null, string? workspaceFilter = null, int? employeeIdFilter = null, string? bookingTimeFilter = null, string? statusFilter = null, string? workspace = null)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("YourDatabaseConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("GetBookingDetailsByDateRangeAndFilters", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.Date) { Value = startDate });
                        command.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.Date) { Value = endDate });
                        command.Parameters.Add(new SqlParameter("@RoomFilter", SqlDbType.NVarChar, 50) { Value = roomFilter });
                        command.Parameters.Add(new SqlParameter("@WorkspaceFilter", SqlDbType.NVarChar, 50) { Value = workspaceFilter });
                        command.Parameters.Add(new SqlParameter("@EmployeeIdFilter", SqlDbType.Int) { Value = employeeIdFilter });
                        command.Parameters.Add(new SqlParameter("@BookingTimeFilter", SqlDbType.NVarChar, 50) { Value = bookingTimeFilter });
                        command.Parameters.Add(new SqlParameter("@StatusFilter", SqlDbType.NVarChar, 50) { Value = statusFilter });

                        await connection.OpenAsync();

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var bookings = new List<bookingDetailsWithFiltersForAdminDTO>();

                            while (reader.Read())
                            {
                                var booking = new bookingDetailsWithFiltersForAdminDTO
                                {

                                    BookingDate = reader.GetDateTime(1),
                                    BookingTime = reader.GetString(2),
                                    BookedRoom = reader.GetString(3),
                                    BookedWorkspace = reader.GetString(4),
                                    EmployeeId = reader.GetInt32(5),
                                    Status = reader.GetString(6)
                                };

                                bookings.Add(booking);
                            }

                            return Ok(bookings);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
