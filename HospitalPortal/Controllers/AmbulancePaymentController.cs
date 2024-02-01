using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    public class AmbulancePaymentController : Controller
    {
        DbEntities ent = new DbEntities();

        public ActionResult Show(int? Id)
        {
            var model = new AmbulanceList();
            if(Id > 0)
            {
                var doctor1 = @"select v.VehicleNumber as DriverName, IsNull(v.VehicleName,'NA') as VehicleName, 
v.Id as VehicleId
from TravelRecordMaster trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1  and trm.RequestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() group by v.VehicleNumber, v.VehicleName, 
v.Id";
                var data1 = ent.Database.SqlQuery<AmbulanceReport>(doctor1).ToList();
                model.Ambulance = data1;
                return View(model);
            }
            var doctor = @"select v.VehicleNumber as DriverName, IsNull(v.VehicleName,'NA') as VehicleName, 
v.Id as VehicleId
from TravelRecordMaster trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1 and trm.RequestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() group by v.VehicleNumber, v.VehicleName, 
v.Id";
            var data = ent.Database.SqlQuery<AmbulanceReport>(doctor).ToList();
            model.Ambulance = data;
            return View(model);
        }

        public ActionResult ViewDetails(int id, DateTime? sdate, DateTime? edate)
        {
             var model = new AmbulanceList();
            var vehicle = @"select * from Vehicle join Driver on Driver.Id = Vehicle.Driver_Id where Vehicle.Id=" + id;
            var mek = ent.Database.SqlQuery<AmbulanceList>(vehicle).ToList();
            model.VehicleName = mek.FirstOrDefault().VehicleName;
            model.VehicleNumber = mek.FirstOrDefault().VehicleNumber;
            model.DriverName = mek.FirstOrDefault().DriverName;
            if (sdate != null && edate != null)
            {
                var doct = @"select CONVERT(VARCHAR(10), AppointmentDate, 111) as AppointmentDate1, P.AppointmentDate, Sum(P.Amount) as Amount, D.DoctorName, D.MobileNumber, D.ClinicName,D.LicenceNumber  from PatientAppointment P join Doctor D on D.Id = p.Doctor_Id where p.IsPaid=1 and p.Doctor_Id='" + id + "' and P.AppointmentDate between '" + sdate + "' and '" + edate + "' GROUP BY P.AppointmentDate, P.Amount, D.DoctorName, D.MobileNumber, D.ClinicName,D.LicenceNumber ";
                var doctor1 = ent.Database.SqlQuery<AmbulanceReport>(doct).ToList();
                //doctorList = doctorList.Where(a => a.AppointmentDate >= sdate && a.AppointmentDate <= edate).ToList();
                model.Ambulance = doctor1;
            }
            else
            {
                var doctor1 = @"select p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
trm.Amount, trm.Distance, d.DriverName,
trm.PickUp_Place, trm.Drop_Place,
v.Id as VehicleId
from TravelRecordMaster trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1 and trm.RequestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE()";
                var doctorList = ent.Database.SqlQuery<AmbulanceReport>(doctor1).ToList();
                model.Ambulance = doctorList;
            }
            ViewBag.Total = model.Ambulance.Sum(a => a.Amount);
            return View(model);
        }

        public ActionResult Driver( int? id, int? pageNumber, DateTime? RequestDate, string name = null)
        {
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();
            var model = new AmbulanceList();
            if (RequestDate != null)
            {
                DateTime dateCriteria = RequestDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
trm.Amount, trm.Distance, d.DriverName,
trm.PickUp_Place, trm.Drop_Place,
v.Id as VehicleId
from TravelRecordMaster trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1 and d.Id = "+id+" and trm.RequestDate between '" + RequestDate + "' and '" + date + "'";
                var data1 = ent.Database.SqlQuery<AmbulanceReport>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Commission = commision;
                    int total = data1.Count;
                    pageNumber = (int?)pageNumber ?? 1;
                    int pageSize = 10;
                    decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                    model.TotalPages = (int)noOfPages;
                    model.PageNumber = (int)pageNumber;
                    data1 = data1.OrderBy(a => a.VehicleId).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                    if (name != null)
                    {
                        data1 = data1.Where(a => a.VehicleNumber.ToLower().Contains(name)).ToList();
                    }
                    model.Ambulance = data1;
                    return View(model);
                }
            }
            var doctor = @"select p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
trm.Amount, trm.Distance, d.DriverName,
trm.PickUp_Place, trm.Drop_Place,
v.Id as VehicleId
from TravelRecordMaster trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1 and d.Id = " + id + " and trm.RequestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE()";
            var data = ent.Database.SqlQuery<AmbulanceReport>(doctor).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record Of Current Week";
                return View(model);
            }
            else
            {
                ViewBag.Commission = commision;
                int total = data.Count;
                pageNumber = (int?)pageNumber ?? 1;
                int pageSize = 10;
                decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                model.TotalPages = (int)noOfPages;
                model.PageNumber = (int)pageNumber;
                data = data.OrderBy(a => a.VehicleId).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                model.Ambulance = data;
                return View(model);
            }
        }

        public ActionResult DriverTDS(int? id, int? pageNumber, DateTime? RequestDate, string name = null)
        {
            double commision = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Vehicle'").FirstOrDefault();
            var model = new AmbulanceList();
            if (RequestDate != null)
            {
                DateTime dateCriteria = RequestDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
trm.Amount, trm.Distance, d.DriverName,
trm.PickUp_Place, trm.Drop_Place,
v.Id as VehicleId
from TravelRecordMaster trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1 and d.Id = " + id + " and trm.RequestDate between '" + RequestDate + "' and '" + date + "'";
                var data1 = ent.Database.SqlQuery<AmbulanceReport>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Commission = commision;
                    int total = data1.Count;
                    pageNumber = (int?)pageNumber ?? 1;
                    int pageSize = 10;
                    decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                    model.TotalPages = (int)noOfPages;
                    model.PageNumber = (int)pageNumber;
                    data1 = data1.OrderBy(a => a.VehicleId).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                    if (name != null)
                    {
                        data1 = data1.Where(a => a.VehicleNumber.ToLower().Contains(name)).ToList();
                    }
                    model.Ambulance = data1;
                    return View(model);
                }
            }
            var doctor = @"select p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
trm.Amount, trm.Distance, d.DriverName,
trm.PickUp_Place, trm.Drop_Place,
v.Id as VehicleId
from TravelRecordMaster trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1 and d.Id = " + id + " and trm.RequestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE()";
            var data = ent.Database.SqlQuery<AmbulanceReport>(doctor).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record Of Current Week";
                return View(model);
            }
            else
            {
                ViewBag.Commission = commision;
                int total = data.Count;
                pageNumber = (int?)pageNumber ?? 1;
                int pageSize = 10;
                decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                model.TotalPages = (int)noOfPages;
                model.PageNumber = (int)pageNumber;
                data = data.OrderBy(a => a.VehicleId).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                model.Ambulance = data;
                return View(model);
            }
        }

        public ActionResult DriverReport()
        {
            return View();
        }

        private int GetDriverId()
        {
            int loginId = Convert.ToInt32(User.Identity.Name);
            int DriverId = ent.Database.SqlQuery<int>("select Id from Driver where AdminLogin_Id=" + loginId).FirstOrDefault();
            return DriverId;
        }

        public ActionResult Daily(string term)
        {
            int id = GetDriverId();
            var model = new AmbulanceList();
            string q = @"selectselect v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
v.Id as VehicleId
from TravelRecordMaster trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1 and v.Id = "+id+ " and Convert(Date,trm.RequestDate) = (GetDate()) group by v.VehicleNumber, v.VehicleName, v.Id";
            var data = ent.Database.SqlQuery<AmbulanceReport>(q).ToList();
            if(data.Count()  == 0)
            {
                TempData["msg"] = "No Current Record Present";
                return View(model);
            }
            model.Ambulance = data;
            return View(model);
        }

        public class AmbulanceList
        {
            public DateTime? RequestDate { get; set; }
            public int TotalPages { get; set; }
            public int PageNumber { get; set; }
            public string VehicleNumber { get; set; }
            public string VehicleName { get; set; }
            public string DriverName { get; set; }

            public IEnumerable<AmbulanceReport> Ambulance { get; set; }
        }

        public class AmbulanceReport
        {
            public int VehicleId { get; set; }

            public string VehicleNumber { get; set; }
            public string VehicleName { get; set; }
            public double Amount { get; set; }
            public double Distance { get; set; }
            public string PatientName { get; set; }
            public string DriverName { get; set; }
            public string PickUp_Place { get; set; }
            public string Drop_Place { get; set; }
        }
        
    }
}