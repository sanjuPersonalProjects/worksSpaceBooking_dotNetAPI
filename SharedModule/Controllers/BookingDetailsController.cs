using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WorkSpaceBooking.Models;


namespace WorkSpaceBooking1.SharedModule.Controllers
{
    [ApiController]
    [Route("api/bookingdetails")]
    public class BookingDetailsController : ControllerBase
    {
        private readonly string _connectionString;

        public BookingDetailsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("YourDatabaseConnection"); // Replace with your actual connection string key
        }

        [HttpPost]
        public async Task<IActionResult> CreateBookingDetails(BookingDetails bookingDetails)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ManageBookingDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Operation", "CREATE");
                        command.Parameters.AddWithValue("@BookingDate", bookingDetails.BookingDate);
                        command.Parameters.AddWithValue("@BookingTime", bookingDetails.BookingTime);
                        command.Parameters.AddWithValue("@BookedRoom", bookingDetails.BookedRoom);
                        command.Parameters.AddWithValue("@BookedWorkspace", bookingDetails.BookedWorkspace);
                        command.Parameters.AddWithValue("@EmployeeId", bookingDetails.EmployeeId);
                        command.Parameters.AddWithValue("@EmployeeName", bookingDetails.EmployeeName);
                        command.Parameters.AddWithValue("@Status", bookingDetails.Status);

                        // Execute the command to interact with the database
                        command.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Booking details created successfully." });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating booking details: {ex.Message}");
            }
        }


        // Add other CRUD actions here (HttpGet, HttpPut, HttpDelete) similarly...

        // Example GetBookingDetails action:
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingDetails(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ManageBookingDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Operation", "READ");
                        command.Parameters.AddWithValue("@BookingId", id);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                await reader.ReadAsync();
                                // Read the result from the database and map it to a BookingDetails object
                                var retrievedBookingDetails = new BookingDetails
                                {
                                    BookingId = reader.GetInt32(reader.GetOrdinal("BookingId")),
                                    BookingDate = reader.GetDateTime(reader.GetOrdinal("BookingDate")),
                                    BookingTime = reader.GetString(reader.GetOrdinal("BookingTime")),
                                    BookedRoom = reader.GetString(reader.GetOrdinal("BookedRoom")),
                                    BookedWorkspace = reader.GetString(reader.GetOrdinal("BookedWorkspace")),
                                    EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                    EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName")),
                                    Status = reader.GetString(reader.GetOrdinal("Status"))
                                };
                                return Ok(retrievedBookingDetails);
                            }
                            else
                            {
                                return NotFound("Booking details not found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error getting booking details: {ex.Message}");
            }
        }
        [HttpGet("get-by-employee-and-status/{id}")]
        public async Task<IActionResult> GetBookingDetailsByEmpIdAndStatus(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ManageBookingDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Operation", "READ_BY_EMPLOYEE_AND_STATUS");
                        command.Parameters.AddWithValue("@EmployeeId", id);
                        command.Parameters.AddWithValue("@Status", "Booked");

                        List<BookingDetails> bookingDetailsList = new List<BookingDetails>();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var bookingDetails = new BookingDetails
                                {
                                    BookingId = reader.GetInt32(reader.GetOrdinal("BookingId")),
                                    BookingDate = reader.GetDateTime(reader.GetOrdinal("BookingDate")),
                                    BookingTime = reader.GetString(reader.GetOrdinal("BookingTime")),
                                    BookedRoom = reader.GetString(reader.GetOrdinal("BookedRoom")),
                                    BookedWorkspace = reader.GetString(reader.GetOrdinal("BookedWorkspace")),
                                    EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                    EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName")),
                                    Status = reader.GetString(reader.GetOrdinal("Status"))
                                };
                                bookingDetailsList.Add(bookingDetails);
                            }
                        }

                        if (bookingDetailsList.Count > 0)
                        {
                            return Ok(bookingDetailsList);
                        }
                        else
                        {
                            return NotFound("Booking details not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error getting booking details: {ex.Message}");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllBookingDetails()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ManageBookingDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Operation", "READ_ALL"); // Use a different operation to retrieve all rows

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            var bookingDetailsList = new List<BookingDetails>();

                            while (reader.Read())
                            {
                                var bookingDetails = new BookingDetails
                                {
                                    BookingId = reader.GetInt32(reader.GetOrdinal("BookingId")),
                                    BookingDate = reader.GetDateTime(reader.GetOrdinal("BookingDate")),
                                    BookingTime = reader.GetString(reader.GetOrdinal("BookingTime")),
                                    BookedRoom = reader.GetString(reader.GetOrdinal("BookedRoom")),
                                    BookedWorkspace = reader.GetString(reader.GetOrdinal("BookedWorkspace")),
                                    EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                    EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName")),
                                    Status = reader.GetString(reader.GetOrdinal("Status"))
                                };

                                bookingDetailsList.Add(bookingDetails);
                            }

                            return Ok(bookingDetailsList);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error getting booking details: {ex.Message}");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBookingDetails(int id, BookingDetails bookingDetails)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ManageBookingDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Operation", "UPDATE");
                        command.Parameters.AddWithValue("@BookingId", id);
                        command.Parameters.AddWithValue("@BookingDate", bookingDetails.BookingDate);
                        command.Parameters.AddWithValue("@BookingTime", bookingDetails.BookingTime);
                        command.Parameters.AddWithValue("@BookedRoom", bookingDetails.BookedRoom);
                        command.Parameters.AddWithValue("@BookedWorkspace", bookingDetails.BookedWorkspace);
                        command.Parameters.AddWithValue("@EmployeeId", bookingDetails.EmployeeId);
                        command.Parameters.AddWithValue("EmployeeName", bookingDetails.EmployeeName);
                        command.Parameters.AddWithValue("@Status", bookingDetails.Status);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                await reader.ReadAsync();
                                // Read the result from the database and map it to a BookingDetails object
                                var updatedBookingDetails = new BookingDetails
                                {
                                    BookingId = reader.GetInt32(reader.GetOrdinal("BookingId")),
                                    BookingDate = reader.GetDateTime(reader.GetOrdinal("BookingDate")),
                                    BookingTime = reader.GetString(reader.GetOrdinal("BookingTime")),
                                    BookedRoom = reader.GetString(reader.GetOrdinal("BookedRoom")),
                                    BookedWorkspace = reader.GetString(reader.GetOrdinal("BookedWorkspace")),
                                    EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                    EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName")),
                                    Status = reader.GetString(reader.GetOrdinal("Status"))
                                };
                                return Ok(updatedBookingDetails);
                            }
                            else
                            {
                                return NotFound("Booking details not found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating booking details: {ex.Message}");
            }
        }
        //[HttpGet]
        //public ActionResult<IEnumerable<Employee>> Get()
        //{
        //    // Replace this with your ADO.NET code to fetch employee IDs
        //    List<Employee> employees = FetchEmployeeIds();

        //    if (employees == null)
        //    {
        //        return NotFound();
        //    }

        //    return employees;
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteBookingDetails(int id)
        //{
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(_connectionString))
        //        {
        //            await connection.OpenAsync();

        //            using (SqlCommand command = new SqlCommand("ManageBookingDetails", connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.Parameters.AddWithValue("@Operation", "DELETE");
        //                command.Parameters.AddWithValue("@BookingId", id);

        //                // Execute the DELETE operation
        //                int rowsAffected = await command.ExecuteNonQueryAsync();

        //                if (rowsAffected > 0)
        //                {
        //                    return Ok("Booking details deleted successfully!");
        //                }
        //                else
        //                {
        //                    return NotFound("Booking details not found.");
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error deleting booking details: {ex.Message}");
        //    }
        //}

    }

}
