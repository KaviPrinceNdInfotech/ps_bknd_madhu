using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class UpdateBankDetails
    {
        public int Id { get; set; }
        public Nullable<int> Login_Id { get; set; }
        public string AccountNo { get; set; }
        public string IFSCCode { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string HolderName { get; set; }
      //  public string ChequeImage { get; set; }
        //public string ChequeImageName { get; set; }
      //  public string ChequeImageBase64Image { get; set; }
        public string MobileNumber { get; set; }
        public bool? isverified { get; set; }
    }



    public class UpdateBankDetailss
    {
        
        public Nullable<int> Login_Id { get; set; }
        public string AccountNo { get; set; }
        public string IFSCCode { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string HolderName { get; set; }
        public bool? isverified { get; set; }
    }
}
