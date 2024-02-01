using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HospitalPortal.Controllers
{
    public class ForgotPasswordController : ApiController
    {
        DbEntities ent = new DbEntities();
        ReturnMessage rm = new ReturnMessage();
        
        [HttpGet]
        public IHttpActionResult ForgetPwd(string Email)
        {
            string qry = @"select EmailId from Patient where EmailId='" + Email + "'";
            var PEmail = ent.Database.SqlQuery<string>(qry).ToList();
            string qry1 = @"select EmailId from Doctor where EmailId='" + Email + "'";
            var DEmail = ent.Database.SqlQuery<string>(qry1).ToList();
            if (PEmail.Count() != 0)
            {
                int Id = ent.Database.SqlQuery<int>("select Id from Patient where EmailId=" + Email).FirstOrDefault();
                var EmialPatient = ent.Patients.Find(Id);
                if(EmialPatient != null)
                {
                    string password = ent.AdminLogins.Where(a => a.Id == EmialPatient.AdminLogin_Id).Select(a => a.Password).FirstOrDefault();
                    string mail = "Your Password is " + password + " <br /> <b>PS WellNess Team</b>";
                    string msg = "Your Password is " + password + " PS WellNess Team";
                    var mobile = ent.Database.SqlQuery<string>(@"select  ns.MobileNumber from Patient ns where ns.EmailId=" + Email).FirstOrDefault();
                    var EmailId = ent.Database.SqlQuery<string>(@"select  ns.EmailId from Patient ns where ns.EmailId=" + Email).FirstOrDefault();
                    Message.SendSms(mobile, msg);
                    EmailOperations.SendEmail1(EmailId, "Forgot Password Request", mail, true);
                    rm.Message = "SuccessFully Sent";
                    rm.Status = 1;
                    return Ok(rm);
                }
            }
            else if (DEmail.Count() != 0)
            {
                int Id = ent.Database.SqlQuery<int>("select Id from Doctor where EmailId='" + Email + "'").FirstOrDefault();
                var DoctorDetails = ent.Doctors.Find(Id);
                if (DoctorDetails != null)
                {
                    string password = ent.AdminLogins.Where(a => a.Id == DoctorDetails.AdminLogin_Id).Select(a => a.Password).FirstOrDefault();
                    string mail = "Your Password is " + password + " <br /> <b>PS WellNess Team</b>";
                    string msg = "Your Password is " + password + " PS WellNess Team";
                    var EmailId = ent.Database.SqlQuery<string>(@"select  ns.EmailId from Doctor ns where ns.EmailId='" + Email + "'").FirstOrDefault();
                    var mobile = ent.Database.SqlQuery<string>(@"select  ns.MobileNumber from Doctor ns where ns.EmailId='" + Email+ "'").FirstOrDefault();
                    Message.SendSms(mobile, msg);
                    EmailOperations.SendEmail(EmailId, "Forgot Password Request", mail, true);
                    rm.Message = "SuccessFully Sent";
                    rm.Status = 1;
                    return Ok(rm);
                }
            
            }
            else
            {
                rm.Message = "No User Exists of this Email Id";
                rm.Status = 0;
                return Ok(rm);
            }
            //string qry2 = @"select EmailId from Hospital where EmailId=" + Email;
            //string qry3 = @"select EmailId from Driver where EmailId=" + Email;
            return Ok(rm);
        }

        [HttpGet]
        public IHttpActionResult ForgetPwds(string UniqueId)
        {
            var data = ent.AdminLogins.Where(a => a.UserID == UniqueId).FirstOrDefault();
            if (data != null)
            {
                string mssg = "Welcome to PSWELLNESS. Your Password is:  " + data.Password + ".";
                Message.SendSms(data.PhoneNumber, mssg);
                rm.Message = "SuccessFully Sent";
                rm.Status = 1;
                return Ok(rm);
            }
            else
            {
                rm.Message = "No User Exists of this Id";
                rm.Status = 0;
                return Ok(rm);
            }
        }
    }
}
