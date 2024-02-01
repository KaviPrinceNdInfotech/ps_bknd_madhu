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
    
    public partial class CheckUpReport
    {
        public int Id { get; set; }
        public Nullable<int> Patient_Id { get; set; }
        public string File { get; set; }
        public Nullable<int> Checkup_Center_Id { get; set; }
        public string TestName { get; set; }
        public Nullable<int> TestId { get; set; }
    
        public virtual Patient Patient { get; set; }
        public virtual LabTest LabTest { get; set; }
        public virtual HealthCheckupCenter HealthCheckupCenter { get; set; }
    }
}
