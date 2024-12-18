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
    
    public partial class BookTestLab
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BookTestLab()
        {
            this.LabBookings = new HashSet<LabBooking>();
        }
    
        public int Id { get; set; }
        public Nullable<int> Lab_Id { get; set; }
        public Nullable<int> Test_Id { get; set; }
        public Nullable<int> Patient_Id { get; set; }
        public string PatientName { get; set; }
        public string PatientAddress { get; set; }
        public string ContactNumber { get; set; }
        public Nullable<System.DateTime> TestDate { get; set; }
        public Nullable<System.TimeSpan> AvailabelTime1 { get; set; }
        public Nullable<System.TimeSpan> AvailableTime2 { get; set; }
        public Nullable<bool> IsTaken { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public Nullable<bool> IsPaid { get; set; }
        public Nullable<int> CityMaster_Id { get; set; }
        public Nullable<int> StateMaster_Id { get; set; }
        public Nullable<int> Slotid { get; set; }
        public Nullable<bool> IsCancel { get; set; }
        public string InvoiceNumber { get; set; }
        public string OrderId { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<bool> IsPayoutPaid { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LabBooking> LabBookings { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual LabTest LabTest { get; set; }
        public virtual Lab Lab { get; set; }
    }
}
