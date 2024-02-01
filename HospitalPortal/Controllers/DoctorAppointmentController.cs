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
    public class DoctorAppointmentController : Controller
    {

        [HttpGet]
        // GET: DoctorAppointment
        public ActionResult ShowAppointment(string term, DateTime? AppointmentDate)
        {
            DbEntities  ent = new DbEntities();
            var model = new Appointment();
            string query = @"select Doctor.*,States.*,Dept.*, Special.*, Patient.Id, Patient.PatientName, dbo.PatientAppointment.EndSlotTime,dbo.PatientAppointment.StartSlotTime, dbo.PatientAppointment.AppointmentDate from Patient join dbo.PatientAppointment on Patient.Id = dbo.PatientAppointment.Patient_Id join Doctor on Doctor.Id = PatientAppointment.Doctor_Id join StateMaster States on States.Id = Doctor.StateMaster_Id join CityMaster City on City.Id = Doctor.CityMaster_Id  join Department Dept on Dept.Id = Doctor.Department_Id join Specialist Special on Special.Id = Doctor.Specialist_Id where dbo.PatientAppointment.IsBooked=1 order by dbo.PatientAppointment.AppointmentDate desc";
            var data = ent.Database.SqlQuery<ViewAppointment>(query).ToList();
            if(!string.IsNullOrEmpty(term))
            {
                data =data.Where(a => a.DoctorName.StartsWith(term)).ToList();
            }
            if (AppointmentDate != null)
            {
                data = data.Where(a => a.AppointmentDate == AppointmentDate).ToList();
            }
            model.viewAppointment = data;
            return View(model);
            
        }
    }
}