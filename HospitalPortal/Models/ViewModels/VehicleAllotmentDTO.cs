using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class VehicleAllotmentDTO
    {
        [Required]
        public string VehicleNumber { get; set; }
        public string DriverName { get; set; }
        [Required]
        public int Id { get; set; }
        public SelectList VehicleList { get; set; }
        public IEnumerable<VehicleLists> VehicleLists {get; set;}
        [Required]
        public int VehicleTypeId { get; set; }
    }

    public class VehicleLists
    {   
        public int Id { get; set; } // Id is using as Driver Id
        public int VehicleId { get; set; }
        public int VehicleTypeId { get; set; }
        public string DriverName { get; set; }
        public string VehicleNumber { get; set; }
    }

}