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
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static HospitalPortal.Utilities.EmailOperations;

namespace HospitalPortal.Controllers
{
    [Authorize(Roles ="admin,nurse,Franchise")]
    public class NurseController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(NurseController));
        GenerateBookingId bk = new GenerateBookingId();

        private int GetNurseId()
        {
            int loginId = Convert.ToInt32(User.Identity.Name);
            int nurseId = ent.Database.SqlQuery<int>("select Id from nurse where AdminLogin_Id=" + loginId).FirstOrDefault();
            return nurseId;
        }

        [AllowAnonymous]
        public ActionResult Add(int vendorId = 0, int hospitalId = 0)
        {
            var model = new NurseDTO();
            if (vendorId > 0)
            {
                model.Vendor_Id = vendorId;
            }
            if (hospitalId > 0)
            {
                model.HospitalId = hospitalId;
            }
            model.Vendor_Id = vendorId;
            model.NurseTypes = new SelectList(ent.NurseTypes.ToList(), "Id", "NurseTypeName");
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Add(NurseDTO model)
        {
            using (var tran = ent.Database.BeginTransaction())
            {
                if (!string.IsNullOrEmpty(model.OtherCity))
                    ModelState.Remove("CityMaster_Id");
                model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                model.NurseTypes = new SelectList(ent.NurseTypes.ToList(), "Id", "NurseTypeName");
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return View(model);
                    }
                    //if (ent.AdminLogins.Any(a=>a.Username==model.EmailId))
                    //{
                    //    TempData["msg"] = "This EmailId has already exists.";
                    //    return RedirectToAction("Add");
                    //}
                    //if (ent.AdminLogins.Any(a => a.PhoneNumber == model.MobileNumber))
                    //{
                    //    TempData["msg"] = "This Mobile Number has already exists.";
                    //    return RedirectToAction("Add");
                    //}

                    if (ent.Nurses.Any(a => a.NurseName == model.NurseName && a.MobileNumber == model.MobileNumber))
                    {
                        var data = ent.Nurses.Where(a => a.NurseName == model.NurseName && a.MobileNumber == model.MobileNumber).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.NurseId).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        TempData["msg"] = "you are already registered with pswellness";
                        return RedirectToAction("Add", "Nurse");
                    }

                    var admin = new AdminLogin
                    {
                        Username = model.EmailId,
                        PhoneNumber=model.MobileNumber,
                        Password = model.Password,
                        Role = "nurse" };
                    ent.AdminLogins.Add(admin);
                    ent.SaveChanges();

                    if(model.AadharImageBase !=null)
                    {
                        string path = Server.MapPath("~/Images/") + model.AadharImageBase.FileName;
                        model.AadharImageBase.SaveAs(path);
                    }
                    model.AadharImageName = model.AadharImageBase != null ? model.AadharImageBase.FileName : "";

                    if (model.NurseImageBase != null)
                    {
                        string path = Server.MapPath("~/Images/") + model.NurseImageBase.FileName;
                        model.NurseImageBase.SaveAs(path);
                    }
                    model.NurseImage = model.NurseImageBase != null ? model.NurseImageBase.FileName : "";


                    // CertificateFile Picture Section 
                    if (model.CertificateFile == null)
                    {
                        TempData["msg"] = "Certificate Image File Picture can not be null";
                        tran.Rollback();
                        return View(model);
                    }
                    var img = FileOperation.UploadImage(model.CertificateFile, "Images");
                    if(img == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg files are allowed.";
                        tran.Rollback();
                        return View(model);
                    }
                    model.CertificateImage = img;

                    // Pan Image
                    if (model.PanImageFile == null)
                    {
                        TempData["msg"] = "Pan Image File Picture can not be null";
                        tran.Rollback();
                        return View(model);
                    }
                    var panimg = FileOperation.UploadImage(model.PanImageFile, "Images");
                    if (panimg == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg files are allowed.";
                        tran.Rollback();
                        return View(model);
                    }
                    model.PanImage = panimg;
                     
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
                    var domainModel = Mapper.Map<Nurse>(model);
                    domainModel.Vendor_Id = model.Vendor_Id == null ? null : model.Vendor_Id;
                    domainModel.Hospital_Id = model.HospitalId == null ? null : model.HospitalId;
                    if (model.CityName != null)
                    {
                        domainModel.CityMaster_Id = (int)model.CityMaster_Id;
                    }
                    domainModel.JoiningDate = DateTime.Now;
                    domainModel.AdminLogin_Id = admin.Id;
                    domainModel.NurseId = bk.GenerateNurseId();
                    admin.UserID = domainModel.NurseId;
                    domainModel.PAN = domainModel.PAN;
                    domainModel.IsBankUpdateApproved = false;
                    ent.Nurses.Add(domainModel);
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
            return RedirectToAction("Add", new { vendorId = model.Vendor_Id, hospitalId = model.HospitalId });
        }

        public ActionResult Edit(int id)
        {
            var data = ent.Nurses.Find(id);
            var model = Mapper.Map<NurseDTO>(data);
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName",model.StateMaster_Id);
            model.Cities = new SelectList(repos.GetCitiesByState(model.StateMaster_Id), "Id", "CityName",model.CityMaster_Id);
                model.NurseTypes = new SelectList(ent.NurseTypes.ToList(), "Id", "NurseTypeName",model.NurseType_Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(NurseDTO model)
        {
            try
            {
                var existingnurse = ent.Nurses.Find(model.Id); 
                if (existingnurse == null)
                {
                    TempData["msg"] = "Nurse not found";
                    return RedirectToAction("Edit", new { id = model.Id });
                }

                // Nurse Image
                if (model.NurseImageBase != null)
                {
                    var nImg = FileOperation.UploadImage(model.NurseImageBase, "Images");
                    if (nImg == "not allowed")
                    {
                        TempData["msg"] = "Only png, jpg, jpeg, pdf files are allowed as Nurse Image";

                        return View(model);
                    }
                    model.NurseImage = nImg;
                }

                // CertificateFile  
                if (model.CertificateFile != null)
                {

                    var img = FileOperation.UploadImage(model.CertificateFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg files are allowed Certificate document.";
                        return View(model);
                    }
                    model.CertificateImage = img;
                }
                 
                // Pan Image
                if (model.PanImageFile != null)
                {
                    var panImg = FileOperation.UploadImage(model.PanImageFile, "Images");
                    if (panImg == "not allowed")
                    {
                        TempData["msg"] = "Only png, jpg, jpeg, pdf files are allowed as Pan document";

                        return View(model);
                    }
                    model.PanImage = panImg;
                }

                //var domainModel = Mapper.Map<Nurse>(model);

                existingnurse.NurseName = model.NurseName;
                existingnurse.NurseType_Id = model.NurseType_Id;
                existingnurse.StateMaster_Id = model.StateMaster_Id;
                existingnurse.CityMaster_Id = model.CityMaster_Id;
                existingnurse.Location = model.Location;
                existingnurse.EmailId = model.EmailId; 
                existingnurse.MobileNumber = model.MobileNumber; 
                existingnurse.Location = model.Location;
                existingnurse.PinCode = model.PinCode;
                existingnurse.CertificateImage = model.CertificateImage;
                existingnurse.CertificateNumber = model.CertificateNumber;
                existingnurse.PanImage = model.PanImage;
                existingnurse.NurseImage = model.NurseImage;
                existingnurse.Fee = model.Fee;
                existingnurse.PAN = model.PAN;

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
            string q = @"select v.*,IsNull(ve.UniqueId,'N/A') as UniqueId, s.StateName,nt.NurseTypeName,c.CityName, IsNull(ve.VendorName,'NA') AS VendorName , IsNull(ve.CompanyName,'NA') as CompanyName from Nurse v 
join StateMaster s on v.StateMaster_Id=s.Id
join NurseType nt on v.NurseType_Id = nt.Id 
join CityMaster c on v.CityMaster_Id = c.Id
left join Vendor ve on ve.Id = v.Vendor_Id
where v.IsDeleted=0 order by v.Id desc";
            var data = ent.Database.SqlQuery<NurseDTO>(q).ToList();
            if (vendorId != null)
                data = data.Where(a => a.Vendor_Id == vendorId).ToList();
            if (term != null)
                data = data.Where(a => a.NurseName.ToLower().Contains(term) || a.NurseId.Contains(term)).ToList();                            
            return View(data);
        }

        public FileResult DownloadFile(int? fileId)
        {
            var file = ent.Nurses.ToList().Find(x => x.Id == fileId.Value);
            return File(file.Data, file.ContentType, file.FileName);

        }

        [HttpGet]
        public ActionResult EverifyDocs(int id)
        {
            var result = ent.Nurses.FirstOrDefault(x => x.Id == id);
            return View(result);
        }
        [HttpPost]
        public ActionResult EverifyDocs(Nurse model , HttpPostedFileBase postedFile)
        {
            var emp = ent.Nurses.FirstOrDefault(x => x.Id == model.Id);
            byte[] bytes;
            using (BinaryReader br = new BinaryReader(postedFile.InputStream))
            {
                bytes = br.ReadBytes(postedFile.ContentLength);
            }

            if (emp != null)
            {
                emp.FileName = Path.GetFileName(postedFile.FileName);
                emp.ContentType = postedFile.ContentType;
                emp.Data = bytes;
            }
            ent.SaveChanges();
            ModelState.Clear();
            return View();
        }
        public ActionResult UpdateStatus(int id)
        {
            string q = @"update Nurse set IsApproved = case when IsApproved=1 then 0 else 1 end where id="+id;
            ent.Database.ExecuteSqlCommand(q);
            string mobile = ent.Database.SqlQuery<string>("select MobileNumber from Nurse where Id=" + id).FirstOrDefault();
            string Email = ent.Database.SqlQuery<string>(@"select EmailId from Nurse where Id=" + id).FirstOrDefault();
            string Name = ent.Database.SqlQuery<string>(@"select NurseName from Nurse where Id=" + id).FirstOrDefault();
            var msg = "Dear " + Name + ", Now you Can Login With Your Registered EmailId " + Email + " and Pasword";
            Message.SendSms(mobile, msg);
            return RedirectToAction("All");
        }
        public ActionResult UpdateBankUpdateStatus(int id)
        {
            string q = @"update Nurse set IsBankUpdateApproved = case when IsBankUpdateApproved=1 then 0 else 1 end where id=" + id;
            ent.Database.ExecuteSqlCommand(q);

            string mobile = ent.Database.SqlQuery<string>("select MobileNumber from Nurse where Id=" + id).FirstOrDefault();
            string Email = ent.Database.SqlQuery<string>(@"select EmailId from Nurse where Id=" + id).FirstOrDefault();
            string Name = ent.Database.SqlQuery<string>(@"select NurseName from Nurse where Id=" + id).FirstOrDefault();
            //var msg = "Dear " + Name + ", Now you Can Upadate your bank details.";
            //Message.SendSms(mobile, msg);
            var query = "SELECT IsBankUpdateApproved FROM Nurse WHERE Id = @Id";
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
            var data = ent.Nurses.Find(id);
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

        public ActionResult RegisterLocation()
        {
            var model = new RegisterNurseLocation();
            model.States = new SelectList(ent.StateMasters.ToList(), "Id", "StateName");

            return View(model);
        }

        [HttpPost]
        public ActionResult RegisterLocation(RegisterNurseLocation model)
        {
            try
            {

                int nurseId = GetNurseId();
                foreach(int locationId in model.LocationIds)
                {
                   ent.Database.ExecuteSqlCommand("insert into Nurse_Location(Nurse_Id,Location_Id) values("+nurseId+","+locationId+")");
                }
                TempData["msg"] = "Location Added successfully.";
            }
            catch(Exception ex)
            {
                TempData["msg"] = "Server error";
                log.Error(ex.Message);
            }
                return RedirectToAction("RegisterLocation");
        }

        public ActionResult Locations()
        {
            int nurseId = GetNurseId();
            string query = @"select nl.Id,nl.Nurse_Id,l.LocationName from Nurse_Location nl
join Location l on nl.Location_Id=l.Id
 where nl.Nurse_Id="+nurseId;
            var data = ent.Database.SqlQuery<NurseLocationModel>(query).ToList();
            return View(data);
        }

        public ActionResult GetLocation(int cityId)
        {
            int nurseId = GetNurseId();
            string query = @"select Location.* From Location where Id not in (select Location_Id from Nurse_Location where Nurse_Id = " + nurseId+ ") and IsDeleted=0 and City_Id=" + cityId;
            var locs = ent.Database.SqlQuery<LocationDTO>(query).ToList();
            return Json(locs,JsonRequestBehavior.AllowGet);
        }


        public ActionResult DeleteLocation(int id)
        {
            try
            {
                string query = @"delete from Nurse_Location where id=" + id;
                ent.Database.ExecuteSqlCommand(query);
                return Content("ok");
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
                return Content("Server error");
            }
        }

        public ActionResult AppointmentRequest()
        {
            int nurseId = GetNurseId();
            var NurseRecord = ent.Nurses.Find(nurseId);
            string query = @"exec GetNurseAppointmentList " + NurseRecord.NurseType_Id + ","+ nurseId ;
            var data = ent.Database.SqlQuery<NurseAppointmentRequestList>(query).ToList();
            return View(data);
        }

        public ActionResult AcceptAppointent(int serviceId)
        {
            try
            {
            int nurseId = GetNurseId();
            double nurseFee = ent.Nurses.Find(nurseId).Fee;
            string query = @"update NurseService set Nurse_Id="+nurseId+ ",ServiceAcceptanceDate=getdate(),ServiceStatus='Approved',PerDayAmount="+nurseFee+" where Id=" + serviceId;
            ent.Database.ExecuteSqlCommand(query);
                // send message to customer
                string msg = "Your request to Nursing Service has approved.Please Check detail  and make payment through Application";
                var mobile = ent.Database.SqlQuery<string>(@"select 
ns.MobileNumber from nurseservice ns 
join Patient p on ns.Patient_Id=p.Id
where ns.Id=
" + serviceId).FirstOrDefault();
                Message.SendSms(mobile, msg);
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
            }

            return RedirectToAction("AppointmentRequest");
        }

        public ActionResult MyAppointments(DateTime ? startDate,DateTime ? endDate, string term="", int page = 1)
        {
            var model =new  NurseAppointmentModel();
            string query = @"select ns.Id,p.PatientName,
                             p.MobileNumber as RegisteredMobileNumber,
                             ns.MobileNUmber as ContactNumber,
                             ns.IsPaid,
case when ns.PaymentDate is null then 'N/A' else Convert(nvarchar(100), ns.PaymentDate, 103) end as PaymentDate,
case when ns.ServiceAcceptanceDate is null then 'N/A' else Convert(nvarchar(100), ns.ServiceAcceptanceDate, 103) end as ServiceAcceptanceDate,
Convert(nvarchar(100), ns.RequestDate, 103) as RequestDate,
ns.RequestDate as rDate,
Convert(nvarchar(100), ns.StartDate, 103) + '-' + Convert(nvarchar(100), ns.EndDate, 103) as ServiceTiming,
Convert(nvarchar(100), ns.ServiceDate, 103) as ServiceDate,
Datediff(day,ns.StartDate,ns.EndDate) as TotalDays,
IsNull(ns.PerDayAmount,0) as Fee,
IsNull(TotalFee,0) as TotalFee,
ns.ServiceType,
ns.ServiceTime,
ns.ServiceStatus
 from NurseService ns
 join Patient p on ns.Patient_Id = p.Id
 join nurse n on ns.Nurse_Id=n.Id
where ns.Nurse_Id =n.Id
and ns.Nurse_Id =" + GetNurseId()+" order by ns.Id desc";
            var data = ent.Database.SqlQuery<NurseAppointmentWithUser>(query).ToList();
            if(term!="")
            {
                term = term.ToLower();
                data = data.Where(a => a.PatientName.ToLower().Contains(term) || a.RegisteredMobileNumber.StartsWith(term) || a.ContactNumber.StartsWith(term)).ToList();
            }
            if(startDate!=null && endDate!=null)
                data = data.Where(a => a.rDate >= startDate && a.rDate <= endDate).ToList();
            int total = data.Count;
            int pageSize = 50;
            int noOfPages = (int)Math.Ceiling((double)total / pageSize);
            int skip = pageSize * (page - 1);
            data = data.Skip(skip).Take(pageSize).ToList();
            model.Appointments = data;
            model.Page = page;
            model.NumberOfPages = noOfPages;
            model.Term = term;
            model.StartDate = startDate;
            model.EndDate = endDate;
            return View(model);
        }

        public ActionResult ChangeApprovalStatus(int serviceId)
        {
            string query = @"update NurseService set ServiceStatus= case When ServiceStatus='Approved' then 'Declined'
when ServiceStatus='Declined' then 'Approved'
else 'Pending' end where Id=" + serviceId;
            try
            {
                ent.Database.ExecuteSqlCommand(query);
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
            }
            return RedirectToAction("MyAppointments");
        }


        //[HttpGet]
        //public ActionResult DocumentList()
        //{
        //    var model = new GallertDTO();
        //    string q = @"select Id,FileName from Nurse where IsDeleted=0";
        //    var data = ent.Database.SqlQuery<Documents>(q).ToList();
        //    if (data.Count() == 0)
        //    {
        //        TempData["msg"] = "No Records";
        //        return View(model);
        //    }
        //    model.Documents = data;
        //    return View(model);
        //}
    }
}