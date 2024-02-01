using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models
{
    public class driverlogin
    {

        public int Id { get; set; }
      
        public string DriverName { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        [Required]
        public int StateMaster_Id { get; set; }
        [Required]
        public int CityMaster_Id { get; set; }
        [Required]
        public string Location { get; set; }
       //public string DriverImage { get; set; }
        public string Password { get; set; }
        public System.DateTime DlValidity { get; set; }
        public string DlNumber { get; set; }
        [Required]
        public int AdminLogin_Id { get; set; }
        //public bool IsApproved { get; set; }
        public string PAN { get; set; }
        public int? VehicleType_Id { get; set; }
        public string VehicleTypeName { get; set; }
        [Required]
        public Nullable<double> BasePrice { get; set; }
        [Required]
        public Nullable<int> AsPerKM { get; set; }
        [Required]
        public Nullable<int> IfExceeds100 { get; set; }
        public string VehicleNumber { get; set; }


    }
}