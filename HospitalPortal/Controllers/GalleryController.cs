using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    [Authorize(Roles = "admin,Franchise")]
    public class GalleryController : Controller
    {
        DbEntities ent = new DbEntities();
        [HttpGet]
        public ActionResult Gallery()
        {
            var model = new GallertDTO();
            string q = @"select * from Gallery where IsDeleted=0 order by Id desc";
            var data = ent.Database.SqlQuery<GalleryList>(q).ToList();
            if(data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            model.GalleryList = data;
            return View(model);
        }

        [HttpPost]
        public ActionResult Gallery(GallertDTO model)
        {
            var Img = FileOperation.UploadImage(model.Image, "Gallery");
            if (Img == "not allowed")
            {
                TempData["msg"] = "Only png,jpg,jpeg files are allowed.";
                return View(model);
            }
            model.Images = Img;
            var domain = new Gallery();
            domain.ImageName = model.ImageName;
            domain.Images = model.Images;
            domain.IsDeleted = false;
            ent.Galleries.Add(domain);
            ent.SaveChanges();
            TempData["msg"] = "Successfully Inserted";
            return RedirectToAction("Gallery");
        }

        public ActionResult Delete(int id)
        {
            var data = ent.Galleries.Find(id);
            try
            {
                data.IsDeleted = true;
                ent.SaveChanges();
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                TempData["msg"] = "Server Error!";
            }
            return RedirectToAction("Gallery");
        }
    }
}