using HospitalPortal.Models;
using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace HospitalPortal.Controllers
{
    public class NurseServicesController : ApiController
    {
        ILog log = LogManager.GetLogger(typeof(NurseServicesController));
        DbEntities ent = new DbEntities();

        [Route("api/NurseServices/BookAppointment")]
        [HttpPost]
        public IHttpActionResult BookAppointment(TestNurseBooking model)
        {
            var rm = new ReturnMessage();
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ",
                ModelState.Values
                .SelectMany(a => a.Errors)
                .Select(a => a.ErrorMessage));
                rm.Status = 0;
                rm.Message = message;
                return Ok(rm);
            }
            try
            {
                if (model.ServiceType == "12HrSlot")
                {
                    var aptPrms = new SqlParameter[7];
                    aptPrms[0] = new SqlParameter("@patientId", model.PatientId);
                    aptPrms[1] = new SqlParameter("@mobile", model.Mobile);
                    aptPrms[2] = new SqlParameter("@nurseTypeId", model.NurseType_Id);
                    aptPrms[3] = new SqlParameter("@locationId", model.LocationId);
                    aptPrms[4] = new SqlParameter("@serviceDate", model.ServiceDate);
                    aptPrms[5] = new SqlParameter("@serviceType", model.ServiceType);
                    aptPrms[6] = new SqlParameter("@serviceTime", model.ServiceTime);
                    string aptQuery = @"exec [AppointmentTestNurse] @patientId,@mobile,@nurseTypeId,@locationId, @serviceDate, @serviceType, @serviceTime";
                    var result = ent.Database.SqlQuery<int>(aptQuery, aptPrms).FirstOrDefault();
                    if (result > 0)
                    {
                        rm.Status = 1;
                        rm.Message = "We have recieved your request.Please wait untill it confirm.";
                    }
                }
                else if (model.ServiceType == "Other")
                {
                    var aptPrms = new SqlParameter[7];
                    aptPrms[0] = new SqlParameter("@patientId", model.PatientId);
                    aptPrms[1] = new SqlParameter("@mobile", model.Mobile);
                    aptPrms[2] = new SqlParameter("@startDate", model.StartDate);
                    aptPrms[3] = new SqlParameter("@endDate", model.EndDate);
                    aptPrms[4] = new SqlParameter("@nurseTypeId", model.NurseType_Id);
                    aptPrms[5] = new SqlParameter("@locationId", model.LocationId);
                    aptPrms[6] = new SqlParameter("@serviceType", model.ServiceType);
                    string aptQuery = @"exec AppointmentWithNurse @patientId,@mobile,@startDate,@endDate,@nurseTypeId,@locationId, @serviceType";
                    var result = ent.Database.SqlQuery<int>(aptQuery, aptPrms).FirstOrDefault();
                    if (result > 0)
                    {
                        rm.Status = 1;
                        rm.Message = "We have recieved your request.Please wait untill it confirm.";
                    }
                }
                else if (model.ServiceType == "24Hrs")
                {
                    var aptPrms = new SqlParameter[6];
                    aptPrms[0] = new SqlParameter("@patientId", model.PatientId);
                    aptPrms[1] = new SqlParameter("@mobile", model.Mobile);
                    aptPrms[2] = new SqlParameter("@nurseTypeId", model.NurseType_Id);
                    aptPrms[3] = new SqlParameter("@locationId", model.LocationId);
                    aptPrms[4] = new SqlParameter("@serviceDate", model.ServiceDate);
                    aptPrms[5] = new SqlParameter("@serviceType", model.ServiceType);
                    string aptQuery = @"exec [AppointmentNurseFor24Hrs] @patientId,@mobile,@nurseTypeId,@locationId, @serviceDate, @serviceType";
                    var result = ent.Database.SqlQuery<int>(aptQuery, aptPrms).FirstOrDefault();
                    if (result > 0)
                    {
                        rm.Status = 1;
                        rm.Message = "We have recieved your request.Please wait untill it confirm.";
                    }
                }
                else
                {
                    rm.Status = 0;
                    rm.Message = "Error in server.Please contact to admin";
                }

                // now notify all the nurses

                var Locationdata = ent.Locations.Find(model.LocationId);
                var CityData = ent.CityMasters.Find(Locationdata.City_Id);
                var StateData = ent.StateMasters.Find(CityData.StateMaster_Id);
                string msg;
                string nurseQuery = @"select mobilenumber from Nurse n
                join Nurse_Location nl on n.Id = nl.Nurse_Id
                where n.NurseType_id=" + model.NurseType_Id + " and nl.Location_Id=" + model.LocationId + " and n.IsApproved=1 and n.IsDeleted=0";
                if (model.ServiceType == "12HrSlot")
                {
                    msg = @"You have enquiry for services date between" + model.ServiceDate.Value.ToString("dd-MM-yyyy") + " for " + model.ServiceTime + ".Please Login to portal to accept the services";
                }
                else if (model.ServiceType == "24Hrs")
                {
                    msg = @"You have enquiry for services date between" + model.ServiceDate.Value.ToString("dd-MM-yyyy") + ".Please Login to portal to accept the services";
                }
                else
                {
                     msg = @"You have enquiry for services date between" + model.StartDate.Value.ToString("dd-MM-yyyy") + " and " + model.EndDate.Value.ToString("dd-MMM-yyyy") + ".Please Login to portal to accept the services";
                }
                var nurseMobiles = ent.Database.SqlQuery<string>(nurseQuery).ToList();

                nurseMobiles.ForEach(phoneNo =>
                {
                    Message.SendSms(phoneNo, msg);
                });
                return Ok(rm);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("api/NurseServices/NurseAppointmentList")]
        [HttpGet]
        public IHttpActionResult NurseAppointmentList(int patientId, DateTime? RequestDate = null)
        {
            try
            {
                dynamic obj = new ExpandoObject();
                string query = @"select ns.Id, ns.ServiceStatus, ns.Nurse_Id, n.MobileNumber, ns.Patient_Id, ns.IsPaid, case when ns.PaymentDate is null then 'N/A' else Convert(nvarchar(100),ns.PaymentDate,103) end as PaymentDate, Convert(nvarchar(100),ns.RequestDate,103) as RequestDate,'From '+ Convert(nvarchar(100),ns.StartDate,103)+' to '+Convert(nvarchar(100),ns.EndDate,103) as ServiceTiming ,IsNull(n.NurseName,'N/A') as NurseName,
IsNull(n.MobileNumber,'N/A') as NurseMobileNumber,
Convert(nvarchar(100), ns.StartDate, 103) + '-' + Convert(nvarchar(100), ns.EndDate, 103) as ServiceTiming,
Convert(nvarchar(100), ns.ServiceDate, 103) as ServiceAcceptanceDate,
Datediff(day,ns.StartDate,ns.EndDate) as TotalDays,
IsNull(TotalFee,0) as TotalFee,
ns.ServiceType,
IsNull(ns.ServiceTime, 'N/A') as ServiceTime,
ns.ServiceStatus
 from NurseService ns 
left join Nurse n on ns.Nurse_Id=n.Id
where ns.Patient_Id="+patientId+ " and ns.ServiceStatus='Approved' order by ns.Id desc";
                var apts = ent.Database.SqlQuery<NurseAppointmentList>(query).ToList();
                if(RequestDate != null)
                {
                    string date = String.Format("{0:dd/MM/yyyy}", RequestDate);
                    apts = apts.Where(a => a.RequestDate == date).ToList();
                    if(apts.Count() == 0)
                    {
                        obj.AppointmentList = apts;
                        obj.Message = "No Data";
                        return Ok(obj);
                    }
                }
                obj.AppointmentList = apts;
                return Ok(obj);
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
                return InternalServerError(ex);
            }
        }


        [Route("api/NurseServices/UpdatePaymentStatus")]
        [HttpGet]
        public IHttpActionResult UpdatePaymentStatus(int serviceId)
        {
            try
            {
                var rm = new ReturnMessage();
                var data = ent.NurseServices.Find(serviceId);
                if (data != null)
                {
                    string query = @"update NurseService set PaymentDate=getdate(),IsPaid=1 where Id=" + serviceId;
                    int Id = ent.Database.SqlQuery<int>(@"select Nurse_Id from NurseService where Id='" + serviceId + "'").FirstOrDefault();
                    int PatientId = ent.Database.SqlQuery<int>(@"select Patient_Id from NurseService where Id='" + serviceId + "'").FirstOrDefault();
                    //DateTime StartDate = ent.Database.SqlQuery<DateTime>(@"select Convert(date,StartDate) from NurseService where Id='" + TestId + "'").FirstOrDefault();
                    //DateTime data = Convert.ToDateTime(Date);
                    string mobile = ent.Database.SqlQuery<string>(@"select MobileNumber from Nurse where Id='" + Id + "'").FirstOrDefault();
                    string Name = ent.Database.SqlQuery<string>(@"select PatientName from Patient where Id='" + PatientId + "'").FirstOrDefault();
                    string msg = "Dear Nurse, Patient '" + Name + "' has been Paid for his/her Appointment.";
                    Message.SendSms(mobile, msg);
                    ent.Database.ExecuteSqlCommand(query);
                    rm.Status = 1;
                    rm.Message = "Successfully updated";
                    return Ok(rm);
                }
                else
                {
                    rm.Status = 0;
                    rm.Message = "Invaild Attempt";
                    return Ok(rm);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("api/NurseServices/NurseServices")]
        [HttpPost]
        public IHttpActionResult NurseServices(NurseBook model)
        {
            try
            {
                NursebookingResponse bookingResponse = new NursebookingResponse();
                var data = new NurseService()
                {
                    Patient_Id = model.Patient_Id,
                    NurseTypeId = model.NurseTypeId,
                    ServiceType = model.ServiceType,
                    ServiceTime = model.ServiceTime,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    PaymentDate = DateTime.Now,
                    ServiceAcceptanceDate = DateTime.Now,
                    RequestDate = DateTime.Now,
                    ServiceDate = DateTime.Now,
                    MobileNumber = model.MobileNumber,
                    LocationId = model.LocationId,
                    Location = model.Location,
                    StateMaster_Id = model.StateMaster_Id,
                    CityMaster_Id = model.CityMaster_Id,
                };
                ent.NurseServices.Add(data);
                ent.SaveChanges();
                TimeSpan duration = (TimeSpan)(model.EndDate - model.StartDate);
                int NumberOfDays = duration.Days;


                bookingResponse.Message = "Add Successfully";
                bookingResponse.NurseBookingId = data.Id; 

                var response = new
                {
                    BookingResponse = bookingResponse,
                    NurseTypeId = model.NurseTypeId,
                    StateMaster_Id = model.StateMaster_Id,
                    CityMaster_Id = model.CityMaster_Id,
                    NumberOfDay = NumberOfDays,
                };

                return Ok(response);
                //return Ok(bookingResponse);
            }
            catch (Exception ex)
            {
                return BadRequest("Internal server error");
            }
        }

        [Route("api/NurseServices/NurseBookings")]
        [HttpPost]
        public IHttpActionResult NurseBookings(NurseBooking model)
        {
            try
            {
                NursebookingResponse bookingResponse = new NursebookingResponse();

                //====GENERATE ORDER NUMBER
                //var lastOrderIdRecord = ent.NurseServices.OrderByDescending(a => a.OrderId).FirstOrDefault();
                dynamic lastOrderIdRecord = ent.NurseServices.OrderByDescending(a => a.Id).Select(a => a.OrderId).FirstOrDefault();
                string lastOrderId = lastOrderIdRecord != null ? lastOrderIdRecord : "N_ord_0"; // Default to "ps_inv_0" if no records exist


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
                string NextOrderId = $"N_ord_{OrderIdnumericPart}";

                //====GENERATE INVOICE NUMBER

                //var lastRecord = ent.NurseServices.OrderByDescending(a => a.InvoiceNumber).FirstOrDefault();
                dynamic lastRecord = ent.NurseServices.OrderByDescending(a => a.Id).Select(a => a.InvoiceNumber).FirstOrDefault();
                string lastInvoiceNumber = lastRecord != null ? lastRecord : "N_inv_0"; // Default to "ps_inv_0" if no records exist


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
                string nextInvoiceNumber = $"N_inv_{numericPart}";

                var data1 = ent.NurseServices.Where(a => a.Id == model.Id).FirstOrDefault();
                if (data1 != null)
                {
                    data1.Patient_Id = data1.Patient_Id;
                    data1.Nurse_Id = model.Nurse_Id;
                    data1.Slotid = model.Slotid;
                    //data1.StartDate = model.StartDate;
                    //data1.EndDate = model.EndDate;
                    data1.PaymentDate = DateTime.Now;
                    data1.ServiceAcceptanceDate = DateTime.Now;
                    data1.RequestDate = DateTime.Now;
                    data1.ServiceDate = model.ServiceDate;
                    data1.MobileNumber = data1.MobileNumber;
                    data1.ServiceStatus = "Approved";
                    data1.InvoiceNumber  = nextInvoiceNumber;
                    data1.OrderId = NextOrderId;
                    data1.OrderDate = DateTime.Now;
                    ent.SaveChanges();
                    bookingResponse.Message = "Add Successfully";
                    bookingResponse.NurseBookingId = data1.Id;
                    return Ok(bookingResponse);
                }
                else
                {
                    return BadRequest("Please check the detail");
                }

            }
            catch (Exception e)
            {
                return Ok("Internal server error");
            }
        }


        [Route("api/NurseServices/NurseAptmt")]
        [HttpGet]
        public IHttpActionResult NurseAptmt(int Nurse_Id, int NurseBookingId)
        {
            var model = new Nurse();
            double gst = ent.Database.SqlQuery<double>(@"select Amount from GSTMaster where IsDeleted=0 and Name='Nurse'").FirstOrDefault();
            string query = $"select Ns.id,Nurse.NurseName,Nt.NurseTypeName,Nurse.Experience,Nurse.Fee,DATEDIFF(day, ns.StartDate, ns.EndDate)  as TotalNumberofdays,{gst} as GST,Nurse.Fee * DATEDIFF(day, ns.StartDate, ns.EndDate)  as TotalFee,Nurse.Fee * DATEDIFF(day, ns.StartDate, ns.EndDate) + (Nurse.Fee *{gst}/100) as TotalFeeWithGST,Ns.serviceDate,Ts.SlotTime from Nurse left join NurseType as Nt on  Nurse.NurseType_Id = Nt.id  left join NurseService as Ns on Ns.Nurse_Id=Nurse.Id left join TimeSlot as Ts on Ts.Slotid=Ns.Slotid where Nurse.IsDeleted=0 and Ns.Nurse_Id=" + Nurse_Id + " and Ns.id =" + NurseBookingId + "";
            //string query = $"select Ns.id,Nurse.NurseName,Nt.NurseTypeName,Nurse.Experience,Nurse.Fee,{gst} as GST,Nurse.Fee * DATEDIFF(day, ns.StartDate, ns.EndDate) + (Nurse.Fee *{gst}/100) as TotalFee,Ns.serviceDate,Ts.SlotTime from Nurse left join NurseType as Nt on  Nurse.NurseType_Id = Nt.id  left join NurseService as Ns on Ns.Nurse_Id=Nurse.Id left join TimeSlot as Ts on Ts.Slotid=Ns.Slotid where Nurse.IsDeleted=0 and Ns.Nurse_Id=" + Nurse_Id + " and Ns.id =" + NurseBookingId + "";
            var data = ent.Database.SqlQuery<NurseAptmt>(query).FirstOrDefault();
            return Ok(data);

        }
        [Route("api/NurseServices/NursePayNow")]
        [HttpPost]
        public IHttpActionResult NursePayNow(NursePayNow model)
        {
            try
            {
                if (model.IsPaid == true)
                {
                    var data = ent.NurseServices.Where(a => a.Nurse_Id == model.Nurse_Id && a.Id == model.Id).FirstOrDefault();
                    data.Nurse_Id = model.Nurse_Id;
                    data.Patient_Id = model.Patient_Id;
                    data.TotalFee = model.TotalFee;
                    data.PaymentDate = DateTime.Now;
                    data.IsPaid = model.IsPaid;
                    ent.SaveChanges();
                    return Ok(new { Message = "Book Appointment Successfully " });
                }
                else if (model.IsPaid == false)
                {
                    var data = ent.NurseServices.Where(a => a.Nurse_Id == model.Nurse_Id && a.Id == model.Id).FirstOrDefault();
                    data.Nurse_Id = model.Nurse_Id;
                    data.Patient_Id = model.Patient_Id;
                    data.TotalFee = model.TotalFee;
                    data.PaymentDate = DateTime.Now;
                    data.IsPaid = model.IsPaid;
                    ent.SaveChanges();
                    return Ok(new { Message = "Book Appointment Successfully " });
                }
                return BadRequest("Please check the detail");
            }
            catch (Exception e)
            {
                return BadRequest("Internal server error");
            }
            //return BadRequest("");
        }
    }
}
