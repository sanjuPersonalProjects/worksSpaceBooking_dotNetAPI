using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkSpaceBooking.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime Dob { get; set; }
        public string Address { get; set; }
        public int Pincode { get; set; }
        public string Aadhar { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        //public string? Phone { get; set; }
    }
}
