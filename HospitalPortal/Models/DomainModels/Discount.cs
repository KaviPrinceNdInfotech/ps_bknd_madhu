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
    
    public partial class Discount
    {
        public int Id { get; set; }
        public string Amount { get; set; }
        public string DiscountCoupon { get; set; }
        public Nullable<int> Professional_Id { get; set; }
    }
}
