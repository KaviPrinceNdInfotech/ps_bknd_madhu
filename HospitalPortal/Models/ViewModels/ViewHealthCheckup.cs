using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class ViewHealthCheckup
    {
        public IEnumerable<ListHealthApp> HealthList { get; set; }
    }

    public class ListHealthApp
    {
        public int Id { get; set; }
        public string BookingId { get; set; }
        public string PatientName { get; set; }
        public DateTime TestDate { get; set; }
        public DateTime RequestDate { get; set; }
        public double Amount { get; set; }
        public string PatientAddress { get; set; }
        public string ContactNo { get; set; }
        public bool IsTaken { get; set; }
    }
}