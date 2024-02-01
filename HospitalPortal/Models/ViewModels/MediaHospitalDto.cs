using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class MediaHospitalDto
    {
        public int id { get; set; }
        public string Title { get; set; }
 
        public HttpPostedFileBase ImageBase { get; set; }


    }
}