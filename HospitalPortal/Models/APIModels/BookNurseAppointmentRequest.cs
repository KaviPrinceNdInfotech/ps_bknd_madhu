using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.APIModels
{
    public class BookNurseAppointmentRequest
    {
        [Required]
        public int NurseType_Id { get; set; }
        [Required]
        public int PatientId { get; set; }
        [Required]
        public string Mobile { get; set; }
       
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public int LocationId { get; set; }
    }
}