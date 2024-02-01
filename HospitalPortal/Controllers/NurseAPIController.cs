 using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Razor.Parser.SyntaxTree;

namespace HospitalPortal.Controllers
{
    public class NurseAPIController : ApiController
    {

        returnMessage rm = new returnMessage();
        DbEntities ent = new DbEntities();
        //private object model;

        [HttpGet]
        [Route("api/NurseAPI/NurseAppointmentList")]
        public IHttpActionResult NurseAppointmentList(int NurseId, DateTime? startDate = null, DateTime? endDate = null, string term = null)
        {
            var model = new NurseAppointmentModel();
            string query = @"select ns.Id,p.PatientName,p.Location,
                             p.MobileNumber as RegisteredMobileNumber,
                             ns.MobileNUmber as ContactNumber,
                             ns.IsPaid,
case when ns.PaymentDate is null then 'N/A' else Convert(nvarchar(100), ns.PaymentDate, 103) end as PaymentDate,
case when ns.ServiceAcceptanceDate is null then 'N/A' else Convert(nvarchar(100), ns.ServiceAcceptanceDate, 103) end as ServiceAcceptanceDate,
Convert(nvarchar(100), ns.RequestDate, 103) as RequestDate,
ns.RequestDate as rDate,
Convert(nvarchar(100), ns.StartDate, 103) + '-' + Convert(nvarchar(100), ns.EndDate, 103) as ServiceTiming,
Convert(nvarchar(100), ns.ServiceDate, 103) as ServiceDate,
Datediff(day,ns.StartDate,ns.EndDate) as TotalDays,
IsNull(ns.PerDayAmount,0) as Fee,
IsNull(TotalFee,0) as TotalFee,
ns.ServiceType,
IsNull(ns.ServiceTime,'N/A') as ServiceTime,
ns.ServiceStatus
 from NurseService ns
 join Patient p on ns.Patient_Id = p.Id
 join nurse n on ns.Nurse_Id=n.Id
where ns.Nurse_Id =n.Id
and ns.Nurse_Id =" + NurseId + " and ns.ServiceStatus='Approved' order by ns.Id desc";
            var data = ent.Database.SqlQuery<NurseAppointmentWithUser>(query).ToList();
            if (term != null)
            {
                term = term.ToLower();
                data = data.Where(a => a.PatientName.ToLower().Contains(term) || a.RegisteredMobileNumber.StartsWith(term) || a.ContactNumber.StartsWith(term)).ToList();
            }
            if (startDate != null && endDate != null)
                data = data.Where(a => a.rDate >= startDate && a.rDate <= endDate).ToList();
            return Ok(data);
        }

        [Route("api/NurseAPI/NurseAppointmentRequests")]
        [HttpGet]
        public IHttpActionResult NurseAppointmentRequests(int NurseTypeId, int NurseId)
        {
            string query = @"exec GetNurseAppointmentList " + NurseTypeId + "," + NurseId;
            var data = ent.Database.SqlQuery<NurseAppointmentRequestList>(query).ToList();
            return Ok(data);
        }

        [Route("api/NurseAPI/AcceptAppointent")]
        [HttpGet]
        public IHttpActionResult AcceptAppointent(int serviceId, int nurseId)
        {
            try
            {
                var record = ent.NurseServices.Find(serviceId);
                double nurseFee = ent.Nurses.Find(nurseId).Fee;
                double nurse12HrFee = Convert.ToDouble(ent.Nurses.Find(nurseId).HrsFeex24);
                double nurseMonthFee = Convert.ToDouble(ent.Nurses.Find(nurseId).MonthFee);
                if (record.ServiceType == "Other")
                {
                    string query = @"update NurseService set Nurse_Id=" + nurseId + ",ServiceAcceptanceDate=getdate(),ServiceStatus='Approved' where Id=" + serviceId;
                    ent.Database.ExecuteSqlCommand(query);
                    double Feeamt = ent.Database.SqlQuery<double>(@"select IsNull(Datediff(day,ns.StartDate,ns.EndDate)*n.Fee,0) as TotalFee from NurseService ns join nurse n on ns.Nurse_Id=n.Id where ns.Id = " + serviceId).FirstOrDefault();
                    string query1 = @"update NurseService set TotalFee=" + Feeamt + " where Id=" + serviceId;
                    ent.Database.ExecuteSqlCommand(query1);
                }
                else if (record.ServiceType == "12HrSlot")
                {
                    string query = @"update NurseService set Nurse_Id=" + nurseId + ",ServiceAcceptanceDate=getdate(),ServiceStatus='Approved',TotalFee=" + nurse12HrFee + " where Id=" + serviceId;
                    ent.Database.ExecuteSqlCommand(query);
                }
                else
                {
                    string query = @"update NurseService set Nurse_Id=" + nurseId + ",ServiceAcceptanceDate=getdate(),ServiceStatus='Approved',TotalFee=" + nurseMonthFee + " where Id=" + serviceId;
                    ent.Database.ExecuteSqlCommand(query);
                }

                // send message to customer
                string msg = "Your request to Nursing Service has approved.Please Check detail  and make payment through Application";
                var mobile = ent.Database.SqlQuery<string>(@"select 
ns.MobileNumber from nurseservice ns 
join Patient p on ns.Patient_Id=p.Id
where ns.Id=
" + serviceId).FirstOrDefault();
                Message.SendSms(mobile, msg);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                rm.Message = "Server Error";
                rm.Status = 0;
                return Ok(rm);

            }
            rm.Status = 1;
            rm.Message = "Successfully Update Current Status";
            return Ok(rm);
        }

        [Route("api/NurseAPI/DeclineAppointent")]
        [HttpGet]
        public IHttpActionResult DeclineAppointent(int serviceId)
        {
            try
            {
                string query = @"update NurseService set ServiceStatus='Declined' where Id=" + serviceId;
                ent.Database.ExecuteSqlCommand(query);
                // send message to customer
                string msg = "Your request to Nursing Service has been Decline. Please Check details in Application";
                var mobile = ent.Database.SqlQuery<string>(@"select 
ns.MobileNumber from nurseservice ns 
join Patient p on ns.Patient_Id=p.Id
where ns.Id=
" + serviceId).FirstOrDefault();
                Message.SendSms(mobile, msg);
            }
            catch (Exception ex)
            {
                rm.Message = "Server Error";
                rm.Status = 0;
                return Ok(rm);

            }
            rm.Status = 1;
            rm.Message = "Successfully Update Current Status";
            return Ok(rm);
        }

        [Route("api/NurseAPI/ChangeApprovalStatus")]
        [HttpPost]
        public IHttpActionResult ChangeApprovalStatus(int serviceId)
        {
            string query = @"update NurseService set ServiceStatus= case When ServiceStatus='Approved' then 'Declined'
when ServiceStatus='Declined' then 'Approved'
else 'Pending' end where Id=" + serviceId;
            try
            {
                ent.Database.ExecuteSqlCommand(query);
            }
            catch (Exception ex)
            {
                rm.Message = "Server Error";
                rm.Status = 0;
                return Ok(rm);
            }
            rm.Status = 1;
            rm.Message = "Successfully Update Current Status";
            return Ok(rm);
        }
        //Patient APi
        [Route("api/NurseAPI/GetLocation")]
        [HttpGet]
        public IHttpActionResult GetLocation(int cityId, int NurseId)
        {
            string query = @"select Location.* From Location where Id not in (select Location_Id from Nurse_Location where Nurse_Id = " + NurseId + ") and IsDeleted=0 and City_Id=" + cityId;
            var locs = ent.Database.SqlQuery<LocationDTO>(query).ToList();
            if (locs.Count() == 0)
            {
                rm.Message = "No Records";
                rm.Status = 0;
                return Ok(rm);
            }
            return Ok(locs);
        }
        //Patient APi
        [Route("api/NurseAPI/Location")]
        [HttpGet]
        public IHttpActionResult Location(int NurseId)
        {
            var model = new NurseLocationAPI();
            string query = @"select nl.Id,nl.Nurse_Id,l.LocationName from Nurse_Location nl
join Location l on nl.Location_Id=l.Id
 where nl.Nurse_Id=" + NurseId;
            var data = ent.Database.SqlQuery<NurseLocationModel>(query).ToList();
            if (data.Count() == 0)
            {
                model.Message = "No Records";
                model.Status = 0;
                model.NurseLocation = data;
                return Ok(model);
            }
            model.Message = "Success";
            model.Status = 1;
            model.NurseLocation = data;
            return Ok(model);
        }

        //===== Anchal shukla ==== [12/02/23]
        [Route("api/NurseAPI/RegisterLocation")]
        [HttpPost]
        public IHttpActionResult RegisterLocation(RegisterNurseLocationApi model)
        {
            try
            {
                foreach (var locationId in model.LocationIds)
                {
                    var domain = new Nurse_Location
                    {
                        Location_Id = locationId.LocationId,
                        Nurse_Id = model.NurseId
                    };
                    ent.Nurse_Location.Add(domain);
                    ent.SaveChanges();
                }
                rm.Message = "Location Added successfully.";
                rm.Status = 1;
            }
            catch (Exception ex)
            {
                rm.Message = "Server error";
                rm.Status = 0;
                return Ok(rm);
            }
            return Ok(rm);
        }
        //======[anchal shukla]=============18/04/2023======================= //
        [HttpGet]
        [Route("api/NurseAPI/AppoinmentHistory")]
        public IHttpActionResult AppoinmentHistory(int ID)
        {
            var rm = new PatientAppointment();
            string query = @"Select NS.Id,P.PatientName,P.MobileNumber,P.Location,NS.TotalFee as Amount,NS.StartDate,NS.EndDate from Patient as P
                   left join NurseService as NS on NS.Patient_Id=P.Id
                left join Nurse as N on N.Id=NS.Nurse_Id 
                where N.Id=" + ID + " and NS.ServiceStatus='Approved' order by NS.Id desc";
            var data = ent.Database.SqlQuery<AppoinmentHistory>(query).ToList();
            return Ok(new { data });
        }

        [HttpGet]
        public IHttpActionResult PaymentHistory(int NurseID)
        {

            var rm = new Nurse();
 //           string query = @" select N.Id,P.PatientName,P.MobileNumber,PA.Amount,PA.PaymentDate from Patient as P
 //left join NurseService as NS On NS.Patient_Id=P.Id
 //left join Nurse as N on N.Id=NS.Nurse_Id
 //Left join PatientAppointment as PA on  P.id = PA.Patient_Id  
 //Where N.Id=" + NurseID + "";

            string query = @"select NS.Id,P.PatientName,P.MobileNumber,NS.TotalFee as Amount,NS.PaymentDate from Patient as P left join NurseService as NS On NS.Patient_Id=P.Id left join Nurse as N on N.Id=NS.Nurse_Id Where N.Id=" + NurseID + " and NS.ServiceStatus='Approved' order by NS.Id desc";
            var NurseHistory = ent.Database.SqlQuery<PAYMENTHISTORY>(query).ToList();
            return Ok(new { NurseHistory });
        }
        //dropdown
        [HttpGet]
        [Route("api/NurseAPI/Patientlist")]
        public IHttpActionResult Patientlist(int ID)
        {
            var rm = new Patient();
            
            string query = @"select Ns.Id,P.PatientName,P.MobileNumber,N.Fee,P.Location,Ns.StartDate,Ns.EndDate from Nurse as N inner join NurseService Ns on Ns.Nurse_Id=N.Id inner join Patient as P on NS.Patient_Id=P.Id where N.Id=" + ID + " and NS.ServiceStatus='Approved' order by Ns.Id desc";
            var data = ent.Database.SqlQuery<NursePatientList>(query).ToList();
            return Ok(data);
        }

        [Route("api/NurseAPI/getNurseList")]
        [HttpGet]
        public IHttpActionResult getNurseList(int State_Id,int City_Id,int NurseType_Id)
        {
            try
            {
                var model = new NurseListV();

                //string query = @"select Nurse.id,Nurse.NurseName,Nurse.Fee,Nurse.About,Nurse.Experience,Nt.NurseTypeName ,(select avg(rating1 + rating2 + rating3 + rating4 + rating5) from Review where Review.pro_Id=Nurse.Id) As Rating  from Nurse
                //               left join NurseType as Nt on Nurse.NurseType_Id = Nt.id                            
                //               where Nurse.IsDeleted = 0 and Nurse.Location_id='" + Loc_id + "'group by Nurse.id,Nurse.NurseName,Nurse.Fee,Nurse.About,Nurse.Experience,Nt.NurseTypeName";
                string query = @"select Nurse.id,Nurse.NurseName,Nurse.Fee,Nurse.About,Nurse.Experience,Nt.NurseTypeName ,(select avg(rating1 + rating2 + rating3 + rating4 + rating5) from Review where Review.pro_Id=Nurse.Id) As Rating  from Nurse
                               left join NurseType as Nt on Nurse.NurseType_Id = Nt.id                            
                               where Nurse.IsDeleted = 0 and Nurse.StateMaster_Id="+ State_Id + " and Nurse.CityMaster_Id="+ City_Id + " and Nurse.NurseType_Id="+ NurseType_Id + " and Nurse.IsApproved=1 group by Nurse.id,Nurse.NurseName,Nurse.Fee,Nurse.About,Nurse.Experience,Nt.NurseTypeName";
                var data = ent.Database.SqlQuery<NurseDetail>(query).ToList();
                model.NurseLists = data;
                return Ok(model);
            }
            catch
            {
                return BadRequest("Server Error");
            }
        }

        [HttpGet]
        public IHttpActionResult NurseDetails(int id)
        {
            var model = new Nurse();
            //string query = @"select Nurse.id,Nurse.NurseName,Nurse.Fee,Nurse.About,Nurse.Experience,Nt.NurseTypeName from Nurse left join NurseType as Nt on  Nurse.NurseType_Id = Nt.id where Nurse.id=" + id + "";
            string query= @"select Nurse.id,Nurse.NurseName,Nurse.Fee,Nurse.About,Nurse.Experience,Nt.NurseTypeName ,Avg(Re.rating1 + Re.rating2 + Re.rating3 + Re.rating4 + Re.rating5)As rating from Nurse
                          left join Review as Re on Nurse.Id = Re.pro_Id
                          left join NurseType as Nt on Nurse.NurseType_Id = Nt.id where Nurse.id ='" + id + "'group by Nurse.id,Nurse.NurseName,Nurse.Fee,Nurse.About,Nurse.Experience,Nt.NurseTypeName";
            var data = ent.Database.SqlQuery<NurseDetail>(query).FirstOrDefault();
            return Ok(data);
        }


        // //New API For Get NurseUpdateprofile  By Anchal Shukla on 20/4/2023

        [HttpGet]

        [Route("api/NurseAPI/GetNurseProfile")]
        
        public IHttpActionResult GetNurseProfile(int Id)
        {
            var rm = new Nurse();
          string query= @" select  Nurse.id,Nurse.NurseName,Nurse.EmailId,Nurse.MobileNumber,Nurse.Location,Nurse.PinCode ,sm.StateName,cm.CityName from Nurse as Nurse with(nolock)
          left join citymaster as cm with(nolock) on cm.id = Nurse.CityMaster_Id
        left join statemaster as sm with(nolock) on sm.id = Nurse.stateMaster_Id where Nurse.id = " + Id + "";
            var NurseProfile = ent.Database.SqlQuery<GetNurseProfile>(query).FirstOrDefault();
            return Ok( NurseProfile );
        }

        //New API For post NurseUpdate  By Anchal Shukla on 20/4/2023
        [HttpPost]
        [Route("api/NurseAPI/UpdateNurseProfile")]
        public IHttpActionResult UpdateNurseProfile(NurseUpdate model)
        {
            
            try
            {
                              
                var data = ent.Nurses.Where(a => a.Id == model.Id).FirstOrDefault();
                data.NurseName = model.NurseName;
                data.MobileNumber = model.MobileNumber;
                data.StateMaster_Id = model.StateMaster_Id;
                data.CityMaster_Id = model.CityMaster_Id;
                data.Location = model.Location;
                data.PinCode = model.PinCode;
               
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = " Nurse Profile has been updated.";
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }

        //=================NURSE UPLOAD REPORT==============//

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/NurseAPI/Nurse_UploadReport")]
        public IHttpActionResult Nurse_UploadReport(Nurse_uploadreport model)
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
                var data = new Nurse_Rep();
                {
                    data.Nurse_Id = model.Nurse_Id;
                    data.Patient_Id = model.Patient_Id;
                    data.File = model.File = img;
                }

                ent.Nurse_Rep.Add(data);
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

        //============NurseViewReport=============//

        [HttpGet]
        [Route("api/NurseAPI/Nurse_ViewReport")]
        public IHttpActionResult Nurse_ViewReport(int Id)
        {
            string qry = @"select NR.Id,P.PatientName,NR.[File] from Patient as P left join Nurse_Rep as NR on NR.Patient_Id=P.Id where NR.Nurse_Id=" + Id + " order by NR.Id desc";
            var NurseViewReport = ent.Database.SqlQuery<Nurse_View_Report>(qry).ToList();
            return Ok(new { NurseViewReport });
        }

        //===============NURSE ABOUT============//

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/NurseAPI/Nurse_About")]

        public IHttpActionResult Nurse_About(int Id)
        {
            string qry = @"select Id,about from Nurse where IsDeleted=0 and Id=" + Id + "";
            var NurseAbout = ent.Database.SqlQuery<NurseAbout>(qry).FirstOrDefault();
            return Ok(NurseAbout);
        }

        //===========NurseViewReportFile=======//
        [HttpGet]
        [Route("api/NurseAPI/Nurse_ViewReport_File")]
        public IHttpActionResult Nurse_ViewReport_File(int Id)
        {
            string qry = @"select [File] from Nurse_Rep where Id=" + Id + "";
            var NurseViewReport_file = ent.Database.SqlQuery<Nurse_View_Report_File>(qry).ToList();

            return Ok(new { NurseViewReport_file });
        }

        //===============NURSE UPLOAD REPORT==PATIENTNAME DROPDOWN====================

        [System.Web.Http.HttpGet, System.Web.Http.Route("api/NurseAPI/NursePatientList")]
        public IHttpActionResult NursePatientList(int Id)
        {
            var qry = @"select p.Id,p.PatientName from Patient as p 
left join NurseService as NS on NS.Patient_Id=p.id
where NS.Nurse_Id=" + Id + "";
            var PatientName = ent.Database.SqlQuery<Labrepo>(qry).ToList();
            return Ok(new { PatientName });
        }

        //==========================Appointment History=====================//
        [HttpGet]
        [Route("api/NurseAPI/NurseAppointmentHistory")]
        public IHttpActionResult NurseAppointmentHistory(int Id)
        {
            string qry = @"Select NS.Id,P.PatientName,P.MobileNumber,P.Location,NS.TotalFee,NS.ServiceDate,TS.SlotTime from Patient as P left join NurseService as NS on NS.Patient_Id=P.Id left join Nurse as N on N.Id=NS.Nurse_Id left join TimeSlot as TS on TS.Slotid=NS.Slotid where N.Id="+ Id + " and ServiceStatus='Approved' order by NS.Id desc";
            var AppointmentHistory = ent.Database.SqlQuery<Apointmenthsitory>(qry).ToList();

            return Ok(new { AppointmentHistory });
        }


        [System.Web.Http.HttpGet, System.Web.Http.Route("api/NurseAPI/NurseInvoice/{Invoice}")]
        public IHttpActionResult NurseInvoice(string Invoice)
        {
            try
            {
                var gst = ent.GSTMasters.Where(x => x.IsDeleted == false).FirstOrDefault(x => x.Name == "Nurse");
                var invoiceData = (from n in ent.Nurses
                                   join ns in ent.NurseServices on n.Id equals ns.Nurse_Id
                                   where ns.InvoiceNumber == Invoice
                                   select new
                                   {
                                       ns.Patient_Id,
                                       ns.Id,
                                       n.NurseName,
                                       TotalFee = (n.Fee) * DbFunctions.DiffDays(ns.StartDate, ns.EndDate) + (n.Fee*(gst.Amount / 100)),
                                       n.Fee,
                                       //ns.TotalFee,
                                       GST = gst.Amount,
                                       ns.OrderId,
                                       ns.InvoiceNumber,
                                       ns.OrderDate,
                                       NumberOfDays = DbFunctions.DiffDays(ns.StartDate, ns.EndDate)
                                   }).ToList();

                if (invoiceData.Count > 0)
                {
                    double? totalAmountWithoutGST = invoiceData.Sum(item => item.Fee);
                    double? gstAmount = (totalAmountWithoutGST * gst.Amount) / 100;
                    double? grandTotal = invoiceData.Sum(item => item.Fee) * invoiceData.Sum(item => item.NumberOfDays) + gstAmount;
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
                            NumberOfDays = invoiceData[0].NumberOfDays,
                            GST = invoiceData[0].GST,
                            GSTAmount = gstAmount,
                            GrandTotal = grandTotal,
                            Finalamount = finalamount,
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