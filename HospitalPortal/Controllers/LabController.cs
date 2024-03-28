using AutoMapper;
using HospitalPortal.BL;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Repositories;
using HospitalPortal.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static HospitalPortal.Utilities.EmailOperations;

namespace HospitalPortal.Controllers
{
    [Authorize]
    public class LabController : Controller
    {
        DbEntities ent = new DbEntities();
        returnMessage rm = new returnMessage();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(LabController));
        GenerateBookingId bk = new GenerateBookingId();

        [AllowAnonymous]
        public ActionResult Add(int vendorId = 0)
        {
            var model = new LabDTO();
            model.Vendor_Id = vendorId;
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Add(LabDTO model)
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

                    if (!ModelState.IsValid)
                        return View(model);
                    if (ent.AdminLogins.Any(a => a.Username == model.EmailId))
                    {
                        TempData["msg"] = "This EmailId has already exists.";
                        return RedirectToAction("Add");
                    }
                    if (ent.AdminLogins.Any(a => a.PhoneNumber == model.MobileNumber))
                    {
                        TempData["msg"] = "This Mobile Number has already exists.";
                        return RedirectToAction("Add");
                    }

                    if (ent.Labs.Any(a => a.LabName == model.LabName && a.MobileNumber == model.MobileNumber))
                    {
                        var data = ent.Labs.Where(a => a.LabName == model.LabName && a.MobileNumber == model.MobileNumber).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.lABId).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        TempData["msg"] = "you are already registered with pswellness";
                        return RedirectToAction("Add", "Lab");
                    }

                    var admin = new AdminLogin
                    {
                        Username = model.EmailId,
                        PhoneNumber = model.MobileNumber,
                        Password = model.Password,
                        Role = "lab"
                    };
                    ent.AdminLogins.Add(admin);
                    ent.SaveChanges();

                    

                    if (model.LicenceImageFile == null)
                    {
                        TempData["msg"] = "DL Picture can not be null";
                        model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                        tran.Rollback();
                        return View(model);
                    }
                    else
                    {
                        var Img = FileOperation.UploadImage(model.LicenceImageFile, "Images");
                        if (Img == "not allowed")
                        {
                            TempData["msg"] = "Only png,jpg,jpeg files are allowed as Licence Image.";
                            //model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                            tran.Rollback();
                            return View(model);
                        }
                        model.LicenceImage = Img;
                    }




                    if (model.PanImageFile != null)
                    {
                        var panImg = FileOperation.UploadImage(model.PanImageFile, "Images");
                        if (panImg == "not allowed")
                        {
                            TempData["msg"] = "Only png,jpg,jpeg files are allowed as Pan card document";
                            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                            tran.Rollback();
                            return View(model);
                        }
                        model.PanImage = panImg;
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
                    var domainModel = Mapper.Map<Lab>(model);
                    domainModel.LabTypeName = model.LabTypeName;
                    domainModel.year = model.year;
                    domainModel.About = model.About;
                    domainModel.PinCode = model.PinCode;
                    domainModel.JoiningDate = DateTime.Now;
                    domainModel.PAN = model.PAN;
                    domainModel.Location_Id = (int)model.Location_Id;                   
                    domainModel.AdminLogin_Id = admin.Id;
                    domainModel.IsBankUpdateApproved = false;
                    domainModel.lABId = bk.GenerateLabId();

                    admin.UserID = domainModel.lABId;
                    ent.Labs.Add(domainModel);
                    ent.SaveChanges();
                    //string msg = "Welcome to PSWELLNESS. Your User Name :  " + domainModel.EmailId + "(" + domainModel.lABId + "), Password : " + admin.Password + ".";
                    //Message.SendSms(domainModel.MobileNumber, msg);
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
                // catch (DbEntityValidationException ex)
                catch (DbEntityValidationException ex)
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
            var data = ent.Labs.Find(id);
            var model = Mapper.Map<LabDTO>(data);
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName", model.StateMaster_Id);
            model.Cities = new SelectList(repos.GetCitiesByState(model.StateMaster_Id), "Id", "CityName", model.CityMaster_Id);
            model.Locations = new SelectList(repos.GetLocationByCity(model.CityMaster_Id),"Id","LocationName",model.Location_Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(LabDTO model)
        { 
                try
                {
                    var existinglab = ent.Labs.Find(model.Id); // Assuming 'Id' is the primary key
                    if (existinglab == null)
                    {
                        TempData["msg"] = "Lab not found";
                        return RedirectToAction("Edit", new { id = model.Id });
                    }
                    //licence Picture Section
                    if (model.LicenceImageFile != null)
                    {

                        var limg = FileOperation.UploadImage(model.LicenceImageFile, "Images");
                        if (limg == "not allowed")
                        {
                            TempData["msg"] = "Only png,jpg,jpeg files are allowed.";
                            
                            return View(model);
                        }
                        model.LicenceImage = limg;
                    }
                    // Pan doc upload
                    if (model.PanImageFile != null)
                    {
                        var PanImg = FileOperation.UploadImage(model.PanImageFile, "Images");
                        if (PanImg == "not allowed")
                        {
                            TempData["msg"] = "Only png,jpg,jpeg files are allowed as Aadhar card document";
                            
                            return View(model);
                        }
                        model.PanImage = PanImg;
                    }
                   // var domainModel = Mapper.Map<Lab>(model);
                    existinglab.LabName = model.LabName;
                    existinglab.MobileNumber = model.MobileNumber;
                    existinglab.PhoneNumber = model.PhoneNumber;
                    existinglab.GSTNumber = model.GSTNumber;
                    existinglab.PhoneNumber = model.PhoneNumber;
                    existinglab.EmailId = model.EmailId;
                    existinglab.StateMaster_Id = model.StateMaster_Id;
                    existinglab.CityMaster_Id = model.CityMaster_Id;
                    existinglab.Location = model.Location;
                    existinglab.Location_Id = (int)model.Location_Id;
                    existinglab.PinCode = model.PinCode;
                    existinglab.AadharNumber = model.AadharNumber;
                    existinglab.PanImage = model.PanImage;
                    existinglab.LicenceImage = model.LicenceImage;
                    existinglab.LicenceNumber = model.LicenceNumber; 
                    existinglab.PAN = model.PAN; 
                    existinglab.StartTime = model.StartTime;
                    existinglab.EndTime = model.EndTime; 
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
            string q = @"select v.*,IsNull(ve.UniqueId,'N/A') as UniqueId, s.StateName,c.CityName,l.LocationName,IsNull(ve.VendorName,'NA') AS VendorName , IsNull(ve.CompanyName,'NA') as CompanyName from Lab v 
join StateMaster s on v.StateMaster_Id=s.Id
join CityMaster c on v.CityMaster_Id = c.Id
left join Location l on v.Location_Id=l.Id
left join Vendor ve on ve.Id = v.Vendor_Id
where v.IsDeleted=0  order by v.Id desc";
            var data = ent.Database.SqlQuery<LabDTO>(q).ToList();
            TempData["Id"] = data.FirstOrDefault().Id;
            if (vendorId != null)
                data = data.Where(a => a.Vendor_Id == vendorId).ToList();
            if (term != null)
                data = data.Where(a => a.LabName.Contains(term) || a.lABId.Contains(term)).ToList();
            return View(data);
        }

        public ActionResult UpdateStatus(int id = 0)
        {
            string q = @"update Lab set IsApproved = case when IsApproved=1 then 0 else 1 end where id="+id;
            ent.Database.ExecuteSqlCommand(q);
            string mobile = ent.Database.SqlQuery<string>("select MobileNumber from Lab  where Id=" + id).FirstOrDefault();
            string Email = ent.Database.SqlQuery<string>(@"select EmailId from Lab where Id=" + id).FirstOrDefault();
            string Name = ent.Database.SqlQuery<string>(@"select LabName from Lab where Id=" + id).FirstOrDefault();
            var msg = "Dear " + Name + ", Now you Can Login With Your Registered EmailId " + Email + " and Pasword";
            Message.SendSms(mobile, msg);
            return RedirectToAction("All");
        }
        public ActionResult UpdateBankUpdateStatus(int id)
        {
            string q = @"update Lab set IsBankUpdateApproved = case when IsBankUpdateApproved=1 then 0 else 1 end where id=" + id;
            ent.Database.ExecuteSqlCommand(q);

            string mobile = ent.Database.SqlQuery<string>("select MobileNumber from Lab where Id=" + id).FirstOrDefault();
            string Email = ent.Database.SqlQuery<string>(@"select EmailId from Lab where Id=" + id).FirstOrDefault();
            string Name = ent.Database.SqlQuery<string>(@"select LabName from Lab where Id=" + id).FirstOrDefault();
            //var msg = "Dear " + Name + ", Now you Can Upadate your bank details.";
            //Message.SendSms(mobile, msg);
            var query = "SELECT IsBankUpdateApproved FROM Lab WHERE Id = @Id";
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
            var data = ent.Labs.Find(id);
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

        [Authorize (Roles ="Admin, Lab")]
        public ActionResult AddTestDetails(int id = 0)
        {
            var data = new AddTestByLabDTO();
            data.Tests = new SelectList(repos.GetTests(), "Id", "TestName");
            if(id != 0)
            {
                data.Lab_Id = id;
            }
            return View(data);
        }

        [HttpPost]
        public ActionResult AddTestDetails(AddTestByLabDTO model)
        {
            var data = new TestLab();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var record = ent.LabTests.Find(model.Test_Id);
            var records = ent.LabTests.Any(a => a.TestName == model.TestName);
            if (records == true)
            {
                TempData["msg"] = "This Test Name Already Exists";
                return RedirectToAction("AddTestDetails", new { id = model.Id });
            }
            var recordCenter = ent.LabTests.Any(a => a.TestName == model.TestName && a.Lab_Id == model.Lab_Id);
            if (recordCenter == true)
            {
                TempData["msg"] = "You've Already Registered this Package";
                return RedirectToAction("AddTestDetails", new { id = model.Id });
            }
            try
            {
                if (model.Test_Id == null)
                {
                    var data1 = new LabTest
                    {
                        TestAmount = model.TestAmount,
                        TestName = model.TestName,
                        TestDesc = model.TestDescription,
                        Lab_Id = model.Lab_Id,
                    };
                    ent.LabTests.Add(data1);
                    ent.SaveChanges();
                    model.Test_Id = data1.Id;

                    var data2 = new TestLab
                    {
                        Lab_Id = (int)model.Lab_Id,
                        Test_Id = model.Test_Id,
                        TestDescription = model.TestDescription,
                        TestAmount = model.TestAmount,
                    };
                    ent.TestLabs.Add(data2);
                    ent.SaveChanges();
                }
                else
                {
                    var data2 = new TestLab
                    {
                        Lab_Id = (int)model.Lab_Id,
                        Test_Id = model.Test_Id,
                        TestDescription = model.TestDescription,
                        TestAmount = model.TestAmount,
                    };
                    ent.TestLabs.Add(data2);
                    ent.SaveChanges();
                }
              
                TempData["msg"] = "Successfully Added";
                return RedirectToAction("AddTestDetails", new {id=model.Id } );
            }
            catch(Exception ex)
            {
                TempData["msg"] = "Server Error";
                return RedirectToAction("AddTestDetails", new { id = model.Id });
            }
            
        }

        public ActionResult ShowTest(int id, string term)
        {
            var model = new TestListByLab();
            var q = @"select TestLab.*, LabTest.TestName, LabTest.Lab_Id as LabId from TestLab join LabTest on TestLab.Test_Id = LabTest.Id where TestLab.Lab_Id=" + id;
            var data = ent.Database.SqlQuery<TestsList>(q).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                data = data.Where(a => a.TestName.Contains(term)).ToList();
            }
            model.labId = id;
            TempData["Lab"] = model.labId;
            model.TestsList = data;
            return View(model);
        }

        public ActionResult EditTest(int testid)
        {
            var data = ent.TestLabs.Find(testid);
            var model = Mapper.Map<AddTestByLabDTO>(data);
            model.Tests = new SelectList(repos.GetTests(), "Id", "TestName", model.Test_Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditTest(AddTestByLabDTO model)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    model.Tests = new SelectList(repos.GetTests(), "Id", "TestName", model.Test_Id);
                    return View(model);
                }

                ModelState.Remove("Lab_Id");
                var domainModel = Mapper.Map<TestLab>(model);
                ent.Entry(domainModel).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();

                string q = @"update LabTest set TestAmount=" + model.TestAmount + ",TestDesc='" + model.TestDescription + "' where Id=" + model.Test_Id;
                ent.Database.ExecuteSqlCommand(q);

                return RedirectToAction("ShowTest", new { id = model.Lab_Id });

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                TempData["msg"] = "Server Error";
                return RedirectToAction("ShowTest", new { id = model.Lab_Id });
            }
        }


        public ActionResult DeleteTest(int Delid)
        {
            var id = TempData["Id"];
            var lab =TempData["Lab"];
            var data = ent.TestLabs.Find(Delid);
            try
            {
                ent.TestLabs.Remove(data);
                ent.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return RedirectToAction("ShowTest","Lab", new { id = lab});

        }

        public ActionResult LabTestHistory(int id, DateTime? date, int? pageNumber)
         {
            var model = new BookedTests();
            var query = @"select BookTestLab.PatientName, BookTestLab.PatientAddress, BookTestLab.ContactNumber, TestLab.TestDescription as TestName, BookTestLab.TestDate, TestLab.TestAmount, BookTestLab.IsTaken, Lab.Id as LabId, BookTestLab.Id,{ fn concat(CONVERT(varchar(15),CAST(AvailabelTime1 AS TIME),100),{fn concat ('-', CONVERT(varchar(15),CAST(AvailableTime2 AS TIME),100))})} AS AvailableTime 
from BookTestLab join Lab on BookTestLab.Lab_Id = Lab.Id 
join LabTest on BookTestLab.Test_Id = LabTest.Id 
join TestLab on TestLab.Test_Id = BookTestLab.Test_Id 
where TestLab.Lab_Id= " + id+" group by BookTestLab.TestDate, BookTestLab.PatientName, BookTestLab.PatientAddress, BookTestLab.ContactNumber, TestLab.TestDescription, TestLab.TestAmount, BookTestLab.IsTaken, Lab.Id, BookTestLab.Id, BookTestLab.AvailabelTime1, BookTestLab.AvailableTime2 order by BookTestLab.TestDate desc";
            var data = ent.Database.SqlQuery<TestList>(query).ToList();
            if(date != null)
            {
                data = data.Where(a => a.TestDate == date).ToList();
            }
            int total = data.Count;
            pageNumber = (int?)pageNumber ?? 1;
            int pageSize = 10;
            decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
            model.TotalPages = (int)noOfPages;
            model.PageNumber = (int)pageNumber;
            data = data.OrderBy(a => a.Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
            model.TestList = data;
            return View(model);
        }



        public ActionResult UpdateTestStatus(int id)
        {
            var lab = ent.BookTestLabs.Find(id);
            string q = @"update BookTestLab set IsTaken = case when IsTaken=0 then 1 else 0 end where Id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("LabTestHistory", "Lab", new { id = lab.Lab_Id });
        }


        public ActionResult LabPatientList(int id, string term, int? pageNumber)
        {
            var mdoel = new PatientDTO();
            var q = @"select p.Patient_Id, patient.PatientName, patient.EmailId, patient.MobileNumber, patient.Location from BookTestLab p JOIN patient on patient.Id = P.Patient_Id where Lab_Id='"+id+"' group by p.Patient_Id,patient.PatientName,patient.EmailId, patient.MobileNumber, patient.Location";
            var data = ent.Database.SqlQuery<PatientList>(q).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                data = data.Where(a => a.PatientName.ToLower().Contains(term)).ToList();
            }
            int total = data.Count;
            pageNumber = (int?)pageNumber ?? 1;
            int pageSize = 10;
            decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
            mdoel.TotalPages = (int)noOfPages;
            mdoel.PageNumber = (int)pageNumber;
            data = data.OrderBy(a => a.Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
            mdoel.Patient = data;
            return View(mdoel);
        }


        [HttpPost]
        public JsonResult GetPatientName(string term)
        {
            var PatientList = (from N in ent.Patients
                               where N.PatientRegNo.StartsWith(term)
                               select new { N.PatientRegNo, N.Id });
            TempData["PatientId"] = PatientList.FirstOrDefault().Id;
            TempData["PatientRegNo"] = PatientList.FirstOrDefault().PatientRegNo;
            return Json(PatientList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadReport(int id)
        {
            var model = new LabReportDTO();
            var Id = ent.Labs.Where(a => a.Id == id).ToList();
            TempData["LabId"] = Id.FirstOrDefault().Id;
            model.TestName = new SelectList(repos.GetTests(), "Id", "TestName");
            return View(model);
        }

        [HttpPost]
        public ActionResult UploadLabReport(LabReportDTO model)
        {
            var list = ent.Patients.Where(a => a.PatientRegNo == model.PatientRegNo).ToList();
            var PatientId = list.FirstOrDefault().Id;
            var LabId = TempData["LabId"];
            string[] allowedExts = { ".jpg", ".png", ".pdf" };
            if (model.File != null)
            {
                if (!FileOperation.CheckAllowedFiles(model.File, allowedExts))
                {
                    TempData["msg"] = "Invalid Extensions";
                    return RedirectToAction("UploadReport", new { id = LabId });
                }
                foreach (HttpPostedFileBase file in model.File)
                {
                    var reportName = FileOperation.UploadFile(file, "LabReports", allowedExts);
                    var domain = Mapper.Map<LabReport>(model);
                    domain.Lab_Id = Convert.ToInt32(LabId);
                    domain.File = reportName;
                    domain.Patient_Id = PatientId;
                    ent.LabReports.Add(domain);
                }
                ent.SaveChanges();
                TempData["msg"] = "Uploaded Successfully";
                return RedirectToAction("UploadReport", new { id = LabId });
            }
            return RedirectToAction("UploadReport", new { id = LabId });
        }

        [AllowAnonymous]
        public ActionResult ViewUploadReports(int id, string term, int? pageNumber)
        {
            var model = new HealthCheckupPatientList();
            var q = @"select P.PatientName, P.Id, P.PatientRegNo from LabReport C join Patient P on P.Id = C.Patient_Id WHERE C.Lab_Id= " + id+ " GROUP BY  P.PatientName,P.Id, P.PatientRegNo";
            var data = ent.Database.SqlQuery<PatientTestListVM>(q).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                data = data.Where(a => a.PatientRegNo.ToLower().Contains(term)).ToList();
            }
            var total = data.Count();
            pageNumber = (int?)pageNumber ?? 1;
            int pageSize = 10;
            decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
            model.TotalPages = (int)noOfPages;
            model.PageNumber = (int)pageNumber;
            data = data.OrderBy(a => a.Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
            model.PatientTestList = data;
            return View(model);
        }
        public ActionResult TestList(int id)
        {
            var model = new HealthCheckupPatientList();
            var q = @"select * from LabReport C join Patient p on c.Patient_Id = p.Id join LabTest lt on lt.Id = C.Test join CityMaster city on city.Id = p.CityMaster_Id join StateMaster statemaster on statemaster.Id = p.StateMaster_Id where  C.Patient_Id=" + id;
            var data = ent.Database.SqlQuery<LabTestReport>(q).ToList();
            model.LabTestReport = data;
            return View(model);
        }

        public ActionResult PayoutReport(int id)
        {
            var model = new ViewPayOutHistory();
            var Name = ent.Database.SqlQuery<string>("select LabName from Lab where Id=" + id).FirstOrDefault();
            model.LabName = Name;
            string qry = @"select Dp.Id, ISNULL(Dp.IsPaid, 0) as IsPaid , Dp.IsGenerated, Dp.Lab_Id, Dp.PaymentDate, Dp.Amount, D.LabName from  LabPayout Dp join Lab D on D.Id = Dp.Lab_Id  where  Dp.Lab_Id=" + id;
            var data = ent.Database.SqlQuery<HistoryOfLab_Payout>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            else
            {
                model.HistoryOfLab_Payout = data;
            }
            return View(model);
        }

        public ActionResult PaymentReport(int id, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDTO();
            var la = @"select * from Lab join CityMaster on CityMaster.Id = Lab.CityMaster_Id join StateMaster on StateMaster.Id = Lab.StateMaster_Id where Lab.Id=" + id;
            var mek = ent.Database.SqlQuery<ReportDTO>(la).ToList();
            model.LabName = mek.FirstOrDefault().LabName;
            model.LicenceNumber = mek.FirstOrDefault().LicenceNumber;
            model.MobileNumber = mek.FirstOrDefault().MobileNumber;
            model.StateName = mek.FirstOrDefault().StateName;
            model.StateName = mek.FirstOrDefault().CityName;
            model.Location = mek.FirstOrDefault().Location;
            if (sdate != null && edate != null)
            {
                var labs = @"select CONVERT(VARCHAR(10), P.TestDate, 111) as TestDate1, P.TestDate, Sum(P.Amount) as Amount, D.LabName,D.Location, D.MobileNumber,D.LicenceNumber,S.StateName, C.CityName from BookTestLab P join Lab D on D.Id = p.Center_Id join CityMaster C on C.Id = D.CityMaster_Id join StateMaster S on S.Id = D.StateMaster_Id WHERE p.Lab_Id='" + id + "' and P.TestDate between '" + sdate + "' and '" + edate + "' GROUP BY P.TestDate, P.Amount, D.LabName, D.MobileNumber, S.StateName,C.CityName, D.LicenceNumber,D.Location ";
                var lab = ent.Database.SqlQuery<ViewLabDetails>(labs).ToList();
                //doctorList = doctorList.Where(a => a.AppointmentDate >= sdate && a.AppointmentDate <= edate).ToList();
                if (lab.Count() == 0)
                {
                    TempData["MSG"] = "No Records";
                }
                else
                {
                    model.ViewLabDetails = lab;
                    ViewBag.Total = model.ViewLabDetails.Sum(a => a.Amount);
                    return View(model);
                }
            }
            else
            {
                var doctor = @"select CONVERT(VARCHAR(10), P.TestDate, 111) as TestDate1, P.TestDate, Sum(P.Amount) as Amount, D.LabName,D.Location, D.MobileNumber,D.LicenceNumber,S.StateName, C.CityName from BookTestLab P join Lab D on D.Id = p.Lab_Id join CityMaster C on C.Id = D.CityMaster_Id join StateMaster S on S.Id = D.StateMaster_Id WHERE p.Lab_Id=" + id + " and  datepart(mm,P.TestDate) =month(getdate()) GROUP BY P.TestDate, P.Amount, D.LabName, D.MobileNumber, S.StateName,C.CityName, D.LicenceNumber,D.Location";
                var labList = ent.Database.SqlQuery<ViewLabDetails>(doctor).ToList();
                if (labList.Count() == 0)
                {
                    TempData["MSG"] = "No Records";
                }
                else
                {
                    model.ViewLabDetails = labList;
                    ViewBag.Total = model.ViewLabDetails.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

    }
}