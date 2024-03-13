using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            //double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Franchise' and Name='Doctor'").FirstOrDefault();
            string q = @"select d.DoctorId ,Sum(pa.TotalFee) as Amount,v.VendorName,v.UniqueId, V.Id, v.CompanyName from Doctor d 
join Vendor v on d.Vendor_Id = v.Id join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id  
where pa.AppointmentDate  >= DATEADD(day,-7, GETDATE())   group by v.VendorName,v.CompanyName, V.Id,d.DoctorId,v.UniqueId";
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
                var razorcomm = (item.Amount * 2) / 100;
                // var razorcomm = item.Amount * (2.36 / 100); 
                var totalrazorcomm = razorcomm;
                item.Amountwithrazorpaycomm = (decimal)(item.Amount + totalrazorcomm);
                 
                ViewBag.FraPaidableamt = (item.Amount * 3) / 100;
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
            //double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Franchise' and Name='Doctor'").FirstOrDefault();
            string q = @"select Sum(trm.Amount) as AmountForVehicle, vp.IsGenerated, ve.VendorName, ve.CompanyName,ve.Id from Vehicle v 
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
            return View(model);
        }
        public ActionResult Pay(int? Vendor_Id, double Amount)
        {
            var model = new VendorPayOut();
            model.Vendor_Id = Vendor_Id;
            model.Amount = Amount;
            model.IsPaid = false;
            model.IsGenerated = true;
            model.PaymentDate = DateTime.Now.Date;
            ent.VendorPayOuts.Add(model);
            ent.SaveChanges();
            return RedirectToAction("VendorPayoutHistory", new { Id = Vendor_Id });
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

        public void DownloadVendorExcel(int? Id)
        {
            String query = @"select vp.Id,v.UniqueId, SUM(vp.Amount) as Amount, v.VendorName,v.EmailId,v.AadharOrPANNumber,V.Location, v.CompanyName,vp.PaymentDate from 
VendorPayout vp join Vendor v on vp.Vendor_Id = v.Id
group by v.VendorName,v.CompanyName, vp.Id,vp.PaymentDate,v.UniqueId,v.EmailId,v.AadharOrPANNumber,V.Location";

            var employeeDetails = ent.Database.SqlQuery<VendorPayoutHistory>(query).ToList();
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");

            Sheet.Cells["A1"].Value = "Vendor Name";
            Sheet.Cells["B1"].Value = "UniqueId";
            Sheet.Cells["C1"].Value = "Company Name";
            Sheet.Cells["D1"].Value = "Location";
            Sheet.Cells["E1"].Value = "AadharOrPANNumber";
            Sheet.Cells["F1"].Value = "Amount";
            Sheet.Cells["G1"].Value = "EmailId";
            Sheet.Cells["H1"].Value = "Payment Date";
            int row = 2;
            double totalAmount = 0.0; // Initialize a variable to store the total MonthSalary

            foreach (var item in employeeDetails)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.VendorName;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.UniqueId;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.CompanyName;                
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Location;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.AadharOrPANNumber;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Amount;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.EmailId;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.PaymentDate;
                Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "yyyy-MM-dd"; // Change the date format as needed
                //Sheet.Cells[string.Format("I{0}", row)].Value = item.BranchName;
                //Sheet.Cells[string.Format("J{0}", row)].Value = item.BranchAddress;
                //Sheet.Cells[string.Format("K{0}", row)].Value = item.HolderName;

                totalAmount += item.Amount; // Add the current MonthSalary to the total
                row++;
            }

            // Create a cell to display the total MonthSalary
            Sheet.Cells[string.Format("E{0}", row)].Value = "Total Amount";
            Sheet.Cells[string.Format("F{0}", row)].Value = totalAmount;

            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=Report.xlsx"); 
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}