using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WorkSpaceBooking1.Model;

namespace WorkSpaceBooking1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingAdminController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public BookingAdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<BookingAdminDto> GetBookings(DateTime startDate, DateTime endDate, string? roomFilter = null, string? workspaceFilter = null, int? employeeIdFilter = null, string? personNameFilter = null, string? statusFilter = null)
        {
            // Construct the connection string using the configuration
            string connectionString = _configuration.GetConnectionString("YourDatabaseConnection");

            // Use a list to store the results
            var bookings = new List<BookingAdminDto>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("GetBookingDetailsByDateRangeAndFilters", connection))
                    {
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@StartDate", startDate);
                            command.Parameters.AddWithValue("@EndDate", endDate);
                            command.Parameters.AddWithValue("@RoomFilter", roomFilter ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@WorkspaceFilter", workspaceFilter ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@EmployeeIdFilter", employeeIdFilter ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@PersonNameFilter", personNameFilter ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@StatusFilter", statusFilter ?? (object)DBNull.Value);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var booking = new BookingAdminDto
                                    {
                                        BookingId = reader.GetInt32(reader.GetOrdinal("bookingId")),
                                        BookingDate = reader.GetDateTime(reader.GetOrdinal("bookingDate")),
                                        BookedRoom = reader.GetString(reader.GetOrdinal("bookedRoom")),
                                        BookedWorkspace = reader.GetString(reader.GetOrdinal("bookedWorkspace")),
                                        EmployeeId = reader.GetInt32(reader.GetOrdinal("employeeId")),
                                        PersonName = reader.GetString(reader.GetOrdinal("personName")),
                                        Status = reader.GetString(reader.GetOrdinal("status"))
                                    };

                                    bookings.Add(booking);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // Handle any exceptions or log them as needed
                // You can return an error response here if necessary
                // Example: return BadRequest("An error occurred: " + ex.Message);
            }

            return bookings;
        }
    }
}
