using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class AppointmentDetailDoctorDTO
    {
        public int Id { get; set; }
        public int Patient_Id { get; set; }
        public string PatientName { get; set; } 
        public string Location { get; set; }
        public string SlotTime { get; set; }
        public DateTime? Appointmentdate { get; set; }
        public string DeviceId { get; set; }
        public string MobileNumber { get; set; }
    }
    public class DoctorPayment
    {
        public int Id { get; set; }
        public string PatientName { get; set; }  
        public  string Location { get; set; }
        public double Amount { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public Nullable<int> PaymentId { get; set; }
    }
    public class Doctorbooking
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string Location { get; set; } 
        public string StateName { get; set; } 
        public string CityName { get; set; } 
        public string PatientRegNo { get; set; } 
        public DateTime? Appointmentdate { get; set; } 
    }
}