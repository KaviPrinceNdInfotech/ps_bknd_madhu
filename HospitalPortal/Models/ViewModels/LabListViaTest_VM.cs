using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class LabListViaTest_VM
    {
        public int LabId { get; set; }
        public string LabName { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        //public TimeSpan StartTime { get; set; }
        //public TimeSpan EndTime { get; set; }
        //public int PatientId { get; set; }
        public string OpeningHours { get; set; }
        public string TestName { get; set; }
        public int TestId { get; set; }
        public double TestAmount { get; set; }
        //public string Messge { get; set; }
        public bool Status { get; set; }
    }
}