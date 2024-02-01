using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.APIModels
{
    public class DoctorListVM
    {
        public string Message { get; set; }
        public int Status { get; set; }
        public IEnumerable<ListOfDoctor> list { get; set; }
    }

    public class ListOfDoctor
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public string DoctorName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        //public int StateMaster_Id { get; set; }
        //public int CityMaster_Id { get; set; }
        public string Location { get; set; }
        //public string LicenceImage { get; set; }
        public string LicenceNumber { get; set; }
        //public string ClinicName { get; set; }
        public int Department_Id { get; set; }
        public int Specialist_Id { get; set; }
        public bool IsApproved { get; set; }
        public string PAN { get; set; }
        public string AadharNumber { get; set; }
        public string DepartmentName { get; set; }
        public string SpecialistName { get; set; }
        public string AvailableTime { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }

    }
}