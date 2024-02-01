using System.Collections.Generic;
using System;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class HospitalFicilityVM
    {
        public string Message { get; set; }
        public int Status { get; set; }
        public IEnumerable<HospitalFacilities> HospitalFacilities { get; set; }
    }


    public class HospitalFacilities
    {
        public int Id { get; set; }
        public int Hospital_Id { get; set; }
        public string FacilityName { get; set; }
        public bool IsDeleted { get; set; }
    }
}