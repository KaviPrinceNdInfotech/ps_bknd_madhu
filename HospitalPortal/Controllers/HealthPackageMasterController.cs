using AutoMapper;
using Common.Logging;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.RequestModel;
using HospitalPortal.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    public class HealthPackageMasterController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(HealthPackageMasterController));

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(HealthPackageMasterDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                if (ent.HealthPackageMasters.Any(a => a.PackageName == model.PackageName))
                {
                    TempData["msg"] = "The Package Name  " + model.PackageName + " Already Exists";
                    return RedirectToAction("Add");
                }
                var domainModel = Mapper.Map<HealthPackageMaster>(model);
                ent.HealthPackageMasters.Add(domainModel);
                ent.SaveChanges();
                TempData["msg"] = "ok";
                //TempData["msg"] = "Successfully Submitted";
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
            var data = ent.HealthPackageMasters.Find(id);
            var model = Mapper.Map<HealthPackageMasterDTO>(data);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(HealthPackageMasterDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                var domainModel = Mapper.Map<HealthPackageMaster>(model);
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

        public ActionResult All(string term = null)
        {
            //var data = ent.HealthPackageMasters.ToList();
            var data = repos.GetPackageNames();
            if (term != null)
            {
                data = data.Where(a => a.PackageName.ToLower().Contains(term.ToLower())).ToList();
            }
            return View(data);
        }

        public ActionResult Delete(int id)
        {
            var data = ent.HealthPackageMasters.Find(id);
            try
            {
                ent.HealthPackageMasters.Remove(data);
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