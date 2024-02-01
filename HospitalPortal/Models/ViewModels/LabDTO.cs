using HospitalPortal.Models.CommonClasses;
using HospitalPortal.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class LabDTO:StateCityAbs
    {
        public bool? IsCheckedTermsCondition { get; set; }
        public string lABId { get; set; }
        public string UniqueId { get; set; }
        public string CompanyName { get; set; }
        public string VendorName { get; set; }
        public int? Vendor_Id { get; set; }
        public int Id { get; set; }
        public string RefId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }
       // [Required(ErrorMessage = "Lab Name Can't Be Empty")]
        public string LabName { get; set; }
     
        public string PhoneNumber { get; set; }
        //[Required]
        //[MobileNumberValidation]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Mobile Number must be numeric")]
        public string MobileNumber { get; set; }
        [Required]
        [FormValidations]
        public string EmailId { get; set; }
        [Required]
        //[DataType(DataType.Password)]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        //[Display(Name = "Password")]
        //[RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string Password { get; set; }
        //[Compare("Password")]
        public string ConfirmPassword { get; set; }
        
        public string LicenceImage { get; set; }
        //[Required(ErrorMessage = "Licence Number Can't Be Empty")]
        public HttpPostedFileBase LicenceImageFile { get; set; }
        
        public string LicenceImagebase64 { get; set; }
        public string LicenceNumber { get; set; }
        public string PAN { get; set; }
        //[Required(ErrorMessage = "State Can't Be Empty")]
        public int StateMaster_Id { get; set; }
        //[Required(ErrorMessage = "City Can't Be Empty")]
        public int CityMaster_Id { get; set; }
        //[Required(ErrorMessage = "Address Can't Be Empty")]
        public string Location { get; set; }
       // [Required]
        //[RegularExpression(@"^(\d{6,6})$", ErrorMessage = "6 Digits Required")]
        public string PinCode { get; set; }
       // [Required]
        public int? Location_Id { get; set; }
        public int AdminLogin_Id { get; set; }
       // [Required(ErrorMessage = "Licence Image File Can't Be Empty")]
       // public HttpPostedFileBase LicenceImageFile { get; set; }
        //[Required(ErrorMessage = "PAN Number Can't Be Empty")]
        //public string PAN { get; set; }
        //[Required(ErrorMessage = "GST Number Can't Be Empty")]
        public string GSTNumber { get; set; }
        public string PanImage { get; set; }
        public HttpPostedFileBase PanImageFile { get; set; }
        public string PanImagebase64 { get; set; }
        public string AadharNumber { get; set; }
        public HttpPostedFileBase AadharImageFile { get; set; }
        //public HttpPostedFileBase PanImageFile { get; set; }
        // [Required(ErrorMessage = "Start Time Can't Be Empty")]
        public TimeSpan StartTime { get; set; }
       // [Required(ErrorMessage = "End Time Can't Be Empty")]
        public TimeSpan EndTime { get; set; }
        public string OtherCity { get; set; }
        public string OtherLocation { get; set; }
       public string About { get; set; }
        public string LabTypeName { get; set; }

         Nullable<System.DateTime> ChooseDate { get; set; }
        public string year { get; set; }
    }


    public class bookdetail
    {
        public int id { get; set; }
        public string LabName { get; set; }
        public string Location { get; set; }
        public string LicenceImage { get; set; }
        public string About { get; set; }
        public string LabTypeName { get; set; }
        public string year { get; set; }
        public Nullable<System.DateTime> ChooseDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class LabBookings
    {
        public int id { get; set; }
        public string LabName { get; set; }
        public string MobileNumber { get; set; }
       
        public string OpeningHours { get; set; }

        public string Location { get; set; }
        public Nullable<double> Fee { get; set; }
        public string WorkingDay { get; set; }

        public int? Rating { get; set; }
    }
    public class LabBooked
    {
        public int StateMaster_Id { get; set; }
        public int CityMaster_Id { get; set; }
        public int testId { get; set; }
    }
    public class Booknow
    {
        public int Id { get; set; }
        public Nullable<int> Lab_Id { get; set; }

        public Nullable<int> Slotid { get; set; }
        public Nullable<System.DateTime> TestDate { get; set; }
        public Nullable<int> Patient_Id { get; set; }
    }
    public class bookingResponse
    {
        public int BookingId { get; set; }
        public string Message { get; set; }
    }

    public class doctorBooknow
    {
        public int Id { get; set; }
        public Nullable<int> Doctor_Id { get; set; }

        public Nullable<int> Slot_id { get; set; }
        public Nullable<System.DateTime> AppointmentDate { get; set; }
        public Nullable<int> BookingMode_Id { get; set; }

    }


    //public class Labregistration 
    //{
    //    public bool? IsCheckedTermsCondition { get; set; }
    //    public string lABId { get; set; }
    //    public string UniqueId { get; set; }
    //      public int Id { get; set; }
       
    //    public bool IsDeleted { get; set; }
    //    public bool IsApproved { get; set; }
    //    //[Required(ErrorMessage = "Lab Name Can't Be Empty")]
    //    public string LabName { get; set; }

    //    public string PhoneNumber { get; set; }
    //    //[Required]
    //    //[MobileNumberValidation]
    //    //[RegularExpression("^[0-9]*$", ErrorMessage = "Mobile Number must be numeric")]
    //    public string MobileNumber { get; set; }
    //    [Required]
    //    [FormValidations]
    //    public string EmailId { get; set; }
        
    //    public string Password { get; set; }
    //    //[Compare("Password")]
    //    public string ConfirmPassword { get; set; }

    //    public string LicenceImage { get; set; }
    //    //[Required(ErrorMessage = "Licence Number Can't Be Empty")]
    //    public string LicenceNumber { get; set; }
    //    //[Required(ErrorMessage = "State Can't Be Empty")]
    //    public int StateMaster_Id { get; set; }
    //    //[Required(ErrorMessage = "City Can't Be Empty")]
    //    public int CityMaster_Id { get; set; }
    //    //[Required(ErrorMessage = "Address Can't Be Empty")]
    //    public string Location { get; set; }
    //    //[Required]
    //    //[RegularExpression(@"^(\d{6,6})$", ErrorMessage = "6 Digits Required")]
    //    public string PinCode { get; set; }
     
    //   // public int Location_Id { get; set; }
    //    public int AdminLogin_Id { get; set; }
        

    //    public string LicenceImagebase64 { get; set; }

       
    //    //[Required(ErrorMessage = "GST Number Can't Be Empty")]
    //    public string GSTNumber { get; set; }

    //    public string AadharNumber { get; set; }
    //    public string PanImage { get; set; }
    //    public string PanImagebase64 { get; set; }

    //   // [Required(ErrorMessage = "Start Time Can't Be Empty")]
    //    public TimeSpan StartTime { get; set; }
    //    //[Required(ErrorMessage = "End Time Can't Be Empty")]
    //    public TimeSpan EndTime { get; set; }
    //  //  public string OtherCity { get; set; }
    //   // public string OtherLocation { get; set; }
    //    public string year { get; set; }
    //}


    public class LabREgis
    {
        public int ID { get; set; }
        public string PAN { get; set; }
        public string LabName { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int StateMaster_Id { get; set; }
        public int CityMaster_Id { get; set; }
        public string Location { get; set; }
        public string LicenceNumber { get; set; }
        public string LicenceImage { get; set; }
        public string LicenceImagebase64 { get; set; }

        public string PinCode { get; set; }
        public int AdminLogin_Id { get; set; }

        public string GSTNumber { get; set; }

        public string AadharNumber { get; set; }
        public string PanImage { get; set; }
        public string PanImagebase64 { get; set; }

        public TimeSpan StartTime { get; set; }
       
        public TimeSpan EndTime { get; set; }
        public string lABId { get; set; }

    }





    public class lab_about
    {
        public string About { get; set; }
    }


}