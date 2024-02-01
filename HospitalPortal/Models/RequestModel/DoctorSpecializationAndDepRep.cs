using HospitalPortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.RequestModel
{
    public class DoctorSpecializationAndDepRep
    {
        [Required]
        public int DoctorId { get; set; }
        public IEnumerable<DepModel> Departments { get; set; }
    }


    //choose department /////
    //
    ////{
    //    [Required]
    //    public int Id { get; set; }
       
    //    public int Department_id { get; set; }
    //    public int Specialist_id { get; set; }
    //    public int State_id { get; set; }
    //    public int City_id { get; set; }
    //    public int Patient_id { get; set; }
    //}

}