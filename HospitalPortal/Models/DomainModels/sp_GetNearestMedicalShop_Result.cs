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
    
    public partial class sp_GetNearestMedicalShop_Result
    {
        public int Id { get; set; }
        public string ChemistName { get; set; }
        public string ShopName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string LicenceImage { get; set; }
        public string Address { get; set; }
        public Nullable<double> Distance { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string LocationName { get; set; }
    }
}
