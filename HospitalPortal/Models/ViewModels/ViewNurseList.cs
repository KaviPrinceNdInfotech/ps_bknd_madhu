using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class ViewNurseList
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string NurseId { get; set; }
        public string NurseName { get; set; }
        public string NurseTypeName {get;set;}
        public string StateName { get; set; }
        public string Location { get; set; }
        public string CityName { get; set; }
        public string MobileNumber { get; set; }
        public string CertificateNumber { get; set; }
        public string ServiceAcceptanceDate { get; set; }
        public string RequestDate { get; set; }
        public int? TotalDays { get; set; }
        
        public double? TotalFee { get; set; }
    }
}