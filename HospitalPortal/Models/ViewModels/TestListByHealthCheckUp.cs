using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class TestListByHealthCheckUp
    {
        public int Id { get; set; }
        public string TestName { get; set; }
        public string File { get; set; }
        public string PatientName { get; set; }
        public string MobileNumber { get; set; }
        public int StateMaster_Id { get; set; }
        public int CityMaster_Id { get; set; }
         public string StateName { get; set; }
        public string CityName { get; set; }
        public string Location { get; set; }
        public string EmailId { get; set; }
    }
}