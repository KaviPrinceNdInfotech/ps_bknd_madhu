using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class CheckUpReportDTO
    {
        public string PatientRegNo { get; set; }
        [Required]
        public int Patient_Id { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]
        public string PatientName { get; set; }
        [Required]
        public HttpPostedFileBase[] File { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; }
        public List<UploadHealthReport> UploadHealthReport { get; set; }
        public CheckUpReportDTO()
        {
            UploadHealthReport = new List<UploadHealthReport> { new UploadHealthReport { TestId = 0 } };
        }
    }

    public class UploadHealthReport
    {
        public string PatientRegNo { get; set; }
        [Required]
        public int Patient_Id { get; set; }
        public int TestId { get; set; }
        public int Id { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]
        public string PatientName { get; set; }
        [Required]
        public HttpPostedFileBase[] File { get; set; }
        public string TestName { get; set; }
    }
}