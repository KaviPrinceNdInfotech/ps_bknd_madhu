using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        public void DownloadDoctorExcel(int? Id)
        {
			double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Doctor'").FirstOrDefault();
			double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault();
			double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault();

			string query = @"SELECT D.DoctorId,A.Doctor_Id,D.DoctorName,SUM(A.TotalFee) AS Amount FROM dbo.PatientAppointment A
JOIN Doctor D ON D.Id = A.Doctor_Id WHERE  A.IsPaid = 1  AND A.AppointmentDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE()
GROUP BY  D.DoctorId, A.Doctor_Id,D.DoctorName;";

            var DoctorCommissionReport = ent.Database.SqlQuery<DoctorCommissionReport>(query).ToList();
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");

            Sheet.Cells["A1"].Value = "Doctor Id";
            Sheet.Cells["B1"].Value = "Doctor Name";
            Sheet.Cells["C1"].Value = "Basic Amount";
			Sheet.Cells["D1"].Value = "Transaction Amount " + Transactionfee + " %";
			Sheet.Cells["E1"].Value = "Commission Amount " + commision + " %";
			Sheet.Cells["F1"].Value = "TDS Amount " + tds + " %";
			Sheet.Cells["G1"].Value = "Payable amount"; 
            int row = 2;

			double totalAmount = 0.0;
			double totalPayableAmount = 0.0;
			double totalCommAmount = 0.0;
			double totalTransactionAmount = 0.0;
			double totalTdsAmount = 0.0;

			foreach (var item in DoctorCommissionReport)
            {
				double commissionamt = (item.Amount * commision) / 100;
				double transactionamt = (item.Amount * Transactionfee) / 100;
				double tdsamt = (item.Amount * tds) / 100;
				double payableamt = item.Amount - (commissionamt + transactionamt + tdsamt);

				Sheet.Cells[string.Format("A{0}", row)].Value = item.DoctorId;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.DoctorName;
				Sheet.Cells[string.Format("C{0}", row)].Value = item.Amount;
				Sheet.Cells[string.Format("D{0}", row)].Value = transactionamt;
				Sheet.Cells[string.Format("E{0}", row)].Value = commissionamt;
				Sheet.Cells[string.Format("F{0}", row)].Value = tdsamt;
				Sheet.Cells[string.Format("G{0}", row)].Value = payableamt;


				totalAmount += item.Amount;
				totalPayableAmount += payableamt;
				totalTdsAmount += tdsamt;
				totalCommAmount += commissionamt;
				totalTransactionAmount += transactionamt;
				row++;
            }

			// Create a cell to display the total MonthSalary
			Sheet.Cells[string.Format("C{0}", row)].Value = totalAmount;
			Sheet.Cells[string.Format("D{0}", row)].Value = totalTransactionAmount;
			Sheet.Cells[string.Format("E{0}", row)].Value = totalCommAmount;
			Sheet.Cells[string.Format("F{0}", row)].Value = totalTdsAmount;
			Sheet.Cells[string.Format("G{0}", row)].Value = totalPayableAmount;

			Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=Report.xlsx"); // Use a semicolon (;) instead of a colon (:)
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public ActionResult Doctor(string term, int? pageNumber,DateTime? startdate, DateTime? enddate, string name = null)
        {
            double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Doctor'").FirstOrDefault();
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='" + term + "'").FirstOrDefault();
            var model = new ReportDTO();
            if (enddate != null)
            {
                DateTime dateCriteria = enddate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy"); 
                var qry1 = @"SELECT D.DoctorId,A.Doctor_Id,D.DoctorName,SUM(A.TotalFee) AS Amount FROM dbo.PatientAppointment A
JOIN Doctor D ON D.Id = A.Doctor_Id
WHERE A.IsPaid = 1 AND CONVERT(DATETIME, A.AppointmentDate, 103) BETWEEN @startdate AND @enddate 
                    GROUP BY D.DoctorId, A.Doctor_Id, D.DoctorName";
                //var data1 = ent.Database.SqlQuery<DoctorCommissionReport>(qry1).ToList();
                var data1 = ent.Database.SqlQuery<DoctorCommissionReport>(qry1,
            new SqlParameter("startdate", startdate),
            new SqlParameter("enddate", enddate)).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
					ViewBag.Transactionfee = Transactionfee;
					ViewBag.Commission = commision;
                    ViewBag.tds = tds;

                    int total = data1.Count;
                    pageNumber = (int?)pageNumber ?? 1;
                    int pageSize = 10;
                    decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                    model.TotalPages = (int)noOfPages;
                    model.PageNumber = (int)pageNumber;
                    data1 = data1.OrderBy(a => a.Doctor_Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                    if (name != null)
                    {
                        data1 = data1.Where(a => a.DoctorName.ToLower().Contains(name.ToLower())).ToList();
                    }
                    model.DoctorCommisionReport = data1;

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
				var doctor = @"SELECT D.DoctorId,A.Doctor_Id,D.DoctorName,SUM(A.TotalFee) AS Amount FROM dbo.PatientAppointment A
JOIN Doctor D ON D.Id = A.Doctor_Id WHERE  A.IsPaid = 1  AND A.AppointmentDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE()
GROUP BY  D.DoctorId, A.Doctor_Id,D.DoctorName;";
				var data = ent.Database.SqlQuery<DoctorCommissionReport>(doctor).ToList();
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
					int total = data.Count;
					pageNumber = (int?)pageNumber ?? 1;
					int pageSize = 10;
					decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
					model.TotalPages = (int)noOfPages;
					model.PageNumber = (int)pageNumber;
					data = data.OrderBy(a => a.Doctor_Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
					if (name != null)
					{
						data = data.Where(a => a.DoctorName.ToLower().Contains(name.ToLower()) || a.DoctorId.ToLower().Contains(name.ToLower())).ToList();

					}
					model.DoctorCommisionReport = data;
					foreach (var item in data)
					{
						//var razorcomm = (item.Amount * 2) / 100;
						var razorcomm = (item.Amount * Transactionfee) / 100;
						var totalrazorcomm = razorcomm;
						item.Amountwithrazorpaycomm = item.Amount + totalrazorcomm;


					}
					return View(model);
				}
			}
           

        }
        public ActionResult DoctorDetails(int? DoctorId, DateTime? AppointmentDate)
        {
			double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Doctor'").FirstOrDefault();
			double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault();
			double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault(); 
            var model = new ReportDTO();
            if (AppointmentDate != null)
            {
                DateTime dateCriteria = AppointmentDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select a.Doctor_Id, a.Id, a.IsPaid, case when a.PaymentDate is null then 'N/A' else Convert(nvarchar(100),a.PaymentDate,103) end as PaymentDate, case when a.AppointmentDate is null then 'N/A' else Convert(nvarchar(100),a.AppointmentDate,103) end as Appointment,IsNull(d.DoctorName,'N/A') as DoctorName,
IsNull(d.MobileNumber,'N/A') as MobileNumber, 
a.TotalFee as Amount from PatientAppointment A join Doctor D on D.Id = A.Doctor_Id where  a.Doctor_Id = " + DoctorId + " and a.IsPaid=1 and CONVERT(VARCHAR, a.AppointmentDate, 103) between '" + dateCriteria + "' and '" + AppointmentDate + "' order by a.Id desc";
                var data1 = ent.Database.SqlQuery<DoctorCommissionReport>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                    model.DoctorCommisionReport = data1;
                    return View(model);

                }
                else
                {
                    ViewBag.Commission = commision;
                    ViewBag.tds = tds;
                    ViewBag.transaction = Transactionfee;
                    model.DoctorName = data1.FirstOrDefault().DoctorName;
                    model.Doctor_Id = data1.FirstOrDefault().Doctor_Id;
                    model.DoctorCommisionReport = data1;

					foreach (var item in data1)
					{
						var razorcomm = (item.Amount * 2) / 100;
						var totalrazorcomm = razorcomm;
						item.Amountwithrazorpaycomm = item.Amount + totalrazorcomm;

					}
					
                }
				return View(model);
			}
            else
            {
				string q = @"select a.Doctor_Id, a.Id, a.IsPaid, case when a.PaymentDate is null then 'N/A' else Convert(nvarchar(100),a.PaymentDate,103) end as PaymentDate, case when a.AppointmentDate is null then 'N/A' else Convert(nvarchar(100),a.AppointmentDate,103) end as Appointment,IsNull(d.DoctorName,'N/A') as DoctorName,
IsNull(d.MobileNumber,'N/A') as MobileNumber, 
a.TotalFee as Amount from PatientAppointment A join Doctor D on D.Id = A.Doctor_Id where d.id=" + DoctorId + " and a.IsPaid=1";
				var data = ent.Database.SqlQuery<DoctorCommissionReport>(q).ToList();
				if (data.Count() == 0)
				{
					TempData["msg"] = "Something Went Wrong";
				}
				else
				{
					ViewBag.Commission = commision;
					ViewBag.tds = tds;
					ViewBag.transaction = Transactionfee;
					model.DoctorName = data.FirstOrDefault().DoctorName;
					model.Doctor_Id = data.FirstOrDefault().Doctor_Id;
					model.DoctorCommisionReport = data;
					foreach (var item in data)
					{
						var razorcomm = (item.Amount * 2) / 100;
						var totalrazorcomm = razorcomm;
						item.Amountwithrazorpaycomm = item.Amount + totalrazorcomm;

					}
				}
				return View(model);
			}
            
        }
		public void DownloadDoctorDetails(int? Id,int? Doctor_Id)
		{
			double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Doctor'").FirstOrDefault();
			double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault();
			double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault();

			string query = @"select a.Doctor_Id,D.DoctorId, a.Id, a.IsPaid, case when a.PaymentDate is null then 'N/A' else Convert(nvarchar(100),a.PaymentDate,103) end as PaymentDate, case when a.AppointmentDate is null then 'N/A' else Convert(nvarchar(100),a.AppointmentDate,103) end as Appointment,IsNull(d.DoctorName,'N/A') as DoctorName,
IsNull(d.MobileNumber,'N/A') as MobileNumber, 
a.TotalFee as Amount from PatientAppointment A join Doctor D on D.Id = A.Doctor_Id where d.id=" + Doctor_Id + " and a.IsPaid=1";

			var DoctorCommissionReport = ent.Database.SqlQuery<DoctorCommissionReport>(query).ToList();
			ExcelPackage Ep = new ExcelPackage();
			ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("DoctorCommissionReport");

			Sheet.Cells["A1"].Value = "Doctor Id";
			Sheet.Cells["B1"].Value = "Doctor Name";
			Sheet.Cells["C1"].Value = "Appointment Date";
			Sheet.Cells["D1"].Value = "Amount"; 
			Sheet.Cells["E1"].Value = "Transaction Amount " + Transactionfee + " %";
			Sheet.Cells["F1"].Value = "Commission Amount " + commision + " %";
			Sheet.Cells["G1"].Value = "TDS Amount " + tds + " %"; 
			Sheet.Cells["H1"].Value = "Payment Date"; 
			Sheet.Cells["I1"].Value = "Payable Amount"; 
			int row = 2;

			double totalAmount = 0.0; 
			double totalPayableAmount = 0.0;  
			double totalCommAmount = 0.0;  
			double totalTransactionAmount = 0.0;  
			double totalTdsAmount = 0.0;  

			foreach (var item in DoctorCommissionReport)
			{
				double commissionamt = (item.Amount * commision) / 100;				
				double transactionamt = (item.Amount * Transactionfee) / 100; 
				double tdsamt = (item.Amount * tds) / 100;
				double payableamt = item.Amount - (commissionamt + transactionamt + tdsamt);

				Sheet.Cells[string.Format("A{0}", row)].Value = item.DoctorId;
				Sheet.Cells[string.Format("B{0}", row)].Value = item.DoctorName;
				Sheet.Cells[string.Format("C{0}", row)].Value = item.Appointment;
				Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = "yyyy-MM-dd"; // Change the date format as needed
				Sheet.Cells[string.Format("D{0}", row)].Value = item.Amount;
				Sheet.Cells[string.Format("E{0}", row)].Value = transactionamt;
				Sheet.Cells[string.Format("F{0}", row)].Value = commissionamt;
				Sheet.Cells[string.Format("G{0}", row)].Value = tdsamt;
				Sheet.Cells[string.Format("H{0}", row)].Value = item.PaymentDate;
				Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "yyyy-MM-dd"; // Change the date format as needed
				Sheet.Cells[string.Format("I{0}", row)].Value = payableamt;

				totalAmount += item.Amount; 
				totalPayableAmount += payableamt;
				totalTdsAmount += tdsamt;
				totalCommAmount += commissionamt;
				totalTransactionAmount += transactionamt;  
				row++;
			}

			// Create a cell to display the total MonthSalary
			//Sheet.Cells[string.Format("D{0}", row)].Value = "Total Amount";
			Sheet.Cells[string.Format("D{0}", row)].Value = totalAmount;
			Sheet.Cells[string.Format("E{0}", row)].Value = totalTransactionAmount;
			Sheet.Cells[string.Format("F{0}", row)].Value = totalCommAmount;
			Sheet.Cells[string.Format("G{0}", row)].Value = totalTdsAmount;
			Sheet.Cells[string.Format("I{0}", row)].Value = totalPayableAmount;

			Sheet.Cells["A:AZ"].AutoFitColumns();
			Response.Clear();
			Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
			Response.AddHeader("content-disposition", "attachment; filename=Report.xlsx"); // Use a semicolon (;) instead of a colon (:)
			Response.BinaryWrite(Ep.GetAsByteArray());
			Response.End();
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
            string q = @"select ns.Nurse_Id, n.NurseName, n.Fee from Nurse n join NurseService ns on n.Id = ns.Nurse_Id where ns.IsPaid=1 group by ns.Nurse_Id, n.NurseName,n.Fee";
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
where ns.Nurse_Id = " + NurseId+ " and ns.IsPaid=1 and CONVERT(VARCHAR, ns.ServiceAcceptanceDate, 103) between '" + dateCriteria+"' and '"+ServiceAcceptanceDate+"' order by ns.Id desc";
                var data1 = ent.Database.SqlQuery<NurseAppointmentList>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                    model.NurseAppointmentList = data1;
                    return View(model);

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

        public ActionResult Driver(int? pageNumber, DateTime? startdate, DateTime? enddate, string name = null)
        {
			double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Ambulance'").FirstOrDefault();
			double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();
			double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();
			var model = new AmbulanceList();
            if (startdate != null)
            {
                DateTime dateCriteria = startdate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
v.Id as VehicleId, d.DriverName, Sum(trm.TotalPrice) as TotalPrice,d.DriverId
from DriverLocation trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.VehicleType_Id = trm.VehicleType_Id
join Patient p on p.Id = trm.PatientId
where trm.IsPay = 'Y' and trm.EntryDate  between Convert(datetime,'" + startdate + "',103) and Convert(datetime,'" + enddate + "',103) group by v.VehicleNumber, v.VehicleName, v.Id,d.DriverName,d.DriverId";
                var data1 = ent.Database.SqlQuery<AmbulanceReport>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
					ViewBag.Transactionfee = Transactionfee;
					ViewBag.Commission = commision;
					ViewBag.tds = tds;

					int total = data1.Count;
                    pageNumber = (int?)pageNumber ?? 1;
                    int pageSize = 10;
                    decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                    model.TotalPages = (int)noOfPages;
                    model.PageNumber = (int)pageNumber;
                    data1 = data1.OrderBy(a => a.VehicleId).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                    
                    model.Ambulance = data1;
					foreach (var item in data1)
					{ 
						var razorcomm = ((double)item.TotalPrice * Transactionfee) / 100;
						var totalrazorcomm = razorcomm;
						item.Amountwithrazorpaycomm = (double)item.TotalPrice + totalrazorcomm;


					}
					
                }
				return View(model);
			}
            else
            {
				var doctor = @"select v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
v.Id as VehicleId, d.DriverName, Sum(trm.TotalPrice) as TotalPrice,d.DriverId
from DriverLocation trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.VehicleType_Id = trm.VehicleType_Id
join Patient p on p.Id = trm.PatientId
where trm.IsPay = 'Y' and trm.EntryDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() group by v.VehicleNumber, v.VehicleName, v.Id,d.DriverName,d.DriverId";
				var data = ent.Database.SqlQuery<AmbulanceReport>(doctor).ToList();
				if (data.Count() == 0)
				{
					TempData["msg"] = "No Record Of Current Week";
					return View(model);
				}
				else
				{
					ViewBag.Transactionfee = Transactionfee;
					ViewBag.Commission = commision;
					ViewBag.tds = tds;

					int total = data.Count;
					pageNumber = (int?)pageNumber ?? 1;
					int pageSize = 10;
					decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
					model.TotalPages = (int)noOfPages;
					model.PageNumber = (int)pageNumber;
					data = data.OrderBy(a => a.VehicleId).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
					if (name != null)
					{
						data = data.Where(a => a.VehicleNumber.ToLower().Contains(name)).ToList();
						TempData["msg"] = "No Record Avialable.";
					}

					model.Ambulance = data;

					foreach (var item in data)
					{
						var razorcomm = ((double)item.TotalPrice * Transactionfee) / 100;
						var totalrazorcomm = razorcomm;
						item.Amountwithrazorpaycomm = (double)item.TotalPrice + totalrazorcomm;


					}
					return View(model);
				}
			}
        }

		public ActionResult AmbulanceDetails(int? VehicleId,DateTime? AppointmentDate)
		{
			double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Ambulance'").FirstOrDefault();
			double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();
			double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();
			var model = new AmbulanceList();
			if (AppointmentDate != null)
			{
				DateTime dateCriteria = AppointmentDate.Value.AddDays(-7);
				string date = dateCriteria.ToString("dd/MM/yyyy");
				var qry1 = @"select a.Doctor_Id, a.Id, a.IsPaid, case when a.PaymentDate is null then 'N/A' else Convert(nvarchar(100),a.PaymentDate,103) end as PaymentDate, case when a.AppointmentDate is null then 'N/A' else Convert(nvarchar(100),a.AppointmentDate,103) end as Appointment,IsNull(d.DoctorName,'N/A') as DoctorName,
IsNull(d.MobileNumber,'N/A') as MobileNumber, 
a.TotalFee as Amount from PatientAppointment A join Doctor D on D.Id = A.Doctor_Id where  a.Doctor_Id = " + VehicleId + " and a.IsPaid=1 and CONVERT(VARCHAR, a.AppointmentDate, 103) between '" + dateCriteria + "' and '" + AppointmentDate + "' order by a.Id desc";
				var data1 = ent.Database.SqlQuery<AmbulanceReport>(qry1).ToList();
				if (data1.Count() == 0)
				{
					TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
					model.Ambulance = data1;
					return View(model);

				}
				else
				{
					ViewBag.Commission = commision;
					ViewBag.tds = tds;
					ViewBag.transaction = Transactionfee;
					model.VehicleNumber = data1.FirstOrDefault().VehicleNumber;
					model.VehicleId = data1.FirstOrDefault().VehicleId;
					model.Ambulance = data1;

					foreach (var item in data1)
					{
						var razorcomm = ((double)item.TotalPrice * 2) / 100;
						var totalrazorcomm = razorcomm;
						item.Amountwithrazorpaycomm = (double)item.TotalPrice + totalrazorcomm;

					}

				}
				return View(model);
			}
			else
			{
				string q = @"select v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
v.Id as VehicleId, d.DriverName, Sum(trm.TotalPrice) as TotalPrice,d.DriverId,trm.EntryDate,CONCAT(
        DAY(trm.PaymentDate), 
        ' ', 
        UPPER(FORMAT(trm.PaymentDate, 'MMM')), 
        ' ', 
        YEAR(trm.PaymentDate), 
        ' ', 
        FORMAT(trm.PaymentDate, 'hh:mm tt')
    ) AS PaymentDate
from DriverLocation trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.VehicleType_Id = trm.VehicleType_Id
join Patient p on p.Id = trm.PatientId
where trm.IsPay = 'Y' and trm.EntryDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() and v.Id=1135 group by v.VehicleNumber, v.VehicleName, v.Id,d.DriverName,d.DriverId,trm.EntryDate,trm.PaymentDate";
				var data = ent.Database.SqlQuery<AmbulanceReport>(q).ToList();
				if (data.Count() == 0)
				{
					TempData["msg"] = "Something Went Wrong";
				}
				else
				{
					ViewBag.Commission = commision;
					ViewBag.tds = tds;
					ViewBag.transaction = Transactionfee;
					model.VehicleNumber = data.FirstOrDefault().VehicleNumber;
					model.VehicleId = data.FirstOrDefault().VehicleId;
					model.Ambulance = data;
					foreach (var item in data)
					{
						var razorcomm = ((double)item.TotalPrice * 2) / 100;
						var totalrazorcomm = razorcomm;
						item.Amountwithrazorpaycomm = (double)item.TotalPrice + totalrazorcomm;

					}
				}
				return View(model);
			}

		}
	}
}