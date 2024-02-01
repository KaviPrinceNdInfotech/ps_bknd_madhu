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


        public ActionResult Doctor(string term, int? Id, DateTime? StartDate, DateTime? EndDate, int? pageNumber, string name = null)
        {
            int id = GetVendorId();
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            double vendorCommission = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster where IsDeleted=0 and Department='Vendor' and Name='Doctor'").FirstOrDefault();
            var model = new ReportDTO();
            if (StartDate != null && EndDate != null)
            {
                DateTime dateCriteria = StartDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select SUM(A.Amount) As Amount, Count(A.Patient_Id) as Counts from dbo.PatientAppointment A join Doctor D on D.Id = A.Doctor_Id join Vendor v on v.Id = D.Vendor_Id where A.IsPaid=1 and v.Id="+ id +" and A.AppointmentDate BETWEEN '" + StartDate + "' and '" + EndDate + "'";
                var data1 = ent.Database.SqlQuery<VendorCommissionReport>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Commission = commision;
                    ViewBag.vendorCommission = vendorCommission;
                    double? Amount = data1.FirstOrDefault().Amount;
                    double amt = (Math.Round( Amount * ViewBag.Commission) / 100);
                    double Value = (double)Amount - amt;
                    double vendorCut = (Math.Round(Value * ViewBag.VendorCommission) / 100);
                    double asd = Math.Round(vendorCut, 0, MidpointRounding.AwayFromZero);
                    int total = data1.Count;
                    pageNumber = (int?)pageNumber ?? 1;
                    int pageSize = 10;
                    decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                    model.TotalPages = (int)noOfPages;
                    model.PageNumber = (int)pageNumber;
                    data1 = data1.OrderBy(a => a.Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                    if (name != null)
                    {
                        data1 = data1.Where(a => a.VendorName.ToLower().Contains(name)).ToList();
                    }
                    model.VendorCommissionReport = data1;
                    return View(model);
                }
            }

            var doctor = @"select Sum(P.Amount) as Amount, Count(P.Patient_Id) as Counts from dbo.PatientAppointment P join Doctor D ON d.Id = p.Doctor_Id  join Vendor v on v.Id = d.Vendor_Id where p.IsPaid=1 and v.Id=" + id + " and  P.AppointmentDate between DateAdd(DD,-7,GETDATE() ) and GETDATE()";
            var data = ent.Database.SqlQuery<VendorCommissionReport>(doctor).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record Of Current Week";
                return View(model);
            }
            else
            {
                ViewBag.Commission = commision;
                ViewBag.vendorCommission = vendorCommission;
                int total = data.Count;
                pageNumber = (int?)pageNumber ?? 1;
                int pageSize = 10;
                decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                model.TotalPages = (int)noOfPages;
                model.PageNumber = (int)pageNumber;
                data = data.OrderBy(a => a.Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                model.VendorCommissionReport = data;
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


        public ActionResult Vehicle(string term, DateTime? JoiningDate, string name = null)
        {
            int Id = GetVendorId();
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();
            double vendorCommission = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster where IsDeleted=0 and Department='Vendor' and Name='Vehicle'").FirstOrDefault();
            var model = new ReportDTO();
            if (JoiningDate != null)
            {
                DateTime dateCriteria = JoiningDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select COUNT(d.Id) as Counts, v.VendorName, v.CompanyName from Vehicle d join Vendor v on d.Vendor_Id = v.Id where d.JoiningDate between '" + dateCriteria + "' and '" + JoiningDate + "' group by v.VendorName, v.CompanyName";
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
                    //ViewBag.Payment = payment;
                    model.VendorCommissionReport = data1;
                    return View(model);
                }
            }
            var vehicle = @"select Sum(trm.Amount) as Amount, ve.VendorName, ve.CompanyName from Vehicle v
join Vendor ve on ve.Id = v.Vendor_Id
join TravelRecordMaster trm on trm.Vehicle_Id = v.Id
where trm.IsDriveCompleted = 1 and ve.Id= " + Id+" and trm.RequestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() group by  ve.VendorName, ve.CompanyName";
            var data = ent.Database.SqlQuery<VendorCommissionReport>(vehicle).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record Of Current Week";
                return View(model);
            }
            else
            {
                //ViewBag.Payment = payment;
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

        public ActionResult Nurse(DateTime? StartDate, DateTime? EndDate, string term = null)
        {
            int Id = GetVendorId();
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='HealthCheckUp'").FirstOrDefault();
            //double vendorCommission = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Vendor'").FirstOrDefault();
            double vendorCommission = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster where IsDeleted=0 and Department='Vendor' and Name='HealthCheckup'").FirstOrDefault();
            var model = new ReportDTO();
            if (StartDate != null && EndDate != null)
            {
                DateTime dateCriteria = StartDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                string q = @"select IsNull(Datediff(day,ns.StartDate,ns.EndDate)*ns.PerDayAmount,0) as Amount from NurseService ns join Nurse n on n.Id = ns.Nurse_Id join Vendor v on v.Id = n.Vendor_Id where v.Id=1 and ns.IsPaid=1 and ns.ServiceAcceptanceDate BETWEEN DateAdd(DD,-7,GETDATE() ) and GETDATE() and ServiceStatus='Approved'";
                var data1 = ent.Database.SqlQuery<VendorCommissionReport>(q).ToList();
                double? gfbdsjb = data1.Sum(a => a.Amount);
                return View(model);
            }
            var vehicle = @"select IsNull(Datediff(day,ns.StartDate,ns.EndDate)*ns.PerDayAmount,0) as Amount from NurseService ns join Nurse n on n.Id = ns.Nurse_Id join Vendor v on v.Id = n.Vendor_Id where v.Id=1 and ns.IsPaid=1 and ns.ServiceAcceptanceDate BETWEEN DateAdd(DD,-7,GETDATE() ) and GETDATE() and ServiceStatus='Approved'";
            var data = ent.Database.SqlQuery<VendorCommissionReport>(vehicle).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record Of Current Week";
                return View(model);
            }
            else
            {
                //ViewBag.Payment = payment;
                ViewBag.Commission = commision;
                //int total = data.Count;
                //pageNumber = (int?)pageNumber ?? 1;
                //int pageSize = 10;
                //decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                //model.TotalPages = (int)noOfPages;
                //model.PageNumber = (int)pageNumber;
                //data = data.OrderBy(a => a.Doctor_Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                double? gfbdsjb = data.Sum(a => a.Amount);
                ViewBag.vendorCommission = vendorCommission;
                ViewBag.Amount = (double)gfbdsjb;
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