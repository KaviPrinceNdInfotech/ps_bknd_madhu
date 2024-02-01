using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class DepartmentDTO
    {
        public int Id { get; set; }
        [Required]
        public string DepartmentName { get; set; }
        public bool IsDeleted { get; set; }
    }
}