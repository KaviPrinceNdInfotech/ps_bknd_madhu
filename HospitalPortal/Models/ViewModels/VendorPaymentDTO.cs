using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class VendorPaymentDTO
    {
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        public IEnumerable<VendorList> Vendorses {get;set;}
        public IEnumerable<VendorsDoctors> VendorDoctor { get; set; }
        public IEnumerable<VendorsDriver> VendorsDriver { get; set; }
        public IEnumerable<VendorsVehicle> VendorsVehicle { get; set; }
        public IEnumerable<VendorsHealth> VendorsHealth { get; set; }
        public IEnumerable<VendorsLab> VendorsLab { get; set; }
        public IEnumerable<VendorsRWA> VendorsRWA { get; set; }
        public IEnumerable<VendorsPatient> VendorsPatient { get; set; }
        public IEnumerable<VendorsNurse> VendorsNurse { get; set; }
        public IEnumerable<VendorsChemist> VendorsChemistss { get; set; }
    }

    public class VendorList
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public string VendorName { get; set; }
        public string UniqueId { get; set; }
        public int TotalPrice { get; set; }
        public string DoctorId { get; set; }
        public double? Amount { get; set; }
        public double? FraPaidableamt { get; set; }
        public decimal Amountwithrazorpaycomm { get; set; }
        public decimal? AmountForVehicle { get; set; }
        public string CompanyName { get; set; }
        public double Counts { get; set; }
      
    }

    public class VendorsDoctors
    {
        public int Id { get; set; }
        public string DoctorName { get; set; }
        public string CityName { get; set; }
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        public double? Amount { get; set;}

        public DateTime? AppointmentDate { get; set; }
    }

        public class VendorsDriver
    {
        public int Id { get; set; }
        public string DriverName { get; set; }
        public string CityName { get; set; }
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        public double? Amount { get; set; }

        public DateTime? AppointmentDate { get; set; }
    }

    public class VendorsHealth
    {
        public int Id { get; set; }
        public string LabName { get; set; }
        public string CityName { get; set; }
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        public double? Amount { get; set; }

        public DateTime? TestDate { get; set; }

    }

    public class VendorsLab
    {
        public int Id { get; set; }
        public string LabName { get; set; }
        public string CityName { get; set; }
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        public double? Amount { get; set; }

        public DateTime? TestDate { get; set; }
    }

    public class VendorsNurse
    {
        public int Id { get; set; }
        public string NurseName { get; set; }
        public string CityName { get; set; }
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        public double? Counts { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
    }

    public class VendorsRWA
    {
        public int Id { get; set; }
        public string AuthorityName { get; set; }
        public string CityName { get; set; }
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
    }

    public class VendorsPatient
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string CityName { get; set; }
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
    }

    public class VendorsVehicle
    {
        public int Id { get; set; }
        public int TotalPrice { get; set; }
        public string VehicleNumber { get; set; }
        public string CityName { get; set; }
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        public double? Amount { get; set; }
    }

    public class VendorsChemist
    {
        public int Id { get; set; }
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        public double? Amount { get; set; }
    }
}