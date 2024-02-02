using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using HospitalPortal.BL;
using HospitalPortal.Models;
using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.RequestModel;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Repositories;
using HospitalPortal.Utilities;
using log4net;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using static HospitalPortal.Utilities.EmailOperations;

namespace HospitalPortal.Controllers
{
    public class DoctorApiController : ApiController
    {
        DbEntities ent = new DbEntities();
        ILog log = LogManager.GetLogger(typeof(DoctorApiController));
        returnMessage rm = new returnMessage();
        GenerateBookingId bk = new GenerateBookingId();
        CommonRepository repos = new CommonRepository();
        public object model;

        [System.Web.Http.HttpPost]
        public IHttpActionResult UpdateProfile(DoctorUpdationRequest model)
        {

            try
            {
                var data = ent.Doctors.Where(a => a.Id == model.Id).FirstOrDefault();
                data.DoctorName = model.DoctorName;
                data.MobileNumber = model.MobileNumber;
                data.StateMaster_Id = model.StateMaster_Id;
                data.CityMaster_Id = model.CityMaster_Id;
                data.Location = model.Location;
                data.ClinicName = model.ClinicName;
                data.Fee = model.Fee;
                data.PinCode = model.PinCode;               
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = "Successfully Updated  Doctor Profile.";
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }


        //[System.Web.Http.HttpGet]
        //[System.Web.Mvc.Route("api/DoctorApi/GETDoctorProfile")]
        //public IHttpActionResult GETDoctorProfile(int ID)
        //{
        //    var rm = new Doctor();
        //    string query = @"select CityMaster.id,s.StateName,CityMaster.CityName from CityMaster
        //               left join stateMaster as s on  CityMaster.StateMaster_Id = s.Id where s.Id =" + ID + "";
        //    var data = ent.Database.SqlQuery<profile>(query).ToList();
        //    return Ok(data);
        //}




        //==========GetDoctorSkills===========05\03\2023====[Anchal Shukla]================================
        public IHttpActionResult GetDoctorSkills(int doctorId)
        {
            dynamic obj = new ExpandoObject();
            var skills = ent.DoctorSkills.Where(a => a.Doctor_Id == doctorId).Select(a => new { a.Id, a.SkillName }).ToList();
            obj.Skills = skills;
            return Ok(obj);
        }

        public IHttpActionResult AddSkill(DoctorSkillsDTO model)
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
                var skill = Mapper.Map<DoctorSkill>(model);
                ent.DoctorSkills.Add(skill);
                ent.SaveChanges();
                rm.Message = "Skill has updated successfully.";
                rm.Status = 1;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult RemoveSkill(int id)
        {
            var rm = new ReturnMessage();
            try
            {
                var data = ent.DoctorSkills.Find(id);
                ent.DoctorSkills.Remove(data);
                ent.SaveChanges();
                rm.Message = "Successfully deleted";
                rm.Status = 1;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }


        [System.Web.Http.HttpGet]
        public IHttpActionResult ShowAppointmentbyDoctor(int id, DateTime? date = null)
        {
            var model = new Appointment();
            string query = @"select Patient.Id,Patient.PatientRegNo,Patient.PatientName,Patient.MobileNumber,{ fn concat(CONVERT(varchar(15),CAST(StartSlotTime AS TIME),100),{fn concat ('-', CONVERT(varchar(15),CAST(EndSlotTime AS TIME),100))})} AS AppointedTime, dbo.PatientAppointment.AppointmentDate from Patient join dbo.PatientAppointment on Patient.Id = dbo.PatientAppointment.Patient_Id where dbo.PatientAppointment.Doctor_Id=" + id + " and dbo.PatientAppointment.IsBooked=1 and dbo.PatientAppointment.IsPaid=1 and Convert(Date,dbo.PatientAppointment.AppointmentDate)=  Convert(Date,GetDate()) order by AppointmentDate  desc";
            var data = ent.Database.SqlQuery<ViewAppointmentByDoctorVM>(query).ToList();
            //if (data.Count() == 0)
            //{
            //    model.Message = "No Records for Current Date";
            //    model.Status = 0;
            //   return Ok(rm);
            //}
            if (date != null)
            {
                data = data.Where(a => a.AppointmentDate == date).ToList();
                if (data.Count() == 0)
                {
                    model.Message = "No Records for Selected Date";
                    model.Status = 0;
                    return Ok(rm);
                }
            }
            model.Message = "Success";
            model.Status = 1;
            model.ViewAppointByDoctor = data;
            return Ok(model);
        }
        //============================appoinment history patient list api=======[30-03-2023]=========================================//
        [System.Web.Http.HttpGet]
        public IHttpActionResult DoctorPatientList(int id, string term = null)
        {
            var mdoel = new PatientListVM();
            var q = @"select patient.PatientRegNo, p.Patient_Id, patient.PatientName, CityMaster.CityName, patient.EmailId, patient.MobileNumber, StateMaster.StateName, patient.Location from dbo.PatientAppointment p JOIN patient on patient.Id = P.Patient_Id join CityMaster on CityMaster.Id = patient.CityMaster_Id join StateMaster on StateMaster.Id = patient.StateMaster_Id where Doctor_Id='" + id + "' group by patient.PatientRegNo, p.Patient_Id,patient.PatientName,patient.EmailId, patient.MobileNumber, patient.Location, CityMaster.CityName, StateMaster.StateName";
            var data = ent.Database.SqlQuery<PatientListApis>(q).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                data = data.Where(a => a.PatientName == term).ToList();
            }
            if (data.Count() == 0)
            {
                rm.Message = "No Data Present";
                rm.Status = 0;
                return Ok(rm);
            }
            mdoel.patients = data;
            return Ok(mdoel);
        }

        [System.Web.Http.HttpGet]
        [System.Web.Mvc.Route("api/DoctorApi/paymentHistory")]
        public IHttpActionResult paymentHistory(int Id, DateTime? Date = null)
        {
            var model = new PaymentHistroyForDosctor();
            string q = @"select Convert(date,AppointmentDate) as AppointmentDate, Sum(TotalFee) as Amount from PatientAppointment where AppointmentDate = GetDate() and  Doctor_Id ='" + Id + "' and IsPaid=1 group by AppointmentDate, TotalFee order by AppointmentDate desc";
            var data = ent.Database.SqlQuery<ListPayment>(q).ToList();
            model.PaymentHistory = data;
            if (Date != null)
            {
                string q1 = @"select Convert(date,AppointmentDate) as AppointmentDate, Sum(TotalFee) as Amount from PatientAppointment where AppointmentDate = '" + Date + "' and  Doctor_Id ='" + Id + "' and IsPaid=1 group by AppointmentDate, TotalFee order by AppointmentDate desc";
                var data1 = ent.Database.SqlQuery<ListPayment>(q1).ToList();
                if (data1.Count() == 0)
                {
                    rm.Message = "No Records Present";
                    rm.Status = 0;
                    return Ok(rm);
                }
                model.PaymentHistory = data1;
                return Ok(model);
            }
            if (data.Count() == 0)
            {
                rm.Message = "No Records Present";
                rm.Status = 0;
                return Ok(rm);
            }
            return Ok(model);
        }


        //[System.Web.Http.HttpGet]
        //public IHttpActionResult paymentHistory(int Id, DateTime? Date = null)
        //{
        //    var model = new PaymentHistroyForDosctor();
        //    string q = @"select Convert(date,AppointmentDate) as AppointmentDate, Sum(Amount) as Amount from PatientAppointment where  Doctor_Id ='" + Id + "' and IsPaid=1 group by AppointmentDate, Amount order by AppointmentDate desc";
        //    var data = ent.Database.SqlQuery<ListPayment>(q).ToList();
        //    model.PaymentHistory = data;
        //    if (Date != null)
        //    {
        //        string q1 = @"select Convert(date,AppointmentDate) as AppointmentDate, Sum(Amount) as Amount from PatientAppointment where AppointmentDate=" + Date + " and A Doctor_Id ='" + Id + "' and IsPaid=1 group by AppointmentDate, Amount order by AppointmentDate desc";
        //        var data1 = ent.Database.SqlQuery<ListPayment>(q1).ToList();
        //        if (data1.Count() == 0)
        //        {
        //            rm.Message = "No Records Present";
        //            rm.Status = 0;
        //            return Ok(rm);
        //        }
        //        model.PaymentHistory = data1;
        //        return Ok(model);
        //    }
        //    if (data.Count() == 0)
        //    {
        //        rm.Message = "No Records Present";
        //        rm.Status = 0;
        //        return Ok(rm);
        //    }
        //    return Ok(model);
        //}

        //[System.Web.Http.HttpGet]
        //[System.Web.Http.Route("api/DoctorApi/paymentHistory")]
        //public IHttpActionResult paymentHistory(int Id, DateTime? Date = null)
        //{
        //    var model = new PaymentHistroyForDosctor();
        //    IList<ListPayment> payment = null;
        //    string q = @"select Convert(date,AppointmentDate) as AppointmentDate, Sum(Amount) as Amount from PatientAppointment where AppointmentDate between DateAdd(DD,-7,GETDATE() ) and GETDATE()  and Doctor_Id ='" + Id + "' and IsPaid=1 group by AppointmentDate, Amount order by AppointmentDate desc";
        //    var data = ent.Database.SqlQuery<ListPayment>(q).ToList();
        //    model.PaymentHistory = data;
        //    payment = model.PaymentHistory;
        //    if (Date != null)
        //    {
        //        DateTime dateCriteria = Date.Value.AddDays(-7);
        //        string Tarikh = dateCriteria.ToString("dd/MM/yyyy");
        //        string q1 = @"select Convert(date,AppointmentDate) as AppointmentDate, Sum(Amount) as Amount from PatientAppointment where AppointmentDate between '" + dateCriteria + "' and '" + Date + "' and Doctor_Id ='" + Id + "' and IsPaid=1 group by AppointmentDate, Amount order by AppointmentDate desc";
        //        var data1 = ent.Database.SqlQuery<ListPayment>(q1).ToList();
        //        model.PaymentHistory = data1;
        //        payment = model.PaymentHistory;
        //        if (payment.Count() == 0)
        //        {
        //            return NotFound();
        //        }
        //        return Ok(payment);

        //    }
        //    if (payment.Count() == 0)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(payment);
        //}

        [System.Web.Http.HttpPost]
        public IHttpActionResult UploadReports(UplaodReportBase model)
        {
            dynamic obj = new ExpandoObject();
            var httpContext = HttpContext.Current;
            var rm = new ReturnMessage();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            var data = new DoctorReport();
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
                    if (model.Reports.Count() <= 0)
                    {
                        obj.Status = 0;
                        obj.Message = "Items length must greater than 0";
                        return Ok(obj);
                    }

                    foreach (var item in model.Reports)
                    {
                        var Img1 = FileOperation.UploadFileWithBase64("Doc_Report", item.Image1Name, item.Image1Base64, allowedExtensions);
                        if (Img1 == "not allowed")
                        {
                            rm.Status = 0;
                            rm.Message = "Only png,jpg,jpeg files are allowed as Aadhar/PAN card document";
                            tran.Rollback();
                            return Ok(rm);
                        }
                        item.Image1 = Img1;
                        data.Doctor_Id = model.Doctor_Id;
                        data.Patient_Id = model.Patient_Id;
                        data.UploadDate = DateTime.Now;
                        data.Image1 = item.Image1;
                        ent.DoctorReports.Add(data);
                        ent.SaveChanges();
                    }
                    tran.Commit();
                    rm.Message = "Uploaded Successfully";
                    rm.Status = 1;
                    return Ok(rm);
                }
                catch (Exception ex)
                {
                    rm.Message = "ERROR!";
                    rm.Status = 0;
                    return Ok(rm);
                }
            }
        }

        //public HttpResponseMessage upload()
        //{
        //        var model = new DoctorReport();
        //        var response = new HttpResponseMessage(HttpStatusCode.OK);
        //        var httpRequest = HttpContext.Current.Request;
        //        if(httpRequest.Files.Count > 0)
        //        {
        //            var files = new List<string>();
        //            foreach(string file in httpRequest.Files)
        //            {
        //                var postedFile = httpRequest.Files[file];
        //                var Patient_Id = HttpContext.Current.Request.Params["Patient_Id"];
        //                var Doctor_Id = HttpContext.Current.Request.Params["Doctor_Id"];
        //                var filePath = HttpContext.Current.Server.MapPath("~/Doc_Report/" + postedFile.FileName);
        //                postedFile.SaveAs(filePath);
        //                files.Add(filePath);
        //                model.Doctor_Id = Convert.ToInt32(Doctor_Id);
        //                model.Patient_Id = Convert.ToInt32(Patient_Id);
        //                model.Image1 = filePath;
        //                ent.DoctorReports.Add(model);
        //                ent.SaveChanges();
        //        }
        //        }
        //        return response;
        //}

        //Search Pateint Name by Doctor
        [System.Web.Http.HttpGet]
        public IHttpActionResult SearchPatient(string term)
        {
            try
            {
                term = term.ToLower();
                var patient = ent.Patients.Where(a => !a.IsDeleted && a.PatientName.ToLower().Contains(term)).ToList();
                var data = (from m in patient
                            select new
                            {
                                m.Id,
                                m.PatientName
                            }).ToList();
                dynamic obj = new ExpandoObject();
                obj.Hospitals = data;
                return Ok(obj);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.HttpGet]
        public IHttpActionResult SearchPatientbyDocId(string term, int Doc_Id)
        {
            try
            {
                dynamic obj = new ExpandoObject();
                var model = new PatientListC();
                term = term.ToLower();
                var patient = ent.Patients.Where(a => !a.IsDeleted && a.PatientRegNo.ToLower().Contains(term)).ToList();
                if (patient.Count() == 0)
                {
                    obj.Message = "No Records";
                    return Ok(obj);
                }
                var data = (from m in patient
                            join pa in ent.PatientAppointments on m.Id equals pa.Patient_Id
                            where pa.Doctor_Id == Doc_Id
                            group new { m, pa } by new
                            {
                                m.Id,
                                m.PatientName,
                                m.PatientRegNo,
                                m.MobileNumber
                            }
                            into g
                            select new PatientDetails
                            {
                                Id = g.Key.Id,
                                MobileNumber = g.Key.MobileNumber,
                                PatientName = g.Key.PatientName,
                                PatientRegNo = g.Key.PatientRegNo,
                            }).ToList();
                if (data.Count() == 0)
                {
                    obj.Message = "Invalid Attempt";
                    obj.Status = 0;
                    return Ok(obj);
                }
                else
                {
                    obj.Id = data.FirstOrDefault().Id;
                    obj.PatientRegNo = data.FirstOrDefault().PatientRegNo;
                    obj.PatientName = data.FirstOrDefault().PatientName;
                    obj.MobileNumber = data.FirstOrDefault().MobileNumber;
                }
                return Ok(obj);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return InternalServerError(ex);
            }
        }


        [System.Web.Mvc.HttpPost]
        [System.Web.Http.Route("api/DoctorApi/PatientList")]//dropdown
        public IHttpActionResult PatientList(PatientReportMasterVM model)
        {
            ModelState.Clear();
            if (model.PatientRegNo != null)
            {
                var q1 = @"select Count(p.Patient_Id) as PatientCount,p.Patient_Id as PatientId, patient.PatientName, patient.PatientRegNo, d.DoctorName, d.Id as DoctorId from 
dbo.PatientAppointment p JOIN patient on patient.Id = P.Patient_Id join Doctor d on d.Id = p.Doctor_Id 
join DoctorReports dr on dr.Doctor_id = d.Id
where patient.PatientRegNo='" + model.PatientRegNo + "' group by p.Patient_Id, patient.PatientName, d.DoctorName, d.Id,patient.PatientRegNo";
                var data1 = ent.Database.SqlQuery<PatientsList>(q1).ToList();
                if (data1.Count() == 0)
                {
                    model.Message = "No Data";
                    model.response = data1;
                    //return Ok(model);
                }
                else
                {
                    model.Message = "Success";
                    model.response = data1;
                    //return Ok(model);
                }
            }
            else
            {
                var q1 = @"select Count(p.Patient_Id) as PatientCount, p.Patient_Id as PatientId, patient.PatientName, patient.PatientRegNo, d.DoctorName, d.Id as DoctorId  from dbo.PatientAppointment p  JOIN patient on patient.Id = P.Patient_Id  join Doctor d on d.Id = p.Doctor_Id  join DoctorReports dr on dr.Doctor_id = d.Id where dr.Doctor_Id = " + model.DoctorId + " group by p.Patient_Id, patient.PatientName, d.DoctorName, d.Id, patient.PatientRegNo";
                var data1 = ent.Database.SqlQuery<PatientsList>(q1).ToList();
                if (data1.Count() == 0)
                {
                    model.Message = "No Data";
                    model.response = data1;
                    //return Ok(model);
                }
                else
                {
                    model.Message = "Success";
                    model.response = data1;

                }
            }
            return Ok(model);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/DoctorApi/SearchByPatientRegNo")]
        public IHttpActionResult SearchByPatientRegNo(PatientReportMasterVM model)
        {
            //var q = @"select Count(p.Patient_Id) as PatientCount,p.Patient_Id as PatientId, patient.PatientName, patient.PatientRegNo, d.DoctorName, d.Id as DoctorId  from dbo.PatientAppointment p  JOIN patient on patient.Id = P.Patient_Id  join Doctor d on d.Id = p.Doctor_Id  join DoctorReports dr on dr.Doctor_id = d.Id where dr.Doctor_Id=" + DoctorId + " group by p.Patient_Id, patient.PatientName, d.DoctorName, d.Id,patient.PatientRegNo";
            //var data = ent.Database.SqlQuery<PatientsList>(q).ToList();
            if (model.PatientRegNo != null)
            {
                var q1 = @"select Count(p.Patient_Id) as PatientCount,p.Patient_Id as PatientId, patient.PatientName, patient.PatientRegNo, d.DoctorName, d.Id as DoctorId from 
dbo.PatientAppointment p JOIN patient on patient.Id = P.Patient_Id join Doctor d on d.Id = p.Doctor_Id 
join DoctorReports dr on dr.Doctor_id = d.Id
where patient.PatientRegNo='" + model.PatientRegNo + "' group by p.Patient_Id, patient.PatientName, d.DoctorName, d.Id,patient.PatientRegNo";
                var data1 = ent.Database.SqlQuery<PatientsList>(q1).ToList();
                if (data1.Count() == 0)
                {
                    model.Message = "No Data";
                    model.response = data1;
                    return Ok(model);
                }
                else
                {
                    model.Message = "Success";
                    model.response = data1;
                    return Ok(model);
                }
            }
            //if (data.Count() == 0)
            //{
            //    model.Message = "No Data";
            //    model.response = data;
            //    return Ok(model);
            //}
            //model.Message = "Success";
            //model.response = data;
            return Ok(model);
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/DoctorApi/DoctorProfile")]
        public IHttpActionResult DoctorProfile(int DoctorId)
        {
            var model = new DoctorProfile();
            string q = @"select *,{ fn concat(CONVERT(varchar(15),CAST(StartTime AS TIME),100),{fn concat ('-', CONVERT(varchar(15),CAST(EndTime AS TIME),100))})} AS AvailableTime
from Doctor d
left join StateMaster sm on sm.Id =  d.StateMaster_Id
left join CityMaster cm on cm.Id =  d.CityMaster_Id
left join Department dm on dm.Id = d.Department_Id
left join Specialist sp on sp.Id = d.Specialist_Id
where d.Id=" + DoctorId + " and d.IsDeleted=0";
            var data = ent.Database.SqlQuery<DoctorProfile>(q).ToList();
            if (data.Count() == 0)
            {
                rm.Message = "No Record";
                rm.Status = 0;
                return Ok(rm);
            }
            else
            {
                model.Id = data.FirstOrDefault().Id;
                model.DoctorName = data.FirstOrDefault().DoctorName;
                model.StateName = data.FirstOrDefault().StateName;
                model.CityName = data.FirstOrDefault().CityName;
                model.ClinicName = data.FirstOrDefault().ClinicName;
                model.DepartmentName = data.FirstOrDefault().DepartmentName;
                model.EmailId = data.FirstOrDefault().EmailId;
                model.Location = data.FirstOrDefault().Location;
                model.MobileNumber = data.FirstOrDefault().MobileNumber;
                model.AvailableTime = data.FirstOrDefault().AvailableTime;
                return Ok(model);
            }
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/DoctorApi/ViewPatientList")] //Appoitment History
        public IHttpActionResult ViewPatientList(int DoctorId)
        {
            var model = new PatientLists();
            var q = @"select PA.Patient_Id as PatientId, P.PatientName, P.PatientRegNo, d.DoctorName, d.Id as DoctorId from Doctor as D inner join PatientAppointment as PA on PA.Doctor_Id=D.Id inner join Patient as P on P.Id=PA.Patient_Id  where D.Id="+ DoctorId + " and PA.IsCancelled=0 and PA.AppointmentIsDone=1 and PA.Appointmentdate<getdate() order by PA.Id desc";
            var data = ent.Database.SqlQuery<PatientRecord>(q).ToList();
            if (data.Count() == 0)
            {
                model.Message = "No Data";
                model.response = data;
                return Ok(rm);
            }
            model.Message = "Success";
            model.response = data;
            return Ok(model);
        }



        [System.Web.Http.HttpGet]

        public IHttpActionResult PatientReports(int? DoctorId = 0, int? PatientId = 0)
        {
            var model = new PatientsData();
            var Id = new SqlParameter("@Id", PatientId);
            string q = @"select *,p.Id as PatientId from Patient p where p.Id=@Id";
            var data = ent.Database.SqlQuery<PatientsData>(q, Id).ToList();
            if (data != null)
            {
                model.PatientId = data.FirstOrDefault().PatientId;
                model.PatientName = data.FirstOrDefault().PatientName;
                model.PatientRegNo = data.FirstOrDefault().PatientRegNo;
                model.EmailId = data.FirstOrDefault().EmailId;
                model.MobileNumber = data.FirstOrDefault().MobileNumber;
                string report = @"select * from DoctorReports where Patient_Id=" + model.PatientId;
                var record = ent.Database.SqlQuery<PatientReports>(report).ToList();
                if (record != null)
                {
                    model.Message = "Success";
                    model.response = record;
                }
                else
                {
                    model.Message = "No Data";
                    model.response = record;
                }
            }
            else
            {
                model.Message = "Success";
                model.PatientId = 0;
                model.PatientName = null;
                model.PatientRegNo = null;
                model.EmailId = null;
                model.MobileNumber = null;
                model.response = null;
                return Ok(model);
            }
            return Ok(model);
        }

        [System.Web.Mvc.HttpGet]
        public IHttpActionResult GetDoctorDepartmentAndSpecialization(int doctorId)
        {
            var ds = (from dep in ent.DoctorDepartments
                      join
d in ent.Departments on dep.Department_Id equals d.Id
                      join s in ent.Specialists on dep.Specialist_Id equals s.Id
                      where dep.Doctor_Id == doctorId
                      select new DepartmentSpecialist
                      {
                          Id = dep.Id,
                          DepartmentName = d.DepartmentName,
                          SpecialistName = s.SpecialistName
                      }
                             ).ToList();
            dynamic obj = new ExpandoObject();
            obj.DepartmentAndSpecializationList = ds;
            return Ok(obj);
        }

        [System.Web.Http.HttpGet]
        public IHttpActionResult DeleteDoctorAndSpecialization(int doctorAndSpecializationId)
        {
            var rm = new ReturnMessage();
            try
            {
                var data = ent.DoctorDepartments.Find(doctorAndSpecializationId);
                if (data == null)
                {
                    rm.Status = 0;
                    rm.Message = "Invalid doctorAndSpecializationId";
                    return Ok(rm);
                }
                ent.DoctorDepartments.Remove(data);
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = "Data has deleted";
            }
            catch (Exception ex)
            {
                rm.Status = 0;
                rm.Message = "Server error";
            }
            return Ok(rm);
        }
        

        [System.Web.Http.HttpGet, System.Web.Mvc.Route("api/DoctorApi/getDoctorList")]
       
        public IHttpActionResult getDoctorList(int cityid)
        {
            try
            {
                var query = (from dept in ent.Departments
                             join d in ent.Doctors on dept.Id equals d.Department_Id
                             where d.IsDeleted == false && d.CityMaster_Id == cityid

                             select new DoctorDetail()
                             {
                                 Id = d.Id,
                                 DoctorName = d.DoctorName,
                                 Fee = d.Fee,
                                 About = d.About,
                                 Experience = (int)d.Experience,
                                 //Department_Id = d.Department_Id,
                                 DepartmentName = dept.DepartmentName,
                             }
                            ).ToList();
                dynamic obj = new ExpandoObject();
                obj.getDoctor = query;
                return Ok(obj);

            }
            catch
            {
                return BadRequest("Server Error");
            }
        }


         
        [System.Web.Http.HttpGet, System.Web.Mvc.Route("api/DoctorApi/DoctorDetails")]
        public IHttpActionResult DoctorDetails(int id) //Checkout api
        { 
            string query = @"select D.Id, D.DoctorName, D.Experience, D.Fee,D.About,de.DepartmentName,(select avg(rating1 + rating2 + rating3 + rating4 + rating5) from Review where Review.pro_Id=D.Id) As Rating
 FROM Doctor D
 left join Review as Re on  D.Id = Re.pro_Id
 left join Department as de on de.Id=D.Department_Id
 where D.Id ="+ id + " group by D.Id, D.DoctorName, D.Experience, D.Fee,D.About,de.DepartmentName";

            var data = ent.Database.SqlQuery<DoctorDetail>(query).FirstOrDefault();
            return Ok(data);
        }


        //==============================26-03-2023======================(done)==============//////////

        [System.Web.Mvc.HttpGet]
        [System.Web.Http.Route("api/DoctorApi/GetAppointmentDetail")]

        public IHttpActionResult GetAppointmentDetail(int Id)
        {

            var rm = new Doctor();
            string query = @"select PA.Id,P.PatientName,P.MobileNumber,P.Location,CONCAT(CONVERT(NVARCHAR, TS.StartTime, 8), ' To ', CONVERT(NVARCHAR, TS.EndTime, 8)) AS SlotTime,PA.Appointmentdate,AL.DeviceId from Doctor as D inner join PatientAppointment as PA on PA.Doctor_Id=D.Id inner join DoctorTimeSlot as TS on TS.Id=PA.Slot_id inner join Patient as P on P.Id=PA.Patient_Id inner join AdminLogin as AL on AL.Id=P.AdminLogin_Id where D.Id=" + Id + " and PA.IsCancelled=0 and PA.BookingMode_Id=1 and PA.AppointmentIsDone=0 order by PA.Id desc";
            var AppointmentDetail = ent.Database.SqlQuery<AppointmentDetailDoctorDTO>(query).ToList();

            return Ok(new { AppointmentDetail });

        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Http.Route("api/DoctorApi/GetVirtualAppointmentDetail")]

        public IHttpActionResult GetVirtualAppointmentDetail(int Id)
        {

            var rm = new Doctor();
            string query = @"select PA.Id,PA.Patient_Id,P.PatientName,P.MobileNumber,P.Location,CONCAT(CONVERT(NVARCHAR, TS.StartTime, 8), ' To ', CONVERT(NVARCHAR, TS.EndTime, 8)) AS SlotTime,PA.Appointmentdate,AL.DeviceId from Doctor as D inner join PatientAppointment as PA on PA.Doctor_Id=D.Id inner join DoctorTimeSlot as TS on TS.Id=PA.Slot_id inner join Patient as P on P.Id=PA.Patient_Id inner join AdminLogin as AL on AL.Id=P.AdminLogin_Id where D.Id=" + Id + " and PA.IsCancelled=0 and PA.BookingMode_Id=2 and PA.AppointmentIsDone=0";
            var AppointmentDetail = ent.Database.SqlQuery<AppointmentDetailDoctorDTO>(query).ToList();

            return Ok(new { AppointmentDetail });

        }


        [System.Web.Http.HttpGet]
        [System.Web.Mvc.Route("api/DoctorApi/DoctorpaymentHistory")]
        public IHttpActionResult DoctorpaymentHistory(int Id)
        {
            var rm = new Patient(); 
            string query = @"select P.Id,P.PatientName,P.Location,PA.TotalFee as Amount,PA.PaymentDate,PA.Id as PaymentId from Doctor as D 
inner join PatientAppointment as PA on PA.Doctor_Id=D.Id 
inner join Patient as P on P.Id=PA.Patient_Id where D.Id=" + Id + " and PA.IsCancelled=0 order by PA.Id desc";
            var PaymentHistory = ent.Database.SqlQuery<DoctorPayment>(query).ToList();

            return Ok(new { PaymentHistory });
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/DoctorApi/DoctorChooseDep")]
        public IHttpActionResult DoctorChooseDep(int StateMaster_Id, int CityMaster_Id, int Department_Id, int Specialist_Id)
        {
            var rm = new returnMessage();
            var Doctordata = ent.Doctors.Where(a => a.StateMaster_Id == StateMaster_Id && a.CityMaster_Id == CityMaster_Id && a.Department_Id == Department_Id && a.Specialist_Id == Specialist_Id).FirstOrDefault();
            if (Doctordata == null)
            {
                var msg = "no data found";
                rm.Message = msg;
                rm.Status = 404;
                return Ok(rm);
            }
            else
            {
                var model1 = new choosedoc();
                var query = @"select D.Id, D.DoctorName, D.Experience, D.Fee,D.About,de.DepartmentName,(select avg(rating1 + rating2 + rating3 + rating4 + rating5) from Review where Review.pro_Id=D.Id) As Rating FROM Doctor D inner join Department as de on de.Id=D.Department_Id where StateMaster_Id=" + StateMaster_Id + " and CityMaster_Id=" + CityMaster_Id + " and Department_Id=" + Department_Id + " and Specialist_Id=" + Specialist_Id + " and D.IsApproved=1 group by D.Id, D.DoctorName, D.Experience, D.Fee,D.About,de.DepartmentName";
                var data = ent.Database.SqlQuery<Doctorchoose>(query).ToList();
                model1.Doctorchoose = data;

                return Ok(model1);
            }
        }

      
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/DoctorApi/DoctorChoose")]
        public IHttpActionResult DoctorChoose(DModel Model)
        {
                var model1 = new choosedoc();
            //var query = @"select D.Id,D.DoctorName,D.Experience,D.Fee,PA.Patient_Id,(SELECT DepartmentName FROM Department WHERE Id=D.Department_Id) AS DepartmentName
            //           FROM Doctor D
            //    left join PatientAppointment as PA on  PA.Patient_Id = PA.Patient_Id
            var query = @"select D.Id, D.DoctorName, D.Experience, D.Fee,PA.Patient_Id,
                      (select SpecialistName from Specialist where Id = D.Specialist_Id) AS SpecialistName,
                       (SELECT DepartmentName FROM Department WHERE Id = D.Department_Id) AS DepartmentName
                                FROM Doctor D
                        left join PatientAppointment as PA on PA.Patient_Id = PA.Patient_Id 
                     where StateMaster_Id =" + Model.StateMaster_Id + " and CityMaster_Id=" + Model.CityMaster_Id + " and Department_Id=" + Model.Department_Id +"";
            /*and Specialist_Id="+Model.Specialist_Id+*/
            var data = ent.Database.SqlQuery<Doctorchoose>(query).ToList();
                model1.Doctorchoose = data;
               return Ok(model1);

        }



        [System.Web.Http.Route("api/DoctorApi/DoctorBooknow")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult DoctorBooknow(doctorBooknow model)
        {
            try
            {
                //====GENERATE ORDER NUMBER
                dynamic lastOrderIdRecord = ent.PatientAppointments.OrderByDescending(a => a.Id).Select(a => a.OrderId).FirstOrDefault();
                string lastOrderId = lastOrderIdRecord != null ? lastOrderIdRecord : "DR_ord_0"; // Default to "ps_inv_0" if no records exist


                string[] OrderIdparts = lastOrderId.Split('_');
                int OrderIdnumericPart = 0;

                if (OrderIdparts.Length == 3 && int.Parse(OrderIdparts[2]) > 0)
                {
                    OrderIdnumericPart = int.Parse(OrderIdparts[2]) + 1; // Increment the numeric part
                }
                else
                {

                    OrderIdnumericPart = 1;
                }

                // Generate the next NextOrderId
                string NextOrderId = $"DR_ord_{OrderIdnumericPart}";

                //====GENERATE INVOICE NUMBER

                //var lastRecord = ent.PatientAppointments.OrderByDescending(a => a.InvoiceNumber).FirstOrDefault();
                dynamic lastRecord = ent.PatientAppointments.OrderByDescending(a => a.Id).Select(a => a.InvoiceNumber).FirstOrDefault();
                //string lastInvoiceNumber = lastRecord != null ? lastRecord.InvoiceNumber : "DR_inv_0"; // Default to "ps_inv_0" if no records exist
                string lastInvoiceNumber = lastOrderIdRecord != null ? lastOrderIdRecord : "DR_inv_0"; // Default to "ps_inv_0" if no records exist


                string[] parts = lastInvoiceNumber.Split('_');
                int numericPart = 0;

                if (parts.Length == 3 && int.Parse(parts[2]) > 0)
                {
                    numericPart = int.Parse(parts[2]) + 1; // Increment the numeric part
                }
                else
                {

                    numericPart = 1;
                }

                // Generate the next invoice number
                string nextInvoiceNumber = $"DR_inv_{numericPart}";

                var data = new PatientAppointment()
                {
                    Doctor_Id = (int)model.Doctor_Id,
                    Slot_id = model.Slot_id,
                    AppointmentDate = model.AppointmentDate,
                    BookingMode_Id = model.BookingMode_Id,
                    InvoiceNumber = nextInvoiceNumber,
                    OrderId = NextOrderId,
                    IsPaid = false,
                    IsCancelled = false,
                    OrderDate = DateTime.Now,
                };
                ent.PatientAppointments.Add(data);
                ent.SaveChanges();
                return Ok(new { data.Id, Message = "Add Successfully" });
            }
            catch (Exception e)
            {
                return Ok("Internal server error");
            }
        }


        [System.Web.Http.Route("api/DoctorApi/DoctorApt")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult DoctorApt(int Doctor_Id,int PA_Id)
        {
            var model = new Doctor();
            double gst = ent.Database.SqlQuery<double>(@"select Amount from GSTMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault();
            string query = $"select  PA.Id,Doctor.DoctorName,Doctor.Fee,{gst} as GST,Doctor.Fee + Doctor.Fee *{gst}/100 as TotalFee,Doctor.Experience,CONCAT(CONVERT(NVARCHAR, TS.StartTime, 8), ' To ', CONVERT(NVARCHAR, TS.EndTime, 8)) AS SlotTime,(SELECT SpecialistName FROM Specialist WHERE Id=Doctor.Specialist_Id) AS SpecialistName,PA.AppointmentDate,al.DeviceId from Doctor left join PatientAppointment as PA on  Doctor.id = PA.Doctor_Id  Left join DoctorTimeSlot as Ts on  TS.Id=PA.Slot_id join AdminLogin as al on al.Id=Doctor.AdminLogin_Id  where Doctor.id=" + Doctor_Id + " and PA.Id=" + PA_Id + " and PA.IsCancelled=0";

            var data = ent.Database.SqlQuery<DoctorAptmt>(query).FirstOrDefault();
            return Ok(data);

        }


        [System.Web.Http.Route("api/DoctorApi/DoctorPayNow")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult DoctorPayNow(DoctorPayNow model)
        {
            try
            {
                if (model.IsPaid == true)
                {
                    var data = ent.PatientAppointments.Where(a => a.Doctor_Id == model.Doctor_Id && a.Id == model.Id).FirstOrDefault();
                    //var data1=ent.Doctors.Where(a=>a.Id == model.Doctor_Id).FirstOrDefault();
                    data.Doctor_Id = model.Doctor_Id;
                    data.Patient_Id = model.Patient_Id;
                    data.TotalFee = model.TotalFee;
                    data.PaymentDate = DateTime.Now;
                    data.IsPaid = model.IsPaid;
                    data.IsBooked = true;
                    data.AppointmentIsDone = false;
                    ent.SaveChanges();
                    return Ok(" Doctor Book Appointment Successfully ");
                }
                else if (model.IsPaid == false)
                {
                    var data = ent.PatientAppointments.Where(a => a.Doctor_Id == model.Doctor_Id && a.Id == model.Id).FirstOrDefault();
                    //var data1 = ent.Doctors.Where(a => a.Id == model.Doctor_Id).FirstOrDefault();
                    data.Doctor_Id = model.Doctor_Id;
                    data.Patient_Id = model.Patient_Id;
                    data.TotalFee = model.TotalFee;
                    data.PaymentDate = DateTime.Now;
                    data.IsPaid = model.IsPaid;
                    ent.SaveChanges();
                    return Ok("  Doctor Update Appointment ");
                }
                return Ok("Please check the detail");
            }
            catch (Exception e)
            {
                return Ok("Internal server error");
            }
        }

        //===============Doctor Upload Report================//

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/DoctorApi/Doctor_UploadReport")]
        public IHttpActionResult Doctor_UploadReport(Doctorupload_report model)
        {
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            try
            {
                if (model.Image1 == null)
                {
                    rm.Message = "Image File Picture can not be null";

                }
                //var img = FileOperation.UploadFile(model.FileName, "images", allowedExtensions);
                var img = FileOperation.UploadFileWithBase64("Images", model.Image1, model.Image1Base64, allowedExtensions);

                if (img == "not allowed")
                {
                    rm.Message = "Only png,jpg,jpeg,pdf files are allowed.";

                    return Ok(rm);

                }
                model.Image1 = img;
                var data = new DoctorReport();
                {
                    data.Doctor_Id = model.Doctor_Id;
                    data.Patient_Id = model.Patient_Id;
                    data.Image1 = model.Image1 = img;
                    data.UploadDate = DateTime.Now;
                }

                ent.DoctorReports.Add(data);
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = "Report Upload Successfully.";
                return Ok(rm);
            }
            catch (Exception e)
            {
                rm.Status = 0;
                return Ok("Internal server error");
            }
            return Ok(rm);

        }


        //======================DoctorViewReport==========================//
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/DoctorApi/Doctor_ViewReport")]
        public IHttpActionResult Doctor_ViewReport(int Id)
        {
            string qry = @"select DR.Id,P.PatientName,DR.Image1 from Patient as P left join DoctorReports as DR on DR.Patient_Id=P.Id where DR.Doctor_Id="+ Id +" order by Dr.Id Desc";
            var DoctorViewReport = ent.Database.SqlQuery<DoctorViewReport>(qry).ToList();
            return Ok(new { DoctorViewReport });
        }


        //===================DoctorViewReportFile======================//

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/DoctorApi/Doctor_ViewReport_File")]
        public IHttpActionResult Doctor_ViewReport_File(int Id)
        {
            var data = new LabReport();
            string qry = @"select Image1 from DoctorReports where Id=" + Id + "";
            var DoctorViewReport_file = ent.Database.SqlQuery<DoctorViewReportFile>(qry).ToList();

            return Ok(new { DoctorViewReport_file });
        }


        //====================Doctor About================//

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/DoctorApi/DoctorAbout")]

        public IHttpActionResult DoctorAbout(int Id)
        {
            string Qry = @"Select About From Doctor Where Id="+ Id +"";
            var About = ent.Database.SqlQuery<DocAbout>(Qry).FirstOrDefault();
            return Ok(new { About });
        }

        //===============DOCTOR UPLOAD REPORT==PATIENTNAME DROPDOWN========//

        [System.Web.Http.HttpGet, System.Web.Http.Route("api/DoctorApi/DoctorPatientList")]
        public IHttpActionResult DoctorPatientList(int Id)
        {
            var qry = @"select p.Id,p.PatientName from Patient as p 
left join PatientAppointment as PA on PA.Patient_Id=p.id
where PA.Doctor_Id=" + Id + "";
            var PatientName = ent.Database.SqlQuery<Labrepo>(qry).ToList();

            return Ok(new { PatientName });
        }

        //======================ADD DOCTOR BANK DETAILS==============//

        [System.Web.Http.Route("api/DoctorApi/Doctor_AddBankDetail")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult Doctor_AddBankDetail(BankDetail model)
        {
            try
            {
                var AddBnkDet = ent.BankDetails.Where(a => a.Login_Id == model.Login_Id).FirstOrDefault();
                if (AddBnkDet == null)
                {
                    var data = new BankDetail()
                    {
                        Login_Id = model.Login_Id,
                        AccountNo = model.AccountNo,
                        IFSCCode = model.IFSCCode,
                        BranchName = model.BranchName,
                        BranchAddress = model.BranchAddress,
                        HolderName = model.HolderName,
                        MobileNumber = model.MobileNumber,
                        isverified = true,
                    };
                    ent.BankDetails.Add(data);
                    ent.SaveChanges();
                    return Ok(new { Message = "Bank Detail Add SuccessFully!!!" });
                }
                else
                {
                    return BadRequest("Already Exist!!!");
                }
            }
            catch (Exception ex)
            {
                return Ok("Internal server error");
            }
           
        }

        [System.Web.Http.Route("api/DoctorApi/BookingMode")]
        [System.Web.Http.HttpGet]

        public IHttpActionResult BookingMode()
        {
            string qry = @"select * from DoctorBookingMode";
            var BookingMode = ent.Database.SqlQuery<BookingMode>(qry).ToList();
            return Ok(new { BookingMode });
        }
        [System.Web.Http.Route("api/DoctorApi/Doctor_TimeSlot")]
        [System.Web.Http.HttpGet]

        public IHttpActionResult Doctor_TimeSlot()
        {
            //string qry = @"select * from DoctorTimeSlot";
            string qry = @"SELECT Id,CONCAT(CONVERT(NVARCHAR, StartTime, 8), ' To ', CONVERT(NVARCHAR, EndTime, 8)) AS SlotTime FROM DoctorTimeSlot";
            var TimeSlot = ent.Database.SqlQuery<Doctorslot>(qry).ToList();
            return Ok(new { TimeSlot });
        }

        // doctor invoice 

        [System.Web.Http.HttpGet, System.Web.Http.Route("api/DoctorApi/DoctorInvoice/{Invoice}")]
        public IHttpActionResult DoctorInvoice(string Invoice)
        {
            try
            {
                var gst = ent.GSTMasters.Where(x=>x.IsDeleted==false).FirstOrDefault(x => x.Name == "Doctor");
                var invoiceData = (from d in ent.Doctors
                                   join pa in ent.PatientAppointments on d.Id equals pa.Doctor_Id
                                   where pa.InvoiceNumber == Invoice
                                   select new
                                   {
                                       pa.Patient_Id,
                                       pa.Id,
                                       d.DoctorName,
                                       d.Fee,
                                       pa.TotalFee,
                                       GST = gst.Amount,
                                       pa.OrderId,
                                       pa.InvoiceNumber,
                                       pa.OrderDate
                                   }).ToList();

                if (invoiceData.Count > 0)
                {
                    double? grandTotal = invoiceData.Sum(item => item.TotalFee);
                    double? totalAmount = invoiceData.Sum(item => item.Fee);
                    double gstAmount = (double)(totalAmount * gst.Amount) / 100;
                    double? finalAmount = grandTotal - gstAmount;

                    int? patientId = invoiceData[0].Patient_Id;

                    var patientData = ent.Patients.FirstOrDefault(x => x.Id == patientId);
                   

                    if (patientData != null)
                    {
                        return Ok(new
                        {
                            InvoiceData = invoiceData,
                            Name = patientData.PatientName,
                            Email = patientData.EmailId,
                            PinCode = patientData.PinCode,
                            MobileNo = patientData.MobileNumber,
                            Address = patientData.Location,
                            InvoiceNumber = Invoice,
                            OrderId = invoiceData[0].OrderId,
                            OrderDate = invoiceData[0].OrderDate,
                            GST = invoiceData[0].GST,
                            GSTAmount = gstAmount,
                            GrandTotal = grandTotal,
                            FinalAmount = finalAmount,
                            Status = 200,
                            Message = "Invoice"
                        });
                    }
                    else
                    {
                        return BadRequest("Patient data not found");
                    }
                }
                else
                {
                    return BadRequest("No Invoice data found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Server Error");
            }

        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/DoctorApi/AddMedicinePrescription")]

        public IHttpActionResult AddMedicinePrescription(MedicinePrescriptionDetail model)
        {
            try
            {
                var userdata = ent.Patients.Where(a => a.Id == model.Patient_Id).FirstOrDefault();
                var doctor = ent.Doctors.Where(a => a.Id == model.Doctor_Id).FirstOrDefault();
                var DomailModel = new MedicinePrescriptionDetail()
                {
                    Doctor_Id = model.Doctor_Id ,
                    Patient_Id  = model.Patient_Id,
                    Weight = model.Weight,
                    PresentComplaint = model.PresentComplaint,
                    Allergies = model.Allergies,
                    Primarydiagnosis = model.Primarydiagnosis,
                    MedicineName1 = model.MedicineName1,
                    Dosage1 = model.Dosage1,
                    Instruction1 = model.Instruction1,
                    MedicineName2 = model.MedicineName2,
                    Dosage2 = model.Dosage2,
                    Instruction2 = model.Instruction2,
                    MedicineName3 = model.MedicineName3,
                    Dosage3 = model.Dosage3,
                    Instruction3 = model.Instruction3,
                    MedicineName4 = model.MedicineName4,
                    Dosage4 = model.Dosage4,
                    Instruction4 = model.Instruction4,
                    MedicineName5 = model.MedicineName5,
                    Dosage5 = model.Dosage5,
                    Instruction5 = model.Instruction5,
                    MedicineName6 = model.MedicineName6,
                    Dosage6 = model.Dosage6,
                    Instruction6 = model.Instruction6,
                    MedicineName7 = model.MedicineName7,
                    Dosage7 = model.Dosage7,
                    Instruction7 = model.Instruction7,
                    MedicineName8 = model.MedicineName8,
                    Dosage8 = model.Dosage8,
                    Instruction8 = model.Instruction8,
                    MedicineName9 = model.MedicineName9,
                    Dosage9 = model.Dosage9,
                    Instruction9 = model.Instruction9,
                    MedicineName10 = model.MedicineName10,
                    Dosage10 = model.Dosage10,
                    Instruction10 = model.Instruction10,
                    TestPrescribed = model.TestPrescribed,
                    PastMedical_SurgicalHistory = model.PastMedical_SurgicalHistory,
                    EntryDate = DateTime.Now 

                };
                ent.MedicinePrescriptionDetails.Add(DomailModel);
                ent.SaveChanges();
                EmailEF ef = new EmailEF()
                {
                    EmailAddress = userdata.EmailId,
                    Message = "Medicine Prescription",
                    Subject = "Prescription Pdf.",
                    //id = id
                };

                EmailOperations.SendEmainewpdf(ef);
                EmailEF drmail = new EmailEF()
                {
                    EmailAddress = doctor.EmailId,
                    Message = "Medicine Prescription",
                    Subject = "Prescription Pdf.",
                    //id = id
                };

                EmailOperations.SendEmainewpdf(drmail);
                Message.SendSmsUserIdPass("gdfsg");
                rm.Status = 1;
                rm.Message = "Detail Add Successfully!!!";
                return Ok(rm);
            }
            catch (Exception ex)
            {
                return BadRequest("Internal Server Error");
            }
        }


        [System.Web.Http.HttpPost, System.Web.Http.Route("api/DoctorApi/AppointmentDone")]

        public IHttpActionResult AppointmentDone(DoctorAptmt model)
        {
            try
            {
                var data = ent.PatientAppointments.Find(model.Id);

                if (data != null)
                {
                    data.AppointmentIsDone = true;
                    ent.SaveChanges();

                    rm.Status = 1;
                    rm.Message = "Appointment is done.";
                    return Ok(rm);
                }
                else
                {
                    rm.Status = 0;
                    return BadRequest("Appointment not found.");
                }
            }
            catch (Exception ex)
            {
                //string msg = ex.ToString();
                string msg = "Internal server error.";
                rm.Status = 0; 
                return BadRequest(msg);
            }
           
        }


    }
}