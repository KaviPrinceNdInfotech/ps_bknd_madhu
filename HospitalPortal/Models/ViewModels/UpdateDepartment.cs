using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class UpdateDepartment
    {
        public int Id { get; set; }
        [Required]
        public string Department { get; set; }
        [Required]
        public string Specialist { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string Requested { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public IEnumerable<DepartmentList> DepartmentList { get; set; }
    }

    public class DepartmentList
    {
        public int Id { get; set; }
        public string Department { get; set; }
        public string Specialist { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string Requested { get; set; }
        public Nullable<bool> IsApproved { get; set; }


    }
}