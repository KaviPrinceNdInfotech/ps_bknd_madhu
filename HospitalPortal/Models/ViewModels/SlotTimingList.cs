using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class SlotTimingList
    {
        //public TimeSpans? SlotStartTime { get; set; }
        //public TimeSpans? SlotEndTime { get; set; }
        public string DoctorName { get; set; }
        public bool? IsBooked { get; set; }
    }
}