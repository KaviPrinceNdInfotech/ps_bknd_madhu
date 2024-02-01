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
    public class DepartmentController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(DepartmentController));

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(DepartmentDTO model)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                if (ent.Departments.Any(a => a.DepartmentName == model.DepartmentName))
                {
                    TempData["msg"] = "This Deparment " + model.DepartmentName + " Already Exists";
                    return RedirectToAction("Add");
                }
                var domainModel = Mapper.Map<Department>(model);
                    ent.Departments.Add(domainModel);
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
            var data = ent.Departments.Find(id);
            var model = Mapper.Map<DepartmentDTO>(data);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(DepartmentDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                var domainModel = Mapper.Map<Department>(model);
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
            var data = repos.GetDepartments();
            if (term != null)
            {
                data = data.Where(a => a.DepartmentName.ToLower().Contains(term.ToLower())).ToList();
            }
            return View(data);
        }

        public ActionResult Delete(int id)
        {
            var data = ent.Departments.Find(id);
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