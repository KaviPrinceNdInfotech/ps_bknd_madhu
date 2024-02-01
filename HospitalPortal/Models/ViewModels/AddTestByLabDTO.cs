using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Models.ViewModels
{
    public class AddTestByLabDTO
    {
        public string TestName { get; set; }
        public bool IsTestAvailable { get; set; }
        public int Id { get; set; }
        public int? Lab_Id { get; set; }
        public int? Test_Id { get; set; }
        public double TestAmount { get; set; }
        public string TestDescription { get; set; }
        public string TestDesc { get; set; }
        public SelectList Tests { get; set; }
        public SelectList TestNameList { get; set; }
        public SelectList LabNameList { get; set; }
        public int? TestNameId { get; set; }
       
        public IEnumerable<TestCategoryList> TestCategoryList { get; set; }
    }

    public class TestCategoryList{
        public int Id { get; set; }
        public string TestName { get; set; }
        public string LabName { get; set; }
    }
}