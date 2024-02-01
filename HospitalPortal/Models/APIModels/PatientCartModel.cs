using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class PatientCartModel
    {
        public int CartId { get; set; }
        public int Medicine_Id { get; set; }
        public string MedicineName { get; set; }
        public string BrandName { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
    }
    public class PatientCart
    {
        public int CartId { get; set; }
        public string MedicineName { get; set; }
        public string BrandName { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
    }
    public class PatientCartReturnModel
    {
        public int TotalProductsInCart { get; set; }
        public double GrandTotal { get; set; }
        public IEnumerable<PatientCartModel> MedicineCart { get; set; }


    }


    public class OrderPlace
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string DeliveryAddress { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string PinCode { get; set; }
        public int StateMaster_Id { get; set; }
        public int CityMaster_Id { get; set; }
        public int Patient_Id { get; set; }
        public double TotalPrice { get; set; }
        public double Deliverycharge { get; set; }
    }



    public class PlaceOrders
    {
        public int PatientId { get; set; }

        public int shippingId { get; set; }
    }

    public class MedicineInvoice
    {
        public int Id { get; set; }
        public string MedicineName { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
        public string OrderId { get; set; }
        public string InvoiceNumber { get; set; }
        public System.DateTime OrderDate { get; set; }
        public string PatientName { get; set; }
        public string EmailId { get; set; }
        public string MobileNumber { get; set; }
        public string Location { get; set; }
        public string PinCode { get; set; }
    }
}