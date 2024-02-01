using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class PatientOrderModel
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public bool IsDelivered { get; set; }
        public bool IsPaid { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string DeliveryAddress { get; set; }
        public string PinCode { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public double? TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string sOrderDate { get { return OrderDate.ToString("dd-MMM-yyyy"); } }
        public IEnumerable<PatientOrderDetailModel> OrderDetail { get; set; }

        public string Remarks { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryStatus { get; set; }
    }

    public class PatientOrderDetailModel
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string BrandName { get; set; }
        public int Quantity { get; set; }
        public double? UnitPrice { get; set; }
        public double? TotalPrice { get; set; }
    }



    


}