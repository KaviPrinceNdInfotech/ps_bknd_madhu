using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class RegisterNurseLocationApi
    {
        public int NurseId { get; set; }

        public List<LocationsId> LocationIds { get; set; }
    }

    public class LocationsId
    {
        public int LocationId { get; set; }
    }
}