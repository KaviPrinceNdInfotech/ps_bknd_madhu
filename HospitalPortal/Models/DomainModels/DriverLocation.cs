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
    
    public partial class DriverLocation
    {
        public int Id { get; set; }
        public Nullable<int> Driver_Id { get; set; }
        public Nullable<double> Lat_Driver { get; set; }
        public Nullable<double> Lang_Driver { get; set; }
        public Nullable<int> PatientId { get; set; }
        public Nullable<bool> key { get; set; }
        public Nullable<int> AmbulanceType_id { get; set; }
        public Nullable<int> VehicleType_id { get; set; }
        public Nullable<double> end_Lat { get; set; }
        public Nullable<double> end_Long { get; set; }
        public Nullable<double> start_Lat { get; set; }
        public Nullable<double> start_Long { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> EntryDate { get; set; }
        public string IsPay { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<int> TotalPrice { get; set; }
        public Nullable<int> ToatlDistance { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public Nullable<bool> RejectedStatus { get; set; }
        public string PaymentStatus { get; set; }
    
        public virtual Driver Driver { get; set; }
        public virtual Patient Patient { get; set; }
    }
}
