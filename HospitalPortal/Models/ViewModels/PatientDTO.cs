using HospitalPortal.Models.CommonClasses;
using HospitalPortal.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class PatientDTO : StateCityAbs
    {
        public bool? IsCheckedTermsCondition { get; set; }
        public int AdminLogin_Id { get; set; }
        public DateTime? Reg_Date { get; set; }
        public string PatientRegNo { get; set; }
        //public string CityName { get; set; }
      //  public string OtherLocationName { get; set; }
        //[Required]
        //[RegularExpression(@"^(\d{6,6})$", ErrorMessage = "6 Digits Required")]
        public string PinCode { get; set; }
        public int Id { get; set; }
        [Required]
        public string PatientName { get; set; }
       // [Required]
        //[FormValidations]
        public string EmailId { get; set; }
        [Required]
        [MobileNumberValidation]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile Number must be numeric")]
        public string MobileNumber { get; set; }
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
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }
        public Nullable<int> Rwa_Id { get; set; }

        public Nullable<int> vendorId { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public IEnumerable<PatientList> Patient { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
    }

    public class PatientUpdateReq
    {
        public int Id { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public int StateMaster_Id { get; set; }
        [Required]
        public int CityMaster_Id { get; set; }
        [Required]
        public string Location { get; set; }
        public string PinCode { get; set; }
    }
    public class PatientUpdate
    {
        public int Id { get; set; }

        public string patientName { get; set; }
        public string MobileNumber { get; set; }

        //public string StateName { get; set; }
        //public string CityName { get; set; }
        public int StateMaster_Id { get; set; }
        public int CityMaster_Id { get; set; }
        public string Location { get; set; }
        public string PinCode { get; set; }

        //public int adminLogin_id { get; set; }
        //public string AccountNo { get; set; }
        //public string IFSCCode { get; set; }
        //public string BranchName { get; set; }

    }

    public partial class LabReportUploadPatient
    {
        public Nullable<int> Lab_Id { get; set; }
        public Nullable<int> Patient_Id { get; set; }
        public string File { get; set; }
        public string FileBase64 { get; set; }
        public Nullable<System.DateTime> UploadDate { get; set; }
    }

    public class LabViewReport_ByPatient
    {
        public int Id { get; set; }
        public string LabName { get; set; }
        public string TestName { get; set; }
        public Nullable<System.DateTime> TestDate { get; set; }
        public string File { get; set; }

    }

    public class DoctorViewReport_bypatient
    {
        public int Id { get; set; }
        public string DoctorName { get; set; }
        public string Image1 { get; set; }
    }

    public class Nurse_View_ReportBypatient
    {
        public int Id { get; set; }
        public string NurseName { get; set; }
        public string File { get; set; }

    }

    public class DriverList_ByLatLong
    {
        public int Id { get; set; }
        public string DriverName { get; set; }
        public string DriverImage { get; set; }
        public string AadharNumber { get; set; }
        public string DlNumber { get; set; }
        public string MobileNumber { get; set; }
    }

    public class DriverDetail
    {
        public int Id { get; set; }
        public string DriverName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }       
        public string DriverImage { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string Location { get; set; }
        public string DlNumber { get; set; }
        
    }
    public class NurseCancelApp    {
        public int Id { get; set; }
        public int Patient_Id { get; set; }
       

    }
    public class CancelAppointent
    {
        public int Id { get; set; }
        public Nullable<int> Pro_Id { get; set; }
        public Nullable<int> Patient_Id { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> UserWalletAmount { get; set; }
        public Nullable<System.DateTime> CancelDate { get; set; }


    }
}