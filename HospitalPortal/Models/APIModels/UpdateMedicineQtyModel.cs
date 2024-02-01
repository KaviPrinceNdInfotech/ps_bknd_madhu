using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class UpdateMedicineQtyModel
    {
        [Required]
        public int PatientId { get; set; }
        [Required]
        public int MedicineId { get; set; }
        [Required]
        public string Type { get; set; }
    }
}