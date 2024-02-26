using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HospitalPortal.Controllers
{
    public class NurseAppointmentAPIController : ApiController
    {
        returnMessage rm = new returnMessage();
        DbEntities ent = new DbEntities();
        [HttpGet]// Nurse booking request
        [Route("api/NurseAppointmentAPI/NurseAppointmentList")]
        public IHttpActionResult NurseAppointmentList(int NurseId)
        {
            var model = new NurseAppointmentModel();
            string query = @"select ns.Id,p.PatientName,
                             p.MobileNumber as RegisteredMobileNumber,
                             ns.MobileNUmber as ContactNumber,
                             ns.IsPaid,
case when ns.PaymentDate is null then 'N/A' else Convert(nvarchar(100), ns.PaymentDate, 103) end as PaymentDate,
case when ns.ServiceAcceptanceDate is null then 'N/A' else Convert(nvarchar(100), ns.ServiceAcceptanceDate, 103) end as ServiceAcceptanceDate,
Convert(nvarchar(100), ns.RequestDate, 103) as RequestDate,
ns.RequestDate as rDate,
Convert(nvarchar(100), ns.StartDate, 103) + '-' + Convert(nvarchar(100), ns.EndDate, 103) as ServiceTiming,
Datediff(day,ns.StartDate,ns.EndDate) as TotalDays,
IsNull(ns.TotalFee,0) as Fee,
IsNull(ns.TotalFee,0) as TotalFee,AL.DeviceId
 from NurseService ns
 join Patient p on ns.Patient_Id = p.Id
 join nurse n on ns.Nurse_Id=n.Id
 join AdminLogin AL on AL.Id=p.AdminLogin_Id
where ns.Nurse_Id =" + NurseId + " and ns.ServiceStatus='Approved'  order by ns.Id desc";
            var data = ent.Database.SqlQuery<NurseAppointmentWithUser>(query).ToList();
            
            return Ok(new { result= data } );
        }
    }
}
