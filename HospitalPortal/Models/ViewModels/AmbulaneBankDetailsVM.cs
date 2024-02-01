using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class AmbulaneBankDetailsVM
    {
        public string AccountNo { get; set; }
        public string IFSCCode { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string HolderName { get; set; }
        public string CancelCheque { get; set; }
        public IEnumerable<BankDetailsList> BankDetails { get; set; }
    }

    public class BankDetailsList
    {
        public string AccountNo { get; set; }
        public string IFSCCode { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string HolderName { get; set; }
        public string CancelCheque { get; set; }
    }
}