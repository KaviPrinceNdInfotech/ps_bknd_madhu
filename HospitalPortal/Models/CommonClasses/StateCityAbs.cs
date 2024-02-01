using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.CommonClasses
{
    public class StateCityAbs
    {
        public SelectList States { get; set; }
        
        public SelectList Cities { get; set; }
        public SelectList Locations { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string LocationName { get; set; }
    }
}