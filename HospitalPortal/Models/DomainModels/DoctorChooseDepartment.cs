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
    
    public partial class DoctorChooseDepartment
    {
        public int Id { get; set; }
        public Nullable<int> Department_id { get; set; }
        public Nullable<int> Specialist_id { get; set; }
        public Nullable<int> State_id { get; set; }
        public Nullable<int> City_id { get; set; }
        public Nullable<int> Patient_id { get; set; }
    
        public virtual DoctorChooseDepartment DoctorChooseDepartment1 { get; set; }
        public virtual DoctorChooseDepartment DoctorChooseDepartment2 { get; set; }
    }
}
