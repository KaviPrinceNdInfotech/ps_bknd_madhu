using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class CompleteCheckup
    {
        public int Id { get; set; }
        public int Center_Id { get; set; }
        public string Name { get; set; }
        public string TestDesc { get; set; }
        public double TestAmount { get; set; }
        public IEnumerable<ShowDesc> showDesc { get; set; }
        public IEnumerable<HealthTest> HealthTest { get; set; }
    }


    public class ShowDesc
    {
        public int Id { get; set; }
        public string PackageName { get; set; }
        public string Name { get; set; }
        public string TestDesc { get; set; }
        public double TestAmt { get; set; }
        public int? gst { get; set; }
        public double? DiscountAmt { get; set; }

    }
}