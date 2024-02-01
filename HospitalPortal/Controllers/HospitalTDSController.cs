using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    public class HospitalTDSController : Controller
    {
        DbEntities ent = new DbEntities();
        // GET: HospitalTDS
        public ActionResult TDSHospital()
        {
            return View();
        }

        public ActionResult Doctor(string term)
        {
            double commision = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Hospital'").FirstOrDefault();
            var model = new ReportDTO();
            var doctor = @"select Sum(P.TotalFee) as AMOUNT, P.Doctor_Id, D.DoctorName from dbo.PatientAppointment P join Doctor D ON d.Id = p.Doctor_Id join Hospital h on h.Id = d.HospitalId where p.IsPaid=1 and p.AppointmentDate between DateAdd(DD, -7, GetDate()) and getdate() group by Doctor_Id, DoctorName";
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
where p.IsPaid=1 and D.Id=" + DoctorId + " and P.AppointmentDate between DateAdd(DD, -7, GetDate()) and getdate() and p.IsPaid=1  GROUP BY P.AppointmentDate, P.Amount, D.DoctorName, D.MobileNumber, H.HospitalName,D.LicenceNumber,Patient.PatientName";
                var doctorList = ent.Database.SqlQuery<DoctorReports>(doctor1).ToList();
                model.DoctorReport = doctorList;
            }
            ViewBag.Total = model.DoctorReport.Sum(a => a.Amount);
            return View(model);
        }
    }
}