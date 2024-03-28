using DocumentFormat.OpenXml.Wordprocessing;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    public class AmbulancePaymentController : Controller
    {
        DbEntities ent = new DbEntities();

        public ActionResult Show(int? Id)
        {
            var model = new AmbulanceList();
            if(Id > 0)
            {
                var doctor1 = @"SELECT distinct v.Id AS VehicleId  ,v.VehicleNumber,d.DriverName,ISNULL(v.VehicleName, 'NA') AS VehicleName
    FROM DriverLocation trm
    JOIN Driver d ON d.Id = trm.Driver_Id
    JOIN Vehicle v ON v.VehicleType_Id = trm.VehicleType_id   
    WHERE trm.IsPay = 'Y' AND trm.EntryDate BETWEEN DATEADD(DD, -7, GETDATE()) AND GETDATE() group by v.Id,v.VehicleNumber, d.DriverName,trm.Id,v.VehicleName";
                var data1 = ent.Database.SqlQuery<AmbulanceReport>(doctor1).ToList();
                model.Ambulance = data1;
                return View(model);
            }
//            var doctor = @"select v.VehicleNumber as DriverName, IsNull(v.VehicleName,'NA') as VehicleName, 
//v.Id as VehicleId
//from TravelRecordMaster trm 
//join Driver d on d.Id = trm.Driver_Id
//join Vehicle v on v.Id = trm.Vehicle_Id
//join Patient p on p.Id = trm.Patient_Id
//where trm.IsDriveCompleted = 1 and trm.RequestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() group by v.VehicleNumber, v.VehicleName, 
//v.Id";
            var doctor = @"SELECT distinct v.Id AS VehicleId  ,v.VehicleNumber,d.DriverName,ISNULL(v.VehicleName, 'NA') AS VehicleName
    FROM DriverLocation trm
    JOIN Driver d ON d.Id = trm.Driver_Id
    JOIN Vehicle v ON v.VehicleType_Id = trm.VehicleType_id   
    WHERE trm.IsPay = 'Y' AND trm.EntryDate BETWEEN DATEADD(DD, -7, GETDATE()) AND GETDATE() group by v.Id,v.VehicleNumber, d.DriverName,trm.Id,v.VehicleName";
            var data = ent.Database.SqlQuery<AmbulanceReport>(doctor).ToList();
            model.Ambulance = data;
            return View(model);
        }

        public ActionResult ViewDetails(int id, DateTime? sdate, DateTime? edate)
        {
             var model = new AmbulanceList();
            var vehicle = @"select * from Vehicle join Driver on Driver.Id = Vehicle.Driver_Id where Vehicle.Id=" + id;
            var mek = ent.Database.SqlQuery<AmbulanceList>(vehicle).ToList();
            model.VehicleName = mek.FirstOrDefault().VehicleName;
            model.VehicleNumber = mek.FirstOrDefault().VehicleNumber;
            model.DriverName = mek.FirstOrDefault().DriverName;
            if (sdate != null && edate != null)
            {
                var doct = @"WITH RankedResults AS (
    SELECT trm.id,
           p.PatientName,
           p.PatientRegNo,
           CONCAT(
               DAY(trm.PaymentDate), 
               ' ', 
               UPPER(FORMAT(trm.PaymentDate, 'MMM')), 
               ' ', 
               YEAR(trm.PaymentDate), 
               ' ', 
               FORMAT(trm.PaymentDate, 'hh:mm tt')
           ) AS PaymentDate,
           v.VehicleNumber,
           ISNULL(v.VehicleName, 'NA') AS VehicleName,
           trm.TotalPrice,
           trm.ToatlDistance AS Distance,
           d.DriverName,
           v.Id AS VehicleId,
           trm.start_Lat,
           trm.start_Long,
           trm.end_Lat,
           trm.end_Long,
           ROW_NUMBER() OVER (PARTITION BY trm.Id ORDER BY trm.id) AS RowNum
    FROM DriverLocation trm
    JOIN Driver d ON d.Id = trm.Driver_Id
    JOIN Vehicle v ON v.VehicleType_Id = trm.VehicleType_id
    JOIN Patient p ON p.Id = trm.PatientId
    WHERE trm.IsPay = 'Y'  AND CONVERT(DATETIME, trm.PaymentDate, 103) BETWEEN @sdate AND @edate) SELECT id, PatientName, PaymentDate, PatientRegNo, VehicleNumber, VehicleName, TotalPrice, Distance, DriverName, VehicleId, start_Lat, start_Long, end_Lat, end_Long FROM RankedResults WHERE RowNum = 1;";
               // var doctor1 = ent.Database.SqlQuery<AmbulanceReport>(doct).ToList();
				var doctor1 = ent.Database.SqlQuery<AmbulanceReport>(doct,
			new SqlParameter("sdate", sdate),
			new SqlParameter("edate", edate)).ToList();
				model.Ambulance = doctor1;
            }
            else
            { 
                var doctor1 = @"WITH RankedResults AS (
    SELECT trm.id,
           p.PatientName,
		   p.PatientRegNo,
		  CONCAT(
        DAY(trm.PaymentDate), 
        ' ', 
        UPPER(FORMAT(trm.PaymentDate, 'MMM')), 
        ' ', 
        YEAR(trm.PaymentDate), 
        ' ', 
        FORMAT(trm.PaymentDate, 'hh:mm tt')
    ) AS PaymentDate,
           v.VehicleNumber,
           ISNULL(v.VehicleName, 'NA') AS VehicleName,
           trm.TotalPrice,
           trm.ToatlDistance AS Distance,
           d.DriverName,
           v.Id AS VehicleId,
           trm.start_Lat,
           trm.start_Long,
           trm.end_Lat,
           trm.end_Long,
           ROW_NUMBER() OVER (PARTITION BY trm.Id ORDER BY trm.id) AS RowNum
    FROM DriverLocation trm
    JOIN Driver d ON d.Id = trm.Driver_Id
    JOIN Vehicle v ON v.VehicleType_Id = trm.VehicleType_id
    JOIN Patient p ON p.Id = trm.PatientId
    WHERE trm.IsPay = 'Y' AND trm.EntryDate BETWEEN DATEADD(DD, -7, GETDATE()) AND GETDATE()
)
SELECT id, PatientName,PaymentDate,PatientRegNo, VehicleNumber, VehicleName, TotalPrice, Distance, DriverName, VehicleId, start_Lat, start_Long, end_Lat, end_Long
FROM RankedResults
WHERE RowNum = 1;";
                var doctorList = ent.Database.SqlQuery<AmbulanceReport>(doctor1).ToList();
                model.Ambulance = doctorList;
            } 
            return View(model);
        }

        public ActionResult Driver( int? id, int? pageNumber, DateTime? RequestDate, string name = null)
        {
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();
            var model = new AmbulanceList();
            if (RequestDate != null)
            {
                DateTime dateCriteria = RequestDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
trm.Amount, trm.Distance, d.DriverName,
trm.PickUp_Place, trm.Drop_Place,
v.Id as VehicleId
from TravelRecordMaster trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1 and d.Id = "+id+" and trm.RequestDate between '" + RequestDate + "' and '" + date + "'";
                var data1 = ent.Database.SqlQuery<AmbulanceReport>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Commission = commision;
                    int total = data1.Count;
                    pageNumber = (int?)pageNumber ?? 1;
                    int pageSize = 10;
                    decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                    model.TotalPages = (int)noOfPages;
                    model.PageNumber = (int)pageNumber;
                    data1 = data1.OrderBy(a => a.VehicleId).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                    if (name != null)
                    {
                        data1 = data1.Where(a => a.VehicleNumber.ToLower().Contains(name)).ToList();
                    }
                    model.Ambulance = data1;
                    return View(model);
                }
            }
            var doctor = @"select p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
trm.Amount, trm.Distance, d.DriverName,
trm.PickUp_Place, trm.Drop_Place,
v.Id as VehicleId
from TravelRecordMaster trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1 and d.Id = " + id + " and trm.RequestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE()";
            var data = ent.Database.SqlQuery<AmbulanceReport>(doctor).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record Of Current Week";
                return View(model);
            }
            else
            {
                ViewBag.Commission = commision;
                int total = data.Count;
                pageNumber = (int?)pageNumber ?? 1;
                int pageSize = 10;
                decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                model.TotalPages = (int)noOfPages;
                model.PageNumber = (int)pageNumber;
                data = data.OrderBy(a => a.VehicleId).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                model.Ambulance = data;
                return View(model);
            }
        }

        public ActionResult DriverTDS(int? id, int? pageNumber, DateTime? RequestDate, string name = null)
        {
            double commision = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Vehicle'").FirstOrDefault();
            var model = new AmbulanceList();
            if (RequestDate != null)
            {
                DateTime dateCriteria = RequestDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
trm.Amount, trm.Distance, d.DriverName,
trm.PickUp_Place, trm.Drop_Place,
v.Id as VehicleId
from TravelRecordMaster trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1 and d.Id = " + id + " and trm.RequestDate between '" + RequestDate + "' and '" + date + "'";
                var data1 = ent.Database.SqlQuery<AmbulanceReport>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Commission = commision;
                    int total = data1.Count;
                    pageNumber = (int?)pageNumber ?? 1;
                    int pageSize = 10;
                    decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                    model.TotalPages = (int)noOfPages;
                    model.PageNumber = (int)pageNumber;
                    data1 = data1.OrderBy(a => a.VehicleId).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                    if (name != null)
                    {
                        data1 = data1.Where(a => a.VehicleNumber.ToLower().Contains(name)).ToList();
                    }
                    model.Ambulance = data1;
                    return View(model);
                }
            }
            var doctor = @"select p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
trm.Amount, trm.Distance, d.DriverName,
trm.PickUp_Place, trm.Drop_Place,
v.Id as VehicleId
from TravelRecordMaster trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1 and d.Id = " + id + " and trm.RequestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE()";
            var data = ent.Database.SqlQuery<AmbulanceReport>(doctor).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record Of Current Week";
                return View(model);
            }
            else
            {
                ViewBag.Commission = commision;
                int total = data.Count;
                pageNumber = (int?)pageNumber ?? 1;
                int pageSize = 10;
                decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                model.TotalPages = (int)noOfPages;
                model.PageNumber = (int)pageNumber;
                data = data.OrderBy(a => a.VehicleId).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                model.Ambulance = data;
                return View(model);
            }
        }

        public ActionResult DriverReport()
        {
            return View();
        }

        private int GetDriverId()
        {
            int loginId = Convert.ToInt32(User.Identity.Name);
            int DriverId = ent.Database.SqlQuery<int>("select Id from Driver where AdminLogin_Id=" + loginId).FirstOrDefault();
            return DriverId;
        }

        public ActionResult Daily(string term)
        {
            int id = GetDriverId();
            var model = new AmbulanceList();
            string q = @"selectselect v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
v.Id as VehicleId
from TravelRecordMaster trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1 and v.Id = "+id+ " and Convert(Date,trm.RequestDate) = (GetDate()) group by v.VehicleNumber, v.VehicleName, v.Id";
            var data = ent.Database.SqlQuery<AmbulanceReport>(q).ToList();
            if(data.Count()  == 0)
            {
                TempData["msg"] = "No Current Record Present";
                return View(model);
            }
            model.Ambulance = data;
            return View(model);
        }

        public class AmbulanceList
        {
            public DateTime? RequestDate { get; set; }
			public DateTime startdate { get; set; }
			public DateTime enddate { get; set; }
			public int TotalPages { get; set; }
            public int VehicleId { get; set; }
            public int PageNumber { get; set; }
            public string VehicleNumber { get; set; }
            public string VehicleName { get; set; }
            public string DriverName { get; set; }

            public IEnumerable<AmbulanceReport> Ambulance { get; set; }
        }

        public class AmbulanceReport
        {
            public int VehicleId { get; set; }
            public int TotalPrice { get; set; }

            public string DriverId { get; set; }
            public string VehicleNumber { get; set; }
            public string VehicleName { get; set; }
            public decimal Amount { get; set; }
			public double Amountwithrazorpaycomm { get; set; }
			public int Distance { get; set; }
            public string PatientName { get; set; }
            public string PatientRegNo { get; set; }
            public string PaymentDate { get; set; }
            public DateTime EntryDate { get; set; }
            public string DriverName { get; set; }  
            public double end_Lat { get; set; }
            public double end_Long { get; set; }
            public double start_Lat { get; set; }
            public double start_Long { get; set; }
            //CODE FOR LAT LONG TO LOCATION 
            public string PickUp_Place
            {
                get { return getlocation(start_Lat.ToString(), start_Long.ToString()); }
            }
            public string Drop_Place
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

    }
}