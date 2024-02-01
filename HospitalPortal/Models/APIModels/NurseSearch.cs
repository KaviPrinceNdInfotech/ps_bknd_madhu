using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class NurseSearch
    {
        public IEnumerable<NurseList> NurseList { get; set; } 
        //public int Id { get; set; }
        //public string  NurseName { get; set; }
        //public string EmailId { get; set; }
        //public string MobileNumber { get; set; }
        //public string StateName { get; set; }
        //public string CityName { get; set; }
        //public double Fee { get; set; }
        //public string Address { get; set; }
        //public string NurseTypeName { get; set; }
    }

    public class NurseList
    {
        public int Id { get; set; }
        public string NurseName { get; set; }
        public string EmailId { get; set; }
        public string MobileNumber { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public double Fee { get; set; }
        public string Location { get; set; }
        public string NurseTypeName { get; set; }
    }

}