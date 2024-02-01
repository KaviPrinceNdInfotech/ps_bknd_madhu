using HospitalPortal.Models.CommonClasses;
using HospitalPortal.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class HospitalDTO : StateCityAbs
    {
        public bool? IsCheckedTermsCondition { get; set; }
        public string HospitalId { get; set; }
        public string UniqueId { get; set; }
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        [Required]
        [RegularExpression(@"^(\d{6,6})$", ErrorMessage = "6 Digits Required")]
        public string PinCode { get; set; }
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        [Required(ErrorMessage = "Hospital Name Can't Be Empty")]
        public string HospitalName { get; set; }
        [Required(ErrorMessage = "State Field Can't Be Empty")]
        public int? StateMaster_Id { get; set; }
        [Required(ErrorMessage = "City Field Can't Be Empty")]
        public int? CityMaster_Id { get; set; }
        [Required(ErrorMessage = "Address Field Can't Be Empty")]
        public string Location { get; set; }
        public string AuthorizationLetterImage { get; set; }
        [Required]
        //[DataType(DataType.Password)]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        //[Display(Name = "Password")]
        //[RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string Password { get; set; }
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public bool IsApproved { get; set; }
    
        public string PhoneNumber { get; set; }
        [Required]
        [FormValidations]
        public string EmailId { get; set; }
        [Required]
        [MobileNumberValidation]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile Number must be numeric")]
        public string MobileNumber { get; set; }
        public int AdminLogin_Id { get; set; }
        [Required(ErrorMessage = "Authorization Image File Can't Be Empty")]
        public HttpPostedFileBase AuthorizationImageFile { get; set; }
        [Required]
        public int? Location_Id { get; set; }
        public int? Vendor_Id { get; set; }
        public string OtherCity { get; set; }
        public string OtherLocation { get; set; }

        public string refiId { get; set; }
    }

    public class HospitalRegistrationReq
    {
        //public string CityName { get; set; }
        [Required]
        public string PinCode { get; set; }
        public int? Id { get; set; }
        [Required]
        public string HospitalName { get; set; }
        [Required]
        public int? StateMaster_Id { get; set; }
        [Required]
        public int? CityMaster_Id { get; set; }
        [Required]
        public string Location { get; set; }
        public string AuthorizationLetterImage { get; set; }//null
        [Required]
        public string Password { get; set; }
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string EmailId { get; set; }
        [Required]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string MobileNumber { get; set; }
        [Required]
        public string AuthorizationLetterImageName { get; set; }//kk.jpg
        [Required]
        public string AuthorizationLetterBase64 { get; set; }//
        [Required]
        public int? Location_Id { get; set; }

        public string State { get; set; }
        public string City { get; set; }
        public string Paidamount { get; set; }
        public string Status { get; set; }
    }
    
}