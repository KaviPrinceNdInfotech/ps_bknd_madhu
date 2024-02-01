using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class CancelBookingVM
    {
        public int Id { get; set; }
        public Nullable<int> Patient_Id { get; set; }
        public Nullable<int> Driver_Id { get; set; }
        public Nullable<double> Lat_PickUp_Patient { get; set; }
        public Nullable<double> Lang_PickUp_Pateint { get; set; }
        public Nullable<double> Lang_Drop { get; set; }
        public Nullable<double> Lat_Drop { get; set; }
        public string PickUp_Place { get; set; }
        public string Drop_Place { get; set; }
        public Nullable<int> Vehicle_Id { get; set; }
        public Nullable<bool> IsDriveCompleted { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<double> Distance { get; set; }
        public Nullable<System.DateTime> RequestDate { get; set; }
        public Nullable<decimal> FullAmount { get; set; }
        public Nullable<int> RecordId { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public Nullable<bool> IsPaid { get; set; }
        public Nullable<System.TimeSpan> RideStartTime { get; set; }
        public Nullable<System.TimeSpan> RideEndTime { get; set; }
        public Nullable<bool> Is8HourRide { get; set; }
        public Nullable<double> ExtraCharges { get; set; }

        
        public string DriverName { get; set; }
        public string PatientName { get; set; }
        
    }

    public class cancelReport
    {
        public IEnumerable<CancelBookingVM> CancelBookingVM { get; set; }
        public IEnumerable<CancelDoctor> CancelDoctor { get; set; }
        public IEnumerable<NurseCancel> NurseCancel { get; set; }
    }

    public class CancelDoctor
    {
        public string DoctorName { get; set; }
        public string DoctorId { get; set; }
        public Nullable<System.DateTime> AppointmentDate { get; set; }
        public string SpecialistName { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string Location { get; set; }
        public Nullable<double> TotalFee { get; set; }
        public string SlotTime { get; set; }
        public int Id { get; set; }

        public string MobileNumber { get; set; }
        public string PatientName { get; set; }
        public string PatientRegNo { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> UserWalletAmount { get; set; }
        public Nullable<System.DateTime> CancelDate { get; set; }
    }

    public class NurseCancel
    {
        public int ID { get; set; }
        public string NurseName { get; set; }
        public string NurseId { get; set; }
        public string Location { get; set; }
        public string MobileNumber { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public double Fee { get; set; }
        public Nullable<System.DateTime> ServiceAcceptanceDate { get; set; }
        public System.DateTime RequestDate { get; set; }
        public string PatientName { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> UserWalletAmount { get; set; }
        public Nullable<System.DateTime> CancelDate { get; set; }
    }

 
}