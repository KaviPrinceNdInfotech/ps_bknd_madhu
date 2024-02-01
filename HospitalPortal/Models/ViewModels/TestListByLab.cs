using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class TestListByLab
    {
        public IEnumerable<TestsList> TestsList { get; set; }
        public int? labId { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
    }

    public class TestsList
    {
        public int? Test_Id { get; set; }
        public string TestName { get; set; }
        public double? TestAmount { get; set; }
        public string TestDescription { get; set; }
        public string TestDesc { get; set; }
        public int? Lab_Id { get; set; }
        public int? LabId { get; set; }
        public int Id { get; set; }
    }
}