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
    
    public partial class usp_getAppintmentSlots_Result
    {
        public int Id { get; set; }
        public Nullable<System.TimeSpan> TSStart { get; set; }
        public Nullable<System.TimeSpan> TSEnd { get; set; }
        public string Timeslot { get; set; }
        public int IsBooked { get; set; }
    }
}