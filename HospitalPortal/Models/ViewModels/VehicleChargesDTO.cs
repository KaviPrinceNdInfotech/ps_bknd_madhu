using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class VehicleChargesDTO
    {
        public int Id { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<int> Category_Id { get; set; }
        public Nullable<int> TypeId { get; set; }
        public SelectList CategoryList { get; set; }
        public SelectList TypeList { get; set; }
        public string CategoryName { get; set; }
        public string AppliedFor { get; set; }
        public string VehicleTypeName { get; set; }
        public IEnumerable<VehicleListClass> ListVehicleType { get; set; }
    }

    public class VehicleListClass
    {
        public int Id { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<int> Category_Id { get; set; }
        public Nullable<int> TypeId { get; set; }
        public string CategoryName { get; set; }
        public string AppliedFor { get; set; }
        public string VehicleTypeName { get; set; }

    }
}