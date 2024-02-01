using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class AppointmentDetails
    {
        public string PatientName { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string AppointmentDate1 { get; set; }
        public double Amount { get; set; }
    }
}