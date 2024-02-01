using HospitalPortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class AppointmentClass
    {
        public IEnumerable<SlotTimingVM> slottiming { get; set; }
    }
}