using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class FileVM
    {
        public IEnumerable<LabFiles> LabFile { get; set; }
        public string Message { get; set; }

        public int Status { get; set; }

        //public IEnumerable<HealthCheckUpFile> HealthCheckupFile {get;set;}
    }
}