using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class NurseCommissionReports
    {
        public int? NurseId { get; set; }
        public DateTime ServiceAcceptanceDate { get; set; }
        public string NurseName { get; set; }
        public IEnumerable<NurseAppointmentList> NurseAppointmentList { get; set; }
    }
}