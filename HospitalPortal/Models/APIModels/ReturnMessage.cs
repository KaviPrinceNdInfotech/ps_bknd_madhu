using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class ReturnMessage
    {
        public int Status { get; set; }
        public string Message { get; set; }
    }
}