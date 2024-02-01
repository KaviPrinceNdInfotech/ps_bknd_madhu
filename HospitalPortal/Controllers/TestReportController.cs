using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using log4net;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HospitalPortal.Controllers
{
   
    public class TestReportController : ApiController
    {
        DbEntities ent = new DbEntities();
        ReturnMessage rm = new ReturnMessage();
        ILog log = LogManager.GetLogger(typeof(TestReportController));
        [HttpGet]
        public IHttpActionResult ViewReport(int PatientId, string term)
        {
            var obj = new ExpandoObject();
            try
            {
                if (term == "Health")
                {
                    var model = new FileVM();
                    var query = @"select * from CheckUpReport where Patient_Id=" + PatientId;
                    var data = ent.Database.SqlQuery<LabFiles>(query).ToList();
                    if (data.Count() == 0)
                    {
                        model.LabFile = data;
                        rm.Message = "No Records Found";
                        rm.Status = 0;
                        return Ok(rm);
                    }
                    else
                    {
                        model.LabFile = data;
                        model.Message = "Success";
                        model.Status = 1;
                        return Ok(model);
                    }
                }
               if (term == "Lab")
                {
                    var model = new FileVM();
                    var query = @"select * from LabReport lr join LabTest lt on lt.Id = lr.Test join Patient on Patient.Id= lr.Patient_Id where Patient.Id=" + PatientId;
                    var data = ent.Database.SqlQuery<LabFiles>(query).ToList();
                    if (data.Count() == 0)
                    {
                        model.LabFile = data;
                        rm.Message = "No Records Found";
                        rm.Status = 0;
                        return Ok(rm);
                    }
                    else
                    {
                        model.LabFile = data;
                        model.Message = "Success";
                        model.Status = 1;
                        return Ok(model);
                    }
                }
            }
            catch(Exception ex) {
                string msg = ex.ToString();
                rm.Message = "Server Error";
                rm.Status = 0;
                return Ok(rm);
            }
            rm.Message = "Invaild Attempt";
            rm.Status = 0;
            return Ok(rm);
        }
    }
}
