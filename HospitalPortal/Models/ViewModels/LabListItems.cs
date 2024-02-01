using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class LabListItems
    {
        public int Id { get; set; }
        public string LabName { get; set; }
        //public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        //public TimeSpan StartTime { get; set; }
        //public TimeSpan EndTime { get; set; }
        public string Location { get; set; }
        public string OpeningHours { get; set; }
        public string WorkingDay { get; set; }
        public double Fee { get; set; }

    }
}