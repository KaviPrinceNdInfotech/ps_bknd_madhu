using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.RequestModel
{
    public class StatusResetRequest
    {
        [Required]
        public int Driver_Id { get; set; }
        [Required]
        public int Patient_Id { get; set; }
    }
}