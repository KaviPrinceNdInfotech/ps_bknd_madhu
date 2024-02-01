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
    public class VehicleTypeController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(VehicleTypeController));

        public ActionResult Add()
        {
            var model = new VehicleTypeDTO();
            model.CategoryList = new SelectList(repos.GetCategory(), "Id", "CategoryName");
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(VehicleTypeDTO model)
        {
            try
            {
                model.CategoryList = new SelectList(repos.GetCategory(), "Id", "CategoryName");
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                    var domainModel = Mapper.Map<VehicleType>(model);
                    ent.VehicleTypes.Add(domainModel);
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
            var data = ent.VehicleTypes.Find(id);
            var model = Mapper.Map<VehicleTypeDTO>(data);
            model.CategoryList = new SelectList(repos.GetCategory(), "Id", "CategoryName", data.Category_Id);
            model.CategoryId = (int)data.Category_Id;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(VehicleTypeDTO model)
        {
            model.CategoryList = new SelectList(repos.GetCategory(), "Id", "CategoryName",model.CategoryId);
            try
            {
                if (!ModelState.IsValid)
                    return View(model);
                var domainModel = Mapper.Map<VehicleType>(model);
                model.CategoryList = new SelectList(repos.GetCategory(), "Id", "CategoryName");
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

        public ActionResult All(int? CategoryId)
        {
            var model = new VehicleTypeLISTvm();
            model.CategoryList = new SelectList(repos.GetCategory(), "Id", "CategoryName");
            string q = @"select * from VehicleType vt join MainCategory mc on mc.Id = vt.Category_Id where vt.IsDeleted=0 and mc.IsDeleted=0 order by CategoryName asc,VehicleTypeName asc";
            var data = ent.Database.SqlQuery<ListVehicleType>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            if (CategoryId != 0 && CategoryId != null)
            {
                string q1 = @"select * from VehicleType vt join MainCategory mc on mc.Id = vt.Category_Id where vt.IsDeleted=0 and mc.IsDeleted=0 and vt.Category_Id=" + CategoryId + " order by CategoryName asc";
                var data1 = ent.Database.SqlQuery<ListVehicleType>(q1).ToList();
                if(data1.Count() == 0)
                {
                    TempData["msg"] = "No Records";
                    return View(model);
                }
                model.ListVehicleType = data1;
                return View(model);
            }
            model.ListVehicleType = data;
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var data = ent.VehicleTypes.Find(id);
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

        public ActionResult PriceList(int VehicleType)
        {
            //var model = new VehicleList();
            //var q = @"select * from VehicleType where Id=" + VehicleType;
            //var data = ent.Database.SqlQuery<VehicleItem>(q).ToList();
            //if(data.Count() == 0)
            //{
            //    TempData["msg"] = "No Records";
            //    return View(model);
            //}
            //model
            //model.vahan_suchi = data;

            var model = ent.VehicleTypes.Find(VehicleType);
            var data = Mapper.Map<VehicleType>(model);
            return View(data);
        }
    }
}