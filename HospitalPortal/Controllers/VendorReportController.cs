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
            var qry = @"select Sum(pa.TotalFee) as Counts, v.VendorName as Name, v.CompanyName as Name1 from Doctor d join Vendor v on d.Vendor_Id = v.Id join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id  where pa.AppointmentDate =  GETDATE() and pa.IsPaid=1 group by v.VendorName, v.CompanyName";
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
                var qry1 = @"select Sum(pa.Amount) as Counts, v.VendorName as Name, v.CompanyName as Name1 from Doctor d join Vendor v on d.Vendor_Id = v.Id join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id  where Convert(Date,pa.AppointmentDate)  =  '"+dt+"'  and pa.IsPaid=1 group by v.VendorName, v.CompanyName";
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
         
        public ActionResult WeeklyDoc(string term, DateTime? date)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
           
            if (date != null)
            {
				DateTime dateCriteria = date.Value.AddDays(-7);
				string Tarikh = dateCriteria.ToString("dd/MM/yyyy");
				var qry1 = @"select Sum(pa.TotalFee) as Counts, v.VendorName as Name, v.CompanyName as Name1 from Doctor d join Vendor v on d.Vendor_Id = v.Id join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id  where pa.AppointmentDate between '" + dateCriteria + "' and '" + Tarikh + "'  and pa.IsPaid=1 group by v.VendorName, v.CompanyName";
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
				var qry = @"select Sum(pa.TotalFee) as Counts, v.VendorName as Name, v.CompanyName as Name1 from Doctor d join Vendor v on d.Vendor_Id = v.Id join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id  where pa.AppointmentDate  between DATEADD(day,-7,GETDATE()) and GetDate()  and pa.IsPaid=1 group by v.VendorName, v.CompanyName";
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
				return View(model);
			}
			
        }

        public ActionResult MonthlyDoc(string term, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(pa.TotalFee) as Counts, v.VendorName as Name, v.Id, v.CompanyName as Name1 from Doctor d join Vendor v on d.Vendor_Id = v.Id join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id  where Month(pa.AppointmentDate) = Month(GetDate()) and pa.IsPaid=1 group by v.VendorName,v.CompanyName, v.Id";
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
                var qry1 = @"select Sum(pa.Amount) as Counts, v.VendorName as Name, v.Id, v.CompanyName as Name1 from Doctor d join Vendor v on d.Vendor_Id = v.Id join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id  where Convert(Date,pa.AppointmentDate) between '" + sdate + "' and '" + edate + "' and pa.IsPaid=1 group by v.VendorName,v.CompanyName, v.Id";
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

        public ActionResult YearlyDoc(string term, int? year)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(pa.TotalFee) as Counts, v.VendorName as Name, d.Id, v.CompanyName as Name1 from Doctor d join Vendor v on d.Vendor_Id = v.Id join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id  where Year(pa.AppointmentDate) = Year(GetDate()) and pa.IsPaid=1 group by v.VendorName,v.CompanyName, d.Id";
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
                var qry1 = @"select Sum(pa.TotalFee) as Counts, v.VendorName as Name, v.CompanyName as Name1 from Doctor d join Vendor v on d.Vendor_Id = v.Id join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id  where Year(pa.AppointmentDate) = '" + year + "' and pa.IsPaid=1 group by v.VendorName, v.CompanyName";
                var data1 = ent.Database.SqlQuery<Vendorses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Entered Year Doesn't Contain any Information.";
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
            var qry = @"select Sum(trm.Amount) as Counts, ve.VendorName as Name, ve.CompanyName as Name1 from Vehicle v
join Vendor ve on ve.Id = v.Vendor_Id
join TravelRecordMaster trm on trm.Vehicle_Id = v.Id
where trm.IsDriveCompleted = 1 and Convert(Date,trm.RequestDate) = GetDate() group by  ve.VendorName, ve.CompanyName";
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
                var qry1 = @"select Sum(trm.Amount) as Counts, ve.VendorName as Name, ve.CompanyName as Name1 from Vehicle v
join Vendor ve on ve.Id = v.Vendor_Id
join TravelRecordMaster trm on trm.Vehicle_Id = v.Id
where trm.IsDriveCompleted = 1 and Convert(Date,trm.RequestDate) = '"+date+"' group by  ve.VendorName, ve.CompanyName";
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

        public ActionResult WeeklyVeh(string term, DateTime? date)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(trm.Amount) as Counts, ve.VendorName as Name, ve.CompanyName as Name1 from Vehicle v
join Vendor ve on ve.Id = v.Vendor_Id
join TravelRecordMaster trm on trm.Vehicle_Id = v.Id
where trm.IsDriveCompleted = 1 and trm.RequestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() group by  ve.VendorName, ve.CompanyName";
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
                var qry1 = @"selectselect Sum(trm.Amount) as Counts, ve.VendorName as Name, ve.CompanyName as Name1 from Vehicle v
join Vendor ve on ve.Id = v.Vendor_Id
join TravelRecordMaster trm on trm.Vehicle_Id = v.Id
where trm.IsDriveCompleted = 1 and trm.RequestDate between '"+Tarikh+"' and '"+date+"' group by  ve.VendorName, ve.CompanyName";
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

        public ActionResult MonthlyVeh(string term, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(trm.Amount) as Counts, ve.VendorName as Name, ve.CompanyName as Name1 from Vehicle v
join Vendor ve on ve.Id = v.Vendor_Id
join TravelRecordMaster trm on trm.Vehicle_Id = v.Id
where trm.IsDriveCompleted = 1 and Month(trm.RequestDate) = Month(GetDate()) group by  ve.VendorName, ve.CompanyName";
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
                var qry1 = @"select Sum(trm.Amount) as Counts, ve.VendorName as Name, ve.CompanyName as Name1 from Vehicle v
join Vendor ve on ve.Id = v.Vendor_Id
join TravelRecordMaster trm on trm.Vehicle_Id = v.Id
where trm.IsDriveCompleted = 1 and Convert(Date,trm.RequestDate) between '" + sdate+"' and '"+edate+"' group by  ve.VendorName, ve.CompanyName";
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

        public ActionResult YearlyVeh(string term, int? year)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select Sum(trm.Amount) as Counts, ve.VendorName as Name, ve.CompanyName as Name1 from Vehicle v
join Vendor ve on ve.Id = v.Vendor_Id
join TravelRecordMaster trm on trm.Vehicle_Id = v.Id
where trm.IsDriveCompleted = 1 and Year(trm.RequestDate) = Year(GetDate()) group by  ve.VendorName, ve.CompanyName";
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
                var qry1 = @"select Sum(trm.Amount) as Counts, ve.VendorName as Name, ve.CompanyName as Name1 from Vehicle v
join Vendor ve on ve.Id = v.Vendor_Id
join TravelRecordMaster trm on trm.Vehicle_Id = v.Id
where trm.IsDriveCompleted = 1 and Year(trm.RequestDate) = '" + year+"' group by  ve.VendorName, ve.CompanyName";
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
            var qry = @"select v.VendorName as Name, v.CompanyName as Name1, V.Id ,
(IsNull(Datediff(day,ns.StartDate,ns.EndDate)* ns.PerDayAmount,0)) as Counts
from Nurse d 
join NurseService ns on ns.Nurse_Id = d.Id
join Vendor v on d.Vendor_Id = v.Id  where Convert(Date,ns.ServiceAcceptanceDate)   = GETDATE()  and ns.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id,ns.StartDate,ns.EndDate,ns.PerDayAmount";
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
                var qry1 = @"select v.VendorName as Name, v.CompanyName as Name1, V.Id ,
(IsNull(Datediff(day,ns.StartDate,ns.EndDate)* ns.PerDayAmount,0)) as Counts
from Nurse d 
join NurseService ns on ns.Nurse_Id = d.Id
join Vendor v on d.Vendor_Id = v.Id  where Convert(Date,ns.ServiceAcceptanceDate)    = '" + dt + "'  and ns.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id,ns.StartDate,ns.EndDate,ns.PerDayAmount";
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

        public ActionResult WeeklyNur(string term, DateTime? date)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select v.VendorName as Name, v.CompanyName as Name1, V.Id ,
(IsNull(Datediff(day,ns.StartDate,ns.EndDate)* ns.PerDayAmount,0)) as Counts
from Nurse d 
join NurseService ns on ns.Nurse_Id = d.Id
join Vendor v on d.Vendor_Id = v.Id  where Convert(Date,ns.ServiceAcceptanceDate)   between DATEADD(day,-7,GETDATE()) and GetDate() and ns.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id,ns.StartDate,ns.EndDate,ns.PerDayAmount
";
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
                var qry1 = @"select v.VendorName as Name, v.CompanyName as Name1, V.Id ,
(IsNull(Datediff(day,ns.StartDate,ns.EndDate)* ns.PerDayAmount,0)) as Counts
from Nurse d 
join NurseService ns on ns.Nurse_Id = d.Id
join Vendor v on d.Vendor_Id = v.Id  where Convert(Date,ns.ServiceAcceptanceDate) between '" + dateCriteria + "' and '" + Tarikh + "'and ns.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id,ns.StartDate,ns.EndDate,ns.PerDayAmount";
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

        public ActionResult MonthlyNur(string term, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select v.VendorName as Name, v.CompanyName as Name1, V.Id ,
(IsNull(Datediff(day,ns.StartDate,ns.EndDate)* ns.PerDayAmount,0)) as Counts
from Nurse d 
join NurseService ns on ns.Nurse_Id = d.Id
join Vendor v on d.Vendor_Id = v.Id  where Month(ns.ServiceAcceptanceDate) = Month(GetDate()) and ns.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id,ns.StartDate,ns.EndDate,ns.PerDayAmount
";
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
                var qry1 = @"select v.VendorName as Name, v.CompanyName as Name1, V.Id ,
(IsNull(Datediff(day,ns.StartDate,ns.EndDate)* ns.PerDayAmount,0)) as Counts
from Nurse d 
join NurseService ns on ns.Nurse_Id = d.Id
join Vendor v on d.Vendor_Id = v.Id  where Convert(DATE,ns.ServiceAcceptanceDate) between '" + sdate + "' and '" + edate + "' and ns.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id,ns.StartDate,ns.EndDate,ns.PerDayAmount";
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

        public ActionResult YearlyNur(string term, int? year)
        {
            var model = new ReportDetails();
            double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var qry = @"select v.VendorName as Name, v.CompanyName as Name1, V.Id ,
(IsNull(Datediff(day,ns.StartDate,ns.EndDate)* ns.PerDayAmount,0)) as Counts
from Nurse d 
join NurseService ns on ns.Nurse_Id = d.Id
join Vendor v on d.Vendor_Id = v.Id  where Year(ns.ServiceAcceptanceDate) = Year(GetDate()) and ns.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id,ns.StartDate,ns.EndDate,ns.PerDayAmount";
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
                var qry1 = @"select v.VendorName as Name, v.CompanyName as Name1, V.Id ,
(IsNull(Datediff(day,ns.StartDate,ns.EndDate)* ns.PerDayAmount,0)) as Counts
from Nurse d 
join NurseService ns on ns.Nurse_Id = d.Id
join Vendor v on d.Vendor_Id = v.Id  where Year(ns.ServiceAcceptanceDate) = = '" + year + "'and ns.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id,ns.StartDate,ns.EndDate,ns.PerDayAmount";
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