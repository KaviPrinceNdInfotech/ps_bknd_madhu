using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Utilities;
using HospitalPortal.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HospitalPortal.Controllers
{
    
    public class HomeController : Controller
    {

        DbEntities ent = new DbEntities();
        EmailOperations mailbhejo = new EmailOperations();
        // GET: Home
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Services()
        {
            return View();
        }

        public ActionResult About(string a)
        {
            var model = new Content();
            var qry = @"select * from Content where PageName=" + a + "order by Id desc";
            var data = ent.Database.SqlQuery<AddContentVM>(qry).ToList();
            model.About = data.FirstOrDefault().About;
            model.PageName = data.FirstOrDefault().PageName;
            return View(model);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            try
            {
                var user = ent.Database.SqlQuery<AdminLogin>("select * from AdminLogin where UserID='" + model.UserID + "' and password='" + model.Password + "'").FirstOrDefault();
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.Id.ToString(), false);
                    UserIdentity.UserRole = user.Role;
                    if (user.Role != "admin")
                    {
                        if (IsUserApproved(user.Role, user.Id))
                            return RedirectToAction("Dashboard");
                        else
                        {
                            return IsLoginApproved(user.Role, user.Id);
                        }
                        // TempData["msg"] = "You are not approved yet.";
                    }
                    return RedirectToAction("Dashboard");
                }
                TempData["msg"] = "Invalid User Name or Password.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                TempData["msg"] = "Server Error.";
                return RedirectToAction("Login");
            }
        }


        bool IsUserApproved(string role, int loginId)
        {
            if (role == "Franchise")
            {
                var Franchise = ent.Vendors.FirstOrDefault(a => a.AdminLogin_Id == loginId && !a.IsDeleted);
                return Franchise.IsApproved;
            }
            else if (role == "chemist")
            {
                var chemist = ent.Chemists.FirstOrDefault(a => a.AdminLogin_Id == loginId && !a.IsDeleted);
                return chemist.IsApproved;
            }
            else if (role == "checkup")
            {
                var checkup = ent.HealthCheckupCenters.FirstOrDefault(a => a.AdminLogin_Id == loginId && !a.IsDeleted);
                return checkup.IsApproved;
            }
            else if (role == "doctor")
            {
                var doctor = ent.Doctors.FirstOrDefault(a => a.AdminLogin_Id == loginId && !a.IsDeleted);
                return doctor.IsApproved;
            }
            else if (role == "driver")
            {
                var driver = ent.Drivers.FirstOrDefault(a => a.AdminLogin_Id == loginId && !a.IsDeleted);
                return driver.IsApproved;
            }
            else if (role == "hospital")
            {
                var hospital = ent.Hospitals.FirstOrDefault(a => a.AdminLogin_Id == loginId && !a.IsDeleted);
                return hospital.IsApproved;
            }
            else if (role == "lab")
            {
                var lab = ent.Labs.FirstOrDefault(a => a.AdminLogin_Id == loginId && !a.IsDeleted);
                return lab.IsApproved;
            }
            else if (role == "nurse")
            {
                var nurse = ent.Nurses.FirstOrDefault(a => a.AdminLogin_Id == loginId && !a.IsDeleted);
                return nurse.IsApproved;
            }
            else if (role == "RWA")
            {
                var RWA = ent.RWAs.FirstOrDefault(a => a.AdminLogin_Id == loginId && !a.IsDeleted);
                return RWA.IsApproved;
            }
            else if (role == "patient")
            {
                var patient = ent.Patients.FirstOrDefault(a => a.AdminLogin_Id == loginId && !a.IsDeleted);
                return patient.IsApproved;
            }
            else
                return false;

        }

        public ActionResult IsLoginApproved(string role, int loginId)
        {
            if (role == "Franchise")
            {
                var Franchise = ent.Vendors.FirstOrDefault(a => a.AdminLogin_Id == loginId && !a.IsDeleted);
                return RedirectToAction("Edit", "Vendor", new { id = Franchise.Id });
            }
            else if (role == "chemist")
            {
                var chemist = ent.Chemists.FirstOrDefault(a => a.AdminLogin_Id == loginId && !a.IsDeleted);
                return RedirectToAction("Edit", "Chemist", new { id = chemist.Id });

            }
            else if (role == "checkup")
            {
                var checkup = ent.HealthCheckupCenters.FirstOrDefault(a => a.AdminLogin_Id == loginId && !a.IsDeleted);
                return RedirectToAction("Edit", "CompletHealthCheckup", new { id = checkup.Id });
            }
            else if (role == "doctor")
            {
                var doctor = ent.Doctors.FirstOrDefault(a => a.AdminLogin_Id == loginId && !a.IsDeleted);
                return RedirectToAction("Edit", "DoctorRegistration", new { id = doctor.Id });
            }
            else if (role == "driver")
            {
                var driver = ent.Drivers.FirstOrDefault(a => a.AdminLogin_Id == loginId && !a.IsDeleted);
                return RedirectToAction("Edit", "Driver", new { id = driver.Id });
            }
            else if (role == "hospital")
            {
                var hospital = ent.Hospitals.FirstOrDefault(a => a.AdminLogin_Id == loginId && !a.IsDeleted);
                return RedirectToAction("Edit", "Hospital", new { id = hospital.Id });
            }
            else if (role == "nurse")
            {
                var nurse = ent.Nurses.FirstOrDefault(a => a.AdminLogin_Id == loginId && !a.IsDeleted);
                return RedirectToAction("Edit", "Nurse", new { id = nurse.Id });
            }
            else if (role == "RWA")
            {
                var RWA = ent.RWAs.FirstOrDefault(a => a.AdminLogin_Id == loginId && !a.IsDeleted);
                return RedirectToAction("Edit", "Rwa", new { id = RWA.Id });
            }
            else if (role == "patient")
            {
                var patient = ent.Patients.FirstOrDefault(a => a.AdminLogin_Id == loginId && !a.IsDeleted);
                return RedirectToAction("Edit", "Patient", new { id = patient.Id });
            }
            else
            {
                var Lab = ent.Labs.FirstOrDefault(a => a.AdminLogin_Id == loginId && !a.IsDeleted);
                return RedirectToAction("Edit", "Lab", new { id = Lab.Id });
            }
        }
        [HttpGet]
        public ActionResult MailUs()
        {
            return View();
        }


        [HttpPost]
        public ActionResult MailUs(MailVM model)
        {
            if (!ModelState.IsValid)
            {
                TempData["msg"] = "Erorr";
                return RedirectToAction("MailUs");
            }
            else
            {
                var Name = model.Name;
                var TelePhone = model.TelePhone;
                var Email = model.Email;
                var Subject = model.Subjects;
                var Msg = "" +model.Msg+ "<br /> Name:"+model.Name+ " <br /> Contact No."+model.TelePhone+" <br /> ReplyTo: " + model.Email+"";
                string m = "Name:" + Name + "<br/>";
                m += "TelePhone:" + TelePhone + "<br/>";
                m += "Email:" + Email + "<br/>";
                m += "Subject:" + Subject + "<br/>";
                m += "Message" + Msg + "<br/>";
                bool result = EmailOperations.SendEmail("" + Email + "", ""+Subject+"",""+Msg+"" ,true);
                TempData["msg"] = "Your Request has been Successfully Sent to Administrator.";
                return RedirectToAction("MailUs");
            }
        }

        public ActionResult ForgotPassword(string detail)
        {
            using (var client = new HttpClient())
            {
                //Call BaseAddress of API
                client.BaseAddress = new Uri("http://localhost:55405/api/");
                //Call WebApi Address With  Controller Name and ActionName + Parameters (If Any)
                var response = client.GetAsync("ForgotPassword/ForgetPwd?Email=" + detail);
                response.Wait();
                //Store the Response Result
                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    return View(); 
                    
                }
                else
                {
                    
                }
            }
            return View();
        }


        public ActionResult TermsCondition()
        {
            return View();
        }

        public ActionResult SignIn()
        {
            return View();
        }
        [HttpGet]
        public ActionResult getBanner()
        {
            return View();
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }

    }
}