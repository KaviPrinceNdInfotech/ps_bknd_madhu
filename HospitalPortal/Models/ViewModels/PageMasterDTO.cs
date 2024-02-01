using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class PageMasterDTO
    {
        public int Id { get; set; }
        public string PageName { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public IEnumerable<PageMasterVM> PageMasterList { get; set; }

    }

    public class PageMasterVM
    {
        public int Id { get; set; }
        public string PageName { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }

}