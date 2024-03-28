using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class TransactionFeeDTO
    {
        public int Id { get; set; }

        public Nullable<double> Fee { get; set; }
    }
}