using AutoMapper;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Repositories;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    public class RoleUpdateController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(RoleUpdateController));
        [HttpGet]
        public ActionResult Department(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Department(UpdateDepartment model)
        {
            var data = ent.AdminLogins.Find(model.Id);
            var domain = AutoMapper.Mapper.Map<TempDepartment>(model);
            domain.IsDeleted = false;
            domain.Requested = data.Role;
            domain.IsApproved = false;
            ent.TempDepartments.Add(domain);
            ent.SaveChanges();
            TempData["msg"] = "Successfuly Sent to Administrator For Approval";
            return View(model);
        }

        [HttpGet]
        public ActionResult ViewMore(int id)
        {
            var model = new UpdateDepartment();
            var data = ent.AdminLogins.Find(id);
            if (data.Role == "doctor")
            {
                string q = @"select * from TempDepartment where Requested= 'Doctor'";
                var doctor = ent.Database.SqlQuery<DepartmentList>(q).ToList();
                model.DepartmentList = doctor;
            }
            else if (data.Role == "hospital")
            {
                string q = @"select * from TempDepartment where Requested= 'Hospital'";
                var doctor = ent.Database.SqlQuery<DepartmentList>(q).ToList();
                model.DepartmentList = doctor;
            }
            else
            {
                string q = @"select * from TempDepartment where Requested= 'Franchise'";
                var doctor = ent.Database.SqlQuery<DepartmentList>(q).ToList();
                model.DepartmentList = doctor;
            }
            return View(model);
        }

        public ActionResult ViewCity()
        {
            var model = new RequestedCity();
            string q = @"select *,ct.Id as CityId from AdminLogin al join CityTemp ct on al.Id = ct.Login_Id join StateMaster sm on sm.Id = ct.State_Id where ct.IsApproved=0";
            var data = ent.Database.SqlQuery<CityList>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Request Available";
                return View(model);
            }
            model.CityList = data;
            return View(model);
        }
        public ActionResult Update(int Id)
        {
            var values = ent.CityTemps.Find(Id);
            var city = new CityMaster();
            ent.Database.ExecuteSqlCommand("update CityTemp set IsApproved = 1 where Id=" + Id);
            city.CityName = values.CityName;
            city.IsDeleted = false;
            city.StateMaster_Id = (int)values.State_Id;
            ent.CityMasters.Add(city);
            ent.SaveChanges();
            return RedirectToAction("ViewCity");
        }


        public ActionResult Add(int id)
        {
            var model = new VehicleTempDTO();
            model.Login_Id = id;
            model.CategoryList = new SelectList(repos.GetCategory(), "Id", "CategoryName");
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(VehicleTempDTO model)
        {
            try
            {
                model.CategoryList = new SelectList(repos.GetCategory(), "Id", "CategoryName");
                if (!ModelState.IsValid)
                {
                    model.CategoryList = new SelectList(repos.GetCategory(), "Id", "CategoryName");
                    return View(model);
                }
                var domainModel = Mapper.Map<VehicleTemp>(model);
                domainModel.Login_Id = model.Id;
                domainModel.IsDeleted = false;
                domainModel.IsApproved = false;
                ent.VehicleTemps.Add(domainModel);
                ent.SaveChanges();
                TempData["msg"] = "Successfully Added";

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                TempData["msg"] = "Server Error";
            }
            return RedirectToAction("Add", new { id = model.Id });
        }

        public ActionResult All(int Id)
        {
            var model = new VendorAddedVehicleVM();
            var record = ent.AdminLogins.Find(Id);
            if (record.Role == "Franchise")
            {
                string Q = @"select * from VehicleTemp join MainCategory on VehicleTemp.Category_Id = MainCategory.Id where VehicleTemp.Login_Id=" + record.Id;
                var data = ent.Database.SqlQuery<AddedVehicleList>(Q).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No Records";
                    return View(model);
                }
                model.AddedVehicleList = data;
            }
            return View(model);

        }
    }
}