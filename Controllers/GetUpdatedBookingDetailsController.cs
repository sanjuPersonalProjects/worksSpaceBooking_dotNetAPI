using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WorkSpaceBooking.Models;
// Replace YourNamespace with the actual namespace for your model


namespace WorkSpaceBooking1.Controllers // Replace YourNamespace with your actual namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetUpdatedBookingDetailsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public GetUpdatedBookingDetailsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetBookingDetails(DateTime? startDate, DateTime? endDate, string? BookingTime, string? BookedRoom, string? Status, string? BookedWorkspace, int? EmployeeId = null)
        {
            string connectionString = _configuration.GetConnectionString("YourDatabaseConnection"); // Retrieve connection string from appsettings.json

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("GetUpdatedBookingDetails", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    cmd.Parameters.Add(new SqlParameter("@startDate", SqlDbType.Date)).Value = startDate ?? (object)DBNull.Value;
                    cmd.Parameters.Add(new SqlParameter("@endDate", SqlDbType.Date)).Value = endDate ?? (object)DBNull.Value;
                    cmd.Parameters.Add(new SqlParameter("@bookingTime", SqlDbType.NVarChar, 10)).Value = string.IsNullOrWhiteSpace(BookingTime) ? (object)DBNull.Value : (object)BookingTime;
                    cmd.Parameters.Add(new SqlParameter("@bookedRoom", SqlDbType.NVarChar, 2)).Value = string.IsNullOrWhiteSpace(BookedRoom) ? (object)DBNull.Value : (object)BookedRoom;
                    cmd.Parameters.Add(new SqlParameter("@employeeId", SqlDbType.Int)).Value = EmployeeId ?? (object)DBNull.Value;
                    cmd.Parameters.Add(new SqlParameter("@status", SqlDbType.NVarChar, 20)).Value = Status; // Replace with the desired status

                    List<WorkSpaceBooking.Models.BookingDetails> bookingDetails = new List<BookingDetails>();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var bookingDetail = new BookingDetails
                            {
                                BookingId = reader["BookingId"] as int? ?? 0,
                                BookingDate = reader["BookingDate"] as DateTime? ?? DateTime.MinValue,
                                BookingTime = reader["BookingTime"] as string,
                                BookedRoom = reader["BookedRoom"] as string,
                                EmployeeId = reader["EmployeeId"] as int? ?? 0,
                                EmployeeName = reader["EmployeeName"] as string,
                                BookedWorkspace = reader["bookedWorkspace"] as string,
                                Status = reader["Status"] as string,
                                Timestamp = reader["Timestamp"] as DateTime? ?? DateTime.MinValue
                            };
                            bookingDetails.Add(bookingDetail);
                        }
                    }

                    return Ok(bookingDetails);
                }
            }
        }
    }
}
