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
    public class LocationController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(CityController));

        public ActionResult Add()
        {
            var model = new LocationDTO();
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(LocationDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                    return View(model);
                }
                if (ent.Locations.Any(a => a.LocationName == model.LocationName))
                {
                    TempData["msg"] = "The Location Name  "+model.LocationName+" Already Exists";
                    return RedirectToAction("Add");
                }
                var domainModel = Mapper.Map<Location>(model);
                ent.Locations.Add(domainModel);
                ent.SaveChanges();
                TempData["msg"] = "ok";

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                TempData["msg"] = "Server Error";
            }
            return RedirectToAction("Add");
        }

        public ActionResult Edit(int id)
        {
            var data = ent.Locations.Find(id);
            var model = Mapper.Map<LocationDTO>(data);
            var city = ent.CityMasters.Find(model.City_Id);
            int stateId = ent.StateMasters.Find(city.StateMaster_Id).Id;
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName",stateId);
            model.Cities = new SelectList(repos.GetCitiesByState(stateId), "Id", "CityName",model.City_Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(LocationDTO model)
        {
            try
            {
                var city = ent.CityMasters.Find(model.City_Id);
                int stateId = ent.StateMasters.Find(city.StateMaster_Id).Id;
                model.States = new SelectList(repos.GetAllStates(), "Id", "StateName", stateId);
                model.Cities = new SelectList(repos.GetCitiesByState(stateId), "Id", "CityName", model.City_Id);
                if (!ModelState.IsValid)
                    return View(model);
                var domainModel = Mapper.Map<Location>(model);
                ent.Entry(domainModel).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                return RedirectToAction("All");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                TempData["msg"] = "Server Error";
                return RedirectToAction("Edit", model.Id);
            }
        }

        public ActionResult All()
        {
            string query = @"select loc.*,ct.CityName, st.StateName from Location loc join
CityMaster ct on loc.City_Id = ct.Id 
join StateMaster st on ct.StateMaster_Id = st.Id
where loc.IsDeleted=0";
            var data = ent.Database.SqlQuery<LocationDTO>(query);
            return View(data);
        }

        public ActionResult Delete(int id)
        {
            var data = ent.Locations.Find(id);
            try
            {
                data.IsDeleted = true;
                ent.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return RedirectToAction("All");
        }

    }
}