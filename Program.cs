using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using WorkSpaceBooking1.AdminModule.Repository;
using WorkSpaceBooking1.AdminModule.Services;
using WorkSpaceBooking1.Model;
using WorkSpaceBooking1.SharedModule.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Data;
using WorkSpaceBooking1.Middlewares;
using Serilog.Formatting.Compact;
using static Serilog.Sinks.MSSqlServer.ColumnOptions;
using System.Collections.ObjectModel;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Access configuration values
var configuration = builder.Configuration;

// In Startup.cs or Program.cs
builder.Services.AddScoped<IChartsService, ChartService>();
builder.Services.AddScoped<IChartsRepository, ChartsRepository>();


// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["JwtSettings:Issuer"],
            ValidAudience = configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]))
        };
    });

// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserPolicy", policy =>
        policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "User"));
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Admin"));
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
});

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

    

builder.Services.AddTransient<BookingController>();
builder.Services.AddTransient<BookingDTO>();

var columnOptions = new ColumnOptions();
//  don't need XML data
columnOptions.Store.Remove(StandardColumn.Properties);

//  do want JSON data and OpenTelemetry
columnOptions.Store.Add(StandardColumn.LogEvent);
// Configure Serilog (moved up)
try
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug().WriteTo.Console(new CompactJsonFormatter()) // Changed to Debug to capture more logs
        .WriteTo.MSSqlServer(
            connectionString: builder.Configuration.GetConnectionString("SqlServerConnection"),
            sinkOptions: new MSSqlServerSinkOptions
            {
                TableName = "Logs",
                AutoCreateSqlTable = false,
                
            },
            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
            logEventFormatter: new CompactJsonFormatter(),
            columnOptions: columnOptions


        )
        .CreateLogger();

    builder.Host.UseSerilog(); // Moved up
}
catch (Exception ex)
{
    Console.WriteLine($"Error configuring Serilog: {ex}");
    // Consider throwing the exception if you want to prevent the application from starting
}

builder.Host.UseSerilog(); // Use Serilog for logging

// Add services to the container
builder.Services.AddControllers();


var app = builder.Build();
Log.Information("Application starting up");




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware for logging
app.UseMiddleware<LoggingMiddleware>();
app.UseCors("AllowAll");
app.UseHttpsRedirection();

// Configure authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
