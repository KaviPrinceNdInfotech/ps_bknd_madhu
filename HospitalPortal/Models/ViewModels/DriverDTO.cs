using HospitalPortal.Models.CommonClasses;
using HospitalPortal.Models.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class DriverDTO: StateCityAbs
    {
        public bool? IsCheckedTermsCondition { get; set; }
        public string UniqueId { get; set; }
        public string DriverId { get; set; }
        public int? Page { get; set; }
        public string RefId { get; set; }
        public int? NumberOfPages { get; set; }
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        public string DriverName { get; set; }
        public string Paidamount { get; set; }
        [Required]
        [MobileNumberValidation]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile Number must be numeric")]
        public string MobileNumber { get; set; }
        //[Required]
        //[FormValidations]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "State Can't Be Empty")]
        public int StateMaster_Id { get; set; }
        [Required(ErrorMessage = "City Can't Be Empty")]
        public int CityMaster_Id { get; set; }
        [Required(ErrorMessage = "Address Can't Be Empty")]
        public string Location { get; set; }
        public string DriverImage { get; set; }
        public string DlImage { get; set; }
        public string DlImage1 { get; set; }
        public string DlImage2 { get; set; }
        public string DlImage3 { get; set; }
        [Required]
        public string DlNumber { get; set; }
        public string PAN { get; set; }
        public Nullable<System.DateTime> DlValidity { get; set; }
        
        [Required]
        
        public string Password { get; set; }
        [Required]
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        public string ConfirmPassword { get; set; }
        public int AdminLogin_Id { get; set; }
        public bool IsApproved { get; set; }
        public bool IsBankUpdateApproved { get; set; } 
        [Required(ErrorMessage = "Driving Licence Can't Be Empty")]
        public HttpPostedFileBase DlFile { get; set; }
        [Required(ErrorMessage = "Driving Licence Can't Be Empty")]
        public HttpPostedFileBase DlFile1 { get; set; } 
        public string AadharNumber { get; set; }
        public string AadharImage { get; set; }
        public HttpPostedFileBase AadharImageFile { get; set; }
        public string AadharImage2 { get; set; }
        public HttpPostedFileBase AadharImageFile2 { get; set; }
        public Nullable<int> Vendor_Id { get; set; }
        public string PanImage { get; set; } 
        public HttpPostedFileBase PanImageFile { get; set; }
        public HttpPostedFileBase DriverImageFile { get; set; }
        public HttpPostedFileBase VerificationImage { get; set; }
        public bool IsVerifiedByPolice { get; set; }
        public string VerificationDoc { get; set; }
        public int? VehicleType_Id { get; set; }
        public SelectList VehicleList { get; set; }
        public string VehicleTypeName { get; set; }
        public string CategoryName { get; set; }
        [Required]
        [RegularExpression(@"^(\d{6,6})$", ErrorMessage = "6 Digits Required")]
        public string PinCode { get; set; }
        public string OtherCity { get; set; }

       
    }


    public class DriverRequestParameter
    {
        public string CityName { get; set; }
        
        
        public string PinCode { get; set; }
        public int? Id { get; set; }
        public int Vendor_Id { get; set; }
         
        public string DriverName { get; set; }
        public string PAN { get; set; }
         
        public string MobileNumber { get; set; }

        public string EmailId { get; set; }
        
        public int StateMaster_Id { get; set; }
       
        public int CityMaster_Id { get; set; }
       
        public string Location { get; set; }
        public string DriverImage { get; set; }
        public string DriverImageBase64 { get; set; }
        
        public string DlImage1 { get; set; }
        public string DlImage1Base64 { get; set; }
        public string DlImage2 { get; set; }
        public string DlImage2Base64 { get; set; }
        public string DlImage3 { get; set; }
      
        public string DlNumber { get; set; }
        
        public System.DateTime DlValidity { get; set; }
        
        public string Password { get; set; }
        
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        public string ConfirmPassword { get; set; }
          
        public string AadharImage { get; set; }
        public string AadharImageBase64 { get; set; }
        public string AadharImage2 { get; set; }
        public string AadharImage2Base64 { get; set; }
        
        public string VerificationDoc { get; set; }
        public int VehicleType_Id { get; set; } 

        public string State { get; set; }
        public string City { get; set; }
        public string Paidamount { get; set; }
        public string Status { get; set; }
        public string BranchName { get; set; }

    }
    public class DriverUpdateProfile
    {
        public int? Id { get; set; }
        public string DriverName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string VehicleName { get; set; }
        public int StateMaster_Id { get; set; }
       
        public int CityMaster_Id { get; set; }
       
        public string Location { get; set; }
        public string PinCode { get; set; }
        
        public string DlNumber { get; set; }
        public string DlImage { get; set; }
        public string DlImageName { get; set; }
        public string DlBase64Image { get; set; }
    }



    public class AppoinmentDetails
    {
        public int Id { get; set; }
        public string DriverName { get; set; }
        public string VehicleName { get; set; }
        public string Location { get; set; }
       
        public string DlImage { get; set;}
        public DateTime? JoiningDate { get;set; }

    }

    public class BookingOrderHistory
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string MobileNumber { get; set;}
        public string StateName { get; set;}
        public string CityName { get; set; }
        public string Location { get; set; }
        public string PinCode { get; set; }

        public double end_Lat { get; set; }
        public double end_Long { get; set; }
        public double start_Lat { get; set; }
        public double start_Long { get; set; }
        //CODE FOR LAT LONG TO LOCATION 
        public string PickUpLoaction
        {
            get { return getlocation(start_Lat.ToString(), start_Long.ToString()); }
        }
        public string DropLocation
        {
            get { return getlocation(end_Lat.ToString(), end_Long.ToString()); }
        }
         
        private string getlocation(string latitude, string longitude)
        {

            string url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + latitude + "," + longitude + "&key=AIzaSyBrbWFXlOYpaq51wteSyFS2UjdMPOWBlQw";

            // Make the HTTP request.
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 10000;

            // Get the response.
            WebResponse response = request.GetResponse();
            string responseText = new StreamReader(response.GetResponseStream()).ReadToEnd();

            // Parse the response JSON.
            var json = JsonConvert.DeserializeObject<dynamic>(responseText);

            // Get the location from the JSON.
            var location = json.results[0].formatted_address;
            return location;
        }

        //END CODE FOR LAT LONG TO LOCATION 
    }

    public class payhistory
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string MobileNumber { get; set; }
        public string Location { get; set; }
        public int PaymentId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string IsPay { get; set; }
        public double end_Lat { get; set; }
        public double end_Long { get; set; }
        public double start_Lat { get; set; }
        public double start_Long { get; set; }
        //CODE FOR LAT LONG TO LOCATION 
        public string PickUpLoaction
        {
            get { return getlocation(start_Lat.ToString(), start_Long.ToString()); }
        }
        public string DropLocation
        {
            get { return getlocation(end_Lat.ToString(), end_Long.ToString()); }
        }

        private string getlocation(string latitude, string longitude)
        {

            string url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + latitude + "," + longitude + "&key=AIzaSyBrbWFXlOYpaq51wteSyFS2UjdMPOWBlQw";

            // Make the HTTP request.
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 10000;

            // Get the response.
            WebResponse response = request.GetResponse();
            string responseText = new StreamReader(response.GetResponseStream()).ReadToEnd();

            // Parse the response JSON.
            var json = JsonConvert.DeserializeObject<dynamic>(responseText);

            // Get the location from the JSON.
            var location = json.results[0].formatted_address;
            return location;
        }

        //END CODE FOR LAT LONG TO LOCATION 
    }
    public class payouthistory
    {
        public int Id { get; set; }
        public Nullable<decimal> Amount { get; set; } 
        public Nullable<System.DateTime> PaymentDate { get; set; } 
    }
    public class Vehicle_type
    {
        public int Id { get; set; }
        public string VehicleTypeName { get; set; }
        
    }
    public class UserdetailOngoingdriver
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string MobileNumber { get; set; }
        public Nullable<int> TotalPrice { get; set; }
        public Nullable<double> Lat_Driver { get; set; }
        public Nullable<double> Lang_Driver { get; set; }
        public Nullable<double> start_Lat { get; set; }
        public Nullable<double> start_Long { get; set; }
        public Nullable<double> end_Lat { get; set; }
        public Nullable<double> end_Long { get; set; }
        public int DriverUserDistance { get; set; }
        public int ExpectedTime { get; set; }
        public Nullable<int> TotalDistance { get; set; }
        public string DeviceId { get; set; }
        //CODE FOR LAT LONG TO LOCATION 
        public string PickupLocation
        {
            get { return getlocation(start_Lat.ToString(), start_Long.ToString()); }
        }
        public string DropLocation
        {
            get { return getlocation(end_Lat.ToString(), end_Long.ToString()); }
        }

        private string getlocation(string latitude, string longitude)
        {
            string url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + latitude + "," + longitude + "&key=AIzaSyBrbWFXlOYpaq51wteSyFS2UjdMPOWBlQw";

            // Make the HTTP request.
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 10000;

            // Get the response.
            WebResponse response = request.GetResponse();
            string responseText = new StreamReader(response.GetResponseStream()).ReadToEnd();

            // Parse the response JSON.
            var json = JsonConvert.DeserializeObject<dynamic>(responseText);

            // Get the location from the JSON.
            var location = json.results[0].formatted_address;
            return location;
        }

        //END CODE FOR LAT LONG TO LOCATION 

    }

}