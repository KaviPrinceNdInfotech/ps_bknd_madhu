using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class PatientTestListVM
    {
        public int Patient_Id { get; set; }
        public string PatientName { get; set; }
        public string LabName { get; set; }
        public string PatientRegNo { get; set; }
        public int Id { get; set; }
    }
}