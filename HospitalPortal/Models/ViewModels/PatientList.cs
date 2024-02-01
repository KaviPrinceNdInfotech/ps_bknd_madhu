
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class PatientList
    {
        public int? vendorId { get; set; }
        public string UniqueId { get; set; }

        public string PatientRegNo { get; set; }
        public int? Id { get; set; }
        public string PatientName { get; set; }
        public string EmailId { get; set; }
        public string MobileNumber { get; set; }
        public int? StateMaster_Id { get; set; }
        public int? CityMaster_Id { get; set; }
        public string Location { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }
        public Nullable<int> Rwa_Id { get; set; }
        public string VendorName { get; set; }
        public int AdminLogin_Id { get; set; }
    }
}