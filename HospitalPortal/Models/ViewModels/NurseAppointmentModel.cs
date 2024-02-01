using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class NurseAppointmentModel
    {
        public IEnumerable<NurseAppointmentWithUser> Appointments { get; set; }
        public int Page { get; set; }
        public int NumberOfPages { get; set; }
        public string Term { get; set; }
        public DateTime ? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}