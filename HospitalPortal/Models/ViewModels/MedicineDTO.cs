using HospitalPortal.Models.APIModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class MedicineDTO
    {

        public SelectList MedicineList { get; set; }
        public int Id { get; set; }
        public int MedicineId { get; set; }
        [Required]
        public string MedicineName { get; set; }
        public string MedicineDescription { get; set; }
        public Nullable<int> MedicineType_Id { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        public double MRP { get; set; }
        public string BrandName { get; set; }
        public string MedicineTypeName { get; set; }
    }

    public class MedicineOrderModel
    {
        public int Id { get; set; }
        public string MedicineName { get; set; }
        public string MedicineTypeName { get; set; }
        public string MedicineDescription { get; set; }
        public string BrandName { get; set; }
        public double Amount { get; set; }
        public int Quantity { get; set; }
        public int Order_Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string DeliveryAddress { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
    }
}