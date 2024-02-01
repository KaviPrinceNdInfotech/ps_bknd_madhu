using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class UpdateBank
    {
       // [Required(ErrorMessage = "Mobile Number Required")]
        public string MobileNumber { get; set; }
        [Required(ErrorMessage = "Account Holder Name Required")]
        public string HolderName { get; set; }
        public int Id { get; set; }
        public Nullable<int> Login_Id { get; set; }
        [Required (ErrorMessage = "Account No. Required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Account Number must be numeric")]
        public string AccountNo { get; set; }
        [Compare("AccountNo")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Account Number must be numeric")]
        public string VerifyAccountNo { get; set; }
        [Required(ErrorMessage = "IFSC Code Required")]
        public string IFSCCode { get; set; }
        [Required(ErrorMessage = "Branch Name Required")]
        public string BranchName { get; set; }
        [Required(ErrorMessage = "Branch Address Required")]
        public string BranchAddress { get; set; }

        //public string CancelCheque { get; set; }

        //public HttpPostedFileBase CancelChequeFile { get; set; }
        public bool? isverfied { get; set; }
    }
}