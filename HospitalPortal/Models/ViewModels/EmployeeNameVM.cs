using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class EmployeeNameVM
    {
        public IEnumerable<Emp_Detail> Emp_Detail {get;set;}
    }

    public class Emp_Detail
    {
        public double CurrentSalary { get; set; }
        public double MonthSalary { get; set; }
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeetName { get; set; }
        public double Salary { get; set; }
        public bool IsPaid { get; set; }
        public DateTime GenerateDate { get; set; }
        public string EmailId { get; set; }
    }
}