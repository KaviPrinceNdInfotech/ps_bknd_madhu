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
    
    public partial class AdminLogin
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AdminLogin()
        {
            this.VehicleTemps = new HashSet<VehicleTemp>();
            this.BankDetails = new HashSet<BankDetail>();
            this.CityTemps = new HashSet<CityTemp>();
            this.PatientComplaints = new HashSet<PatientComplaint>();
            this.Hospitals = new HashSet<Hospital>();
            this.RWAs = new HashSet<RWA>();
            this.Vendors = new HashSet<Vendor>();
            this.Nurses = new HashSet<Nurse>();
            this.HealthCheckupCenters = new HashSet<HealthCheckupCenter>();
            this.NurseComplaints = new HashSet<NurseComplaint>();
            this.Chemists = new HashSet<Chemist>();
        }
    
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
        public string Token { get; set; }
        public Nullable<bool> IsLoginApproved { get; set; }
        public string TransactionPwd { get; set; }
        public string UserID { get; set; }
        public string Confirmpassword { get; set; }
        public string DeviceId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VehicleTemp> VehicleTemps { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BankDetail> BankDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CityTemp> CityTemps { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PatientComplaint> PatientComplaints { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Hospital> Hospitals { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RWA> RWAs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Vendor> Vendors { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Nurse> Nurses { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HealthCheckupCenter> HealthCheckupCenters { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NurseComplaint> NurseComplaints { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Chemist> Chemists { get; set; }
    }
}
