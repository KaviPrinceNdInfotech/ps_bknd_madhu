using AutoMapper;
using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HospitalPortal.Controllers
{
    public class HelloController : ApiController
    {
        DbEntities ent = new DbEntities();
        ILog log = LogManager.GetLogger(typeof(HelloController));
        returnMessage rm = new returnMessage();
        [HttpPost]
        public IHttpActionResult Test(DoctorRegistrationRequest model)
        {
            var rm = new ReturnMessage();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        var message = string.Join(" | ",ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage));
                        rm.Message = message;
                        rm.Status = 0;
                        return Ok(rm);
}
                    if (ent.AdminLogins.Any(a => a.Username == model.EmailId))
                    {
                        rm.Status = 0;
                        rm.Message = "This email id has already exists.";
                        return Ok(rm);
                    }

                    if (ent.AdminLogins.Any(a => a.PhoneNumber == model.MobileNumber))
                    {
                        rm.Status = 0;
                        rm.Message = "This Mobile Number has already exists.";
                        return Ok(rm);
                    }

                    var admin = new AdminLogin
                    {
                        Username = model.EmailId,
                        PhoneNumber = model.MobileNumber,
                        Password = model.Password,
                        Role = "doctor"
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
                    // aadhar doc upload

                    //var aadharImg = FileOperation.UploadFileWithBase64("Images", model.AadharImageName, model.AadharBase64, allowedExtensions);
                    //if (aadharImg == "not allowed")
                    //{
                    //    rm.Status = 0;
                    //    rm.Message = "Only png,jpg,jpeg files are allowed as Aadhar/PAN card document";
                    //    tran.Rollback();
                    //    return Ok(rm);
                    //}
                    //model.AadharImage = aadharImg;

                    // Licence upload
                    //var licenceImg = FileOperation.UploadFileWithBase64("Images", model.LicenceImage, model.LicenceBase64, allowedExtensions);
                    //if (licenceImg == "not allowed")
                    //{
                    //    rm.Status = 0;
                    //    rm.Message = "Only png,jpg,jpeg files are allowed as Licence document";
                    //    tran.Rollback();
                    //    return Ok(rm);
                    //}
                    //model.LicenceImage = licenceImg;

                    var domainModel = Mapper.Map<Doctor>(model);
                    domainModel.AdminLogin_Id = admin.Id;
                    domainModel.SlotTime =Convert.ToInt32(model.SlotTiming);
                    //if (model.CityName != null)
                    //{
                    //    domainModel.CityMaster_Id = model.CityMaster_Id;
                    //}
                    domainModel.EndTime = model.EndTime;
                    domainModel.StartTime = model.StartTime;
                    ent.Doctors.Add(domainModel);
                    ent.SaveChanges();

                    tran.Commit();

                    rm.Status = 1;
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



        [HttpPost]
        public IHttpActionResult Patient(PatientDTO model)
        {
            var rm = new ReturnMessage();
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

                    var admin = new AdminLogin
                    {
                        Username = model.EmailId,
                        PhoneNumber = model.MobileNumber,
                        Password = model.Password,
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
                    domainModel.IsApproved = true;
                    if (model.CityName != null)
                    {
                        domainModel.CityMaster_Id = model.CityMaster_Id;
                    }
                    ent.Patients.Add(domainModel);
                    ent.SaveChanges();
                    tran.Commit();
                    rm.Message = "Thanks for joining us.Please wait for approval to login";
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











    }
}
