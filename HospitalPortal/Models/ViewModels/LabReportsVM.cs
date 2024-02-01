using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class LabReportsVM
    {
        public int Lab_Id { get; set; }
        public string LabName { get; set; }
        public double Amount { get; set; }
        public string MobileNumber { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string LicenceNumber { get; set; }
        public double TotalAmount { get; set; }
        public DateTime? TestDate { get; set; }
        public string TestDate1 { get; set; }
        public string Location { get; set; }
    }
}