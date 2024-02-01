using HospitalPortal.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class Nurse_Complaints
    {
        public int ID { get; set; }
        public string Subjects { get; set; }
        public string Complaints { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsResolved { get; set; }
        public string Others { get; set; }
        public Nullable<int> Login_Id { get; set; }
    }

   
}