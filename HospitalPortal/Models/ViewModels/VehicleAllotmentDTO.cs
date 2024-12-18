﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class VehicleAllotmentDTO
    { 
        public string VehicleNumber { get; set; }
        public string DriverName { get; set; }
         
        public int Id { get; set; }
        public SelectList VehicleList { get; set; }
        public IEnumerable<VehicleLists> VehicleLists {get; set;}
         
        public int VehicleTypeId { get; set; }
        public int VehicleNumberId { get; set; }
		public int DriverId { get; set; }
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