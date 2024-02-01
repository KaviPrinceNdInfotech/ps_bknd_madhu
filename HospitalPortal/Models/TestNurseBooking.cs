using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models
{
    public class TestNurseBooking
    {

        [Required]
        public string ServiceType { get; set; }
        public string ServiceTime { get; set; }
        [Required]
        public int NurseType_Id { get; set; }
        [Required]
        public int PatientId { get; set; }
        [Required]
        public string Mobile { get; set; }
        public DateTime? ServiceDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required]
        public int LocationId { get; set; }
    }
    public class NurseBook
    {
        public int Id { get; set; }
        public Nullable<int> NurseTypeId { get; set; }
        public string ServiceType { get; set; }
        public string ServiceTime { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public int Patient_Id { get; set; }
        public string MobileNumber { get; set; }
        public string Location { get; set; }
        public Nullable<int> LocationId { get; set; }
        public Nullable<int> StateMaster_Id { get; set; }
        public Nullable<int> CityMaster_Id { get; set; }


    }

    public class NurseBooking
    {
        public int Id { get; set; }
        public Nullable<int> Nurse_Id { get; set; }
        public Nullable<System.DateTime> ServiceDate { get; set; }
        public Nullable<int> Slotid { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
    }
        public class NursePayNow
        {
            public int Id { get; set; }
            public Nullable<int> Nurse_Id { get; set; }
            public int Patient_Id { get; set; }
            public Nullable<double> TotalFee { get; set; }
            public bool IsPaid { get; set; }

        }





    public class DoctorPayNow
    {
        public int Id { get; set; }
        public Nullable<int> Doctor_Id { get; set; }
        public int Patient_Id { get; set; }
        public Nullable<double> TotalFee { get; set; }
        public bool IsPaid { get; set; }

    }
    public class NursebookingResponse
    {
        public int NurseBookingId { get; set; }
        public string Message { get; set; }
    }

    public class Doctorupload_report
    {
        public Nullable<int> Doctor_Id { get; set; }
        public Nullable<int> Patient_Id { get; set; }
        public string Image1 { get; set; }
        public string Image1Base64 { get; set; }
        public Nullable<System.DateTime> UploadDate { get; set; }

    }

    public class DoctorViewReport
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string Image1 { get; set; }
    }

    public class DoctorViewReportFile
    {
        public string Image1 { get; set; }
    }

    public class DocAbout
    {
        public string About { get; set; }
    }
}
