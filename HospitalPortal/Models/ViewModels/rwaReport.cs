using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class rwaReport
    {
        public DateTime sdate { get; set; }
        public DateTime edate { get; set; }
        public string AuthorityName { get; set; }
        public DateTime Reg_Date { get; set; }
        public IEnumerable<rwaList> rwaList { get; set; }
    }


    public class rwaList
    {
        
        public int Id { get; set; }
        public string AuthorityName { get; set; }
        public int Counts { get; set; }
        public DateTime Reg_Date { get; set; }
        public string PatientName { get; set; }

    }
}