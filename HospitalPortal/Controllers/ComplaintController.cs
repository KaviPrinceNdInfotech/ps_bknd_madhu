using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ComplaintController : Controller
    {
        DbEntities ent = new DbEntities();
        // GET: Complaint
        
        public ActionResult Complaint()
        {
            
            return View();
        }

        public ActionResult Doctor(string term, DateTime? date = null)
        {
            var model = new ComplaintVM();
            //int AdminId = ent.Database.SqlQuery<int>(@"select Id from AdminLogin where Role=" + term).FirstOrDefault();
            //string qry = @"select * from Doctor where AdminLogin_Id=" + AdminId;
            //var data = ent.Database.SqlQuery<ComplaintVM>(qry).ToList();
            
            string qry = @"select * from dbo.PatientComplaints pc join AdminLogin al on al.Id = pc.Login_Id where patsubid='" + term+ "' and Month(pc.ComplaintDate) = Month(GetDate())";
            var data = ent.Database.SqlQuery<Complaint_Doc>(qry).ToList();
            if(data.Count() == 0)
            {
                TempData["msg"] = "No Compliants Registered, yet";
                return View(model);
            }
            if(date != null)
            {
                string q = @"select * from dbo.PatientComplaints pc join AdminLogin al on al.Id = pc.Login_Id where Subjects='" + term + "' and Month(pc.ComplaintDate) ="+date;
                var data1 = ent.Database.SqlQuery<Complaint_Doc>(q).ToList();
            }
            model.Complaint_Doc = data;
            return View(model);
        }

        public ActionResult ViewDetails(int? Id, string role)
        {
            var model = new ComplaintVM();
            //string role = ent.Database.SqlQuery<string>(@"select * from ") 
            //int AdminId = ent.Database.SqlQuery<int>(@"select Id from AdminLogin where Role='"+role+"'").FirstOrDefault();
            string UserRole = role;
            TempData["role"] = role;
            switch (UserRole){
                case "doctor":
                    string Doctor = @"select pc.Id as Id,* from Doctor join dbo.PatientComplaints pc on pc.Login_Id = Doctor.AdminLogin_Id where Doctor.AdminLogin_Id=" + Id;
                    var Doctordata = ent.Database.SqlQuery<Complaint_Doc>(Doctor).ToList();
                    model.Complaint_Doc = Doctordata;
                    break;
                case "hospital":
                    string hospital = @"select pc.Id as Id,* from Hospital join dbo.PatientComplaints pc on pc.Login_Id = Hospital.AdminLogin_Id where Hospital.AdminLogin_Id=" + Id;
                    var Hospitaldata = ent.Database.SqlQuery<Complaint_Hospital>(hospital).ToList();
                    model.Complaint_Hospital = Hospitaldata;
                    break;
                case "patient":
                    //int AdminId = ent.Database.SqlQuery<int>(@"select AdminLogin_Id from Patient where Id="+Id+"").FirstOrDefault();
                    string Patient = @"select pc.Id as Id,* from Patient join dbo.PatientComplaints pc on pc.Login_Id = Patient.AdminLogin_Id where pc.Id=" + Id;
                    var Patientdata = ent.Database.SqlQuery<Complaint_Pateint>(Patient).ToList();
                    model.Complaint_Patient = Patientdata;
                    break;
                case "driver":
                    string driver = @"select pc.Id as Id,* from Driver join dbo.PatientComplaints pc on pc.Login_Id = Driver.AdminLogin_Id where Driver.AdminLogin_Id=" + Id;
                    var Driverdata = ent.Database.SqlQuery<Complaint_Driver>(driver).ToList();
                    model.Complaint_Driver = Driverdata;
                    break;
                case "Ambulance":
                    string Ambulance = @"select pc.Id as Id,* from Ambulance join dbo.PatientComplaints pc on pc.Login_Id = Ambulance.AdminLogin_Id where Ambulance.AdminLogin_Id=" +Id;
                    var Ambulancedata = ent.Database.SqlQuery<Complaint_Ambulance>(Ambulance).ToList();
                   model.Complaint_Ambulance = Ambulancedata;
                    break;
            }
            return View(model);
        }

        public ActionResult UpdateStatus(int id, int Rating)
        {
            var lab = ent.PatientComplaints.Find(id);
            string role = Convert.ToString(TempData["role"]);
            string q = @"update PatientComplaints set Rating="+ Rating + ",IsResolved = case when IsResolved=0 then 1 else 0 end where Id="+id;
            ent.Database.ExecuteSqlCommand(q);
            string msg = "Your Complaint Has been Resolved.";
            switch (role)
            {
                case "doctor":
                    var mobile = ent.Database.SqlQuery<string>(@"select MobileNumber from Doctor join dbo.PatientComplaints pc on pc.Login_Id = Doctor.AdminLogin_Id where AdminLogin_Id=" + lab.Login_Id).FirstOrDefault();
                    Message.SendSms(mobile, msg);
                    break;
                case "hospital":
                    var hospital = ent.Database.SqlQuery<string>(@"select MobileNumber from Hospital join dbo.PatientComplaints pc on pc.Login_Id = Hospital.AdminLogin_Id where AdminLogin_Id=" + lab.Login_Id).FirstOrDefault();
                    Message.SendSms(hospital, msg);
                    break;
                case "patient":
                    var patient = ent.Database.SqlQuery<string>(@"select MobileNumber from Patient join dbo.PatientComplaints pc on pc.Login_Id = Patient.AdminLogin_Id where AdminLogin_Id=" + lab.Login_Id).FirstOrDefault();
                    Message.SendSms(patient, msg);
                    break;
                    case "driver":
                    var driver = ent.Database.SqlQuery<string>(@"select MobileNumber from Driver join dbo.PatientComplaints pc on pc.Login_Id = Driver.AdminLogin_Id where AdminLogin_Id=" + lab.Login_Id).FirstOrDefault();
                    Message.SendSms(driver, msg);
                    break;
            }
            return RedirectToAction("ViewDetails", "Complaint", new { role = role, Id= lab.Id });
        }

        public ActionResult ViewComp(string term,DateTime? ComplaintDate)
        {
            var model = new ComplaintVM();
            //int AdminId = ent.Database.SqlQuery<int>(@"select Id from AdminLogin where Role=" + term).FirstOrDefault();
            //string qry = @"select * from Doctor where AdminLogin_Id=" + AdminId;
            //var data = ent.Database.SqlQuery<ComplaintVM>(qry).ToList();
            string qry = @"select * from dbo.PatientComplaints pc join AdminLogin al on al.Id = pc.Login_Id";
            var data = ent.Database.SqlQuery<Complaint_Doc>(qry).ToList();
            
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Compliants Registered, yet";
                return View(model);
            }
            if (ComplaintDate != null)
            {
                string q = @"select * from dbo.PatientComplaints pc join AdminLogin al on al.Id = pc.Login_Id where  pc.ComplaintDate =Convert(datetime,'" + ComplaintDate + "',103)";
                var data1 = ent.Database.SqlQuery<Complaint_Doc>(q).ToList();
                model.Complaint_Doc = data1;
                return View(model);
            }
            if (term != null)
            {
                string q = @"select * from dbo.PatientComplaints pc join AdminLogin al on al.Id = pc.Login_Id where al.Role ='" +term+"'";
                var data1 = ent.Database.SqlQuery<Complaint_Doc>(q).ToList();
                model.Complaint_Doc = data1;
                return View(model);
            }
            model.Complaint_Doc = data;
            return View(model);
        }
        
        public ActionResult UpdateRating(ComplaintVM model)
        {
            return View(model);
        }
    }
}