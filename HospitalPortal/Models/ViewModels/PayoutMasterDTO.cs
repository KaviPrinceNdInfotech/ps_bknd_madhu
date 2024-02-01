using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class PayoutMasterDTO
    {
        public IEnumerable<PayoutList> payoutList { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }


    public class PayoutList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}