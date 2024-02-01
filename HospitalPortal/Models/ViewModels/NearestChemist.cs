using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class NearestChemist
    {
        public int Id { get; set; }
        public string ChemistName { get; set; }
        public string ShopName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string LicenceImage { get; set; }
        public string Location { get; set; }
        public double Distance { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string LocationName { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }
}