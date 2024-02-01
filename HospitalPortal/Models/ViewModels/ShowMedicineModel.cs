using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class ShowMedicineModel
    {
        public int page { get; set; }
        public int totalPages { get; set; }
        public List<MedicineDTO> medicines { get; set; }
    }
}