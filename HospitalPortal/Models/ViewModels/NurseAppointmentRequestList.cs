using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class NurseAppointmentRequestList
    {
        public int Id { get; set; }
        public string LocationName { get; set; }
        public int? Nurse_Id { get; set; }
        public int Patient_Id { get; set; }
        public string MobileNumber { get; set; } //payment history
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ServiceDate { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaymentDate { get; set; }  //payment history
        public DateTime? ServiceAcceptanceDate { get; set; }
        public DateTime RequestDate { get; set; }
        public string PatientName { get; set; } //payment history
        public string ServiceStatus { get; set; }
        public string ServiceType { get; set; }
        public string ServiceTime { get; set; }

        public string Amount { get; set; }  //payment history
    }

    public class NurseAppointmentList
    {
        public string ServiceTiming { get; set; }
        public int Id { get; set; }
        public int? Nurse_Id { get; set; }
        public int? Patient_Id { get; set; }
        public string MobileNumber { get; set; }
        public bool IsPaid { get; set; }
        public string PaymentDate { get; set; }
        public string ServiceAcceptanceDate { get; set; }
        public string RequestDate { get; set; }
        public string NurseName { get; set; }
        public string NurseId { get; set; }
        public string NurseMobileNumber { get; set; }
        public string ServiceStatus { get; set; } //nurse history
        public int? TotalDays { get; set; } //nurse history
        public double? TotalFee { get; set; }
        public double? Fee { get; set; }
        public double? Amountwithrazorpaycomm { get; set; }
        public string ServiceType { get; set; }  //nurse history
        public string ServiceTime { get; set; }  //nurse history
        public string ServiceDate { get; set; } 
	}

    public class NurseAppointmentWithUser
    {
        public int? Id { get; set; }
        public string ServiceType { get; set; }
        public string ServiceTime { get; set; }
        
        public string ServiceStatus { get; set; }
        public int? Nurse_Id { get; set; }
        public string PatientName { get; set; }
        public string RegisteredMobileNumber { get; set; }
        public string ContactNumber { get; set; }
        public string ServiceTiming { get; set; }
        public string ServiceDate { get; set; }
        public bool IsPaid { get; set; }
        public string PaymentDate { get; set; }
        public string ServiceAcceptanceDate { get; set; }
        public string RequestDate { get; set; }
        public DateTime rDate { get; set; }
        public int? TotalDays { get; set; }
        //public double Fee { get; set; }
        public double TotalFee { get; set; }

        public string Location { get; set; }
        public string DeviceId { get; set; }
        
    }



    public class PAYMENTHISTORY
    {
        public int Id { get; set; }
        public string PatientName { get; set; }

        public string MobileNumber { get; set; }
        public double? Amount { get; set; }
        public DateTime? PaymentDate { get; set; }   
    }
}