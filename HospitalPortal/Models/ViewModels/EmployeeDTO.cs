using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class EmployeeDTO
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        public string EmployeetName { get; set; }
        [Required]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string MobileNumber { get; set; }
        [Required,EmailAddress]
        public string EmailId { get; set; }
        [Required]
        public string EmployeeAddress { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public double Salary { get; set; }
        [Required]
        public string EmployeeId { get; set; }
    }
}