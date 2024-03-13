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
    public class ChemistDTO:StateCityAbs 
    {
        public bool IsCheckedTermsCondition { get; set; }
        public string UniqueId { get; set; }
        public int? Location_Id { get; set; }
        public bool IsCityAvailable { get; set; }
        //public string PanImage { get; set; }
        //[Required(ErrorMessage = "Pan Image File Required")]
        //public HttpPostedFileBase PanImageFile { get; set; }
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }
        public bool IsBankUpdateApproved { get; set; }
        [Required (ErrorMessage ="Chemist Name Required")]
        public string ChemistName { get; set; }
        [Required(ErrorMessage = "Shop Name Required")]
        public string ShopName { get; set; }
        [Required]
        [MobileNumberValidation]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile Number must be numeric")]
        public string MobileNumber { get; set; }
        [Required]
        [FormValidations]
        public string EmailId { get; set; }
        public string LicenceImage { get; set; }
        public string Certificateimg { get; set; }
        
        public string PAN { get; set; }
        public string LicenceNumber { get; set; }
        [Required]
        //[DataType(DataType.Password)]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        //[Display(Name = "Password")]
        //[RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string Password { get; set; }
        
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "State Field Can't be null")]
        public int StateMaster_Id { get; set; }
        [Required(ErrorMessage ="City cant be null")]
        public int CityMaster_Id { get; set; }
        [Required(ErrorMessage = "Location Field Can't be null")]
        public string Location { get; set; }
        public string GSTNumber { get; set; }
        public int AdminLogin_Id { get; set; }
        //public string PAN { get; set; }
        [Required(ErrorMessage = "Licence Field Can't be null")]
        public HttpPostedFileBase LicenceImageFile { get; set; }
        public Nullable<System.DateTime> LicenseValidity { get; set; }
        public int? Vendor_Id { get; set; }
        [Required]
        [RegularExpression(@"^(\d{6,6})$", ErrorMessage = "6 Digits Required")]
        public string PinCode { get; set; }
        //public string CityNames { get; set; }
        public bool IsLocationAvail { get; set; }
        //public string LocationName { get; set; }
        public string OtherCity { get; set; }
        public string OtherLocation { get; set; }

        public string RefId { get; set; }
        public string ChemistId { get; set; }




        public class Chemistregistration
        {
            public bool IsCheckedTermsCondition { get; set; }
            public string UniqueId { get; set; }
            public int Location_Id { get; set; }
            public bool IsCityAvailable { get; set; }
            public string VendorName { get; set; }
            public string CompanyName { get; set; }
            public int Id { get; set; }
            public bool IsDeleted { get; set; }
            public bool IsApproved { get; set; }
            [Required(ErrorMessage = "Chemist Name Required")]
            public string ChemistName { get; set; }
            [Required(ErrorMessage = "Shop Name Required")]
            public string PAN { get; set; }
            public string ShopName { get; set; }
            [Required]
            [MobileNumberValidation]
            [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile Number must be numeric")]
            public string MobileNumber { get; set; }
            [Required]
            [FormValidations]
            public string EmailId { get; set; }
            public string Certificateimg { get; set; }
            public string Certificateimgbase64 { get; set; }
            [Required(ErrorMessage = "Licence Number Required")]
            public string LicenceNumber { get; set; }
            [Required]

            public Nullable<System.DateTime> LicenseValidity { get; set; }
            public string Password { get; set; }
             
            public string ConfirmPassword { get; set; }
            [Required(ErrorMessage = "State Field Can't be null")]
            public int StateMaster_Id { get; set; }
            [Required(ErrorMessage = "City cant be null")]
            public int CityMaster_Id { get; set; }
            [Required(ErrorMessage = "Location Field Can't be null")]
            public string Location { get; set; }
            public string GSTNumber { get; set; }
            public int AdminLogin_Id { get; set; }



           
            public int? Vendor_Id { get; set; }
            [Required]
            [RegularExpression(@"^(\d{6,6})$", ErrorMessage = "6 Digits Required")]
            public string PinCode { get; set; }

            public bool IsLocationAvail { get; set; }

            public string OtherCity { get; set; }
            public string OtherLocation { get; set; }

            public string RefId { get; set; }
            public string ChemistId { get; set; }
        }

        public class chemistdetail
        {
            public int Id { get; set; }

            public string ChemistName { get; set; }

            public string MobileNumber { get; set; }
            //public string EmailId { get; set; }

            public string cityname { get; set; }
            public string StateName { get; set; }
            public string Location { get; set; }

            public Nullable<double> Amount { get; set; }


        }

        public class paymentdetail
        {
            public string ChemistName { get; set; }
            public string BranchName { get; set; }
            public Nullable<double> Amount { get; set; }
            public int? PaymentId { get; set; }
            public Nullable<System.DateTime> PaymentDate { get; set; }
        }

        public class chemistpro_detail
        {
            public int Id { get; set; }
            public string ChemistName { get; set; }
            public string EmailId { get; set; }
            public string MobileNumber { get; set; }
            public string Location { get; set; }
            public string StateName { get; set; }
            public string cityname { get; set; }
            public string PinCode { get; set; }
            public int StateMaster_Id { get; set; }
            public int CityMaster_Id { get; set; }
        }

        public class payoutdetail
        {
            public int? Id { get; set; }
            public string ChemistName { get; set; }
            public Nullable<double> Amount { get; set; }
            public Nullable<System.DateTime> PaymentDate { get; set; }

        }

        public class ChemistUpdateProfile
        {
            public int? Id { get; set; }
            public string ChemistName { get; set; }
            public string ShopName { get; set; }
            public int StateMaster_Id { get; set; }
            public int CityMaster_Id { get; set; }
            public string Location { get; set; }
            public string GSTNumber { get; set; } 
            public string EmailId { get; set; } 
            public string LicenceNumber { get; set; }
            public string LicenceImage { get; set; }
            public string LicenceImageBase64 { get; set; }
        }

        public class ChemistPro_bnkUpdate
        {
            public int Id { get; set; }
            public string ShopName { get; set; }
            public string MobileNumber { get; set; }
            public int StateMaster_Id { get; set; }
            public int CityMaster_Id { get; set; }
            public string Location { get; set; }
            public int AdminLogin_Id { get; set; }
            public string PinCode { get; set; }
            public string AccountNo { get; set; }
            public string IFSCCode { get; set; }
            public string BranchName { get; set; }

        }

        public class Chemist_Comp
        {
            public int Id { get; set; }
            public Nullable<int> patsubid { get; set; }
            public string others { get; set; }
            public string Complaints { get; set; }
            public Nullable<bool> IsDeleted { get; set; }
            public Nullable<int> Login_Id { get; set; }
        }


        public class Ch_About
        {
            public int Id { get; set; }
            public string About { get; set; }
        }


    }



}

