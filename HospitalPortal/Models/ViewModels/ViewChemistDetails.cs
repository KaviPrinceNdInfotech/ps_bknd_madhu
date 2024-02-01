using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class ViewChemistDetails
    {
        public DateTime? OrderDate { get; set; }
        public DateTime? OrderDate1 { get; set; }
        public string MedicineName { get; set; }
        public string  Name{ get; set; }
        public string MobileNumber { get; set; }
        public string DeliveryAddress { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string PinCode { get; set; }
        public string InvoiceNumber { get; set; }
        public double MRP { get; set; }
        public double Amount { get; set; }
        public int Quantity { get; set; }
    }
}