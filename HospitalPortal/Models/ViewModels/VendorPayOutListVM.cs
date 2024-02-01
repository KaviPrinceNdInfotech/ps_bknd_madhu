using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class VendorPayOutListVM
    {
        public IEnumerable<VendorPayoutHistory> VendorList { get; set; }
    }

    public class VendorPayoutHistory
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public string VendorName { get; set; }
        public string UniqueId { get; set; }
        public string CompanyName { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string AadharOrPANNumber { get; set; }
        public bool IsPaid { get; set; }
    }
}