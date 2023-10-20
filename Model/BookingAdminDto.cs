namespace WorkSpaceBooking1.Model
{
    public class BookingAdminDto
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookedRoom { get; set; }
        public string BookedWorkspace { get; set; }
        public int EmployeeId { get; set; }
        public string PersonName { get; set; }
        public string Status { get; set; }
    }
}
