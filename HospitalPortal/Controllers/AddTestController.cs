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
    public class AddTestController : Controller
    {
        // GET: AddTest
        DbEntities ent = new DbEntities();
        ILog log = LogManager.GetLogger(typeof(TestController));
        [HttpGet]
        public ActionResult Add()
        {
            //tewst
            return View();
        }

        [HttpPost]
        public ActionResult Add(TestDTO model)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage));
            }
            if (ent.LabTests.Any(a => a.TestName== model.TestName))
            {
                TempData["msg"] = "Already Exist with this Name";
                return RedirectToAction("Add");
            }
            var Domain = AutoMapper.Mapper.Map<LabTest>(model);
            ent.LabTests.Add(Domain);
            ent.SaveChanges();
            TempData["msg"] = "Successfully Saved";
            return RedirectToAction("Add");
        }

        [HttpGet]
        public ActionResult All(string term = null)
        {
            if (!string.IsNullOrEmpty(term))
            {
                string qry = @"select * from LabTest where TestName='" + term + "'";
                var data1 = ent.Database.SqlQuery<TestDTO>(qry);
                return View(data1);
            }
            string query = @"select * from LabTest";
            var data = ent.Database.SqlQuery<TestDTO>(query);
            return View(data);

        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var data = ent.LabTests.Find(id);
            try
            {
                ent.LabTests.Remove(data);
                ent.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return RedirectToAction("All");
        }

        public ActionResult Edit(int id)
        {
            var data = ent.LabTests.Find(id);
            var model = Mapper.Map<TestDTO>(data);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(TestDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var domainModel = Mapper.Map<LabTest>(model);
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

        public ActionResult aa()
        {
            return View();
        }
    }
}