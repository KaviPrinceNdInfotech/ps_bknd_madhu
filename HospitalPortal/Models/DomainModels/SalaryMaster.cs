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
    
    public partial class SalaryMaster
    {
        public int Id { get; set; }
        public Nullable<int> Emp_Id { get; set; }
        public Nullable<double> Salary { get; set; }
        public Nullable<System.DateTime> PaidDate { get; set; }
        public Nullable<bool> IsPaid { get; set; }
        public Nullable<System.DateTime> GenerateDate { get; set; }
        public Nullable<bool> IsGenerated { get; set; }
        public Nullable<int> Leave { get; set; }
    
        public virtual Employee Employee { get; set; }
    }
}