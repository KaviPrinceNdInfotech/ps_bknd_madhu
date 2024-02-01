using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class ChemistReport
    {
        public int Id { get; set; }
        public string ChemistName { get; set; }
        public double Amount { get; set; }
        public string MobileNumber { get; set; }
        public string ShopName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string LicenceNumber { get; set; }
        public double TotalAmount { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? OrderDate1 { get; set; }
        public string Location { get; set; }
             
    }
}