using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class PatientAppointmentVM
    {
        public int Id { get; set; }
        [Required]
        public Nullable<int> Patient_Id { get; set; }
        [Required]
        public Nullable<System.DateTime> AppointmentDate { get; set; }
        //public TimeSpans? StartSlotTime { get; set; }
        //public TimeSpans? EndSlotTime { get; set; }
        [Required]
        public Nullable<int> Doctor_Id { get; set; }
        public Nullable<int> HospitalDoc_Id { get; set; }
        public Nullable<int> Hospital_Id { get; set; }
        public Nullable<int> Specialist_Id { get; set; }
        [Required]
        public string TimeSlot { get; set; }
        public double Fee { get; set; }
        public IEnumerable<ShowAppointmentVM> showAppointment { get; set; }
       //public IEnumerable<SlotTimingVM> slottiming { get; set; }
    }

    public class ShowAppointmentVM
    {
        public int Id { get; set; }
        public Nullable<int> Patient_Id { get; set; }
        public Nullable<System.DateTime> AppointmentDate { get; set; }
        public string TimeSlot { get; set; }
        public Nullable<int> Doctor_Id { get; set; }
        public Nullable<int> Specialist_Id { get; set; }
    }
}