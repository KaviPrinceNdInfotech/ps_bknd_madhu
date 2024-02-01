using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class CancelAppointementDTO
    {
        public int Id { get; set; }
        public Nullable<int> Patient_Id { get; set; }
        public Nullable<System.DateTime> AppointmentDate { get; set; }
        //public TimeSpans? StartSlotTime { get; set; }
        //public TimeSpans? EndSlotTime { get; set; }
        public Nullable<int> Doctor_Id { get; set; }
        public Nullable<int> HospitalDoc_Id { get; set; }
        public Nullable<int> Hospital_Id { get; set; }
        public Nullable<int> Specialist_Id { get; set; }
        public string TimeSlot { get; set; }
        public double Fee { get; set; }
    }
}