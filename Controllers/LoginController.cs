using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorkSpaceBooking1.Model; // Make sure this namespace matches your model's namespace

namespace WorkSpaceBooking1.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class LoginController : ControllerBase
    {
        private readonly string _connectionString; // Your database connection string

        public LoginController(string connectionString)
        {
            _connectionString = connectionString;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("usp_UserLogin", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Email", request.Email);
                command.Parameters.AddWithValue("@Password", request.Password);

                var result = command.ExecuteScalar() as string; // The stored procedure should return a result indicating success or failure

                if (result != null)
                {
                    if (result == "Login successful")
                    {
                        // Authentication was successful
                        // Generate a JWT token and return it upon successful login.
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, request.Email), // Use a unique identifier as NameIdentifier
                            new Claim(ClaimTypes.Name, request.Email), // Use the user's email as Name
                            // Add other claims as needed
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKey"));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            issuer: "YourIssuer",
                            audience: "YourAudience",
                            claims: claims,
                            expires: DateTime.UtcNow.AddMinutes(30), // Token expiration time
                            signingCredentials: creds
                        );

                        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                        return Ok(new { Token = tokenString });
                    }
                    else
                    {
                        // Authentication failed
                        return BadRequest("Authentication failed");
                    }
                }
                else
                {
                    // User with the provided email was not found
                    return NotFound("User not found");
                }
            }
        }
    }
}
