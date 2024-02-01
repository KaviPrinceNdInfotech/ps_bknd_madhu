using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class RWATDSDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string Department { get; set; }
        public IEnumerable<RWAtdsList> RWATDSLIST { get; set; }
    }
    public class RWAtdsList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string Department { get; set; }
    }
}
