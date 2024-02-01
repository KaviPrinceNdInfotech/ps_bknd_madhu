using AutoMapper;
using ExcelDataReader;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Repositories;
using iTextSharp.text.pdf.qrcode;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    public class CommonController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(CommonController));

        public ActionResult GetCitiesByState(int? stateId)
        {
            var cities = repos.GetCitiesByState(stateId);
            var c = Mapper.Map<IEnumerable<CityMasterDTO>>(cities);
            return Json(c, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSpecialistByDept(int depId)
        {
            var data = ent.Specialists.Where(a => a.Department_Id == depId && a.IsDeleted == false).ToList();
            var c = Mapper.Map<IEnumerable<SpecialistDTO>>(data);
            return Json(c, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetLocationByCity(int cityId)
        {
            var data = ent.Locations.Where(a => a.City_Id == cityId && a.IsDeleted == false).ToList();
            var c = Mapper.Map<IEnumerable<LocationDTO>>(data);
            return Json(c, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTypeUsingCategory(int? CatId)
        {
            var data = Mapper.Map<IEnumerable<VehicleTypeDTO>>(ent.VehicleTypes.Where(A=>A.IsDeleted == false && A.Category_Id == CatId).ToList());
            return Json(data, JsonRequestBehavior.AllowGet);
        }

       

        public ActionResult SignupLinks()
        {
            return View();
        }

        public ActionResult HomeSignUp()
        {
            return View();
        }
        public ActionResult ComingSoon()
        {
            return View();
        }
        [HttpGet]
        public ActionResult TestList()
        {
            var data = Mapper.Map<IEnumerable<TestArray>>(ent.LabTests.ToList());
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CategoryList()
        {
            var data = Mapper.Map<IEnumerable<MainCategoryList>>(ent.MainCategories.Where(A => A.IsDeleted == false).ToList());
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult Commision()
        {
            var model = new CommissionDTO();
            var Q = @"select * from CommissionMaster where IsDeleted=0 order by Name asc";
            var data = ent.Database.SqlQuery<CommissionList>(Q).ToList();
            model.CommissionList = data;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddCommision(string Name="", string Commission="")
        {
            if (!ModelState.IsValid)
            {
                TempData["msg"] = "Some Error";
                return RedirectToAction("Commision");
            }
            if (ent.CommissionMasters.Any(a => a.Name == Name && a.IsDeleted == false))
            {
                TempData["msg"] = "Already Exist with this Name";
                return RedirectToAction("Commision");
            }
            NumberFormatInfo provider = new NumberFormatInfo();
            var data = new CommissionMaster();
            data.Name = Name;
            data.Commission =Convert.ToDouble(Commission);
            data.IsDeleted = false;
            ent.CommissionMasters.Add(data);
            ent.SaveChanges();
            TempData["msg"] = "SuccessFully Saved";
            return RedirectToAction("Commision");
        }

        public ActionResult Update(int id)
        {
            var data = ent.CommissionMasters.Find(id);
            data.IsDeleted = true;
            ent.SaveChanges();
            return RedirectToAction("Commision");
        }


        [HttpGet]
        public ActionResult Payout()
        {
            var model = new PayoutMasterDTO();
            var Q = @"select * from PayoutMaster where IsDeleted=0";
            var data = ent.Database.SqlQuery<PayoutList>(Q).ToList();
            model.payoutList = data;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddPayout(PayoutMasterDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["msg"] = "Some Error";
                return RedirectToAction("Payout");
            }
            var data = new PayoutMaster();
            data.Name = model.Name;
            data.Amount = model.Amount;
            data.IsDeleted = false;
            ent.PayoutMasters.Add(data);
            ent.SaveChanges();
            TempData["msg"] = "SuccessFully Saved";
            return RedirectToAction("Payout");
        }

        public ActionResult UpdatePayout(int id)
        {
            var data = ent.PayoutMasters.Find(id);
            data.IsDeleted = true;
            ent.SaveChanges();
            return RedirectToAction("Payout");
        }


        [HttpGet]
        public ActionResult Payment()
        {
            var model = new PaymentMasterDTO();
            var Q = @"select * from PaymentMaster where IsDeleted=0 order by Name asc";
            var data = ent.Database.SqlQuery<paymentList>(Q).ToList();
            model.paymentList = data;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddPayment(PaymentMasterDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["msg"] = "Some Error";
                return RedirectToAction("Payment");
            }
            var data = new PaymentMaster();
            data.Name = model.Name;
            data.Department = model.Department;
            data.Amount = model.Amount;
            data.IsDeleted = false;
            ent.PaymentMasters.Add(data);
            ent.SaveChanges();
            TempData["msg"] = "SuccessFully Saved";
            return RedirectToAction("Payment");
        }

        [HttpGet]
        public ActionResult EditFrachiseCommission(int id)
        {
            var data = ent.PaymentMasters.Find(id);
            var model = Mapper.Map<PaymentMasterDTO>(data);
            return View(model);
        }


        [HttpPost]
        public ActionResult EditFrachiseCommission(PaymentMasterDTO data)
        {
            ModelState.Remove("Id");
            ModelState.Remove("IsDeleted");
            ModelState.Remove("Name");
            ModelState.Remove("Department");
            var model = Mapper.Map<PaymentMaster>(data);
            ent.Entry(model).State = System.Data.Entity.EntityState.Modified;
            ent.SaveChanges();
            TempData["msg"] = "Successfully Updated";
            return RedirectToAction("Payment", new { id = model.Id });
        }

        public ActionResult RWAGST()
        {
            var model = new rwaPaymentDTO();
            var Q = @"select * from RWAGstMaster where IsDeleted=0 order by Name asc";
            var data = ent.Database.SqlQuery<rwaGSTList>(Q).ToList();
            model.rwaGSTList = data;
            return View(model);
        }
        [HttpPost]
        public ActionResult AddRWAGST(rwaPaymentDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["msg"] = "Some Error";
                return RedirectToAction("Payment");
            }
            var data = new RWAGstMaster();
            data.Name = model.Name;
            data.Department = model.Department;
            data.Amount = model.Amount;
            data.IsDeleted = false;
            ent.RWAGstMasters.Add(data);
            ent.SaveChanges();
            TempData["msg"] = "SuccessFully Saved";
            return RedirectToAction("RWAGST");
        }
        public ActionResult DeleteRWAGST(int id)
        {
            var data = ent.RWAGstMasters.Find(id);
            data.IsDeleted = true;
            ent.SaveChanges();
            return RedirectToAction("RWAGST");
        }
        [HttpGet]
        public ActionResult EditRwaGST(int id)
        {
            var data = ent.RWAGstMasters.Find(id);
            var model = Mapper.Map<rwaPaymentDTO>(data);
            model.Department = data.Department;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditRwaGST(rwaPaymentDTO data)
        {
            ModelState.Remove("Id");
            ModelState.Remove("IsDeleted");
            ModelState.Remove("Name");
            ModelState.Remove("Departement");
            var model = Mapper.Map<RWAGstMaster>(data);
            model.Department = data.Department;
            ent.Entry(model).State = System.Data.Entity.EntityState.Modified;
            ent.SaveChanges();
            TempData["msg"] = "Successfully Updated";
            return RedirectToAction("RWAGST");
        }

        public ActionResult FranchiseGST()
        {
            var model = new rwaPaymentDTO();
            var Q = @"select * from FranchiseGstMaster where IsDeleted=0 order by Name asc";
            var data = ent.Database.SqlQuery<FraGSTList>(Q).ToList();
            model.FraGSTList = data;
            return View(model);
        }
        [HttpPost]
        public ActionResult AddFranchiseGST(rwaPaymentDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["msg"] = "Some Error";
                return RedirectToAction("Payment");
            }
            var data = new FranchiseGstMaster();
            data.Name = model.Name;
            data.Department = model.Department;
            data.Amount = model.Amount;
            data.IsDeleted = false;
            ent.FranchiseGstMasters.Add(data);
            ent.SaveChanges();
            TempData["msg"] = "SuccessFully Saved";
            return RedirectToAction("FranchiseGST");
        }
        public ActionResult DeleteFranchiseGST(int id)
        {
            var data = ent.FranchiseGstMasters.Find(id);
            data.IsDeleted = true;
            ent.SaveChanges();
            return RedirectToAction("FranchiseGST");
        }
        [HttpGet]
        public ActionResult EditFranchiseGST(int id)
        {
            var data = ent.FranchiseGstMasters.Find(id);
            var model = Mapper.Map<rwaPaymentDTO>(data);
            model.Department = data.Department;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditFranchiseGST(rwaPaymentDTO data)
        {
            ModelState.Remove("Id");
            ModelState.Remove("IsDeleted");
            ModelState.Remove("Name");
            ModelState.Remove("Departement");
            var model = Mapper.Map<FranchiseGstMaster>(data);
            model.Department = data.Department;
            ent.Entry(model).State = System.Data.Entity.EntityState.Modified;
            ent.SaveChanges();
            TempData["msg"] = "Successfully Updated";
            return RedirectToAction("FranchiseGST");
        }

        [HttpGet]
        public ActionResult RWAPayment()
        {
            var model = new rwaPaymentDTO();
            var Q = @"select * from RWAPayment where IsDeleted=0 order by Name asc";
            var data = ent.Database.SqlQuery<rwapaymentList>(Q).ToList();
            model.rwapaymentList = data;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddRWAPayment(rwaPaymentDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["msg"] = "Some Error";
                return RedirectToAction("Payment");
            }
            var data = new RWAPayment();
            data.Name = model.Name;
            data.Departement = model.Department;
            data.Amount = model.Amount;
            data.IsDeleted = false;
            ent.RWAPayments.Add(data);
            ent.SaveChanges();
            TempData["msg"] = "SuccessFully Saved";
            return RedirectToAction("RWAPayment");
        }

        [HttpGet]
        public ActionResult EditRwaPayment(int id)
        {
            var data = ent.RWAPayments.Find(id);
            var model = Mapper.Map<rwaPaymentDTO>(data);
            model.Department = data.Departement;
            return View(model);
        }


        [HttpPost]
        public ActionResult EditRwaPayment(rwaPaymentDTO data)
        {
            ModelState.Remove("Id");
            ModelState.Remove("IsDeleted");
            ModelState.Remove("Name");
            ModelState.Remove("Departement");
            var model = Mapper.Map<RWAPayment>(data);
            model.Departement = data.Department;
            ent.Entry(model).State = System.Data.Entity.EntityState.Modified;
            ent.SaveChanges();
            TempData["msg"] = "Successfully Updated";
            return RedirectToAction("RWAPayment");
        }

        public ActionResult UpdatePayment(int id)
        {
            var data = ent.PaymentMasters.Find(id);
            data.IsDeleted = true;
            ent.SaveChanges();
            return RedirectToAction("Payment");
        }


        public ActionResult DeletePayment(int id)
        {
            var data = ent.RWAPayments.Find(id);
            data.IsDeleted = true;
            ent.SaveChanges();
            return RedirectToAction("RWAPayment");
        }

        [HttpGet]
        public ActionResult GST()
        {
            var model = new GSTMasterDTO();
            var Q = @"select * from GSTMaster where IsDeleted=0 order by Name asc";
            var data = ent.Database.SqlQuery<gstList>(Q).ToList();
            model.GSTLIST = data;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddGST(GSTMasterDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["msg"] = "Some Error";
                return RedirectToAction("GST");
            }
            if (ent.GSTMasters.Any(a => a.Name == model.Name && a.IsDeleted == false))
            {
                TempData["msg"] = "Already Exist with this Name";
                return RedirectToAction("GST");
            }
            var data = new GSTMaster();
            data.Name = model.Name;
            data.Amount = model.Amount;
            data.IsDeleted = false;
            ent.GSTMasters.Add(data);
            ent.SaveChanges();
            TempData["msg"] = "SuccessFully Saved";
            return RedirectToAction("GST");
        }
        public ActionResult DeleteGST(int Id)
        {
            var data = ent.GSTMasters.Find(Id);
            data.IsDeleted = true;
            ent.SaveChanges();
            return RedirectToAction("GST");
        }

        [HttpGet]
        public ActionResult EditGST(int id)
        {
            var data = ent.GSTMasters.Find(id);
            var model = Mapper.Map<GSTMasterDTO>(data);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditGST(GSTMasterDTO data)
        {
            ModelState.Remove("Id");
            ModelState.Remove("IsDeleted");
            ModelState.Remove("Name");
            var model = Mapper.Map<GSTMaster>(data);
            ent.Entry(model).State = System.Data.Entity.EntityState.Modified;
            ent.SaveChanges();
            TempData["msg"] = "ok";
            return RedirectToAction("EditGST", new { id = model.Id });
        }

        [HttpGet]
        public ActionResult RWATDS()
        {
            var model = new RWATDSDTO();
            var Q = @"select * from RWATDSMaster where IsDeleted=0 order by Name asc";
            var data = ent.Database.SqlQuery<RWAtdsList>(Q).ToList();
            model.RWATDSLIST = data;
            return View(model);
        }
       
        [HttpPost]
        public ActionResult AddRWATDS(RWATDSDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["msg"] = "Some Error";
                return RedirectToAction("TDS");
            }
            if (ent.RWATDSMasters.Any(a => a.Name == model.Name && a.IsDeleted == false))
            {
                TempData["msg"] = "Already Exist with this Name";
                return RedirectToAction("RWATDS");
            }
            var data = new RWATDSMaster();
            data.Name = model.Name;
            data.Amount = model.Amount;
            data.IsDeleted = false;
            data.Department = model.Department;
            ent.RWATDSMasters.Add(data);
            ent.SaveChanges();
            TempData["msg"] = "SuccessFully Saved";
            return RedirectToAction("RWATDS");
        }

        public ActionResult DeleteRWATDS(int Id)
        {
            var data = ent.RWATDSMasters.Find(Id);
            data.IsDeleted = true;
            ent.SaveChanges();
            return RedirectToAction("RWATDS");
        }

        [HttpGet]
        public ActionResult EditRWATDS(int id)
        {
            var data = ent.RWATDSMasters.Find(id);
            var model = Mapper.Map<RWATDSDTO>(data);
            model.Department = data.Department;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditRWATDS(RWATDSDTO data)
        {
            ModelState.Remove("Id");
            ModelState.Remove("IsDeleted");
            ModelState.Remove("Name");
            ModelState.Remove("Department");
            var model = Mapper.Map<RWATDSMaster>(data);
            model.Department = data.Department;
            ent.Entry(model).State = System.Data.Entity.EntityState.Modified;
            ent.SaveChanges();
            TempData["msg"] = "Successfully Updated";
            return RedirectToAction("RWATDS");
        }
        [HttpGet]
        public ActionResult FranchiseTDS()
        {
            var model = new FranchiseTDSDTO();
            var Q = @"select * from FranchiseTDSMaster where IsDeleted=0 order by Name asc";
            var data = ent.Database.SqlQuery<FranchiseTDSLIST>(Q).ToList();
            model.FranchiseTDSLIST = data;
            return View(model);
        }
        [HttpPost]
        public ActionResult AddFranchiseTDS(FranchiseTDSDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["msg"] = "Some Error";
                return RedirectToAction("FranchiseTDS");
            }
            if (ent.FranchiseTDSMasters.Any(a => a.Name == model.Name && a.IsDeleted == false))
            {
                TempData["msg"] = "Already Exist with this Name";
                return RedirectToAction("FranchiseTDS");
            }
            var data = new FranchiseTDSMaster();
            data.Name = model.Name;
            data.Amount = model.Amount;
            data.IsDeleted = false;
            data.Department = model.Department;
            ent.FranchiseTDSMasters.Add(data);
            ent.SaveChanges();
            TempData["msg"] = "SuccessFully Saved";
            return RedirectToAction("FranchiseTDS");
        }
        public ActionResult DeleteFranchiseTDS(int Id)
        {
            var data = ent.FranchiseTDSMasters.Find(Id);
            data.IsDeleted = true;
            ent.SaveChanges();
            return RedirectToAction("FranchiseTDS");
        }

        [HttpGet]
        public ActionResult EditFranchiseTDS(int id)
        {
            var data = ent.FranchiseTDSMasters.Find(id);
            var model = Mapper.Map<FranchiseTDSDTO>(data);
            model.Department = data.Department;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditFranchiseTDS(FranchiseTDSDTO data)
        {
            ModelState.Remove("Id");
            ModelState.Remove("IsDeleted");
            ModelState.Remove("Name");
            ModelState.Remove("Department");
            var model = Mapper.Map<FranchiseTDSMaster>(data);
            model.Department = data.Department;
            ent.Entry(model).State = System.Data.Entity.EntityState.Modified;
            ent.SaveChanges();
            TempData["msg"] = "Successfully Updated";
            return RedirectToAction("FranchiseTDS");
        }

        [HttpGet]
        public ActionResult TDS()
        {
            var model = new TDSMasterDTO();
            var Q = @"select * from TDSMaster where IsDeleted=0 order by Name asc";
            var data = ent.Database.SqlQuery<tdsList>(Q).ToList();
            model.TDSLIST = data;
            return View(model);
        }
        
        [HttpPost]
        public ActionResult AddTDS(TDSMasterDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["msg"] = "Some Error";
                return RedirectToAction("TDS");
            }
           if(ent.TDSMasters.Any(a => a.Name == model.Name && a.IsDeleted == false))
            {
                TempData["msg"] = "Already Exist with this Name";
                return RedirectToAction("TDS");
            }
                var data = new TDSMaster();
                data.Name = model.Name;
                data.Amount = model.Amount;
                data.IsDeleted = false;
                ent.TDSMasters.Add(data);
                ent.SaveChanges();
                TempData["msg"] = "SuccessFully Saved";
                return RedirectToAction("TDS");
        }

        public ActionResult UpdateTDS(int id)
        {
            var data = ent.TDSMasters.Find(id);
            data.IsDeleted = true;
            ent.SaveChanges();
            return RedirectToAction("TDS");
        }

        public ActionResult DeleteTDS(int Id)
        {
            var data = ent.TDSMasters.Find(Id);
            data.IsDeleted = true;
            ent.SaveChanges();
            return RedirectToAction("TDS");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var data = ent.TDSMasters.Find(id);
            var model = Mapper.Map<TDSMasterDTO>(data);
            return View(model);
        }


        [HttpPost]
        public ActionResult Edit(TDSMasterDTO data)
        {
            ModelState.Remove("Id");
            ModelState.Remove("IsDeleted");
            ModelState.Remove("Name");
            var model = Mapper.Map<TDSMaster>(data);
            ent.Entry(model).State = System.Data.Entity.EntityState.Modified;
            ent.SaveChanges();
            TempData["msg"] = "ok";
            return RedirectToAction("Edit", new { id = model.Id });
        }

        [HttpGet]
        public ActionResult EditCommission(int id)
        {
            var data = ent.CommissionMasters.Find(id);
            var model = Mapper.Map<CommissionDTO>(data);
            return View(model);
        }


        [HttpPost]
        public ActionResult EditCommission(CommissionDTO data)
        {
            ModelState.Remove("Id");
            ModelState.Remove("IsDeleted");
            ModelState.Remove("Name");
            var model = Mapper.Map<CommissionMaster>(data);
            ent.Entry(model).State = System.Data.Entity.EntityState.Modified;
            ent.SaveChanges();
            TempData["msg"] = "ok";
            return RedirectToAction("EditCommission", new { id = model.Id });
        }


        public ActionResult DeleteCommission(int Id)
        {
            var data = ent.CommissionMasters.Find(Id);
            data.IsDeleted = true;
            ent.SaveChanges();
            return RedirectToAction("Commision");
        }

        [HttpGet]
        public ActionResult ViewMore(string term)
        {
            var model = new UpdateDepartment();
            string q = @"select * from TempDepartment where IsDeleted = 0 and IsApproved=0 order by Id desc";
            var result = ent.Database.SqlQuery<DepartmentList>(q).ToList();
          
            if (term == "Doctor")
            {
                string main = @"select * from TempDepartment where Requested= 'Doctor' and IsDeleted = 0 and IsApproved=0 order by Id desc";
                var doctor = ent.Database.SqlQuery<DepartmentList>(main).ToList();
                if(doctor.Count() == 0)
                {
                    TempData["msg"] = "No Records Available";
                    return View(model);
                }
                model.DepartmentList = doctor;
                return View(model);
            }
            if (term == "Hospital")
            {
                string q1 = @"select * from TempDepartment where Requested= 'Hospital' and IsDeleted = 0 and IsApproved=0 order by Id desc";
                var Hospital = ent.Database.SqlQuery<DepartmentList>(q1).ToList();
                if (Hospital.Count() == 0)
                {
                    TempData["msg"] = "No Records Available";
                    return View(model);
                }
                model.DepartmentList = Hospital;
                return View(model);
            }
            if(term == "Vendor")
            {
                string q2 = @"select * from TempDepartment where Requested= 'Vendor' and IsDeleted = 0 and IsApproved=0 order by Id desc";
                var Vendor = ent.Database.SqlQuery<DepartmentList>(q2).ToList();
                if (Vendor.Count() == 0)
                {
                    TempData["msg"] = "No Records Available";
                    return View(model);
                }
                model.DepartmentList = Vendor;
                return View(model);
            }
            model.DepartmentList = result;
            return View(model);
        }


        public ActionResult updateDep(int id)
        {
            var values = ent.TempDepartments.Find(id);
            var dept = new Department();
            var spl = new Specialist();
            ent.Database.ExecuteSqlCommand("update TempDepartment set IsApproved = 1 where Id=" + id);
            dept.DepartmentName = values.Department;
            dept.IsDeleted = false;
            ent.Departments.Add(dept);
            ent.SaveChanges();
            spl.SpecialistName = values.Specialist;
            spl.Department_Id = dept.Id;
            spl.IsDeleted = false;
            ent.Specialists.Add(spl);
            ent.SaveChanges();
            return RedirectToAction("ViewMore");
        }

        public ActionResult deleteDep(int id)
        {
       
            ent.Database.ExecuteSqlCommand("update TempDepartment set IsDeleted = 1 where Id=" + id);
            return RedirectToAction("ViewMore");
        }


        [HttpGet]
        public ActionResult ViewVehicle()
        {
            var model = new VehicleList();
            string q = @"select * from VehicleTemp where IsApproved=0 order by Id desc";
            var result = ent.Database.SqlQuery<VehicleItem>(q).ToList();
            if(result.Count() == 0)
            {
                TempData["msg"] = "No Records Available";
                return View(model);
            }
            model.vahan_suchi = result;
            return View(model);
        }


        public ActionResult UpdateList(int id)
        {
            var values = ent.VehicleTemps.Find(id);
            ent.Database.ExecuteSqlCommand("update VehicleTemp set IsApproved = 1 where Id=" + id);
            var vehicleType = new VehicleType
            {
                VehicleTypeName = values.VehicleTypeName,
                Category_Id = values.Category_Id
            };
            ent.VehicleTypes.Add(vehicleType);
            ent.SaveChanges();
            //var model = new VehicleType();
            //model.under5KM = values.under5KM;
            //model.under6_10KM = values.under6_10KM;
            //model.under11_20KM = values.under11_20KM;
            //model.under21_40KM = values.under21_40KM;
            //model.under41_60KM = values.under41_60KM;
            //model.under5KM = values.under61_80KM;
            //model.under81_100KM = values.under81_100KM;
            //model.under100_150KM = values.under100_150KM;
            //model.under151_200KM = values.under151_200KM;
            //model.under201_250KM = values.under201_250KM;
            //model.under251_300KM = values.under251_300KM;
            //model.under301_350KM = values.under301_350KM;
            //model.under351_400KM = values.under351_400KM;
            //model.under401_450KM = values.under401_450KM;
            //model.under451_500KM = values.under451_500KM;
            //model.Above500KM = values.Above500KM;
            //model.VehicleTypeName = values.VehicleTypeName;
            //model.IsDeleted = false;
            //ent.VehicleTypes.Add(model);
            //ent.SaveChanges();
            return RedirectToAction("Edit","VehicleType", new { id=vehicleType.Id});
        }


        [HttpGet]
        public ActionResult LabReport(string term = null, int pageNumber = 0)
        {
            var model = new HealthCheckupPatientList();
            var q = @"select P.PatientName, P.Id, P.PatientRegNo, L.LabName from LabReport C join Patient P on P.Id = C.Patient_Id 
join Lab L on L.Id = C.Lab_Id 
GROUP BY  P.PatientName,P.Id, P.PatientRegNo, L.LabName";
            var data = ent.Database.SqlQuery<PatientTestListVM>(q).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                data = data.Where(a => a.PatientRegNo.Contains(term)).ToList();
                if(data.Count() == 0)
                {
                    TempData["msg"] = "No";
                    return View(model);
                }
            }
            var total = data.Count();
            pageNumber = (int?)pageNumber ?? 1;
            int pageSize = 10;
            decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
            model.TotalPages = (int)noOfPages;
            model.PageNumber = (int)pageNumber;
            data = data.OrderBy(a => a.Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
            model.PatientTestList = data;
            return View(model);
        }

        [HttpGet]
        public ActionResult LabTestReport(int id)
        {
            var model = new HealthCheckupPatientList();
            var q = @"select * from LabReport C join Patient p on c.Patient_Id = p.Id join LabTest lt on lt.Id = C.Test join CityMaster city on city.Id = p.CityMaster_Id join StateMaster statemaster on statemaster.Id = p.StateMaster_Id where  C.Patient_Id=" + id;
            var data = ent.Database.SqlQuery<LabTestReport>(q).ToList();
            model.LabTestReport = data;
            return View(model);
        }

        [HttpGet]
        public ActionResult DoctorReports(string term = null, int pageNumber = 0)
        {
            var model = new ViewDoctorReports();
            string q = @"select C.Patient_Id, P.PatientName, p.PatientRegNo, d.DoctorName from DoctorReports C join Patient P on P.Id = C.Patient_Id 
join Doctor d on d.Id = C.Doctor_Id 
GROUP BY C.Patient_Id, P.PatientName, p.PatientRegNo, d.DoctorName,C.UploadDate
order by C.UploadDate desc";
            var data = ent.Database.SqlQuery<PatientItem>(q).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                data = data.Where(a => a.PatientRegNo.Contains(term)).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No";
                    return View(model);
                }
            }
            var total = data.Count();
            pageNumber = (int?)pageNumber ?? 1;
            int pageSize = 10;
            decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
            model.TotalPages = (int)noOfPages;
            model.PageNumber = (int)pageNumber;
            data = data.OrderBy(a => a.Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No";
                return View(model);
            }
            model.patientItem = data;
            return View(model);
        }

        [HttpGet]
        public ActionResult DoctorTestReports(int id)
        {
            var model = new ViewDoctorReports();
            var q = @"select * from DoctorReports C join Patient p on c.Patient_Id = p.Id  join CityMaster city on city.Id = p.CityMaster_Id join StateMaster statemaster on statemaster.Id = p.StateMaster_Id where  C.Patient_Id=" + id;
            var data = ent.Database.SqlQuery<TestHistory>(q).ToList();
            model.test = data;
            return View(model);
        }

        public ActionResult HealthReport(string term = null, int pageNumber = 0)
        {
            var model = new HealthCheckupPatientList();
            var q = @"select P.PatientName, P.Id, P.PatientRegNo, L.LabName from CheckUpReport C join Patient P on P.Id = C.Patient_Id 
join Lab L on L.Id = C.Checkup_Center_Id 
GROUP BY  P.PatientName,P.Id, P.PatientRegNo, L.LabName";
            var data = ent.Database.SqlQuery<PatientTestListVM>(q).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                data = data.Where(a => a.PatientRegNo.Contains(term)).ToList();
                if (data.Count() == 0)
                {
                    TempData["msg"] = "No";
                    return View(model);
                }
            }
            var total = data.Count();
            pageNumber = (int?)pageNumber ?? 1;
            int pageSize = 10;
            decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
            model.TotalPages = (int)noOfPages;
            model.PageNumber = (int)pageNumber;
            data = data.OrderBy(a => a.Id).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
            model.PatientTestList = data;
            return View(model);
        }

        [HttpGet]
        public ActionResult HealthTestReport(int id)
        {
            var model = new HealthCheckupPatientList();
            var q = @"select * from CheckUpReport C join Patient p on c.Patient_Id = p.Id  join CityMaster city on city.Id = p.CityMaster_Id join StateMaster statemaster on statemaster.Id = p.StateMaster_Id where  C.Patient_Id=" + id;
            var data = ent.Database.SqlQuery<LabTestReport>(q).ToList();
            model.LabTestReport = data;
            return View(model);
        }

        [HttpGet]
        public ActionResult BankDetails(int Id)
        {
            var data = ent.BankDetails.Where(a => a.Login_Id == Id).ToList();
            return View(data);
        }

        [HttpGet]
        public ActionResult AmbulaneBankDetails(int Id)
        {
            var model = new AmbulaneBankDetailsVM();
            string q = @"select * from Vehicle where Id="+ Id;
            var data = ent.Database.SqlQuery<AmbulaneBankDetailsVM>(q).ToList();
            model.AccountNo = data.FirstOrDefault().AccountNo;
            model.BranchAddress = data.FirstOrDefault().BranchAddress;
            model.BranchName = data.FirstOrDefault().BranchName;
            model.CancelCheque = data.FirstOrDefault().CancelCheque;
            model.HolderName = data.FirstOrDefault().HolderName;
            model.IFSCCode = data.FirstOrDefault().IFSCCode;
            return View(model);
        }

        [HttpPost]
        public JsonResult GetAllFranchise(string term)
        {
            var data = Mapper.Map<IEnumerable<VendorDTO>>(ent.Vendors.Where(a => a.UniqueId.ToUpper().Contains(term.ToUpper())).ToList());
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        

    }
}