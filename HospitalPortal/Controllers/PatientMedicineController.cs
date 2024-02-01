using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Http;
using HospitalPortal.BL;
using System.Net.Http;
using HospitalPortal.Models.ViewModels;
using System.Web.UI.WebControls;
using System.Web.Http.Results;

namespace HospitalPortal.Controllers
{

    public class PatientMedicineController : ApiController
    {
        // GET: PatientMedicine
        ILog log = LogManager.GetLogger(typeof(PatientMedicineController));
        DbEntities ent = new DbEntities();
        Rmwithparm rmPrm = new Rmwithparm();
        //Common common = new Common();
        NearestMedicine common = new NearestMedicine();
        GenerateBookingId bk = new GenerateBookingId();

        private PatientCartReturnModel GetMedicineCart(int patientId)
        {
            var model = new PatientCartReturnModel();
            string query = @"execute GetMedicineCart @patientId=" + patientId;
            var data = ent.Database.SqlQuery<PatientCartModel>(query).ToList();
            model.TotalProductsInCart = data.Count;
            model.GrandTotal = data.Sum(a => a.TotalPrice);
            model.MedicineCart = data;
            return model;
        }
        //New API For Get Medicine List By Anchal Shukla On 28 Jan 2023
        [HttpGet]
        public IHttpActionResult GetMedicines()
        {
            return GetAllMedicine();
        }
        public IHttpActionResult GetAllMedicine()
        {
            Medicine objAllMedicines = new Medicine();
            string query = @"execute GEtAllMedicines ";
            var data = ent.Database.SqlQuery<Medicine>(query).ToList();
            return Ok(new { data });
            //return data;//
        }
        //END
        [HttpGet]
        public IHttpActionResult SearchMedicine(string term)
        {
            try
            {
                term = term.ToLower();
                var medicine = ent.Medicines.Where(a => !a.IsDeleted && a.MedicineName.ToLower().Contains(term)).ToList();
                var data = (from m in medicine
                            join t in ent.MedicineTypes
                            on m.MedicineType_Id equals t.Id into tm
                            from t1 in tm.DefaultIfEmpty()
                            select new
                            {
                                m.Id,
                                m.MedicineName,
                                m.BrandName,
                                t1.MedicineTypeName,
                                m.MedicineDescription,
                                m.MRP
                            }).ToList();
                dynamic obj = new ExpandoObject();
                obj.Medicines = data;
                return Ok(obj);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult ValidateDeliveryLocation(float lat, float lng)
        {
            try
            {
                var chemist = common.GetNearsestMedicalStore(lat, lng);
                int isValid = common == null ? 0 : 1;
                dynamic obj = new ExpandoObject();
                obj.IsValid = isValid;
                return Ok(obj);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult AddMedicineToCart(AddMedicineCartRequest model)
        {
            dynamic obj = new ExpandoObject();
            if (!ModelState.IsValid)
            {
                var message = string.Join("|",
                ModelState.Values
                .SelectMany(a => a.Errors)
                .Select(a => a.ErrorMessage));
                obj.Status = 0;
                obj.Message = message;
                return Ok(obj);
            }
            //if(model.Items.Count()<1)
            //{
            //    obj.Status = 0;
            //    obj.Message = "Items length must greater than 0";
            //    return Ok(obj);
            //}
            try
            {
                //foreach (var item in model.Items)
                //{
                    string query = @"exec AddMedicineToCart @patientId,@medicineId,@qty";
                    SqlParameter[] prms = new SqlParameter[3];
                    prms[0] = new SqlParameter("@patientId", model.PatientId);
                    prms[1] = new SqlParameter("@medicineId", model.MedicineId);
                    prms[2] = new SqlParameter("@qty", model.Quantity);
                    ent.Database.ExecuteSqlCommand(query, prms);
               //}
                obj.Status = 1;
                obj.Message = "Items has addedd successfully.";
                obj.MedicineCart = GetMedicineCart(model.PatientId);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return InternalServerError(ex);
            }
        }

        //[HttpGet]
        //public IHttpActionResult DeleteMedicineFromCart(int cartId)
        //{
        //    dynamic obj = new ExpandoObject();
        //    try
        //    {
        //        int patientId = ent.Database.SqlQuery<int>("select Patient_Id from MedicineCart where id=" + cartId).FirstOrDefault();
        //        if (patientId < 1)
        //        {
        //            obj.Status = 0;
        //            obj.Message = "No item found with this id";
        //            return Ok(obj);
        //        }
        //        string query = "exec DeleteMedicineFromCart @cartId=" + cartId;
        //        ent.Database.ExecuteSqlCommand(query);
        //        obj.Status = 1;
        //        obj.Message = "Deletion was successful";
        //        obj.MedicineCartDetail = GetMedicineCart(patientId);
        //        return Ok(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message);
        //        return InternalServerError(ex);
        //    }
        //}

        [HttpGet]
        public IHttpActionResult MedicineCart(int patientId)
        {
            try
            {
                dynamic obj = new ExpandoObject();
                obj.MedicineCartDetail = GetMedicineCart(patientId);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult UpdateMedicineQuantity(UpdateMedicineQtyModel model)
        {
            try
            {
                dynamic obj = new ExpandoObject();
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ",
                    ModelState.Values
                    .SelectMany(a => a.Errors)
                    .Select(a => a.ErrorMessage));
                    obj.Status = 0;
                    obj.Message = message;
                    return Ok(obj);
                }
                string[] acceptedArray = { "i", "d" };
                if (!acceptedArray.Contains(model.Type))
                {
                    obj.Message = "Type accepts only 'i' and 'd' values";
                    obj.Status = 0;
                    return Ok(obj);
                }

                string query = "exec UpdateCartQuantity @patientId,@medicineId,@type";
                var prms = new SqlParameter[3];
                prms[0] = new SqlParameter("@patientId", model.PatientId);
                prms[1] = new SqlParameter("@medicineId", model.MedicineId);
                prms[2] = new SqlParameter("@type", model.Type);
                string result = ent.Database.SqlQuery<string>(query, prms).FirstOrDefault();
                if (result != "ok")
                {
                    obj.Status = 0;
                    obj.Message = result;
                    return Ok(obj);
                }
                obj.MedicineCart = GetMedicineCart(model.PatientId);
                obj.Status = 1;
                obj.Message = "Cart has updated successfully";
                return Ok(obj);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult GenerateMedicineOrder(OrderReq model)
        {
           
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ",
                ModelState.Values
                .SelectMany(a => a.Errors)
                .Select(a => a.ErrorMessage));
                rmPrm.Message = message;
                rmPrm.Status = 0;
                return Ok(rmPrm);
            }
            try
            {
                double lat = ent.Database.SqlQuery<double>(@"select lat from Location where Id=" + model.LocationId).FirstOrDefault();
                double lng = ent.Database.SqlQuery<double>(@"select lng from Location where Id=" + model.LocationId).FirstOrDefault();
                var chemist = common.GetNearsestMedicalStore(lat, lng);
                if (chemist == null)
                {
                    rmPrm.Status = 0;
                    rmPrm.Message = "No medical store found near.";
                    return Ok(rmPrm);
                }
                var prms = new SqlParameter[9];
                prms[0] = new SqlParameter("@patientId", model.PatientId);
                prms[1] = new SqlParameter("@chemistId", chemist.Id);
                prms[2] = new SqlParameter("@name", model.Name);
                prms[3] = new SqlParameter("@mobile", model.Mobile);
                prms[4] = new SqlParameter("@stateId", model.StateId);
                prms[5] = new SqlParameter("@cityId", model.CityId);
                prms[6] = new SqlParameter("@address", model.Address);
                prms[7] = new SqlParameter("@pincode", model.PinCode == null ? DBNull.Value : (object)model.PinCode);
                prms[8] = new SqlParameter("@LocationId", model.LocationId);
                string query = @"exec dbo.sp_saveOrder @patientId,@name,@mobile,@stateId,@cityId,@address,@pincode,@LocationId";
                int result = ent.Database.SqlQuery<int>(query, prms).FirstOrDefault();
                if (result > 0)
                {
                    int Id = ent.Database.SqlQuery<int>(@"select Top 1 Id from MedicineOrder order by Id desc").FirstOrDefault();
                    rmPrm.orderId = Id;
                    rmPrm.Status = 1;
                    rmPrm.Message = "Order has placed successfully";
                }
                else if (result == -1)
                {
                    rmPrm.Status = 0;
                    rmPrm.Message = "Cart is empty";
                }
                else
                {
                    rmPrm.Status = 0;
                    rmPrm.Message = "Server error";
                }
                return Ok(rmPrm);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult UpdateMedicineOrderPaymentStatus(int orderId)
        {
            var rm = new ReturnMessage();
            try
            {
                string query = @"update MedicineOrder set IsPaid=1 where Id=" + orderId;
                ent.Database.ExecuteSqlCommand(query);
                rm.Status = 1;
                rm.Message = "Payment status has updated successfully.";
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
                rm.Status = 0;
                rm.Message = "server error";
            }
            return Ok(rm);

        }

        [HttpGet]
        public IHttpActionResult MedicineOrderHistory(int patientId)
        {
            try
            {
                string orderQuery = @"exec sp_getOrder @patientId="+ patientId;
                var ord = ent.Database.SqlQuery<PatientOrderModel>(orderQuery).ToList();
                foreach(var o in ord)
                {
                    string orderDetailQuery = @"exec sp_getOrderDetail @orderId="+o.Id;
                    var od = ent.Database.SqlQuery<PatientOrderDetailModel>(orderDetailQuery).ToList();
                    o.OrderDetail = od;
                }
                dynamic obj = new ExpandoObject();
                obj.OrderHistory = ord;
                return Ok(obj);
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        public IHttpActionResult MedicineDetailsByPatient(int PatientId)
        {
            var model = new MedicineListp();
            string query = @"select MeO.id,MeO.DeliveryAddress,MeO.DeliveryDate,MeO.InvoiceNumber,MeO.OrderId ,Me.MedicineName,Me.BrandName,Me.MedicineDescription,
MeT.MedicineTypeName,MEOD.Amount,MEOD.Quantity
from MedicineOrder AS MeO with(nolock)  
INNER JOIN MedicineOrderDetail AS  MEOD with(nolock) on MeO.Id = MEOD.Order_Id
INNER JOIN Medicine  AS Me with(nolock) On MEOD.Medicine_Id = Me.Id
INNER JOIN MedicineType AS MeT with(nolock) ON Me.MedicineType_Id =MeT.Id
WHERE  MeO.Patient_Id= " + PatientId + " order by MeO.Id desc";
            var data = ent.Database.SqlQuery<PaMedicine>(query).ToList();
            model.PaMedicine = data;
            return Ok(model);
        }

        [Route("api/PatientMedicine/AddMedicineCart")]
        [HttpPost]
        public IHttpActionResult AddMedicineCart(AddMedicineCartRequests model)
        {
            try
            {
                var adcard = ent.MedicineCarts.Where(a => a.Patient_Id == model.PatientId && a.Medicine_Id == model.MedicineId).FirstOrDefault();
                if (adcard == null)
                {
                    var Medicine = ent.Medicines.Where(a => a.Id == model.MedicineId).FirstOrDefault();
                    var data = new MedicineCart()
                    {
                        Patient_Id = model.PatientId,
                        Medicine_Id = model.MedicineId,
                        Quantity = 1,
                        price = Medicine.MRP,
                        TotalPrice = Medicine.MRP * 1,
                    };
                    ent.MedicineCarts.Add(data);
                    ent.SaveChanges();
                    return Ok(new { Message = "Add To Cart SuccessFully " });
                }
                else
                {
                    return BadRequest("Already exist");
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
                return InternalServerError(ex);
            }
        }
        [Route("api/PatientMedicine/AddMedicinelist")]
        [HttpGet]
        public IHttpActionResult AddMedicinelist(int patientId)
        {
            string query = @"select cart.Id as CartId,cart.Medicine_Id,cart.Quantity,m.MedicineName,m.BrandName,m.MRP as UnitPrice,
  (m.MRP * cart.Quantity) as TotalPrice from MedicineCart cart
 left join Medicine m on cart.Medicine_Id = m.Id
 where cart.Patient_Id= " + patientId;
            var data = ent.Database.SqlQuery<PatientCart>(query).FirstOrDefault();
            return Ok(data);
        }


        [HttpPost, Route("api/PatientMedicine/PlusAddToCart")]
        public IHttpActionResult PlusAddToCart(AddMedicineCartRequests model)
        {
            try
            {
                var data = ent.MedicineCarts.Where(a => a.Medicine_Id == model.MedicineId && a.Patient_Id == model.PatientId).FirstOrDefault();
                data.Quantity = data.Quantity + 1;
                data.TotalPrice = data.price * data.Quantity;
                if (data != null)
                {
                    data.price = data.price;
                    data.TotalPrice = data.TotalPrice;
                    data.Quantity = data.Quantity;
                }
                ent.SaveChanges();
                return Ok(new { status = 200, data.Quantity, data.TotalPrice, message = "Add SuccessFully" });
            }
            catch
            {
                return BadRequest("Server Error");
            }
        }

        [HttpPost, Route("api/PatientMedicine/minusAddToCart")]
        public IHttpActionResult minusAddToCart(AddMedicineCartRequests model)
        {
            try
            {
                var data = ent.MedicineCarts.Where(a => a.Medicine_Id == model.MedicineId && a.Patient_Id == model.PatientId).FirstOrDefault();
                data.Quantity = data.Quantity - 1;
                data.TotalPrice = data.price * data.Quantity;
                if (data != null)
                {
                    data.price = data.price;
                    data.TotalPrice = data.TotalPrice;
                    data.Quantity = data.Quantity;

                }

                if (data.Quantity == 0)
                {
                    ent.MedicineCarts.Remove(data);
                    ent.SaveChanges();
                }
                ent.SaveChanges();
                return Ok(new { status = 200, data.Quantity, data.TotalPrice, message = "Delete SuccessFully" });
            }
            catch
            {
                return BadRequest("Server Error");
            }
        }

        [Route("api/PatientMedicine/MedicineDetails")]
        [HttpGet]
        public IHttpActionResult MedicineDetails(int PatientId, PatientCart model)
        {
            try
            {
                var data = (from s in ent.MedicineCarts
                            join r in ent.Medicines on s.Medicine_Id equals r.Id
                            where s.Patient_Id == PatientId
                            select new
                            {
                                id = r.Id,
                                MedicineName = r.MedicineName,
                                BrandName = r.BrandName,
                                Quantity = s.Quantity,
                                UnitPrice = r.MRP,
                                TotalPrice = r.MRP * s.Quantity,
                            }).ToList();
                var TotalPrice = ent.MedicineCarts.Where(x => x.Patient_Id == PatientId).ToList().Sum(x => Convert.ToInt32(x.TotalPrice));

                var Quantity = ent.MedicineCarts.Where(x => x.Patient_Id == PatientId).ToList().Count();

                if (data != null)
                {
                    return Ok(new { data, TotalPrice, Quantity, status = 200, message = "Cart list" });
                }
                else
                {
                    return BadRequest("No Cart Available Data");
                }

            }
            catch
            {
                throw new Exception("Server Error");
            }


        }

        [Route("api/PatientMedicine/OrderPlacedDetails")]
        [HttpGet]
        public IHttpActionResult OrderPlacedDetails(int PatientId, int id, OrderPlace model)
        {
            try
            {
                double gst = ent.Database.SqlQuery<double>(@"select Amount from GSTMaster where IsDeleted=0 and Name='Medicine'").FirstOrDefault();
                int Medicinedelcherge = ent.Database.SqlQuery<int>(@"select Amount from MedicineDeliveryCharge").FirstOrDefault();
              
                var TotalPrice1 = ent.MedicineCarts.Where(x => x.Patient_Id == PatientId).ToList().Sum(x => Convert.ToInt32(x.TotalPrice));

                var data = (from s in ent.MedicineCarts
                            join r1 in ent.Medicines on s.Medicine_Id equals r1.Id
                            join r in ent.Medicinedelivers on s.Patient_Id equals r.Patient_Id
                            join cm in ent.CityMasters on r.CityMaster_Id equals cm.Id
                            join sm in ent.StateMasters on cm.StateMaster_Id equals sm.Id
                            where r.Patient_Id == PatientId && r.id == id
                            select new
                            {
                                Name = r.Name,
                                MobileNumber = r.MobileNumber,
                                DeliveryAddress = r.DeliveryAddress,
                                CityName = cm.CityName,
                                StateName = sm.StateName,
                                PinCode = r.PinCode,
                                Deliverycharge = Medicinedelcherge,
                                TotalPrice = TotalPrice1,
                                //FinalPrice = TotalPrice1 + 40,
                                GST = gst,
                                FinalPrice = (TotalPrice1 + TotalPrice1* gst/100) + Medicinedelcherge,

                            }).FirstOrDefault();

                return Ok(new { data, Message = "SuccessFully" });

            }
            catch
            {
                throw new Exception("Server Error");
            }
        }


        [Route("api/PatientMedicine/MedicineOrders")]
        [HttpPost]
        public IHttpActionResult MedicineOrders(PlaceOrders model)
        {
            try
            {
                //====GENERATE ORDER NUMBER
                //var lastOrderIdRecord = ent.MedicineOrders.OrderByDescending(a => a.OrderId).FirstOrDefault();
                dynamic lastOrderIdRecord = ent.MedicineOrders.OrderByDescending(a => a.Id).Select(a => a.OrderId).FirstOrDefault();
                string lastOrderId = lastOrderIdRecord != null ? lastOrderIdRecord : "ps_ord_0"; // Default to "ps_inv_0" if no records exist


                string[] OrderIdparts = lastOrderId.Split('_');
                int OrderIdnumericPart = 0;

                if (OrderIdparts.Length == 3 && int.Parse(OrderIdparts[2]) > 0)
                {
                    OrderIdnumericPart = int.Parse(OrderIdparts[2]) + 1; // Increment the numeric part
                }
                else
                {

                    OrderIdnumericPart = 1;
                }

                // Generate the next NextOrderId
                string NextOrderId = $"ps_ord_{OrderIdnumericPart}";
                // Use NextOrderId for your purposes



                //====GENERATE INVOICE NUMBER

                //var lastRecord = ent.MedicineOrders.OrderByDescending(a => a.InvoiceNumber).FirstOrDefault();
                dynamic lastRecord = ent.MedicineOrders.OrderByDescending(a => a.Id).Select(a => a.InvoiceNumber).FirstOrDefault();
                string lastInvoiceNumber = lastRecord != null ? lastRecord : "ps_inv_0"; // Default to "ps_inv_0" if no records exist

              
                string[] parts = lastInvoiceNumber.Split('_');
                int numericPart=0;
                
                if (parts.Length == 3 && int.Parse(parts[2]) > 0)
                {
                    numericPart= int.Parse(parts[2])+1; // Increment the numeric part
                }
                else
                {
                   
                    numericPart = 1;
                }

                // Generate the next invoice number
                string nextInvoiceNumber = $"ps_inv_{numericPart}";
                // Use nextInvoiceNumber for your purposes
               


                var data1 = ent.Medicinedelivers.FirstOrDefault(X => X.id == model.shippingId && X.Patient_Id == model.PatientId);
                var data = ent.MedicineCarts.Where(x => x.Patient_Id == model.PatientId).ToList();
                foreach (var item in data)
                {
                    var MedicineOrder = new MedicineOrder()
                    {
                        OrderDate = DateTime.Now,
                        IsPaid = false,
                        IsDelivered = false,
                        Patient_Id = item.Patient_Id,
                        Name = data1.Name,
                        MobileNumber = data1.MobileNumber,
                        DeliveryAddress = data1.DeliveryAddress,
                        StateMaster_Id = data1.StateMaster_Id,
                        CityMaster_Id = data1.CityMaster_Id,
                        PinCode = data1.PinCode,
                        Location_Id = data1.id,
                        DeliveryStatus = "Pending",
                        DeliveryDate = DateTime.Now.AddDays(10),
                        OrderId = NextOrderId,
                        Remarks = "Test",
                        //InvoiceNumber = "ps_inv_1",
                        InvoiceNumber = nextInvoiceNumber,
                        Chemist_Id = 150
                    };
                    var MedicineOrders = ent.MedicineOrders.Add(MedicineOrder);
                    ent.SaveChanges();
                    var medicineOrderDetail = new MedicineOrderDetail()
                    {
                        Order_Id = MedicineOrders.Id,
                        Medicine_Id = item.Medicine_Id,
                        Quantity = item.Quantity,
                        Amount = item.price * item.Quantity?? 0
                    };
                    ent.MedicineOrderDetails.Add(medicineOrderDetail);

                    ent.SaveChanges();

                }
                //ent.Database.ExecuteSqlCommand("delete MedicineCart where Patient_Id =" + model.PatientId + "");
                return Ok(new { Message = "Order SuccessFully " });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return InternalServerError(ex);
            }
        }


        [Route("api/PatientMedicine/MedicinePayNow")]
        [HttpPost]
        public IHttpActionResult MedicinePayNow(MedicinePayNows model)
        {
            try
            {
                int Medicinedelcherge = ent.Database.SqlQuery<int>(@"select Amount from MedicineDeliveryCharge").FirstOrDefault();
                //====GENERATE ORDER NUMBER
                var lastOrderIdRecord = ent.MedicineOrders.OrderByDescending(a => a.OrderId).FirstOrDefault();
                string lastOrderId = lastOrderIdRecord != null ? lastOrderIdRecord.OrderId : "ps_ord_0"; // Default to "ps_inv_0" if no records exist


                string[] OrderIdparts = lastOrderId.Split('_');
                int OrderIdnumericPart = 0;

                if (OrderIdparts.Length == 3 && int.Parse(OrderIdparts[2]) > 0)
                {
                    OrderIdnumericPart = int.Parse(OrderIdparts[2]) + 1; // Increment the numeric part
                }
                else
                {

                    OrderIdnumericPart = 1;
                }

                // Generate the next NextOrderId
                string NextOrderId = $"ps_ord_{OrderIdnumericPart}";
                // Use NextOrderId for your purposes



                //====GENERATE INVOICE NUMBER

                var lastRecord = ent.MedicineOrders.OrderByDescending(a => a.InvoiceNumber).FirstOrDefault();
                string lastInvoiceNumber = lastRecord != null ? lastRecord.InvoiceNumber : "ps_inv_0"; // Default to "ps_inv_0" if no records exist


                string[] parts = lastInvoiceNumber.Split('_');
                int numericPart = 0;

                if (parts.Length == 3 && int.Parse(parts[2]) > 0)
                {
                    numericPart = int.Parse(parts[2]) + 1; // Increment the numeric part
                }
                else
                {

                    numericPart = 1;
                }

                // Generate the next invoice number
                string nextInvoiceNumber = $"ps_inv_{numericPart}";
                // Use nextInvoiceNumber for your purposes

                var data1 = ent.Medicinedelivers.FirstOrDefault(X => X.id == model.shippingId && X.Patient_Id == model.Patient_Id);
                var datas = ent.MedicineCarts.Where(x => x.Patient_Id == model.Patient_Id).ToList();
                foreach (var item in datas)
                {
                    var MedicineOrder = new MedicineOrder()
                    {
                        OrderDate = DateTime.Now,
                        IsPaid = false,
                        IsDelivered = false,
                        Patient_Id = item.Patient_Id,
                        Name = data1.Name,
                        MobileNumber = data1.MobileNumber,
                        DeliveryAddress = data1.DeliveryAddress,
                        StateMaster_Id = data1.StateMaster_Id,
                        CityMaster_Id = data1.CityMaster_Id,
                        PinCode = data1.PinCode,
                        Location_Id = data1.id,
                        DeliveryStatus = "Pending",
                        DeliveryDate = DateTime.Now.AddDays(10),
                        OrderId = NextOrderId,
                        Remarks = "Test",
                        //InvoiceNumber = "ps_inv_1",
                        InvoiceNumber = nextInvoiceNumber,
                        Chemist_Id = 150
                    };
                    
                    var MedicineOrders = ent.MedicineOrders.Add(MedicineOrder);
                    ent.SaveChanges();
                    var medicineOrderDetail = new MedicineOrderDetail()
                    {
                        Order_Id = MedicineOrders.Id,
                        Medicine_Id = item.Medicine_Id,
                        Quantity = item.Quantity,
                        Amount = item.price * item.Quantity + Medicinedelcherge ?? 0
                    };
                    ent.MedicineOrderDetails.Add(medicineOrderDetail);

                    ent.SaveChanges();
                    //ent.Database.ExecuteSqlCommand("delete MedicineCart where Patient_Id =" + model.Patient_Id + "");
                }
                
                //return Ok(new { Message = "Order SuccessFully " });

                var data = ent.MedicineOrders.Where(a => a.Patient_Id == model.Patient_Id).FirstOrDefault();

                data.OrderDate = DateTime.Now;
                data.IsPaid = model.IsPaid;
                ent.SaveChanges();
                ent.Database.ExecuteSqlCommand("delete MedicineCart where Patient_Id =" + model.Patient_Id + "");
                return Ok("Update MedicinePay ");


                //if (model.IsPaid == true)  //cod online=true
                //{
                //    var data = ent.MedicineOrders.Where(a => a.Patient_Id == model.Patient_Id).FirstOrDefault();

                //    data.OrderDate = DateTime.Now;
                //    data.IsPaid = model.IsPaid;
                //    ent.MedicineOrders.Add(data);
                //    ent.SaveChanges();
                //    return Ok("MedicinePay  Successfully");
                //}
                //else if (model.IsPaid == false)
                //{
                //    var data = ent.MedicineOrders.Where(a => a.Patient_Id== model.Patient_Id).FirstOrDefault();

                //    data.OrderDate = DateTime.Now;
                //    data.IsPaid = model.IsPaid;
                //    ent.SaveChanges();
                //    return Ok("Update MedicinePay ");
                //}
                //return Ok("Please check the detail");

            }
            catch (Exception ex)
            {
                return Ok("Internal server error");
            }
        }

        // medicine invoice 

        [System.Web.Http.HttpGet, Route("api/PatientMedicine/MedicineInvoice/{Invoice}")]
        public IHttpActionResult MedicineInvoice(string Invoice)
        {
            try
            {
                
                var gst = ent.GSTMasters.Where(x => x.IsDeleted == false).FirstOrDefault(x => x.Name == "Medicine");
                var invoiceData = (from mo in ent.MedicineOrders
                                   join p in ent.Patients on mo.Patient_Id equals p.Id
                                   join mod in ent.MedicineOrderDetails on mo.Id equals mod.Order_Id
                                   join m in ent.Medicines on mod.Medicine_Id equals m.Id
                                   where mo.InvoiceNumber == Invoice
                                   select new
                                   {
                                       mo.Patient_Id,
                                       mo.Id,
                                       m.MedicineName,
                                       mod.Quantity,
                                       m.MRP,
                                       GST = gst.Amount,
                                       Amount = m.MRP * (1 + (gst.Amount / 100)) * mod.Quantity,                                  
                                       mo.OrderId,
                                       mo.InvoiceNumber,
                                       mo.OrderDate
                                   }).ToList();

                if (invoiceData.Count > 0)
                {
                    int Medicinedelcherge = ent.Database.SqlQuery<int>(@"select Amount from MedicineDeliveryCharge").FirstOrDefault();
                    double? totalAmount = invoiceData.Sum(item => item.MRP * item.Quantity);
                    double? grandTotal = invoiceData.Sum(item => item.MRP * item.Quantity)+ ((double)(totalAmount * gst.Amount) / 100 ) + Medicinedelcherge;
                   
                    double gstAmount = (double)(totalAmount * gst.Amount) / 100; // Calculate the GST amount
                    //double? finalAmount = grandTotal - gstAmount;


                    int patientId = invoiceData[0].Patient_Id;

                    var patientData = ent.Patients.FirstOrDefault(x => x.Id == patientId);

                    if (patientData != null)
                    {
                        return Ok(new
                        {
                            InvoiceData = invoiceData,
                            Name = patientData.PatientName,
                            Email = patientData.EmailId,
                            PinCode = patientData.PinCode,
                            MobileNo = patientData.MobileNumber,
                            Address = patientData.Location,
                            InvoiceNumber = Invoice,
                            OrderId = invoiceData[0].OrderId,
                            OrderDate = invoiceData[0].OrderDate,
                            GST = invoiceData[0].GST,
                            GSTAmount = gstAmount,
                            GrandTotal = grandTotal,
                            FinalAmount = totalAmount,
                            DeliveryCharge = Medicinedelcherge,
                            Status = 200,
                            Message = "Invoice"
                        });
                    }
                    else
                    {
                        return BadRequest("Patient data not found");
                    }
                }
                else
                {
                    return BadRequest("No Invoice data found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Server Error");
            }

        }

        
    }
}