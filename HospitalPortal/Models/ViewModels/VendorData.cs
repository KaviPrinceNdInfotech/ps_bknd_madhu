using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class VendorData
    {
        public IEnumerable<VendorMsgList> VendorMsgList { get; set; }
    }

    public class VendorMsgList
    {
        public string VehicleNumber { get; set; }
        public DateTime? Validity { get; set; }

        public DateTime? FitnessCertificateValidity { get; set; }
        public DateTime? PollutionDate { get; set; }
        public DateTime? InsurranceDate { get; set; }
        public string MobileNumber { get; set; }
    }
}