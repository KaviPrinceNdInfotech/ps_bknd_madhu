using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class Doctor_Complaint
    {
        public int Id { get; set; }
        public Nullable<int> Login_Id { get; set; }
        [Required]
        public string Subjects { get; set; }
        [Required]
        public string Complaints { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsResolved { get; set; }
        public string Others { get; set; }
    }
    
}