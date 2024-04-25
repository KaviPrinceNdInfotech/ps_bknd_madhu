using HospitalPortal.Models.CommonClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class VehicleDTO : StateCityAbs
    {
        public string Type { get; set; }
        public double? DriverCharges { get; set; }
        public double? DriverCharge { get; set; }
        public string UniqueId { get; set; }
        public int? Page { get; set; }
        public int? NumberOfPages { get; set; }
        public SelectList Drivers { get; set; }
        public SelectList CategoryList { get; set; }
        public SelectList DriverList { get; set; }
        public SelectList VendorList { get; set; }
        public int Cat_Id { get; set; }
        
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        public string RefId { get; set; }
        public string VehicleName { get; set; }
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
       
        public string VehicleNumber { get; set; }
         
        public bool IsApproved { get; set; }
        
        public int VehicleType_Id { get; set; }
        public int? Driver_Id { get; set; }
        public string VehicleOwnerName { get; set; }
        public Nullable<int> Vendor_Id { get; set; } 
        public String FitnessCertificateValidityDate { get; set; } 
        public string VehicleTypeName { get; set; }
        public List<SelectListItem> VehicleTypes { get; set; } 
        public Nullable<double> BasePrice { get; set; }
     
        public Nullable<int> AsPerKM { get; set; }
        public string CategoryName { get; set; }
        public Nullable<int> IfExceeds100 { get; set; }

        [Required(ErrorMessage = "Account No. Required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Account Number must be numeric")]
        public string AccountNo { get; set; }
        [System.ComponentModel.DataAnnotations.Compare("AccountNo")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Account Number must be numeric")]
        public string VerifyAccountNo { get; set; }
        [Required(ErrorMessage = "IFSC Code Required")]
        public string IFSCCode { get; set; } 
        public string BranchName { get; set; } 
        public string BranchAddress { get; set; }
        [Required(ErrorMessage = "Account Holder Name Required")]
        public string HolderName { get; set; }
        public string CancelCheque { get; set; }

        public HttpPostedFileBase CancelChequeFile { get; set; } 
    }
	public class VehicleTypes
	{
		public int Id { get; set; }
		public string VehicleTypeName { get; set; }
	}
	public class VehicleNumbers
    {
		public int Id { get; set; }
		public string VehicleNumber { get; set; }
		public string DriverName { get; set; }
	}
    public class DriversName
	{
		public int Id { get; set; } 
		public string DriverName { get; set; }
	}
}