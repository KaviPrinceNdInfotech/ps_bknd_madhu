using AutoMapper;
using HospitalPortal.BL;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Repositories;
using HospitalPortal.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static HospitalPortal.Utilities.EmailOperations;

namespace HospitalPortal.Controllers
{
    [Authorize(Roles = "admin,Franchise,driver")]
    public class DriverController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(DriverController));
        GenerateBookingId Driver = new GenerateBookingId();

        [AllowAnonymous]
        public ActionResult Add(int vendorId = 0)
        {
            var model = new DriverDTO();
            model.Vendor_Id = vendorId;
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
            model.VehicleList = new SelectList(repos.GetVehicleType(), "Id", "VehicleTypeName");
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Add(DriverDTO model)
        {
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (!string.IsNullOrEmpty(model.OtherCity))
                        ModelState.Remove("CityMaster_Id");
                    if (!ModelState.IsValid)
                    {
                        model.VehicleList = new SelectList(repos.GetVehicleType(), "Id", "VehicleName");
                        model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                        return View(model);
                    }
                    //if (ent.AdminLogins.Any(a => a.Username == model.MobileNumber))
                    //{
                    //    TempData["msg"] = "This Mobile Number has already exists.";
                    //    return RedirectToAction("Add");
                    //}
                    //if (ent.AdminLogins.Any(a => a.PhoneNumber == model.MobileNumber))
                    //{
                    //    TempData["msg"] = "This Mobile Number has already exists.";
                    //    return RedirectToAction("Add");
                    //}
                    if (ent.Drivers.Any(a => a.DriverName == model.DriverName && a.MobileNumber == model.MobileNumber))
                    {
                        var data = ent.Drivers.Where(a => a.DriverName == model.DriverName && a.MobileNumber == model.MobileNumber).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.DriverId).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        TempData["msg"] = "you are already registered with pswellness";
                        return RedirectToAction("Add", "Driver");
                    }

                    var admin = new AdminLogin
                    {
                        Username = model.EmailId,
                        PhoneNumber = model.MobileNumber,
                        Password = model.Password,
                        Role = "driver"
                    };
                    ent.AdminLogins.Add(admin);
                    ent.SaveChanges();
                     

                    //DL image upload section
                    if (model.DlFile == null)
                    {
                        TempData["msg"] = "DL Picture can not be null";
                        model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                        tran.Rollback();
                        return View(model);
                    }
                    else
                    {
                        var dlImg = FileOperation.UploadImage(model.DlFile, "Images");
                        if (dlImg == "not allowed")
                        {
                            TempData["msg"] = "Only png,jpg,jpeg files are allowed as DL Image.";
                            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                            tran.Rollback();
                            return View(model);
                        }
                        model.DlImage1 = dlImg;
                    }

                    if (model.DlFile1 == null)
                    {
                        TempData["msg"] = "DL Picture can not be null";
                        model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                        tran.Rollback();
                        return View(model);
                    }
                    else
                    {
                        var dlImg1 = FileOperation.UploadImage(model.DlFile1, "Images");
                        if (dlImg1 == "not allowed")
                        {
                            TempData["msg"] = "Only png,jpg,jpeg files are allowed as DL Image.";
                            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                            tran.Rollback();
                            return View(model);
                        }
                        model.DlImage2 = dlImg1;
                    }
                     
                    //Pan Doc Upload
                    if (model.PanImageFile != null)
                    {
                        var PanImg = FileOperation.UploadImage(model.PanImageFile, "Images");
                        if (PanImg == "not allowed")
                        {
                            TempData["msg"] = "Only png,jpg,jpeg files are allowed as Aadhar card document";
                            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                            tran.Rollback();
                            return View(model);
                        }
                        model.PanImage = PanImg;
                    }
                    // police verification image upload
                    if (model.VerificationImage != null)
                    {
                        var verf = FileOperation.UploadImage(model.VerificationImage, "Images");
                        if (verf == "not allowed")
                        {
                            TempData["msg"] = "Only png, jpg, jpeg, pdf files are allowed as police verification document";
                            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                        tran.Rollback();
                            return View(model);
                        }
                        model.VerificationDoc = verf;
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
                        model.CityMaster_Id = cityMaster.Id;
                    }
                    var domainModel = Mapper.Map<Driver>(model);                   
                    domainModel.EmailId = model.EmailId?? "null";
                    domainModel.DriverId = Driver.GenerateDriverId();
                    admin.UserID = domainModel.DriverId;                   
                    domainModel.AdminLogin_Id = admin.Id;
                    domainModel.PAN = domainModel.PAN;
                    domainModel.JoiningDate = DateTime.Now;
                    domainModel.IsBankUpdateApproved = false;
                    ent.Drivers.Add(domainModel);
                    ent.SaveChanges();
                     
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

            return RedirectToAction("Add", new { vendorId = model.Vendor_Id });
        }

        public ActionResult Edit(int id)
        {
            var data = ent.Drivers.Find(id);
            var model = Mapper.Map<DriverDTO>(data);
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName", model.StateMaster_Id);
            model.Cities = new SelectList(repos.GetCitiesByState(model.StateMaster_Id), "Id", "CityName", model.CityMaster_Id);
            model.VehicleList = new SelectList(repos.GetVehicleType(), "Id", "VehicleTypeName", model.VehicleType_Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(DriverDTO model)
        {
            try
            {
                var existingdriver = ent.Drivers.Find(model.Id);
                if (existingdriver == null)
                {
                    TempData["msg"] = "Driver not found";
                    return RedirectToAction("Edit", new { id = model.Id });
                }

                if (model.DlFile != null)
                {
                    var dlImg = FileOperation.UploadImage(model.DlFile, "Images");
                    if (dlImg == "not allowed")
                    {

                        TempData["msg"] = "Only png,jpg,jpeg files are allowed as DL Image.";

                        return View(model);
                    }
                    model.DlImage1 = dlImg;
                }
                if (model.DlFile1 != null)
                {
                    var dlImg = FileOperation.UploadImage(model.DlFile1, "Images");
                    if (dlImg == "not allowed")
                    {

                        TempData["msg"] = "Only png,jpg,jpeg files are allowed as DL Image.";

                        return View(model);
                    }
                    model.DlImage2 = dlImg;
                }
                if (model.AadharImageFile != null)
                {
                    var adImg = FileOperation.UploadImage(model.AadharImageFile, "Images");
                    if (adImg == "not allowed")
                    {

                        TempData["msg"] = "Only png,jpg,jpeg files are allowed as DL Image.";

                        return View(model);
                    }
                    model.AadharImage = adImg;
                }
                if (model.AadharImageFile2 != null)
                {
                    var adImg2 = FileOperation.UploadImage(model.AadharImageFile2, "Images");
                    if (adImg2 == "not allowed")
                    {

                        TempData["msg"] = "Only png,jpg,jpeg files are allowed as DL Image.";

                        return View(model);
                    }
                    model.AadharImage2 = adImg2;
                }

                //Pan Doc
                if (model.PanImageFile != null)
                {
                    var PanImg = FileOperation.UploadImage(model.PanImageFile, "Images");
                    if (PanImg == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg files are allowed as Aadhar card document";
                        model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");

                        return View(model);
                    }
                    model.PanImage = PanImg;
                }
                // verfn doc

                if (model.VerificationImage != null)
                {
                    var verf = FileOperation.UploadImage(model.VerificationImage, "Images");
                    if (verf == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg files are allowed as Aadhar card document";
                        model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");

                        return View(model);
                    }
                    model.VerificationDoc = verf;
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
                    model.CityMaster_Id = cityMaster.Id;
                }
                var domainModel = Mapper.Map<Driver>(model);
                existingdriver.DriverName = model.DriverName;
                existingdriver.EmailId = model.EmailId;
                existingdriver.StateMaster_Id = model.StateMaster_Id;
                existingdriver.CityMaster_Id = model.CityMaster_Id;
                existingdriver.Location = model.Location;
                existingdriver.EmailId = model.EmailId;
                existingdriver.MobileNumber = model.MobileNumber;
                existingdriver.Location = model.Location;
                existingdriver.PinCode = model.PinCode;
                existingdriver.VehicleType_Id = model.VehicleType_Id;
                existingdriver.Paidamount = model.Paidamount;
                existingdriver.DlNumber = model.DlNumber;
                existingdriver.DlValidity = model.DlValidity;
                existingdriver.PAN = model.PAN;
                existingdriver.DlValidity = model.DlValidity;
                existingdriver.DlImage1 = model.DlImage1;
                existingdriver.DlImage2 = model.DlImage2;
                existingdriver.AadharImage = model.AadharImage;
                existingdriver.AadharImage2 = model.AadharImage2;
                existingdriver.PanImage = model.PanImage;
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

        public ActionResult All(int? vendorId, string term = null, int? page = 0)
        {
            var model = new DriverDTO();
            string q = @"select d.Id, d.DriverId, d.DriverName, d.MobileNumber, d.EmailId, d.Location, d.DriverImage, d.DlImage,
d.DlImage1,
d.DlImage2,
d.DlImage3,
d.DlNumber,
d.Paidamount,
d.DlValidity, d.PAN, d.AadharNumber, d.AadharImage, 
d.AadharImage2,
d.PanImage,
IsNull(vt.Id,0) as VehicleType_Id,
d.VerificationDoc,
d.Vendor_Id,
IsNull(ve.UniqueId,'N/A') as UniqueId,
d.IsApproved,d.IsBankUpdateApproved, d.IsDeleted,
IsNull(vt.VehicleTypeName,'na') as VehicleTypeName, s.StateName,c.CityName, IsNull(ve.VendorName,'NA') AS VendorName , IsNull(ve.CompanyName,'NA') from Driver d 
join StateMaster s on d.StateMaster_Id=s.Id
join CityMaster c on d.CityMaster_Id = c.Id
left join VehicleType vt on vt.Id = d.VehicleType_Id
left join Vendor ve on ve.Id = d.Vendor_Id
where d.IsDeleted=0 order by d.Id asc";
            var data = ent.Database.SqlQuery<DriverDTO>(q).ToList();
            if (vendorId != null)
                data = data.Where(a => a.Vendor_Id == vendorId).ToList();
            if (term != null)
                data = data.Where(A => A.DriverName.ToLower().Contains(term) || A.DriverId.Contains(term)).ToList();
            if(data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(data);
            }
            int total = data.Count;
            page = page ?? 1;
            int pageSize = 10;
            decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
            model.NumberOfPages = (int)noOfPages;
            model.Page = page;
            data = data.OrderByDescending(a => a.Id).Skip(pageSize * ((int)page - 1)).Take(pageSize).ToList();
            data.FirstOrDefault().NumberOfPages = model.NumberOfPages;
            data.FirstOrDefault().Page = model.Page;
            return View(data);
        }

        public ActionResult UpdateStatus(int id)
        {
            string q = @"update Driver set IsApproved = case when IsApproved=1 then 0 else 1 end where id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            string mobile = ent.Database.SqlQuery<string>("select MobileNumber from Driver where Id=" + id).FirstOrDefault();
            string Email = ent.Database.SqlQuery<string>(@"select EmailId from Driver where Id=" + id).FirstOrDefault();
            string Name = ent.Database.SqlQuery<string>(@"select DriverName from Driver where Id=" + id).FirstOrDefault();
            var msg = "Dear " + Name + ", Now you Can Login With Your Registered EmailId " + Email + " and Pasword";
            Message.SendSms(mobile, msg);
            return RedirectToAction("All");
        }

        public ActionResult UpdateBankUpdateStatus(int id)
        {
            string q = @"update Driver set IsBankUpdateApproved = case when IsBankUpdateApproved=1 then 0 else 1 end where id=" + id;
            ent.Database.ExecuteSqlCommand(q);

            string mobile = ent.Database.SqlQuery<string>("select MobileNumber from Driver where Id=" + id).FirstOrDefault();
            string Email = ent.Database.SqlQuery<string>(@"select EmailId from Driver where Id=" + id).FirstOrDefault();
            string Name = ent.Database.SqlQuery<string>(@"select DriverName from Driver where Id=" + id).FirstOrDefault();
            //var msg = "Dear " + Name + ", Now you Can Upadate your bank details.";
            //Message.SendSms(mobile, msg);
            var query = "SELECT IsBankUpdateApproved FROM Driver WHERE Id = @Id";
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
            var data = ent.Drivers.Find(id);
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

        [AllowAnonymous]
        [HttpGet]
        public ActionResult UpdateVehicle(int? id, VehicleAllotmentDTO model)
        {
            if (!string.IsNullOrEmpty(model.VehicleNumber))
            {
                int VehicleId = ent.Database.SqlQuery<int>(@"select Id from Vehicle where VehicleNumber=" + model.VehicleNumber).FirstOrDefault();
                int Id = ent.Database.SqlQuery<int>(@"select Id from Vehicle where Driver_Id=" + model.Id).FirstOrDefault();
                string q = @"update Vehicle set Driver_Id = " + model.Id + "  where Id=" + Id;
                ent.Database.ExecuteSqlCommand(q);
                string VehicleNumber = ent.Database.SqlQuery<string>("select VehicleNumber from Vehicle where Driver_Id=" + model.Id).FirstOrDefault();
                string DriverName = ent.Database.SqlQuery<string>("select DriverName from Driver where Id=" + model.Id).FirstOrDefault();
                TempData["msg"] = "The Vehicle Number" + VehicleNumber + " has been Replaced to " + DriverName;
                return RedirectToAction("UpdateVehicle", new { model.Id });
            }
            return View();
        }

        //public JsonResult GetVehicleNumberList(string term)
        //{
        //    var VehicleList = (from N in ent.Vehicles
        //                       where N.VehicleNumber.StartsWith(term)
        //                       select new { N.VehicleNumber, N.Id });
        //    //TempData["VehicleId"] = VehicleList.FirstOrDefault().Id;
        //    //TempData["VehicleNumber"] = VehicleList.FirstOrDefault().VehicleNumber;
        //    return Json(VehicleList, JsonRequestBehavior.AllowGet);
        //}

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Update(VehicleAllotmentDTO model)
        {
            int VehicleId = ent.Database.SqlQuery<int>(@"select Id from Vehicle where VehicleNumber='" + model.VehicleNumber+"'").FirstOrDefault();
            if (VehicleId > 0)
            {
                string q = @"update Vehicle set Driver_Id = " + model.Id + "  where Id=" + VehicleId;
                ent.Database.ExecuteSqlCommand(q);
                string VehicleNumber = ent.Database.SqlQuery<string>("select VehicleNumber from Vehicle where Driver_Id=" + model.Id).FirstOrDefault();
                string DriverName = ent.Database.SqlQuery<string>("select DriverName from Driver where Id=" + model.Id).FirstOrDefault();
                TempData["msg"] = "The Vehicle Number" + VehicleNumber + " has been Replaced to " + DriverName;
            }
            else
            {
                TempData["msg"] = "Kindly Check Vehicle Number.";
            }
            return RedirectToAction("UpdateVehicle", new { model.Id });
        }

        public ActionResult TravelHistory(int? id = null)
        {
            var model = new TravelHistoryVM();
            string q = @"select * from TravelRecordMaster trm join Patient p on p.Id = trm.Patient_Id join Driver d on d.Id = trm.Driver_Id 
join Vehicle v on v.Id = trm.Vehicle_Id join VehicleType vt on v.VehicleType_Id = vt.Id where Convert(Date,RequestDate)= GETdATE() ORDER BY RequestDate DESC";
            var data = ent.Database.SqlQuery<travelHistoryValues>(q).ToList();
            if(data.Count() < 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            if(id != 0)
            {
                data = data.Where(a => a.Driver_Id == id).ToList();
                if (data.Count() < 0)
                {
                    TempData["msg"] = "No Records";
                    return View(model);
                }
            }
            model.travelHistory = data;
            return View(model);
        }
    }
}