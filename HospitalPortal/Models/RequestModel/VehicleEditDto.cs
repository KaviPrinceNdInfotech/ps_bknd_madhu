﻿using HospitalPortal.Models.CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.RequestModel
{
    public class VehicleEditDto : StateCityAbs
    {
         
        public string Type { get; set; }
        public double? DriverCharges { get; set; }
        public string UniqueId { get; set; } 
        public int VehicleCat_Id { get; set; }
        public int VehicleType_Id { get; set; }
        public SelectList CategoryList { get; set; }
        public SelectList VehicleTypeList { get; set; }
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        public string VehicleName { get; set; }
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
       
        public string VehicleNumber { get; set; }  
        public bool IsApproved { get; set; } 
        public int? Driver_Id { get; set; }
        public Nullable<int> Vendor_Id { get; set; } 
        public string InsuranceImage { get; set; }
        public string PollutionImage { get; set; }
        public string FitnessCerficateImage { get; set; }
        public string VehicleTypeName { get; set; }
        public string VehicleOwnerName { get; set; } 
        
        public string VehicleImg { get; set; }
        public HttpPostedFileBase VehicleImgFile { get; set; }
        
        public HttpPostedFileBase PollutionImageFile { get; set; }
        
        public HttpPostedFileBase InsurranceImageFile { get; set; }
        
        public HttpPostedFileBase FitnessCertificateImageFile { get; set; }
        public string RC_Image { get; set; }
        
        public HttpPostedFileBase RC_ImageFile { get; set; }
        
        public string RC_No { get; set; }
        public Nullable<double> BasePrice { get; set; }

        public Nullable<int> AsPerKM { get; set; }
        public string CategoryName { get; set; }
        public Nullable<int> IfExceeds100 { get; set; }
        public string AccountNo { get; set; }
        
        public string VerifyAccountNo { get; set; }
        public string IFSCCode { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string HolderName { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public HttpPostedFileBase CancelChequeFile { get; set; }
    }
}