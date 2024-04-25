using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class ReportDetails
    {
        public DateTime RegistrationDate { get; set; }
        public DateTime JoiningDate { get; set; }
        public  IEnumerable<Vendorses> Vendors { get; set; }
        public IEnumerable<Nurses>  Nurses { get; set; }
        public DateTime sdate { get; set;}
        public DateTime ServiceAcceptanceDate { get; set; }
        public DateTime edate { get; set; }
        public DateTime week { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime TestDate { get; set; }
        public DateTime OrderDate { get; set; }
        public IEnumerable<Doctors> doctorList { get; set;}
        public IEnumerable<Labs> LabList { get; set; }
        public IEnumerable<Chemists> ChemistsList { get; set; }
        public IEnumerable<Health> HealthList { get; set; }
    }

    public class Doctors
    {
        public DateTime AppointmentDate { get; set; }
        public string AppointmentDate1 { get; set; }
        public double? Amount { get; set; }
        public string Year { get; set; }
        public string Weeks { get; set; }
        public string DoctorName { get; set; }
        public string DoctorId { get; set; }
        
        public double? PayoutAmt { get; set; }

        
        
    }

    public class Labs
    {
        public DateTime TestDate { get; set; }
        public string TestDate1 { get; set; }
        public double? Amount { get; set; }
        public string Year { get; set; }
        public string Weeks { get; set; }
    }

    public class Chemists
    {
        public DateTime OrderDate { get; set; }
        public string OrderDate1 { get; set; }
        public double? Amount { get; set; }
        public string Year { get; set; }
        public string Weeks { get; set; }
    }

    public class Health
    {
        public DateTime TestDate { get; set; }
        public string TestDate1 { get; set; }
        public double? Amount { get; set; }
        public string Year { get; set; }
        public string Weeks { get; set; }
    }


    public class Nurses
    {
        public DateTime ServiceAcceptanceDate { get; set; }
        public DateTime RequestDate { get; set; }
        public double? TotalFee { get; set; }
        public string NurseName { get; set; }
        public string NurseId { get; set; }

    }

    public class Vendorses
    {
        public double? Counts { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string UniqueId { get; set; }
        public string VendorId { get; set; }
        public string UserName { get; set; }
        public string Name1 { get; set; }
        public string VehicleName { get; set; }
        public string VehicleNumber { get; set; }
        public int commission { get; set; }
        public double? payment { get; set; }
        public decimal? Amount { get; set; }
    }
}