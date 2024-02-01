using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class VendorCommissionReport
    {
        public double? Amount { get; set; }
        public int Id { get; set; }
        public string VendorName { get; set; } 
        public string CompanyName { get; set; }
        public double Counts { get; set; }
        public double Commission { get; set; }

    }
}