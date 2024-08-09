namespace WorkSpaceBooking1.AdminModule.Models
{
    public class bookingDetailsWithFiltersForAdminDTO
    {

        public DateTime BookingDate { get; set; }
        public string? BookingTime { get; set; }
        public string? BookedRoom { get; set; }
        public string? BookedWorkspace { get; set; }
        public int? EmployeeId { get; set; }
        public string? Status { get; set; }
    }
}
