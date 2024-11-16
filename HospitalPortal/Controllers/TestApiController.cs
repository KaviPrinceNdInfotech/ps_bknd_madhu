using AutoMapper;
using HospitalPortal.BL;
using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.RequestModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity.Spatial;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Http;


namespace HospitalPortal.Controllers
{
    public class TestApiController : ApiController
    {

        DbEntities ent = new DbEntities();
        ReturnMessage RM = new ReturnMessage();
        Notification send = new Notification();

        [HttpPost]
        public IHttpActionResult Test(TestModule model)
        {
            //DbGeography searchLocation = DbGeography.FromText(String.Format("POINT({0} {1})", model.Longtitude, model.Latitude));
            using (var trans = ent.Database.BeginTransaction())
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage));
                    trans.Rollback();
                    RM.Message = message;
                    RM.Status = 0;
                    trans.Rollback();
                    return Ok(RM);
                }




                //Get the List of Drivers && Who have Completed there Drive
                var data = (from d in ent.Drivers
                            join v in ent.Vehicles
                            on d.Id equals v.Driver_Id
                            join al in ent.AdminLogins
                            on d.AdminLogin_Id equals al.Id
                            where v.VehicleType_Id == model.VehicleType_Id
                            //&& v.IsApproved == true
                            //&& d.IsApproved == true
                            select new TokenNo
                            {
                                Token = al.Token
                            }).ToList();


                //Send Notification to all Driver With
                if (data != null)
                {
                    data.ForEach(a =>
                    {
                        if (!string.IsNullOrEmpty(a.Token))
                        {
                            send.Message(a.Token, "PsWellness", "Yor've got a request.Kindly accept the request.");
                        }
                    });
                }
                else
                {
                    RM.Message = "No Driver Found Near You";
                    RM.Status = 0;
                    return Ok(RM);
                }

                //Details Have Been Saved
                var domain = Mapper.Map<PatientRequest>(model);
                domain.Request = DateTime.Now;
                domain.Status = false;
                domain.VehicelType_Id = model.VehicleType_Id;
                domain.StatusKey = 0;
                ent.PatientRequests.Add(domain);
                ent.SaveChanges();
                trans.Commit();
                RM.Message = "Request Sent";
                RM.Status = 1;
                model.Status = (bool)domain.Status;
                return Ok(model);
            }
        }

        [HttpPost]
        public IHttpActionResult ResponseIn(ResponseInReturn model)
        {
            using (var trans = ent.Database.BeginTransaction())
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage));
                    //trans.Rollback();
                    RM.Message = message;
                    RM.Status = 0;
                    trans.Rollback();
                    return Ok(RM);
                }
                int TempId = 0;
                string Token = "";
                string DriverName = "";

                try
                {
                    //Get The Vehicle_Id using VehicleType Id that has been Passed in model
                    if (model.Driver_Id > 0)
                    {
                        int VehicleId = ent.Database.SqlQuery<int>("select Id from Vehicle where Driver_Id=" + model.Driver_Id).FirstOrDefault();
                        TempId = VehicleId;
                    }

                    //Get The Patient Record who made a Request from the Application
                    if (model.Patient_Id > 0)
                    {
                        string query = @"select Top 1 * from Patient p join PatientRequest pr on pr.Patient_Id = p.Id where p.Id=" + model.Patient_Id + " order by pr.Id desc";
                        var data = ent.Database.SqlQuery<PatientDetail>(query).ToList();
                        model.Patient = data;
                        Token = ent.Database.SqlQuery<string>("select Token from AdminLogin al join Patient p on p.AdminLogin_Id = al.Id where al.Id=39").FirstOrDefault();
                    }

                    ////Get The Driver Record who accept the Request from the Application
                    //if (model.Driver_Id > 0)
                    //{
                    //    string query = @"select Top 1 * from Driver d join Vehicle v on v.Driver_Id = d.Id join TravelMaster tm on tm.Driver_Id = d.Id where d.Id = " + model.Driver_Id + " order by tm.Id desc";
                    //    var data1 = ent.Database.SqlQuery<DriverDetail>(query).ToList();
                    //    model.Driver = data1;
                    //    DriverName = data1.FirstOrDefault().DriverName;
                    //}

                    //Submit the Details into the Database
                    var domain = new TravelMaster();
                    domain.Driver_Id = model.Driver_Id;
                    domain.Lang_Driver = model.Lang_Driver;
                    domain.Lat_Driver = model.Lat_Driver;
                    domain.Patient_Id = model.Patient_Id;
                    domain.VehicleType_Id = model.VehicleType_Id;
                    domain.Vehicle_Id = TempId;
                    domain.DriverKey = true;
                    domain.StatusKey = 1;
                    domain.IsDriveCompleted = false;
                    domain.RequestDate = DateTime.Now;
                    ent.TravelMasters.Add(domain);
                    ent.SaveChanges();

                    //Send Notification to the Patient who made the Request
                    if (!string.IsNullOrEmpty(Token))
                    {
                        send.Message(Token, "PsWellness", "Your Driver " + DriverName + " is near you;");
                    }
                    model.Vehicle_Id = TempId;
                    string q = @"update PatientRequest set Status = 1, StatusKey=1 where Patient_Id=" + model.Patient_Id;
                    ent.Database.ExecuteSqlCommand(q);

                    //Driver Lat. and Lang
                    var driver = new DriverLocation
                    {
                        Driver_Id = model.Driver_Id,
                        Lang_Driver = model.Lang_Driver,
                        Lat_Driver = model.Lat_Driver,
                        PatientId = model.Patient_Id,
                        key = true
                    };
                    ent.DriverLocations.Add(driver);
                    ent.SaveChanges();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    RM.Message = ex.Message;
                    RM.Status = 0;
                    return Ok(RM);
                }
                return Ok(model);
            }
        }

        [HttpPost]
        public IHttpActionResult DriveCompleted(DriveComplete model)
        {
            DriverReturnMessage driverRM = new DriverReturnMessage();
            double? amt;
            string Token;
            int temp = 0;
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(b => b.Errors).Select(b => b.ErrorMessage));
                RM.Message = message;
                RM.Status = 0;
                return Ok(RM);
            }
            try
            {
                double dDistance = Double.MinValue;
                //Get the Kilometer using 2 Long. & Lat. 
                //double dDistance = Double.MinValue;
                //double dLat1InRad = model.Lat_PickUp_Patient * (Math.PI / 180.0);
                //double dLong1InRad = model.Lang_PickUp_Pateint * (Math.PI / 180.0);
                //double dLat2InRad = model.Lat_Drop * (Math.PI / 180.0);
                //double dLong2InRad = model.Lang_Drop * (Math.PI / 180.0);

                //double dLongitude = dLong2InRad - dLong1InRad;
                //double dLatitude = dLat2InRad - dLat1InRad;

                //// Intermediate result a.
                //double a = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) +
                //           Math.Cos(dLat1InRad) * Math.Cos(dLat2InRad) *
                //           Math.Pow(Math.Sin(dLongitude / 2.0), 2.0);

                //// Intermediate result c (great circle distance in Radians).
                //double c = 2.0 * Math.Asin(Math.Sqrt(a));
                //// Distance.
                //// const Double kEarthRadiusMiles = 3956.0;
                //const Double kEarthRadiusKms = 6376.5;
                double lon1 = model.Lang_PickUp_Pateint;
                double lat1 = model.Lat_PickUp_Patient;
                double lon2 = model.Lang_Drop;
                double lat2 = model.Lat_Drop;
                double rlat1 = Math.PI * lat1 / 180;
                double rlat2 = Math.PI * lat2 / 180;
                double theta = lon1 - lon2;
                double rtheta = Math.PI * theta / 180;
                double dist =
                    Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                    Math.Cos(rlat2) * Math.Cos(rtheta);
                dist = Math.Acos(dist);
                dist = dist * 180 / Math.PI;
                dist = dist * 60 * 1.1515;
                double val = dist * Convert.ToDouble(1.609344);
                dDistance = val;



                //IF the Distance is between 0 to 5
                if (dDistance <= 5)
                {
                    string Q = @"select * from VehicleType where Id=" + model.VehicleType_Id;
                    var data = ent.Database.SqlQuery<VehiclePrice>(Q).ToList();
                    model.VehiclePrice = data;
                    amt = model.VehiclePrice.FirstOrDefault().under5KM;
                }
                //IF the Distance is between 5 to 10
                else if (dDistance > 5 && dDistance < 10)
                {
                    string Q = @"select * from VehicleType where Id=" + model.VehicleType_Id;
                    var data = ent.Database.SqlQuery<VehiclePrice>(Q).ToList();
                    model.VehiclePrice = data;
                    amt = model.VehiclePrice.FirstOrDefault().under6_10KM;
                }

                //IF the Distance is between 11 to 20
                else if (dDistance > 10 && dDistance < 20)
                {
                    string Q = @"select * from VehicleType where Id=" + model.VehicleType_Id;
                    var data = ent.Database.SqlQuery<VehiclePrice>(Q).ToList();
                    model.VehiclePrice = data;
                    amt = model.VehiclePrice.FirstOrDefault().under11_20KM;
                }

                //IF the Distance is between 21 to 40
                else if (dDistance > 20 && dDistance < 40)
                {
                    string Q = @"select * from VehicleType where Id=" + model.VehicleType_Id;
                    var data = ent.Database.SqlQuery<VehiclePrice>(Q).ToList();
                    model.VehiclePrice = data;
                    amt = model.VehiclePrice.FirstOrDefault().under21_40KM;
                }
                //IF the Distance is between 41 to 60
                else if (dDistance > 40 && dDistance < 60)
                {
                    string Q = @"select * from VehicleType where Id=" + model.VehicleType_Id;
                    var data = ent.Database.SqlQuery<VehiclePrice>(Q).ToList();
                    model.VehiclePrice = data;
                    amt = model.VehiclePrice.FirstOrDefault().under41_60KM;
                }
                //IF the Distance is between 61 to 80
                else if (dDistance > 60 && dDistance < 80)
                {
                    string Q = @"select * from VehicleType where Id=" + model.VehicleType_Id;
                    var data = ent.Database.SqlQuery<VehiclePrice>(Q).ToList();
                    model.VehiclePrice = data;
                    amt = model.VehiclePrice.FirstOrDefault().under61_80KM;
                }
                //IF the Distance is between 81 to 100
                else if (dDistance > 80 && dDistance < 100)
                {
                    string Q = @"select * from VehicleType where Id=" + model.VehicleType_Id;
                    var data = ent.Database.SqlQuery<VehiclePrice>(Q).ToList();
                    model.VehiclePrice = data;
                    amt = model.VehiclePrice.FirstOrDefault().under81_100KM;
                }
                //IF the Distance is between 100 to 150
                else if (dDistance > 100 && dDistance < 150)
                {
                    string Q = @"select * from VehicleType where Id=" + model.VehicleType_Id;
                    var data = ent.Database.SqlQuery<VehiclePrice>(Q).ToList();
                    model.VehiclePrice = data;
                    amt = model.VehiclePrice.FirstOrDefault().under100_150KM;
                }
                //IF the Distance is between 151 to 200
                else if (dDistance > 150 && dDistance < 200)
                {
                    string Q = @"select * from VehicleType where Id=" + model.VehicleType_Id;
                    var data = ent.Database.SqlQuery<VehiclePrice>(Q).ToList();
                    model.VehiclePrice = data;
                    amt = model.VehiclePrice.FirstOrDefault().under151_200KM;
                }
                //IF the Distance is between 201 to 250
                else if (dDistance > 200 && dDistance < 250)
                {
                    string Q = @"select * from VehicleType where Id=" + model.VehicleType_Id;
                    var data = ent.Database.SqlQuery<VehiclePrice>(Q).ToList();
                    model.VehiclePrice = data;
                    amt = model.VehiclePrice.FirstOrDefault().under201_250KM;
                }
                //IF the Distance is between 251 to 300
                else if (dDistance > 251 && dDistance < 300)
                {
                    string Q = @"select * from VehicleType where Id=" + model.VehicleType_Id;
                    var data = ent.Database.SqlQuery<VehiclePrice>(Q).ToList();
                    model.VehiclePrice = data;
                    amt = model.VehiclePrice.FirstOrDefault().under251_300KM;
                }
                //IF the Distance is between 301 to 350
                else if (dDistance > 300 && dDistance < 350)
                {
                    string Q = @"select * from VehicleType where Id=" + model.VehicleType_Id;
                    var data = ent.Database.SqlQuery<VehiclePrice>(Q).ToList();
                    model.VehiclePrice = data;
                    amt = model.VehiclePrice.FirstOrDefault().under301_350KM;
                }
                //IF the Distance is between 351 to 400
                else if (dDistance > 350 && dDistance < 400)
                {
                    string Q = @"select * from VehicleType where Id=" + model.VehicleType_Id;
                    var data = ent.Database.SqlQuery<VehiclePrice>(Q).ToList();
                    model.VehiclePrice = data;
                    amt = model.VehiclePrice.FirstOrDefault().under351_400KM;
                }
                //IF the Distance is between 401 to 450
                else if (dDistance > 400 && dDistance < 450)
                {
                    string Q = @"select * from VehicleType where Id=" + model.VehicleType_Id;
                    var data = ent.Database.SqlQuery<VehiclePrice>(Q).ToList();
                    model.VehiclePrice = data;
                    amt = model.VehiclePrice.FirstOrDefault().under401_450KM;
                }
                //IF the Distance is between 451 to 500
                else if (dDistance > 450 && dDistance < 500)
                {
                    string Q = @"select * from VehicleType where Id=" + model.VehicleType_Id;
                    var data = ent.Database.SqlQuery<VehiclePrice>(Q).ToList();
                    model.VehiclePrice = data;
                    amt = model.VehiclePrice.FirstOrDefault().under451_500KM;
                }
                //IF the Distance is Above 500
                else
                {
                    string Q = @"select * from VehicleType where Id=" + model.VehicleType_Id;
                    var data = ent.Database.SqlQuery<VehiclePrice>(Q).ToList();
                    model.VehiclePrice = data;
                    double? totalAmt = model.VehiclePrice.FirstOrDefault().Above500KM;
                    amt = totalAmt * Math.Round(dDistance);
                }

                if (model.Driver_Id > 0)
                {
                    int VehicleId = ent.Database.SqlQuery<int>("select Id from Vehicle where Driver_Id=" + model.Driver_Id).FirstOrDefault();
                    temp = VehicleId;
                }
                int DriveCmplteId = ent.Database.SqlQuery<int>("select Top 1 Id from TravelRecordMaster where Driver_Id = " + model.Driver_Id + " order by Id desc").FirstOrDefault();
                var domain = ent.TravelRecordMasters.Find(DriveCmplteId);
                var Vehicle = ent.Vehicles.Find(domain.Vehicle_Id);
                TimeSpan? duration = model.RideEndTime - domain.RideStartTime;
                string timeStamp = Convert.ToString(duration);
                var segments = timeStamp.Split(':');
                TimeSpan t = new TimeSpan(0, Convert.ToInt32(segments[0]),
                               Convert.ToInt32(segments[1]), Convert.ToInt32(segments[2]));
                string time = string.Format("{0}",
                           ((int)t.TotalHours));
                if (time == "3")
                {
                    domain.FullAmount = (decimal)amt + (decimal)Vehicle.DriverCharges;
                }
                else
                {
                    domain.FullAmount = (decimal)amt;
                }
                domain.Distance = dDistance;
                domain.Drop_Place = model.Drop_Place;
                domain.Lang_Drop = model.Lang_Drop;
                domain.Lang_PickUp_Pateint = model.Lang_PickUp_Pateint;
                domain.Lat_Drop = model.Lat_Drop;
                domain.Lat_PickUp_Patient = model.Lat_PickUp_Patient;
                domain.Patient_Id = model.Patient_Id;
                domain.PickUp_Place = model.PickUp_Place;
                domain.Vehicle_Id = temp;
                domain.RequestDate = DateTime.Now;
                domain.IsDriveCompleted = true;
                domain.IsPaid = false;
                ent.Entry<TravelRecordMaster>(domain).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                //Update the Drive Compeleted in Travel Master Table
                string q = @"update TravelRecordMaster set FullAmount =" + amt + " where Id=" + DriveCmplteId;
                ent.Database.ExecuteSqlCommand(q);
                string q1 = @"update PatientRequest set StatusKey = 4 where Patient_Id=" + model.Patient_Id;
                ent.Database.ExecuteSqlCommand(q1);
                string q2 = @"update TravelMaster set StatusKey = 4 where Driver_Id=" + model.Driver_Id;
                ent.Database.ExecuteSqlCommand(q2);

                //string q1 = @"delete from TravelMaster where Driver_Id=" + model.Driver_Id;
                //ent.Database.ExecuteSqlCommand(q1);
                //string q2 = @"delete from PatientRequest where Patient_Id=" + model.Patient_Id;
                //ent.Database.ExecuteSqlCommand(q2);

                //Push Notification
                //Token = ent.Database.SqlQuery<string>("select Token from AdminLogin al join Patient p on p.AdminLogin_Id = al.Id where p.Id=" + model.Patient_Id).FirstOrDefault();
                //if (!string.IsNullOrEmpty(Token))
                //{
                //    send.Message(Token, "PsWellness", "Your Amount is "+amt+"");
                //}
                model.Vehicle_Id = temp;
                double amount = ent.Database.SqlQuery<double>(@"select Amount from TravelRecordMaster where Id=" + DriveCmplteId).FirstOrDefault();
                driverRM.PreviousAmount = amount;
                driverRM.CurrentAmount = (double)amt;
                driverRM.TotalAmount = (double)amt - amount;
                driverRM.DriveCompleteId = DriveCmplteId;
            }
            catch (Exception ex)
            {
            }
            return Ok(driverRM);
        }

        [HttpPost]
        public IHttpActionResult HalfPayment(DriveComplete model)
        {
            //double? amt;
            //string Token;
            int temp = 0;
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(b => b.Errors).Select(b => b.ErrorMessage));
                RM.Message = message;
                RM.Status = 0;
                return Ok(RM);
            }
            try
            {
                if (model.Driver_Id > 0)
                {
                    int VehicleId = ent.Database.SqlQuery<int>("select Id from Vehicle where Driver_Id=" + model.Driver_Id).FirstOrDefault();
                    temp = VehicleId;
                }

                var domain = new TravelRecordMaster();
                domain.Amount = model.Amount;
                domain.Distance = model.Distance;
                domain.Driver_Id = model.Driver_Id;
                domain.Drop_Place = model.Drop_Place;
                domain.IsDriveCompleted = false;
                domain.Lang_Drop = model.Lang_Drop;
                domain.Lang_PickUp_Pateint = model.Lang_PickUp_Pateint;
                domain.Lat_Drop = model.Lat_Drop;
                domain.Lat_PickUp_Patient = model.Lat_PickUp_Patient;
                domain.Patient_Id = model.Patient_Id;
                domain.PickUp_Place = model.PickUp_Place;
                domain.Vehicle_Id = temp;
                domain.RequestDate = DateTime.Now;
                domain.IsDriveCompleted = true;
                ent.TravelRecordMasters.Add(domain);
                ent.SaveChanges();


                //Update the Drive Compeleted in Travel Master Table
                string q = @"update TravelMaster set StatusKey = 2 where Driver_Id=" + model.Driver_Id;
                ent.Database.ExecuteSqlCommand(q);
                string q1 = @"update PatientRequest set StatusKey = 2 where Patient_Id=" + model.Patient_Id;
                ent.Database.ExecuteSqlCommand(q1);

                //string q1 = @"delete from TravelMaster where Driver_Id=" + model.Driver_Id;
                //ent.Database.ExecuteSqlCommand(q1);
                //string q2 = @"delete from PatientRequest where Patient_Id=" + model.Patient_Id;
                //ent.Database.ExecuteSqlCommand(q2);

                //Push Notification
                //Token = ent.Database.SqlQuery<string>("select Token from AdminLogin al join Patient p on p.AdminLogin_Id = al.Id where p.Id=" + model.Patient_Id).FirstOrDefault();
                //if (!string.IsNullOrEmpty(Token))
                //{
                //    send.Message(Token, "PsWellness", "Your Amount is "+amt+"");
                //}
                model.Vehicle_Id = temp;
                model.Amount = (double)domain.Amount;
                model.DriveCompleteId = domain.Id;
            }
            catch (Exception EX) { }

            return Ok(model);
        }

        public IHttpActionResult ResetStatusKey(StatusResetRequest model)
        {
            var rm = new ReturnMessage();
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(b => b.Errors).Select(b => b.ErrorMessage));
                rm.Message = message;
                rm.Status = 0;
                return Ok(RM);
            }
            try
            {
                string q = @"update TravelMaster set StatusKey = 0 where Driver_Id=" + model.Driver_Id;
                ent.Database.ExecuteSqlCommand(q);
                string q1 = @"update PatientRequest set StatusKey = 0 where Patient_Id=" + model.Patient_Id;
                ent.Database.ExecuteSqlCommand(q1);
                rm.Status = 1;
                rm.Message = "Stutus key has reset.";
            }
            catch (Exception ex)
            {
                rm.Status = 0;
                rm.Message = "Server error";
            }
            return Ok(rm);
        }

        [HttpGet]
        [Route("api/TestApi/GetPatient")]
        public IHttpActionResult GetPatient(int DriverId, double? Lang_Driver = null, double? Lat_Driver = null)
        {
            int Id = 0;
            int DriveCmplteId = 0;
            var model = new RecordList();
            DriverReturnMessage driverRM = new DriverReturnMessage();
            var statusKey = ent.TravelMasters.Where(a => a.Driver_Id == DriverId).Select(A => A.StatusKey).FirstOrDefault();
            if (statusKey == 4)
            {
                DriveCmplteId = ent.Database.SqlQuery<int>("select Top 1 Id from TravelRecordMaster where Driver_Id = " + DriverId + " order by Id desc").FirstOrDefault();
                double amount = ent.Database.SqlQuery<double>(@"select Amount from TravelRecordMaster where Id=" + DriveCmplteId).FirstOrDefault();
                decimal amt = ent.Database.SqlQuery<decimal>(@"select FullAmount from TravelRecordMaster where Id=" + DriveCmplteId).FirstOrDefault();
                driverRM.PreviousAmount = amount;
                driverRM.CurrentAmount = (double)amt;
                driverRM.TotalAmount = (double)amt - amount;
                driverRM.DriveCompleteId = DriveCmplteId;
                driverRM.StatusKey = statusKey;
                return Ok(driverRM);
            }
            else
            {
                if (DriverId != 0)
                {
                    //Vehicle Id
                    Id = ent.Database.SqlQuery<int>("select VehicleType_Id from Vehicle where Driver_Id=" + DriverId).FirstOrDefault();
                    if (Lang_Driver != null && Lat_Driver != null)
                    {
                        string q = @"update TravelMaster set Lang_Driver =" + Lang_Driver + ",Lat_Driver=" + Lat_Driver + " where Driver_Id= " + DriverId;
                        ent.Database.ExecuteSqlCommand(q);
                    }
                }
                // Vehicle Request
                var data = ent.Database.SqlQuery<RecordList>("select Top 1 pr.Patient_Id, pr.Latitude,pr.StatusKey, pr.Longtitude,pr.DropLongtitude,pr.DropLatitude, p.PatientName, p.MobileNumber from VehicleType vt join PatientRequest pr on pr.VehicelType_Id = vt.Id join Patient p on p.Id = pr.Patient_Id where pr.VehicelType_Id = " + Id + "").ToList();
                if (data.Count() == 0)
                {
                    RM.Status = 0;
                    RM.Message = "No Records";
                    return Content(HttpStatusCode.NotFound, new { success = false }, Configuration.Formatters.JsonFormatter);
                }
                DriveCmplteId = ent.Database.SqlQuery<int>("select Top 1 Id from TravelRecordMaster where Driver_Id = " + DriverId + " order by Id desc").FirstOrDefault();
                model.Patient_Id = data.FirstOrDefault().Patient_Id;
                model.Latitude = data.FirstOrDefault().Latitude;
                model.Longtitude = data.FirstOrDefault().Longtitude;
                model.StatusKey = data.FirstOrDefault().StatusKey;
                model.DropLatitude = data.FirstOrDefault().DropLatitude;
                model.DropLongtitude = data.FirstOrDefault().DropLongtitude;
                model.PatientName = data.FirstOrDefault().PatientName;
                model.MobileNumber = data.FirstOrDefault().MobileNumber;

                return Ok(model);
            }
        }

        [HttpGet]
        [Route("api/TestApi/GetDriver")]
        public IHttpActionResult GetDriver(int PatientId)
        {

            PateintReturnMessage driverRM = new PateintReturnMessage();
            var model = new DriverDetail();
            int Id = 0;
            var statusKey = ent.PatientRequests.Where(a => a.Patient_Id == PatientId).Select(A => A.StatusKey).FirstOrDefault();
            if (statusKey == 4)
            {
                int DriveCmplteId = ent.Database.SqlQuery<int>("select Top 1 Id from TravelRecordMaster where Patient_Id = " + PatientId + " order by Id desc").FirstOrDefault();
                double amount = ent.Database.SqlQuery<double>(@"select Amount from TravelRecordMaster where Id=" + DriveCmplteId).FirstOrDefault();
                decimal amt = ent.Database.SqlQuery<decimal>(@"select FullAmount from TravelRecordMaster where Id=" + DriveCmplteId).FirstOrDefault();
                driverRM.PreviousAmount = amount;
                driverRM.CurrentAmount = (double)amt;
                driverRM.TotalAmount = (double)amt - amount;
                driverRM.DriveCompleteId = DriveCmplteId;
                driverRM.StatusKey = statusKey;
                driverRM.Status = 1;
                int DriverId = ent.Database.SqlQuery<int>(@"select Driver_Id from TravelRecordMaster where Id=" + DriveCmplteId).FirstOrDefault();
                var Driverdata = ent.Drivers.Find(DriverId);
                driverRM.DriverName = Driverdata.DriverName;
                int vehicleId = ent.Database.SqlQuery<int>(@"select Id from Vehicle where Driver_Id=" + DriverId).FirstOrDefault();
                var Vehicledata = ent.Vehicles.Find(vehicleId);
                driverRM.VehicleNumber = Vehicledata.VehicleNumber;
                return Ok(driverRM);
            }
            else
            {
                if (PatientId > 0)
                {
                    //Vehicle Id
                    Id = ent.Database.SqlQuery<int>("select Top 1 Driver_Id from TravelMaster tm join PatientRequest pr on tm.Patient_Id = pr.Patient_Id where IsDriveCompleted=0 and tm.Patient_Id=" + PatientId + " order by Driver_Id desc").FirstOrDefault();
                }

                var status = ent.PatientRequests.Where(a => a.Patient_Id == PatientId).Select(a => a.Status).FirstOrDefault();
                if (status == true)
                {
                    // Vehicle Request
                    var data = ent.Database.SqlQuery<DriverDetail>(@"select Top 1 d.DriverName, d.Id, d.DriverImage, d.MobileNumber,
v.VehicleNumber, tm.StatusKey, tm.Lat_Driver, tm.Lang_Driver, pr.VehicelType_Id as VehicleType_Id,
 pr.DropLongtitude as Patient_DropLongtitude, pr.DropLatitude as Patient_DropLatitude
 , pr.Latitude, pr.Longtitude from Driver d
 join TravelMaster tm on tm.Driver_Id = d.Id join Vehicle v on v.Driver_Id = d.Id
  left join PatientRequest pr on pr.Patient_Id = tm.Patient_Id where IsDriveCompleted = 0 and d.Id = " + Id).ToList();
                    if (data.Count() < 0)
                    {
                        RM.Status = 0;
                        RM.Message = "No Records";
                        return Content(HttpStatusCode.NotFound, new { success = false }, Configuration.Formatters.JsonFormatter);
                    }
                    model.DriverImage = data.FirstOrDefault().DriverImage;
                    model.DriverName = data.FirstOrDefault().DriverName;
                    model.Lang_Driver = data.FirstOrDefault().Lang_Driver;
                    model.Lat_Driver = data.FirstOrDefault().Lat_Driver;
                    model.MobileNumber = data.FirstOrDefault().MobileNumber;
                    model.Status = 1;
                    model.StatusKey = data.FirstOrDefault().StatusKey;
                    model.VehicleNumber = data.FirstOrDefault().VehicleNumber;
                    model.VehicleType_Id = data.FirstOrDefault().VehicleType_Id;
                    model.Patient_DropLatitude = data.FirstOrDefault().Patient_DropLatitude;
                    model.Patient_DropLongtitude = data.FirstOrDefault().Patient_DropLongtitude;
                    model.Latitude = data.FirstOrDefault().Latitude;
                    model.Longtitude = data.FirstOrDefault().Longtitude;
                    model.Id = data.FirstOrDefault().Id;
                    return Ok(model);
                }
                else
                {
                    RM.Message = "No Driver Found.";
                    RM.Status = 0;
                    return Ok(RM);
                }
            }
        }

        public IHttpActionResult GetAmount(int DriveCompleteId = 0)
        {

            var model = new PatientAmountRecord();
            if (DriveCompleteId > 0)
            {
                string q = @"select Amount as PreviousAmount, ExtraCharges as DriverCharges, Is8HourRide, FullAmount as CurrentAmount from TravelRecordMaster where Id=" + DriveCompleteId;
                var data = ent.Database.SqlQuery<PatientAmountRecord>(q).ToList();
                if (data != null)
                {
                    if (data.FirstOrDefault().Is8HourRide == true)
                    {
                        model.DriverCharges = data.FirstOrDefault().DriverCharges;
                        model.CurrentAmount = data.FirstOrDefault().CurrentAmount;
                        model.PreviousAmount = data.FirstOrDefault().PreviousAmount;
                        model.TotalAmount = (double)model.CurrentAmount - (double)model.PreviousAmount;
                    }
                    else
                    {
                        model.CurrentAmount = data.FirstOrDefault().CurrentAmount;
                        model.PreviousAmount = data.FirstOrDefault().PreviousAmount;
                        model.TotalAmount = (double)model.CurrentAmount - (double)model.PreviousAmount;
                    }
                    return Ok(model);
                }
                else
                {

                }
            }
            else
            {
                RM.Message = "No Records";
                RM.Status = 0;
                return Ok(RM);

            }
            return Ok(model);
        }

        [HttpPost]
        [Route("api/TestApi/RideStart")]
        public IHttpActionResult RideStart(RideStartDTO model)
        {
            using (var trans = ent.Database.BeginTransaction())
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage));
                    RM.Message = message;
                    RM.Status = 0;
                    trans.Rollback();
                    return Ok(RM);
                }
                else
                {

                    if (ent.PatientRequests.Any(a => a.Patient_Id == model.PatientId))
                    {
                        string q = @"update PatientRequest set Status = 1, StatusKey=3 where Patient_Id=" + model.PatientId;
                        ent.Database.ExecuteSqlCommand(q);
                    }
                    else
                    {
                        RM.Message = "Invalid Attempt";
                        RM.Status = 0;
                        trans.Rollback();
                        return Ok(RM);
                    }
                    if (ent.TravelMasters.Any(a => a.Driver_Id == model.DriverId))
                    {
                        string q1 = @"update TravelMaster set StatusKey=3 where Driver_Id=" + model.DriverId;
                        ent.Database.ExecuteSqlCommand(q1);
                    }
                    else
                    {
                        RM.Message = "Invalid Attempt";
                        RM.Status = 0;
                        trans.Rollback();
                        return Ok(RM);
                    }
                    if (model.DriveCompletId != 0)
                    {
                        string q2 = @"update TravelRecordMaster set RideStartTime = " + model.RideStartTime + " where Id=" + model.DriveCompletId;
                        ent.Database.ExecuteSqlCommand(q2);
                    }
                    else
                    {
                        RM.Message = "Invalid Attempt";
                        RM.Status = 0;
                        trans.Rollback();
                        return Ok(RM);
                    }
                    trans.Commit();
                    RM.Message = "Ride Started";
                    RM.Status = 1;
                    return Ok(RM);
                }
            }
        }

        [HttpPost]
        [Route("api/TestApi/Retry")]
        public IHttpActionResult Retry(RetryDTO model)
        {
            using (var trans = ent.Database.BeginTransaction())
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage));
                    RM.Message = message;
                    RM.Status = 0;
                    trans.Rollback();
                    return Ok(RM);
                }
                try
                {
                    string q = @"delete PatientRequest where Patient_Id=" + model.PatientId;
                    ent.Database.ExecuteSqlCommand(q);
                    string q1 = @"delete TravelMaster where Patient_Id=" + model.PatientId;
                    ent.Database.ExecuteSqlCommand(q1);
                    trans.Commit();
                    RM.Message = "Request Deleted";
                    RM.Status = 1;
                    return Ok(RM);
                }
                catch (Exception ex)
                {
                    return Ok();
                }
            }
        }

        [HttpGet]
        [Route("api/TestApi/PaymentSuccess")]
        public IHttpActionResult PaymentSuccess(int DriveCompleteId)
        {
            ReturnMessage rm = new ReturnMessage();
            var data = ent.TravelRecordMasters.Find(DriveCompleteId);
            if (data != null)
            {
                string DriverToken;
                string PatientToken;
                string q = @"update TravelRecordMaster set IsPaid =1 , PaymentDate=GetDate() where Id=" + DriveCompleteId;
                ent.Database.ExecuteSqlCommand(q);
                decimal amt = ent.Database.SqlQuery<decimal>(@"select FullAmount from TravelRecordMaster where Id=" + DriveCompleteId).FirstOrDefault();
                int DriverId = ent.Database.SqlQuery<int>(@"select Driver_Id from TravelRecordMaster where Id=" + DriveCompleteId).FirstOrDefault();
                int PatientId = ent.Database.SqlQuery<int>(@"select Patient_Id from TravelRecordMaster where Id=" + DriveCompleteId).FirstOrDefault();
                PatientToken = ent.Database.SqlQuery<string>("select Token from AdminLogin al join Patient p on p.AdminLogin_Id = al.Id where p.Id=" + PatientId).FirstOrDefault();
                DriverToken = ent.Database.SqlQuery<string>("select Token from AdminLogin al join Driver p on p.AdminLogin_Id = al.Id where p.Id=" + PatientId).FirstOrDefault();
                if (!string.IsNullOrEmpty(DriverToken))
                {
                    send.Message(DriverToken, "PsWellness", "The Amount  " + amt + " has been Successfully Paid");
                }
                if (!string.IsNullOrEmpty(PatientToken))
                {
                    send.Message(PatientToken, "PsWellness", "The Rest Amount  " + amt + " has been Successfully Paid");
                }
                string q1 = @"delete from TravelMaster where Driver_Id=" + DriverId;
                ent.Database.ExecuteSqlCommand(q1);
                string q2 = @"delete from PatientRequest where Patient_Id=" + PatientId;
                ent.Database.ExecuteSqlCommand(q2);
                rm.Message = "Successfully Paid";
                rm.Status = 1;
            }
            else
            {
                rm.Message = "Something Went Wrong";
                rm.Status = 0;
            }
            return Ok(rm);
        }

        //[HttpPost]
        //public IHttpActionResult UpdateDriverLat_Lan(DriverLocationDTO model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var message = string.Join("|", ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage));
        //        RM.Message = message;
        //        RM.Status = 0;
        //        return Ok(RM);
        //    }
        //    var domain = new DriverLocation();
        //    var driver = ent.DriverLocations.Where(a => a.Driver_Id == model.Driver_Id).ToList(); 
        //    if(driver == null)
        //    {
        //        domain.Driver_Id = model.Driver_Id;
        //        domain.Lang_Driver = model.Lang_Driver;
        //        domain.Lat_Driver = model.Lat_Driver;
        //        domain.key = model.Key;
        //        domain.PatientId = model.Patient_Id;
        //        ent.DriverLocations.Add(domain);
        //        ent.SaveChanges();
        //        RM.Message = "Successfully Inserted";
        //        RM.Status = 1;
        //    }
        //    else
        //    {
        //        if (model.Key == true)
        //        {
        //            string q = @"update DriverLocation set Lang_Driver = " + model.Lang_Driver + " , Lat_Driver = " + model.Lat_Driver + ", [Key] = 1, PatientId= " + model.Patient_Id + " where Id=" + driver.FirstOrDefault().Id+ "";
        //            ent.Database.ExecuteSqlCommand(q);
        //            string q1 = @"update TravelMaster set Lang_Driver = " + model.Lang_Driver + " , Lat_Driver = " + model.Lat_Driver + " where Driver_Id=" + model.Driver_Id + "";
        //            ent.Database.ExecuteSqlCommand(q1);
        //        }
        //        else
        //        {
        //            string q = @"update DriverLocation set Lang_Driver = " + model.Lang_Driver + " , Lat_Driver = " + model.Lat_Driver + ", [Key] = 0, PatientId= " + model.Patient_Id + " where Driver_Id=" + model.Driver_Id + "";
        //            ent.Database.ExecuteSqlCommand(q);
        //            string q1 = @"update TravelMaster set Lang_Driver = " + model.Lang_Driver + " , Lat_Driver = " + model.Lat_Driver + " where Driver_Id=" + model.Driver_Id + "";
        //            ent.Database.ExecuteSqlCommand(q1);
        //        }
        //        RM.Message = "Successfully Updated";
        //        RM.Status = 1;
        //    }
        //    return Ok(RM);
        //}



        //[HttpGet]
        //[Route("api/TestApi/GetDriverUpdatedLatLang")]
        //public IHttpActionResult GetDriverUpdatedLatLang(int PatientId)
        //{
        //    var model = new DriverLocationVM();
        //    var data = (from e in ent.DriverLocations
        //                where e.PatientId == PatientId
        //                && e.key == true
        //                select new DriverLocations
        //                {
        //                    Driver_Id = e.Driver_Id,
        //                    Lang_Driver = e.Lang_Driver,
        //                    Lat_Driver = e.Lat_Driver,
        //                    Key = (bool)e.key,
        //                    Patient_Id = e.PatientId
        //                }).ToList();
        //    model.CurrentLocation = data;
        //    return Ok(model);
        //}
        public IHttpActionResult CancelAppointment(DriveComplete model)
        {
            int temp = 0;
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(b => b.Errors).Select(b => b.ErrorMessage));
                RM.Message = message;
                RM.Status = 0;
                return Ok(RM);
            }
            if (model.Driver_Id > 0)
            {
                int VehicleId = ent.Database.SqlQuery<int>("select Id from Vehicle where Driver_Id=" + model.Driver_Id).FirstOrDefault();
                temp = VehicleId;
            }
            var domain = new TravelRecordMaster();
            domain.Driver_Id = model.Driver_Id;
            domain.Drop_Place = model.Drop_Place;
            domain.IsDriveCompleted = false;
            domain.Lang_Drop = model.Lang_Drop;
            domain.Lang_PickUp_Pateint = model.Lang_PickUp_Pateint;
            domain.Lat_Drop = model.Lat_Drop;
            domain.Lat_PickUp_Patient = model.Lat_PickUp_Patient;
            domain.Patient_Id = model.Patient_Id;
            domain.PickUp_Place = model.PickUp_Place;
            domain.Vehicle_Id = temp;
            domain.RequestDate = DateTime.Now;
            domain.IsDriveCompleted = false;
            ent.TravelRecordMasters.Add(domain);
            ent.SaveChanges();
            var UserData = ent.Patients.Find(model.Patient_Id);
            double AppointmentCancelCharges = ent.Database.SqlQuery<double>(@"select Amount from VehicleCharges where TypeId=" + model.VehicleType_Id).FirstOrDefault();
            double CalculateAmt = model.Amount * AppointmentCancelCharges / 100;
            double RestAmount = model.Amount - CalculateAmt;
            var Wallet = new UserWallet
            {
                UserId = model.Patient_Id,
                Amount = RestAmount,
                AdminId = UserData.AdminLogin_Id,
                TransactionType = "cr"
            };
            ent.UserWallets.Add(Wallet);
            ent.SaveChanges();
            string q1 = @"delete from TravelMaster where Driver_Id=" + model.Driver_Id;
            ent.Database.ExecuteSqlCommand(q1);
            string q2 = @"delete from PatientRequest where Patient_Id=" + model.Patient_Id;
            ent.Database.ExecuteSqlCommand(q2);
            RM.Message = "Successfully Cancelled Your Ride";
            RM.Status = 1;
            return Ok(RM);
        }
        public class DriverLocationDTO
        {
            [Required]
            public Nullable<int> Driver_Id { get; set; }
            [Required]
            public Nullable<double> Lat_Driver { get; set; }
            [Required]
            public Nullable<double> Lang_Driver { get; set; }
            [Required]
            public Nullable<int> Patient_Id { get; set; }
            [Required]
            public bool Key { get; set; }

        }
        public class RideStartDTO
        {
            public int DriverId { get; set; }
            public int PatientId { get; set; }
            public int DriveCompletId { get; set; }
            public TimeSpan RideStartTime { get; set; }
        }

        public class RetryDTO
        {
            public int PatientId { get; set; }
        }

        public class DriverLocationVM
        {
            public IEnumerable<DriverLocations> CurrentLocation { get; set; }
        }

        public class DriverLocations
        {
            public Nullable<int> Driver_Id { get; set; }
            public Nullable<double> Lat_Driver { get; set; }
            public Nullable<double> Lang_Driver { get; set; }
            public Nullable<int> Patient_Id { get; set; }
            public bool Key { get; set; }
        }



        public class RecordList
        {
            public int Patient_Id { get; set; }
            public string PatientName { get; set; }
            public string MobileNumber { get; set; }
            public double Latitude { get; set; }
            public double Longtitude { get; set; }
            public double DropLongtitude { get; set; }
            public double DropLatitude { get; set; }
            public int StatusKey { get; set; }
            //public IEnumerable<PatientRecord> PatientRecord { get; set; }
        }
        public class PatientRecord
        {
            public int Patient_Id { get; set; }
            public double Latitude { get; set; }
            public double Longtitude { get; set; }
        }
        public class TestModule
        {
            public int VehicleType_Id { get; set; }
            public int Patient_Id { get; set; }
            public double Latitude { get; set; }
            public double Longtitude { get; set; }
            public double DropLatitude { get; set; }
            public double DropLongtitude { get; set; }
            public bool Status { get; set; }
        }

        public class ResponseInReturn
        {
            public int Patient_Id { get; set; }
            public int Driver_Id { get; set; }
            public double Lat_Driver { get; set; }
            public double Lang_Driver { get; set; }
            public int Vehicle_Id { get; set; }
            public int VehicleType_Id { get; set; }
            //public IEnumerable<DriverDetail> Driver { get; set;}
            public IEnumerable<PatientDetail> Patient { get; set; }

        }

        public class DriverDetail
        {
            public double PreviousAmount { get; set; }
            public double CurrentAmount { get; set; }
            public double TotalAmount { get; set; }
            public int? StatusKey { get; set; }
            public int Id { get; set; }
            public string DriverName { get; set; }
            public string DriverImage { get; set; }
            public string MobileNumber { get; set; }
            public string VehicleNumber { get; set; }
            public int? VehicleType_Id { get; set; }
            public double Lat_Driver { get; set; }
            public double Lang_Driver { get; set; }
            public int Status { get; set; }
            public double Patient_DropLongtitude { get; set; }
            public double Patient_DropLatitude { get; set; }
            public double Longtitude { get; set; }
            public double Latitude { get; set; }

        }
        public class DriverReturnMessage
        {
            public int? StatusKey { get; set; }
            public double? PreviousAmount { get; set; }
            public double CurrentAmount { get; set; }
            public double TotalAmount { get; set; }
            public int DriveCompleteId { get; set; }
        }

        public class PateintReturnMessage
        {
            public string DriverName { get; set; }
            public string MobileNumber { get; set; }
            public string VehicleNumber { get; set; }
            public double Patient_DropLongtitude { get; set; }
            public double Patient_DropLatitude { get; set; }
            public int? Status { get; set; }
            public int? StatusKey { get; set; }
            public double? PreviousAmount { get; set; }
            public double CurrentAmount { get; set; }
            public double TotalAmount { get; set; }
            public int DriveCompleteId { get; set; }
        }

        public class PatientDetail
        {
            public int Id { get; set; }
            public string PatientName { get; set; }
            public string MobileNumber { get; set; }
            public double Latitude { get; set; }
            public double Longtitude { get; set; }
            public double Patient_DropLongtitude { get; set; }
            public double Patient_DropLatitude { get; set; }
        }

        public class DriveComplete
        {
            public TimeSpan RideEndTime { get; set; }
            public double Amount { get; set; }
            public double Distance { get; set; }
            public int Patient_Id { get; set; }
            public int Driver_Id { get; set; }
            public double Lat_PickUp_Patient { get; set; }
            public double Lang_PickUp_Pateint { get; set; }
            public double Lang_Drop { get; set; }
            public double Lat_Drop { get; set; }
            public string PickUp_Place { get; set; }
            public string Drop_Place { get; set; }
            public int Vehicle_Id { get; set; }
            public int VehicleType_Id { get; set; }
            public bool IsDriveCompleted { get; set; }
            public double? PreviousAmount { get; set; }
            public IEnumerable<VehiclePrice> VehiclePrice { get; set; }
            public double CurrentAmount { get; set; }
            public double TotalAmount { get; set; }
            public int DriveCompleteId { get; set; }
            public string CancelBy { get; set; }
        }

        public class TokenNo
        {
            public string Token { get; set; }
        }

        public class VehiclePrice
        {
            public Nullable<double> under5KM { get; set; }
            public Nullable<double> under6_10KM { get; set; }
            public Nullable<double> under11_20KM { get; set; }
            public Nullable<double> under21_40KM { get; set; }
            public Nullable<double> under41_60KM { get; set; }
            public Nullable<double> under61_80KM { get; set; }
            public Nullable<double> under81_100KM { get; set; }
            public Nullable<double> under100_150KM { get; set; }
            public Nullable<double> under151_200KM { get; set; }
            public Nullable<double> under201_250KM { get; set; }
            public Nullable<double> under251_300KM { get; set; }
            public Nullable<double> under301_350KM { get; set; }
            public Nullable<double> under351_400KM { get; set; }
            public Nullable<double> under401_450KM { get; set; }
            public Nullable<double> under451_500KM { get; set; }
            public Nullable<double> Above500KM { get; set; }
            public Nullable<double> DriverCharge { get; set; }
        }

        public class PatientAmountRecord
        {
            public bool Is8HourRide { get; set; }
            public int Vehicle_Id { get; set; }
            public double? DriverCharges { get; set; }
            public double PreviousAmount { get; set; }
            public decimal CurrentAmount { get; set; }
            public double TotalAmount { get; set; }

        }

        public class ReturnMessage
        {
            public string Message { get; set; }
            public int Status { get; set; }
        }


        public class DriverLocationDT
        {
            public int Id { get; set; }

            public Nullable<double> end_Lat { get; set; }
            public Nullable<double> end_Long { get; set; }
            public Nullable<double> start_Lat { get; set; }
            public Nullable<double> start_Long { get; set; }

            public Nullable<int> Patient_Id { get; set; }
            public Nullable<int> Driver_Id { get; set; }
            public Nullable<int> AmbulanceType_id { get; set; }
            public Nullable<int> VehicleType_id { get; set; }
        }

        //public class UpdatelocationDriver
        //{
        //    public int DriverId { get; set; }
        //    public Nullable<double> Lat { get; set; }
        //    public Nullable<double> Long { get; set; }
        //}
        public class UpdatelocationDriver
        {
            public int DriverId { get; set; }
            public double Lat { get; set; }
            public double Lang { get; set; }
            public Nullable<int> Charge { get; set; }
            public string DriverName { get; set; }
            public string MobileNumber { get; set; }
            public string DlNumber { get; set; }
            public string DeviceId { get; set; }
            
        }
        public class UpdatelocationPatient
        {
            public int PatientId { get; set; }
            public double Lat { get; set; }
            public double Lang { get; set; } 

        }

        public class DriverListNearByUser
        {
            public int Id { get; set; }
            public int DriverId { get; set; }
            public int KM { get; set; }
            public string Name { get; set; }
            public string DL { get; set; }
            public Nullable<int> Charge { get; set; }
            public Nullable<int> TotalPrice { get; set; }
            public string DeviceId { get; set; }
            public string MobileNumber { get; set; }
            public Nullable<int> ToatlDistance { get; set; }
        }

        public class Update_driverid
        {
            public int Id { get; set; }
            public string UserId { get; set; }
            public string Password { get; set; }
            public string DeviceId { get; set; }
        }

        public class UserListForBookingAmbulance
        {
            public int Id { get; set; }
            public int PatientId { get; set; }
            public string PatientName { get; set; }
            public string MobileNumber { get; set; }
            public double endLat { get; set; }
            public double endLong { get; set; }
            public double startLat { get; set; }
            public double startLong { get; set; }
            public string DeviceId { get; set; }
            public Nullable<int> TotalPrice { get; set; }
            public Nullable<int> ToatlDistance { get; set; }


            //CODE FOR LAT LONG TO LOCATION 
            public string ReverseStartLatLong_To_Location
            {
                get { return getlocation(startLat.ToString(), startLong.ToString()); }
            }
            public string ReverseEndLatLong_To_Location
            {
                get { return getlocation(endLat.ToString(), endLong.ToString()); }
            }
            public string Duration
            {
                get { return getlocation(endLat.ToString(), endLong.ToString()); }
            }

            private string getlocation(string latitude, string longitude)
            {
               
                string url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + latitude + "," + longitude + "&key=AIzaSyBrbWFXlOYpaq51wteSyFS2UjdMPOWBlQw";

                // Make the HTTP request.
                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 10000;

                // Get the response.
                WebResponse response = request.GetResponse();
                string responseText = new StreamReader(response.GetResponseStream()).ReadToEnd();

                // Parse the response JSON.
                var json = JsonConvert.DeserializeObject<dynamic>(responseText);

                // Get the location from the JSON.
                var location = json.results[0].formatted_address;
                return location;
            }

            //END CODE FOR LAT LONG TO LOCATION 
        }

        public class BookingAmbulanceAcceptRejectDTO
        {
            public int Id { get; set; }
            public int DriverId { get; set; }
            public string StatusId { get; set; }
            public Nullable<bool> RejectedStatus { get; set; }
        }

        public class GetAcceptedReq_DriverDetail 
        {
            public int Id { get; set; }
            public int DriverId { get; set; }
            public string DriverName { get; set; }
            public string MobileNumber { get; set; }
            public string DeviceId { get; set; }
            public string DriverImage { get; set; }
            public string DlNumber { get; set; }
            public Nullable<int> TotalPrice { get; set; }
            public Nullable<decimal> PayableAmount { get; set; }
            public string VehicleNumber { get; set; }
            public string VehicleTypeName { get; set; }
            public Nullable<int> ToatlDistance { get; set; }

        }

        public class Driver_PayNow
        {
            public int Id { get; set; }
            public int PatientId { get; set; }
            public int Driver_Id { get; set; }
            public decimal Amount { get; set; }

        }

        public class getdriverbookinglist
        {
            public int Id { get; set; }
            public int DriverId { get; set; }
            public string DriverName { get; set; }
            public string InvoiceNumber { get; set; }
            public string MobileNumber { get; set; }
            public string DriverImage { get; set; }
            public string DlNumber { get; set; }
            public Nullable<int> TotalPrice { get; set; }
            public Nullable<decimal> PaidAmount { get; set; }
            public Nullable<decimal> RemainingAmount { get; set; }
            public string VehicleNumber { get; set; }
            public string VehicleTypeName { get; set; }
            public Nullable<int> ToatlDistance { get; set; }
            public Nullable<double> Lat_Driver { get; set; }
            public Nullable<double> Lang_Driver { get; set; }
            public Nullable<double> UserLat { get; set; }
            public Nullable<double> UserLong { get; set; } 
            public Nullable<double> end_Lat { get; set; }
            public Nullable<double> end_Long { get; set; }
            public Nullable<double> DriverUserDistance { get; set; }
            public double ExpectedTime { get; set; }
            public Nullable<System.DateTime> PaymentDate { get; set; }
            //CODE FOR LAT LONG TO LOCATION 
            public string PickupLocation
            {
                get { return getlocation(UserLat.ToString(), UserLong.ToString()); }
            }
            public string DropLocation
            {
                get { return getlocation(end_Lat.ToString(), end_Long.ToString()); }
            }

            private string getlocation(string latitude, string longitude)
            {
                string url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + latitude + "," + longitude + "&key=AIzaSyBrbWFXlOYpaq51wteSyFS2UjdMPOWBlQw";

                // Make the HTTP request.
                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 10000;

                // Get the response.
                WebResponse response = request.GetResponse();
                string responseText = new StreamReader(response.GetResponseStream()).ReadToEnd();

                // Parse the response JSON.
                var json = JsonConvert.DeserializeObject<dynamic>(responseText);

                // Get the location from the JSON.
                var location = json.results[0].formatted_address;
                return location;
            }

            //END CODE FOR LAT LONG TO LOCATION 

        }

        public class Days
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
