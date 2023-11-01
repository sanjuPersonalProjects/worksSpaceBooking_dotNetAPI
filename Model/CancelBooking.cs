namespace WorkSpaceBooking1.Model
{
    public class CancelBooking
    {
        public DateTime BookingDate { get; set; }
        public string BookingTime { get; set; }
        public string BookedRoom { get; set; }
        public string BookedWorkspace { get; set; }
        public int bookingId { get; set; }
    }
}

