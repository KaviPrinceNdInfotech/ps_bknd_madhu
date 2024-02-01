using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class ViewLabDetails
    {
        public string TestDate1 { get; set; }
        public string PatientName { get; set; }
        public string PatientAddress { get; set; }
        public string ContactNumber { get; set; }
        public string TestName { get; set; }
        public double Amount { get; set; }
    }
}