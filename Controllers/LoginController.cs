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
                var command = new SqlCommand("SELECT * FROM Users WHERE Email = @Email", connection);
                command.Parameters.AddWithValue("@Email", request.Email);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var storedPasswordHash = reader["PasswordHash"].ToString();

                        // You should have a method to verify the password here, e.g., using a library like BCrypt.
                        // For simplicity, we're comparing the password directly in this example.
                        if (storedPasswordHash == request.Password)
                        {
                            // Generate a JWT token and return it upon successful login.
                            var claims = new[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, reader["Id"].ToString()), // Use a unique identifier as NameIdentifier
                                new Claim(ClaimTypes.Name, reader["Username"].ToString()), // Use the user's name or email as Name
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
                    }
                }
            }

            return BadRequest("Invalid email or password");
        }
    }
}
