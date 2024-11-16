using AutoMapper;
using Common.Logging;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using HospitalPortal.BL;
using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Repositories;
using HospitalPortal.Utilities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Web.Http; 
using static HospitalPortal.Controllers.TestApiController;
using static HospitalPortal.Models.ViewModels.ChemistDTO;
using static HospitalPortal.Models.ViewModels.VendorDTO;
using static HospitalPortal.Utilities.EmailOperations;
using Gallery = HospitalPortal.Models.DomainModels.Gallery;
using ReturnMessage = HospitalPortal.Models.APIModels.ReturnMessage;

namespace HospitalPortal.Controllers
{
    public class FranchisesApiController : ApiController
    {
        DbEntities ent = new DbEntities();
        returnMessage rm = new returnMessage();
        GenerateBookingId bk = new GenerateBookingId();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(PatientApiController));

        //===========================EditProfile==========================//

        [HttpPost]
        [Route("api/FranchisesApi/EditProfile")]
        public IHttpActionResult EditProfile(Fra_EditProfile model)
        {
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            
            try
            { 
                if (model.AadharOrPANImage == null)
                {
                    rm.Message = "AadharOrPANImage Image File Picture can not be null";
                    
                    return Ok(rm);
                }
                var img = FileOperation.UploadFileWithBase64("Images", model.AadharOrPANImage, model.AadharOrPANImagebase64, allowedExtensions);
                if (img == "not allowed")
                {
                    rm.Message = "Only png,jpg,jpeg,pdf files are allowed.";
                    
                    return Ok(rm);
                }
                model.AadharOrPANImage = img;

                var data = ent.Vendors.Find(model.Id);
                if (data == null)
                {
                    rm.Status = 0;
                    rm.Message = "no data found to updata";
                    return Ok(rm);
                }

                data.CompanyName = model.VendorName;
                data.MobileNumber = model.MobileNumber;
                data.EmailId = model.EmailId;
                data.StateMaster_Id = model.StateMaster_Id;
                data.City_Id = model.City_Id;
                data.Location = model.Location;
                data.GSTNumber = model.GSTNumber;
                data.AadharOrPANNumber = model.AadharOrPANNumber;
                data.AadharOrPANImage = model.AadharOrPANImage;
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = " Franchise profile updated Successfully .";
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
            
        }

        //===========================ADD GALLERY==========================//

        [HttpPost]
        [Route("api/FranchisesApi/Add_Gallery")]
        public IHttpActionResult Add_Gallery(Fran_AddGallery model)
        {
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {

                    if (model.Images == null)
                    {
                        rm.Message = "Image can not be null";
                        tran.Rollback();
                        return Ok(rm);
                    }
                    var img = FileOperation.UploadFileWithBase64("Images", model.Images, model.Imagesbase64, allowedExtensions);
                    if (img == "not allowed")
                    {
                        rm.Message = "Only png,jpg,jpeg,pdf files are allowed.";
                        tran.Rollback();
                        return Ok(rm);
                    }
                    model.Images = img;
                    //var data = ent.Vendors.Where(a => a.Id == model.Franchise_Id).FirstOrDefault();
                    var domainModel = new Gallery();
                    domainModel.ImageName = model.ImageName;
                    domainModel.Franchise_Id = model.Franchise_Id;
                    domainModel.Images = model.Images;
                    domainModel.IsDeleted = false;
                    ent.Galleries.Add(domainModel);
                    ent.SaveChanges();
                    tran.Commit();
                    rm.Status = 1;
                    rm.Message = "Image Add Successfully.";
                    return Ok(rm);
                }
                catch (Exception ex)
                {

                    log.Error(ex.Message);
                    tran.Rollback();
                    return InternalServerError(ex);
                }

            }

        }

        //===========================GET GALLERY==========================//

        [HttpGet]
        [Route("api/FranchisesApi/Get_Gallery")]

        public IHttpActionResult Get_Gallery(int Id)
        {            
            string qry = @"Select Id,ImageName,Images From Gallery Where Franchise_Id="+ Id + " And IsDeleted=0 order by Id desc";
            var Gallery = ent.Database.SqlQuery<GetGallery>(qry).ToList();
            if (Gallery.Count() == 0)
            {
                return BadRequest ("No Record");
                 
            }
            return Ok(new { Gallery });
        }

        //===============DeleteGallery======================//

        [HttpPost]
        [Route("api/FranchisesApi/DeleteGallery")]

        public IHttpActionResult DeleteGallery(int Id)
        {
            var data = ent.Galleries.Find(Id);
            try
            {
                data.IsDeleted = true;
                ent.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return Ok(new { Message = "Record Deleted Successfully!!!" });
        }



        //===============Get TestList==================//

        [HttpGet]
        [Route("api/FranchisesApi/Test_List")]

        public IHttpActionResult Test_List()
        {
            string qry = @"select Id,TestName from LabTest where TestName != 'null'";
            var TestList = ent.Database.SqlQuery<GetTestList>(qry).ToList();
            return Ok(new { TestList });
        }

        //===========================EditTestList==========================//

        [HttpPost]
        [Route("api/FranchisesApi/EditTestList")]
        public IHttpActionResult EditTestList(Edit_TestList model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ",
ModelState.Values
.SelectMany(a => a.Errors)
.Select(a => a.ErrorMessage));
                    rm.Message = message;
                    rm.Status = 0;
                    return Ok(rm);
                }
                var data = ent.LabTests.Find(model.Id);
                if (data == null)
                {
                    rm.Status = 0;
                    rm.Message = "no data found to updata";
                    return Ok(rm);
                }
                data.TestName = model.TestName;
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = " Test Name Updated Successfully .";
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }

        //===============DeleteTestList======================//

        [HttpPost]
        [Route("api/FranchisesApi/DeleteTestList")]

        public IHttpActionResult DeleteTestList(int Id)
        {
            var data = ent.LabTests.Find(Id);
            try
            {
                ent.SaveChanges();
                string dltqry = @"delete from LabTest where Id=" + Id;
                ent.Database.ExecuteSqlCommand(dltqry);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return Ok(new { Message = "Record Deleted Successfully!!!" });
        }
         
        //========================FRANCHISE EDIT PROFILE=================//

        [HttpPost]
        [Route("api/FranchisesApi/Fra_EditProfile")]
        public IHttpActionResult Fra_EditProfile(FranchiseEditProfile model)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ",
ModelState.Values
.SelectMany(a => a.Errors)
.Select(a => a.ErrorMessage));
                    rm.Message = message;
                    rm.Status = 0;
                    return Ok(rm);
                }

                var data = ent.Vendors.Find(model.Id);
                if (data == null)
                {
                    rm.Status = 0;
                    rm.Message = "no data found to updata";
                    return Ok(rm);
                }


                data.VendorName = model.VendorName;
                data.CompanyName = model.CompanyName;
                data.MobileNumber = model.MobileNumber;
                data.EmailId = model.EmailId;
                data.StateMaster_Id = model.StateMaster_Id;
                data.City_Id = model.City_Id;
                data.Location = model.Location;
                data.PinCode = model.PinCode;
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = "Franchise Profile Updated Successfully .";
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);

        }


        //=============================ProfileDetail===============================//
         
        [HttpGet]
        [Route("api/FranchisesApi/Fra_ProfileDetail")]

        public IHttpActionResult Fra_ProfileDetail(int Id)
        {
            string qry = @"select V.Id,V.UniqueId,V.VendorName,V.EmailId,V.MobileNumber,V.Location,sm.StateName,cm.cityname,V.PinCode,V.CompanyName,V.GSTNumber,V.AadharOrPANImage,V.AadharOrPANNumber,V.StateMaster_Id,V.City_ID from Vendor as V with(nolock) left join citymaster as cm with(nolock) on cm.id=V.City_Id left join statemaster as sm with(nolock) on sm.id=V.StateMaster_Id where V.Id=" + Id + "";
            var FrancjiseProfile = ent.Database.SqlQuery<Fra_ProDetail>(qry).FirstOrDefault();
            return Ok(FrancjiseProfile);

        }

        //========================PAYOUT HISTORY==================//

        [HttpGet]
        [Route("api/FranchisesApi/Fra_PayoutHistory")]

        public IHttpActionResult Fra_PayoutHistory(int Id)
        {
            string qry = @"select VP.Id,V.VendorName,VP.Amount,V.Location,VP.PaymentDate from VendorPayOut as VP left join Vendor as V on V.Id=VP.Vendor_Id where VP.Vendor_Id="+ Id +"";
            var PayoutHistory = ent.Database.SqlQuery<Fra_payout_his>(qry).ToList();
            return Ok(new { PayoutHistory });

        }

        //========================FRANCHISE DEPARTMENT DROPDOWN==================//

        [HttpGet]
        [Route("api/FranchisesApi/Fra_DepartmentDropdown")]

        public IHttpActionResult Fra_DepartmentDropdown()
        {
            //string qry = @"select Id,DepartmentName from Franchise_Department where IsDeleted='false'";
            //var DepartmentName = ent.Database.SqlQuery<Departmentdropdown>(qry).ToList();
            //return Ok(new { DepartmentName });

            dynamic obj = new ExpandoObject();
            obj.Department = Mapper.Map<IEnumerable<DepartmentDTO>>(ent.Departments.Where(a => a.IsDeleted == false).OrderBy(a => a.DepartmentName).ToList());
            return Ok(obj);
        }

        //========================FRANCHISE SPECIALIST DROPDOWN By_DepID==================//

        [HttpGet]
        [Route("api/FranchisesApi/Fra_SpecialistDropdown_By_DepID")]

        public IHttpActionResult Fra_SpecialistDropdown_By_DepID(int dep_Id)
        {
            //            string qry = @"select FS.Id,FS.SpecialistName from Franchise_Specialist as FS
            //inner join Franchise_Department as FD on FD.Id=FS.Department_Id 
            //where fs.Department_Id=" + dep_Id + " and fs.IsDeleted=0";
            //            var SpecialistName = ent.Database.SqlQuery<Specialistdropdown>(qry).ToList();
            //            return Ok(new { SpecialistName });

            dynamic obj = new ExpandoObject();
            obj.Specialist = Mapper.Map<IEnumerable<SpecialistDTO>>(ent.Specialists.Where(a => a.Department_Id == dep_Id && a.IsDeleted == false).OrderBy(a => a.SpecialistName).ToList());
            return Ok(obj);
        }


        //==============Add Department=========================//

        [HttpPost]
        [Route("api/FranchisesApi/AddDepartment")]
        public IHttpActionResult AddDepartment(Add_dep model)
        {
            try
            {
                if (ent.AddFranchiseDepartments.Any(a => a.Spec_Id == model.Spec_Id && a.Admin_LoginId == model.AdminLogin_Id))
                {
                    return BadRequest("This Department has already Added.");
                }
                var data = new AddFranchiseDepartment()
                {
                    Dep_Id = model.Dep_Id,
                    Spec_Id = model.Spec_Id,
                    Status = "Pending",
                    Admin_LoginId = model.AdminLogin_Id,
                    

                };
                ent.AddFranchiseDepartments.Add(data);
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = "Department Add Successfully";

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }

        //==============Dept & spec List=========================//

        [HttpGet]
        [Route("api/FranchisesApi/Dept_spec_List")]

        public IHttpActionResult Dept_spec_List(int Id)
        {
            string qry = @"select AFD.id,FD.Departmentname,FS.SpecialistName,AFD.Status from AddFranchiseDepartment as AFD left join Department as FD on FD.Id=AFD.Dep_Id left join Specialist as FS on FS.Id=AFD.Spec_Id left join Vendor as V on v.Id=AFD.Admin_LoginId where AFD.Admin_LoginId="+Id+" order by AFD.Id desc";
            var DeptspecList = ent.Database.SqlQuery<DeptAndSpec_List>(qry).ToList();
            return Ok(new { DeptspecList });
        }

        //===============Get DepartmentList(view dept & spec)==================//

        [HttpGet]
        [Route("api/FranchisesApi/DepartmentList")]

        public IHttpActionResult DepartmentList()
        {
            string qry = @"select Id,Departmentname From Department where IsDeleted=0 order by Id desc";
            var DeptList = ent.Database.SqlQuery<Department_List>(qry).ToList();
            return Ok(new { DeptList });
        }

        //===========================EditDepartmentList==========================//

        [HttpPost]
        [Route("api/FranchisesApi/EditDepartment")]

        public IHttpActionResult EditDepartment(Edit_Department model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ",
ModelState.Values
.SelectMany(a => a.Errors)
.Select(a => a.ErrorMessage));
                    rm.Message = message;
                    rm.Status = 0;
                    return Ok(rm);
                }
                var data = ent.Departments.Find(model.Id);
                if (data == null)
                {
                    rm.Status = 0;
                    rm.Message = "no data found to updata";
                    return Ok(rm);
                }
                data.DepartmentName = model.DepartmentName;
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = " Department Name Updated Successfully .";
            }

            catch (Exception ex)
            {
                string msg = ex.ToString();
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }

        //===============DeleteDepartment======================//

        [HttpPost]
        [Route("api/FranchisesApi/DeleteDepartment")]

        public IHttpActionResult DeleteDepartment(int Id)
        {
            var data = ent.Departments.Find(Id);
            try
            {
                data.IsDeleted = true;
                ent.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return Ok(new { Message = "Record Deleted Successfully!!!" });
        }

        //==============Add Department By List(only department)=========================//

        [HttpPost]
        [Route("api/FranchisesApi/AddDepartment_ByList")]
        public IHttpActionResult AddDepartment_ByList(Add_dep_ByList model)
        {
            try
            {

                if (ent.Departments.Any(a => a.DepartmentName == model.DepartmentName))
                {
                    rm.Message = "This Department has already exists.";
                    rm.Status = 0;
                    return Ok(rm);
                }
                var data = new Department()
                {
                    DepartmentName = model.DepartmentName,
                    IsDeleted = false

                };
                ent.Departments.Add(data);
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = "New Department Add Successfully";

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }

        //=====================Get PaymentHistory===================//

        [HttpGet, Route("api/FranchisesApi/Fra_PaymentHistory")]

        public IHttpActionResult Fra_PaymentHistory()
        {
            string qry = @"select v.Id,V.VendorName,v.Location,VP.Amount from Vendor as V
Inner join VendorPayOut as VP on VP.Vendor_Id=V.Id";
            var PaymentHistory = ent.Database.SqlQuery<FraPaymentHis>(qry).ToList();
            return Ok(new { PaymentHistory });
        }

        //=====================GetRole For PaymentHistory dropdown===================//

        [HttpGet, Route("api/FranchisesApi/GetRole")]

        public IHttpActionResult GetRole()
        {
            string query = @"select * from franchiseRole";
            var Role = ent.Database.SqlQuery<GetRole_dropdown>(query).ToList();
            return Ok(new { Role });
        }


        //==============PaymentHistories by role===================//

        [HttpGet, Route("api/FranchisesApi/PaymentHistories_ByRole")]
        public IHttpActionResult PaymentHistories_ByRole(string Role,int?VendorId)
        {
            if (Role == "Chemist")//chemist
            {
                string qry = @"select Ch.Id,Ch.ChemistName as Name,CP.Amount as PaidFees,CP.Id as PaymentId,Ch.Location,CONVERT(VARCHAR(11), CP.PaymentDate, 103) AS PaymentDate,FORMAT(CP.PaymentDate, 'hh:mm') AS PaymentTime from ChemistPayOut as CP left join Chemist as Ch on CP.Chemist_Id=ch.Id";
                var PaymentHistory = ent.Database.SqlQuery<Payment_History>(qry).ToList();
                return Ok(new { PaymentHistory });

            }
            else if (Role == "Nurse")//Nurse
            {
                string qry = @"select P.Id,p.NurseId as UniqueId,P.NurseName as Name,sm.StateName+','+cm.CityName+','+P.Location as Location,SUM(ns.TotalFee) as PaidFees from Nurse P 
join NurseService ns on ns.Nurse_Id = P.Id 
left join StateMaster as sm on sm.Id=p.StateMaster_Id
left join CityMaster as cm on cm.Id=p.CityMaster_Id
join Vendor as v on v.Id=p.Vendor_Id
where ns.ServiceAcceptanceDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() and ns.IsPaid=1 and v.Id="+VendorId+" group by  P.NurseName,P.Id,p.NurseId,sm.StateName,cm.CityName,p.Location";
                var PaymentHistory = ent.Database.SqlQuery<Payment_History>(qry).ToList();
                return Ok(new { PaymentHistory });
            } 
            else if (Role == "Lab")//lab
            {
                string query = @"select l.Id, l.LabName as Name,l.lABId as UniqueId,sm.StateName+','+cm.CityName+','+l.Location as Location,SUM(btl.Amount) as PaidFees from BookTestLab btl
join Lab l ON l.Id = btl.Lab_Id 
left join StateMaster as sm on sm.Id=l.StateMaster_Id
left join CityMaster as cm on cm.Id=l.CityMaster_Id
join Vendor as v on v.Id=l.Vendor_Id
where btl.TestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() and  btl.IsPaid=1 and v.Id"+VendorId+" group by l.Id,l.LabName,l.lABId,sm.StateName,cm.CityName,l.Location";
                var PaymentHistory = ent.Database.SqlQuery<Payment_History>(query).ToList();
                return Ok(new { PaymentHistory });
            }
            else if (Role == "Doctor")//doctor
            {
                string query = @"select D.Id,D.DoctorId as UniqueId,D.DoctorName as Name,sm.StateName+','+cm.CityName+','+d.Location as Location,SUM(P.TotalFee) as PaidFees from dbo.PatientAppointment P 
join Doctor D ON d.Id = p.Doctor_Id 
join Vendor as v on v.Id=D.Vendor_Id
left join StateMaster as sm on sm.Id=D.StateMaster_Id
left join CityMaster as cm on cm.Id=d.CityMaster_Id
where p.IsPaid=1 and  P.AppointmentDate between DateAdd(DD,-7,GETDATE() ) and GETDATE() and v.Id=" + VendorId + " group by d.Id,d.DoctorName,D.DoctorId,sm.StateName,cm.CityName,d.Location";
                var PaymentHistory = ent.Database.SqlQuery<Payment_History>(query).ToList();
                return Ok(new { PaymentHistory });
            }
            else if (Role == "Ambulance")//Ambulance
            {
                string query = @"SELECT distinct v.Id AS VehicleId,v.VehicleNumber,d.DriverId,d.DriverName,ISNULL(v.VehicleName, 'NA') AS Name,sm.StateName+','+cm.CityName+','+d.Location as Location FROM DriverLocation trm
JOIN Driver d ON d.Id = trm.Driver_Id
JOIN Vehicle v ON v.Id = d.Vehicle_Id
join Vendor as ve on v.Id=d.Vendor_Id
left join StateMaster as sm on sm.Id=d.StateMaster_Id
left join CityMaster as cm on cm.Id=d.CityMaster_Id
WHERE trm.IsPay = 'Y' AND trm.EntryDate BETWEEN DATEADD(DD, -7, GETDATE()) AND GETDATE() and ve.Id="+VendorId+" group by v.Id,v.VehicleNumber, d.DriverName,trm.Id,v.VehicleName,d.DriverId,sm.StateName,cm.CityName,d.Location";
                var PaymentHistory = ent.Database.SqlQuery<Payment_History>(query).ToList();
                return Ok(new { PaymentHistory });
            }
            else
            {
                rm.Message = "Record Not Found!!!";
            }
            return Ok(rm);
        }


        //===============VEHICLE CATEGORY DROPDOWN(ADD VEHICLE TYPE)============//
        [HttpGet]
        [Route("api/FranchisesApi/Fra_vehicleCat_dropdown")]

        public IHttpActionResult Fra_vehicleCat_dropdown()
        { 
            string qry = @"SELECT Id, 
       CONCAT('[', Type, '] ', CategoryName) AS CategoryName 
FROM MainCategory 
WHERE IsDeleted = 0 
ORDER BY CategoryName;";
            var VehicleCatDropdown = ent.Database.SqlQuery<Vehiclecat>(qry).ToList();
            return Ok(new { VehicleCatDropdown });
        }

        //===============VEHICLE TYPE DROPDOWN BY CATEGOTY============//
        [HttpGet]
        [Route("api/FranchisesApi/Fra_VehicleType_By_cat")]


        public IHttpActionResult Fra_VehicleType_By_cat(int Id)
        {
            string qry = @"select vt.id,vt.VehicleTypeName from  VehicleType as vt with(nolock)
 Left join MainCategory as mcy with(nolock) on mcy.id=vt.Category_Id
 where vt.Category_Id=" + Id + " AND Vt.IsDeleted=0 and vt.IsApproved=1";
            var VehicleTypeName = ent.Database.SqlQuery<VehicleType_Name>(qry).ToList();
            return Ok(new { VehicleTypeName });
        }


        [HttpPost, Route("api/FranchisesApi/AddVehicle_type")]
        public IHttpActionResult AddVehicle_type(AddVehicletype model)
        {
            try
            {
                if (ent.FraAddVehicleTypes.Any(a => a.VehicleType_Id == model.VehicleType_Id && a.Admin_LoginId == model.AdminLogin_Id))
                {
                    return BadRequest("This Vehicle has already Added.");
                }
                var data = new FraAddVehicleType()
                {
                    Category_Id = model.Category_Id,
                    VehicleType_Id = model.VehicleType_Id,
                    Status = "Yet to be Approved",
                    Admin_LoginId = model.AdminLogin_Id,

                };
                ent.FraAddVehicleTypes.Add(data);
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = "VehicleType Add Successfully";

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        } 

        //===================View Vehicle Type List================//

        [HttpGet, Route("api/FranchisesApi/AddedVehicleList")]
        public IHttpActionResult AddedVehicleList(int AdminLoginId)
        {
            string qry = @"select AVT.Id,MCY.CategoryName,VT.VehicleTypeName,AVT.Status  from FraAddVehicleType as  AVT left join MainCategory as MCY on MCY.Id=AVT.Category_Id left join VehicleType as VT on VT.Id=AVT.VehicleType_Id where Admin_LoginId="+ AdminLoginId + " order by AVT.Id desc";
            var VehicleList = ent.Database.SqlQuery<Vehicle_List>(qry).ToList();
            return Ok(new { VehicleList });
        }

        //=======================FRANCHISE CHEMIST REGISTRATION==================//

        [HttpPost]
        [Route("api/FranchisesApi/Fra_ChemistReg")]

        public IHttpActionResult Fra_ChemistReg(fra_Chemistregistration model)
        {
            var rm = new ReturnMessage();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            
            try
            {
                if (ent.AdminLogins.Any(a => a.Username == model.EmailId))
                {
                    rm.Message = "This EmailId has already exists.";
                    rm.Status = 0;
                    return Ok(rm);
                }
                if (ent.AdminLogins.Any(a => a.PhoneNumber == model.MobileNumber))
                {
                    rm.Status = 0;
                    rm.Message = "This Mobile Number has already exists.";
                    return Ok(rm);
                }

                if (ent.Chemists.Any(L => L.ChemistName == model.ChemistName && L.MobileNumber == model.MobileNumber))
                {
                    var data = ent.Chemists.Where(L => L.ChemistName == model.ChemistName && L.MobileNumber == model.MobileNumber).FirstOrDefault();
                    var logdata = ent.AdminLogins.Where(L => L.UserID == data.ChemistId).FirstOrDefault();
                    string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                    Message.SendSms(logdata.PhoneNumber, mssg);
                    rm.Message = "you are already Lab  registered with pswellness";
                    rm.Status = 0;
                    return Ok(rm);
                }

                var admin = new AdminLogin
                {
                    Username = model.EmailId,
                    PhoneNumber = model.MobileNumber,
                    Password = model.Password,
                    Role = "chemist"
                };
                ent.AdminLogins.Add(admin);
                ent.SaveChanges();

                var img = FileOperation.UploadFileWithBase64("Images", model.Certificateimg, model.Certificateimgbase64, allowedExtensions);
                if (img == "not allowed")
                {
                    rm.Message = "Only png,jpg,jpeg files are allowed.";
                    
                    return Ok(rm);
                }
                model.Certificateimg = img;


                var domainModel = new Chemist();
                {
                    domainModel.ChemistName = model.ChemistName;
                    domainModel.ShopName = model.ShopName;
                    domainModel.EmailId = model.EmailId;
                    domainModel.Password = model.Password;
                    domainModel.ConfirmPassword = model.ConfirmPassword;
                    domainModel.MobileNumber = model.MobileNumber;
                    domainModel.Location = model.Location;
                    domainModel.StateMaster_Id = model.StateMaster_Id;
                    domainModel.CityMaster_Id = model.CityMaster_Id;
                    domainModel.LicenceNumber = model.LicenceNumber;
                    domainModel.Certificateimg = model.Certificateimg;
                    domainModel.LicenseValidity = model.LicenseValidity;
                    domainModel.PAN = model.PAN;
                    domainModel.PinCode = model.PinCode;
                    domainModel.Vendor_Id = model.Vendor_Id;
                    domainModel.JoiningDate = DateTime.Now;
                    domainModel.AdminLogin_Id = admin.Id;
                    domainModel.ChemistId = bk.GenerateChemistId();
                    domainModel.IsBankUpdateApproved = false;
                    admin.UserID = domainModel.ChemistId;
                };
                ent.Chemists.Add(domainModel);
                ent.SaveChanges();

                string msg = @"<!DOCTYPE html>
<html>
<head>
    <title>PS Wellness Registration Confirmation</title>
</head>
<body>
    <h1>Welcome to PS Wellness!</h1>
    <p>Your signup is complete. To finalize your registration, please use the following login credentials:</p>
    <ul>
        <li><strong>User ID:</strong> " + admin.UserID + @"</li>
        <li><strong>Password:</strong> " + admin.Password + @"</li>
    </ul>
    <p>Thank you for choosing PS Wellness. We look forward to assisting you on your wellness journey.</p>
</body>
</html>";

                
                EmailEF ef = new EmailEF()
                {
                    EmailAddress = model.EmailId,
                    Message = msg,
                    Subject = "PS Wellness Registration Confirmation"
                };

                EmailOperations.SendEmainew(ef);

                string msg1 = "Welcome to PSWELLNESS. Your User Name :  " + domainModel.ChemistName + "(" + domainModel.ChemistId + "), Password : " + admin.Password + ".";
                Message.SendSmsUserIdPass(model.MobileNumber, model.ChemistName, domainModel.ChemistId, model.Password);


                rm.Message = "Welcome to PS Wellness. Sign up process completed. Approval pending.";
                rm.Status = 1;
                return Ok(rm);

            }

            catch (Exception ex)
            {
                log.Error(ex.Message); 
                return InternalServerError(ex);
            } 
        }

        //======================FRANCHISE DOCTOR REGISTRATION==========================//

        [HttpPost, Route("api/FranchisesApi/fra_DoctorRegistration")]
        public IHttpActionResult Fra_DoctorRegistration(fra_DoctorReg model)
        {
            var rm = new ReturnMessage();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    
                    if (ent.Doctors.Any(a => a.DoctorName == model.DoctorName && a.MobileNumber == model.MobileNumber))
                    {
                        var data = ent.Doctors.Where(a => a.DoctorName == model.DoctorName && a.MobileNumber == model.MobileNumber).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.DoctorId).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        rm.Message = "you are already registered with pswellness";
                        rm.Status = 0;
                        return Ok(rm);
                    }
                    var UniqeIdDoc = bk.GenerateDoctorId();
                    var admin = new AdminLogin
                    {
                        Username = model.EmailId,
                        PhoneNumber = model.MobileNumber,
                        Password = model.Password,
                        UserID = UniqeIdDoc,
                        Role = "doctor"
                    };
                    ent.AdminLogins.Add(admin);
                    ent.SaveChanges();

                    // Licence upload
                    var licenceImg = FileOperation.UploadFileWithBase64("Images", model.LicenceImage, model.LicenceBase64, allowedExtensions);
                    if (licenceImg == "not allowed")
                    {
                        rm.Status = 0;
                        rm.Message = "Only png,jpg,jpeg files are allowed as Licence document";
                        tran.Rollback();
                        return Ok(rm);
                    }
                    model.LicenceImage = licenceImg;

                    // upload Pan
                    var panImg = FileOperation.UploadFileWithBase64("Images", model.PanImage, model.PanImageBase64, allowedExtensions);
                    if (panImg == "not allowed")
                    {
                        rm.Status = 0;
                        rm.Message = "Only png,jpg,jpeg files are allowed as Licence document";
                        tran.Rollback();
                        return Ok(rm);
                    }
                    model.PanImage = panImg;

                    // Signature upload
                    var SignatureImg = FileOperation.UploadFileWithBase64("Images", model.SignaturePic, model.SignaturePicBase64, allowedExtensions);
                    if (SignatureImg == "not allowed")
                    {
                        rm.Status = 0;
                        rm.Message = "Only png,jpg,jpeg files are allowed as Licence document";
                        tran.Rollback();
                        return Ok(rm);
                    }
                    model.SignaturePic = SignatureImg;

                    //var domainModel = Mapper.Map<Doctor>(model);
                    var domainModel = new Doctor();
                    domainModel.DoctorName = model.DoctorName;
                    domainModel.EmailId = model.EmailId;
                    domainModel.Password = model.Password;
                    domainModel.MobileNumber = model.MobileNumber;
                    domainModel.PhoneNumber = model.PhoneNumber;
                    domainModel.ClinicName = model.ClinicName;
                    domainModel.StateMaster_Id = model.StateMaster_Id;
                    domainModel.CityMaster_Id = model.CityMaster_Id;
                    domainModel.Location = model.Location;
                    domainModel.LicenceImage = model.LicenceImage;
                    domainModel.LicenceNumber = model.LicenceNumber;
                    domainModel.LicenseValidity = model.LicenseValidity;
                    domainModel.PinCode = model.PinCode;
                    domainModel.Vendor_Id = model.Vendor_Id;
                    domainModel.PanImage = model.PanImage;
                    domainModel.AdminLogin_Id = admin.Id;
                    domainModel.SlotTime = Convert.ToInt32(model.SlotTime);
                    domainModel.EndTime = model.EndTime;
                    domainModel.StartTime = model.StartTime;
                    domainModel.SlotTime2 = Convert.ToInt32(model.SlotTime2);
                    domainModel.EndTime2 = model.EndTime2;
                    domainModel.StartTime2 = model.StartTime2;
                    domainModel.Experience = model.Experience;
                    domainModel.Department_Id = model.Department_Id;
                    domainModel.Specialist_Id = model.Specialist_Id;
                    domainModel.PAN = model.PAN;
                    domainModel.Fee = model.Fee;
                    domainModel.RegistrationNumber = model.RegistrationNumber;
                    domainModel.Qualification = model.Qualification;
                    domainModel.SignaturePic = model.SignaturePic;
                    domainModel.JoiningDate = DateTime.Now;
                    domainModel.DoctorId = UniqeIdDoc;
                    domainModel.About = model.About;
                    domainModel.Day_Id = model.Day_Id;
                    domainModel.VirtualFee = model.VirtualFee;
                    domainModel.IsBankUpdateApproved = false;
                    ent.Doctors.Add(domainModel);
                    ent.SaveChanges();

                    string msg = @"<!DOCTYPE html>
<html>
<head>
    <title>PS Wellness Registration Confirmation</title>
</head>
<body>
    <h1>Welcome to PS Wellness!</h1>
    <p>Your signup is complete. To finalize your registration, please use the following login credentials:</p>
    <ul>
        <li><strong>User ID:</strong> " + admin.UserID + @"</li>
        <li><strong>Password:</strong> " + admin.Password + @"</li>
    </ul>
    <p>Thank you for choosing PS Wellness. We look forward to assisting you on your wellness journey.</p>
</body>
</html>";

                    // string msg1 = "Welcome to PS Wellness. Your signup is complete. To finalize your registration please proceed to log in using the credentials you provided during the signup process. Your User Id: " + admin.UserID + ", Password: " + admin.Password + ".";

                    EmailEF ef = new EmailEF()
                    {
                        EmailAddress = model.EmailId,
                        Message = msg,
                        Subject = "PS Wellness Registration Confirmation"
                    };

                    EmailOperations.SendEmainew(ef);

                    string msg1 = "Welcome to PSWELLNESS. Your User Name :  " + admin.Username + "(" + domainModel.DoctorId + "), Password : " + admin.Password + ".";
                    Message.SendSmsUserIdPass(model.MobileNumber, model.DoctorName, UniqeIdDoc, model.Password);
                    tran.Commit();
                    rm.Status = 1;
                    rm.Message = "Welcome to PS Wellness. Sign up process completed. Approval pending.";
                    return Ok(rm);
                }

                catch (Exception ex)
                {
                    if (ex.Message == "Invalid length for a Base-64 char array or string.")
                    {
                        rm.Message = "Invalid Attempt of Image";
                        rm.Status = 0;
                        tran.Rollback();
                        return Ok(rm);
                    }
                    log.Error(ex.Message);
                    tran.Rollback();
                    return InternalServerError(ex);
                }
            }
        }


        //======================FRANCHISE LAB REGISTRATION==========================//
        [HttpPost, Route("api/FranchisesApi/Fra_LabRegistration")]
        public IHttpActionResult Fra_LabRegistration(fra_LabReg model)
        {
            var rm = new ReturnMessage();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            try
            {
                if (ent.AdminLogins.Any(a => a.Username == model.EmailId))
                {
                    rm.Message = "This EmailId has already exists.";
                    rm.Status = 0;
                    return Ok(rm);
                }
                if (ent.AdminLogins.Any(a => a.PhoneNumber == model.PhoneNumber))
                {
                    rm.Status = 0;
                    rm.Message = "This Mobile Number has already exists.";
                    return Ok(rm);
                }

                if (ent.Labs.Any(L => L.LabName == model.LabName && L.PhoneNumber == model.PhoneNumber))
                {
                    var data = ent.Labs.Where(L => L.LabName == model.LabName && L.PhoneNumber == model.PhoneNumber).FirstOrDefault();
                    var logdata = ent.AdminLogins.Where(L => L.UserID == data.lABId).FirstOrDefault();
                    string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                    Message.SendSms(logdata.PhoneNumber, mssg);
                    rm.Message = "you are already Lab  registered with pswellness";
                    rm.Status = 0;
                    return Ok(rm);
                }

                var admin = new AdminLogin
                {
                    Username = model.EmailId,
                    PhoneNumber = model.PhoneNumber,
                    Password = model.Password,
                    Role = "lab"
                };
                ent.AdminLogins.Add(admin);
                ent.SaveChanges();
                var img = FileOperation.UploadFileWithBase64("Images", model.LicenceImage, model.LicenceImagebase64, allowedExtensions);
                if (img == "not allowed")
                {
                    rm.Message = "Only png,jpg,jpeg files are allowed.";
                    //tran.Rollback();
                    return Ok(rm);
                }
                model.LicenceImage = img;
                var PanImg = FileOperation.UploadFileWithBase64("Images", model.PanImage, model.PanImagebase64, allowedExtensions);
                if (PanImg == "not allowed")
                {
                    rm.Message = "Only png,jpg,jpeg files are allowed as Aadhar card document";
                    // tran.Rollback();
                    return Ok(rm);
                }
                model.PanImage = PanImg;


                var domainModel = new Lab();
                {
                    // var domainModel = Mapper.Map<Lab>(model);
                    domainModel.LabName = model.LabName;
                    domainModel.EmailId = model.EmailId;
                    domainModel.Password = model.Password;
                    domainModel.PhoneNumber = model.PhoneNumber;
                    domainModel.MobileNumber = model.PhoneNumber; //same no.
                    domainModel.Location = model.Location;
                    domainModel.StateMaster_Id = model.StateMaster_Id;
                    domainModel.CityMaster_Id = model.CityMaster_Id;
                    domainModel.LicenceNumber = model.LicenceNumber;
                    domainModel.LicenceImage = model.LicenceImage;
                    domainModel.PanImage = model.PanImage;
                    domainModel.StartTime = model.StartTime;
                    domainModel.EndTime = model.EndTime;
                    domainModel.GSTNumber = model.GSTNumber;
                    domainModel.AadharNumber = model.AadharNumber;
                    domainModel.Vendor_Id = model.Vendor_Id;
                    domainModel.PinCode = model.PinCode;
                    domainModel.PAN = model.PAN;
                    domainModel.JoiningDate = DateTime.Now;
                    domainModel.AdminLogin_Id = admin.Id;
                    domainModel.IsBankUpdateApproved = false;
                    domainModel.lABId = bk.GenerateLabId();

                    admin.UserID = domainModel.lABId;
                };
                ent.Labs.Add(domainModel);
                ent.SaveChanges();

                string msg = @"<!DOCTYPE html>
<html>
<head>
    <title>PS Wellness Registration Confirmation</title>
</head>
<body>
    <h1>Welcome to PS Wellness!</h1>
    <p>Your signup is complete. To finalize your registration, please use the following login credentials:</p>
    <ul>
        <li><strong>User ID:</strong> " + admin.UserID + @"</li>
        <li><strong>Password:</strong> " + admin.Password + @"</li>
    </ul>
    <p>Thank you for choosing PS Wellness. We look forward to assisting you on your wellness journey.</p>
</body>
</html>";

                
                EmailEF ef = new EmailEF()
                {
                    EmailAddress = model.EmailId,
                    Message = msg,
                    Subject = "PS Wellness Registration Confirmation"
                };

                EmailOperations.SendEmainew(ef);

                string msg1 = "Welcome to PSWELLNESS. Your User Name :  " + domainModel.EmailId + "(" + domainModel.lABId + "), Password : " + admin.Password + ".";
                Message.SendSmsUserIdPass(domainModel.MobileNumber, model.LabName, model.lABId, model.Password);
                rm.Message = "Welcome to PS Wellness. Sign up process completed. Approval pending.";
                rm.Status = 1;
                return Ok(rm);

            }

            catch (Exception ex)
            {
                log.Error(ex.Message);
                //tran.Rollback();
                return InternalServerError(ex);
            }

            // }
        }

        //======================FRANCHISE PATIENT REGISTRATION==========================//
        [HttpPost, Route("api/FranchisesApi/Fra_PatientRegistration")]
        public IHttpActionResult Fra_PatientRegistration(fra_PatientReg model)
        {
            var rm = new ReturnMessage();
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (ent.AdminLogins.Any(a => a.Username == model.EmailId))
                    {
                        rm.Message = "This Email-Id has already exists.";
                        rm.Status = 0;
                        return Ok(rm);
                    }
                    if (ent.AdminLogins.Any(a => a.PhoneNumber == model.MobileNumber))
                    {
                        rm.Message = "This Mobile Number has already exists.";
                        rm.Status = 0;
                        return Ok(rm);
                    }

                    if (ent.Patients.Any(a => a.PatientName == model.PatientName && a.MobileNumber == model.MobileNumber))
                    {
                        var data = ent.Patients.Where(a => a.PatientName == model.PatientName && a.MobileNumber == model.MobileNumber).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.PatientRegNo).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        rm.Message = "you are already registered with pswellness";
                        rm.Status = 0;
                        return Ok(rm);
                    }

                    var admin = new AdminLogin
                    {
                        Username = model.EmailId,
                        PhoneNumber = model.MobileNumber,
                        Password = model.Password,

                        Role = "patient"
                    };
                    ent.AdminLogins.Add(admin);
                    ent.SaveChanges();

                    //Add City Additional CityName

                    //var domainModel = Mapper.Map<Patient>(model);
                    var domainModel = new Patient();
                    {

                        domainModel.PatientName = model.PatientName;
                        domainModel.MobileNumber = model.MobileNumber;
                        domainModel.EmailId = model.EmailId;
                        domainModel.Password = model.Password;
                        domainModel.Location = model.Location;
                        domainModel.StateMaster_Id = model.StateMaster_Id;
                        domainModel.CityMaster_Id = model.CityMaster_Id;
                        domainModel.PinCode = model.PinCode;
                        domainModel.vendorId = model.vendorId;
                        domainModel.AdminLogin_Id = admin.Id;
                        domainModel.IsApproved = true;
                        domainModel.Rwa_Id = 0;
                        domainModel.DOB = model.DOB;
                        domainModel.Gender = model.Gender;
                        domainModel.Reg_Date = DateTime.Now;
                        domainModel.PatientRegNo = bk.GeneratePatientRegNo();

                        admin.UserID = domainModel.PatientRegNo;
                    };
                    ent.Patients.Add(domainModel);
                    ent.SaveChanges();

                    string msg = @"<!DOCTYPE html>
<html>
<head>
    <title>PS Wellness Registration Confirmation</title>
</head>
<body>
    <h1>Welcome to PS Wellness!</h1>
    <p>Your signup is complete. To finalize your registration, please use the following login credentials:</p>
    <ul>
        <li><strong>User ID:</strong> " + admin.UserID + @"</li>
        <li><strong>Password:</strong> " + admin.Password + @"</li>
    </ul>
    <p>Thank you for choosing PS Wellness. We look forward to assisting you on your wellness journey.</p>
</body>
</html>";

                    // string msg1 = "Welcome to PS Wellness. Your signup is complete. To finalize your registration please proceed to log in using the credentials you provided during the signup process. Your User Id: " + admin.UserID + ", Password: " + admin.Password + ".";

                    EmailEF ef = new EmailEF()
                    {
                        EmailAddress = model.EmailId,
                        Message = msg,
                        Subject = "PS Wellness Registration Confirmation"
                    };

                    EmailOperations.SendEmainew(ef);

                    tran.Commit(); 
                    string msg1 = "Welcome to PSWELLNESS. Your User Name :  " + admin.Username + "(" + domainModel.PatientRegNo + "), Password : " + admin.Password + ".";
                    Message.SendSmsUserIdPass(model.MobileNumber, model.PatientName, model.PatientRegNo, model.Password);
                    rm.Message = "Welcome to PS Wellness. Sign up process completed. Approval pending.";
                    rm.Status = 1;
                    return Ok(rm);
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    tran.Rollback();
                    return InternalServerError(ex);
                }
            }
        }
         
        //======================FRANCHISE DRIVER REGISTRATION==========================//

        [HttpPost, Route("api/FranchisesApi/Fra_DriverRegistration")]
        public IHttpActionResult Fra_DriverRegistration(fra_DriverReg model)
        {
            var rm = new ReturnMessage();
            string[] allowedExtensions = { ".jpg", ".png", ".jpeg" };
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        var message = string.Join(" | ",
ModelState.Values
.SelectMany(a => a.Errors)
.Select(a => a.ErrorMessage));
                        rm.Message = message;
                        rm.Status = 0;
                        return Ok(rm);
                    }

                    if (ent.AdminLogins.Any(a => a.Username == model.MobileNumber))
                    {
                        rm.Message = "This Mobile Number has already exists.";
                        rm.Status = 0;
                        return Ok(rm);
                    }
                    if (ent.AdminLogins.Any(a => a.PhoneNumber == model.MobileNumber))
                    {
                        rm.Message = "This Mobile Number has already exists.";
                        rm.Status = 0;
                        return Ok(rm);
                    }
                    if (ent.Drivers.Any(a => a.DriverName == model.DriverName && a.MobileNumber == model.MobileNumber))
                    {
                        var data = ent.Drivers.Where(a => a.DriverName == model.DriverName && a.MobileNumber == model.MobileNumber).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.DriverId).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        rm.Message = "you are already registered with pswellness";
                        rm.Status = 0;
                        return Ok(rm);
                    }



                    var admin = new AdminLogin
                    {
                        Username = model.EmailId,
                        PhoneNumber = model.MobileNumber,
                        Password = model.Password,
                        Role = "driver"
                    };
                    ent.AdminLogins.Add(admin);
                    ent.SaveChanges();

                    // upload driver profile image

                    if (model.DriverImageBase64 != null)
                    {
                        var dlImg = FileOperation.UploadFileWithBase64("Images", model.DriverImage,model.DriverImageBase64, allowedExtensions);
                        if (dlImg == "not allowed")
                        {
                            rm.Message = "Only png,jpg,jpeg files are allowed as Profile Image.";
                            rm.Status = 0;
                            tran.Rollback();
                            return Ok(rm);
                        }
                        model.DriverImage = dlImg;
                    }

                    if (model.DlImage1Base64 != null)
                    {
                        var dlImg = FileOperation.UploadFileWithBase64("Images", model.DlImage1, model.DlImage1Base64, allowedExtensions);
                        if (dlImg == "not allowed")
                        {
                            rm.Message = "Only png,jpg,jpeg files are allowed as DL Image.";
                            rm.Status = 0;
                            tran.Rollback();
                            return Ok(rm);
                        }
                        model.DlImage1 = dlImg;
                    }
                    if (model.DlImage2Base64 != null)
                    {
                        var dlImg1 = FileOperation.UploadFileWithBase64("Images", model.DlImage2, model.DlImage2Base64, allowedExtensions);
                        if (dlImg1 == "not allowed")
                        {
                            rm.Message = "Only png,jpg,jpeg files are allowed as DL Picture.";
                            rm.Status = 0;
                            tran.Rollback();
                            return Ok(rm);
                        }
                        model.DlImage2 = dlImg1;
                    }


                    // aadhar image upload
                    if (model.AadharImageBase64Image != null)
                    {
                        var aadharImg = FileOperation.UploadFileWithBase64("Images", model.AadharImage, model.AadharImageBase64Image, allowedExtensions);

                        if (aadharImg == "not allowed")
                        {
                            rm.Message = "Only png,jpg,jpeg files are allowed as Aadhar Picture.";
                            rm.Status = 0;
                            tran.Rollback();
                            return Ok(rm);
                        }
                        model.AadharImage = aadharImg;
                    }

                    if (model.AadharImage64Image1 != null)
                    {
                        var aadharImg2 = FileOperation.UploadFileWithBase64("Images", model.AadharImage2, model.AadharImage64Image1, allowedExtensions);

                        if (aadharImg2 == "not allowed")
                        {
                            rm.Message = "Only png,jpg,jpeg files are allowed as Aadhar Picture.";
                            rm.Status = 0;
                            tran.Rollback();
                            return Ok(rm);
                        }
                        model.AadharImage2 = aadharImg2;
                    }
                    //var domainModel = Mapper.Map<Driver>(model);
                    var domainModel = new Driver();
                    {
                        domainModel.DriverName = model.DriverName;
                        domainModel.Password = model.Password;
                        domainModel.MobileNumber = model.MobileNumber;
                        domainModel.EmailId = model.EmailId;
                        domainModel.Location = model.Location;
                        domainModel.StateMaster_Id = model.StateMaster_Id;
                        domainModel.CityMaster_Id = model.CityMaster_Id;
                        domainModel.DlNumber = model.DlNumber;
                        domainModel.DlImage1 = model.DlImage1;
                        domainModel.DlImage2 = model.DlImage2;
                        domainModel.DlValidity = model.DlValidity;
                        domainModel.DriverImage = model.DriverImage;
                        domainModel.AadharImage = model.AadharImage;
                        domainModel.AadharImage2 = model.AadharImage2;
                        domainModel.PinCode = model.PinCode;
                        domainModel.Vendor_Id = model.Vendor_Id;
                        domainModel.Paidamount = model.Paidamount;
                        domainModel.PAN = model.PAN;
                        domainModel.JoiningDate = DateTime.Now;
                        domainModel.AdminLogin_Id = admin.Id;
                        domainModel.DriverId = bk.GenerateDriverId();
                        domainModel.IsBankUpdateApproved = false;
                        domainModel.IsBooked = false;
                        admin.UserID = domainModel.DriverId;
                    };


                    ent.Drivers.Add(domainModel);
                    ent.SaveChanges();

                    string msg = @"<!DOCTYPE html>
<html>
<head>
    <title>PS Wellness Registration Confirmation</title>
</head>
<body>
    <h1>Welcome to PS Wellness!</h1>
    <p>Your signup is complete. To finalize your registration, please use the following login credentials:</p>
    <ul>
        <li><strong>User ID:</strong> " + admin.UserID + @"</li>
        <li><strong>Password:</strong> " + admin.Password + @"</li>
    </ul>
    <p>Thank you for choosing PS Wellness. We look forward to assisting you on your wellness journey.</p>
</body>
</html>";

                    // string msg1 = "Welcome to PS Wellness. Your signup is complete. To finalize your registration please proceed to log in using the credentials you provided during the signup process. Your User Id: " + admin.UserID + ", Password: " + admin.Password + ".";

                    EmailEF ef = new EmailEF()
                    {
                        EmailAddress = model.EmailId,
                        Message = msg,
                        Subject = "PS Wellness Registration Confirmation"
                    };

                    EmailOperations.SendEmainew(ef);
                    rm.Status = 1;
                    string msg1 = "Welcome to PSWELLNESS. Your User Name :  " + admin.Username + "(" + domainModel.DriverId + "), Password : " + admin.Password + ".";
                    Message.SendSmsUserIdPass(model.MobileNumber, model.DriverName, domainModel.DriverId, model.Password);
                    rm.Message = "Welcome to PS Wellness. Sign up process completed. Approval pending.";
                    tran.Commit();
                    return Ok(rm);
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Invalid length for a Base-64 char array or string.")
                    {
                        rm.Message = "Invalid Attempt of Image";
                        rm.Status = 0;
                        tran.Rollback();
                        return Ok(rm);
                    }
                    log.Error(ex.Message);
                    tran.Rollback();
                    return InternalServerError(ex);
                }
            }

        }

        //======================FRANCHISE RWA REGISTRATION==========================//

        [HttpPost, Route("api/FranchisesApi/Fra_RWARegistration")]
        public IHttpActionResult Fra_RWARegistration(RWA_Registration model)
        {
            var rm = new ReturnMessage();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {

                    if (ent.AdminLogins.Any(a => a.Username == model.EmailId))
                    {
                        rm.Message = "This EmailId has already exists.";
                        rm.Status = 0;
                        return Ok(rm);
                    }
                    if (ent.AdminLogins.Any(a => a.PhoneNumber == model.MobileNumber))
                    {
                        rm.Status = 0;
                        rm.Message = "This Mobile Number has already exists.";
                        return Ok(rm);
                    }
                    if (ent.RWAs.Any(a => a.AuthorityName == model.AuthorityName && a.PhoneNumber == model.MobileNumber))
                    {
                        var data = ent.RWAs.Where(a => a.AuthorityName == model.AuthorityName && a.PhoneNumber == model.MobileNumber).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.RWAId).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        rm.Message = "you are already registered with pswellness";
                        rm.Status = 0;
                        return Ok(rm);
                    }


                    var admin = new AdminLogin
                    {
                        Username = model.EmailId,
                        PhoneNumber = model.MobileNumber,
                        Password = model.Password,
                        //UserID = uniqHospiId,
                        Role = "RWA"
                    };
                    ent.AdminLogins.Add(admin);
                    ent.SaveChanges();


                    var img = FileOperation.UploadFileWithBase64("Images", model.CertificateImage, model.CertificateImagebase64, allowedExtensions);
                    if (img == "not allowed")
                    {
                        rm.Status = 0;
                        rm.Message = "Only png,jpg,jpeg files are allowed.";
                        tran.Rollback();
                        return Ok(rm);
                    }
                    model.CertificateImage = img;

                    var domainModel = new RWA();
                    {
                        domainModel.AdminLogin_Id = admin.Id;
                        domainModel.AuthorityName = model.AuthorityName;
                        domainModel.MobileNumber = model.MobileNumber;
                        domainModel.EmailId = model.EmailId;
                        domainModel.Password = model.Password;
                        domainModel.CertificateImage = model.CertificateImage;
                        domainModel.StateMaster_Id = model.StateMaster_Id;
                        domainModel.CityMaster_Id = model.CityMaster_Id;
                        domainModel.Location = model.Location;
                        domainModel.LandlineNumber = model.LandlineNumber;
                        domainModel.Pincode = model.Pincode;
                        domainModel.PAN = model.PAN;
                        domainModel.Vendor_Id = model.Vendor_Id;
                        domainModel.JoiningDate = DateTime.Now;
                        domainModel.IsBankUpdateApproved = false;
                        domainModel.RWAId = bk.GenerateRWA_Id();
                        admin.UserID = domainModel.RWAId;
                };


                    ent.RWAs.Add(domainModel);
                    ent.SaveChanges();

                    string msg = @"<!DOCTYPE html>
<html>
<head>
    <title>PS Wellness Registration Confirmation</title>
</head>
<body>
    <h1>Welcome to PS Wellness!</h1>
    <p>Your signup is complete. To finalize your registration, please use the following login credentials:</p>
    <ul>
        <li><strong>User ID:</strong> " + admin.UserID + @"</li>
        <li><strong>Password:</strong> " + admin.Password + @"</li>
    </ul>
    <p>Thank you for choosing PS Wellness. We look forward to assisting you on your wellness journey.</p>
</body>
</html>";

                    // string msg1 = "Welcome to PS Wellness. Your signup is complete. To finalize your registration please proceed to log in using the credentials you provided during the signup process. Your User Id: " + admin.UserID + ", Password: " + admin.Password + ".";

                    EmailEF ef = new EmailEF()
                    {
                        EmailAddress = model.EmailId,
                        Message = msg,
                        Subject = "PS Wellness Registration Confirmation"
                    };

                    EmailOperations.SendEmainew(ef);
                    tran.Commit();
                    rm.Status = 1;
                    string msg1 = "Welcome to PSWELLNESS. Your User Name :  " + admin.Username + "(" + domainModel.Id + "), Password : " + admin.Password + ".";
                    Message.SendSms(domainModel.PhoneNumber, msg1);
                    rm.Message = "Welcome to PS Wellness. Sign up process completed. Approval pending.";
                    return Ok(rm);
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    tran.Rollback();
                    return InternalServerError(ex);
                }
            }
        }

        //======================FRANCHISE NURSE REGISTRATION==========================//

        [HttpPost, Route("api/FranchisesApi/Fra_NurseRegistration")]
        public IHttpActionResult Fra_NurseRegistration(fra_NurseReg model)
        {
            var rm = new ReturnMessage();
            string[] allowedExtensions = { ".jpg", ".png", ".jpeg" };
            using (var tran = ent.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        var message = string.Join(" | ",
                        ModelState.Values
                        .SelectMany(a => a.Errors)
                        .Select(a => a.ErrorMessage));
                        rm.Message = message;
                        rm.Status = 0;
                        return Ok(rm);
                    }
                    if (ent.AdminLogins.Any(a => a.Username == model.EmailId))
                    {
                        rm.Message = "This EmailId has already exists.";
                        rm.Status = 0;
                        return Ok(rm);
                    }
                    if (ent.AdminLogins.Any(a => a.PhoneNumber == model.MobileNumber))
                    {
                        rm.Message = "This Mobile Number has already exists.";
                        rm.Status = 0;
                        return Ok(rm);
                    }

                    if (ent.Nurses.Any(a => a.NurseName == model.NurseName && a.MobileNumber == model.MobileNumber))
                    {
                        var data = ent.Nurses.Where(a => a.NurseName == model.NurseName && a.MobileNumber == model.MobileNumber).FirstOrDefault();
                        var logdata = ent.AdminLogins.Where(a => a.UserID == data.NurseId).FirstOrDefault();
                        string mssg = "Welcome to PSWELLNESS. Your User Name :  " + logdata.Username + "(" + logdata.UserID + "), Password : " + logdata.Password + ".";
                        Message.SendSms(logdata.PhoneNumber, mssg);
                        rm.Message = "you are already registered with pswellness";
                        rm.Status = 0;
                        return Ok(rm);
                    }
                    var HUniquId = bk.GenerateNurseId();

                    var admin = new AdminLogin
                    {
                        Username = model.EmailId,
                        PhoneNumber = model.MobileNumber,
                        Password = model.Password,
                        UserID = HUniquId,
                        Role = "nurse"
                    };
                    ent.AdminLogins.Add(admin);
                    ent.SaveChanges();

                    //Add City Additional CityName
                    //if (model.CityName != null)
                    //{
                    //    var city = new CityTemp
                    //    {
                    //        CityName = model.CityName,
                    //        Login_Id = admin.Id,
                    //        IsApproved = false,
                    //        State_Id = model.StateMaster_Id
                    //    };
                    //    ent.CityTemps.Add(city);
                    //    ent.SaveChanges();
                    //    model.CityMaster_Id = city.Id;
                    //}

                    // CertificateImage upload
                    var cetificateImg = FileOperation.UploadFileWithBase64("Images", model.CertificateImage, model.CertificateBase64Image, allowedExtensions);
                    if (cetificateImg == "not allowed")
                    {
                        rm.Status = 0;
                        rm.Message = "Only png,jpg,jpeg files are allowed as cetificate document";
                        tran.Rollback();
                        return Ok(rm);
                    }
                    model.CertificateImage = cetificateImg;
                    
                     
                    var domainModel = new Nurse();
                    {
                        domainModel.NurseName = model.NurseName;
                        domainModel.EmailId = model.EmailId;
                        domainModel.Password = model.Password;
                        domainModel.MobileNumber = model.MobileNumber;
                        domainModel.Location = model.Location;
                        domainModel.StateMaster_Id = model.StateMaster_Id;
                        domainModel.CityMaster_Id = model.CityMaster_Id;
                        domainModel.CertificateImage = model.CertificateImage;
                        domainModel.CertificateNumber = model.CertificateNumber;
                        domainModel.PinCode = model.PinCode;
                        domainModel.NurseType_Id = model.NurseType_Id;
                        domainModel.Fee = model.Fee;
                        domainModel.Location_id = model.Location_id;
                        domainModel.JoiningDate = DateTime.Now;
                        domainModel.Vendor_Id = model.Vendor_Id;
                        domainModel.experience = model.experience;
                        domainModel.PAN = model.PAN;
                        domainModel.AdminLogin_Id = admin.Id;
                        domainModel.NurseId = HUniquId;
                        domainModel.IsBankUpdateApproved = false;
                    };


                    ent.Nurses.Add(domainModel);
                    ent.SaveChanges();
                    string msg = @"<!DOCTYPE html>
<html>
<head>
    <title>PS Wellness Registration Confirmation</title>
</head>
<body>
    <h1>Welcome to PS Wellness!</h1>
    <p>Your signup is complete. To finalize your registration, please use the following login credentials:</p>
    <ul>
        <li><strong>User ID:</strong> " + admin.UserID + @"</li>
        <li><strong>Password:</strong> " + admin.Password + @"</li>
    </ul>
    <p>Thank you for choosing PS Wellness. We look forward to assisting you on your wellness journey.</p>
</body>
</html>";

                    // string msg1 = "Welcome to PS Wellness. Your signup is complete. To finalize your registration please proceed to log in using the credentials you provided during the signup process. Your User Id: " + admin.UserID + ", Password: " + admin.Password + ".";

                    EmailEF ef = new EmailEF()
                    {
                        EmailAddress = model.EmailId,
                        Message = msg,
                        Subject = "PS Wellness Registration Confirmation"
                    };

                    EmailOperations.SendEmainew(ef);

                    rm.Message = "Welcome to PS Wellness. Sign up process completed. Approval pending.";
                    rm.Status = 1;
                    string msg1 = "Welcome to PSWELLNESS. Your User Name :  " + admin.Username + "(" + domainModel.NurseId + "), Password : " + admin.Password + ".";
                    Message.SendSmsUserIdPass(model.MobileNumber, model.NurseName, domainModel.NurseId, model.Password);
                    tran.Commit();
                    return Ok(rm);

                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    rm.Message = "Server Error";
                    tran.Rollback();
                    return Ok(rm);
                }
            }
        }

        //======================FRANCHISE VEHICLE REGISTRATION==========================//

        [HttpPost, Route("api/FranchisesApi/Fra_VehicleRegistration")]
        public IHttpActionResult Fra_VehicleRegistration(fra_VehicleReg model)
        {
            var rm = new ReturnMessage();
            
                try
                {  
                    var domainModel = new Vehicle();
                    {
                        domainModel.VehicleName = model.VehicleName;
                        domainModel.VehicleOwnerName = model.VehicleOwnerName;
                        domainModel.VehicleNumber = model.VehicleNumber;
                        domainModel.AccountNo = model.AccountNo;
                        domainModel.DriverCharges = model.DriverCharges;
                        domainModel.HolderName = model.AccountHolderName;
                        domainModel.VehicleCat_Id = model.VehicleCat_Id;
                        domainModel.VehicleType_Id = model.VehicleType_Id;
                        domainModel.IFSCCode = model.IFSCCode;
                        domainModel.CancelCheque = model.CancelCheque;
                        domainModel.PAN = model.PAN;
                        domainModel.IsDeleted = false;
                        domainModel.RegistrationDate = DateTime.Now;
                        domainModel.Vendor_Id = model.Vendor_Id;
                        domainModel.VehicleOwnerName = model.VehicleOwnerName;

                    };


                    ent.Vehicles.Add(domainModel);
                    ent.SaveChanges();


                    rm.Message = "Welcome to PS Wellness. Sign up process completed. Approval pending.";

                     
                    return Ok(rm);

                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    rm.Message = "Server Error";
                    
                    return Ok(rm);
                }
            
        }


        //======================FRANCHISE (CHEMIST REGISTRATION DETAIL)==========================//

        [HttpGet, Route("api/FranchisesApi/Fra_ChemistRegistrationDetail")]

        public IHttpActionResult Fra_ChemistRegistrationDetail(int VendorId)
        {
            string qry = @"select Ch.Id,Ch.ChemistId,Ch.ChemistName,Ch.Vendor_Id,Ch.ShopName,Ch.MobileNumber,Ch.EmailId,Ch.Location,Ch.GSTNumber,Ch.LicenceNumber,Ch.IsApproved from Chemist as Ch inner join Vendor as V on V.Id=Ch.Vendor_Id Where Ch.IsDeleted=0 and V.Id="+ VendorId + " order by Ch.Id desc";
            var ChemistRegDetail = ent.Database.SqlQuery<fra_ChemistregDetail>(qry).ToList();
            return Ok(new { ChemistRegDetail });
        }


        //======================FRANCHISE (EDIT CHEMIST REGISTRATION DETAIL)==========================//

        [HttpPost, Route("api/FranchisesApi/Fra_EditChemRegDetail")]

        public IHttpActionResult Fra_EditChemRegDetail(ChemsitEditchregdetail model)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ",
ModelState.Values
.SelectMany(a => a.Errors)
.Select(a => a.ErrorMessage));
                    rm.Message = message;
                    rm.Status = 0;
                    return Ok(rm);
                }



                var data = ent.Chemists.Find(model.Id);
                if (data == null)
                {
                    rm.Status = 0;
                    rm.Message = "no data found to updata";
                    return Ok(rm);
                }


                data.ChemistName = model.ChemistName;
                data.ShopName = model.ShopName;
                data.MobileNumber = model.MobileNumber;
                data.EmailId = model.EmailId;
                data.Location = model.Location;
                data.GSTNumber = model.GSTNumber;
                data.LicenceNumber = model.LicenceNumber;
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = " Chemist Registration Detail Updated Successfully .";
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }

        //======================FRANCHISE (DELETE CHEMIST REGISTRATION DETAIL)==========================//

        [HttpPost, Route("api/FranchisesApi/Fra_DeleteChemistRegDetail")]

        public IHttpActionResult Fra_DeleteChemistRegDetail(int Id)
        {
            var data = ent.Chemists.Find(Id);
            try
            {
                data.IsDeleted = true;
                ent.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return Ok(new { Message = "Record Deleted Successfully!!!" });
        }

        //======================FRANCHISE(LAB REGISTRATION DETAIL)==========================//

        [HttpGet, Route("api/FranchisesApi/Fra_LabRegistrationDetail")]

        public IHttpActionResult Fra_LabRegistrationDetail(int VendorId)
        {
            string qry = @"select L.Id,L.lABId,L.LabName,V.VendorName as Franchise,L.PhoneNumber,L.MobileNumber,L.EmailId,L.Location,L.LicenceNumber,L.GSTNumber,L.AadharNumber,L.IsApproved from Lab as L inner join Vendor as V on V.Id=L.Vendor_Id where L.IsDeleted=0 and V.Id="+ VendorId + " order by L.Id desc";
            var LabRegDetail = ent.Database.SqlQuery<FraLabRegDetail>(qry).ToList();
            return Ok(new { LabRegDetail });
        }


        //======================FRANCHISE (EDIT LAB REGISTRATION DETAIL)==========================//

        [HttpPost, Route("api/FranchisesApi/Fra_EditLabRegDetail")]

        public IHttpActionResult Fra_EditLabRegDetail(FraEditlabregdetail model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage));
                    rm.Message = message;
                    rm.Status = 0;
                    return Ok(rm);
                }

                var data = ent.Labs.Find(model.Id);
                if (data == null)
                {
                    rm.Status = 0;
                    rm.Message = "no data found to updata";
                    return Ok(rm);
                }


                data.LabName = model.LabName;
                data.PhoneNumber = model.PhoneNumber;
                data.MobileNumber = model.MobileNumber;
                data.EmailId = model.EmailId;
                data.Location = model.Location;
                data.GSTNumber = model.GSTNumber;
                data.LicenceNumber = model.LicenceNumber;
                data.AadharNumber = model.AadharNumber;
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = " Lab Registration Detail Updated Successfully .";
            }

            catch (Exception ex)
            {
                string msg = ex.ToString();
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }

        //======================FRANCHISE (DELETE LAB REGISTRATION DETAIL)==========================//

        [HttpPost, Route("api/FranchisesApi/Fra_DeleteLabRegDetail")]

        public IHttpActionResult Fra_DeleteLabRegDetail(int Id)
        {
            try
            {
                var data = ent.Labs.Find(Id);
                data.IsDeleted = true;
                ent.SaveChanges();

            }

            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return Ok(new { Message = "Record Deleted Successfully!!!" });
        }

        //======================FRANCHISE(DOCTOR REGISTRATION DETAIL)==========================//

        [HttpGet, Route("api/FranchisesApi/Fra_DoctorRegistrationDetail")]

        public IHttpActionResult Fra_DoctorRegistrationDetail(int VendorId)
        {
            string qry = @"select D.Id,D.DoctorId,D.DoctorName,D.Fee as Amount,D.Location,V.VendorName,DD.DepartmentName,S.SpecialistName,D.MobileNumber,D.EmailId,D.LicenceNumber from Doctor as D left join Specialist as S on s.Id=D.Specialist_Id inner join Vendor as V on V.Id=D.Vendor_Id left join Department as DD on DD.Id=D.Department_Id where D.IsDeleted=0 and V.Id="+ VendorId + " order by D.Id desc";
            var DoctorRegDetail = ent.Database.SqlQuery<FraDoctorRegDetail>(qry).ToList();
            return Ok(new { DoctorRegDetail });
        }

        //======================FRANCHISE(DRIVER REGISTRATION DETAIL)==========================//

        [HttpGet, Route("api/FranchisesApi/Fra_DriverRegistrationDetail")]


        public IHttpActionResult Fra_DriverRegistrationDetail(int VendorId)
        {
            string qry = @"select D.Id,D.DriverName,D.DriverId,D.EmailId,D.MobileNumber,sm.StateName +','+cm.CityName +','+D.Location as Location,VT.VehicleTypeName,D.DlNumber from Driver as D 
left join VehicleType as VT on VT.Id=D.VehicleType_Id 
join StateMaster as sm on sm.Id=D.StateMaster_Id
join CityMaster as cm on cm.Id=D.CityMaster_Id
join Vendor as V on V.Id=D.Vendor_Id where D.IsDeleted=0 and V.Id=" + VendorId + " order by D.Id desc";
            var DriverRegDetail = ent.Database.SqlQuery<FraDriverRegDetail>(qry).ToList();
            return Ok(new { DriverRegDetail });
        }

        //======================FRANCHISE(VEHICLE REGISTRATION DETAIL)==========================//

        [HttpGet, Route("api/FranchisesApi/Fra_VehicleRegistrationDetail")]

        public IHttpActionResult Fra_VehicleRegistrationDetail(int VendorId)
        {
            string qry = @"select V.Id,V.VehicleNumber,V.VehicleOwnerName,Ven.VendorName as [Franchise],VT.VehicleTypeName,Cat.CategoryName,V.DriverCharges from Vehicle as V inner join Vendor as Ven on Ven.Id=V.Vendor_Id left join VehicleType as VT on VT.Id=V.VehicleType_Id left join MainCategory Cat on Cat.Id=VT.Category_Id where V.IsDeleted=0 and Ven.Id="+ VendorId + " Order by V.Id desc";
            var VehicleRegDetail = ent.Database.SqlQuery<FraVehicleRegDetail>(qry).ToList();
            return Ok(new { VehicleRegDetail });
        }

        //======================FRANCHISE(NURSE REGISTRATION DETAIL)==========================//

        [HttpGet, Route("api/FranchisesApi/Fra_NurseRegistrationDetail")]

        public IHttpActionResult Fra_NurseRegistrationDetail(int VendorId)
        {
            string qry = @"select N.Id,N.NurseId,N.NurseName,NT.NurseTypeName,V.VendorName,N.MobileNumber,N.EmailId,sm.StateName +','+cm.CityName +','+N.Location as Location,N.CertificateNumber,N.IsApproved from Nurse as N 
left join NurseType as NT on NT.Id=N.NurseType_Id 
left join StateMaster as sm on sm.Id=N.StateMaster_Id
left join CityMaster as cm on cm.Id=N.CityMaster_Id
join Vendor as V on V.Id=N.Vendor_Id where N.IsDeleted=0 and V.Id=" + VendorId + " order by N.Id desc";
            var NurseRegDetail = ent.Database.SqlQuery<FraNurseRegDetail>(qry).ToList();
            return Ok(new { NurseRegDetail });
        }

        //======================FRANCHISE (EDIT NURSE REGISTRATION DETAIL)==========================//

        [HttpPost, Route("api/FranchisesApi/Fra_EditNurseRegDetail")]

        public IHttpActionResult Fra_EditNurseRegDetail(FraEditNurseregdetail model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage));
                    rm.Message = message;
                    rm.Status = 0;
                    return Ok(rm);
                }

                var data = ent.Nurses.Find(model.Id);
                if (data == null)
                {
                    rm.Status = 0;
                    rm.Message = "no data found to updata";
                    return Ok(rm);
                }


                data.NurseName = model.NurseName;
                data.MobileNumber = model.MobileNumber;
                data.EmailId = model.EmailId;
                data.Location = model.Location;
                data.CertificateNumber = model.CertificateNumber;
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = " Nurse Registration Detail Updated Successfully .";
            }

            catch (Exception ex)
            {
                string msg = ex.ToString();
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }

        //======================FRANCHISE (DELETE NURSE REGISTRATION DETAIL)==========================//

        [HttpPost, Route("api/FranchisesApi/Fra_DeleteNurseRegDetail")]

        public IHttpActionResult Fra_DeleteNurseRegDetail(int Id)
        {
            try
            {
                var data = ent.Nurses.Find(Id);
                data.IsDeleted = true;
                ent.SaveChanges();


            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return Ok(new { Message = "Record Deleted Successfully!!!" });
        }


        //======================FRANCHISE(PATIENT REGISTRATION DETAIL)==========================//

        [HttpGet, Route("api/FranchisesApi/Fra_PatientRegistrationDetail")]

        public IHttpActionResult Fra_PatientRegistrationDetail(int VendorId)
        {
            string qry = @" select P.Id,P.PatientName,P.PatientRegNo,V.VendorName,P.MobileNumber,P.Location,CM.CityName,SM.StateName from Patient as P
 left join StateMaster as SM on SM.Id=P.StateMaster_Id 
 join Vendor as V on V.Id=P.vendorId 
 left join CityMaster as CM on CM.Id=P.CityMaster_Id where P.IsDeleted=0 and V.Id=" + VendorId + " order by P.Id desc";
            var PatientRegDetail = ent.Database.SqlQuery<FraPatientRegDetail>(qry).ToList();
            return Ok(new { PatientRegDetail });
        }

        //======================FRANCHISE(RWA REGISTRATION DETAIL)==========================//

        [HttpGet, Route("api/FranchisesApi/Fra_RWARegistrationDetail")]

        public IHttpActionResult Fra_RWARegistrationDetail(int VendorId)
        {
            string qry = @"select RWA.Id,RWA.RWAId,RWA.AuthorityName,RWA.MobileNumber,RWA.LandlineNumber as PhoneNumber,RWA.EmailId,RWA.Location,RWA.CertificateNo,RWA.IsApproved from RWA inner join Vendor as V on V.Id=RWA.Vendor_Id where RWA.IsDeleted=0 and V.Id="+ VendorId + " order by Id desc";
            var RWARegDetail = ent.Database.SqlQuery<FraRWARegDetail>(qry).ToList();
            return Ok(new { RWARegDetail });
        } 
        //======================FRANCHISE (EDIT RWA REGISTRATION DETAIL)==========================//

        [HttpPost, Route("api/FranchisesApi/Fra_EditRWARegDetail")]

        public IHttpActionResult Fra_EditRWARegDetail(FraEditRWAregdetail model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage));
                    rm.Message = message;
                    rm.Status = 0;
                    return Ok(rm);
                }

                var data = ent.RWAs.Find(model.Id);
                if (data == null)
                {
                    rm.Status = 0;
                    rm.Message = "no data found to updata";
                    return Ok(rm);
                }


                data.AuthorityName = model.AuthorityName;
                data.PhoneNumber = model.PhoneNumber;
                data.MobileNumber = model.MobileNumber;
                data.EmailId = model.EmailId;
                data.Location = model.Location;
                data.CertificateNo = model.CertificateNo;
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = " RWA Registration Detail Updated Successfully .";
            }

            catch (Exception ex)
            {
                string msg = ex.ToString();
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }

        //======================FRANCHISE (DELETE RWA REGISTRATION DETAIL)==========================//

        [HttpPost, Route("api/FranchisesApi/Fra_DeleteRWARegDetail")]

        public IHttpActionResult Fra_DeleteRWARegDetail(int Id)
        {
            try
            {
                var data = ent.RWAs.Find(Id);
                data.IsDeleted = true;
                ent.SaveChanges();

            }

            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return Ok(new { Message = "Record Deleted Successfully!!!" });
        } 

        //==============GET OLD DRIVER LIST==============//
        [HttpGet]
        [Route("api/FranchisesApi/GetOldDriverList")]

        public IHttpActionResult GetOldDriverList(int VendorId)
        {
            string qry = @"select V.Id,V.VehicleNumber,D.DriverName from Driver as d
join Vehicle as V on V.Id=D.Vehicle_Id
join Vendor as ve on ve.Id=d.Vendor_Id
where D.IsDeleted=0 and V.IsDeleted=0 and ve.Id=" + VendorId + "";
            var GetOldDriver = ent.Database.SqlQuery<Get_oldDriver>(qry).ToList();
            return Ok(new { GetOldDriver });

        }

        //==============UPDATE OLD DRIVER ==============//

        [HttpPost]
        [Route("api/FranchisesApi/UpdateOldDriver")]

        public IHttpActionResult UpdateOldDriver(Update_oldDriver model)
        {
            try
            {
                string qry = @"UPDATE Driver SET DriverName ='" + model.DriverName + "' FROM Driver JOIN Vehicle ON Driver.Id = Vehicle.Driver_Id WHERE Driver.Id=" + model.Id + " UPDATE Vehicle SET VehicleNumber ='" + model.VehicleNumber + "' FROM Driver JOIN Vehicle ON Driver.Id = Vehicle.Driver_Id WHERE Vehicle.Driver_Id=" + model.Id + "";
                ent.Database.ExecuteSqlCommand(qry);
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = " Old Driver Updated Successfully .";
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }

        //======================FRANCHISE (DELETE OLD DRIVER)==========================//

        [HttpPost, Route("api/FranchisesApi/DeleteOldDriver")]

        public IHttpActionResult DeleteOldDriver(int Id)
        {
            try
            {
                string qry = @"UPDATE Driver SET IsDeleted ='1' FROM Driver JOIN Vehicle ON Driver.Id = Vehicle.Driver_Id WHERE Driver.Id=" + Id + " UPDATE Vehicle SET IsDeleted ='1' FROM Driver JOIN Vehicle ON Driver.Id = Vehicle.Driver_Id WHERE Vehicle.Driver_Id=" + Id + "";
                ent.Database.ExecuteSqlCommand(qry);
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = " Old Driver Deleted Successfully .";
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }


        //==============GET NEW DRIVER LIST==============//
        [HttpGet]
        [Route("api/FranchisesApi/GetNewDriverList")]

        public IHttpActionResult GetNewDriverList(int VendorId)
        { 
            string query = @"select d.Id,CONCAT('[', d.DriverId, '] ', d.DriverName) AS DriverName from Driver as d
join Vendor as v on v.Id=d.Vendor_Id
where (d.VehicleType_Id = 0 OR d.VehicleType_Id IS NULL) and d.IsDeleted = 0 and v.Id=" + VendorId + "";
            var NewDriver = ent.Database.SqlQuery<NewDriverList>(query).ToList();
            return Ok(new { NewDriver });
        }

        //==============UPDATE NEW DRIVER LIST==============//

        [HttpPost]
        [Route("api/FranchisesApi/UpdateNewDriver")]
        public IHttpActionResult UpdateNewDriver(NewDriverList model)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ",
ModelState.Values
.SelectMany(a => a.Errors)
.Select(a => a.ErrorMessage));
                    rm.Message = message;
                    rm.Status = 0;
                    return Ok(rm);
                }


                var data = ent.Drivers.Find(model.Id);
                if (data == null)
                {
                    rm.Status = 0;
                    rm.Message = "no data found to updata";
                    return Ok(rm);
                }


                data.DriverName = model.DriverName;
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = "New Driver Updated Successfully .";
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }

        //=====================FRANCHISE ABOUT==================//

        [HttpGet]
        [Route("api/FranchisesApi/Franchise_About")]

        public IHttpActionResult Franchise_About(int Id)
        {
            string qry = @"select About from Vendor where IsDeleted=0 and Id=" + Id + "";
            var FranchiseAbout = ent.Database.SqlQuery<Fra_About>(qry).FirstOrDefault();
            return Ok(FranchiseAbout);
        }

        //===========================TDS DROPDOWN=======================//
        [HttpGet]
        [Route("api/FranchisesApi/GetTDSDropdown")]

        public IHttpActionResult GetTDSDropdown()
        {
            string qry = @"select * from [Role]";
            var TDSDropdown = ent.Database.SqlQuery<TDS_Dropdown>(qry).ToList();
            return Ok(new { TDSDropdown });
        }

        //==============TDS REPORT BY ROLE===================//

        [HttpGet, Route("api/FranchisesApi/TDSReports_ByRole")]
        public IHttpActionResult TDSReports_ByRole(string Role,int? VendorId)
        {
            if (Role == "Chemist")//chemist
            {
                string qry = @"select CP.Id,Ch.ChemistName as Name,CP.Amount as PaidFees,CP.Id as PaymentId,Ch.Location,CONVERT(VARCHAR(11), CP.PaymentDate, 103) AS PaymentDate,FORMAT(CP.PaymentDate, 'hh:mm') AS PaymentTime from ChemistPayOut as CP left join Chemist as Ch on CP.Chemist_Id=ch.Id where ch.IsDeleted=0";
                var TDSReport = ent.Database.SqlQuery<Payment_History>(qry).ToList();
                return Ok(new { TDSReport });

            }
            else if (Role == "Nurse")//Nurse
            {
                double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='" + Role + "'").FirstOrDefault();

                string qry = @"select A.Nurse_Id,D.NurseId as UniqueId, D.NurseName as Name,sm.StateName+','+ cm.CityName+','+d.Location as Location, SUM(A.TotalFee)  As PaidFees from dbo.NurseService A 
join Nurse D on D.Id = A.Nurse_Id
join Vendor as v on v.Id=D.Vendor_Id
left join StateMaster sm on sm.Id=D.StateMaster_Id
left join CityMaster cm on cm.Id=D.CityMaster_Id
where A.IsPaid=1 and A.ServiceDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() and D.Vendor_Id="+VendorId+" group by  D.NurseName, A.Nurse_Id,NurseId,D.Location,sm.StateName,cm.CityName";
                var TDSReport = ent.Database.SqlQuery<Payment_History>(qry).ToList();
                foreach (var item in TDSReport)
                {
                    item.tdsamt = (item.PaidFees * tds) / 100;

                    item.PayableAmount = item.PaidFees - (item.tdsamt);
                }
                return Ok(new { TDSReport });
            } 
            else if (Role == "Lab")//lab
            {
                double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='" + Role + "'").FirstOrDefault();

                string query = @"select A.Lab_Id,D.lABId as UniqueId, D.LabName as Name,sm.StateName+','+ cm.CityName+','+d.Location as Location, SUM(A.Amount)  As PaidFees from dbo.BookTestLab A 
join Lab D on D.Id = A.Lab_Id
join Vendor as v on v.Id=D.Vendor_Id
left join StateMaster sm on sm.Id=D.StateMaster_Id
left join CityMaster cm on cm.Id=D.CityMaster_Id
where A.IsPaid=1 and A.TestDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() and D.Vendor_Id="+VendorId+" group by  D.LabName, A.Lab_Id,D.lABId,D.Location,sm.StateName,cm.CityName";
                var TDSReport = ent.Database.SqlQuery<Payment_History>(query).ToList();
                foreach (var item in TDSReport)
                {
                    item.tdsamt = (item.PaidFees * tds) / 100;

                    item.PayableAmount = item.PaidFees - (item.tdsamt);
                }
                return Ok(new { TDSReport });
            }
            else if (Role == "Doctor")//doctor
            {
                double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='" + Role + "'").FirstOrDefault();

                string query = @"select A.Doctor_Id,D.DoctorId as UniqueId, D.DoctorName as Name,sm.StateName+','+ cm.CityName+','+d.Location as Location, SUM(A.TotalFee)  As PaidFees from dbo.PatientAppointment A 
join Doctor D on D.Id = A.Doctor_Id
join Vendor as v on v.Id=D.Vendor_Id
left join StateMaster sm on sm.Id=D.StateMaster_Id
left join CityMaster cm on cm.Id=D.CityMaster_Id
where A.IsPaid=1 and A.AppointmentDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() and D.Vendor_Id="+ VendorId + " group by  D.DoctorName, A.Doctor_Id,D.DoctorId,D.Location,sm.StateName,cm.CityName";
                var TDSReport = ent.Database.SqlQuery<Payment_History>(query).ToList();

                foreach (var item in TDSReport)
                { 
                    item.tdsamt = (item.PaidFees * tds) / 100; 
                     
                    item.PayableAmount = item.PaidFees - (item.tdsamt);
                }

                return Ok(new { TDSReport });
            }
            else if (Role == "Ambulance")//doctor
            {
                double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='" + Role + "'").FirstOrDefault();

                string query = @"select A.Driver_Id,D.DriverId as UniqueId, D.DriverName as Name,sm.StateName+','+ cm.CityName+','+d.Location as Location, CAST(SUM(A.TotalPrice) AS float)  As PaidFees from dbo.DriverLocation A 
join Driver D on D.Id = A.Driver_Id
join Vendor as v on v.Id=D.Vendor_Id
left join StateMaster sm on sm.Id=D.StateMaster_Id
left join CityMaster cm on cm.Id=D.CityMaster_Id
where A.IsPay='Y' and A.EntryDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() and D.Vendor_Id=" + VendorId+" group by  D.DriverName, A.Driver_Id,D.DriverId,D.Location,sm.StateName,cm.CityName";
                var TDSReport = ent.Database.SqlQuery<Payment_History>(query).ToList();

                foreach (var item in TDSReport)
                { 
                    item.tdsamt = (item.PaidFees * tds) / 100; 
                     
                    item.PayableAmount = item.PaidFees - (item.tdsamt);
                }

                return Ok(new { TDSReport });
            }
            else
            {
                rm.Message = "Record Not Found!!!";
            }
            return Ok(rm);
        }


        //===========================COMMISSION DROPDOWN=======================//
        [HttpGet]
        [Route("api/FranchisesApi/GetCommissionDropdown")]

        public IHttpActionResult GetCommissionDropdown()
        {
            string qry = @"select * from [Role]";
            var CommissionDropdown = ent.Database.SqlQuery<TDS_Dropdown>(qry).ToList();
            return Ok(new { CommissionDropdown });
        }

        //==============COMMISSION REPORT BY ROLE===================//

        [HttpGet, Route("api/FranchisesApi/CommissionReports_ByRole")]
        public IHttpActionResult CommissionReports_ByRole(string Role,int? VendorId)
        {
            if (Role == "Chemist")//chemist
            {
                string qry = @"select CP.Id,Ch.ChemistName as Name,CP.Amount as PaidFees,CP.Id as PaymentId,Ch.Location,CONVERT(VARCHAR(11), CP.PaymentDate, 103) AS PaymentDate,FORMAT(CP.PaymentDate, 'hh:mm') AS PaymentTime from ChemistPayOut as CP left join Chemist as Ch on CP.Chemist_Id=ch.Id where ch.IsDeleted=0";
                var CommissionReport = ent.Database.SqlQuery<Payment_History>(qry).ToList();
                return Ok(new { CommissionReport });

            }
            else if (Role == "Nurse")//Nurse
            {
                double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Nurse'").FirstOrDefault();
                double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Nurse'").FirstOrDefault();
                double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Nurse'").FirstOrDefault();

                //string qry = @"select NP.Id,N.NurseName as Name,NP.Amount as PaidFees,NP.Id as PaymentId,N.Location,CONVERT(VARCHAR(11), NP.PaymentDate, 103) AS PaymentDate,FORMAT(NP.PaymentDate, 'hh:mm') AS PaymentTime from NursePayout as NP Left join Nurse as N on NP.Nurse_Id=N.Id";
                string qry = @"select n.Id,n.NurseId as UniqueId,sm.StateName +','+cm.CityName +','+n.Location as Location,ns.Nurse_Id,n.NurseName as Name,CAST(SUM(ns.TotalFee) AS float) AS PaidFees from NurseService as ns
join Nurse as n on n.Id=ns.Nurse_Id
left join StateMaster sm on sm.Id=n.StateMaster_Id
left join CityMaster cm on cm.Id=n.CityMaster_Id
join Vendor as v on v.Id=n.Vendor_Id
WHERE  ns.IsPaid = 1 AND ns.ServiceDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() AND n.Vendor_Id=" + VendorId + " GROUP BY  n.NurseId, ns.Nurse_Id,n.NurseName,n.Location,sm.StateName,cm.CityName,n.Id;";
                var CommissionReport = ent.Database.SqlQuery<Payment_History>(qry).ToList();

                foreach (var item in CommissionReport)
                {
                    item.commamt = (item.PaidFees * commision) / 100;
                    item.tdsamt = (item.PaidFees * tds) / 100;
                    item.transactionamt = (item.PaidFees * Transactionfee) / 100;

                    var razorcomm = (item.PaidFees * Transactionfee) / 100;
                    var totalrazorcomm = razorcomm;
                    item.Amountwithrazorpaycomm = item.PaidFees + totalrazorcomm;

                    item.PayableAmount = item.PaidFees - (item.commamt + item.tdsamt + item.transactionamt);
                    item.FraPaidableamt = (item.PaidFees * 3) / 100;
                }
                return Ok(new { CommissionReport });
            } 
            else if (Role == "Lab")//lab
            {
                double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Lab'").FirstOrDefault();
                double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Lab'").FirstOrDefault();
                double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Lab'").FirstOrDefault();

                string query = @"SELECT D.Id,D.lABId as UniqueId,sm.StateName +','+cm.CityName +','+D.Location as Location,A.Lab_Id,D.LabName as Name,SUM(A.Amount) AS PaidFees FROM BookTestLab A
JOIN Lab D ON D.Id = A.Lab_Id
left join StateMaster sm on sm.Id=D.StateMaster_Id
left join CityMaster cm on cm.Id=D.CityMaster_Id
join Vendor as v on v.Id=d.Vendor_Id
where D.IsDeleted=0 and A.IsPaid = 1 and A.TestDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() AND d.Vendor_Id="+ VendorId + " GROUP BY  D.lABId, A.Lab_Id,D.LabName,D.Location,sm.StateName,cm.CityName,D.Id;";
                var CommissionReport = ent.Database.SqlQuery<Payment_History>(query).ToList();

                foreach (var item in CommissionReport)
                {
                    item.commamt = (item.PaidFees * commision) / 100;
                    item.tdsamt = (item.PaidFees * tds) / 100;
                    item.transactionamt = (item.PaidFees * Transactionfee) / 100;

                    var razorcomm = (item.PaidFees * Transactionfee) / 100;
                    var totalrazorcomm = razorcomm;
                    item.Amountwithrazorpaycomm = item.PaidFees + totalrazorcomm;

                    item.PayableAmount = item.PaidFees - (item.commamt + item.tdsamt + item.transactionamt);
                    item.FraPaidableamt = (item.PaidFees * 3) / 100;
                }
                return Ok(new { CommissionReport });
            }
            else if (Role == "Doctor")//doctor
            {
                double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Doctor'").FirstOrDefault();
                double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault();
                double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault();

                
                string query = @"SELECT D.Id,D.DoctorId as UniqueId,sm.StateName +','+cm.CityName +','+D.Location as Location,A.Doctor_Id,D.DoctorName as Name,SUM(A.TotalFee) AS PaidFees FROM dbo.PatientAppointment A
JOIN Doctor D ON D.Id = A.Doctor_Id
left join StateMaster sm on sm.Id=D.StateMaster_Id
left join CityMaster cm on cm.Id=D.CityMaster_Id
join Vendor as v on v.Id=d.Vendor_Id
WHERE  A.IsPaid = 1 AND A.AppointmentDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() AND d.Vendor_Id="+ VendorId + " GROUP BY  D.DoctorId, A.Doctor_Id,D.DoctorName,D.Location,sm.StateName,cm.CityName,D.Id;";
                var CommissionReport = ent.Database.SqlQuery<Payment_History>(query).ToList();
                

                foreach (var item in CommissionReport)
                {
                    item.commamt = (item.PaidFees * commision) / 100;
                    item.tdsamt = (item.PaidFees * tds) / 100;
                    item.transactionamt = (item.PaidFees * Transactionfee) / 100;

                    var razorcomm = (item.PaidFees * Transactionfee) / 100;
                    var totalrazorcomm = razorcomm;
                    item.Amountwithrazorpaycomm = item.PaidFees + totalrazorcomm;

                    item.PayableAmount = item.PaidFees - (item.commamt + item.tdsamt + item.transactionamt);
                    item.FraPaidableamt = (item.PaidFees * 3) / 100;
                }
                return Ok(new { CommissionReport });
            }
            else if (Role == "Ambulance")//Ambulance
            {
                double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Ambulance'").FirstOrDefault();
                double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();
                double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();


                string query = @"SELECT D.Id,D.DriverId as UniqueId,sm.StateName +','+cm.CityName +','+D.Location as Location,A.Driver_Id,D.DriverName as Name,CAST(SUM(A.TotalPrice) AS float) AS PaidFees FROM DriverLocation A
JOIN Driver D ON D.Id = A.Driver_Id
left join StateMaster sm on sm.Id=D.StateMaster_Id
left join CityMaster cm on cm.Id=D.CityMaster_Id
join Vendor as v on v.Id=d.Vendor_Id
WHERE  A.IsPay = 'Y' AND A.EntryDate BETWEEN DATEADD(DAY, -7, GETDATE()) AND GETDATE() AND d.Vendor_Id=" + VendorId + " GROUP BY  D.DriverId, A.Driver_Id,D.DriverName,D.Location,sm.StateName,cm.CityName,D.Id";
                var CommissionReport = ent.Database.SqlQuery<Payment_History>(query).ToList();


                foreach (var item in CommissionReport)
                {
                    item.commamt = (item.PaidFees * commision) / 100;
                    item.tdsamt = (item.PaidFees * tds) / 100;
                    item.transactionamt = (item.PaidFees * Transactionfee) / 100;

                    var razorcomm = (item.PaidFees * Transactionfee) / 100;
                    var totalrazorcomm = razorcomm;
                    item.Amountwithrazorpaycomm = item.PaidFees + totalrazorcomm;

                    item.PayableAmount = item.PaidFees - (item.commamt + item.tdsamt + item.transactionamt);
                    item.FraPaidableamt = (item.PaidFees * 3) / 100;
                }
                return Ok(new { CommissionReport });
            }
            else
            {
                rm.Message = "Record Not Found!!!";
            }
            return Ok(rm);
        }



        //==============GET CHEMIST DATA BY YMWD===================//

        [HttpGet, Route("api/FranchisesApi/GetChemistReport_By_YMWD")]

        public IHttpActionResult GetChemistReport_By_YMWD(string Timeperiod, int? VendorId)
        {
            if (Timeperiod == "Weekly")
            {
                string qry = @"select Ch.Id,Ch.ChemistId,Ch.ChemistName,V.VendorName as Franchise,Ch.ShopName,Ch.MobileNumber,Ch.EmailId,Ch.Location,Ch.GSTNumber,Ch.LicenceNumber,Ch.IsApproved from Chemist as Ch
join Vendor as V on V.Id=Ch.Vendor_Id
where ch.IsDeleted=0 and JoiningDate BETWEEN DATEADD(d,-7,
CONVERT(nvarchar(10),GETDATE(),101)) 
AND CONVERT(nvarchar(10),GETDATE(),101) and V.Id="+ VendorId + "";
                var ChemistYMWD_Report = ent.Database.SqlQuery<ChemistReport_By_YMWD>(qry).ToList();
                return Ok(new { ChemistYMWD_Report });

            }
            else if (Timeperiod == "Monthly")
            {
                string qry = @"select Ch.Id,Ch.ChemistId,Ch.ChemistName,V.VendorName as Franchise,Ch.ShopName,Ch.MobileNumber,Ch.EmailId,Ch.Location,Ch.GSTNumber,Ch.LicenceNumber,Ch.IsApproved from Chemist as Ch
join Vendor as V on V.Id=Ch.Vendor_Id
where ch.IsDeleted=0 and JoiningDate BETWEEN DATEADD(d,-30,
CONVERT(nvarchar(10),GETDATE(),101)) 
AND CONVERT(nvarchar(10),GETDATE(),101) and V.Id="+ VendorId + "";
                var ChemistYMWD_Report = ent.Database.SqlQuery<ChemistReport_By_YMWD>(qry).ToList();
                return Ok(new { ChemistYMWD_Report });
            }
            else if (Timeperiod == "Daily")
            {
                string qry = @"select Ch.Id,Ch.ChemistId,Ch.ChemistName,V.VendorName as Franchise,Ch.ShopName,Ch.MobileNumber,Ch.EmailId,Ch.Location,Ch.GSTNumber,Ch.LicenceNumber,Ch.IsApproved from Chemist as Ch
left join Vendor as V on V.Id=Ch.Vendor_Id
where ch.IsDeleted=0 and JoiningDate > CAST(GETDATE() AS DATE) and V.Id="+ VendorId + "";
                var ChemistYMWD_Report = ent.Database.SqlQuery<ChemistReport_By_YMWD>(qry).ToList();
                return Ok(new { ChemistYMWD_Report });
            }
            else if (Timeperiod == "Yearly")
            {
                string query = @"select Ch.Id,Ch.ChemistId,Ch.ChemistName,V.VendorName as Franchise,Ch.ShopName,Ch.MobileNumber,Ch.EmailId,Ch.Location,Ch.GSTNumber,Ch.LicenceNumber,Ch.IsApproved from Chemist as Ch
left join Vendor as V on V.Id=Ch.Vendor_Id
where ch.IsDeleted=0 and JoiningDate > DATEADD(year,-1,GETDATE()) and V.Id="+ VendorId + "";
                var ChemistYMWD_Report = ent.Database.SqlQuery<ChemistReport_By_YMWD>(query).ToList();
                return Ok(new { ChemistYMWD_Report });
            }

            else
            {
                rm.Message = "Record Not Found!!!";
            }
            return Ok(rm);
        }


        //==============GET LAB DATA BY YMWD===================//

        [HttpGet, Route("api/FranchisesApi/GetLabReport_By_YMWD")]

        public IHttpActionResult GetLabReport_By_YMWD(string Timeperiod, int? VendorId)
        {
            if (Timeperiod == "Weekly")
            {
                string qry = @"select l.Id,l.lABId,l.LabName,l.PhoneNumber,l.MobileNumber,l.EmailId,l.Location,l.LicenceNumber,l.GSTNumber,l.AadharNumber,l.IsApproved from Lab as l
join Vendor as v on v.Id=l.Vendor_Id
where l.IsDeleted=0 and l.JoiningDate BETWEEN DATEADD(d,-7,
CONVERT(nvarchar(10),GETDATE(),101)) 
AND CONVERT(nvarchar(10),GETDATE(),101) AND v.Id="+VendorId+"";
                var LabYMWD_Report = ent.Database.SqlQuery<LabReport_By_MYWD>(qry).ToList();
                return Ok(new { LabYMWD_Report });

            }
            else if (Timeperiod == "Monthly")
            {
                string qry = @"select l.Id,l.lABId,l.LabName,l.PhoneNumber,l.MobileNumber,l.EmailId,l.Location,l.LicenceNumber,l.GSTNumber,l.AadharNumber,l.IsApproved from Lab as l
join Vendor as v on v.Id=l.Vendor_Id
where l.IsDeleted=0 and l.JoiningDate BETWEEN DATEADD(d,-30,
CONVERT(nvarchar(10),GETDATE(),101)) 
AND CONVERT(nvarchar(10),GETDATE(),101) AND v.Id="+VendorId+"";
                var LabYMWD_Report = ent.Database.SqlQuery<LabReport_By_MYWD>(qry).ToList();
                return Ok(new { LabYMWD_Report });
            }
            else if (Timeperiod == "Daily")
            {
                string qry = @"select l.Id,l.lABId,l.LabName,l.PhoneNumber,l.MobileNumber,l.EmailId,l.Location,l.LicenceNumber,l.GSTNumber,l.AadharNumber,l.IsApproved from Lab as l
join Vendor as v on v.Id=l.Vendor_Id
where l.IsDeleted=0 and l.JoiningDate > CAST(GETDATE() AS DATE) AND v.Id="+VendorId+"";
                var LabYMWD_Report = ent.Database.SqlQuery<LabReport_By_MYWD>(qry).ToList();
                return Ok(new { LabYMWD_Report });
            }
            else if (Timeperiod == "Yearly")
            {
                string query = @"select l.Id,l.lABId,l.LabName,l.PhoneNumber,l.MobileNumber,l.EmailId,l.Location,l.LicenceNumber,l.GSTNumber,l.AadharNumber,l.IsApproved from Lab as l
join Vendor as v on v.Id=l.Vendor_Id
where l.IsDeleted=0 and l.JoiningDate > DATEADD(year,-1,GETDATE()) AND v.Id="+VendorId+"";
                var LabYMWD_Report = ent.Database.SqlQuery<LabReport_By_MYWD>(query).ToList();
                return Ok(new { LabYMWD_Report });
            }

            else
            {
                rm.Message = "Record Not Found!!!";
            }
            return Ok(rm);
        }


        //==============GET NURSE DATA BY YMWD===================//

        [HttpGet, Route("api/FranchisesApi/GetNurseReport_By_YMWD")]

        public IHttpActionResult GetNurseReport_By_YMWD(string Timeperiod, int? VendorId)
        {
            if (Timeperiod == "Weekly")
            {
                string qry = @"select SUM(P.TotalFee ) as TotalAmount,n.NurseId,n.NurseName,nt.NurseTypeName from NurseService P
join Nurse n on n.Id = p.Nurse_Id 
join Vendor ve on ve.Id = n.Vendor_Id
join NurseType as NT on NT.Id=N.NurseType_Id
where Convert(Date,p.ServiceAcceptanceDate)  between DATEADD(day,-7,GETDATE()) and GetDate()
and P.IsPaid=1 and ve.Id=" + VendorId + " GROUP BY n.NurseId,n.NurseName,nt.NurseTypeName";
                var NurseYMWD_Report = ent.Database.SqlQuery<NurseReport_By_MYWD>(qry).ToList();
                return Ok(new { NurseYMWD_Report });

            }
            else if (Timeperiod == "Monthly")
            {
                string qry = @"select SUM(P.TotalFee ) as TotalAmount,n.NurseId,n.NurseName,nt.NurseTypeName from NurseService P
join Nurse n on n.Id = p.Nurse_Id 
join Vendor ve on ve.Id = n.Vendor_Id
join NurseType as NT on NT.Id=N.NurseType_Id
where Month(p.ServiceAcceptanceDate) = Month(GetDate())
and P.IsPaid=1 and ve.Id=" + VendorId + " GROUP BY n.NurseId,n.NurseName,nt.NurseTypeName";
                var NurseYMWD_Report = ent.Database.SqlQuery<NurseReport_By_MYWD>(qry).ToList();
                return Ok(new { NurseYMWD_Report });
            }
            else if (Timeperiod == "Daily")
            {
                string qry = @"select SUM(P.TotalFee ) as TotalAmount,n.NurseId,n.NurseName,nt.NurseTypeName from NurseService P
join Nurse n on n.Id = p.Nurse_Id 
join Vendor ve on ve.Id = n.Vendor_Id
join NurseType as NT on NT.Id=N.NurseType_Id
where CONVERT(DATE, P.ServiceAcceptanceDate) >= CAST(GETDATE() AS DATE)
and P.IsPaid=1 and ve.Id=" + VendorId+" GROUP BY n.NurseId,n.NurseName,nt.NurseTypeName";
                var NurseYMWD_Report = ent.Database.SqlQuery<NurseReport_By_MYWD>(qry).ToList();
                return Ok(new { NurseYMWD_Report });
            }
            else if (Timeperiod == "Yearly")
            {
                string query = @"select SUM(P.TotalFee ) as TotalAmount,n.NurseId,n.NurseName,nt.NurseTypeName from NurseService P
join Nurse n on n.Id = p.Nurse_Id 
join Vendor ve on ve.Id = n.Vendor_Id
join NurseType as NT on NT.Id=N.NurseType_Id
where Year(p.ServiceAcceptanceDate) = Year(GetDate())
and P.IsPaid=1 and ve.Id=" + VendorId + " GROUP BY n.NurseId,n.NurseName,nt.NurseTypeName";
                var NurseYMWD_Report = ent.Database.SqlQuery<NurseReport_By_MYWD>(query).ToList();
                return Ok(new { NurseYMWD_Report });
            }

            else
            {
                rm.Message = "Record Not Found!!!";
            }
            return Ok(rm);
        }

        //==============GET DOCTOR DATA BY YMWD===================//

        [HttpGet, Route("api/FranchisesApi/GetDoctorReport_By_YMWD")]

        public IHttpActionResult GetDoctorReport_By_YMWD(string Timeperiod, int? VendorId)
        {
            if (Timeperiod == "Weekly")
            {
                string qry = @"select Sum(P.TotalFee) as Fee,D.DoctorId,D.DoctorName,D.Location,DD.DepartmentName,S.SpecialistName from PatientAppointment P 
join Doctor D on D.Id = p.Doctor_Id 
join Vendor ve on ve.Id = d.Vendor_Id
left join Department as DD on DD.Id=D.Department_Id  
left join Specialist as S on S.Id=D.Specialist_Id
where p.AppointmentDate  between DATEADD(day,-7,GETDATE()) and GetDate() and P.IsPaid=1 and ve.Id=" + VendorId + " GROUP BY D.DoctorId,D.DoctorName,D.Location,DD.DepartmentName,S.SpecialistName";
                var DoctorYMWD_Report = ent.Database.SqlQuery<DoctorReport_By_MYWD>(qry).ToList();
                return Ok(new { DoctorYMWD_Report });

            }
            else if (Timeperiod == "Monthly")
            {
                string qry = @"select Sum(P.TotalFee) as Fee,D.DoctorId,D.DoctorName,D.Location,DD.DepartmentName,S.SpecialistName from PatientAppointment P 
join Doctor D on D.Id = p.Doctor_Id 
join Vendor ve on ve.Id = d.Vendor_Id
left join Department as DD on DD.Id=D.Department_Id  
left join Specialist as S on S.Id=D.Specialist_Id
where Month(p.AppointmentDate) = Month(GetDate()) and P.IsPaid=1 and ve.Id=" + VendorId + " GROUP BY D.DoctorId,D.DoctorName,D.Location,DD.DepartmentName,S.SpecialistName";
                var DoctorYMWD_Report = ent.Database.SqlQuery<DoctorReport_By_MYWD>(qry).ToList();
                return Ok(new { DoctorYMWD_Report });
            }
            else if (Timeperiod == "Daily")
            {
                string qry = @"select Sum(P.TotalFee) as Fee,D.DoctorId,D.DoctorName,D.Location,DD.DepartmentName,S.SpecialistName from PatientAppointment P 
join Doctor D on D.Id = p.Doctor_Id 
join Vendor ve on ve.Id = d.Vendor_Id
left join Department as DD on DD.Id=D.Department_Id  
left join Specialist as S on S.Id=D.Specialist_Id
where Year(p.AppointmentDate) = Year(GetDate()) and P.IsPaid=1 and ve.Id=" + VendorId+" GROUP BY D.DoctorId,D.DoctorName,D.Location,DD.DepartmentName,S.SpecialistName";
                var DoctorYMWD_Report = ent.Database.SqlQuery<DoctorReport_By_MYWD>(qry).ToList();
                return Ok(new { DoctorYMWD_Report });
            }
            else if (Timeperiod == "Yearly")
            {
                string query = @"select Sum(P.TotalFee) as Fee,D.DoctorId,D.DoctorName,D.Location,DD.DepartmentName,S.SpecialistName from PatientAppointment P 
join Doctor D on D.Id = p.Doctor_Id 
join Vendor ve on ve.Id = d.Vendor_Id
left join Department as DD on DD.Id=D.Department_Id  
left join Specialist as S on S.Id=D.Specialist_Id
where Convert(Date, P.AppointmentDate) = Convert(Date,GETDATE()) and P.IsPaid=1 and ve.Id=" + VendorId + " GROUP BY D.DoctorId,D.DoctorName,D.Location,DD.DepartmentName,S.SpecialistName";
                var DoctorYMWD_Report = ent.Database.SqlQuery<DoctorReport_By_MYWD>(query).ToList();
                return Ok(new { DoctorYMWD_Report });
            }

            else
            {
                rm.Message = "Record Not Found!!!";
            }
            return Ok(rm);
        }

        //==============GET VEHICLE DATA BY YMWD===================//

        [HttpGet, Route("api/FranchisesApi/GetVehicleReport_By_YMWD")]

        public IHttpActionResult GetVehicleReport_By_YMWD(string Timeperiod,int?VendorId)
        {
            if (Timeperiod == "Weekly")
            {
                string qry = @"select distinct v.Id,MCY.CategoryName,SUM(trm.Amount) as TotalAmount,v.VehicleNumber,v.VehicleOwnerName,vt.VehicleTypeName as Type, IsNull(v.VehicleName,'NA') as VehicleName,SUM(trm.Amount) as Amount
from DriverLocation trm 
JOIN Driver d ON d.Id = trm.Driver_Id
JOIN Vehicle v ON v.Id = d.Vehicle_Id
join Vendor ve on ve.Id = v.Vendor_Id
join VehicleType as vt on vt.Id=v.VehicleType_Id
left join MainCategory as MCY on MCY.Id=v.VehicleCat_Id
where trm.EntryDate between DateAdd(DD,-7,GETDATE()) and GETDATE() and trm.IsPay='Y' and ve.Id=" + VendorId + " group by v.VehicleNumber, v.VehicleName,v.Id,v.VehicleOwnerName,vt.VehicleTypeName,MCY.CategoryName";
                var VehicleYMWD_Report = ent.Database.SqlQuery<VehicleReport_By_MYWD>(qry).ToList();
                return Ok(new { VehicleYMWD_Report });

            }
            else if (Timeperiod == "Monthly")
            {
                string qry = @"select distinct v.Id,v.VehicleNumber,MCY.CategoryName,SUM(trm.Amount) as TotalAmount,v.VehicleOwnerName,vt.VehicleTypeName as Type, IsNull(v.VehicleName,'NA') as VehicleName,SUM(trm.Amount) as Amount
from DriverLocation trm 
JOIN Driver d ON d.Id = trm.Driver_Id
JOIN Vehicle v ON v.Id = d.Vehicle_Id
join Vendor ve on ve.Id = v.Vendor_Id
join VehicleType as vt on vt.Id=v.VehicleType_Id
left join MainCategory as MCY on MCY.Id=v.VehicleCat_Id
where Month(trm.EntryDate) = Month(GetDate()) and trm.IsPay='Y' and ve.Id=" + VendorId + " group by v.VehicleNumber, v.VehicleName,v.Id,v.VehicleOwnerName,vt.VehicleTypeName,MCY.CategoryName";
                var VehicleYMWD_Report = ent.Database.SqlQuery<VehicleReport_By_MYWD>(qry).ToList();
                return Ok(new { VehicleYMWD_Report });
            }
            else if (Timeperiod == "Daily")
            {
                string qry = @"select distinct v.Id,v.VehicleNumber,MCY.CategoryName,SUM(trm.Amount) as TotalAmount,v.VehicleOwnerName,vt.VehicleTypeName as Type, IsNull(v.VehicleName,'NA') as VehicleName,SUM(trm.Amount) as Amount
from DriverLocation trm 
JOIN Driver d ON d.Id = trm.Driver_Id
JOIN Vehicle v ON v.Id = d.Vehicle_Id
join Vendor ve on ve.Id = v.Vendor_Id
join VehicleType as vt on vt.Id=v.VehicleType_Id
left join MainCategory as MCY on MCY.Id=v.VehicleCat_Id
where trm.EntryDate > CAST(GETDATE() AS DATE) and trm.IsPay='Y' and ve.Id=" + VendorId+ " group by v.VehicleNumber, v.VehicleName,v.Id,v.VehicleOwnerName,vt.VehicleTypeName,MCY.CategoryName";
                var VehicleYMWD_Report = ent.Database.SqlQuery<VehicleReport_By_MYWD>(qry).ToList();
                return Ok(new { VehicleYMWD_Report });
            }
            else if (Timeperiod == "Yearly")
            {
                string query = @"select distinct v.Id,v.VehicleNumber,SUM(trm.Amount) as TotalAmount,v.VehicleOwnerName,vt.VehicleTypeName as Type, IsNull(v.VehicleName,'NA') as VehicleName,MCY.CategoryName,SUM(trm.Amount) as Amount
from DriverLocation trm 
JOIN Driver d ON d.Id = trm.Driver_Id
JOIN Vehicle v ON v.Id = d.Vehicle_Id
join Vendor ve on ve.Id = v.Vendor_Id
join VehicleType as vt on vt.Id=v.VehicleType_Id
left join MainCategory as MCY on MCY.Id=v.VehicleCat_Id
where Year(trm.EntryDate) = Year(GetDate()) and trm.IsPay='Y' and ve.Id=" + VendorId + " group by v.VehicleNumber, v.VehicleName,v.Id,v.VehicleOwnerName,vt.VehicleTypeName,MCY.CategoryName";
                var VehicleYMWD_Report = ent.Database.SqlQuery<VehicleReport_By_MYWD>(query).ToList();
                return Ok(new { VehicleYMWD_Report });
            }
            else
            {
                rm.Message = "Record Not Found!!!";
            }
            return Ok(rm);
        }

        //==============TOTAL TDS AMOUNT BY ROLE===================//

        //[HttpGet, Route("api/FranchisesApi/TotalTDSAmount_ByRole")]
        //public IHttpActionResult TotalTDSAmount_ByRole(string Role)
        //{
        //    if (Role == "Chemist")//chemist
        //    {
        //        string qry = @"select sum(amount) as Amount from ChemistPayout";
        //        var TotalTDSAmount = ent.Database.SqlQuery<TotalTDSAmount>(qry).FirstOrDefault();
        //        return Ok(TotalTDSAmount);
        //    }
        //    else if (Role == "Nurse")//Nurse
        //    {
        //        string qry = @"select sum(amount) as Amount from NursePayout";
        //        var TotalTDSAmount = ent.Database.SqlQuery<TotalTDSAmount>(qry).FirstOrDefault();
        //        return Ok(TotalTDSAmount);
        //    }
        //    else if (Role == "Franchise")//Vendor(franchise)
        //    {
        //        string qry = @"select sum(amount) as Amount from VendorPayOut";
        //        var TotalTDSAmount = ent.Database.SqlQuery<TotalTDSAmount>(qry).FirstOrDefault();
        //        return Ok(TotalTDSAmount);
        //    }
        //    else if (Role == "Lab")//lab
        //    {
        //        string query = @"select sum(amount) as Amount from LabPayout";
        //        var TotalTDSAmount = ent.Database.SqlQuery<TotalTDSAmount>(query).FirstOrDefault();
        //        return Ok(TotalTDSAmount);
        //    }
        //    else if (Role == "Doctor")//doctor
        //    {
        //        string query = @"select sum(amount) as Amount from DoctorPayOut";
        //        var TotalTDSAmount = ent.Database.SqlQuery<TotalTDSAmount>(query).FirstOrDefault();
        //        return Ok(TotalTDSAmount);
        //    }
        //    else
        //    {
        //        rm.Message = "Record Not Found!!!";
        //    }
        //    return Ok(rm);
        //}

        //==============TDS REPORT By To Date From Date===================//

        [HttpGet, Route("api/FranchisesApi/GetTDSData_ByToDateFromDate")]

        public IHttpActionResult GetTDSData_ByToDateFromDate(string Role, string FromDate, string ToDate,int? VendorId)
        {
            try
            {
                if (FromDate.CompareTo(ToDate) <= 0)
                {
                    if (Role == "Chemist")//chemist
                    {
                        string qry = @"select Ch.Id,Ch.ChemistName as Name,CP.Amount as PaidFees,CP.Id as PaymentId,Ch.Location,CONVERT(VARCHAR(11), CP.PaymentDate, 103) AS PaymentDate,FORMAT(CP.PaymentDate, 'hh:mm') AS PaymentTime,((CP.Amount*10)/100) AS TDS ,(CP.Amount- ((CP.Amount*10)/100)) PayAmount from ChemistPayOut as CP left join Chemist as Ch on CP.Chemist_Id=ch.Id where PaymentDate BETWEEN '" + FromDate + "' AND '" + ToDate + "'";
                        var TDSReport = ent.Database.SqlQuery<TDS_Report>(qry).ToList();
                        return Ok(new { TDSReport });

                    }
                    else if (Role == "Nurse")//Nurse
                    {
                        double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='" + Role + "'").FirstOrDefault();

                        string query = @"select A.Nurse_Id,D.NurseId as UniqueId, D.NurseName as Name,sm.StateName+','+ cm.CityName+','+d.Location as Location, SUM(A.TotalFee)  As PaidFees from dbo.NurseService A 
join Nurse D on D.Id = A.Nurse_Id
join Vendor as v on v.Id=D.Vendor_Id
left join StateMaster sm on sm.Id=D.StateMaster_Id
left join CityMaster cm on cm.Id=D.CityMaster_Id
where A.IsPaid=1 and A.ServiceDate BETWEEN '"+FromDate+"' AND '"+ToDate+"' and D.Vendor_Id=" + VendorId + " group by  D.NurseName, A.Nurse_Id,NurseId,D.Location,sm.StateName,cm.CityName";
                        var TDSReport = ent.Database.SqlQuery<TDS_Report>(query).ToList();
                        foreach (var item in TDSReport)
                        {
                            item.tdsamt = (item.PaidFees * tds) / 100;

                            item.PayableAmount = item.PaidFees - (item.tdsamt);
                        }
                        return Ok(new { TDSReport });
                    } 
                    else if (Role == "Lab")//lab
                    {
                        double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='" + Role + "'").FirstOrDefault();

                        string query = @"select A.Lab_Id,D.lABId as UniqueId, D.LabName as Name,sm.StateName+','+ cm.CityName+','+d.Location as Location, SUM(A.Amount)  As PaidFees from dbo.BookTestLab A 
join Lab D on D.Id = A.Lab_Id
join Vendor as v on v.Id=D.Vendor_Id
left join StateMaster sm on sm.Id=D.StateMaster_Id
left join CityMaster cm on cm.Id=D.CityMaster_Id
where A.IsPaid=1 and A.TestDate BETWEEN '"+FromDate+"' AND '"+ToDate+"' and D.Vendor_Id=" + VendorId + " group by  D.LabName, A.Lab_Id,D.lABId,D.Location,sm.StateName,cm.CityName";
                        var TDSReport = ent.Database.SqlQuery<TDS_Report>(query).ToList();
                        foreach (var item in TDSReport)
                        {
                            item.tdsamt = (item.PaidFees * tds) / 100;

                            item.PayableAmount = item.PaidFees - (item.tdsamt);
                        }
                        return Ok(new { TDSReport });
                    }
                    else if (Role == "Doctor")//doctor
                    {
                        double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='" + Role + "'").FirstOrDefault();

                        string query = @"select A.Doctor_Id,D.DoctorId as UniqueId, D.DoctorName as Name,sm.StateName+','+ cm.CityName+','+d.Location as Location, SUM(A.TotalFee)  As PaidFees from dbo.PatientAppointment A 
join Doctor D on D.Id = A.Doctor_Id
join Vendor as v on v.Id=D.Vendor_Id
left join StateMaster sm on sm.Id=D.StateMaster_Id
left join CityMaster cm on cm.Id=D.CityMaster_Id
where A.IsPaid=1 and A.AppointmentDate BETWEEN '"+FromDate+"' AND '" + ToDate + "' and D.Vendor_Id="+VendorId+" group by  D.DoctorName, A.Doctor_Id,D.DoctorId,D.Location,sm.StateName,cm.CityName";
                        var TDSReport = ent.Database.SqlQuery<TDS_Report>(query).ToList();
                        foreach (var item in TDSReport)
                        {
                            item.tdsamt = (item.PaidFees * tds) / 100;

                            item.PayableAmount = item.PaidFees - (item.tdsamt);
                        }
                        return Ok(new { TDSReport });
                    }
                    else if (Role == "Ambulance")//Ambulance
                    {
                        double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='" + Role + "'").FirstOrDefault();

                        string query = @"select A.Driver_Id,D.DriverId as UniqueId, D.DriverName as Name,sm.StateName+','+ cm.CityName+','+d.Location as Location, CAST(SUM(A.TotalPrice) AS float)  As PaidFees from dbo.DriverLocation A 
join Driver D on D.Id = A.Driver_Id
join Vendor as v on v.Id=D.Vendor_Id
left join StateMaster sm on sm.Id=D.StateMaster_Id
left join CityMaster cm on cm.Id=D.CityMaster_Id
where A.IsPay='Y' and A.EntryDate BETWEEN '"+FromDate+"' AND '"+ToDate+"' and D.Vendor_Id=" + VendorId + " group by  D.DriverName, A.Driver_Id,D.DriverId,D.Location,sm.StateName,cm.CityName";
                        var TDSReport = ent.Database.SqlQuery<TDS_Report>(query).ToList();
                        foreach (var item in TDSReport)
                        {
                            item.tdsamt = (item.PaidFees * tds) / 100;

                            item.PayableAmount = item.PaidFees - (item.tdsamt);
                        }
                        return Ok(new { TDSReport });
                    }
                    else
                    {
                        return BadRequest("Record Not Found!!!");
                        //rm.Message = "Record Not Found!!!";
                    }
                }
                else
                {
                    return BadRequest("From Date smaller than or equal To Date");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //==============COMMISSION REPORT BY From Date And To Date===================//

        [HttpGet, Route("api/FranchisesApi/GetCommissionData_ByToDateFromDate")]
        public IHttpActionResult GetCommissionData_ByToDateFromDate(string Role, string FromDate, string ToDate,int? VendorId)
        {
            try
            {
                if (FromDate.CompareTo(ToDate) <= 0)
                {
                    if (Role == "Chemist")//chemist
                    {
                        string qry = @"select Ch.Id,Ch.ChemistName as Name,CP.Amount as PaidFees,CP.Id as PaymentId,Ch.Location,CONVERT(VARCHAR(11), CP.PaymentDate, 103) AS PaymentDate,FORMAT(CP.PaymentDate, 'hh:mm') AS PaymentTime,((CP.Amount*10)/100) AS Commission ,(CP.Amount- ((CP.Amount*10)/100)) PayAmount from ChemistPayOut as CP left join Chemist as Ch on CP.Chemist_Id=ch.Id where ch.IsDeleted=0 and PaymentDate BETWEEN '" + FromDate + "' AND '" + ToDate + "'";
                        var CommissionReport = ent.Database.SqlQuery<Commission_Report>(qry).ToList();
                        return Ok(new { CommissionReport });

                    }
                    else if (Role == "Nurse")//Nurse
                    {
                        double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Nurse'").FirstOrDefault();
                        double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Nurse'").FirstOrDefault();
                        double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Nurse'").FirstOrDefault();

                        string qry = @"select n.Id,n.NurseId as UniqueId,sm.StateName +','+cm.CityName +','+n.Location as Location,ns.Nurse_Id,n.NurseName as Name,CAST(SUM(ns.TotalFee) AS float) AS PaidFees from NurseService as ns
join Nurse as n on n.Id=ns.Nurse_Id
left join StateMaster sm on sm.Id=n.StateMaster_Id
left join CityMaster cm on cm.Id=n.CityMaster_Id
join Vendor as v on v.Id=n.Vendor_Id
where n.IsDeleted=0 and ns.ServiceDate BETWEEN '" + FromDate + "' AND '" + ToDate + "' and n.Vendor_Id="+VendorId+ " GROUP BY  n.NurseId, ns.Nurse_Id,n.NurseName,n.Location,sm.StateName,cm.CityName,n.Id;";
                        var CommissionReport = ent.Database.SqlQuery<Commission_Report>(qry).ToList();

                        foreach (var item in CommissionReport)
                        {
                            item.commamt = (item.PaidFees * commision) / 100;
                            item.tdsamt = (item.PaidFees * tds) / 100;
                            item.transactionamt = (item.PaidFees * Transactionfee) / 100;

                            var razorcomm = (item.PaidFees * Transactionfee) / 100;
                            var totalrazorcomm = razorcomm;
                            item.Amountwithrazorpaycomm = item.PaidFees + totalrazorcomm;

                            item.PayableAmount = item.PaidFees - (item.commamt + item.tdsamt + item.transactionamt);
                            item.FraPaidableamt = (item.PaidFees * 3) / 100;
                        }
                        return Ok(new { CommissionReport });
                    } 
                    else if (Role == "Lab")//lab
                    {
                        double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Lab'").FirstOrDefault();
                        double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Lab'").FirstOrDefault();
                        double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Lab'").FirstOrDefault();

                        string query = @"SELECT D.Id,D.lABId as UniqueId,sm.StateName +','+cm.CityName +','+D.Location as Location,A.Lab_Id,D.LabName as Name,SUM(A.Amount) AS PaidFees FROM BookTestLab A
JOIN Lab D ON D.Id = A.Lab_Id
left join StateMaster sm on sm.Id=D.StateMaster_Id
left join CityMaster cm on cm.Id=D.CityMaster_Id
join Vendor as v on v.Id=d.Vendor_Id
where D.IsDeleted=0 and A.TestDate BETWEEN '" + FromDate + "' AND '" + ToDate + "' AND d.Vendor_Id="+VendorId+" GROUP BY  D.lABId, A.Lab_Id,D.LabName,D.Location,sm.StateName,cm.CityName,D.Id";
                        var CommissionReport = ent.Database.SqlQuery<Commission_Report>(query).ToList();
                        foreach (var item in CommissionReport)
                        {
                            item.commamt = (item.PaidFees * commision) / 100;
                            item.tdsamt = (item.PaidFees * tds) / 100;
                            item.transactionamt = (item.PaidFees * Transactionfee) / 100;

                            var razorcomm = (item.PaidFees * Transactionfee) / 100;
                            var totalrazorcomm = razorcomm;
                            item.Amountwithrazorpaycomm = item.PaidFees + totalrazorcomm;

                            item.PayableAmount = item.PaidFees - (item.commamt + item.tdsamt + item.transactionamt);
                            item.FraPaidableamt = (item.PaidFees * 3) / 100;
                        }
                        return Ok(new { CommissionReport });
                    }
                    else if (Role == "Doctor")//doctor
                    {
                        double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Doctor'").FirstOrDefault();
                        double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault();
                        double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault();

                        //string query = @"select D.Id,D.DoctorName as Name,DP.Amount as PaidFees,DP.Id as PaymentId,D.Location,CONVERT(VARCHAR(11), DP.PaymentDate, 103) AS PaymentDate,FORMAT(DP.PaymentDate, 'hh:mm') AS PaymentTime,((DP.Amount*10)/100) AS Commission ,(DP.Amount- ((DP.Amount*10)/100)) PayAmount From DoctorPayOut as DP left join Doctor as D on DP.Doctor_Id=D.Id where PaymentDate BETWEEN '" + FromDate + "' AND '" + ToDate + "'";
                        string query = @"SELECT D.Id,D.DoctorId as UniqueId,sm.StateName +','+cm.CityName +','+D.Location as Location,A.Doctor_Id,D.DoctorName as Name,SUM(A.TotalFee) AS PaidFees FROM dbo.PatientAppointment A
JOIN Doctor D ON D.Id = A.Doctor_Id
left join StateMaster sm on sm.Id=D.StateMaster_Id
left join CityMaster cm on cm.Id=D.CityMaster_Id
join Vendor as v on v.Id=d.Vendor_Id
where A.AppointmentDate BETWEEN '" + FromDate + "' AND '" + ToDate + "' AND d.Vendor_Id="+VendorId+ " GROUP BY  D.DoctorId, A.Doctor_Id,D.DoctorName,D.Location,sm.StateName,cm.CityName,D.Id;";
                        var CommissionReport = ent.Database.SqlQuery<Commission_Report>(query).ToList();

                        foreach (var item in CommissionReport)
                        {
                            item.commamt = (item.PaidFees * commision) / 100;
                            item.tdsamt = (item.PaidFees * tds) / 100;
                            item.transactionamt = (item.PaidFees * Transactionfee) / 100;

                            var razorcomm = (item.PaidFees * Transactionfee) / 100;
                            var totalrazorcomm = razorcomm;
                            item.Amountwithrazorpaycomm = item.PaidFees + totalrazorcomm;

                            item.PayableAmount = item.PaidFees - (item.commamt + item.tdsamt + item.transactionamt);
                            item.FraPaidableamt = (item.PaidFees * 3) / 100;
                        }
                        return Ok(new { CommissionReport });
                    }
                    else if (Role == "Ambulance")//Ambulance
                    {
                        double Transactionfee = ent.Database.SqlQuery<double>(@"select Fee from TransactionFeeMaster where Name='Ambulance'").FirstOrDefault();
                        double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();
                        double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();
                         
                        string query = @"SELECT D.Id,D.DriverId as UniqueId,sm.StateName +','+cm.CityName +','+D.Location as Location,A.Driver_Id,D.DriverName as Name,CAST(SUM(A.TotalPrice) AS float) AS PaidFees FROM DriverLocation A
JOIN Driver D ON D.Id = A.Driver_Id
left join StateMaster sm on sm.Id=D.StateMaster_Id
left join CityMaster cm on cm.Id=D.CityMaster_Id
join Vendor as v on v.Id=d.Vendor_Id
where A.EntryDate BETWEEN '" + FromDate + "' AND '" + ToDate + "' AND d.Vendor_Id=" + VendorId + " GROUP BY  D.DriverId, A.Driver_Id,D.DriverName,D.Location,sm.StateName,cm.CityName,D.Id";
                        var CommissionReport = ent.Database.SqlQuery<Commission_Report>(query).ToList();

                        foreach (var item in CommissionReport)
                        {
                            item.commamt = (item.PaidFees * commision) / 100;
                            item.tdsamt = (item.PaidFees * tds) / 100;
                            item.transactionamt = (item.PaidFees * Transactionfee) / 100;

                            var razorcomm = (item.PaidFees * Transactionfee) / 100;
                            var totalrazorcomm = razorcomm;
                            item.Amountwithrazorpaycomm = item.PaidFees + totalrazorcomm;

                            item.PayableAmount = item.PaidFees - (item.commamt + item.tdsamt + item.transactionamt);
                            item.FraPaidableamt = (item.PaidFees * 3) / 100;
                        }
                        return Ok(new { CommissionReport });
                    }
                    else
                    {
                        return BadRequest("Record Not Found!!!");
                    }
                }

                else
                {
                    return BadRequest("From Date smaller than or equal To Date");
                }
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //========================PAYOUT HISTORY BY TO DATE FROM DATE==================//

        [HttpGet]
        [Route("api/FranchisesApi/PayoutHistory_ByTodateFromdate")]

        public IHttpActionResult PayoutHistory_ByTodateFromdate(string FromDate, string ToDate)
        {
            try
            {
                if (FromDate.CompareTo(ToDate) <= 0)
                {
                    string qry = @"select VP.Id,V.VendorName,VP.Amount,V.Location,VP.PaymentDate from VendorPayOut as VP left join Vendor as V on V.Id=VP.Vendor_Id Where PaymentDate BETWEEN '" + FromDate + "' AND '" + ToDate + "'";
                    var PayoutHistory = ent.Database.SqlQuery<Fra_payout_his>(qry).ToList();
                    return Ok(new { PayoutHistory });
                }
                else
                {
                    return BadRequest("FromDate smaller than or equal to ToDate");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            } 

        }

        //==============TOTAL TDS AMOUNT BY FROM DATE TO DATE===================//

        [HttpGet, Route("api/FranchisesApi/TotalTDSAmount_ByFromTodate")]
        public IHttpActionResult TotalTDSAmount_ByFromTodate(string Role, string FromDate, string ToDate,int? VendorId)
        {
            if (FromDate.CompareTo(ToDate) <= 0)
            {
                if (Role == "Chemist")//chemist
                {
                    double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Chemist'").FirstOrDefault(); 

                    string qry = @"select sum((Amount*2)/100) as TotalTDSAmount from ChemistPayout Where PaymentDate Between '" + FromDate + "' and '" + ToDate + "'";
                    var TotalTDSAmount = ent.Database.SqlQuery<Total_TDSAmount>(qry).FirstOrDefault();
                    return Ok(TotalTDSAmount);
                }
                else if (Role == "Nurse")//Nurse
                {
                    double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Nurse'").FirstOrDefault();

                    string qry = $"select sum((ns.TotalFee*{tds})/100) as TotalTDSAmount from NurseService ns join Nurse as n on n.Id=ns.Nurse_Id join Vendor as v on v.Id=n.Vendor_Id Where ns.ServiceDate Between '" + FromDate + "' and '" + ToDate + "' and n.Vendor_Id=" + VendorId + " and ns.IsPaid=1";
                    var TotalTDSAmount = ent.Database.SqlQuery<Total_TDSAmount>(qry).FirstOrDefault();
                    return Ok(TotalTDSAmount);
                } 
                else if (Role == "Lab")//lab
                {
                    double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Lab'").FirstOrDefault();

                    string query = $"select sum((ns.Amount*{tds})/100) as TotalTDSAmount from BookTestLab ns join Lab as n on n.Id=ns.Lab_Id join Vendor as v on v.Id=n.Vendor_Id Where ns.TestDate Between '" + FromDate + "' and '" + ToDate + "' and n.Vendor_Id=" + VendorId + " and ns.IsPaid=1";
                    var TotalTDSAmount = ent.Database.SqlQuery<Total_TDSAmount>(query).FirstOrDefault();
                    return Ok(TotalTDSAmount);
                }
                else if (Role == "Doctor")//doctor
                {
                    double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault();
                    
                    string query = $"select sum((pa.TotalFee*{tds})/100) as TotalTDSAmount from PatientAppointment pa join Doctor as d on d.Id=pa.Doctor_Id join Vendor as v on v.Id=d.Vendor_Id Where AppointmentDate Between '" + FromDate + "' and '" + ToDate + "' And d.Vendor_Id=" + VendorId + " and pa.IsPaid=1";
                    var TotalTDSAmount = ent.Database.SqlQuery<Total_TDSAmount>(query).FirstOrDefault();
                    return Ok(TotalTDSAmount);
                }
                else if (Role == "Ambulance")//doctor
                {
                    double tds = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();
                    string query = $"select sum((pa.TotalPrice*{tds})/100) as TotalTDSAmount from DriverLocation pa join Driver as d on d.Id=pa.Driver_Id join Vendor as v on v.Id=d.Vendor_Id Where EntryDate Between '" + FromDate + "' and '" + ToDate + "' And d.Vendor_Id=" + VendorId + " and pa.IsPaY='Y'";
                    var TotalTDSAmount = ent.Database.SqlQuery<Total_TDSAmount>(query).FirstOrDefault();
                    return Ok(TotalTDSAmount);
                }
                else
                {
                    rm.Message = "Record Not Found!!!";
                }
            }
            else
            {
                return BadRequest("From Date smaller than or equal To Date");
            }
            return Ok(rm);
        }

        //==============TOTAL COMMISSION AMOUNT BY FROM DATE TO DATE===================//

        [HttpGet, Route("api/FranchisesApi/TotalCommissionAmount_ByFromTodate")]
        public IHttpActionResult TotalCommissionAmount_ByFromTodate(string Role, string FromDate, string ToDate, int? VendorId)
        {
            if (FromDate.CompareTo(ToDate) <= 0)
            {
                if (Role == "Chemist")//chemist
                {
                    double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Chemist'").FirstOrDefault();

                    string qry = @"select sum(Amount- ((Amount*10)/100)) as TotalCommissionAmount from ChemistPayout Where PaymentDate Between '" + FromDate + "' and '" + ToDate + "'";
                    var TotalCommissionAmount = ent.Database.SqlQuery<Total_CommissionAmount>(qry).FirstOrDefault();
                    return Ok(TotalCommissionAmount);
                }
                else if (Role == "Nurse")//Nurse
                {
                    double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Nurse'").FirstOrDefault();

                    string qry = $"select sum((ns.TotalFee*{commision})/100) as TotalCommissionAmount from NurseService ns join Nurse as n on n.Id=ns.Nurse_Id join Vendor as v on v.Id=n.Vendor_Id Where ns.ServiceDate Between '" + FromDate + "' and '" + ToDate + "' and n.Vendor_Id="+VendorId+ " and ns.IsPaid=1";
                    var TotalCommissionAmount = ent.Database.SqlQuery<Total_CommissionAmount>(qry).FirstOrDefault();
                    return Ok(TotalCommissionAmount);
                }                
                else if (Role == "Lab")//lab
                {
                    double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Lab'").FirstOrDefault();

                    string query = $"select sum((ns.Amount*{commision})/100) as TotalCommissionAmount from BookTestLab ns join Lab as n on n.Id=ns.Lab_Id join Vendor as v on v.Id=n.Vendor_Id Where ns.TestDate Between '" + FromDate + "' and '" + ToDate + "' and n.Vendor_Id="+VendorId+ " and ns.IsPaid=1";
                    var TotalTDSAmount = ent.Database.SqlQuery<Total_CommissionAmount>(query).FirstOrDefault();
                    return Ok(TotalTDSAmount);
                }
                else if (Role == "Doctor")//doctor
                {                    
                    double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault(); 

                   // string query = @"select sum(Amount- ((Amount*10)/100)) as TotalCommissionAmount from DoctorPayOut Where PaymentDate Between '" + FromDate + "' and '" + ToDate + "'";
                    string query = $"select sum((pa.TotalFee*{commision})/100) as TotalCommissionAmount from PatientAppointment pa join Doctor as d on d.Id=pa.Doctor_Id join Vendor as v on v.Id=d.Vendor_Id Where AppointmentDate Between '" + FromDate + "' and '" + ToDate + "' And d.Vendor_Id="+VendorId+ " and pa.IsPaid=1";
                    var TotalTDSAmount = ent.Database.SqlQuery<Total_CommissionAmount>(query).FirstOrDefault();
                    return Ok(TotalTDSAmount);
                }
                else if (Role == "Ambulance")//Ambulance
                {                    
                    double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Doctor'").FirstOrDefault(); 
                     
                    string query = $"select cast(sum((pa.TotalPrice*{commision})/100)as float) as TotalCommissionAmount from DriverLocation pa join Driver as d on d.Id=pa.Driver_Id join Vendor as v on v.Id=d.Vendor_Id Where EntryDate Between '" + FromDate + "' and '" + ToDate + "' And d.Vendor_Id="+VendorId+ " and pa.IsPay='Y'";
                    var TotalTDSAmount = ent.Database.SqlQuery<Total_CommissionAmount>(query).FirstOrDefault();
                    return Ok(TotalTDSAmount);
                }
                else
                {
                    rm.Message = "Record Not Found!!!";
                }
            }
            else
            {
                return BadRequest("From Date smaller than or equal To Date");
            }
            return Ok(rm);
        }

        [HttpPost, Route("api/FranchisesApi/AddVehCat_Type")]
        public IHttpActionResult AddVehCat_Type(AddCat_Vehicletype model)
        {
            try
            {
                var existingCategory = ent.MainCategories.FirstOrDefault(a => a.CategoryName == model.CategoryName);

                if (existingCategory != null)
                {
                    var data1 = new VehicleType()
                    {
                        Category_Id = existingCategory.Id,
                        VehicleTypeName = model.VehicleTypeName,
                        IsDeleted = false,
                        IsApproved = false
                    };

                    ent.VehicleTypes.Add(data1);
                    ent.SaveChanges();

                    rm.Status = 1;
                    rm.Message = "New VehicleType Added Successfully";
                }
                else
                {
                    var newCategory = new MainCategory()
                    {
                        CategoryName = model.CategoryName,
                        IsDeleted = false
                    };

                    ent.MainCategories.Add(newCategory);
                    ent.SaveChanges();

                    var data1 = new VehicleType()
                    {
                        Category_Id = newCategory.Id,
                        VehicleTypeName = model.VehicleTypeName,
                        IsDeleted = false
                    };

                    ent.VehicleTypes.Add(data1);
                    ent.SaveChanges();
                    rm.Status = 1;
                    rm.Message = "New Category and VehicleType Added Successfully";
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }



		[HttpPost, Route("api/FranchisesApi/VehicleAllotment")]		 

		public IHttpActionResult VehicleAllotment(VehicleAllotmentDTO model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{ 
				var list = ent.Vehicles.Where(a => a.Id == model.VehicleNumberId).ToList();
				var vehicle = list.FirstOrDefault();
                var getexistVehicle = ent.Drivers.Where(d => d.Vehicle_Id == model.VehicleNumberId && d.IsDeleted==false).FirstOrDefault();
				
				if (getexistVehicle!=null)
                {
					string ExistdriverName = ent.Database.SqlQuery<string>("select DriverName from Driver where Vehicle_Id=" + model.VehicleNumberId).FirstOrDefault();
					rm.Message= "The Selected Vehicle is Already Running on " + ExistdriverName;

				}
                else
                {
					if (vehicle == null)
					{
						return BadRequest("Vehicle not found");
					} 

					var vehicleId = vehicle.Id;
					var vehicleNumber = vehicle.VehicleNumber;


					string updateVehicleQuery = $"update Vehicle set Driver_Id = {model.DriverId} where Id = {vehicleId}";
					ent.Database.ExecuteSqlCommand(updateVehicleQuery);

					string updateDriverQuery = $"update Driver set VehicleType_Id = {model.VehicleTypeId},Vehicle_Id={model.VehicleNumberId} where Id = {model.DriverId}";
					ent.Database.ExecuteSqlCommand(updateDriverQuery);
					string driverName = ent.Database.SqlQuery<string>($"select DriverName from Driver where Id = {model.DriverId}").FirstOrDefault();

					rm.Message = $"The Vehicle Number {vehicleNumber} has been Replaced to {driverName}";
                    rm.Status = 1;

				}


				return Ok(rm);
			}
			catch (Exception ex)
			{ 
				return InternalServerError(ex);
			}
		}

		[HttpGet, Route("api/FranchisesApi/GetDriverVehicleId")]
		public IHttpActionResult GetDriverVehicleId(int VehicleNumberId)
		{
			string qry = @"Select d.Id,v.VehicleNumber,d.DriverName from Driver as d
join Vehicle as v on v.Id=d.Vehicle_Id where d.Vehicle_Id="+ VehicleNumberId + " and v.IsApproved=1";
			var VehicleNumberdetail = ent.Database.SqlQuery<VehicleNumbers>(qry).ToList();
			return Ok(new { VehicleNumberdetail });
		}
		[HttpGet, Route("api/FranchisesApi/GetDriverForUpdate")]
		public IHttpActionResult GetDriverForUpdate(int VendorId)
		{
			string qry = @"Select D.Id,CONCAT('[', d.DriverId, '] ', d.DriverName) AS DriverName from Driver as D
join Vendor as v on v.Id=d.Vendor_Id
where D.IsDeleted=0 and V.IsDeleted=0 and d.IsApproved=1 and v.Id=" + VendorId + "";
			var Drivers = ent.Database.SqlQuery<DriversName>(qry).ToList();
			return Ok(new { Drivers });
		}

		[HttpPost, Route("api/FranchisesApi/SwapDriver")]
		public IHttpActionResult SwapDriver(VehicleAllotmentDTO model)
		{
			var list = ent.Drivers.Where(a => a.Id == model.DriverId).ToList();
			if (list.Count == 0)
			{
				return Content(HttpStatusCode.NotFound, "Driver not found");
			}

			var driverId = list.First().Id;
			var name = list.First().DriverName;


			//var vehicle = ent.Vehicles.Find(model.Id);

            var vehicle = ent.Vehicles
    .Where(V => V.Id == model.Id)
    .Select(v => new { VehicleTypeId = v.VehicleType_Id, DriverId = v.Driver_Id })
    .ToList();
            var vehicleTypeId = vehicle.FirstOrDefault().VehicleTypeId;
            var GetDriverId = vehicle.FirstOrDefault().DriverId;
            if (vehicle == null)
			{
				return Content(HttpStatusCode.NotFound, "Vehicle not found");
			}


            //==========driver already exist validation============

            //         if (ent.Vehicles.Any(a => a.Driver_Id == driverId))
            //{
            //	string vehicleNumber = ent.Database.SqlQuery<string>("select VehicleNumber from Vehicle where Driver_Id = "+driverId+"").FirstOrDefault();
            //	return Content(HttpStatusCode.BadRequest, "The Selected Driver is Already Running on " + vehicleNumber);
            //}

            string updateexistdriver = @"update Driver set VehicleType_Id = null,Vehicle_Id=null where Id=" + GetDriverId;
            ent.Database.ExecuteSqlCommand(updateexistdriver);
           

            string q = @"update Vehicle set Driver_Id = " + driverId + "  where Id=" + model.Id;
            ent.Database.ExecuteSqlCommand(q);

            var driver = ent.Drivers.Find(driverId);
			if (driver != null)
			{
				driver.VehicleType_Id = vehicleTypeId;
				driver.Vehicle_Id = model.Id;
				ent.SaveChanges();
			}

			string newVehicleNumber = ent.Database.SqlQuery<string>("select VehicleNumber from Vehicle where Driver_Id = "+ driverId + "").FirstOrDefault();
			return Content(HttpStatusCode.OK, "The Vehicle Number " + newVehicleNumber + " has been Replaced to " + name);

		}
	}
}