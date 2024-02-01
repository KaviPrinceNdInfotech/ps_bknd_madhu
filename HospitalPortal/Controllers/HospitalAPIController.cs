using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Repositories;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace HospitalPortal.Controllers
{
 
    public class HospitalAPIController : ApiController
    {
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(HospitalAPIController));
        ReturnMessage rm = new ReturnMessage();
         

        //Hospital Doctors List
        [HttpGet]
        [Route("api/HospitalAPI/DoctorList")]
        public IHttpActionResult DoctorList(int hospitalId, string DoctorName = null)
        {
            var model = new DoctorListVM();
            string query = @"select hd.*,d.DepartmentName,s.SpecialistName,
cm.CityName,sm.StateName,{ fn concat(CONVERT(varchar(15),CAST(StartTime AS TIME),100),
{fn concat ('-', CONVERT(varchar(15),CAST(EndTime AS TIME),100))})} AS AvailableTime
 from Doctor hd
join Department d 
on hd.Department_Id=d.Id
join Specialist s
on hd.Specialist_Id=s.Id
join StateMaster sm
on hd.StateMaster_Id=sm.Id
join CityMaster cm on hd.CityMaster_Id=cm.Id
where hd.IsDeleted=0 and hd.HospitalId=" + hospitalId;
            var data = ent.Database.SqlQuery<ListOfDoctor>(query).ToList();
            if (data.Count() == 0)
            {
                model.Message = "No Records";
                model.Status = 0;
                model.list = data;
                return Ok(model);
            }
            if (DoctorName != null)
            {
                data = data.Where(a => a.DoctorName == DoctorName).ToList();
                if (data.Count() == 0)
                {
                    model.Message = "No Records Present";
                    model.Status = 0;
                    model.list = data;
                    return Ok(model);
                }
            }
            model.Message = "Success";
            model.Status = 1;
            model.list = data;
            return Ok(model);
        }

        //Hospital Nurses List
        [HttpGet]
        [Route("api/HospitalAPI/NurseList")]
        public IHttpActionResult NurseList(int hospitalId, int? typeId = 0 )
        {
            var model = new NurseListVM();
            string q = @"select * from HospitalNurse hn
join NurseType nt on nt.Id = hn.NurseType_Id
where hn.IsDeleted=0 and hn.Hospital_Id="+hospitalId;
            var data = ent.Database.SqlQuery<NurseListof>(q).ToList();
            if(data.Count() == 0)
            {
                model.Message = "No Records";
                model.Status = 0;
                model.NurseList = data;
                return Ok(model);
            }
            if(typeId != 0)
            {
                data = data.Where(a => a.Id == typeId).ToList();
                if (data.Count() == 0)
                {
                    model.Message = "No Records";
                    model.Status = 0;
                    model.NurseList = data;
                    return Ok(model);
                }
            }
            model.Message = "Success";
            model.Status = 1;
            model.NurseList = data;
            return Ok(model);
        }

        [HttpGet]
        [Route("api/HospitalAPI/AppointmentList")]
        public IHttpActionResult AppointmentList(int hosptalId, DateTime? date = null)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            var query = @"select Doctor.Id as Doctor_Id,p.PatientName, p.MobileNumber as PatientMobileNumber,PatientAppointment.Id as AppointmentId, AppointmentDate, Doctor.MobileNumber as MobileNo ,h.Id,
PatientAppointment.Specialist_Id, Specialist.SpecialistName as Specility,
Doctor.DoctorName, h.Location as Address, h.HospitalName, Doctor.DoctorName as DoctornName, { fn concat(CONVERT(varchar(15),CAST(StartSlotTime AS TIME),100),
{fn concat ('-', CONVERT(varchar(15),CAST(EndSlotTime AS TIME),100))})} AS AppointedTime from PatientAppointment 
join Doctor on Doctor.Id = PatientAppointment.Doctor_Id join Specialist on Doctor.Specialist_Id = Specialist.Id
join Hospital h on h.Id = Doctor.HospitalId
join Patient p on p.Id = PatientAppointment.Patient_Id
where Doctor.HospitalId="+hosptalId+" and Convert(Date,AppointmentDate) =  Convert(Date,GETDATE()) order by AppointmentDate desc";
            var data = ent.Database.SqlQuery<AppointmentOFhospital>(query).ToList();
            if (data.Count() == 0)
            {
                dict["Message"] = "No Records";
                dict["Status"] = 0;
                dict["AppointmentList"] = data;
                return Ok(dict);
            }
            if (date != null)
            {
                data = data.Where(a => a.AppointmentDate == date).ToList();
                if (data.Count() == 0)
                {
                    dict["Message"] = "No Records";
                    dict["Status"] = 0;
                    dict["AppointmentList"] = data;
                    return Ok(dict);
                }
            }
            dict["Message"] = "Success";
            dict["Status"] = 1;
            dict["AppointmentList"] = data;
            return Ok(dict);
        }

        [HttpGet]
        [Route("api/HospitalAPI/SearchHospital")]
        public IHttpActionResult SearchHospital(string term)
        {
            try
            {
                term = term.ToLower();
                var hospital = ent.Hospitals.Where(a => !a.IsDeleted && a.HospitalName.ToLower().Contains(term)).ToList();
                var data = (from m in hospital
                            join t in ent.HospitalDoctors
                            on m.Id equals t.Hospital_Id into tm
                            from t1 in tm.DefaultIfEmpty()
                            select new
                            {
                                m.Id,
                                m.HospitalName,
                            }).ToList();
                dynamic obj = new ExpandoObject();
                obj.Hospitals = data;
                return Ok(obj);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/HospitalAPI/ViewFacilities")]
        public IHttpActionResult ViewFacilities(int HospitalId, string FacilityName = null)
        {
            var model = new HospitalFicilityVM();
            var Id = new SqlParameter("@Id", HospitalId);
            string q = @"select * from HopitalFaciltiy where IsDeleted=0 and Hospital_Id= @Id";
            var data = ent.Database.SqlQuery<HospitalFacilities>(q, Id).ToList();
            if(data.Count() == 0)
            {
                model.Message = "No Records Available";
                model.Status = 0;
                model.HospitalFacilities = data;
                return Ok(model);
            }
            if (FacilityName != null)
            {
                data = data.Where(a => a.FacilityName == FacilityName).ToList();
                if (data.Count() == 0)
                {
                    model.Message = "No Records Present";

                    model.Status = 0;
                    model.HospitalFacilities = data;
                    return Ok(model);
                   
                }
            }
            model.Message = "Success";
            model.Status = 1;
            model.HospitalFacilities = data;
            return Ok(model);
        }

        [HttpGet]
        [Route("api/HospitalAPI/LabList")]
        public IHttpActionResult LabList(int HospitalId, string LabName = null)
        {
            var model = new HospitalLabVM();
            var query = @"select * ,{ fn concat(CONVERT(varchar(15),CAST(StartTime AS TIME),100),{fn concat ('-', CONVERT(varchar(15),CAST(EndTime AS TIME),100))})} AS OpeningHours from Lab where HospitalId=" + HospitalId;
            var data = ent.Database
                .SqlQuery<HospitalLabs>(query).ToList();
            if(data.Count() == 0)
            {
                model.Message = "No Records Found";
                model.Status = 0;
                return Ok(model);
            }
            if(LabName != null)
            {
                data = data.Where(a => a.LabName == LabName).ToList();
                if (data.Count() == 0)
                {
                    model.Message = "No Records Present";
                    model.Status = 0;
                    return Ok(model);
                }
            }
            model.Message = "Success";
            model.Status = 1;
            model.HospitalLabs = data;
            return Ok(model);
        }
    }
}
