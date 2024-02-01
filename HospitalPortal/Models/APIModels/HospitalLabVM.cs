using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class HospitalLabVM
    {
        
        public string Message { get; set; }
        public int Status { get; set; }
        public IEnumerable<HospitalLabs> HospitalLabs { get; set; }
       
    }

    public class HospitalLab
    {
        
        public string Message { get; set; }
        public int Status { get; set; }
        

    }
    public class LabDet
    {
        public int Id { get; set; }
        public string LabName { get; set; }
        public string LabTypeName { get; set; }
        public string year { get; set; }

        public double? Fee { get; set; }
        public int GST { get; set; }
        public Nullable<double> TotalFee { get; set; }
        public Nullable<System.DateTime> TestDate { get; set; }
        public string SlotTime { get; set; }

    }



   


  


    public class HospitalLabs
    {
        public int Id { get; set; }
        public string LabName { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        //public TimeSpan StartTime { get; set; }
        //public TimeSpan EndTime { get; set; }
        public string OpeningHours { get; set; }

    }




    public class LabPayNow
    {
        public int Id { get; set; }
        public Nullable<int> Lab_Id { get; set; }
        public int Patient_Id { get; set; }
        public Nullable<double> Amount { get; set; }
        public bool IsPaid { get; set; }

    }





    //Lab section =================//
    public class LAbHISBOOK
    {
        public int id { get; set; }
        public string PatientName { get; set; }
        public string MobileNumber { get; set; }
        public string Location { get; set; }
        public Nullable<Double> Amount { get; set; }
        public TimeSpan StartSlotTime { get; set; }
        public TimeSpan EndSlotTime { get; set; }
    }


    public class LAbpaymentHistory
    {
        public int Id { get; set; }
        public string PatientName { get; set; }

        public string MobileNumber { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string SlotTime { get; set; }
    }


    public class LAbAppoinmentHistory
    {
        public int id { get; set; }
        public string PatientName { get; set; }
        public string MobileNumber { get; set; }
        public string Location { get; set; }
        public string CityName { get; set; }
        public Nullable<Double> Amount { get; set; }
        public string SlotTime { get; set; }
        //public Nullable<System.TimeSpan> StartSlotTime { get; set; }
        //public Nullable<System.TimeSpan> EndSlotTime { get; set; }
    }


   public class LabUpdate
    {
        public int id { get; set; }
        public string LabName { get; set; }
        public string MobileNumber { get; set; }
        public int StateMaster_Id { get; set; }
        public int CityMaster_Id { get; set; }
        public string Location { get; set; }
        public Nullable<double> fee { get; set; }
        public string PinCode { get; set; }

        public int adminLogin_id { get; set; }
        public string AccountNo { get; set; }
        public string IFSCCode { get; set; }
        public string BranchName { get; set; }


    }


    public class labprofileDetails
    {
        public int Id { get; set; }
        public string LabName { get; set; }
        public string MobileNumber { get; set; }

        public string EmailId { get; set; }

        public string StateName { get; set; }
        public string CityName { get; set; }
        public string PinCode { get; set; }

        public string Location { get; set; }
    }


    public class labAPTHISTORY
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string MobileNumber { get; set; }
        public string Location { get; set; }
        public Nullable<Double> Amount { get; set; }

        public string SlotTime { get; set; }

    }

    public class upload_report
    {
        public Nullable<int> Lab_Id { get; set; }
        public Nullable<int> Patient_Id { get; set; }
        public Nullable<int> Test { get; set; }
        public string File { get; set; }
        public string Filebase64 { get; set; }

        //public HttpPostedFileBase FileName { get; set; }

    }

    public class Lab_View_Report
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string TestName { get; set; }
        public Nullable<System.DateTime> TestDate { get; set; }
        public string File { get; set; }

    }


    public class Lab_View_Report_File
    {
        public string File { get; set; }
    }


    public class LabModel1
    {
        public int? Id { get; set; }
        public string LabName { get; set; }
        public string TestName { get; set; }
        public DateTime? TestDate { get; set; }
        public double? TestAmount { get; set; }
        public string Location { get; set; }
        public string InvoiceNumber { get; set; }
        public string OrderId { get; set; }

    }

    public class lablist
    {
        public int Id { get; set; }
        public string TestName { get; set; }
    }
    public class Test_name
    {
        public int Id { get; set; }
        public string TestDesc { get; set; }
    }


}