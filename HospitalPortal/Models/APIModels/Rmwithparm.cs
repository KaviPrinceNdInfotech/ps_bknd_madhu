using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class Rmwithparm
    {
        public int orderId { get; set; }
        public int TestId { get; set; }
        public int Checkup_Id { get; set; }
        public int AppointmentId { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }
}