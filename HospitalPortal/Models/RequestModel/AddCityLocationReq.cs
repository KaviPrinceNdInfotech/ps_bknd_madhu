using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.RequestModel
{
    public class AddCityLocationReq
    {
        [Required]
        public int  StateId { get; set; }
        public int  CityId { get; set; }
        public string CityName { get; set; }
        [Required]
        public string  LocationName { get; set; }
    }
}