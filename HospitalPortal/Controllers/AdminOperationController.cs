using AutoMapper;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.RequestModel;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    public class AdminOperationController : Controller
    {
        CommonRepository repos = new CommonRepository();
        private DbEntities ent = new DbEntities();

        #region HealthPackageSection
        [HttpGet]
        public ActionResult AddPackage(int Center_Id = 0)
        {
            var model = new HealthCheckUpPackageDTO();
            model.LabTest = ent.LabTests.Where(a => a.TestName == null).Select(a => new SelectListItem { Text = a.TestDesc, Value = a.Id.ToString() }).ToList();
            model.PackageList = new SelectList(ent.HealthPackageMasters.ToList(), "Id", "PackageName");
            //if (Center_Id != 0)
            //{
            //    model.CenterId = Center_Id;
            //}
            return View(model);
        }

        [HttpPost]
        public ActionResult AddPackage(HealthCheckUpPackageDTO model)
        {
            //var recordCenter = ent.HealthCheckUpPackages.Any(a => a.PackageName == model.PackageName);
            //if (recordCenter == true)
            //{
            //    TempData["msg"] = "You've Already Registered this Package";
            //    return View(model);
            //}
            using (var trans = ent.Database.BeginTransaction())
            {
                try
                {

                    var domain = Mapper.Map<HealthCheckUpPackage>(model);
                    domain.packageId = model.packageId;
                    domain.TestAmt = model.GrandTotal;
                    domain.DiscountAmt = model.DiscountAmt;
                    domain.Gst = model.Gst;
                    ent.HealthCheckUpPackages.Add(domain);
                    ent.SaveChanges();
                    if (model.chosenIds.Length < 1)
                        throw new Exception("You must have test");

                    foreach (int item in model.chosenIds)
                    {
                        var tests = ent.LabTests.Find(item);
                        double? price = tests.TestAmount;
                        var od = new HealthLabTest
                        {
                            TestId = item,
                            HealthPackageID = domain.Id,
                            TestAmt = price
                        };
                        ent.HealthLabTests.Add(od);
                        ent.SaveChanges();
                    }
                    trans.Commit();
                    TempData["msg"] = "Successfully Saved";
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    TempData["msg"] = "Server Error";
                }

            }
            return RedirectToAction("AddPackage");
        }

        [HttpGet]
        public ActionResult ViewPackage()
        {
            var model = new CompleteCheckup();
            string q = @"select hcp.Id, hpm.PackageName,Sum(hlt.TestAmt) as TestAmt, hcp.gst, hcp.DiscountAmt from HealthPackageMaster hpm join HealthCheckUpPackage hcp on hpm.id = hcp.packageId join HealthLabTest hlt on hcp.Id = hlt.HealthPackageID group by hcp.Id, hpm.PackageName, hcp.gst, hcp.DiscountAmt";
            var data = ent.Database.SqlQuery<ShowDesc>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record";
                return View(model);
            }
            model.showDesc = data;
            return View(model);
        }


        [HttpGet]
        public ActionResult EditPacakge(int Id)
        {
            var data = ent.HealthCheckUpPackages.Find(Id);
            var model = Mapper.Map<HealthCheckUpPackageDTO>(data);
            model.PackageList = new SelectList(repos.GetPackageNames(), "Id", "PackageName", model.HealthPackageID);
            string q = @"select HealthLabTest.Id, Labtest.TestDesc, labtest.testAmount from LabTest join HealthLabTest on labTest.id = Healthlabtest.TESTid where Healthlabtest.HealthPackageID = " + Id;
            var TestRecord = ent.Database.SqlQuery<HealthTest>(q).ToList();
            model.HealthTest = TestRecord;
            var testRecord = ent.HealthCheckUpPackages.Find(Id);
            model.gTotal = TestRecord.Sum(a => a.TestAmount);
            double? DiscountAmt =  testRecord.DiscountAmt == null ? 0 : testRecord.DiscountAmt;
            double? GrndTotal = model.gTotal - DiscountAmt;
            int? gst = testRecord.Gst == null ? 0 : testRecord.Gst;
            model.GrandTotal = Math.Round(Convert.ToDouble(GrndTotal + (GrndTotal * gst / 100)));
            return View(model);
        }


        [HttpPost]
        public ActionResult EditPacakge(HealthCheckUpPackageDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                return View(model);
              
                //var tests = ent.HealthCheckUpPackages.Find(model.Id);
                var domainModel = Mapper.Map<HealthCheckUpPackage>(model);
                domainModel.TestAmt = model.GrandTotal;
                ent.Entry(domainModel).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                return RedirectToAction("EditPacakge", new { Id = model.Id });
                
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server Error";
                return RedirectToAction("EditPacakge", new { Id = model.Id });
            }
        }

        [HttpGet]
        public ActionResult ViewHealthTest(int Id)
        {
            var model = new CompleteCheckup();
            string q = @"select HealthLabTest.Id, Labtest.TestDesc, labtest.testAmount from LabTest join HealthLabTest on labTest.id = Healthlabtest.TESTid where Healthlabtest.HealthPackageID = " + Id;
            var data = ent.Database.SqlQuery<HealthTest>(q).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record";
                return View(model);
            }
            model.HealthTest = data;
            return View(model);
        }

        [HttpGet]
        public ActionResult EditHealthTest(int Id, int statusKey = 0)
        {
            var data = ent.HealthLabTests.Find(Id);
            var model = Mapper.Map<HealthCheckUpPackageDTO>(data);
            model.statusKey = statusKey;
            model.Tests = new SelectList(repos.GetTestsName(), "Id", "TestDesc", model.Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditHealthTest(HealthCheckUpPackageDTO model)
        {
            try
            {
                ModelState.Remove("Test_Id");
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var tests = ent.LabTests.Find(model.TestId);
                var domainModel = Mapper.Map<HealthLabTest>(model);
                domainModel.TestAmt = tests.TestAmount;
                ent.Entry(domainModel).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                if (model.statusKey == 0)
                {
                    return RedirectToAction("ViewHealthTest", new { Id = model.HealthPackageID });
                }
                else
                {
                    return RedirectToAction("EditPacakge", new { Id = model.HealthPackageID });
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server Error";
                return RedirectToAction("ViewHealthTest");
            }
        }

        [HttpGet]
        public ActionResult DeleteHealthPackage(int Id)
        {
            var data = ent.HealthCheckUpPackages.Find(Id);
            try
            {
                ent.HealthCheckUpPackages.Remove(data);
                ent.SaveChanges();
                string q = @"delete from HealthLabTest where HealthPackageId=" + Id;
                ent.Database.ExecuteSqlCommand(q);
            }
            catch (Exception ex)
            {
                //log.Error(ex.Message);
            }
            return RedirectToAction("ViewPackage"); 

        }

        [HttpGet]
        public ActionResult DeleteHealthTest(int Id, int statusKey = 0)
        {
            var data = ent.HealthLabTests.Find(Id);
            try
            {
                ent.HealthLabTests.Remove(data);
                ent.SaveChanges();
            }
            catch (Exception ex)
            {
                //log.Error(ex.Message);
            }
            if (statusKey == 0)
            {
                return RedirectToAction("ViewHealthTest", new { Id = data.HealthPackageID });
            }
            else
            {
                return RedirectToAction("EditPacakge", new { Id = data.HealthPackageID });
            }
        }

        #endregion


        public ActionResult AddTestDetails(int id = 0)
        {
            var data = new AddTestByLabDTO();
            data.Tests = new SelectList(repos.GetTests(), "Id", "TestName");
            if (id != 0)
            {
                data.TestNameList = new SelectList(repos.GetTestsName(), "Id", "TestDesc");
                data.Lab_Id = id;
            }
            return View(data);
        }

        [HttpPost]
        public ActionResult AddTestDetails(AddTestByLabDTO model)
        {
            var data = new TestLab();
            if (!ModelState.IsValid)
            {
                model.Tests = new SelectList(repos.GetTests(), "Id", "TestName");
                return View(model);
            }
         
            try
            {
                //Only Admin Can Enter The Test Details.
                if (User.IsInRole("admin"))
                {
                    if (ent.LabTests.Any(a => a.TestDesc == model.TestDesc))
                    {
                        TempData["msg"] = "This Test " +model.TestDesc+ " Already Exists";
                        return RedirectToAction("AddTestDetails", new { Id = model.Lab_Id });
                    }

                    var data1 = new LabTest
                    {
                        TestAmount = model.TestAmount,
                        TestName = model.TestName,
                        TestDesc = model.TestDesc,
                        Test_Id = model.Test_Id
                    };
                    ent.LabTests.Add(data1);
                    ent.SaveChanges();
                }
                else
                {
                    if (model.TestNameId == null)
                    {
                        var data1 = new LabTest
                        {
                            TestAmount = model.TestAmount,
                            TestName = model.TestName,
                            TestDesc = model.TestDescription,
                            Lab_Id = model.Lab_Id,
                        };
                        ent.LabTests.Add(data1);
                        ent.SaveChanges();
                        model.Test_Id = data1.Id;

                        var data2 = new TestLab
                        {
                            Lab_Id = (int)model.Lab_Id,
                            Test_Id = model.Test_Id,
                            TestDescription = model.TestDescription,
                            TestAmount = model.TestAmount,
                        };
                        ent.TestLabs.Add(data2);
                        ent.SaveChanges();
                    }
                }
                //else
                //{
                //    var recordCenter = ent.LabTests.Any(a => a.TestDesc == model.TestDesc);
                //    if (recordCenter == true)
                //    {
                //        TempData["msg"] = "You've Already Added this Test";
                //        return RedirectToAction("AddTestDetails", new { Id = model.Lab_Id });
                //    }
                //    var data2 = new TestLab
                //    {
                //        Lab_Id = (int)model.Lab_Id,
                //        Test_Id = model.TestNameId,
                //        TestDescription = model.TestDescription,
                //        TestAmount = model.TestAmount,
                //    };
                //    ent.TestLabs.Add(data2);
                //    ent.SaveChanges();
                //}
                TempData["msg"] = "Successfully Added";
                return RedirectToAction("AddTestDetails", new {Id= model.Lab_Id });
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex;
                return RedirectToAction("AddTestDetails");
            }
        }

        public ActionResult ShowTest(int LabId = 0, string term = null)
        {
            var model = new TestListByLab();
            model.labId = LabId;
            var q = @"select a.TestName,b.TestAmount, b.Id , b.TestDesc from LabTest a  join LabTest b on a.Id = b.Test_Id order by a.TestName asc ,a.TestDesc asc";
            var data = ent.Database.SqlQuery<TestsList>(q).ToList();
            if (!string.IsNullOrEmpty(term))
            {
                data = data.Where(a => a.TestDesc.Contains(term)).ToList();
            }
            if(LabId != 0)
            {
                data = data.Where(a => a.LabId == LabId).ToList();
            }
            model.TestsList = data;
            return View(model);
        }

        public ActionResult DeleteTest(int Delid)
        {
            var data = ent.LabTests.Find(Delid);
            try
            {
                ent.LabTests.Remove(data);
                ent.SaveChanges();
            }
            catch (Exception ex)
            {
                
            }
            return RedirectToAction("ShowTest", "AdminOperation");

        }

        public ActionResult EditTest(int testid)
        {
            var data = ent.LabTests.Find(testid);
            var model = Mapper.Map<AddTestByLabDTO>(data);
            model.Tests = new SelectList(repos.GetTests(), "Id", "TestName");
            return View(model);
        }

        [HttpPost]
        public ActionResult EditTest(AddTestByLabDTO model)
        {
            try
            {
                ModelState.Remove("Test_Id");
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var domainModel = Mapper.Map<LabTest>(model);
                ent.Entry(domainModel).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                return RedirectToAction("ShowTest", new { testid = model.Id });
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server Error";
                return RedirectToAction("ShowTest");
            }
        }

      

        [HttpGet]
        public ActionResult AddTestCartegory(int id = 0)
        {
            var model = new AddTestByLabDTO();
            model.LabNameList = new SelectList(ent.Labs.ToList(), "Id", "LabName");
            //string q  = @"select LT.Id,LT.TestName,L.LabName from LabTest as LT left join Lab as L on L.Id=LT.Lab_Id  where TestName != 'null' order by LT.Id desc";
            string q  = @"select Id,TestName from LabTest where TestName != 'null' order by TestName asc";
            var data = ent.Database.SqlQuery<TestCategoryList>(q).ToList();
            model.TestCategoryList = data;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddTestCartegory(AddTestByLabDTO model)
        {
            
            //if (!ModelState.IsValid)
            //{
            //    model.LabNameList = new SelectList(repos.GetTests(), "Id", "LabName");
            //    return View(model);
            //}
            var recordCenter = ent.LabTests.Any(a => a.TestName == model.TestName);

            if (recordCenter == true)
            {
                TempData["msg"] = "You've Already Added this Test Category";
                return RedirectToAction("AddTestDetails");
            }
            try
            {
                
                var data1 = new LabTest
                {
                    TestName = model.TestName,
                    //Lab_Id = model.Lab_Id
                };
                ent.LabTests.Add(data1);
                ent.SaveChanges();
                TempData["msg"] = "Successfully Added";

                //var data = ent.Labs.Where(a => a.Id == model.Lab_Id).FirstOrDefault();
                //data.LabTest_Id = data1.Id;
                //ent.SaveChanges();
                
                return RedirectToAction("AddTestCartegory");
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex;
                return RedirectToAction("AddTestCartegory");
            }
        }

        [HttpGet]
        public ActionResult RefundMoney(int Id)
        {
            var data = new refundAmountDTO();
            data.Patient_Id = Id;
            return View(data);
        }

        [HttpPost]
        public ActionResult RefundMoney(refundAmountDTO model)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ",
ModelState.Values
.SelectMany(a => a.Errors)
.Select(a => a.ErrorMessage));
                TempData["Msg"] = message;
                return View(model);
            }
            var userData = ent.Patients.Find(model.Patient_Id);
            //var domain = Mapper.Map<UserWallet>(model);
            //domain.AdminId = userData.AdminLogin_Id;
            //domain.TransactionType = "cr";
            //ent.UserWallets.Add(domain);
            //ent.SaveChanges();
            //TempData["Msg"] = "Successfully Refunded";
            //return RedirectToAction("RefundMoney");
            var domain = new UserWallet()
            {
                AdminId = userData.AdminLogin_Id,
                UserId = model.Patient_Id,
                Amount = model.Amount,
                TransactionType = "cr",

            };
            ent.UserWallets.Add(domain);
            ent.SaveChanges();
            TempData["Msg"] = "Successfully Refunded";
            return RedirectToAction("RefundMoney", new { id = model.Patient_Id });
        }


        [HttpGet]
        public ActionResult EditCategory(int Id)
        {
            var data = ent.LabTests.Find(Id);
            var record = Mapper.Map<LabTest>(data);
            return View(record);
        }

        [HttpPost]
        public ActionResult EditCategory(LabTest model)
        {
            try
            {
                ModelState.Remove("Id");
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var domainModel = Mapper.Map<LabTest>(model);
                ent.Entry(domainModel).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
                return RedirectToAction("AddTestCartegory");
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Server Error";
                return RedirectToAction("AddTestCartegory");
            }
        }

        public ActionResult DeleteCategory(int Id)
        {
            var data = ent.LabTests.Find(Id);
            try
            {
                ent.LabTests.Remove(data);
                ent.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("AddTestCartegory", "AdminOperation");

        }

        public JsonResult GetTestDetail(string term)
        {
            var products = (from N in ent.LabTests
                            where N.TestDesc.StartsWith(term)
                            && N.TestName == null
                            select new { N.TestDesc, N.Id, N.TestAmount });
            return Json(products, JsonRequestBehavior.AllowGet);
        }


        private static List<SelectListItem> PopulateFruits()
        {
            DbEntities ent = new DbEntities();
            List<SelectListItem> items = new List<SelectListItem>();
            items = ent.LabTests.Where(a => a.TestName == null).Select(a => new SelectListItem { Text = a.TestDesc, Value = a.Id.ToString() }).ToList();
            return items;
        }
    }
}