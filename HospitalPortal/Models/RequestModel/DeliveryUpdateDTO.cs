using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.RequestModel
{
    public class DeliveryUpdateDTO
    {
        public int id { get; set; }
        [Required]
        public int code { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public DateTime date { get; set; }
        [Required]
        public string deliveryStatus { get; set; }
    }
}