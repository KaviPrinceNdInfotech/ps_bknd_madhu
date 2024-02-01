using HospitalPortal.Models.CommonClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class HealthCheckupCenterDTO : StateCityAbs
    {
        public bool? IsCheckedTermsCondition { get; set; }
        public string HealthCheckUpId { get; set; }
        public string UniqueId { get; set; }
        public string CompanyName { get; set; }
        public string VendorName { get; set; }
        [Required]
        [RegularExpression(@"^(\d{6,6})$", ErrorMessage = "6 Digits Required")]
        public string PinCode { get; set; }
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }
        [Required]
        public string LabName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string MobileNumber { get; set; }
        [Required]
        public string EmailId { get; set; }
        [Required]
        public string Password { get; set; }
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        
        public string LicenceImage { get; set; }
        [Required]
        public string LicenceNumber { get; set; }
        [Required]
        public int? StateMaster_Id { get; set; }
        [Required]
        public int? CityMaster_Id { get; set; }
        [Required]
        public string Location { get; set; }
        public int? AdminLogin_Id { get; set; }
        [Required]
        public string PAN { get; set; }
        public string GSTNumber { get; set; }
        public string AadharImage { get; set; }
        public string AadharNumber { get; set; }
        [Required]
        public HttpPostedFileBase LicenceImageFile { get; set; }
        [Required]
        public HttpPostedFileBase AadharImageFile { get; set; }
        [Required]
        public int? Location_Id { get; set; }
        public int? Vendor_Id { get; set; }
        [Required(ErrorMessage ="Registraion Certificate Number is mand")]
        public string RegNo { get; set; }
        public string RegImage { get; set; }
        [Required(ErrorMessage ="Registration Certifacite file is mandatory")]
        public HttpPostedFileBase RegImageFile { get; set; }
        public string OtherCity { get; set; }
        public string OtherLocation { get; set; }

        public Nullable<System.TimeSpan> StateTime { get; set; }
        public Nullable<System.TimeSpan> EndTime { get; set; }
    }
} 