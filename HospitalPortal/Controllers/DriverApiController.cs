using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HospitalPortal.BL;
using static HospitalPortal.Controllers.TestApiController;
using HospitalPortal.Models.APIModels;
using ReturnMessage = HospitalPortal.Models.APIModels.ReturnMessage;
using AutoMapper;
using System.Web.Services.Description;
using HospitalPortal.Utilities;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.Http.Results;
using System.Configuration;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Web.Razor.Parser.SyntaxTree;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.EMMA;

namespace HospitalPortal.Controllers
{
    public class DriverApiController : ApiController
    {
        DbEntities ent = new DbEntities();
        returnMessage rm = new returnMessage();
        Notification send = new Notification();
        
        [HttpGet]
        public IHttpActionResult UpdateVehicle(int? id, string VehicleNumber = null)
        {
            if (!string.IsNullOrEmpty(VehicleNumber))
            {
                var vehicle = ent.Vehicles.Any(a => a.VehicleNumber == VehicleNumber);
                if (vehicle != false)
                {
                    int vehicleid = ent.Database.SqlQuery<int>(@"select id from vehicle where vehiclenumber='" + VehicleNumber + "'").FirstOrDefault();
                    //int Id = ent.Database.SqlQuery<int>(@"select Id from Vehicle where Driver_Id=" + id).FirstOrDefault();
                    string q = @"update Vehicle set Driver_Id = " + id + "  where Id=" + vehicleid;
                    ent.Database.ExecuteSqlCommand(q);
                    string V_Number = ent.Database.SqlQuery<string>("select VehicleNumber from Vehicle where Driver_Id=" + id).FirstOrDefault();
                    string DriverName = ent.Database.SqlQuery<string>("select DriverName from Driver where Id=" + id).FirstOrDefault();
                    rm.Message = "The Vehicle Number " + VehicleNumber + " has been Replaced to " + DriverName;
                    rm.Status = 1;



                    //int Id = ent.Database.SqlQuery<int>(@"select Id from Vehicle where Driver_Id=" + id).FirstOrDefault();
                    //string q = @"update Vehicle set Driver_Id = " + id + "  where Id=" + Id;
                    //ent.Database.ExecuteSqlCommand(q);
                    //string V_Number = ent.Database.SqlQuery<string>("select VehicleNumber from Vehicle where Driver_Id=" + id).FirstOrDefault();
                    //string DriverName = ent.Database.SqlQuery<string>("select DriverName from Driver where Id=" + id).FirstOrDefault();
                    //rm.Message = "The Vehicle Number " + VehicleNumber + " has been Replaced to " + DriverName;
                    //rm.Status = 1;
                }
                else
                {
                    rm.Message = "Kindly Contact to Administrator";
                    rm.Status = 0;
                }
            }
            else
            {
                rm.Message = "Kindly Contact to Administrator";
                rm.Status = 0;
            }
            return Ok(rm);
        }


        [HttpPost]
        public IHttpActionResult DriveCompleted(DriveComplete model)
        {
            DriverReturnMessage driverRM = new DriverReturnMessage();
            double? amt;
            string Token;
            int temp = 0;
            //if (!ModelState.IsValid)
            //{
            //    var message = string.Join(" | ", ModelState.Values.SelectMany(b => b.Errors).Select(b => b.ErrorMessage));
            //    rm.Message = message;
            //    rm.Status = 0;
            //    return Ok(rm);
            //}
            try
            {
                //Get the Kilometer using 2 Long. & Lat. 
                double dDistance = Double.MinValue;
                double dLat1InRad = model.Lat_PickUp_Patient * (Math.PI / 180.0);
                double dLong1InRad = model.Lat_Drop * (Math.PI / 180.0);
                double dLat2InRad = model.Lang_PickUp_Pateint * (Math.PI / 180.0);
                double dLong2InRad = model.Lang_Drop * (Math.PI / 180.0);

                double dLongitude = dLong2InRad - dLong1InRad;
                double dLatitude = dLat2InRad - dLat1InRad;

                // Intermediate result a.
                double a = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) +
                           Math.Cos(dLat1InRad) * Math.Cos(dLat2InRad) *
                           Math.Pow(Math.Sin(dLongitude / 2.0), 2.0);

                // Intermediate result c (great circle distance in Radians).
                double c = 2.0 * Math.Asin(Math.Sqrt(a));
                // Distance.
                // const Double kEarthRadiusMiles = 3956.0;
                const Double kEarthRadiusKms = 6376.5;
                dDistance = kEarthRadiusKms * c;

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
                var domain = new TravelRecordMaster();
                domain.FullAmount = (decimal)amt;
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
                ent.SaveChanges();
                //Update the Drive Compeleted in Travel Master Table
                string q = @"update TravelRecordMaster set FullAmount =" + amt + " where Id=" + model.DriveCompleteId;
                ent.Database.ExecuteSqlCommand(q);
                //string q1 = @"update PatientRequest set StatusKey = 4 where Patient_Id=" + model.Patient_Id;
                //ent.Database.ExecuteSqlCommand(q1);
                string q1 = @"delete from TravelMaster where Driver_Id=" + model.Driver_Id;
                ent.Database.ExecuteSqlCommand(q1);
                string q2 = @"delete from PatientRequest where Patient_Id=" + model.Patient_Id;
                ent.Database.ExecuteSqlCommand(q2);

                //Push Notification
                Token = ent.Database.SqlQuery<string>("select Token from AdminLogin al join Patient p on p.AdminLogin_Id = al.Id where p.Id=" + model.Patient_Id).FirstOrDefault();
                if (!string.IsNullOrEmpty(Token))
                {
                    send.Message(Token, "PsWellness", "Your Amount is " + amt + "");
                }
                model.Vehicle_Id = temp;
                double amount = ent.Database.SqlQuery<double>(@"select Amount from TravelRecordMaster where Id=" + model.DriveCompleteId).FirstOrDefault();
                driverRM.PreviousAmount = amount;
                driverRM.CurrentAmount = (double)amt;
                driverRM.TotalAmount = (double)amt - amount;
                driverRM.DriveCompleteId = model.DriveCompleteId;
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                rm.Message = "Server Error";
                rm.Status = 0;
                return Ok(rm);
            }
            return Ok(driverRM);
        }
        //update profile driver ====================[08/04/2023]==============================
        [HttpPost]
        public IHttpActionResult UpdateProfile(DriverUpdateProfile model)
        {
            try
            {
                var data = ent.Drivers.Where(a => a.Id == model.Id).FirstOrDefault();

                data.DriverName = model.DriverName;
                data.MobileNumber = model.MobileNumber;
                data.EmailId = model.EmailId;
                data.StateMaster_Id = model.StateMaster_Id;
                data.CityMaster_Id = model.CityMaster_Id;
                data.Location = model.Location;
                data.PinCode = model.PinCode;
                //data.DlNumber = model.DlNumber;
                //data.DlImage = model.DlImage;


                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = "Driver profile Successfully updated.";
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/DriverApi/getAppointmentDetail")]
        public IHttpActionResult getAppointmentDetail(int Id)
        {
            var rm = new Driver();
            string query = @"select Driver.Id,Driver.DriverName,Driver.VehicleName,Driver.Location,Driver.JoiningDate ,Driver.DlImage from Driver where Id= " + Id + "";
            var AppointmentDetail = ent.Database.SqlQuery<AppoinmentDetails>(query).ToList();
            return Ok(new { AppointmentDetail });
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/DriverApi/BookingHistory")]
        public IHttpActionResult BookingHistory(int DriverId)
        { 
            string query = @"select P.Id,P.PatientName,P.MobileNumber,sm.StateName,cm.CityName,P.PinCode,P.Location,DL.start_Lat,DL.start_Long,DL.end_Long,DL.end_Lat from DriverLocation as DL left join Patient as P on Dl.PatientId=P.Id left join citymaster as cm with(nolock) on cm.id=P.CityMaster_Id left join statemaster as sm with(nolock) on sm.id=P.StateMaster_Id left join Driver as D on D.Id=DL.Driver_Id where D.Id=" + DriverId + " and DL.IsPay='Y'";
            var BookingHistory = ent.Database.SqlQuery<BookingOrderHistory>(query).ToList();
            return Ok(new { BookingHistory });

        }

        [System.Web.Http.HttpGet]
        [Route("api/DriverApi/PaymentHistory")]
        public IHttpActionResult PaymentHistory(int Id)
        {
            var rm = new Driver();
            string query = @"select P.Id,P.PatientName,P.MobileNumber,P.Location,DL.Id as PaymentId,DL.Amount,DL.PaymentDate,DL.IsPay,DL.start_Lat,DL.start_Long,DL.end_Long,DL.end_Lat from DriverLocation as DL left join Patient as P on Dl.PatientId=P.Id left join Driver as D on D.Id=DL.Driver_Id where D.Id=" + Id + " and DL.IsPay='Y'";
            var Data = ent.Database.SqlQuery<payhistory>(query).ToList();
            return Ok(Data);

        }

        [HttpGet]
        [Route("api/DriverApi/payouthistory")]
        public IHttpActionResult PayoutHistory(int id)
        {
            var rm = new Driver();
            string query = @"select D.Id,DL.Amount,DL.PaymentDate from Driver as D join DriverPayOut as DL on D.Id=Dl.Driver_Id where D.Id=" + id + "";
            var data = ent.Database.SqlQuery<payouthistory>(query).ToList();
            return Ok(data);
        }


        [HttpPost]
        //driver complaint api ===========[14/04/2023]
        public IHttpActionResult DriverComplaintss(DriverComplaints model)
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
                var Driver_Complaint = AutoMapper.Mapper.Map<DriverComplaint>(model);
                Driver_Complaint.Others = model.Others;
                Driver_Complaint.Subjects = model.Subjects;
                Driver_Complaint.Complaints = model.Complaints;
                Driver_Complaint.IsDeleted = false;
                Driver_Complaint.IsResolved = false;
                ent.SaveChanges();
                ent.DriverComplaints.Add(Driver_Complaint);

                rm.Message = "Successfully Forwarded to Higher Authority.";
                rm.Status = 1;
                return Ok(rm);
            }
        }

        [HttpGet]
        public IHttpActionResult GetDriverProfile(int Id)
        { 
            string query = @"select D.Id,D.DriverName,D.EmailId,D.MobileNumber,SM.StateName,CM.CityName,D.PinCode,D.Location,D.StateMaster_Id,D.CityMaster_Id from Driver as D left join StateMaster as SM with(nolock) on SM.Id=D.StateMaster_Id left join CityMaster as CM with(nolock) on CM.Id=D.CityMaster_Id where D.Id= " + Id + "";
            var data = ent.Database.SqlQuery<GetDriverProfile>(query).FirstOrDefault();
            return Ok(data);
        }


        [HttpPost, Route("api/DriverApi/BookDriver")]
        public IHttpActionResult BookDriver(DriverLocationDT model)
        {
            try
            {
                var mod = new DriverLocation()
                {
                    start_Lat = model.start_Lat,
                    start_Long = model.start_Long,
                    end_Long = model.end_Long,
                    end_Lat = model.end_Lat,
                    PatientId = model.Patient_Id,
                    Driver_Id = model.Driver_Id,
                    AmbulanceType_id = model.AmbulanceType_id,
                    VehicleType_id = model.VehicleType_id,
                    EntryDate = DateTime.Now,
                    IsPay = "N",
                    Status = "0"

                };
                // ent.DriverLocations.Add(mod);
                ent.SaveChanges();
                return Ok(new { model.VehicleType_id, model.start_Lat, model.start_Long, Message = "Driver Booked SuccessFully " });
            }
            catch
            {
                return BadRequest("Server Error");
            }
        }


        [HttpGet, Route("api/DriverApi/GetVehicleTypeDropdown")]

        public IHttpActionResult GetVehicleTypeDropdown()
        {
            string qry = @"select Id,VehicleTypeName from VehicleType where IsDeleted=0";
            var VehicleType = ent.Database.SqlQuery<Vehicle_type>(qry).ToList();
            return Ok(new { VehicleType });
        }

        [HttpPost, Route("api/DriverApi/UpdateDriverLocation")]
        public IHttpActionResult UpdateDriverLocation(UpdatelocationDriver model)
        {
            try
            {
                var data = ent.Drivers.FirstOrDefault(a => a.Id == model.DriverId);
                data.Lat = model.Lat;
                data.Lang = model.Lang;
                ent.SaveChanges();
                return Ok(new { Message = "Update Driver Location SuccessFully" });
            }
            catch
            {
                return BadRequest("Server Error");
            }
        }

        [HttpPost, Route("api/DriverApi/AddAmbulance")]
        public IHttpActionResult AddAmbulance(DriverLocationDT model)
        {
            try
            {
                double DriveLat = 0.00;
                double DriveLong = 0.00;
                double distance = 0.00;
                int Charge = 0;
                ent.Database.ExecuteSqlCommand("exec DeleteNearDriver");


                List<DriverListNearByUser> driverListNearByUser = new List<DriverListNearByUser>();
//                var Driver = ent.Database.SqlQuery<UpdatelocationDriver>(@"select D.Id AS DriverId,D.Lat, D.Lang,D.DriverName,D.DlNumber,dbo.DriverCharges() as Charge,AL.DeviceId from Driver AS D with(nolock)
//INNER JOIN AdminLogin AS AL with(nolock) ON D.AdminLogin_Id = AL.Id
//where D.Lat IS NOT NULL and D.Lang IS NOT NULL and d.VehicleType_id=" + model.VehicleType_id + " and D.IsApproved=1").ToList();
                
                var Driver = ent.Database.SqlQuery<UpdatelocationDriver>(@"select D.Id AS DriverId,D.Lat, D.Lang,D.DriverName,D.DlNumber,dbo.DriverCharges() as Charge,AL.DeviceId from Driver AS D with(nolock)
INNER JOIN AdminLogin AS AL with(nolock) ON D.AdminLogin_Id = AL.Id
INNER JOIN Vehicle as v on V.VehicleType_Id=d.VehicleType_id
INNER JOIN VehicleType as vt on Vt.Id=v.VehicleType_id
where D.Lat IS NOT NULL and D.Lang IS NOT NULL and d.VehicleType_id=" + model.VehicleType_id + " and D.IsApproved=1").ToList();
                foreach (var item in Driver)
                {
                    //==================USER AND DRIVER DISTANCE========================
                    // Driver

                    var lat1 = item.Lat;
                    var lon1 = item.Lang;

                    //Save DriverLat and Long 
                    DriveLat = lat1;
                    DriveLong = lon1;

                    //User
                    var lat2 = model.start_Lat;
                    var lon2 = model.start_Long;

                    double rlat1 = Math.PI * lat1 / 180;
                    double rlat2 = (double)(Math.PI * lat2 / 180);
                    double theta = (double)(lon1 - lon2);
                    double rtheta = Math.PI * theta / 180;
                    double dist =
                        Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                        Math.Cos(rlat2) * Math.Cos(rtheta);
                    dist = Math.Acos(dist);
                    dist = dist * 180 / Math.PI;
                    dist = dist * 60 * 1.1515; 

                    // TOTAL DISTANCE (START LOCTION TO DESTINATION)

                    //User
                    var Startlat = model.start_Lat;
                    var Startlong = model.start_Long;
                    var Endlat = model.end_Lat;
                    var Endlong = model.end_Long;

               
                    // Create the URL for the Google Maps API.
                    string url = "https://maps.googleapis.com/maps/api/distancematrix/json?origins=" + Startlat + "," + Startlong + "&destinations=" + Endlat + "," + Endlong + "&key=AIzaSyBrbWFXlOYpaq51wteSyFS2UjdMPOWBlQw";
                    
                    // Make the HTTP request to the API.
                    WebRequest request = WebRequest.Create(url);
                    request.Method = "GET";
                    request.Timeout = 30000;
                    WebResponse response = request.GetResponse();


                    string responseText = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    // Parse the response JSON.
                    var json = JsonConvert.DeserializeObject<dynamic>(responseText);



                    // Get the distance between the two places.
                    var distancetext = ExtractDecimalValue((json.rows[0].elements[0].distance.value).ToString());

                    //for duration

                    //var duration = ExtractDecimalValue((json.rows[0].elements[0].duration.value).ToString());
                    //duration = json.rows[0].elements[0].duration.value / 60;
                    //durationtime = duration;
                    //if (durationtime > 60)
                    //{
                    //    durationtime = duration/60;
                    //}

                    //distance = double.Parse(json.rows[0].elements[0].distance.text);
                    //distance=double.Parse(distancetext);
                    distance = json.rows[0].elements[0].distance.value/1000;
                    // Print the distance.
                    Console.WriteLine("The distance between the two places is " + distance + " meters.");

                    ;
                    int drivId = item.DriverId;
                    string drivName = item.DriverName;
                    string DlNumber = item.DlNumber;
                    Charge = item.Charge ?? 0;
                    //dist1 = distance;

                    string DeviceId = (item.DeviceId == null) ? "0000" : item.DeviceId;
                    string query = @"exec GetNearDriver '" + drivId + "'," + dist + ",'" + drivName + "','" + DlNumber + "'," + Charge + ",'" + DeviceId + "'," + distance + "";


                    driverListNearByUser = ent.Database.SqlQuery<DriverListNearByUser>(query).ToList();
                }
                
                var mod = new DriverLocation()
                {
                     
                    Lat_Driver = DriveLat,
                    Lang_Driver = DriveLong,
                    //Driver_Id = model.Driver_Id,
                    start_Lat = model.start_Lat,
                    start_Long = model.start_Long,
                    end_Long = model.end_Long,
                    end_Lat = model.end_Lat,
                    PatientId = model.Patient_Id,
                    AmbulanceType_id = model.AmbulanceType_id,
                    VehicleType_id = model.VehicleType_id,
                    EntryDate = DateTime.Now,
                    IsPay = "N",
                    Status = "0",
                    RejectedStatus =false ,
                    ToatlDistance = Convert.ToInt32(distance),
                    TotalPrice = Charge * Convert.ToInt32(distance)
                };
                ent.DriverLocations.Add(mod);
                ent.SaveChanges();
                return Ok(new { model.start_Lat, model.start_Long, model.end_Long, model.end_Lat, model.AmbulanceType_id, model.VehicleType_id, model.Patient_Id, Message = driverListNearByUser });

            }
            catch (Exception Ex)
            {
                return BadRequest("Server Error");
            }
        }
        static string ExtractDecimalValue(string text)
        {
            // Define the regular expression pattern to match decimal values

            //string pattern = @"\b\d+\b"; only for integer
            //string pattern = @"\b(\d+\.\d+|\d+)\b";  for both in and decimal

            string pattern = @"\b\d+\.\d+\b";

            // Use Regex.Match to find the first occurrence of the pattern
            Match match = Regex.Match(text, pattern);

            // Check if a match is found and return the value
            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                // If no decimal value is found, you may handle it as per your requirement
                return "No decimal value found";
            }




        }
        //===========================UpadateDiviceId==========================//

        [HttpPost]
        [Route("api/DriverApi/UpadateDiviceId")]
        public IHttpActionResult UpadateDiviceId(Update_driverid model)
        {
            try
            {
                string qry = @"Update AdminLogin Set DeviceId='" + model.DeviceId + "' Where UserId='" + model.UserId + "'";
                ent.Database.ExecuteSqlCommand(qry);
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = "Device Id Updated Successfully!!!";
            }
            catch (Exception ex)
            {
                rm.Status = 0;
                return BadRequest("Server Error");
            }
            return Ok(rm);

        }

        [HttpGet, Route("api/DriverApi/UserListForBookingAmbulance")]
        public IHttpActionResult UserListForBookingAmbulance()
        {
            var data = ent.Database.SqlQuery<UserListForBookingAmbulance>(@"select D.Id,D.PatientId,P.PatientName, P.MobileNumber, 
D.end_Lat AS endLat,D.end_Long As endLong,D.start_Lat AS startLat,
D.start_Long AS startLong,AL.DeviceId,D.TotalPrice,D.ToatlDistance
from DriverLocation AS D with(nolock)
INNER JOIN Patient AS P with(nolock) ON D.PatientId =P.Id
inner join AdminLogin as AL on AL.Id=P.AdminLogin_Id 
WHERE D.[Status] = 0 and D.RejectedStatus=0").ToList();

            return Ok(new { UserListForBookingAmbulance = data });

        }

        [HttpPost, Route("api/DriverApi/BookingAmbulanceAcceptReject")]
        public IHttpActionResult BookingAmbulanceAcceptReject(BookingAmbulanceAcceptRejectDTO bookingAmbulanceAcceptRejectDTO)
        {
            var data = ent.DriverLocations.Where(a => a.Id == bookingAmbulanceAcceptRejectDTO.Id).FirstOrDefault();
            data.Status = bookingAmbulanceAcceptRejectDTO.StatusId;
            data.Driver_Id = bookingAmbulanceAcceptRejectDTO.DriverId;
            ent.SaveChanges();
            return Ok("Success");
        }

        [HttpPost, Route("api/DriverApi/AmbulanceReject")]
        public IHttpActionResult AmbulanceReject(BookingAmbulanceAcceptRejectDTO bookingAmbulanceAcceptRejectDTO)
        {
            var data = ent.DriverLocations.Where(a => a.Id == bookingAmbulanceAcceptRejectDTO.Id).FirstOrDefault();
            data.RejectedStatus = true;
            data.Driver_Id = bookingAmbulanceAcceptRejectDTO.DriverId;
            ent.SaveChanges();
            return Ok("Success");
        }

        [HttpGet, Route("api/DriverApi/GetAcceptedReqDriverDetail")]
        public IHttpActionResult GetAcceptedReqDriverDetail(int Id)
        {
            string qry = @"SELECT DL.Id,D.Id AS DriverId,D.DriverName,D.MobileNumber,D.DlNumber,D.DriverImage,ND.TotalPrice,V.VehicleNumber,VT.VehicleTypeName,ND.ToatlDistance FROM Driver as D INNER JOIN NearDriver AS ND ON ND.DriverId=D.Id INNER JOIN Vehicle AS V ON V.Driver_Id=D.Id INNER JOIN DriverLocation AS DL ON D.Id=DL.Driver_Id INNER JOIN VehicleType AS VT ON VT.Id=V.VehicleType_Id WHERE ND.Id="+Id+" and DL.[Status]=1 and DL.IsPay='N' order by DL.Id desc";
            var data = ent.Database.SqlQuery<GetAcceptedReq_DriverDetail>(qry).FirstOrDefault();
            return Ok(data);
        }

        [HttpPost, Route("api/DriverApi/DriverPayNow")]
        public IHttpActionResult DriverPayNow(Driver_PayNow DriverPayNow)
        {
            try
            {
                var data = ent.DriverLocations.Where(a => a.PatientId == DriverPayNow.PatientId && a.Driver_Id == DriverPayNow.Driver_Id && a.IsPay == "N").OrderByDescending(a => a.Id).FirstOrDefault();

                data.Amount = DriverPayNow.Amount;
                data.IsPay = "Y";
                data.PaymentDate = DateTime.Now;
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = "Payment Success";
            }
            catch (Exception ex)
            {
                rm.Status = 0;
                return BadRequest("Server Error");
            }
            return Ok(rm);

        }

        [System.Web.Http.HttpGet, Route("api/DriverApi/GetDriverBookingHistory")]

        //use user section
        public IHttpActionResult GetDriverBookingHistory(int PatientId)
        {
            var driverDetails = (from dl in ent.DriverLocations
                                 join d in ent.Drivers on dl.Driver_Id equals d.Id
                                 join v in ent.Vehicles on dl.VehicleType_id equals v.VehicleType_Id
                                 join vt in ent.VehicleTypes on dl.VehicleType_id equals vt.Id
                                 join p in ent.Patients on dl.PatientId equals p.Id
                                 where dl.IsPay == "Y" && p.Id == PatientId
                                 orderby dl.Id descending 
                                 select new getdriverbookinglist
                                 {
                                     Id = dl.Id,
                                     DriverId = d.Id,
                                     DriverName = d.DriverName,
                                     MobileNumber = d.MobileNumber,
                                     DlNumber = d.DlNumber,
                                     DriverImage = d.DriverImage,
                                     VehicleNumber = v.VehicleNumber,
                                     VehicleTypeName = vt.VehicleTypeName,
                                     ToatlDistance = dl.ToatlDistance,
                                     PaidAmount = dl.Amount,
                                     TotalPrice = dl.TotalPrice,
                                     RemainingAmount = dl.TotalPrice - dl.Amount,
                                     UserLat = dl.start_Lat,
                                     UserLong = dl.start_Long,
                                     Lat_Driver = dl.Lat_Driver,
                                     Lang_Driver = dl.Lang_Driver,
                                     //Lat_Driver = d.Lat,
                                     //Lang_Driver = d.Lang,
                                     PaymentDate = dl.PaymentDate,
                                 }).ToList();

            foreach (var detail in driverDetails)
            {
                // Driver
                var lat1 = (double)detail.Lat_Driver;
                var lon1 = (double)detail.Lang_Driver;

                // User
                var lat2 = (double)detail.UserLat;
                var lon2 = (double)detail.UserLong;

                double rlat1 = Math.PI * lat1 / 180;
                double rlat2 = Math.PI * lat2 / 180;
                double theta = lon1 - lon2;
                double rtheta = Math.PI * theta / 180;

                double dist = Math.Sin(rlat1) * Math.Sin(rlat2) +
                              Math.Cos(rlat1) * Math.Cos(rlat2) * Math.Cos(rtheta);

                dist = Math.Acos(dist);
                dist = dist * 180 / Math.PI;
                dist = dist * 60 * 1.1515; // Convert miles to kilometers
                dist = dist * 1.609344;   // Convert miles to kilometers

                detail.DriverUserDistance = dist;

                // Calculate expected time
                //double expectedTimeMinutes = dist * 4; // 2 minutes per kilometer

                //detail.ExpectedTime = expectedTimeMinutes;

                double expectedTimeMinutes = dist * 4; // 2 minutes per kilometer

                // Convert the expectedTimeMinutes to an integer
                int expectedTimeMinutesInt = Convert.ToInt32(expectedTimeMinutes);

                detail.ExpectedTime = expectedTimeMinutesInt;
            }

            return Ok(new { driverDetails });
        }
         
        //public double CalculateHaversineDistance(double userLat, double userLong, double driverLat, double driverLong)
        //{
        //    double earthRadius = 6371; // Earth's radius in kilometers

        //    double lat1 = userLat * Math.PI / 180;
        //    double lon1 = userLong * Math.PI / 180;
        //    double lat2 = driverLat * Math.PI / 180;
        //    double lon2 = driverLong * Math.PI / 180;

        //    double dlat = lat2 - lat1;
        //    double dlon = lon2 - lon1;

        //    double a = Math.Sin(dlat / 2) * Math.Sin(dlat / 2) +
        //               Math.Cos(lat1) * Math.Cos(lat2) *
        //               Math.Sin(dlon / 2) * Math.Sin(dlon / 2);

        //    double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        //    return earthRadius * c;
        //}

        [HttpGet, Route("api/DriverApi/GetAcceptedandPaidamtDriverDetail")]
        public IHttpActionResult GetAcceptedandPaidamtDriverDetail(int Id)
        {
            var driverDetails = (from d in ent.Drivers
                                 join dl in ent.DriverLocations on d.Id equals dl.Driver_Id
                                 join v in ent.Vehicles on dl.VehicleType_id equals v.VehicleType_Id
                                 join vt in ent.VehicleTypes on dl.VehicleType_id equals vt.Id
                                 where dl.PatientId == Id && dl.Status == "1" && dl.IsPay == "Y"
                                 orderby dl.Id descending
                                 select new getdriverbookinglist
                                 {
                                     Id = dl.Id,
                                     DriverId = d.Id,
                                     DriverName = d.DriverName,
                                     MobileNumber = d.MobileNumber,
                                     DlNumber = d.DlNumber,
                                     DriverImage = d.DriverImage,
                                     VehicleNumber = v.VehicleNumber,
                                     VehicleTypeName = vt.VehicleTypeName,
                                     ToatlDistance = dl.ToatlDistance,
                                     PaidAmount = dl.Amount,
                                     TotalPrice = dl.TotalPrice,
                                     RemainingAmount = dl.TotalPrice - dl.Amount,
                                     UserLat = dl.start_Lat,
                                     UserLong = dl.start_Long,
                                     Lat_Driver = d.Lat,
                                     Lang_Driver = d.Lang,
                                     PaymentDate = dl.PaymentDate,
                                 }).FirstOrDefault();



            if (driverDetails != null)
            {
                // Driver
                var lat1 = (double)driverDetails.Lat_Driver;
                var lon1 = (double)driverDetails.Lang_Driver;

                // User
                var lat2 = (double)driverDetails.UserLat;
                var lon2 = (double)driverDetails.UserLong;

                double rlat1 = Math.PI * lat1 / 180;
                double rlat2 = Math.PI * lat2 / 180;
                double theta = lon1 - lon2;
                double rtheta = Math.PI * theta / 180;

                double dist = Math.Sin(rlat1) * Math.Sin(rlat2) +
                              Math.Cos(rlat1) * Math.Cos(rlat2) * Math.Cos(rtheta);

                dist = Math.Acos(dist);
                dist = dist * 180 / Math.PI;
                dist = dist * 60 * 1.1515; // Convert miles to kilometers
                dist = dist * 1.609344;   // Convert miles to kilometers

                driverDetails.DriverUserDistance = dist;

                // Calculate expected time
                double expectedTimeMinutes = dist * 2; // 2 minutes per kilometer

                // Convert the expectedTimeMinutes to an integer
                int expectedTimeMinutesInt = Convert.ToInt32(expectedTimeMinutes);

                driverDetails.ExpectedTime = expectedTimeMinutesInt;
            }

            return Ok(driverDetails);
        }


    }
}
