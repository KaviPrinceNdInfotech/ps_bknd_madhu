using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class DoctorReports
    {
        public string PatientName { get; set; }
        public int Commission { get; set; }
        public int Doctor_Id { get; set;}
        public string DoctorName { get; set; }
        public double? Amount { get; set; }
        public string MobileNumber { get; set; }
        public string ClinicName { get; set; }
        public string LicenceNumber { get; set; }
        public double TotalAmount { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string AppointmentDate1 { get; set; }
        public int HospitalDoc_Id { get; set; }
        public string HospitalName { get; set; }
    }
}