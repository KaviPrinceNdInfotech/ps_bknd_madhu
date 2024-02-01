using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class Appointment
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public IEnumerable<ViewAppointment> viewAppointment { get; set; }

        public IEnumerable<ViewAppointmentByDoctorVM> ViewAppointByDoctor { get; set; }
         public int TotalPages { get; set; }

        public int PageNumber { get; set; }
    }
}