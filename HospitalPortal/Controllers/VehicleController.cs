using AutoMapper;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.RequestModel;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Repositories;
using HospitalPortal.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
   
    public class VehicleController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(VehicleController));

        [AllowAnonymous]
        public ActionResult Add(int vendorId = 0)
        {
            var model = new VehicleDTO();
            //  model.States = new SelectList(repos.GetAllStates(), "Id", "StateName");
            //model.Drivers = new SelectList(repos.GetAllDrivers(), "Id", "DriverName");
            model.CategoryList = new SelectList(repos.GetCategory(), "Id", "CategoryName");
            model.VehicleTypes = new List<SelectListItem>(ent.VehicleTypes.Where(a => a.IsDeleted == false).Select(a => new SelectListItem { Text = a.VehicleTypeName, Value = a.Id.ToString() }));
            model.Vendor_Id = vendorId;
            model.VendorList = new SelectList(ent.Vendors.Where(a => a.IsDeleted == false && a.IsApproved == true).ToList(), "Id", "CompanyName");
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult tt()
        {
            return Content(Guid.NewGuid().ToString());
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Add(VehicleDTO model)
        {
            model.CategoryList = new SelectList(repos.GetCategory(), "Id", "CategoryName");
            model.VehicleTypes = new List<SelectListItem>(ent.VehicleTypes.Select(a => new SelectListItem { Text = a.VehicleTypeName, Value = a.Id.ToString() }));
            model.VendorList = new SelectList(ent.Vendors.Where(a => a.IsDeleted == false && a.IsApproved == true).ToList(), "Id", "CompanyName");
            try
            { 
                //Chek if Any Vehicle Exists Before

                 if(ent.Vehicles.Any(a=>a.VehicleNumber == model.VehicleNumber))
                 {
                    TempData["msg"] = "This Vehicle Number Already Registerd With Us.";
                    return View(model);
                 }
                 
                //Cancel Cheque Image
                if (model.CancelChequeFile != null)
                {
                    var ChequeImg = FileOperation.UploadImage(model.CancelChequeFile, "Images");
                    if (ChequeImg == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg docs with Cancel Cheque are allowed.";
                        return View(model);
                    }
                    model.CancelCheque = ChequeImg;
                }
                var domainModel = Mapper.Map<Vehicle>(model);
                domainModel.RegistrationDate = DateTime.Now;
                if (model.Vendor_Id == 0)
                {
                    domainModel.Vendor_Id = null;
                }
                else
                {
                    domainModel.Vendor_Id = model.Vendor_Id;
                }
                ent.Vehicles.Add(domainModel);
                ent.SaveChanges();
                TempData["msg"] = "ok";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                TempData["msg"] = "Server Error";
            }
            return RedirectToAction("Add", new { vendorId = model.Vendor_Id });
        }

        public ActionResult Edit(int id)
        {
            var data = ent.Vehicles.Find(id);
            var model = Mapper.Map<VehicleEditDto>(data);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(VehicleEditDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                // fitness certificate image upload
                if (model.FitnessCertificateImageFile != null)
                {
                    var FitnessImg = FileOperation.UploadImage(model.FitnessCertificateImageFile, "Images");
                    if (FitnessImg == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg files are allowed.";
                        return View(model);
                    }
                    model.FitnessCerficateImage = FitnessImg;
                }
                // insurrance doc upload
                if (model.FitnessCertificateImageFile != null)
                {
                    var insurranceImg = FileOperation.UploadImage(model.InsurranceImageFile, "Images");
                    if (insurranceImg == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg docs with insurrance are allowed.";
                        return View(model);
                    }
                    model.InsuranceImage = insurranceImg;
                }

                // pollution doc upload
                if (model.PollutionImageFile != null)
                {
                    var pollutionImg = FileOperation.UploadImage(model.PollutionImageFile, "Images");
                    if (pollutionImg == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg docs with pollution are allowed.";
                        return View(model);
                    }
                    model.PollutionImage = pollutionImg;
                }
                //RC File
                if (model.RC_ImageFile != null)
                {
                    var rcImg = FileOperation.UploadImage(model.RC_ImageFile, "Images");
                    if (rcImg == "not allowed")
                    {
                        TempData["msg"] = "Only png,jpg,jpeg docs with RC are allowed.";
                        return View(model);
                    }
                    model.RC_Image = rcImg;
                }
                var domainModel = Mapper.Map<Vehicle>(model);
                //domainModel.InsurranceDate = model.InsurranceDate.Value.Date;
                domainModel.RegistrationDate = model.RegistrationDate ?? domainModel.RegistrationDate;
                ent.Entry(domainModel).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                TempData["msg"] = "Successfully Updated";
                return RedirectToAction("Edit", model.Id);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                TempData["msg"] = "Server Error";
                return RedirectToAction("Edit", model.Id);
            }
        }

        public ActionResult All(int? vendorId, string term = null, int? page = 0)
        {
            var model = new VehicleDTO();
            string q = @"select v.*,IsNull(ve.UniqueId,'N/A') as UniqueId,vt.VehicleTypeName, IsNull(ve.VendorName,'NA') AS VendorName , IsNull(ve.CompanyName,'NA') as CompanyName, mc.* from Vehicle v 
join VehicleType vt on v.VehicleType_Id = vt.Id
left join Vendor ve on ve.Id = v.Vendor_Id
join MainCategory mc on mc.Id = vt.Category_Id
where v.IsDeleted=0 order by v.Id desc";
            var data = ent.Database.SqlQuery<VehicleDTO>(q).ToList();
            if (vendorId != null)
            {
                data = data.Where(a => a.Vendor_Id == vendorId).ToList();
            }
            if (term != null)
                data = data.Where(a => a.VehicleNumber.Contains(term)).ToList();

            if (data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(data);
            }
            int total = data.Count;
            page = page ?? 1;
            int pageSize = 10;
            decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
            model.NumberOfPages = (int)noOfPages;
            model.Page = page;
            data = data.OrderByDescending(a => a.Id).Skip(pageSize * ((int)page - 1)).Take(pageSize).ToList();
            data.FirstOrDefault().NumberOfPages = model.NumberOfPages;
            data.FirstOrDefault().Page = model.Page;

            //int total = data.Count;
            //page = page ?? 1;
            //int pageSize = 10;
            //decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
            //model.NumberOfPages = (int)noOfPages;
            //model.Page = page;
            //data = data.OrderByDescending(a => a.Id).Skip(pageSize * ((int)page - 1)).Take(pageSize).ToList();
            //data.FirstOrDefault().NumberOfPages = model.NumberOfPages;
            //data.FirstOrDefault().Page = model.Page;
            return View(data);
        }

        public ActionResult UpdateStatus(int id)
        {
            string q = @"update Vehicle set IsApproved = case when IsApproved=1 then 0 else 1 end where id=" + id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("All");
        }

        public ActionResult Delete(int id)
        {
            var data = ent.Vehicles.Find(id);
            try
            {
                data.IsDeleted = true;
                ent.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return RedirectToAction("All");
        }

        public ActionResult VehicleAllotment(int? Id, VehicleAllotmentDTO model)
        {
            //Using AutoSearch get Vehicle Number from passing model 
            if (model.VehicleNumber != null)
            {
                //model.VehicleList = new List<SelectListItem>(ent.VehicleTypes.Select(a => new SelectListItem { Text = a.VehicleTypeName, Value = a.Id.ToString() }));
                var list = ent.Vehicles.Where(a => a.VehicleNumber == model.VehicleNumber).ToList();
                int Ids = list.FirstOrDefault().Id;
                if (Ids != 0)
                {
                    string Q = @"select vt.Id as VehicleTypeId, v.Id as VehicleId, d.Id, d.DriverName, v.VehicleNumber from VehicleType vt join Vehicle v on vt.Id = v.VehicleType_Id join Driver d on d.Id = v.Driver_Id  where v.Id=" + Ids;
                    var data = ent.Database.SqlQuery<VehicleLists>(Q).ToList();
                    model.VehicleLists = data;
                    return View(model);
                }
            }
            // model.VehicleList = new List<SelectListItem>(ent.VehicleTypes.Select(a => new SelectListItem { Text = a.VehicleTypeName, Value = a.Id.ToString() }));
            if (Id != null)
            {
                string Q = @"select D.Id as DriverId, IsNull(d.DriverName,'NA') as DriverName,IsNull(v.VehicleNumber,'NA') AS VehicleNumber,vt.Id as VehicleTypeId,coalesce(cast(V.Id as varchar(255)), 'NA') as VehicleId from Driver d  join VehicleType vt on vt.Id = d.VehicleType_Id left join Vehicle v on d.Id  = v.Driver_Id where vt.Id=" + Id;
                var data = ent.Database.SqlQuery<VehicleLists>(Q).ToList();
                model.VehicleLists = data;
                return View(model);
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult UpdateDriver(int Id)
        {
            TempData["Id"] = Id;
            return View();
        }

        [HttpPost]
        public ActionResult UpdateDriver(VehicleAllotmentDTO model)
        {
            var list = ent.Drivers.Where(a => a.DriverName == model.DriverName).ToList();
            var DriverId = list.FirstOrDefault().Id;
            var Name = list.FirstOrDefault().DriverName;

            var GetVehiletypeId=ent.Vehicles.Where(V=>V.Id== model.Id).Select(v=>v.VehicleType_Id).FirstOrDefault();

            if (ent.Vehicles.Any(a => a.Driver_Id == DriverId))
            {
                string VehicleNumber1 = ent.Database.SqlQuery<string>("select VehicleNumber from Vehicle where Driver_Id=" + DriverId).FirstOrDefault();
                TempData["msg"] = "The Selected Driver is Already Running on " + VehicleNumber1;
                return RedirectToAction("UpdateDriver", new { model.Id });
            }
            string q = @"update Vehicle set Driver_Id = " + DriverId + "  where Id=" + model.Id;
            ent.Database.ExecuteSqlCommand(q);

			string dq = @"update Driver set VehicleType_Id = " + GetVehiletypeId + " ,Vehicle_Id="+ model.Id + " where Id=" + DriverId;
			ent.Database.ExecuteSqlCommand(dq);
			string VehicleNumber = ent.Database.SqlQuery<string>("select VehicleNumber from Vehicle where Driver_Id=" + DriverId).FirstOrDefault();
            TempData["msg"] = "The Vehicle Number" + VehicleNumber + " has been Replaced to " + Name;
            return RedirectToAction("UpdateDriver", new { model.Id });
        }

        [HttpPost]
        public JsonResult GetDriverName(string term)
        {
            var DriverList = (from N in ent.Drivers
                              where N.DriverName.StartsWith(term)
                              && N.IsDeleted == false
                              select new { N.DriverName, N.Id });
            //TempData["DriverId"] = DriverList.FirstOrDefault().Id;
            //TempData["DriverName"] = DriverList.FirstOrDefault().DriverName;
            return Json(DriverList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetVehicleNumber(string term)
        {
            var VehicleList = (from N in ent.Vehicles
                               where N.VehicleNumber.StartsWith(term)
                               && (N.Driver_Id == null)
                               && N.IsDeleted == false
                               select new { N.VehicleNumber, N.Id });
            //TempData["VehicleId"] = VehicleList.FirstOrDefault().Id;
            //TempData["VehicleNumber"] = VehicleList.FirstOrDefault().VehicleNumber;
            return Json(VehicleList, JsonRequestBehavior.AllowGet);
        }
		public JsonResult GetVehicleNumberByVehicleType(int vehicleTypeId, string term)

		{
			var VehicleList = (from N in ent.Vehicles
							   where N.VehicleNumber.StartsWith(term)
							   && N.IsDeleted == false
							   && N.VehicleType_Id == vehicleTypeId
							   select new { N.VehicleNumber, N.Id });

			return Json(VehicleList, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetVehicleNumberList(string term)
        {
            var VehicleList = (from N in ent.Vehicles
                               where N.VehicleNumber.StartsWith(term)
                               && N.IsDeleted == false
                               select new { N.VehicleNumber, N.Id });
            //TempData["VehicleId"] = VehicleList.FirstOrDefault().Id;
            //TempData["VehicleNumber"] = VehicleList.FirstOrDefault().VehicleNumber;
            return Json(VehicleList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateVehicle(int? Id, VehicleAllotmentDTO model)
        {
            //Using AutoSearch get Vehicle Number from passing model 
            model.VehicleList = new SelectList(repos.GetVehicleTypes(), "Id", "VehicleTypeName");
            //string Q = @"select * from driver where VehicleType_Id is null and IsDeleted = 0";
            string Q = @"select * from driver where VehicleType_Id=0 or VehicleType_Id is null and IsDeleted = 0";
            var data = ent.Database.SqlQuery<VehicleLists>(Q).ToList();
            model.VehicleLists = data;
            return View(model);
        }
        [HttpGet]
        public ActionResult UpdateVehicles(int Id)
        {
            var model = new VehicleAllotmentDTO();
            model.VehicleList = new SelectList(repos.GetVehicleTypes(), "Id", "VehicleTypeName");
            TempData["Id"] = Id;
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateVehicles(VehicleAllotmentDTO model)
        {
            model.VehicleList = new SelectList(repos.GetVehicleTypes(), "Id", "VehicleTypeName");
            var list = ent.Vehicles.Where(a => a.VehicleNumber == model.VehicleNumber).ToList();
            var VehileId = list.FirstOrDefault().Id;
            var Number = list.FirstOrDefault().VehicleNumber;
            int DriverId = Convert.ToInt32(TempData["Id"]);
			var getexistVehicle = ent.Drivers.Where(d => d.Vehicle_Id == VehileId && d.IsDeleted == false).FirstOrDefault();
			if (getexistVehicle != null)
			{
				string ExistdriverName = ent.Database.SqlQuery<string>("select DriverName from Driver where Vehicle_Id=" + VehileId).FirstOrDefault();
				TempData["msg"] = "The Selected Vehicle is Already Running on " + ExistdriverName;
				return RedirectToAction("UpdateVehicles", new { model.Id });
			}
			string q = @"update Vehicle set Driver_Id = " + DriverId + " where Id=" + VehileId;
            ent.Database.ExecuteSqlCommand(q);
            //
            string qry = @"update Driver set VehicleType_Id = " + model.Id + ",Vehicle_Id="+ VehileId + " where Id=" + DriverId;
            ent.Database.ExecuteSqlCommand(qry);
            //
            string Name = ent.Database.SqlQuery<string>("select DriverName from Driver where Id=" + DriverId).FirstOrDefault();
            string VehicleNumber = ent.Database.SqlQuery<string>("select VehicleNumber from Vehicle where Id=" + VehileId).FirstOrDefault();
            TempData["msg"] = "The Vehicle Number" + VehicleNumber + " has been Replaced to " + Name;
            return RedirectToAction("UpdateVehicles", new { model.Id });
        }
    }
}