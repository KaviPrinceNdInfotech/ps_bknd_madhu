using HospitalPortal.Models.CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.RequestModel
{
    public class EditClinicDTO :StateCityAbs
    {
        public int Id { get; set; }
        public int StateMaster_Id { get; set; }
        public int CityMaster_Id { get; set; }
        public string Location { get; set; }
        public string ClinicName { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string SlotTiming { get; set; }
        public string FullAddress { get; set; }
        public string AppointedTime { get; set; }
    }
}