using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    public class HealthPackageController : Controller
    {
        // GET: HealthPackage
        public ActionResult Test()
        {

            //Assign Values
            Person p1 = new Person();
            p1.Age = 42;
            p1.name = "Raghav";
            p1.IdInfo = new IdInfo(101);
            Person p2 = p1.ShallowCopy();
            object p = p2;
            return Content(p.ToString());
        }

        public class IdInfo
        {
            public int Id { get; set; }
            public IdInfo(int Id)
            {
                this.Id = Id;
            }
        }

        public class Person
        {
            public int Id { get; set; }
            public string name { get; set; }
            public int Age { get; set; }
            public IdInfo IdInfo;
            public Person ShallowCopy()
            {
                return (Person)this.MemberwiseClone();
            }
        }
     
        

        
    }
}