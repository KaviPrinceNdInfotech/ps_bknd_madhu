using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class GetBankDetails
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public int Id { get; set; }
        public Nullable<int> Login_Id { get; set; }
        public string AccountNo { get; set; }
        public string IFSCCode { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string HolderName { get; set; }
        public string CancelCheque { get; set; }
        public string MobileNumber { get; set; }
        public Nullable<bool> isverified { get; set; }

    }
}