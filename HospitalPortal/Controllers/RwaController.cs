using AutoMapper;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Repositories;
using HospitalPortal.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    [Authorize]
    public class RwaController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(RwaController));

        [AllowAnonymous]
        public ActionResult Add(int vendorId = 0)
        {
            var model = new RWADTO();
            model.Vendor_Id = vendorId;
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Add(RWADTO model)
        {
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (!string.IsNullOrEmpty(model.OtherCity))
                        ModelState.Remove("CityMaster_Id");
                    if (!ModelState.IsValid)
                    {
                        model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                        return View(model);
                    }
                    //if (ent.AdminLogins.Any(a=>a.Username==model.EmailId))
                    //{
                    //    TempData["msg"] = "This Email-Id has already exists.";
                    //    return RedirectToAction("Add");
                    //}
                    //if (ent.AdminLogins.Any(a => a.PhoneNumber == model.MobileNumber))
                    //{
                    //    TempData["msg"] = "This Mobile Number has already exists.";
                    //    return RedirectToAction("Add");
                    //}
                    if (ent.RWAs.Any(a => a.AuthorityName == model.AuthorityName && a.MobileNumber == model.MobileNumber))
                    {
                        var data = ent.RWAs.Where(a => a.AuthorityName == model.AuthorityName && a.MobileNumber == model.MobileNumber).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.RWAId).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        TempData["msg"] = "you are already registered with pswellness";
                        return RedirectToAction("Add", "Rwa");
                    }

                    var admin = new AdminLogin
                    {
                        Username = model.EmailId,
                        PhoneNumber=model.MobileNumber,
                        Password = model.Password,
                        Role = "RWA" };
                    ent.AdminLogins.Add(admin);
                    ent.SaveChanges();

                    if (model.CertificateFile != null)
                    {
                        var verf = FileOperation.UploadImage(model.CertificateFile, "Images");
                        if (verf == "not allowed")
                        {
                            TempData["msg"] = "Only png,jpg,jpeg files are allowed as certificate doc..";
                            tran.Rollback();
                            return View(model);
                        }
                        model.CertificateImage = verf;
                    }
                    // save other city and locations

                    if (!string.IsNullOrEmpty(model.OtherCity))
                    {
                        var cityMaster = new CityMaster
                        {
                            CityName = model.OtherCity,
                            StateMaster_Id = (int)model.StateMaster_Id
                        };
                        ent.CityMasters.Add(cityMaster);
                        ent.SaveChanges();
                        model.CityMaster_Id = cityMaster.Id;
                    }
                    string uid = "RWA001";
                    var lastRecord = ent.RWAs.OrderByDescending(a => a.Id).FirstOrDefault(a => a.RWAId != null);
                    if (lastRecord != null)
                    {
                        int len = uid.Length;
                        string iPart = lastRecord.RWAId.Substring(3, len - 3);
                        int next =  Convert.ToInt32(iPart) + 1;
                        uid = "RWA00" + next;
                    }
                    
                    var domainModel = Mapper.Map<RWA>(model);
                    domainModel.AdminLogin_Id = admin.Id;
                    domainModel.JoiningDate = DateTime.Now;
                    domainModel.RWAId = uid;
                    admin.UserID = domainModel.RWAId;
                    ent.RWAs.Add(domainModel);
                    ent.SaveChanges();
                    string msg = "Welcome to PSWELLNESS. Your User Name :  " + domainModel.EmailId + "(" + domainModel.RWAId + "), Password : " + admin.Password + ".";
                    Message.SendSms(domainModel.MobileNumber, msg);
                    string msg1 = "Welcome to PSWELLNESS. Your User Name :  " + admin.Username + "(" + admin.UserID + "), Password : " + admin.Password + ".";

                    Utilities.EmailOperations.SendEmail1(model.EmailId, "Ps Wellness", msg1, true);

                    TempData["msg"] = "ok";
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    TempData["msg"] = "Server Error";
                    tran.Rollback();
                }
            }
            return RedirectToAction("Add", new { VendorId = model.Vendor_Id });
        }

        public ActionResult Edit(int id)
        {
            var data = ent.RWAs.Find(id);
            var model = Mapper.Map<RWADTO>(data);
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName",model.StateMaster_Id);
            model.Cities = new SelectList(repos.GetCitiesByState(model.StateMaster_Id), "Id", "CityName", model.CityMaster_Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(RWADTO model)
        {
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName", model.StateMaster_Id);
            model.Cities = new SelectList(repos.GetCitiesByState(model.StateMaster_Id), "Id", "CityName", model.CityMaster_Id);
            try
            {
                ModelState.Remove("MobileNumber");
                ModelState.Remove("EmailId");
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
                ModelState.Remove("IsCheckedTermsCondition");
                if (!ModelState.IsValid)
                    return View(model);
                if (model.CertificateFile != null)
                {
                    var verf = FileOperation.UploadImage(model.CertificateFile, "Images");
                    if (verf == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg files are allowed as certificate doc..";
                        return View(model);
                    }
                    model.CertificateImage = verf;
                }

                var domainModel = Mapper.Map<RWA>(model);
                ent.Entry(domainModel).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                TempData["msg"] = "ok";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                TempData["msg"] = "Server Error";
            }
                return RedirectToAction("Edit", new { id = model.Id });
        }

        public ActionResult All(int? vendorId, string term = null)
        {
            string q = @"select v.*,s.StateName,c.CityName from RWA v 
join StateMaster s on v.StateMaster_Id=s.Id
join CityMaster c on v.CityMaster_Id = c.Id
where v.IsDeleted=0  order by v.Id desc";
            var data = ent.Database.SqlQuery<RWADTO>(q).ToList();
            if (vendorId != null)
                data = data.Where(a => a.Vendor_Id == vendorId).ToList();
            if (term != null)
                data = data.Where(a => a.AuthorityName.ToLower().Contains(term) || a.RWAId.Contains(term)).ToList();
            return View(data);
        }

        public ActionResult UpdateStatus(int id)
        {
            string q = @"update RWA set IsApproved = case when IsApproved=1 then 0 else 1 end where id="+id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("All");
        }

        public ActionResult Delete(int id)
        {
            var data = ent.RWAs.Find(id);
            try
            {
                data.IsDeleted = true;
                ent.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return RedirectToAction("All");
        }

        public ActionResult PayOutReport(int id)
        {
            var model = new ViewPayOutHistory();
            var Name = ent.Database.SqlQuery<string>("select AuthorityName from Rwa where Id=" + id).FirstOrDefault();
            model.AuthorityName = Name;
            string qry = @"select Dp.Id, ISNULL(Dp.IsPaid, 0) as IsPaid , Dp.IsGenerated, Dp.Rwa_Id, Dp.PaymentDate, Dp.Amount, D.AuthorityName from  RwaPayOut Dp join Rwa D on D.Id = Dp.Rwa_Id  where  Dp.Rwa_Id=" + id+ "order by Dp.PaymentDate";
            var data = ent.Database.SqlQuery<HistoryOfRWA_pAYOUT>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            model.HistoryOfRWA_Payout = data;
            return View(model);
        }

        public ActionResult PaymentReport(int id)
        {
            var model = new rwaReport();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Name='RWA'").FirstOrDefault();
            string q = @"select rwa.Id, Count(P.Id) as Counts, rwa.AuthorityName from Patient p join RWA rwa on rwa.Id = p.Rwa_Id where p.Rwa_Id="+id+" and Convert(Date,P.Reg_Date) between DATEADD(day,-7,GETDATE()) and GetDate() group by rwa.AuthorityName,rwa.Id";
            var data = ent.Database.SqlQuery<rwaList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record";
                return View(model);
            }
            ViewBag.Payment = payment;
            model.rwaList = data;
            return View(model);
        }
    }
}