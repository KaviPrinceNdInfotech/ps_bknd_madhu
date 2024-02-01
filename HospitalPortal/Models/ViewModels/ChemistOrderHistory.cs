using HospitalPortal.Models.APIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class ChemistOrderHistory
    {
        public int NoOfPages { get; set; }
        public int Page { get; set; }
        public IEnumerable<PatientOrderModel> Orders { get; set; }
        public string term { get; set; }
        public DateTime? sDate { get; set; }
        public DateTime? eDate { get; set; }
    }
}