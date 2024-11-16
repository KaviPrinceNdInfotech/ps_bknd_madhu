using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static HospitalPortal.Controllers.AmbulancePaymentController;

namespace HospitalPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VendorPayoutController : Controller
    {
        DbEntities ent = new DbEntities();
        public ActionResult Vendor()
        {
            return View();
        }
        // GET: VendorPayout
        public ActionResult Doctor()
        {
            var model = new VendorPaymentDTO();
            //double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault();
            //double vendorCommission = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Vendor'").FirstOrDefault();
            double vendorCommission = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster where IsDeleted=0 and Department='Franchise' and Name='Doctor'").FirstOrDefault();
            decimal gst = ent.Database.SqlQuery<decimal>(@"select Amount from FranchiseGstMaster where IsDeleted=0 and Department='Franchise' and Name='Doctor'").FirstOrDefault();
            decimal tds = ent.Database.SqlQuery<decimal>(@"select Amount from FranchiseTDSMaster where IsDeleted=0 and Department='Franchise' and Name='Doctor'").FirstOrDefault();
			double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Doctor'").FirstOrDefault();
			string q = @"select d.DoctorId ,Sum(pa.TotalFee) as Amount,v.VendorName,v.UniqueId, V.Id, v.CompanyName from Doctor d 
join Vendor v on d.Vendor_Id = v.Id join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id  
where pa.AppointmentDate  >= DATEADD(day,-7, GETDATE()) and pa.IsPayoutPaid=0 group by v.VendorName,v.CompanyName, V.Id,d.DoctorId,v.UniqueId";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            } 
            ViewBag.VendorCommission = (decimal)vendorCommission;
            ViewBag.gst = (decimal)gst;
            ViewBag.tds = (decimal)tds;
            model.Vendorses = data;
            foreach (var item in data)
            {
                var razorcomm = (item.Amount * Transactionfee) / 100;
                // var razorcomm = item.Amount * (2.36 / 100); 
                var totalrazorcomm = razorcomm;
                item.Amountwithrazorpaycomm = (decimal)(item.Amount + totalrazorcomm);
                 
                item.FraPaidableamt = (item.Amount * 3) / 100;
            }
            return View(model);
        }
        public ActionResult Driver()
        {
            var model = new VendorPaymentDTO();
            double payout = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Driver'").FirstOrDefault();
            //double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Franchise' and Name='Doctor'").FirstOrDefault();
            string q = @"select COUNT(d.Id) as Amount, v.VendorName,v.CompanyName, V.Id from Driver d join Vendor v on d.Vendor_Id = v.Id  where d.JoiningDate  >= DATEADD(day,-7, GETDATE()) group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
           //ViewBag.Payment = payment;
            ViewBag.Payout = payout;
            model.Vendorses = data;
            return View(model);
        }
        public ActionResult Vehicle()
        {
            var model = new VendorPaymentDTO();
            //double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Vendor'").FirstOrDefault();
            //double vendorCommission = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Vendor'").FirstOrDefault();
            double vendorCommission = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster where IsDeleted=0 and Department='Franchise' and Name='Vehicle' and IsDeleted=0").FirstOrDefault();
            decimal gst = ent.Database.SqlQuery<decimal>(@"select Amount from FranchiseGstMaster where IsDeleted=0 and Department='Franchise' and Name='Vehicle'").FirstOrDefault();
            decimal tds = ent.Database.SqlQuery<decimal>(@"select Amount from FranchiseTDSMaster where IsDeleted=0 and Department='Franchise' and Name='Vehicle'").FirstOrDefault();
			double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Vehicle'").FirstOrDefault();
			string q = @"select Sum(trm.TotalPrice) as AmountForVehicle, vp.IsGenerated, ve.VendorName, ve.CompanyName,ve.Id from Vehicle v 
join Vendor ve on ve.Id = v.Vendor_Id 
join Driverlocation trm on trm.Driver_Id = v.Driver_Id 
left join VendorPayout vp on vp.Vendor_Id  = ve.Id 
where trm.EntryDate between DateAdd(DD,-7,GETDATE() ) 
and GETDATE() group by  vp.IsGenerated,ve.VendorName, ve.CompanyName, ve.Id";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            //ViewBag.Payment = payment;
            //ViewBag.Commission = commision;
            ViewBag.VendorCommission = (decimal)vendorCommission;
            ViewBag.gst = (decimal)gst;
            ViewBag.tds = (decimal)tds;
            //ViewBag.paymentPercent = paymentPercent;
            model.Vendorses = data;
			foreach (var item in data)
			{
				var razorcomm = ((decimal)item.AmountForVehicle * (decimal)Transactionfee) / 100;
				// var razorcomm = item.Amount * (2.36 / 100); 
				var totalrazorcomm = razorcomm;
				item.Amountwithrazorpaycomm = (decimal)(item.AmountForVehicle + totalrazorcomm);

				item.FraPaidableamt = (double?)((item.AmountForVehicle * 3) / 100);
			}
			return View(model);
        }
        public ActionResult Pay(int? Vendor_Id, double? Amount,string multyid)
        {
           
            if (!string.IsNullOrEmpty(multyid))
            {
                string[] mulidoc = multyid == null ? null : multyid.Split('-');
                for (int i = 0; i < mulidoc.Length - 1; i++)
                {
                    string[] perdoc = mulidoc[i].Split(',');
                    int vendorid = Convert.ToInt32(perdoc[0]);
                    double amount = Convert.ToDouble(perdoc[1]);
                    var model1 = new VendorPayOut();
                    model1.Amount = amount;
                    model1.IsPaid = true;
                    model1.IsGenerated = true;
                    model1.PaymentDate = DateTime.Now.Date;
                    model1.Vendor_Id = vendorid;
                    ent.VendorPayOuts.Add(model1);
                    ent.SaveChanges();


                }
                return RedirectToAction("VendorPayOutList");
            }
            else
            {
                var model = new VendorPayOut();
                model.Vendor_Id = Vendor_Id;
                model.Amount = Amount;
                model.IsPaid = true;
                model.IsGenerated = true;
                model.PaymentDate = DateTime.Now.Date;
                ent.VendorPayOuts.Add(model);
                ent.SaveChanges();
                return RedirectToAction("VendorPayoutHistory", new { Id = Vendor_Id });
            }
        }

        public ActionResult VendorPayoutHistory(int Id)
        {
            Session["msg"] = Id; 
            var model = new ViewPayOutHistory();
            var Name = ent.Database.SqlQuery<string>("select VendorName from Vendor where Id=" + Id).FirstOrDefault();
            model.LabName = Name;
            string qry = @"select Dp.Id, ISNULL(Dp.IsPaid, 0) as IsPaid , Dp.IsGenerated, Dp.Vendor_Id, Dp.PaymentDate, Dp.Amount, D.VendorName from  VendorPayOut Dp join Vendor D on D.Id = Dp.Vendor_Id  where  Dp.Vendor_Id=" + Id;
            var data = ent.Database.SqlQuery<HistoryOfVendor_Payout>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            else
            {
                ViewBag.Amount = data.Sum(a => a.Amount);
                model.HistoryOfVendor_Payout = data;
            }
            return View(model);
        }
        public ActionResult UpdateHealthStatus(int id)
        {
            int Id = Convert.ToInt32(Session["msg"].ToString());
            string q = @"update VendorPayOut set IsGenerated = case when IsGenerated=1 then 0 else 1 end where Id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("ViewVendorPayoutHistory", new { Id = Id });
        }
        public ActionResult UpdateVendorPayment(int id)
        {
            int Id = Convert.ToInt32(Session["msg"]);
            string q = @"update VendorPayOut set IsPaid = case when IsPaid=1 then 0 else 1 end where Id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("VendorPayoutHistory", new { Id = Id });
        }
        public ActionResult HealthCheckUp()
        {
            var model = new VendorPaymentDTO();
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='HealthCheckup'").FirstOrDefault();
            //double vendorCommission = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Vendor'").FirstOrDefault();
            double vendorCommission = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster where IsDeleted=0 and Department='Franchise' and Name='HealthCheckup' and IsDeleted=0").FirstOrDefault();
            //double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Franchise' and Name='Doctor'").FirstOrDefault();
            string q = @"select SUM(cc.Amount) as Amount, v.VendorName, vp.IsGenerated, V.Id, v.CompanyName from [HealthCheckupCenter] d 
join Vendor v on d.Vendor_Id = v.Id  
join CmpltCheckUp cc on cc.Center_Id = d.Id
join HealthBooking hb on hb.PatientId = cc.PatientId
left join VendorPayout vp on vp.Vendor_Id  = v.Id
where Convert(Date,cc.TestDate)  between DATEADD(day,-7,GETDATE()) and GetDate() and hb.IsPaid=1 and vp.IsGenerated is null group by v.VendorName,v.CompanyName, V.Id,vp.IsGenerated";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            ViewBag.vendorCommission = vendorCommission;

            ViewBag.Commission = commision;
            //ViewBag.Payment = payment;
            //ViewBag.Payout = payout;
            model.Vendorses = data;
            return View(model);
        }
        public ActionResult Lab()
        {
            var model = new VendorPaymentDTO();
            //double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Lab'").FirstOrDefault();
            //double vendorCommission = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Vendor'").FirstOrDefault();
            double vendorCommission = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster where IsDeleted=0 and Department='Franchise' and Name='Lab'").FirstOrDefault();
            decimal gst = ent.Database.SqlQuery<decimal>(@"select Amount from FranchiseGstMaster where IsDeleted=0 and Department='Franchise' and Name='Lab'").FirstOrDefault();
            decimal tds = ent.Database.SqlQuery<decimal>(@"select Amount from FranchiseTDSMaster where IsDeleted=0 and Department='Franchise' and Name='Lab'").FirstOrDefault();
            //double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Franchise' and Name='Doctor'").FirstOrDefault();
            //string q = @"select Sum(bt.Amount) as Amount, vp.IsGenerated, v.VendorName,v.CompanyName, V.Id from Lab d join Vendor v on d.Vendor_Id = v.Id join BookTestLab bt on bt.Lab_Id = d.Id join LabBooking lb on lb.PatientId = bt.Patient_Id left join VendorPayout vp on vp.Vendor_Id  = v.Id where  Convert(Date,bt.TestDate) between DATEADD(day,-7,GETDATE()) and GetDate() and lb.IsPaid =1 group by v.VendorName,v.CompanyName, V.Id, vp.IsGenerated";
            string q = @"select d.lABId as UniqueId,Sum(bt.Amount) as Amount, v.VendorName,v.CompanyName, V.Id from Lab d 
join Vendor v on d.Vendor_Id = v.Id 
join BookTestLab bt on bt.Lab_Id = d.Id
where  Convert(Date,bt.TestDate) between DATEADD(day,-7,GETDATE()) and GetDate() group by v.VendorName,v.CompanyName, V.Id,d.lABId";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            //ViewBag.Payment = payment;
            //ViewBag.Commission = commision;
            ViewBag.VendorCommission = (decimal)vendorCommission;
            ViewBag.gst = (decimal)gst;
            ViewBag.tds = (decimal)tds;
            model.Vendorses = data;

            foreach (var item in data)
            {
                var razorcomm = item.Amount * (2.36 / 100);
                // var razorcommafter = razorcomm * 2.36 / 100;
                var totalrazorcomm = razorcomm;
                item.Amountwithrazorpaycomm = (decimal)(item.Amount + totalrazorcomm);

            }
            return View(model);
        }
        public ActionResult Nurse()
        {
            var model = new VendorPaymentDTO();
            //double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Nurse'").FirstOrDefault();
            //double vendorCommission = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Vendor'").FirstOrDefault();
            double vendorCommission = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster where IsDeleted=0 and Department='Franchise' and Name='Nurse' and IsDeleted=0").FirstOrDefault();
            decimal gst = ent.Database.SqlQuery<decimal>(@"select Amount from FranchiseGstMaster where IsDeleted=0 and Department='Franchise' and Name='Nurse'").FirstOrDefault();
            decimal tds = ent.Database.SqlQuery<decimal>(@"select Amount from FranchiseTDSMaster where IsDeleted=0 and Department='Franchise' and Name='Nurse'").FirstOrDefault();
            //double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Franchise' and Name='Doctor'").FirstOrDefault();
            string q = @"select d.NurseId as UniqueId, v.VendorName, vp.IsGenerated, v.CompanyName,  V.Id ,ns.TotalFee as Amount from Nurse d join NurseService ns on ns.Nurse_Id = d.Id join Vendor v on d.Vendor_Id = v.Id left join VendorPayOut vp on  v.Id = vp.Vendor_Id where Convert(Date,ns.ServiceAcceptanceDate)   between DATEADD(day,-7,GETDATE()) and GetDate() and ns.IsPaid=1  group by v.VendorName,v.CompanyName, V.Id,ns.TotalFee, vp.IsGenerated,d.NurseId";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            //ViewBag.Payment = payment;
            //ViewBag.Commission = commision;
            ViewBag.VendorCommission = (decimal)vendorCommission;
            ViewBag.gst = (decimal)gst;
            ViewBag.tds = (decimal)tds;
            //ViewBag.paymentPercent = paymentPercent;
            model.Vendorses = data;
            foreach (var item in data)
            {
                var razorcomm = item.Amount * (2.36 / 100);
                // var razorcommafter = razorcomm * 2.36 / 100;
                var totalrazorcomm = razorcomm;
                item.Amountwithrazorpaycomm = (decimal)(item.Amount + totalrazorcomm);

            }
            return View(model);
        }
        public ActionResult Chemist()
        {
            var model = new VendorPaymentDTO();
            //double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Chemist'").FirstOrDefault();
            //double vendorCommission = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Vendor'").FirstOrDefault();
            double vendorCommission = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster where IsDeleted=0 and Department='Franchise' and Name='Chemist' and IsDeleted=0").FirstOrDefault();
            decimal gst = ent.Database.SqlQuery<decimal>(@"select Amount from FranchiseGstMaster where IsDeleted=0 and Department='Franchise' and Name='Chemist'").FirstOrDefault();
            decimal tds = ent.Database.SqlQuery<decimal>(@"select Amount from FranchiseTDSMaster where IsDeleted=0 and Department='Franchise' and Name='Chemist'").FirstOrDefault();
            //double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Franchise' and Name='Doctor'").FirstOrDefault();
            string q = @"select d.ChemistId as UniqueId, SUM(md.Amount) as Amount, v.VendorName,v.CompanyName,v.Id from Chemist d join Vendor v on d.Vendor_Id = v.Id join MedicineOrder mo on mo.Chemist_Id = d.Id join MedicineOrderDetail md on md.Order_Id = mo.Id where Convert(Date,mo.OrderDate) between DATEADD(day,-7,GETDATE()) and GetDate() and mo.IsPaid=1 group by v.VendorName,v.CompanyName,v.Id,d.ChemistId";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            //ViewBag.Payment = payment;
            //ViewBag.Commission = commision;
            ViewBag.VendorCommission = (decimal)vendorCommission;
            ViewBag.gst = (decimal)gst;
            ViewBag.tds = (decimal)tds;
            //ViewBag.paymentPercent = paymentPercent;
            model.Vendorses = data;
            foreach (var item in data)
            {
                var razorcomm = item.Amount * (2.36 / 100);
                // var razorcommafter = razorcomm * 2.36 / 100;
                var totalrazorcomm = razorcomm;
                item.Amountwithrazorpaycomm = (decimal)(item.Amount + totalrazorcomm);

            }
            return View(model);
        }

        public ActionResult VendorPayOutList()
        {
            var model = new VendorPayOutListVM();
            string q = @"select vp.Id,v.UniqueId, SUM(vp.Amount) as Amount, v.VendorName, v.CompanyName,vp.PaymentDate from 
VendorPayout vp join Vendor v on vp.Vendor_Id = v.Id
group by v.VendorName,v.CompanyName, vp.Id,vp.PaymentDate,v.UniqueId";
            var data = ent.Database.SqlQuery<VendorPayoutHistory>(q).ToList();
            model.VendorList = data;
            return View(model);
        }

        public void DownloadVendorExcel()
        {
            string query = "select vp.*,v.UniqueId, v.VendorName as BeneficiaryName,bd.* from VendorPayout vp join Vendor v on vp.Vendor_Id = v.Id join BankDetails as bd on bd.Login_Id=v.AdminLogin_Id";
            List<DetailsForBank> employeeDetails = ent.Database.SqlQuery<DetailsForBank>(query, Array.Empty<object>()).ToList();
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "Payment Method";
            Sheet.Cells["B1"].Value = "Payment Amount";
            Sheet.Cells["C1"].Value = "Activation Date";
            Sheet.Cells["D1"].Value = "Beneficiary Name";
            Sheet.Cells["E1"].Value = "Account No in text";
            Sheet.Cells["F1"].Value = "Email";
            Sheet.Cells["G1"].Value = "Email Body";
            Sheet.Cells["H1"].Value = "Debit Account No";
            Sheet.Cells["I1"].Value = "CRN No";
            Sheet.Cells["J1"].Value = "Receiver IFSC Code";
            Sheet.Cells["K1"].Value = "Receiver Account";
            Sheet.Cells["L1"].Value = "Remarks";
            Sheet.Cells["M1"].Value = "Phone No";
            int row = 2;
            CRNGenerator crnGenerator = new CRNGenerator();
            foreach (DetailsForBank item in employeeDetails)
            {
                string dvrId = item.UniqueId;
                long sdds = Convert.ToInt64(item.AccountNo);
                string crn = crnGenerator.GenerateCRN(dvrId);
                Sheet.Cells[$"A{row}"].Value = "N";
                Sheet.Cells[$"B{row}"].Value = item.Amount;
                Sheet.Cells[$"C{row}"].Value = DateTime.Now;
                Sheet.Cells[$"C{row}"].Style.Numberformat.Format = "yyyy-MM-dd";
                Sheet.Cells[$"D{row}"].Value = item.BeneficiaryName;
                Sheet.Cells[$"E{row}"].Value = NumberToWords(sdds);
                Sheet.Cells[$"F{row}"].Value = item.EmailId;
                Sheet.Cells[$"G{row}"].Value = "This is your payment report";
                Sheet.Cells[$"H{row}"].Value = "924020004812750";
                Sheet.Cells[$"I{row}"].Value = crn;
                Sheet.Cells[$"J{row}"].Value = item.IFSCCode;
                Sheet.Cells[$"K{row}"].Value = item.AccountNo;
                Sheet.Cells[$"L{row}"].Value = "Enter your remark";
                Sheet.Cells[$"M{row}"].Value = item.MobileNumber;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            base.Response.Clear();
            base.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            base.Response.AddHeader("content-disposition", "attachment; filename=Report.xlsx");
            base.Response.BinaryWrite(Ep.GetAsByteArray());
            base.Response.End();
        }
        static string NumberToWords(long number)
        {
            if (number == 0)
                return "zero";

            int val;
            long next, num_digits;
            long[] a = new long[19]; // Maximum number of digits in a long is 19

            // words for every digits from 0 to 9
            string[] digits_words = {
        "zero",
        "one",
        "two",
        "three",
        "four",
        "five",
        "six",
        "seven",
        "eight",
        "nine"
    };

            string words = "";

            val = 0;
            next = 0;
            num_digits = 0;

            while (number > 0)
            {
                next = number % 10;
                a[val] = next;
                val++;
                num_digits++;
                number = number / 10;
            }

            for (val = (int)(num_digits - 1); val >= 0; val--)
            {
                words += digits_words[a[val]] + " ";
            }

            return words.Trim(); // Trim any trailing whitespace
        }
    }
}