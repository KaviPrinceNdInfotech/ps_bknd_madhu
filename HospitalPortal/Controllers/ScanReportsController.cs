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
    public class ScanReportsController : Controller
    {
        DbEntities ent = new DbEntities();


        // GET: ScanReports
        public ActionResult Lab()
        {
            var model = new ScanReportVM();
            model.StateList = new SelectList(ent.StateMasters.ToList(), "Id", "StateName");
            return View(model);
        }
        public ActionResult GetCityByStateId(int StateMaster_Id)
        {
            var dpt = ent.CityMasters.Where(a => a.StateMaster_Id == StateMaster_Id).ToList();
            var data = Mapper.Map<IEnumerable<CityMasterDTO>>(dpt);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocationByCityId(int CityMaster_Id)
        {
            var dpt = ent.Locations.Where(a => a.City_Id == CityMaster_Id).ToList();
            var data = Mapper.Map<IEnumerable<LocationDTO>>(dpt);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LabList(int State, int City, int Location, string LabName = null)
        {
            var model = new ScanReportVM();
            string qry = @"select l.Id,l.LabName from Lab l join BookTestLab bl on bl.Lab_Id = l.Id where bl.IsPaid=1 and bl.IsTaken=1 AND l.CityMaster_Id="+City+" and l.StateMaster_Id="+State+" and l.Location_Id="+Location+ "  GROUP BY l.LabName, l.Id";
            var data = ent.Database.SqlQuery<ViewLabReport>(qry).ToList();
            if(data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            if(LabName != null)
            {
                data = data.Where(a => a.LabName.ToLower().Contains(LabName)).ToList();
            }
            model.LabList = data;
            return View(model);
        }

        public ActionResult Health()
        {
            var model = new ScanReportVM();
            model.StateList = new SelectList(ent.StateMasters.ToList(), "Id", "StateName");
            return View(model);
        }

        public ActionResult HealthList(int State, int City, int Location, string LabName = null)
        {
            var model = new ScanReportVM();
            string qry = @"select l.Id,l.LabName from Lab l join CmpltCheckUp bl on bl.Center_Id = l.Id where bl.IsPaid=1 and bl.IsTaken=1 AND l.CityMaster_Id=" + City + " and l.StateMaster_Id=" + State + " and l.Location_Id=" + Location + "  GROUP BY l.LabName, l.Id";
            var data = ent.Database.SqlQuery<ViewLabReport>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            if (LabName != null)
            {
                data = data.Where(a => a.LabName.ToLower().Contains(LabName)).ToList();
            }
            model.LabList = data;
            return View(model);
        }

        public ActionResult Doctor()
        {
            var model = new ScanReportVM();
            model.StateList = new SelectList(ent.StateMasters.ToList(), "Id", "StateName");
            return View(model);
        }

        public ActionResult DoctorList(int State, int City, string DoctorName = null)
        {
            var model = new ScanReportVM();
            string qry = @"select l.Id,l.DoctorName from Doctor l join DoctorReports bl on bl.Doctor_Id = l.Id where l.CityMaster_Id=" + City + " and l.StateMaster_Id=" + State + " GROUP BY l.DoctorName, l.Id";
            var data = ent.Database.SqlQuery<ViewDoctorReport>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            if (DoctorName != null)
            {
                data = data.Where(a => a.DoctorName.ToLower().Contains(DoctorName)).ToList();
            }
            model.DoctorList = data;
            return View(model);
        }


        public ActionResult Hospital()
        {
            var model = new ScanReportVM();
            model.StateList = new SelectList(ent.StateMasters.ToList(), "Id", "StateName");
            return View(model);
        }


        public ActionResult HospitalList(int State, int City, string HospitalName = null)
        {
            var model = new ScanReportVM();
            string qry = @"select l.Id,l.HospitalName from Hospital l join HospitalReport bl on bl.Hospital_Id = l.Id where l.CityMaster_Id=" + City + " and l.StateMaster_Id=" + State + " GROUP BY l.HospitalName, l.Id";
            var data = ent.Database.SqlQuery<ViewHospitalReport>(qry).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            if (HospitalName != null)
            {
                data = data.Where(a => a.HospitalName.ToLower().Contains(HospitalName)).ToList();
            }
            model.HospitalList = data;
            return View(model);
        }
    }
}