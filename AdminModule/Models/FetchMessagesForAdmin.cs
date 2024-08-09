namespace WorkSpaceBooking1.AdminModule.Models
{
    public class FetchMessagesForAdmin
    {

        public int MessageID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Subject { get; set; }
        public string MessageText { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
    }
}

