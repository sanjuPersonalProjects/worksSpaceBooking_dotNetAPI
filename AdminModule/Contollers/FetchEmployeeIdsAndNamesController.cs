using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WorkSpaceBooking1.AdminModule.Models;

namespace WorkSpaceBooking1.AdminModule.Contollers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FetchEmployeeIdsAndNamesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public FetchEmployeeIdsAndNamesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<FetchEmployeeIdsDTO> Get()
        {
            List<FetchEmployeeIdsDTO> employees = new List<FetchEmployeeIdsDTO>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("YourDatabaseConnection")))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("GetEmployeeIds", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employees.Add(new FetchEmployeeIdsDTO
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
