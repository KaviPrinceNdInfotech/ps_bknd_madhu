using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class ChargeDTO
    {
        public int Id { get; set; }
        public Nullable<double> Amount { get; set; }
        public string Role { get; set; }

        public IEnumerable<ChargeList> Charges { get; set; }
        public IEnumerable<MedcineDelChargelist> MedcineDelChargelist { get; set; }
    }

    public class ChargeList
    {
        public int Id { get; set; }
        public Nullable<double> Amount { get; set; }
        public string Role { get; set; }
    }

    public class MedcineDelChargelist
    {
        public int Id { get; set; }
        public int? Amount { get; set; }
    }
}