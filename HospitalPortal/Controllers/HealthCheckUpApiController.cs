using HospitalPortal.BL;
using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
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
    public class HealthCheckUpApiController : ApiController
    {
        ILog log = LogManager.GetLogger(typeof(HealthCheckUpApiController));
        DbEntities ent = new DbEntities();
        ReturnMessage rm = new ReturnMessage();
        GenerateBookingId Book = new GenerateBookingId();
        Rmwithparm rwithprm = new Rmwithparm();

        //Patient will get the list the HealthCheckupCenters using City Id
        //[HttpGet]
        //public IHttpActionResult H_CheckUpList(int cityId)
        //{
        //    var model = new HealthCheckUpVM();
        //    string qry = @"select HealthCheckupCenter.Id,HealthCheckupCenter.LabName,HealthCheckupCenter.Location,
        //             HealthCheckupCenter.MobileNumber,CityMaster.CityName,hp.TestAmt
        //            from HealthCheckupCenter 
        //             left join HealthCheckUp  on HealthCheckupCenter.Id = HealthCheckUp.Center_Id 
        //           left join Location on Location.Id = HealthCheckupCenter.Location_Id 
        //          left join CityMaster on CityMaster.Id = HealthCheckupCenter.CityMaster_Id 
        //            left join HealthCheckUpPackage hp on hp.CenterId = HealthCheckupCenter.id 
        //       where HealthCheckupCenter.Isdeleted=0 and HealthCheckupCenter.CityMaster_Id=" + cityId + "";
        //                var data = ent.Database.SqlQuery<HealthCheckUpList>(qry).ToList();
        //    if(data.Count() == 0)
        //    {
        //        rm.Message = "No Lab, Near You";
        //        rm.Status = 0;
        //        return Ok(rm);
        //    }
        //    model.HealthCheckupList = data;
        //    model.Message = "Yes Lab, Near You";
        //    model.Status = 1;
        //    return Ok(model);

        //}



        //Patient will get the Details the Details of Related Test 
        [Route("api/HealthCheckUpApi/ViewMore")]
        [HttpGet]
        public IHttpActionResult ViewMore(int HealthId)
        {
            var data = new ViewMoreHealth();
            string qry = @"select hc.Id,hc.LabName,hc.About,hc.HealthType,hc.year,L.LocationName from HealthCheckupCenter as hc with(nolock)
left join Location as L with(nolock) on L.id=hc.Location_Id
where hc.Id=" + HealthId + "";
            var model = ent.Database.SqlQuery<ViewMoreHealth>(qry).FirstOrDefault();
           
            return Ok(model);
        }


        //Patient Will book Complete HealthCheckUp
        //[HttpPost]
        //   public IHttpActionResult BookCompleteCheckUp(BookHealthCheckUp model)
        // {
        //     using (var trans = ent.Database.BeginTransaction())
        //     {
        //         if (!ModelState.IsValid)
        //         {
        //             var message = string.Join(" | ", ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage));
        //             trans.Rollback();
        //             rwithprm.Message = message;
        //             rwithprm.Status = 0;
        //             return Ok(rwithprm);
        //         }
        //         try
        //         {
        //             if (model != null)
        //             {
        //                 var healthCheck = ent.HealthCheckUps.Find(model.Test_Id);
        //                 double amount = healthCheck.TestAmount;
        //                 int count = model.Patient.Count();
        //                 foreach (var name in model.Patient)
        //                 {
        //                     var data = new CmpltCheckUp();
        //                     data.Amount = amount;
        //                     data.Center_Id = model.Center_Id;
        //                     data.ContactNo = model.ContactNo;
        //                     data.IsPaid = false;
        //                     data.IsTaken = false;
        //                     data.PatientId = model.PatientId;
        //                     data.PatientName = model.Patient.FirstOrDefault().PatientName;
        //                     data.PatientAddress = model.PatientAddress;
        //                     data.TestDate = DateTime.Now;
        //                     data.Test_Id = model.Test_Id;
        //                     data.RequestDate = DateTime.Now;
        //                     data.BookingId = Book.GenerateBooking4HC(); // Call the GenerateBooking4HC (For HealthCheck up) for Generate Booking Ids and store into Booking Id..
        //                     ent.CmpltCheckUps.Add(data);
        //                 }
        //                 ent.SaveChanges();
        //                 var health = new HealthBooking
        //                 {
        //                     IsPaid = false,
        //                     PatientId = model.PatientId,
        //                     Test_Id = model.Test_Id,
        //                     Total = count,
        //                     TotalAmount = count * amount,
        //                 };
        //                 ent.HealthBookings.Add(health);
        //                 ent.SaveChanges();
        //                 rwithprm.Checkup_Id = health.Id;
        //                 rwithprm.Message = "Successfully Booked";
        //                 trans.Commit();
        //                 rwithprm.Status = 1;
        //                 return Ok(rwithprm);
        //             }
        //             else
        //             {
        //                 trans.Rollback();
        //                 rwithprm.Message = "Some Error";
        //                 rwithprm.Status = 0;
        //                 return Ok(rwithprm);
        //             }
        //         }
        //         catch (Exception ex)
        //         {
        //             trans.Rollback();
        //             return InternalServerError(ex);
        //         }
        //     }
        // }


        //Update Payment Status
        [HttpGet]
        public IHttpActionResult updatePaymentStatus(int Checkup_Id)
        {
            try
            {
                string query = @"update HealthBooking set PaymentDate=getdate(),IsPaid=1 where Id=" + Checkup_Id;
                int Id = ent.Database.SqlQuery<int>(@"select PatientId from CmpltCheckUp where Id='" + Checkup_Id + "'").FirstOrDefault();
                DateTime Date = ent.Database.SqlQuery<DateTime>(@"select Convert(date,TestDate) from CmpltCheckUp where Id='" + Checkup_Id + "'").FirstOrDefault();
                DateTime data = Convert.ToDateTime(Date);
                string mobile = ent.Database.SqlQuery<string>(@"select MobileNumber from Patient where Id='" + Id + "'").FirstOrDefault();
                string Name = ent.Database.SqlQuery<string>(@"select PatientName from Patient where Id='" + Id + "'").FirstOrDefault();
                string msg = "Dear " + Name + ", Your Appointment has been confirmed on " + data + " .We will revert you soon.";
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


        [Route("api/HealthCheckUpApi/HCheckUpList")]
        [HttpGet]
        public IHttpActionResult HCheckUpList(int CityId, int StateId, int testId)
        {
            var model = new HealthCheckUpV();
            string qry = @"select hc.Id,hc.LabName,hc.MobileNumber,Lt.TestAmount,L.LocationName,Cm.CityName from HealthCheckupCenter AS HC with(nolock)
  INNER JOIN LabTest AS LT with(nolock) ON HC.LabName = LT.TestName
  Inner join Location As L with(nolock) on L.id=hc.Location_Id
  Inner join CityMaster As Cm with(nolock) on Cm.Id=HC.CityMaster_Id
  where  hc.IsDeleted=0 and hc.StateMaster_Id=" + StateId + " and hc.CityMaster_Id=" + CityId + " and LT.id=" + testId;
            var data = ent.Database.SqlQuery<HealthCheckUpList>(qry).ToList();
            model.HealthCheckupList = data;
            model.Message = "Yes Lab, Near You";
            return Ok(model);

        }

        [Route("api/HealthCheckUpApi/HealthCheckUpBooking")]
        [HttpPost]
        public IHttpActionResult HealthCheckUpBooking(HealthCheckUpBooked model)
        {
            var model1 = new HealthCheckUpV();
            string qry = @"select hc.Id,hc.LabName,hc.MobileNumber,Lt.TestAmount,L.LocationName,Cm.CityName from HealthCheckupCenter AS HC with(nolock)
  INNER JOIN LabTest AS LT with(nolock) ON HC.LabName = LT.TestName
  Inner join Location As L with(nolock) on L.id=hc.Location_Id
  Inner join CityMaster As Cm with(nolock) on Cm.Id=HC.CityMaster_Id
  where  hc.IsDeleted=0 and hc.StateMaster_Id=" + model.StateMaster_Id + " and hc.CityMaster_Id=" + model.CityMaster_Id + " and LT.id=" + model.testId;
            var data = ent.Database.SqlQuery<HealthCheckUpList>(qry).ToList();
            model1.HealthCheckupList = data;
            model1.Message = "Yes Lab, Near You";
            return Ok(model1);

        }


        [Route("api/HealthCheckUpApi/HealthBooknow")]
        [HttpPost]
        public IHttpActionResult HealthBooknow(HealthBooknow model)
        {
            try
            {
                var data = new HealthBooking()
                {
                    Test_Id = model.Test_Id,
                    Slotid = model.Slotid,
                    TestDate = model.TestDate,
                };
                ent.HealthBookings.Add(data);
                ent.SaveChanges();
                return Ok("Add Successfully ");
            }
            catch (Exception e)
            {
                return Ok("Internal server error");
            }
        }


        [Route("api/HealthCheckUpApi/HealthAptmt")]
        [HttpGet]
        public IHttpActionResult HealthAptmt(int Test_Id)
        {
            string query = @"select hc.Id,hc.LabName,Lt.TestAmount,hc.HealthType,Ts.SlotTime,HB.TestDate from HealthCheckupCenter AS HC with(nolock)
  INNER JOIN LabTest AS LT with(nolock) ON HC.LabName = LT.TestName
  Inner join HealthBooking As HB with(nolock) on HB.Test_Id=LT.Id
 INNER join TimeSlot as Ts with(nolock) on Ts.Slotid=HB.Slotid
  where  hc.IsDeleted=0 and LT.id=" + Test_Id + "";
            var data = ent.Database.SqlQuery<HealthDet>(query).FirstOrDefault();
            return Ok(data);
        }

        [Route("api/HealthCheckUpApi/HealthPayNow")]
        [HttpPost]
        public IHttpActionResult HealthPayNow(HealthPayNow model)
        {
            try
            {
                if (model.IsPaid == true)
                {
                    var data = ent.HealthBookings.Where(a => a.Test_Id == model.Test_Id).FirstOrDefault();
                    data.Test_Id = model.Test_Id;
                    data.PatientId = model.Patient_Id;
                    data.PaymentDate = DateTime.Now;
                    data.TotalAmount = model.TotalAmount;
                    data.IsPaid = model.IsPaid;
                    ent.HealthBookings.Add(data);
                    ent.SaveChanges();
                    return Ok("Book Appointment Successfully ");
                }
                else if (model.IsPaid == false)
                {
                    var data = ent.HealthBookings.Where(a => a.Test_Id == model.Test_Id).FirstOrDefault();
                    data.Test_Id = model.Test_Id;
                    data.PatientId = model.Patient_Id;
                    data.TotalAmount = model.TotalAmount;
                    data.PaymentDate = DateTime.Now;
                    data.IsPaid = model.IsPaid;
                    ent.SaveChanges();
                    return Ok("Update Appointment ");
                }
                return Ok("Please check the detail");
            }
            catch (Exception e)
            {
                return Ok("Internal server error");
            }
        }

    }
}
