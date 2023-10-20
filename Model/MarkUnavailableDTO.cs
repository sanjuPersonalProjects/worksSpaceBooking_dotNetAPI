namespace WorkSpaceBooking1.Model
{
    public class MarkUnavailableDTO
    {
        public string BookedRoom { get; set; }
        public string BookingDate { get; set; }
        public string BookingTime { get; set; }
        public List<int> MarkUnavailable { get; set; }
    }

}
