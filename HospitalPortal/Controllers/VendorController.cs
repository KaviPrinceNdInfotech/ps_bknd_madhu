using AutoMapper;
using HospitalPortal.BL;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Repositories;
using HospitalPortal.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static HospitalPortal.Utilities.EmailOperations;

namespace HospitalPortal.Controllers
{
    [Authorize(Roles ="Franchise,admin")]
    public class VendorController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(VendorController));
        GenerateBookingId bk = new GenerateBookingId();

        [AllowAnonymous]
        public ActionResult Add()
        {
            var model = new VendorDTO();
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Add(VendorDTO model)
        {
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (!string.IsNullOrEmpty(model.OtherCity))
                        ModelState.Remove("City_Id");
                    if (!ModelState.IsValid)
                    {
                        model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                        return View(model);
                    }
                    //if (ent.AdminLogins.Any(a => a.Username == model.EmailId))
                    //{
                    //    TempData["msg"] = "This Email-Id has already exists.";
                    //    return RedirectToAction("Add");
                    //}
                    //if (ent.AdminLogins.Any(a => a.PhoneNumber == model.MobileNumber))
                    //{
                    //    TempData["msg"] = "This Mobile Number has already exists.";
                    //    return RedirectToAction("Add");
                    //}
                    if(ent.Vendors.Any(a=>a.VendorName == model.VendorName && a.MobileNumber == model.MobileNumber))
                    {
                        var data = ent.Vendors.Where(a => a.VendorName == model.VendorName && a.MobileNumber == model.MobileNumber).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.UniqueId).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        TempData["msg"] = "you are already registered with pswellness";
                        return RedirectToAction("Add", "Vendor");
                    }


                    var admin = new AdminLogin
                    {
                        Username = model.EmailId,
                        PhoneNumber = model.MobileNumber,
                        Password = model.Password,
                        Role = "Franchise"
                    };
                    ent.AdminLogins.Add(admin);
                    ent.SaveChanges();

                    // aadhar doc upload
                    if (model.AadharOrImageFile != null)
                    {
                        var aadharImg = FileOperation.UploadImage(model.AadharOrImageFile, "Images");
                        if (aadharImg == "not allowed")
                        {
                            TempData["msg"] = "Only png,jpg,jpeg files are allowed as Aadhar card document";
                            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                        tran.Rollback();
                            return View(model);
                        }
                        model.AadharOrPANImage = aadharImg;
                    }
                    if (model.AadharOrImageFile2 != null)
                    {
                        var aadharImg2 = FileOperation.UploadImage(model.AadharOrImageFile2, "Images");
                        if (aadharImg2 == "not allowed")
                        {
                            TempData["msg"] = "Only png,jpg,jpeg files are allowed as Aadhar card document";
                            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                            tran.Rollback();
                            return View(model);
                        }
                        model.AadharOrPANImage2 = aadharImg2;
                    }
                    if (model.PanFile != null)
                    {
                        var PanImg = FileOperation.UploadImage(model.PanFile, "Images");
                        if (PanImg == "not allowed")
                        {
                            TempData["msg"] = "Only png,jpg,jpeg files are allowed as PAN card document";
                            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                            return View(model);
                        }
                        model.PanImage = PanImg;
                    }

                  
                    //string uid = "FR1";
                    //var lastRecord = ent.Vendors.OrderByDescending(a => a.Id).FirstOrDefault(a => a.UniqueId != null);
                    //if(lastRecord!=null)
                    //{
                    //    int len = uid.Length;
                    //    string iPart = lastRecord.UniqueId.Substring(2, len - 2);
                    //    int next = Convert.ToInt32(iPart)+1;
                    //    uid = "FR" + next;
                    //}
                    if (!string.IsNullOrEmpty(model.OtherCity))
                    {
                        var cityMaster = new CityMaster
                        {
                            CityName = model.OtherCity,
                            StateMaster_Id = (int)model.StateMaster_Id
                        };
                        ent.CityMasters.Add(cityMaster);
                        ent.SaveChanges();
                        model.City_Id = cityMaster.Id;
                    }
                    var domainModel = Mapper.Map<Vendor>(model);
                    domainModel.UniqueId = bk.GenerateVenderId(); 
                    //domainModel.UniqueId = uid;
                    admin.UserID = domainModel.UniqueId;
                    domainModel.AdminLogin_Id = admin.Id;
                    ent.Vendors.Add(domainModel);
                    ent.SaveChanges();
                    //string msg = "Welcome to PSWELLNESS. Your User Name :  " + domainModel.EmailId + "(" + domainModel.UniqueId + "), Password : " + admin.Password + ".";
                    //Message.SendSms(domainModel.MobileNumber, msg);

                    //string msg1 = "Welcome to PSWELLNESS. Your User Name :  " + admin.Username + "(" + admin.UserID + "), Password : " + admin.Password + ".";

                    //Utilities.EmailOperations.SendEmail1(model.EmailId, "Ps Wellness", msg1, true);

                    string msg = @"<!DOCTYPE html>
<html>
<head>
    <title>PS Wellness Registration Confirmation</title>
</head>
<body>
    <h1>Welcome to PS Wellness!</h1>
    <p>Your signup is complete. To finalize your registration, please use the following login credentials:</p>
    <ul>
        <li><strong>User ID:</strong> " + admin.UserID + @"</li>
        <li><strong>Password:</strong> " + admin.Password + @"</li>
    </ul>
    <p>Thank you for choosing PS Wellness. We look forward to assisting you on your wellness journey.</p>
</body>
</html>";

                    string msg1 = "Welcome to PS Wellness. Your signup is complete. To finalize your registration please proceed to log in using the credentials you provided during the signup process. Your User Id: " + admin.UserID + ", Password: " + admin.Password + ".";

                    EmailEF ef = new EmailEF()
                    {
                        EmailAddress = model.EmailId,
                        Message = msg,
                        Subject = "PS Wellness Registration Confirmation"
                    };

                    EmailOperations.SendEmainew(ef);

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

            return RedirectToAction("Add");
        }

        public ActionResult Edit(int id)
        {
            var data = ent.Vendors.Find(id);
            var model = Mapper.Map<VendorDTO>(data);
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName", data.StateMaster_Id);
            model.Cities = new SelectList(repos.GetCitiesByState(model.StateMaster_Id), "Id", "CityName", data.City_Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(VendorDTO model)
        {
            try
            {
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
                ModelState.Remove("MobileNumber");
                ModelState.Remove("EmailId");
                ModelState.Remove("ConfirmPassword");
                ModelState.Remove("IsCheckedTermsCondition");
                if (!string.IsNullOrEmpty(model.OtherCity))
                    ModelState.Remove("City_Id");
                model.States = new SelectList(repos.GetAllStates(), "Id", "StateName", model.StateMaster_Id);
                model.Cities = new SelectList(repos.GetCitiesByState(model.StateMaster_Id), "Id", "CityName", model.City_Id);
                //if (!ModelState.IsValid)
                //    return View(model);
                // aadhar doc upload
                if (model.AadharOrImageFile != null)
                {
                    var aadharImg = FileOperation.UploadImage(model.AadharOrImageFile, "Images");
                    if (aadharImg == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg files are allowed as Aadhar/PAN card document";
                        model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                        return View(model);
                    }
                    model.AadharOrPANImage = aadharImg;
                }

                if (model.PanFile != null)
                {
                    var PanImg = FileOperation.UploadImage(model.PanFile, "Images");
                    if (PanImg == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg files are allowed as PAN card document";
                        model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                        return View(model);
                    }
                    model.PanImage = PanImg;
                }
                if (!string.IsNullOrEmpty(model.OtherCity))
                {
                    var cityMaster = new CityMaster
                    {
                        CityName = model.OtherCity,
                        StateMaster_Id = (int)model.StateMaster_Id
                    };
                    ent.CityMasters.Add(cityMaster);
                    ent.SaveChanges();
                    model.City_Id = cityMaster.Id;
                }
                var domainModel = Mapper.Map<Vendor>(model);
                ent.Entry(domainModel).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                TempData["msg"] = "Server Error";
            }
                return RedirectToAction("Edit",new { id=model.Id});
        }

        public ActionResult All(string term = null)
        {
            string q = @"select v.*,s.StateName,c.CityName from Vendor v 
join StateMaster s on v.StateMaster_Id=s.Id
join CityMaster c on v.City_Id = c.Id
where v.IsDeleted=0 order by v.Id desc";
            var data = ent.Database.SqlQuery<VendorDTO>(q).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                data = data.Where(a => a.VendorName.ToLower().Contains(term) || a.UniqueId.StartsWith(term)).ToList();
            }
            return View(data);
        }

        public ActionResult UpdateStatus(int id)
        {
            string q = @"update Vendor set IsApproved = case when IsApproved=1 then 0 else 1 end where id="+id;
            ent.Database.ExecuteSqlCommand(q);
            string mobile = ent.Database.SqlQuery<string>("select MobileNumber from Vendor where Id=" + id).FirstOrDefault();
            string Email = ent.Database.SqlQuery<string>(@"select EmailId from Vendor where Id=" + id).FirstOrDefault();
            string Name = ent.Database.SqlQuery<string>(@"select VendorName from Vendor where Id=" + id).FirstOrDefault();
            var msg = "Dear " + Name + ", Now you Can Login With Your Registered EmailId " + Email + " and Pasword";
            Message.SendSms(mobile, msg);
            return RedirectToAction("All");
        }
        public ActionResult UpdateBankUpdateStatus(int id)
        {
            string q = @"update Vendor set IsBankUpdateApproved = case when IsBankUpdateApproved=1 then 0 else 1 end where id=" + id;
            ent.Database.ExecuteSqlCommand(q);

            string mobile = ent.Database.SqlQuery<string>("select MobileNumber from Vendor where Id=" + id).FirstOrDefault();
            string Email = ent.Database.SqlQuery<string>(@"select EmailId from Vendor where Id=" + id).FirstOrDefault();
            string Name = ent.Database.SqlQuery<string>(@"select VendorName from Vendor where Id=" + id).FirstOrDefault();
            //var msg = "Dear " + Name + ", Now you Can Upadate your bank details.";
            //Message.SendSms(mobile, msg);
            var query = "SELECT IsBankUpdateApproved FROM Vendor WHERE Id = @Id";
            var parameters = new SqlParameter("@Id", id);
            bool isApproved = ent.Database.SqlQuery<bool>(query, parameters).FirstOrDefault();

            var mailmsg = "Dear " + Name + ", Now you Can Update your bank details.";

            if (isApproved == true)
            {
                EmailEF ef = new EmailEF()
                {
                    EmailAddress = Email,
                    Message = mailmsg,
                    Subject = "PS Wellness Approval Status."
                };
                EmailOperations.SendEmainew(ef);

            }
            return RedirectToAction("All");
        }
        public ActionResult Delete(int id)
        {
            var data = ent.Vendors.Find(id);
            try
            {
                data.IsDeleted = true;
                ent.SaveChanges();
                string q = @"delete from AdminLogin where Id=" + data.AdminLogin_Id;
                ent.Database.ExecuteSqlCommand(q);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return RedirectToAction("All");
        }

        private int GetVendorId()
        {
            int loginId = Convert.ToInt32(User.Identity.Name);
            int VendorId = ent.Database.SqlQuery<int>("select Id from Vendor where AdminLogin_Id=" + loginId).FirstOrDefault();
            return VendorId;
        }

        public ActionResult Doctor(int id)
        {
            var model = new VendorId();
            int Id = ent.Database.SqlQuery<int>(@"select Id from Vendor where Id=" + id).FirstOrDefault();
            model.Id = Id;
            return View(model);
        }

        public ActionResult Driver(int id)
        {
            var model = new VendorId();
            int Id = ent.Database.SqlQuery<int>(@"select Id from Vendor where Id=" + id).FirstOrDefault();
            model.Id = Id;
            return View(model);
        }

        public ActionResult Vehicle(int id)
        {
            var model = new VendorId();
            int Id = ent.Database.SqlQuery<int>(@"select Id from Vendor where Id=" + id).FirstOrDefault();
            model.Id = Id;
            return View(model);
        }

        public ActionResult Chemist(int id)
        {
            var model = new VendorId();
            int Id = ent.Database.SqlQuery<int>(@"select Id from Vendor where Id=" + id).FirstOrDefault();
            model.Id = Id;
            return View(model);
        }

        public ActionResult Nurse(int id)
        {
            var model = new VendorId();
            int Id = ent.Database.SqlQuery<int>(@"select Id from Vendor where Id=" + id).FirstOrDefault();
            model.Id = Id;
            return View(model);
        }

        public ActionResult Lab(int id)
        {
            var model = new VendorId();
            int Id = ent.Database.SqlQuery<int>(@"select Id from Vendor where Id=" + id).FirstOrDefault();
            model.Id = Id;
            return View(model);
        }

        public ActionResult HealthCheckUp(int id)
        {
            var model = new VendorId();
            int Id = ent.Database.SqlQuery<int>(@"select Id from Vendor where Id=" + id).FirstOrDefault();
            model.Id = Id;
            return View(model);
        }

        public ActionResult DailyDoc(string term, DateTime? date)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(pa.TotalFee) as Counts, v.VendorName as Name, v.CompanyName as Name1 from Doctor d join Vendor v on d.Vendor_Id = v.Id join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id  where v.Id = " + Id+" and pa.AppointmentDate =  GETDATE() and pa.IsPaid=1 group by v.VendorName, v.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (date == null)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                }
            }
            else
            {
                string dt = date.Value.ToString("MM/dd/yyyy");
                var qry1 = @"select Sum(pa.TotalFee) as Counts, v.VendorName as Name, v.CompanyName as Name1 from Doctor d join Vendor v on d.Vendor_Id = v.Id join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id  where  v.Id = " + Id + " and Convert(Date,pa.AppointmentDate)  =  '" + dt + "'  and pa.IsPaid=1 group by v.VendorName, v.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult WeeklyDoc(string term, DateTime? date)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(pa.TotalFee) as Counts, v.VendorName as Name, v.CompanyName as Name1 from Doctor d join Vendor v on d.Vendor_Id = v.Id join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id  where  v.Id = " + Id + " and pa.AppointmentDate  between DATEADD(day,-7,GETDATE()) and GetDate()  and pa.IsPaid=1 group by v.VendorName, v.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (date == null)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                }
            }
            else
            {
                DateTime dateCriteria = date.Value.AddDays(-7);
                string Tarikh = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select Sum(pa.TotalFee) as Counts, v.VendorName as Name, v.CompanyName as Name1 from Doctor d join Vendor v on d.Vendor_Id = v.Id join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id  where  v.Id = " + Id + " and pa.AppointmentDate between '" + dateCriteria + "' and '" + Tarikh + "'  and pa.IsPaid=1 group by v.VendorName, v.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult MonthlyDoc(string term, DateTime? sdate, DateTime? edate)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(pa.TotalFee) as Counts, v.VendorName as Name, v.Id, v.CompanyName as Name1 from Doctor d join Vendor v on d.Vendor_Id = v.Id join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id  where v.Id = " + Id + " and Month(pa.AppointmentDate) = Month(GetDate()) and pa.IsPaid=1 group by v.VendorName,v.CompanyName, v.Id";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (sdate == null && edate == null)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                }
            }
            else
            {
                var qry1 = @"select Sum(pa.TotalFee) as Counts, v.VendorName as Name, v.Id, v.CompanyName as Name1 from Doctor d join Vendor v on d.Vendor_Id = v.Id join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id  where v.Id = " + Id + " and Convert(Date,pa.AppointmentDate) between '" + sdate + "' and '" + edate + "' and pa.IsPaid=1 group by v.VendorName,v.CompanyName, v.Id";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult YearlyDoc(string term, int? year)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(pa.TotalFee) as Counts, v.VendorName as Name, d.Id, v.CompanyName as Name1 from Doctor d join Vendor v on d.Vendor_Id = v.Id join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id  where v.Id = " + Id + " and Year(pa.AppointmentDate) = Year(GetDate()) and pa.IsPaid=1 group by v.VendorName,v.CompanyName, d.Id";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (!year.HasValue)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                }
            }
            else
            {
                var qry1 = @"select Sum(pa.TotalFee) as Counts, v.VendorName as Name, v.CompanyName as Name1 from Doctor d join Vendor v on d.Vendor_Id = v.Id join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id  where v.Id = " + Id + " and Year(pa.AppointmentDate) = '" + year + "' and pa.IsPaid=1 group by v.VendorName, v.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Entered Year Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        //Driver Section

        public ActionResult DailyDri(string term, DateTime? date)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select COUNT(d.Id) as Counts, v.VendorName,v.CompanyName from Driver d join Vendor v on d.Vendor_Id = v.Id where v.Id = " + Id + " and Convert(Date,d.JoiningDate) = Convert(Date,GETDATE()) group by v.VendorName,v.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (date != null)
            {
                string dt = date.Value.ToString("MM/dd/yyyy");
                var qry1 = @"select COUNT(d.Id) as Counts, v.VendorName,v.CompanyName from Driver d join Vendor v on d.Vendor_Id = v.Id where v.Id = " + Id + " and Convert(Date,d.JoiningDate) = '" + dt + "' group by v.VendorName,v.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult WeeklyDri(string term, DateTime? date)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select COUNT(d.Id) as Counts, v.VendorName,v.CompanyName from Driver d join Vendor v on d.Vendor_Id = v.Id where v.Id = " + Id + " and Convert(Date,d.RegistrationDate) between DATEADD(day,-7,GETDATE()) and GetDate() group by v.VendorName,v.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (date != null)
            {
                DateTime dateCriteria = date.Value.AddDays(-7);
                string Tarikh = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select COUNT(d.Id) as Counts, v.VendorName,v.CompanyName from Driver d join Vendor v on d.Vendor_Id = v.Id where v.Id = " + Id + " and Convert(Date,d.JoiningDate) between '" + dateCriteria + "' and '" + Tarikh + "' group by v.VendorName,v.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult MonthlyDri(string term, DateTime? sdate, DateTime? edate)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select COUNT(d.Id) as Counts, v.VendorName,v.CompanyName from Driver d join Vendor v on d.Vendor_Id = v.Id where v.Id = " + Id + " and Month(d.JoiningDate) = Month(GetDate()) group by v.VendorName,v.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (sdate != null && edate != null)
            {
                var qry1 = @"select COUNT(d.Id) as Counts, v.VendorName,v.CompanyName from Driver d join Vendor v on d.Vendor_Id = v.Id where v.Id = " + Id + " and Convert(DATE,d.JoiningDate) between '" + sdate + "' and '" + edate + "' group by v.VendorName,v.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult YearlyDri(string term, int? year)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select COUNT(d.Id) as Counts, v.VendorName,v.CompanyName from Driver d join Vendor v on d.Vendor_Id = v.Id where v.Id = " + Id + " and Year(d.JoiningDate) = Year(GetDate()) group by v.VendorName,v.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (year != null)
            {
                var qry1 = @"select COUNT(d.Id) as Counts, v.VendorName,v.CompanyName from Driver d join Vendor v on d.Vendor_Id = v.Id where v.Id = " + Id + " and Year(d.JoiningDate) = '" + year + "' group by v.VendorName,v.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        //Vehicle Section

        public ActionResult DailyVeh(string term, DateTime? date)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(trm.Amount) as Counts, ve.VendorName as Name, ve.CompanyName as Name1 from Vehicle v
join Vendor ve on ve.Id = v.Vendor_Id
join TravelRecordMaster trm on trm.Vehicle_Id = v.Id
where trm.IsDriveCompleted = 1 and v.Id = " + Id + "  and Convert(Date,trm.RequestDate) = GetDate() group by  ve.VendorName, ve.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (date != null)
            {
                string dt = date.Value.ToString("MM/dd/yyyy");
                var qry1 = @"select Sum(trm.Amount) as Counts, ve.VendorName as Name, ve.CompanyName as Name1 from Vehicle v
join Vendor ve on ve.Id = v.Vendor_Id
join TravelRecordMaster trm on trm.Vehicle_Id = v.Id
where v.Id = " + Id + " and trm.IsDriveCompleted = 1 and Convert(Date,trm.RequestDate) = '" + date + "' group by  ve.VendorName, ve.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult WeeklyVeh(string term, DateTime? date)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(trm.Amount) as Counts, ve.VendorName as Name, ve.CompanyName as Name1 from Vehicle v
join Vendor ve on ve.Id = v.Vendor_Id
join TravelRecordMaster trm on trm.Vehicle_Id = v.Id
where v.Id = " + Id + " and trm.IsDriveCompleted = 1 and trm.RequestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() group by  ve.VendorName, ve.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (date != null)
            {
                DateTime dateCriteria = date.Value.AddDays(-7);
                string Tarikh = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"selectselect Sum(trm.Amount) as Counts, ve.VendorName as Name, ve.CompanyName as Name1 from Vehicle v
join Vendor ve on ve.Id = v.Vendor_Id
join TravelRecordMaster trm on trm.Vehicle_Id = v.Id
where v.Id = " + Id + " and trm.IsDriveCompleted = 1 and trm.RequestDate between '" + Tarikh + "' and '" + date + "' group by  ve.VendorName, ve.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult MonthlyVeh(string term, DateTime? sdate, DateTime? edate)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(trm.Amount) as Counts, ve.VendorName as Name, ve.CompanyName as Name1 from Vehicle v
join Vendor ve on ve.Id = v.Vendor_Id
join TravelRecordMaster trm on trm.Vehicle_Id = v.Id
where v.Id = " + Id + " and trm.IsDriveCompleted = 1 and Month(trm.RequestDate) = Month(GetDate()) group by  ve.VendorName, ve.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (sdate != null && edate != null)
            {
                var qry1 = @"select Sum(trm.Amount) as Counts, ve.VendorName as Name, ve.CompanyName as Name1 from Vehicle v
join Vendor ve on ve.Id = v.Vendor_Id
join TravelRecordMaster trm on trm.Vehicle_Id = v.Id
where  v.Id = " + Id + " andtrm.IsDriveCompleted = 1 and Convert(Date,trm.RequestDate) between '" + sdate + "' and '" + edate + "' group by  ve.VendorName, ve.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult YearlyVeh(string term, int? year)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(trm.Amount) as Counts, ve.VendorName as Name, ve.CompanyName as Name1 from Vehicle v
join Vendor ve on ve.Id = v.Vendor_Id
join TravelRecordMaster trm on trm.Vehicle_Id = v.Id
where v.Id = " + Id + " and trm.IsDriveCompleted = 1 and Year(trm.RequestDate) = Year(GetDate()) group by  ve.VendorName, ve.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (year != null)
            {
                var qry1 = @"select Sum(trm.Amount) as Counts, ve.VendorName as Name, ve.CompanyName as Name1 from Vehicle v
join Vendor ve on ve.Id = v.Vendor_Id
join TravelRecordMaster trm on trm.Vehicle_Id = v.Id
where v.Id = " + Id + " and trm.IsDriveCompleted = 1 and Year(trm.RequestDate) = '" + year + "' group by  ve.VendorName, ve.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }


        //Chemist

        public ActionResult DailyChe(string term, DateTime? date)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select SUM(md.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1 from Chemist d join Vendor v on d.Vendor_Id = v.Id 
join MedicineOrder mo on mo.Chemist_Id = d.Id
join MedicineOrderDetail md on md.Order_Id = mo.Id
where v.Id = " + Id + " and Convert(Date,mo.OrderDate) = Convert(Date,GETDATE()) and mo.IsPaid=1 group by v.VendorName,v.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
            }
            if (date != null)
            {
                string dt = date.Value.ToString("MM/dd/yyyy");
                var qry1 = @"select SUM(md.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1 from Chemist d join Vendor v on d.Vendor_Id = v.Id 
join MedicineOrder mo on mo.Chemist_Id = d.Id
join MedicineOrderDetail md on md.Order_Id = mo.Id
where v.Id = " + Id + " and Convert(Date,mo.OrderDate) = '" + dt + "' and mo.IsPaid=1 group by v.VendorName,v.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult WeeklyChe(string term, DateTime? date)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select SUM(md.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1 from Chemist d join Vendor v on d.Vendor_Id = v.Id 
join MedicineOrder mo on mo.Chemist_Id = d.Id
join MedicineOrderDetail md on md.Order_Id = mo.Id
where v.Id = " + Id + " and Convert(Date,mo.OrderDate) between DATEADD(day,-7,GETDATE()) and GetDate() and mo.IsPaid=1 group by v.VendorName,v.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (date != null)
            {
                DateTime dateCriteria = date.Value.AddDays(-7);
                string Tarikh = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select SUM(md.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1 from Chemist d join Vendor v on d.Vendor_Id = v.Id 
join MedicineOrder mo on mo.Chemist_Id = d.Id
join MedicineOrderDetail md on md.Order_Id = mo.Id
where v.Id = " + Id + " and Convert(Date,mo.OrderDate) between '" + dateCriteria + "' and '" + Tarikh + "' and mo.IsPaid=1 group by v.VendorName,v.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult MonthlyChe(string term, DateTime? sdate, DateTime? edate)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select SUM(md.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1 from Chemist d join Vendor v on d.Vendor_Id = v.Id 
join MedicineOrder mo on mo.Chemist_Id = d.Id
join MedicineOrderDetail md on md.Order_Id = mo.Id
where v.Id = " + Id + " and Month(mo.OrderDate) = Month(GetDate()) and mo.IsPaid=1 group by v.VendorName,v.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (sdate != null && edate != null)
            {
                var qry1 = @"select SUM(md.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1 from Chemist d join Vendor v on d.Vendor_Id = v.Id 
join MedicineOrder mo on mo.Chemist_Id = d.Id
join MedicineOrderDetail md on md.Order_Id = mo.Id
where v.Id = " + Id + " and Convert(Date,mo.OrderDate) between '" + sdate + "' and '" + edate + "' and mo.IsPaid=1 group by v.VendorName,v.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult YearlyChe(string term, int? year)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select SUM(md.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1 from Chemist d join Vendor v on d.Vendor_Id = v.Id 
join MedicineOrder mo on mo.Chemist_Id = d.Id
join MedicineOrderDetail md on md.Order_Id = mo.Id
where v.Id = " + Id + " and Year(mo.OrderDate) = Year(GetDate()) and mo.IsPaid=1 group by v.VendorName,v.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (year != null)
            {
                var qry1 = @"select SUM(md.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1 from Chemist d join Vendor v on d.Vendor_Id = v.Id 
join MedicineOrder mo on mo.Chemist_Id = d.Id
join MedicineOrderDetail md on md.Order_Id = mo.Id
where v.Id = " + Id + " and Year(mo.OrderDate) = '" + year + "' and mo.IsPaid=1 group by v.VendorName,v.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        //Nurse
        public ActionResult DailyNur(string term, DateTime? date)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select v.VendorName as Name, v.CompanyName as Name1, V.Id ,
(IsNull(Datediff(day,ns.StartDate,ns.EndDate)* ns.PerDayAmount,0)) as Counts
from Nurse d 
join NurseService ns on ns.Nurse_Id = d.Id
join Vendor v on d.Vendor_Id = v.Id  where v.Id = " + Id + " and Convert(Date,ns.ServiceAcceptanceDate)   = GETDATE()  and ns.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id,ns.StartDate,ns.EndDate,ns.PerDayAmount";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
            }
            if (date != null)
            {
                string dt = date.Value.ToString("MM/dd/yyyy");
                var qry1 = @"select v.VendorName as Name, v.CompanyName as Name1, V.Id ,
(IsNull(Datediff(day,ns.StartDate,ns.EndDate)* ns.PerDayAmount,0)) as Counts
from Nurse d 
join NurseService ns on ns.Nurse_Id = d.Id
join Vendor v on d.Vendor_Id = v.Id  where v.Id = " + Id + " and Convert(Date,ns.ServiceAcceptanceDate)    = '" + dt + "'  and ns.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id,ns.StartDate,ns.EndDate,ns.PerDayAmount";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult WeeklyNur(string term, DateTime? date)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select v.VendorName as Name, v.CompanyName as Name1, V.Id ,
(IsNull(Datediff(day,ns.StartDate,ns.EndDate)* ns.PerDayAmount,0)) as Counts
from Nurse d 
join NurseService ns on ns.Nurse_Id = d.Id
join Vendor v on d.Vendor_Id = v.Id  where v.Id = " + Id + " and Convert(Date,ns.ServiceAcceptanceDate)   between DATEADD(day,-7,GETDATE()) and GetDate() and ns.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id,ns.StartDate,ns.EndDate,ns.PerDayAmount";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (date != null)
            {
                DateTime dateCriteria = date.Value.AddDays(-7);
                string Tarikh = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select v.VendorName as Name, v.CompanyName as Name1, V.Id ,
(IsNull(Datediff(day,ns.StartDate,ns.EndDate)* ns.PerDayAmount,0)) as Counts
from Nurse d 
join NurseService ns on ns.Nurse_Id = d.Id
join Vendor v on d.Vendor_Id = v.Id  where v.Id = " + Id + " and Convert(Date,ns.ServiceAcceptanceDate) between '" + dateCriteria + "' and '" + Tarikh + "'and ns.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id,ns.StartDate,ns.EndDate,ns.PerDayAmount";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult MonthlyNur(string term, DateTime? sdate, DateTime? edate)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select v.VendorName as Name, v.CompanyName as Name1, V.Id ,
(IsNull(Datediff(day,ns.StartDate,ns.EndDate)* ns.PerDayAmount,0)) as Counts
from Nurse d 
join NurseService ns on ns.Nurse_Id = d.Id
join Vendor v on d.Vendor_Id = v.Id  where v.Id = " + Id + " and Month(ns.ServiceAcceptanceDate) = Month(GetDate()) and ns.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id,ns.StartDate,ns.EndDate,ns.PerDayAmount";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (sdate != null && edate != null)
            {
                var qry1 = @"select v.VendorName as Name, v.CompanyName as Name1, V.Id ,
(IsNull(Datediff(day,ns.StartDate,ns.EndDate)* ns.PerDayAmount,0)) as Counts
from Nurse d 
join NurseService ns on ns.Nurse_Id = d.Id
join Vendor v on d.Vendor_Id = v.Id  where v.Id = " + Id + " and Convert(DATE,ns.ServiceAcceptanceDate) between '" + sdate + "' and '" + edate + "' and ns.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id,ns.StartDate,ns.EndDate,ns.PerDayAmount";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult YearlyNur(string term, int? year)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select v.VendorName as Name, v.CompanyName as Name1, V.Id ,
(IsNull(Datediff(day,ns.StartDate,ns.EndDate)* ns.PerDayAmount,0)) as Counts
from Nurse d 
join NurseService ns on ns.Nurse_Id = d.Id
join Vendor v on d.Vendor_Id = v.Id  where v.Id = " + Id + " and Year(ns.ServiceAcceptanceDate) = Year(GetDate()) and ns.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id,ns.StartDate,ns.EndDate,ns.PerDayAmount";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (year != null)
            {
                var qry1 = @"select v.VendorName as Name, v.CompanyName as Name1, V.Id ,
(IsNull(Datediff(day,ns.StartDate,ns.EndDate)* ns.PerDayAmount,0)) as Counts
from Nurse d 
join NurseService ns on ns.Nurse_Id = d.Id
join Vendor v on d.Vendor_Id = v.Id  where v.Id = " + Id + " and Year(ns.ServiceAcceptanceDate) = = '" + year + "'and ns.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id,ns.StartDate,ns.EndDate,ns.PerDayAmount";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }


        //Lab
        public ActionResult DailyLab(string term, DateTime? date)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(bt.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1, V.Id from Lab d join Vendor v on d.Vendor_Id = v.Id  
join BookTestLab bt on bt.Lab_Id = d.Id
join LabBooking lb on lb.PatientId = bt.Patient_Id
where v.Id = " + Id + " and Convert(Date,bt.TestDate) = GETDATE() and lb.IsPaid =1 group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (date == null)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                }
            }
            else
            {
                string dt = date.Value.ToString("MM/dd/yyyy");
                var qry1 = @"select Sum(bt.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1, V.Id from Lab d join Vendor v on d.Vendor_Id = v.Id  
join BookTestLab bt on bt.Lab_Id = d.Id
join LabBooking lb on lb.PatientId = bt.Patient_Id
where v.Id = " + Id + " and  Convert(Date,bt.TestDate) = '" + dt + "' and lb.IsPaid =1 group by v.VendorName,v.CompanyName, V.Id";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult WeeklyLab(string term, DateTime? date)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(bt.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1, V.Id from Lab d join Vendor v on d.Vendor_Id = v.Id  
join BookTestLab bt on bt.Lab_Id = d.Id
join LabBooking lb on lb.PatientId = bt.Patient_Id
where v.Id = " + Id + " and  Convert(Date,bt.TestDate) between DATEADD(day,-7,GETDATE()) and GetDate() and lb.IsPaid =1 group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (date == null)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                }
            }
            else
            {

                DateTime dateCriteria = date.Value.AddDays(-7);
                string Tarikh = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select Sum(bt.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1, V.Id from Lab d join Vendor v on d.Vendor_Id = v.Id  
join BookTestLab bt on bt.Lab_Id = d.Id
join LabBooking lb on lb.PatientId = bt.Patient_Id
where  v.Id = " + Id + " and Convert(Date,bt.TestDate) between '" + dateCriteria + "' and '" + Tarikh + "' and lb.IsPaid =1 group by v.VendorName,v.CompanyName, V.Id";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult MonthlyLab(string term, DateTime? sdate, DateTime? edate)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(bt.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1, V.Id from Lab d join Vendor v on d.Vendor_Id = v.Id  
join BookTestLab bt on bt.Lab_Id = d.Id
join LabBooking lb on lb.PatientId = bt.Patient_Id
where v.Id = " + Id + " and Month(bt.TestDate) = Month(GetDate()) and lb.IsPaid =1 group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (sdate == null && edate == null)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                }
            }
            else
            {

                var qry1 = @"select Sum(bt.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1, V.Id from Lab d join Vendor v on d.Vendor_Id = v.Id  
join BookTestLab bt on bt.Lab_Id = d.Id
join LabBooking lb on lb.PatientId = bt.Patient_Id
where v.Id = " + Id + " and Convert(Date,bt.TestDate) between '" + sdate + "' and '" + edate + "' and lb.IsPaid =1 group by v.VendorName,v.CompanyName, V.Id";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult YearlyLab(string term, int? year)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(bt.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1, V.Id from Lab d join Vendor v on d.Vendor_Id = v.Id  
join BookTestLab bt on bt.Lab_Id = d.Id
join LabBooking lb on lb.PatientId = bt.Patient_Id
where v.Id = " + Id + " and Year(bt.TestDate) = Year(GetDate()) and lb.IsPaid =1 group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (!year.HasValue)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                }
            }
            else
            {

                var qry1 = @"select Sum(bt.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1, V.Id from Lab d join Vendor v on d.Vendor_Id = v.Id  
join BookTestLab bt on bt.Lab_Id = d.Id
join LabBooking lb on lb.PatientId = bt.Patient_Id
where v.Id = " + Id + " and Year(bt.TestDate) = '" + year + "' and lb.IsPaid =1 group by v.VendorName,v.CompanyName, V.Id";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }


        //HealthCheckUp
        public ActionResult DailyHealth(string term, DateTime? date)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select SUM(cc.Amount) as Counts, v.VendorName, V.Id, v.CompanyName from [HealthCheckupCenter] d 
join Vendor v on d.Vendor_Id = v.Id  
join CmpltCheckUp cc on cc.Center_Id = d.Id
join HealthBooking hb on hb.PatientId = cc.PatientId
where v.Id = " + Id + " and cc.TestDate  = GETDATE() and hb.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (date == null)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                }
            }
            else
            {
                string dt = date.Value.ToString("MM/dd/yyyy");
                var qry1 = @"select SUM(cc.Amount) as Counts, v.VendorName, V.Id, v.CompanyName from [HealthCheckupCenter] d 
join Vendor v on d.Vendor_Id = v.Id  
join CmpltCheckUp cc on cc.Center_Id = d.Id
join HealthBooking hb on hb.PatientId = cc.PatientId
where v.Id = " + Id + " and cc.TestDate  = '" + dt + "' and hb.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult WeeklyHealth(string term, DateTime? date)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"
select SUM(cc.Amount) as Counts, v.VendorName, V.Id, v.CompanyName from [HealthCheckupCenter] d 
join Vendor v on d.Vendor_Id = v.Id  
join CmpltCheckUp cc on cc.Center_Id = d.Id
join HealthBooking hb on hb.PatientId = cc.PatientId
where v.Id = " + Id + " and Convert(Date,cc.TestDate)  between DATEADD(day,-7,GETDATE()) and GetDate() and hb.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (date != null)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                }
            }
            else
            {
                DateTime dateCriteria = date.Value.AddDays(-7);
                string Tarikh = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select SUM(cc.Amount) as Counts, v.VendorName, V.Id, v.CompanyName from [HealthCheckupCenter] d 
join Vendor v on d.Vendor_Id = v.Id  
join CmpltCheckUp cc on cc.Center_Id = d.Id
join HealthBooking hb on hb.PatientId = cc.PatientId
where v.Id = " + Id + " and Convert(Date,cc.TestDate)  between '" + dateCriteria + "' and '" + Tarikh + "' and hb.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult MonthlyHealth(string term, DateTime? sdate, DateTime? edate)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select SUM(cc.Amount) as Counts, v.VendorName, V.Id, v.CompanyName from [HealthCheckupCenter] d 
join Vendor v on d.Vendor_Id = v.Id  
join CmpltCheckUp cc on cc.Center_Id = d.Id
join HealthBooking hb on hb.PatientId = cc.PatientId
where v.Id = " + Id + " and Month(cc.TestDate) = Month(GetDate()) and hb.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (sdate == null && edate == null)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                }
            }
            else
            {
                var qry1 = @"select SUM(cc.Amount) as Counts, v.VendorName, V.Id, v.CompanyName from [HealthCheckupCenter] d 
join Vendor v on d.Vendor_Id = v.Id  
join CmpltCheckUp cc on cc.Center_Id = d.Id
join HealthBooking hb on hb.PatientId = cc.PatientId
where v.Id = " + Id + " and Convert(Date,cc.TestDate) between '" + sdate + "' and '" + edate + "' and hb.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult YearlyHealth(string term, int? year)
        {
            int Id = GetVendorId();
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select SUM(cc.Amount) as Counts, v.VendorName, V.Id, v.CompanyName from [HealthCheckupCenter] d 
join Vendor v on d.Vendor_Id = v.Id  
join CmpltCheckUp cc on cc.Center_Id = d.Id
join HealthBooking hb on hb.PatientId = cc.PatientId
where v.Id = " + Id + " and Year(cc.TestDate) = Year(GetDate()) and hb.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (!year.HasValue)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                }
            }
            else
            {
                var qry1 = @"select SUM(cc.Amount) as Counts, v.VendorName, V.Id, v.CompanyName from [HealthCheckupCenter] d 
join Vendor v on d.Vendor_Id = v.Id  
join CmpltCheckUp cc on cc.Center_Id = d.Id
join HealthBooking hb on hb.PatientId = cc.PatientId
where v.Id = " + Id + " and Year(cc.TestDate) = '" + year + "' and hb.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult VendorPayoutHistory(int id)
        {
            var model = new ViewPayOutHistory();
            var Name = ent.Database.SqlQuery<string>("select VendorName from Vendor where Id=" + id).FirstOrDefault();
            model.LabName = Name;
            string qry = @"select Dp.Id, ISNULL(Dp.IsPaid, 0) as IsPaid , Dp.IsGenerated, Dp.Vendor_Id, Dp.PaymentDate, Dp.Amount, D.VendorName from  VendorPayOut Dp join Vendor D on D.Id = Dp.Vendor_Id  where  Dp.Vendor_Id=" + id;
            var data = ent.Database.SqlQuery<HistoryOfVendor_Payout>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            else
            {
                ViewBag.Amount = data.Sum(a => a.Amount);
                model.HistoryOfVendor_Payout = data;
            }
            return View(model);
        }

        public ActionResult DoctorPayment()
        {
            int id = GetVendorId();
            var model = new VendorPaymentDTO();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Vendor' and Name='Doctor'").FirstOrDefault();
            string q = @"select Sum(pa.TotalFee) as Counts, v.VendorName, v.CompanyName from Doctor d join Vendor v on d.Vendor_Id = v.Id join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id  where  v.Id = " + id + " and pa.AppointmentDate  between DATEADD(day,-7,GETDATE()) and GetDate()  and pa.IsPaid=1 group by v.VendorName, v.CompanyName";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            ViewBag.Payment = payment;
            model.Vendorses = data;
            return View(model);
        }
        public ActionResult Vendor()
        {
            return View();
        }

        public ActionResult ChemsitPayment()
        {
            int Id = GetVendorId();
            var model = new VendorPaymentDTO();
            var qry = @"select SUM(md.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1 from Chemist d join Vendor v on d.Vendor_Id = v.Id 
join MedicineOrder mo on mo.Chemist_Id = d.Id
join MedicineOrderDetail md on md.Order_Id = mo.Id
where v.Id = " + Id + " and Convert(Date,mo.OrderDate) between DATEADD(day,-7,GETDATE()) and GetDate() and mo.IsPaid=1 group by v.VendorName,v.CompanyName";
            var data = ent.Database.SqlQuery<VendorList>(qry).ToList();
            model.Vendorses = data;
            return View(model);
        }
        public ActionResult DriverPayment()
        {
            int id = GetVendorId();
            var model = new VendorPaymentDTO();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Vendor' and Name='Driver'").FirstOrDefault();
            string q = @"select COUNT(d.Id) as Counts, v.VendorName,v.CompanyName, V.Id from Driver d join Vendor v on d.Vendor_Id = v.Id  where v.Id=" + id + " and d.JoiningDate  >= DATEADD(day,-7, GETDATE()) group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            ViewBag.Payment = payment;
            model.Vendorses = data;
            return View(model);
        }
        public ActionResult VehiclePayment()
        {
            int id = GetVendorId();
            var model = new VendorPaymentDTO();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Vendor' and Name='Vehicle'").FirstOrDefault();
            string q = @"select Sum(trm.Amount) as Counts, ve.VendorName as Name, ve.CompanyName as Name1 from Vehicle v
join Vendor ve on ve.Id = v.Vendor_Id
join TravelRecordMaster trm on trm.Vehicle_Id = v.Id
where v.Id = " + id + " and trm.IsDriveCompleted = 1 and trm.RequestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() group by  ve.VendorName, ve.CompanyName";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            ViewBag.Payment = payment;
            model.Vendorses = data;
            return View(model);
        }

        public ActionResult HealthPayment()
        {
            int Id = GetVendorId();
            var model = new VendorPaymentDTO();
            //double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"
select SUM(cc.Amount) as Counts, v.VendorName, V.Id, v.CompanyName from [HealthCheckupCenter] d 
join Vendor v on d.Vendor_Id = v.Id  
join CmpltCheckUp cc on cc.Center_Id = d.Id
join HealthBooking hb on hb.PatientId = cc.PatientId
where v.Id = " + Id + " and Convert(Date,cc.TestDate)  between DATEADD(day,-7,GETDATE()) and GetDate() and hb.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<VendorList>(qry).ToList();
                    //ViewBag.Payment = payment;
                    model.Vendorses = data;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                
        }

        public ActionResult DoctorDetails(int id)
        {
            var model = new VendorPaymentDTO();
            string query = @"select CompanyName, VendorName from Vendor where Id=" + id;
            var datas = ent.Database.SqlQuery<VendorPaymentDTO>(query).ToList();
            model.CompanyName = datas.FirstOrDefault().CompanyName;
            model.VendorName = datas.FirstOrDefault().VendorName;
            string q = @"select d.Id, v.VendorName, v.CompanyName, d.DoctorName, c.CityName from Doctor d join Vendor v on d.Vendor_Id = v.Id 
join CityMaster c on c.Id = d.CityMaster_Id
where v.Id=" + id + "  group by v.VendorName,v.CompanyName, d.DoctorName,c.CityName,d.Id order by d.Id desc";
            var data = ent.Database.SqlQuery<VendorsDoctors>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["Msg"] = "No Records";
                return View(model);
            }
            model.VendorDoctor = data;
            return View(model);
        }

        public ActionResult DriverDetails(int id)
        {
            var model = new VendorPaymentDTO();
            string query = @"select CompanyName, VendorName from Vendor where Id=" + id;
            var datas = ent.Database.SqlQuery<VendorPaymentDTO>(query).ToList();
            model.CompanyName = datas.FirstOrDefault().CompanyName;
            model.VendorName = datas.FirstOrDefault().VendorName;
            string q = @"select d.Id, v.VendorName, v.CompanyName, d.DriverName, c.CityName from Driver d join Vendor v on d.Vendor_Id = v.Id 
join CityMaster c on c.Id = d.CityMaster_Id
where v.Id=" + id + "  group by v.VendorName,v.CompanyName, d.DoctorName,c.CityName,d.Id order by d.Id desc";
            var data = ent.Database.SqlQuery<VendorsDriver>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["Msg"] = "No Records";
                return View(model);
            }
            model.VendorsDriver = data;
            return View(model);
        }

        public ActionResult VehicleDetails(int id)
        {
            var model = new VendorPaymentDTO();
            string query = @"select CompanyName, VendorName from Vendor where Id=" + id;
            var datas = ent.Database.SqlQuery<VendorPaymentDTO>(query).ToList();
            model.CompanyName = datas.FirstOrDefault().CompanyName;
            model.VendorName = datas.FirstOrDefault().VendorName;
            string q = @"select d.Id, v.VendorName, v.CompanyName, d.VehicleNumber from Vehicle d join Vendor v on d.Vendor_Id = v.Id where v.Id="+id+"  group by d.Id, v.VendorName, v.CompanyName, d.VehicleNumber order by d.Id desc";
            var data = ent.Database.SqlQuery<VendorsVehicle>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["Msg"] = "No Records";
                return View(model);
            }
            model.VendorsVehicle = data;
            return View(model);
        }

        //public ActionResult DoctorTDS(DateTime? date)
        //{
        //    int id = GetVendorId();
        //    var model = new VendorPaymentDTO();
        //    double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster p where IsDeleted=0 and p.Name='Doctor'").FirstOrDefault();
        //    string q = @"select A.Doctor_Id, D.DoctorName, SUM(A.TotalFee)  As Amount from dbo.PatientAppointment A join Doctor D on D.Id = A.Doctor_Id join Vendor v on v.Id = D.Vendor_Id where A.IsPaid=1 and A.AppointmentDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() and v.Id=" + id+"  group by D.DoctorName, A.Doctor_Id";
        //    var data = ent.Database.SqlQuery<VendorsDoctors>(q).ToList();
        //    if (date != null)
        //    {
        //        DateTime dateCriteria = date.Value.AddDays(-7);
        //        string dDate = dateCriteria.ToString("dd/MM/yyyy");
        //        var qry1 = @"select A.Doctor_Id, D.DoctorName, SUM(A.TotalFee)  As Amount from dbo.PatientAppointment A join Doctor D on D.Id = A.Doctor_Id join Vendor v on v.Id = D.Vendor_Id where A.IsPaid=1 and v.Id=" + id + " and A.AppointmentDate BETWEEN '" + dateCriteria+"' AND '"+date+"' group by D.DoctorName, A.Doctor_Id";
        //        var data1 = ent.Database.SqlQuery<VendorsDoctors>(qry1).ToList();
        //        if (data1.Count() == 0)
        //        {
        //            TempData["msg"] = "Your Selected Date Doesn't Contain Data.";
        //            return View(model);
        //        }
        //        else
        //        {
        //            ViewBag.Payment = tds;
        //            model.VendorDoctor = data1;
        //            return View(model);
        //        }
        //    }
        //    if (data.Count() == 0)
        //    {
        //        TempData["msg"] = "No Result";
        //        ViewBag.Payment = 0;
        //        ViewBag.Amount = 0;
        //        return View(model);
        //    }
        //    ViewBag.Payment = tds;
        //    ViewBag.Amount = data.Sum(a => a.Amount);
        //    model.VendorDoctor = data;
        //    return View(model);
        //}

        public ActionResult DoctorTDS(DateTime? date)
        {
            int id = GetVendorId();
            var model = new VendorPaymentDTO();

            double tds = ent.Database.SqlQuery<double>(@"SELECT Amount FROM TDSMaster p WHERE IsDeleted = 0 AND p.Name = 'Doctor'").FirstOrDefault();

            string baseQuery = @"SELECT A.Doctor_Id, D.DoctorName, SUM(A.TotalFee) AS Amount 
                         FROM dbo.PatientAppointment A 
                         JOIN Doctor D ON D.Id = A.Doctor_Id 
                         JOIN Vendor V ON V.Id = D.Vendor_Id 
                         WHERE A.IsPaid = 1 AND V.Id = @id";

            // Modify the base query to include date criteria if provided
            if (date != null)
            {
                DateTime dateCriteria = date.Value.AddDays(-7);
                baseQuery += " AND A.AppointmentDate BETWEEN @startDate AND @endDate";
            }

            baseQuery += " GROUP BY D.DoctorName, A.Doctor_Id";

            // Execute the query with parameters
            var parameters = new List<SqlParameter>
    {
        new SqlParameter("@id", id)
    };

            if (date != null)
            {
                DateTime dateCriteria = date.Value.AddDays(-7);
                DateTime endDate = date.Value;

                parameters.Add(new SqlParameter("@startDate", dateCriteria));
                parameters.Add(new SqlParameter("@endDate", endDate));
            }

            var data = ent.Database.SqlQuery<VendorsDoctors>(baseQuery, parameters.ToArray()).ToList();

            if (data.Count == 0)
            {
                TempData["msg"] = date != null ? "Your Selected Date Doesn't Contain Data." : "No Result";
                ViewBag.Payment = 0;
                ViewBag.Amount = 0;
                return View(model);
            }

            ViewBag.Payment = tds;
            ViewBag.Amount = data.Sum(a => a.Amount);
            model.VendorDoctor = data;
            return View(model);
        }

        public ActionResult Vendortds()
        {
            return View();
        }

        public ActionResult Drivertds(DateTime? date)
        {
            int id = GetVendorId();
            var model = new VendorPaymentDTO();
            double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster p where p.Name='Vendor'").FirstOrDefault();
            string q = @"select COUNT(d.Id) as Counts, v.VendorName,v.CompanyName, V.Id from Driver d join Vendor v on d.Vendor_Id = v.Id  where v.Id=" + id + " and d.JoiningDate  >= DATEADD(day,-7, GETDATE()) group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (date != null)
            {
                DateTime dateCriteria = date.Value.AddDays(-7);
                string dDate = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select COUNT(d.Id) as Counts, v.VendorName, V.Id, v.CompanyName from Driver d join Vendor v on d.Vendor_Id = v.Id  where  d.JoiningDate between '" + dateCriteria + "' and '" + date + "' group by v.VendorName,v.CompanyName, V.Id";
                var data1 = ent.Database.SqlQuery<VendorList>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain Data.";
                    return View(model);
                }
                else
                {
                    ViewBag.Payment = tds;
                    model.Vendorses = data1;
                    return View(model);
                }
            }
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            ViewBag.Payment = tds;
            model.Vendorses = data;
            return View(model);
        }
        public ActionResult Vehicletds(DateTime? date)
        {
            int id = GetVendorId();
            var model = new VendorPaymentDTO();
            double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster p where p.Name='Vehicle'").FirstOrDefault();
            string q = @"select Sum(trm.Amount) as Counts, ve.VendorName as Name, ve.CompanyName as Name1 from Vehicle v join Vendor ve on ve.Id = v.Vendor_Id join TravelRecordMaster trm on trm.Vehicle_Id = v.Id where v.Id = " + id + " and trm.IsDriveCompleted = 1 and trm.RequestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() group by  ve.VendorName, ve.CompanyName";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (date != null)
            {
                DateTime dateCriteria = date.Value.AddDays(-7);
                string dDate = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select COUNT(d.Id) as Counts, v.VendorName, V.Id, v.CompanyName from Vehicle d join Vendor v on d.Vendor_Id = v.Id  where  d.RegistartionDate between '" + dateCriteria + "' and '" + date + "' group by v.VendorName,v.CompanyName, V.Id";
                var data1 = ent.Database.SqlQuery<VendorList>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain Data.";
                    return View(model);
                }
                else
                {
                    ViewBag.Payment = tds;
                    model.Vendorses = data1;
                    return View(model);
                }
            }

            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            ViewBag.Payment = tds;
            model.Vendorses = data;
            return View(model);
        }

        public ActionResult DoctortdsList(int id)
        {
            var model = new VendorPaymentDTO();
            string query = @"select CompanyName, VendorName from Vendor where Id=" + id;
            var datas = ent.Database.SqlQuery<VendorPaymentDTO>(query).ToList();
            model.CompanyName = datas.FirstOrDefault().CompanyName;
            model.VendorName = datas.FirstOrDefault().VendorName;
            string q = @"select d.Id, v.VendorName, v.CompanyName, d.DoctorName, c.CityName from Doctor d join Vendor v on d.Vendor_Id = v.Id 
join CityMaster c on c.Id = d.CityMaster_Id
where v.Id=" + id + "  group by v.VendorName,v.CompanyName, d.DoctorName,c.CityName,d.Id order by d.Id desc";
            var data = ent.Database.SqlQuery<VendorsDoctors>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["Msg"] = "No Records";
                return View(model);
            }
            model.VendorDoctor = data;
            return View(model);
        }

        public ActionResult DrivertdsList(int id)
        {
            var model = new VendorPaymentDTO();
            string query = @"select CompanyName, VendorName from Vendor where Id=" + id;
            var datas = ent.Database.SqlQuery<VendorPaymentDTO>(query).ToList();
            model.CompanyName = datas.FirstOrDefault().CompanyName;
            model.VendorName = datas.FirstOrDefault().VendorName;
            string q = @"select d.Id, v.VendorName, v.CompanyName, d.DriverName, c.CityName from Driver d join Vendor v on d.Vendor_Id = v.Id 
join CityMaster c on c.Id = d.CityMaster_Id
where v.Id=" + id + "  group by v.VendorName,v.CompanyName, d.DoctorName,c.CityName,d.Id order by d.Id desc";
            var data = ent.Database.SqlQuery<VendorsDriver>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["Msg"] = "No Records";
                return View(model);
            }
            model.VendorsDriver = data;
            return View(model);
        }

        public ActionResult VehicletdsList(int id)
        {
            var model = new VendorPaymentDTO();
            string query = @"select CompanyName, VendorName from Vendor where Id=" + id;
            var datas = ent.Database.SqlQuery<VendorPaymentDTO>(query).ToList();
            model.CompanyName = datas.FirstOrDefault().CompanyName;
            model.VendorName = datas.FirstOrDefault().VendorName;
            string q = @"select d.Id, v.VendorName, v.CompanyName, d.VehicleNumber from Vehicle d join Vendor v on d.Vendor_Id = v.Id where v.Id=" + id + "  group by v.VendorName,v.CompanyName, d.DoctorName, d.Id order by d.Id desc";
            var data = ent.Database.SqlQuery<VendorsVehicle>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["Msg"] = "No Records";
                return View(model);
            }
            model.VendorsVehicle = data;
            return View(model);
        }




        public ActionResult HealthCheckupVendor()
        {
            int id = GetVendorId();
            var model = new VendorPaymentDTO();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Vendor' and Name='HealthCheckup'").FirstOrDefault();
            string q = @"select SUM(cc.Amount) as Counts, v.VendorName, V.Id, v.CompanyName from[HealthCheckupCenter] d join Vendor v on d.Vendor_Id = v.Id join CmpltCheckUp cc on cc.Center_Id = d.Id join HealthBooking hb on hb.PatientId = cc.PatientId where v.Id = " + id + " and Convert(Date, cc.TestDate)  between DATEADD(day, -7, GETDATE()) and GetDate() and hb.IsPaid = 1 group by v.VendorName, v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            ViewBag.Payment = payment;
            model.Vendorses = data;
            return View(model);
        }



        public ActionResult LabVendor()
        {
            int id = GetVendorId();
            var model = new VendorPaymentDTO();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Vendor' and Name='Lab'").FirstOrDefault();
            string q = @"select Sum(bt.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1, V.Id from Lab d join Vendor v on d.Vendor_Id = v.Id join BookTestLab bt on bt.Lab_Id = d.Id join LabBooking lb on lb.PatientId = bt.Patient_Id where v.Id = " + id + " and  Convert(Date,bt.TestDate) between DATEADD(day,-7,GETDATE()) and GetDate() and lb.IsPaid =1 group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            ViewBag.Payment = payment;
            model.Vendorses = data;
            return View(model);
        }



        public ActionResult HealthCheckupVDetails(int id)
        {
            var model = new VendorPaymentDTO();
            string query = @"select CompanyName, VendorName from Vendor where Id=" + id;
            var datas = ent.Database.SqlQuery<VendorPaymentDTO>(query).ToList();
            model.CompanyName = datas.FirstOrDefault().CompanyName;
            model.VendorName = datas.FirstOrDefault().VendorName;
            string q = @"select d.Id, v.VendorName, v.CompanyName, d.[LabName], c.CityName from [HealthCheckupCenter] d join Vendor v on d.Vendor_Id = v.Id 
join CityMaster c on c.Id = d.CityMaster_Id
where v.Id=" + id + "  group by v.VendorName,v.CompanyName, d.[LabName],c.CityName,d.Id order by d.Id desc";
            var data = ent.Database.SqlQuery<VendorsHealth>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["Msg"] = "No Records";
                return View(model);
            }
            model.VendorsHealth = data;
            return View(model);
        }

        public ActionResult LabVDetails(int id)
        {
            var model = new VendorPaymentDTO();
            string query = @"select CompanyName, VendorName from Vendor where Id=" + id;
            var datas = ent.Database.SqlQuery<VendorPaymentDTO>(query).ToList();
            model.CompanyName = datas.FirstOrDefault().CompanyName;
            model.VendorName = datas.FirstOrDefault().VendorName;
            string q = @"select d.Id, v.VendorName, v.CompanyName, d.LabName, c.CityName from Lab d join Vendor v on d.Vendor_Id = v.Id 
join CityMaster c on c.Id = d.CityMaster_Id
where v.Id=" + id + "  group by v.VendorName,v.CompanyName, d.LabName,c.CityName,d.Id order by d.Id desc";
            var data = ent.Database.SqlQuery<VendorsLab>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["Msg"] = "No Records";
                return View(model);
            }
            model.VendorsLab = data;
            return View(model);
        }
        public ActionResult NurseVendor()
        {
            int id = GetVendorId();
            var model = new VendorPaymentDTO();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Vendor' and Name='Nurse'").FirstOrDefault();
            string q = @"select v.VendorName as Name, v.CompanyName as Name1, V.Id ,
(IsNull(Datediff(day,ns.StartDate,ns.EndDate)* ns.PerDayAmount,0)) as Counts
from Nurse d 
join NurseService ns on ns.Nurse_Id = d.Id
join Vendor v on d.Vendor_Id = v.Id  where v.Id = " + id + " and Convert(Date,ns.ServiceAcceptanceDate)   between DATEADD(day,-7,GETDATE()) and GetDate() and ns.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id,ns.StartDate,ns.EndDate,ns.PerDayAmount";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            ViewBag.Payment = payment;
            model.Vendorses = data;
            return View(model);
        }
        public ActionResult NurseVDetails(int id)
        {
            var model = new VendorPaymentDTO();
            string query = @"select CompanyName, VendorName from Vendor where Id=" + id;
            var datas = ent.Database.SqlQuery<VendorPaymentDTO>(query).ToList();
            model.CompanyName = datas.FirstOrDefault().CompanyName;
            model.VendorName = datas.FirstOrDefault().VendorName;
            string q = @"select d.Id, v.VendorName, v.CompanyName, d.NurseName from Nurse d join Vendor v on d.Vendor_Id = v.Id where v.Id=" + id + "  group by v.VendorName,v.CompanyName, d.NurseName, d.Id order by d.Id desc";
            var data = ent.Database.SqlQuery<VendorsNurse>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["Msg"] = "No Records";
                return View(model);
            }
            model.VendorsNurse = data;
            return View(model);
        }

        public ActionResult RWAVendor()
        {
            int id = GetVendorId();

            var model = new VendorPaymentDTO();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Vendor' and Name='RWA'").FirstOrDefault();

            string q = @"select COUNT(d.Id) as Counts, v.VendorName, v.CompanyName, V.Id from RWA d join Vehicle v on d.Vendor_Id = v.Id  where v.Id= "+id+" and d.JoiningDate  >= DATEADD(day,-7, GETDATE()) group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            ViewBag.Payment = payment;
            model.Vendorses = data;
            return View(model);
        }
        public ActionResult RWAVDetails(int id)
        {
            var model = new VendorPaymentDTO();
            string query = @"select CompanyName, VendorName from Vendor where Id=" + id;
            var datas = ent.Database.SqlQuery<VendorPaymentDTO>(query).ToList();
            model.CompanyName = datas.FirstOrDefault().CompanyName;
            model.VendorName = datas.FirstOrDefault().VendorName;
            string q = @"select d.Id, v.VendorName, v.CompanyName, d.AuthorityName from RWA d join Vendor v on d.Vendor_Id = v.Id where v.Id=" + id + "  group by v.VendorName,v.CompanyName, d.AuthorityName, d.Id order by d.Id desc";
            var data = ent.Database.SqlQuery<VendorsRWA>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["Msg"] = "No Records";
                return View(model);
            }
            model.VendorsRWA = data;
            return View(model);
        }

        public ActionResult PatientVendor()
        {
            int id = GetVendorId();
            var model = new VendorPaymentDTO();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Vendor' and Name='RWA'").FirstOrDefault();
            string q = @"select COUNT(d.Id) as Counts, v.VendorName, v.CompanyName, V.Id from Patient d join Vendor v on d.Vendor_Id = v.Id  where v.Id= " + id + " and  d.JoiningDate  >= DATEADD(day,-7, GETDATE()) group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            ViewBag.Payment = payment;
            model.Vendorses = data;
            return View(model);
        }
        public ActionResult PatientVDetails(int id)
        {
            var model = new VendorPaymentDTO();
            string query = @"select CompanyName, VendorName from Vendor where Id=" + id;
            var datas = ent.Database.SqlQuery<VendorPaymentDTO>(query).ToList();
            model.CompanyName = datas.FirstOrDefault().CompanyName;
            model.VendorName = datas.FirstOrDefault().VendorName;
            string q = @"select d.Id, v.VendorName, v.CompanyName, d.PatientName from Patient d join Vendor v on d.Vendor_Id = v.Id where v.Id=" + id + "  group by v.VendorName,v.CompanyName, d.Patient, d.Id order by d.Id desc";
            var data = ent.Database.SqlQuery<VendorsPatient>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["Msg"] = "No Records";
                return View(model);
            }
            model.VendorsPatient = data;
            return View(model);
        }


        public ActionResult CommissionReport()
        {

            return View();
        }
    }
}