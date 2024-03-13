using AutoMapper;
using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.RequestModel;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Utilities;
using HospitalPortal.Utility;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HospitalPortal.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
         ILog log = LogManager.GetLogger(typeof(AdminController));
        private DbEntities ent = new DbEntities();

        public ActionResult Dashboard()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            try
            {
                var user = ent.Database.SqlQuery<AdminLogin>("select * from AdminLogin where UserId='" + model.UserID + "' and password='" + model.Password + "'").FirstOrDefault();
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

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
       

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Content("Any fields can not be blank.");
                if (model.Password != model.ConfirmPassword)
                {
                    return Content("your password and confirm password are not same");
                }

                int id = Convert.ToInt32(User.Identity.Name);
                if (id > 0)
                {
                    var record = ent.AdminLogins.Find(id);
                    if (record != null)
                    {
                        record.Password = model.Password;
                        ent.SaveChanges();
                        return Content("ok");
                    }
                }
                return Content("Some error has occured.");
            }
            catch (Exception ex)
            {
              log.Error(ex.Message + " and inner exception : " + ex.InnerException);
              return Content("Server error.");
            }

        }
        public ActionResult ChangeTransactionPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangeTransactionPassword(ChangePasswordModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    TempData["msg"]="Any fields can not be blank.";
                if (model.TransactionPwd != model.ConfirmPassword)
                {
                    TempData["msg"]="your password and confirm password are not same";
                    return RedirectToAction("ChangeTransactionPassword");
                }

                int id = Convert.ToInt32(User.Identity.Name);
                if (id > 0)
                {
                    var record = ent.AdminLogins.Find(id);
                    if (record != null)
                    {
                        record.TransactionPwd = model.TransactionPwd;
                        ent.SaveChanges();
                        TempData["msg"] = "ok";
                    }
                }
                return RedirectToAction("ChangeTransactionPassword");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + " and inner exception : " + ex.InnerException);
                return Content("Server error.");
            }

        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            UserIdentity.UnAssignUserRole();
            return RedirectToAction("Login","Home");
        }

        bool IsUserApproved(string role,int loginId)
        {
            if (role== "Franchise")
            {
                var Franchise = ent.Vendors.FirstOrDefault(a => a.AdminLogin_Id ==loginId && !a.IsDeleted);
                return Franchise.IsApproved;
            }
            else if (role== "chemist")
            {
                var chemist = ent.Chemists.FirstOrDefault(a => a.AdminLogin_Id ==loginId && !a.IsDeleted);
                return chemist.IsApproved;
            }
            else if (role== "checkup")
            {
                var checkup = ent.HealthCheckupCenters.FirstOrDefault(a => a.AdminLogin_Id ==loginId && !a.IsDeleted);
                return checkup.IsApproved;
            }
            else if (role== "doctor")
            {
                var doctor = ent.Doctors.FirstOrDefault(a => a.AdminLogin_Id ==loginId && !a.IsDeleted);
                return doctor.IsApproved;
            }
            else if (role== "driver")
            {
                var driver = ent.Drivers.FirstOrDefault(a => a.AdminLogin_Id ==loginId && !a.IsDeleted);
                return driver.IsApproved;
            }
            else if (role== "hospital")
            {
                var hospital = ent.Hospitals.FirstOrDefault(a => a.AdminLogin_Id ==loginId && !a.IsDeleted);
                return hospital.IsApproved;
            }
            else if (role== "lab")
            {
                var lab = ent.Labs.FirstOrDefault(a => a.AdminLogin_Id ==loginId && !a.IsDeleted);
                return lab.IsApproved;
            }
            else if (role== "nurse")
            {
                var nurse = ent.Nurses.FirstOrDefault(a => a.AdminLogin_Id ==loginId && !a.IsDeleted);
                return nurse.IsApproved;
            }
            else if (role== "RWA")
            {
                var RWA = ent.RWAs.FirstOrDefault(a => a.AdminLogin_Id ==loginId && !a.IsDeleted);
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
        public ActionResult AddContent()
        {
            var model = new AddContentVM();
            model.PageNameList = new SelectList(ent.PageMasters.Where (a=>a.IsDeleted==false ).ToList(), "Id", "PageName");
            return View(model);
        }

        public ActionResult Call(int term)
        {
            var model = new AddContentVM();
            var selectedItem = ent.PageMasters.Where(a => a.Id == term).FirstOrDefault().PageName;
            if (selectedItem == "About Us")
            {
                //string q = @"select Top 1 Content from Content where Name='AboutUs' order by Id desc";
                string data = ent.Database.SqlQuery<string>("select Top 1 About from Content where PageName = 'AboutUs' order by Id desc").FirstOrDefault();
                model.About = data;
            }
            if (selectedItem == "Support")
            {
                string data = ent.Database.SqlQuery<string>("select Top 1 About from Content where PageName = 'Support' order by Id desc").FirstOrDefault();
                model.About = data;
            }
            return Json(model.About, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddContent (AddContentVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                model.PageNameList = new SelectList(ent.PageMasters.ToList(), "Id", "PageName");
                var selectedItem = ent.PageMasters.Where(a => a.Id == model.Id).FirstOrDefault().PageName; 
                string[] PageCode = { selectedItem };
                if (!PageCode.Contains(selectedItem))
                {
                    TempData["msg"] = "You can not create page without our permision";
                    return RedirectToAction("AddContent");

                }
                var page = Mapper.Map<Content>(model);
                page.PageName = selectedItem;
                //if (model.Id < 1)
                    ent.Contents.Add(page);
                //else
                //    ent.Entry(page).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                TempData["msg"] = "Record has saved";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("AddContent");
        }

        [HttpGet]
        public ActionResult ShareToVendor(int Id, string Role)
        {
            var model = new SharetoVendorDTO();
            model.Id = Id;
            model.Role = Role;
            return View(model);
        }
        
        [HttpPost]
        public ActionResult ShareToVendor(SharetoVendorDTO  model)
        {
            //var list = ent.Vendors.Where(a => a.VendorName == model.VendorName).ToList();
            //int VendorId = list.FirstOrDefault().Id;
            if (model.Role == "Doctor")
            {
                string q = "Update Doctor set Vendor_Id = "+model.Vendor_Id+" where Id ="+model.Id+"";
                ent.Database.ExecuteSqlCommand(q);
            }
            else if (model.Role == "Nurse")
            {
                string q = "Update Nurse set Vendor_Id = " + model.Vendor_Id + " where Id =" + model.Id + "";
                ent.Database.ExecuteSqlCommand(q);
            }
            else if (model.Role == "Lab")
            {
                string q = "Update Lab set Vendor_Id = " + model.Vendor_Id + " where Id =" + model.Id + "";
                ent.Database.ExecuteSqlCommand(q);
            }
            else if (model.Role == "HealthCheckUp")
            {
                string q = "Update HealthCheckupCenter set Vendor_Id = " + model.Vendor_Id + " where Id =" + model.Id + "";
                ent.Database.ExecuteSqlCommand(q);
            }
            else if (model.Role == "Chemist")
            {
                string q = "Update Chemist set Vendor_Id = " + model.Vendor_Id + " where Id =" + model.Id + "";
                ent.Database.ExecuteSqlCommand(q);
            }
            else if (model.Role == "Driver")
            {
                string q = "Update Driver set Vendor_Id = " + model.Vendor_Id + " where Id =" + model.Id + "";
                ent.Database.ExecuteSqlCommand(q);
            }
            else if (model.Role == "Vehicle")
            {
                string q = "Update Vehicle set Vendor_Id = " + model.Vendor_Id + " where Id =" + model.Id + "";
                ent.Database.ExecuteSqlCommand(q);
            }
            else if (model.Role == "Patient")
            {
                string q = "Update Patient set vendorId = " + model.Vendor_Id + " where Id =" + model.Id + "";
                ent.Database.ExecuteSqlCommand(q);
            }
            else
            {
                TempData["msg"] = "Some Error";
                return View(model);
            }
            TempData["msg"] = "Successfully Shared.";
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public ActionResult TransactionLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TransactionLogin(TrasnsactionPwdRequestModel model)
        {
            try
            {
                var data = ent.AdminLogins.Where(a => a.TransactionPwd == model.Password && a.Role == "admin").FirstOrDefault();
                if (data != null)
                {
                    return Content("ok");
                }
                else
                {
                    return Content("Invalid Transaction Password");
                }
            }
            catch (Exception ex)
            {
                return Content("Server Error");
            }
        }

        

        [Authorize(Roles = "admin")]
        public ActionResult Payout(bool IsApproved = false)
        {
            if(IsApproved == false)
            {
                TempData["msg"] = "You are not Authorised for this action.";
                return RedirectToAction("Unsuccess", "UnsuccessfulAttempt");
            }
            return View();
        }

        public ActionResult DeleteMedia(int id)
        {
            var result = ent.MediaHospitals.FirstOrDefault(x => x.Id == id);
            if(result !=null)
            {
                ent.MediaHospitals.Remove(result);
                ent.SaveChanges();
            }
            return RedirectToAction("ListMedia");
        }
        public ActionResult ListMedia()
        {
            return View(ent.MediaHospitals.OrderBy (a=>a.Title).ToList());
        }

        public ActionResult MediaSection()
        {
            return View();
        }
        [HttpPost]
        public ActionResult MediaSection(MediaHospitalDto model)
        {
            try
            {
                if(model.ImageBase !=null)
                {
                    string path = Server.MapPath("/Images/") + model.ImageBase.FileName;
                    model.ImageBase.SaveAs(path);
                }
                MediaHospital emp = new MediaHospital()
                {
                    Title=model.Title,
                    ImageName = model.ImageBase !=null?model.ImageBase.FileName:"",
                };
                ent.MediaHospitals.Add(emp);
                ent.SaveChanges();
                return RedirectToAction("ListMedia");

            }
            catch
            {
                throw new Exception("Server Error");
            }
        }

        public ActionResult DiscountDelete(int id)
        {
            try
            {
                var result = ent.Discounts.FirstOrDefault(x => x.Id == id);
                if(result !=null)
                {
                    ent.Discounts.Remove(result);
                    ent.SaveChanges();
                }
                return RedirectToAction("DiscountList");
            }
            catch
            {
                throw new Exception("Server error");
            }
        }
        public ActionResult DiscountList()
        {
            return View(ent.Discounts.ToList());
        }
        public ActionResult DiscountCreate()
        {
            ViewBag.Bannerprofessional = new SelectList(ent.Bannerprofessionals.ToList(), "ID", "professionals");

            return View();
        }
        [HttpPost]
        public ActionResult DiscountCreate(Discount model)
        {
            try
            {
                Random r = new Random();
                string OTP = r.Next(1000, 9999).ToString();
                Discount emp = new Discount()
                {
                    Amount = model.Amount,
                    DiscountCoupon="PsDisc"+OTP,
                    Professional_Id =model .Professional_Id ,
                };
                ent.Discounts.Add(emp);
                ent.SaveChanges();
                return RedirectToAction("DiscountList");

            }
            catch
            {
                throw new Exception("Server Error");
            }
        }


        //Banner Section========[Anchal shukla 21/02/22]==========================//
        public ActionResult AddBanner()
         {
               ViewBag.Bannerprofessional = new SelectList(ent.Bannerprofessionals.ToList(), "ID", "professionals");

            return View();
        }
        [HttpPost]
        public ActionResult AddBanner(BannerAddDTO model)
        {
            try
            {
                ViewBag.Bannerprofessional = new SelectList(ent.Bannerprofessionals.ToList(), "Id", "professionals");

                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var ban=  Mapper.CreateMap<BannerAddDTO, Banner>();
                var banner = Mapper.Map<Banner>(model);
                
                if (model.ImageFile != null)
                {
                    if (model.ImageFile.ContentLength > 2 * 1024 * 1024)
                    {
                        TempData["msg"] = "Image should not succeed 2 mb";
                        return View(model);
                    }
                    var uploadResult = FileOperation.UploadImage(model.ImageFile, "Images");
                    if (uploadResult == "not allowed")
                    {
                        TempData["msg"] = "Only .jpg,.jpeg,.png and .gif files are allowed";
                        return View(model);
                    }
                    
                    banner.BannerPath = uploadResult;
                    
                }
                
                ent.Banners.Add(banner);
                ent.SaveChanges();
                TempData["msg"] = "Records has added successfully.";
            }
            catch
            {
                TempData["msg"] = "Error has been occured.";

            }
            return RedirectToAction("AddBanner");
        }

        //NEW
        public ActionResult BannerList()
        {
            var model = new Banner_List();
            string q = @"select B.ID,BP.professionals,B.BannerPath From Banner as B left join BannerProfessional as BP on BP.Id=B.pro_id";
            var data = ent.Database.SqlQuery<listBanner>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record";
                return View(model);
            }
            model.listBanner = data;
            return View(model);
            //return View(ent.Banners.ToList());
        }
        public ActionResult DeleteBanner(int id)
        {
            try
            {
                var result = ent.Banners.FirstOrDefault(x => x.ID == id);
                if (result != null)
                {
                    ent.Banners.Remove(result);
                    ent.SaveChanges();
                }
                return RedirectToAction("BannerList");
            }
            catch
            {
                throw new Exception("Server error");
            }
        }
    }
}