using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Management_System.Models
{
    public interface IFines : IComparable<IFines>
    {
        public int FineId { get; set; }
        public int IssueId { get; set; }
        public float FineAmount { get; set; }
        public float RemainingFineAmount { get; set; }
    }
}
