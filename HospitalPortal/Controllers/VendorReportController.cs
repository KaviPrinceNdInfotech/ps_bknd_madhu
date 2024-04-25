using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VendorReportController : Controller
    {

       DbEntities ent = new DbEntities();
        // GET: VendorReport
        public ActionResult VendorReport()
        {
            return View();
        }

        private int GetVendorId()
        {
            int loginId = Convert.ToInt32(User.Identity.Name);
            int VendorId = ent.Database.SqlQuery<int>("select Id from Vendor where AdminLogin_Id=" + loginId).FirstOrDefault();
            return VendorId;
        }

        public ActionResult Doctor()
        {
            return View();
        }

        public ActionResult Driver()
        {
            return View();
        }

        public ActionResult Vehicle()
        {
            return View();
        }

        public ActionResult Chemist()
        {
            return View();
        }

        public ActionResult Nurse()
        {
            return View();
        }

        public ActionResult Lab()
        {
            return View();
        }

        public ActionResult HealthCheckUp()
        {
            return View();
        }

        public ActionResult DailyDoc(string term, DateTime? date)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(P.TotalFee) as Amount,D.DoctorId as UniqueId,D.DoctorName as UserName,ve.VendorName as Name,ve.UniqueId as VendorId,ve.CompanyName as Name1 from PatientAppointment P 
join Doctor D on D.Id = p.Doctor_Id 
join Vendor ve on ve.Id = d.Vendor_Id
where Convert(Date, P.AppointmentDate) = Convert(Date,GETDATE()) and P.IsPaid=1 GROUP BY D.DoctorId,D.DoctorName,ve.VendorName,ve.CompanyName,ve.UniqueId";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
             
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                }
             
              return View(model);
        }
         
        public ActionResult WeeklyDoc(string term, DateTime? date)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
           
            if (date != null)
            {
				DateTime dateCriteria = date.Value.AddDays(-7);
				string Tarikh = dateCriteria.ToString("dd/MM/yyyy");
				var qry1 = @"select Sum(P.TotalFee) as Amount,D.DoctorId as UniqueId,D.DoctorName as UserName,ve.VendorName as Name,ve.UniqueId as VendorId,ve.CompanyName as Name1 from PatientAppointment P 
join Doctor D on D.Id = p.Doctor_Id 
join Vendor ve on ve.Id = d.Vendor_Id
where p.AppointmentDate between '" + dateCriteria + "' and '" + Tarikh + "' and P.IsPaid=1 GROUP BY D.DoctorId,D.DoctorName,ve.VendorName,ve.CompanyName,ve.UniqueId";
				var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
				if (data1.Count() == 0)
				{
					TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
				}
				else
				{
					ViewBag.Payment = payment;
					model.Vendors = data1; 
					
				}
				return View(model);
			}
            else {
				var qry = @"select Sum(P.TotalFee) as Amount,D.DoctorId as UniqueId,D.DoctorName as UserName,ve.VendorName as Name,ve.UniqueId as VendorId,ve.CompanyName as Name1 from PatientAppointment P 
join Doctor D on D.Id = p.Doctor_Id 
join Vendor ve on ve.Id = d.Vendor_Id
where p.AppointmentDate  between DATEADD(day,-7,GETDATE()) and GetDate() and P.IsPaid=1 GROUP BY D.DoctorId,D.DoctorName,ve.VendorName,ve.CompanyName,ve.UniqueId";
				var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
				

				if (data.Count() == 0)
				{
					TempData["msg"] = "No Record of Current Week";
				}
				else
				{
					model.Vendors = data;
					ViewBag.Payment = payment;
					//ViewBag.Total = model.LabList.Sum(a => a.Amount);
				}
				return View(model);
			}
			
        }

        public ActionResult MonthlyDoc(string term, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            
            if (sdate == null && edate == null)
            {
                var qry = @"select Sum(P.TotalFee) as Amount,D.DoctorId as UniqueId,D.DoctorName as UserName,ve.VendorName as Name,ve.UniqueId as VendorId,ve.CompanyName as Name1 from PatientAppointment P 
join Doctor D on D.Id = p.Doctor_Id 
join Vendor ve on ve.Id = d.Vendor_Id
where Month(p.AppointmentDate) = Month(GetDate()) and P.IsPaid=1 GROUP BY D.DoctorId,D.DoctorName,ve.VendorName,ve.CompanyName,ve.UniqueId";
                var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Month";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment; 
                }
                return View(model);
            }
            else { 
                var qry1 = @"select Sum(P.TotalFee) as Amount,D.DoctorId as UniqueId,D.DoctorName as UserName,ve.VendorName as Name,ve.UniqueId as VendorId,ve.CompanyName as Name1 from PatientAppointment P 
join Doctor D on D.Id = p.Doctor_Id 
join Vendor ve on ve.Id = d.Vendor_Id
where p.AppointmentDate between Convert(datetime,'" + sdate + "',103) and Convert(datetime,'" + edate + "',103) and P.IsPaid=1 GROUP BY D.DoctorId,D.DoctorName,ve.VendorName,ve.CompanyName,ve.UniqueId";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1; 
                    
                }
                return View(model);
            }
             
        }

        public ActionResult YearlyDoc(string term, int? year)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            
            if (!year.HasValue)
            {
                var qry = @"select Sum(P.TotalFee) as Amount,D.DoctorId as UniqueId,D.DoctorName as UserName,ve.VendorName as Name,ve.UniqueId as VendorId,ve.CompanyName as Name1 from PatientAppointment P 
join Doctor D on D.Id = p.Doctor_Id 
join Vendor ve on ve.Id = d.Vendor_Id
where Year(p.AppointmentDate) = Year(GetDate()) and P.IsPaid=1 GROUP BY D.DoctorId,D.DoctorName,ve.VendorName,ve.CompanyName,ve.UniqueId";
                var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Year";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                }
                return View(model);
            }
            else {
                var qry1 = @"select Sum(P.TotalFee) as Amount,D.DoctorId as UniqueId,D.DoctorName as UserName,ve.VendorName as Name,ve.UniqueId as VendorId,ve.CompanyName as Name1 from PatientAppointment P 
join Doctor D on D.Id = p.Doctor_Id 
join Vendor ve on ve.Id = d.Vendor_Id
where Year(p.AppointmentDate) = '" + year + "' and P.IsPaid=1 GROUP BY D.DoctorId,D.DoctorName,ve.VendorName,ve.CompanyName,ve.UniqueId";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Entered Year Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1; 
                   
                }
                return View(model);
            }
             
        }
        
        //Driver Section

        public ActionResult DailyDri(string term, DateTime? date)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select COUNT(d.Id) as Counts, v.VendorName,v.CompanyName from Driver d join Vendor v on d.Vendor_Id = v.Id where Convert(Date,d.JoiningDate) = Convert(Date,GETDATE()) group by v.VendorName,v.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (date != null)
            {
                string dt = date.Value.ToString("MM/dd/yyyy");
                var qry1 = @"select COUNT(d.Id) as Counts, v.VendorName,v.CompanyName from Driver d join Vendor v on d.Vendor_Id = v.Id where Convert(Date,d.JoiningDate) = '" + dt + "' group by v.VendorName,v.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult WeeklyDri(string term, DateTime? date)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select COUNT(d.Id) as Counts, v.VendorName,v.CompanyName from Driver d join Vendor v on d.Vendor_Id = v.Id where Convert(Date,d.RegistrationDate) between DATEADD(day,-7,GETDATE()) and GetDate() group by v.VendorName,v.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (date != null)
            {
                DateTime dateCriteria = date.Value.AddDays(-7);
                string Tarikh = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select COUNT(d.Id) as Counts, v.VendorName,v.CompanyName from Driver d join Vendor v on d.Vendor_Id = v.Id where Convert(Date,d.JoiningDate) between '" + dateCriteria + "' and '" + Tarikh + "' group by v.VendorName,v.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult MonthlyDri(string term, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select COUNT(d.Id) as Counts, v.VendorName,v.CompanyName from Driver d join Vendor v on d.Vendor_Id = v.Id where Month(d.JoiningDate) = Month(GetDate()) group by v.VendorName,v.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (sdate != null && edate != null)
            {
                var qry1 = @"select COUNT(d.Id) as Counts, v.VendorName,v.CompanyName from Driver d join Vendor v on d.Vendor_Id = v.Id where Convert(DATE,d.JoiningDate) between '" + sdate + "' and '" + edate + "' group by v.VendorName,v.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult YearlyDri(string term, int? year)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select COUNT(d.Id) as Counts, v.VendorName,v.CompanyName from Driver d join Vendor v on d.Vendor_Id = v.Id where Year(d.JoiningDate) = Year(GetDate()) group by v.VendorName,v.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (year != null)
            {
                var qry1 = @"select COUNT(d.Id) as Counts, v.VendorName,v.CompanyName from Driver d join Vendor v on d.Vendor_Id = v.Id where Year(d.JoiningDate) = '" + year + "' group by v.VendorName,v.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        //Vehicle Section

        public ActionResult DailyVeh(string term, DateTime? date)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select distinct v.Id as VehicleId,v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,ve.VendorName as Name,ve.UniqueId,ve.CompanyName as Name1,SUM(trm.Amount) as Amount
from DriverLocation trm 
JOIN Driver d ON d.Id = trm.Driver_Id
JOIN Vehicle v ON v.Id = d.Vehicle_Id
join Vendor ve on ve.Id = v.Vendor_Id
where trm.EntryDate > CAST(GETDATE() AS DATE) and trm.IsPay='Y' group by v.VehicleNumber, v.VehicleName,v.Id,ve.VendorName,ve.CompanyName,ve.UniqueId";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment; 
            }
          
            return View(model);
        }

        public ActionResult WeeklyVeh(string term, DateTime? week)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            
            if (week != null)
            {
                DateTime dateCriteria = week.Value.AddDays(-7);
                string Tarikh = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select distinct v.Id as VehicleId,v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,ve.VendorName as Name,ve.UniqueId,ve.CompanyName as Name1,SUM(trm.Amount) as Amount
from DriverLocation trm 
JOIN Driver d ON d.Id = trm.Driver_Id
JOIN Vehicle v ON v.Id = d.Vehicle_Id
join Vendor ve on ve.Id = v.Vendor_Id
where trm.EntryDate between convert(datetime,'" + Tarikh+ "',103) and convert(datetime,'" + week + "',103) and trm.IsPay='Y' group by v.VehicleNumber, v.VehicleName,v.Id,ve.VendorName,ve.CompanyName,ve.UniqueId";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1; 
                }
                return View(model);
            }
            else
            {
                var qry = @"select distinct v.Id as VehicleId,v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,ve.VendorName as Name,ve.UniqueId,ve.CompanyName as Name1,SUM(trm.Amount) as Amount
from DriverLocation trm 
JOIN Driver d ON d.Id = trm.Driver_Id
JOIN Vehicle v ON v.Id = d.Vehicle_Id
join Vendor ve on ve.Id = v.Vendor_Id
where trm.EntryDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() and trm.IsPay='Y' group by v.VehicleNumber, v.VehicleName,v.Id,ve.VendorName,ve.CompanyName,ve.UniqueId";
                var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Week";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                }
                return View(model);
            }
          
        }

        public ActionResult MonthlyVeh(string term, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
           
            if (sdate != null && edate != null)
            {
                var qry1 = @"select distinct v.Id as VehicleId,v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,ve.VendorName as Name,ve.UniqueId,ve.CompanyName as Name1,SUM(trm.Amount) as Amount
FROM DriverLocation trm 
JOIN Driver d ON d.Id = trm.Driver_Id
JOIN Vehicle v ON v.Id = d.Vehicle_Id
join Vendor ve on ve.Id = v.Vendor_Id
WHERE trm.EntryDate BETWEEN CONVERT(DATETIME,'" + sdate+ "',103) and CONVERT(DATETIME,'" + edate+ "',103) and trm.IsPay='Y' group by v.VehicleNumber, v.VehicleName,v.Id,ve.VendorName,ve.CompanyName,ve.UniqueId";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1; 
                }
                return View(model);
            }
            else
            {
                var qry = @"select distinct v.Id as VehicleId,v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,ve.VendorName as Name,ve.UniqueId,ve.CompanyName as Name1,SUM(trm.Amount) as Amount
from DriverLocation trm 
JOIN Driver d ON d.Id = trm.Driver_Id
JOIN Vehicle v ON v.Id = d.Vehicle_Id
join Vendor ve on ve.Id = v.Vendor_Id
where Month(trm.EntryDate) = Month(GetDate()) and trm.IsPay='Y' group by v.VehicleNumber, v.VehicleName,v.Id,ve.VendorName,ve.CompanyName,ve.UniqueId";
                var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Month";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                }
                return View(model);
            }
            
        }

        public ActionResult YearlyVeh(string term, int? year)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            
            if (year != null)
            {
                var qry1 = @"select distinct v.Id as VehicleId,v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,ve.VendorName as Name,ve.UniqueId,ve.CompanyName as Name1,SUM(trm.Amount) as Amount
from DriverLocation trm 
JOIN Driver d ON d.Id = trm.Driver_Id
JOIN Vehicle v ON v.Id = d.Vehicle_Id
join Vendor ve on ve.Id = v.Vendor_Id
where Year(trm.EntryDate) = '" + year + "' and trm.IsPay='Y' group by v.VehicleNumber, v.VehicleName,v.Id,ve.VendorName,ve.CompanyName,ve.UniqueId";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Year Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1; 
                   
                }
                return View(model);
            }
            else
            {
                var qry = @"select distinct v.Id as VehicleId,v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,ve.VendorName as Name,ve.UniqueId,ve.CompanyName as Name1,SUM(trm.Amount) as Amount
from DriverLocation trm 
JOIN Driver d ON d.Id = trm.Driver_Id
JOIN Vehicle v ON v.Id = d.Vehicle_Id
join Vendor ve on ve.Id = v.Vendor_Id
where Year(trm.EntryDate) = Year(GetDate()) and trm.IsPay='Y' group by v.VehicleNumber, v.VehicleName,v.Id,ve.VendorName,ve.CompanyName,ve.UniqueId";
                var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Year";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                }
                return View(model);
            }
            
        }


        //Chemist

        public ActionResult DailyChe(string term, DateTime? date)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select SUM(md.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1 from Chemist d join Vendor v on d.Vendor_Id = v.Id 
join MedicineOrder mo on mo.Chemist_Id = d.Id
join MedicineOrderDetail md on md.Order_Id = mo.Id
where Convert(Date,mo.OrderDate) = Convert(Date,GETDATE()) and mo.IsPaid=1
group by v.VendorName,v.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
            }
            if (date != null)
            {
                string dt = date.Value.ToString("MM/dd/yyyy");
                var qry1 = @"select SUM(md.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1 from Chemist d join Vendor v on d.Vendor_Id = v.Id 
join MedicineOrder mo on mo.Chemist_Id = d.Id
join MedicineOrderDetail md on md.Order_Id = mo.Id
where Convert(Date,mo.OrderDate) = '" + dt + "' and mo.IsPaid=1 group by v.VendorName,v.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult WeeklyChe(string term, DateTime? date)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select SUM(md.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1 from Chemist d join Vendor v on d.Vendor_Id = v.Id 
join MedicineOrder mo on mo.Chemist_Id = d.Id
join MedicineOrderDetail md on md.Order_Id = mo.Id
where Convert(Date,mo.OrderDate) between DATEADD(day,-7,GETDATE()) and GetDate() and mo.IsPaid=1
group by v.VendorName,v.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (date != null)
            {
                DateTime dateCriteria = date.Value.AddDays(-7);
                string Tarikh = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select SUM(md.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1 from Chemist d join Vendor v on d.Vendor_Id = v.Id 
join MedicineOrder mo on mo.Chemist_Id = d.Id
join MedicineOrderDetail md on md.Order_Id = mo.Id
where Convert(Date,mo.OrderDate) between '" + dateCriteria + "' and '" + Tarikh + "' and mo.IsPaid=1 group by v.VendorName,v.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult MonthlyChe(string term, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select SUM(md.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1 from Chemist d join Vendor v on d.Vendor_Id = v.Id 
join MedicineOrder mo on mo.Chemist_Id = d.Id
join MedicineOrderDetail md on md.Order_Id = mo.Id
where Month(mo.OrderDate) = Month(GetDate()) and mo.IsPaid=1
group by v.VendorName,v.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (sdate != null && edate != null)
            {
                var qry1 = @"select SUM(md.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1 from Chemist d join Vendor v on d.Vendor_Id = v.Id 
join MedicineOrder mo on mo.Chemist_Id = d.Id
join MedicineOrderDetail md on md.Order_Id = mo.Id
where Convert(Date,mo.OrderDate) between '" + sdate + "' and '" + edate + "' and mo.IsPaid=1 group by v.VendorName,v.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult YearlyChe(string term, int? year)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select SUM(md.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1 from Chemist d join Vendor v on d.Vendor_Id = v.Id 
join MedicineOrder mo on mo.Chemist_Id = d.Id
join MedicineOrderDetail md on md.Order_Id = mo.Id
where Year(mo.OrderDate) = Year(GetDate()) and mo.IsPaid=1
group by v.VendorName,v.CompanyName";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (year != null)
            {
                var qry1 = @"select SUM(md.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1 from Chemist d join Vendor v on d.Vendor_Id = v.Id 
join MedicineOrder mo on mo.Chemist_Id = d.Id
join MedicineOrderDetail md on md.Order_Id = mo.Id
where Year(mo.OrderDate) = '" + year + "' and mo.IsPaid=1 group by v.VendorName,v.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        //Nurse
        public ActionResult DailyNur(string term, DateTime? date)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select P.ServiceAcceptanceDate, SUM(P.TotalFee ) as Counts,n.NurseId as UniqueId,n.NurseName as UserName ,ve.VendorName as Name,ve.UniqueId as VendorId,ve.CompanyName as Name1 from NurseService P
join Nurse n on n.Id = p.Nurse_Id 
join Vendor ve on ve.Id = n.Vendor_Id
where CONVERT(DATE, P.ServiceAcceptanceDate) >= CAST(GETDATE() AS DATE)
and P.IsPaid=1 GROUP BY P.ServiceAcceptanceDate,n.NurseId,n.NurseName,ve.VendorName,ve.UniqueId,ve.CompanyName ";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Vendors = data;
                ViewBag.Payment = payment;
            }
             
            return View(model);
        }

        public ActionResult WeeklyNur(string term, DateTime? week)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
           
            if (week != null)
            {
                DateTime dateCriteria = week.Value.AddDays(-7);
                string Tarikh = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select P.ServiceAcceptanceDate, SUM(P.TotalFee ) as Counts,n.NurseId as UniqueId,n.NurseName as UserName ,ve.VendorName as Name,ve.UniqueId as VendorId,ve.CompanyName as Name1 from NurseService P
join Nurse n on n.Id = p.Nurse_Id 
join Vendor ve on ve.Id = n.Vendor_Id
where p.ServiceAcceptanceDate between Convert(Datetime,'" + dateCriteria + "',103) and Convert(Datetime,'" + Tarikh + "',103) and P.IsPaid=1 GROUP BY P.ServiceAcceptanceDate,n.NurseId,n.NurseName,ve.VendorName,ve.UniqueId,ve.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1; 
                }
                return View(model);
            }
            else
            {
                var qry = @"select P.ServiceAcceptanceDate, SUM(P.TotalFee ) as Counts,n.NurseId as UniqueId,n.NurseName as UserName ,ve.VendorName as Name,ve.UniqueId as VendorId,ve.CompanyName as Name1 from NurseService P
join Nurse n on n.Id = p.Nurse_Id 
join Vendor ve on ve.Id = n.Vendor_Id
where Convert(Date,p.ServiceAcceptanceDate)  between DATEADD(day,-7,GETDATE()) and GetDate()
and P.IsPaid=1 GROUP BY P.ServiceAcceptanceDate,n.NurseId,n.NurseName,ve.VendorName,ve.UniqueId,ve.CompanyName";
                var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Week";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment; 
                }
                return View(model);
            }
            
        }

        public ActionResult MonthlyNur(string term, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            
            if (sdate != null && edate != null)
            {
                var qry1 = @"select P.ServiceAcceptanceDate, SUM(P.TotalFee ) as Counts,n.NurseId as UniqueId,n.NurseName as UserName ,ve.VendorName as Name,ve.UniqueId as VendorId,ve.CompanyName as Name1 from NurseService P
join Nurse n on n.Id = p.Nurse_Id 
join Vendor ve on ve.Id = n.Vendor_Id
where p.ServiceAcceptanceDate between Convert(Datetime,'" + sdate + "',103) and Convert(Datetime,'" + edate + "',103) and P.IsPaid=1 GROUP BY P.ServiceAcceptanceDate,n.NurseId,n.NurseName,ve.VendorName,ve.UniqueId,ve.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1; 
                }
                return View(model);
            }
            else
            {
                var qry = @"select P.ServiceAcceptanceDate, SUM(P.TotalFee ) as Counts,n.NurseId as UniqueId,n.NurseName as UserName ,ve.VendorName as Name,ve.UniqueId as VendorId,ve.CompanyName as Name1 from NurseService P
join Nurse n on n.Id = p.Nurse_Id 
join Vendor ve on ve.Id = n.Vendor_Id
where Month(p.ServiceAcceptanceDate) = Month(GetDate())
and P.IsPaid=1 GROUP BY P.ServiceAcceptanceDate,n.NurseId,n.NurseName,ve.VendorName,ve.UniqueId,ve.CompanyName";
                var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Month";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                }
                return View(model);
            }
            

        }

        public ActionResult YearlyNur(string term, int? year)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            
            if (year != null)
            {
                var qry1 = @"select P.ServiceAcceptanceDate, SUM(P.TotalFee ) as Counts,n.NurseId as UniqueId,n.NurseName as UserName ,ve.VendorName as Name,ve.UniqueId as VendorId,ve.CompanyName as Name1 from NurseService P
join Nurse n on n.Id = p.Nurse_Id 
join Vendor ve on ve.Id = n.Vendor_Id
where Year(p.ServiceAcceptanceDate) = = '" + year + "' and P.IsPaid=1 GROUP BY P.ServiceAcceptanceDate,n.NurseId,n.NurseName,ve.VendorName,ve.UniqueId,ve.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1; 
                }
                return View(model);
            }
            else
            {
                var qry = @"select P.ServiceAcceptanceDate, SUM(P.TotalFee ) as Counts,n.NurseId as UniqueId,n.NurseName as UserName ,ve.VendorName as Name,ve.UniqueId as VendorId,ve.CompanyName as Name1 from NurseService P
join Nurse n on n.Id = p.Nurse_Id 
join Vendor ve on ve.Id = n.Vendor_Id
where Year(p.ServiceAcceptanceDate) = Year(GetDate())
and P.IsPaid=1 GROUP BY P.ServiceAcceptanceDate,n.NurseId,n.NurseName,ve.VendorName,ve.UniqueId,ve.CompanyName";
                var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Year";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                }
                return View(model);
            }
            
        }


        //Lab
        public ActionResult DailyLab(string term, DateTime? date)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(bt.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1, V.Id from Lab d join Vendor v on d.Vendor_Id = v.Id  
join BookTestLab bt on bt.Lab_Id = d.Id
join LabBooking lb on lb.PatientId = bt.Patient_Id
where  Convert(Date,bt.TestDate) = GETDATE() and lb.IsPaid =1 group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (date == null)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                }
            }
            else { 
                string dt = date.Value.ToString("MM/dd/yyyy");
                var qry1 = @"select Sum(bt.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1, V.Id from Lab d join Vendor v on d.Vendor_Id = v.Id  
join BookTestLab bt on bt.Lab_Id = d.Id
join LabBooking lb on lb.PatientId = bt.Patient_Id
where  Convert(Date,bt.TestDate) = '" + dt + "' and lb.IsPaid =1 group by v.VendorName,v.CompanyName, V.Id";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult WeeklyLab(string term, DateTime? date)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(bt.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1, V.Id from Lab d join Vendor v on d.Vendor_Id = v.Id  
join BookTestLab bt on bt.Lab_Id = d.Id
join LabBooking lb on lb.PatientId = bt.Patient_Id
where  Convert(Date,bt.TestDate) between DATEADD(day,-7,GETDATE()) and GetDate() and lb.IsPaid =1 group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (date == null)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                }
            }
            else { 
            
                DateTime dateCriteria = date.Value.AddDays(-7);
                string Tarikh = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select Sum(bt.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1, V.Id from Lab d join Vendor v on d.Vendor_Id = v.Id  
join BookTestLab bt on bt.Lab_Id = d.Id
join LabBooking lb on lb.PatientId = bt.Patient_Id
where  Convert(Date,bt.TestDate) between '" + dateCriteria + "' and '" + Tarikh + "' and lb.IsPaid =1 group by v.VendorName,v.CompanyName, V.Id";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult MonthlyLab(string term, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(bt.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1, V.Id from Lab d join Vendor v on d.Vendor_Id = v.Id  
join BookTestLab bt on bt.Lab_Id = d.Id
join LabBooking lb on lb.PatientId = bt.Patient_Id
where Month(bt.TestDate) = Month(GetDate()) and lb.IsPaid =1 group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (sdate == null && edate == null)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                }
            }
            else { 
           
                var qry1 = @"select Sum(bt.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1, V.Id from Lab d join Vendor v on d.Vendor_Id = v.Id  
join BookTestLab bt on bt.Lab_Id = d.Id
join LabBooking lb on lb.PatientId = bt.Patient_Id
where Convert(Date,bt.TestDate) between '" + sdate + "' and '" + edate + "' and lb.IsPaid =1 group by v.VendorName,v.CompanyName, V.Id";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult YearlyLab(string term, int? year)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(bt.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1, V.Id from Lab d join Vendor v on d.Vendor_Id = v.Id  
join BookTestLab bt on bt.Lab_Id = d.Id
join LabBooking lb on lb.PatientId = bt.Patient_Id
where Year(bt.TestDate) = Year(GetDate()) and lb.IsPaid =1 group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (!year.HasValue)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                }
            }
            else { 
            
                var qry1 = @"select Sum(bt.Amount) as Counts, v.VendorName as Name,v.CompanyName as Name1, V.Id from Lab d join Vendor v on d.Vendor_Id = v.Id  
join BookTestLab bt on bt.Lab_Id = d.Id
join LabBooking lb on lb.PatientId = bt.Patient_Id
where Year(bt.TestDate) = '"+year+"' and lb.IsPaid =1 group by v.VendorName,v.CompanyName, V.Id";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }


        //HealthCheckUp
        public ActionResult DailyHealth(string term, DateTime? date)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select SUM(cc.Amount) as Counts, v.VendorName, V.Id, v.CompanyName from [HealthCheckupCenter] d 
join Vendor v on d.Vendor_Id = v.Id  
join CmpltCheckUp cc on cc.Center_Id = d.Id
join HealthBooking hb on hb.PatientId = cc.PatientId
where cc.TestDate  = GETDATE() and hb.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (date == null)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                }
            }
            else { 
                string dt = date.Value.ToString("MM/dd/yyyy");
                var qry1 = @"select SUM(cc.Amount) as Counts, v.VendorName, V.Id, v.CompanyName from [HealthCheckupCenter] d 
join Vendor v on d.Vendor_Id = v.Id  
join CmpltCheckUp cc on cc.Center_Id = d.Id
join HealthBooking hb on hb.PatientId = cc.PatientId
where cc.TestDate  = '"+dt+"' and hb.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult WeeklyHealth(string term, DateTime? date)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"
select SUM(cc.Amount) as Counts, v.VendorName, V.Id, v.CompanyName from [HealthCheckupCenter] d 
join Vendor v on d.Vendor_Id = v.Id  
join CmpltCheckUp cc on cc.Center_Id = d.Id
join HealthBooking hb on hb.PatientId = cc.PatientId
where Convert(Date,cc.TestDate)  between DATEADD(day,-7,GETDATE()) and GetDate() and hb.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (date != null)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                }
            }
            else { 
                DateTime dateCriteria = date.Value.AddDays(-7);
                string Tarikh = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select SUM(cc.Amount) as Counts, v.VendorName, V.Id, v.CompanyName from [HealthCheckupCenter] d 
join Vendor v on d.Vendor_Id = v.Id  
join CmpltCheckUp cc on cc.Center_Id = d.Id
join HealthBooking hb on hb.PatientId = cc.PatientId
where Convert(Date,cc.TestDate)  between '" + dateCriteria + "' and '" + Tarikh + "' and hb.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult MonthlyHealth(string term, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select SUM(cc.Amount) as Counts, v.VendorName, V.Id, v.CompanyName from [HealthCheckupCenter] d 
join Vendor v on d.Vendor_Id = v.Id  
join CmpltCheckUp cc on cc.Center_Id = d.Id
join HealthBooking hb on hb.PatientId = cc.PatientId
where Month(cc.TestDate) = Month(GetDate()) and hb.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (sdate == null && edate == null)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                }
            }
            else { 
                var qry1 = @"select SUM(cc.Amount) as Counts, v.VendorName, V.Id, v.CompanyName from [HealthCheckupCenter] d 
join Vendor v on d.Vendor_Id = v.Id  
join CmpltCheckUp cc on cc.Center_Id = d.Id
join HealthBooking hb on hb.PatientId = cc.PatientId
where Convert(Date,cc.TestDate) between '" + sdate + "' and '" + edate + "' and hb.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult YearlyHealth(string term, int? year)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select SUM(cc.Amount) as Counts, v.VendorName, V.Id, v.CompanyName from [HealthCheckupCenter] d 
join Vendor v on d.Vendor_Id = v.Id  
join CmpltCheckUp cc on cc.Center_Id = d.Id
join HealthBooking hb on hb.PatientId = cc.PatientId
where Year(cc.TestDate) = Year(GetDate()) and hb.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<Vendorses>(qry).ToList();
            if (!year.HasValue)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    model.Vendors = data;
                    ViewBag.Payment = payment;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                }
            }else { 
                var qry1 = @"select SUM(cc.Amount) as Counts, v.VendorName, V.Id, v.CompanyName from [HealthCheckupCenter] d 
join Vendor v on d.Vendor_Id = v.Id  
join CmpltCheckUp cc on cc.Center_Id = d.Id
join HealthBooking hb on hb.PatientId = cc.PatientId
where Year(cc.TestDate) = '"+year+"' and hb.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.Vendors = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }
    }
}