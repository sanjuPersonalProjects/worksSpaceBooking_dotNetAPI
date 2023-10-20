using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorkSpaceBooking1.Controllers;
using WorkSpaceBooking1.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});
builder.Services.AddTransient<BookingController>();
builder.Services.AddTransient<BookingDTO>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

// Configure authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

// Inside ConfigureServices method
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Validate the server that created the token
            ValidateAudience = true, // Ensure the audience is valid
            ValidateLifetime = true, // Check if the token is expired
            ValidateIssuerSigningKey = true, // Validate the security key
            ValidIssuer = "YourIssuer", // Set the server's issuer
            ValidAudience = "YourAudience", // Set the audience
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKey")) // Set your secret key
        };
    });

// Inside your authentication logic

