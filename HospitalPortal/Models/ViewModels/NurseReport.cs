using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class NurseReport
    {
        public IEnumerable<NurseNameList> NursesList { get; set; }
        public SelectList NurseTypeList { get; set; }
        public int Id { get; set; }
    }

    public class NurseNameList
    {
        public int Id { get; set; }
        public string NurseName { get; set; }
        public string NurseTypeName { get; set; }
    }
}