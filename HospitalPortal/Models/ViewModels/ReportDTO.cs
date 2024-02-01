using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class ReportDTO
    {

        public IEnumerable<HospitalNurseCommissionReport> HospitalNurseCommissionReportList { get; set; }
        public double Amount { get; set; }
        public string VendorName { get; set; }
        public DateTime? JoiningDate { get; set; }
        public IEnumerable<VendorCommissionReport>  VendorCommissionReport { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime TestDate { get; set; }
        public IEnumerable<HealthCommissionReport> HealthCommisionReport { get; set; }
        public IEnumerable<DoctorCommissionReport> DoctorCommisionReport { get; set; }
        public IEnumerable<LabCommissionReport> LabCommisionReport { get; set; }
        public IEnumerable<DoctorReports> DoctorReport { get; set; }
        public IEnumerable<LabReportsVM> LabReport { get; set; }
        public IEnumerable<ChemistReport> ChemistReport { get; set; }
        public IEnumerable<ViewLabDetails> ViewLabDetails { get; set; }
        public IEnumerable<ViewHealthDetails> ViewHealthDetails { get; set; }
        public IEnumerable<ViewChemistDetails> VieChemistDetails { get; set; }
        public IEnumerable<AppointmentDetails> AppointmentDetails { get; set; }
        public IEnumerable<ViewNurseList> NurseList { get; set; }
        public IEnumerable<HealthCheckListVM> ViewCmplteCheckUp { get; set; }
        public string MedicineName { get; set; }
        public double MRP { get; set; }
        public string DoctorName { get; set; }
        public string HospitalName { get; set; }
        public string MobileNumber { get; set; }
        public string LicenceNumber { get; set; }
        public string ClinicName { get; set; }
        public string ChemistName { get; set; }
        public string ShopName { get; set; }
        //public string MobileNumber { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string Location { get; set; }
        public string LabName { get; set; }
        public string NurseName { get; set; }
        
        public string EmailId { get; set; }
        public int Id { get; set; }
        public SelectList NurseTypeList { get; set; }
        public string CertificateNumber { get; set; }

    }
}