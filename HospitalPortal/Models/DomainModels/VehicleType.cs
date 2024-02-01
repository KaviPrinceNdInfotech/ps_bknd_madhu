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
    
    public partial class VehicleType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VehicleType()
        {
            this.PatientRequests = new HashSet<PatientRequest>();
            this.TravelMasters = new HashSet<TravelMaster>();
            this.VehicleCharges = new HashSet<VehicleCharge>();
        }
    
        public int Id { get; set; }
        public string VehicleTypeName { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<double> under5KM { get; set; }
        public Nullable<double> under6_10KM { get; set; }
        public Nullable<double> under11_20KM { get; set; }
        public Nullable<double> under21_40KM { get; set; }
        public Nullable<double> under41_60KM { get; set; }
        public Nullable<double> under61_80KM { get; set; }
        public Nullable<double> under81_100KM { get; set; }
        public Nullable<double> under100_150KM { get; set; }
        public Nullable<double> under151_200KM { get; set; }
        public Nullable<double> under201_250KM { get; set; }
        public Nullable<double> under251_300KM { get; set; }
        public Nullable<double> under301_350KM { get; set; }
        public Nullable<double> under351_400KM { get; set; }
        public Nullable<double> under401_450KM { get; set; }
        public Nullable<double> under451_500KM { get; set; }
        public Nullable<double> Above500KM { get; set; }
        public Nullable<int> Category_Id { get; set; }
    
        public virtual MainCategory MainCategory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PatientRequest> PatientRequests { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TravelMaster> TravelMasters { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VehicleCharge> VehicleCharges { get; set; }
    }
}