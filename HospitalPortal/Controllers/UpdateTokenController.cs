using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HospitalPortal.Controllers
{
    public class UpdateTokenController : ApiController
    {
        DbEntities ent = new DbEntities();
        ReturnMessage rm = new ReturnMessage();

        [HttpPost]
        public IHttpActionResult Update(int Id, string deviceId)
        {
            try
            {
                if (Id > 0)
                {
                    string q = @"update AdminLogin set Token ='" + deviceId + "' where Id=" + Id;
                    ent.Database.ExecuteSqlCommand(q);
                    rm.Message = "Successfully Updated";
                    rm.Status = 1;
                    return Ok(rm);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                rm.Message = "Server Error";
                rm.Status = 0;
                return Ok(rm);
            }
            return Ok();
        }


        [HttpPost]
        public IHttpActionResult DeleteToken(int Id)
        {
            try
            {
                if (Id > 0)
                {
                    string q = @"update AdminLogin set Token = null where Id=" + Id;
                    ent.Database.ExecuteSqlCommand(q);
                    rm.Message = "Successfully Updated";
                    rm.Status = 1;
                    return Ok(rm);
                }
            }
            catch (Exception ex)
            {
                rm.Message = "Server Error";
                rm.Status = 0;
                return Ok(rm);
            }
            return Ok();

        }

    }
}
