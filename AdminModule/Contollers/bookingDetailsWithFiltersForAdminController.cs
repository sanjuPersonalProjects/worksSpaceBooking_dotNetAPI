using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WorkSpaceBooking1.AdminModule.Models;

namespace WorkSpaceBooking1.AdminModule.Controllers
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<bookingDetailsWithFiltersForAdminDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBookingDetailsWithFilters(
            DateTime startDate, DateTime endDate, string? roomFilter = null,
            string? workspaceFilter = null, int? employeeIdFilter = null,
            string? bookingTimeFilter = null, string? statusFilter = null)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("YourDatabaseConnection");
                string storedProcedure = _configuration["StoredProcedures:Admin:GetBookingHistoryForAdmin"];

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync(); // Asynchronous connection opening

                    using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters and handle nulls using DBNull.Value
                        command.Parameters.AddWithValue("@StartDate", startDate);
                        command.Parameters.AddWithValue("@EndDate", endDate);
                        command.Parameters.AddWithValue("@RoomFilter", (object?)roomFilter ?? DBNull.Value);
                        command.Parameters.AddWithValue("@WorkspaceFilter", (object?)workspaceFilter ?? DBNull.Value);
                        command.Parameters.AddWithValue("@EmployeeIdFilter", (object?)employeeIdFilter ?? DBNull.Value);
                        command.Parameters.AddWithValue("@BookingTimeFilter", (object?)bookingTimeFilter ?? DBNull.Value);
                        command.Parameters.AddWithValue("@StatusFilter", (object?)statusFilter ?? DBNull.Value);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var bookings = new List<bookingDetailsWithFiltersForAdminDTO>();

                            while (await reader.ReadAsync())
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
