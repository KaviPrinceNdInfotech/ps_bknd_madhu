using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class SharetoVendorDTO
    {
        public int Vendor_Id { get; set; }
        public string VendorName { get; set; }
        public string Role { get; set; }
        public int Id { get; set; }
    }
}