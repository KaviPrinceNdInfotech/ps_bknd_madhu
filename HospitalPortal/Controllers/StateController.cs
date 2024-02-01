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
    public class StateController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(StateController));

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(StateMasterDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                if (ent.StateMasters.Where(a => a.IsDeleted == false).Any(a => a.StateName == model.StateName))
                {
                    TempData["msg"] = "The State Name  " + model.StateName + " Already Exists";
                    return RedirectToAction("Add");
                }
                var domainModel = Mapper.Map<StateMaster>(model);
                    ent.StateMasters.Add(domainModel);
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
            var data = ent.StateMasters.Find(id);
            var model = Mapper.Map<StateMasterDTO>(data);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(StateMasterDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                var domainModel = Mapper.Map<StateMaster>(model);
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
            var data = repos.GetAllStates();
            if (term != null)
            {
                data = data.Where(a => a.StateName.ToLower().Contains(term.ToLower())).ToList();
            }
            return View(data);
        }

        public ActionResult Delete(int id)
        {
            var data = ent.StateMasters.Find(id);
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