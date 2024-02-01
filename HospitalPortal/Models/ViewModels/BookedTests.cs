using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class BookedTests
    {
        public IEnumerable<TestList> TestList { get; set; }
        public IEnumerable<BookingTestHistory> BookingHistory { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public string PatientName { get; set; }
    }

    public  class TestList
    {
        public int Id { get; set; }
        public int LabId { get; set; }
        public string PatientName { get; set; }
        public string PatientAddress { get; set; }
        public string ContactNumber { get; set; }
        public Nullable<System.DateTime> TestDate { get; set; }
        public string AvailableTime { get; set; }
        public string TestName { get; set; }
        public double? TestAmount { get; set; }
        public bool IsTaken { get; set; }
    }


    public class BookingTestHistory
    {
        public int Id { get; set; }
        //public int LabId { get; set; }
        public string LabName { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public Nullable<System.DateTime> TestDate { get; set; }
        public string AvailableTime { get; set; }
        public string TestName { get; set; }
        public double TestAmount { get; set; }
    }
}
