using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class BookHealthCheckUp
    {
        public int Center_Id { get; set; }
        public string BookingId { get; set; }
        public Nullable<int> PatientId { get; set; }
        public Nullable<System.DateTime> TestDate { get; set; }
        public string PatientAddress { get; set; }
        public string ContactNo { get; set; }
        public Nullable<System.DateTime> RequestDate { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public Nullable<bool> IsPaid { get; set; }
        public int? Test_Id { get; set; }
        public string[] PatientName { get; set; }
        public List<Patientl> Patient { get; set; }
    }

    public class Patientl
    {
        public string PatientName { get; set; }

    }
}