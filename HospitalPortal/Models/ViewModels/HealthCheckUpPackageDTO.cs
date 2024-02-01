using HospitalPortal.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class HealthCheckUpPackageDTO
    {
        public bool IsPackageAvailable { get; set; }
        public int Id { get; set; }
        public int? packageId { get; set; }
        public int statusKey { get; set; }
        public SelectList PackageList { get; set; }
        public Nullable<int> HealthPackageID { get; set; }

        public double TestAmt { get; set; }
        public int Gst { get; set; }
        public double? DiscountAmt { get; set; }
        public double? GrandTotal { get; set; }
        public double? gTotal { get; set; }
        public List<SelectListItem> LabTest { get; set; }
        public int? TestId { get; set; }
        public SelectList Tests { get; set; }
        public int[] chosenIds { get; set; }

        public IEnumerable<HealthTest> HealthTest { get; set; }


        //
        public int? CenterId { get; set; }

        public String PackageName { get; set; }

        //public int? PackageId { get; set; }

        public String TestName { get; set; }

        public String TestDesc { get; set; }

    }


}