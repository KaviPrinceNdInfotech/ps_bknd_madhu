using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class HospitalReportDTO
    {
        public int Id { get; set; }
        public Nullable<int> Lab_Id { get; set; }
        public Nullable<int> Patient_Id { get; set; }
        public Nullable<int> Test { get; set; }
        public SelectList TestName { get; set; }
        public string Department { get; set; }
        public string FileName { get; set; }
        public string PatientName { get; set; }
        public HttpPostedFileBase[] File { get; set; }
    }
}