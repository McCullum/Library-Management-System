using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Management_System.Models
{
    public interface IUsers : IComparable<IUsers>
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public string ContactNumber { get; set; }
        public string Password { get; set; }
        public char Role { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsDeleted { get; set; }
    }
}
