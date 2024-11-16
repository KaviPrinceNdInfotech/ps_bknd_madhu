using HospitalPortal.Models.CommonClasses;
using HospitalPortal.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class VendorDTO:StateCityAbs
    {
        public bool? IsCheckedTermsCondition { get; set; }
        [Required]
        [RegularExpression(@"^(\d{6,6})$", ErrorMessage = "6 Digits Required")]
        public string PinCode { get; set; }
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
      
        public string CompanyName { get; set; }
        [Required]
        [MobileNumberValidation]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile Number must be numeric")]
        public string MobileNumber { get; set; }
        [Required]
        [EmailAddress]
        public string EmailId { get; set; }
        [Required(ErrorMessage = "State Required")]
        public int StateMaster_Id { get; set; }
        [Required(ErrorMessage = "City Required")]
        public int City_Id { get; set; }
        [Required (ErrorMessage ="Address Required")]
        public string Location { get; set; }
        //[Required(ErrorMessage = "GST Number Required")]
        public string GSTNumber { get; set; }
        [Required]
        //[DataType(DataType.Password)]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        //[Display(Name = "Password")]
        //[RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string Password { get; set; }
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        [Required]
        public string ConfirmPassword { get; set; }
        public bool IsApproved { get; set; }
        public bool IsBankUpdateApproved { get; set; }
        public int AdminLogin_Id { get; set; }
        [Required(ErrorMessage = "Vendor Name Required")]
        public string VendorName { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "Aadhar Number must be Numeric")]
        public string AadharOrPANNumber { get; set; }
        public string AadharOrPANImage { get; set; }
        public string AadharOrPANImage2 { get; set; }
        public HttpPostedFileBase AadharOrImageFile { get; set; }
        public HttpPostedFileBase AadharOrImageFile2 { get; set; }
        public string PanImage { get; set; }
        public HttpPostedFileBase PanFile { get; set; }
        public string PanNumber { get; set; }
        public int Vendor_Id { get; set; }
        public string UniqueId { get; set; }
        public string OtherCity { get; set; }
    }

    public class Franchises_Reg
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public string CompanyName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string PinCode { get; set; }
        public int StateMaster_Id { get; set; }
        public int City_Id { get; set; }
        public string Location { get; set; }
        public string GSTNumber { get; set; }
        public string Password { get; set; }
        public bool IsApproved { get; set; }

        public string AadharOrPANNumber { get; set; }
        public string AadharOrPANImage { get; set; }
        public string AadharOrPANImagebase64 { get; set; }
        public string PanNumber { get; set; }

        public string VendorName { get; set; }


    }

    public class FranchisesBankDetails
    {
        public int Id { get; set; }
        public Nullable<int> Login_Id { get; set; }
        public string AccountNo { get; set; }
        public string IFSCCode { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string HolderName { get; set; }
        public string MobileNumber { get; set; }
        public bool? isverified { get; set; }
    }

    public class Fra_EditProfile
    {
        public int Id { get; set; }
        public string VendorName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public int StateMaster_Id { get; set; }
        public int City_Id { get; set; }
        public string Location { get; set; }
        public string GSTNumber { get; set; }
        public string AadharOrPANNumber { get; set; }
        public string AadharOrPANImage { get; set; }
        public string AadharOrPANImagebase64 { get; set; }
    }

    public class Fran_AddGallery
    {
        public int Id { get; set; }
        public string ImageName { get; set; }
        public string Images { get; set; }
        public string Imagesbase64 { get; set; }
        public int Franchise_Id { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

    }

    public class GetGallery
    {
        public int Id { get; set; }
        public string ImageName { get; set; }
        public string Images { get; set; }

    }

    public class GetTestList
    {
        public int Id { get; set; }
        public string TestName { get; set; }

    }

    public class Edit_TestList
    {
        public int Id { get; set; }
        public String TestName { get; set; }
    }

    public class Fra_ProDetail
    {
        public int Id { get; set; }
        public string UniqueId { get; set; }
        public string VendorName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string StateName { get; set; }
        public string cityname { get; set; }
        public string Location { get; set; }
        public string CompanyName { get; set; }
        public string GSTNumber { get; set; }
        public string AadharOrPANImage { get; set; }
        public string AadharOrPANNumber { get; set; }
        public string PinCode { get; set; }
        public int? City_ID { get; set; }
        public int? StateMaster_Id { get; set; }
    }

    public class Fra_payout_his
    {
        public int Id { get; set; }
        public string VendorName { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string Location { get; set; }
    }

    public class Fra_Compliant
    {
        public int Id { get; set; }
        public Nullable<int> patsubid { get; set; }
        public string Others { get; set; }
        public string Complaints { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsResolved { get; set; }
        public Nullable<int> Login_Id { get; set; }


    }

    public class Departmentdropdown
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }
    }

    public class Specialistdropdown
    {
        public int Id { get; set; }
        public string SpecialistName { get; set; }

    }

    public class Add_dep
    {
        public int Id { get; set; }
        public Nullable<int> Dep_Id { get; set; }
        public Nullable<int> Spec_Id { get; set; }
        [Required]
        public int AdminLogin_Id { get; set; }
        public string Status { get; set; }

    }

    public class DeptAndSpec_List
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public string SpecialistName { get; set; }
        public string Status { get; set; }
    }

    public class Department_List
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }

    }

    public class Edit_Department
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }

    }

    public class Add_dep_ByList
    {
        public string DepartmentName { get; set; }
        public string IsDeleted { get; set; }

    }

    public class GetRole_dropdown
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
    }

    public class FraPaymentHis
    {
        public int Id { get; set; }
        public String VendorName { get; set; }
        public String Location { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
       

    }
    public class Payment_History
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UniqueId { get; set; }
        public double PaidFees { get; set; }
        public double transactionamt { get; set; }
        public double PayableAmount { get; set; }
        public double commamt { get; set; }
        public double tdsamt { get; set; }
        public double Amountwithrazorpaycomm { get; set; }
        public int? PaymentId { get; set; }

        public string Location { get; set; } 
        public string PaymentDate { get; set; }
        public string PaymentTime { get; set; }
    }
    public class TDS_Report
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UniqueId { get; set; }
        public Nullable<double> PaidFees { get; set; }
        public Nullable<double> tdsamt { get; set; }
        public Nullable<double> PayableAmount { get; set; }
        public int? PaymentId { get; set; }

        public string Location { get; set; }
        public string PaymentDate { get; set; }
        public string PaymentTime { get; set; }
        public double TDS { get; set; }
        public double PayAmount { get; set; }
    }
    public class Commission_Report
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double PaidFees { get; set; }
        public int? PaymentId { get; set; }

        public string Location { get; set; } 
        public string PaymentDate { get; set; }
        public string PaymentTime { get; set; }
        public double Commission { get; set; }
        public double PayAmount { get; set; } 
        public string UniqueId { get; set; } 
        public double transactionamt { get; set; }
        public double PayableAmount { get; set; }
        public double commamt { get; set; }
        public double tdsamt { get; set; }
        public double Amountwithrazorpaycomm { get; set; }
      
    }
    public class Vehiclecat
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }
    }

    public class VehicleType_Name
    {
        public int Id { get; set; }

        public string VehicleTypeName { get; set; }
    }

    public class AddVehicletype
    {
        public int Id { get; set; }
        public Nullable<int> Category_Id { get; set; }
        public Nullable<int> VehicleType_Id { get; set; }
        public int AdminLogin_Id { get; set; }
        public string Status { get; set; }

    }
    public class AddCat_Vehicletype
    {
        public int Id { get; set; }
        public Nullable<int> Category_Id { get; set; }
        public Nullable<int> Result { get; set; }
        public string VehicleTypeName { get; set; }
        public string CategoryName { get; set; }

    }

    public class Vehicle_List
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string VehicleTypeName { get; set; }
        public string Status { get; set; }
    }

    public class fra_Chemistregistration
    {
        public bool IsCheckedTermsCondition { get; set; }
        public string UniqueId { get; set; }
        public int Location_Id { get; set; }
        public bool IsCityAvailable { get; set; }

        public string PAN { get; set; }
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }
        [Required(ErrorMessage = "Chemist Name Required")]
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
        public string Certificateimg { get; set; }
        public string Certificateimgbase64 { get; set; }
        [Required(ErrorMessage = "Licence Number Required")]
        public string LicenceNumber { get; set; }
        [Required]


        public string Password { get; set; }
        [System.Web.Mvc.Compare("Password")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "State Field Can't be null")]
        public int StateMaster_Id { get; set; }
        [Required(ErrorMessage = "City cant be null")]
        public int CityMaster_Id { get; set; }
        [Required(ErrorMessage = "Location Field Can't be null")]
        public string Location { get; set; }
        public string GSTNumber { get; set; }
        public int AdminLogin_Id { get; set; }



        public Nullable<System.DateTime> LicenseValidity { get; set; }
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


    public class fra_DoctorReg
    {

        public string PinCode { get; set; }

        public string DoctorName { get; set; }

        public string PhoneNumber { get; set; }
        //[Required]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string MobileNumber { get; set; }
        //[Required]
        public string EmailId { get; set; }
        //[Required]
        public int StateMaster_Id { get; set; }

        //[Required]
        public int CityMaster_Id { get; set; }
        public int Vendor_Id { get; set; }
        //[Required]
        public string Location { get; set; }
        // public int Location_Id { get; set; }
        //[Required]
        public string LicenceNumber { get; set; }
        //[Required]
        public string ClinicName { get; set; }


        public string Password { get; set; }
        [Required, System.ComponentModel.DataAnnotations.Compare("Password")]
        public string ConfirmPassword { get; set; }

        //public string PAN { get; set; }

        //public string AadharNumber { get; set; }

        public string LicenceBase64 { get; set; }
        //[Required]
        public string LicenceImageName { get; set; }
        ///[Required]
        public string PAN { get; set; }
        public string PanImage { get; set; }
        public string PanImageBase64 { get; set; }
        public string LicenceImage { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string SlotTime { get; set; }
        public string SlotTime2 { get; set; }

        public TimeSpan? StartTime2 { get; set; }
        public TimeSpan? EndTime2 { get; set; }
        public string SlotTiming2 { get; set; }

        public DateTime? JoiningDate { get; set; }
        public Nullable<System.DateTime> LicenseValidity { get; set; }
        public Nullable<int> Experience { get; set; }
        public int Department_Id { get; set; }
        public int Specialist_Id { get; set; }
        public double Fee { get; set; }
        public string Qualification { get; set; }
        public string RegistrationNumber { get; set; }
        public string SignaturePic { get; set; }
        public string SignaturePicBase64 { get; set; }
        public string About { get; set; }
        public int Day_Id { get; set; }
        public int VirtualFee { get; set; }
    }

    public class fra_LabReg
    {

        public string LabName { get; set; }
        public string PhoneNumber { get; set; }
       
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int StateMaster_Id { get; set; }
        public int CityMaster_Id { get; set; }
        public string Location { get; set; }
        public string LicenceNumber { get; set; }
        public string LicenceImage { get; set; }
        public string LicenceImagebase64 { get; set; }

        public string PAN { get; set; }
        public string PinCode { get; set; }
        public int Vendor_Id { get; set; }
        public int AdminLogin_Id { get; set; }

        public string GSTNumber { get; set; }

        public string AadharNumber { get; set; }
        public string PanImage { get; set; }
        public string PanImagebase64 { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
        public string lABId { get; set; }

    }

    public class fra_PatientReg
    {
        public string PatientName { get; set; }

        [Required]
        [MobileNumberValidation]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile Number must be numeric")]
        public string MobileNumber { get; set; }

        // [Required]
        //[FormValidations]
        public string EmailId { get; set; }
        public int AdminLogin_Id { get; set; }
        public DateTime? Reg_Date { get; set; }
        public string PatientRegNo { get; set; }
        //public string CityName { get; set; }
        //  public string OtherLocationName { get; set; }
        //[Required]
        //[RegularExpression(@"^(\d{6,6})$", ErrorMessage = "6 Digits Required")]
        public string PinCode { get; set; }

        //[Required(ErrorMessage = "State Can't Be Empty")]
        public Nullable<int> StateMaster_Id { get; set; }

        public Nullable<int> CityMaster_Id { get; set; }

        public string Location { get; set; }
        [Required]
        [DataType(DataType.Password)]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        //[Display(Name = "Password")]
        //[RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string Password { get; set; }
        //[Required,Compare("Password")]
        public string ConfirmPassword { get; set; }
        //[Required(ErrorMessage = "Confirmation Password is required.")]
        //[Compare("Password", ErrorMessage = "Password and Confirmation Password must match.")]

        public Nullable<int> vendorId { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
    }

    public class fra_DriverReg
    {

        //[Required]
        [MaxLength(6)]
        [MinLength(6)]
        //[RegularExpression("[^0-9]", ErrorMessage = "Pincode must be numeric")]
        public string PinCode { get; set; }

        [Required]
        public string DriverName { get; set; }
        
        public string MobileNumber { get; set; }

        public string EmailId { get; set; }
       
        public int StateMaster_Id { get; set; }
       
        public int CityMaster_Id { get; set; }
        
        public string Location { get; set; }

        public Nullable<System.DateTime> DlValidity { get; set; }
        public string DlNumber { get; set; }
        public string DlImage1 { get; set; }
        public string DlImage1Base64 { get; set; }
        public string DlImage2 { get; set; }

        public string DlImage2Base64 { get; set; }

         
        public string Password { get; set; }
       
      
        public string ConfirmPassword { get; set; }

        public string AadharImage { get; set; }
        public string AadharImageBase64Image { get; set; }
        public string AadharImage2 { get; set; }
        public string AadharImage64Image1 { get; set; }
        public string PAN { get; set; }
        public int Vendor_Id { get; set; }
		public string Paidamount { get; set; }
        public string DriverImage { get; set; }
        public string DriverImageBase64 { get; set; }

    }

    public class fra_NurseReg
    {
        public int? NurseType_Id { get; set; }
        public string CertificateNumber { get; set; }
        public string CityName { get; set; }
        [Required]
        public string PinCode { get; set; }
        [Required]
        public string NurseName { get; set; }
        public string PAN { get; set; }
       
        public string MobileNumber { get; set; }
        [Required]
        public string EmailId { get; set; }
        [Required]
        public int StateMaster_Id { get; set; }

        public int CityMaster_Id { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public double Fee { get; set; }
        [Required]
        public string Password { get; set; }
        [Required, System.ComponentModel.DataAnnotations.Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string CertificateBase64Image { get; set; }
        //public bool IsVerifiedByPolice { get; set; }
        public Nullable<int> experience { get; set; }
        public string CertificateImage { get; set; }
        public int Vendor_Id { get; set; }
        public int Location_id { get; set; }


    }

    public class fra_VehicleReg
    {
        public string VehicleName { get; set; }
        public string VehicleOwnerName { get; set; }
        public string VehicleNumber { get; set; }
        public string AccountNo { get; set; }
        public string ConfirmAccountNo { get; set; }
        public double? DriverCharges { get; set; }
        public string AccountHolderName { get; set; }
        public int VehicleCat_Id { get; set; }
        public int VehicleType_Id { get; set; }
        public string IFSCCode { get; set; }
        public string PAN { get; set; }
        public string CancelCheque { get; set; }
        public string CancelChequeBase64 { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> Vendor_Id { get; set; }
    }

    public class fra_ChemistregDetail
    {
        public int Id { get; set; }
        public string ChemistId { get; set; }
        public string ChemistName { get; set; }
        public int? Vendor_Id { get; set; }
        public string ShopName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string GSTNumber { get; set; }
        public string LicenceNumber { get; set; }
        public bool IsApproved { get; set; }


    }

    public class ChemsitEditchregdetail
    {
        public int Id { get; set; }

        public string ChemistName { get; set; }

        public string ShopName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string GSTNumber { get; set; }
        public string LicenceNumber { get; set; }
    }

    public class FraLabRegDetail

    {
        public int Id { get; set; }
        public string lABId { get; set; }
        public string LabName { get; set; }
        public string Franchise { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string LicenceNumber { get; set; }

        public string GSTNumber { get; set; }

        public string AadharNumber { get; set; }
        public bool IsApproved { get; set; }

    }

    

    public class FraEditlabregdetail
    {
        public int Id { get; set; }
        public string LabName { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string LicenceNumber { get; set; }

        public string GSTNumber { get; set; }

        public string AadharNumber { get; set; }
    }

    public class FraDoctorRegDetail
    {
        public int Id { get; set; }
        public string DoctorId { get; set; }
        public string DoctorName { get; set; }
        public double? Amount { get; set; }
        public string Location { get; set; }
        public string VendorName { get; set; }
        public string DepartmentName { get; set; }
        public string SpecialistName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string LicenceNumber { get; set; }
    }

    public class FraDriverRegDetail
    {
        public int Id { get; set; }
        public string DriverId { get; set; }
        public string DriverName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string VehicleTypeName { get; set; }
        public string DlNumber { get; set; }

    }

    public class FraVehicleRegDetail
    {
        public int Id { get; set; }
        public string VehicleNumber { get; set; }
        public string VehicleOwnerName { get; set; }
        public string Franchise { get; set; }
        public string VehicleTypeName { get; set; }
        public string CategoryName { get; set; }
        public double? DriverCharges { get; set; }
    }

    public class FraNurseRegDetail
    {
        public int Id { get; set; }
        public string NurseId { get; set; }
        public string NurseName { get; set; }
        public string NurseTypeName { get; set; }
        public string VendorName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string CertificateNumber { get; set; }
        public bool IsApproved { get; set; }
    }


    public class FraEditNurseregdetail
    {
        public int Id { get; set; }
        public string NurseName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string CertificateNumber { get; set; }
    }
    public class FraPatientRegDetail
    {
        public int Id { get; set; }
        public string PatientRegNo { get; set; }
        public string PatientName { get; set; }
        public string VendorName { get; set; }
        public string MobileNumber { get; set; }
        public string Location { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public bool IsApproved { get; set; }
    }

    public class FraRWARegDetail
    {
        public int Id { get; set; }
        public string RWAId { get; set; }
        public string AuthorityName { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string CertificateNo { get; set; }
        public bool IsApproved { get; set; }
    }

    public class FranchiseEditProfile
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string VendorName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public int StateMaster_Id { get; set; }
        public int City_Id { get; set; }
        public string Location { get; set; }

        public string PinCode { get; set; }

    }

    public class FraEditRWAregdetail
    {
        public int Id { get; set; }
        public string AuthorityName { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string CertificateNo { get; set; }
    }

    public class Get_oldDriver
    {
        public int Id { get; set; }
        //public int? Driver_Id { get; set; }
        public string VehicleNumber { get; set; }
        public string DriverName { get; set; }

    }

    public class Update_oldDriver
    {
        public int Id { get; set; }
        public int? Driver_Id { get; set; }
        public string VehicleNumber { get; set; }
        public string DriverName { get; set; }

    }
    public class Dltolddriver
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class NewDriverList
    {
        public int Id { get; set; }
        public string DriverName { get; set; }
    }

    public class Fra_About
    {
        public int Id { get; set; }
        public string About { get; set; }
    }

    public class TDS_Dropdown
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ChemistReport_By_YMWD
    {
        public int Id { get; set; }
        public string ChemistId { get; set; }
        public string ChemistName { get; set; }
        public string Franchise { get; set; }
        public string ShopName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string GSTNumber { get; set; }
        public string LicenceNumber { get; set; }
        public bool IsApproved { get; set; }

    }

    public class LabReport_By_MYWD
    {
        public int Id { get; set; }
        public string lABId { get; set; }
        public string LabName { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string LicenceNumber { get; set; }
        public string GSTNumber { get; set; }
        public string AadharNumber { get; set; }
        public bool IsApproved { get; set; }
    }

    public class NurseReport_By_MYWD
    {
        public int Id { get; set; }
        public string NurseId { get; set; }
        public string NurseName { get; set; }
        public string NurseTypeName { get; set; }
        public string Franchise { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string CertificateNumber { get; set; }
        public bool IsApproved { get; set; }
    }

    public class DoctorReport_By_MYWD
    {
        public int Id { get; set; }
        public string DoctorId { get; set; }
        public string DoctorName { get; set; }
        public double Fee { get; set; }
        public string Location { get; set; }
        public string DepartmentName { get; set; }
        public string SpecialistName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailId { get; set; }
        public string LicenceNumber { get; set; }


    }

    public class VehicleReport_By_MYWD
    {
        public int Id { get; set; }
        public string VehicleNumber { get; set; }
        public string VehicleOwnerName { get; set; }
        public string Franchise { get; set; }
        public string Type { get; set; }
        public string CategoryName { get; set; }
        public double? DriverCharges { get; set; }

    }

    public class Total_TDSAmount
    {
        public double? TotalTDSAmount { get; set; }
    }
    public class Total_CommissionAmount
    {
        public double TotalCommissionAmount { get; set; }
    }

}