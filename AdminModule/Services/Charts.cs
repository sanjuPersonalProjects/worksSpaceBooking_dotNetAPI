using WorkSpaceBooking1.AdminModule.Dtos;
using WorkSpaceBooking1.AdminModule.Repository;

namespace WorkSpaceBooking1.AdminModule.Services
{
    public interface IChartsService
    {
        Task<BookingByGenderDto> GetBookingsByGenderChartAsync(DateOnly date);
    }

    public class ChartService : IChartsService
    {
        private readonly IChartsRepository _chartsRepository;

        public ChartService(IChartsRepository chartsRepository)
        {
            _chartsRepository = chartsRepository;
        }

        public async Task<BookingByGenderDto> GetBookingsByGenderChartAsync(DateOnly date)
        {
            return await _chartsRepository.GetBookingsByGenderChartAsync(date);
        }
    }

}
