using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Management_System.Models
{
    public interface IIssuedBooks : IComparable<IIssuedBooks>
    {
        public int IssuedId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime ReturnedDate { get; set; }
        public bool IsFined { get; set; }
    }
}
