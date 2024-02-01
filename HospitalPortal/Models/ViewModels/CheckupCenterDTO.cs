using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class CheckupCenterDTO
    {
        public int Id { get; set; }
        [Required]
        public int Hospital_Id { get; set; }
        [Required]
        public int Department_Id { get; set; }
    }
}