using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class ViewDoctorReports
    {
        public IEnumerable<TestHistory> test { get; set; }
        public IEnumerable<PatientItem> patientItem { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
    }

    public class PatientItem
    {
        public int Patient_Id { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string PatientRegNo { get; set; }
        public int Id { get; set; }
    }

    public class TestHistory
    {
        public int Id { get; set; }
        public string TestName { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }
        public string Image5 { get; set; }
        public DateTime UploadDate { get; set; }
        public string PatientName { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public int StateMaster_Id { get; set; }
        public int CityMaster_Id { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string Location { get; set; }
        public string EmailId { get; set; }
        public string LabName { get; set; }

        public int Patient_Id { get; set; }
    }
}