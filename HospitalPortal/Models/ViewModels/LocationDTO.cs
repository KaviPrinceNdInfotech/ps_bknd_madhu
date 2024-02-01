using HospitalPortal.Models.CommonClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class LocationDTO : StateCityAbs
    {
        public int Id { get; set; }
        [Required]
        public int City_Id { get; set; }
         
        public bool IsDeleted { get; set; }
        public SelectList CityList { get; set; }
       
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class ComplaintDTO
    {
        public int Id { get; set; }
        public string Subjects { get; set; }

        //public bool IsDeleted { get; set; }
    }

}
