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
    
    public partial class NurseService
    {
        public int Id { get; set; }
        public Nullable<int> Nurse_Id { get; set; }
        public int Patient_Id { get; set; }
        public string MobileNumber { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public bool IsPaid { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public Nullable<System.DateTime> ServiceAcceptanceDate { get; set; }
        public System.DateTime RequestDate { get; set; }
        public string ServiceStatus { get; set; }
        public Nullable<double> PerDayAmount { get; set; }
        public Nullable<int> NurseTypeId { get; set; }
        public Nullable<int> LocationId { get; set; }
        public Nullable<System.DateTime> ServiceDate { get; set; }
        public string ServiceType { get; set; }
        public string ServiceTime { get; set; }
        public Nullable<double> TotalFee { get; set; }
        public Nullable<int> Slotid { get; set; }
        public string Location { get; set; }
        public Nullable<int> StateMaster_Id { get; set; }
        public Nullable<int> CityMaster_Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string OrderId { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<bool> IsPayoutPaid { get; set; }
    }
}
