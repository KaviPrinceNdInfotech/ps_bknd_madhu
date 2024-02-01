using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class HealthTest
    {
        public int Id { get; set; }
        public string TestDesc { get; set; }
        public double? TestAmount { get; set; }
    }
}