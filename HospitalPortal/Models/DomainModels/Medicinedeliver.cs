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
    
    public partial class Medicinedeliver
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string DeliveryAddress { get; set; }
        public Nullable<int> CityMaster_Id { get; set; }
        public Nullable<int> StateMaster_Id { get; set; }
        public string PinCode { get; set; }
        public Nullable<int> Patient_Id { get; set; }
    }
}
