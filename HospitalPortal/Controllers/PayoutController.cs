using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using iTextSharp.text.pdf;
using log4net.Util.TypeConverters;
using OfficeOpenXml;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Collections.Specialized.BitVector32;

namespace HospitalPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PayoutController : Controller
    {
        DbEntities ent = new DbEntities();

        public ActionResult PayOutHistory()
        {
            return View();
        }
        //Doctor PAYOUT History Section
        public ActionResult Doctor(DateTime? startdate, DateTime? enddate, string name = null)
        {
            var model = new PayOutVM();
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault();
            double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Doctor'").FirstOrDefault();
            double gst = ent.Database.SqlQuery<double>(@"select Amount from GSTMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault();
            double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault();
            if (startdate != null)
            {
                DateTime dateCriteria = startdate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");


                var qry1 = @"SELECT D.DoctorId, A.Doctor_Id, D.DoctorName, SUM(A.TotalFee) AS Amount 
FROM PatientAppointment A 
JOIN Doctor D ON D.Id = A.Doctor_Id  
WHERE A.IsPayoutPaid=0 AND A.AppointmentDate between @startdate and @enddate group by D.DoctorName, A.Doctor_Id,D.DoctorId";
                //var data1 = ent.Database.SqlQuery<PayOutDocHistroy>(qry1).ToList();
                var data1 = ent.Database.SqlQuery<PayOutDocHistroy>(qry1,
             new SqlParameter("startdate", startdate),
             new SqlParameter("enddate", enddate)).ToList();

                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {


                    ViewBag.Transactionfee = Transactionfee;
                    ViewBag.Amount = (double?)commision;
                    ViewBag.gstAmount = (double?)gst;
                    ViewBag.tdsAmount = (double?)tds;
                    model.PayHistory = data1;
                    foreach (var item in data1)
                    {
                        var razorcomm = (item.Amount * Transactionfee) / 100;
                        var totalrazorcomm = razorcomm;
                        item.Amountwithrazorpaycomm = item.Amount + totalrazorcomm;

                    }

                }
                return View(model);
            }
            //var qry = @"select A.Doctor_Id, D.DoctorName, SUM(A.Amount) as Amount, (SUM(A.Amount) - (SUM(A.Amount) * 7 /100)) As NetAmount from PatientAppointment A join Doctor D on D.Id = A.Doctor_Id where A.AppointmentDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() group by A.Amount, D.DoctorName, A.Doctor_Id";
            else
            {
                //var qry = @"select D.DoctorId,A.Doctor_Id, D.DoctorName, SUM(A.TotalFee) as Amount from PatientAppointment A join Doctor D on D.Id = A.Doctor_Id where A.AppointmentDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() group by D.DoctorName, A.Doctor_Id,D.DoctorId";
                var qry = @"SELECT D.DoctorId, A.Doctor_Id, D.DoctorName, SUM(A.TotalFee) AS Amount 
FROM PatientAppointment A 
JOIN Doctor D ON D.Id = A.Doctor_Id  
WHERE A.IsPayoutPaid=0 AND A.AppointmentDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE()
GROUP BY D.DoctorId, A.Doctor_Id, D.DoctorName;";
                var data = ent.Database.SqlQuery<PayOutDocHistroy>(qry).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Week";
                }
                else
                {
                    ViewBag.Transactionfee = Transactionfee;
                    ViewBag.Amount = (double?)commision;
                    ViewBag.gstAmount = (double?)gst;
                    ViewBag.tdsAmount = (double?)tds;
                    model.PayHistory = data;
                    foreach (var item in data)
                    {
                        var razorcomm = (item.Amount * Transactionfee) / 100;
                        var totalrazorcomm = razorcomm;
                        item.Amountwithrazorpaycomm = item.Amount + totalrazorcomm;


                    }
                    return View(model);
                }
                return View(model);
            }

        }

        public ActionResult PayDoctor(int? Doctor_Id, double? Amount, string multydocid)
        {
            if (!string.IsNullOrEmpty(multydocid))
            {
                string[] mulidoc = multydocid == null ? null : multydocid.Split('-');
                for (int i = 0; i < mulidoc.Length - 1; i++)
                {
                    string[] perdoc = mulidoc[i].Split(',');
                    int doctorid = Convert.ToInt32(perdoc[0]);
                    double doctoramount = Convert.ToDouble(perdoc[1]);
                    var model1 = new DoctorPayOut();
                    model1.Amount = doctoramount;
                    model1.IsPaid = true;
                    model1.IsGenerated = true;
                    model1.PaymentDate = DateTime.Now.Date;
                    model1.Doctor_Id = doctorid;
                    ent.DoctorPayOuts.Add(model1);
                    ent.SaveChanges();

                    //update payout status
                    var existdata = ent.PatientAppointments.Where(d => d.Doctor_Id == doctorid && d.IsPayoutPaid == false).ToList();
                    if (existdata != null)
                    {
                        foreach (var item in existdata)
                        {
                            item.IsPayoutPaid = true;
                            ent.SaveChanges();
                        }
                    }
                    else
                    {
                        TempData["msg"] = "Data not found.";
                    }

                }
                return RedirectToAction("DoctorList");
            }
            else
            {
                var model = new DoctorPayOut();
                model.Amount = (double)Amount;
                model.IsPaid = true;
                model.IsGenerated = true;
                model.PaymentDate = DateTime.Now.Date;
                model.Doctor_Id = (int)Doctor_Id;
                ent.DoctorPayOuts.Add(model);
                ent.SaveChanges();
                var existdata = ent.PatientAppointments.Where(d => d.Doctor_Id == Doctor_Id && d.IsPayoutPaid == false).FirstOrDefault();
                if (existdata != null)
                {
                    existdata.IsPayoutPaid = true;
                    ent.SaveChanges();

                }
                return RedirectToAction("ViewPayoutHistory", new { Id = model.Doctor_Id });

            }

        }
        public ActionResult ViewPayoutHistory(int Id, DateTime? date)
        {
            Session["msg"] = Id;
            var model = new ViewPayOutHistory();
            var Name = ent.Database.SqlQuery<string>("select DoctorName from Doctor where Id=" + Id).FirstOrDefault();
            model.DoctorName = Name;
            //string qry = @"select Dp.Id, ISNULL(Dp.IsPaid, 0) as IsPaid , Dp.IsGenerated, Dp.Doctor_Id, Dp.PaymentDate, Dp.Amount, D.DoctorName from  DoctorPayOut Dp join Doctor D on D.Id = Dp.Doctor_Id  where  Dp.Doctor_Id=" + Id;
            string qry = @"SELECT A.Id, D.DoctorId, A.Doctor_Id, D.DoctorName, A.TotalFee AS Amount ,p.Id,p.PatientName,p.PatientRegNo,p.MobileNumber,p.EmailId,a.PaymentDate,CONVERT (VARCHAR,A.AppointmentDate,107) AS AppointmentDate
FROM PatientAppointment A 
JOIN Doctor D ON D.Id = A.Doctor_Id 
join Patient as p on p.Id=A.Patient_Id
WHERE A.AppointmentDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() AND D.Id=" + Id;
            var data = ent.Database.SqlQuery<HistoryOfDoc_Payout>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            else
            {
                model.HistoryOfDoc_Payout = data;
            }
            return View(model);
        }
        public ActionResult UpdateStatus(int id)
        {
            int Id = Convert.ToInt32(Session["msg"]);
            string q = @"update DoctorPayOut set IsGenerated = case when IsGenerated=1 then 0 else 1 end where Id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("ViewPayoutHistory", new { Id = Id });
        }
        public ActionResult UpdatePayment(int id)
        {
            //int Id = Convert.ToInt32(Session["msg"].ToString());
            int Id = Convert.ToInt32(Session["msg"]);
            string q = @"update DoctorPayOut set IsPaid = case when IsPaid=1 then 0 else 1 end where Id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("ViewPayoutHistory", new { Id = Id });
        }
        //Lab Payout
        [HttpGet]
        public ActionResult Lab(DateTime? startdate, DateTime? enddate, string LabName = null)
        {
            var model = new PayOutVM();
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Lab'").FirstOrDefault();
            double gst = ent.Database.SqlQuery<double>(@"select Amount from GSTMaster where IsDeleted=0 and Name='Lab'").FirstOrDefault();
            double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Lab'").FirstOrDefault();
            double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Lab'").FirstOrDefault();
            if (startdate != null && enddate != null)
            {
                //DateTime dateCriteria = week.Value.AddDays(-7);
                //string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select D.lABId,A.Lab_Id, D.LabName, SUM(A.Amount) as Amount, (SUM(A.Amount) - (SUM(A.Amount) * 7 /100)) As NetAmount from BookTestLab A 
join Lab D on D.Id = A.Lab_Id 
WHERE A.IsPayoutPaid=0 AND A.TestDate between Convert(datetime,'" + startdate + "',103) and Convert(datetime,'" + enddate + "',103) GROUP BY  D.LabName, A.Lab_Id,D.lABId";
                var data1 = ent.Database.SqlQuery<PayOutLabHistoty>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                    return View(model);
                }
                else
                {
                    ViewBag.Transactionfee = Transactionfee;
                    ViewBag.Amount = (double?)commision;
                    ViewBag.gstAmount = (double?)gst;
                    ViewBag.tdsAmount = (double?)tds;
                    model.LabHistory = data1;
                    foreach (var item in data1)
                    {
                        var razorcomm = (item.Amount * Transactionfee) / 100;
                        var totalrazorcomm = razorcomm;
                        item.Amountwithrazorpaycomm = item.Amount + totalrazorcomm;

                    }
                    return View(model);
                }
            }
            else
            {
                var qry = @"select D.lABId,A.Lab_Id, D.LabName, SUM(A.Amount) as Amount, (SUM(A.Amount) - (SUM(A.Amount) * 7 /100)) As NetAmount from BookTestLab A 
join Lab D on D.Id = A.Lab_Id 
WHERE A.IsPayoutPaid=0 AND Convert(Date,A.TestDate) between DATEADD(DAY, -7, GETDATE()) AND GETDATE() GROUP BY  D.LabName, A.Lab_Id,D.lABId";
                var data = ent.Database.SqlQuery<PayOutLabHistoty>(qry).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Week";
                    return View(model);
                }
                else
                {
                    if (LabName != null)
                    {
                        data = data.Where(a => a.LabName.Contains(LabName)).ToList();
                        if (data.Count() <= 0)
                        {
                            TempData["msg"] = "No Record Found";
                            return View(model);
                        }
                    }
                    ViewBag.Transactionfee = Transactionfee;
                    ViewBag.Amount = commision;
                    ViewBag.gstAmount = (double?)gst;
                    ViewBag.tdsAmount = (double?)tds;
                    model.LabHistory = data;
                    foreach (var item in data)
                    {
                        var razorcomm = (item.Amount * Transactionfee) / 100;
                        var totalrazorcomm = razorcomm;
                        item.Amountwithrazorpaycomm = item.Amount + totalrazorcomm;

                    }
                    return View(model);
                }
            }

        }
        private int GetLabId()
        {
            int loginId = Convert.ToInt32(User.Identity.Name);
            int labId = ent.Database.SqlQuery<int>("select Id from Lab where AdminLogin_Id=" + loginId).FirstOrDefault();
            return labId;
        }
        //Lab PAYOUT History Section        
        public ActionResult LabPay(int? Lab_Id, double? Amount, string multyid)
        {
            var model = new LabPayOut();
            if (!string.IsNullOrEmpty(multyid))
            {
                string[] mulidoc = multyid == null ? null : multyid.Split('-');
                for (int i = 0; i < mulidoc.Length - 1; i++)
                {
                    string[] perdoc = mulidoc[i].Split(',');
                    int labid = Convert.ToInt32(perdoc[0]);
                    double amount = Convert.ToDouble(perdoc[1]);
                    var model1 = new LabPayOut();
                    model1.Amount = amount;
                    model1.IsPaid = true;
                    model1.IsGenerated = true;
                    model1.PaymentDate = DateTime.Now.Date;
                    model1.Lab_Id = labid;
                    ent.LabPayOuts.Add(model1);
                    ent.SaveChanges();

                    var existdata = ent.BookTestLabs.Where(d => d.Lab_Id == labid && d.IsPayoutPaid == false).ToList();
                    if (existdata != null)
                    {
                        foreach (var item in existdata)
                        {
                            if (item.IsPayoutPaid == false)
                            {
                                item.IsPayoutPaid = true;
                                ent.SaveChanges();
                            }

                        }
                    }
                    else
                    {
                        TempData["msg"] = "Data not found.";
                    }

                }
                return RedirectToAction("LabList");
            }
            else
            {
                model.Lab_Id = Lab_Id;
                model.Amount = Amount;
                model.IsPaid = true;
                model.IsGenerated = true;
                model.PaymentDate = DateTime.Now.Date;
                ent.LabPayOuts.Add(model);
                ent.SaveChanges();
                return RedirectToAction("ViewLabPayoutHistory", new { Id = Lab_Id });
            }
        }
        public ActionResult ViewLabPayoutHistory(int Id)
        {
            Session["msg"] = Id;
            var model = new ViewPayOutHistory();
            var Name = ent.Database.SqlQuery<string>("select LabName from Lab where Id=" + Id).FirstOrDefault();
            model.LabName = Name;
            //string qry = @"select Dp.Id, ISNULL(Dp.IsPaid, 0) as IsPaid , Dp.IsGenerated, Dp.Lab_Id, Dp.PaymentDate, Dp.Amount, D.LabName from  LabPayOut Dp join Lab D on D.Id = Dp.Lab_Id  where  Dp.Lab_Id=" + Id;
            string qry = @"select btl.Id, l.lABId, btl.Lab_Id, l.LabName, btl.Amount ,p.Id,p.PatientName,p.PatientRegNo,p.MobileNumber,p.EmailId,btl.PaymentDate,btl.TestDate from BookTestLab as btl
join Lab l on l.Id = btl.Lab_Id  
join Patient as p on p.Id=btl.Patient_Id
WHERE btl.TestDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() AND l.Id=" + Id;
            var data = ent.Database.SqlQuery<HistoryOfLab_Payout>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            model.HistoryOfLab_Payout = data;
            return View(model);
        }
        public ActionResult UpdateLabStatus(int id)
        {
            int Id = Convert.ToInt32(Session["msg"]);
            string q = @"update LabPayOut set IsGenerated = case when IsGenerated=1 then 0 else 1 end where Id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("ViewLabPayoutHistory", new { Id = Id });
        }
        public ActionResult UpdateLabPayment(int id)
        {
            int Id = Convert.ToInt32(Session["msg"]);
            string q = @"update LabPayOut set IsPaid = case when IsPaid=1 then 0 else 1 end where Id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("ViewLabPayoutHistory", new { Id = Id });
        }
        //Health CheckUp PayOut
        public ActionResult Health(DateTime? week, string name = null)
        {
            var model = new PayOutVM();
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Health'").FirstOrDefault();
            double gst = ent.Database.SqlQuery<double>(@"select Amount from GSTMaster where IsDeleted=0 and Name='Health'").FirstOrDefault();
            double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Health'").FirstOrDefault();
            if (week != null)
            {
                DateTime dateCriteria = week.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select A.Center_Id, D.LabName, hp.IsGenerated, SUM(A.Amount) as Amount,(SUM(A.Amount) - (SUM(A.Amount) * 7 /100)) As NetAmount from CmpltCheckUp A join HealthCheckupCenter D on D.Id = A.Center_Id left join HealthPayOut hp on D.Id = hp.Health_Id where A.TestDate between CONVERT(datetime, '" + dateCriteria + "', 103) and CONVERT(datetime, '" + week + "', 103) and hp.IsGenerated Is Null GROUP BY D.LabName, A.Center_Id,hp.IsGenerated";
                var data1 = ent.Database.SqlQuery<PayOutHealthHistoty>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    if (name != null)
                    {
                        data1 = data1.Where(a => a.LabName.ToLower().Contains(name)).ToList();
                    }
                    ViewBag.Amount = commision;
                    ViewBag.gstAmount = (double?)gst;
                    ViewBag.tdsAmount = (double?)tds;
                    model.HealthHistory = data1;
                    return View(model);
                }
            }
            var qry = @"select A.Center_Id, D.LabName, hp.IsGenerated, SUM(A.Amount) as Amount,(SUM(A.Amount) - (SUM(A.Amount) * 7 /100)) As NetAmount from CmpltCheckUp A join HealthCheckupCenter D on D.Id = A.Center_Id left join HealthPayOut hp on D.Id = hp.Health_Id where A.TestDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() and hp.IsGenerated Is Null group by D.LabName, A.Center_Id,hp.IsGenerated";
            var data = ent.Database.SqlQuery<PayOutHealthHistoty>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Week";
            }
            else
            {
                if (name != null)
                {
                    data = data.Where(a => a.LabName.ToLower().Contains(name)).ToList();
                }
                ViewBag.Amount = commision;
                ViewBag.gstAmount = (double?)gst;
                ViewBag.tdsAmount = (double?)tds;
                model.HealthHistory = data;
                return View(model);
            }


            return View(model);
        }
        public ActionResult HealthPay(int? Health_Id, double Amount)
        {
            var model = new HealthPayOut();
            //int labId = ent.Database.SqlQuery<int>("select AdminLogin_Id from Lab where Id=" + Lab_Id).FirstOrDefault();
            //var Id = labId;
            model.Health_Id = Health_Id;
            model.Amount = Amount;
            model.IsPaid = false;
            model.IsGenerated = true;
            model.PaymentDate = DateTime.Now.Date;
            ent.HealthPayOuts.Add(model);
            ent.SaveChanges();
            return RedirectToAction("ViewHealthPayoutHistory", new { Id = model.Id });
        }
        public ActionResult ViewHealthPayoutHistory(int Id)
        {
            var model = new ViewPayOutHistory();
            var Name = ent.Database.SqlQuery<string>("select LabName from HealthCheckupCenter where Id=" + Id).FirstOrDefault();
            model.LabName = Name;
            string qry = @"select Dp.Id, ISNULL(Dp.IsPaid, 0) as IsPaid , Dp.IsGenerated, Dp.Health_Id, Dp.PaymentDate, Dp.Amount, D.LabName from  HealthPayOut Dp join HealthCheckupCenter D on D.Id = Dp.Health_Id  where  Dp.Health_Id=" + Id;
            var data = ent.Database.SqlQuery<HistoryOfHealth_Payout>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            else
            {
                model.HistoryOfHealth_Payout = data;
            }
            return View(model);
        }
        public ActionResult UpdateHealthStatus(int id)
        {
            int Id = Convert.ToInt32(Session["msg"]);
            string q = @"update HealthPayOut set IsGenerated = case when IsGenerated=1 then 0 else 1 end where Id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("ViewHealthPayoutHistory", new { Id = Id });
        }
        public ActionResult UpdateHealthPayment(int id)
        {
            int Id = Convert.ToInt32(Session["msg"]);
            string q = @"update HealthPayOut set IsPaid = case when IsPaid=1 then 0 else 1 end where Id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("ViewHealthPayoutHistory", new { Id = Id });
        }
        //Chemist PayOut
        public ActionResult Chemist(DateTime? week)
        {
            var model = new PayOutVM();
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Chemist'").FirstOrDefault();
            double gst = ent.Database.SqlQuery<double>(@"select Amount from GSTMaster where IsDeleted=0 and Name='Chemist'").FirstOrDefault();
            double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Chemist'").FirstOrDefault();
            // var qry = @"SELECT C.ChemistName, O.OrderDate, cp.IsGenerated, O.Chemist_Id, Sum(A.Amount) as Amount, (SUM(A.Amount) - (SUM(A.Amount) * 7 /100)) As Amount from MedicineOrderDetail A join MedicineOrder O on A.Order_Id = O.Id join Chemist C on C.Id = O.Chemist_Id left join ChemistPayOut cp on C.Id = cp.Chemist_Id where O.OrderDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE()  and O.IsPaid=1 and cp.IsGenerated Is Null group by C.ChemistName,  O.OrderDate, O.Chemist_Id, cp.IsGenerated";
            var qry = @"SELECT C.ChemistId,C.ChemistName, O.OrderDate, cp.IsGenerated, O.Chemist_Id, Sum(A.Amount) as Amount, (SUM(A.Amount) - (SUM(A.Amount) * 7 /100)) As Amount from MedicineOrderDetail A left join MedicineOrder O on A.Order_Id = O.Id left join Chemist C on C.Id = O.Chemist_Id left join ChemistPayOut cp on C.Id = cp.Chemist_Id where O.OrderDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE()  and O.IsPaid=1 group by C.ChemistName,  O.OrderDate, O.Chemist_Id, cp.IsGenerated,C.ChemistId";
            var data = ent.Database.SqlQuery<PayOutChemistHistoty>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Week";
            }
            else
            {
                ViewBag.Amount = commision;
                ViewBag.gstAmount = (double?)gst;
                ViewBag.tdsAmount = (double?)tds;
                model.ChemistHistory = data;
                foreach (var item in data)
                {
                    var razorcomm = item.Amount * (2.36 / 100);
                    // var razorcommafter = razorcomm * 2.36 / 100;
                    var totalrazorcomm = razorcomm;
                    item.Amountwithrazorpaycomm = item.Amount + totalrazorcomm;


                }
                return View(model);
            }

            if (week != null)
            {
                DateTime dateCriteria = week.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"SELECT C.ChemistId,C.ChemistName, O.Chemist_Id, Sum(A.Amount) as Amount, (SUM(A.Amount) - (SUM(A.Amount) * 7 /100)) As Amount from MedicineOrderDetail A join MedicineOrder O on A.Order_Id = O.Id join Chemist C on C.Id = O.Chemist_Id where O.OrderDate between Convert(datetime,'" + dateCriteria + "',103) and '" + week + "' GROUP BY C.ChemistName, O.Chemist_Id,C.ChemistId";
                var data1 = ent.Database.SqlQuery<PayOutChemistHistoty>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Amount = commision;
                    ViewBag.gstAmount = (double?)gst;
                    ViewBag.tdsAmount = (double?)tds;
                    model.ChemistHistory = data1;
                    foreach (var item in data1)
                    {
                        var razorcomm = item.Amount * (2.36 / 100);
                        // var razorcommafter = razorcomm * 2.36 / 100;
                        var totalrazorcomm = razorcomm;
                        item.Amountwithrazorpaycomm = item.Amount + totalrazorcomm;


                    }
                    return View(model);
                }
            }
            return View(model);
        }
        public ActionResult ChemistPay(int Chemist_Id, double Amount)
        {
            var model = new ChemistPayOut();
            //int labId = ent.Database.SqlQuery<int>("select AdminLogin_Id from Lab where Id=" + Lab_Id).FirstOrDefault();
            //var Id = labId;
            model.Chemist_Id = Chemist_Id;
            model.Amount = Amount;
            model.IsPaid = false;
            model.IsGenerated = true;
            model.PaymentDate = DateTime.Now.Date;
            ent.ChemistPayOuts.Add(model);
            ent.SaveChanges();
            return RedirectToAction("ViewChemistPayoutHistory", new { Id = Chemist_Id });
        }
        public ActionResult ViewChemistPayoutHistory(int Id)
        {
            Session["msg"] = Id;
            var model = new ViewPayOutHistory();
            var Name = ent.Database.SqlQuery<string>("select ChemistName from Chemist where Id=" + Id).FirstOrDefault();
            model.ChemistName = Name;
            string qry = @"select Dp.Id, ISNULL(Dp.IsPaid, 0) as IsPaid , Dp.IsGenerated, Dp.Chemist_Id, Dp.PaymentDate, Dp.Amount, D.ChemistName from  ChemistPayOut Dp join Chemist D on D.Id = Dp.Chemist_Id  where  Dp.Chemist_Id=" + Id;
            var data = ent.Database.SqlQuery<HistoryOfChemist_Payout>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            else
            {
                model.HistoryOfChemist_Payout = data;
            }
            return View(model);
        }
        public ActionResult UpdateChemistStatus(int id)
        {
            int Id = Convert.ToInt32(Session["msg"]);
            string q = @"update ChemistPayOut set IsGenerated = case when IsGenerated=1 then 0 else 1 end where Id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("ViewChemistPayoutHistory", new { Id = Id });
        }
        public ActionResult UpdateChemistPayment(int id)
        {
            int Id = Convert.ToInt32(Session["msg"]);
            string q = @"update ChemistPayOut set IsPaid = case when IsPaid=1 then 0 else 1 end where Id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("ViewChemistPayoutHistory", new { Id = Id });
        }
        //Nurse Payout
        public ActionResult Nurse(DateTime? startdate, DateTime? enddate, string name)
        {
            var model = new PayOutVM();
            model.NurseTypeList = new SelectList(ent.NurseTypes.ToList(), "Id", "NurseTypeName");
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Nurse'").FirstOrDefault();
            double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Nurse'").FirstOrDefault();
            double gst = ent.Database.SqlQuery<double>(@"select Amount from GSTMaster where IsDeleted=0 and Name='Nurse'").FirstOrDefault();
            double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Nurse'").FirstOrDefault();

            //var NurseQuery = @"select P.NurseId,P.Id, P.NurseName, d.NurseTypeName from Nurse P join NurseType D ON d.Id = p.NurseType_Id join NurseService ns on ns.Nurse_Id = P.Id where ns.ServiceAcceptanceDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() and ns.ServiceStatus='Approved' group by  P.NurseName, d.NurseTypeName,P.Id,P.NurseId";

            if (startdate != null)
            {
                DateTime dateCriteria = startdate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");

                var qry1 = @"select n.NurseId,ns.Nurse_Id,n.NurseName, nt.NurseTypeName, ns.Id, case when ns.PaymentDate is null then 'N/A' else Convert(nvarchar(100),
ns.PaymentDate, 103) end as PaymentDate, case when ns.ServiceAcceptanceDate is null then 'N/A' else Convert(nvarchar(100), 
ns.ServiceAcceptanceDate, 103) end as ServiceAcceptanceDate, Convert(nvarchar(100), ns.RequestDate, 103) as RequestDate,
'From ' + Convert(nvarchar(100), ns.StartDate, 103) + ' to ' + Convert(nvarchar(100), ns.EndDate, 103) as ServiceTiming ,
IsNull(n.NurseName, 'N/A') as NurseName, Sum(ns.TotalFee) as Amount from NurseService ns 
join Nurse n on ns.Nurse_Id = n.Id
join NurseType nt ON nt.Id = n.NurseType_Id  
WHERE ns.IsPayoutPaid=0 and ns.ServiceAcceptanceDate between @startdate and @enddate and ns.ServiceStatus='Approved' and ns.IsPaid = 1 GROUP BY n.NurseId,ns.Nurse_Id,n.NurseName, nt.NurseTypeName, ns.Id,ns.ServiceAcceptanceDate,ns.RequestDate,ns.PaymentDate,ns.StartDate,ns.EndDate";

                var data1 = ent.Database.SqlQuery<PayOutNurseHistory>(qry1,
             new SqlParameter("startdate", startdate),
             new SqlParameter("enddate", enddate)).ToList();
                if (name != null)
                {
                    data1 = data1.Where(a => a.NurseName.ToLower().Contains(name)).ToList();
                }
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {


                    ViewBag.Transactionfee = Transactionfee;
                    ViewBag.Amount = (double?)commision;
                    ViewBag.gstAmount = (double?)gst;
                    ViewBag.tdsAmount = (double?)tds;
                    model.NurseHistory = data1;
                    foreach (var item in data1)
                    {
                        var razorcomm = (item.Amount * Transactionfee) / 100;
                        var totalrazorcomm = razorcomm;
                        item.Amountwithrazorpaycomm = item.Amount + totalrazorcomm;

                    }

                }
                return View(model);
            }
            else
            {
                var NurseQuery = @"select n.NurseId,ns.Nurse_Id,n.NurseName, nt.NurseTypeName, ns.Id, case when ns.PaymentDate is null then 'N/A' else Convert(nvarchar(100),
ns.PaymentDate, 103) end as PaymentDate, case when ns.ServiceAcceptanceDate is null then 'N/A' else Convert(nvarchar(100), 
ns.ServiceAcceptanceDate, 103) end as ServiceAcceptanceDate, Convert(nvarchar(100), ns.RequestDate, 103) as RequestDate,
'From ' + Convert(nvarchar(100), ns.StartDate, 103) + ' to ' + Convert(nvarchar(100), ns.EndDate, 103) as ServiceTiming ,
IsNull(n.NurseName, 'N/A') as NurseName, Sum(ns.TotalFee) as Amount from NurseService ns 
join Nurse n on ns.Nurse_Id = n.Id
join NurseType nt ON nt.Id = n.NurseType_Id  
WHERE ns.IsPayoutPaid=0 and ns.ServiceAcceptanceDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() and ns.ServiceStatus='Approved' and ns.IsPaid = 1 GROUP BY n.NurseId,ns.Nurse_Id,n.NurseName, nt.NurseTypeName, ns.Id,ns.ServiceAcceptanceDate,ns.RequestDate,ns.PaymentDate,ns.StartDate,ns.EndDate";
                var data = ent.Database.SqlQuery<PayOutNurseHistory>(NurseQuery).ToList();


                ViewBag.Transactionfee = Transactionfee;
                ViewBag.Amount = (double?)commision;
                ViewBag.gstAmount = (double?)gst;
                ViewBag.tdsAmount = (double?)tds;

                model.NurseHistory = data;
                foreach (var item in data)
                {
                    var razorcomm = (item.Amount * Transactionfee) / 100;
                    var totalrazorcomm = razorcomm;
                    item.Amountwithrazorpaycomm = item.Amount + totalrazorcomm;


                }
                return View(model);
            }


        }
        public ActionResult NurseDetails(int? NurseId, DateTime? ServiceAcceptanceDate)
        {
            var model = new NurseCommissionReports();
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Nurse'").FirstOrDefault();
            double gst = ent.Database.SqlQuery<double>(@"select Amount from GSTMaster where IsDeleted=0 and Name='Nurse'").FirstOrDefault();
            double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Nurse'").FirstOrDefault();
            //            string q = @"select ns.Nurse_Id, ns.Id, ns.ServiceStatus, ns.IsPaid, np.IsGenerated, case when ns.PaymentDate is null then 'N/A' else Convert(nvarchar(100),ns.PaymentDate,103) end as PaymentDate, case when ns.ServiceAcceptanceDate is null then 'N/A' else Convert(nvarchar(100),ns.ServiceAcceptanceDate,103) end as ServiceAcceptanceDate, Convert(nvarchar(100),ns.RequestDate,103) as RequestDate,'From '+ Convert(nvarchar(100),ns.StartDate,103)+' to '+Convert(nvarchar(100),ns.EndDate,103) as ServiceTiming ,IsNull(n.NurseName,'N/A') as NurseName,
            //IsNull(n.MobileNumber,'N/A') as NurseMobileNumber,
            //ns.TotalFee
            // from NurseService ns 
            //join Nurse n on ns.Nurse_Id=n.Id
            //left join NursePayOut np on np.Nurse_Id = n.Id
            //where ns.Nurse_Id = " + NurseId + " and ns.ServiceAcceptanceDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() and ns.ServiceStatus='Approved' and ns.IsPaid=1 and np.IsGenerated Is Null order by ns.Id desc";

            //            string q = @"select ns.Nurse_Id, ns.Id, ns.ServiceStatus, ns.IsPaid, np.IsGenerated, case when ns.PaymentDate is null then 'N/A' else Convert(nvarchar(100),ns.PaymentDate,103) end as PaymentDate, case when ns.ServiceAcceptanceDate is null then 'N/A' else Convert(nvarchar(100),ns.ServiceAcceptanceDate,103) end as ServiceAcceptanceDate, Convert(nvarchar(100),ns.RequestDate,103) as RequestDate,'From '+ Convert(nvarchar(100),ns.StartDate,103)+' to '+Convert(nvarchar(100),ns.EndDate,103) as ServiceTiming ,IsNull(n.NurseName,'N/A') as NurseName, IsNull(n.MobileNumber,'N/A') as NurseMobileNumber, ns.TotalFee from NurseService ns 
            //join Nurse n on ns.Nurse_Id=n.Id 
            //left join NursePayOut np on np.Nurse_Id = n.Id 
            //where ns.Nurse_Id = " + NurseId + " and ns.ServiceAcceptanceDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() and ns.ServiceStatus='Approved' and ns.IsPaid=1 order by ns.Id desc";
            string q = @"select ns.Nurse_Id, ns.Id, ns.ServiceStatus, ns.IsPaid, case when ns.PaymentDate is null then 'N/A' else Convert(nvarchar(100), ns.PaymentDate, 103) end as PaymentDate, case when ns.ServiceAcceptanceDate is null then 'N/A' else Convert(nvarchar(100), ns.ServiceAcceptanceDate, 103) end as ServiceAcceptanceDate, Convert(nvarchar(100), ns.RequestDate, 103) as RequestDate,'From ' + Convert(nvarchar(100), ns.StartDate, 103) + ' to ' + Convert(nvarchar(100), ns.EndDate, 103) as ServiceTiming ,IsNull(n.NurseName, 'N/A') as NurseName, IsNull(n.MobileNumber, 'N/A') as NurseMobileNumber, ns.TotalFee from NurseService ns join Nurse n on ns.Nurse_Id = n.Id where ns.ServiceAcceptanceDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() and ns.ServiceStatus='Approved' and ns.IsPaid = 1 and ns.Nurse_Id=" + NurseId + " order by ns.Id desc";

            var data = ent.Database.SqlQuery<NurseAppointmentList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "Something Went Wrong.";
            }
            else
            {
                ViewBag.Amount = commision;
                ViewBag.gstAmount = (double?)gst;
                ViewBag.tdsAmount = (double?)tds;
                model.NurseName = data.FirstOrDefault().NurseName;
                model.NurseId = data.FirstOrDefault().Nurse_Id;
                model.NurseAppointmentList = data;
                foreach (var item in data)
                {
                    var razorcomm = item.TotalFee * (2.36 / 100);
                    // var razorcommafter = razorcomm * 2.36 / 100;
                    var totalrazorcomm = razorcomm;
                    item.Amountwithrazorpaycomm = item.TotalFee + totalrazorcomm;

                }
            }
            return View(model);
        }
        public ActionResult NursePay(int? Nurse_Id, double? Amount, string multyid)
        {
            if (!string.IsNullOrEmpty(multyid))
            {
                string[] mulidoc = multyid == null ? null : multyid.Split('-');
                for (int i = 0; i < mulidoc.Length - 1; i++)
                {
                    string[] perdoc = mulidoc[i].Split(',');
                    int nurseid = Convert.ToInt32(perdoc[0]);
                    double amount = Convert.ToDouble(perdoc[1]);
                    var model = new NursePayout();
                    model.Amount = amount;
                    model.IsPaid = true;
                    model.IsGenerated = true;
                    model.PaymentDate = DateTime.Now.Date;
                    model.Nurse_Id = nurseid;
                    ent.NursePayouts.Add(model);
                    ent.SaveChanges();

                    var existdata = ent.NurseServices.Where(d => d.Nurse_Id == nurseid && d.IsPayoutPaid == false).ToList();
                    if (existdata != null)
                    {
                        foreach (var item in existdata)
                        {
                            if (item.IsPayoutPaid == false)
                            {
                                item.IsPayoutPaid = true;
                                ent.SaveChanges();
                            }

                        }
                    }
                    else
                    {
                        TempData["msg"] = "Data not found.";
                    }

                }
                return RedirectToAction("NurseList");
            }
            else
            {
                var model = new NursePayout();
                model.Nurse_Id = Nurse_Id;
                model.Amount = Amount;
                model.IsPaid = true;
                model.IsGenerated = true;
                model.PaymentDate = DateTime.Now.Date;
                ent.NursePayouts.Add(model);
                ent.SaveChanges();
                return RedirectToAction("ViewNursePayoutHistory", new { Id = Nurse_Id });
            }


        }
        public ActionResult ViewNursePayoutHistory(int Id)
        {
            var model = new ViewPayOutHistory();
            var Name = ent.Database.SqlQuery<string>("select NurseName from Nurse where Id=" + Id).FirstOrDefault();
            model.ChemistName = Name;
            string qry = @"select Dp.Id, ISNULL(Dp.IsPaid, 0) as IsPaid , Dp.IsGenerated, Dp.Nurse_Id, Dp.PaymentDate, Dp.Amount, D.NurseName from  NursePayOut Dp join Nurse D on D.Id = Dp.Nurse_Id  where  Dp.Nurse_Id=" + Id;
            var data = ent.Database.SqlQuery<HistoryOfNurse_Payout>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            else
            {
                model.HistoryOfNurse_Payout = data;
                Session["msg"] = data.FirstOrDefault().Nurse_Id;
            }
            return View(model);
        }
        public ActionResult UpdateNurseStatus(int id)
        {
            int Id = Convert.ToInt32(Session["msg"]);
            string q = @"update NursePayOut set IsGenerated = case when IsGenerated=1 then 0 else 1 end where Id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("ViewNursePayoutHistory", new { Id = Id });
        }
        public ActionResult UpdateNursePayment(int id)
        {
            int Id = Convert.ToInt32(Session["msg"]);
            string q = @"update NursePayOut set IsPaid = case when IsPaid=1 then 0 else 1 end where Id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("ViewNursePayoutHistory", new { Id = Id });
        }
        public ActionResult Ambulance(DateTime? startdate, DateTime? enddate)
        {
            var model = new AmbulancesReport();
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();
            double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Ambulance'").FirstOrDefault();
            double gst = ent.Database.SqlQuery<double>(@"select Amount from GSTMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();
            double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();

            if (startdate != null)
            {
                var qry1 = @"select d.DriverId,trm.Driver_Id,v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,v.Id as VehicleId, d.DriverName,
Sum(trm.TotalPrice) as TotalPrice from DriverLocation trm 
join Driver d on d.Id = trm.Driver_Id 
join Vehicle v on v.Id = d.Vehicle_Id 
join Patient p on p.Id = trm.PatientId 
WHERE trm.IsPayoutPaid=0 and trm.EntryDate between Convert(datetime,'" + startdate + "',103) and Convert(datetime,'" + enddate + "',103) group by v.VehicleNumber, v.VehicleName, v.Id,d.DriverName,trm.Driver_Id,d.DriverId";
                var data1 = ent.Database.SqlQuery<Ambulance>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Transactionfee = Transactionfee;
                    ViewBag.Amount = (double?)commision;
                    ViewBag.gstAmount = (double?)gst;
                    ViewBag.tdsAmount = (double?)tds;
                    model.Ambulance = data1;
                    foreach (var item in data1)
                    {

                        var razorcomm = ((double?)item.TotalPrice * Transactionfee) / 100;
                        var totalrazorcomm = razorcomm;
                        item.Amountwithrazorpaycomm = (double?)item.TotalPrice + totalrazorcomm;

                    }
                }
                return View(model);
            }
            else
            {
                var qry = @"select d.DriverId,trm.Driver_Id,v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,v.Id as VehicleId, d.DriverName,
Sum(trm.TotalPrice) as TotalPrice from DriverLocation trm 
join Driver d on d.Id = trm.Driver_Id 
join Vehicle v on v.Id = d.Vehicle_Id 
join Patient p on p.Id = trm.PatientId 
WHERE trm.IsPayoutPaid=0 and trm.EntryDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() and trm.IsPay='Y' group by v.VehicleNumber, v.VehicleName, v.Id,d.DriverName,trm.Driver_Id,d.DriverId";
                var data = ent.Database.SqlQuery<Ambulance>(qry).ToList();

                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record Available.";
                }
                else
                {
                    ViewBag.Transactionfee = Transactionfee;
                    ViewBag.Amount = (double?)commision;
                    ViewBag.gstAmount = (double?)gst;
                    ViewBag.tdsAmount = (double?)tds;
                    model.Ambulance = data;
                    foreach (var item in data)
                    {
                        var razorcomm = ((double?)item.TotalPrice * Transactionfee) / 100;
                        var totalrazorcomm = razorcomm;
                        item.Amountwithrazorpaycomm = (double?)item.TotalPrice + totalrazorcomm;
                    }
                }
                return View(model);
            }

        }


        public ActionResult PayAmbulance_Driver(int? Driver_Id, double? Amount, string multyid)
        {

            if (!string.IsNullOrEmpty(multyid))
            {
                string[] multi = multyid == null ? null : multyid.Split('-');
                for (int i = 0; i < multi.Length - 1; i++)
                {
                    string[] perdoc = multi[i].Split(',');
                    int driverid = Convert.ToInt32(perdoc[0]);
                    double amount = Convert.ToDouble(perdoc[1]);
                    var model1 = new DriverPayOut();
                    model1.Amount = amount;
                    model1.IsPaid = true;
                    model1.IsGenerated = true;
                    model1.PaymentDate = DateTime.Now.Date;
                    model1.Driver_Id = driverid;
                    ent.DriverPayOuts.Add(model1);
                    ent.SaveChanges();

                    var existdata = ent.DriverLocations.Where(d => d.Driver_Id == driverid && d.IsPayoutPaid == false).ToList();
                    if (existdata != null)
                    {
                        foreach (var item in existdata)
                        {
                            if (item.IsPayoutPaid == false)
                            {
                                item.IsPayoutPaid = true;
                                ent.SaveChanges();
                            }

                        }
                    }
                    else
                    {
                        TempData["msg"] = "Data not found.";
                    }


                }
                return RedirectToAction("AmbulanceList");
            }
            else
            {
                var model = new DriverPayOut();
                model.Amount = (double)Amount;
                model.IsPaid = true;
                model.IsGenerated = true;
                model.PaymentDate = DateTime.Now.Date;
                model.Driver_Id = (int)Driver_Id;
                ent.DriverPayOuts.Add(model);
                ent.SaveChanges();
                return RedirectToAction("ViewDriver_AmbulancePayoutHistory", new { Id = model.Driver_Id });
            }

        }

        public ActionResult Driver(DateTime? startdate, DateTime? enddate)
        {
            var model = new AmbulancesReport();
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();
            double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Ambulance'").FirstOrDefault();
            double gst = ent.Database.SqlQuery<double>(@"select Amount from GSTMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();
            double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();

            if (startdate != null)
            {
                var qry1 = @"SELECT d.DriverId,trm.Driver_Id,v.VehicleNumber,ISNULL(v.VehicleName,'NA') AS VehicleName,v.Id AS VehicleId,d.DriverName,SUM(trm.TotalPrice) AS TotalPrice,vah.AllocateDate,vah.Vehicle_Id,vah.IsActive FROM DriverLocation trm 
JOIN Driver d ON d.Id = trm.Driver_Id 
JOIN Vehicle v ON v.Id = d.Vehicle_Id 
JOIN VehicleAllotHistory AS vah ON vah.Driver_Id = d.Id
WHERE trm.IsPayoutPaid = 0 and trm.EntryDate between Convert(datetime,'" + startdate + "',103) and Convert(datetime,'" + enddate + "',103) AND trm.IsPay = 'Y' GROUP BY v.VehicleNumber,v.VehicleName,v.Id,d.DriverName,trm.Driver_Id,d.DriverId,vah.AllocateDate,vah.Vehicle_Id,vah.IsActive;";
                var data1 = ent.Database.SqlQuery<Ambulance>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Transactionfee = Transactionfee;
                    ViewBag.Amount = (double?)commision;
                    ViewBag.gstAmount = (double?)gst;
                    ViewBag.tdsAmount = (double?)tds;
                    model.Ambulance = data1;
                    foreach (var item in data1)
                    {

                        var razorcomm = ((double?)item.TotalPrice * Transactionfee) / 100;
                        var totalrazorcomm = razorcomm;
                        item.Amountwithrazorpaycomm = (double?)item.TotalPrice + totalrazorcomm;

                    }
                }
                return View(model);
            }
            else
            {
                //AND trm.EntryDate BETWEEN DATEADD(DD, -7, GETDATE()) AND GETDATE()
                var qry = @"select d.DriverId,trm.Driver_Id,v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,v.Id as VehicleId, d.DriverName,
Sum(trm.TotalPrice) as TotalPrice from DriverLocation trm 
join Driver d on d.Id = trm.Driver_Id 
join VehicleAllotHistory as vah on vah.Driver_Id=d.Id
left join Vehicle v on v.Id = vah.Vehicle_Id 
join Patient p on p.Id = trm.PatientId
WHERE trm.IsPayoutPaid=0 and trm.EntryDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() and trm.IsPay='Y' group by v.VehicleNumber, v.VehicleName, v.Id,d.DriverName,trm.Driver_Id,d.DriverId";
                var data = ent.Database.SqlQuery<Ambulance>(qry).ToList();

                var qry1 = @"select *, Vehicle_Id as VehicleId from VehicleAllotHistory";
                var data1 = ent.Database.SqlQuery<Ambulance>(qry1).ToList();

                var groupByVehicle = data1.GroupBy(x => x.VehicleId);
                
                foreach (var vehicleGroup in groupByVehicle)
                {
                    var vehicleId = vehicleGroup.Key;
                    var groupdriverforvehicle = vehicleGroup.GroupBy(x => x.Driver_Id);

                    foreach (var driverAllocation in groupdriverforvehicle)
                    {
                                               
                        var alltime = data1.Where(a => a.Driver_Id == driverAllocation.Key).Select(x => new { AllocateDate = x.AllocateDate, IsActive = x.IsActive }).OrderByDescending(x => x.AllocateDate).ToList();
                        TimeSpan? allallocatetime = TimeSpan.Zero;
                        int daycount=0;
                        if (alltime.Count()%2==0) {
                         
                            for(int i=0;i<alltime.Count/2;i++)
                            {
                                var starttime = alltime[i].AllocateDate;
                                var endtime = alltime[i + 1].AllocateDate;
                                var allocateTimed = starttime- endtime;
                                allallocatetime = allallocatetime + allocateTimed;
                                if(allallocatetime.Value.Hours<24)
                                {
                                    daycount++;
                                }else if(allallocatetime.Value.Hours > 24)
                                {
                                   var ad= allallocatetime.Value.Hours/ 24;
                                    daycount = daycount + ad;
                                }
                            }
                        }
                        else
                        {
                            int allcount = 0;
                            if (alltime.Count==1)
                            {
                                allcount=2;
                            }else
                            {
                                allcount = alltime.Count+1;
                            }
                            for (int i = 0; i < allcount / 2; i++)
                            {
                                var starttime = alltime[i].AllocateDate;
                                var endtime = (bool)alltime.Last().IsActive ? DateTime.Now : alltime[i + 1].AllocateDate; 
                                var allocateTimed = starttime - endtime;
                                allallocatetime = allallocatetime + allocateTimed;
                                if (allallocatetime.Value.Hours < 24)
                                {
                                    daycount++;
                                }
                                else if (allallocatetime.Value.Hours > 24)
                                {
                                    var ad = allallocatetime.Value.Hours / 24;
                                    daycount = daycount + ad;
                                }
                            }

                        }
                        foreach(var dta in data)
                        {
                            if(dta.Driver_Id== driverAllocation.Key)
                            {
                                dta.RunDay = daycount;
                            }
                        }
                      
                        // Output the results
                        Debug.WriteLine($"VehicleId: {vehicleId}, DriverId: {driverAllocation.Key}, AllocateTime: {allallocatetime},daycount:{daycount}");
                    }
                }                //var groupdriver = data.GroupBy(x => x.Driver_Id);
                //foreach (var driver in groupdriver)
                //{
                //    Debug.WriteLine(driver.Key);

                //}
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record Available.";
                }
                else
                {
                    ViewBag.Transactionfee = Transactionfee;
                    ViewBag.Amount = (double?)commision;
                    ViewBag.gstAmount = (double?)gst;
                    ViewBag.tdsAmount = (double?)tds;
                    model.Ambulance = data;
                    foreach (var item in data)
                    {
                        var razorcomm = ((double?)item.TotalPrice * Transactionfee) / 100;
                        var totalrazorcomm = razorcomm;
                        item.Amountwithrazorpaycomm = (double?)item.TotalPrice + totalrazorcomm;
                    }
                }
                return View(model);
            }

        }

        public ActionResult ViewDriver_AmbulancePayoutHistory(int Id, DateTime? date)
        {
            Session["msg"] = Id;
            var model = new ViewPayOutHistory();
            var Name = ent.Database.SqlQuery<string>("select DriverName from Driver where Id=" + Id).FirstOrDefault();
            model.DriverName = Name;
            string qry = @"Select dp.Id, ISNULL(dp.IsPaid, 0) as IsPaid , Dp.IsGenerated, dp.Driver_Id, dp.PaymentDate, dp.Amount,d.DriverName from DriverPayOut as dp join Driver as d on d.Id=dp.Driver_Id where dp.Driver_Id=" + Id;
            var data = ent.Database.SqlQuery<HistoryOfAmbulance_Payout>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            else
            {
                model.HistoryOfAmbulance_Payout = data;
            }
            return View(model);
        }

        public ActionResult UpdateDriver_AmbStatus(int id)
        {
            int Id = Convert.ToInt32(Session["msg"]);
            string q = @"update DriverPayOut set IsGenerated = case when IsGenerated=1 then 0 else 1 end where Id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("ViewDriver_AmbulancePayoutHistory", new { Id = Id });
        }
        public ActionResult UpdateDriver_AmbPayment(int id)
        {
            //int Id = Convert.ToInt32(Session["msg"].ToString());
            int Id = Convert.ToInt32(Session["msg"]);
            string q = @"update DriverPayOut set IsPaid = case when IsPaid=1 then 0 else 1 end where Id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("ViewDriver_AmbulancePayoutHistory", new { Id = Id });
        }

        public ActionResult AmbulanceList()
        {
            var model = new ViewPayOutHistory();
            var data = ent.Database.SqlQuery<HistoryOfAmbulance_Payout>(@"WITH CTE AS (
    SELECT dp.*, v.VehicleNumber, v.VehicleName, d.DriverName,d.DriverId,
           ROW_NUMBER() OVER(PARTITION BY dp.Id ORDER BY dp.Id) AS RowNum
    FROM Driver d
    JOIN DriverPayOut dp ON dp.Driver_Id = d.Id
    JOIN DriverLocation AS dl ON dl.Driver_Id = d.Id
    JOIN Vehicle AS v ON v.Id = d.Vehicle_Id
)
SELECT Id,Driver_Id ,Amount ,IsPaid ,PaymentDate ,IsGenerated , VehicleNumber, VehicleName, DriverName,DriverId
FROM CTE
WHERE RowNum = 1
ORDER BY Id").ToList();
            model.HistoryOfAmbulance_Payout = data;
            return View(model);
        }

        [HttpGet]
        public ActionResult WeeklyReport(int Id, DateTime? date)
        {
            var model = new AmbulancesReport();
            double payment = ent.Database.SqlQuery<double>(@"select Top 1 Commission from CommissionMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();
            //double payment = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            //            var qry = @"select p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
            //trm.Amount, trm.Distance, d.DriverName,
            //trm.PickUp_Place, trm.Drop_Place,
            //v.Id as VehicleId
            //from TravelRecordMaster trm 
            //join Driver d on d.Id = trm.Driver_Id
            //join Vehicle v on v.Id = trm.Vehicle_Id
            //join Patient p on p.Id = trm.Patient_Id
            //where trm.IsDriveCompleted = 1 and trm.RequestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() and v.Id=" + Id;

            var qry = @"select p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
trm.Amount, trm.Distance, d.DriverName,
trm.PickUp_Place, trm.Drop_Place,
v.Id as VehicleId
from TravelRecordMaster trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1 and trm.RequestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() and v.Id=" + Id;
            var data = ent.Database.SqlQuery<Ambulance>(qry).ToList();
            if (date == null)
            {
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record of Current Date";
                }
                else
                {
                    ViewBag.Amount = payment;
                    model.PayAmt = (double?)data.Sum(a => a.Amount);
                    model.Ambulance = data;
                }
            }
            else
            {
                DateTime dateCriteria = date.Value.AddDays(-7);
                string Tarikh = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @" select p.PatientName, v.VehicleNumber, IsNull(v.VehicleName, 'NA') as VehicleName, 
trm.Amount, trm.Distance, d.DriverName,
trm.PickUp_Place, trm.Drop_Place,
v.Id as VehicleId
from TravelRecordMaster trm
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1 and trm.RequestDate between '" + dateCriteria + "' and '" + date + "' and v.Id =" + Id;
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
        public ActionResult DoctorList()
        {
            var model = new ViewPayOutHistory();
            var data = ent.Database.SqlQuery<HistoryOfDoc_Payout>(@"select d.DoctorName, dp.* from Doctor d join DoctorPayout dp on dp.Doctor_Id = d.Id order By dp.Id").ToList();
            model.HistoryOfDoc_Payout = data;
            return View(model);
        }


        public ActionResult NurseList()
        {
            var model = new ViewPayOutHistory();
            var data = ent.Database.SqlQuery<HistoryOfNurse_Payout>(@"select n.NurseName, np.* from Nurse n join NursePayout np on np.Nurse_Id = n.Id order By np.Id").ToList();
            model.HistoryOfNurse_Payout = data;
            return View(model);
        }
        public ActionResult LabList()
        {
            var model = new ViewPayOutHistory();
            var data = ent.Database.SqlQuery<HistoryOfLab_Payout>(@"select l.LabName, lp.* from Lab l join LabPayout lp on lp.Lab_Id = l.Id order By lp.Id").ToList();
            model.HistoryOfLab_Payout = data;
            return View(model);
        }
        public ActionResult HealthCheckUpList()
        {

            var model = new ViewPayOutHistory();
            var data = ent.Database.SqlQuery<HistoryOfHealth_Payout>(@"select hc.LabName, hp.* from HealthCheckupCenter hc join HealthPayOut hp on hp.Health_Id = hc.Id order By hp.Id").ToList();
            model.HistoryOfHealth_Payout = data;
            return View(model);
        }
        public ActionResult ChemistList()
        {
            var model = new ViewPayOutHistory();
            var data = ent.Database.SqlQuery<HistoryOfChemist_Payout>(@"select c.ChemistName, cp.* from Chemist c join ChemistPayOut cp on cp.Chemist_Id = c.Id order By cp.Id").ToList();
            model.HistoryOfChemist_Payout = data;
            return View(model);
        }

        //For Bank
        public void DownloadDoctorExcel(int? Id)
        {
            //            String query = @"select d.DoctorId,d.DoctorName,d.PAN,dp.*,bd.*,pa.AppointmentDate from Doctor d 
            //join DoctorPayout dp on dp.Doctor_Id = d.Id 
            //left join BankDetails as bd on bd.Login_Id=d.AdminLogin_Id
            //inner join PatientAppointment as pa on pa.Doctor_Id=d.Id
            //order By dp.Id";
            String query = @"WITH CTE AS (
    SELECT dp.*,pa.AppointmentDate,d.DoctorId, d.DoctorName,d.PAN,bd.BranchName,bd.IFSCCode,bd.HolderName,
           ROW_NUMBER() OVER(PARTITION BY dp.Id ORDER BY dp.Id) AS RowNum
    FROM Doctor d
    JOIN DoctorPayOut dp ON dp.Doctor_Id = d.Id
    JOIN PatientAppointment AS pa ON pa.Doctor_Id = d.Id
   
	left join BankDetails as bd on bd.Login_Id=d.AdminLogin_Id 
)
SELECT Id,DoctorId ,DoctorName,AppointmentDate,Amount ,IsPaid ,PaymentDate ,IsGenerated ,PAN,BranchName,IFSCCode,HolderName
FROM CTE
WHERE RowNum = 1
ORDER BY Id";

            var employeeDetails = ent.Database.SqlQuery<HistoryOfDoc_Payout>(query).ToList();
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");

            Sheet.Cells["A1"].Value = "Doctor Id";
            Sheet.Cells["B1"].Value = "Appointment Date";
            Sheet.Cells["C1"].Value = "Doctor Name";
            Sheet.Cells["D1"].Value = "Amount";
            //Sheet.Cells["C1"].Value = "Payment Date";
            Sheet.Cells["E1"].Value = "PAN No.";
            Sheet.Cells["F1"].Value = "IFSCCode";
            Sheet.Cells["G1"].Value = "Branch Name";
            Sheet.Cells["H1"].Value = "AC Holder Name";
            int row = 2;
            double totalAmount = 0.0; // Initialize a variable to store the total MonthSalary

            foreach (var item in employeeDetails)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.DoctorId;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.AppointmentDate;
                Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = "yyyy-MM-dd"; // Change the date format as needed
                Sheet.Cells[string.Format("C{0}", row)].Value = item.DoctorName;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Amount;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.PAN;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.IFSCCode;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.BranchName;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.HolderName;
                totalAmount += item.Amount; // Add the current MonthSalary to the total
                row++;
            }

            // Create a cell to display the total MonthSalary
            Sheet.Cells[string.Format("C{0}", row)].Value = "Total Amount";
            Sheet.Cells[string.Format("D{0}", row)].Value = totalAmount;

            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=Report.xlsx"); // Use a semicolon (;) instead of a colon (:)
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

        public void DownloadLabExcel(int? Id)
        {
            //            String query = @"select l.lABId,BTL.TestDate,l.LabName,l.PAN ,lp.*,bd.* from Lab l 
            //join LabPayout lp on lp.Lab_Id = l.Id 
            //left join BankDetails as bd on bd.Login_Id =l.AdminLogin_Id
            //INNER JOIN BookTestLab AS BTL on BTL .Lab_Id=l.Id
            //order By lp.Id";
            String query = @"WITH CTE AS (
    SELECT lp.*,BTL.TestDate,l.lABId, l.LabName,l.PAN,bd.BranchName,bd.IFSCCode,bd.HolderName,
           ROW_NUMBER() OVER(PARTITION BY lp.Id ORDER BY lp.Id) AS RowNum
    FROM Lab l
   JOIN LabPayOut lp on lp.Lab_Id = l.Id 
   LEFT JOIN BankDetails as bd on bd.Login_Id =l.AdminLogin_Id
   JOIN BookTestLab AS BTL on BTL .Lab_Id=l.Id
)
SELECT Id,lABId ,LabName,TestDate,Amount ,IsPaid ,PaymentDate ,IsGenerated ,PAN,BranchName,IFSCCode,HolderName
FROM CTE
WHERE RowNum = 1
ORDER BY Id";

            var employeeDetails = ent.Database.SqlQuery<HistoryOfLab_Payout>(query).ToList();
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");

            Sheet.Cells["A1"].Value = "Lab Id";
            Sheet.Cells["B1"].Value = "Test Date";
            Sheet.Cells["C1"].Value = "Lab Name";
            Sheet.Cells["D1"].Value = "Amount";
            Sheet.Cells["E1"].Value = "PAN No.";
            Sheet.Cells["F1"].Value = "IFSCCode";
            Sheet.Cells["G1"].Value = "Branch Name";
            Sheet.Cells["H1"].Value = "AC Holder Name";

            int row = 2;
            double totalAmount = 0.0; // Initialize a variable to store the total MonthSalary

            foreach (var item in employeeDetails)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.lABId;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.TestDate;
                Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = "yyyy-MM-dd"; // Change the date format as needed
                Sheet.Cells[string.Format("C{0}", row)].Value = item.LabName;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Amount;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.PAN;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.IFSCCode;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.BranchName;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.HolderName;

                totalAmount += item.Amount; // Add the current MonthSalary to the total
                row++;
            }

            // Create a cell to display the total MonthSalary
            Sheet.Cells[string.Format("C{0}", row)].Value = "Total Amount";
            Sheet.Cells[string.Format("D{0}", row)].Value = totalAmount;

            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=Report.xlsx"); // Use a semicolon (;) instead of a colon (:)
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

        public void DownloadChemistExcel(int? Id)
        {
            String query = @"select c.ChemistId,c.ChemistName,c.EmailId,c.Location,c.PinCode,c.LicenceNumber,c.PAN , cp.*,bd.* from Chemist c 
join ChemistPayOut cp on cp.Chemist_Id = c.Id
left join BankDetails as bd on bd.Login_Id =c.AdminLogin_Id
 order By cp.Id";

            var employeeDetails = ent.Database.SqlQuery<HistoryOfChemist_Payout>(query).ToList();
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");

            Sheet.Cells["A1"].Value = "Chemist Id";
            Sheet.Cells["B1"].Value = "Payment Date";
            Sheet.Cells["C1"].Value = "Chemist Name";
            Sheet.Cells["D1"].Value = "Amount";
            Sheet.Cells["E1"].Value = "PAN No.";
            Sheet.Cells["F1"].Value = "IFSCCode";
            Sheet.Cells["G1"].Value = "Branch Name";
            Sheet.Cells["H1"].Value = "AC Holder Name";
            int row = 2;
            double totalAmount = 0.0; // Initialize a variable to store the total MonthSalary

            foreach (var item in employeeDetails)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.ChemistId;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.PaymentDate;
                Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = "yyyy-MM-dd"; // Change the date format as needed
                Sheet.Cells[string.Format("C{0}", row)].Value = item.ChemistName;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Amount;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.PAN;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.IFSCCode;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.BranchName;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.HolderName;


                totalAmount += item.Amount; // Add the current MonthSalary to the total
                row++;
            }

            // Create a cell to display the total MonthSalary
            Sheet.Cells[string.Format("C{0}", row)].Value = "Total Amount";
            Sheet.Cells[string.Format("D{0}", row)].Value = totalAmount;

            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=Report.xlsx"); // Use a semicolon (;) instead of a colon (:)
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public void DownloadNurseExcel(int? Id)
        {
            String query = @"WITH CTE AS (
    SELECT lp.*,ns.ServiceDate, l.NurseName,l.NurseId,l.PAN,bd.BranchName,bd.IFSCCode,bd.HolderName,
           ROW_NUMBER() OVER(PARTITION BY lp.Id ORDER BY lp.Id) AS RowNum
    FROM Nurse l
   JOIN NursePayout lp on lp.Nurse_Id = l.Id 
   LEFT JOIN BankDetails as bd on bd.Login_Id =l.AdminLogin_Id
   JOIN Nurseservice AS ns on ns.Nurse_Id=l.Id
)
SELECT Id,NurseId,NurseName ,ServiceDate,Amount ,IsPaid ,PaymentDate ,IsGenerated ,PAN,BranchName,IFSCCode,HolderName
FROM CTE
WHERE RowNum = 1
ORDER BY Id";

            var employeeDetails = ent.Database.SqlQuery<HistoryOfNurse_Payout>(query).ToList();
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");

            Sheet.Cells["A1"].Value = "Nurse Id";
            Sheet.Cells["B1"].Value = "Service Date";
            Sheet.Cells["C1"].Value = "Nurse Name";
            Sheet.Cells["D1"].Value = "Amount";
            Sheet.Cells["E1"].Value = "PAN No.";
            Sheet.Cells["F1"].Value = "IFSCCode";
            Sheet.Cells["G1"].Value = "Branch Name";
            Sheet.Cells["H1"].Value = "AC Holder Name";
            int row = 2;
            double totalAmount = 0.0; // Initialize a variable to store the total MonthSalary

            foreach (var item in employeeDetails)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.NurseId;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.ServiceDate;
                Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = "yyyy-MM-dd"; // Change the date format as needed
                Sheet.Cells[string.Format("C{0}", row)].Value = item.NurseName;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Amount;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.PAN;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.IFSCCode;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.BranchName;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.HolderName;

                totalAmount += item.Amount; // Add the current MonthSalary to the total
                row++;
            }

            // Create a cell to display the total MonthSalary
            Sheet.Cells[string.Format("C{0}", row)].Value = "Total Amount";
            Sheet.Cells[string.Format("D{0}", row)].Value = totalAmount;

            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=Report.xlsx"); // Use a semicolon (;) instead of a colon (:)
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

        public void DownloadAmbulanceExcel(int? Id)
        {
            string query = @"WITH CTE AS (
    SELECT dp.*, v.VehicleNumber, v.VehicleName, d.DriverName,d.PAN,dl.EntryDate,bd.BranchName,bd.IFSCCode,bd.HolderName,
           ROW_NUMBER() OVER(PARTITION BY dp.Id ORDER BY dp.Id) AS RowNum
    FROM Driver d
    JOIN DriverPayOut dp ON dp.Driver_Id = d.Id
    JOIN DriverLocation AS dl ON dl.Driver_Id = d.Id
    JOIN Vehicle AS v ON v.Driver_Id = dl.Driver_Id
	left join BankDetails as bd on bd.Login_Id=d.AdminLogin_Id 
)
SELECT Id,Driver_Id ,Amount ,IsPaid ,PaymentDate ,IsGenerated , VehicleNumber, VehicleName, DriverName,PAN,EntryDate,BranchName,IFSCCode,HolderName
FROM CTE
WHERE RowNum = 1
ORDER BY Id";

            var employeeDetails = ent.Database.SqlQuery<HistoryOfAmbulance_Payout>(query).ToList();
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");

            Sheet.Cells["A1"].Value = "Driver Id";
            Sheet.Cells["B1"].Value = "Appointment Date";
            Sheet.Cells["C1"].Value = "Driver Name";
            Sheet.Cells["D1"].Value = "Vehicle Name";
            Sheet.Cells["E1"].Value = "Vehicle Number";
            Sheet.Cells["F1"].Value = "Amount";
            //Sheet.Cells["C1"].Value = "Payment Date";
            Sheet.Cells["G1"].Value = "PAN No.";
            Sheet.Cells["H1"].Value = "IFSCCode";
            Sheet.Cells["I1"].Value = "Branch Name";
            Sheet.Cells["J1"].Value = "AC Holder Name";
            int row = 2;
            double totalAmount = 0.0; // Initialize a variable to store the total MonthSalary

            foreach (var item in employeeDetails)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.Driver_Id;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.EntryDate;
                Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = "yyyy-MM-dd"; // Change the date format as needed
                Sheet.Cells[string.Format("C{0}", row)].Value = item.DriverName;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.VehicleName;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.VehicleNumber;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Amount;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.PAN;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.IFSCCode;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.BranchName;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.HolderName;
                totalAmount += item.Amount; // Add the current MonthSalary to the total
                row++;
            }

            // Create a cell to display the total MonthSalary
            Sheet.Cells[string.Format("E{0}", row)].Value = "Total Amount";
            Sheet.Cells[string.Format("F{0}", row)].Value = totalAmount;

            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=Report.xlsx"); // Use a semicolon (;) instead of a colon (:)
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }


        //        public void AdminDoctorExcel()
        //        {
        //            double commission = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault();
        //            double gst = ent.Database.SqlQuery<double>(@"select Amount from GSTMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault();
        //            double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault();

        //            var qry = @"select D.DoctorId, A.Doctor_Id, D.DoctorName, SUM(A.TotalFee) as Amount from PatientAppointment A join Doctor D on D.Id = A.Doctor_Id where A.AppointmentDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() group by D.DoctorName, A.Doctor_Id, D.DoctorId";
        //            var employeeDetails = ent.Database.SqlQuery<PayOutDocHistroy>(qry).ToList();

        //            ExcelPackage Ep = new ExcelPackage();
        //            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");

        //            Sheet.Cells["A1"].Value = "Doctor Id"; 
        //            Sheet.Cells["B1"].Value = "Doctor Name";
        //            Sheet.Cells["C1"].Value = "Basic Amount"; // Change the header to indicate the amount with GST
        //            Sheet.Cells["D1"].Value = "GST";
        //            Sheet.Cells["E1"].Value = "Total Amount";
        //            Sheet.Cells["F1"].Value = "Commission";
        //            Sheet.Cells["G1"].Value = "TDS";
        //            Sheet.Cells["H1"].Value = "Payble Amount";
        //            int row = 2;
        //            double totalAmount = 0.0; // Initialize a variable to store the total Amount

        //            foreach (var item in employeeDetails)
        //            {
        //                Sheet.Cells[string.Format("A{0}", row)].Value = item.DoctorId;
        //                Sheet.Cells[string.Format("B{0}", row)].Value = item.DoctorName;
        //                Sheet.Cells[string.Format("C{0}", row)].Value = item.Amount;

        //                // Calculate commission amount and TDS amount
        //                double? commAmt = (item.Amount * commission) / 100;
        //                double? tdsAmt = (item.Amount * tds) / 100;

        //                // Calculate the payable amount
        //                double? payableAmount = item.Amount - (commAmt + tdsAmt);

        //                // Calculate the amount with GST
        //                double? amountWithGST = item.Amount * (1 + (gst / 100)); 
        //                Sheet.Cells[string.Format("D{0}", row)].Value = gst;
        //                Sheet.Cells[string.Format("E{0}", row)].Value = amountWithGST;
        //                Sheet.Cells[string.Format("F{0}", row)].Value = commission;
        //                Sheet.Cells[string.Format("G{0}", row)].Value = tds;
        //                Sheet.Cells[string.Format("H{0}", row)].Value = payableAmount;
        //                totalAmount += (double)payableAmount; 
        //                row++;
        //            }

        //            // Create a cell to display the total amount
        //            Sheet.Cells[string.Format("G{0}", row)].Value = "Total Amount";
        //            Sheet.Cells[string.Format("H{0}", row)].Value = totalAmount;

        //            Sheet.Cells["A:AZ"].AutoFitColumns();
        //            Response.Clear();
        //            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //            Response.AddHeader("content-disposition", "attachment; filename=Report.xlsx");
        //            Response.BinaryWrite(Ep.GetAsByteArray());
        //            Response.End();
        //        }

        //        public void AdminLabExcel()
        //        {
        //            double commission = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Lab'").FirstOrDefault();
        //            double gst = ent.Database.SqlQuery<double>(@"select Amount from GSTMaster where IsDeleted=0 and Name='Lab'").FirstOrDefault();
        //            double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Lab'").FirstOrDefault();

        //            var qry = @"select D.lABId,A.Lab_Id, D.LabName, SUM(A.Amount) as Amount, (SUM(A.Amount) - (SUM(A.Amount) * 7 /100)) As NetAmount from BookTestLab A join Lab D on D.Id = A.Lab_Id where Convert(Date,A.TestDate) between DATEADD(DAY, -7, GETDATE()) AND GETDATE() GROUP BY  D.LabName, A.Lab_Id,D.lABId";
        //            var employeeDetails = ent.Database.SqlQuery<PayOutLabHistoty>(qry).ToList();
        //            foreach (var item in employeeDetails)
        //            {
        //                var razorcomm = item.Amount * 18 / 100;
        //                var razorcommafter = razorcomm * 3 / 100;
        //                var totalrazorcomm = razorcomm + razorcommafter;
        //                item.Amountwithrazorpaycomm = item.Amount + totalrazorcomm;

        //            }
        //            ExcelPackage Ep = new ExcelPackage();
        //            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");

        //            Sheet.Cells["A1"].Value = "Lab Id";
        //            Sheet.Cells["B1"].Value = "Lab Name";
        //            Sheet.Cells["C1"].Value = "Basic Amount"; // Change the header to indicate the amount with GST
        //            Sheet.Cells["D1"].Value = "Transaction Fee"; // Change the header to indicate the amount with GST
        //            Sheet.Cells["E1"].Value = "GST";
        //            Sheet.Cells["F1"].Value = "Total Amount";
        //            Sheet.Cells["G1"].Value = "Commission";
        //            Sheet.Cells["H1"].Value = "TDS";
        //            Sheet.Cells["I1"].Value = "Payble Amount";
        //            int row = 2;
        //            double totalAmount = 0.0; // Initialize a variable to store the total Amount

        //            foreach (var item in employeeDetails)
        //            {
        //                Sheet.Cells[string.Format("A{0}", row)].Value = item.lABId;
        //                Sheet.Cells[string.Format("B{0}", row)].Value = item.LabName;
        //                Sheet.Cells[string.Format("C{0}", row)].Value = item.Amount;
        //                double? razorpaycomm = item.Amountwithrazorpaycomm;
        //                double? basicamt = item.Amount;
        //                double? razarfeeamt = razorpaycomm - basicamt;
        //                Sheet.Cells[string.Format("D{0}", row)].Value = razarfeeamt;



        //                // Calculate commission amount and TDS amount


        //                // Calculate the amount with GST
        //                double? GSTAmt = (item.Amount * gst) / 100;
        //                double? commAmt = (item.Amount * commission) / 100;
        //                double? tdsAmt = (item.Amount * tds) / 100;
        //                double? WithGSTAmt = item.Amount + razarfeeamt - (GSTAmt);
        //                // Calculate the payable amount
        //                double? payableAmount = WithGSTAmt - (commAmt + tdsAmt);
        //                //double? WithGSTAmt = item.Amount + GSTAmt;

        //                double? amountWithGST = item.Amount * (1 + (gst / 100));
        //                Sheet.Cells[string.Format("E{0}", row)].Value = gst;
        //                Sheet.Cells[string.Format("F{0}", row)].Value = WithGSTAmt;
        //                Sheet.Cells[string.Format("G{0}", row)].Value = commission;
        //                Sheet.Cells[string.Format("H{0}", row)].Value = tds;
        //                Sheet.Cells[string.Format("I{0}", row)].Value = payableAmount;
        //                totalAmount += (double)payableAmount;
        //                row++;
        //            }

        //            // Create a cell to display the total amount
        //            Sheet.Cells[string.Format("G{0}", row)].Value = "Total Amount";
        //            Sheet.Cells[string.Format("H{0}", row)].Value = totalAmount;

        //            Sheet.Cells["A:AZ"].AutoFitColumns();
        //            Response.Clear();
        //            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //            Response.AddHeader("content-disposition", "attachment; filename=Report.xlsx");
        //            Response.BinaryWrite(Ep.GetAsByteArray());
        //            Response.End();
        //        }

        //        public void AdminChemistExcel()
        //        {
        //            double commission = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Chemist'").FirstOrDefault();
        //            double gst = ent.Database.SqlQuery<double>(@"select Amount from GSTMaster where IsDeleted=0 and Name='Chemist'").FirstOrDefault();
        //            double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Chemist'").FirstOrDefault();

        //            var qry = @"SELECT C.ChemistId,C.ChemistName, O.OrderDate, cp.IsGenerated, O.Chemist_Id, Sum(A.Amount) as Amount, (SUM(A.Amount) - (SUM(A.Amount) * 7 /100)) As Amount from MedicineOrderDetail A left join MedicineOrder O on A.Order_Id = O.Id left join Chemist C on C.Id = O.Chemist_Id left join ChemistPayOut cp on C.Id = cp.Chemist_Id where O.OrderDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE()  and O.IsPaid=1 group by C.ChemistName,  O.OrderDate, O.Chemist_Id, cp.IsGenerated,C.ChemistId";
        //            var employeeDetails = ent.Database.SqlQuery<PayOutChemistHistoty>(qry).ToList();

        //            ExcelPackage Ep = new ExcelPackage();
        //            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");

        //            Sheet.Cells["A1"].Value = "Chemist Id";
        //            Sheet.Cells["B1"].Value = "Chemist Name";
        //            Sheet.Cells["C1"].Value = "Basic Amount"; // Change the header to indicate the amount with GST
        //            Sheet.Cells["D1"].Value = "GST";
        //            Sheet.Cells["E1"].Value = "Total Amount";
        //            Sheet.Cells["F1"].Value = "Commission";
        //            Sheet.Cells["G1"].Value = "TDS";
        //            Sheet.Cells["H1"].Value = "Payble Amount";
        //            int row = 2;
        //            double totalAmount = 0.0; // Initialize a variable to store the total Amount

        //            foreach (var item in employeeDetails)
        //            {
        //                Sheet.Cells[string.Format("A{0}", row)].Value = item.ChemistId;
        //                Sheet.Cells[string.Format("B{0}", row)].Value = item.ChemistName;
        //                Sheet.Cells[string.Format("C{0}", row)].Value = item.Amount;

        //                // Calculate commission amount and TDS amount
        //                double? commAmt = (item.Amount * commission) / 100;
        //                double? tdsAmt = (item.Amount * tds) / 100;

        //                // Calculate the payable amount
        //                double? payableAmount = item.Amount - (commAmt + tdsAmt);

        //                // Calculate the amount with GST
        //                double? amountWithGST = item.Amount * (1 + (gst / 100));
        //                Sheet.Cells[string.Format("D{0}", row)].Value = gst;
        //                Sheet.Cells[string.Format("E{0}", row)].Value = amountWithGST;
        //                Sheet.Cells[string.Format("F{0}", row)].Value = commission;
        //                Sheet.Cells[string.Format("G{0}", row)].Value = tds;
        //                Sheet.Cells[string.Format("H{0}", row)].Value = payableAmount;
        //                totalAmount += (double)payableAmount;
        //                row++;
        //            }

        //            // Create a cell to display the total amount
        //            Sheet.Cells[string.Format("G{0}", row)].Value = "Total Amount";
        //            Sheet.Cells[string.Format("H{0}", row)].Value = totalAmount;

        //            Sheet.Cells["A:AZ"].AutoFitColumns();
        //            Response.Clear();
        //            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //            Response.AddHeader("content-disposition", "attachment; filename=Report.xlsx");
        //            Response.BinaryWrite(Ep.GetAsByteArray());
        //            Response.End();
        //        }

        //        public void AdminNurseExcel(int? NurseId)
        //        {
        //            double commission = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Nurse'").FirstOrDefault();
        //            double gst = ent.Database.SqlQuery<double>(@"select Amount from GSTMaster where IsDeleted=0 and Name='Nurse'").FirstOrDefault();
        //            double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Nurse'").FirstOrDefault();

        //            var qry = @"select n.NurseId,ns.Nurse_Id, ns.Id, ns.ServiceStatus, ns.IsPaid, case when ns.PaymentDate is null then 'N/A' else Convert(nvarchar(100), ns.PaymentDate, 103) end as PaymentDate, case when ns.ServiceAcceptanceDate is null then 'N/A' else Convert(nvarchar(100), ns.ServiceAcceptanceDate, 103) end as ServiceAcceptanceDate, Convert(nvarchar(100), ns.RequestDate, 103) as RequestDate,'From ' + Convert(nvarchar(100), ns.StartDate, 103) + ' to ' + Convert(nvarchar(100), ns.EndDate, 103) as ServiceTiming ,IsNull(n.NurseName, 'N/A') as NurseName, IsNull(n.MobileNumber, 'N/A') as NurseMobileNumber, ns.TotalFee from NurseService ns join Nurse n on ns.Nurse_Id = n.Id where ns.ServiceAcceptanceDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() and ns.ServiceStatus='Approved' and ns.IsPaid = 1 and ns.Nurse_Id='" + NurseId + "' order by ns.Id desc";
        //            var employeeDetails = ent.Database.SqlQuery<NurseAppointmentList>(qry).ToList();

        //            ExcelPackage Ep = new ExcelPackage();
        //            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");

        //            Sheet.Cells["A1"].Value = "Nurse Id";
        //            Sheet.Cells["B1"].Value = "Nurse Name";
        //            Sheet.Cells["C1"].Value = "Basic Amount";
        //            Sheet.Cells["E1"].Value = "Total Amount";
        //            Sheet.Cells["F1"].Value = "Commission";
        //            Sheet.Cells["G1"].Value = "TDS";
        //            Sheet.Cells["H1"].Value = "Payble Amount";
        //            int row = 2;
        //            double totalAmount = 0.0; // Initialize a variable to store the total Amount

        //            foreach (var item in employeeDetails)
        //            {
        //                Sheet.Cells[string.Format("A{0}", row)].Value = item.NurseId;
        //                Sheet.Cells[string.Format("B{0}", row)].Value = item.NurseName;
        //                Sheet.Cells[string.Format("C{0}", row)].Value = item.TotalFee;

        //                // Calculate commission amount and TDS amount
        //                double? commAmt = (item.TotalFee * commission) / 100;
        //                double? tdsAmt = (item.TotalFee * tds) / 100;

        //                // Calculate the payable amount
        //                double? payableAmount = item.TotalFee - (commAmt + tdsAmt);

        //                // Calculate the amount with GST
        //                double? amountWithGST = item.TotalFee * (1 + (gst / 100));
        //                Sheet.Cells[string.Format("D{0}", row)].Value = gst;
        //                Sheet.Cells[string.Format("E{0}", row)].Value = amountWithGST;
        //                Sheet.Cells[string.Format("F{0}", row)].Value = commission;
        //                Sheet.Cells[string.Format("G{0}", row)].Value = tds;
        //                Sheet.Cells[string.Format("H{0}", row)].Value = payableAmount;
        //                totalAmount += (double)payableAmount;
        //                row++;
        //            }

        //            // Create a cell to display the total amount
        //            Sheet.Cells[string.Format("G{0}", row)].Value = "Total Amount";
        //            Sheet.Cells[string.Format("H{0}", row)].Value = totalAmount;

        //            Sheet.Cells["A:AZ"].AutoFitColumns();
        //            Response.Clear();
        //            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //            Response.AddHeader("content-disposition", "attachment; filename=Report.xlsx");
        //            Response.BinaryWrite(Ep.GetAsByteArray());
        //            Response.End();
        //        }

        //        public void AdminAmbulanceExcel()
        //        {
        //            double commission = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();
        //            double gst = ent.Database.SqlQuery<double>(@"select Amount from GSTMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();
        //            double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();

        //            var qry = @"select d.DriverId,trm.Driver_Id,v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName,v.Id as VehicleId, d.DriverName,
        //Sum(trm.TotalPrice) as Amount from DriverLocation trm 
        // join Driver d on d.Id = trm.Driver_Id 
        // join Vehicle v on v.Driver_Id = trm.Driver_Id 
        // join Patient p on p.Id = trm.PatientId
        //where trm.EntryDate between DateAdd(DD,-7,GETDATE() ) 
        //and GETDATE() and trm.IsPay='Y' group by v.VehicleNumber, v.VehicleName, v.Id,d.DriverName,trm.Driver_Id,d.DriverId";
        //            var employeeDetails = ent.Database.SqlQuery<Ambulance>(qry).ToList();

        //            ExcelPackage Ep = new ExcelPackage();
        //            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");

        //            Sheet.Cells["A1"].Value = "Driver Id";
        //            Sheet.Cells["B1"].Value = "Vehicle Number";
        //            Sheet.Cells["C1"].Value = "Vehicle Name"; 
        //            Sheet.Cells["D1"].Value = "Driver Name"; 
        //            Sheet.Cells["E1"].Value = "Basic Amount";
        //            Sheet.Cells["F1"].Value = "GST";
        //            Sheet.Cells["G1"].Value = "Total Amount";
        //            Sheet.Cells["H1"].Value = "Commission";
        //            Sheet.Cells["I1"].Value = "TDS";
        //            Sheet.Cells["J1"].Value = "Payble Amount";
        //            int row = 2;
        //            double totalAmount = 0.0; // Initialize a variable to store the total Amount

        //            foreach (var item in employeeDetails)
        //            {
        //                Sheet.Cells[string.Format("A{0}", row)].Value = item.DriverId;
        //                Sheet.Cells[string.Format("B{0}", row)].Value = item.VehicleNumber;
        //                Sheet.Cells[string.Format("C{0}", row)].Value = item.VehicleName;
        //                Sheet.Cells[string.Format("D{0}", row)].Value = item.DriverName;
        //                Sheet.Cells[string.Format("E{0}", row)].Value = item.Amount;

        //                // Calculate commission amount and TDS amount
        //                double? commAmt = ((double?)item.Amount * commission) / 100;
        //                double? tdsAmt = ((double?)item.Amount * tds) / 100;

        //                // Calculate the payable amount
        //                double? payableAmount = (double?)item.Amount - (commAmt + tdsAmt);

        //                // Calculate the amount with GST
        //                double? amountWithGST = (double?)item.Amount * (1 + (gst / 100));
        //                Sheet.Cells[string.Format("F{0}", row)].Value = gst;
        //                Sheet.Cells[string.Format("G{0}", row)].Value = amountWithGST;
        //                Sheet.Cells[string.Format("H{0}", row)].Value = commission;
        //                Sheet.Cells[string.Format("I{0}", row)].Value = tds;
        //                Sheet.Cells[string.Format("J{0}", row)].Value = payableAmount;
        //                totalAmount += (double)payableAmount;
        //                row++;
        //            }

        //            // Create a cell to display the total amount
        //            Sheet.Cells[string.Format("I{0}", row)].Value = "Total Amount";
        //            Sheet.Cells[string.Format("J{0}", row)].Value = totalAmount;

        //            Sheet.Cells["A:AZ"].AutoFitColumns();
        //            Response.Clear();
        //            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //            Response.AddHeader("content-disposition", "attachment; filename=Report.xlsx");
        //            Response.BinaryWrite(Ep.GetAsByteArray());
        //            Response.End();
        //        }
    }
}