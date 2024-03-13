using HospitalPortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace HospitalPortal.Models.APIModels
{
    public class NurseAppointmentHistory

    {
        public int id { get; set; }
        public string ServiceTime { get; set; }

        public string ServiceType { get; set; }

        public string ServiceStatus { get; set; }

        public int? TotalDays { get; set; }
    }

    public class AppoinmentHistory
    {
        public int id { get; set; }
        public string PatientName { get; set; }
        public string MobileNumber { get; set; }
        public string Location { get; set; }
        public Nullable<Double> Amount { get; set; }
        public TimeSpan StartSlotTime { get; set; }
        public TimeSpan EndSlotTime { get; set; }
       
    }

    public class NursePatientList
    {
        public int? Id { get; set; }
        public string PatientName { get; set; }
        public string MobileNumber { get; set; }
        public string Location { get; set; }
        public double Fee { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }

    }

    public class profile
    {
        public int id { get; set; }
        //public int? StateMaster_Id { get; set; }
        public string StateName { get; set; }
        //public int? CityMaster_Id { get; set; }
        public string CityName { get; set; }
    }

    // //New API For Get NurseUpdate  By Anchal Shukla on 20/4/2023
    public class NurseUpdate
    {
        public int Id { get; set; }

        public string NurseName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string ClinicName { get; set; }

        public int StateMaster_Id { get; set; }

        public int CityMaster_Id { get; set; }

        public string Location { get; set; }

        public double Fee { get; set; }

        public string PinCode { get; set; }

        

    }


    public class TimeSlotA
    {
        public IEnumerable<TimeSlots> TimeSlots { get; set; }
    }
    public class TimeSlots
    {
        public int Slotid { get; set; }
        public string SlotTime { get; set; }

    }


    public class Labrepo
    {
        public int Id { get; set; }
        public string PatientName { get; set; }

    }


    public class Nurse_uploadreport
    {
        public Nullable<int> Nurse_Id { get; set; }
        public Nullable<int> Patient_Id { get; set; }
        public string File { get; set; }
        public string Filebase64 { get; set; }

    }

    public class Nurse_View_Report
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string File { get; set; }

    }

    public class NurseAbout
    {
        public int Id { get; set; }
        public string About { get; set; }
    }

    public class Nurse_View_Report_File
    {
        public string File { get; set; }
    }
    public class Apointmenthsitory
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string MobileNumber { get; set; }
        public string Location { get; set; }
        public Nullable<double> TotalFee { get; set; }
        public Nullable<System.DateTime> ServiceDate { get; set; }
        public string SlotTime { get; set; }

    }

}
