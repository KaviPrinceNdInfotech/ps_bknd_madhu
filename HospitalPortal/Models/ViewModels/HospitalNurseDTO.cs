using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class HospitalNurseDTO
    {
        public int Id { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string NurseName { get; set; }
        public Nullable<int> NurseType_Id { get; set; }
        public Nullable<int> Hospital_Id { get; set; }
        public SelectList NurseTypeList { get; set; }
    }
}