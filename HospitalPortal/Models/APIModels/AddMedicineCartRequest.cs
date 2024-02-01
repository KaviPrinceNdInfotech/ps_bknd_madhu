using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{

    public class AddMedicineCartRequest
    {
        [Required]
        public int PatientId { get; set; }
        [Required]
        public int MedicineId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }

    public class AddMedicineCartRequests
    {
        [Required]
        public int PatientId { get; set; }
        [Required]
        public int MedicineId { get; set; }
        [Required]
        public int Id { get; set; }
    }




    public class MedicinePayNows
    {
        public int Id { get; set; }
        //public int Medicine_Id { get; set; }
        public int Patient_Id { get; set; }

        public bool IsPaid { get; set; }
        public int shippingId { get; set; }
    }

}