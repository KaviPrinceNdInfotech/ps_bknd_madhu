using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class VendorCommissionReport
    {
        public double? Amount { get; set; }
        public double? FraPaidableamt { get; set; }
        public double? Amountwithrazorpaycomm { get; set; }
        public int Id { get; set; }
        public string VendorName { get; set; } 
        public string CompanyName { get; set; }
        public string DoctorName { get; set; }
        public DateTime PaymentDate { get; set; }
        public string DoctorId { get; set; }
        public int Counts { get; set; }
        public double Commission { get; set; }
        

    }
}