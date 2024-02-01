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
    [Authorize(Roles = "Admin")]
    public class CommisionReportController : Controller
    {
        DbEntities ent = new DbEntities();
        // GET: CommisionReport
        public ActionResult Commision()
        {
            return View();
        }


        //Doctor Commission Report
        public ActionResult Doctor(string term, int? pageNumber, DateTime? AppointmentDate, string name = null)
        {
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term+"'").FirstOrDefault();
            var model = new ReportDTO();
            if (AppointmentDate != null)
            {
                DateTime dateCriteria = AppointmentDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select A.Doctor_Id, D.DoctorName, SUM(A.TotalFee) As Amount from dbo.PatientAppointment A join Doctor D on D.Id = A.Doctor_Id  where A.IsPaid=1 and A.AppointmentDate between '" + dateCriteria + "' and '" + AppointmentDate + "' GROUP BY A.TotalFee, D.DoctorName, A.Doctor_Id";
                var data1 = ent.Database.SqlQuery<DoctorCommissionReport>(qry1).ToList();
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
                    data1 = data1.OrderBy(a => a.Doctor_Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                    if(name!= null)
                    {
                        data1 = data1.Where(a => a.DoctorName.ToLower().Contains(term)).ToList();
                    }
                    model.DoctorCommisionReport = data1;
                    return View(model);
                }
            }
            var doctor = @"select A.Doctor_Id, D.DoctorName, SUM(A.TotalFee)  As Amount from dbo.PatientAppointment A join Doctor D on D.Id = A.Doctor_Id where A.IsPaid=1 and A.AppointmentDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() group by A.TotalFee, D.DoctorName, A.Doctor_Id";
            var data = ent.Database.SqlQuery<DoctorCommissionReport>(doctor).ToList();
            if(data.Count() == 0)
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
                data = data.OrderBy(a => a.Doctor_Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                model.DoctorCommisionReport = data;
                return View(model);
            }
         
        }

        //Lab Commission Report
        public ActionResult Lab(string term, int? pageNumber, DateTime? TestDate, string name = null)
        {
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var model = new ReportDTO();
            if (TestDate != null)
            {
                DateTime dateCriteria = TestDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select A.Lab_Id, D.LabName, SUM(A.Amount) As Amount from BookTestLab A join Lab D on D.Id = A.Lab_Id  where A.IsPaid=1 and A.TestDate between '" + dateCriteria + "' and '" + TestDate + "' GROUP BY A.Amount, D.LabName, A.Lab_Id";
                var data1 = ent.Database.SqlQuery<LabCommissionReport>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Commission = commision;
                    if (name != null)
                    {
                        data1 = data1.Where(a => a.LabName.ToLower().Contains(term)).ToList();
                    }
                    model.LabCommisionReport = data1;
                    return View(model);
                }
            }
            var doctor = @"select A.Lab_Id, D.LabName, SUM(A.Amount) As Amount from BookTestLab A join Lab D on D.Id = A.Lab_Id  where A.IsPaid=1 and A.TestDate between DATEADD(DAY, -7, GETDATE()) AND GETDATE() group by A.Amount, D.LabName, A.Lab_Id";
            var data = ent.Database.SqlQuery<LabCommissionReport>(doctor).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record Of Current Week";
                return View(model);
            }
            else
            {
                ViewBag.Commission = commision;
                model.LabCommisionReport = data;
                return View(model);
            }

        }

        //Health Commission Report
        public ActionResult Health(string term, int? pageNumber, DateTime? AppointmentDate, string name = null)
        {
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var model = new ReportDTO();
            if (AppointmentDate != null)
            {
                DateTime dateCriteria = AppointmentDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select A.Center_Id, D.LabName, SUM(A.Amount) As Amount from CmpltCheckUp A join HealthCheckupCenter D on D.Id = A.Center_Id  where A.IsPaid=1 and A.TestDate between  '" + dateCriteria + "' and '" + AppointmentDate + "' GROUP BY A.Amount, D.LabName, A.Center_Id";
                var data1 = ent.Database.SqlQuery<HealthCommissionReport>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Commission = commision;
                    if (name != null)
                    {
                        data1 = data1.Where(a => a.LabName.ToLower().Contains(term)).ToList();
                    }
                    model.HealthCommisionReport = data1;
                    return View(model);
                }
            }
            var doctor = @"select A.Center_Id, D.LabName, SUM(A.Amount) As Amount from CmpltCheckUp A join HealthCheckupCenter D on D.Id = A.Center_Id  where A.IsPaid=1 and A.TestDate between DATEADD(DAY, -7, GETDATE()) AND GETDATE() group by A.Amount, D.LabName, A.Center_Id";
            var data = ent.Database.SqlQuery<HealthCommissionReport>(doctor).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record Of Current Week";
                return View(model);
            }
            else
            {
                ViewBag.Commission = commision;
                model.HealthCommisionReport = data;
                return View(model);
            }

        }

        //Nurse Commission Report
        public ActionResult Nurse()
        {
            var model = new NurseDTO();
            string q = @"select ns.Nurse_Id, n.NurseName, n.Fee from Nurse n join NurseService ns on n.Id = ns.Nurse_Id group by ns.Nurse_Id, n.NurseName,n.Fee";
            var data = ent.Database.SqlQuery<Nurse4Commission>(q).ToList();
            model.Nurse4Commission = data;
            return View(model);
        }

        public ActionResult NurseDetails(int? NurseId, DateTime? ServiceAcceptanceDate)
        {
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Nurse'").FirstOrDefault();
            var model = new NurseCommissionReports();
            if (ServiceAcceptanceDate != null)
            {
                DateTime dateCriteria = ServiceAcceptanceDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select ns.Nurse_Id, ns.Id, ns.ServiceStatus, ns.IsPaid, case when ns.PaymentDate is null then 'N/A' else Convert(nvarchar(100),ns.PaymentDate,103) end as PaymentDate, case when ns.ServiceAcceptanceDate is null then 'N/A' else Convert(nvarchar(100),ns.ServiceAcceptanceDate,103) end as ServiceAcceptanceDate, Convert(nvarchar(100),ns.RequestDate,103) as RequestDate,'From '+ Convert(nvarchar(100),ns.StartDate,103)+' to '+Convert(nvarchar(100),ns.EndDate,103) as ServiceTiming ,IsNull(n.NurseName,'N/A') as NurseName,
IsNull(n.MobileNumber,'N/A') as NurseMobileNumber,
Datediff(day,ns.StartDate,ns.EndDate) as TotalDays,
IsNull(ns.PerDayAmount,0) as Fee,
ns.TotalFee
 from NurseService ns 
left join Nurse n on ns.Nurse_Id=n.Id
where ns.Nurse_Id = " + NurseId+" and ns.IsPaid=1 and ns.ServiceAcceptanceDate between '"+dateCriteria+"' and '"+ServiceAcceptanceDate+"' order by ns.Id desc";
                var data1 = ent.Database.SqlQuery<NurseAppointmentList>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Commission = commision;
                    model.NurseName = data1.FirstOrDefault().NurseName;
                    model.NurseId = data1.FirstOrDefault().Nurse_Id;
                    model.NurseAppointmentList = data1;
                    return View(model);
                }
            }
            string q = @"select ns.Nurse_Id, ns.Id, ns.ServiceStatus, ns.IsPaid, case when ns.PaymentDate is null then 'N/A' else Convert(nvarchar(100),ns.PaymentDate,103) end as PaymentDate, case when ns.ServiceAcceptanceDate is null then 'N/A' else Convert(nvarchar(100),ns.ServiceAcceptanceDate,103) end as ServiceAcceptanceDate, Convert(nvarchar(100),ns.RequestDate,103) as RequestDate,'From '+ Convert(nvarchar(100),ns.StartDate,103)+' to '+Convert(nvarchar(100),ns.EndDate,103) as ServiceTiming ,IsNull(n.NurseName,'N/A') as NurseName,
IsNull(n.MobileNumber,'N/A') as NurseMobileNumber,
Datediff(day,ns.StartDate,ns.EndDate) as TotalDays,
IsNull(ns.PerDayAmount,0) as Fee,
ns.TotalFee
 from NurseService ns 
left join Nurse n on ns.Nurse_Id=n.Id
where ns.Nurse_Id = '" + NurseId+"' and ns.IsPaid=1 order by ns.Id desc";
            var data = ent.Database.SqlQuery<NurseAppointmentList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "Something Went Wrong";
            }
            else
            {
                ViewBag.Commission = commision;
                model.NurseName = data.FirstOrDefault().NurseName;
                model.NurseId = data.FirstOrDefault().Nurse_Id;
                model.NurseAppointmentList = data;
            }
            return View(model);
        }


        //public ActionResult Nurse(string term, int? pageNumber, DateTime? AppointmentDate, string name = null)
        //{
        //    double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
        //    var model = new ReportDTO();
        //    if (AppointmentDate != null)
        //    {
        //        DateTime dateCriteria = AppointmentDate.Value.AddDays(-7);
        //        string date = dateCriteria.ToString("dd/MM/yyyy");
        //        var qry1 = @"select A.Doctor_Id, D.DoctorName, SUM(A.Amount) As Amount from dbo.PatientAppointment A join Doctor D on D.Id = A.Doctor_Id  where A.IsPaid=1 and A.AppointmentDate between '" + dateCriteria + "' and '" + AppointmentDate + "' GROUP BY A.Amount, D.DoctorName, A.Doctor_Id";
        //        var data1 = ent.Database.SqlQuery<DoctorCommissionReport>(qry1).ToList();
        //        if (data1.Count() == 0)
        //        {
        //            TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
        //        }
        //        else
        //        {
        //            ViewBag.Commission = commision;
        //            int total = data1.Count;
        //            pageNumber = (int?)pageNumber ?? 1;
        //            int pageSize = 10;
        //            decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
        //            model.TotalPages = (int)noOfPages;
        //            model.PageNumber = (int)pageNumber;
        //            data1 = data1.OrderBy(a => a.Doctor_Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
        //            if (name != null)
        //            {
        //                data1 = data1.Where(a => a.DoctorName.ToLower().Contains(term)).ToList();
        //            }
        //            model.DoctorCommisionReport = data1;
        //            return View(model);
        //        }
        //    }
        //    var doctor = @"select A.Doctor_Id, D.DoctorName, SUM(A.Amount)  As Amount from dbo.PatientAppointment A join Doctor D on D.Id = A.Doctor_Id where A.IsPaid=1 and A.AppointmentDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() group by A.Amount, D.DoctorName, A.Doctor_Id";
        //    var data = ent.Database.SqlQuery<DoctorCommissionReport>(doctor).ToList();
        //    if (data.Count() == 0)
        //    {
        //        TempData["msg"] = "No Record Of Current Week";
        //        return View(model);
        //    }
        //    else
        //    {
        //        ViewBag.Commission = commision;
        //        int total = data.Count;
        //        pageNumber = (int?)pageNumber ?? 1;
        //        int pageSize = 10;
        //        decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
        //        model.TotalPages = (int)noOfPages;
        //        model.PageNumber = (int)pageNumber;
        //        data = data.OrderBy(a => a.Doctor_Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
        //        model.DoctorCommisionReport = data;
        //        return View(model);
        //    }

        //}

        //Medicine Commission Report



        public ActionResult Medicine()
        {
            var model = new MedicineList();
            string q = @"select D.Id, D.ChemistName, SUM(od.Amount) As Amount from dbo.MedicineOrder A join Chemist D on D.Id = A.Chemist_Id join MedicineOrderDetail od on od.Order_Id = A.Id  group by D.Id, D.ChemistName";
            var data = ent.Database.SqlQuery<ChemistDTO>(q).ToList();
            model.ChemistListDTO = data;
            return View(model);
        }

        public ActionResult ChemistDetails(int ChemistId, DateTime? OrderDate)
        {
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Medicine'").FirstOrDefault();
            var model = new ChemistCommissionReport();
            if (OrderDate != null)
            {
                DateTime dateCriteria = OrderDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select D.Id, D.ChemistName, SUM(od.Amount) As Amount, A.OrderId from dbo.MedicineOrder A join Chemist D on D.Id = A.Chemist_Id join MedicineOrderDetail od on od.Order_Id = A.Id where A.IsPaid=1 and A.OrderDate between '"+dateCriteria+"' and '"+OrderDate+ "' and D.Id="+ChemistId+" GROUP BY od.Amount, D.ChemistName, D.Id, A.OrderId";
                var data1 = ent.Database.SqlQuery<ChemsitDetails>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Commission = commision;
                    model.ChemistName = data1.FirstOrDefault().ChemistName;
                    model.ChemistId = data1.FirstOrDefault().Id;
                    model.chemistDetails = data1;
                    return View(model);
                }
            }
            string q = @"select D.Id, D.ChemistName, SUM(od.Amount) As Amount from dbo.MedicineOrder A 
join Chemist D on D.Id = A.Chemist_Id 
join MedicineOrderDetail od on od.Order_Id = A.Id  
where A.IsPaid=1 and A.OrderDate between DATEADD(DAY, -7, GETDATE()) AND GETDATE() and D.Id="+ChemistId+" GROUP BY od.Amount, D.ChemistName, D.Id,A.OrderId";
            var data = ent.Database.SqlQuery<ChemsitDetails>(q).ToList();
            ViewBag.Commission = commision;
            model.ChemistName = data.FirstOrDefault().ChemistName;
            model.ChemistId = data.FirstOrDefault().Id; 
            model.chemistDetails = data;
            return View(model);
        }


        public ActionResult Hospital()
        {
            TempData["Msg"] = "Under Maintenance";
            return View();
        }

        public ActionResult Driver(int? pageNumber, DateTime? RequestDate, string name = null)
        {
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();
            var model = new AmbulanceList();
            if (RequestDate != null)
            {
                DateTime dateCriteria = RequestDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
v.Id as VehicleId, d.DriverName, Sum(trm.Amount) as Amount
from TravelRecordMaster trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1 and trm.RequestDate between '"+dateCriteria+"' and '"+ RequestDate + "' group by v.VehicleNumber, v.VehicleName, v.Id,d.DriverName";
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
            var doctor = @"select v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
v.Id as VehicleId, d.DriverName, Sum(trm.Amount) as Amount
from TravelRecordMaster trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1 and trm.RequestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() group by v.VehicleNumber, v.VehicleName, v.Id,d.DriverName";
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
    }
}