using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class VendorAddedVehicleVM
    {
     public IEnumerable<AddedVehicleList> AddedVehicleList { get; set; }
    }

    public class AddedVehicleList
    {
        public int Id { get; set; }
        public string VehicleTypeName { get; set; }
        public string CategoryName { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDeleted { get; set; }
    }
}