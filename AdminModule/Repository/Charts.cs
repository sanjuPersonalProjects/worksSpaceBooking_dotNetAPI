using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WorkSpaceBooking1.AdminModule.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WorkSpaceBooking1.AdminModule.Repository
{
    public interface IChartsRepository
    {
        Task<BookingByGenderDto> GetBookingsByGenderChartAsync(DateOnly date);
    }

    public class ChartsRepository : IChartsRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<ChartsRepository> _logger;

        public ChartsRepository(IConfiguration configuration, ILogger<ChartsRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("YourDatabaseConnection");
            _logger = logger;
        }

        public async Task<BookingByGenderDto> GetBookingsByGenderChartAsync(DateOnly date)
        {
            var result = new BookingByGenderDto();

            try
            {
                _logger.LogInformation("Getting bookings by gender chart for date: {Date}", date);

                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand("usp_GetBookingsByGenderGraph", connection))
                    {
                        var dateTime = date.ToDateTime(TimeOnly.MinValue);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Date", dateTime);

                        await connection.OpenAsync();
                        _logger.LogInformation("Database connection opened successfully.");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                result.TotalBookings = reader.IsDBNull(reader.GetOrdinal("TotalBookings")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalBookings"));
                                result.TotalWomenBookings = reader.IsDBNull(reader.GetOrdinal("TotalWomenBookings")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalWomenBookings"));
                                result.TotalMenBookings = reader.IsDBNull(reader.GetOrdinal("TotalMenBookings")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalMenBookings"));
                                result.TotalOtherBookings = reader.IsDBNull(reader.GetOrdinal("TotalOtherBookings")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalOtherBookings"));
                                result.TotalWomen = reader.IsDBNull(reader.GetOrdinal("TotalWomen")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalWomen"));
                                result.TotalMen = reader.IsDBNull(reader.GetOrdinal("TotalMen")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalMen"));
                            }
                        }
                    }
                }

                _logger.LogInformation("Successfully retrieved bookings by gender chart for date: {Date}", date);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving bookings by gender chart for date: {Date}", date);
                throw;
            }

            return result;
        }
    }
}
