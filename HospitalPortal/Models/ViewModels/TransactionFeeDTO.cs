using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class TransactionFeeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Nullable<double> Fee { get; set; }
        public IEnumerable<TransactionList> TransactionList { get; set; }
    }
    public class TransactionList
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public Nullable<double> Fee { get; set; }
    }
}