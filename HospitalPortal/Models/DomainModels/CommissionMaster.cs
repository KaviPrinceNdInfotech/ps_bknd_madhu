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
    
    public partial class CommissionMaster
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<double> Commission { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}