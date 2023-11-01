using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using WorkSpaceBooking.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using WorkSpaceBooking1.Model;

namespace WorkSpaceBooking1.Controllers
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
