using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkSpaceBooking.Models
{
    public class BookingDetails
    {
        public int? BookingId { get; set; }
        public DateTime? BookingDate { get; set; }
        public string? BookingTime { get; set; }
        public string? BookedRoom { get; set; }
        public string BookedWorkspace { get; set; }
        public int? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
