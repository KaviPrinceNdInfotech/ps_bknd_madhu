using AutoMapper;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    public class ContentManagmentController : Controller
    {
        DbEntities ent = new DbEntities();
        // GET: ContentManagment
        [HttpGet]
        public ActionResult Add()
        {
            var model = new PageMasterDTO();
            var Q = @"select * from PageMaster where IsDeleted=0";
            var data = ent.Database.SqlQuery<PageMasterVM>(Q).ToList();
            model.PageMasterList = data;
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(PageMasterDTO model)
        {
            var domain = new PageMaster();
            domain.PageName = model.PageName;
            domain.IsDeleted = false;
            ent.PageMasters.Add(domain);
            ent.SaveChanges();
            return RedirectToAction("Add");
        }

        public ActionResult Delete(int id)
        {
            var data = ent.PageMasters.Find(id);
            data.IsDeleted = true;
            ent.SaveChanges();
            return RedirectToAction("Add");
        }

        public ActionResult Edit(int id)
        {
            var data = ent.PageMasters.Find(id);
            var model = Mapper.Map<PageMasterDTO>(data);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(PageMasterDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                var domainModel = Mapper.Map<PageMaster>(model);
                ent.Entry(domainModel).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                return RedirectToAction("Add");
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                TempData["msg"] = "Server Error";
                return RedirectToAction("Edit", model.Id);
            }
        }
    }
}