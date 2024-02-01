using HospitalPortal.Models.CommonClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class RWADTO:StateCityAbs
    {
        public bool? IsCheckedTermsCondition { get; set; }
        public string RWAId { get; set; }
        [Required]
        [RegularExpression(@"^(\d{6,6})$", ErrorMessage = "6 Digits Required")]
        public string PinCode { get; set; }
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }
        [Required]
        public string AuthorityName { get; set; }
        public int? Vendor_Id { get; set; }
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
        [Required]
        public int StateMaster_Id { get; set; }
        [Required]
        public int CityMaster_Id { get; set; }
        [Required]
        public string Location { get; set; }
        public int AdminLogin_Id { get; set; }
        public string CertificateImage { get; set; }
        public string OtherCity { get; set; }
        public HttpPostedFileBase CertificateFile { get; set; }
    }
}