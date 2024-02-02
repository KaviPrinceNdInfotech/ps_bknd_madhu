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
using HospitalPortal.Models;
using static HospitalPortal.Models.ViewModels.ChemistDTO;

namespace HospitalPortal.Controllers
{
    public class LabApiController : ApiController
    {
        ILog log = LogManager.GetLogger(typeof(LabApiController));
        DbEntities ent = new DbEntities();
        returnMessage rm = new returnMessage();
        Rmwithparm rwithprm = new Rmwithparm();
        //USER SECTION
        [HttpGet]
        public IHttpActionResult LabsList(int CityId, int StateId, int testId)
        {
            var model1 = new LabV();
            //var query = @"select L.Id,L.LabName,L.MobileNumber,L.Location,L.WorkingDay,L.Fee,(select AVG(Rating1 + Rating2 + Rating3 + Rating4 + Rating5) from Review where Review.pro_Id=L.Id)as Rating ,{ fn concat(CONVERT(varchar(15),CAST(StartTime AS TIME),100),{fn concat ('-', CONVERT(varchar(15),CAST(EndTime AS TIME),100))})} AS OpeningHours from Lab as L left join LabTest as Lt on Lt.Id=L.LabTest_Id where L.CityMaster_Id=" + CityId + " and StateMaster_Id = " + StateId + " and Lt.Id =" + testId;
            var query = @"select L.Id,L.LabName,L.MobileNumber,L.Location,L.WorkingDay,LTT.Testamount as Fee,(select AVG(Rating1 + Rating2 + Rating3 + Rating4 + Rating5) from Review where Review.pro_Id=L.Id)as Rating ,{ fn concat(CONVERT(varchar(15),CAST(StartTime AS TIME),100),{fn concat ('-', CONVERT(varchar(15),CAST(EndTime AS TIME),100))})} AS OpeningHours from TestInLab as TL
  left join Lab as L on L.Id=TL.Lab_Id
  left join LabTest as LTT on LTT.Id=TL.Test_Id 
  where L.CityMaster_Id=" + CityId + " and StateMaster_Id = " + StateId + " and TL.Test_Id =" + testId + " and L.IsApproved=1";
            var data = ent.Database.SqlQuery<LabBookings>(query).ToList();
            model1.LabBookings = data;
            return Ok(model1);
        }

        [HttpGet]
        public IHttpActionResult LabTest(string TestName = null, string CityName = null)
        {
            var model = new LabVM();
            var Test = ent.LabTests.FirstOrDefault(a => a.TestName == TestName);
            var City = ent.CityMasters.FirstOrDefault(a => a.CityName == CityName);
            //                var query = @"select LabTest.Id as TestId ,TestLab.TestDescription as TestName,Lab.Id as LabId,
            //Lab.LabName, Lab.PhoneNumber, Lab.MobileNumber, TestLab.TestAmount,
            // { fn concat(CONVERT(varchar(15),CAST(StartTime AS TIME),100),{fn concat ('-', CONVERT(varchar(15),CAST(EndTime AS TIME),100))})} AS OpeningHours from Lab join TestLab on Lab.Id = TestLab.Lab_Id join LabTest on LabTest.Id = TestLab.Test_Id join CityMaster on Lab.CityMaster_Id = CityMaster.Id
            // where LabTest.TestName='" + TestName + "'and CityMaster.CityName='"+CityName+"'";

            var query = @"select LabTest.Id as TestId,LabTest.TestDesc as TestName ,Lab.Id as LabId,
Lab.LabName, Lab.PhoneNumber, Lab.MobileNumber, LabTest.TestAmount,
  { fn concat(CONVERT(varchar(15),CAST(StartTime AS TIME),100),{fn concat ('-', CONVERT(varchar(15),CAST(EndTime AS TIME),100))})} AS OpeningHours 
 from Lab 
  join LabTest on Lab.Id = LabTest.Lab_Id
  join CityMaster on Lab.CityMaster_Id = CityMaster.Id
 where LabTest.TestName='" + TestName + "'and CityMaster.CityName='" + CityName + "'";


            var data = ent.Database.SqlQuery<LabListViaTest_VM>(query).ToList();
            if (data.Count() == 0)
            {
                rm.Message = "Desired Test Not Available in any lab, Try Another Time";
                rm.Status = 0;
                return Ok(rm);
            }
            model.LabListViaTest = data;
            model.LabListViaTest.FirstOrDefault().Status = true;
            model.Message = "Available in  lab";
            model.Status = 1;
            return Ok(model);
        }

        //USER Section
        [HttpGet]
        public IHttpActionResult LabDetails(int id)
        {
            var model = new Lab();
            string query = @"select L.id,L.LabName,L.about,L.LabTypeName,L.year,L.Location,L.fee,L.WorkingDay,
(select AVG(Rating1 + Rating2 + Rating3 + Rating4 + Rating5) from Review where Review.pro_Id=L.Id)as Rating 
 from Lab as L left join Review as R on R.pro_Id=L.Id 
where L.Id=" + id + "";
            var data = ent.Database.SqlQuery<LabList>(query).FirstOrDefault();
            return Ok(new { data });
        }
        //USER SECTION
        [Route("api/LabApi/LabBooking")]
        [HttpPost]
        public IHttpActionResult LabBooking(LabBooked model)
        {
            var model1 = new LabV();
            var query = @"select L.Id,L.LabName,L.MobileNumber,L.Location,L.WorkingDay,L.Fee,
(select AVG(Rating1 + Rating2 + Rating3 + Rating4 + Rating5) from Review where Review.pro_Id=L.Id)as Rating ,
        { fn concat(CONVERT(varchar(15),CAST(StartTime AS TIME),100),{fn concat ('-', CONVERT(varchar(15),CAST(EndTime AS TIME),100))})} AS OpeningHours from Lab as L
        inner join LabTest as Lt on Lt.Id=L.LabTest_Id 
		inner join Review as R on R.pro_Id=L.Id 
where L.CityMaster_Id=" + model.CityMaster_Id + " and StateMaster_Id = " + model.StateMaster_Id + " and Lt.Id =" + model.testId;
            var data = ent.Database.SqlQuery<LabBookings>(query).ToList();
            model1.LabBookings = data;
            return Ok(new { data });
        }

        [HttpGet]
        public IHttpActionResult BookedTestHistory(int LabId)
        {
            var model = new BookedTests();
            var query = @"select *, Lab.Id as LabId, BookTestLab.Id,{ fn concat(CONVERT(varchar(15),CAST(AvailabelTime1 AS TIME),100),{fn concat ('-', CONVERT(varchar(15),CAST(AvailableTime2 AS TIME),100))})} AS AvailableTime from BookTestLab join Lab on BookTestLab.Lab_Id = Lab.Id join LabTest on BookTestLab.Test_Id = LabTest.Id and BookTestLab.IsPaid=0 join TestLab on TestLab.Test_Id = BookTestLab.Test_Id where  TestLab.Lab_Id=" + LabId;
            var data = ent.Database.SqlQuery<TestList>(query).ToList();
            //if (date != null)
            //{
            //data = data.Where(a => a.TestDate == date).ToList();
            if (data.Count() == 0)
            {
                rm.Message = "No Records for Selected Date";
                rm.Status = 0;
                return Ok(rm);
            }

            model.TestList = data;
            return Ok(model);
        }

        [HttpGet]
        public IHttpActionResult DeleteLabBooking(int Id)
        {
            var Lab_Id = ent.BookTestLabs.Find(Id);
            if (Lab_Id != null)
            {
                ent.BookTestLabs.Remove(Lab_Id);
                ent.SaveChanges();
                rm.Message = "SuccessFully Removed";
                rm.Status = 1;
                return Ok(rm);
            }
            else
            {
                rm.Message = "Some error has occurred";
                rm.Status = 0;
                return Ok(rm);
            }
        }
        [HttpGet]
        public IHttpActionResult LabPaymentStatus(int TestId)
        {
            try
            {
                string query = @"update LabBooking set PaymentDate=getdate(),IsPaid=1 where Id=" + TestId;
                int Test_Id = ent.Database.SqlQuery<int>(@"select Test from LabBooking where Id='" + TestId + "'").FirstOrDefault();
                int Lab_Id = ent.Database.SqlQuery<int>(@"select Lab_Id from TestLab where Test_Id='" + Test_Id + "'").FirstOrDefault();
                int PId = ent.Database.SqlQuery<int>(@"select PatientId from LabBooking where Id='" + TestId + "'").FirstOrDefault();
                DateTime Date = ent.Database.SqlQuery<DateTime>(@"select Convert(date,TestDate) from LabBooking where Id='" + TestId + "'").FirstOrDefault();
                string AppointmentTime = ent.Database.SqlQuery<string>(@"select { fn concat(CONVERT(varchar(15),CAST(AvailabelTime1 AS TIME),100), {fn concat ('-', CONVERT(varchar(15),CAST(AvailabelTime2 AS TIME),100))})} AS AppointedTime from LabBooking where Id =" + TestId).FirstOrDefault();
                string mobile = ent.Database.SqlQuery<string>(@"select MobileNumber from Lab where Id='" + Lab_Id + "'").FirstOrDefault();
                string Name = ent.Database.SqlQuery<string>(@"select PatientName from Patient where Id='" + PId + "'").FirstOrDefault();
                string msg = "Dear " + Name + ", You have been booked for a Lab Test. For more detials visit app.";
                Message.SendSms(mobile, msg);
                ent.Database.ExecuteSqlCommand(query);
                rm.Status = 1;
                rm.Message = "Successfully updated";
                return Ok(rm);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }



        //USER SECTION
        [Route("api/LabApi/Booknow")]
        [HttpPost]
        public IHttpActionResult Booknow(Booknow model)
        {
            try
            {
                bookingResponse bookingResponse = new bookingResponse();

                //====GENERATE ORDER NUMBER
                //var lastOrderIdRecord = ent.BookTestLabs.OrderByDescending(a => a.OrderId).FirstOrDefault();
                dynamic lastOrderIdRecord = ent.BookTestLabs.OrderByDescending(a => a.Id).Select(a => a.OrderId).FirstOrDefault();
                string lastOrderId = lastOrderIdRecord != null ? lastOrderIdRecord : "Lab_ord_0"; // Default to "ps_inv_0" if no records exist


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
                string NextOrderId = $"Lab_ord_{OrderIdnumericPart}";

                //====GENERATE INVOICE NUMBER

                dynamic lastRecord = ent.BookTestLabs.OrderByDescending(a => a.Id).Select(a => a.InvoiceNumber).FirstOrDefault();
                //var lastRecord = ent.BookTestLabs.OrderByDescending(a => a.InvoiceNumber).FirstOrDefault();
                string lastInvoiceNumber = lastRecord != null ? lastRecord : "Lab_inv_0"; // Default to "ps_inv_0" if no records exist


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
                string nextInvoiceNumber = $"Lab_inv_{numericPart}";

                var data1 = ent.TestInLabs.Where(a => a.Lab_Id == model.Lab_Id).FirstOrDefault();
                var data = new BookTestLab()
                {
                    Patient_Id = model.Patient_Id,
                    Lab_Id = model.Lab_Id,
                    Slotid = model.Slotid,
                    TestDate = model.TestDate,
                    Test_Id = data1.Test_Id,
                    IsPaid = false,
                    InvoiceNumber = nextInvoiceNumber,
                    OrderId = NextOrderId,
                    OrderDate = DateTime.Now,
                };

                ent.BookTestLabs.Add(data);
                ent.SaveChanges();
                bookingResponse.Message = "Add Successfully";
                bookingResponse.BookingId = data.Id; 
                return Ok(bookingResponse);

                
            }
            catch (Exception e)
            {
                return Ok("Internal server error");
            }
        }

        //USER SECTION
        [Route("api/LabApi/LabAptmt")]
        [HttpGet]
        public IHttpActionResult LabAptmt(int Lab_Id, int Bookid)
        {
            double gst = ent.Database.SqlQuery<double>(@"select Amount from GSTMaster where IsDeleted=0 and Name='Lab'").FirstOrDefault();
            string query = $"select BTL.Id,l.LabName,l.LabTypeName,l.year,LT.TestAmount as Fee,{gst} as GST,LT.TestAmount + LT.TestAmount *{gst}/100 as TotalFee,convert(date,BTL.TestDate)as TestDate,Ts.SlotTime,al.DeviceId from Lab as l left join BookTestLab as BTL on BTL.Lab_Id=l.Id left join TimeSlot as Ts on Ts.Slotid=BTL.Slotid left join LabTest as LT on LT.Id=BTL.Test_Id left join AdminLogin as al on al.Id=l.AdminLogin_Id where l.IsDeleted=0 and BTL.Lab_Id=" + Lab_Id + " and BTL.Id=" + Bookid + "";
            var data = ent.Database.SqlQuery<LabDet>(query).FirstOrDefault();
            return Ok(data);

        }

        //USER SECTION
        [Route("api/LabApi/LabPayNow")]
        [HttpPost]
        public IHttpActionResult LabPayNow(LabPayNow model)
        {
            try
            {
                if (model.IsPaid == true)  //cod=true
                {
                    var data = ent.BookTestLabs.Where(a => a.Lab_Id == model.Lab_Id && a.Id == model.Id).FirstOrDefault();
                    data.Lab_Id = model.Lab_Id;
                    data.Patient_Id = model.Patient_Id;
                    data.Amount = model.Amount;
                    data.PaymentDate = DateTime.Now;
                    data.IsPaid = model.IsPaid;
                    ent.SaveChanges();
                    return Ok(new { Message = "Book Appointment Successfully " });
                }
                else if (model.IsPaid == false)
                {
                    var data = ent.BookTestLabs.Where(a => a.Lab_Id == model.Lab_Id && a.Id == model.Id).FirstOrDefault();
                    data.Lab_Id = model.Lab_Id;
                    data.Patient_Id = model.Patient_Id;
                    data.Amount = model.Amount;
                    data.PaymentDate = DateTime.Now;
                    data.IsPaid = model.IsPaid;
                    ent.SaveChanges();
                    return Ok(new { Message = "Book Appointment Successfully " });
                }

                return Ok("Please check the detail");
            }
            catch (Exception ex)
            {
                return Ok("Internal server error");
            }
        }




        //======lab section============LabBookHistory===============================//
        //done
        [HttpGet]
        public IHttpActionResult LabBookHistory(int id)
        {
            Lab hist = new Lab();
            string query = @"select PA.Id, Patient.PatientName,Patient.MobileNumber,Patient.Location,PA.Totalfee as Amount,PA.StartSlotTime,PA.EndSlotTime from Patient left join PatientAppointment as PA on  Patient.id = PA.Patient_Id  where Patient.id="+ id + " order by PA.Id desc";
            var data = ent.Database.SqlQuery<LAbHISBOOK>(query).FirstOrDefault();
            return Ok(data);
        }
        //=============================LabPaymentHistory ==============================================
        //done
        [Route("api/LabApi/LabPayHis")]
        [HttpGet]
        public IHttpActionResult LabPayHis(int Id)
        {
            //string query = @"select Patient.id,Patient.PatientName,Patient.MobileNumber,PA.Amount,PaymentDate from Patient
            //Left join PatientAppointment as PA on  Patient.id = PA.Patient_Id  where Patient.id=" + Id + "";

            string query = @"select BTL.Id,P.PatientName,P.MobileNumber,BTL.Amount,BTL.PaymentDate,TS.SlotTime from BookTestLab as BTL inner join Patient as P on P.Id=BTL.Patient_Id left join TimeSlot as TS on TS.Slotid=BTL.Slotid where BTL.Lab_Id="+ Id + " order by BTL.Id desc";
            var LabPayHis = ent.Database.SqlQuery<LAbpaymentHistory>(query).ToList();
            return Ok(new { LabPayHis });
        }


        //=================== AppoinmentDetailAPi===============================//
        [Route("api/LabApi/AppoinmentDetail")]
        [HttpGet]
        public IHttpActionResult AppoinmentDetail(int Id)
        {
            var rm = new Lab();
            //         string query = @"select Patient.id, Patient.PatientName,Patient.MobileNumber,Patient.Location,PA.Amount,cm.CityName,PA.PaymentDate,PA.StartSlotTime,PA.EndSlotTime from Patient
            //      left join PatientAppointment as PA on  Patient.id = PA.Patient_Id
            //left join citymaster as cm with(nolock) on cm.id= CityMaster_Id

            //left join LabBooking as LB on  LB.id = LB.Id where LB.id=" + Id + "";

            string query = @"select BTl.Id,P.PatientName,P.MobileNumber,P.Location,cm.CityName,BTL.Amount,BTL.PaymentDate,TS.SlotTime from BookTestLab as BTL inner join Patient as P on P.Id=BTL.Patient_Id left join TimeSlot as TS on TS.Slotid=BTL.Slotid left join citymaster as cm with(nolock) on cm.id=P.CityMaster_Id where BTL.Lab_Id="+ Id +" order by BTL.Id desc";
            var LabAD = ent.Database.SqlQuery<LAbAppoinmentHistory>(query).ToList();

            return Ok(new { LabAD });
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/LabApi/UpdateProfilebyLab")]
        public IHttpActionResult UpdateProfilebyLab(LabUpdate model)
        {

            try
            {
                var data = ent.Labs.Where(a => a.Id == model.id).FirstOrDefault();
                data.LabName = model.LabName;
                data.MobileNumber = model.MobileNumber;
                data.StateMaster_Id = model.StateMaster_Id;
                data.CityMaster_Id = model.CityMaster_Id;
                data.Location = model.Location;
                data.fee = model.fee;
                data.PinCode = model.PinCode;
                var l = ent.BankDetails.Where(a => a.Login_Id == model.adminLogin_id).FirstOrDefault();
                l.AccountNo = model.AccountNo;
                l.BranchName = model.BranchName;
                l.IFSCCode = model.IFSCCode;
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = " lab Profile has updated.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);

        }



        [HttpGet]
        [Route("api/LabApi/LabUpdateProfiledetail")]
        public IHttpActionResult LabUpdateProfiledetail(int Id)
        {
            //var data = new Lab();
            string query = @"select Lab.Id,Lab.LabName,Lab.EmailId,Lab.MobileNumber,Lab.Location,Lab.PinCode,CityName,StateName from Lab left join statemaster as sm with(nolock) on sm.id=stateMaster_Id left join citymaster as cm with(nolock) on cm.id= CityMaster_Id where Lab.Id=" + Id + "";
            var data1 = ent.Database.SqlQuery<labprofileDetails>(query).FirstOrDefault();

            return Ok(data1);
        }

        //=============Appointment History=============//
        [HttpGet]
        [Route("api/LabApi/LabAppoinH")]
        public IHttpActionResult LabAppoinH(int Id)
        {
            var data = new Lab();
            String Query = @"select BTL.Id,P.PatientName,P.MobileNumber,P.Location,BTL.Amount,TS.SlotTime from BookTestLab as BTL inner join Patient as P on P.Id=BTL.Patient_Id left join TimeSlot as TS on TS.Slotid=BTL.Slotid left join citymaster as cm with(nolock) on cm.id=P.CityMaster_Id where BTL.Lab_Id="+ Id + " order by BTL.Id desc";
            var LabApp_History = ent.Database.SqlQuery<labAPTHISTORY>(Query).ToList();
            return Ok(new { LabApp_History });
        }


        //===========LAB ABOUT=============//


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/LabApi/LabAbout")]

        public IHttpActionResult LabAbout(int Id)
        {

            var data = new Lab();
            string qry = @"select About from Lab Where Id=" + Id + " ";
            var LA = ent.Database.SqlQuery<lab_about>(qry).FirstOrDefault();
            return Ok(LA);

        }

        //==========================Upload Report=======================//

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/LabApi/Lab_UploadReport")]
        public IHttpActionResult Lab_UploadReport(upload_report model)
        {
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            try
            {
                if (model.File == null)
                {
                    rm.Message = "Image File Picture can not be null";

                }
                //var img = FileOperation.UploadFile(model.FileName, "images", allowedExtensions);
                var img = FileOperation.UploadFileWithBase64("Images", model.File, model.Filebase64, allowedExtensions);

                if (img == "not allowed")
                {
                    rm.Message = "Only png,jpg,jpeg,pdf files are allowed.";

                    return Ok(rm);

                }
                model.File = img;

                var data1 = ent.BookTestLabs.Where(a => a.Lab_Id == model.Lab_Id).FirstOrDefault();
                var data = new LabReport();
                {
                    data.Lab_Id = data1.Lab_Id;
                    data.Patient_Id = model.Patient_Id;
                    data.Test = model.Test;
                    data.File = model.File = img;


                }

                ent.LabReports.Add(data);
                ent.SaveChanges();
                rm.Status = 0;
                rm.Message = "Report Upload Successfully.";
                return Ok(rm);
            }
            catch (Exception e)
            {
                return Ok("Internal server error");
            }
            return Ok(rm);

        }



        //=====================LabViewReport========================//
        [HttpGet]
        [Route("api/LabApi/Lab_ViewReport")]
        public IHttpActionResult Lab_ViewReport(int Id)
        {
            string qry = @"select LR.Id,P.PatientName,LT.TestName,BTL.TestDate,LR.[File] from Patient as P left join LabReport as LR on LR.Patient_Id=P.Id left join LabTest as LT on LT.Id=LR.Test left join BookTestLab as BTL on LT.Id=BTL.Test_Id where LR.Lab_Id=" + Id + " order by LR.Id desc";
            var LabViewReport = ent.Database.SqlQuery<Lab_View_Report>(qry).ToList();
            return Ok(new { LabViewReport });
        }


        //==========================LabViewReportFile=====================//

        [HttpGet]
        [Route("api/LabApi/Lab_ViewReport_File")]
        public IHttpActionResult Lab_ViewReport_File(int Id)
        {
            var data = new LabReport();
            string qry = @"select [File] from LabReport where Id =" + Id + "";
            var LabViewReport_file = ent.Database.SqlQuery<Lab_View_Report_File>(qry).ToList();

            return Ok(new { LabViewReport_file });
        } 

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/LabApi/LabAddTest")]
        public IHttpActionResult LabAddTest(TestInLab model)
        {
            try
            {
                if (ent.TestInLabs.Any(a => a.Test_Id == model.Test_Id && a.Lab_Id ==model .Lab_Id))
                {
                    return BadRequest("This Test has already Added.");
                }
                var data = new TestInLab();
                {
                    data.Lab_Id = model.Lab_Id;
                    data.Test_Id = model.Test_Id;
                }

                ent.TestInLabs.Add(data);
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = "Add Test Successfully.";
                return Ok(rm);
            }
            catch (Exception e)
            {
                return BadRequest("Internal Server Error");
            }
           
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/LabApi/RemoveTest")]
        public IHttpActionResult RemoveTest(int Id)
        {
            var rm = new ReturnMessage();
            try
            {
                var data = ent.TestInLabs.Find(Id);
                ent.TestInLabs.Remove(data);
                ent.SaveChanges();
                rm.Message = "Successfully deleted";
                rm.Status = 1;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                rm.Status = 0;
                return BadRequest("Internal server error");
            }
            return Ok(rm);
        }

        [HttpGet]
        [Route("api/LabApi/LabTestList")]
        public IHttpActionResult LabTestList(int Id)
        {
            string qry = @"select TL.Id,LT.TestDesc as TestName from TestInLab as TL left join LabTest as LT on LT.Id=TL.Test_Id where TL.Lab_Id=" + Id + " order by TL.Id desc";
            var LabTest = ent.Database.SqlQuery<lablist>(qry).ToList();
            return Ok(new { LabTest });
        }


        [System.Web.Http.HttpGet, System.Web.Http.Route("api/LabApi/LabInvoice/{Invoice}")]
        public IHttpActionResult LabInvoice(string Invoice)
        {
            try
            {
                var gst = ent.GSTMasters.Where(x => x.IsDeleted == false).FirstOrDefault(x => x.Name == "Lab");
                var invoiceData = (from l in ent.Labs
                                   join btl in ent.BookTestLabs on l.Id equals btl.Lab_Id
                                   join lt in ent.LabTests on btl.Test_Id equals lt.Id
                                   where btl.InvoiceNumber == Invoice
                                   select new
                                   {
                                       btl.Patient_Id,
                                       btl.Id,
                                       l.LabName,
                                       lt.TestAmount,
                                       btl.Amount,
                                       GST = gst.Amount,
                                       btl.OrderId,
                                       btl.InvoiceNumber,
                                       btl.OrderDate
                                   }).ToList();

                if (invoiceData.Count > 0)
                {
                    double? totalAmountWithoutGST = invoiceData.Sum(item => item.TestAmount);  
                    double? gstAmount = (totalAmountWithoutGST * gst.Amount) / 100;
                    double? grandTotal = totalAmountWithoutGST + gstAmount; 
                    double? finalamount = grandTotal - gstAmount; 
                    

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
                            finalAmount = finalamount,
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
    }

}
