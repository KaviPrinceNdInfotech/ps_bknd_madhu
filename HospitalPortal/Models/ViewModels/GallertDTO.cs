using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class GallertDTO
    {
        public int Id { get; set; }
        public string ImageName { get; set; }
        public string Images { get; set; }
        public HttpPostedFileBase Image { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public IEnumerable<GalleryList> GalleryList { get; set; }
        public IEnumerable<Documents> Documents { get; set; }
    }

    public class GalleryList
    {
        public int Id { get; set; }
        public string ImageName { get; set; }
        public string Images { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
    public class Documents
    {
        public int Id { get; set; }
        public string FileName { get; set; }
    }
}