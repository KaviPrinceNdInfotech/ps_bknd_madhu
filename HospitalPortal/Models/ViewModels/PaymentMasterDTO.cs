using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class PaymentMasterDTO
    {
        public int Id { get; set; }
        public string Department { get; set; }
        public string Name { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        
        public IEnumerable<paymentList> paymentList { get; set; }

    }

    public class paymentList
    {
        public int Id { get; set; }
        public string Department { get; set; }
        public string Name { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }

}