using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WorkSpaceBooking.Models;

namespace WorkSpaceBooking.Controllers
{
    [ApiController]
    [Route("api/employees")]
    public class EmployeeController : Controller
    {
        private readonly string _connectionString;

        public EmployeeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("YourDatabaseConnection"); // Replace with your actual connection string key
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(Employee employee)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ManageEmployee", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parameters for the stored procedure
                        command.Parameters.AddWithValue("@Operation", "CREATE");
                        command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                        command.Parameters.AddWithValue("@LastName", employee.LastName);
                        command.Parameters.AddWithValue("@Gender", employee.Gender);
                        command.Parameters.AddWithValue("@Dob", employee.Dob);
                        command.Parameters.AddWithValue("@Address", employee.Address);
                        command.Parameters.AddWithValue("@Pincode", employee.Pincode);
                        command.Parameters.AddWithValue("@Aadhar", employee.Aadhar);
                        command.Parameters.AddWithValue("@Department", employee.Department);
                        command.Parameters.AddWithValue("@Position", employee.Position);
                        command.Parameters.AddWithValue("@Email", employee.Email);
                        command.Parameters.AddWithValue("@Password", employee.Password);
                        command.Parameters.AddWithValue("@Phone", employee.Phone);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                await reader.ReadAsync();
                                // Read the result from the database if needed
                            }
                        }
                    }
                }


                return Ok(new { success = true, message = "Employee created successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating employee: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ManageEmployee", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parameters for the stored procedure
                        command.Parameters.AddWithValue("@Operation", "READ");
                        command.Parameters.AddWithValue("@Id", id);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                await reader.ReadAsync();

                                // Read the result from the database and map it to an Employee object
                                Employee employee = new Employee
                                {
                                    Id = (int)reader["Id"],
                                    FirstName = reader["FirstName"].ToString().Trim(),
                                    LastName = reader["LastName"].ToString().Trim(),
                                    Gender = reader["Gender"].ToString().Trim(),
                                    Dob = (DateTime)reader["Dob"],
                                    Address = reader["Address"].ToString().Trim(),
                                    Pincode = (int)reader["Pincode"],
                                    Aadhar = reader["Aadhar"].ToString(),
                                    Department = reader["Department"].ToString().Trim(),
                                    Position = reader["Position"].ToString().Trim(),
                                    Email = reader["Email"].ToString().Trim(),
                                    Password = reader["Password"].ToString().Trim(),
                                    Phone = reader["Phone"].ToString()
                                };

                                // Return the employee data
                                return Ok(employee);
                            }
                            else
                            {
                                return NotFound("Employee not found");
                            }
                        }
                    }
                }

                return Ok(/* Return the Employee object */);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error getting employee: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ManageEmployee", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parameters for the stored procedure
                        command.Parameters.AddWithValue("@Operation", "UPDATE");
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                        command.Parameters.AddWithValue("@LastName", employee.LastName);
                        command.Parameters.AddWithValue("@Gender", employee.Gender);
                        command.Parameters.AddWithValue("@Dob", employee.Dob);
                        command.Parameters.AddWithValue("@Address", employee.Address);
                        command.Parameters.AddWithValue("@Pincode", employee.Pincode);
                        command.Parameters.AddWithValue("@Aadhar", employee.Aadhar);
                        command.Parameters.AddWithValue("@Department", employee.Department);
                        command.Parameters.AddWithValue("@Position", employee.Position);
                        command.Parameters.AddWithValue("@Email", employee.Email);
                        command.Parameters.AddWithValue("@Password", employee.Password);
                        command.Parameters.AddWithValue("@Phone", employee.Phone);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok("Employee updated successfully!");
                        }
                        else
                        {
                            return NotFound("Employee not found");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating employee: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ManageEmployee", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parameters for the stored procedure
                        command.Parameters.AddWithValue("@Operation", "DELETE");
                        command.Parameters.AddWithValue("@Id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok("Employee deleted successfully!");
                        }
                        else
                        {
                            return NotFound("Employee not found");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting employee: {ex.Message}");
            }
        }

    }
}








