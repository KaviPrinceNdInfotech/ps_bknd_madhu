using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class ViewHealthDetails
    {
        public string TestDate1 { get; set; }
        public string PatientName { get; set; }
        public string PatientAddress { get; set; }
        public string ContactNo { get; set; }
        public string Name { get; set; }
        public double TestAmount { get; set; }
    }
}