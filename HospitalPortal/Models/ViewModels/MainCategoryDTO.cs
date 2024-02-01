using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class MainCategoryDTO
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string Type { get; set; }
        public int AmbulanceType_id { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public IEnumerable<MainCategoryList> MainCategoryList { get; set; }
    }

    public class MainCategoryList
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string Type { get; set; }
        public int? AmbulanceType_id { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}