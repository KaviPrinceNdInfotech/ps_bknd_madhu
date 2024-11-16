using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static HospitalPortal.Controllers.AmbulancePaymentController;

namespace HospitalPortal.Controllers
{
    public class AmbulanceReportController : Controller
    {
        DbEntities ent = new DbEntities();
        // GET: AmbulanceReport
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Ambulance()
        {
            return View();
        }

        public ActionResult Daily(string term, DateTime? date)
        {
            var model = new AmbulancesReport();
            
            var qry = @"select v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
v.Id as VehicleId,SUM(TRM.Amount) as Amount
from DriverLocation trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = d.Vehicle_Id
join Patient p on p.Id = trm.PatientId
where trm.EntryDate > CAST(GETDATE() AS DATE) and trm.IsPay='Y' and trm.RideComplete=1 group by v.VehicleNumber, v.VehicleName,v.Id, 
v.Id";
            var data = ent.Database.SqlQuery<Ambulance>(qry).ToList();
            if (date == null)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Ambulance = data; 
                }
            }
            else
            {
                var qry1 = @"select v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
v.Id as VehicleId,SUM(TRM.Amount) as Amount
from DriverLocation trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = d.Vehicle_Id
join Patient p on p.Id = trm.PatientId
where Convert(Date,trm.EntryDate) = '" + date + "' and trm.IsPay='Y' and trm.RideComplete=1 group by v.VehicleNumber, v.VehicleName,v.Id";
                var data1 = ent.Database.SqlQuery<Ambulance>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    //ViewBag.Payment = payment;
                    model.Ambulance = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }


        public ActionResult DailyRecord(int Id, DateTime? sdate, DateTime? edate)
        {
            var model = new AmbulancesReport(); 
            var qry = @"select trm.Id,p.PatientRegNo as UniqueId,p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,trm.Amount,trm.ToatlDistance as Distance,d.DriverId,d.DriverName,v.Id as VehicleId,trm.start_Lat,trm.start_Long,trm.end_Lat,trm.end_Long from DriverLocation trm 
join Driver d on d.Id = trm.Driver_Id 
join Vehicle v on v.Id = d.Vehicle_Id 
join Patient p on p.Id = trm.PatientId  
where trm.EntryDate > CAST(GETDATE() AS DATE) and trm.IsPay='Y' and trm.RideComplete=1 and v.Id=" + Id;
            var data = ent.Database.SqlQuery<Ambulance>(qry).ToList();
            if (sdate == null && edate == null)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Ambulance = data; 
                }
            }
            else
            {
                var qry1 = @"select trm.Id,p.PatientRegNo as UniqueId,p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,trm.Amount,trm.ToatlDistance as Distance, d.DriverName,v.Id as VehicleId,trm.start_Lat,trm.start_Long,trm.end_Lat,trm.end_Long from DriverLocation trm 
join Driver d on d.Id = trm.Driver_Id 
join Vehicle v on v.Id = d.Vehicle_Id 
join Patient p on p.Id = trm.PatientId 
where v.Id = " + Id + " and Convert(Date,trm.EntryDate)  between '" + sdate + "' and '" + edate + "' and IsPay='Y' and trm.RideComplete=1 group by v.VehicleNumber, v.VehicleName, v.Id";
                var data1 = ent.Database.SqlQuery<Ambulance>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    //ViewBag.Payment = payment;
                    model.Ambulance = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }
        public ActionResult Weekly(string term, DateTime? date)
        {
            var model = new AmbulancesReport(); 
            
            if (date != null)
            {
				DateTime dateCriteria = date.Value.AddDays(-7);
				string Tarikh = dateCriteria.ToString("dd/MM/yyyy");
				var qry1 = @"sselect distinct v.Id as VehicleId,v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,SUM(trm.Amount) as Amount
from DriverLocation trm 
JOIN Driver d ON d.Id = trm.Driver_Id
JOIN Vehicle v ON v.Id = d.Vehicle_Id
JOIN Patient p on p.Id = trm.PatientId
where trm.EntryDate between '" + dateCriteria + "' and '" + date + "' and trm.IsPay='Y' and trm.RideComplete=1 group by v.VehicleNumber, v.VehicleName,v.Id";
				var data1 = ent.Database.SqlQuery<Ambulance>(qry1).ToList();
				if (data1.Count() == 0)
				{
					TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
				}
				else
				{
					model.Ambulance = data1;
					
				}

				return View(model);
			}
            else
            {
				var qry = @"select distinct v.Id as VehicleId,v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,SUM(trm.Amount) as Amount
from DriverLocation trm 
JOIN Driver d ON d.Id = trm.Driver_Id
JOIN Vehicle v ON v.Id = d.Vehicle_Id
JOIN Patient p on p.Id = trm.PatientId
where trm.EntryDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() and trm.IsPay='Y' and trm.RideComplete=1 group by v.VehicleNumber, v.VehicleName, 
v.Id";
				var data = ent.Database.SqlQuery<Ambulance>(qry).ToList();
				if (data.Count() == 0)
				{
					TempData["msg"] = "No Record of Current Week";
				}
				else
				{
					model.Ambulance = data;
				}
			}
            return View(model);
        }

        public ActionResult WeeklyReport(int Id, DateTime? date)
        {
            var model = new AmbulancesReport();
             
            var qry = @"select trm.Id,p.PatientRegNo as UniqueId,p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,trm.Amount,trm.ToatlDistance as Distance,d.DriverId,d.DriverName,v.Id as VehicleId,trm.start_Lat,trm.start_Long,trm.end_Lat,trm.end_Long from DriverLocation trm 
join Driver d on d.Id = trm.Driver_Id 
join Vehicle v on v.Id = d.Vehicle_Id 
join Patient p on p.Id = trm.PatientId
where trm.EntryDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() and trm.IsPay='Y' and trm.RideComplete=1 and v.Id=" + Id;
            var data = ent.Database.SqlQuery<Ambulance>(qry).ToList();
            if (date == null)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Week";
                }
                else
                {
                    model.Ambulance = data;
                }
            }
            else
            {
                DateTime dateCriteria = date.Value.AddDays(-7);
                string Tarikh = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select trm.Id,p.PatientRegNo as UniqueId,p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,trm.Amount,trm.ToatlDistance as Distance,d.DriverId,d.DriverName,v.Id as VehicleId,trm.start_Lat,trm.start_Long,trm.end_Lat,trm.end_Long from DriverLocation trm 
join Driver d on d.Id = trm.Driver_Id 
join Vehicle v on v.Id = d.Vehicle_Id 
join Patient p on p.Id = trm.PatientId
where v.Id = " + Id + " and trm.EntryDate between '" + dateCriteria + "' and '" + date + "' and trm.IsPay='Y' and trm.RideComplete=1 group by v.VehicleNumber, v.VehicleName, v.Id";
                var data1 = ent.Database.SqlQuery<Ambulance>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    model.Ambulance = data;
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult Monthly(string term, DateTime? startdate, DateTime? enddate)
        {
            var model = new AmbulancesReport();
            //double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            
            if (startdate != null && enddate != null)
            {
				var qry1 = @"select distinct v.Id as VehicleId,v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,SUM(trm.Amount) as Amount
from DriverLocation trm 
JOIN Driver d ON d.Id = trm.Driver_Id
JOIN Vehicle v ON v.Id = d.Vehicle_Id
JOIN Patient p on p.Id = trm.PatientId
where Convert(Date,trm.EntryDate)  between Convert(datetime,'" + startdate + "',103) and Convert(datetime,'" + enddate + "',103) and trm.IsPay='Y' and trm.RideComplete=1 group by v.VehicleNumber, v.VehicleName, v.Id";
				var data1 = ent.Database.SqlQuery<Ambulance>(qry1).ToList();
				if (data1.Count() == 0)
				{
					TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
				}
				else
				{
					//ViewBag.Payment = payment;
					model.Ambulance = data1;
					//ViewBag.Total = model.LabList.Sum(a => a.Amount);
					return View(model);
				}				
            }
            else
            {
				var qry = @"select distinct v.Id as VehicleId,v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,SUM(trm.Amount) as Amount
from DriverLocation trm 
JOIN Driver d ON d.Id = trm.Driver_Id
JOIN Vehicle v ON v.Id = d.Vehicle_Id
JOIN Patient p on p.Id = trm.PatientId
where Month(trm.EntryDate) = Month(GetDate()) and trm.IsPay='Y' and trm.RideComplete=1 group by v.VehicleNumber, v.VehicleName,v.Id";
				var data = ent.Database.SqlQuery<Ambulance>(qry).ToList();

				if (data.Count() == 0)
				{
					TempData["msg"] = "No Record of Current Month";
				}
				else
				{
					model.Ambulance = data; 
				}
			}
            return View(model);
        }

        public ActionResult MonthlyRecord(int Id, DateTime? startdate, DateTime? enddate)
        {
            var model = new AmbulancesReport();
            //double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select trm.Id,p.PatientRegNo as UniqueId,p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,trm.Amount,trm.ToatlDistance as Distance,d.DriverId,d.DriverName,v.Id as VehicleId,trm.start_Lat,trm.start_Long,trm.end_Lat,trm.end_Long from DriverLocation trm 
join Driver d on d.Id = trm.Driver_Id 
join Vehicle v on v.Id = d.Vehicle_Id 
join Patient p on p.Id = trm.PatientId
where Month(trm.EntryDate) = Month(GetDate()) and trm.IsPay='Y' and trm.RideComplete=1 and v.Id=" + Id;
            var data = ent.Database.SqlQuery<Ambulance>(qry).ToList();
            if (startdate == null && enddate == null)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Ambulance = data;
                    //ViewBag.Payment = payment;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                }
            }
            else
            {
                var qry1 = @"select trm.Id,p.PatientRegNo as UniqueId,p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,trm.Amount,trm.ToatlDistance as Distance,d.DriverId,d.DriverName,v.Id as VehicleId,trm.start_Lat,trm.start_Long,trm.end_Lat,trm.end_Long from DriverLocation trm 
join Driver d on d.Id = trm.Driver_Id 
join Vehicle v on v.Id = d.Vehicle_Id 
join Patient p on p.Id = trm.PatientId
where v.Id = " + Id + " and Convert(Date,trm.EntryDate)  between '" + startdate + "' and '" + enddate + "' and IsPay='Y' and trm.RideComplete=1 group by v.VehicleNumber, v.VehicleName, v.Id";
                var data1 = ent.Database.SqlQuery<Ambulance>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                { 
                    model.Ambulance = data1; 
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult Yearly(string term, int? year)
        {
            var model = new AmbulancesReport(); 
            
            if (year != null)
            {
				var qry1 = @"select distinct v.Id as VehicleId,v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,SUM(trm.Amount) as Amount
from DriverLocation trm 
JOIN Driver d ON d.Id = trm.Driver_Id
JOIN Vehicle v ON v.Id = d.Vehicle_Id
JOIN Patient p on p.Id = trm.PatientId
where Year(trm.EntryDate) = '" + year + "' and trm.IsPay='Y' and trm.RideComplete=1 group by v.VehicleNumber, v.VehicleName,v.Id";
				var data1 = ent.Database.SqlQuery<Ambulance>(qry1).ToList();
				if (data1.Count() == 0)
				{
					TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
				}
				else
				{ 
					model.Ambulance = data1; 
				}
				return View(model);
			}
            else
            {
				var qry = @"select distinct v.Id as VehicleId,v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,SUM(trm.Amount) as Amount
from DriverLocation trm 
JOIN Driver d ON d.Id = trm.Driver_Id
JOIN Vehicle v ON v.Id = d.Vehicle_Id
JOIN Patient p on p.Id = trm.PatientId
where Year(trm.EntryDate) = Year(GetDate()) and trm.IsPay='Y' and trm.RideComplete=1 group by v.VehicleNumber, v.VehicleName,v.Id";
				var data = ent.Database.SqlQuery<Ambulance>(qry).ToList();
				if (data.Count() == 0)
				{
					TempData["msg"] = "No Record of Current Date";
				}
				else
				{
					model.Ambulance = data;
				}
			}
            return View(model);
        }

        public ActionResult YearlyRecord(int Id, int? year)
        {
            var model = new AmbulancesReport();
             
            if (year != null)
            {
				var qry1 = @"select trm.Id,p.PatientRegNo as UniqueId,p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,trm.Amount,trm.ToatlDistance as Distance,d.DriverId,d.DriverName,v.Id as VehicleId,trm.start_Lat,trm.start_Long,trm.end_Lat,trm.end_Long from DriverLocation trm 
join Driver d on d.Id = trm.Driver_Id 
join Vehicle v on v.Id = d.Vehicle_Id 
join Patient p on p.Id = trm.PatientId
where Year(trm.EntryDate) = '" + year + "' and trm.IsPay='Y' and trm.RideComplete=1 and v.Id=" + Id;
				var data1 = ent.Database.SqlQuery<Ambulance>(qry1).ToList();
				if (data1.Count() == 0)
				{
					TempData["msg"] = "Your Selected year Doesn't Contain any Information.";
				}
				else
				{ 
					model.Ambulance = data1; 
					return View(model);
				}
				
            }
            else
            {
				var qry = @"select trm.Id,p.PatientRegNo as UniqueId,p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,trm.Amount,trm.ToatlDistance as Distance,d.DriverId,d.DriverName,v.Id as VehicleId,trm.start_Lat,trm.start_Long,trm.end_Lat,trm.end_Long from DriverLocation trm 
join Driver d on d.Id = trm.Driver_Id 
join Vehicle v on v.Id = d.Vehicle_Id 
join Patient p on p.Id = trm.PatientId
where Year(trm.EntryDate) = Year(GetDate()) and trm.IsPay='Y' and trm.RideComplete=1 and v.Id=" + Id;
				var data = ent.Database.SqlQuery<Ambulance>(qry).ToList();
				if (data.Count() == 0)
				{
					TempData["msg"] = "No Record";
				}
				else
				{
					model.Ambulance = data; 
				}
			}
            return View(model);
        }

        public ActionResult RoadAccidentReport()
        {
            var model = new AmbulancesReport();

            
                var qry = @"select trm.Id,p.PatientRegNo as UniqueId,p.PatientName,v.VehicleName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,trm.Amount,trm.ToatlDistance as Distance,d.DriverId,d.DriverName,v.Id as VehicleId,trm.start_Lat,trm.start_Long,trm.end_Lat,trm.end_Long from DriverLocation trm 
join Driver d on d.Id = trm.Driver_Id 
join Vehicle v on v.Id = d.Vehicle_Id 
join Patient p on p.Id = trm.PatientId
where MONTH(trm.EntryDate) = MONTH(GETDATE()) AND trm.RideComplete = 1 AND trm.AmbulanceType_id = 2 order by trm.Id desc";
                var data = ent.Database.SqlQuery<Ambulance>(qry).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record";
                }
                else
                {
                    model.Ambulance = data;
                } 
             
             return View(model);
        }
    }
}