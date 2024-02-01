using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models
{
    public class HospitalList
    {
        public SelectList HospitalsList { get; set; }
        public int? HospitalId { get; set; } 
    }
}