using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.RequestModel
{
    public class refundAmountDTO
    {
        public int? Patient_Id { get; set; }
        public double? Amount { get; set; }
    }
}