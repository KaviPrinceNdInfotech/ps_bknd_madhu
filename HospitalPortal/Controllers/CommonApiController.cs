using AutoMapper;
using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HospitalPortal.Controllers
{
    public class CommonApiController : ApiController
    {
        ReturnMessage RM = new ReturnMessage();
        DbEntities ent = new DbEntities();
        [HttpGet]
        public IHttpActionResult GetStates()
        {
            dynamic obj = new ExpandoObject();
            obj.States = Mapper.Map<IEnumerable<StateMasterDTO>>(ent.StateMasters.Where(a=>a.IsDeleted ==false ).OrderBy(a=>a.StateName).ToList());
            return Ok(obj);
        }

        [HttpGet]
        public IHttpActionResult GetCitiesByState(int stateId)
        {
            dynamic obj = new ExpandoObject();
            obj.Cities = Mapper.Map<IEnumerable<CityMasterDTO>>(ent.CityMasters.Where(a=>a.StateMaster_Id==stateId && a.IsDeleted == false).OrderBy(a=>a.CityName).ToList());
            return Ok(obj);
        }

        

        [HttpGet]
        public IHttpActionResult GetLocationByCity(int cityId)
        {
            dynamic obj = new ExpandoObject(); 
            var locs= ent.Locations.Where(a => a.City_Id == cityId).Where(a => a.IsDeleted == false).OrderBy(a => a.LocationName).ToList();
            obj.Locations = Mapper.Map<IEnumerable<LocationDTO>>(locs);
            return Ok(obj);
        }

        [HttpGet]
        public IHttpActionResult GetDepartments()
        {
            dynamic obj = new ExpandoObject();
            obj.Departments = Mapper.Map<IEnumerable<DepartmentDTO>>(ent.Departments.Where(a => a.IsDeleted == false).OrderBy(a => a.DepartmentName).ToList());
            return Ok(obj);
        }

        
        public IHttpActionResult GetSpecialistByDept(int depId)
        {
            dynamic obj = new ExpandoObject();
            obj.Specialist = Mapper.Map<IEnumerable<SpecialistDTO>>(ent.Specialists.Where(a=>a.Department_Id==depId && a.IsDeleted == false).OrderBy(a=>a.SpecialistName).ToList());
            return Ok(obj);
        }

        public IHttpActionResult GetCities()
        {
            dynamic obj = new ExpandoObject();
            obj.Cities = Mapper.Map<IEnumerable<CityMasterDTO>>(ent.CityMasters.Where(a => a.IsDeleted == false).OrderBy(a => a.CityName).ToList());
            return Ok(obj);
        }

        [HttpGet, Route("api/CommonApi/GetSpecialist")]
        public IHttpActionResult GetSpecialist(int depId)
        {
            dynamic obj = new ExpandoObject();
            obj.Specialist = Mapper.Map<IEnumerable<SpecialistDTO>>(ent.Specialists.Where(a=> a.IsDeleted == false && a.Department_Id == depId).OrderBy(a => a.SpecialistName).ToList());
            return Ok(obj);
        }

        [HttpGet]
        public IHttpActionResult TestList()
        {
            dynamic obj = new ExpandoObject();
            obj.Tests = Mapper.Map<IEnumerable<TestArray>>(ent.LabTests.Where(a => a.TestName != null).OrderBy(a => a.TestName).ToList());
            return Ok(obj);
        }

        [HttpGet]
        public IHttpActionResult NurseList()
        {
            dynamic obj = new ExpandoObject();
            obj.Nurse = Mapper.Map<IEnumerable<NurseTypeAPI>>(ent.NurseTypes.Where(a => a.IsDeleted == false).OrderBy(a => a.NurseTypeName).ToList());
            return Ok(obj);
        }

        [HttpGet]
        public IHttpActionResult Content(string PageName)
        {
            var model = new ContentVM();
            string obj = @"select Top 1 * from Content P where P.PageName='" + PageName + "'order by P.Id desc";
            var data = ent.Database.SqlQuery<Contentpage>(obj).ToList();
            if(data.Count() == 0)
            {
                RM.Message = "No Data";
                RM.Status = 0;
            }
            model.content = data;
            return Ok(model);
        }

        [HttpGet]
        public IHttpActionResult GetLocation()
        {
            dynamic obj = new ExpandoObject();
            obj.Location = Mapper.Map<IEnumerable<LocationDTO>>(ent.Locations.Where(a => a.IsDeleted == false).OrderBy(a => a.LocationName).ToList());
            return Ok(obj);
        }
        
        [HttpGet]
        public IHttpActionResult GetComplaint()
        {
            var data1 = new ComplaintDTO();

            string query = @"select Id,Subjects from DoctorComplaints";

            // obj.Complaint = Mapper.Map<IEnumerable<ComplaintDTO>>(ent.DoctorComplaints.Where(a => a.IsDeleted == false).ToList());
            var data = ent.Database.SqlQuery<ComplaintDTO>(query).ToList();
            return Ok(data);
        }


        public IHttpActionResult GetHospitals()
        {
            dynamic obj = new ExpandoObject();
            obj.Hospitals = Mapper.Map<IEnumerable<HospitalDTO>>(ent.Hospitals.ToList());
            return Ok(obj);
        }

        public IHttpActionResult GetVehicles()
        {
            dynamic obj = new ExpandoObject();
            obj.Vehicles = Mapper.Map<IEnumerable<VehicleTypeDTO>>(ent.VehicleTypes.Where(a=> a.IsDeleted == false).OrderBy(a => a.VehicleTypeName).ToList());
            return Ok(obj);
        }

        [HttpGet]
        public IHttpActionResult GetPatientList(int Id)
        {
            var model = new PatientListC();
            string q = @"select PatientName from dbo.PatientAppointment pa join Patient p on pa.Patient_Id = p.Id join Doctor d on d.Id = pa.Doctor_Id  where pa.Doctor_Id="+Id+" and d.IsDeleted=0 and p.IsDeleted=0 group by p.Id, p.PatientName";
            var data = ent.Database.SqlQuery<PatientDetails>(q).ToList();
            model.PatientDetail = data;
            return Ok(data);
        }

        [HttpGet]
        public IHttpActionResult PatientSubjects()
        {
            var model = new ComplaintPatient();
            var qry = @"select * from PatientSubjects";
            var data = ent.Database.SqlQuery<Complaint41Patient>(qry).ToList();
            model.Complaint41Patient = data;
            return Ok(model);
        }

        [HttpGet, Route("api/CommonApi/TimeSlot")]
        public IHttpActionResult TimeSlot()
        {
            var model = new TimeSlotA();
            var qry = @"select * from TimeSlot";
            var data = ent.Database.SqlQuery<TimeSlots>(qry).ToList();
            model.TimeSlots = data;
            return Ok(model);
        }


        //===============LAB UPLOAD REPORT==PATIENTNAME DROPDOWN=================//

        [HttpGet, Route("api/CommonApi/Lab_report")]
        public IHttpActionResult Lab_report(int Id)
        {
            var model = new Patient();
            var qry = @"select p.Id,p.PatientName from Patient as p 
inner join BookTestLab as BT on BT.Patient_Id=p.id
where BT.Lab_Id=" + Id + "";
            var PatientName = ent.Database.SqlQuery<Labrepo>(qry).ToList();

            return Ok(new { PatientName });
        }

        //=====================UPDATE BANK DETAIL===================//
        [HttpPost]
        [Route("api/CommonApi/UpdateBank")]
        public IHttpActionResult UpdateBank(UpdateBankDetailss model)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ",
ModelState.Values
.SelectMany(a => a.Errors)
.Select(a => a.ErrorMessage));
                RM.Message = message;
                RM.Status = 0;
                return Ok(RM);
            }

            var data = ent.BankDetails.Where(a => a.Login_Id == model.Login_Id).ToList();

            string qry = @"update BankDetails set AccountNo='" + model.AccountNo + "',BranchName='" + model.BranchName + "', IFSCCode='" + model.IFSCCode + "', isverified=0 where Login_Id=" + model.Login_Id + "";
            ent.Database.ExecuteSqlCommand(qry);
            RM.Message = "Successfully Updated";
            RM.Status = 1;
            return Ok(RM);
        }

        //==========================Test Dropdown=====================//

        [HttpGet]
        [Route("api/CommonApi/TestDropdown")]
        public IHttpActionResult TestDropdown()
        {
            var data = new LabReport();
            string qry = @"select Id,TestDesc from LabTest where TestDesc !='null'";
            var LabTestName = ent.Database.SqlQuery<Test_name>(qry).ToList();

            return Ok(new { LabTestName });
        }



    }
}
