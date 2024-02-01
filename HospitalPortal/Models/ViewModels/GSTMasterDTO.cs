using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class GSTMasterDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public IEnumerable<gstList> GSTLIST { get; set; }
    }

    public class gstList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}