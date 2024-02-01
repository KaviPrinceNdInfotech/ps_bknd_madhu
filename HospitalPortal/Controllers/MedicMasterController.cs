using AutoMapper;
using ExcelDataReader;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using log4net;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    [Authorize (Roles ="admin")]
    public class MedicMasterController : Controller
    {
        // GET: MedicMaster
        DbEntities ent = new DbEntities();
        ILog log = LogManager.GetLogger(typeof(MedicMasterController));

        public ActionResult MedicMaster()
        {
            return View();
        }

        public ActionResult GetMedicineType()
        {
            var data = ent.MedicineTypes.ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddMedicine(MedicineDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Content("All fields with * mark are mandatory.");
                var medicine = Mapper.Map<Medicine>(model);
                ent.Medicines.Add(medicine);
                ent.SaveChanges();
                return Content("ok");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return Content("error");
            }
        }

        public ActionResult UpdateMedicine(MedicineDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Content("All fields with * mark are mandatory.");
                var medicine = Mapper.Map<Medicine>(model);
                ent.Entry(medicine).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                return Content("ok");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return Content("error");
            }
        }

        public ActionResult DeleteMedicine(int id)
        {
            try
            {
                var data = ent.Medicines.Find(id);
                data.IsDeleted = true;
                ent.SaveChanges();
                return Content("ok");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return Content("Error");
            }
        }

        public ActionResult GetAllMedicines(string term, int page = 1)
        {
            int pageSize = 50;
            int totalRecords = string.IsNullOrEmpty(term) ?
                ent.Medicines.Count() :
                ent.Medicines.Where(a => a.MedicineName.ToLower().Contains(term)).Count();
            int totalPages = Convert.ToInt32(Math.Ceiling(totalRecords / (double)pageSize));
            int skip = pageSize * (page - 1);
            var medicines = new List<MedicineDTO>();
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                medicines = (from m in ent.Medicines
                             join t in ent.MedicineTypes
     on m.MedicineType_Id equals t.Id
     into mt
                             from nt in mt.DefaultIfEmpty()
                             where !m.IsDeleted
                             && m.MedicineName.ToLower().Contains(term)
                             select new MedicineDTO
                             {
                                 Id = m.Id,
                                 BrandName = m.BrandName,
                                 MedicineDescription = m.MedicineDescription,
                                 MedicineName = m.MedicineName,
                                 MRP = m.MRP,
                                 MedicineTypeName = nt == null ? "N/A" : nt.MedicineTypeName,
                                 MedicineType_Id = m.MedicineType_Id
                             }).OrderByDescending(a => a.Id).Skip(skip).Take(pageSize).ToList();

                // medicines = medicines.Where(a => !a.IsDeleted && a.MedicineName.ToLower().Contains(term)).Skip(pageSize).Take(take).ToList();

            }
            else
            {
                medicines = (from m in ent.Medicines
                             join t in ent.MedicineTypes
     on m.MedicineType_Id equals t.Id
     into mt
                             from nt in mt.DefaultIfEmpty()
                             where !m.IsDeleted
                             select new MedicineDTO
                             {
                                 Id = m.Id,
                                 BrandName = m.BrandName,
                                 MedicineDescription = m.MedicineDescription,
                                 MedicineName = m.MedicineName,
                                 MRP = m.MRP,
                                 MedicineTypeName = nt.MedicineTypeName,
                                 MedicineType_Id = m.MedicineType_Id
                             }).OrderBy(a => a.MedicineName).ToList();
            }
            medicines = medicines.Skip(skip).Take(pageSize).ToList();
            var rm = new ShowMedicineModel();
            rm.medicines = medicines;
            rm.page = page;
            rm.totalPages = totalPages;
            return Json(rm, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public async Task<ActionResult> ImportFile(HttpPostedFileBase importFile)
        {
            if (importFile == null) return Json(new { Status = 0, Message = "No File Selected" });

            try
            {
                var fileData = GetDataFromExcel(importFile.InputStream);

                 
                return Json(new { Status = 1, Message = "File Imported Successfully " });
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, Message = ex.Message });
            }
        }

        private List<Medicine> GetDataFromExcel(Stream stream)
        {
            var medicines = new List<Medicine>();
            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first sheet

                    for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        var medicine = new Medicine
                        {
                            MedicineName = worksheet.Cells[row, 1].Value?.ToString(),
                            BrandName = worksheet.Cells[row, 2].Value?.ToString(),
                            MedicineDescription = worksheet.Cells[row, 3].Value?.ToString(),
                            MedicineType_Id = Convert.ToInt32(worksheet.Cells[row, 4].Value),
                            MRP = (double)Convert.ToDecimal(worksheet.Cells[row, 5].Value),
                            IsDeleted = false
                        };
                        ent.Medicines.Add(medicine);
                        ent.SaveChanges(); 
                    } 
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                throw;
            }
            return medicines;
        }
    }
}