using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SalaryMasterController : Controller
    {
        DbEntities ent = new DbEntities();
        
        // GET: SalaryMaster
        public ActionResult Employee(string term = null)
        {
            var model = new EmployeeNameVM();
            string q = @"select Id, Salary, EmployeeId,EmployeetName from Employee where IsDeleted=0";
            var data = ent.Database.SqlQuery<Emp_Detail>(q).ToList();
            if(data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            if (term != null)
            {
                data = data.Where(A => A.EmployeetName.ToLower().Contains(term)).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Match";
                    return View(model);
                }
            }
            model.Emp_Detail = data;
            return View(model);
        }

        public JsonResult SaveSalary(int Id, double salary)
        {
            var model = new SalaryMaster();
            model.Emp_Id = Id;
            model.Salary = salary;
            model.IsPaid = false;
            model.IsGenerated = true;
            model.GenerateDate = DateTime.Now;
            ent.SalaryMasters.Add(model);
            ent.SaveChanges();

            string query = @"select top 1 id from SalaryMaster where emp_id="+ Id + " order by GenerateDate desc";
            var employeeDetails = ent.Database.SqlQuery<Emp_Detail>(query).FirstOrDefault();
            return Json(employeeDetails.Id);
        }

        public ActionResult ViewHistory(string term = null,string year=null, string Month= null)
        {
            var model = new EmployeeNameVM();
            Session["year"] = year;
            Session["month"] = Month;
            ViewBag.year = year;
            ViewBag.month = Month;
            string q = @"select sm.Id, emp.EmployeeId, emp.Salary as CurrentSalary, sm.Salary as MonthSalary, sm.GenerateDate, sm.IsPaid, emp.EmployeetName, emp.EmployeetName from SalaryMaster sm join Employee emp on emp.Id = sm.Emp_Id where IsGenerated = 1";

            if (!string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(Month))
            {
                q += " AND format(GenerateDate,'yyyy-MM')='" + year + "-" + Month + "'";
            }

            var data = ent.Database.SqlQuery<Emp_Detail>(q).ToList();

            // string q = @"select sm.Id, emp.EmployeeId, emp.Salary as CurrentSalary, sm.Salary as MonthSalary, sm.GenerateDate, sm.IsPaid, emp.EmployeetName, emp.EmployeetName from SalaryMaster sm join Employee emp on emp.Id = sm.Emp_Id where Month(sm.GenerateDate) = Month(GetDate()) and IsGenerated=1";

            //string q = @"select sm.Id, emp.EmployeeId, emp.Salary as CurrentSalary, sm.Salary as MonthSalary, sm.GenerateDate, sm.IsPaid, emp.EmployeetName, emp.EmployeetName from SalaryMaster sm join Employee emp on emp.Id = sm.Emp_Id where format(GenerateDate,'yyyy-MM')='"+ year + "-"+ Month + "' and IsGenerated=1";

            //var data = ent.Database.SqlQuery<Emp_Detail>(q).ToList();
            if(data.Count() == 0)
            {
                TempData["msg"] = "No Records of this Month";
                return View(model);
            }
            //if(Month != null)
            //{
            //    string q1 = @"select sm.Id,emp.EmployeeId, emp.Salary as CurrentSalary, sm.Salary as MonthSalary, sm.GenerateDate, sm.IsPaid, emp.EmployeetName, emp.EmployeetName from SalaryMaster sm join Employee emp on emp.Id = sm.Emp_Id where Month(sm.GenerateDate) = " + Month + " and IsGenerated=1";
            //    var data1 = ent.Database.SqlQuery<Emp_Detail>(q1).ToList();
            //    if (data1.Count() == 0)
            //    {
            //        TempData["msg"] = "No Records of this Month";
            //        return View(model);
            //    }
            //    model.Emp_Detail = data1;
            //    return View(model);
            //}
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower(); // Convert the search term to lowercase.
                data = data.Where(A => A.EmployeetName.ToLower().Contains(term)).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Match";
                    return View(model);
                }
            }


            model.Emp_Detail = data;
            return View(model);
        }

        public ActionResult DownloadEmpExcel(string term = null, string year = null, string Month = null)
        {
             year = (string)Session["Year"];
             Month = (string)Session["Month"];
            var model = new EmployeeNameVM();
            string q = @"select sm.Id, emp.EmployeeId, emp.Salary as CurrentSalary, sm.Salary as MonthSalary, sm.GenerateDate, sm.IsPaid, emp.EmployeetName, emp.EmployeetName from SalaryMaster sm join Employee emp on emp.Id = sm.Emp_Id where IsGenerated = 1";

            if (!string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(Month))
            {
                q += " AND format(GenerateDate,'yyyy-MM')='" + year + "-" + Month + "'";
            }

            var data = ent.Database.SqlQuery<Emp_Detail>(q).ToList();

            // Create a new Excel package
            using (var package = new ExcelPackage())
            {
                // Add a worksheet
                var worksheet = package.Workbook.Worksheets.Add("EmployeeData");

                // Define the columns in the Excel sheet
              
                worksheet.Cells[1, 1].Value = "EmployeeId";
                worksheet.Cells[1, 2].Value = "CurrentSalary";
                worksheet.Cells[1, 3].Value = "MonthSalary";
                worksheet.Cells[1, 4].Value = "GenerateDate";
                worksheet.Cells[1, 5].Value = "IsPaid";
                worksheet.Cells[1, 6].Value = "EmployeetName";

                // Populate the data from your model into the Excel sheet
                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].EmployeeId.ToString(); // Convert to string
                    worksheet.Cells[i + 2, 2].Value = data[i].CurrentSalary.ToString();  
                    worksheet.Cells[i + 2, 3].Value = data[i].MonthSalary.ToString();  
                    worksheet.Cells[i + 2, 4].Value = data[i].GenerateDate.ToString("yyyy-MM-dd"); // Convert to string with specific format
                    worksheet.Cells[i + 2, 5].Value = data[i].IsPaid.ToString();  
                    worksheet.Cells[i + 2, 6].Value = data[i].EmployeetName;  
                }


                // Set the content type and headers for the response
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=EmployeeData.xlsx");

                // Write the Excel file to the response
                Response.BinaryWrite(package.GetAsByteArray());
                Response.End();
            }
            return View(model);

            // You can return a view or redirect to another action after the download if needed.
        }

        public ActionResult UpdatePayment(int id)
        {
            DateTime date = DateTime.Now;
            string q = @"update SalaryMaster set PaidDate='" + date+"' , IsPaid = 1 where Id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("ViewHistory");
        }

        public void DownloadExcel(int? Id)
        {
            double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Employee'").FirstOrDefault();
            string query = "";
            if (Id != null)
            {
                query = $"SELECT Top 1 SM.Id, E.EmployeeId, E.Salary, E.EmployeetName, E.EmailId,{tds} as TDS, SM.Salary-(SM.Salary *{tds}/100) as MonthSalary, SM.GenerateDate FROM Employee as E inner join SalaryMaster SM on SM.Emp_Id = E.Id where SM.Id=" + Id;
            }
            else if (Id == null)
            {
                query = $"SELECT SM.Id, E.EmployeeId, E.Salary, E.EmployeetName, E.EmailId,{tds} as TDS, SM.Salary-(SM.Salary *{tds}/100) as MonthSalary, SM.GenerateDate FROM Employee as E inner join SalaryMaster SM on SM.Emp_Id = E.Id";
            }

            var employeeDetails = ent.Database.SqlQuery<Emp_Detail>(query).ToList();
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");

            Sheet.Cells["A1"].Value = "EMPId";
            Sheet.Cells["B1"].Value = "Name";
            Sheet.Cells["C1"].Value = "Salary";
            Sheet.Cells["D1"].Value = "MonthSalary";
            Sheet.Cells["E1"].Value = "EmailId";
            Sheet.Cells["F1"].Value = "GenerateDate";
            Sheet.Cells["G1"].Value = "TDS";
            int row = 2;
            double totalMonthSalary = 0.0; // Initialize a variable to store the total MonthSalary

            foreach (var item in employeeDetails)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.EmployeeId;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.EmployeetName;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Salary;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.MonthSalary;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.EmailId;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.GenerateDate;
                Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "yyyy-MM-dd"; // Change the date format as needed
                Sheet.Cells[string.Format("G{0}", row)].Value = tds;

                totalMonthSalary += item.MonthSalary; // Add the current MonthSalary to the total
                row++;
            }

            // Create a cell to display the total MonthSalary
            Sheet.Cells[string.Format("C{0}", row)].Value = "Total MonthSalary";
            Sheet.Cells[string.Format("D{0}", row)].Value = totalMonthSalary;

            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=Report.xlsx"); // Use a semicolon (;) instead of a colon (:)
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}