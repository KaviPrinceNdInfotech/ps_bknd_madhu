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
    
    public partial class GetNearDriver_Result
    {
        public int Id { get; set; }
        public Nullable<int> DriverId { get; set; }
        public Nullable<int> KM { get; set; }
        public string Name { get; set; }
        public string DL { get; set; }
        public Nullable<int> Charge { get; set; }
        public Nullable<int> TotalPrice { get; set; }
        public string DeviceId { get; set; }
    }
}