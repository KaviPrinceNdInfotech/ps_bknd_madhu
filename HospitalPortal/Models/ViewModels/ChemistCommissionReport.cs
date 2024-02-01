using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class ChemistCommissionReport
    {
        public DateTime OrderDate { get; set; }
        public IEnumerable<ChemsitDetails> chemistDetails { get; set; }
        public string ChemistName { get; set; }
        public int ChemistId { get; set; }
    }


    public class ChemsitDetails
    {
        public DateTime OrderDate { get; set; }
        public int ChemistId { get; set; }
        public int Id { get; set; }
        public string ChemistName { get; set; }
        public double Amount { get; set; }
        public double Commission { get; set; }
        public int OrderId { get; set; }
      
    }
}