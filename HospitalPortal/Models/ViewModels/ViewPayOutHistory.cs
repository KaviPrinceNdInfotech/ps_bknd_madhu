using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class ViewPayOutHistory
    {
        public string AuthorityName { get; set; }
        public DateTime PaymentDate { get; set; }
        public string DoctorName { get; set; }
        public string ChemistName { get; set; }
        public string LabName { get; set; }
        public string NurseName { get; set; }
        public string VendorName { get; set; }
        public string DriverName { get; set; }
        public IEnumerable<HistoryOfRWA_pAYOUT> HistoryOfRWA_Payout { get; set; }
        public IEnumerable<HistoryOfVendor_Payout> HistoryOfVendor_Payout { get; set; }
        public IEnumerable<HistoryOfNurse_Payout> HistoryOfNurse_Payout { get; set; }
        public IEnumerable<HistoryOfDoc_Payout> HistoryOfDoc_Payout { get; set; }
        public IEnumerable<HistoryOfLab_Payout> HistoryOfLab_Payout { get; set; }
        public IEnumerable<HistoryOfHealth_Payout> HistoryOfHealth_Payout { get; set; }
        public IEnumerable<HistoryOfChemist_Payout> HistoryOfChemist_Payout { get; set; }
        public IEnumerable<HistoryOfAmbulance_Payout> HistoryOfAmbulance_Payout { get; set; }
    }

    public class HistoryOfAmbulance_Payout
    {

        public int Id { get; set; }
        public int Driver_Id { get; set; }
        public string DriverId { get; set; }
        public string DriverName { get; set; }
        public string VehicleName { get; set; }
        public string VehicleNumber { get; set; }
        public string PAN { get; set; }
        public Nullable<System.DateTime> EntryDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public double Amount { get; set; }

        public bool? IsGenerated { get; set; }
        public bool? IsPaid { get; set; }
        public string IFSCCode { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string HolderName { get; set; }
    }
    public class HistoryOfDoc_Payout
    {

        public int Id { get; set; }
        public int Doctor_Id { get; set; }
        public string DoctorName { get; set; }
        public string DoctorId { get; set; }
        public string PatientName { get; set; }
        public string PatientRegNo { get; set; }
        public string AppointmentDate { get; set; } 
        public string EmailId { get; set; }
        public string MobileNumber { get; set; }
        public string Location { get; set; }
        public string PinCode { get; set; }
        public string LicenceNumber { get; set; }
        public string PAN { get; set; }
        public string IFSCCode { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string HolderName { get; set; }
        public DateTime? PaymentDate { get; set; }
        public double Amount { get; set; }

        public bool? IsGenerated { get; set; }
        public bool? IsPaid { get; set; }
    }

    public class HistoryOfNurse_Payout
    {

        public int Id { get; set; }
        public int Nurse_Id { get; set; }
        public string NurseName { get; set; }
        public string NurseId { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string PinCode { get; set; }
        public Nullable<System.DateTime> ServiceDate { get; set; }
        public string PAN { get; set; }
        public string IFSCCode { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string HolderName { get; set; }
        public DateTime? PaymentDate { get; set; }
        public double Amount { get; set; }
        public bool IsGenerated { get; set; }
        public bool IsPaid { get; set; }
    }


    public class HistoryOfLab_Payout
    {
        public int Id { get; set; }
        public int Lab_Id { get; set; }
        public string lABId { get; set; }
        public string MobileNumber { get; set; }
        public string PatientName { get; set; }
        public string PatientRegNo { get; set; }
        public string LabName { get; set; }
        public Nullable<System.DateTime> TestDate { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string PinCode { get; set; }
        public string PAN { get; set; }
        public string LicenceNumber { get; set; }
        public string AadharNumber { get; set; }
        public string IFSCCode { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string HolderName { get; set; }
        public DateTime? PaymentDate { get; set; }
        public double Amount { get; set; }
        public bool IsGenerated { get; set; }
        public bool IsPaid { get; set; }
    }

    public class HistoryOfHealth_Payout
    {
        public int Id { get; set; }
        public int? Health_Id { get; set; }
        public string LabName { get; set; }
        public DateTime? PaymentDate { get; set; }
        public double Amount { get; set; }
        public bool IsGenerated { get; set; }
        public bool IsPaid { get; set; }
    }


    public class HistoryOfChemist_Payout
    {
        public int Id { get; set; }
        public int Chemist_Id { get; set; }
        public string ChemistId { get; set; }
        public string ChemistName { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string PinCode { get; set; }
        public string LicenceNumber { get; set; }
        public string PAN { get; set; }
        public string IFSCCode { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string HolderName { get; set; }
        public DateTime? PaymentDate { get; set; }
        public double Amount { get; set; }
        public bool IsGenerated { get; set; }
        public bool IsPaid { get; set; }
    }

    public class HistoryOfRWA_pAYOUT
    {
        public int Id { get; set; }
        public Nullable<int> Rwa_Id { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<bool> IsGenerated { get; set; }
        public Nullable<bool> IsPaid { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
    }
    public class HistoryOfVendor_Payout
    {
        public int Id { get; set; }
        public int Vendor_Id { get; set; }
        public string VendorName { get; set; }
        public DateTime? PaymentDate { get; set; }
        public double Amount { get; set; }
        public bool IsGenerated { get; set; }
        public bool IsPaid { get; set; }
    }
}