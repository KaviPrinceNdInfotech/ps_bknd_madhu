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
    
    public partial class BankDetail
    {
        public int Id { get; set; }
        public Nullable<int> Login_Id { get; set; }
        public string AccountNo { get; set; }
        public string IFSCCode { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string HolderName { get; set; }
        public string CancelCheque { get; set; }
        public string MobileNumber { get; set; }
        public Nullable<bool> isverified { get; set; }
    
        public virtual AdminLogin AdminLogin { get; set; }
    }
}
