using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class PatientReportMasterVM
    {
        public int DoctorId { get; set; }
        public string PatientRegNo { get; set; }
        public string Message { get; set; }
        public IEnumerable<PatientsList> response { get; set; }
    }

    public class PatientsList
    {
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string PatientRegNo { get; set; }
        public int PatientCount { get; set; }
    }

    public class DoctorProfile
    {
        public int Id { get; set; }
        public string DoctorName { get; set; }
        public string MobileNumber { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string ClinicName { get; set; }
        public string DepartmentName { get; set; }
        //public TimeSpan StartTime { get; set; }
        //public TimeSpan EndTime { get; set; }
        public string AvailableTime { get; set; }

    }

   
    public class GetDriverProfile
    {
        public int Id { get; set; }
        public string DriverName { get; set; }
        public string MobileNumber { get; set; } 
        public string EmailId { get; set; } 
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string PinCode { get; set; }

        public string Location { get; set; }


    }




    public class PatientLists
    {
        public string Message { get; set; }
        public IEnumerable<PatientRecord> response { get; set; }
    }


    public class PatientRecord
    {
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientRegNo { get; set; }
    }

    public class  PatientBasicModel
    {
        
        public List<PatientsData> response { get; set; } 
       
    }

    public class PatientsData
    {
        public string Message { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientRegNo { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        
        public List<PatientReports> response { get; set; }
    }

    public class PatientReports
    {
        public string Image1 { get; set; }
        public DateTime UploadDate { get; set; }
    }
}