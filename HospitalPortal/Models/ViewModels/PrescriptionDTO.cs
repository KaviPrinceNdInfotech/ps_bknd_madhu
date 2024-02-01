using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class PrescriptionDTO
    {
        public string PreferredAppointmentDateTime1 { get; set; }
        public string PreferredAppointmentDateTime2 { get; set; }
        public string PreferredAppointmentDateTime3 { get; set; }
        public string HospitalName { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSpeciality { get; set; }
        public string DoctorCode { get; set; }
        public string RegistrationNumber { get; set; }
        public string AppointmentHour1 { get; set; }
        public string AppointmentMin1 { get; set; }
        public string AppointmentHour2 { get; set; }
        public string AppointmentMin2 { get; set; }
        public string AppointmentHour3 { get; set; }
        public string AppointmentMin3 { get; set; }
    }
    
}