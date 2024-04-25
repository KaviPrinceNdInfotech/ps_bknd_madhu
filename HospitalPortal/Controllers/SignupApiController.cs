using AutoMapper;
using HospitalPortal.BL;
using HospitalPortal.Models;
using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.RequestModel;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Repositories;
using HospitalPortal.Utilities;
using iTextSharp.text.pdf.qrcode;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Mvc;
using static HospitalPortal.Models.ViewModels.ChemistDTO;
using static HospitalPortal.Utilities.EmailOperations;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;

namespace HospitalPortal.Controllers
{
    public class SignupApiController : ApiController
    {
        GenerateBookingId Patient = new GenerateBookingId();
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(SignupApiController));
        returnMessage rm = new returnMessage();
        GenerateBookingId bk = new GenerateBookingId();

        private readonly Random _random = new Random();

        public IHttpActionResult GetStates()
        {
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult PatientRegistration(PatientDTO model)
        {
            var rm = new ReturnMessage();
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                { 
                    if (ent.AdminLogins.Any(a => a.Username == model.EmailId))
                    {
                        rm.Message = "This Email-Id has already exists.";
                        rm.Status = 0;
                        return Ok(rm);
                    }
                    if (ent.AdminLogins.Any(a => a.PhoneNumber == model.MobileNumber))
                    {
                        rm.Message = "This Mobile Number has already exists.";
                        rm.Status = 0;
                        return Ok(rm);
                    }

                    if (ent.Patients.Any(a => a.PatientName == model.PatientName && a.MobileNumber == model.MobileNumber))
                    {
                        var data = ent.Patients.Where(a => a.PatientName == model.PatientName && a.MobileNumber == model.MobileNumber).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.PatientRegNo).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        rm.Message = "you are already registered with pswellness";
                        rm.Status = 1;
                        return Ok(rm);
                    }
                    var PUniqId = Patient.GeneratePatientRegNo();

                    var admin = new AdminLogin
                    {
                        Username = model.EmailId,
                        PhoneNumber = model.MobileNumber,
                        Password = model.Password,
                        UserID = PUniqId,
                        Role = "patient"
                    };
                    ent.AdminLogins.Add(admin);
                    ent.SaveChanges();

                    //Add City Additional CityName
                    if (model.CityName != null)
                    {
                        var city = new CityTemp
                        {
                            CityName = model.CityName,
                            Login_Id = admin.Id,
                            IsApproved = false,
                            State_Id = model.StateMaster_Id
                        };
                        ent.CityTemps.Add(city);
                        ent.SaveChanges();
                        model.CityMaster_Id = city.Id;
                    }
                    var domainModel = Mapper.Map<Patient>(model);
                    domainModel.AdminLogin_Id = admin.Id;
                    domainModel.DOB = model.DOB;
                    domainModel.IsApproved = true;
                    if (model.CityName != null)
                    {
                        domainModel.CityMaster_Id = model.CityMaster_Id;
                    }
                    domainModel.PatientRegNo = PUniqId;
                    domainModel.Rwa_Id = 0;
                    domainModel.Reg_Date = DateTime.Now;
                    ent.Patients.Add(domainModel);
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
                     
                    EmailEF ef = new EmailEF()
                    {
                        EmailAddress = model.EmailId,
                        Message = msg,
                        Subject = "PS Wellness Registration Confirmation"
                    };

                    EmailOperations.SendEmainew(ef);

                    tran.Commit();
                    string msg1 = "Welcome to PSWELLNESS. Your User Name :  " + admin.Username + "(" + domainModel.PatientRegNo + "), Password : " + admin.Password + ".";
                    Message.SendSms(domainModel.MobileNumber, msg1);
                    rm.Message = "Thanks for joining us.Please check your mail for the crediantials.";
                    rm.Status = 1;
                    return Ok(rm);
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    tran.Rollback();
                    return InternalServerError(ex);
                }
            }
        } 

        [HttpPost, Route("api/SignupApi/DoctorRegistration")]
        public IHttpActionResult DoctorRegistration(DoctorRegistrationRequest model)
        {
            var rm = new ReturnMessage();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        var message = string.Join(" | ", ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage));
                        rm.Message = message;
                        rm.Status = 0;
                        return Ok(rm);
                    }
                    // check department array must contain one element;
                    // if (model.Departments.Count < 1)
                    //{
                    //   rm.Status = 0;
                    //   rm.Message = "Atleast select one department";
                    //  return Ok(rm);
                    ///  }



                    // if (ent.AdminLogins.Any(a => a.Username == model.EmailId))
                    // {
                    //    rm.Status = 0;
                    //   rm.Message = "This email id has already exists.";
                    //    return Ok(rm);
                    // }

                    // if (ent.AdminLogins.Any(a => a.PhoneNumber == model.MobileNumber))
                    // {
                    //     rm.Status = 0;
                    //    rm.Message = "This Mobile Number has already exists.";
                    //    return Ok(rm);
                    // }

                    if (ent.Doctors.Any(a => a.DoctorName == model.DoctorName && a.MobileNumber == model.MobileNumber && a.Disease == model.Disease))
                    {
                        var data = ent.Doctors.Where(a => a.DoctorName == model.DoctorName && a.MobileNumber == model.MobileNumber && a.Disease == model.Disease).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.DoctorId).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        rm.Message = "you are already registered with pswellness";
                        rm.Status = 1;
                        return Ok(rm);
                    }
                    var UniqeIdDoc = bk.GenerateDoctorId();
                    var admin = new AdminLogin
                    {
                        Username = model.EmailId,
                        PhoneNumber = model.MobileNumber,
                        Password = model.Password,
                        UserID = UniqeIdDoc,
                        Role = "doctor"
                    };
                    ent.AdminLogins.Add(admin);
                    ent.SaveChanges();
                     
                    // Licence upload
                    var licenceImg = FileOperation.UploadFileWithBase64("Images", model.LicenceImage, model.LicenceBase64, allowedExtensions);
                    if (licenceImg == "not allowed")
                    {
                        rm.Status = 0;
                        rm.Message = "Only png,jpg,jpeg files are allowed as Licence document";
                        tran.Rollback();
                        return Ok(rm);
                    }
                    model.LicenceImage = licenceImg;

                    // Pan upload
                    var PanImg = FileOperation.UploadFileWithBase64("Images", model.PanImage, model.PanImageBase64, allowedExtensions);
                    if (PanImg == "not allowed")
                    {
                        rm.Status = 0;
                        rm.Message = "Only png,jpg,jpeg files are allowed as Pan document";
                        tran.Rollback();
                        return Ok(rm);
                    }
                    model.PanImage = PanImg;

                    // Signature upload
                    var SignatureImg = FileOperation.UploadFileWithBase64("Images", model.SignaturePic, model.SignaturePicBase64, allowedExtensions);
                    if (SignatureImg == "not allowed")
                    {
                        rm.Status = 0;
                        rm.Message = "Only png,jpg,jpeg files are allowed as Licence document";
                        tran.Rollback();
                        return Ok(rm);
                    }
                    model.SignaturePic = SignatureImg;

                    var domainModel = Mapper.Map<Doctor>(model);
                    domainModel.AdminLogin_Id = admin.Id;
                    domainModel.SlotTime = Convert.ToInt32(model.SlotTiming);
                    domainModel.SlotTime2 = Convert.ToInt32(model.SlotTiming2);
                    domainModel.EndTime = model.EndTime;
                    domainModel.StartTime = model.StartTime;
                    domainModel.DoctorId = UniqeIdDoc;
                    domainModel.Disease = model.Disease;
                    domainModel.Experience = model.Experience;
                    domainModel.PAN = model.PAN;
                    domainModel.RegistrationNumber = model.RegistrationNumber;
                    domainModel.Qualification = model.Qualification; 
                    domainModel.JoiningDate = DateTime.Now;
                    domainModel.Day_Id = model.Day_Id;
                    domainModel.VirtualFee = model.VirtualFee;
                    domainModel.LicenseValidity = model.LicenseValidity;
                    domainModel.About = model.About;
                    domainModel.Vendor_Id = model.Vendor_Id;
                    domainModel.IsBankUpdateApproved = false;
                    ent.Doctors.Add(domainModel);
                    ent.SaveChanges();
                 
                    var dept = new DoctorDepartment
                    {
                        Doctor_Id = domainModel.Id,
                        Department_Id = model.Department_Id,
                        Specialist_Id = model.Specialist_Id
                    };
                    ent.DoctorDepartments.Add(dept);


                    //foreach (var dep in model.Departments)
                    //{
                    //    var Dept = new DoctorDepartment
                    //    {
                    //        Doctor_Id = domainModel.Id,
                    //        Department_Id = dep.Department_Id,
                    //        Specialist_Id = dep.Specialist_Id
                    //    };
                    //    ent.DoctorDepartments.Add(dept);
                    //}
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

                    // string msg1 = "Welcome to PS Wellness. Your signup is complete. To finalize your registration please proceed to log in using the credentials you provided during the signup process. Your User Id: " + admin.UserID + ", Password: " + admin.Password + ".";

                    EmailEF ef = new EmailEF()
                    {
                        EmailAddress = model.EmailId,
                        Message = msg,
                        Subject = "PS Wellness Registration Confirmation"
                    };

                    EmailOperations.SendEmainew(ef);

                    string msg1 = "Welcome to PSWELLNESS. Your User Name :  " + admin.Username + "(" + domainModel.DoctorId + "), Password : " + admin.Password + ".";
                    Message.SendSms(domainModel.MobileNumber, msg1);
                    tran.Commit();
                    rm.Status = 1;
                    rm.Message = "Welcome to PS Wellness. Sign up process completed. Approval pending.";
                    return Ok(rm);
                }
                 
                catch (Exception ex)
                {
                    if (ex.Message == "Invalid length for a Base-64 char array or string.")
                    {
                        rm.Message = "Invalid Attempt of Image";
                        rm.Status = 0;
                        tran.Rollback();
                        return Ok(rm);
                    }
                    log.Error(ex.Message);
                    tran.Rollback();
                    return InternalServerError(ex);
                }
            }
        }

        //Driver Registration
        public IHttpActionResult DriverRegistration(DriverRequestParameter model)
        {
            var rm = new ReturnMessage();
            string[] allowedExtensions = { ".jpg", ".png", ".jpeg" };
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                     

                    if (ent.AdminLogins.Any(a => a.Username == model.EmailId))
                    {
                        rm.Status = 0;
                        return BadRequest("This Email has already exists.");
                    }
                    if (ent.AdminLogins.Any(a => a.PhoneNumber == model.MobileNumber))
                    {
                        //rm.Message = "This Mobile Number has already exists.";
                        rm.Status = 0;
                        return BadRequest("This Mobile Number has already exists.");
                    }
                    if (ent.Drivers.Any(a => a.DriverName == model.DriverName && a.MobileNumber == model.MobileNumber))
                    {
                        var data = ent.Drivers.Where(a => a.DriverName == model.DriverName && a.MobileNumber == model.MobileNumber).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.DriverId).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        rm.Message = "you are already registered with pswellness";
                        rm.Status = 0;
                        return Ok(rm);
                    }

                    var UniuqId = Patient.GenerateDriverId();

                    var admin = new AdminLogin
                    {
                        Username = model.EmailId,
                        PhoneNumber = model.MobileNumber,
                        Password = model.Password,
                        Role = "driver"
                    };
                    ent.AdminLogins.Add(admin);
                    ent.SaveChanges();

                    if (model.DriverImageBase64 != null)
                    {
                        var dlImg = FileOperation.UploadFileWithBase64("Images", model.DriverImage, model.DriverImageBase64, allowedExtensions);
                        if (dlImg == "not allowed")
                        {
                            rm.Message = "Only png,jpg,jpeg files are allowed as DL Image.";
                            rm.Status = 0;
                            tran.Rollback();
                            return Ok(rm);
                        }
                        model.DriverImage = dlImg;
                    }

                   
                    if (model.DlImage1Base64 != null)
                    {
                        var dlImg1 = FileOperation.UploadFileWithBase64("Images", model.DlImage1, model.DlImage1Base64, allowedExtensions);
                        if (dlImg1 == "not allowed")
                        {
                            rm.Message = "Only png,jpg,jpeg files are allowed as DL Picture.";
                            rm.Status = 0;
                            tran.Rollback();
                            return Ok(rm);
                        }
                        model.DlImage1 = dlImg1;
                    }

                    if (model.DlImage2Base64 != null)
                    {
                        var dlImg2 = FileOperation.UploadFileWithBase64("Images", model.DlImage2, model.DlImage2Base64, allowedExtensions);
                        if (dlImg2 == "not allowed")
                        {
                            rm.Message = "Only png,jpg,jpeg files are allowed as Driver Picture.";
                            rm.Status = 0;
                            tran.Rollback();
                            return Ok(rm);
                        }
                        model.DlImage2 = dlImg2;
                    }

                  
                    // aadhar image upload
                    if (model.AadharImageBase64 != null)
                    {
                        var aadharImg = FileOperation.UploadFileWithBase64("Images", model.AadharImage, model.AadharImageBase64, allowedExtensions);

                        if (aadharImg == "not allowed")
                        {
                            rm.Message = "Only png,jpg,jpeg files are allowed as Aadhar Picture.";
                            rm.Status = 0;
                            tran.Rollback();
                            return Ok(rm);
                        }
                        model.AadharImage = aadharImg;
                    }


                    if (model.AadharImage2Base64 != null)
                    {
                        var aadharImg2 = FileOperation.UploadFileWithBase64("Images", model.AadharImage2, model.AadharImage2Base64, allowedExtensions);

                        if (aadharImg2 == "not allowed")
                        {
                            rm.Message = "Only png,jpg,jpeg files are allowed as Aadhar Picture.";
                            rm.Status = 0;
                            tran.Rollback();
                            return Ok(rm);
                        }
                        model.AadharImage2 = aadharImg2;
                    }

                    

                    var domainModel = Mapper.Map<Driver>(model);
                    domainModel.JoiningDate = DateTime.Now;
                    domainModel.AdminLogin_Id = admin.Id;
                    domainModel.DriverId = UniuqId;
                    domainModel.PAN = model.PAN;
                    domainModel.Vendor_Id = model.Vendor_Id;
                    domainModel.Paidamount = model.Paidamount;
                    admin.UserID = domainModel.DriverId;
                    domainModel.IsBankUpdateApproved = false;
                    ent.Drivers.Add(domainModel);
                    ent.SaveChanges();
                    rm.Status = 1;

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

                    // string msg1 = "Welcome to PS Wellness. Your signup is complete. To finalize your registration please proceed to log in using the credentials you provided during the signup process. Your User Id: " + admin.UserID + ", Password: " + admin.Password + ".";

                    EmailEF ef = new EmailEF()
                    {
                        EmailAddress = model.EmailId,
                        Message = msg,
                        Subject = "PS Wellness Registration Confirmation"
                    };

                    EmailOperations.SendEmainew(ef);

                    string msg1 = "Welcome to PSWELLNESS. Your User Name :  " + admin.Username + "(" + domainModel.DriverId + "), Password : " + admin.Password + ".";
                    Message.SendSms(domainModel.MobileNumber, msg1);
                    rm.Message = "Welcome to PS Wellness. Sign up process completed. Approval pending.";
                    tran.Commit();
                    return Ok(rm);
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Invalid length for a Base-64 char array or string.")
                    {
                        rm.Message = "Invalid Attempt of Image";
                        rm.Status = 0;
                        tran.Rollback();
                        return Ok(rm);
                    }
                    log.Error(ex.Message);
                    tran.Rollback();
                    return InternalServerError(ex);
                }
            }

        }


        [HttpPost, Route("api/SignupApi/HospitalRegistration")]
        public IHttpActionResult HospitalRegistration(HospitalRegistrationReq model)
        {
            var rm = new ReturnMessage();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        var message = string.Join(" | ",
ModelState.Values
.SelectMany(a => a.Errors)
.Select(a => a.ErrorMessage));
                        rm.Message = message;
                        rm.Status = 0;
                        return Ok(rm);
                    }
                    //if (ent.AdminLogins.Any(a => a.Username == model.EmailId))
                    //{
                    //    rm.Message = "This EmailId has already exists.";
                    //    rm.Status = 0;
                    //    return Ok(rm);
                    //}
                    //if (ent.AdminLogins.Any(a => a.PhoneNumber == model.MobileNumber))
                    //{
                    //    rm.Status = 0;
                    //    rm.Message = "This Mobile Number has already exists.";
                    //    return Ok(rm);
                    //}
                    if (ent.Hospitals.Any(a => a.HospitalName == model.HospitalName && a.PhoneNumber == model.PhoneNumber))
                    {
                        var data = ent.Hospitals.Where(a => a.HospitalName == model.HospitalName && a.PhoneNumber == model.PhoneNumber).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.HospitalId).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        rm.Message = "you are already registered with pswellness";
                        rm.Status = 1;
                        return Ok(rm);
                    }

                    var uniqHospiId = bk.GenerateHospitalId();

                    var admin = new AdminLogin
                    {
                        Username = model.EmailId,
                        PhoneNumber = model.MobileNumber,
                        Password = model.Password,
                        UserID = uniqHospiId,
                        Role = "hospital"
                    };
                    ent.AdminLogins.Add(admin);
                    ent.SaveChanges();

                    //Add City Additional CityName
                    //if (model.CityName != null)
                    //{
                    //    var city = new CityTemp
                    //    {
                    //        CityName = model.CityName,
                    //        Login_Id = admin.Id,
                    //        IsApproved = false,
                    //        State_Id = model.StateMaster_Id
                    //    };
                    //    ent.CityTemps.Add(city);
                    //    ent.SaveChanges();
                    //    model.CityMaster_Id = city.Id;
                    //}

                    // authorization letter  Section 

                    var img = FileOperation.UploadFileWithBase64("Images", model.AuthorizationLetterImageName, model.AuthorizationLetterBase64, allowedExtensions);
                    if (img == "not allowed")
                    {
                        rm.Status = 0;
                        rm.Message = "Only png,jpg,jpeg files are allowed.";
                        tran.Rollback();
                        return Ok(rm);
                    }
                    model.AuthorizationLetterImage = img;

                    var domainModel = Mapper.Map<Hospital>(model);
                    domainModel.AdminLogin_Id = admin.Id;
                    domainModel.HospitalId = uniqHospiId;
                    //if (model.CityName != null) 
                    //{
                    //    domainModel.CityMaster_Id = (int)model.CityMaster_Id;
                    //}
                    ent.Hospitals.Add(domainModel);
                    ent.SaveChanges();
                    tran.Commit();
                    rm.Status = 1;
                    string msg = "Welcome to PSWELLNESS. Your User Name :  " + admin.Username + "(" + domainModel.HospitalId + "), Password : " + admin.Password + ".";
                    Message.SendSms(domainModel.MobileNumber, msg);
                    rm.Message = "You have registered successfully.Please wait for approval to login.";
                    return Ok(rm);
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    tran.Rollback();
                    return InternalServerError(ex);
                }
            }

        }

        //Nurse Registration===========================
        public IHttpActionResult NurseRegistration(NurseRequestedParams model)
        {

            string[] allowedExtensions = { ".jpg", ".png", ".jpeg" };
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                     
                    if (ent.AdminLogins.Any(a => a.Username == model.EmailId))
                    {
                        rm.Message = "This EmailId has already exists.";
                        rm.Status = 0;
                        return Ok(rm);
                    }
                    if (ent.AdminLogins.Any(a => a.PhoneNumber == model.MobileNumber))
                    {
                        rm.Message = "This Mobile Number has already exists.";
                        rm.Status = 0;
                        return Ok(rm);
                    }

                    if (ent.Nurses.Any(a => a.NurseName == model.NurseName && a.MobileNumber == model.MobileNumber))
                    {
                        var data = ent.Nurses.Where(a => a.NurseName == model.NurseName && a.MobileNumber == model.MobileNumber).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.NurseId).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        rm.Message = "you are already registered with pswellness";
                        rm.Status = 0;
                        return Ok(rm);
                    }
                    var HUniquId = bk.GenerateNurseId();

                    var admin = new AdminLogin
                    {
                        Username = model.EmailId,
                        PhoneNumber = model.MobileNumber,
                        Password = model.Password,
                        UserID = HUniquId,
                        Role = "nurse"
                    };
                    ent.AdminLogins.Add(admin);
                    ent.SaveChanges();

                     

                    // CertificateFile Picture Section 
                    var cetificateImg = FileOperation.UploadFileWithBase64("Images", model.CertificateImage, model.CertificateBase64Image, allowedExtensions);
                    if (cetificateImg == "not allowed")
                    {
                        rm.Message = "Only png,jpg,jpeg files are allowed.";
                        tran.Rollback();
                        return Ok(rm);
                    }
                    model.CertificateImage = cetificateImg;
                    var panImg = FileOperation.UploadFileWithBase64("Images", model.PanImage, model.PanBase64Image, allowedExtensions);
                    if (panImg == "not allowed")
                    {
                        rm.Message = "Only png,jpg,jpeg files are allowed.";
                        tran.Rollback();
                        return Ok(rm);
                    }
                    model.PanImage = panImg;
                    var nurseImg = FileOperation.UploadFileWithBase64("Images", model.NurseImage, model.NurseImageBase64Image, allowedExtensions);
                    if (nurseImg == "not allowed")
                    {
                        rm.Message = "Only png,jpg,jpeg files are allowed.";
                        tran.Rollback();
                        return Ok(rm);
                    }
                    model.NurseImage = nurseImg;

                    var domainModel = new Nurse();
                    {
                        domainModel.NurseName = model.NurseName;
                        domainModel.EmailId = model.EmailId;
                        domainModel.Password = model.Password;
                        domainModel.MobileNumber = model.MobileNumber; 
                        domainModel.StateMaster_Id = model.StateMaster_Id;
                        domainModel.CityMaster_Id = model.CityMaster_Id;
                        domainModel.Location = model.Location;
                        domainModel.CertificateImage = model.CertificateImage;
                        domainModel.NurseImage = model.NurseImage;
                        domainModel.PanImage = model.PanImage;
                        domainModel.CertificateNumber = model.CertificateNumber;
                        domainModel.PinCode = model.PinCode;
                        domainModel.NurseType_Id = model.NurseType_Id;
                        domainModel.Fee = model.Fee;
                        domainModel.Location_id = model.Location_id;
                        domainModel.Vendor_Id = model.Vendor_Id;
                        domainModel.PAN = model.PAN;
                        domainModel.about = model.about;
                        domainModel.experience = model.experience;
                        domainModel.JoiningDate = DateTime.Now;
                        domainModel.AdminLogin_Id = admin.Id;
                        domainModel.NurseId = HUniquId;
                        domainModel.IsBankUpdateApproved = false; 

                    };


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

                   
                    EmailEF ef = new EmailEF()
                    {
                        EmailAddress = model.EmailId,
                        Message = msg,
                        Subject = "PS Wellness Registration Confirmation"
                    };

                    EmailOperations.SendEmainew(ef);

                    rm.Message = "Welcome to PS Wellness. Sign up process completed. Approval pending.";
                    rm.Status = 1;
                    string msg1 = "Welcome to PSWELLNESS. Your User Name :  " + admin.Username + "(" + domainModel.NurseId + "), Password : " + admin.Password + ".";
                    Message.SendSms(domainModel.MobileNumber, msg1);
                    tran.Commit();
                    return Ok(rm);

                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    rm.Message = "Server Error";
                    tran.Rollback();
                    return Ok(rm);
                }
            }
        }


        [HttpPost, Route("api/SignupApi/Login")]
        public IHttpActionResult Login(LoginModel model)
        {
            dynamic obj = new ExpandoObject();

            var login = ent.AdminLogins.FirstOrDefault(a => a.UserID == model.Username && a.Password == model.Password);
            if (login == null)
            {
                obj.Status = 0;
                obj.Message = "Invalid Username or Password";
                return Ok(obj);
            }
            string role = login.Role;
            switch (role)
            {
                case "driver":
                    var driver = ent.Drivers.FirstOrDefault(a => a.AdminLogin_Id == login.Id && !a.IsDeleted);
                    if (!driver.IsApproved)
                        goto Inactive;
                    if (driver.IsDeleted)
                        goto Delete;

                    var vehicle = ent.Vehicles.Any(a => a.Driver_Id == driver.Id);
                    if (vehicle == false)
                    {
                        obj.Status = 0;
                        obj.Message = "You haven't alloted vehilce Yet. Kindly Contact with Administrator.";
                        return Ok(obj);
                    }

                    var driverData = (from d in ent.Drivers
                                      join s in ent.StateMasters on d.StateMaster_Id equals s.Id
                                      join c in ent.CityMasters on d.CityMaster_Id equals c.Id
                                      join v in ent.Vehicles on d.Id equals v.Driver_Id
                                      join vt in ent.VehicleTypes on v.VehicleType_Id equals vt.Id
                                      where d.AdminLogin_Id == login.Id
                                      select new
                                      {
                                          d.Id,
                                          //v.Driver_Id,
                                          d.DriverName,
                                          d.EmailId,
                                          d.MobileNumber,
                                          d.DlNumber,
                                          d.DlValidity,                                          
                                          d.PAN,
                                          s.StateName,
                                          c.CityName,
                                          d.StateMaster_Id,
                                          d.CityMaster_Id,
                                          d.Location,
                                          d.IsApproved,
                                          d.AdminLogin_Id,
                                          d.Password,
                                          d.DriverId  
                                      }).FirstOrDefault();
                    if (driverData != null)
                    {
                        obj.role = "driver";
                        obj.data = driverData;
                        obj.Message = "Successfully Logged In";
                        obj.Status = 1;
                    }
                    else
                    {
                        obj.role = "driver";
                        obj.Message = "Successfully Logged In";
                        obj.Status = 1;
                    }
                    return Ok(obj);
                case "nurse":
                    var nurse = ent.Nurses.FirstOrDefault(a => a.AdminLogin_Id == login.Id && !a.IsDeleted);
                    if (!nurse.IsApproved)
                        goto Inactive;
                    if (nurse.IsDeleted)
                        goto Delete;

                    var nurseData = (from d in ent.Nurses
                                     join s in ent.StateMasters on d.StateMaster_Id equals s.Id
                                     join c in ent.CityMasters on d.CityMaster_Id equals c.Id
                                     join v in ent.NurseTypes on d.NurseType_Id equals v.Id
                                     where d.AdminLogin_Id == login.Id
                                     select new
                                     {
                                         d.Id,
                                         d.EmailId,
                                         d.MobileNumber,
                                         d.NurseName,
                                         d.PinCode,
                                         v.NurseTypeName,
                                         d.PAN,
                                         s.StateName,
                                         d.IsApproved,
                                         c.CityName,
                                         d.StateMaster_Id,
                                         d.CityMaster_Id,
                                         d.Location,
                                         d.AdminLogin_Id,
                                         d.Password,
                                         d.NurseId,
                                         d.IsVerifiedByPolice,
                                         d.NurseType_Id
                                     }).FirstOrDefault();
                    obj.role = "nurse";
                    obj.data = nurseData;

                    obj.Message = "Successfully Logged In";
                    obj.Status = 1;
                    return Ok(obj);

                //==========================New Code=========================================================

                case "RWA":
                    var RWA = ent.RWAs.FirstOrDefault(a => a.AdminLogin_Id == login.Id && !a.IsDeleted);
                    if (!RWA.IsApproved)
                        goto Inactive;
                    if (RWA.IsDeleted)
                        goto Delete;

                    var RWAData = (from d in ent.RWAs
                                   join s in ent.StateMasters on d.StateMaster_Id equals s.Id
                                   join c in ent.CityMasters on d.CityMaster_Id equals c.Id
                                   where d.AdminLogin_Id == login.Id
                                   select new
                                   {

                                       d.Id,
                                       d.AuthorityName,
                                       d.EmailId,
                                       d.MobileNumber,
                                       s.StateName,
                                       c.CityName,
                                       d.StateMaster_Id,
                                       d.CityMaster_Id,
                                       d.Location,
                                       d.Pincode,
                                       d.IsApproved,
                                       d.AdminLogin_Id,
                                       d.Password,
                                       d.RWAId,

                                   }).FirstOrDefault();
                    if (RWAData != null)
                    {
                        obj.role = "RWA";
                        obj.data = RWAData;
                        obj.Message = "Successfully Logged In";
                        obj.Status = 1;
                    }
                    else
                    {
                        obj.role = "RWA";
                        obj.Message = "Successfully Logged In";
                        obj.Status = 1;
                    }
                    return Ok(obj);

                case "chemist":
                    var chemist = ent.Chemists.FirstOrDefault(a => a.AdminLogin_Id == login.Id && !a.IsDeleted);

                    if (!chemist.IsApproved)
                        goto Inactive;
                    if (chemist.IsDeleted)
                        goto Delete;


                    var chemistData = (from d in ent.Chemists
                                       join s in ent.StateMasters on d.StateMaster_Id equals s.Id
                                       join c in ent.CityMasters on d.CityMaster_Id equals c.Id
                                       where d.AdminLogin_Id == login.Id
                                       select new
                                       {

                                           d.Id,
                                           d.ChemistName,
                                           d.EmailId,
                                           d.MobileNumber,
                                           s.StateName,
                                           c.CityName,
                                           d.StateMaster_Id,
                                           d.CityMaster_Id,
                                           d.Location,
                                           d.IsApproved,
                                           d.AdminLogin_Id,
                                           d.Password,
                                           d.ChemistId,
                                       }).FirstOrDefault();
                    if (chemistData != null)
                    {
                        obj.role = "chemist";
                        obj.data = chemistData;
                        obj.Message = "Successfully Logged In";
                        obj.Status = 1;
                    }
                    else
                    {
                        obj.role = "chemist";
                        obj.Message = "Successfully Logged In";
                        obj.Status = 1;
                    }
                    return Ok(obj);

                case "Franchise":
                    var Franchise = ent.Vendors.FirstOrDefault(a => a.AdminLogin_Id == login.Id && !a.IsDeleted);
                    if (!Franchise.IsApproved)
                        goto Inactive;
                    if (Franchise.IsDeleted)
                        goto Delete;

                    var FranchiseData = (from d in ent.Vendors
                                         join s in ent.StateMasters on d.StateMaster_Id equals s.Id
                                         join c in ent.CityMasters on d.City_Id equals c.Id
                                         where d.AdminLogin_Id == login.Id
                                         select new
                                         {
                                             d.Id,
                                             d.CompanyName,
                                             d.EmailId,
                                             d.MobileNumber,
                                             s.StateName,
                                             c.CityName,
                                             d.StateMaster_Id,
                                             d.City_Id,
                                             d.Location,
                                             d.IsApproved,
                                             d.AdminLogin_Id,
                                             d.Password,
                                             d.UniqueId,

                                         }).FirstOrDefault();
                    if (FranchiseData != null)
                    {
                        obj.role = "Franchise";
                        obj.data = FranchiseData;
                        obj.Message = "Successfully Logged In";
                        obj.Status = 1;
                    }
                    else
                    {
                        obj.role = "Franchise";
                        obj.Message = "Successfully Logged In";
                        obj.Status = 1;
                    }
                    return Ok(obj);

                case "lab":
                    var lab = ent.Labs.FirstOrDefault(a => a.AdminLogin_Id == login.Id && !a.IsDeleted);
                    if (!lab.IsApproved)
                        goto Inactive;
                    if (lab.IsDeleted)
                        goto Delete;

                    var labData = (from d in ent.Labs
                                   join s in ent.StateMasters on d.StateMaster_Id equals s.Id
                                   join c in ent.CityMasters on d.CityMaster_Id equals c.Id 
                                   where d.AdminLogin_Id == login.Id
                                   select new
                                   {
                                       d.Id, 
                                       d.EmailId,
                                       d.MobileNumber, 
                                       s.StateName,
                                       c.CityName,
                                       d.StateMaster_Id,
                                       d.CityMaster_Id,
                                       d.Location,
                                       d.IsApproved,
                                       d.AdminLogin_Id,
                                       d.Password, 
                                       d.lABId, 
                                   }).FirstOrDefault();
                    if (labData != null)
                    {
                        obj.role = "lab";
                        obj.data = labData;
                        obj.Message = "Successfully Logged In";
                        obj.Status = 1;
                    }
                    else
                    {
                        obj.role = "lab";
                        obj.Message = "Successfully Logged In";
                        obj.Status = 1;
                    }
                    return Ok(obj);

                case "doctor":
                    var doctor = ent.Doctors.FirstOrDefault(a => a.AdminLogin_Id == login.Id);
                    if (!doctor.IsApproved)
                        goto Inactive;
                    if (doctor.IsDeleted)
                        goto Delete;
                    var doctorData = (from d in ent.Doctors
                                      join s in ent.StateMasters on d.StateMaster_Id equals s.Id
                                      join c in ent.CityMasters on d.CityMaster_Id equals c.Id
                                      //join dpt in ent.Departments on d.Department_Id equals dpt.Id
                                      //join spl in ent.Specialists on d.Specialist_Id equals spl.Id
                                      where d.AdminLogin_Id == login.Id
                                      select new DoctorLoginResponse
                                      {
                                          Id = d.Id,
                                          EmailId = d.EmailId,
                                          MobileNumber = d.MobileNumber,
                                          ClinicName = d.ClinicName,
                                          DoctorName = d.DoctorName,
                                          LicenceNumber = d.LicenceNumber,
                                          Location = d.Location,
                                          PAN = d.PAN,
                                          IsApproved = d.IsApproved,
                                          //dpt.DepartmentName,
                                          ////spl.SpecialistName,
                                          //d.Department_Id,
                                          //d.Specialist_Id,
                                          StateMaster_Id = d.StateMaster_Id,
                                          CityMaster_Id = d.CityMaster_Id,
                                          StateName = s.StateName,
                                          CityName = c.CityName,
                                          Fee = d.Fee,
                                          AdminLogin_Id = d.AdminLogin_Id,
                                          Password = d.Password,
                                          SlotTime = d.SlotTime,
                                          StartTime = d.StartTime,
                                          EndTime = d.EndTime,
                                          PinCode = d.PinCode,
                                          DoctorId = d.DoctorId
                                      }).FirstOrDefault(); 

                    var skills = ent.DoctorSkills.Where(a => a.Doctor_Id == doctorData.Id).ToList().Select(a => a.SkillName);
                    foreach (var d in skills)
                    {
                        if (skills.Count() == 0)
                        {
                            obj.Skills = "NA";
                        }
                        else
                        {
                            obj.Skills = string.Join(",", skills);
                        }
                    };
                    obj.role = "doctor";
                    obj.data = doctorData;
                    obj.Message = "Successfully Logged In";
                    obj.Status = 1;
                    return Ok(obj);

                case "hospital":
                    var hospital = ent.Hospitals.FirstOrDefault(a => a.AdminLogin_Id == login.Id && !a.IsDeleted);
                    if (!hospital.IsApproved)
                        goto Inactive;
                    var hospitalData = (from d in ent.Hospitals
                                        join s in ent.StateMasters on d.StateMaster_Id equals s.Id
                                        join c in ent.CityMasters on d.CityMaster_Id equals c.Id
                                        //join l in ent.Locations on d.Location_Id equals l.Id
                                        where d.AdminLogin_Id == login.Id
                                        select new
                                        {
                                            d.PinCode,
                                            d.Id,
                                            d.EmailId,
                                            d.MobileNumber,
                                            d.HospitalName,
                                            d.Location,
                                            d.Location_Id,
                                            d.PhoneNumber,
                                            //l.LocationName,
                                            d.IsApproved,
                                            s.StateName,
                                            c.CityName,
                                            d.StateMaster_Id,
                                            d.CityMaster_Id,
                                            d.AdminLogin_Id,
                                            d.Password,
                                            d.HospitalId
                                        }).FirstOrDefault();
                    obj.role = "hospital";
                    obj.data = hospitalData;
                    obj.Message = "Successfully Logged In";
                    obj.Status = 1;
                    return Ok(obj);
                case "patient":
                    var login1 = ent.AdminLogins.FirstOrDefault(a => a.UserID == model.Username || a.PhoneNumber == model.PhoneNumber && a.Password == model.Password);
                    if (login1 == null)
                    {
                        obj.Status = 0;
                        obj.Message = "Invalid Username or Password";
                        return Ok(obj);
                    }
                    var patient = ent.Patients.FirstOrDefault(a => a.AdminLogin_Id == login.Id && !a.IsDeleted);
                    //if (!patient.IsApproved)
                    //    goto Inactive;
                    var patientData = (from d in ent.Patients
                                       join s in ent.StateMasters on d.StateMaster_Id equals s.Id
                                       join c in ent.CityMasters on d.CityMaster_Id equals c.Id
                                       where d.AdminLogin_Id == login.Id
                                       select new
                                       {
                                           d.Id,
                                           d.EmailId,
                                           d.MobileNumber,
                                           d.Location,
                                           d.PatientName,
                                           s.StateName,
                                           c.CityName,
                                           d.IsApproved,
                                           d.StateMaster_Id,
                                           d.CityMaster_Id,
                                           d.AdminLogin_Id,
                                           login.Password,
                                           d.PinCode,
                                           d.PatientRegNo
                                       }).FirstOrDefault();

                    obj.role = "patient";
                    obj.data = patientData;
                    obj.Message = "Successfully Logged In";
                    obj.Status = 1;
                    return Ok(obj);
                default:
                    obj.Status = 0;
                    obj.Message = "Invalid role of user.";
                    return Ok(obj);
            }
        Inactive:
            {
                obj.Status = 0;
                obj.Message = "Your account is not approved by admin yet";
                return Ok(obj);
            }
        Delete:
            {
                obj.Status = 0;
                obj.Message = "Your account is not available";
                return Ok(obj);
            }
        }
    
        [HttpPost]

        public IHttpActionResult ChangePassword(ChangePasswordModel model)
        {
            var rm = new ReturnMessage();
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ",
ModelState.Values
.SelectMany(a => a.Errors)
.Select(a => a.ErrorMessage));
                    rm.Message = message;
                    rm.Status = 0;
                    return Ok(rm);
                }
                if (model.Password != model.ConfirmPassword)
                {
                    rm.Status = 0;
                    rm.Message = "Password and comfirm password does not match.";
                    return Ok(rm);
                }
                var record = ent.AdminLogins.Find(model.Id);
                if (record != null)
                {
                    record.Password = model.Password;
                    ent.SaveChanges();
                    rm.Status = 1;
                    rm.Message = "Password has updated successfully.";
                    return Ok(rm);
                }
                rm.Status = 0;
                rm.Message = "Passing invalid User Id";
                return Ok(rm);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + " and inner exception : " + ex.InnerException);
                return InternalServerError(ex);
            }

        }

        [HttpGet]
        public IHttpActionResult BankDetails(int Login_Id)
        {
            var model = new GetBankDetails();
            var data = ent.BankDetails.Where(a => a.Login_Id == Login_Id).ToList();
            if (data.Count() != 0)
            {
                model.AccountNo = data.FirstOrDefault().AccountNo;
                model.BranchAddress = data.FirstOrDefault().BranchAddress;
                model.BranchName = data.FirstOrDefault().BranchName;
                model.IFSCCode = data.FirstOrDefault().IFSCCode;
                //model.CancelCheque = data.FirstOrDefault().CancelCheque;
                model.Id = data.FirstOrDefault().Id;
                model.Login_Id = data.FirstOrDefault().Login_Id;
                model.isverified = data.FirstOrDefault().isverified;
                model.HolderName = data.FirstOrDefault().HolderName;
                model.MobileNumber = data.FirstOrDefault().MobileNumber;
                model.Status = 1;
                model.Message = "Success";
                return Ok(model);
            }
            else
            {
                rm.Message = "No Records";
                rm.Status = 0;
                return Ok(rm);
            }
        }
        //Update bank api========================================================= /////////////////////////////////////
        [HttpPost]
        public IHttpActionResult UpdateBank(UpdateBankDetails model)
        { 
            
            var data = ent.BankDetails.Where(a => a.Login_Id == model.Login_Id).ToList();
            
            var domain = Mapper.Map<BankDetail>(model);
           
            if (data.Count() == 0)
            {
                domain.isverified = true;
                ent.BankDetails.Add(domain);
                ent.SaveChanges();
                rm.Message = "Successfully Inserted";
                rm.Status = 1;
            }
            else
            {
                string q = @"update BankDetails set AccountNo='" + model.AccountNo + "',BranchAddress ='" + model.BranchAddress + "',BranchName='" + model.BranchName + "', IFSCCode='" + model.IFSCCode + "', HolderName='" + model.HolderName + "', MobileNumber='" + model.MobileNumber + "', isverified=1 where Login_Id=" + model.Login_Id + "";
                ent.Database.ExecuteSqlCommand(q);
                rm.Message = "Successfully Updated";
                rm.Status = 1;
            }
            return Ok(rm);
        }

       
        //Banner  Get Api=============[Anchal shukla 21/02/22]=================//

        [HttpGet, Route("api/SignupApi/getBanner")]
        public IHttpActionResult getBanner(int id)
        {
            try
            {
                var BannerImageList = ent.Banners.Where(x => x.pro_id == id).Select(x => new BannerAddDTO()
                {
                    BannerPath = x.BannerPath,
                    pro_id = id
                }).ToList();

                if (BannerImageList != null)
                {
                    return Ok(new { BannerImageList, status = 200, message = "Banner Image" });
                }
                else
                {
                    return BadRequest("No Banner Image ");
                }
            }
            catch
            {
                return BadRequest("Server Error");
            }
        }

        public IHttpActionResult AddCityLocation(AddCityLocationReq model)
        {
            var rm = new ReturnMessage();
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ",
ModelState.Values
.SelectMany(a => a.Errors)
.Select(a => a.ErrorMessage));
                rm.Message = message;
                rm.Status = 0;
                return Ok(rm);
            }
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (model.CityId == 0 && string.IsNullOrEmpty(model.CityName))
                    {
                        rm.Status = 0;
                        rm.Message = "City name is necessary.";
                        return Ok(rm);
                    }
                    // if user want to add city and city
                    if (model.CityId == 0)
                    {
                        var cm = new CityMaster
                        {
                            StateMaster_Id = (int)model.StateId,
                            CityName = model.CityName
                        };
                        ent.CityMasters.Add(cm);
                        ent.SaveChanges();
                        model.CityId = cm.Id;
                    }

                    // add location
                    var loc = new Location { City_Id = (int)model.CityId, LocationName = model.LocationName };
                    ent.Locations.Add(loc);
                    ent.SaveChanges();
                    rm.Status = 1;
                    rm.Message = "City and location has added successfully";
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    rm.Status = 0;
                    rm.Message = "Server error";
                    tran.Rollback();
                }
                return Ok(rm);
            }
        }

        //RWA SignUp========
        [HttpPost, Route("api/SignupApi/SignUpRWA")]
        public IHttpActionResult SignUpRWA(RWA_Registration model)
        {
            var rm = new ReturnMessage();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {

                    if (ent.AdminLogins.Any(a => a.Username == model.EmailId))
                    {
                        rm.Message = "This EmailId has already exists.";
                        rm.Status = 0;
                        return Ok(rm);
                    }
                    if (ent.AdminLogins.Any(a => a.PhoneNumber == model.PhoneNumber))
                    {
                        rm.Status = 0;
                        rm.Message = "This Mobile Number has already exists.";
                        return Ok(rm);
                    }

                    if (ent.Hospitals.Any(a => a.HospitalName == model.AuthorityName && a.PhoneNumber == model.PhoneNumber))
                    {
                        var data = ent.Hospitals.Where(a => a.HospitalName == model.AuthorityName && a.PhoneNumber == model.PhoneNumber).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.HospitalId).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        rm.Message = "you are already registered with pswellness";
                        rm.Status = 1;
                        return Ok(rm);
                    }


                    var admin = new AdminLogin
                    {
                        Username = model.EmailId,
                        PhoneNumber = model.PhoneNumber,
                        Password = model.Password,
                        Role = "RWA"
                    };
                    ent.AdminLogins.Add(admin);
                    ent.SaveChanges();


                    var img = FileOperation.UploadFileWithBase64("Images", model.CertificateImage, model.CertificateImagebase64, allowedExtensions);
                    if (img == "not allowed")
                    {
                        rm.Status = 0;
                        rm.Message = "Only png,jpg,jpeg files are allowed.";
                        tran.Rollback();
                        return Ok(rm);
                    }
                    model.CertificateImage = img;

                    var domainModel = new RWA();
                    {
                        domainModel.AdminLogin_Id = admin.Id;
                        domainModel.AuthorityName = model.AuthorityName;
                        domainModel.PhoneNumber = model.PhoneNumber;
                        domainModel.MobileNumber = model.MobileNumber;
                        domainModel.EmailId = model.EmailId;
                        domainModel.Password = model.Password;
                        domainModel.CertificateNo = model.CertificateNo;
                        domainModel.CertificateImage = model.CertificateImage;
                        domainModel.StateMaster_Id = model.StateMaster_Id;
                        domainModel.CityMaster_Id = model.CityMaster_Id;
                        domainModel.Location = model.Location;
                        domainModel.LandlineNumber = model.LandlineNumber;
                        domainModel.JoiningDate = DateTime.Now;
                        domainModel.Pincode = model.Pincode;
                        domainModel.PAN = model.PAN;
                        domainModel.IsBankUpdateApproved = false; 
                        domainModel.RWAId = bk.GenerateRWA_Id();
                        admin.UserID = domainModel.RWAId;
                    };


                    ent.RWAs.Add(domainModel);
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

                    // string msg1 = "Welcome to PS Wellness. Your signup is complete. To finalize your registration please proceed to log in using the credentials you provided during the signup process. Your User Id: " + admin.UserID + ", Password: " + admin.Password + ".";

                    EmailEF ef = new EmailEF()
                    {
                        EmailAddress = model.EmailId,
                        Message = msg,
                        Subject = "PS Wellness Registration Confirmation"
                    };

                    EmailOperations.SendEmainew(ef);

                    tran.Commit();
                    rm.Status = 1;
                    string msg1 = "Welcome to PSWELLNESS. Your User Name :  " + admin.Username + "(" + domainModel.Id + "), Password : " + admin.Password + ".";
                    Message.SendSms(domainModel.PhoneNumber, msg1);
                    rm.Message = "Welcome to PS Wellness. Sign up process completed. Approval pending.";
                    return Ok(rm);
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    tran.Rollback();
                    return InternalServerError(ex);
                }
            }
        }
        //============================LAB REGISTRATION=================== //
        [HttpPost, Route("api/SignupApi/LabRegistration")]
        public IHttpActionResult LabRegistration(LabREgis model)
        {
            var rm = new ReturnMessage();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            //using (var tran = ent.Database.BeginTransaction())
            //{
            try
            {
                if (ent.AdminLogins.Any(a => a.Username == model.EmailId))
                {
                    rm.Message = "This EmailId has already exists.";
                    rm.Status = 1;
                    return Ok(rm);
                }
                if (ent.AdminLogins.Any(a => a.PhoneNumber == model.PhoneNumber))
                {
                    rm.Status = 1;
                    rm.Message = "This Mobile Number has already exists.";
                    return Ok(rm);
                }

                if (ent.Labs.Any(L => L.LabName == model.LabName && L.MobileNumber == model.MobileNumber))
                {
                    var data = ent.Labs.Where(L => L.LabName == model.LabName && L.MobileNumber == model.MobileNumber).FirstOrDefault();
                    var logdata = ent.AdminLogins.Where(L => L.UserID == data.lABId).FirstOrDefault();
                    string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                    Message.SendSms(logdata.PhoneNumber, mssg);
                    rm.Message = "you are already Lab  registered with pswellness";
                    rm.Status = 1;
                    return Ok(rm);
                }

                var admin = new AdminLogin
                {
                    Username = model.EmailId,
                    PhoneNumber = model.PhoneNumber,
                    Password = model.Password,
                    Role = "lab"
                };
                ent.AdminLogins.Add(admin);
                ent.SaveChanges();
                 
                var img = FileOperation.UploadFileWithBase64("Images", model.LicenceImage, model.LicenceImagebase64, allowedExtensions);
                if (img == "not allowed")
                {
                    rm.Message = "Only png,jpg,jpeg files are allowed.";
                     
                    return Ok(rm);
                }
                model.LicenceImage = img;
                
                var PanImg = FileOperation.UploadFileWithBase64("Images", model.PanImage, model.PanImagebase64, allowedExtensions);
                if (PanImg == "not allowed")
                {
                    rm.Message = "Only png,jpg,jpeg files are allowed as Aadhar card document";
                     
                    return Ok(rm);
                }
                model.PanImage = PanImg;


                var domainModel = new Lab();
                {
                    // var domainModel = Mapper.Map<Lab>(model);
                    domainModel.LabName = model.LabName;
                    domainModel.EmailId = model.EmailId;
                    domainModel.Password = model.Password;
                    domainModel.MobileNumber = model.MobileNumber;
                    domainModel.PhoneNumber = model.PhoneNumber;
                    domainModel.Location = model.Location;
                    domainModel.StateMaster_Id = model.StateMaster_Id;
                    domainModel.CityMaster_Id = model.CityMaster_Id;
                    domainModel.LicenceNumber = model.LicenceNumber;
                    domainModel.LicenceImage = model.LicenceImage;
                    domainModel.PanImage = model.PanImage;
                    domainModel.PAN = model.PAN; 
                    domainModel.StartTime = model.StartTime;
                    domainModel.EndTime = model.EndTime;
                    domainModel.GSTNumber = model.GSTNumber;
                    domainModel.AadharNumber = model.AadharNumber;
                    domainModel.PinCode = model.PinCode;
                    domainModel.JoiningDate = DateTime.Now;
                    domainModel.AdminLogin_Id = admin.Id;
                    domainModel.IsBankUpdateApproved = false;
                    domainModel.Vendor_Id = model.Vendor_Id;
                    domainModel.lABId = bk.GenerateLabId();

                    admin.UserID = domainModel.lABId;
                };
                ent.Labs.Add(domainModel);
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

               
                EmailEF ef = new EmailEF()
                {
                    EmailAddress = model.EmailId,
                    Message = msg,
                    Subject = "PS Wellness Registration Confirmation"
                };

                EmailOperations.SendEmainew(ef);

                string msg1 = "Welcome to PSWELLNESS. Your User Name :  " + domainModel.EmailId + "(" + domainModel.lABId + "), Password : " + admin.Password + ".";
                Message.SendSms(domainModel.MobileNumber, msg1);
                rm.Message = "Welcome to PS Wellness. Sign up process completed. Approval pending.";
                rm.Status = 1;
                return Ok(rm);

            }

            catch (Exception ex)
            {
                log.Error(ex.Message); 
                return InternalServerError(ex);
            } 

        }
       
        //============================CHEMIST REGISTRATION=================== //
        [HttpPost, Route("api/SignupApi/chemistRegistration")]
        public IHttpActionResult chemistRegistration(Chemistregistration model)
        {
            var rm = new ReturnMessage();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            //using (var tran = ent.Database.BeginTransaction())
            //{
            try
            {
                if (ent.AdminLogins.Any(a => a.Username == model.EmailId))
                {
                    rm.Message = "This EmailId has already exists.";
                    rm.Status = 0;
                    return Ok(rm);
                }
                if (ent.AdminLogins.Any(a => a.PhoneNumber == model.MobileNumber))
                {
                    rm.Status = 1;
                    rm.Message = "This Mobile Number has already exists.";
                    return Ok(rm);
                }

                if (ent.Chemists.Any(L => L.ChemistName == model.ChemistName && L.MobileNumber == model.MobileNumber))
                {
                    var data = ent.Chemists.Where(L => L.ChemistName == model.ChemistName && L.MobileNumber == model.MobileNumber).FirstOrDefault();
                    var logdata = ent.AdminLogins.Where(L => L.UserID == data.ChemistId).FirstOrDefault();
                    string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                    Message.SendSms(logdata.PhoneNumber, mssg);
                    rm.Message = "you are already Lab  registered with pswellness";
                    rm.Status = 1;
                    return Ok(rm);
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

                var img = FileOperation.UploadFileWithBase64("Images", model.Certificateimg, model.Certificateimgbase64, allowedExtensions);
                if (img == "not allowed")
                {
                    rm.Message = "Only png,jpg,jpeg files are allowed.";
                    //tran.Rollback();
                    return Ok(rm);
                }
                model.Certificateimg = img;


                var domainModel = new Chemist();
                {
                    // var domainModel = Mapper.Map<Lab>(model);
                    domainModel.ChemistName = model.ChemistName;
                    domainModel.ShopName = model.ShopName;
                    domainModel.EmailId = model.EmailId;
                    domainModel.Password = model.Password;
                    domainModel.ConfirmPassword = model.ConfirmPassword;
                    domainModel.MobileNumber = model.MobileNumber;
                    domainModel.Location = model.Location;
                    domainModel.StateMaster_Id = model.StateMaster_Id;
                    domainModel.CityMaster_Id = model.CityMaster_Id;
                    domainModel.Vendor_Id = model.Vendor_Id;
                    domainModel.LicenceNumber = model.LicenceNumber;
                    domainModel.PAN = model.PAN;
                    domainModel.Certificateimg = model.Certificateimg;
                    domainModel.LicenseValidity = model.LicenseValidity;
                    domainModel.PinCode = model.PinCode;
                    domainModel.IsBankUpdateApproved = false;
                    domainModel.AdminLogin_Id = admin.Id;
                    domainModel.ChemistId = bk.GenerateChemistId();

                    admin.UserID = domainModel.ChemistId;
                };
                ent.Chemists.Add(domainModel);
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

                // string msg1 = "Welcome to PS Wellness. Your signup is complete. To finalize your registration please proceed to log in using the credentials you provided during the signup process. Your User Id: " + admin.UserID + ", Password: " + admin.Password + ".";

                EmailEF ef = new EmailEF()
                {
                    EmailAddress = model.EmailId,
                    Message = msg,
                    Subject = "PS Wellness Registration Confirmation"
                };

                EmailOperations.SendEmainew(ef);

                string msg1 = "Welcome to PSWELLNESS. Your User Name :  " + domainModel.EmailId + "(" + domainModel.ChemistId + "), Password : " + admin.Password + ".";
                Message.SendSms(domainModel.MobileNumber, msg1);
                rm.Message = "Welcome to PS Wellness. Sign up process completed. Approval pending.";
                rm.Status = 1;
                return Ok(rm);

            }

            catch (Exception ex)
            {
                log.Error(ex.Message);
                //tran.Rollback();
                return InternalServerError(ex);
            }
        }

        //============================FRANCHISE REGISTRATION=================== //

        [HttpPost, Route("api/SignupApi/FranchiseRegistration")]
        public IHttpActionResult FranchiseRegistration(Franchises_Reg model)
        {
            var rm = new ReturnMessage();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {

                    if (ent.Vendors.Any(a => a.VendorName == model.VendorName && a.MobileNumber == model.MobileNumber))
                    {
                        var data = ent.Vendors.Where(a => a.VendorName == model.VendorName && a.MobileNumber == model.MobileNumber).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.UniqueId).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        rm.Message = "you are already registered with pswellness";
                        return Ok(rm);
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

                    if (model.AadharOrPANImage == null)
                    {
                        rm.Message = "AadharOrPANImage Image File Picture can not be null";
                        tran.Rollback();
                        return Ok(rm);
                    }
                     

                    var img = FileOperation.UploadFileWithBase64("Images", model.AadharOrPANImage, model.AadharOrPANImagebase64, allowedExtensions);
                    if (img == "not allowed")
                    {
                        rm.Message = "Only png,jpg,jpeg files are allowed.";
                        //tran.Rollback();
                        return Ok(rm);
                    }
                    model.AadharOrPANImage = img;

                    //var domainModel = Mapper.Map<Vendor>(model);
                    var domainModel = new Vendor();
                    domainModel.VendorName = model.VendorName;
                    domainModel.CompanyName = model.CompanyName;
                    domainModel.StateMaster_Id = model.StateMaster_Id;
                    domainModel.City_Id = model.City_Id;
                    domainModel.MobileNumber = model.MobileNumber;
                    domainModel.Location = model.Location;
                    domainModel.EmailId = model.EmailId;
                    domainModel.Password = model.Password;
                    domainModel.PinCode = model.PinCode;
                    domainModel.GSTNumber = model.GSTNumber;
                    domainModel.PanNumber = model.PanNumber;
                    domainModel.AadharOrPANNumber = model.AadharOrPANNumber;
                    domainModel.PanImage = model.AadharOrPANImage;
                    domainModel.AdminLogin_Id = admin.Id;
                    domainModel.IsBankUpdateApproved = false;
                    domainModel.UniqueId = bk.GenerateVenderId();
                    admin.UserID = domainModel.UniqueId;
                     
                    ent.Vendors.Add(domainModel);
                    ent.SaveChanges();
                    // string msg = "Welcome to PSWELLNESS. Your User Name :  " + domainModel.EmailId + "(" + domainModel.UniqueId + ") , Password : " + admin.Password + ".";
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
                    tran.Commit();
                    rm.Status = 1;
                    rm.Message = "Welcome to PS Wellness. Sign up process completed. Approval pending.";
                    return Ok(rm);
                }
                catch (Exception ex)
                {

                    log.Error(ex.Message);
                    tran.Rollback();
                    return InternalServerError(ex);
                }

            }
        }


        //================Franchise Update Bank===========

        [HttpPost]
        [Route("api/SignupApi/Franchises_UpdateBank")]
        public IHttpActionResult Franchises_UpdateBank(FranchisesBankDetails model)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ",
ModelState.Values
.SelectMany(a => a.Errors)
.Select(a => a.ErrorMessage));
                rm.Message = message;
                rm.Status = 0;
                return Ok(rm);
            }

            var data = ent.BankDetails.Where(a => a.Login_Id == model.Login_Id).ToList();

            var domain = new BankDetail();
            domain.Login_Id = model.Login_Id;
            domain.AccountNo = model.AccountNo;
            domain.IFSCCode = model.IFSCCode;
            domain.BranchName = model.BranchName;
            domain.BranchAddress = model.BranchAddress;
            domain.HolderName = model.HolderName;
            domain.MobileNumber = model.MobileNumber;

            if (data.Count() == 0)
            {
                domain.isverified = true;
                ent.BankDetails.Add(domain);
                ent.SaveChanges();
                rm.Message = "Successfully Inserted";
                rm.Status = 1;
            }
            else
            {
                string q = @"update BankDetails set AccountNo='" + model.AccountNo + "',BranchAddress ='" + model.BranchAddress + "',BranchName='" + model.BranchName + "', IFSCCode='" + model.IFSCCode + "', HolderName='" + model.HolderName + "',isverified=1 where Login_Id=" + model.Login_Id + "";
                ent.Database.ExecuteSqlCommand(q);
                rm.Message = "Successfully Updated";
                rm.Status = 1;
            }
            return Ok(rm);
        }

        [HttpPost]
        [Route("api/SignupApi/ForgotPassword")]
        public IHttpActionResult ForgotPassword(ForgotPasswordDTO model)
        {
            var user = ent.AdminLogins.Where(a => a.Username == model.EmailId).FirstOrDefault();

            if (user == null)
            {
                return BadRequest("Please enter your valid Email Id.");    
            }

            // Generate  an OTP
            var otp = GenerateRandomOtp(); 


            if (user == null)
            {
                rm.Status = 0;
                rm.Message = "No record found";
                return Ok(rm);
            }
            user.Password = otp;
            ent.SaveChanges();

            
            EmailEF ef = new EmailEF()
            {
                EmailAddress = model.EmailId,
                Subject = "Change password", 
                Message = @"<!DOCTYPE html>
<html>
<head>
    <title>Change password request.</title>
</head>
<body> 
    
    <ul>
        <li><strong>Your new password is:</strong> " + otp + @"</li> 
    </ul>
    <p>You can now login with your new password.</p>
</body>
</html>"
            };

            EmailOperations.SendEmainew(ef); 

            return Ok("New Password sent to your Email Id.");  
        }
        private string GenerateRandomOtp()
        { 
            const string chars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 6).Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }    
}

        



    

