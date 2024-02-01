using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class HealthCheckUpVM
    {
       
        public IEnumerable<ViewMoreHealth> ViewMoreHealth { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class HealthCheckUpList
    {
        public int Id { get; set; }
        public string LabName { get; set; }
        public string MobileNumber { get; set; }
        public string CityName { get; set; }
        public string LocationName { get; set; }
        public Nullable<double> TestAmount { get; set; }

    }

    public class ViewMoreHealth
    {
        public int Id { get; set; }
        public string LabName { get; set; }
        public string LocationName { get; set; }
        public string year { get; set; }
        public string About { get; set; }
        public string HealthType { get; set; }
    }



    public class HealthCheckUpBooked
    {
        public int StateMaster_Id { get; set; }
        public int CityMaster_Id { get; set; }
        public int testId { get; set; }
    }

    public class HealthCheckUpV
    {
        public IEnumerable<HealthCheckUpList> HealthCheckupList { get; set; }
        public string Message { get; set; }
    }



    public class HealthBooknow
    {
        public int Id { get; set; }
        public Nullable<int> Test_Id { get; set; }

        public Nullable<int> Slotid { get; set; }
        public Nullable<System.DateTime> TestDate { get; set; }

    }


    public class HealthDet
    {
        public int Id { get; set; }
        public string LabName { get; set; }
        public string HealthType { get; set; }

        public Nullable<double> TestAmount { get; set; }
        public Nullable<System.DateTime> TestDate { get; set; }
        public string SlotTime { get; set; }

    }


    public class HealthPayNow
    {
        public int Id { get; set; }
        public Nullable<int> Test_Id { get; set; }
        public int Patient_Id { get; set; }
        public Nullable<double> TotalAmount { get; set; }
        public bool IsPaid { get; set; }

    }
}