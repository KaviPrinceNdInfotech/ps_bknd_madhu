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
    
    public partial class VehicleCharge
    {
        public int Id { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<int> Category_Id { get; set; }
        public Nullable<int> TypeId { get; set; }
        public string AppliedFor { get; set; }
    
        public virtual MainCategory MainCategory { get; set; }
        public virtual VehicleType VehicleType { get; set; }
    }
}
