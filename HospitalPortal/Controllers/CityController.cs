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
    [Authorize]
    public class CityController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(CityController));

        public ActionResult Add()
        {
            var model = new CityMasterDTO();
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(CityMasterDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
                    return View(model);
                }
                if (ent.CityMasters.Where(a => a.IsDeleted == false).Any(a => a.CityName == model.CityName))
                {
                    TempData["msg"] = "The City Name  " + model.CityName + " Already Exists";
                    return RedirectToAction("Add");
                }
                var domainModel = Mapper.Map<CityMaster>(model);
                    ent.CityMasters.Add(domainModel);
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
            var data = ent.CityMasters.Find(id);
            var model = Mapper.Map<CityMasterDTO>(data);
            model.States = new SelectList(repos.GetAllStates(), "Id", "StateName", model.StateMaster_Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CityMasterDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.States = new SelectList(repos.GetAllStates(), "Id", "StateName",model.StateMaster_Id);
                    return View(model);
                }
                var domainModel = Mapper.Map<CityMaster>(model);
                ent.Entry(domainModel).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                return RedirectToAction("All");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                TempData["msg"] = "Server Error";
                return RedirectToAction("Edit",model.Id);
            }
        }

        public ActionResult All(string term = null)
        {
            string query= @"select cm.*,sm.StateName from citymaster cm join StateMaster sm
on cm.StateMaster_Id = sm.Id
where cm.IsDeleted=0 order by sm.StateName asc, cm.CityName asc";
            var data = ent.Database.SqlQuery<CityMasterDTO>(query).ToList();
            if(term != null)
            {
                data = data.Where(a => a.CityName.ToLower().Contains(term.ToLower()) || a.StateName.ToLower().Contains(term.ToLower())).ToList();
            }
            return View(data);
        }

        public ActionResult Delete(int id)
        {
            var data = ent.CityMasters.Find(id);
            try
            {
                data.IsDeleted = true;
                ent.SaveChanges();
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
            }
            return RedirectToAction("All");
        }


    }
}