using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class Banner_List
    {
        public int ID { get; set; }
        public string BannerPath { get; set; }
        public Nullable<int> pro_id { get; set; }
        public IEnumerable<listBanner> listBanner { get; set; }
        public string professionals { get; set; }

    }

    public class listBanner
    {
        public int ID { get; set; }
        public string BannerPath { get; set; }
        public string professionals { get; set; }
    }
}