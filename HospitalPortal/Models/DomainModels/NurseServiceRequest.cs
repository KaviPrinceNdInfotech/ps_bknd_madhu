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
    
    public partial class NurseServiceRequest
    {
        public int Id { get; set; }
        public int NurseService_Id { get; set; }
        public int Nurse_Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}