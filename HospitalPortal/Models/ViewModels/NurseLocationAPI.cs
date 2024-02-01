using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class NurseLocationAPI
    {
        public string Message { get; set; }
        public int Status { get; set; }
        public IEnumerable<NurseLocationModel> NurseLocation { get;set; }
    }
}