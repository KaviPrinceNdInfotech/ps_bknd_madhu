using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class DoctorSearchResult
    {
        public string Skills { get; set; }
        //public int Id { get; set; }
        public int DoctorId { get; set; }
        //public int HospitalId { get; set; }
        public string ClinicName { get; set; }
        public string HospitalName { get; set; }
        public string DoctorName { get; set; }
        public string EmailId { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string DepartmentName { get; set; }
        public string SpecialistName { get; set; }
        //public string Skills { get; set; }
        public TimeSpan? Availability1 { get; set; }
        public TimeSpan? Availability2 { get; set; }
        public double? Fee { get; set; }
        public string Location { get; set; }
    }
}