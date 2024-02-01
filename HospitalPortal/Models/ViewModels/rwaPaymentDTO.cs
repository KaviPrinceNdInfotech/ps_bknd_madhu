using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class rwaPaymentDTO
    {
        public int Id { get; set; }
        public string Department { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

        public IEnumerable<rwapaymentList> rwapaymentList { get; set; }
        public IEnumerable<rwaGSTList> rwaGSTList { get; set; }
        public IEnumerable<FraGSTList> FraGSTList { get; set; }
    }



    public class rwapaymentList
    {
        public int Id { get; set; }
        public string Departement { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
    public class rwaGSTList
    {
        public int Id { get; set; }
        public string Department { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }

    public class FraGSTList
    {
        public int Id { get; set; }
        public string Department { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}