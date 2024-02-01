using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class MedicineList
    {
        public IEnumerable<ChemistDTO> ChemistListDTO { get; set; }
    }
    public class MedicineListp
    {
        public IEnumerable<PaMedicine> PaMedicine { get; set; }

       // public IEnumerable<ChemistDTO> ChemistListDTO { get; set; }
    }

    public partial class PaMedicine
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