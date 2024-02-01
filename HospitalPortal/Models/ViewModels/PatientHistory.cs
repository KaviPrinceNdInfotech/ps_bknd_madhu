using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class PatientHistory

    {
        public IEnumerable<AppointmentSearchBy_Patient> AppointmentHistory { get; set; }
    }
}