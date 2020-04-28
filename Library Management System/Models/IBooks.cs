using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Management_System.Models
{
    public interface IBooks : IComparable<IBooks>
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string BookType { get; set; }
        public string Author { get; set; }
        public int Quantity { get; set; }
        public int QuantityOnHand { get; set; }
    }
}
