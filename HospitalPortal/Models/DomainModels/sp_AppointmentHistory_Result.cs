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
    
    public partial class sp_AppointmentHistory_Result
    {
        public int AppointmentId { get; set; }
        public Nullable<System.DateTime> AppointmentDate { get; set; }
        public string MobileNo { get; set; }
        public Nullable<int> Doctor_Id { get; set; }
        public Nullable<int> Specialist_Id { get; set; }
        public string Specility { get; set; }
        public Nullable<bool> IsCancelled { get; set; }
        public string ClinicName { get; set; }
        public string ClinicAddress { get; set; }
        public string PhoneNo { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<System.TimeSpan> StartTime { get; set; }
        public string DoctornName { get; set; }
        public string AppointedTime { get; set; }
    }
}
