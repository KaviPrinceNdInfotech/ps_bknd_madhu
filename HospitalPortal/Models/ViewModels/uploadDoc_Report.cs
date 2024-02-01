using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class uploadDoc_Report
    {
        public int Id { get; set; }
        public Nullable<int> Doctor_Id { get; set; }
        public Nullable<int> Patient_Id { get; set; }
        public string Image1Base64 { get; set; }
        public string Image1Name { get; set; }
        public string Image1 { get; set; }
        
    }

    public class UplaodReportBase
    {
        public int Doctor_Id { get; set; }
        public int Patient_Id { get; set; }
       public IEnumerable<uploadDoc_Report> Reports { get; set; }
    }
}