using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class HospitalDepartmentDTO
    {
        public int Id { get; set; }
        [Required]
        public int Hospital_Id { get; set; }
        [Required]
        public int Department_Id { get; set; }
        public bool IsDeleted { get; set; }
        public string DeptName { get; set; }
        public SelectList Depts { get; set; }
    }
}