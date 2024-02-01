using AutoMapper;
using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static HospitalPortal.Models.ViewModels.ChemistDTO;

namespace HospitalPortal.Controllers
{
    public class ComplaintApiController : ApiController
    {
        ILog log = LogManager.GetLogger(typeof(HealthCheckUpApiController));
        DbEntities ent = new DbEntities();
        ReturnMessage rm = new ReturnMessage();
        EmailOperations mail = new EmailOperations();

        [HttpPost]
        public IHttpActionResult PatientComplaint(Complaint4Patient model)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage));
                rm.Message = message;
                rm.Status = 0;
                return Ok(rm);
            }
            else
            {
                var cmplaint = AutoMapper.Mapper.Map<PatientComplaint>(model);
                cmplaint.IsDeleted = false;
                cmplaint.IsResolved = false;
                cmplaint.Login_Id = model.Login_Id;
                cmplaint.ComplaintDate = DateTime.Now;
                ent.PatientComplaints.Add(cmplaint);
                ent.SaveChanges();
                string Role = ent.Database.SqlQuery<string>(@"select Role from AdminLogin where Role='" + cmplaint.Roles + "'").FirstOrDefault();
                string msg = "Your Request has been sent to Our Higher Authority. We will revert you in 72 Hrs.";
                switch (Role)
                {
                    case "doctor":
                        var mobile = ent.Database.SqlQuery<string>(@"select MobileNumber from Doctor join dbo.PatientComplaints pc on pc.Login_Id = Doctor.AdminLogin_Id where AdminLogin_Id=" + cmplaint.Login_Id).FirstOrDefault();
                        Message.SendSms(mobile, msg);
                        break;
                    case "hospital":
                        var hospital = ent.Database.SqlQuery<string>(@"select MobileNumber from Hospital join dbo.PatientComplaints pc on pc.Login_Id = Hospital.AdminLogin_Id where AdminLogin_Id=" + cmplaint.Login_Id).FirstOrDefault();
                        Message.SendSms(hospital, msg);
                        break;
                    case "patient":
                        var patient = ent.Database.SqlQuery<string>(@"select MobileNumber from Patient join dbo.PatientComplaints pc on pc.Login_Id = Patient.AdminLogin_Id where AdminLogin_Id=" + cmplaint.Login_Id).FirstOrDefault();
                        //var patientemail = ent.Database.SqlQuery<string>(@"select Email from Patient join dbo.PatientComplaints pc on pc.Login_Id = Hospital.AdminLogin_Id where AdminLogin_Id=" + lab.Login_Id).FirstOrDefault();
                        Message.SendSms(patient, msg);
                        //mail.SendEmail(patientemail, msg, "your Request", 'Hello');
                        break;
                    case "driver":
                        var driver = ent.Database.SqlQuery<string>(@"select MobileNumber from Driver join dbo.PatientComplaints pc on pc.Login_Id = Driver.AdminLogin_Id where AdminLogin_Id=" + cmplaint.Login_Id).FirstOrDefault();
                        Message.SendSms(driver, msg);
                        break;
                }
                rm.Message = "Successfully Forwarded to Higher Authority.";
                rm.Status = 1;
                return Ok(rm);
            }
        }

        /// <summary>
        /// Doctor complaint model ==========================================
       
        

        [HttpPost]
        public IHttpActionResult DoctorComplaint(Doctor_Complaint model)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage));
                rm.Message = message;
                rm.Status = 0;
                return Ok(rm);
            }
            else
            {
                var Doc_cmplaint = AutoMapper.Mapper.Map<DoctorComplaint>(model);
                Doc_cmplaint.IsDeleted = false;
                Doc_cmplaint.IsResolved = false;
                ent.DoctorComplaints.Add(Doc_cmplaint);
                ent.SaveChanges();
                rm.Message = "Successfully Forwarded to Higher Authority.";
                rm.Status = 1;
                return Ok(rm);
            }
        }
        //====NurseComplaints api ====================================

        [HttpPost]
        public IHttpActionResult NurseComplaints(Nurse_Complaints model)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage));
                rm.Message = message;
                rm.Status = 0;
                return Ok(rm);
            }
            else
            {
                Mapper.CreateMap<Nurse_Complaints, NurseComplaint>();
                var Nur_cmplaint = AutoMapper.Mapper.Map<NurseComplaint>(model);
                Nur_cmplaint.IsDeleted = false;
                Nur_cmplaint.IsResolved = false;
                ent.NurseComplaints.Add(Nur_cmplaint);
                ent.SaveChanges();
                rm.Message = "Successfully Forwarded to Higher Authority.";
                rm.Status = 1;
                return Ok(rm);
            }
        }


        [HttpPost]
        public IHttpActionResult DriverComplaints(DriverComplaints model)
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
                else
                {
                    Mapper.CreateMap<DriverComplaints, DriverComplaint>();
                    var Driver_cmplaint = AutoMapper.Mapper.Map<DriverComplaint>(model);
                    Driver_cmplaint.IsDeleted = false;
                    Driver_cmplaint.IsResolved = false;
                    ent.DriverComplaints.Add(Driver_cmplaint);
                    ent.SaveChanges();
                   
                }
            }
            catch (DbEntityValidationException ex)
            {
                string msg = ex.ToString();
                rm.Message = "Server Error";
                rm.Status = 0;
                return Ok(rm);

            }
            rm.Status = 1;
            rm.Message = "Successfully Forwarded to Higher Authority.";
            return Ok(rm);
        }


        [HttpPost]
        public IHttpActionResult PatientComplaints(ComplaintPatientes model)
        {
            try
            {
                var data = new PatientComplaint()
                {
                    patsubid = model.patsubid,
                    Others = model.Others,
                    Complaints = model.Complaints,
                    IsDeleted = false,
                    IsResolved = false,
                    Login_Id = model.Login_Id,
                    ComplaintDate = DateTime.Now
                };
                ent.PatientComplaints.Add(data);
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = "Complaints add Successfully";

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }
        // ===================== lab api complaint===================//
        [HttpPost]
        [Route("api/ComplaintApi/LabComplaint")]
        public IHttpActionResult LabComplaint(Lab_Comp model)
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
                else
                {
                    Mapper.CreateMap<Lab_Comp, LabComplaint>();
                    var Lab_Complaints = AutoMapper.Mapper.Map<LabComplaint>(model);
                    Lab_Complaints.IsDeleted = false;
                    Lab_Complaints.IsResolved = false;
                    ent.LabComplaints.Add(Lab_Complaints);
                    ent.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                string msg = ex.ToString();
                rm.Message = "Server Error";
                rm.Status = 0;
                return Ok(rm);

            }
            rm.Status = 1;
            rm.Message = "Successfully Forwarded to Higher Authority.";
            return Ok(rm);
        }


        // ===================== CHEMIST COMPLAINT=============================//
        [HttpPost]
        [Route("api/ComplaintApi/Chemist_Complaint")]
        public IHttpActionResult Chemist_Complaint(Chemist_Comp model)
        {
            try
            {
                var data = new chemistcomplaint()
                {
                    patsubid = model.patsubid,
                    others = model.others,
                    Complaints = model.Complaints,
                    IsDeleted = false,
                    Login_Id = model.Login_Id
                };
                ent.chemistcomplaints.Add(data);
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = "Complaints add Successfully";

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }




        //===================FranchiseComplaint====================//

        [HttpPost]
        [Route("api/ComplaintApi/Franchise_Complaint")]
        public IHttpActionResult Franchise_Complaint(Fra_Compliant model)
        {
            try
            {
                var data = new FranchiseComplaint()
                {
                    patsubid = model.patsubid,
                    Others = model.Others,
                    Complaints = model.Complaints,
                    IsDeleted = false,
                    IsResolved = false,
                    Login_Id = model.Login_Id
                };
                ent.FranchiseComplaints.Add(data);
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = "Complaints add Successfully";

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }
    }

    
}
