using HospitalPortal.Models;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    public class HospitalCommissionController : Controller
    {
        DbEntities ent = new DbEntities();
        // GET: HospitalCommission
        public ActionResult Hospital()
        {
            var model = new HospitalList();
            model.HospitalsList = new SelectList(ent.Hospitals.Where(a => a.IsDeleted == false && a.IsApproved == true).ToList(), "Id", "HospitalName");
            return View(model);
        }

        public ActionResult Doctor(int HospitalId, string term)
        {
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Hospital'").FirstOrDefault();
            var model = new ReportDTO();
            var doctor = @"select Sum(P.Amount) as AMOUNT, P.Doctor_Id, D.DoctorName from dbo.PatientAppointment P join Doctor D ON d.Id = p.Doctor_Id join Hospital h on h.Id = d.HospitalId where p.IsPaid=1 and p.AppointmentDate between DateAdd(DD, -7, GetDate()) and getdate() and h.Id="+HospitalId+" group by Doctor_Id, DoctorName";
            var data = ent.Database.SqlQuery<DoctorReports>(doctor).ToList();
            model.DoctorReport = data;
            ViewBag.Commission = commision == 0 ? 0 : commision;
            return View(model);
        }

        public ActionResult ViewDoctorDetails(int DoctorId, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDTO();
            var doctor = @"select * from Doctor d join Hospital h on d.HospitalId = h.Id where d.Id=" + DoctorId;
            var mek = ent.Database.SqlQuery<ReportDTO>(doctor).ToList();
            model.HospitalName = mek.FirstOrDefault().HospitalName;
            model.DoctorName = mek.FirstOrDefault().DoctorName;
            model.LicenceNumber = mek.FirstOrDefault().LicenceNumber;
            model.MobileNumber = mek.FirstOrDefault().MobileNumber;
            if (sdate != null && edate != null)
            {
                var doct = @"select CONVERT(VARCHAR(10), AppointmentDate, 111) as AppointmentDate1, H.HospitalName, P.AppointmentDate, Sum(P.Amount) as Amount, D.DoctorName, D.MobileNumber, D.LicenceNumber  from dbo.PatientAppointment P join Doctor D on D.Id = p.Doctor_Id join Hospital h on h.Id = d.HospitalId where p.IsPaid=1 and and D.Id='" + DoctorId + "' and P.AppointmentDate between '" + sdate + "' and '" + edate + "' GROUP BY P.AppointmentDate, P.Amount, D.DoctorName, D.MobileNumber, D.HospitalName,D.LicenceNumber ";
                var doctor1 = ent.Database.SqlQuery<DoctorReports>(doct).ToList();
                //doctorList = doctorList.Where(a => a.AppointmentDate >= sdate && a.AppointmentDate <= edate).ToList();
                model.DoctorReport = doctor1;
            }
            else
            {
                var doctor1 = @"select CONVERT(VARCHAR(10), AppointmentDate, 111) as AppointmentDate1,Patient.PatientName, H.HospitalName, P.AppointmentDate, Sum(P.Amount) as Amount, D.DoctorName, D.MobileNumber, D.LicenceNumber  from dbo.PatientAppointment P join Doctor D on D.Id = p.Doctor_Id join Hospital h on h.Id = d.HospitalId 
join Patient on Patient.Id = p.Patient_Id
where p.IsPaid=1 and D.Id="+ DoctorId + " and P.AppointmentDate between DateAdd(DD, -7, GetDate()) and getdate() and p.IsPaid=1  GROUP BY P.AppointmentDate, P.Amount, D.DoctorName, D.MobileNumber, H.HospitalName,D.LicenceNumber,Patient.PatientName";
                var doctorList = ent.Database.SqlQuery<DoctorReports>(doctor1).ToList();
                model.DoctorReport = doctorList;
            }
            ViewBag.Total = model.DoctorReport.Sum(a => a.Amount);
            return View(model);
        }

        public ActionResult Nurse(int HospitalId, string term)
        {
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Nurse'").FirstOrDefault();
            var model = new ReportDTO();
            var doctor = @"select Sum(P.TotalFee) as AMOUNT, P.Nurse_Id, D.NurseName from NurseService P join Nurse D ON d.Id = p.Nurse_Id join Hospital h on h.Id = d.Hospital_Id where p.IsPaid=1 and p.ServiceAcceptanceDate between DateAdd(DD, -7, GetDate()) and getdate() and IsPaid = 1 and h.Id= "+HospitalId+" and p.Nurse_Id != Null group by p.Nurse_Id, d.NurseName";
            var data = ent.Database.SqlQuery<HospitalNurseCommissionReport>(doctor).ToList();
            model.HospitalNurseCommissionReportList = data;
            ViewBag.Commission = commision == 0 ? 0 : commision;
            return View(model);
        }


        public ActionResult ViewNurseDetails(int NurseId, DateTime? sdate, DateTime? edate)
        {
            var model = new ReportDTO();
            var nurse = @"select *, d.NurseName as DoctorName from Nurse d join Hospital h on d.Hospital_Id = h.Id where d.Id=" + NurseId;
            var mek = ent.Database.SqlQuery<ReportDTO>(nurse).ToList();
            model.HospitalName = mek.FirstOrDefault().HospitalName;
            model.DoctorName = mek.FirstOrDefault().DoctorName;
            model.MobileNumber = mek.FirstOrDefault().MobileNumber;
            if (sdate != null && edate != null)
            {
                var doct = @"select ns.Id,p.PatientName,
                             n.MobileNumber as ContactNumber,
                             ns.IsPaid,
case when ns.PaymentDate is null then 'N/A' else Convert(nvarchar(100), ns.PaymentDate, 103) end as PaymentDate,
case when ns.ServiceAcceptanceDate is null then 'N/A' else Convert(nvarchar(100), ns.ServiceAcceptanceDate, 103) end as ServiceAcceptanceDate,
Convert(nvarchar(100), ns.RequestDate, 103) as RequestDate,
ns.RequestDate as rDate,
Convert(nvarchar(100), ns.StartDate, 103) + '-' + Convert(nvarchar(100), ns.EndDate, 103) as ServiceTiming,
Convert(nvarchar(100), ns.ServiceDate, 103) as ServiceDate,
Datediff(day,ns.StartDate,ns.EndDate) as TotalDays,
IsNull(ns.PerDayAmount,0) as Fee,
IsNull(TotalFee,0) as TotalFee,
ns.ServiceType,
ns.ServiceTime,
ns.ServiceStatus
 from NurseService ns
 join Patient p on ns.Patient_Id = p.Id
 join nurse n on ns.Nurse_Id=n.Id
 join Hospital h on h.Id = n.Hospital_Id
where ns.Nurse_Id = n.Id
and between '" + sdate + "' and '" + edate + "' and n.Nurse_Id = " + NurseId + " order by ns.Id desc";
                var doctor1 = ent.Database.SqlQuery<DoctorReports>(doct).ToList();
                //doctorList = doctorList.Where(a => a.AppointmentDate >= sdate && a.AppointmentDate <= edate).ToList();
                model.DoctorReport = doctor1;
            }
            else
            {
                var nurse1 = @"select ns.Id,n.NurseName,
                             n.MobileNumber as ContactNumber,
                             ns.IsPaid,
case when ns.PaymentDate is null then 'N/A' else Convert(nvarchar(100), ns.PaymentDate, 103) end as PaymentDate,
case when ns.ServiceAcceptanceDate is null then 'N/A' else Convert(nvarchar(100), ns.ServiceAcceptanceDate, 103) end as ServiceAcceptanceDate,
Convert(nvarchar(100), ns.RequestDate, 103) as RequestDate,
ns.RequestDate as rDate,
Convert(nvarchar(100), ns.StartDate, 103) + '-' + Convert(nvarchar(100), ns.EndDate, 103) as ServiceTiming,
Convert(nvarchar(100), ns.ServiceDate, 103) as ServiceDate,
Datediff(day,ns.StartDate,ns.EndDate) as TotalDays,
IsNull(ns.PerDayAmount,0) as Fee,
IsNull(TotalFee,0) as TotalFee,
ns.ServiceType,
ns.ServiceTime,
ns.ServiceStatus
 from NurseService ns
 join Patient p on ns.Patient_Id = p.Id
 join nurse n on ns.Nurse_Id=n.Id
 join Hospital h on h.Id = n.Hospital_Id
where ns.Nurse_Id = n.Id
and and ns.ServiceAcceptanceDate between DateAdd(DD, -7, GetDate()) and getdate()
and n.Nurse_Id = " + NurseId+" order by ns.Id desc";
                var nurseList = ent.Database.SqlQuery <HospitalNurseCommissionReport>(nurse1).ToList();
                model.HospitalNurseCommissionReportList = nurseList;
            }
            ViewBag.Total = model.HospitalNurseCommissionReportList.Sum(a => a.Amount);
            return View(model);
        }
    }
}