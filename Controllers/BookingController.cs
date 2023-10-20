using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using WorkSpaceBooking1.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace WorkSpaceBooking1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public BookingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetBookingData( int? employeeId , string? filterRoom , string? filterWorkspace , DateTime targetDate)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("YourDatabaseConnection")))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand("GetBookingData", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        cmd.Parameters.AddWithValue("@targetDate", targetDate);
                        cmd.Parameters.AddWithValue("@employeeId", employeeId);

                        // Check if filterRoom is provided and add the parameter if it's not null or empty
                        if (!string.IsNullOrEmpty(filterRoom))
                        {
                            cmd.Parameters.AddWithValue("@filterRoom", filterRoom);
                        }

                        // Check if filterWorkspace is provided and add the parameter if it's not null or empty
                        if (!string.IsNullOrEmpty(filterWorkspace))
                        {
                            cmd.Parameters.AddWithValue("@filterWorkspace", filterWorkspace);
                        }

                        List<BookingDTO> bookings = new List<BookingDTO>();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                BookingDTO booking = new BookingDTO
                                {
                                    BookingDate = reader.GetDateTime(0),
                                    BookingTime = reader.GetString(1),
                                    BookedRoom = reader.GetString(2),
                                    BookedWorkspace = reader.GetString(3),
                                    Status = reader.GetString(4)
                                };
                                bookings.Add(booking);
                            }
                        }

                        return Ok(bookings);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
