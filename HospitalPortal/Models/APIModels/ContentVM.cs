using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class ContentVM
    {
        public IEnumerable<Contentpage> content { get; set; }
        public int Id { get; set; }
        public string PageName { get; set; }
        public string About { get; set; }
    }


    public class Contentpage
    {
        public int Id { get; set; }
        public string PageName { get; set; }
        public string About { get; set; }
    }
}