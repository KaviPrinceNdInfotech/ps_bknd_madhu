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
    
    public partial class CmpltCheckUp
    {
        public int Id { get; set; }
        public string BookingId { get; set; }
        public Nullable<int> PatientId { get; set; }
        public Nullable<System.DateTime> TestDate { get; set; }
        public string PatientName { get; set; }
        public string PatientAddress { get; set; }
        public string ContactNo { get; set; }
        public Nullable<System.DateTime> RequestDate { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public Nullable<bool> IsPaid { get; set; }
        public Nullable<int> Center_Id { get; set; }
        public Nullable<bool> IsTaken { get; set; }
        public Nullable<int> Test_Id { get; set; }
    
        public virtual Patient Patient { get; set; }
        public virtual HealthCheckUp HealthCheckUp { get; set; }
        public virtual HealthCheckupCenter HealthCheckupCenter { get; set; }
    }
}