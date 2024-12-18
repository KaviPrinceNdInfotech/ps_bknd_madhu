﻿using HospitalPortal.Models.DomainModels;
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

        private int GetVendorId()
        {
            int loginId = Convert.ToInt32(User.Identity.Name);
            int FranchiseId = ent.Database.SqlQuery<int>("select Id from Vendor where AdminLogin_Id=" + loginId).FirstOrDefault();
            return FranchiseId;
        }

        [HttpGet]
        public ActionResult Gallery(int? id)
        {
            var model = new GallertDTO();
            if(id==null || id==0)
            {
                string q = @"select * from Gallery where IsDeleted=0  order by Id desc";
                var data = ent.Database.SqlQuery<GalleryList>(q).ToList();
                model.GalleryList = data;
                return View(model);
            }
            else
            {
                string q = @"select * from Gallery where IsDeleted=0 and Franchise_Id=" + id + " order by Id desc";
                var data = ent.Database.SqlQuery<GalleryList>(q).ToList();
                model.GalleryList = data;
                return View(model);
            } 
            
        }

        [HttpPost]
        public ActionResult Gallery(GallertDTO model)
        {
            //var Img = FileOperation.UploadImage(model.Image, "Gallery");
            var Img = FileOperation.UploadImage(model.Image, "Images");
            if (Img == "not allowed")
            {
                TempData["msg"] = "Only png,jpg,jpeg files are allowed.";
                return View(model);
            }
            model.Images = Img;
            var domain = new Gallery();
            domain.ImageName = model.ImageName;
            domain.Franchise_Id = GetVendorId();
            domain.Images = model.Images;
            domain.IsDeleted = false;
            ent.Galleries.Add(domain);
            ent.SaveChanges();
            TempData["msg"] = "Successfully Inserted";
            return RedirectToAction("Gallery", new {id= GetVendorId() });
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
            
            if (GetVendorId()!=null)
            {
                return RedirectToAction("Gallery", new { id = GetVendorId() });
            }
            else
            {
                return RedirectToAction("Gallery");
            }
            
        }
    }
}