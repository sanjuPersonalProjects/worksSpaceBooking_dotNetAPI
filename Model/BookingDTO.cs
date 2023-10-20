namespace WorkSpaceBooking1.Model
{
    public class BookingDTO
    {
        public DateTime BookingDate { get; set; }
        public string ? BookingTime { get; set; }
        public string ? BookedRoom { get; set; }
        public string ? BookedWorkspace { get; set; }
        public string ? Status { get; set; }
    }
}
