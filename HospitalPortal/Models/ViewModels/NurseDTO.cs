using HospitalPortal.Models.CommonClasses;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class NurseDTO:StateCityAbs
    {
        public bool? IsCheckedTermsCondition { get; set; }
        public string NurseId { get; set; }
        public string UniqueId { get; set; }
        public HttpPostedFileBase AadharImageBase { get; set; }
        public string AadharImage { get; set; }
        public string NurseImage { get; set; }
        public HttpPostedFileBase NurseImageBase { get; set; }
        public int? HospitalId { get; set; }
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }
        [Required (ErrorMessage ="Nurse Name Required")]
        public string NurseName { get; set; }
        [Required]
        [MobileNumberValidation]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile Number must be numeric")]
        public string MobileNumber { get; set; }
        [Required]
        [FormValidations]
        public string EmailId { get; set; }
        public string VendorName { get; set; }

        public string CompanyName { get; set; }
        public string CertificateImage { get; set; }
       
        public string CertificateNumber { get; set; }
        [Required]
        //[DataType(DataType.Password)]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        //[Display(Name = "Password")]
        //[RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string Password { get; set; }
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "State Required")]
        public int? StateMaster_Id { get; set; }
       
        public int? CityMaster_Id { get; set; }
        [Required(ErrorMessage = "Address Required")]

        public string Location { get; set; }
        public int? AdminLogin_Id { get; set; }
        //[Required(ErrorMessage = "Pan No Required")]

        //public string PAN { get; set; }
        //[Required(ErrorMessage = "Aadhar No Required")]

        //public string AadharNumber { get; set; }
    
        //public string AadharImage { get; set; }
        public string PanImage { get; set; }
        //public string AadharImage2 { get; set; }
        [Required(ErrorMessage = "Fee Required")]

        public double Fee { get; set; }
        //public HttpPostedFileBase VerificationImage { get; set; }
       // public HttpPostedFileBase PanImageFile { get; set; }
        //public HttpPostedFileBase AadharImageFile { get; set; }
        //public HttpPostedFileBase AadharImageFile2 { get; set; }
        public HttpPostedFileBase CertificateFile { get; set; }
        [Required(ErrorMessage ="This field is required")]
        public int? NurseType_Id { get; set; }
        public SelectList NurseTypes { get; set; }
        public int? Vendor_Id { get; set; }
        public string PanImageName { get; set; }
        public string PAN { get; set; }
        //[Required]
        public string PanBase64Image { get; set; }
        //[Required]
        public string AadharImageName { get; set; }
        //[Required]
        public string AadharBase64Image { get; set; }

        //public string AadharImageName2 { get; set; }
        //[Required]
        public string AadharBase64Image2 { get; set; }
        //public string VerificationBase64Image { get; set; }
        //public string VerificationImageName { get; set; }
        public string CertificateBase64Image { get; set; }
        public string CertificateImageName { get; set; }
       // public bool IsVerifiedByPolice { get; set; }
        public string VerificationDoc { get; set; }
        [Required]
        [RegularExpression(@"^(\d{6,6})$", ErrorMessage = "6 Digits Required")]
        public string PinCode { get; set; }
        public IEnumerable<Nurse4Commission> Nurse4Commission { get; set; }
        public decimal? HrsFeex24 { get; set; }
        public decimal? MonthFee { get; set; }
        public bool IsCityAvailable { get; set; }
        public string OtherCity { get; set; }
        public string NurseTypeName { get; set; }
        public string RefId { get; set; }

    }

    public class Nurse4Commission
    {

        public int Nurse_Id { get; set; }
        public int Id { get; set; }
        public string NurseName { get; set; }
        public double Fee { get; set; }
    }

    public class NurseRequestedParams
    {
        public int? NurseType_Id { get; set; }
        public string CertificateNumber { get; set; }
        public string CityName { get; set; }
        [Required]
        public string PinCode { get; set; }
        [Required]
        public string NurseName { get; set; }
        public Nullable<int> experience { get; set; }
        public string MobileNumber { get; set; }
        public string PanImage { get; set; }
        public string PAN { get; set; }
        public string PanBase64Image { get; set; }
        public string NurseImage { get; set; }
        public string NurseImageBase64Image { get; set; }
        [Required]
        public string EmailId { get; set; }
        [Required]
        public int StateMaster_Id { get; set; }
        public string ConfirmPassword { get; set; }
        public int CityMaster_Id { get; set; } 
        [Required]
        public double Fee { get; set; }
        [Required]
        public string Password { get; set; }
        [Required, System.ComponentModel.DataAnnotations.Compare("Password")]
        public string Location { get; set; }
        public string CertificateBase64Image { get; set; }
        //public bool IsVerifiedByPolice { get; set; }

        public string CertificateImage { get; set; } 
        public int Location_id { get; set; }
    }



}