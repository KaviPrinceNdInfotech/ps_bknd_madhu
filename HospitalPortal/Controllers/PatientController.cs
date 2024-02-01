using AutoMapper;
using HospitalPortal.BL;
using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Repositories;
using HospitalPortal.Utilities;
using iTextSharp.text.pdf;
using iTextSharp.text;
using log4net;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace HospitalPortal.Controllers
{
    [Authorize(Roles = "patient,RWA,admin,Franchise")]
    public class PatientController : Controller
    {
        DbEntities ent = new DbEntities();

        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(PatientController));
        GenerateBookingId Patient = new GenerateBookingId();


        [AllowAnonymous]
        public ActionResult Add(int rwaId = 0, int vendorId = 0)
        {
            var model = new PatientDTO();
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
            model.Rwa_Id = rwaId;
            model.vendorId = vendorId;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Add(PatientDTO model)
        {

            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                        return View(model);
                    }
                    //if (ent.AdminLogins.Any(a => a.Username == model.EmailId))
                    //{
                    //    TempData["msg"] = "This Email-Id has already exists.";
                    //    return RedirectToAction("Add");
                    //}
                    //if (ent.AdminLogins.Any(a => a.PhoneNumber == model.MobileNumber))
                    //{
                    //    TempData["msg"] = "This Mobile Number has already exists.";
                    //    return RedirectToAction("Add");
                    //}
                    if (ent.Patients.Any(a => a.PatientName == model.PatientName && a.MobileNumber == model.MobileNumber))
                    {
                        var data = ent.Patients.Where(a => a.PatientName == model.PatientName && a.MobileNumber == model.MobileNumber).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.PatientRegNo).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        TempData["msg"] = "you are already registered with pswellness";
                        return RedirectToAction("Add", "Patient");
                    }


                    var admin = new AdminLogin
                    {
                        Username = model.EmailId.ToLower(),
                        PhoneNumber = model.MobileNumber,
                        Password = model.Password,
                        Role = "patient"
                    };
                    ent.AdminLogins.Add(admin);
                    ent.SaveChanges();

                    var domainModel = Mapper.Map<Patient>(model);
                    domainModel.AdminLogin_Id = admin.Id;
                    domainModel.Reg_Date = DateTime.Now;
                    domainModel.IsApproved = true;
                    domainModel.EmailId = model.EmailId.ToLower();
                    domainModel.PatientRegNo = Patient.GeneratePatientRegNo();
                    admin.UserID = domainModel.PatientRegNo;
                    ent.Patients.Add(domainModel);
                    ent.SaveChanges();
                    string msg = "Welcome to PSWELLNESS. Your User Name :  " + domainModel.EmailId + "(" + domainModel.PatientRegNo + "), Password : "+ admin.Password+".";
                    Message.SendSms(domainModel.MobileNumber, msg);
                    string msg1 = "Welcome to PSWELLNESS. Your User Name :  " + admin.Username + "(" + admin.UserID + "), Password : " + admin.Password + ".";

                    Utilities.EmailOperations.SendEmail1(model.EmailId, "Ps Wellness", msg1, true);

                    TempData["msg"] = "ok";
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    TempData["msg"] = "Server Error";
                    tran.Rollback();
                }
                return RedirectToAction("Add",new { rwaId=model.Rwa_Id});
            }

        }

        public ActionResult Edit(int id)
        {
            var data = ent.Patients.Find(id);
            var model = Mapper.Map<PatientDTO>(data);
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName", model.StateMaster_Id);
            model.Cities = new SelectList(repos.GetCitiesByState(model.StateMaster_Id), "Id", "CityName", model.CityMaster_Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(PatientDTO model)
        {
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Password");
            ModelState.Remove("MobileNumber");
            ModelState.Remove("Reg_Date");
            ModelState.Remove("IsCheckedTermsCondition");
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName", model.StateMaster_Id);
            model.Cities = new SelectList(repos.GetCitiesByState(model.StateMaster_Id), "Id", "CityName", model.CityMaster_Id);
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                var data = Mapper.Map<Patient>(model);
                ent.Entry<Patient>(data).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                TempData["msg"] = "ok";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                TempData["msg"] = "Server error";
            }
            return RedirectToAction("PatientList", new { id = model.Id });
        }


        [Authorize(Roles = "RWA,admin,Franchise")]
        public ActionResult PatientList(string term, int? pageNumber, int rwaId = 0, int fid = 0)
        {
            var model = new PatientListModel();
            var result = (from p in ent.Patients
                          join s in ent.StateMasters
                           on p.StateMaster_Id equals s.Id
                          join c in ent.CityMasters
                          on p.CityMaster_Id equals c.Id
                          join v in ent.Vendors
                          on p.vendorId equals v.Id
                          into t from rt in t.DefaultIfEmpty()
                          //orderby p.Id descending
                          select new PatientList 
                          {
                              PatientRegNo = p.PatientRegNo,
                              CityName = c.CityName,
                              StateName = s.StateName,
                              Id = p.Id,
                              IsApproved = p.IsApproved,
                              IsDeleted = p.IsDeleted,
                              CityMaster_Id = p.CityMaster_Id,
                              EmailId = p.EmailId,
                              MobileNumber = p.MobileNumber,
                              StateMaster_Id = p.StateMaster_Id,
                              PatientName = p.PatientName,
                              Rwa_Id = p.Rwa_Id,
                              Location = p.Location,
                              AdminLogin_Id = p.AdminLogin_Id,
                              VendorName = rt.VendorName,
                              vendorId = p.vendorId,
                              UniqueId = rt.UniqueId
                          }).OrderByDescending(x=>x.Id).ToList();
            if(!string.IsNullOrEmpty(term))
            {
                result = result.Where(a => a.PatientName.Contains(term) || a.MobileNumber.StartsWith(term) || a.PatientRegNo.StartsWith(term)).ToList();
            }
            if(fid != 0)
            {
                result = result.Where(a => a.vendorId == fid).ToList();
            }
            int total = result.Count;
            pageNumber = (int?)pageNumber ?? 1;
            int pageSize = 10;
            decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
            model.TotalPages = (int)noOfPages;
            model.PageNumber = (int)pageNumber;
            result = result.OrderByDescending(a => a.Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
            model.Patient = result;
            return View(model);
        }


        [Authorize(Roles = "RWA,admin")]
        public ActionResult All(int rwaId=0)
        {
            return View(rwaId);
        }

        [Authorize(Roles = "RWA,admin")]
        public string GetPatientList(string sEcho,  int iDisplayLength, string sSearch, int iDisplayStart = 0,int rwaId=0)
        {
            sSearch = sSearch.ToLower();
            int totalRecord = ent.Patients.Where(a=>a.Rwa_Id==rwaId).Count();
            var patients = new List<Patient>();
            if (!string.IsNullOrEmpty(sSearch))
                patients = ent.Patients.Where(a=> a.Rwa_Id==rwaId && a.EmailId.ToLower().Contains(sSearch)
                || a.PatientName.ToLower().Contains(sSearch)
                || a.MobileNumber.StartsWith(sSearch)
                ).ToList();
            else
                patients = ent.Patients.Where(a=> a.Rwa_Id == rwaId).ToList();
            patients = patients.OrderBy(a => a.Id).Skip(iDisplayStart).Take(iDisplayLength).ToList();
            var result = (from p in patients join s in ent.StateMasters
                         on  p.StateMaster_Id equals s.Id
                         join c in ent.CityMasters 
                         on p.CityMaster_Id equals c.Id
                         select new PatientDTO
                         {
                             CityName=c.CityName,
                             StateName=s.StateName,
                             Id=p.Id,
                             IsApproved=p.IsApproved,
                             IsDeleted=p.IsDeleted,
                             CityMaster_Id=p.CityMaster_Id,
                             EmailId=p.EmailId,
                             MobileNumber=p.MobileNumber,
                             StateMaster_Id=p.StateMaster_Id,
                             PatientName=p.PatientName,
                             Rwa_Id=p.Rwa_Id,
                             Location=p.Location,
                         }
                         ).ToList();

            StringBuilder sb = new StringBuilder();
            sb.Clear();
            sb.Append("{");
            sb.Append("\"sEcho\": ");
            sb.Append(sEcho);
            sb.Append(",");
            sb.Append("\"iTotalRecords\": ");
            sb.Append(totalRecord);
            sb.Append(",");
            sb.Append("\"iTotalDisplayRecords\": ");
            sb.Append(totalRecord);
            sb.Append(",");
            sb.Append("\"aaData\": ");
            sb.Append(JsonConvert.SerializeObject(result));
            sb.Append("}");
            return sb.ToString();
        }

        [AllowAnonymous]
        public void test()
        {
            //var sr = new JavaScriptSerializer();
            //var d = new HospitalRegistrationReq
            //{
            //    HospitalName = "credfdf",
            //    StateMaster_Id = 2,
            //    CityMaster_Id = 1,
            //    EmailId = "drue@abc.com",
            //    Location = "new asbc asdf",
            //    Password = "12345",
            //    ConfirmPassword = "12345",
            //    MobileNumber = "7895857584",
            //    AuthorizationLetterImageName = "cart1.png",
            //    AuthorizationLetterBase64 = "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",
            //    Location_Id = 1,
            //    PhoneNumber = "1312322"
            //};

            //string j = sr.Serialize(d).ToString();
            //   Response.Write(j);

            //Execute().Wait();
        }

        //[AllowAnonymous]
        //static async Task Execute()
        //{
        //    var apiKey = Environment.GetEnvironmentVariable("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");
        //    var client = new SendGridClient(apiKey);
        //    var from = new EmailAddress("ravindra@ndinfotech.co.in", "Example User");
        //    var subject = "Sending with SendGrid is Fun";
        //    var to = new EmailAddress("ravindra.devrani003@gmail.com", "Example User");
        //    var plainTextContent = "and easy to do anywhere, even with C#";
        //    var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
        //    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        //    var response = await client.SendEmailAsync(msg);
        //    var status = response.StatusCode; 

        //}

        public ActionResult AppointmentHistroy(int PatientId, DateTime? date = null)
        {
            var model = new PatientHistory();
            var query = @"select AppointmentDate, StateMaster.StateName, CityMaster.CityName, Doctor.MobileNumber as MobileNo , Doctor_Id, PatientAppointment.Specialist_Id, Specialist.SpecialistName as Specility,Doctor.ClinicName, Doctor.Location as ClinicAddress, Doctor.PhoneNumber as PhoneNo, Doctor.DoctorName as DoctorName, { fn concat(CONVERT(varchar(15),CAST(StartSlotTime AS TIME),100),{fn concat ('-', CONVERT(varchar(15),CAST(EndSlotTime AS TIME),100))})} AS AppointedTime from PatientAppointment join Doctor on Doctor.Id = PatientAppointment.Doctor_Id join Specialist on Doctor.Specialist_Id = Specialist.Id join CityMaster on Doctor.CityMaster_Id = CityMaster.Id join StateMaster on Doctor.StateMaster_Id = StateMaster.Id where Patient_Id='" + PatientId + "'order by AppointmentDate desc";
            var data = ent.Database.SqlQuery<AppointmentSearchBy_Patient>(query).ToList();

            if (date != null)
            {
                data = data.Where(a => a.AppointmentDate == date).ToList();
            }
            //var data1 = (from appointment in data
            //             join Doctors in ent.Doctors on appointment equals Doctors.Id
            //             join Speciality in ent.Specialists on appointment equals Speciality.Id
            //             where appointment == PatientId
            //             select new AppointmentSearchBy_Patient
            //             {
            //                 DoctornName = Doctors.DoctorName,
            //                 AppointmentDate = appointment,
            //                 MobileNo = Doctors.MobileNumber,
            //                 PhoneNo = Doctors.PhoneNumber,
            //                 ClinicAddress = Doctors.Location,
            //                 ClinicName = Doctors.ClinicName,
            //                 Specility = Speciality.SpecialistName,
            //                 //AppointmentEndTime = appointment.EndSlotTime,
            //                 //AppointmentStartTime = appointment.StartSlotTime,
            //                 AppointedTime = appointment.StartSlotTime+" - "+ appointment.EndSlotTime,
            //                 }).ToList();
            model.AppointmentHistory = data;
            return View(model);
        }


        public ActionResult TestHistory(int PatientId, DateTime? date, int? pageNumber)
        {
            var model = new BookedTests();
            var query = @"select TestDate, Lab.LabName, Lab.PhoneNumber, Lab.MobileNumber, Lab.Location,
LabTest.TestName, TestLab.TestAmount,{ fn concat(CONVERT(varchar(15),CAST(AvailabelTime1 AS TIME),100),{fn concat ('-', CONVERT(varchar(15),CAST(AvailableTime2 AS TIME),100))})} AS AvailableTime from BookTestLab 
join Lab on Lab.Id = BookTestLab.Lab_Id join LabTest on LabTest.Id = BookTestLab.Test_Id join TestLab on TestLab.Test_Id = LabTest.Id  where Patient_Id='" + PatientId + "'order by TestDate desc";
            var data = ent.Database.SqlQuery<BookingTestHistory>(query).ToList();
            if (date != null)
            {
                data = data.Where(a => a.TestDate == date).ToList();
            }
            int total = data.Count;
            pageNumber = (int?)pageNumber ?? 1;
            int pageSize = 5;
            decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
            model.TotalPages = (int)noOfPages;
            model.PageNumber = (int)pageNumber;
            data = data.OrderBy(a => a.Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
            model.BookingHistory = data;
            return View(model);
        }

        public ActionResult ViewCheckUpHistory(int id)
        {
            var model = new HealthCheckupPatientList();
            var q = @"select  C.Patient_Id, P.LabName from CheckUpReport C join HealthCheckupCenter P on P.Id = C.Patient_Id WHERE C.Patient_Id='"+id+"' group BY C.Patient_Id, P.LabName";
            var data = ent.Database.SqlQuery<HealthCheckUpRoprt>(q).ToList();
            model.HealthCheckUpReprt = data;
            return View(model);
        }


        public ActionResult ViewTestReport(int id)
        {
            var model = new HealthCheckupPatientList();
            var query= @"select * from CheckUpReport where Patient_Id=" + id;
            var data = ent.Database.SqlQuery<HealthCheckUpRoprt>(query).ToList();
            model.HealthCheckUpReprt = data;
            return View(model);
        }


        public ActionResult ViewLabTestHistory(int id)
        {
            var model = new HealthCheckupPatientList();
            //var q = @"select  C.Patient_Id, P.LabName from LabReport C join Lab P on P.Id = C.Patient_Id WHERE C.Patient_Id='" + id + "' group BY C.Patient_Id, P.LabName";
            var q = @"select  LR.Patient_Id, L.LabName from LabReport LR join Lab L on L.Id = LR.Lab_Id WHERE LR.Patient_Id="+ id + "";
            var data = ent.Database.SqlQuery<LabTestReport>(q).ToList();
            model.LabTestReport = data;
            return View(model);
        }


        public ActionResult ViewLabReport(int id)
        {
            var model = new HealthCheckupPatientList();
            var query = @"select * from LabReport lr join LabTest lt on lt.Id = lr.Test where Patient_Id=" + id;
            var data = ent.Database.SqlQuery<LabTestReport>(query).ToList();
            model.LabTestReport = data;
            return View(model);
        }

        public ActionResult MedicineOrderHistory( int id)
        {
            var model = new MedicineVM();
            string query = @"select MeO.id,MeO.DeliveryAddress,MeO.DeliveryDate,MeO.InvoiceNumber,MeO.OrderId ,Me.MedicineName,Me.BrandName,Me.MedicineDescription,
MeT.MedicineTypeName,MEOD.Amount,MEOD.Quantity
from MedicineOrder AS MeO with(nolock)  
INNER JOIN MedicineOrderDetail AS  MEOD with(nolock) on MeO.Id = MEOD.Order_Id
INNER JOIN Medicine  AS Me with(nolock) On MEOD.Medicine_Id = Me.Id
INNER JOIN MedicineType AS MeT with(nolock) ON Me.MedicineType_Id =MeT.Id
WHERE  MeO.Patient_Id= " + id + " order by MeO.Id desc";
            var data = ent.Database.SqlQuery<MedicineOrderModel>(query).ToList();
            model.MedicineOrderHis = data;
            return View(model);
        }
        public ActionResult DownloadMedicinePdf(int id)
        {

            var model1 = new MedicineVM();
            string query = @"select MeO.id,MeO.DeliveryAddress,MeO.DeliveryDate,MeO.InvoiceNumber,MeO.OrderId ,Me.MedicineName,Me.BrandName,Me.MedicineDescription,
MeT.MedicineTypeName,MEOD.Amount,MEOD.Quantity
from MedicineOrder AS MeO with(nolock)  
INNER JOIN MedicineOrderDetail AS  MEOD with(nolock) on MeO.Id = MEOD.Order_Id
INNER JOIN Medicine  AS Me with(nolock) On MEOD.Medicine_Id = Me.Id
INNER JOIN MedicineType AS MeT with(nolock) ON Me.MedicineType_Id =MeT.Id
WHERE  MeO.Id= " + id + " order by MeO.Id desc";
            var model2 = ent.Database.SqlQuery<MedicineOrderModel>(query).ToList();
            // model.MedicineOrderHis = data;
            // Replace with your actual model and values
            foreach (var model in model2)
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var document = new Document())
                    {
                        PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                        document.Open();

                        document.Add(new iTextSharp.text.Paragraph("Medicine Information"));
                        document.Add(new iTextSharp.text.Paragraph($"Medicine Name: {model.MedicineName}"));
                        document.Add(new iTextSharp.text.Paragraph($"Medicine Type: {model.MedicineTypeName}"));
                        document.Add(new iTextSharp.text.Paragraph($"Medicine Description: {model.MedicineDescription}"));
                        document.Add(new iTextSharp.text.Paragraph($"Brand Name: {model.BrandName}"));
                        document.Add(new iTextSharp.text.Paragraph($"Amount: {model.Amount:C}"));
                        document.Add(new iTextSharp.text.Paragraph($"Quantity: {model.Quantity}"));
                        document.Add(new iTextSharp.text.Paragraph($"Order ID: {model.Order_Id}"));
                        document.Add(new iTextSharp.text.Paragraph($"Invoice Number: {model.InvoiceNumber}"));
                        document.Add(new iTextSharp.text.Paragraph($"Delivery Address: {model.DeliveryAddress}"));
                        document.Add(new iTextSharp.text.Paragraph($"Delivery Date: {model.DeliveryDate?.ToString("yyyy-MM-dd HH:mm")}"));

                        document.Close();
                    }

                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", "attachment; filename=MedicineInfo.pdf");
                    Response.BinaryWrite(memoryStream.ToArray());
                    Response.End();
                }
            }
            return new EmptyResult();
        }

    }
}