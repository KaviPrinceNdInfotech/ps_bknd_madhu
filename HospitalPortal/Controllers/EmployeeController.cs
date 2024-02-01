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
    public class EmployeeController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(StateController));

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(EmployeeDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                    var domainModel = Mapper.Map<Employee>(model);
                    ent.Employees.Add(domainModel);
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
            var data = ent.Employees.Find(id);
            var model = Mapper.Map<EmployeeDTO>(data);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EmployeeDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                var domainModel = Mapper.Map<Employee>(model);
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
            var data = ent.Employees.Where(a=>!a.IsDeleted).ToList();
            if (term != null)
                data = data.Where(A => A.EmployeetName.ToLower().Contains(term)).ToList();
            return View(data);
        }

        public ActionResult Delete(int id)
        {
            var data = ent.Employees.Find(id);
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