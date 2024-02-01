using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class LabFiles
    {
        public string File { get; set; }
        public string TestName { get; set; }
    }

    public class LabList
    {
        public int Id { get; set; }
        public string LabName { get; set; }
        public string About { get; set; }
        public string LabTypeName { get; set; }
        public string year { get; set; }
        public string Location { get; set; }

        public string WorkingDay { get; set; }
        public double? Fee { get; set; }
        public int? Rating { get; set; }


    }
}