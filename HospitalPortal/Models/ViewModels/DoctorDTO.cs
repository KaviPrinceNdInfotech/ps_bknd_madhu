using HospitalPortal.Models.CommonClasses;
using HospitalPortal.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class DoctorDTO:StateCityAbs
    {
        public bool? IsCheckedTermsCondition { get; set; }
        public string DoctorId { get; set; }
        public string UniqueId { get; set; }
        public IEnumerable<DepartmentModelClass> DeptList { get; set; }
        public int? Page { get; set; }
        public int? NumberOfPages { get; set; }        
        public string PinCode { get; set; }
        public DateTime? JoiningDate { get; set; }
        public int? Id { get; set; }
        public bool IsDeleted { get; set; }
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        
        public string DoctorName { get; set; }   
        public string PhoneNumber { get; set; }
        
        public string MobileNumber { get; set; }
        
        public string EmailId { get; set; }
        
        public int StateMaster_Id { get; set; }
        
        public int CityMaster_Id { get; set; } 
        public string Location { get; set; }
        public string Qualification { get; set; }
        public string RegistrationNumber { get; set; }
        public string SignaturePic { get; set; }
        public HttpPostedFileBase SignatureImageFile { get; set; }
        public string LicenceImage { get; set; }
        
        public string LicenceNumber { get; set; }
        public string PAN { get; set; }
         
        public string ClinicName { get; set; }
        
        public int? Department_Id { get; set; }   
        
        public int? Specialist_Id { get; set; }
        
        public double Fee { get; set; }
        public double? VirtualFee { get; set; }   
        public string Password { get; set; }
        
        public string ConfirmPassword { get; set; }
        public bool IsApproved { get; set; }
        public bool IsBankUpdateApproved { get; set; }
        public int? AdminLogin_Id { get; set; }
       
        public SelectList DepartmentList { get; set; }
        public SelectList SpecialistList { get; set; }
        public SelectList VendorList { get; set; }
        public SelectList DayList { get; set; }
        public SelectList DurationTimeList { get; set; }
         
       
        public HttpPostedFileBase LicenceImageFile { get; set; }
        public string DepartmentName { get; set; }
        public string SpecialistName { get; set; }
        public int? Vendor_Id { get; set; }
        public int? HospitalId { get; set; }
        public string LicenceBase64 { get; set; }
        public string LicenceImageName { get; set; }
        public string AadharBase64 { get; set; }
        public string AadharImageName { get; set; }
        
        public TimeSpan? StartTime { get; set; }
        
        public TimeSpan? EndTime { get; set; }
        
        public string SlotTiming { get; set; }
         

        public TimeSpan? StartTime2 { get; set; }
        public TimeSpan? EndTime2 { get; set; }
        public string SlotTiming2 { get; set; }
        public DateTime? LicenseValidity { get; set; }
        public string OtherCity { get; set; }

        public string RefId { get; set; }
        public int? Day_Id { get; set; }

        public int? Experience { get; set; }  

        public string About { get; set; }    

        public string Disease { get; set; }  
        public bool Status { get; set; }
        public string Message { get; set; }
        public int? SlotTime { get; set; }
        public int? SlotTime2 { get; set; }
    }
     
    public class DepModel {
        public int Id { get; set; }
        [Required]
        public int Department_Id { get; set; }
        [Required]
        public int Specialist_Id { get; set; }
        
    }

    public class DModel
    {
        public int Id { get; set; }
        
        public int Department_Id { get; set; }
        [Required]
        public int Specialist_Id { get; set; }
        [Required]
        public int StateMaster_Id { get; set; }
        [Required]
        public int CityMaster_Id { get; set; }

         public int Pro_Id { get; set; }
        public string Professional { get; set; }    
        public Nullable  <int>  Rating { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string ImageBase { get; set; }




    }



    public class DModelRating
    {
       
        public int? Pro_Id { get; set; }
        public int? Patient_Id { get; set; }
        public string Professional { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int?  Rating { get; set; }
       
    }



    public class DModelRatings
    {

        public int Pro_Id { get; set; }
        public int Patient_Id { get; set; }
        public string Professional { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Rating { get; set; }

    }


    //public class Dmodeltotalrating
    //{
    //    public int Id { get; set; }
    //    public int Rating { get; set; }

    //}

    public class Doctorchoose
    {
        public int  Id { get; set; }
        public string DoctorName { get; set; }
        public string DepartmentName { get; set; }
        public double Fee { get; set; } 
        public int Experience { get; set; }
        public int? Rating { get; set; }
        public string About { get; set; }

    }

    public class DoctorAptmt
    {
        public int Id { get; set; }

        public string DoctorName { get; set; }
        public int GST { get; set; }
        public string SpecialistName { get; set; }
        public Nullable<int> Experience { get; set; }
        public double Fee { get; set; }
        public Nullable<double> TotalFee { get; set; }
        public Nullable<System.DateTime> AppointmentDate { get; set; }
        public TimeSpan  SlotTime { get; set; }
        public string DeviceId { get; set; }
        


    }
    
    public class DepartmentModelClass
    {
        public int Id { get; set; }

        [Required]
        public int Department_Id { get; set; }
        [Required]
        public int Specialist_Id { get; set; }
        public string DepartmentName { get; set; }
        public string SpecialistName { get; set; }
       
    }
    public class Doctorslot 
    {
        public int Id { get; set; }

        public string SlotTime { get; set; }

       
    }

    public class DoctorRegistrationRequest
    {
        public List<DepModel> Departments {get;set;}
        //public string CityName { get; set; }
       // public string OtherLocationName { get; set; }
        //[Required]
        public string PinCode { get; set; }
        public int ? Id { get; set; }
        //[Required]
        public string DoctorName { get; set; }
        
        public string  PhoneNumber { get; set; }
        //[Required]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string MobileNumber { get; set; }
        //[Required]
        public string EmailId { get; set; }
        //[Required]
        public int StateMaster_Id { get; set; }
        
        //[Required]
        public int CityMaster_Id { get; set; }
        //[Required]
        public string Location { get; set; }
       // public int Location_Id { get; set; }
        //[Required]
        public string LicenceNumber { get; set; }
        public string PAN { get; set; }
        //[Required]
        public string ClinicName { get; set; }
        
        public int Department_Id { get; set; }
     
        public int Specialist_Id { get; set; }
        //[Required]
        public double Fee { get; set; }
        //[Required]
        public string Password { get; set; }
        [Required, System.ComponentModel.DataAnnotations.Compare("Password")]
        public string ConfirmPassword { get; set; }
   
        //public string PAN { get; set; }
      
        //public string AadharNumber { get; set; }
        
       public string LicenceBase64 { get; set; }
       //[Required]
        public string LicenceImageName { get; set; }
        ///[Required]
        //public string AadharBase64 { get; set; }
        //[Required]
        //public string AadharImageName { get; set; }
        //[Required]
        //public string AadharBase641 { get; set; }
        //[Required]
        //public string AadharImageName1 { get; set; }

        //public string PanImageName { get; set; }
        //[Required]
        public string PanImage { get; set; }
        public string PanImageBase64 { get; set; }
        public string LicenceImage { get; set; }
        //public string AadharImage { get; set; }
        //public string AadharImage2 { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string SlotTiming { get; set; }
        public string Disease { get; set; }
        public TimeSpan? StartTime2 { get; set; }
        public TimeSpan? EndTime2 { get; set; }
        public string SlotTiming2 { get; set; }

        public DateTime? JoiningDate { get; set; }
        public string Qualification { get; set; }
        public string RegistrationNumber { get; set; }
        public string SignaturePic { get; set; }
        public string SignaturePicBase64 { get; set; }
        public int? Experience { get; set; }
        public int? Day_Id { get; set; }
        public double VirtualFee { get; set; }
        public Nullable<System.DateTime> LicenseValidity { get; set; } 
        public string About { get; set; } 
        public int Vendor_Id { get; set; } 
    }


    public class DoctorUpdationRequest
    {
        public int Id { get; set; }
        public string DoctorName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
          
        public int StateMaster_Id { get; set; }  
        public int CityMaster_Id { get; set; } 
        public string Location { get; set; } 
        public string PinCode { get; set; } 
        public string ClinicName { get; set; } 
        public double Fee { get; set; } 
    }
    public class DoctorDetail
    {
        public int Id { get; set; }

        public string DoctorName { get; set; }
       
        public Nullable<int> Experience { get; set; }
        public double? Fee { get; set; }
        public string About { get; set; }
        public string DepartmentName { get; set; }

        public int? Rating { get; set; } 
     
    }

    public class RWA_Registration
    {
        public int ID { get; set; }
        public string AuthorityName { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int StateMaster_Id { get; set; }
        public int CityMaster_Id { get; set; }
        public string Location { get; set; }
        public string CertificateNo { get; set; }
        public string CertificateImage { get; set; }
        public string CertificateImagebase64 { get; set; }
        public string Pincode { get; set; }
        public string LandlineNumber { get; set; }
        public string PAN { get; set; }
        public int Vendor_Id { get; set; }
    }

    public class PatientData
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string MobileNumber { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string Location { get; set; }
        public string Pincode { get; set; }
        public string EmailId { get; set; }
    }


    public class RWA_ProfileDetails
    {
        public int Id { get; set; }
        public string AuthorityName { get; set; }
        public string EmailId { get; set; }
        public string PhoneNumber { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string Location { get; set; }
        public string Pincode { get; set; }
        public int StateMaster_Id { get; set; }
        public int CityMaster_Id { get; set; }
    }
    public class RWAComplaint
    {
        public int Id { get; set; }
        public int RWA_Id { get; set; }
        public string Subjects { get; set; }
        public string Complaints { get; set; }
        public Boolean IsDeleted { get; set; }
        public Boolean IsResolved { get; set; }
        public string Others { get; set; }
    }

    public class GetRWA_Payout
    {
        public int Id { get; set; }
        public string AuthorityName { get; set; }
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }

    public class getRating
    {
        public int Id { get; set; }
        public Nullable<int> Rating1 { get; set; }
        public Nullable<int> Rating2 { get; set; }
        public Nullable<int> Rating3 { get; set; }
        public Nullable<int> Rating4 { get; set; }
        public Nullable<int> Rating5 { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string ImageBase { get; set; }
        public Nullable<int> Patient_Id { get; set; }
        public Nullable<int> pro_Id { get; set; }

        public string Professional { get; set; }
    }
    public class RWAAbout
    {
        public int Id { get; set; }
        public string About { get; set; }
    }
    public class UserAbout
    {
        public int Id { get; set; }
        public string About { get; set; }
    }

    public class BookingMode
    {
        public int Id { get; set; }
        public string Mode { get; set; }
    }

}