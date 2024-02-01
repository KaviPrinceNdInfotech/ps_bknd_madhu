using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class BookLabClass
    {
        public int Id { get; set; }
        [Required]
        public Nullable<int> Lab_Id { get; set; }
        [Required]
        public Nullable<int> Test_Id { get; set; }
        public Nullable<int> Patient_Id { get; set; }
        public string PatientName { get; set; }
        [Required]
        public string PatientAddress { get; set; }
        [Required]
        public string ContactNumber { get; set; }
        [Required]
        public Nullable<System.DateTime> TestDate { get; set; }
        [Required]
        public Nullable<System.TimeSpan> AvailabelTime1 { get; set; }
        [Required]
        public Nullable<System.TimeSpan> AvailableTime2 { get; set; }
        public double Amount { get; set; }

        public int StateMaster_Id { get; set; }
        public int CityMaster_Id { get; set; }
        public List<LabL> Patient { get; set; }
    }


    public class LabL
    {
        public string PatientName { get; set; }
    }
    // public class BookNow
    //{
    //    public string PatientName { get; set; }
    //    public string PatientAddress { get; set; }
    //    public string LabTypeName { get; set; }

    //    public Nullable<System.DateTime> TestDate { get; set; }
    //}
        
}