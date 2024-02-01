using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class DoctorSkillsDTO
    {
        public int Id { get; set; }
        [Required]
        public int Doctor_Id { get; set; }
        [Required]
        public string SkillName { get; set; }
    }
}