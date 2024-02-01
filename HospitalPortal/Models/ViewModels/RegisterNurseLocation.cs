using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class RegisterNurseLocation
    {
        public SelectList States { get; set; }
        public int[] LocationIds { get; set; }
    }
}