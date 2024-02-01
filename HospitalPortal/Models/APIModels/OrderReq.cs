using HospitalPortal.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class OrderReq
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Mobile { get; set; }
        [Required]
        public int StateId { get; set; }
        [Required]
        public int CityId { get; set; }
        [Required]
        public int LocationId { get; set; }
        [Required]
        public string Address { get; set; }
        public string PinCode { get; set; }
        //[Required]
        //public double Lat { get; set; }
        //[Required]
        //public double Lng { get; set; }
    }



    public class AddAddressMedicine
    {
        public int Id { get; set; }
      
        public string Name { get; set; }
       
        public string MobileNumber { get; set; }
        
        public string Email { get; set; }

        public int StateMaster_Id { get; set; }
 
        public int CityMaster_Id { get; set; }

 
        public string DeliveryAddress { get; set; }

        
        public string PinCode { get; set; }

        public int? Patient_Id { get; set; }
    }



    public class AddAddress
    {
        public IEnumerable<AddAddressMedicines> AddAddressMediciness { get; set; }
    }


    public class AddAddressMedicines
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [MobileNumberValidation]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile Number must be numeric")]
        public string MobileNumber { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string StateName { get; set; }
        
        public string CityName { get; set; }

       
        [Required]
        public string DeliveryAddress { get; set; }

        [Required]
        [RegularExpression(@"^(\d{6,6})$", ErrorMessage = "6 Digits Required")]
        public string PinCode { get; set; }

    }
}