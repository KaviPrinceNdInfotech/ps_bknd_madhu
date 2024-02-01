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
    public class SpecialistController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(SpecialistController));

        public ActionResult Add()
        {
            var model = new SpecialistDTO();
            model.Departments = new SelectList(repos.GetDepartments(), "Id", "DepartmentName");
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(SpecialistDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
            model.Departments = new SelectList(repos.GetDepartments(), "Id", "DepartmentName");
                    return View(model);
                }
                if (ent.Specialists.Any(a => a.SpecialistName == model.SpecialistName))
                {
                    TempData["msg"] = "This Specialist " + model.SpecialistName + " Already Exists";
                    return RedirectToAction("Add");
                }
                var domainModel = Mapper.Map<Specialist>(model);
                    ent.Specialists.Add(domainModel);
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
            var data = ent.Specialists.Find(id);
            var model = Mapper.Map<SpecialistDTO>(data);
            model.Departments = new SelectList(repos.GetDepartments(), "Id", "DepartmentName", model.Department_Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SpecialistDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.Departments = new SelectList(repos.GetDepartments(), "Id", "DepartmentName",model.Department_Id);
                    return View(model);
                }
                var domainModel = Mapper.Map<Specialist>(model);
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
            string query= @"select sp.*,d.DepartmentName from Specialist sp join Department d on sp.Department_Id = d.Id where sp.IsDeleted=0 order by SpecialistName";
            var data = ent.Database.SqlQuery<SpecialistDTO>(query).ToList();
            if (term != null)
            {
                data = data.Where(a => a.SpecialistName.ToLower().Contains(term.ToLower())).ToList();
            }
            return View(data);
        }

        public ActionResult Delete(int id)
        {
            var data = ent.Specialists.Find(id);
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