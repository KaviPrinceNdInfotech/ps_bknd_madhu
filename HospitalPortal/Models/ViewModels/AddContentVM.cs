using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class AddContentVM
    {
        public SelectList PageNameList { get; set; }

        public IEnumerable<AddContentpage> contentpage { get; set; }
        public string PageName { get; set; }
        public int Id { get; set; }
        [System.Web.Mvc.AllowHtml]
        public string About { get; set; }
    }
    public class AddContentpage
    {
        public int Id { get; set; }
        public string PageName { get; set; }

        [System.Web.Mvc.AllowHtml]
        public string About { get; set; }
    }
}