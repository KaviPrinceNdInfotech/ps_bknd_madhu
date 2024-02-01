using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class BannerAddDTO
    {
        public int ID { get; set; }
        public string BannerPath { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        //public HttpPostedFileBase ImageFile { get; set; }
        //public bool IsActive { get; set; }
        
        public int pro_id { get; set; }
        public SelectList bannerlist { get; set; }
    }
}