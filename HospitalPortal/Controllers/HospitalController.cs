using AutoMapper;
using HospitalPortal.BL;
using HospitalPortal.Models.APIModels;
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
using static HospitalPortal.Utilities.EmailOperations;

namespace HospitalPortal.Controllers
{
    [Authorize]
    public class HospitalController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(HospitalController));
        GenerateBookingId bk = new GenerateBookingId();


        [AllowAnonymous]
        public ActionResult Add(int vendorId = 0)
        {
            var model = new HospitalDTO();
            model.Vendor_Id = vendorId;
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Add(HospitalDTO model)
        {
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                    if(!string.IsNullOrEmpty(model.OtherCity))
                        ModelState.Remove("CityMaster_Id");
                    if (!string.IsNullOrEmpty(model.OtherLocation))
                        ModelState.Remove("Location_Id");
                    if (!ModelState.IsValid)
                        return View(model);
                    //if (ent.AdminLogins.Any(a => a.Username == model.EmailId))
                    //{
                    //    TempData["msg"] = "This EmailId has already exists.";
                    //    return RedirectToAction("Add");
                    //}
                    //if (ent.AdminLogins.Any(a => a.PhoneNumber == model.MobileNumber))
                    //{
                    //    TempData["msg"] = "This Mobile Number has already exists.";
                    //    return RedirectToAction("Add");
                    //}

                    if (ent.Hospitals.Any(a => a.HospitalName == model.HospitalName && a.PhoneNumber == model.PhoneNumber))
                    {
                        var data = ent.Hospitals.Where(a => a.HospitalName == model.HospitalName && a.PhoneNumber == model.PhoneNumber).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.HospitalId).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        TempData["msg"] = "you are already registered with pswellness";
                        return RedirectToAction("Add", "Hospital");
                    }

                    var admin = new AdminLogin
                    {
                        Username = model.EmailId,
                        PhoneNumber = model.MobileNumber,
                        Password = model.Password,
                        Role = "hospital"
                    };
                    ent.AdminLogins.Add(admin);
                    ent.SaveChanges();

                    // save other city and locations

                    if(!string.IsNullOrEmpty(model.OtherCity))
                    {
                        var cityMaster = new CityMaster
                        {
                            CityName = model.OtherCity,
                           StateMaster_Id=(int)model.StateMaster_Id
                        };
                        ent.CityMasters.Add(cityMaster);
                        ent.SaveChanges();
                        model.CityMaster_Id = cityMaster.Id;
                    }
                    if(!string.IsNullOrEmpty(model.OtherLocation))
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

                    // authorization letter  Section 
                    if (model.AuthorizationImageFile == null)
                    {
                        TempData["msg"] = "Authorization Image File Picture can not be null";
                        tran.Rollback();
                        return View(model);
                    }
                    var img = FileOperation.UploadImage(model.AuthorizationImageFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg files are allowed.";
                        tran.Rollback();
                        return View(model);
                    }
                    model.AuthorizationLetterImage = img;

                    var domainModel = Mapper.Map<Hospital>(model);
                    domainModel.AdminLogin_Id = admin.Id;
                    domainModel.HospitalId = bk.GenerateHospitalId();
                    admin.UserID = domainModel.HospitalId;
                    ent.Hospitals.Add(domainModel);
                    ent.SaveChanges();
                    //string msg = "Welcome to PSWELLNESS. Your User Name :  " + domainModel.EmailId + "(" + domainModel.HospitalId + "), Password : " + admin.Password + ".";
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
            var data = ent.Hospitals.Find(id);
            var model = Mapper.Map<HospitalDTO>(data);
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName", model.StateMaster_Id);
            model.Cities = new SelectList(repos.GetCitiesByState(model.StateMaster_Id), "Id", "CityName", model.CityMaster_Id);
            model.Locations = new SelectList(repos.GetLocationByCity(model.CityMaster_Id), "Id", "LocationName", model.Location_Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(HospitalDTO model)
        {
            try
            {

                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
                ModelState.Remove("MobileNumber");
                ModelState.Remove("EmailId");
                ModelState.Remove("AuthorizationImageFile");
                if (!string.IsNullOrEmpty(model.OtherCity))
                    ModelState.Remove("CityMaster_Id");
                if (!string.IsNullOrEmpty(model.OtherLocation))
                    ModelState.Remove("Location_Id");
                ModelState.Remove("IsCheckedTermsCondition");
                model.States = new SelectList(repos.GetAllStates(), "Id", "StateName", model.StateMaster_Id);
                model.Cities = new SelectList(repos.GetCitiesByState(model.StateMaster_Id), "Id", "CityName", model.CityMaster_Id);
                model.Locations = new SelectList(repos.GetLocationByCity(model.CityMaster_Id), "Id", "LocationName", model.Location_Id);
                if (!ModelState.IsValid)
                    return View(model);
                // authorization letter  Section 
                if (model.AuthorizationImageFile != null)
                {

                    var img = FileOperation.UploadImage(model.AuthorizationImageFile, "Images");
                    if (img == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg files are allowed.";
                        return View(model);
                    }
                    model.AuthorizationLetterImage = img;
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
                var domainModel = Mapper.Map<Hospital>(model);
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
            string q = @"select v.*,IsNull(ve.UniqueId,'N/A') as UniqueId, s.StateName,IsNull(l.LocationName,'NA') AS LocationName,c.CityName, IsNull(ve.VendorName,'NA') AS VendorName , IsNull(ve.CompanyName,'NA') as CompanyName from Hospital v 
join StateMaster s on v.StateMaster_Id=s.Id
join CityMaster c on v.CityMaster_Id = c.Id
left join Location l on v.Location_Id=l.Id
left join Vendor ve on ve.Id = v.Vendor_Id
where v.IsDeleted=0 order by v.Id desc";
            var data = ent.Database.SqlQuery<HospitalDTO>(q).ToList();
            if (vendorId != null)
                data = data.Where(a => a.Vendor_Id == vendorId).ToList();
            if (term != null)
                data = data.Where(A => A.HospitalName.ToLower().Contains(term) || A.HospitalId.Contains(term)).ToList();
            return View(data);
        }

        public ActionResult UpdateStatus(int id)
        {
            string q = @"update Hospital set IsApproved = case when IsApproved=1 then 0 else 1 end where id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            string mobile = ent.Database.SqlQuery<string>("select MobileNumber from Hospital where Id=" + id).FirstOrDefault();
            string Email = ent.Database.SqlQuery<string>(@"select EmailId from Hospital where Id=" + id).FirstOrDefault();
            string Name = ent.Database.SqlQuery<string>(@"select HospitalName from Hospital where Id=" + id).FirstOrDefault();
            var msg = "Dear " + Name + ", Now you Can Login With Your Registered EmailId " + Email + " and Pasword";
            Message.SendSms(mobile, msg);
            return RedirectToAction("All");
        }

        public ActionResult Delete(int id)
        {
            var data = ent.Hospitals.Find(id);
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

        public ActionResult AddDepartment(int hospitalId)
        {
            var model = new HospitalDepartmentDTO();
            model.Hospital_Id = hospitalId;
            var depts = ent.Database.SqlQuery<Department>(@"select * from department  where isDeleted=0 and id not in(select Department_Id from HospitalDepartment where HospitalDepartment.Hospital_Id=" + model.Hospital_Id + ")").ToList();
            model.Depts = new SelectList(depts, "Id", "DepartmentName");
            return View(model);
        }

        [HttpPost]
        public ActionResult AddDepartment(HospitalDepartmentDTO model)
        {
            var depts = ent.Database.SqlQuery<Department>(@"select * from department  where isDeleted=0 and id not in(select Department_Id from HospitalDepartment where HospitalDepartment.Hospital_Id=" + model.Hospital_Id + ")").ToList();
            model.Depts = new SelectList(depts, "Id", "DepartmentName");
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                if(ent.HospitalDepartments.Any(a=>a.Hospital_Id==model.Hospital_Id && a.Department_Id==model.Department_Id))
                {
                    TempData["msg"] = "This department has added already.";
                    return View(model);
                }
                var dept = Mapper.Map<HospitalDepartment>(model);
                ent.HospitalDepartments.Add(dept);
                ent.SaveChanges();
                TempData["msg"] = "ok";
            }
            catch(Exception ex)
            {
                TempData["msg"] = "Server Exception";
                log.Error(ex.Message);
            }
            return RedirectToAction("AddDepartment", new { hospitalId = model.Hospital_Id });
        }

        public ActionResult Departments(int hospitalId)
        {
            var depts = (from hDept in ent.HospitalDepartments
                         join dep in ent.Departments
                         on hDept.Department_Id equals dep.Id
                         where hDept.Hospital_Id==hospitalId && !hDept.IsDeleted
                         select new HospitalDepartmentDTO
                         {
                             Id=hDept.Id,
                             IsDeleted=hDept.IsDeleted,
                             Department_Id=hDept.Department_Id,
                             Hospital_Id=hDept.Hospital_Id,
                             DeptName=dep.DepartmentName
                         }
                ).ToList();
            return View(depts);
        }

        public ActionResult RemoveDept(int hospitalId)
        {
            try
            {
                var dept = ent.HospitalDepartments.Find(hospitalId);
                ent.HospitalDepartments.Remove(dept);
                ent.SaveChanges();
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
            }
            return RedirectToAction("Departments", new { hospitalId=hospitalId});
        }

        public ActionResult AddFacilty(int hospitalId)
        {
            var model = new HospitalFacilityDTO();
            model.Hospital_Id = hospitalId;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddFacilty(HospitalFacilityDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                var data = Mapper.Map<HopitalFaciltiy>(model);
                ent.HopitalFaciltiys.Add(data);
                ent.SaveChanges();
                TempData["msg"] = "ok";
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
                TempData["msg"] = "Server exception";
            }
            return RedirectToAction("AddFacilty", new { hospitalId = model.Hospital_Id });
        }

        public ActionResult Facilities(int hospitalId)
        {
            var data = ent.HopitalFaciltiys.Where(a=>!a.IsDeleted).ToList();
            return View(data);
        }

        public ActionResult RemoveFaciltiy(int hospitalId)
        {
            try
            {
                var data = ent.HopitalFaciltiys.Find(hospitalId);
                data.IsDeleted = true;
                ent.SaveChanges();
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
            }
            return RedirectToAction("Facilities", new { hospitalId = hospitalId });
        }

        public ActionResult AddDoctor(int hospitalId)
        {
            var model = new HospitalDoctorDTO();
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
            model.Hospital_Id = hospitalId;
            model.DepartmentList = new SelectList(ent.Departments.Where(a => !a.IsDeleted).ToList(), "Id", "DepartmentName");
            return View(model);
        }

        [HttpPost]
        public ActionResult AddDoctor(HospitalDoctorDTO model)
        {
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
           
                try
                {
                    if (!ModelState.IsValid)
                        return View(model);
                    // aadhar doc upload
                    if (model.AadharImageFile != null)
                    {
                        var aadharImg = FileOperation.UploadImage(model.AadharImageFile, "Images");
                        if (aadharImg == "not allowed")
                        {
                            TempData["msg"] = "Only png,jpg,jpeg files are allowed as Aadhar/PAN card document";
                            return View(model);
                        }
                        model.AadharImage = aadharImg;
                    }
                    // Licence upload
                    if (model.LicenceImageFile != null)
                    {
                        var licenceImg = FileOperation.UploadImage(model.LicenceImageFile, "Images");
                        if (licenceImg == "not allowed")
                        {
                            TempData["msg"] = "Only png,jpg,jpeg files are allowed as Licence document";
                            return View(model);
                        }
                        model.LicenceImage = licenceImg;
                    }
                    var domainModel = Mapper.Map<HospitalDoctor>(model);
                    domainModel.EndTime = model.EndTime;
                    domainModel.SlotTime = model.SlotTime;
                    domainModel.StartTime = model.StartTime;
                    ent.HospitalDoctors.Add(domainModel);
                    ent.SaveChanges();
                    TempData["msg"] = "ok";
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    TempData["msg"] = "Server Error";
                }
            return RedirectToAction("AddDoctor",new { hospitalId=model.Hospital_Id});
        }

        public ActionResult Doctors(int hospitalId)
        {
            string query = @"select hd.*,d.DepartmentName,s.SpecialistName,
cm.CityName,sm.StateName
 from HospitalDoctor hd
join Department d 
on hd.Department_Id=d.Id
join Specialist s
on hd.Specialist_Id=s.Id
join StateMaster sm
on hd.StateMaster_Id=sm.Id
join CityMaster cm on hd.CityMaster_Id=cm.Id
where hd.IsDeleted=0 and hd.Hospital_Id=" + hospitalId;
            var data = ent.Database.SqlQuery<HospitalDoctorDTO>(query).ToList();
            return View(data);
        }

        public ActionResult RemoveDoctor(int docId,int hospitalId)
        {
            try
            {
                var data = ent.HospitalDoctors.Find(docId);
                data.IsDeleted = true;
                ent.SaveChanges();
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
            }
            return RedirectToAction("Doctors", new { hospitalId = hospitalId });
        }

        public ActionResult EditDoctor(int docId)
        {
            var doc = ent.HospitalDoctors.Find(docId);
            var model = Mapper.Map<HospitalDoctorDTO>(doc);
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName", model.StateMaster_Id);
            model.Cities = new SelectList(repos.GetCitiesByState(model.StateMaster_Id), "Id", "CityName", model.CityMaster_Id);
            model.DepartmentList = new SelectList(ent.Departments.ToList(), "Id", "DepartmentName", model.Department_Id);
            model.SpecialistList = new SelectList(ent.Specialists.Where(a => a.Department_Id == model.Department_Id).ToList(), "Id", "SpecialistName", model.Specialist_Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditDoctor(HospitalDoctorDTO model)
        {
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName", model.StateMaster_Id);
            model.Cities = new SelectList(repos.GetCitiesByState(model.StateMaster_Id), "Id", "CityName", model.CityMaster_Id);
            model.DepartmentList = new SelectList(ent.Departments.ToList(), "Id", "DepartmentName", model.Department_Id);
            model.SpecialistList = new SelectList(ent.Specialists.Where(a => a.Department_Id == model.Department_Id).ToList(), "Id", "SpecialistName", model.Specialist_Id);
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                // aadhar doc upload
                if (model.AadharImageFile != null)
                {
                    var aadharImg = FileOperation.UploadImage(model.AadharImageFile, "Images");
                    if (aadharImg == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg files are allowed as Aadhar/PAN card document";
                        return View(model);
                    }
                    model.AadharImage = aadharImg;
                }
                // Licence upload
                if (model.LicenceImageFile != null)
                {
                    var licenceImg = FileOperation.UploadImage(model.LicenceImageFile, "Images");
                    if (licenceImg == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg files are allowed as Licence document";
                        return View(model);
                    }
                    model.LicenceImage = licenceImg;
                }
                var domainModel = Mapper.Map<HospitalDoctor>(model);
                domainModel.StartTime = model.StartTime;
                domainModel.EndTime = model.EndTime;
                domainModel.SlotTime = model.SlotTime;
                ent.Entry(domainModel).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                TempData["msg"] = "ok";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                TempData["msg"] = "Server Error";
            }
            return RedirectToAction("EditDoctor", new { docId = model.Id });
        }

        public ActionResult AddNurse(int hospitalId)
        {
            var model = new HospitalNurseDTO();
            model.NurseTypeList = new SelectList(ent.NurseTypes.ToList(), "Id", "   NurseTypeName", model.NurseType_Id);
            TempData["Id"] = hospitalId;
            model.Hospital_Id = hospitalId;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddNurse(HospitalNurseDTO model)
        {
            model.NurseTypeList = new SelectList(ent.NurseTypes.ToList(), "Id", "   NurseTypeName", model.NurseType_Id);
            try
            {
                var domain = Mapper.Map<HospitalNurse>(model);
                domain.Hospital_Id = Convert.ToInt32(TempData["Id"].ToString());
                domain.IsDeleted = false;
                ent.HospitalNurses.Add(domain);
                ent.SaveChanges();
                TempData["msg"] = "ok";
            }
            catch(Exception ex)
            {
                string msg = ex.ToString();
                TempData["msg"] = "Server Error!";
            }
            return RedirectToAction("AddNurse", new {hospitalId = TempData["Id"] });
        }

        public ActionResult ShowNurse(int hospitalId)
        {
            var model = new NurseListVM();
            string qry = @"select * from HospitalNurse hn join NurseType nt on nt.Id = hn.NurseType_Id  where hn.IsDeleted=0 and Hospital_Id="+hospitalId;
            var data = ent.Database.SqlQuery<NurseListof>(qry).ToList();
            model.NurseList = data;
            return View(model);
        }

        public ActionResult DeleteNurse(int id)
        {
            var data = ent.HospitalNurses.Find(id);
            data.IsDeleted = true;
            ent.SaveChanges();
            return RedirectToAction("ShowNurse", new { hospitalId= data.Hospital_Id });
        }


        public ActionResult HospitalReport(int id)
        {
            var model = new HospitalReportDTO();
            var Id = ent.Hospitals.Where(a => a.Id == id).ToList();
            TempData["HospitalId"] = Id.FirstOrDefault().Id;
            model.TestName = new SelectList(repos.GetTests(), "Id", "TestName");
            return View(model);
        }

        [HttpPost]
        public ActionResult UploadReport(HospitalReportDTO model)
        {
            var list = ent.Patients.Where(a => a.PatientName == model.PatientName).ToList();
            var PatientId = list.FirstOrDefault().Id;
            var HospitalId = TempData["HospitalId"];
            string[] allowedExts = { ".jpg", ".png", ".pdf" };
            if (model.File != null)
            {
                if (!FileOperation.CheckAllowedFiles(model.File, allowedExts))
                {
                    TempData["msg"] = "Invalid Extensions";
                    return RedirectToAction("HospitalReport", new { id = HospitalId });
                }
                foreach (HttpPostedFileBase file in model.File)
                {
                    var reportName = FileOperation.UploadFile(file, "HospitalReports", allowedExts);
                    var domain = Mapper.Map<HospitalReport>(model);
                    domain.Hospital_Id = Convert.ToInt32(HospitalId);
                    domain.File = reportName;
                    domain.Department = model.Department;
                    domain.Test = model.Test;
                    domain.Patient_Id = PatientId;
                    ent.HospitalReports.Add(domain);
                }
                ent.SaveChanges();
                TempData["msg"] = "Uploaded Successfully";
                return RedirectToAction("HospitalReport", new { id = HospitalId });
            }
            return RedirectToAction("HospitalReport", new { id = HospitalId });
        }


        [AllowAnonymous]
        public ActionResult ViewUploadReports(int id, string term, int? pageNumber)
        {
            var model = new HealthCheckupPatientList();
            var q = @"select C.Id, C.Patient_Id, P.PatientName from HospitalReport C join Patient P on P.Id = C.Patient_Id WHERE C.Hospital_Id= '" + id + "'GROUP BY C.Patient_Id, P.PatientName, C.Id";
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
            var q = @"select * from HospitalReport C 
join Patient P on P.Id = C.Patient_Id
join  LabTest lt on lt.Id = C.Test
join CityMaster cm on cm.Id = P.CityMaster_Id
join StateMaster sm on sm.Id = P.StateMaster_Id
WHERE C.Patient_Id=" + id;
            var data = ent.Database.SqlQuery<HospitalReports>(q).ToList();
            model.HospitalReports = data;
            return View(model);
        }
    }
}