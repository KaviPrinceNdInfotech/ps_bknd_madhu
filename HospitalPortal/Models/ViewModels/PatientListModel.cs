using HospitalPortal.Models.APIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class PatientListModel
    {
        
        public IEnumerable<PatientList> Patient { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
    }
}