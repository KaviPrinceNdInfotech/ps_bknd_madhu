using AutoMapper;
using Common.Logging;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    public class CategoryController : Controller
    {
        DbEntities ent = new DbEntities();
        ILog log = LogManager.GetLogger(typeof(CategoryController));
        // GET: Category
        public ActionResult Add()
        {
            var model = new MainCategoryDTO();
            string q = @"select * from MainCategory where IsDeleted= 0 order by CategoryName asc";
            var data = ent.Database.SqlQuery<MainCategoryList>(q).ToList();
            if(data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            model.MainCategoryList = data;
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(MainCategoryDTO model)
        {
            var domain = Mapper.Map<MainCategory>(model);
            domain.IsDeleted = false;
            domain.AmbulanceType_id =model.AmbulanceType_id;
            if(model .AmbulanceType_id==1)
            {
                domain.Type = "Regular";
            }
            else if(model.AmbulanceType_id == 2)
            {
                domain.Type = "Road Accident";
            }
            else if (model.AmbulanceType_id == 3)
            {
                domain.Type = "Funeral/MortuaryService";

            }
            
            ent.MainCategories.Add(domain);
            ent.SaveChanges();
            TempData["msg1"] = "Successfully Saved";
            return RedirectToAction("Add");
        }


        public ActionResult Delete(int Id)
        {
            string q = @"update MainCategory set IsDeleted = 1 where Id=" + Id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("Add");
        }


        public ActionResult Edit(int id)
        {
            var data = ent.MainCategories.Find(id);
            var model = Mapper.Map<MainCategoryDTO>(data);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(MainCategoryDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var domainModel = Mapper.Map<MainCategory>(model);
                
                domainModel.AmbulanceType_id = model.AmbulanceType_id;
                if (model.AmbulanceType_id == 1)
                {
                    domainModel.Type = "Regular";
                }
                else if (model.AmbulanceType_id == 2)
                {
                    domainModel.Type = "Road Accident";
                }
                else if (model.AmbulanceType_id == 3)
                {
                    domainModel.Type = "Funeral/MortuaryService";

                }
                ent.Entry(domainModel).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                return RedirectToAction("Add");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                TempData["msg"] = "Server Error";
                return RedirectToAction("Edit", model.Id);
            }
        }
    }
}