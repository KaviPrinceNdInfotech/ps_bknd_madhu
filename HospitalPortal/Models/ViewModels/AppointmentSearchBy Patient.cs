using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class AppointmentSearchBy_Patient
    {
        public int Doctor_Id { get; set; }
        public string DoctorName { get; set; }
        public string ClinicName { get; set; }
        public string ClinicAddress { get; set; }
        public string PhoneNo { get; set; }
        public string MobileNo { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string Specility { get; set; }
        public string AppointedTime { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public double Amount { get; set; }
        public int? AppointmentId { get; set; }
        public int? Specialist_Id { get; set; }
        public bool IsCancelled { get; set; }
        public TimeSpan? StartTime { get; set; }
        //public TimeSpans? AppointmentStartTime { get; set; }
    }
}