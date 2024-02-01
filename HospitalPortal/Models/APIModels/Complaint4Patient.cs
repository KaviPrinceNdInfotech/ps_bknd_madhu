using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class Complaint4Patient
    {
        public int Id { get; set; }
        public Nullable<int> Login_Id { get; set; }
        [Required]
        public string Subjects { get; set; }
        [Required]
        public string Complaints { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsResolved { get; set; }
        public string Roles { get; set; }

        public string Others { get; set; }
    }


    //Complaint api ==============================  ////////// 
    public class ComplaintPatient
    {
        public IEnumerable<Complaint41Patient> Complaint41Patient { get; set; }
    }
    public class Complaint41Patient
    {
        public int Subid { get; set; }
        public string SubjectName { get; set; }

    }


    public class ComplaintPatientes
    {
        public int? Id { get; set; }
        public string Subjects { get; set; }
        public string Complaints { get; set; }
        public Nullable<int> Login_Id { get; set; }
        public int patsubid { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsResolved { get; set; }
        public string Others { get; set; }
    }


}