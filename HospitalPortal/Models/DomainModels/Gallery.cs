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
    
    public partial class Gallery
    {
        public int Id { get; set; }
        public string ImageName { get; set; }
        public string Images { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> Franchise_Id { get; set; }
    }
}