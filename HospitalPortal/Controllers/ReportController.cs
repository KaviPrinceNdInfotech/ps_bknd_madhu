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
    public class ReportController : Controller
    {
        DbEntities ent = new DbEntities();
        // GET: Report
        public ActionResult DoctorReport()
        {
            var model = new ReportDTO();
            var doctor = @"select P.Doctor_Id, D.DoctorName from dbo.PatientAppointment P join Doctor D ON d.Id = p.Doctor_Id where p.IsPaid=1 and  P.AppointmentDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() group by Doctor_Id, DoctorName";
            var data = ent.Database.SqlQuery<DoctorReports>(doctor).ToList();
            model.DoctorReport = data;
            return View(model);
        }

        public ActionResult ViewDoctorDetails(int DoctorId, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDTO();
            var doctor = @"select * from Doctor where Id=" + DoctorId;
            var mek = ent.Database.SqlQuery<ReportDTO>(doctor).ToList();
            model.ClinicName = mek.FirstOrDefault().ClinicName;
            model.DoctorName = mek.FirstOrDefault().DoctorName;
            model.LicenceNumber = mek.FirstOrDefault().LicenceNumber;
            model.MobileNumber = mek.FirstOrDefault().MobileNumber;
            if (sdate != null && edate != null)
            {
                var doct = @"select CONVERT(VARCHAR(10), AppointmentDate, 111) as AppointmentDate1, P.AppointmentDate, Sum(P.TotalFee) as Amount, D.DoctorName, D.MobileNumber, D.ClinicName,D.LicenceNumber  from PatientAppointment P join Doctor D on D.Id = p.Doctor_Id where p.IsPaid=1 and p.Doctor_Id='" + DoctorId + "' and P.AppointmentDate between '" + sdate + "' and '" + edate + "' GROUP BY P.AppointmentDate, P.TotalFee, D.DoctorName, D.MobileNumber, D.ClinicName,D.LicenceNumber ";
                var doctor1 = ent.Database.SqlQuery<DoctorReports>(doct).ToList();
                //doctorList = doctorList.Where(a => a.AppointmentDate >= sdate && a.AppointmentDate <= edate).ToList();
                model.DoctorReport = doctor1;
            }
            else
            {
                var doctor1 = @"select CONVERT(VARCHAR(10), AppointmentDate, 111) as AppointmentDate1, P.AppointmentDate, Sum(P.TotalFee) as Amount, D.DoctorName, D.MobileNumber, D.ClinicName,D.LicenceNumber  from PatientAppointment P join Doctor D on D.Id = p.Doctor_Id where p.IsPaid=1 and p.Doctor_Id='" + DoctorId + "' and P.AppointmentDate between DateAdd(DD,-7,GETDATE() ) and GETDATE()  GROUP BY P.AppointmentDate, P.TotalFee, D.DoctorName, D.MobileNumber, D.ClinicName,D.LicenceNumber";
                var doctorList = ent.Database.SqlQuery<DoctorReports>(doctor1).ToList();
                model.DoctorReport = doctorList;
            }
            ViewBag.Total = model.DoctorReport.Sum(a => a.Amount);
            return View(model);
        }

        public ActionResult GetTest()
        {
            var test = ent.LabTests.ToList();
            var data = Mapper.Map<IEnumerable<TestDTO>>(test);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LabReport()
        {
            var model = new ReportDTO();
            var lab = @"select P.Lab_Id, D.LabName from BookTestLab P join Lab D ON d.Id = p.Lab_Id where P.TestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() and  P.IsPaid=1 group by Lab_Id, LabName";
            var data = ent.Database.SqlQuery<LabReportsVM>(lab).ToList();
            if (data.Count() == 0)
            {
                TempData["MSG"] = "No Records";
                return View(model);
            }
            else
            {
                model.LabReport = data;

            }
            return View(model);
        }

        public ActionResult ViewLabDetails(int LabId, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDTO();
            var la = @"select * from Lab join CityMaster on CityMaster.Id = Lab.CityMaster_Id join StateMaster on StateMaster.Id = Lab.StateMaster_Id where Lab.Id=" + LabId;
            var mek = ent.Database.SqlQuery<ReportDTO>(la).ToList();
            model.LabName = mek.FirstOrDefault().LabName;
            model.LicenceNumber = mek.FirstOrDefault().LicenceNumber;
            model.MobileNumber = mek.FirstOrDefault().MobileNumber;
            model.StateName = mek.FirstOrDefault().StateName;
            model.StateName = mek.FirstOrDefault().CityName;
            model.Location = mek.FirstOrDefault().Location;
            if (sdate != null && edate != null)
            {
                var labs = @"select CONVERT(VARCHAR(10), P.TestDate, 111) as TestDate1, P.TestDate, Sum(P.Amount) as Amount, D.LabName,D.Location, D.MobileNumber,D.LicenceNumber,S.StateName, C.CityName from BookTestLab P join Lab D on D.Id = p.Lab_Id join CityMaster C on C.Id = D.CityMaster_Id join StateMaster S on S.Id = D.StateMaster_Id WHERE p.Lab_Id='" + LabId + "' and P.TestDate between '" + sdate + "' and '" + edate + "' GROUP BY P.TestDate, P.Amount, D.LabName, D.MobileNumber, S.StateName,C.CityName, D.LicenceNumber,D.Location ";
                var lab = ent.Database.SqlQuery<LabReportsVM>(labs).ToList();
                //doctorList = doctorList.Where(a => a.AppointmentDate >= sdate && a.AppointmentDate <= edate).ToList();
                if (lab.Count() == 0)
                {
                    TempData["MSG"] = "No Records";
                }
                else
                {
                    model.LabReport = lab;
                    ViewBag.Total = model.LabReport.Sum(a => a.Amount);
                    return View(model);
                }
            }
            else
            {
                var doctor = @"select P.TestDate, Sum(P.Amount) as Amount, D.LabName,D.Location, D.MobileNumber,D.LicenceNumber,S.StateName, C.CityName from BookTestLab P join Lab D on D.Id = p.Lab_Id join CityMaster C on C.Id = D.CityMaster_Id join StateMaster S on S.Id = D.StateMaster_Id WHERE p.Lab_Id='" + LabId + "'  and P.TestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() GROUP BY P.TestDate, P.Amount, D.LabName, D.MobileNumber, S.StateName,C.CityName, D.LicenceNumber,D.Location";
                var labList = ent.Database.SqlQuery<LabReportsVM>(doctor).ToList();
                if (labList.Count() == 0)
                {
                    TempData["MSG"] = "No Records";
                }
                else
                {
                    model.LabReport = labList;
                    ViewBag.Total = model.LabReport.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult ChemistReport()
        {
            var model = new ReportDTO();
            var doctor = @"select P.Chemist_Id,D.Id, D.ChemistName, D.ShopName from MedicineOrder P join Chemist D ON d.Id = p.Chemist_Id where P.OrderDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() and  p.IsPaid=1  group by Chemist_Id, ChemistName,D.Id,D.ShopName";
            var data = ent.Database.SqlQuery<ChemistReport>(doctor).ToList();
            model.ChemistReport = data;
            return View(model);
        }

        public ActionResult ViewChemistDetails(int ChemistId, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDTO();
            var chemist = @"select * from Chemist join CityMaster on CityMaster.Id = Chemist.CityMaster_Id join StateMaster on StateMaster.Id = Chemist.StateMaster_Id where Chemist.Id=" + ChemistId;
            var mek = ent.Database.SqlQuery<ReportDTO>(chemist).ToList();
            model.ShopName = mek.FirstOrDefault().ShopName;
            model.ChemistName = mek.FirstOrDefault().ChemistName;
            model.LicenceNumber = mek.FirstOrDefault().LicenceNumber;
            model.MobileNumber = mek.FirstOrDefault().MobileNumber;
            model.StateName = mek.FirstOrDefault().StateName;
            model.CityName = mek.FirstOrDefault().CityName;
            model.Location = mek.FirstOrDefault().Location;
            if (sdate != null && edate != null)
            {
                var labs = @"select  convert(date, MO.OrderDate) as OrderDate1, MO.OrderDate, Sum(MODs.Amount) as Amount,C.ChemistName, C.ShopName, C.MobileNumber,CM.CityName, S.StateName, C.Location, C.LicenceNumber from Chemist C join MedicineOrder MO on C.Id = MO.Chemist_Id join MedicineOrderDetail MODs on MODs.Order_Id = MO.Id join CityMaster CM on CM.Id = C.CityMaster_Id join StateMaster S on S.Id = C.StateMaster_Id WHERE C.Id='" + ChemistId + "' and MO.OrderDate between '" + sdate + "' and '" + edate + "' group by MO.OrderDate,C.ChemistName, C.ShopName, C.MobileNumber,CM.CityName, S.StateName, C.Location, C.LicenceNumber ";
                var lab = ent.Database.SqlQuery<ChemistReport>(labs).ToList();
                //doctorList = doctorList.Where(a => a.AppointmentDate >= sdate && a.AppointmentDate <= edate).ToList();
                model.ChemistReport = lab;
            }
            else
            {
                var doctor = @"select  convert(date, MO.OrderDate) as OrderDate1,Sum(MODs.Amount) as Amount,C.ChemistName, C.ShopName, C.MobileNumber,CM.CityName, S.StateName from Chemist C join MedicineOrder MO on C.Id = MO.Chemist_Id join MedicineOrderDetail MODs on MODs.Order_Id = MO.Id join CityMaster CM on CM.Id = C.CityMaster_Id join StateMaster S on S.Id = C.StateMaster_Id WHERE C.Id='" + ChemistId + "'  and MO.OrderDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() group by MO.OrderDate,C.ChemistName, C.ShopName, C.MobileNumber,CM.CityName, S.StateName, C.Location, C.LicenceNumber ";
                var labList = ent.Database.SqlQuery<ChemistReport>(doctor).ToList();
                model.ChemistReport = labList;
            }
            ViewBag.Total = model.ChemistReport.Sum(a => a.Amount);
            return View(model);
        }
        //ViewLabAppointment
        public ActionResult ViewAll(string date)
        {
            var model = new ReportDTO();
            var lab = @"select CONVERT(VARCHAR(10), TestDate, 111) as TestDate1,* from BookTestLab Join LabTest on LabTest.Id= BookTestLab.Test_Id where Convert(Date,TestDate)='" + date + "'";
            var data = ent.Database.SqlQuery<ViewLabDetails>(lab).ToList();
            if (data.Count() == 0)
            {
                TempData["MSG"] = "No Records";
            }
            else
            {
                model.ViewLabDetails = data;
            }
            ViewBag.Total = model.ViewLabDetails.Sum(a => a.Amount);
            return View(model);

            
        }

        public ActionResult ViewMedicinePurchase(DateTime date)
        {
            var model = new ReportDTO();
            var medicine = @"select * from Medicine";
            var mek = ent.Database.SqlQuery<ReportDTO>(medicine).ToList();
            model.MedicineName = mek.FirstOrDefault().MedicineName;
            model.MRP = mek.FirstOrDefault().MRP;
            var qa = @"select convert(date, MedicineOrder.OrderDate) as OrderDate1,* from MedicineOrderDetail join MedicineOrder on MedicineOrderDetail.Order_Id = MedicineOrder.Id WHERE convert(date, MedicineOrder.OrderDate) ='" + date + "'order by MedicineOrder.OrderDate desc ";
            var data = ent.Database.SqlQuery<ViewChemistDetails>(qa).ToList();
            if (data.Count() == 0)
            {
                TempData["MSG"] = "No Records";
            }
            else
            {
                model.VieChemistDetails = data;
            }
            ViewBag.Total = model.VieChemistDetails.Sum(a => a.Amount);
            return View(model);
        }

        public ActionResult ViewDoctorAppointment(string date)
        {
            var model = new ReportDTO();
            var lab = @"select CONVERT(VARCHAR(10), AppointmentDate, 111) as AppointmentDate1,* from PatientAppointment join Patient on Patient.Id = PatientAppointment.Patient_Id where PatientAppointment.IsPaid=1 and Convert(Date,AppointmentDate)='" + date + "'";
            var data = ent.Database.SqlQuery<AppointmentDetails>(lab).ToList();
            model.AppointmentDetails = data;
            ViewBag.Total = model.AppointmentDetails.Sum(a => a.TotalFee);
            return View(model);
        }


        public ActionResult NurseReport(int? Id)
        {
            var model = new ReportDTO();
            model.NurseTypeList = new SelectList(ent.NurseTypes.ToList(), "Id", "NurseTypeName");
            var Nurse = @"select P.Id, P.NurseName, d.NurseTypeName from Nurse P join NurseType D ON d.Id = p.NurseType_Id join NurseService ns on ns.Nurse_Id = P.Id where ns.ServiceAcceptanceDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() and ns.IsPaid=1  group by  P.NurseName, d.NurseTypeName,P.Id";
            var data = ent.Database.SqlQuery<ViewNurseList>(Nurse).ToList();
            model.NurseList = data;
            return View(model);
        }

        public ActionResult ViewNurseList(int Id, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDTO();
            var Nurse = @"select * from Nurse join CityMaster cm on cm.Id = Nurse.CityMaster_Id join StateMaster sm on Nurse.StateMaster_Id = sm.Id where Nurse.Id=" + Id;
            var mek = ent.Database.SqlQuery<ReportDTO>(Nurse).ToList();
            model.NurseName = mek.FirstOrDefault().NurseName;
            model.StateName = mek.FirstOrDefault().StateName;
            model.CityName = mek.FirstOrDefault().CityName;
            model.MobileNumber = mek.FirstOrDefault().MobileNumber;
            if (sdate != null && edate != null)
            {
                var qry = @"select n.NurseName, n.Location, n.CertificateNumber, ns.IsPaid, cm.CityName, sm.StateName, case when ns.ServiceAcceptanceDate is null then 'N/A' else Convert(nvarchar(100), ns.ServiceAcceptanceDate, 103) end as ServiceAcceptanceDate, Convert(nvarchar(100), ns.RequestDate, 103) as RequestDate,
ns.RequestDate as rDate,
Datediff(day,ns.StartDate,ns.EndDate) as TotalDays,
IsNull(ns.PerDayAmount,0) as Fee,
ns.TotalFee,
p.PatientName
from NurseService ns
join nurse n on ns.Nurse_Id=n.Id
join CityMaster cm on cm.Id = n.CityMaster_Id
join StateMaster sm on sm.Id = n.StateMaster_Id
join Patient p on p.Id = ns.Patient_Id
where ns.Nurse_Id =n.Id
and ns.Nurse_Id ='" + Id + "' and ns.ServiceAcceptanceDate between "+sdate+" and "+edate+"  and ns.IsPaid=1 order by ns.Id desc ";
                var Nurses = ent.Database.SqlQuery<ViewNurseList>(qry).ToList();
                //doctorList = doctorList.Where(a => a.AppointmentDate >= sdate && a.AppointmentDate <= edate).ToList();
                model.NurseList = Nurses;
            }
            else
            {
                var doctor1 = @"select n.NurseName, n.Location, n.CertificateNumber, ns.IsPaid, cm.CityName, sm.StateName, case when ns.ServiceAcceptanceDate is null then 'N/A' else Convert(nvarchar(100), ns.ServiceAcceptanceDate, 103) end as ServiceAcceptanceDate, Convert(nvarchar(100), ns.RequestDate, 103) as RequestDate,ns.RequestDate as rDate,Datediff(day, ns.StartDate, ns.EndDate) as TotalDays,IsNull(ns.PerDayAmount, 0) as Fee,ns.TotalFee,p.PatientName from NurseService ns join nurse n on ns.Nurse_Id = n.Id join CityMaster cm on cm.Id = n.CityMaster_Id join StateMaster sm on sm.Id = n.StateMaster_Id join Patient p on p.Id = ns.Patient_Id where ns.Nurse_Id = n.Id and ns.Nurse_Id = " + Id+" and IsPaid = 1 and ns.ServiceAcceptanceDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() order by ns.Id desc";
                var doctorList = ent.Database.SqlQuery<ViewNurseList>(doctor1).ToList();
                model.NurseList = doctorList;
            }
            ViewBag.Total = model.NurseList.Sum(a => a.TotalFee);
            return View(model);
        }

        public ActionResult HealthCheckUp()
        {
            var model = new ReportDTO();
            var lab = @"select P.Center_Id, D.LabName from CmpltCheckUp P join HealthCheckupCenter D ON d.Id = p.Center_Id where P.IsPaid=1 group by Center_Id, LabName";
            var data = ent.Database.SqlQuery<HealthCheckListVM>(lab).ToList();
            if (data.Count() == 0)
            {
                TempData["MSG"] = "No Records";
                return View(model);
            }
            else
            {
                model.ViewCmplteCheckUp = data;
            }
            return View(model);
        }

        public ActionResult ViewHealthDetails(int CenterId, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDTO();
            var la = @"select * from HealthCheckupCenter join CityMaster on CityMaster.Id = HealthCheckupCenter.CityMaster_Id join StateMaster on StateMaster.Id = HealthCheckupCenter.StateMaster_Id where HealthCheckupCenter.Id=" + CenterId;
            var mek = ent.Database.SqlQuery<ReportDTO>(la).ToList();
            model.LabName = mek.FirstOrDefault().LabName;
            model.LicenceNumber = mek.FirstOrDefault().LicenceNumber;
            model.MobileNumber = mek.FirstOrDefault().MobileNumber;
            model.StateName = mek.FirstOrDefault().StateName;
            model.StateName = mek.FirstOrDefault().CityName;
            model.Location = mek.FirstOrDefault().Location;
            if (sdate != null && edate != null)
            {
                var labs = @"select CONVERT(VARCHAR(10), P.TestDate, 111) as TestDate1, P.TestDate, Sum(P.Amount) as Amount, D.LabName,D.Location, D.MobileNumber,D.LicenceNumber,S.StateName, C.CityName from CmpltCheckUp P join HealthCheckupCenter D on D.Id = p.Center_Id join CityMaster C on C.Id = D.CityMaster_Id join StateMaster S on S.Id = D.StateMaster_Id WHERE p.Center_Id='" + CenterId + "' and P.TestDate between '" + sdate + "' and '" + edate + "' GROUP BY P.TestDate, P.Amount, D.LabName, D.MobileNumber, S.StateName,C.CityName, D.LicenceNumber,D.Location ";
                var lab = ent.Database.SqlQuery<HealthCheckListVM>(labs).ToList();
                //doctorList = doctorList.Where(a => a.AppointmentDate >= sdate && a.AppointmentDate <= edate).ToList();
                if (lab.Count() == 0)
                {
                    TempData["MSG"] = "No Records";
                }
                else
                {
                    model.ViewCmplteCheckUp = lab;
                    ViewBag.Total = model.ViewCmplteCheckUp.Sum(a => a.Amount);
                    return View(model);
                }
            }
            else
            {
                var doctor = @"select CONVERT(VARCHAR(10), P.TestDate, 111) as TestDate1, P.TestDate, Sum(P.Amount) as Amount, D.LabName,D.Location, D.MobileNumber,D.LicenceNumber,S.StateName, C.CityName from CmpltCheckUp P join HealthCheckupCenter D on D.Id = p.Center_Id join CityMaster C on C.Id = D.CityMaster_Id join StateMaster S on S.Id = D.StateMaster_Id WHERE p.Center_Id=1 and  datepart(mm,P.TestDate) =month(getdate()) GROUP BY P.TestDate, P.Amount, D.LabName, D.MobileNumber, S.StateName,C.CityName, D.LicenceNumber,D.Location";
                var labList = ent.Database.SqlQuery<HealthCheckListVM>(doctor).ToList();
                if (labList.Count() == 0)
                {
                    TempData["MSG"] = "No Records";
                }
                else
                {
                    model.ViewCmplteCheckUp = labList;
                    ViewBag.Total = model.ViewCmplteCheckUp.Sum(a => a.Amount);
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult ViewHealth(string date)
        {
            var model = new ReportDTO();
            var lab = @"select CONVERT(VARCHAR(10), TestDate, 111) as TestDate1,* from CmpltCheckUp Join HealthCheckUp on HealthCheckUp.Id= CmpltCheckUp.Center_Id where  Convert(date,TestDate)='" + date + "'";
            var data = ent.Database.SqlQuery<ViewHealthDetails>(lab).ToList();
            if (data.Count() == 0)
            {
                TempData["MSG"] = "No Records";
            }
            else
            {
                model.ViewHealthDetails = data;
            }
            ViewBag.Total = model.ViewHealthDetails.Sum(a => a.TestAmount);
            return View(model);
        }


        public ActionResult Vendor()
        {
            return View();
        }
        public ActionResult Doctor()
        {
            var model = new VendorPaymentDTO();
            //double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Vendor' and Name='Doctor'").FirstOrDefault();
            string q = @"select Sum(pa.TotalFee) as Counts,v.VendorName, d.DoctorName, v.Id, d.ClinicName,  v.CompanyName from Doctor d join Vendor v on d.Vendor_Id = v.Id join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id  where pa.AppointmentDate  >= DATEADD(day,-7, GETDATE()) and pa.IsPaid=1 group by d.DoctorName,d.ClinicName, v.Id, v.CompanyName,v.VendorName";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if(data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            //ViewBag.Payment = payment;
            model.Vendorses = data;
            return View(model);
        }
       


        public ActionResult Driver()
        {
            var model = new VendorPaymentDTO();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Vendor' and Name='Driver'").FirstOrDefault();
            string q = @"select COUNT(d.Id) as Counts, v.VendorName,v.CompanyName, V.Id from Driver d join Vendor v on d.Vendor_Id = v.Id  where d.JoiningDate  >= DATEADD(day,-7, GETDATE()) group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            ViewBag.Payment = payment;
            model.Vendorses = data;
            return View(model);
        }
        public ActionResult Vehicle()
        {
            var model = new VendorPaymentDTO();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Vendor' and Name='Vehicle'").FirstOrDefault();
            string q = @"select COUNT(d.Id) as Amount, d.VendorName, d.CompanyName, d.Id from Vendor d 
join Vehicle v on v.Vendor_Id = d.Id  
join TravelRecordMaster trm on trm.Vehicle_Id = v.Id
where trm.RequestDate  >= DATEADD(day,-7, GETDATE()) and trm.IsPaid =1  group by d.VendorName,d.CompanyName, d.Id";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            ViewBag.Payment = payment;
            model.Vendorses = data;
            return View(model);
        }

        public ActionResult DoctorDetails(int id)
        {
            var model = new VendorPaymentDTO();
            string query = @"select CompanyName, VendorName from Vendor where Id=" + id;
            var datas = ent.Database.SqlQuery<VendorPaymentDTO>(query).ToList();
            model.CompanyName = datas.FirstOrDefault().CompanyName;
            model.VendorName = datas.FirstOrDefault().VendorName;
            string q = @"select d.Id, v.VendorName, v.CompanyName, d.DoctorName, pa.AppointmentDate, c.CityName, pa.Amount  from Doctor d join Vendor v on d.Vendor_Id = v.Id 
join CityMaster c on c.Id = d.CityMaster_Id
join dbo.PatientAppointment pa on pa.Doctor_Id = d.Id
where v.Id="+id+" and pa.IsPaid=1 and pa.AppointmentDate  >= DATEADD(day,-7, GETDATE()) order by d.Id desc";
var data = ent.Database.SqlQuery<VendorsDoctors>(q).ToList();
            if(data.Count() == 0)
            {
                TempData["Msg"] = "No Records";
                return View(model);
            }
            model.VendorDoctor = data;
            return View(model);
        }

        public ActionResult DriverDetails(int id)
        {
            var model = new VendorPaymentDTO();
            string query = @"select CompanyName, VendorName from Vendor where Id=" + id;
            var datas = ent.Database.SqlQuery<VendorPaymentDTO>(query).ToList();
            model.CompanyName = datas.FirstOrDefault().CompanyName;
            model.VendorName = datas.FirstOrDefault().VendorName;
            string q = @"select d.Id, v.VendorName, v.CompanyName, d.DriverName, c.CityName from Driver d join Vendor v on d.Vendor_Id = v.Id 
join CityMaster c on c.Id = d.CityMaster_Id
where v.Id=" + id + "  group by v.VendorName,v.CompanyName, d.DriverName,c.CityName,d.Id order by d.Id desc";
var data = ent.Database.SqlQuery<VendorsDriver>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["Msg"] = "No Records";
                return View(model);
            }
            model.VendorsDriver = data;
            return View(model);
        }

        public ActionResult VehicleDetails(int id)
        {
            var model = new VendorPaymentDTO();
            string query = @"select CompanyName, VendorName from Vendor where Id=" + id;
            var datas = ent.Database.SqlQuery<VendorPaymentDTO>(query).ToList();
            model.CompanyName = datas.FirstOrDefault().CompanyName;
            model.VendorName = datas.FirstOrDefault().VendorName;
            string q = @"select d.Id, v.VendorName, v.CompanyName, d.VehicleNumber from Vehicle d 
join TravelRecordMaster trm on trm.Vehicle_Id = d.Id
join Vendor v on d.Vendor_Id = v.Id where v.Id="+id+"  group by v.VendorName,v.CompanyName,d.VehicleNumber, d.Id order by d.Id desc";
            var data = ent.Database.SqlQuery<VendorsVehicle>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["Msg"] = "No Records";
                return View(model);
            }
            model.VendorsVehicle = data;
            return View(model);
        }

        public ActionResult HealthCheckupVendor()
        {
            var model = new VendorPaymentDTO();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Vendor' and Name='HealthCheckup'").FirstOrDefault();
            string q = @"select SUM(cc.Amount) as Counts, v.VendorName, V.Id, v.CompanyName from [HealthCheckupCenter] d 
join Vendor v on d.Vendor_Id = v.Id  
join CmpltCheckUp cc on cc.Center_Id = d.Id
join HealthBooking hb on hb.PatientId = cc.PatientId
where cc.TestDate  >= DATEADD(day,-7, GETDATE()) and hb.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            ViewBag.Payment = payment;
            model.Vendorses = data;
            return View(model);
        }

        public ActionResult LabVendor()
        {
            var model = new VendorPaymentDTO();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Vendor' and Name='Lab'").FirstOrDefault();
            string q = @"select Sum(bt.Amount) as Counts, v.VendorName,v.CompanyName, V.Id from Lab d join Vendor v on d.Vendor_Id = v.Id  
join BookTestLab bt on bt.Lab_Id = d.Id
join LabBooking lb on lb.PatientId = bt.Patient_Id
where bt.TestDate  >= DATEADD(day,-7, GETDATE()) and lb.IsPaid =1 group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            ViewBag.Payment = payment;
            model.Vendorses = data;
            return View(model);
        }

        public ActionResult HealthCheckupVDetails(int id)
        {
            var model = new VendorPaymentDTO();
            string query = @"select CompanyName, VendorName from Vendor where Id=" + id;
            var datas = ent.Database.SqlQuery<VendorPaymentDTO>(query).ToList();
            model.CompanyName = datas.FirstOrDefault().CompanyName;
            model.VendorName = datas.FirstOrDefault().VendorName;
            string q = @"select d.Id, v.VendorName, v.CompanyName, d.[LabName], c.CityName , cc.Amount, cc.TestDate from [HealthCheckupCenter] d join Vendor v on d.Vendor_Id = v.Id 
join CityMaster c on c.Id = d.CityMaster_Id
join CmpltCheckUp cc on cc.Center_Id = d.Id
join HealthBooking hb on hb.PatientId = cc.PatientId
where v.Id="+id+" and cc.TestDate  >= DATEADD(day,-7, GETDATE()) and  hb.IsPaid=1 group by v.VendorName,v.CompanyName, d.[LabName],cc.Amount,c.CityName,d.Id,cc.TestDate order by d.Id desc";
            var data = ent.Database.SqlQuery<VendorsHealth>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["Msg"] = "No Records";
                return View(model);
            }
            model.VendorsHealth = data;
            return View(model);
        }

        public ActionResult LabVDetails(int id)
        {
            var model = new VendorPaymentDTO();
            string query = @"select CompanyName, VendorName from Vendor where Id=" + id;
            var datas = ent.Database.SqlQuery<VendorPaymentDTO>(query).ToList();
            model.CompanyName = datas.FirstOrDefault().CompanyName;
            model.VendorName = datas.FirstOrDefault().VendorName;
            string q = @"select d.Id, v.VendorName, v.CompanyName, d.LabName, c.CityName, bt.TestDate, bt.Amount from Lab d join Vendor v on d.Vendor_Id = v.Id 
join CityMaster c on c.Id = d.CityMaster_Id
join BookTestLab bt on bt.Lab_Id = d.Id
join LabBooking lb on lb.PatientId = bt.Patient_Id
where v.Id="+id+" and lb.IsPaid=1 and bt.IsTaken=1  group by v.VendorName,v.CompanyName, d.LabName,c.CityName,d.Id,bt.TestDate, bt.Amount order by d.Id desc";
            var data = ent.Database.SqlQuery<VendorsLab>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["Msg"] = "No Records";
                return View(model);
            }
            model.VendorsLab = data;
            return View(model);
        }
        public ActionResult NurseVendor()
        {
            var model = new VendorPaymentDTO();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Vendor' and Name='Nurse'").FirstOrDefault();
            string q = @"select v.VendorName, v.CompanyName, V.Id ,
(IsNull(Datediff(day,ns.StartDate,ns.EndDate)* ns.PerDayAmount,0)) as Counts
from Nurse d 
join NurseService ns on ns.Nurse_Id = d.Id
join Vendor v on d.Vendor_Id = v.Id  where ns.ServiceAcceptanceDate  >= DATEADD(day,-7, GETDATE())  and ns.IsPaid=1 group by v.VendorName,v.CompanyName, V.Id,ns.StartDate,ns.EndDate,ns.PerDayAmount";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            ViewBag.Payment = payment;
            model.Vendorses = data;
            return View(model);
        }
        public ActionResult NurseVDetails(int id)
        {
            var model = new VendorPaymentDTO();
            string query = @"select CompanyName, VendorName from Vendor where Id=" + id;
            var datas = ent.Database.SqlQuery<VendorPaymentDTO>(query).ToList();
            model.CompanyName = datas.FirstOrDefault().CompanyName;
            model.VendorName = datas.FirstOrDefault().VendorName;
            string q = @"select d.Id, v.VendorName, v.CompanyName, d.NurseName,
(IsNull(Datediff(day,ns.StartDate,ns.EndDate)* ns.PerDayAmount,0)) as Counts,
ns.StartDate, ns.EndDate
from Nurse d
join NurseService ns on ns.Nurse_Id = d.Id
join Vendor v on d.Vendor_Id = v.Id where v.Id="+id+" and ns.IsPaid=1  group by v.VendorName,v.CompanyName, d.NurseName, d.Id,ns.StartDate,ns.EndDate,ns.PerDayAmount order by d.Id desc";
            var data = ent.Database.SqlQuery<VendorsNurse>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["Msg"] = "No Records";
                return View(model);
            }
            model.VendorsNurse = data;
            return View(model);
        }

        public ActionResult RWAVendor()
        {
            var model = new VendorPaymentDTO();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Vendor' and Name='RWA'").FirstOrDefault();
            string q = @"select COUNT(d.Id) as Amount, v.VendorName, v.CompanyName, V.Id from Vendor v join RWA d on d.Vendor_Id = v.Id  where d.JoiningDate  >= DATEADD(day,-7, GETDATE()) group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            ViewBag.Payment = payment;
            model.Vendorses = data;
            return View(model);
        }
        public ActionResult RWAVDetails(int id)
        {
            var model = new VendorPaymentDTO();
            string query = @"select CompanyName, VendorName from Vendor where Id=" + id;
            var datas = ent.Database.SqlQuery<VendorPaymentDTO>(query).ToList();
            model.CompanyName = datas.FirstOrDefault().CompanyName;
            model.VendorName = datas.FirstOrDefault().VendorName;
            string q = @"select d.Id, v.VendorName, v.CompanyName, d.AuthorityName from RWA d join Vendor v on d.Vendor_Id = v.Id where v.Id=" + id + "  group by v.VendorName,v.CompanyName, d.AuthorityName, d.Id order by d.Id desc";
            var data = ent.Database.SqlQuery<VendorsRWA>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["Msg"] = "No Records";
                return View(model);
            }
            model.VendorsRWA = data;
            return View(model);
        }

        public ActionResult PatientVendor()
        {
            var model = new VendorPaymentDTO();
            double payment = ent.Database.SqlQuery<double>(@"select Amount from PaymentMaster p where p.Department='Vendor' and Name='RWA'").FirstOrDefault();

            string q = @"select COUNT(d.Id) as Counts, v.VendorName, v.CompanyName, V.Id from Patient d join Vehicle v on d.Vendor_Id = v.Id  where d.JoiningDate  >= DATEADD(day,-7, GETDATE()) group by v.VendorName,v.CompanyName, V.Id";
            var data = ent.Database.SqlQuery<VendorList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Result";
                return View(model);
            }
            ViewBag.Payment = payment;
            model.Vendorses = data;
            return View(model);
        }
        public ActionResult PatientVDetails(int id)
        {
            var model = new VendorPaymentDTO();
            string query = @"select CompanyName, VendorName from Vendor where Id=" + id;
            var datas = ent.Database.SqlQuery<VendorPaymentDTO>(query).ToList();
            model.CompanyName = datas.FirstOrDefault().CompanyName;
            model.VendorName = datas.FirstOrDefault().VendorName;
            string q = @"select d.Id, v.VendorName, v.CompanyName, d.PatientName from Patient d join Vendor v on d.Vendor_Id = v.Id where v.Id=" + id + "  group by v.VendorName,v.CompanyName, d.Patient, d.Id order by d.Id desc";
            var data = ent.Database.SqlQuery<VendorsPatient>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["Msg"] = "No Records";
                return View(model);
            }
            model.VendorsPatient = data;
            return View(model);
        }

        public ActionResult CancelBookingDriver()
        {
            var model = new cancelReport();
            string query = @"select d.DriverName,trm.*
                            From Driver d
                            join TravelRecordMaster trm on d.Id=trm.Driver_Id 
                            where trm.CancelBy= 'driver';";
            var datas = ent.Database.SqlQuery<CancelBookingVM>(query).ToList();
            model.CancelBookingVM = datas;
            return View(model);
        }

        public ActionResult CancelBookingPatient()
        {
            var model = new cancelReport();
            string query = @"select p.PatientName,TRM.*
                             from Patient p
                             join  TravelRecordMaster TRM on p.Id=TRM.Patient_Id
                             where trm.CancelBy='Patient';";
            var data = ent.Database.SqlQuery<CancelBookingVM>(query).ToList();
            model.CancelBookingVM = data;
            return View(model);
        }

        public ActionResult DoctorCancelReportByUser()
        {
            var model = new cancelReport();
            string query = @"select PatientAppointment.Id,PatientAppointment.AppointmentDate,PatientAppointment.PaymentDate,CONCAT(CONVERT(NVARCHAR, TS.StartTime, 8), ' To ', CONVERT(NVARCHAR, TS.EndTime, 8)) AS SlotTime,
Specialist.SpecialistName,Doctor.Location,PatientAppointment.TotalFee,Doctor.DoctorId,Doctor.DoctorName,Doctor.MobileNumber,P.PatientName from PatientAppointment 
join Doctor on Doctor.Id = PatientAppointment.Doctor_Id 
join Specialist on Doctor.Specialist_Id = Specialist.Id 
join DoctorTimeSlot as TS on PatientAppointment.Slot_id=TS.Id 
left join Patient as P on P.Id=PatientAppointment.Patient_Id
where PatientAppointment.IsCancelled=1 order by PatientAppointment.Id desc";
            var data = ent.Database.SqlQuery<CancelDoctor>(query).ToList();
            model.CancelDoctor = data;
            return View(model);
        }

        public ActionResult NurseCancelReportByUser()
        {
            var model = new cancelReport();
            string query = @"select NS.Id,Nurse.NurseId,Nurse.NurseName,Nurse.MobileNumber,Nurse.Location,Nurse.Fee,NS.PaymentDate,NS.ServiceAcceptanceDate,NS.RequestDate,P.PatientName from Nurse
        left join NurseService as NS on Nurse.Id=ns.Nurse_Id  
		left join Patient as P on P.Id=NS.Patient_Id
        where NS.ServiceStatus='Cancelled' order by NS.Id desc";
            var data = ent.Database.SqlQuery<NurseCancel>(query).ToList();
            model.NurseCancel = data;
            return View(model);
        }

        public ActionResult CancelAppointmentBydoctor()
        {
            var model = new cancelReport();
            string query = @"select distinct PAA.ID,P.PatientRegNo,P.PatientName,P.MobileNumber,P.Location,CONCAT(CONVERT(NVARCHAR, TS.StartTime, 8), ' To ', CONVERT(NVARCHAR, TS.EndTime, 8)) AS SlotTime,
Specialist.SpecialistName,PA.AppointmentDate,PAA.UserWalletAmount,PAA.Amount,PAA.CancelDate,D.DoctorName from PenaltyAmount as PAA
inner join Doctor as D on PAA.Pro_Id=D.Id
join Specialist on D.Specialist_Id = Specialist.Id 
join Patient as P on P.Id=PAA.Patient_Id
inner join PatientAppointment as PA on PA.Doctor_Id=PAA.Pro_Id
join DoctorTimeSlot as TS on PA.Slot_id=TS.Id
where PA.IsCancelled=1";
            var data = ent.Database.SqlQuery<CancelDoctor>(query).ToList();
            model.CancelDoctor = data;
            return View(model);
        }
        public ActionResult CancelAppointmentByNurse()
        {
            var model = new cancelReport();
            string query = @"select Distinct PA.Id,P.PatientName,P.MobileNumber,P.Location,Nurse.Fee,NS.PaymentDate,PA.Amount,PA.UserWalletAmount,PA.CancelDate,NS.ServiceAcceptanceDate,NS.RequestDate,Nurse.NurseName from Nurse
          join NurseService as NS on Nurse.Id=ns.Nurse_Id  
		  join Patient as P on P.Id=NS.Patient_Id
		  join PenaltyAmount as PA on Nurse.Id=PA.Pro_Id
        where NS.ServiceStatus='Cancelled'";
            var data = ent.Database.SqlQuery<NurseCancel>(query).ToList();
            model.NurseCancel = data;
            return View(model);
        }
    }

}