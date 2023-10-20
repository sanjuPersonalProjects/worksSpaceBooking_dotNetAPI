using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using WorkSpaceBooking.Models;
using System.Collections.Generic;

using System.Configuration;
using WorkSpaceBooking1.Model;

Model = FetchEmployeeIdsDTO
namespace WorkSpaceBooking1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FetchEmployeeIdsAndNamesController : ControllerBase
    {



        public IEnumerable<Employee> Get()
        {
            List<Employee> employees = new List<Employee>();

            // Define your connection string
            string connectionString = ConfigurationManager.ConnectionStrings["YourConnectionString"].ToString();

            // Create a connection to the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Create a command to call the stored procedure
                using (SqlCommand command = new SqlCommand("GetEmployeeIdsAndNames", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employees.Add(new Employee
                            {
                                Id = (int)reader["Id"],
                                FullName = reader["FullName"].ToString()
                            });
                        }
                    }
                }
            }

            return employees;
        }
    }
}