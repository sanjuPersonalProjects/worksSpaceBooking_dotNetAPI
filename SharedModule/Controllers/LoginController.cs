using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WorkSpaceBooking1.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace WorkSpaceBooking1.SharedModule.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class LoginController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("YourDatabaseConnection"); // Specify your connection string name
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var command = new SqlCommand("usp_UserLogin", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Email", request.Email);
                    command.Parameters.AddWithValue("@Password", request.Password);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string result = reader["Result"].ToString();
                            int userId = reader["UserID"] != DBNull.Value ? (int)reader["UserID"] : -1;
                            string fullName = reader["FullName"].ToString();
                            bool isAdmin = userId == int.Parse(_configuration["IsAdmin:AdminUserId"]);

                            if (result == "Login successful")
                            {
                                var claims = new List<Claim>
                                {
                                    new Claim("id", userId.ToString()),
                                    new Claim("name", fullName.Trim())
                                };

                                // Dynamic addition of claims based on conditions
                                if (isAdmin)
                                {
                                    claims.Add(new Claim("role", "Admin"));

                                }
                                else
                                {
                                    claims.Add(new Claim("role", "User"));

                                }

                                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
                                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                                var token = new JwtSecurityToken(
                                    issuer: _configuration["JwtSettings:Issuer"],
                                    audience: _configuration["JwtSettings:Audience"],
                                    claims: claims,
                                    expires: DateTime.UtcNow.AddMinutes(30),
                                    signingCredentials: creds
                                );

                                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                                return Ok(new { Token = tokenString });
                            }
                            else
                            {
                                return BadRequest("Authentication failed");
                            }
                        }
                        else
                        {
                            return NotFound("User not found");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
