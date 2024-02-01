using AutoMapper;
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
    public class ExpenseController : Controller
    {
        DbEntities ent = new DbEntities();
        // GET: Expense
        public ActionResult Expense()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Expense(ExpenseReportDTO model)
        {
            using(var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return View(model);
                    }                  
                    var domain = Mapper.Map<ExpenseReport>(model);
                    domain.IsDeleted = false;
                    domain.IsPaid = false;
                    ent.ExpenseReports.Add(domain);
                    ent.SaveChanges();
                    TempData["msg"] = "Saved!";
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    string msg = ex.ToString();
                    TempData["msg"] = "Server Error";
                    tran.Rollback();
                }
            }
            return View(model);
        }


        public ActionResult ViewExpenseDetail(DateTime? TransactionDate, string term, string Transaction_Type = null, DateTime? startdate = null, DateTime? enddate = null)
        {
            try
            {

           
            var model = new ExpenseReportDTO();

            if(term==null|| term == "")
            {
                string q = @"select * from ExpenseReport where IsPaid=0";
                var data = ent.Database.SqlQuery<ExpenseList>(q).ToList();

                if (TransactionDate != null)
                {
                    data = data.Where(a => a.TransactionDate == TransactionDate).ToList();
                    if (data.Count() == 0)
                    {
                        TempData["msg"] = "No Records";
                        return View(model);
                    }
                }

                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Records";
                    return View(model);
                }
                if (startdate != null && enddate != null)
                {
                    data = data.Where(x => x.TransactionDate >= startdate && x.TransactionDate <= enddate).ToList();
                    if (data.Count() == 0)
                    {
                        TempData["msg"] = "No Records";
                        return View(model);
                    }
                }
                if (TransactionDate != null)
                {
                    data = data.Where(a => a.TransactionDate == TransactionDate).ToList();
                    if (data.Count() == 0)
                    {
                        TempData["msg"] = "No Records";
                        return View(model);
                    }
                }
                model.ExpenselList = data;
            }

            else if (term == "Daily")
            {
                string qry = @"select Expense,Type_of_Expense,Transaction_Type,Amount,TransactionDate from ExpenseReport where Convert(Date,TransactionDate) = Convert(Date,GETDATE()) and IsPaid=0";
                var data = ent.Database.SqlQuery<ExpenseList>(qry).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Records";
                    //return View(data);
                }
                    model.ExpenselList = data;
                }
            else if (term == "Monthly")
            {
               // string qry = @"SELECT DATENAME(month,TransactionDate)as  TransactionDate, DATENAME(yy,TransactionDate)as  Year,Expense,Type_of_Expense,Transaction_Type,Amount,TransactionDate from ExpenseReport Where Month(TransactionDate) = Month(GetDate()) and IsPaid=0";
                string qry = @"SELECT Expense,Type_of_Expense,Transaction_Type,Amount,TransactionDate from ExpenseReport Where Month(TransactionDate) = Month(GetDate()) and IsPaid=0";
                var data = ent.Database.SqlQuery<ExpenseList>(qry).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Records";
                    //return View(data);
                }
                    model.ExpenselList = data;
                }
            else if (term == "Yearly")
            {
                string qry = @"SELECT  DATENAME(YEAR,TransactionDate)as  Year ,Expense,Type_of_Expense,Transaction_Type,Amount,TransactionDate from ExpenseReport where YEAR(TransactionDate) = YEAR(getdate()) and IsPaid=0";
                var data = ent.Database.SqlQuery<ExpenseList>(qry).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Records";
                    return View(data);
                }
                    model.ExpenselList = data;
                }
            else if (term == "Weekly")
            {
                string qry = @"SELECT 'Week'+'-'+ DATENAME(WW, TransactionDate)as  Weeks, DATENAME(YY,TransactionDate) as Year,Expense,Type_of_Expense,Transaction_Type,Amount,TransactionDate  from ExpenseReport where datepart(ww,TransactionDate) =  datepart(ww, getdate()) and IsPaid=0";
                var data = ent.Database.SqlQuery<ExpenseList>(qry).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Records";
                    //return View(data);
                }
                    model.ExpenselList = data;
                }
                
           ViewBag.termselect=term;
            return View(model); 
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View();
        }

      
    }
}