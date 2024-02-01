using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class PaymentHistroyForDosctor
    {
        public DateTime? AppointmentDate { get; set; }
        public IList<ListPayment> PaymentHistory { get; set; }
    }

    public class ListPayment
    {
        public DateTime? AppointmentDate { get; set; }
        public double? Amount { get; set; }
    }
}