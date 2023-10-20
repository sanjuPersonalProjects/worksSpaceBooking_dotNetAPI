using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WorkSpaceBooking1.Model;

namespace WorkSpaceBooking1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminActionsController : ControllerBase
    {
        private readonly string _connectionString;

        public AdminActionsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("YourDatabaseConnection");
            // Replace "YourDatabaseConnection" with the actual name of your connection string in appsettings.json.
        }

        [HttpPost("MarkUnavailable")]
        public async Task<IActionResult> MarkUnavailable([FromBody] MarkUnavailableDTO request)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Convert the list of workspace numbers to a delimited string
                    string workspaceNumbers = string.Join(",", request.MarkUnavailable);

                    // Call the stored procedure with the delimited string
                    using (var command = new SqlCommand("MarkWorkspacesAsUnavailable", connection)) // Updated to call the new stored procedure
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@room", request.BookedRoom));
                        command.Parameters.Add(new SqlParameter("@date", request.BookingDate));
                        command.Parameters.Add(new SqlParameter("@time", request.BookingTime));
                        command.Parameters.Add(new SqlParameter("@employeeId", 25)); // Default employee ID
                        command.Parameters.Add(new SqlParameter("@employeeName", "ADMIN")); // Default employee name
                        command.Parameters.Add(new SqlParameter("@WorkspaceNumbers", workspaceNumbers));

                        await command.ExecuteNonQueryAsync();
                    }

                    return Ok("Workspaces marked as unavailable.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

    }
}
