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
    
    public partial class MainCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MainCategory()
        {
            this.VehicleTemps = new HashSet<VehicleTemp>();
            this.VehicleTypes = new HashSet<VehicleType>();
            this.VehicleCharges = new HashSet<VehicleCharge>();
        }
    
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string Type { get; set; }
        public Nullable<int> AmbulanceType_id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VehicleTemp> VehicleTemps { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VehicleType> VehicleTypes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VehicleCharge> VehicleCharges { get; set; }
        public virtual AmbulanceType AmbulanceType { get; set; }
    }
}
