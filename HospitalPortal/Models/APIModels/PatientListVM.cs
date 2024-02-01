using HospitalPortal.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.APIModels
{
    public class PatientListVM
    {
        public IEnumerable<PatientListApis> patients { get; set; }

    }


    public class PatientListp
    {

        public IEnumerable<PatientprofileDetail> Patientprofile { get; set; }

    }
    //nurse history ///
    public class NurseHistory
    {
        public IEnumerable<NurseAppointmentDetail> NurseAppointments { get; set; }
    }

    public class NurseAppointmentDetail
    {
        
        public int ID { get; set; }
        public string NurseName { get; set; }
        public string Location { get; set; }

        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }

        public double Fee { get; set; }
        public Nullable<Double> TotalFee { get; set; }
        public int? TotalNumberofdays { get; set; }
        public string DeviceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string OrderId { get; set; }
    }

    public class PatientprofileDetail
    {
        public int id { get; set; }

        public string PatientName { get; set; }
        public string EmailId { get; set; }
        public string MobileNumber { get; set; }
        public string Location { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string Pincode { get; set; }
    }


    public class PatientListApis
    {
        public string PatientName { get; set; }
        public string EmailId { get; set; }
        public string MobileNumber { get; set; }
        public string Location { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string PatientRegNo { get; set; }
    }
}