using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class ViewAppointmentByDoctorVM
    {
        public string PatientRegNo { get; set; }
        public int Id { get; set; }
        public string PatientName { get; set; }
        public Nullable<System.DateTime> AppointmentDate { get; set; }
        //public string Specility { get; set; }
        public string AppointedTime { get; set; }
        public string MobileNumber { get; set; }
        public bool IsCancelled { get; set; }

    }
}