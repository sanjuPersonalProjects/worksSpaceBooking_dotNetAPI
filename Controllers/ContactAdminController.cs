using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WorkSpaceBooking1.Model;

namespace WorkSpaceBooking1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactAdminController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ContactAdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult PostContactMessage(ContactMessage message)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("YourDatabaseConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("InsertContactMessage", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    //command.Parameters.AddWithValue("@Name", message.Name);
                    //command.Parameters.AddWithValue("@Email", message.Email);
                    command.Parameters.AddWithValue("@Subject", message.Subject);
                    command.Parameters.AddWithValue("@MessageText", message.MessageText);
                    command.Parameters.AddWithValue("@EmployeeId", message.EmployeeId);

                    command.ExecuteNonQuery();

                    return Ok(new { message = "Message sent successfully" });

                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions or errors, and return an appropriate response.
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
