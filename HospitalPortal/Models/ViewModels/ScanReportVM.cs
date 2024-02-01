using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class ScanReportVM
    {
        public string LabName { get; set; }
        public SelectList StateList { get; set; }
        public SelectList CityList { get; set; }
        public SelectList Location { get; set; }
        public int Location_Id { get; set; }
        public int CityMaster_Id { get; set; }
        public int StateMaster_Id { get; set; }
        public string DoctorName { get; set; }
        public string HospitalName { get; set; }
        public IEnumerable<ViewLabReport> LabList { get; set; }
        public IEnumerable<ViewHealthReport> HealthList { get; set; }
        public IEnumerable<ViewDoctorReport> DoctorList { get; set; }
        public IEnumerable<ViewHospitalReport> HospitalList { get; set; }
    }

    public class ViewLabReport
    {
        public int Id { get; set; }
        public string LabName { get; set; }
    }

    public class ViewHealthReport
    {
        public int Id { get; set; }
        public string LabName { get; set; }
    }

    public class ViewDoctorReport
    {
        public int Id { get; set; }
        public string DoctorName { get; set; }
    }

    public class ViewHospitalReport
    {
        public int Id { get; set; }
        public string HospitalName { get; set; }
    }
}