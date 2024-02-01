using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class HospitalNurseCommissionReport
    {
        public string PatientName { get; set; }
        public int Commission { get; set; }
        public int Nurse_Id { get; set; }
        public string NurseName { get; set; }
        public double Amount { get; set; }
        public string MobileNumber { get; set; }
        public int HospitalDoc_Id { get; set; }
        public string HospitalName { get; set; }
    }
}