using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WorkSpaceBooking1.AdminModule.Dtos;

namespace WorkSpaceBooking1.AdminModule.Repository
{
    public interface IChartsRepository
    {
        Task<BookingByGenderDto> GetBookingsByGenderChartAsync(DateOnly date);
    }

    public class ChartsRepository : IChartsRepository
    {
        private readonly string _connectionString;

        

        public ChartsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("YourDatabaseConnection");
        }

        public async Task<BookingByGenderDto> GetBookingsByGenderChartAsync(DateOnly date)
        {
            var result = new BookingByGenderDto();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand("usp_GetBookingsByGenderGraph", connection))
                    {
                        var dateTime = date.ToDateTime(TimeOnly.MinValue);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Date", dateTime);

                        await connection.OpenAsync();

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
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., to a file or monitoring system)
                // For example:
                Console.WriteLine($"An error occurred: {ex.Message}");

                // Consider rethrowing or handling the exception as needed
                throw;
            }

            return result;
        }
    }
}
