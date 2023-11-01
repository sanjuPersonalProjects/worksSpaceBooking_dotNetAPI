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
    public class FetchMessageForAdminController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public FetchMessageForAdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("dateRange")]
        public ActionResult<List<FetchMessagesForAdmin>> GetMessagesByDateRange(DateTime startDate, DateTime endDate)
        {
            List<FetchMessagesForAdmin > messages = new List<FetchMessagesForAdmin>();

            string connectionString = _configuration.GetConnectionString("YourDatabaseConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("GetMessagesByDateRange", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@StartDate", startDate));
                    command.Parameters.Add(new SqlParameter("@EndDate", endDate));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            FetchMessagesForAdmin message = new FetchMessagesForAdmin
                            {
                                MessageID = (int)reader["MessageID"],
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Subject = reader["Subject"].ToString(),
                                MessageText = reader["MessageText"].ToString(),
                                Timestamp = (DateTime)reader["Timestamp"],
                                IsRead = (bool)reader["IsRead"]
                            };
                            messages.Add(message);
                        }
                    }
                }
            }

            return Ok(messages);
        }
    }
}
