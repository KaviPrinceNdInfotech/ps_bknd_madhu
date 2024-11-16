using AutoMapper;
using DocumentFormat.OpenXml.EMMA;
using HospitalPortal.BL;
using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.RequestModel;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Repositories;
using HospitalPortal.Utilities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.RightsManagement;
using System.Web.Mvc;
using static HospitalPortal.Utilities.EmailOperations;
using System.Windows.Interop;
using System.Data.SqlClient;

namespace HospitalPortal.Controllers
{
    //[Authorize(Roles = "admin,doctor,vendor")]
    public class DoctorRegistrationController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(DoctorRegistrationController));
        GenerateBookingId bk = new GenerateBookingId(); 
        private int GetDoctorId()
        {
            int loginId = Convert.ToInt32(User.Identity.Name);
            int DocId = ent.Database.SqlQuery<int>("select Id from Doctor where AdminLogin_Id=" + loginId).FirstOrDefault();
            return DocId;
        }

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Add(int vendorId = 0, int hospitalId = 0 )
        { 
            var model = new DoctorDTO();
            if (vendorId > 0)
            {
                model.Vendor_Id = vendorId;
            }
            if(hospitalId > 0)
            {
                model.HospitalId = hospitalId;
            }
            model.JoiningDate = DateTime.Now;
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
            model.DepartmentList = new SelectList(ent.Departments.ToList(), "Id", "DepartmentName");

            model.Vendor_Id = vendorId;
            model.VendorList = new SelectList(ent.Vendors.Where(a => a.IsDeleted == false && a.IsApproved == true).ToList(), "Id", "CompanyName");

            model.DayList = new SelectList(ent.DayNames.ToList(), "Id", "Name");
            model.DurationTimeList = new SelectList(ent.DurationTimes.ToList(), "Id", "Duration");
            return View(model);
        }
       
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Add(DoctorDTO model)
        {
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
            model.DepartmentList = new SelectList(ent.Departments.ToList(), "Id", "DepartmentName");
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (!string.IsNullOrEmpty(model.OtherCity))
                        ModelState.Remove("CityMaster_Id");
                   

                    if (ent.Doctors.Any(a => a.DoctorName == model.DoctorName && a.MobileNumber == model.MobileNumber))
                    {
                        var data = ent.Doctors.Where(a => a.DoctorName == model.DoctorName && a.MobileNumber == model.MobileNumber).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.DoctorId).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        TempData["msg"] = "you are already registered with pswellness";
                        return RedirectToAction("Add", "DoctorRegistration");
                    }

                    var admin = new AdminLogin
                    {
                        Username = model.EmailId.ToLower(),         
                        PhoneNumber = model.MobileNumber,
                        Password = model.Password,
                        Role = "doctor"
                    };
                    ent.AdminLogins.Add(admin);
                    ent.SaveChanges();

                    // Licence upload
                    if (model.LicenceImageFile != null)
                    {
                        var licenceImg = FileOperation.UploadImage(model.LicenceImageFile, "Images");
                        if (licenceImg == "not allowed")
                        {
                            TempData["msg"] = "Only png, jpg, jpeg, pdf files are allowed as Licence document";
                            tran.Rollback();
                            return View(model);
                        }
                        model.LicenceImage = licenceImg;
                    }

                    // signature upload
                    if (model.SignatureImageFile != null)
                    {
                        var SignatureImg = FileOperation.UploadImage(model.SignatureImageFile, "Images");
                        if (SignatureImg == "not allowed")
                        {
                            TempData["msg"] = "Only png, jpg, jpeg, pdf files are allowed as Licence document";
                            tran.Rollback();
                            return View(model);
                        }
                        model.SignaturePic = SignatureImg;
                    }
                    // pan upload
                    if (model.PanImageFile != null)
                    {
                        var Img = FileOperation.UploadImage(model.PanImageFile, "Images");
                        if (Img == "not allowed")
                        {
                            TempData["msg"] = "Only png, jpg, jpeg, pdf files are allowed as Pan document";
                            tran.Rollback();
                            return View(model);
                        }
                        model.PanImage = Img;
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

                    var domainModel = Mapper.Map<Doctor>(model);
                    if (model.Vendor_Id == 0)
                    {
                        domainModel.Vendor_Id = null;
                    }
                    else
                    {
                        domainModel.Vendor_Id = model.Vendor_Id;
                    }

                    domainModel.AdminLogin_Id = admin.Id;
                    domainModel.SlotTime = Convert.ToInt32(model.SlotTiming);
                    domainModel.SlotTime2 = Convert.ToInt32(model.SlotTiming2);
                    domainModel.DoctorId = bk.GenerateDoctorId();
                    admin.UserID = domainModel.DoctorId;
                    domainModel.EmailId = model.EmailId.ToLower();
                    domainModel.JoiningDate = DateTime.Now;
                    domainModel.Location = domainModel.Location;
                    domainModel.Qualification = domainModel.Qualification;
                    domainModel.RegistrationNumber = domainModel.RegistrationNumber;
                    domainModel.SignaturePic = model.SignaturePic;
                    domainModel.PanImage = model.PanImage;
                    domainModel.PAN = domainModel.PAN;
                    domainModel.Day_Id = domainModel.Day_Id;
                    domainModel.Fee = domainModel.Fee;
                    domainModel.VirtualFee = domainModel.VirtualFee;
                    domainModel.IsBankUpdateApproved = false;
                    ent.Doctors.Add(domainModel);
                    ent.SaveChanges();
                    //string msg = "Welcome to PSWELLNESS. Your User Name :  " + admin.Username + "(" + admin.UserID + "), Password : " + admin.Password + ".";

                    //Utilities.EmailOperations.SendEmail1(model.EmailId, "Ps Wellness", msg, true);
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
                    Message.SendSmsUserIdPass(model.MobileNumber, model.DoctorName, domainModel.DoctorId, model.Password);
                    TempData["msg"] = "ok";
                    tran.Commit();
                }
                //catch (Exception ex)
                catch (DbEntityValidationException  ex)
                {
                    log.Error(ex.Message);
                    TempData["msg"] = "Server Error";
                    tran.Rollback();
                   
                }
            }
            return RedirectToAction("Add", new { vendorId = model.Vendor_Id });
        }

        [HttpPost]
        public JsonResult AddDepartmentSpecialization(DoctorDepartment DepartmentData)
        {
            DoctorDepartment DocDept = new DoctorDepartment
            {
                Department_Id = (int)DepartmentData.Department_Id,
                Specialist_Id = (int)DepartmentData.Specialist_Id,
                Doctor_Id = GetDoctorId()
            };
            ent.DoctorDepartments.Add(DocDept);
            ent.SaveChanges();
            return Json(DocDept, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRecords()
        {
            string q = @"select * from DoctorDepartment dt join Department d on dt.Department_Id = d.Id join Specialist s on s.Id = dt.Specialist_Id";
            var data = ent.Database.SqlQuery<DoctorDepartment>(q).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteDepartment(int Id)
        {
            string q = @"delete from DoctorDepartment where Id=" + Id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("UpdateDepartment", new { Id= GetDoctorId()});
        }
        
        public ActionResult Edit(int id)
        {
            var data = ent.Doctors.Find(id);
            var model = Mapper.Map<DoctorDTO>(data);
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName", model.StateMaster_Id);
            model.Cities = new SelectList(repos.GetCitiesByState(model.StateMaster_Id), "Id", "CityName", model.CityMaster_Id);
            model.DepartmentList = new SelectList(ent.Departments.ToList(), "Id", "DepartmentName", model.Department_Id);
            model.SpecialistList = new SelectList(ent.Specialists.Where(a => a.Department_Id == model.Department_Id).ToList(), "Id", "SpecialistName", model.Specialist_Id);
            model.DayList = new SelectList(ent.DayNames.ToList(), "Id", "Name");
            model.DurationTimeList = new SelectList(ent.DurationTimes.ToList(), "Id", "Duration");
            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Edit(DoctorDTO model)
        {
            try
            {                 
                var existingDoctor = ent.Doctors.Find(model.Id); // Assuming 'Id' is the primary key
                if (existingDoctor == null)
                {
                    TempData["msg"] = "Doctor not found";
                    return RedirectToAction("Edit", new { id = model.Id });
                }
                // Licence upload
                if (model.LicenceImageFile != null)
                {
                    var licenceImg = FileOperation.UploadImage(model.LicenceImageFile, "Images");
                    if (licenceImg == "not allowed")
                    {
                        TempData["msg"] = "Only png, jpg, jpeg, pdf files are allowed as Licence document";
                       
                        return View(model);
                    }
                    model.LicenceImage = licenceImg;
                }

                // signature upload
                if (model.SignatureImageFile != null)
                {
                    var SignatureImg = FileOperation.UploadImage(model.SignatureImageFile, "Images");
                    if (SignatureImg == "not allowed")
                    {
                        TempData["msg"] = "Only png, jpg, jpeg, pdf files are allowed as Licence document";
                       
                        return View(model);
                    }
                    model.SignaturePic = SignatureImg;
                }
                // pan upload
                if (model.PanImageFile != null)
                {
                    var Img = FileOperation.UploadImage(model.PanImageFile, "Images");
                    if (Img == "not allowed")
                    {
                        TempData["msg"] = "Only png, jpg, jpeg, pdf files are allowed as Pan document";
                        
                        return View(model);
                    }
                    model.PanImage = Img;
                }

                // Update the existing entity with values from the model
                existingDoctor.DoctorName = model.DoctorName;
                existingDoctor.ClinicName = model.ClinicName;
                existingDoctor.Department_Id = (int)model.Department_Id;
                existingDoctor.Specialist_Id = (int)model.Specialist_Id;
                existingDoctor.PhoneNumber = model.PhoneNumber;
                existingDoctor.MobileNumber = model.MobileNumber;
                existingDoctor.EmailId = model.EmailId;
                existingDoctor.StateMaster_Id = model.StateMaster_Id;
                existingDoctor.CityMaster_Id = model.CityMaster_Id;
                existingDoctor.Location = model.Location;
                existingDoctor.LicenceImage = model.LicenceImage;
                existingDoctor.SignaturePic = model.SignaturePic;
                existingDoctor.PanImage = model.PanImage;
                existingDoctor.PAN = model.PAN;
                existingDoctor.LicenceNumber = model.LicenceNumber;
                existingDoctor.LicenseValidity = model.LicenseValidity;
                existingDoctor.Day_Id = model.Day_Id;
                existingDoctor.Fee = model.Fee;
                existingDoctor.VirtualFee = model.VirtualFee;
                existingDoctor.StartTime = model.StartTime;
                existingDoctor.EndTime = model.EndTime;
                existingDoctor.StartTime2 = model.StartTime2;
                existingDoctor.EndTime2 = model.EndTime2;
                existingDoctor.SlotTime = Convert.ToInt32(model.SlotTime);
                existingDoctor.SlotTime2 = Convert.ToInt32(model.SlotTime2);

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
            var model = new DoctorDTO();
            string q = @"select v.*,IsNull(ve.UniqueId,'N/A') as UniqueId, s.StateName,Special.*,Dept.*, c.CityName,IsNull(ve.VendorName,'NA') AS VendorName , IsNull(ve.CompanyName,'NA') as CompanyName from Doctor v join StateMaster s on v.StateMaster_Id=s.Id join CityMaster c on v.CityMaster_Id = c.Id inner join Department Dept on Dept.Id = v.Department_Id inner join Specialist Special on Special.Id = v.Specialist_Id left join Vendor ve on ve.Id = v.Vendor_Id where v.IsDeleted=0 order by v.Id desc";
            var data = ent.Database.SqlQuery<DoctorDTO>(q).ToList();
            if (vendorId != null)
                data = data.Where(a => a.Vendor_Id == vendorId).ToList();
            if (term != null)
            {
                data = data.Where(a =>a.DoctorId.Contains(term)).ToList();
            }
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View();
            }
             
            return View(data);
        }

        public ActionResult UpdateStatus(int id)
        {
            string q = @"update Doctor set IsApproved = case when IsApproved=1 then 0 else 1 end where id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            string mobile = ent.Database.SqlQuery<string>("select MobileNumber from Doctor where Id=" + id).FirstOrDefault();
            string Email = ent.Database.SqlQuery<string>(@"select EmailId from Doctor where Id="+ id).FirstOrDefault();
            string username = ent.Database.SqlQuery<string>(@"select DoctorId from Doctor where Id="+ id).FirstOrDefault();
            string Name = ent.Database.SqlQuery<string>(@"select DoctorName from Doctor where Id=" + id).FirstOrDefault();
            // Assuming you have access to the DbContext instance named 'ent'
            var query = "SELECT IsApproved FROM Doctor WHERE Id = @Id";
            var parameters = new SqlParameter("@Id", id);
            bool isApproved = ent.Database.SqlQuery<bool>(query, parameters).FirstOrDefault();

            var msg = "Dear " + Name + ", Now you Can Login With Your Username " + username + " and Pasword";
            Message.SendSms(mobile, msg);
             
            if(isApproved == true)
            {
                EmailEF ef = new EmailEF()
                {
                    EmailAddress = Email,
                    Message = msg,
                    Subject = "PS Wellness Approval Status."
                };
                EmailOperations.SendEmainew(ef);
            }
           

            return RedirectToAction("All");
        }

        public ActionResult UpdateBankUpdateStatus(int id)
        {
            string q = @"update Doctor set IsBankUpdateApproved = case when IsBankUpdateApproved=1 then 0 else 1 end where id=" + id;
            ent.Database.ExecuteSqlCommand(q);

            string mobile = ent.Database.SqlQuery<string>("select MobileNumber from Doctor where Id=" + id).FirstOrDefault();
            string Email = ent.Database.SqlQuery<string>(@"select EmailId from Doctor where Id=" + id).FirstOrDefault();
            string Name = ent.Database.SqlQuery<string>(@"select DoctorName from Doctor where Id=" + id).FirstOrDefault();
            //var msg = "Dear " + Name + ", Now you Can Upadate your bank details.";
            //Message.SendSms(mobile, msg);
            var query = "SELECT IsBankUpdateApproved FROM Doctor WHERE Id = @Id";
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
            var data = ent.Doctors.Find(id);
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

        public ActionResult Skills()
        {
            int doctorId = Convert.ToInt32(User.Identity.Name);
            var skills = ent.DoctorSkills.Where(a => a.Doctor_Id == doctorId).ToList();
            ViewBag.docId = doctorId;
            return View(skills);
        }

        public ActionResult AddSkill(DoctorSkillsDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Content("error");
                int doctorId = Convert.ToInt32(User.Identity.Name);
                model.Doctor_Id = doctorId;
                var skill = Mapper.Map<DoctorSkill>(model);
                ent.DoctorSkills.Add(skill);
                ent.SaveChanges();
                return Json(skill,JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return Content("error");
            }
        }

        public ContentResult RemoveSkill(int id)
        {
            try
            {
                var data = ent.DoctorSkills.Find(id);
                ent.DoctorSkills.Remove(data);
                ent.SaveChanges();
                return Content("ok");
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
                return Content("Server exception");
            }
        }
        [System.Web.Mvc.HttpGet]
        public ActionResult UpdateDepartment(int Id)
        {
            var model = new DoctorDTO();
            model.DepartmentList = new SelectList(ent.Departments.ToList(), "Id", "DepartmentName");
            string q = @"select * from DoctorDepartment dt join Department d on dt.Department_Id = d.Id join Specialist s on s.Id = dt.Specialist_Id where dt.Doctor_Id=" + Id;
            var data = ent.Database.SqlQuery<DepartmentModelClass>(q).ToList();
            model.DeptList = data;
            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult UpdateDepartment(DoctorDepartment DepartmentData)
        {
            DoctorDepartment DocDept = new DoctorDepartment
            {
                Department_Id = (int)DepartmentData.Department_Id,
                Specialist_Id = (int)DepartmentData.Specialist_Id,
                Doctor_Id = GetDoctorId()
            };
            ent.DoctorDepartments.Add(DocDept);
            ent.SaveChanges();
            return RedirectToAction("UpdateDepartment", new { Id = GetDoctorId() });
        }

        public ActionResult ViewAppoinments(int id, string term, int? pageNumber, DateTime? AppointmentDate)
        {
            var model = new Appointment();
            string query = @"select Patient.Id,Patient.MobileNumber, Patient.PatientName,{ fn concat(CONVERT(varchar(15),CAST(StartSlotTime AS TIME),100),{fn concat ('-', CONVERT(varchar(15),CAST(EndSlotTime AS TIME),100))})} AS AppointedTime, dbo.PatientAppointment.AppointmentDate,dbo.PatientAppointment.IsCancelled from Patient join dbo.PatientAppointment on Patient.Id = dbo.PatientAppointment.Patient_Id where Convert(Date,dbo.PatientAppointment.AppointmentDate)= Convert(Date,GETDATE()) and dbo.PatientAppointment.Doctor_Id=" + id + " and dbo.PatientAppointment.IsBooked=1 and dbo.PatientAppointment.IsPaid=1 order by AppointmentDate desc";
            var data = ent.Database.SqlQuery<ViewAppointmentByDoctorVM>(query).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                string query2 = @"select Patient.Id,Patient.MobileNumber, Patient.PatientName,{ fn concat(CONVERT(varchar(15),CAST(StartSlotTime AS TIME),100),{fn concat ('-', CONVERT(varchar(15),CAST(EndSlotTime AS TIME),100))})} AS AppointedTime, dbo.PatientAppointment.AppointmentDate,dbo.PatientAppointment.IsCancelled from Patient join dbo.PatientAppointment on Patient.Id = dbo.PatientAppointment.Patient_Id where dbo.PatientAppointment.Doctor_Id=" + id + " and dbo.PatientAppointment.IsBooked=1 and dbo.PatientAppointment.IsPaid=1 and Patient.PatientName='"+ term + "' or Patient.MobileNumber='"+ term + "' order by AppointmentDate desc";
                var data2 = ent.Database.SqlQuery<ViewAppointmentByDoctorVM>(query2).ToList();
                //data2 = data2.Where(a => a.PatientName.Contains(term) || a.MobileNumber.Contains(term)).ToList();
                model.ViewAppointByDoctor = data2;
                return View(model);
            }
            if(AppointmentDate != null)
            {
                string query1 = @"select Patient.Id,Patient.MobileNumber, Patient.PatientName,{ fn concat(CONVERT(varchar(15),CAST(StartSlotTime AS TIME),100),{fn concat ('-', CONVERT(varchar(15),CAST(EndSlotTime AS TIME),100))})} AS AppointedTime, dbo.PatientAppointment.AppointmentDate,dbo.PatientAppointment.IsCancelled from Patient join dbo.PatientAppointment on Patient.Id = dbo.PatientAppointment.Patient_Id where Convert(Date,dbo.PatientAppointment.AppointmentDate)= '" + AppointmentDate+"' and dbo.PatientAppointment.Doctor_Id=" + id + " and dbo.PatientAppointment.IsBooked=1 and dbo.PatientAppointment.IsPaid=1 order by AppointmentDate desc";
                var data1 = ent.Database.SqlQuery<ViewAppointmentByDoctorVM>(query1).ToList();
                model.ViewAppointByDoctor = data1;
                return View(model);
            }
            int total = data.Count();
            pageNumber = (int?)pageNumber ?? 1;
            int pagesize = 10;
            decimal noOfPages = Math.Ceiling((decimal)total / pagesize);
            model.TotalPages = (int)noOfPages;
            model.PageNumber = (int)pageNumber;
            data = data.OrderBy(a => a.Id).Skip(pagesize * ((int)pageNumber - 1)).Take(pagesize).ToList();
            model.ViewAppointByDoctor = data;
            return View(model);
        }

        public ActionResult DoctorPatientList(int? id, string term, int? pageNumber)
        {
            var mdoel = new PatientDTO();
            var q = @"select p.Patient_Id, patient.PatientName, patient.EmailId, patient.MobileNumber, patient.Location from dbo.PatientAppointment p JOIN patient on patient.Id = P.Patient_Id where Doctor_Id='"+id+"' group by p.Patient_Id,patient.PatientName,patient.EmailId, patient.MobileNumber, patient.Location";
            var data = ent.Database.SqlQuery<PatientList>(q).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                data = data.Where(a => a.PatientName.ToLower().Contains(term)).ToList();
            }
            int total = data.Count();
            pageNumber = (int?)pageNumber ?? 1;
            int pagesize = 10;
            decimal noOfpages = Math.Ceiling((decimal)total / pagesize);
            mdoel.TotalPages = (int)noOfpages;
            mdoel.PageNumber = (int)pageNumber;
            data = data.OrderBy(a => a.Id).Skip(pagesize * ((int)pageNumber - 1)).Take(pagesize).ToList();
            mdoel.Patient = data;
            return View(mdoel);
        }

        public ActionResult ViewReports(int id)
        {
            var model = new ViewDoctorReports();
            string q = @"select C.Patient_Id, P.PatientName, p.PatientRegNo from DoctorReports C join Patient P on P.Id = C.Patient_Id WHERE C.Doctor_Id= '" + id + "'GROUP BY C.Patient_Id, P.PatientName, p.PatientRegNo";
            var data = ent.Database.SqlQuery<PatientItem>(q).ToList();
            model.patientItem = data;
            return View(model);
        }

        public ActionResult TestList(int id)
        {
            var model = new ViewDoctorReports();
            var q = @"select * from DoctorReports C join Patient p on c.Patient_Id = p.Id  join CityMaster city on city.Id = p.CityMaster_Id join StateMaster statemaster on statemaster.Id = p.StateMaster_Id where  C.Patient_Id=" + id;
            var data = ent.Database.SqlQuery<TestHistory>(q).ToList();
            model.test = data;
            return View(model);
        }

        public ActionResult PaymentHistory(int id, DateTime? date = null)
        {
            //Call The Model
            var model = new PaymentHistroyForDosctor();
            IEnumerable<ListPayment> payment = null;
            // Use Http Client to Initialize HttpClient Class
            using (var client = new HttpClient())
            {
                //Call BaseAddress of API
                client.BaseAddress = new Uri("http://pswellness.in/api/");
                //Call WebApi Address With  Controller Name and ActionName + Parameters (If Any)
                var response = client.GetAsync("DoctorApi/paymentHistory?Id=" + id + "&Date=" + date);
                response.Wait();
                //Store the Response Result
                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    //Read the Content
                    var readTask = result.Content.ReadAsAsync<List<ListPayment>>();
                    //Serialise the JSON Object
                    string jsonstring = JsonConvert.SerializeObject(readTask);
                    readTask.Wait();
                    //Store the Result into IEnumerable List
                    model.PaymentHistory = readTask.Result;
                    payment = model.PaymentHistory;
                }
                else
                {
                    payment = Enumerable.Empty<ListPayment>();
                    ModelState.AddModelError(string.Empty, "No Records.");
                }
            }
            return View(payment);
        }

        public ActionResult ViewPayoutHistory(int id, DateTime? PaymentDate)
        {
            var model = new ViewPayOutHistory();
            var Name = ent.Database.SqlQuery<string>("select DoctorName from Doctor where Id=" + id).FirstOrDefault();
            model.DoctorName = Name;
            TempData["Id"] = id;
            string qry = @"select Dp.Id, ISNULL(Dp.IsPaid, 0) as IsPaid , Dp.IsGenerated, Dp.Doctor_Id, Dp.PaymentDate, Dp.Amount, D.DoctorName from  DoctorPayOut Dp join Doctor D on D.Id = Dp.Doctor_Id  where  Dp.Doctor_Id=" + id;
            var data = ent.Database.SqlQuery<HistoryOfDoc_Payout>(qry).ToList();
            if(PaymentDate != null)
            {
                string qry1 = @"select Dp.Id, ISNULL(Dp.IsPaid, 0) as IsPaid , Dp.IsGenerated, Dp.Doctor_Id, Dp.PaymentDate, Dp.Amount, D.DoctorName from  DoctorPayOut Dp join Doctor D on D.Id = Dp.Doctor_Id  where Dp.PaymentDate='"+PaymentDate+"' and  Dp.Doctor_Id=" + id;
                var data1 = ent.Database.SqlQuery<HistoryOfDoc_Payout>(qry1).ToList();
                model.HistoryOfDoc_Payout = data1;
                return View(model);
            }
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            model.HistoryOfDoc_Payout = data;
            return View(model);
        }
        //all user use in api =============================================================
        [System.Web.Http.Authorize(Roles = "admin,doctor,vendor,driver")]
        public ActionResult UpdateBank(int id)
        {
            var model = new UpdateBank();
            var data = ent.BankDetails.Where(a => a.Login_Id == id).ToList();
            if (data.Count() == 0)
            {
                return View();
            }
            else
            {
                model.AccountNo = data.FirstOrDefault().AccountNo;
                model.BranchAddress = data.FirstOrDefault().BranchAddress;
                model.BranchName = data.FirstOrDefault().BranchName;
                model.IFSCCode = data.FirstOrDefault().IFSCCode;
                model.Id = data.FirstOrDefault().Id;
                model.Login_Id = data.FirstOrDefault().Login_Id;
                return View(model);
            }
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult MyTDS(int id, int? pageNumber, DateTime? AppointmentDate)
        {
            double TDS = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault();
            var model = new ReportDTO();
            if (AppointmentDate != null)
            {
                DateTime dateCriteria = AppointmentDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select A.Doctor_Id, D.DoctorName, SUM(A.TotalFee) As Amount from dbo.PatientAppointment A join Doctor D on D.Id = A.Doctor_Id  where A.Doctor_Id=" + id + " and A.IsPaid=1 and A.AppointmentDate between '" + dateCriteria + "' and '" + AppointmentDate + "' GROUP BY A.TotalFee, D.DoctorName, A.Doctor_Id";
                var data1 = ent.Database.SqlQuery<DoctorCommissionReport>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.TDS = TDS;
                    int total = data1.Count;
                    pageNumber = (int?)pageNumber ?? 1;
                    int pageSize = 10;
                    decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                    model.TotalPages = (int)noOfPages;
                    model.PageNumber = (int)pageNumber;
                    data1 = data1.OrderBy(a => a.Doctor_Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                    model.DoctorCommisionReport = data1;
                    return View(model);
                }
            }
            var doctor = @"select A.Doctor_Id, D.DoctorName, SUM(A.TotalFee)  As Amount from dbo.PatientAppointment A join Doctor D on D.Id = A.Doctor_Id where A.Doctor_Id=" + id + " and A.IsPaid=1 and A.AppointmentDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() group by  D.DoctorName, A.Doctor_Id";
            var data = ent.Database.SqlQuery<DoctorCommissionReport>(doctor).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record Of Current Week";
                return View(model);
            }
            else
            {
                ViewBag.TDS = TDS;
                int total = data.Count;
                pageNumber = (int?)pageNumber ?? 1;
                int pageSize = 10;
                decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                model.TotalPages = (int)noOfPages;
                model.PageNumber = (int)pageNumber;
                data = data.OrderBy(a => a.Doctor_Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                model.DoctorCommisionReport = data;
                return View(model);
            }

        }

        public ActionResult MyCommission(int id, int? pageNumber, DateTime? AppointmentDate)
        {
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault();
            var model = new ReportDTO();
            if (AppointmentDate != null)
            {
                DateTime dateCriteria = AppointmentDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select A.Doctor_Id, D.DoctorName, SUM(A.Amount) As Amount from dbo.PatientAppointment A join Doctor D on D.Id = A.Doctor_Id  where A.IsPaid=1 and A.AppointmentDate between '" + dateCriteria + "' and '" + AppointmentDate + "' GROUP BY A.Amount, D.DoctorName, A.Doctor_Id";
                var data1 = ent.Database.SqlQuery<DoctorCommissionReport>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Commission = commision;
                    int total = data1.Count;
                    pageNumber = (int?)pageNumber ?? 1;
                    int pageSize = 10;
                    decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                    model.TotalPages = (int)noOfPages;
                    model.PageNumber = (int)pageNumber;
                    data1 = data1.OrderBy(a => a.Doctor_Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                    model.DoctorCommisionReport = data1;
                    return View(model);
                }
            }
            var doctor = @"select A.Doctor_Id, D.DoctorName, SUM(A.TotalFee)  As Amount from dbo.PatientAppointment A join Doctor D on D.Id = A.Doctor_Id where A.IsPaid=1 and A.AppointmentDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() group by A.TotalFee, D.DoctorName, A.Doctor_Id";
            var data = ent.Database.SqlQuery<DoctorCommissionReport>(doctor).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record Of Current Week";
                return View(model);
            }
            else
            {
                ViewBag.Commission = commision;
                int total = data.Count;
                pageNumber = (int?)pageNumber ?? 1;
                int pageSize = 10;
                decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                model.TotalPages = (int)noOfPages;
                model.PageNumber = (int)pageNumber;
                data = data.OrderBy(a => a.Doctor_Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                model.DoctorCommisionReport = data;
                return View(model);
            }

        }

        [System.Web.Mvc.HttpGet]
        public ActionResult RegisterClinic()
        {
            var model = new AddClinicDTO();
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
            int DocId = GetDoctorId();
            int noOfClinics = ent.DoctorClinics.Where(a => a.DoctorId == DocId).Count();
            model.noOfClinic = noOfClinics;
            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult RegisterClinic(AddClinicDTO model)
        {
            if (!ModelState.IsValid)
            {
                model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                return View(model);
            }
            try
            {
               foreach(var item in model.Clinic)
                {
                    var domain = new DoctorClinic();
                    domain.DoctorId = GetDoctorId();
                    domain.StartTime = item.StartTime;
                    domain.StateId = item.StateMaster_Id;
                    domain.FullAddress = item.FullAddress;
                    domain.EndTime = item.EndTime;
                    domain.ClinicName = item.ClinicName;
                    domain.CityId = item.CityMaster_Id;
                    ent.DoctorClinics.Add(domain);
                    ent.SaveChanges();
                }   
            }
            catch(Exception ex)
            {

            }
            return RedirectToAction("RegisterClinic");
        }
        public ActionResult ViewClinics()
        {
            var model = new AddClinicDTO();
            string q = @"select dc.Id, dc.ClinicName, cm.CityName, dc.FullAddress, sm.StateName, { fn concat(CONVERT(varchar(15),CAST(StartTime AS TIME),100),
{fn concat ('-', CONVERT(varchar(15),CAST(EndTime AS TIME),100))})} AS AppointedTime from DoctorClinics dc
join CityMaster cm on cm.Id = dc.CityId
join StateMaster sm on sm.Id = dc.StateId
where dc.DoctorId=" + GetDoctorId();
            var data = ent.Database.SqlQuery<ClinicDetails>(q).ToList();
            model.Clinic = data;
            return View(model);
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult EditClinic(int id)
        {
            var data = ent.DoctorClinics.Find(id);
            var model= Mapper.Map<EditClinicDTO>(data);
            model.StateMaster_Id = (int)data.StateId;
            model.CityMaster_Id = (int)data.CityId;
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName",model.StateMaster_Id);
            model.Cities = new SelectList(repos.GetCitiesByState(model.StateMaster_Id), "Id", "CityName", model.CityMaster_Id);
            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult EditClinic(EditClinicDTO model)
        {
            string q = @"update DoctorClinics set CityId= "+ model.CityMaster_Id + ",ClinicName='"+ model.ClinicName + "',EndTime='"+ model.EndTime + "',FullAddress='"+ model.FullAddress + "',StartTime='"+ model.StartTime + "',StateId="+model.StateMaster_Id+"where Id=" + model.Id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("EditClinic", new { id = model.Id });
        }
        public ActionResult DeleteClinic(int id)
        {
            string q = @"delete from DoctorClinics where Id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("ViewClinics");
        }

        public ActionResult Prescription()
        {
            
            return View();
            
        }
        [HttpPost]
        public ActionResult Prescription(PrescriptionDTO model)
        {
            try
            {
                var domainModel = Mapper.Map<PrescriptionAppointment>(model);
                //domainModel.SlotTime = Convert.ToInt32(model.SlotTiming);
                //domainModel.SlotTime2 = Convert.ToInt32(model.SlotTiming2);
                domainModel.Doctor_Id = GetDoctorId();
                domainModel.PreferredAppointmentDateTime1 = model.PreferredAppointmentDateTime1;
                domainModel.PreferredAppointmentDateTime2 = model.PreferredAppointmentDateTime2;
                domainModel.PreferredAppointmentDateTime3 = model.PreferredAppointmentDateTime3;
                domainModel.DoctorName = model.DoctorName;
                domainModel.HospitalName = model.HospitalName;
                domainModel.RegistrationNumber = model.RegistrationNumber;
                domainModel.DoctorCode = model.DoctorCode;
                domainModel.DoctorSpeciality = model.DoctorSpeciality;
                domainModel.EntryDate = DateTime.Now;
                domainModel.IsDeleted = false;
                domainModel.AppointmentHour1 = model.AppointmentHour1;
                domainModel.AppointmentMin1 = model.AppointmentMin1;
                domainModel.AppointmentHour2 = model.AppointmentHour2;
                domainModel.AppointmentMin2 = model.AppointmentMin2;
                domainModel.AppointmentHour3 = model.AppointmentHour3;
                domainModel.AppointmentMin3 = model.AppointmentMin3;
                ent.PrescriptionAppointments.Add(domainModel);
                ent.SaveChanges();
                TempData["msg"] = "ok";

            }
            //catch (Exception ex)
            catch (DbEntityValidationException ex)
            {
                log.Error(ex.Message);
                TempData["msg"] = "Server Error";

            }

            return RedirectToAction("Prescription");

        }
        public static int CalculateAge(DateTime dob)
        {
            DateTime currentDate = DateTime.Now;
            int age = currentDate.Year - dob.Year;

             
            if (currentDate < dob.AddYears(age))
            {
                age--;
            }

            return age;
        }

        public ActionResult PatientPerticuler(int? Id)
        {
            var model = new MedicinePrescriptionDTO();
            if (Id != null)
            {
                
                var data = ent.Patients.Where(a => a.Id == Id).FirstOrDefault();
                var data1 = ent.PatientAppointments.Where(a => a.Patient_Id == Id).OrderByDescending(a=>a.Id).FirstOrDefault();
                var data2 = ent.MedicinePrescriptionDetails.Where(a => a.Patient_Id == Id).FirstOrDefault();
                if(data1==null)
                {
                    TempData["msg"] = "This patient have not Appointment";

                    model.Patient = new SelectList(repos.GetPatient(), "Id", "PatientName");


                    return View(model);
                }

                model.Patient_Id = Id;
                model.PatientName = data.PatientName;
                model.PatientRegNo = data.PatientRegNo;
                model.Location = data.Location;
                model.EmailId = data.EmailId;
                model.MobileNumber = data.MobileNumber;
                model.DOB = data.DOB;
                model.PatientRegNo = data.PatientRegNo;
                model.Gender = data.Gender;
                //model.AppointmentId = data1.Id;
                model.AppointmentDate = data1.AppointmentDate;


                var Age = CalculateAge((DateTime)data.DOB);


                model.Patient = new SelectList(repos.GetPatient(), "Id", "PatientName");

                model.AgeValue = Age;
                return View(model);
            }

            model.Patient = new SelectList(repos.GetPatient(), "Id", "PatientName");

            
            return View(model);

        }

        [HttpPost]
        public ActionResult PatientPerticuler(MedicinePrescriptionDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.Patient = new SelectList(repos.GetPatient(), "Id", "PatientName");
                    return View(model);
                }

                 

                var domainModel = Mapper.Map<MedicinePrescriptionDetail>(model);
                //domainModel.SlotTime = Convert.ToInt32(model.SlotTiming);
                //domainModel.SlotTime2 = Convert.ToInt32(model.SlotTiming2);
                domainModel.Doctor_Id = GetDoctorId();
                domainModel.Patient_Id = model.Patient_Id;
                domainModel.PresentComplaint = model.PresentComplaint;
                domainModel.Allergies = model.Allergies;
                domainModel.Weight = model.Weight;
                domainModel.Primarydiagnosis = model.Primarydiagnosis;
                domainModel.Furtherrefferal_Recommendations = model.Furtherrefferal_Recommendations;
                domainModel.PastMedical_SurgicalHistory = model.PastMedical_SurgicalHistory;
                domainModel.MedicineName1 = model.MedicineName1;
                domainModel.Dosage1 = model.Dosage1;
                domainModel.Instruction1 = model.Instruction1;
                domainModel.MedicineName2 = model.MedicineName2;
                domainModel.Dosage2 = model.Dosage2;
                domainModel.Instruction2 = model.Instruction2;
                domainModel.MedicineName3 = model.MedicineName3;
                domainModel.Dosage3 = model.Dosage3;
                domainModel.Instruction3 = model.Instruction3;
                domainModel.MedicineName4 = model.MedicineName4;
                domainModel.Dosage4 = model.Dosage4;
                domainModel.Instruction4 = model.Instruction4;
                domainModel.MedicineName5 = model.MedicineName5;
                domainModel.Dosage5 = model.Dosage5;
                domainModel.Instruction5 = model.Instruction5;
                domainModel.MedicineName6 = model.MedicineName6;
                domainModel.Dosage6 = model.Dosage6;
                domainModel.Instruction6 = model.Instruction6;
                domainModel.MedicineName7 = model.MedicineName7;
                domainModel.Dosage7 = model.Dosage7;
                domainModel.Instruction7 = model.Instruction7;
                domainModel.MedicineName8 = model.MedicineName8;
                domainModel.Dosage8 = model.Dosage8;
                domainModel.Instruction8 = model.Instruction8;
                domainModel.MedicineName9 = model.MedicineName9;
                domainModel.Dosage9 = model.Dosage9;
                domainModel.Instruction9 = model.Instruction9;
                domainModel.MedicineName10 = model.MedicineName10;
                domainModel.Dosage10 = model.Dosage10;
                domainModel.Instruction10 = model.Instruction10;
                domainModel.TestPrescribed = model.TestPrescribed;
                domainModel.EntryDate = DateTime.Now;

                ent.MedicinePrescriptionDetails.Add(domainModel);
                ent.SaveChanges();
                TempData["msg"] = "ok";

                var lastrecordId = ent.MedicinePrescriptionDetails.OrderByDescending(a => a.Id).Select(a => a.Id).FirstOrDefault();
                string query = @"SELECT DISTINCT mpd.Id,mpd.EntryDate, d.Id,d.EmailId as DoctorEmailId,d.doctorId,d.RegistrationNumber, d.DoctorName, pa.AppointmentDate, p.PatientRegNo, p.PatientName, p.EmailId, p.MobileNumber, p.Gender,p.DOB,
mpd.*
FROM PatientAppointment AS pa
JOIN Doctor AS d ON d.Id = pa.Doctor_Id 
JOIN PrescriptionAppointments ppa ON ppa.Doctor_Id = d.Id
JOIN MedicinePrescriptionDetail mpd ON mpd.Doctor_Id = D.Id
JOIN Patient AS p ON p.Id = mpd.Patient_Id
WHERE mpd.Id = " + lastrecordId + " order by mpd.EntryDate desc";

                var prescription = ent.Database.SqlQuery<PrescriptionPdfModel>(query).FirstOrDefault();

                EmailEF ef = new EmailEF()
                {
                    EmailAddress = prescription.EmailId,
                    Message = "Medicine Prescription",
                    Subject = "Prescription Pdf.",

                };

                EmailOperations.SendEmainewpdf(ef);

                EmailEF drmail = new EmailEF()
                {
                    EmailAddress = prescription.DoctorEmailId,
                    Message = "Medicine Prescription",
                    Subject = "Prescription Pdf.",

                };

                EmailOperations.SendEmainewpdf(drmail); 

            }
            //catch (Exception ex)
            catch (DbEntityValidationException ex)
            {
                log.Error(ex.Message);
                TempData["msg"] = "Server Error";

            }

            return RedirectToAction("PatientPerticuler");
        }

        //Prescription pdf
        public string Sendemailpdf()  
        {

            var lastrecordId = ent.MedicinePrescriptionDetails.OrderByDescending(a => a.Id).Select(a => a.Id).FirstOrDefault();
            string query = @"SELECT DISTINCT mpd.Id,mpd.EntryDate, d.Id, d.doctorId,d.EmailId as DoctorEmailId,d.RegistrationNumber, d.DoctorName, pa.AppointmentDate, p.PatientRegNo, p.PatientName, p.EmailId, p.MobileNumber, p.Gender,p.DOB,
mpd.*
FROM PatientAppointment AS pa
JOIN Doctor AS d ON d.Id = pa.Doctor_Id 
JOIN PrescriptionAppointments ppa ON ppa.Doctor_Id = d.Id
JOIN MedicinePrescriptionDetail mpd ON mpd.Doctor_Id = D.Id
JOIN Patient AS p ON p.Id = mpd.Patient_Id
WHERE mpd.Id = " + lastrecordId + " order by mpd.EntryDate desc";

            var prescription = ent.Database.SqlQuery<PrescriptionPdfModel>(query).FirstOrDefault();

            EmailEF ef = new EmailEF()
            {
                EmailAddress = prescription.EmailId,
                Message = "Medicine Prescription",
                Subject = "Prescription Pdf.",
                 
            };

            EmailOperations.SendEmainewpdf(ef);

            EmailEF drmail = new EmailEF()
            {
                EmailAddress = prescription.DoctorEmailId,
                Message = "Medicine Prescription",
                Subject = "Prescription Pdf.",
                
            };

            EmailOperations.SendEmainewpdf(drmail);
             
            return "Email sent successfully";
        }
        public ActionResult MedicinePdf()
        {
            var lastrecordId =ent.MedicinePrescriptionDetails.OrderByDescending (a=>a.Id).Select (a=>a.Id).FirstOrDefault();
            // Execute the SQL query to retrieve prescription data based on the provided ID
            string query = @"SELECT DISTINCT mpd.Id,mpd.EntryDate, d.Id, d.doctorId, d.DoctorName,d.RegistrationNumber,d.SignaturePic, pa.AppointmentDate, p.PatientRegNo, p.PatientName, p.EmailId, p.MobileNumber, p.Gender,p.DOB,
mpd.*
FROM PatientAppointment AS pa
JOIN Doctor AS d ON d.Id = pa.Doctor_Id 
LEFT JOIN PrescriptionAppointments ppa ON ppa.Doctor_Id = d.Id
JOIN MedicinePrescriptionDetail mpd ON mpd.Doctor_Id = D.Id
JOIN Patient AS p ON p.Id = mpd.Patient_Id
WHERE mpd.Id = " + lastrecordId + " order by mpd.EntryDate desc";

            var prescription = ent.Database.SqlQuery<PrescriptionPdfModel>(query).FirstOrDefault();

            if (prescription == null)
            {
                return HttpNotFound();
            }

			using (var memoryStream = new MemoryStream())
			{
				using (var document = new Document())
				{
					PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
					document.Open();

					var header = new PdfPTable(2); // Two columns for doctor details and logo
					header.WidthPercentage = 100;
					header.SetWidths(new float[] { 1f, 1f }); // Set equal widths for the two columns
					header.DefaultCell.Border = PdfPCell.NO_BORDER;

					// Create a base color for the header background
					BaseColor headerBackgroundColor = new BaseColor(0, 255, 255); // Cyan  color

					// Add hospital name to the header with the background color
					var hospitalNameCell = new PdfPCell();
					hospitalNameCell.Border = PdfPCell.NO_BORDER;
					hospitalNameCell.BackgroundColor = headerBackgroundColor; // Set the background color
																			  //hospitalNameCell.FixedHeight = -10f;

					// Add hospital name
					var hospitalNameParagraph = new Paragraph("PS WELLNESS", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD));
					hospitalNameParagraph.Alignment = Element.ALIGN_LEFT;
					hospitalNameCell.AddElement(hospitalNameParagraph);
					hospitalNameCell.AddElement(new Paragraph($"Dr. {prescription.DoctorName}"));
					hospitalNameCell.AddElement(new Paragraph($"{prescription.Qualification}"));
					hospitalNameCell.AddElement(new Paragraph($"Reg No.: {prescription.RegistrationNumber}"));

					header.AddCell(hospitalNameCell);

					// Add the logo to the header with the background color
					var logoAndNameCell = new PdfPCell();
					logoAndNameCell.Border = PdfPCell.NO_BORDER;
					logoAndNameCell.BackgroundColor = headerBackgroundColor; // Set the background color
					logoAndNameCell.FixedHeight = -10f; // Corrected from hospitalNameCell

					// Add the logo
					var logoImage = Image.GetInstance(Server.MapPath("~/Images/PsLogo.png")); // Replace "logo.png" with your logo file path
					logoImage.ScaleToFit(100f, 100f); // Set the logo size as needed
					logoImage.Alignment = Element.ALIGN_RIGHT;
					logoAndNameCell.AddElement(logoImage);

					header.AddCell(logoAndNameCell);

					// Add a blank line as separator with the same background color
					var blankCell = new PdfPCell(new Phrase(" "));
					blankCell.Border = PdfPCell.NO_BORDER; // Remove cell border
					blankCell.BackgroundColor = headerBackgroundColor; // Set the background color
					blankCell.Colspan = 2; // Make the blank cell span both columns
					header.AddCell(blankCell);

					// Add doctor details to the header with the same background color
					// ... (previous code)

					document.Add(header); // Close the header section

					// Add patient details below the header
					var patientDetailsTable = new PdfPTable(4); // Create a table with 4 columns
					patientDetailsTable.DefaultCell.Border = PdfPCell.NO_BORDER;
					patientDetailsTable.WidthPercentage = 100; // Set the width percentage to align it to the left

					// Calculate age based on DOB
					DateTime dob = prescription.DOB;
					DateTime currentDate = DateTime.Now;

					int age = currentDate.Year - dob.Year;

					// Check if the birthday hasn't occurred this year yet
					if (currentDate.Month < dob.Month || (currentDate.Month == dob.Month && currentDate.Day < dob.Day))
					{
						age--;
					}

					// Add the cells with patient details
					// First column (labels)
					patientDetailsTable.AddCell(new PdfPCell(new Phrase("Date:", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD))) { Border = PdfPCell.NO_BORDER });
					patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.EntryDate.ToShortDateString(), new Font(Font.FontFamily.HELVETICA, 14))) { Border = PdfPCell.NO_BORDER });
					//patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.EntryDate.ToShortDateString())) { Border = PdfPCell.NO_BORDER });
					patientDetailsTable.AddCell(new PdfPCell(new Phrase("UHID:", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD))) { Border = PdfPCell.NO_BORDER });
					patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.PatientRegNo, new Font(Font.FontFamily.HELVETICA, 14))) { Border = PdfPCell.NO_BORDER });
					//patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.PatientRegNo)) { Border = PdfPCell.NO_BORDER });
					patientDetailsTable.AddCell(new PdfPCell(new Phrase("Name:", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD))) { Border = PdfPCell.NO_BORDER });
					patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.PatientName, new Font(Font.FontFamily.HELVETICA, 14))) { Border = PdfPCell.NO_BORDER });
					//patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.PatientName)) { Border = PdfPCell.NO_BORDER });
					patientDetailsTable.AddCell(new PdfPCell(new Phrase("Email:", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD))) { Border = PdfPCell.NO_BORDER });
					patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.EmailId, new Font(Font.FontFamily.HELVETICA, 14))) { Border = PdfPCell.NO_BORDER });
					//patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.EmailId)) { Border = PdfPCell.NO_BORDER });
					patientDetailsTable.AddCell(new PdfPCell(new Phrase("Mobile:", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD))) { Border = PdfPCell.NO_BORDER });
					patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.MobileNumber, new Font(Font.FontFamily.HELVETICA, 14))) { Border = PdfPCell.NO_BORDER });
					//patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.MobileNumber)) { Border = PdfPCell.NO_BORDER });
					patientDetailsTable.AddCell(new PdfPCell(new Phrase("Gender:", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD))) { Border = PdfPCell.NO_BORDER });
					patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.Gender, new Font(Font.FontFamily.HELVETICA, 14))) { Border = PdfPCell.NO_BORDER });
					//patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.Gender)) { Border = PdfPCell.NO_BORDER });
					patientDetailsTable.AddCell(new PdfPCell(new Phrase("Weight:", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD))) { Border = PdfPCell.NO_BORDER });
					patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.Weight, new Font(Font.FontFamily.HELVETICA, 14))) { Border = PdfPCell.NO_BORDER });
					//patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.Weight)) { Border = PdfPCell.NO_BORDER });
					patientDetailsTable.AddCell(new PdfPCell(new Phrase("Age:", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD))) { Border = PdfPCell.NO_BORDER });
					patientDetailsTable.AddCell(new PdfPCell(new Phrase(age.ToString(), new Font(Font.FontFamily.HELVETICA, 14))) { Border = PdfPCell.NO_BORDER });
					//patientDetailsTable.AddCell(new PdfPCell(new Phrase(age.ToString())) { Border = PdfPCell.NO_BORDER });



					document.Add(patientDetailsTable);


					var line = new LineSeparator(0.5f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, 1);

					var paragraphWithMargin = new Paragraph();
					paragraphWithMargin.Add(line);
					paragraphWithMargin.SpacingBefore = 5f; // Adjust the margin top value (10f in this example)
					paragraphWithMargin.Font.Size = 14f; // Change the size to your desired value
					document.Add(paragraphWithMargin);



					var captionParagraph = new Paragraph("Thank you for contacting  PS Wellness regarding your medical concern. As discussed with the doctor, please find below telemedical advice.");
					captionParagraph.Alignment = Element.ALIGN_LEFT;
					captionParagraph.SpacingBefore = 20f; // Adjust the margin top value (10f in this example)
					captionParagraph.Font.Size = 14f; // Change the size to your desired value
					document.Add(captionParagraph);



					var presentComplaintsSubheading = new Paragraph("Present Complaints", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
					presentComplaintsSubheading.Alignment = Element.ALIGN_LEFT;
					presentComplaintsSubheading.Alignment = Element.ALIGN_LEFT;
					presentComplaintsSubheading.SpacingBefore = 5f;
					document.Add(presentComplaintsSubheading);


					document.Add(paragraphWithMargin);


					var presentComplaintsContent = new Paragraph(prescription.PresentComplaint);
					presentComplaintsContent.Alignment = Element.ALIGN_LEFT;
					presentComplaintsContent.SpacingBefore = 20f;
					presentComplaintsContent.Font.Size = 14f; // Change the size to your desired value
					document.Add(presentComplaintsContent);


					var pastMedicalSubheading = new Paragraph("Past Medical/Surgical History", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
					pastMedicalSubheading.Alignment = Element.ALIGN_LEFT;
					pastMedicalSubheading.SpacingBefore = 5f;
					pastMedicalSubheading.Font.Size = 14f; // Change the size to your desired value
					document.Add(pastMedicalSubheading);

					document.Add(paragraphWithMargin);

					var pastMedicalContent = new Paragraph(prescription.PastMedical_SurgicalHistory);
					pastMedicalContent.Alignment = Element.ALIGN_LEFT;
					pastMedicalContent.SpacingBefore = 20f;
					pastMedicalContent.Font.Size = 14f;
					document.Add(pastMedicalContent);

					var allergiesSubheading = new Paragraph("Allergies, if any", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
					allergiesSubheading.Alignment = Element.ALIGN_LEFT;
					allergiesSubheading.SpacingBefore = 5f;
					allergiesSubheading.Font.Size = 14f; // Change the size to your desired value
					document.Add(allergiesSubheading);

					document.Add(paragraphWithMargin);

					var allergiesContent = new Paragraph(prescription.Allergies);
					allergiesContent.Alignment = Element.ALIGN_LEFT;
					allergiesContent.SpacingBefore = 20f;
					allergiesContent.Font.Size = 14f;
					document.Add(allergiesContent);

					var primaryDiagnosisSubheading = new Paragraph("Primary Diagnosis Based on Symptoms", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
					primaryDiagnosisSubheading.Alignment = Element.ALIGN_LEFT;
					primaryDiagnosisSubheading.SpacingBefore = 5f;
					document.Add(primaryDiagnosisSubheading);


					document.Add(paragraphWithMargin);

					var primaryDiagnosisContent = new Paragraph(prescription.Primarydiagnosis);
					primaryDiagnosisContent.Alignment = Element.ALIGN_LEFT;
					primaryDiagnosisContent.SpacingBefore = 20f;
					primaryDiagnosisContent.Font.Size = 14f;
					document.Add(primaryDiagnosisContent);

					var PrescribedMedicine = new Paragraph("Prescribed Medicine", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
					PrescribedMedicine.Alignment = Element.ALIGN_LEFT;
					PrescribedMedicine.SpacingBefore = 5f;
					document.Add(PrescribedMedicine);
					// Create a new PdfPTable with 4 columns
					var medicinedetail = new PdfPTable(4);
					medicinedetail.DefaultCell.Border = PdfPCell.BOTTOM_BORDER;
					medicinedetail.DefaultCell.BorderWidthLeft = 1f;
					medicinedetail.DefaultCell.BorderWidthRight = 1f;
					medicinedetail.WidthPercentage = 100;

					int rowNumber = 1;
					BaseColor purpleColor = new BaseColor(0, 255, 255); // Cyan color

					// Add cells with purple background
					// Assuming you have a font object defined somewhere, you can create it like this:
					Font font = new Font(Font.FontFamily.HELVETICA, 14f, Font.NORMAL); // Change the size (10f) as per your requirement

					// Then use this font in your PdfPCell creation like this:
					medicinedetail.AddCell(new PdfPCell(new Phrase("S#", font)) { Border = PdfPCell.BOTTOM_BORDER, BackgroundColor = purpleColor });
					medicinedetail.AddCell(new PdfPCell(new Phrase("Medicine Name", font)) { Border = PdfPCell.BOTTOM_BORDER, BackgroundColor = purpleColor });
					medicinedetail.AddCell(new PdfPCell(new Phrase("Dosage", font)) { Border = PdfPCell.BOTTOM_BORDER, BackgroundColor = purpleColor });
					medicinedetail.AddCell(new PdfPCell(new Phrase("Instruction", font)) { Border = PdfPCell.BOTTOM_BORDER, BackgroundColor = purpleColor });




					if (!string.IsNullOrEmpty(prescription.MedicineName1))
					{
						medicinedetail.AddCell(new PdfPCell(new Phrase(rowNumber.ToString(), font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.MedicineName1, font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Dosage1, font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Instruction1, font)) { Border = PdfPCell.BOTTOM_BORDER });
						rowNumber++;
					}

					if (!string.IsNullOrEmpty(prescription.MedicineName2))
					{
						medicinedetail.AddCell(new PdfPCell(new Phrase(rowNumber.ToString(), font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.MedicineName2, font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Dosage2, font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Instruction2, font)) { Border = PdfPCell.BOTTOM_BORDER });
						rowNumber++;
					}

					if (!string.IsNullOrEmpty(prescription.MedicineName3))
					{
						medicinedetail.AddCell(new PdfPCell(new Phrase(rowNumber.ToString(), font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.MedicineName3, font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Dosage3, font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Instruction3, font)) { Border = PdfPCell.BOTTOM_BORDER });
						rowNumber++;
					}

					if (!string.IsNullOrEmpty(prescription.MedicineName4))
					{
						medicinedetail.AddCell(new PdfPCell(new Phrase(rowNumber.ToString(), font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.MedicineName4, font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Dosage4, font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Instruction4, font)) { Border = PdfPCell.BOTTOM_BORDER });
						rowNumber++;
					}

					if (!string.IsNullOrEmpty(prescription.MedicineName5))
					{
						medicinedetail.AddCell(new PdfPCell(new Phrase(rowNumber.ToString(), font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.MedicineName5, font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Dosage5, font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Instruction5, font)) { Border = PdfPCell.BOTTOM_BORDER });
						rowNumber++;
					}

					if (!string.IsNullOrEmpty(prescription.MedicineName6))
					{
						medicinedetail.AddCell(new PdfPCell(new Phrase(rowNumber.ToString(), font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.MedicineName6, font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Dosage6, font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Instruction6, font)) { Border = PdfPCell.BOTTOM_BORDER });
						rowNumber++;
					}

					if (!string.IsNullOrEmpty(prescription.MedicineName7))
					{
						medicinedetail.AddCell(new PdfPCell(new Phrase(rowNumber.ToString(), font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.MedicineName7, font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Dosage7, font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Instruction7, font)) { Border = PdfPCell.BOTTOM_BORDER });
						rowNumber++;
					}

					if (!string.IsNullOrEmpty(prescription.MedicineName8))
					{
						medicinedetail.AddCell(new PdfPCell(new Phrase(rowNumber.ToString(), font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.MedicineName8, font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Dosage8, font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Instruction8, font)) { Border = PdfPCell.BOTTOM_BORDER });
						rowNumber++;
					}

					if (!string.IsNullOrEmpty(prescription.MedicineName9))
					{
						medicinedetail.AddCell(new PdfPCell(new Phrase(rowNumber.ToString(), font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.MedicineName9, font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Dosage9, font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Instruction9, font)) { Border = PdfPCell.BOTTOM_BORDER });
						rowNumber++;
					}

					if (!string.IsNullOrEmpty(prescription.MedicineName10))
					{
						medicinedetail.AddCell(new PdfPCell(new Phrase(rowNumber.ToString(), font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.MedicineName10, font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Dosage10, font)) { Border = PdfPCell.BOTTOM_BORDER });
						medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Instruction10, font)) { Border = PdfPCell.BOTTOM_BORDER });
						rowNumber++;
					}

					document.Add(medicinedetail);

					var TestPrescribed = new Paragraph("Test Prescribed", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
					TestPrescribed.Alignment = Element.ALIGN_LEFT;
					TestPrescribed.SpacingBefore = 5f;
					document.Add(TestPrescribed);

					document.Add(paragraphWithMargin);

					var TestPrescribedContent = new Paragraph(prescription.TestPrescribed);
					TestPrescribedContent.Alignment = Element.ALIGN_LEFT;
					TestPrescribedContent.SpacingBefore = 20f;
					TestPrescribedContent.Font.Size = 14f;
					document.Add(TestPrescribedContent);

					var Recommendations = new Paragraph("Further Referral / Recommendations", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
					Recommendations.Alignment = Element.ALIGN_LEFT;
					Recommendations.SpacingBefore = 5f;
					document.Add(Recommendations);

					document.Add(paragraphWithMargin);

					var RecommendationsContent = new Paragraph(prescription.Furtherrefferal_Recommendations);
					RecommendationsContent.Alignment = Element.ALIGN_LEFT;
					RecommendationsContent.SpacingBefore = 20f;
					RecommendationsContent.Font.Size = 14f;
					document.Add(RecommendationsContent);

					// Create a table for doctor's name, registration number, and signature
					var doctorInfoTable = new PdfPTable(2);
					doctorInfoTable.DefaultCell.Border = PdfPCell.NO_BORDER;
					doctorInfoTable.WidthPercentage = 100;


					// Cell for doctor's signature image
					var signatureCell = new PdfPCell();
					signatureCell.Border = PdfPCell.NO_BORDER;

					// Load the doctor's signature image
					string physicalPath = System.Web.HttpContext.Current.Server.MapPath("~/" + "Images" + "/" + prescription.SignaturePic);

					var signatureImage = Image.GetInstance(Server.MapPath("~/" + "Images" + "/" + prescription.SignaturePic)); // Replace with the actual path to the doctor's signature image

					if (signatureImage != null)
					{
						signatureImage.ScaleToFit(100f, 100f); // Set the image size as needed
						signatureImage.Alignment = Element.ALIGN_RIGHT; // Align the image to the right
						signatureCell.AddElement(signatureImage);
					}

					// Create a subtable for doctor's name and registration number
					var subtable = new PdfPTable(1);
					subtable.DefaultCell.Border = PdfPCell.NO_BORDER;


					// Add the doctor's name and registration number
					var doctorName = new Paragraph($"Dr. {prescription.DoctorName}", new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL));
					doctorName.Alignment = Element.ALIGN_RIGHT;
					var regNumber = new Paragraph($"Reg No.: {prescription.RegistrationNumber}", new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL));

					subtable.AddCell(doctorName);
					subtable.AddCell(regNumber);

					// Add the subtable and signatureCell to the doctorInfoTable
					doctorInfoTable.AddCell(subtable);
					doctorInfoTable.AddCell(signatureCell);

					document.Add(doctorInfoTable);





					// Add the disclaimer content
					var disclaimerText = "Disclaimer: The objective of this medical advice is to provide users of such services with information for a better understanding of their health and medical condition. This information and advice are not intended to be a substitute for professional physical meeting, examination, medical advice, diagnosis, or treatment. It should not be treated as a physical medical consultation and cannot be used for any medico-legal purpose. The decision to follow the medical advice is at the sole discretion of the user, and the advising doctor cannot be held responsible for providing such medical advice.";

					// Create a paragraph for the disclaimer
					var disclaimer = new Paragraph();

					// Create a smaller font for the disclaimer
					var smallerFont = FontFactory.GetFont(FontFactory.HELVETICA, 10f);

					// Create a bold font for the "Disclaimer:" label
					var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);

					// Add "Disclaimer:" in bold
					disclaimer.Add(new Chunk("Disclaimer: ", boldFont));

					// Add the rest of the disclaimer in regular font
					disclaimer.Add(new Chunk(disclaimerText.Substring("Disclaimer: ".Length), smallerFont));

					// Set alignment to left
					disclaimer.Alignment = Element.ALIGN_LEFT;

					// Add spacing before the disclaimer
					disclaimer.SpacingBefore = 20f;

					// Add the disclaimer paragraph to the document
					document.Add(disclaimer);

					document.Close();
				}

				// Prepare the response to download the PDF
				Response.Clear();
				Response.ContentType = "application/pdf";
				Response.AddHeader("Content-Disposition", "attachment; filename=Prescription.pdf");
				Response.BinaryWrite(memoryStream.ToArray());
				Response.End();
			}

			return new EmptyResult();
        }

        public ActionResult ViewPrescriptionReport(int id, string term, int? pageNumber)
        {

            var mdoel = new PatientDTO();
            //var q = @"SELECT * FROM MedicinePrescriptionDetail AS MPD JOIN patient AS P ON P.Id=MPD.Patient_Id where MPD.Doctor_Id='" + id + "'";
            var q = @"WITH CTE AS (
    SELECT MPD.Id,MPD.Patient_Id,MPD.Doctor_Id,p.PatientRegNo,p.PatientName,p.EmailId,p.MobileNumber,p.Location,
           ROW_NUMBER() OVER(PARTITION BY MPD.Patient_Id ORDER BY MPD.Id DESC) AS RowNum
    FROM MedicinePrescriptionDetail MPD
   JOIN Patient p on p.Id = MPD.Patient_Id
)
SELECT Id,Patient_Id,PatientRegNo,PatientName,EmailId,MobileNumber,Location
FROM CTE
WHERE RowNum = 1 and Doctor_Id='" + id + "' ORDER BY Id DESC";
           
            
            var data = ent.Database.SqlQuery<PatientList>(q).ToList();
            
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower(); // Convert the search term to lowercase.
                //data = data.Where(A => A.PatientName.ToLower().Contains(term)).ToList();
                data = data.Where(A => A.PatientName.ToLower().Contains(term) || A.PatientRegNo.ToLower() == term).ToList();

                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Match";
                    return View(mdoel);
                }
            }
            int total = data.Count();
            pageNumber = (int?)pageNumber ?? 1;
            int pagesize = 10;
            decimal noOfpages = Math.Ceiling((decimal)total / pagesize);
            mdoel.TotalPages = (int)noOfpages;
            mdoel.PageNumber = (int)pageNumber;
            data = data.OrderBy(a => a.Id).Skip(pagesize * ((int)pageNumber - 1)).Take(pagesize).ToList();
            mdoel.Patient = data;
            return View(mdoel);
        }

        public ActionResult DowloadPrescriptionReportPdf(int? Id)
        {
            //var lastrecordId = ent.MedicinePrescriptionDetails.OrderByDescending(a => a.Id).Select(a => a.Id).FirstOrDefault();
            // Execute the SQL query to retrieve prescription data based on the provided ID
            string query = @"SELECT DISTINCT mpd.Id,mpd.EntryDate, d.Id, d.doctorId, d.DoctorName,d.SignaturePic,d.Qualification,d.RegistrationNumber, pa.AppointmentDate, p.PatientRegNo, p.PatientName, p.EmailId, p.MobileNumber, p.Gender,p.DOB,
mpd.*
FROM PatientAppointment AS pa
JOIN Doctor AS d ON d.Id = pa.Doctor_Id 
LEFT JOIN PrescriptionAppointments ppa ON ppa.Doctor_Id = d.Id
LEFT JOIN MedicinePrescriptionDetail mpd ON mpd.Doctor_Id = D.Id
JOIN Patient AS p ON p.Id = mpd.Patient_Id
WHERE mpd.Id = " + Id + " order by mpd.EntryDate desc";

            var prescription = ent.Database.SqlQuery<PrescriptionPdfModel>(query).FirstOrDefault();

            if (prescription == null)
            {
                TempData["msg"] = "Record not found.";
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var document = new Document())
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    var header = new PdfPTable(2); // Two columns for doctor details and logo
                    header.WidthPercentage = 100;
                    header.SetWidths(new float[] { 1f, 1f }); // Set equal widths for the two columns
                    header.DefaultCell.Border = PdfPCell.NO_BORDER;

                    // Create a base color for the header background
                    BaseColor headerBackgroundColor = new BaseColor(0, 255, 255); // Cyan  color

                    // Add hospital name to the header with the background color
                    var hospitalNameCell = new PdfPCell();
                    hospitalNameCell.Border = PdfPCell.NO_BORDER;
                    hospitalNameCell.BackgroundColor = headerBackgroundColor; // Set the background color
                    //hospitalNameCell.FixedHeight = -10f;

                    // Add hospital name
                    var hospitalNameParagraph = new Paragraph("PS WELLNESS", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD));
                    hospitalNameParagraph.Alignment = Element.ALIGN_LEFT;
                    hospitalNameCell.AddElement(hospitalNameParagraph);
                    hospitalNameCell.AddElement(new Paragraph($"Dr. {prescription.DoctorName}"));
                    hospitalNameCell.AddElement(new Paragraph($"{prescription.Qualification}"));
                    hospitalNameCell.AddElement(new Paragraph($"Reg No.: {prescription.RegistrationNumber}"));

                    header.AddCell(hospitalNameCell);

                    // Add the logo to the header with the background color
                    var logoAndNameCell = new PdfPCell();
                    logoAndNameCell.Border = PdfPCell.NO_BORDER;
                    logoAndNameCell.BackgroundColor = headerBackgroundColor; // Set the background color
                    logoAndNameCell.FixedHeight = -10f; // Corrected from hospitalNameCell

                    // Add the logo
                    var logoImage = Image.GetInstance(Server.MapPath("~/Images/PsLogo.png")); // Replace "logo.png" with your logo file path
                    logoImage.ScaleToFit(100f, 100f); // Set the logo size as needed
                    logoImage.Alignment = Element.ALIGN_RIGHT;
                    logoAndNameCell.AddElement(logoImage);

                    header.AddCell(logoAndNameCell);

                    // Add a blank line as separator with the same background color
                    var blankCell = new PdfPCell(new Phrase(" "));
                    blankCell.Border = PdfPCell.NO_BORDER; // Remove cell border
                    blankCell.BackgroundColor = headerBackgroundColor; // Set the background color
                    blankCell.Colspan = 2; // Make the blank cell span both columns
                    header.AddCell(blankCell);

                    // Add doctor details to the header with the same background color
                    // ... (previous code)

                    document.Add(header); // Close the header section

                    // Add patient details below the header
                    var patientDetailsTable = new PdfPTable(4); // Create a table with 4 columns
                    patientDetailsTable.DefaultCell.Border = PdfPCell.NO_BORDER;
                    patientDetailsTable.WidthPercentage = 100; // Set the width percentage to align it to the left

                    // Calculate age based on DOB
                    DateTime dob = prescription.DOB;
                    DateTime currentDate = DateTime.Now;

                    int age = currentDate.Year - dob.Year;

                    // Check if the birthday hasn't occurred this year yet
                    if (currentDate.Month < dob.Month || (currentDate.Month == dob.Month && currentDate.Day < dob.Day))
                    {
                        age--;
                    }

                    // Add the cells with patient details
                    // First column (labels)
                    patientDetailsTable.AddCell(new PdfPCell(new Phrase("Date:", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD))) { Border = PdfPCell.NO_BORDER });
                    patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.EntryDate.ToShortDateString(), new Font(Font.FontFamily.HELVETICA, 14))) { Border = PdfPCell.NO_BORDER });
                    //patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.EntryDate.ToShortDateString())) { Border = PdfPCell.NO_BORDER });
                    patientDetailsTable.AddCell(new PdfPCell(new Phrase("UHID:", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD))) { Border = PdfPCell.NO_BORDER });
                    patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.PatientRegNo, new Font(Font.FontFamily.HELVETICA, 14))) { Border = PdfPCell.NO_BORDER });
                    //patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.PatientRegNo)) { Border = PdfPCell.NO_BORDER });
                    patientDetailsTable.AddCell(new PdfPCell(new Phrase("Name:", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD))) { Border = PdfPCell.NO_BORDER });
                    patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.PatientName, new Font(Font.FontFamily.HELVETICA, 14))) { Border = PdfPCell.NO_BORDER });
                    //patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.PatientName)) { Border = PdfPCell.NO_BORDER });
                    patientDetailsTable.AddCell(new PdfPCell(new Phrase("Email:", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD))) { Border = PdfPCell.NO_BORDER });
                    patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.EmailId, new Font(Font.FontFamily.HELVETICA, 14))) { Border = PdfPCell.NO_BORDER });
                    //patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.EmailId)) { Border = PdfPCell.NO_BORDER });
                    patientDetailsTable.AddCell(new PdfPCell(new Phrase("Mobile:", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD))) { Border = PdfPCell.NO_BORDER });
                    patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.MobileNumber, new Font(Font.FontFamily.HELVETICA, 14))) { Border = PdfPCell.NO_BORDER });
                    //patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.MobileNumber)) { Border = PdfPCell.NO_BORDER });
                    patientDetailsTable.AddCell(new PdfPCell(new Phrase("Gender:", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD))) { Border = PdfPCell.NO_BORDER });
                    patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.Gender, new Font(Font.FontFamily.HELVETICA, 14))) { Border = PdfPCell.NO_BORDER });
                    //patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.Gender)) { Border = PdfPCell.NO_BORDER });
                    patientDetailsTable.AddCell(new PdfPCell(new Phrase("Weight:", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD))) { Border = PdfPCell.NO_BORDER });
                    patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.Weight, new Font(Font.FontFamily.HELVETICA, 14))) { Border = PdfPCell.NO_BORDER });
                    //patientDetailsTable.AddCell(new PdfPCell(new Phrase(prescription.Weight)) { Border = PdfPCell.NO_BORDER });
                    patientDetailsTable.AddCell(new PdfPCell(new Phrase("Age:", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD))) { Border = PdfPCell.NO_BORDER });
                    patientDetailsTable.AddCell(new PdfPCell(new Phrase(age.ToString(), new Font(Font.FontFamily.HELVETICA, 14))) { Border = PdfPCell.NO_BORDER });
                    //patientDetailsTable.AddCell(new PdfPCell(new Phrase(age.ToString())) { Border = PdfPCell.NO_BORDER });



                    document.Add(patientDetailsTable);


                    var line = new LineSeparator(0.5f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, 1);

                    var paragraphWithMargin = new Paragraph();
                    paragraphWithMargin.Add(line);
                    paragraphWithMargin.SpacingBefore = 5f; // Adjust the margin top value (10f in this example)
					paragraphWithMargin.Font.Size = 14f; // Change the size to your desired value
					document.Add(paragraphWithMargin);



                    var captionParagraph = new Paragraph("Thank you for contacting  PS Wellness regarding your medical concern. As discussed with the doctor, please find below telemedical advice.");
                    captionParagraph.Alignment = Element.ALIGN_LEFT;
                    captionParagraph.SpacingBefore = 20f; // Adjust the margin top value (10f in this example)
					captionParagraph.Font.Size = 14f; // Change the size to your desired value
					document.Add(captionParagraph);



                    var presentComplaintsSubheading = new Paragraph("Present Complaints", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
                    presentComplaintsSubheading.Alignment = Element.ALIGN_LEFT;
                    presentComplaintsSubheading.Alignment = Element.ALIGN_LEFT;
                    presentComplaintsSubheading.SpacingBefore = 5f;
                    document.Add(presentComplaintsSubheading);


                    document.Add(paragraphWithMargin);


                    var presentComplaintsContent = new Paragraph(prescription.PresentComplaint);
                    presentComplaintsContent.Alignment = Element.ALIGN_LEFT;
                    presentComplaintsContent.SpacingBefore = 20f;
					presentComplaintsContent.Font.Size = 14f; // Change the size to your desired value
					document.Add(presentComplaintsContent);


                    var pastMedicalSubheading = new Paragraph("Past Medical/Surgical History", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
                    pastMedicalSubheading.Alignment = Element.ALIGN_LEFT;
                    pastMedicalSubheading.SpacingBefore = 5f;
					pastMedicalSubheading.Font.Size = 14f; // Change the size to your desired value
					document.Add(pastMedicalSubheading);

                    document.Add(paragraphWithMargin);

                    var pastMedicalContent = new Paragraph(prescription.PastMedical_SurgicalHistory);
                    pastMedicalContent.Alignment = Element.ALIGN_LEFT;
                    pastMedicalContent.SpacingBefore = 20f;
					pastMedicalContent.Font.Size = 14f;
					document.Add(pastMedicalContent);

                    var allergiesSubheading = new Paragraph("Allergies, if any", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
                    allergiesSubheading.Alignment = Element.ALIGN_LEFT;
                    allergiesSubheading.SpacingBefore = 5f;
					allergiesSubheading.Font.Size = 14f; // Change the size to your desired value
					document.Add(allergiesSubheading);

                    document.Add(paragraphWithMargin);

                    var allergiesContent = new Paragraph(prescription.Allergies);
                    allergiesContent.Alignment = Element.ALIGN_LEFT;
                    allergiesContent.SpacingBefore = 20f;
					allergiesContent.Font.Size = 14f;
					document.Add(allergiesContent);

                    var primaryDiagnosisSubheading = new Paragraph("Primary Diagnosis Based on Symptoms", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
                    primaryDiagnosisSubheading.Alignment = Element.ALIGN_LEFT;
                    primaryDiagnosisSubheading.SpacingBefore = 5f;
                    document.Add(primaryDiagnosisSubheading);


                    document.Add(paragraphWithMargin);

                    var primaryDiagnosisContent = new Paragraph(prescription.Primarydiagnosis);
                    primaryDiagnosisContent.Alignment = Element.ALIGN_LEFT;
                    primaryDiagnosisContent.SpacingBefore = 20f;
					primaryDiagnosisContent.Font.Size = 14f;
					document.Add(primaryDiagnosisContent);

                    var PrescribedMedicine = new Paragraph("Prescribed Medicine", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
                    PrescribedMedicine.Alignment = Element.ALIGN_LEFT;
                    PrescribedMedicine.SpacingBefore = 5f;
                    document.Add(PrescribedMedicine);
                    // Create a new PdfPTable with 4 columns
                    var medicinedetail = new PdfPTable(4);
                    medicinedetail.DefaultCell.Border = PdfPCell.BOTTOM_BORDER;
                    medicinedetail.DefaultCell.BorderWidthLeft = 1f;
                    medicinedetail.DefaultCell.BorderWidthRight = 1f;
                    medicinedetail.WidthPercentage = 100;

                    int rowNumber = 1;
                    BaseColor purpleColor = new BaseColor(0, 255, 255); // Cyan color

					// Add cells with purple background
					// Assuming you have a font object defined somewhere, you can create it like this:
					Font font = new Font(Font.FontFamily.HELVETICA, 14f, Font.NORMAL); // Change the size (10f) as per your requirement

					// Then use this font in your PdfPCell creation like this:
					medicinedetail.AddCell(new PdfPCell(new Phrase("S#", font)) { Border = PdfPCell.BOTTOM_BORDER, BackgroundColor = purpleColor });
					medicinedetail.AddCell(new PdfPCell(new Phrase("Medicine Name", font)) { Border = PdfPCell.BOTTOM_BORDER, BackgroundColor = purpleColor });
					medicinedetail.AddCell(new PdfPCell(new Phrase("Dosage", font)) { Border = PdfPCell.BOTTOM_BORDER, BackgroundColor = purpleColor });
					medicinedetail.AddCell(new PdfPCell(new Phrase("Instruction", font)) { Border = PdfPCell.BOTTOM_BORDER, BackgroundColor = purpleColor });


					 

                    if (!string.IsNullOrEmpty(prescription.MedicineName1))
                    {
                        medicinedetail.AddCell(new PdfPCell(new Phrase(rowNumber.ToString(), font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.MedicineName1, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Dosage1, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Instruction1, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        rowNumber++;
                    }

                    if (!string.IsNullOrEmpty(prescription.MedicineName2))
                    {
                        medicinedetail.AddCell(new PdfPCell(new Phrase(rowNumber.ToString(), font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.MedicineName2, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Dosage2, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Instruction2, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        rowNumber++;
                    }

                    if (!string.IsNullOrEmpty(prescription.MedicineName3))
                    {
                        medicinedetail.AddCell(new PdfPCell(new Phrase(rowNumber.ToString(), font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.MedicineName3, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Dosage3, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Instruction3, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        rowNumber++;
                    }

                    if (!string.IsNullOrEmpty(prescription.MedicineName4))
                    {
                        medicinedetail.AddCell(new PdfPCell(new Phrase(rowNumber.ToString(), font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.MedicineName4, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Dosage4, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Instruction4, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        rowNumber++;
                    }

                    if (!string.IsNullOrEmpty(prescription.MedicineName5))
                    {
                        medicinedetail.AddCell(new PdfPCell(new Phrase(rowNumber.ToString(), font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.MedicineName5, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Dosage5, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Instruction5, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        rowNumber++;
                    }

                    if (!string.IsNullOrEmpty(prescription.MedicineName6))
                    {
                        medicinedetail.AddCell(new PdfPCell(new Phrase(rowNumber.ToString(), font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.MedicineName6, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Dosage6, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Instruction6, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        rowNumber++;
                    }

                    if (!string.IsNullOrEmpty(prescription.MedicineName7))
                    {
                        medicinedetail.AddCell(new PdfPCell(new Phrase(rowNumber.ToString(), font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.MedicineName7, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Dosage7, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Instruction7, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        rowNumber++;
                    }

                    if (!string.IsNullOrEmpty(prescription.MedicineName8))
                    {
                        medicinedetail.AddCell(new PdfPCell(new Phrase(rowNumber.ToString(), font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.MedicineName8, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Dosage8, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Instruction8, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        rowNumber++;
                    }

                    if (!string.IsNullOrEmpty(prescription.MedicineName9))
                    {
                        medicinedetail.AddCell(new PdfPCell(new Phrase(rowNumber.ToString(), font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.MedicineName9, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Dosage9, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Instruction9, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        rowNumber++;
                    }

                    if (!string.IsNullOrEmpty(prescription.MedicineName10))
                    {
                        medicinedetail.AddCell(new PdfPCell(new Phrase(rowNumber.ToString(), font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.MedicineName10, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Dosage10, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        medicinedetail.AddCell(new PdfPCell(new Phrase(prescription.Instruction10, font)) { Border = PdfPCell.BOTTOM_BORDER });
                        rowNumber++;
                    }

                    document.Add(medicinedetail);

                    var TestPrescribed = new Paragraph("Test Prescribed", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
                    TestPrescribed.Alignment = Element.ALIGN_LEFT;
                    TestPrescribed.SpacingBefore = 5f;
                    document.Add(TestPrescribed);

                    document.Add(paragraphWithMargin);

                    var TestPrescribedContent = new Paragraph(prescription.TestPrescribed);
                    TestPrescribedContent.Alignment = Element.ALIGN_LEFT;
                    TestPrescribedContent.SpacingBefore = 20f;
					TestPrescribedContent.Font.Size = 14f;
					document.Add(TestPrescribedContent);

                    var Recommendations = new Paragraph("Further Referral / Recommendations", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
                    Recommendations.Alignment = Element.ALIGN_LEFT;
                    Recommendations.SpacingBefore = 5f;
                    document.Add(Recommendations);

                    document.Add(paragraphWithMargin);

                    var RecommendationsContent = new Paragraph(prescription.Furtherrefferal_Recommendations);
                    RecommendationsContent.Alignment = Element.ALIGN_LEFT;
                    RecommendationsContent.SpacingBefore = 20f;
					RecommendationsContent.Font.Size = 14f;
					document.Add(RecommendationsContent);

                    // Create a table for doctor's name, registration number, and signature
                    var doctorInfoTable = new PdfPTable(2);
                    doctorInfoTable.DefaultCell.Border = PdfPCell.NO_BORDER;
                    doctorInfoTable.WidthPercentage = 100;


                    // Cell for doctor's signature image
                    var signatureCell = new PdfPCell();
                    signatureCell.Border = PdfPCell.NO_BORDER;

                    // Load the doctor's signature image
                    string physicalPath = System.Web.HttpContext.Current.Server.MapPath("~/" + "Images" + "/" + prescription.SignaturePic);

                    var signatureImage = Image.GetInstance(Server.MapPath("~/" + "Images" + "/" + prescription.SignaturePic)); // Replace with the actual path to the doctor's signature image

                    if (signatureImage != null)
                    {
                        signatureImage.ScaleToFit(100f, 100f); // Set the image size as needed
                        signatureImage.Alignment = Element.ALIGN_RIGHT; // Align the image to the right
                        signatureCell.AddElement(signatureImage);
                    }

                    // Create a subtable for doctor's name and registration number
                    var subtable = new PdfPTable(1);
                    subtable.DefaultCell.Border = PdfPCell.NO_BORDER;


                    // Add the doctor's name and registration number
                    var doctorName = new Paragraph($"Dr. {prescription.DoctorName}", new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL));
                    doctorName.Alignment = Element.ALIGN_RIGHT;
                    var regNumber = new Paragraph($"Reg No.: {prescription.RegistrationNumber}", new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL));

                    subtable.AddCell(doctorName);
                    subtable.AddCell(regNumber);

                    // Add the subtable and signatureCell to the doctorInfoTable
                    doctorInfoTable.AddCell(subtable);
                    doctorInfoTable.AddCell(signatureCell);

                    document.Add(doctorInfoTable);





                    // Add the disclaimer content
                    var disclaimerText = "Disclaimer: The objective of this medical advice is to provide users of such services with information for a better understanding of their health and medical condition. This information and advice are not intended to be a substitute for professional physical meeting, examination, medical advice, diagnosis, or treatment. It should not be treated as a physical medical consultation and cannot be used for any medico-legal purpose. The decision to follow the medical advice is at the sole discretion of the user, and the advising doctor cannot be held responsible for providing such medical advice.";

                    // Create a paragraph for the disclaimer
                    var disclaimer = new Paragraph();

                    // Create a smaller font for the disclaimer
                    var smallerFont = FontFactory.GetFont(FontFactory.HELVETICA, 10f);

                    // Create a bold font for the "Disclaimer:" label
                    var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f);

                    // Add "Disclaimer:" in bold
                    disclaimer.Add(new Chunk("Disclaimer: ", boldFont));

                    // Add the rest of the disclaimer in regular font
                    disclaimer.Add(new Chunk(disclaimerText.Substring("Disclaimer: ".Length), smallerFont));

                    // Set alignment to left
                    disclaimer.Alignment = Element.ALIGN_LEFT;

                    // Add spacing before the disclaimer
                    disclaimer.SpacingBefore = 20f;

                    // Add the disclaimer paragraph to the document
                    document.Add(disclaimer);

                    document.Close();
                }

                // Prepare the response to download the PDF
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment; filename=Prescription.pdf");
                Response.BinaryWrite(memoryStream.ToArray());
                Response.End();
            }

            return new EmptyResult();
        }
    }
}
