//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HospitalPortal.Models.DomainModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class CancelledAppointment
    {
        public int Id { get; set; }
        public Nullable<int> Patient_Id { get; set; }
        public Nullable<System.DateTime> AppointmentDate { get; set; }
        public Nullable<System.TimeSpan> StartSlotTime { get; set; }
        public Nullable<System.TimeSpan> EndSlotTime { get; set; }
        public Nullable<int> Doctor_Id { get; set; }
        public Nullable<int> Specialist_Id { get; set; }
        public Nullable<bool> IsBooked { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<bool> IsPaid { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public Nullable<int> Hospital_Id { get; set; }
        public Nullable<int> HospitalDoc_Id { get; set; }
        public Nullable<bool> IsCancelled { get; set; }
        public Nullable<int> CancelledId { get; set; }
    
        public virtual PatientAppointment PatientAppointment { get; set; }
        public virtual HospitalDoctor HospitalDoctor { get; set; }
        public virtual HospitalDoctor HospitalDoctor1 { get; set; }
        public virtual Specialist Specialist { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Doctor Doctor { get; set; }
    }
}
