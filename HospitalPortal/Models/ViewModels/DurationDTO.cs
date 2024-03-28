using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class DurationDTO
    {
        public int Id { get; set; }
        public string Duration { get; set; }
        public IEnumerable<DurationList> DurationLists { get; set; }
    }
    public class DurationList
    {
        public int Id { get; set; }
        public string Duration { get; set; }
    }
    public class Durations
    {
        public int DurationId { get; set; }
        public string DurationTime { get; set; } 
    }
    public class vendors
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
    }
    public class DoctorDuration
    {
        public int Id { get; set; }
        public string Duration { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public TimeSpan? StartTime2 { get; set; }
        public TimeSpan? EndTime2 { get; set; }
    }
}
