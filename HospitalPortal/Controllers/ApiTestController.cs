using HospitalPortal.Models;
using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Web.Http;

namespace HospitalPortal.Controllers
{
    public class ApiTestController : ApiController
    {
        DbEntities ent = new DbEntities();

        [Route("api/ApiTest/BookAppointment")]
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
                    string aptQuery = @"exec [AppointmentTestNurse] @patientId,@mobile,@nurseTypeId,@locationId, @serviceDate, @serviceType";
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

                //var Locationdata = ent.Locations.Find(model.LocationId);
                //var CityData = ent.CityMasters.Find(Locationdata.City_Id);
                //var StateData = ent.StateMasters.Find(CityData.StateMaster_Id); 

                //                string nurseQuery = @"select mobilenumber from Nurse n
                //join Nurse_Location nl on n.Id = nl.Nurse_Id
                //where n.NurseType_id=" + model.NurseType_Id + " and nl.Location_Id=" + model.LocationId + " and n.IsApproved=1 and n.IsDeleted=0";

                //                string msg = @"You have enquiry for services date between" + model.StartDate.ToString("dd-MM-yyyy") + " and " + model.EndDate.ToString("dd-MMM-yyyy") + ".Please Login to portal to accept the services";
                //                var nurseMobiles = ent.Database.SqlQuery<string>(nurseQuery).ToList();

                //                nurseMobiles.ForEach(phoneNo => {
                //                    Message.SendSms(phoneNo, msg);
                //});
                return Ok(rm);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        
    }
}
