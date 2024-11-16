using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class InvoiceDTO
    {
        public int Id { get; set; }
        public string ServiceProviderName { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public string EmailId { get; set; }
        public string PatientEmailId { get; set; }
        public string PinCode { get; set; }
        public string PatientPinCode { get; set; }
        public string Qualification { get; set; }
        public string MobileNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string OrderId { get; set; }
        public string UniqueId { get; set; }
        public int Patient_Id { get; set; } 
        public double Fee { get; set; }
        public string ClinicName { get; set; }
        public string NurseTypeName { get; set; }
        public string PAN { get; set; }
        public double? TotalFee { get; set; }
        public double GST { get; set; } 
        public DateTime OrderDate { get; set; } 
        public DateTime Startdate { get; set; } 
        public DateTime Enddate { get; set; } 
        public int TotalNumberofdays { get; set; } 
        public string Location { get; set; }
        public string PatientLocation { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal FinalAmount { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string PatientStateName { get; set; }
        public string PatientCityName { get; set; }
        public string PatientRegNo { get; set; }
        public string DepartmentName { get; set; }
        public string SpecialistName { get; set; }
    }
}