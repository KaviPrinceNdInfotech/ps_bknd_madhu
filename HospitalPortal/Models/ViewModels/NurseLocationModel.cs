using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{   
    public class NurseLocationModel
    {
        
        public int Id { get; set; }
        public int Nurse_Id { get; set; }
        public string LocationName { get; set; }
       
    }
}