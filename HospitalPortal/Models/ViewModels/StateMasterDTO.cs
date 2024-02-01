using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class StateMasterDTO
    {
        public int Id { get; set; }
        [Required]
        public string StateName { get; set; }
        public bool IsDeleted { get; set; }
    }
}