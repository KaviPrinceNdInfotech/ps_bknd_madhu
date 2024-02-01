using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class LabVM
    {
        
        public IEnumerable<LabListItems> LabList { get; set; }      
        
        public IEnumerable<LabListViaTest_VM> LabListViaTest { get; set; }
        
        public string Message { get; set; }

        public int Status { get; set; }
    }


    public class LabV
    {

        public IEnumerable<LabBookings> LabBookings { get; set; }

    }





    public class choosedoc
    {

        public IEnumerable<Doctorchoose> Doctorchoose { get; set; }

    }

}