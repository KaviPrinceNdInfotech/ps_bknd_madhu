using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class DoctorslotDTO
    {
        public int Id { get; set; }

        public string SlotTime { get; set; }

        public Nullable<System.TimeSpan> StartTime { get; set; }

        public Nullable<System.TimeSpan> EndTime { get; set; }
        public IEnumerable<Slotlist> Slotlist { get; set; }
    }
    public class Slotlist
    {
        public int Id { get; set; }

        public string SlotTime { get; set; }
    }
}