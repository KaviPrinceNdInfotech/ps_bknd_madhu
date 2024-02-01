using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class DoctorLoginResponse
    {
        public bool IsApproved { get; set; }
        public int Id { get; set; }
        public string EmailId { get; set; }
        public string MobileNumber { get; set; }
        public string ClinicName { get; set; }
        public string DoctorName { get; set; }
        public string LicenceNumber { get; set; }
        public string Location { get; set; }
        public string PAN { get; set; }

        public int StateMaster_Id { get; set; }
        public int CityMaster_Id { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public double Fee { get; set; }
        public int AdminLogin_Id { get; set; }
        public string Password { get; set; }
        public int ? SlotTime { get; set; }
        public TimeSpan ? StartTime { get; set; }
        public TimeSpan ? EndTime { get; set; }
        public string PinCode { get; set; }
        public string DoctorId { get; set; }
        public IEnumerable<DepartmentSpecialist> DepartmentAndSpecialization { get; set; }
    }

    public class DepartmentSpecialist
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public string SpecialistName { get; set; }

    }
}