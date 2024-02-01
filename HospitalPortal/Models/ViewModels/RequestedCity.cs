using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class RequestedCity
    {
        public int Id { get; set; }
        public string CityName { get; set; }
        public Nullable<int> Login_Id { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public Nullable<int> State_Id { get; set; }

        public IEnumerable<CityList> CityList { get; set; }
    }


    public class CityList
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public Nullable<int> Login_Id { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public Nullable<int> State_Id { get; set; }
        public string StateName { get; set; }
        public string Role { get; set; }
    }
}