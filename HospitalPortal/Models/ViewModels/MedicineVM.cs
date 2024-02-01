using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class MedicineVM
    {
        public IEnumerable<MedicineOrderModel> MedicineOrderHis { get; set; }
    }
}