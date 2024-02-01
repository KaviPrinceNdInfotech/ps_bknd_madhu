using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class ViewAppointment
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public Nullable<System.DateTime> AppointmentDate { get; set; }
        public TimeSpan? StartSlotTime { get; set; }
        public TimeSpan? EndSlotTime { get; set; }
        public string DoctorName { get; set; }
        public string ClinicName { get; set; }
        public string Location { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string DepartmentName { get; set; }
        public string SpecialistName { get; set; }
        public string Disease { get; set; }
    }
}