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
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HospitalPortal.Controllers
{
    [Authorize]
    public class CompletHealthCheckupController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(CompletHealthCheckupController));
        GenerateBookingId bk = new GenerateBookingId();

        [AllowAnonymous]
        public ActionResult Add(int vendorId = 0)
        {
            var model = new HealthCheckupCenterDTO();
            model.Vendor_Id = vendorId;
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Add(HealthCheckupCenterDTO model)
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
                    //if (ent.AdminLogins.Any(a => a.Username == model.EmailId))
                    //{
                    //    TempData["msg"] = "This Email has already exists.";
                    //    return RedirectToAction("Add");
                    //}
                    //if (ent.AdminLogins.Any(a => a.PhoneNumber == model.MobileNumber))
                    //{
                    //    TempData["msg"] = "This Mobile Number has already exists.";
                    //    return RedirectToAction("Add");
                    //}
                    if (ent.HealthCheckupCenters.Any(a => a.LabName == model.LabName && a.MobileNumber == model.MobileNumber))
                    {
                        var data = ent.HealthCheckupCenters.Where(a => a.LabName == model.LabName && a.MobileNumber == model.MobileNumber).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.HealthCheckUpId).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        TempData["msg"] = "you are already registered with pswellness";
                        return RedirectToAction("Add", "CompletHealthCheckup");
                    }

                    var admin = new AdminLogin
                    {
                        Username = model.EmailId,
                        PhoneNumber = model.MobileNumber,
                        Password = model.Password,
                        Role = "checkup"
                    };
                    ent.AdminLogins.Add(admin);
                    ent.SaveChanges();

                    // licence Picture Section 
                    if (model.LicenceImageFile == null)
                    {
                        TempData["msg"] = "Certificate Image File Picture can not be null";
                        tran.Rollback();
                        return View(model);
                    }
                    var img = FileOperation.UploadImage(model.LicenceImageFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg files are allowed.";
                        tran.Rollback();
                        return View(model);
                    }
                    model.LicenceImage = img;

                    // aadhar image upload

                    if (model.AadharImageFile != null)
                    {
                        var aadharImg = FileOperation.UploadImage(model.AadharImageFile, "Images");
                        if (aadharImg == "not allowed")
                        {
                            TempData["msg"] = "Only png,jpg,jpeg files are allowed as Aadhar card document";
                            tran.Rollback();
                            return View(model);
                        }
                        model.AadharImage = aadharImg;
                    }

                    if (model.RegImageFile != null)
                    {
                        var regImg = FileOperation.UploadImage(model.RegImageFile, "Images");
                        if (regImg == "not allowed")
                        {
                            TempData["msg"] = "Only png,jpg,jpeg files are allowed as Aadhar card document";
                            tran.Rollback();
                            return View(model);
                        }
                        model.RegImage = regImg;
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
                    var domainModel = Mapper.Map<HealthCheckupCenter>(model);
                    domainModel.AdminLogin_Id = admin.Id;
                    domainModel.JoiningDate = DateTime.Now;
                    domainModel.HealthCheckUpId = bk.GenerateHealthCenterId();
                    admin.UserID = domainModel.HealthCheckUpId;
                    ent.HealthCheckupCenters.Add(domainModel);
                    ent.SaveChanges();
                    string msg = "Welcome to PSWELLNESS. Your User Name :  " + domainModel.EmailId + "(" + domainModel.HealthCheckUpId + "), Password : " + admin.Password + ".";
                    Message.SendSms(domainModel.MobileNumber, msg);
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
            var data = ent.HealthCheckupCenters.Find(id);
            var model = Mapper.Map<HealthCheckupCenterDTO>(data);
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName", model.StateMaster_Id);
            model.Cities = new SelectList(repos.GetCitiesByState(model.StateMaster_Id), "Id", "CityName", model.CityMaster_Id);
            model.Locations = new SelectList(repos.GetLocationByCity(model.CityMaster_Id), "Id", "LocationName", model.Location_Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(HealthCheckupCenterDTO model)
        {
            try
            {
                ModelState.Remove("MobileNumber");
                ModelState.Remove("EmailId");
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
                ModelState.Remove("LicenceImageFile");
                ModelState.Remove("AadharImageFile");
                ModelState.Remove("RegImageFile");
                model.States = new SelectList(repos.GetAllStates(), "Id", "StateName", model.StateMaster_Id);
                model.Cities = new SelectList(repos.GetCitiesByState(model.StateMaster_Id), "Id", "CityName", model.CityMaster_Id);
                model.Locations = new SelectList(repos.GetLocationByCity(model.CityMaster_Id), "Id", "LocationName", model.Location_Id);

                if (!ModelState.IsValid)
                    return View(model);
                // licence Picture Section 
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
                // aadhar image upload

                if (model.AadharImageFile != null)
                {
                    var aadharImg = FileOperation.UploadImage(model.AadharImageFile, "Images");
                    if (aadharImg == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg files are allowed as Aadhar card document";
                        return View(model);
                    }
                    model.AadharImage = aadharImg;
                }

                if (model.RegImageFile != null)
                {
                    var regImg = FileOperation.UploadImage(model.RegImageFile, "Images");
                    if (regImg == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg files are allowed as Aadhar card document";
                        return View(model);
                    }
                    model.RegImage = regImg;
                }
                var domainModel = Mapper.Map<HealthCheckupCenter>(model);
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

        public ActionResult All(int? vendorId)
        {
            string q = @"select v.*, IsNull(ve.UniqueId,'N/A') as UniqueId,s.StateName,l.LocationName,c.CityName, IsNull(ve.VendorName,'NA') AS VendorName , IsNull(ve.CompanyName,'NA') as CompanyName from HealthCheckupCenter v 
join StateMaster s on v.StateMaster_Id=s.Id
join CityMaster c on v.CityMaster_Id = c.Id
left join Location l on v.Location_Id=l.Id
left join Vendor ve on ve.Id = v.Vendor_Id
where v.IsDeleted=0 order by v.Id desc";
            var data = ent.Database.SqlQuery<HealthCheckupCenterDTO>(q).ToList();
            if (vendorId != null)
                data = data.Where(a => a.Vendor_Id == vendorId).ToList();
            return View(data);
        }

        public ActionResult UpdateStatus(int id)
        {
            string q = @"update HealthCheckupCenter set IsApproved = case when IsApproved=1 then 0 else 1 end where id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            string mobile = ent.Database.SqlQuery<string>("select MobileNumber from HealthCheckupCenter where Id=" + id).FirstOrDefault();
            string Email = ent.Database.SqlQuery<string>(@"select EmailId from HealthCheckupCenter where Id=" + id).FirstOrDefault();
            string Name = ent.Database.SqlQuery<string>(@"select LabName from HealthCheckupCenter where Id=" + id).FirstOrDefault();
            var msg = "Dear " + Name + ", Now you Can Login With Your Registered EmailId " + Email + " and Pasword";
            Message.SendSms(mobile, msg);
            return RedirectToAction("All");
        }

        public ActionResult Delete(int id)
        {
            var data = ent.HealthCheckupCenters.Find(id);
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


        public ActionResult AddDepartment(int hospitalId)
        {
            var model = new CheckupCenterDepartmentDTO();
            model.Hospital_Id = hospitalId;
            var depts = ent.Database.SqlQuery<Department>(@"select * from department  where isDeleted=0 and id not in(select Department_Id from ChekcupCenterDepartment where ChekcupCenterDepartment.Hospital_Id=" + model.Hospital_Id + ")").ToList();
            model.Depts = new SelectList(depts, "Id", "DepartmentName");
            return View(model);
        }

        [HttpPost]
        public ActionResult AddDepartment(CheckupCenterDepartmentDTO model)
        {
            var depts = ent.Database.SqlQuery<Department>(@"select * from department  where isDeleted=0 and id not in(select Department_Id from ChekcupCenterDepartment where ChekcupCenterDepartment.Hospital_Id=" + model.Hospital_Id + ")").ToList();

            model.Depts = new SelectList(depts, "Id", "DepartmentName");
            if (!ModelState.IsValid)
                return View(model);
            try
            {

                var dept = Mapper.Map<ChekcupCenterDepartment>(model);
                ent.ChekcupCenterDepartments.Add(dept);
                ent.SaveChanges();
                TempData["msg"] = "ok";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server Exception";
                log.Error(ex.Message);
            }
            return RedirectToAction("AddDepartment", new { hospitalId = model.Hospital_Id });
        }

        public ActionResult Departments(int hospitalId)
        {
            var depts = (from hDept in ent.ChekcupCenterDepartments
                         join dep in ent.Departments
                         on hDept.Department_Id equals dep.Id
                         where hDept.Hospital_Id == hospitalId && !hDept.IsDeleted
                         select new CheckupCenterDepartmentDTO
                         {
                             Id = hDept.Id,
                             IsDeleted = hDept.IsDeleted,
                             Department_Id = hDept.Department_Id,
                             Hospital_Id = hDept.Hospital_Id,
                             DeptName = dep.DepartmentName
                         }

                ).ToList();
            return View(depts);
        }

        public ActionResult RemoveDept(int hospitalId)
        {
            try
            {
                var dept = ent.ChekcupCenterDepartments.Find(hospitalId);
                ent.ChekcupCenterDepartments.Remove(dept);
                ent.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return RedirectToAction("Departments", new { hospitalId = hospitalId });
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

        [HttpPost]
        public JsonResult GetTest(string term)
        {
            var TestList = (from N in ent.LabTests
                               where N.TestName.StartsWith(term)
                               select new { N.TestName, N.Id });
            return Json(TestList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult UploadReport(int id)
        {
            var Id = ent.HealthCheckupCenters.Where(a => a.Id == id).ToList();
            TempData["CheckUpId"] = Id.FirstOrDefault().Id;
            return View();
        }

        [HttpPost]
        public ActionResult UploadPatientReport(CheckUpReportDTO model)
        {
            var list = ent.Patients.Where(a => a.PatientRegNo == model.PatientRegNo).ToList();
            var PatientId = list.FirstOrDefault().Id;
            var CheckUpId = TempData["CheckUpId"];
            string[] allowedExts = { ".jpg", ".png", ".pdf" };
            try
            {
                if (model.UploadHealthReport != null)
                {
                    if (!FileOperation.CheckAllowedFiles(model.UploadHealthReport.FirstOrDefault().File, allowedExts))
                    {
                        TempData["msg"] = "Invalid Extensions";
                        return RedirectToAction("UploadReport", new { id = CheckUpId });
                    }
                    foreach (HttpPostedFileBase file in model.UploadHealthReport.FirstOrDefault().File)
                    {
                        var reportName = FileOperation.UploadFile(file, "LabReports", allowedExts);
                        var domain = Mapper.Map<CheckUpReport>(model);
                        domain.Checkup_Center_Id = Convert.ToInt32(CheckUpId);
                        domain.File = reportName;
                        domain.Patient_Id = PatientId;
                        domain.TestId = model.UploadHealthReport.FirstOrDefault().Id;
                        ent.CheckUpReports.Add(domain);
                    }
                    ent.SaveChanges();
                    TempData["msg"] = "Uploaded Successfully";
                    return RedirectToAction("UploadReport", new { id = CheckUpId });
                }
            }
            catch(Exception ex)
            {
                string msg = ex.ToString();
                     TempData["msg"] = "Something Went Wrong";
                return RedirectToAction("UploadReport", new { id = CheckUpId });
            }
            return RedirectToAction("UploadReport", new { id = CheckUpId });
        }

        [AllowAnonymous]
        public ActionResult ViewPatientList(int id, string term, int? pageNumber)
        {
            var model = new HealthCheckupPatientList();
            var q = @"select C.Id, C.Patient_Id, P.PatientName from CheckUpReport C join Patient P on P.Id = C.Patient_Id WHERE C.Id= '" + id + "'GROUP BY C.Patient_Id, P.PatientName, C.Id";
            var data = ent.Database.SqlQuery<PatientTestListVM>(q).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                data = data.Where(a => a.PatientName.ToLower().Contains(term)).ToList();
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
            var q = @"select * from CheckUpReport C join Patient p on c.Patient_Id = p.Id join CityMaster city on city.Id = p.CityMaster_Id join StateMaster statemaster on statemaster.Id = p.StateMaster_Id where  C.Patient_Id=" + id;
            var data = ent.Database.SqlQuery<TestListByHealthCheckUp>(q).ToList();
            model.testlistbyhealthcheckup = data;
            return View(model);
        }


        public ActionResult ViewPackage(int id = 0)
        {
            var model = new CompleteCheckup();
            string qry = @"select * from HealthCheckUp join HealthCheckUpPackage on HealthCheckUp.PackageId = HealthCheckUpPackage.Id where HealthCheckUp.Center_Id=" + id;
            var data = ent.Database.SqlQuery<ShowDesc>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record";
                return View(model);
            }
            model.showDesc = data;
            //model.Center_Id = data.FirstOrDefault().Id;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddDescription(CompleteCheckup model)
        {
            if (!ModelState.IsValid)
            {
                TempData["msg"] = string.Join(" | ", ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage));
                return View(model);
            }
            else
            {
                try
                {
                    var data = Mapper.Map<HealthCheckUp>(model);
                    ent.HealthCheckUps.Add(data);
                    ent.SaveChanges();
                    return RedirectToAction("AddDescription", new { Center_Id = data.Center_Id });
                }
                catch (Exception ex)
                {
                    TempData["msg"] = ex;
                    return View(model);
                }
            }
        }

        public ActionResult DeleteDesc(int HealthId)
        {
            var data = ent.HealthCheckUps.Find(HealthId);
            try
            {
                ent.HealthCheckUps.Remove(data);
                ent.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return RedirectToAction("AddDescription", new { Center_Id = data.Center_Id });
        }

        public ActionResult ViewAppointmentList(int id, DateTime? date)
        {
            var model = new ViewHealthCheckup();
            string qry = @"select * from CmpltCheckup where IsPaid=1 and Center_Id="+id;
            var data = ent.Database.SqlQuery<ListHealthApp>(qry).ToList();
            if (date != null)
            {
                data = data.Where(a => a.TestDate == date).ToList();
            }
            if(data.Count() == 0)
            {
                TempData["msg"] = "No Records Found";
            }
            model.HealthList = data;
            return View(model);
        }

        public ActionResult UpdateTestStatus(int HealthId)
        {
            var health = ent.CmpltCheckUps.Find(HealthId);
            string q = @"update CmpltCheckup set IsTaken = case when IsTaken=0 then 1 else 0 end where Id=" + HealthId;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("ViewAppointmentList", "CompletHealthCheckup", new { id = health.Center_Id });
        }

        public ActionResult PayoutReport(int id)
        {
            var model = new ViewPayOutHistory();
            var Name = ent.Database.SqlQuery<string>("select LabName from HealthCheckupCenter where Id=" + id).FirstOrDefault();
            model.LabName = Name;
            string qry = @"select Dp.Id, ISNULL(Dp.IsPaid, 0) as IsPaid , Dp.IsGenerated, Dp.Health_Id, Dp.PaymentDate, Dp.Amount, D.LabName from  HealthPayOut Dp join HealthCheckupCenter D on D.Id = Dp.Health_Id  where  Dp.Health_Id=" + id;
            var data = ent.Database.SqlQuery<HistoryOfHealth_Payout>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            else
            {
                model.HistoryOfHealth_Payout = data;
            }
            return View(model);
        }

        public ActionResult PaymentReport(int id, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDTO();
            var la = @"select * from HealthCheckupCenter join CityMaster on CityMaster.Id = HealthCheckupCenter.CityMaster_Id join StateMaster on StateMaster.Id = HealthCheckupCenter.StateMaster_Id where HealthCheckupCenter.Id=" + id;
            var mek = ent.Database.SqlQuery<ReportDTO>(la).ToList();
            model.LabName = mek.FirstOrDefault().LabName;
            model.LicenceNumber = mek.FirstOrDefault().LicenceNumber;
            model.MobileNumber = mek.FirstOrDefault().MobileNumber;
            model.StateName = mek.FirstOrDefault().StateName;
            model.StateName = mek.FirstOrDefault().CityName;
            model.Location = mek.FirstOrDefault().Location;
            if (sdate != null && edate != null)
            {
                var labs = @"select CONVERT(VARCHAR(10), P.TestDate, 111) as TestDate1, P.TestDate, Sum(P.Amount) as Amount, D.LabName,D.Location, D.MobileNumber,D.LicenceNumber,S.StateName, C.CityName from CmpltCheckUp P join HealthCheckupCenter D on D.Id = p.Center_Id join CityMaster C on C.Id = D.CityMaster_Id join StateMaster S on S.Id = D.StateMaster_Id WHERE p.Center_Id='" + id + "' and P.TestDate between '" + sdate + "' and '" + edate + "' GROUP BY P.TestDate, P.Amount, D.LabName, D.MobileNumber, S.StateName,C.CityName, D.LicenceNumber,D.Location ";
                var lab = ent.Database.SqlQuery<HealthCheckListVM>(labs).ToList();
                //doctorList = doctorList.Where(a => a.AppointmentDate >= sdate && a.AppointmentDate <= edate).ToList();
                if (lab.Count() == 0)
                {
                    TempData["MSG"] = "No Records";
                }
                else
                {
                    model.ViewCmplteCheckUp = lab;
                    ViewBag.Total = model.ViewCmplteCheckUp.Sum(a => a.Amount);
                    return View(model);
                }
            }
            else
            {
                var doctor = @"select CONVERT(VARCHAR(10), P.TestDate, 111) as TestDate1, P.TestDate, Sum(P.Amount) as Amount, D.LabName,D.Location, D.MobileNumber,D.LicenceNumber,S.StateName, C.CityName from CmpltCheckUp P join HealthCheckupCenter D on D.Id = p.Center_Id join CityMaster C on C.Id = D.CityMaster_Id join StateMaster S on S.Id = D.StateMaster_Id WHERE p.Center_Id="+id+" and  datepart(mm,P.TestDate) =month(getdate()) GROUP BY P.TestDate, P.Amount, D.LabName, D.MobileNumber, S.StateName,C.CityName, D.LicenceNumber,D.Location";
                var labList = ent.Database.SqlQuery<HealthCheckListVM>(doctor).ToList();
                if (labList.Count() == 0)
                {
                    TempData["MSG"] = "No Records";
                }
                else
                {
                    model.ViewCmplteCheckUp = labList;
                    ViewBag.Total = model.ViewCmplteCheckUp.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult AddPackage(int Center_Id = 0)
        {
            var model = new HealthCheckUpPackageDTO();
            model.PackageList = new SelectList(ent.HealthPackageMasters.ToList(), "Id", "PackageName");
            if (Center_Id != 0)
            {
                model.CenterId = Center_Id;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult AddPackage(HealthCheckUpPackageDTO model)
        {
            //var record = ent.HealthCheckUpPackages.Any(a => a.PackageName == model.PackageName);
            //if (record == true)
            //{
            //    TempData["msg"] = "This Package Name Already Exists";
            //    return RedirectToAction("AddPackage");
            //}
            //var recordCenter = ent.HealthCheckUps.Any(a => a.PackageId == model.PackageId && a.Center_Id == model.CenterId);
            //if (recordCenter == true)
            //{
            //    TempData["msg"] = "You've Already Registered this Package";
            //    return View(model);
            //}

            if(model.packageId == null)
            {
                var data1 = new HealthCheckUpPackage
                {
                    //PackageName = model.PackageName,
                    //CenterId = (int)model.CenterId,
                    //TestAmt = model.TestAmt,
                };
                ent.HealthCheckUpPackages.Add(data1);
                ent.SaveChanges();
                model.packageId = data1.Id;
            }

            var data = new HealthCheckUp
            {
                Center_Id = (int)model.CenterId,
                IsDeleted = false,
                //Name = model.TestName,
                //TestDesc = model.TestDesc,
                //TestAmount = model.TestAmt,
                PackageId = model.packageId
            };
            ent.HealthCheckUps.Add(data);
            ent.SaveChanges();

            //var domain = Mapper.Map<HealthCheckUpPackage>(model);
            //ent.HealthCheckUpPackages.Add(domain);
            //ent.SaveChanges();
            TempData["msg"] = "Successfully Added";
            return RedirectToAction("AddPackage", new { /*id = model.CenterId*/ });
        }
     
    }
}