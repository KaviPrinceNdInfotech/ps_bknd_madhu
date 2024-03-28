using AutoMapper;
using HospitalPortal.BL;
using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.RequestModel;
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
    [Authorize(Roles = "admin,chemist,Franchise")]
    public class ChemistController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(ChemistController));
        GenerateBookingId bk = new GenerateBookingId();


        private int GetChemistId()
        {
            int loginId = Convert.ToInt32(User.Identity.Name);
            int ChemistId = ent.Database.SqlQuery<int>("select Id from Chemist where AdminLogin_Id=" + loginId).FirstOrDefault();
            return ChemistId;
        }
        [AllowAnonymous]
        public ActionResult Add(int vendorId = 0)
        {
            var model = new ChemistDTO();
            model.Vendor_Id = vendorId;
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
            model.Cities = new SelectList(ent.CityMasters.ToList(), "Id", "StateName");
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Add(ChemistDTO model)
        {
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (!string.IsNullOrEmpty(model.OtherCity))
                        ModelState.Remove("CityMaster_Id");
                    if (!string.IsNullOrEmpty(model.OtherLocation))
                        ModelState.Remove("Location_Id");
                    model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                   
                    if (ent.Chemists.Any(a => a.ChemistName == model.ChemistName && a.MobileNumber == model.MobileNumber))
                    {
                        var data = ent.Chemists.Where(a => a.ChemistName == model.ChemistName && a.MobileNumber == model.MobileNumber).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.ChemistId).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        TempData["msg"] = "you are already registered with pswellness";
                        return RedirectToAction("Add", "Chemist");
                    }

                    var admin = new AdminLogin
                    {
                        Username = model.EmailId,
                        PhoneNumber = model.MobileNumber,
                        Password = model.Password,
                        Role = "chemist"
                    };
                    ent.AdminLogins.Add(admin);
                    ent.SaveChanges();


                    // Licence Section 
                    if (model.LicenceImageFile == null)
                    {
                        TempData["msg"] = "Licence Image File Picture can not be null";
                        tran.Rollback();
                        return View(model);
                    }
                    var img = FileOperation.UploadImage(model.LicenceImageFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg,pdf files are allowed.";
                        tran.Rollback();
                        return View(model);
                    }

                    model.LicenceImage = img;
                    

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
                    if (!string.IsNullOrEmpty(model.OtherLocation))
                    {
                        var locationMaster = new Location
                        {
                            LocationName = model.OtherLocation,
                            City_Id = (int)model.CityMaster_Id
                        };
                        ent.Locations.Add(locationMaster);
                        ent.SaveChanges();
                        model.Location_Id = locationMaster.Id;
                    }
                    var domainModel = Mapper.Map<Chemist>(model);
                    domainModel.AdminLogin_Id = admin.Id;
                    domainModel.JoiningDate = DateTime.Now;
                    domainModel.Vendor_Id = model.Vendor_Id == 0 ? null : model.Vendor_Id;
                    domainModel.ChemistId = bk.GenerateChemistId();
                    admin.UserID = domainModel.ChemistId;
                    domainModel.PAN = domainModel.PAN;
                    domainModel.IsBankUpdateApproved = false;
                    //domainModel.Vendor_Id = model.Vendor_Id??0;
                    domainModel.IsCheckedTermsCondition = model.IsCheckedTermsCondition;
                    //if (model.CityNames != null)
                    //{
                    //    domainModel.CityMaster_Id = model.CityMaster_Id;
                    ////}
                    ent.Chemists.Add(domainModel);
                    ent.SaveChanges();
                    //string msg = "Welcome to PSWELLNESS. Your User Name :  " + domainModel.EmailId + "(" + domainModel.ChemistId + ") , Password : " + admin.Password + ".";
                    //Message.SendSms(domainModel.MobileNumber, msg);

                    //string msg1 = "Welcome to PSWELLNESS. Your User Name :  " + admin.Username + "(" + admin.UserID + "), Password : " + admin.Password + ".";

                    //bool Isvalid = EmailOperations.SendEmail1(model.EmailId, "Ps Wellness", msg1, true);
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
            var data = ent.Chemists.Find(id);
            var model = Mapper.Map<ChemistDTO>(data);
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName", model.StateMaster_Id);
            model.Cities = new SelectList(repos.GetCitiesByState(model.StateMaster_Id), "Id", "CityName", model.CityMaster_Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ChemistDTO model)
        {
            var existingchemist = ent.Chemists.Find(model.Id);  
            if (existingchemist == null)
            {
                TempData["msg"] = "Chemist not found";
                return RedirectToAction("Edit", new { id = model.Id });
            }
            try
            {
                
                // Licence Section 
                if (model.LicenceImageFile != null)
                {
                    var img = FileOperation.UploadImage(model.LicenceImageFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg files are allowed.";
                        return View(model);
                    }

                    model.LicenceImage = img;
                }
                //var domainModel = Mapper.Map<Chemist>(model);
                existingchemist.ChemistName = model.ChemistName;
                existingchemist.ShopName = model.ShopName;
                existingchemist.StateMaster_Id = model.StateMaster_Id;
                existingchemist.CityMaster_Id = model.CityMaster_Id;
                existingchemist.Location = model.Location;
                existingchemist.EmailId = model.EmailId;
                existingchemist.MobileNumber = model.MobileNumber;
                existingchemist.Location = model.Location;
                existingchemist.PinCode = model.PinCode;
                existingchemist.GSTNumber = model.GSTNumber;
                existingchemist.LicenseValidity = model.LicenseValidity;
                existingchemist.LicenceImage = model.LicenceImage;
                existingchemist.LicenceNumber = model.LicenceNumber;  
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
            string q = @"select v.*, IsNull(ve.UniqueId,'N/A') as UniqueId,s.StateName,c.CityName,  IsNull(ve.VendorName,'NA') AS VendorName , IsNull(ve.CompanyName,'NA') as CompanyName from Chemist v 
join StateMaster s on v.StateMaster_Id=s.Id
join CityMaster c on v.CityMaster_Id = c.Id
left join Vendor ve on ve.Id = v.Vendor_Id 
where v.IsDeleted=0 order by v.Id asc";
            var data = ent.Database.SqlQuery<ChemistDTO>(q).ToList();
            if (vendorId != null)
                data = data.Where(a => a.Vendor_Id == vendorId).ToList();
            if (term != null)
            {
                data = data.Where(A => A.ChemistName.ToLower().Contains(term) || A.ChemistId.Contains(term)).ToList();
            }
            return View(data);
        }

        [Authorize(Roles = "admin")]
        public ActionResult UpdateStatus(int id)
        {
            string q = @"update Chemist set IsApproved = case when IsApproved=1 then 0 else 1 end where id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            string mobile = ent.Database.SqlQuery<string>("select MobileNumber from Chemist where Id=" + id).FirstOrDefault();
            string Email = ent.Database.SqlQuery<string>(@"select EmailId from Chemist where Id=" + id).FirstOrDefault();
            string Name = ent.Database.SqlQuery<string>(@"select ChemistName from Chemist where Id=" + id).FirstOrDefault();
            var msg = "Dear " + Name + ", Now you Can Login With Your Registered EmailId " + Email + " and Pasword";
            Message.SendSms(mobile, msg);
            return RedirectToAction("All");
        }

        public ActionResult UpdateBankUpdateStatus(int id)
        {
            string q = @"update Chemist set IsBankUpdateApproved = case when IsBankUpdateApproved=1 then 0 else 1 end where id=" + id;
            ent.Database.ExecuteSqlCommand(q);

            string mobile = ent.Database.SqlQuery<string>("select MobileNumber from Chemist where Id=" + id).FirstOrDefault();
            string Email = ent.Database.SqlQuery<string>(@"select EmailId from Chemist where Id=" + id).FirstOrDefault();
            string Name = ent.Database.SqlQuery<string>(@"select ChemistName from Chemist where Id=" + id).FirstOrDefault();
            //var msg = "Dear " + Name + ", Now you Can Upadate your bank details.";
            //Message.SendSms(mobile, msg);
            var query = "SELECT IsBankUpdateApproved FROM Chemist WHERE Id = @Id";
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

        [Authorize(Roles = "admin")]
        public ActionResult Delete(int id)
        {
            var data = ent.Chemists.Find(id);
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

        public ActionResult OrderHistory(string term, DateTime? startDate, DateTime? endDate, int page = 1)
        {
            int loginId = Convert.ToInt32(User.Identity.Name);
            int chemistId = ent.Chemists.FirstOrDefault(a => a.AdminLogin_Id == loginId).Id;
            string orderQuery = @"exec sp_getChemistOrder @chemistId=" + chemistId;
            var ord = ent.Database.SqlQuery<PatientOrderModel>(orderQuery).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                ord = ord.Where(a => a.Name.Contains(term) || a.OrderId.StartsWith(term)).ToList();
            }
            if (startDate != null && endDate != null)
            {
                ord = ord.Where(a => a.OrderDate >= startDate && a.OrderDate <= endDate).ToList();
            }
            foreach (var o in ord)
            {
                string orderDetailQuery = @"exec sp_getOrderDetail @orderId=" + o.Id;
                var od = ent.Database.SqlQuery<PatientOrderDetailModel>(orderDetailQuery).ToList();
                o.OrderDetail = od;
            }
            //////
            int totalRecords = ord.Count;
            int recordPerpage = 5;
            double totalPages = Math.Ceiling((double)totalRecords / recordPerpage);
            int skip = recordPerpage * (page - 1);
            ord = ord.OrderByDescending(a => a.Id).Skip(skip).Take(recordPerpage).ToList();
            var model = new ChemistOrderHistory();
            model.Page = page;
            model.NoOfPages = (int)totalPages;
            model.Orders = ord;
            model.sDate = startDate;
            model.eDate = endDate;
            model.term = term;
            return View(model);
        }

        public ActionResult UpdateDeliveryStatus(int orderId, string term, DateTime? startDate, DateTime? endDate, int page = 1)
        {
            try
            {
                string query = @"update MedicineOrder set IsDelivered = Case when IsDelivered = 0 then 1 else 0 end where Id=" + orderId;
                ent.Database.ExecuteSqlCommand(query);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return RedirectToAction("OrderHistory", new { term = term, startDate = startDate, endDate = endDate, page = page });
        }


        public ActionResult PayoutReport(int id)
        {
            var model = new ViewPayOutHistory();
            var Name = ent.Database.SqlQuery<string>("select ChemistName from Chemist where Id=" + id).FirstOrDefault();
            model.ChemistName = Name;
            string qry = @"select Dp.Id, ISNULL(Dp.IsPaid, 0) as IsPaid , Dp.IsGenerated, Dp.Health_Id, Dp.PaymentDate, Dp.Amount, D.LabName from  HealthPayOut Dp join HealthCheckupCenter D on D.Id = Dp.Health_Id  where  Dp.Health_Id=" + id;
            var data = ent.Database.SqlQuery<HistoryOfChemist_Payout>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            else
            {
                model.HistoryOfChemist_Payout = data;
            }
            return View(model);
        }

        public ActionResult PaymentReport(int id, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDTO();
            var chemist = @"select * from Chemist join CityMaster on CityMaster.Id = Chemist.CityMaster_Id join StateMaster on StateMaster.Id = Chemist.StateMaster_Id where Chemist.Id=" + id;
            var mek = ent.Database.SqlQuery<ReportDTO>(chemist).ToList();
            model.ShopName = mek.FirstOrDefault().ShopName;
            model.ChemistName = mek.FirstOrDefault().ChemistName;
            model.LicenceNumber = mek.FirstOrDefault().LicenceNumber;
            model.MobileNumber = mek.FirstOrDefault().MobileNumber;
            model.StateName = mek.FirstOrDefault().StateName;
            model.CityName = mek.FirstOrDefault().CityName;
            model.Location = mek.FirstOrDefault().Location;
            if (sdate != null && edate != null)
            {
                var labs = @"select  convert(date, MO.OrderDate) as OrderDate1, MO.OrderDate, Sum(MODs.Amount) as Amount,C.ChemistName, C.ShopName, C.MobileNumber,CM.CityName, S.StateName, C.Location, C.LicenceNumber from Chemist C join MedicineOrder MO on C.Id = MO.Chemist_Id join MedicineOrderDetail MODs on MODs.Order_Id = MO.Id join CityMaster CM on CM.Id = C.CityMaster_Id join StateMaster S on S.Id = C.StateMaster_Id WHERE C.Id='" + id + "' and MO.OrderDate between '" + sdate + "' and '" + edate + "' group by MO.OrderDate,C.ChemistName, C.ShopName, C.MobileNumber,CM.CityName, S.StateName, C.Location, C.LicenceNumber ";
                var lab = ent.Database.SqlQuery<ChemistReport>(labs).ToList();
                //doctorList = doctorList.Where(a => a.AppointmentDate >= sdate && a.AppointmentDate <= edate).ToList();
                model.ChemistReport = lab;
            }
            else
            {
                var doctor = @"select  convert(date, MO.OrderDate) as OrderDate1, MO.OrderDate, Sum(MODs.Amount) as Amount,C.ChemistName, C.ShopName, C.MobileNumber,CM.CityName, S.StateName from Chemist C join MedicineOrder MO on C.Id = MO.Chemist_Id join MedicineOrderDetail MODs on MODs.Order_Id = MO.Id join CityMaster CM on CM.Id = C.CityMaster_Id join StateMaster S on S.Id = C.StateMaster_Id WHERE C.Id='" + id + "'  and datepart(mm,MO.OrderDate) =month(getdate()) group by MO.OrderDate,C.ChemistName, C.ShopName, C.MobileNumber,CM.CityName, S.StateName, C.Location, C.LicenceNumber ";
                var labList = ent.Database.SqlQuery<ChemistReport>(doctor).ToList();
                model.ChemistReport = labList;
            }
            ViewBag.Total = model.ChemistReport.Sum(a => a.Amount);
            return View(model);
        }


        [HttpPost]
        public ActionResult UpdateDelivery(DeliveryUpdateDTO model)
        {
            string msg = "";
            var data = ent.MedicineOrders.Find(model.code);
            var domainModel = Mapper.Map<MedicineOrder>(data);
            if (data.DeliveryDate != null)
            {
                domainModel.DeliveryStatus = model.deliveryStatus;
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    msg = string.Join("|", ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage));
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    domainModel.IsDelivered = true;
                    domainModel.DeliveryDate = model.date.Date;
                    domainModel.DeliveryStatus = model.deliveryStatus;
                    domainModel.Remarks = model.description;
                }
            }
            ent.Entry(domainModel).State = System.Data.Entity.EntityState.Modified;
            ent.SaveChanges();
            msg = "Success";
            return Json(msg, JsonRequestBehavior.AllowGet);

    }

        [HttpGet]
        public ActionResult AddChemistMedicine()
        {
            var model = new MedicineDTO();
            model.MedicineList = new SelectList(ent.Medicines.Where(a => a.IsDeleted == false).ToList(), "Id", "MedicineName");
            return View(model);
        }
        [HttpPost]
        public ActionResult AddChemistMedicine(MedicineDTO model)
        {
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (ent.ChemistMedicines.Where(a => a.IsDeleted == false).Any(a => a.Medicine_Id == model.MedicineId))
                    {
                        TempData["msg"] = "Already exist.";
                        return RedirectToAction("AddChemistMedicine");
                    }
                     
                    var domailmodel = new ChemistMedicine
                    {
                        Chemist_Id = GetChemistId(),
                        Medicine_Id = model.MedicineId, 
                        IsDeleted = false,
                        EntryDate = DateTime.Now,
                    };
                    ent.ChemistMedicines.Add(domailmodel);
                    ent.SaveChanges();
                     
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

            return RedirectToAction("AddChemistMedicine");
        }
        [HttpGet]
        public ActionResult MedicineList()
        {
            string query = @"select * from ChemistMedicine cm 
join Medicine m on cm.Medicine_Id = m.Id
where cm.IsDeleted=0 and cm.Chemist_Id="+ GetChemistId() + " order by m.MedicineName asc";
            var data = ent.Database.SqlQuery<MedicineDTO>(query).ToList();
             
            return View(data);
        }

        public ActionResult DeleteMedicine(int id)
        {
            var data = ent.ChemistMedicines.Find(id);
            try
            {
                data.IsDeleted = true;
                ent.SaveChanges();
                TempData["msg"] = "ok";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return RedirectToAction("MedicineList");
        }
    }
}
