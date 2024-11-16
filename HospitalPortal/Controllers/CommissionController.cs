using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    public class CommissionController : Controller
    {
        DbEntities ent = new DbEntities();
        // GET: Commission
        public ActionResult CommissionReport()
        {
            return View();
        }


        public ActionResult Doctor(string term, DateTime? JoiningDate, DateTime? EndDate, string name = null)
        {
            int Id = GetVendorId();
            double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Doctor'").FirstOrDefault();
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            //double vendorCommission = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Vendor'").FirstOrDefault();
            double vendorCommission = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster where IsDeleted=0 and Department='Franchise' and Name='Doctor'").FirstOrDefault();
            var model = new ReportDTO();
            if (JoiningDate != null && EndDate != null)
            {
                DateTime dateCriteria = JoiningDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select Sum(pa.TotalFee) as Amount, v.VendorName, v.CompanyName,v.UniqueId as VendorId,d.DoctorName,d.DoctorId as UniqueId from Doctor d 
join Vendor v on d.Vendor_Id = v.Id 
join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id where Convert(varchar,pa.AppointmentDate,103) between '" + JoiningDate + "' and '" + EndDate + "'  and pa.IsPaid=1 and v.Id="+ Id + " group by v.VendorName, v.CompanyName,d.DoctorName,d.DoctorId,,v.UniqueId";
                var data1 = ent.Database.SqlQuery<VendorCommissionReport>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                    return View(model);
                }
                else
                {
                    ViewBag.Commission = commision;
                    ViewBag.Transactionfee = Transactionfee;
                    ViewBag.tds = tds;
                    ViewBag.vendorCommission = vendorCommission;

                    foreach (var item in data1)
                    {
                        var razorcomm = (item.Amount * Transactionfee) / 100;
                        var totalrazorcomm = razorcomm;
                        item.Amountwithrazorpaycomm = item.Amount + totalrazorcomm;
                        item.FraPaidableamt = (item.Amount * 3) / 100;

                    }

                    if (name != null)
                    {
                        data1 = data1.Where(a => a.VendorName.ToLower().Contains(name)).ToList();
                    }
                    model.VendorCommissionReport = data1;
                    return View(model);
                }
            }
            else
            {
                var doctor = @"select Sum(pa.TotalFee) as Amount, v.VendorName,v.UniqueId as VendorId, v.CompanyName,d.DoctorName,d.DoctorId as UniqueId from Doctor d 
join Vendor v on d.Vendor_Id = v.Id 
join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id 
where pa.AppointmentDate  between DATEADD(day,-7,GETDATE()) and GetDate()  and pa.IsPaid=1 and v.Id="+ Id + " group by v.VendorName, v.CompanyName,d.DoctorName,d.DoctorId,v.UniqueId";
                //var doctor = @"select COUNT(d.Id) as Counts, v.VendorName, v.CompanyName from Doctor d join Vendor v on d.Vendor_Id = v.Id where d.JoiningDate  >= DATEADD(day,-7, GETDATE()) group by v.VendorName,v.CompanyName";
                var data = ent.Database.SqlQuery<VendorCommissionReport>(doctor).ToList();

                // Filter by search term
                if (!string.IsNullOrEmpty(name))
                {
                    data = data.Where(a =>
                        a.DoctorName.ToLower().Contains(name.ToLower()) ||
                        a.UniqueId.Trim().Equals(name.Trim(), StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record.";
                    return View(model);
                }
                else
                {
                    ViewBag.Commission = commision;
                    ViewBag.Transactionfee = Transactionfee;
                    ViewBag.tds = tds;
                    ViewBag.vendorCommission = vendorCommission;

                    foreach (var item in data)
                    {
                        var razorcomm = (item.Amount * Transactionfee) / 100;
                        var totalrazorcomm = razorcomm;
                        item.Amountwithrazorpaycomm = item.Amount + totalrazorcomm;
                        item.FraPaidableamt = (item.Amount * 3) / 100;

                    }

                    model.VendorCommissionReport = data;

                }
                return View(model);
            }

        }


        //Lab Commission Report
        public ActionResult Lab(string term, DateTime? StartDate, DateTime? EndDate, string name = null)
        {
            int Id = GetVendorId();
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            //double vendorCommission = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Lab'").FirstOrDefault();
            double vendorCommission = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster where IsDeleted=0 and Department='Vendor' and Name='Lab'").FirstOrDefault();
            var model = new ReportDTO();
            if (StartDate != null && EndDate != null)
            {
                DateTime dateCriteria = StartDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");

                var qry1 = @"select Sum(P.Amount) as Amount, Count(p.Patient_Id) as Patient from BookTestLab P join Lab D ON d.Id = p.Lab_Id join Vendor v on v.Id = d.Vendor_Id where P.IsPaid=1 and v.Id="+Id+" and  P.TestDate between '" + StartDate + "' and '" + EndDate + "'" ;
                var data1 = ent.Database.SqlQuery<VendorCommissionReport>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    //int total = data1.Count;
                    //pageNumber = (int?)pageNumber ?? 1;
                    //int pageSize = 10;
                    //decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                    //model.TotalPages = (int)noOfPages;
                    //model.PageNumber = (int)pageNumber;
                    //data1 = data1.OrderBy(a => a.Doctor_Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                    if (name != null)
                    {
                        data1 = data1.Where(a => a.VendorName.ToLower().Contains(name)).ToList();
                    }
                    ViewBag.Commission = commision;
                    ViewBag.vendorCommission = vendorCommission;
                    model.VendorCommissionReport = data1;
                    return View(model);
                }
            }
            var vehicle = @"select IsNull(Sum(P.Amount),0) as Amount, Count(p.Patient_Id) as Patient from BookTestLab P join Lab D ON d.Id = p.Lab_Id join Vendor v on v.Id = d.Vendor_Id where P.IsPaid=1 and v.Id=" + Id + " and  P.TestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE()";
            var data = ent.Database.SqlQuery<VendorCommissionReport>(vehicle).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record Of Current Week";
                return View(model);
            }
            else
            {
                ViewBag.Commission = commision;
                ViewBag.vendorCommission = vendorCommission;
                //int total = data.Count;
                //pageNumber = (int?)pageNumber ?? 1;
                //int pageSize = 10;
                //decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                //model.TotalPages = (int)noOfPages;
                //model.PageNumber = (int)pageNumber;
                //data = data.OrderBy(a => a.Doctor_Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                model.VendorCommissionReport = data;
                return View(model);
            }
        }

        public ActionResult HealthCheckuUp(string term, DateTime? StartDate, DateTime? EndDate, string name = null)
        {
            int Id = GetVendorId();
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='HealthCheckUp'").FirstOrDefault();
            double vendorCommission = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster where IsDeleted=0 and Department='Vendor' and Name='HealthCheckup'").FirstOrDefault();
            var model = new ReportDTO();
            if (StartDate != null && EndDate != null)
            {
                DateTime dateCriteria = StartDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select  SUM(A.Amount)  As Amount, Count(A.PatientId) as Patient from CmpltCheckUp A join HealthCheckupcenter D on D.Id = A.Center_Id join Vendor v on v.Id = D.Vendor_Id where A.IsPaid=1 and v.Id="+Id+" and A.RequestDate BETWEEN '"+StartDate+"' and '"+EndDate+"'";
                var data1 = ent.Database.SqlQuery<VendorCommissionReport>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    //ViewBag.Commission = commision;
                    //int total = data1.Count;
                    //pageNumber = (int?)pageNumber ?? 1;
                    //int pageSize = 10;
                    //decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                    //model.TotalPages = (int)noOfPages;
                    //model.PageNumber = (int)pageNumber;
                    //data1 = data1.OrderBy(a => a.Doctor_Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                    if (name != null)
                    {
                        data1 = data1.Where(a => a.VendorName.ToLower().Contains(name)).ToList();
                    }
                    ViewBag.vendorCommission = vendorCommission;
                    ViewBag.Commission = commision;
                    model.VendorCommissionReport = data1;
                    return View(model);
                }
            }
            var vehicle = @"select  SUM(A.Amount)  As Amount, Count(A.PatientId) as Patient from CmpltCheckUp A join HealthCheckupcenter D on D.Id = A.Center_Id join Vendor v on v.Id = D.Vendor_Id where A.IsPaid=1 and v.Id=" + Id + " and A.RequestDate BETWEEN '2018-08-11' and '2018-12-10' group by A.Amount,V.VendorName, v.CompanyName";
            var data = ent.Database.SqlQuery<VendorCommissionReport>(vehicle).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record Of Current Week";
                return View(model);
            }
            else
            {
                ViewBag.Commission = commision;
                ViewBag.vendorCommission = vendorCommission;
                //ViewBag.paymentPercent = paymentPercent;
                //int total = data.Count;
                //pageNumber = (int?)pageNumber ?? 1;
                //int pageSize = 10;
                //decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                //model.TotalPages = (int)noOfPages;
                //model.PageNumber = (int)pageNumber;
                //data = data.OrderBy(a => a.Doctor_Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                model.VendorCommissionReport = data;
                return View(model);
            }

        }


        public ActionResult Vehicle(string term, DateTime? JoiningDate, DateTime? EndDate, string name = null)
        {
            int Id = GetVendorId();
            double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Ambulance'").FirstOrDefault();
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();
            double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();

            //double vendorCommission = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Vendor'").FirstOrDefault();
            double vendorCommission = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster where IsDeleted=0 and Department='Vendor' and Name='Vehicle'").FirstOrDefault();
            var model = new ReportDTO();
            if (JoiningDate != null)
            {
                DateTime dateCriteria = JoiningDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select v.Id,Sum(trm.TotalPrice) as Counts, ve.VendorName,ve.UniqueId as VendorId, ve.CompanyName,v.VehicleNumber,v.VehicleName,d.DriverName,d.DriverId as UniqueId from Vehicle v
join Vendor ve on ve.Id = v.Vendor_Id 
join Driver as d on d.Vehicle_Id=v.Id
join DriverLocation trm on trm.Driver_Id = d.Id 
where Convert(varchar,trm.EntryDate,103) between '" + JoiningDate + "' and '" + EndDate + "' and trm.IsPay='Y' and ve.Id="+Id+" group by ve.VendorName, ve.CompanyName,v.VehicleNumber,v.VehicleName,d.DriverName,d.DriverId,v.Id,ve.UniqueId";
                var data1 = ent.Database.SqlQuery<VendorCommissionReport>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {

                    ViewBag.Commission = commision;
                    ViewBag.Transactionfee = Transactionfee;
                    ViewBag.tds = tds;
                    ViewBag.vendorCommission = vendorCommission;

                    foreach (var item in data1)
                    {
                        var razorcomm = (item.Counts * Transactionfee) / 100;
                        var totalrazorcomm = razorcomm;
                        item.Amountwithrazorpaycomm = item.Counts + totalrazorcomm;
                        item.FraPaidableamt = (item.Counts * 3) / 100;

                    }
                    model.VendorCommissionReport = data1;

                }
                return View(model);
            }
            else
            {
                var vehicle = @"select v.Id,Sum(trm.TotalPrice) as Counts, ve.VendorName,ve.UniqueId as VendorId, ve.CompanyName,v.VehicleNumber,v.VehicleName,d.DriverName,d.DriverId as UniqueId from Vehicle v
join Vendor ve on ve.Id = v.Vendor_Id 
join Driver as d on d.Vehicle_Id=v.Id
join DriverLocation trm on trm.Driver_Id = d.Id 
where trm.EntryDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() and trm.IsPay='Y' and ve.Id="+Id+" group by  ve.VendorName, ve.CompanyName,v.VehicleNumber,v.VehicleName,d.DriverName,d.DriverId,v.Id,ve.UniqueId";
                var data = ent.Database.SqlQuery<VendorCommissionReport>(vehicle).ToList();

                // Filter by search term
                if (!string.IsNullOrEmpty(name))
                {
                    data = data.Where(a =>
                        a.VendorName.ToLower().Contains(name.ToLower()) ||
                        a.VendorId.Trim().Equals(name.Trim(), StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record Of Current Week";
                    return View(model);
                }
                else
                {
                    ViewBag.Commission = commision;
                    ViewBag.Transactionfee = Transactionfee;
                    ViewBag.tds = tds;
                    ViewBag.vendorCommission = vendorCommission;

                    foreach (var item in data)
                    {
                        var razorcomm = (item.Counts * Transactionfee) / 100;
                        var totalrazorcomm = razorcomm;
                        item.Amountwithrazorpaycomm = item.Counts + totalrazorcomm;
                        item.FraPaidableamt = (item.Counts * 3) / 100;

                    }
                    model.VendorCommissionReport = data;
                    return View(model);
                }
            }

        }


        //public ActionResult Driver(string term, DateTime? JoiningDate, string name = null)
        //{
        //    double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
        //    double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Vendor' and Name='" + term + "'").FirstOrDefault();

        //    var model = new ReportDTO();
        //    if (JoiningDate != null)
        //    {
        //        DateTime dateCriteria = JoiningDate.Value.AddDays(-7);
        //        string date = dateCriteria.ToString("dd/MM/yyyy");
        //        var qry1 = @"select COUNT(d.Id) as Counts, v.VendorName, v.CompanyName from Vehicle d join Vendor v on d.Vendor_Id = v.Id where d.JoiningDate between '" + dateCriteria + "' and '" + JoiningDate + "' group by v.VendorName, v.CompanyName";
        //        var data1 = ent.Database.SqlQuery<VendorCommissionReport>(qry1).ToList();
        //        if (data1.Count() == 0)
        //        {
        //            TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
        //        }
        //        else
        //        {
        //            //ViewBag.Commission = commision;
        //            //int total = data1.Count;
        //            //pageNumber = (int?)pageNumber ?? 1;
        //            //int pageSize = 10;
        //            //decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
        //            //model.TotalPages = (int)noOfPages;
        //            //model.PageNumber = (int)pageNumber;
        //            //data1 = data1.OrderBy(a => a.Doctor_Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
        //            if (name != null)
        //            {
        //                data1 = data1.Where(a => a.VendorName.ToLower().Contains(name)).ToList();
        //            }
        //            ViewBag.Payment = payment;
        //            ViewBag.Commission = commision;
        //            model.VendorCommissionReport = data1;
        //            return View(model);
        //        }
        //    }
        //    var vehicle = @"select COUNT(d.Id) as Counts, v.VendorName, v.CompanyName from Driver d join Vendor v on d.Vendor_Id = v.Id  where d.JoiningDate  >= DATEADD(day,-7, GETDATE()) group by v.VendorName,v.CompanyName";
        //    var data = ent.Database.SqlQuery<VendorCommissionReport>(vehicle).ToList();
        //    if (data.Count() == 0)
        //    {
        //        TempData["msg"] = "No Record Of Current Week";
        //        return View(model);
        //    }
        //    else
        //    {
        //        ViewBag.Payment = payment;
        //        ViewBag.Commission = commision;
        //        //int total = data.Count;
        //        //pageNumber = (int?)pageNumber ?? 1;
        //        //int pageSize = 10;
        //        //decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
        //        //model.TotalPages = (int)noOfPages;
        //        //model.PageNumber = (int)pageNumber;
        //        //data = data.OrderBy(a => a.Doctor_Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
        //        model.VendorCommissionReport = data;

        //        return View(model);
        //    }



        //}
        private int GetVendorId()
        {
            int loginId = Convert.ToInt32(User.Identity.Name);
            int VendorId = ent.Database.SqlQuery<int>("select Id from Vendor where AdminLogin_Id=" + loginId).FirstOrDefault();
            return VendorId;
        }

        public ActionResult Nurse(string term, DateTime? JoiningDate, DateTime? EndDate, string name = null)
        {
            int Id = GetVendorId();
            var model = new ReportDTO();
            double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Nurse'").FirstOrDefault();
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Nurse'").FirstOrDefault();
            double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Nurse'").FirstOrDefault();

            double vendorCommission = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster where IsDeleted=0 and Department='Vendor' and Name='Nurse' and IsDeleted=0").FirstOrDefault();

            if (JoiningDate != null && EndDate != null)
            {
                DateTime dateCriteria = JoiningDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select v.VendorName ,v.CompanyName ,n.Fee,n.Id,n.NurseName,n.NurseId as UniqueId,v.UniqueId as VendorId,SUM(ns.TotalFee) as Amount from Nurse n  
join NurseService ns on ns.Nurse_Id = n.Id 
join Vendor v on n.Vendor_Id = v.Id
where ns.ServiceDate between Convert(datetime,'" + JoiningDate + "',103) and Convert(datetime,'" + EndDate + "',103) and ns.IsPaid=1 and v.Id="+ Id + " group by v.VendorName,v.CompanyName,n.Id,ns.StartDate,ns.EndDate,n.Fee,n.NurseName,n.NurseId,v.UniqueId";
                var data1 = ent.Database.SqlQuery<VendorCommissionReport>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {

                    ViewBag.Commission = commision;
                    ViewBag.Transactionfee = Transactionfee;
                    ViewBag.tds = tds;
                    ViewBag.vendorCommission = vendorCommission;

                    foreach (var item in data1)
                    {
                        var razorcomm = (item.Amount * Transactionfee) / 100;
                        var totalrazorcomm = razorcomm;
                        item.Amountwithrazorpaycomm = item.Amount + totalrazorcomm;
                        item.FraPaidableamt = (item.Amount * 3) / 100;

                    }
                    model.VendorCommissionReport = data1;

                }
                return View(model);
            }
            else
            {
                string q = @"select v.VendorName ,v.CompanyName ,n.Fee,n.Id,n.NurseName,n.NurseId as UniqueId,v.UniqueId as VendorId,SUM(ns.TotalFee) as Amount from Nurse n  
join NurseService ns on ns.Nurse_Id = n.Id 
join Vendor v on n.Vendor_Id = v.Id  
where Convert(Date,ns.ServiceDate) between DATEADD(day,-7,GETDATE()) and GetDate() and ns.IsPaid=1 and v.Id="+ Id + " group by v.VendorName,v.CompanyName,n.Id,ns.StartDate,ns.EndDate,n.Fee,n.NurseName,n.NurseId,v.UniqueId";
                var data = ent.Database.SqlQuery<VendorCommissionReport>(q).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Result";
                    return View(model);
                }

                ViewBag.Commission = commision;
                ViewBag.Transactionfee = Transactionfee;
                ViewBag.tds = tds;
                ViewBag.vendorCommission = vendorCommission;

                foreach (var item in data)
                {
                    var razorcomm = (item.Amount * Transactionfee) / 100;
                    var totalrazorcomm = razorcomm;
                    item.Amountwithrazorpaycomm = item.Amount + totalrazorcomm;
                    item.FraPaidableamt = (item.Amount * 3) / 100;

                }
                model.VendorCommissionReport = data;
                return View(model);
            }
        }


        public class TestClass{
          public IEnumerable<Hello> Hello { get; set; }
        }

        public class Hello
        {
            public double? TotalFee { get; set; }
        }
    }
}