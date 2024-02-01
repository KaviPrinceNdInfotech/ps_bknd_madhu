using HospitalPortal.Models.CommonClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class HospitalDoctorDTO: StateCityAbs
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        public string DoctorName { get; set; }
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string MobileNumber { get; set; }
        [Required]
        [EmailAddress]
        public string EmailId { get; set; }
        [Required]
        public int StateMaster_Id { get; set; }
        [Required]
        public int CityMaster_Id { get; set; }
        [Required]
        public string Location { get; set; }
        public string LicenceImage { get; set; }
        public string LicenceNumber { get; set; }
        public string ClinicName { get; set; }
        [Required]
        public int Department_Id { get; set; }
        [Required]
        public int Specialist_Id { get; set; }
        public bool IsApproved { get; set; }
        public string PAN { get; set; }
        public string AadharNumber { get; set; }
        public string AadharImage { get; set; }
        public int Hospital_Id { get; set; }
        public SelectList DepartmentList { get; set; }
        public SelectList SpecialistList { get; set; }
        public HttpPostedFileBase AadharImageFile { get; set; }
        public HttpPostedFileBase LicenceImageFile { get; set; }
        public string DepartmentName { get; set; }
        public string SpecialistName { get; set; }
        public Nullable<System.TimeSpan> StartTime { get; set; }
        public Nullable<System.TimeSpan> EndTime { get; set; }
        public Nullable<int> SlotTime { get; set; }
        public Nullable<double> Fee { get; set; }
    }
}