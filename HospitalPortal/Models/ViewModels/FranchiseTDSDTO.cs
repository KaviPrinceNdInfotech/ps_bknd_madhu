using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class FranchiseTDSDTO
    {
        public int Id { get; set; }
        public string Department { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string Name { get; set; }
        public IEnumerable<FranchiseTDSLIST> FranchiseTDSLIST { get; set; }
    }
    public class FranchiseTDSLIST
    {
        public int Id { get; set; }
        public string Department { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string Name { get; set; }
    }
}