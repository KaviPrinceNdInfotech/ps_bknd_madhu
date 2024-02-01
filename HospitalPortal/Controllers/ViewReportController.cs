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
    public class ViewReportController : Controller
    {
        DbEntities ent = new DbEntities();
        // GET: ViewReport
        public ActionResult Doctor()
        {
            return View();
        }


        public ActionResult DailyDoctor(string term,DateTime? date)
        {
                var model = new ReportDetails();
                var qry = @"select P.AppointmentDate, Sum(P.TotalFee) as Amount from PatientAppointment P join Doctor D on D.Id = p.Doctor_Id where Convert(Date, P.AppointmentDate) = Convert(Date,GETDATE()) and P.IsPaid=1 GROUP BY P.AppointmentDate, P.TotalFee";
                var data = ent.Database.SqlQuery<Doctors>(qry).ToList();
                 if(data.Count() == 0)
                 {
                 TempData["msg"] = "No Record of Current Date";
                 }
                 else
                 {
                model.doctorList = data;
                ViewBag.Total = model.doctorList.Sum(a => a.Amount);
                }
                if (date != null)
                {
                var qry1 = @"select P.AppointmentDate, Sum(P.Amount) as Amount from PatientAppointment P join Doctor D on D.Id = p.Doctor_Id where P.AppointmentDate = '"+date+ "' and P.IsPaid=1 GROUP BY P.AppointmentDate, P.Amount";
                var data1 = ent.Database.SqlQuery<Doctors>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    model.doctorList = data1;
                    ViewBag.Total = model.doctorList.Sum(a => a.Amount);
                    return View(model);
                }
                }
            return View(model);
        }

        public ActionResult MonthlyDoctor(string term, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDetails();
            var qry = @"SELECT DATENAME(month, A.AppointmentDate)as  AppointmentDate1, DATENAME(yy, A.AppointmentDate)as  Year,SUM(A.TotalFee) as Amount from PatientAppointment A Where Month(A.AppointmentDate) = Month(GetDate()) and A.IsPaid=1 GROUP BY DATENAME(month, A.AppointmentDate), DATENAME(yy, A.AppointmentDate) order by  Year('1' + DATENAME(MONTH, A.AppointmentDate) +'00')  , Year";
            var data = ent.Database.SqlQuery<Doctors>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Month";
            }
            else
            {
                model.doctorList = data;
                ViewBag.Total = model.doctorList.Sum(a => a.Amount);
            }
                if (sdate != null && edate != null)
            {
               //data = data.Where(A => A.AppointmentDate == sdate && A.AppointmentDate == edate).ToList();
                var qry1 = @"SELECT  DATENAME(month, A.AppointmentDate)as  AppointmentDate1, DATENAME(yy, A.AppointmentDate) as  Year ,SUM(A.Amount) as Amount from PatientAppointment A where A.AppointmentDate between '" + sdate + "' and '" + edate+ "' and A.IsPaid=1 GROUP BY DATENAME(month, A.AppointmentDate), DATENAME(yy, A.AppointmentDate) order by Year('1' + DATENAME(MONTH, A.AppointmentDate) +'00')  , Year";
                var data1 = ent.Database.SqlQuery<Doctors>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    model.doctorList = data1;
                    ViewBag.Total = model.doctorList.Sum(a => a.Amount);
                    return View(model);
                }
            }
           
            return View(model);
        }


        public ActionResult yearlyDoctor(string term, int? year)
        {
            var model = new ReportDetails();
            var qry = @"SELECT  DATENAME(YEAR, A.AppointmentDate)as  Year, SUM(A.TotalFee) as Amount from PatientAppointment A where YEAR(A.AppointmentDate) = YEAR(getdate()) and A.IsPaid=1 GROUP BY DATENAME(YEAR, A.AppointmentDate)";
            var data = ent.Database.SqlQuery<Doctors>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Year";
            }
           if (year != null)
            {
                var qry1 = @"SELECT  DATENAME(YEAR, A.AppointmentDate)as  Year, SUM(A.TotalFee) as Amount from PatientAppointment A where DATEPART(YY,A.AppointmentDate) = '"+year+ "' and A.IsPaid=1 GROUP BY DATENAME(YEAR, A.AppointmentDate)";
                var data1 = ent.Database.SqlQuery<Doctors>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Entered Year Doesn't Contain any Information.";
                }
               
                    model.doctorList = data1;
                    ViewBag.Total = model.doctorList.Sum(a => a.Amount);
                    return View(model);
               
            }
            model.doctorList = data;
            ViewBag.Total = model.doctorList.Sum(a => a.Amount);
            return View(model);
        }

        public ActionResult WeeklyDoctor(string term, DateTime? week)
        {
            var model = new ReportDetails();
            var qry = @"SELECT 'Week'+'-'+ DATENAME(WW, A.AppointmentDate)as  Weeks, DATENAME(YY,A.AppointmentDate) as Year, SUM(A.TotalFee) as Amount from PatientAppointment AS A where datepart(ww,A.AppointmentDate) =  datepart(ww, getdate()) and A.IsPaid=1 GROUP BY DATENAME(WW, A.AppointmentDate),DATENAME(YY, A.AppointmentDate)";
            var data = ent.Database.SqlQuery<Doctors>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Week";
            }
            else
            {
                model.doctorList = data;
                ViewBag.Total = model.doctorList.Sum(a => a.Amount);
            }

                if (week != null)
            {
                DateTime dateCriteria = week.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select 'Week'+'-'+ DATENAME(WW, P.AppointmentDate)as  Weeks, DATENAME(YY, P.AppointmentDate)as  Year, Sum(P.TotalFee) as Amount from PatientAppointment P where P.AppointmentDate between '" + dateCriteria + "' and '"+week+"' GROUP BY P.AppointmentDate, P.TotalFee";
                var data1 = ent.Database.SqlQuery<Doctors>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    model.doctorList = data1;
                    ViewBag.Total = model.doctorList.Sum(a => a.Amount);
                    return View(model);
                }
            }
          
            return View(model);
        }


        public ActionResult Lab()
        {
            return View();
        }

        public ActionResult DailyLab(string term, DateTime? date)
        {
            var model = new ReportDetails();
            var qry = @"SELECT CONVERT(DATE, P.TestDate) AS TestDate, SUM(P.Amount) AS Amount
FROM BookTestLab P
WHERE CONVERT(DATE, P.TestDate) >= CAST(GETDATE() AS DATE)
GROUP BY CONVERT(DATE, P.TestDate)";
            var data = ent.Database.SqlQuery<Labs>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.LabList = data;
                ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
                if (date != null)
            {
                string dt = date.Value.ToString("MM/dd/yyyy");
                var qry1 = @"select P.TestDate, Sum(P.Amount) as Amount from BookTestLab P where P.TestDate = '"+dt+"'  GROUP BY P.TestDate, P.Amount";
                var data1 = ent.Database.SqlQuery<Labs>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    model.LabList = data1;
                    ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
           
            return View(model);
        }

        public ActionResult MonthlyLab(string term, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDetails();
            var qry = @"SELECT  DATENAME(month, A.TestDate)as  TestDate1, DATENAME(yy, A.TestDate)as  Year,SUM(A.Amount) as Amount from BookTestLab  A where Month(A.TestDate) = Month(GETDATE()) GROUP BY DATENAME(month, A.TestDate), DATENAME(yy, A.TestDate) order by  Year('1' + DATENAME(MONTH, A.TestDate) +'00')  , Year";
            var data = ent.Database.SqlQuery<Labs>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Month";
            }
            else
            {
                model.LabList = data;
                ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
                if (sdate != null && edate != null)
            {
                var qry1 = @"SELECT  DATENAME(month, A.TestDate)as  TestDate1, DATENAME(yy, A.TestDate)as  Year,SUM(A.Amount) as Amount from BookTestLab  A where A.TestDate between '" + sdate + "' and '" + edate + "' GROUP BY DATENAME(month, A.TestDate), DATENAME(yy, A.TestDate)  order by  Year('1' + DATENAME(MONTH, A.TestDate) +'00')  , Year";
                var data1 = ent.Database.SqlQuery<Labs>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    model.LabList = data1;
                    ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
           
            return View(model);
        }

        public ActionResult yearlyLab(string term, int? year)
        {
            var model = new ReportDetails();
            var qry = @"SELECT  DATENAME(YEAR, A.TestDate)as  Year, SUM(A.Amount) as Amount from BookTestLab AS A WHERE  YEAR(A.TestDate) = YEAR(getdate())  GROUP BY DATENAME(YEAR, A.TestDate)";
            var data = ent.Database.SqlQuery<Labs>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Year";
            }
           
            
            
            if (year != null)
            {
                var qry1 = @"SELECT  DATENAME(YEAR, A.TestDate)as  Year, SUM(A.Amount) as Amount from BookTestLab A where DATEPART(YY,A.TestDate) = '" + year + "' GROUP BY DATENAME(YEAR, A.TestDate)";
                var data1 = ent.Database.SqlQuery<Labs>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Entered Year Doesn't Contain any Information.";
                }
                    model.LabList = data1;
                    ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
            }
            model.LabList = data;
            ViewBag.Total = model.LabList.Sum(a => a.Amount);
            return View(model);
        }

        public ActionResult WeeklyLab(string term, DateTime? week)
        {
            var model = new ReportDetails();
            var qry = @"SELECT 'Week'+'-'+ DATENAME(WW, A.TestDate)as  Weeks, DATENAME(YY,A.TestDate) as Year, SUM(A.Amount) as Amount from BookTestLab A where datepart(ww,A.TestDate) =  datepart(ww, getdate()) GROUP BY DATENAME(WW, A.TestDate),DATENAME(YY, A.TestDate)";
            var data = ent.Database.SqlQuery<Labs>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Week";
            }
            else
            {
                model.LabList = data;
                ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (week != null)
            {
                DateTime dateCriteria = week.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select 'Week'+'-'+ DATENAME(WW, P.TestDate)as  Weeks, DATENAME(YY, P.TestDate)as  Year, Sum(P.Amount) as Amount from BookTestLab P where P.TestDate between '" + dateCriteria + "' and '" + week + "' GROUP BY P.TestDate, P.Amount";
                var data1 = ent.Database.SqlQuery<Labs>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    model.LabList = data1;
                    ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult Chemist()
        {
            return View();
        }

        public ActionResult DailyChe(string term, DateTime? date)
        {
            var model = new ReportDetails();
            var qry = @"select convert(date, MedicineOrder.OrderDate) as OrderDate, Sum(MedicineOrderDetail.Amount) as Amount from MedicineOrderDetail join MedicineOrder on MedicineOrderDetail.Order_Id = MedicineOrder.Id where Convert(Date,MedicineOrder.OrderDate) = Convert(Date,getdate()) and MedicineOrder.IsPaid=1 group by MedicineOrder.OrderDate order by MedicineOrder.OrderDate desc";
            var data = ent.Database.SqlQuery<Chemists>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.ChemistsList = data;
                ViewBag.Total = model.ChemistsList.Sum(a => a.Amount);
            }
                if (date != null)
            {
                string dt = date.Value.ToString("MM/dd/yyyy");
                var qry1 = @"select convert(date, MedicineOrder.OrderDate) as OrderDate, Sum(MedicineOrderDetail.Amount) as Amount from MedicineOrderDetail join MedicineOrder on MedicineOrderDetail.Order_Id = MedicineOrder.Id where  convert(date, MedicineOrder.OrderDate) = '"+dt+"' group by MedicineOrder.OrderDate order by MedicineOrder.OrderDate desc";
                var data1 = ent.Database.SqlQuery<Chemists>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    model.ChemistsList = data1;
                    ViewBag.Total = model.ChemistsList.Sum(a => a.Amount);
                    return View(model);
                }
            }
           
            return View(model);
        }

        public ActionResult MonthlyChe(string term, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDetails();
            var qry = @"SELECT  DATENAME(month, MedicineOrder.OrderDate)as  OrderDate1, DATENAME(yy, MedicineOrder.OrderDate)as  Year,SUM(MedicineOrderDetail.Amount) as Amount from MedicineOrderDetail join MedicineOrder on MedicineOrderDetail.Order_Id = MedicineOrder.Id  where MONTH(OrderDate) = Month(getdate()) and MedicineOrder.IsPaid=1 GROUP BY DATENAME(month, MedicineOrder.OrderDate), DATENAME(yy, MedicineOrder.OrderDate)  order by  Year('1' + DATENAME(MONTH, MedicineOrder.OrderDate) +'00')  , Year";
            var data = ent.Database.SqlQuery<Chemists>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Month";
            }
            else
            {
                model.ChemistsList = data;
                ViewBag.Total = model.ChemistsList.Sum(a => a.Amount);
            }
            if (sdate != null && edate != null)
            {
                var qry1 = @"SELECT  DATENAME(month, MedicineOrder.OrderDate)as  OrderDate1, DATENAME(yy, MedicineOrder.OrderDate)as  Year,SUM(MedicineOrderDetail.Amount) as Amount from MedicineOrderDetail join MedicineOrder on MedicineOrderDetail.Order_Id = MedicineOrder.Id  where MedicineOrder.OrderDate between '" + sdate + "' and '" + edate + "' GROUP BY DATENAME(month, MedicineOrder.OrderDate), DATENAME(yy, MedicineOrder.OrderDate)  order by  Year('1' + DATENAME(MONTH, MedicineOrder.OrderDate) +'00')  , Year";
                var data1 = ent.Database.SqlQuery<Chemists>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    model.ChemistsList = data1;
                    ViewBag.Total = model.ChemistsList.Sum(a => a.Amount);
                    return View(model);
                }
                return View(model);
            }
           
            return View(model);
        }

        public ActionResult yearlyChe(string term, int? year)
        {
            var model = new ReportDetails();
            var qry = @"SELECT  DATENAME(YEAR, MedicineOrder.OrderDate)as  Year, SUM(MedicineOrderDetail.Amount) as Amount from MedicineOrderDetail join MedicineOrder on MedicineOrderDetail.Order_Id = MedicineOrder.Id where Year(MedicineOrder.OrderDate) = YEAR(getdate()) and MedicineOrder.IsPaid=1 GROUP BY DATENAME(YEAR, MedicineOrder.OrderDate)";
            var data = ent.Database.SqlQuery<Chemists>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Year";
            }
            
            if (year != null)
            {
                var qry1 = @"SELECT  DATENAME(YEAR, MedicineOrder.OrderDate)as  Year, SUM(MedicineOrderDetail.Amount) as Amount from MedicineOrderDetail join MedicineOrder on MedicineOrderDetail.Order_Id = MedicineOrder.Id WHERE DATEPART(YY, MedicineOrder.OrderDate) = '" + year + "'  GROUP BY DATENAME(YEAR, MedicineOrder.OrderDate)";
                var data1 = ent.Database.SqlQuery<Chemists>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Entered Year Doesn't Contain any Information.";
                }
                
                    model.ChemistsList = data1;
                    ViewBag.Total = model.ChemistsList.Sum(a => a.Amount);
                    return View(model);
            }
            model.ChemistsList = data;
            ViewBag.Total = model.ChemistsList.Sum(a => a.Amount);
            return View(model);
        }

        public ActionResult WeeklyChe(string term, DateTime? week)
        {
            var model = new ReportDetails();
            var qry = @"SELECT 'Week'+'-'+ DATENAME(WW, MedicineOrder.OrderDate)as  Weeks, DATENAME(YY,MedicineOrder.OrderDate) as Year, SUM(MedicineOrderDetail.Amount) as Amount from MedicineOrderDetail join MedicineOrder on MedicineOrderDetail.Order_Id = MedicineOrder.Id where datepart(ww,MedicineOrder.OrderDate)=datepart(ww, getdate()) and MedicineOrder.IsPaid=1  GROUP BY DATENAME(WW, MedicineOrder.OrderDate),DATENAME(YY, MedicineOrder.OrderDate)";
            var data = ent.Database.SqlQuery<Chemists>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Week";
            }
            else
            {
                model.ChemistsList = data;
                ViewBag.Total = model.ChemistsList.Sum(a => a.Amount);
            }
            if (week != null)
            {
                DateTime dateCriteria = week.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"SELECT 'Week'+'-'+ DATENAME(WW, MedicineOrder.OrderDate)as  Weeks, DATENAME(YY,MedicineOrder.OrderDate) as Year, SUM(MedicineOrderDetail.Amount) as Amount from MedicineOrderDetail join MedicineOrder on MedicineOrderDetail.Order_Id = MedicineOrder.Id WHERE MedicineOrder.OrderDate  between '" + dateCriteria + "' and '" + week + "' GROUP BY DATENAME(WW, MedicineOrder.OrderDate),DATENAME(YY, MedicineOrder.OrderDate)";
                var data1 = ent.Database.SqlQuery<Chemists>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    model.ChemistsList = data1;
                    ViewBag.Total = model.ChemistsList.Sum(a => a.Amount);
                    return View(model);
                }
                return View(model);
            }
            return View(model);
        }


        public ActionResult Health()
        {
            return View();
        }

        public ActionResult DailyHealth(string term, DateTime? date)
        {
            var model = new ReportDetails();
            var qry = @"select P.TestDate, Sum(P.Amount) as Amount from CmpltCheckUp P where P.IsTaken = 1 and Convert(Date,P.TestDate) = Convert(Date,GetDate()) GROUP BY P.TestDate, P.Amount";
            var data = ent.Database.SqlQuery<Labs>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.LabList = data;
                ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (date != null)
            {
                string dt = date.Value.ToString("MM/dd/yyyy");
                var qry1 = @"select P.TestDate, Sum(P.Amount) as Amount from CmpltCheckUp P where P.IsTaken = 1 and Convert(Date,P.TestDate) = '" + dt + "'  GROUP BY P.TestDate, P.Amount";
                var data1 = ent.Database.SqlQuery<Labs>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    model.LabList = data1;
                    ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }

            return View(model);
        }

        public ActionResult MonthlyHealth(string term, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDetails();
            var qry = @"SELECT  DATENAME(month, A.TestDate)as  TestDate1, DATENAME(yy, A.TestDate)as  Year,SUM(A.Amount) as Amount from CmpltCheckUp  A where Month(A.TestDate) = Month(GETDATE()) GROUP BY DATENAME(month, A.TestDate), DATENAME(yy, A.TestDate) order by  Year('1' + DATENAME(MONTH, A.TestDate) +'00')  , Year";
            var data = ent.Database.SqlQuery<Labs>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Month";
            }
            else
            {
                model.LabList = data;
                ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (sdate != null && edate != null)
            {
                var qry1 = @"SELECT  DATENAME(month, A.TestDate)as  TestDate1, DATENAME(yy, A.TestDate)as  Year,SUM(A.Amount) as Amount from CmpltCheckUp  A where A.TestDate between '" + sdate + "' and '" + edate + "' GROUP BY DATENAME(month, A.TestDate), DATENAME(yy, A.TestDate)  order by  Year('1' + DATENAME(MONTH, A.TestDate) +'00')  , Year";
                var data1 = ent.Database.SqlQuery<Labs>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    model.LabList = data1;
                    ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }

            return View(model);
        }

        public ActionResult yearlyHealth(string term, int? year)
        {
            var model = new ReportDetails();
            var qry = @"SELECT  DATENAME(YEAR, A.TestDate)as  Year, SUM(A.Amount) as Amount from CmpltCheckUp AS A WHERE  YEAR(A.TestDate) = YEAR(getdate())  GROUP BY DATENAME(YEAR, A.TestDate)";
            var data = ent.Database.SqlQuery<Labs>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Year";
            }



            if (year != null)
            {
                var qry1 = @"SELECT  DATENAME(YEAR, A.TestDate)as  Year, SUM(A.Amount) as Amount from CmpltCheckUp A where DATEPART(YY,A.TestDate) = '" + year + "' GROUP BY DATENAME(YEAR, A.TestDate)";
                var data1 = ent.Database.SqlQuery<Labs>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Entered Year Doesn't Contain any Information.";
                }
                model.LabList = data1;
                ViewBag.Total = model.LabList.Sum(a => a.Amount);
                return View(model);
            }
            model.LabList = data;
            ViewBag.Total = model.LabList.Sum(a => a.Amount);
            return View(model);
        }

        public ActionResult WeeklyHealth(string term, DateTime? week)
        {
            var model = new ReportDetails();
            var qry = @"SELECT 'Week'+'-'+ DATENAME(WW, A.TestDate)as  Weeks, DATENAME(YY,A.TestDate) as Year, SUM(A.Amount) as Amount from CmpltCheckUp A where datepart(ww,A.TestDate) =  datepart(ww, getdate()) GROUP BY DATENAME(WW, A.TestDate),DATENAME(YY, A.TestDate)";
            var data = ent.Database.SqlQuery<Labs>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Week";
            }
            else
            {
                model.LabList = data;
                ViewBag.Total = model.LabList.Sum(a => a.Amount);
            }
            if (week != null)
            {
                DateTime dateCriteria = week.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select 'Week'+'-'+ DATENAME(WW, P.TestDate)as  Weeks, DATENAME(YY, P.TestDate)as  Year, Sum(P.Amount) as Amount from CmpltCheckUp P where P.TestDate between '" + dateCriteria + "' and '" + week + "' GROUP BY P.TestDate, P.Amount";
                var data1 = ent.Database.SqlQuery<Labs>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    model.LabList = data1;
                    ViewBag.Total = model.LabList.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }



        //Nurse Section for Yearly, Monthly, Weekly & Daily Report
        public ActionResult Nurse(int? Id)
        {
            var model = new NurseReport();
            model.NurseTypeList = new SelectList(ent.NurseTypes.ToList(), "Id", "NurseTypeName");
            var Nurse = @"select P.Id, P.NurseName, d.NurseTypeName from Nurse P join NurseType D ON d.Id = p.NurseType_Id join NurseService ns on ns.Nurse_Id = P.Id where d.Id='" + Id + "' group by  P.NurseName, d.NurseTypeName,P.Id";
            var data = ent.Database.SqlQuery<NurseNameList>(Nurse).ToList();
            model.NursesList = data;
            return View(model);
        }

        public ActionResult Nurses()
        {
            return View();
        }


        public ActionResult DailyNurse(string term, DateTime? date)
        {
            var model = new ReportDetails();
            var qry = @"select P.ServiceAcceptanceDate,  P.TotalFee from NurseService P
join Nurse D on D.Id = p.Nurse_Id 
where CONVERT(DATE, P.ServiceAcceptanceDate) >= CAST(GETDATE() AS DATE)
and P.IsPaid=1 GROUP BY P.ServiceAcceptanceDate, P.TotalFee";
            var data = ent.Database.SqlQuery<Nurses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Date";
            }
            else
            {
                model.Nurses = data;
                ViewBag.Total = model.Nurses.Sum(a => a.TotalFee);
            }
            if (date != null)
            {
                string dt = date.Value.ToString("MM/dd/yyyy");
                  var qry1 = @"select P.ServiceAcceptanceDate,  P.TotalFee from NurseService P join Nurse D on D.Id = p.Nurse_Id where P.ServiceAcceptanceDate = '" + dt + "' and P.IsPaid = 1 GROUP BY P.ServiceAcceptanceDate, P.TotalFee";
                var data1 = ent.Database.SqlQuery<Nurses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    model.Nurses = data1;
                    ViewBag.Total = model.Nurses.Sum(a => a.TotalFee);
                    return View(model);
                }
            }

            return View(model);
        }

        public ActionResult MonthlyNurse(string term, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDetails();
            var qry = @"select  P.ServiceAcceptanceDate,  p.TotalFee from NurseService P join Nurse D on D.Id = p.Nurse_Id where Month(p.ServiceAcceptanceDate) = Month(GetDate()) GROUP BY P.ServiceAcceptanceDate, P.TotalFee";
            var data = ent.Database.SqlQuery<Nurses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Month";
            }
            else
            {
                model.Nurses = data;
                ViewBag.Total = model.Nurses.Sum(a => a.TotalFee);
            }
            if (sdate != null && edate != null)
            {
               
                var qry1 = @" select P.ServiceAcceptanceDate, P.TotalFee from NurseService P join Nurse D on D.Id = p.Nurse_Id where p.ServiceAcceptanceDate) between'" + sdate + "' and '" + edate + "' GROUP BY P.ServiceAcceptanceDate, P.TotalFee";
                var data1 = ent.Database.SqlQuery<Nurses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    model.Nurses = data1;
                    ViewBag.Total = model.Nurses.Sum(a => a.TotalFee);
                    return View(model);
                }
            }

            return View(model);
        }

        public ActionResult yearlyNurse(string term, int? year)
        {
            var model = new ReportDetails();
            var qry = @"select  P.ServiceAcceptanceDate, p.TotalFee from NurseService P join Nurse D on D.Id = p.Nurse_Id where Year(p.ServiceAcceptanceDate) = Year(GetDate()) GROUP BY P.ServiceAcceptanceDate, P.TotalFee";
            var data = ent.Database.SqlQuery<Nurses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Year";
            }
            if (year != null)
            {
                var qry1 = @"select P.ServiceAcceptanceDate, P.TotalFee from NurseService P join Nurse D on D.Id = p.Nurse_Id where DATEPART(YY, A.ServiceAcceptanceDate) = '" + year + "' GROUP BY P.ServiceAcceptanceDate, P.TotalFee";
                var data1 = ent.Database.SqlQuery<Nurses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Entered Year Doesn't Contain any Information.";
                }
                model.Nurses = data1;
                ViewBag.Total = model.Nurses.Sum(a => a.TotalFee);
                return View(model);
            }
            model.Nurses = data;
            ViewBag.Total = model.Nurses.Sum(a => a.TotalFee);
            return View(model);
        }

        public ActionResult WeeklyNurse(string term, DateTime? week)
        {
            var model = new ReportDetails();
            var qry = @"SELECT P.ServiceAcceptanceDate,  P.TotalFee from NurseService P join Nurse D on D.Id = p.Nurse_Id where datepart(ww,P.ServiceAcceptanceDate) =  datepart(ww, getdate()) GROUP BY P.ServiceAcceptanceDate, P.TotalFee";
            var data = ent.Database.SqlQuery<Nurses>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record of Current Week";
            }
            else
            {
                model.Nurses = data;
                ViewBag.Total = model.Nurses.Sum(a => a.TotalFee);
            }
            if (week != null)
            {
                DateTime dateCriteria = week.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"SELECT P.ServiceAcceptanceDate,  P.TotalFee from NurseService P join Nurse D on D.Id = p.Nurse_Id where P.ServiceAcceptanceDate between '" + dateCriteria+"' and '"+week+ "' GROUP BY P.ServiceAcceptanceDate, P.TotalFee";
                var data1 = ent.Database.SqlQuery<Nurses>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    model.Nurses = data1;
                    ViewBag.Total = model.Nurses.Sum(a => a.TotalFee);
                    return View(model);
                }
            }
            return View(model);
        }
    }
}