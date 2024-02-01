using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class LabCommissionReport
    {
        public double Commission { get; set; }
        public int Lab_Id { get; set; }
        public string LabName { get; set; }
        public double Amount { get; set; }
        public string MobileNumber { get; set; }
        public string ClinicName { get; set; }
        public string LicenceNumber { get; set; }
        public double TotalAmount { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string AppointmentDate1 { get; set; }
        public int HospitalDoc_Id { get; set; }
        public int HospitalName { get; set; }
    }
}