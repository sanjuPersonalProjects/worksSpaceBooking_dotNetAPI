namespace WorkSpaceBooking1.DTOs.charts
{
    public class BookingGenderFilters
    {
    public DateOnly StartDate {  get; set; } 
    public DateOnly EndDate {  get; set; }
    public string RoomFilter {  get; set; }
    public int WorkspaceFilter {  get; set; }
    public string bookingTimeFilter { get; set; }

    }
}
