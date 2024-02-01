using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class SpecialistDTO
    {
        public int Id { get; set; }
        [Required]
        public string SpecialistName { get; set; }
        [Required]
        public int Department_Id { get; set; }
        public bool IsDeleted { get; set; }
        public string DepartmentName { get; set; }
        public SelectList Departments { get; set; }
    }
}