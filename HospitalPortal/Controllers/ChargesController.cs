using AutoMapper;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static HospitalPortal.Utilities.EmailOperations;

namespace HospitalPortal.Controllers
{
    public class ChargesController : Controller
    {
        DbEntities ent = new DbEntities();

        [HttpGet]
        public ActionResult Common()
        {
            var model = new ChargeDTO();
            string q = @"select * from Charges";
            var data = ent.Database.SqlQuery<ChargeList>(q).ToList();
            model.Charges = data;
            return View(model);
        }

        [HttpPost]
        public ActionResult Common(ChargeDTO model)
        {
            if(ent.Charges.Any(a=>a.Role == model.Role))
            {
                TempData["msg"] = "Already Exists";
                return RedirectToAction("Common");
            }
            var domain = Mapper.Map<Charge>(model);
            ent.Charges.Add(domain);
            ent.SaveChanges();
            TempData["msg"] = "ok";
            return RedirectToAction("Common");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var data = ent.Charges.Find(id);
            var model = Mapper.Map<ChargeDTO>(data);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ChargeDTO model)
        {
            var domain = Mapper.Map<Charge>(model);
            ent.Entry(domain).State = System.Data.Entity.EntityState.Modified;
            ent.SaveChanges();
            return RedirectToAction("Common", new { id = model.Id });
        }

        [HttpGet]
        public ActionResult VehicleList(int? CategoryId)
        {
            var model = new VehicleChargesDTO();
            model.CategoryList = new SelectList(ent.MainCategories.Where(a => a.IsDeleted == false).ToList(), "Id", "CategoryName");
            model.TypeList = new SelectList(ent.VehicleTypes.Where(a => a.IsDeleted == false).ToList(), "Id", "VehicleTypeName");
            string q = @"select * from VehicleCharges vt join MainCategory mc on mc.Id = vt.Category_Id join VehicleType at on at.Id = vt.TypeId  where mc.IsDeleted=0 and at.IsDeleted=0";
            var data = ent.Database.SqlQuery<VehicleListClass>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Records";
                return View(model);
            }
            if (CategoryId != 0 && CategoryId != null)
            {
                string q1 = @"select * from VehicleCharges vt join MainCategory mc on mc.Id = vt.Category_Id join VehicleType at on at.Id = vt.TypeId  where mc.IsDeleted=0 and at.IsDeleted=0 and vt.Category_Id=" + CategoryId;
                var data1 = ent.Database.SqlQuery<VehicleListClass>(q1).ToList();
                if (data1.Count() == 0)
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


        [HttpPost]
        public ActionResult VehicleList(VehicleChargesDTO model)
        {
            if (!ModelState.IsValid)
            {
                model.CategoryList = new SelectList(ent.MainCategories.Where(a => a.IsDeleted == false).ToList(), "Id", "CategoryName");
                model.TypeList = new SelectList(ent.VehicleTypes.Where(a => a.IsDeleted == false).ToList(), "Id", "VehicleTypeName");
                TempData["msg"] = "Some Error";
                return View(model);
            }
            model.CategoryList = new SelectList(ent.MainCategories.Where(a => a.IsDeleted == false).ToList(), "Id", "CategoryName");
            model.TypeList = new SelectList(ent.VehicleTypes.Where(a => a.IsDeleted == false).ToList(), "Id", "VehicleTypeName");
            var domain = Mapper.Map<VehicleCharge>(model);
            ent.VehicleCharges.Add(domain);
            ent.SaveChanges();
            return RedirectToAction("VehicleList");
        }


        [HttpGet]
        public ActionResult EditCharges(int id)
        {
            var data = ent.VehicleCharges.Find(id);
            var data1 = ent.VehicleTypes.Where(a => a.Id == data.TypeId).ToList();
            var data2 = ent.MainCategories.Where(a => a.Id == data.Category_Id).ToList();
            var model = Mapper.Map<VehicleChargesDTO>(data);
            model.VehicleTypeName = data1.FirstOrDefault().VehicleTypeName;
            model.CategoryName = data2.FirstOrDefault().CategoryName;
            return View(model);
        }


        [HttpPost]
        public ActionResult EditCharges(VehicleChargesDTO model)
        {
            var domain = Mapper.Map<VehicleCharge>(model);
            ent.Entry(domain).State = System.Data.Entity.EntityState.Modified;
            ent.SaveChanges();
            return RedirectToAction("VehicleList", new { id = model.Id });
        }

        [HttpPost]
        public ActionResult MedicineDeliveryCharge(ChargeDTO model)
        {


            dynamic existingRecord = ent.MedicineDeliveryCharges.OrderByDescending(a => a.Id).Select(a => a.Amount).FirstOrDefault();
            //var existingRecord = ent.MedicineDeliveryCharges.FirstOrDefault(mdc => mdc.Amount == model.Amount);

            if (existingRecord == null)
            {
                var domain = new MedicineDeliveryCharge();
                domain.Amount = (int)model.Amount;
                ent.MedicineDeliveryCharges.Add(domain);
                ent.SaveChanges();
                TempData["msg"] = "ok";
            }
            else
            {
                TempData["msg"] = "Can't add more than one";
            }

            return RedirectToAction("MedicineDeliveryCharge");

        }
        public ActionResult MedicineDeliveryCharge()
        {
            var model = new ChargeDTO();
            string q = @"select * from MedicineDeliveryCharge";
            var data = ent.Database.SqlQuery<MedcineDelChargelist>(q).ToList();
            model.MedcineDelChargelist = data;
            return View(model);
        }
        public ActionResult DeleteMedDelCharge(int Id)
        {
            string q = @"Delete From MedicineDeliveryCharge where Id=" + Id;
            ent.Database.ExecuteSqlCommand(q);
            return RedirectToAction("MedicineDeliveryCharge");
        }

        public ActionResult DoctorSlot()
        {
            return View();  
        }
        [HttpPost ]
        public ActionResult DoctorSlot(DoctorslotDTO model)
        {
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    var domainModel = new DoctorTimeSlot(); 
                    domainModel.StartTime = model.StartTime;
                    domainModel.EndTime = model.EndTime;
                    
                    ent.DoctorTimeSlots.Add(domainModel);
                    ent.SaveChanges();

                    TempData["SuccessMessage"] = "Doctor slot saved successfully!";
                    tran.Commit();
                }
                //catch (Exception ex)
                catch (DbEntityValidationException ex)
                {
                    
                    TempData["msg"] = "Server Error";
                    tran.Rollback();

                }
            }
            return RedirectToAction("DoctorSlot");
        }

        public ActionResult ViewDoctorSlot()
        {
            var model = new DoctorslotDTO();
            string q = @"SELECT Id,CONCAT(CONVERT(NVARCHAR, StartTime, 8), ' To ', CONVERT(NVARCHAR, EndTime, 8)) AS SlotTime FROM DoctorTimeSlot";
            var data = ent.Database.SqlQuery<Slotlist>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record";
                return View(model);
            }
            model.Slotlist = data;
            return View(model);
        }
        public ActionResult DeleteDoctorSlot(int id)
        {
            try
            {
                var result = ent.DoctorTimeSlots.FirstOrDefault(x => x.Id == id);
                if (result != null)
                {
                    ent.DoctorTimeSlots.Remove(result);
                    ent.SaveChanges();
                }
                return RedirectToAction("ViewDoctorSlot");
            }
            catch
            {
                throw new Exception("Server error");
            }
        }
    }
}