using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class TravelHistoryVM
    {
        public IEnumerable<travelHistoryValues> travelHistory { get; set; }
    }

    public class travelHistoryValues
    {
        public int Driver_Id { get; set; }
        public string DriverName { get; set; }
        public string PatientName { get; set; }
        public string PickUp_Place { get; set; }
        public string Drop_Place { get; set; }
        public double Amount { get; set; }
        public double Distance { get; set; }
        public bool IsDriveCompleted { get; set; }

    }
}