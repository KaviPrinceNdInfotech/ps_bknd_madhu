using HospitalPortal.Models.APIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class NurseListVM
    {
        public string Message { get; set; }
        public int Status { get; set; }
        public IEnumerable<NurseListof> NurseList { get; set; }
        

    }

    public class NurseListV
    {
        
        public IEnumerable<NurseDetail> NurseLists { get; set; }

    }


    public class NurseListof
    {
        public int Id { get; set; }
        public string NurseName { get; set; }
        public string NurseTypeName { get; set; }
    }

    public class NurseDetail
    {
        public int Id { get; set; }

        public string NurseName { get; set; }
        public Nullable<int> Experience { get; set; }
        public double Fee { get; set; }
         public string about { get; set; }
        public string NurseTypeName { get; set; }

        public int? Rating { get; set; }
        
    }


    public class GetNurseProfile
    {
        public int Id { get; set; }
       
        public string NurseName { get; set; }
        public string MobileNumber { get; set; }

        public string EmailId { get; set; }

        public string StateName { get; set; }
        public string CityName { get; set; }
        public string PinCode { get; set; }

        public string Location { get; set; }


    }
    public class NurseAptmt
    {
        public int Id { get; set; }
        public string NurseName { get; set; }
        public string NurseTypeName { get; set; }
        public Nullable<int> Experience { get; set; }
        public double Fee { get; set; }
        public int GST { get; set; }
        public Nullable<double> TotalFee { get; set; }
        public Nullable<double> TotalFeeWithGST { get; set; }
        public Nullable<System.DateTime> ServiceDate { get; set; }
        public string SlotTime { get; set; }
        public int? TotalNumberofdays { get; set; }
        public string DeviceId { get; set; }
    }

    


}

