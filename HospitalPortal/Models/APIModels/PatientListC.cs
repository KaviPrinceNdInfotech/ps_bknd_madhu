using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class PatientListC
    {
        public IEnumerable<PatientDetails> PatientDetail { get; set; }
    }

    public class PatientDetails
    {
        public int Id { get; set; }
        public string PatientRegNo { get; set; }
        public string MobileNumber { get; set; }
        public string PatientName { get; set; }
    }

    public partial class RWA_PaymentReports
    {
        public int Id { get; set; }
        //public Nullable<int> RWA_Id { get; set; }
        //public Nullable<int> User_Id { get; set; }
        //public Nullable<int> Bank_Id { get; set; }
        //public string Payment_Id { get; set; }
        public string PatientName { get; set; }
        public string  BankName { get; set; }

        public Nullable<decimal> PaidAmount { get; set; }
        public string PaymentDate { get; set; }
        public string PaymentTime { get; set; }
        //public Nullable<bool> PaymentStatus { get; set; }
    }



}