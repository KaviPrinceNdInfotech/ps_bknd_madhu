using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class TDSMasterDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public IEnumerable<tdsList> TDSLIST { get; set; }

    }

    public class tdsList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}
    