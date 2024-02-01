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
    public class Rwa_ReportController : Controller
    {
        DbEntities ent = new DbEntities();
        // GET: Rwa_Report
        public ActionResult RwaList()
        {
            var model = new rwaReport();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Name='RWA'").FirstOrDefault();
            string q = @"select rwa.Id, Count(P.Id) as Counts, rwa.AuthorityName from Patient p join RWA rwa on rwa.Id = p.Rwa_Id where Convert(Date,P.Reg_Date) between DATEADD(day,-7,GETDATE()) and GetDate() group by rwa.AuthorityName,rwa.Id";
            var data = ent.Database.SqlQuery<rwaList>(q).ToList();
            if(data.Count() == 0)
            {
                TempData["msg"] = "No Record";
                return View(model);
            }
            ViewBag.Payment = payment;
            model.rwaList = data;
            return View(model);
        }
        

        public ActionResult PatientList(int Id, DateTime? Reg_Date)
        {
            var model = new rwaReport();
            string q = @"select p.PatientName, p.Reg_Date, rwa.AuthorityName from Patient p join RWA rwa on rwa.Id = p.Rwa_Id where Convert(Date,P.Reg_Date) between DATEADD(day,-7,GETDATE()) and GetDate()";
            var data = ent.Database.SqlQuery<rwaList>(q).ToList();
            if(data.Count() ==0)
            {
                TempData["msg"] = "No Record";
                return View(model);
            }
            if(Reg_Date != null)
            {
                data = data.Where(a => a.Reg_Date == Reg_Date).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Record";
                    return View(model);
                }
            }
            model.rwaList = data;
            return View(model);
        }

        public ActionResult ViewReport()
        {
            return View();
        }


        public ActionResult Daily(string term, DateTime? date)
        {
            var model = new rwaReport();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Name='RWA'").FirstOrDefault();
            var qry = @"select COUNT(d.Id) as Counts, v.AuthorityName from Patient d join RWA v on d.Rwa_Id = v.Id where Convert(date,d.Reg_Date) = Convert(date,getDATE()) group by v.AuthorityName";
            var data = ent.Database.SqlQuery<rwaList>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.rwaList = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (date != null)
            {
                string dt = date.Value.ToString("MM/dd/yyyy");
                var qry1 = @"select COUNT(d.Id) as Counts, v.AuthorityName from Patient d join RWA v on d.Rwa_Id = v.Id where Convert(date,d.Reg_Date) = Convert(date,'"+dt+"') group by v.AuthorityName";
                var data1 = ent.Database.SqlQuery<rwaList>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.rwaList = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult Weekly(string term, DateTime? date)
        {
            var model = new rwaReport();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Name='RWA'").FirstOrDefault();
            var qry = @"select rwa.Id, Count(P.Id) as Counts, rwa.AuthorityName from Patient p join RWA rwa on rwa.Id = p.Rwa_Id where Convert(Date,P.Reg_Date) between DATEADD(day,-7,GETDATE()) and GetDate() group by rwa.AuthorityName,rwa.Id";
            var data = ent.Database.SqlQuery<rwaList>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.rwaList = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (date != null)
            {
                DateTime dateCriteria = date.Value.AddDays(-7);
                string Tarikh = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select rwa.Id, Count(P.Id) as Counts, rwa.AuthorityName from Patient p join RWA rwa on rwa.Id = p.Rwa_Id where Convert(Date,P.Reg_Date) between '"+ dateCriteria + "' and '"+Tarikh+"' group by rwa.AuthorityName,rwa.Id";
                var data1 = ent.Database.SqlQuery<rwaList>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.rwaList = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult Monthly(string term, DateTime? sdate, DateTime? edate)
        {
            var model = new rwaReport();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Name='RWA'").FirstOrDefault();
            var qry = @"select COUNT(d.Id) as Counts, v.AuthorityName from Patient d join RWA v on d.Rwa_Id = v.Id where Month(d.Reg_Date) = Month(getDATE()) group by v.AuthorityName";
            var data = ent.Database.SqlQuery<rwaList>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.rwaList = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (sdate != null && edate != null)
            {
                var qry1 = @"select COUNT(d.Id) as Counts, v.AuthorityName from Patient d join RWA v on d.Rwa_Id = v.Id where Month(d.Reg_Date) between Convert(datetime,'" + edate+ "',103) and Convert(datetime,'" + sdate+"',103) group by v.AuthorityName";
                var data1 = ent.Database.SqlQuery<rwaList>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.rwaList = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult Yearly(string term, int? year)
        {
            var model = new rwaReport();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Name='RWA'").FirstOrDefault();
            var qry = @"select COUNT(d.Id) as Counts, v.AuthorityName from Patient d join RWA v on d.Rwa_Id = v.Id where Year(d.Reg_Date) = Year(getDATE()) group by v.AuthorityName";
            var data = ent.Database.SqlQuery<rwaList>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Year";
            }
            else
            {
                model.rwaList = data;
                ViewBag.Payment = payment;
                //ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (year != null)
            {
                var qry1 = @"select COUNT(d.Id) as Counts, v.AuthorityName from Patient d join RWA v on d.Rwa_Id = v.Id where Year(d.Reg_Date) = Year('"+year+"') group by v.AuthorityName";
                var data1 = ent.Database.SqlQuery<rwaList>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Payment = payment;
                    model.rwaList = data1;
                    //ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult RWA(DateTime? Reg_Date, string name = null)
        {
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='RWA'").FirstOrDefault();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Name='RWA'").FirstOrDefault();
            var model = new rwaReport();
            if (Reg_Date != null)
            {
                DateTime dateCriteria = Reg_Date.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select COUNT(d.Id) as Counts, v.AuthorityName from Patient d join RWA v on d.Rwa_Id = v.Id where d.Reg_Date between '"+dateCriteria+"' and '"+Reg_Date+"' group by v.AuthorityName";
                var data1 = ent.Database.SqlQuery<rwaList>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Commission = commision;
                    ViewBag.Payment = payment;
                    //int total = data1.Count;
                    //pageNumber = (int?)pageNumber ?? 1;
                    //int pageSize = 10;
                    //decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                    //model.TotalPages = (int)noOfPages;
                    //model.PageNumber = (int)pageNumber;
                    //data1 = data1.OrderBy(a => a.Doctor_Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                    if (name != null)
                    {
                        data1 = data1.Where(a => a.AuthorityName.ToLower().Contains(name)).ToList();
                    }
                    model.rwaList = data1;
                    return View(model);
                }
            }
            var doctor = @"select COUNT(d.Id) as Counts, v.AuthorityName from Patient d join RWA v on d.Rwa_Id = v.Id where d.Reg_Date  >= DATEADD(day,-7, GETDATE()) group by v.AuthorityName";
            var data = ent.Database.SqlQuery<rwaList>(doctor).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record Of Current Week";
                return View(model);
            }
            else
            {
                ViewBag.Payment = payment;
                ViewBag.Commission = commision;
                //int total = data.Count;
                //pageNumber = (int?)pageNumber ?? 1;
                //int pageSize = 10;
                //decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                //model.TotalPages = (int)noOfPages;
                //model.PageNumber = (int)pageNumber;
                //data = data.OrderBy(a => a.Doctor_Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                model.rwaList = data;
                return View(model);
            }

        }


        public ActionResult RwaPayOut(DateTime? week, string name = null)
        {
            var model = new rwaReport();
            double amount = ent.Database.SqlQuery<double>(@"select Amount from PayoutMaster where IsDeleted=0 and Name='RWA'").FirstOrDefault();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Name='RWA'").FirstOrDefault();
            if (week != null)
            {
                DateTime dateCriteria = week.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                //var qry1 = @"select A.Doctor_Id, D.DoctorName, SUM(A.Amount) as Amount, (SUM(A.Amount) - (SUM(A.Amount) * 7 /100)) As NetAmount from PatientAppointment A join Doctor D on D.Id = A.Doctor_Id  where A.AppointmentDate between '" + dateCriteria + "' and '" + week + "' GROUP BY A.Amount, D.DoctorName, A.Doctor_Id";
                var qry1 = @"select COUNT(d.Id) as Counts, v.Id, v.AuthorityName from Patient d join RWA v on d.Rwa_Id = v.Id where d.Reg_Date between Convert(datetime,'"+dateCriteria+"',103) and Convert(datetime,'" + week + "',103) group by v.AuthorityName,v.Id";
                var data1 = ent.Database.SqlQuery<rwaList>(qry1).ToList();
                if (name != null)
                {
                    data1 = data1.Where(a => a.AuthorityName.ToLower().Contains(name)).ToList();
                }
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Amount = amount;
                    ViewBag.payment = payment;
                    model.rwaList = data1;
                    return View(model);
                }
            }
            //var qry = @"select A.Doctor_Id, D.DoctorName, SUM(A.Amount) as Amount, (SUM(A.Amount) - (SUM(A.Amount) * 7 /100)) As NetAmount from PatientAppointment A join Doctor D on D.Id = A.Doctor_Id where A.AppointmentDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() group by A.Amount, D.DoctorName, A.Doctor_Id";
            var qry = @"select COUNT(d.Id) as Counts,v.Id, v.AuthorityName from Patient d join RWA v on d.Rwa_Id = v.Id where d.Reg_Date  >= DATEADD(day,-7, GETDATE()) group by v.AuthorityName,v.Id";
            var data = ent.Database.SqlQuery<rwaList>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Week";
            }
            else
            {
                ViewBag.payment = payment;
                ViewBag.Amount = amount;
                model.rwaList = data;
                return View(model);
            }
            return View(model);
        }
        public ActionResult Pay(int? Id, double Amount)
        {
            var model = new RwaPayout();
            model.Rwa_Id = Id;
            model.Amount = Amount;
            model.IsPaid = false;
            model.IsGenerated = true;
            model.PaymentDate = DateTime.Now.Date;
            ent.RwaPayouts.Add(model);
            ent.SaveChanges();
            return RedirectToAction("ViewHistory", new { Id = model.Rwa_Id });
        }

        public ActionResult ViewHistory(int Id, DateTime? date)
        {
            Session["msg"] = Id;
            var model = new ViewPayOutHistory();
            var Name = ent.Database.SqlQuery<string>("select AuthorityName from Rwa where Id=" + Id).FirstOrDefault();
            model.AuthorityName = Name;
            TempData["Id"] = Id;
            string qry = @"select Dp.Id, ISNULL(Dp.IsPaid, 0) as IsPaid , Dp.IsGenerated, Dp.Rwa_Id, Dp.PaymentDate, Dp.Amount, D.AuthorityName from  RwaPayOut Dp join Rwa D on D.Id = Dp.Rwa_Id  where  Dp.Rwa_Id=" + Id;
            var data = ent.Database.SqlQuery<HistoryOfRWA_pAYOUT>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            model.HistoryOfRWA_Payout = data;
            return View(model);
        }

        public ActionResult UpdateStatus(int id)
        {
            int Id = Convert.ToInt32(Session["msg"]);
            string q = @"update RwaPayOut set IsGenerated = case when IsGenerated=1 then 0 else 1 end where Id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("ViewHistory", new { Id = Id });
        }

        public ActionResult UpdatePayment(int id)
        {
            int Id = Convert.ToInt32(Session["msg"]);
            string q = @"update RwaPayOut set IsPaid = case when IsPaid=1 then 0 else 1 end where Id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("ViewHistory", new { Id = Id });
        }

    }
}