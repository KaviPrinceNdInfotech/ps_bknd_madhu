using AutoMapper;
using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HospitalPortal.Controllers
{
    public class WalletController : ApiController
    {
        ReturnMessage rm = new ReturnMessage();
        DbEntities ent = new DbEntities();

        [HttpPost]
        public IHttpActionResult AddValue(WalletDTO model )
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
            var domain = Mapper.Map<UserWallet>(model);
            domain.TransactionType = "cr";
            ent.UserWallets.Add(domain);
            ent.SaveChanges();
            rm.Status = 1;
            rm.Message = "Success";
            return Ok(rm);
        }

        [HttpGet]
        public IHttpActionResult GetValue(int UserId)
        {
            if (UserId != 0)
            {
                var model = new GetWalletDTO();
                double? DebitAmt = ent.Database.SqlQuery<double>(@"select IsNull(Sum(Amount),0) as DebitAmt from UserWallet where TransactionType='dr' and UserId=" + UserId).FirstOrDefault();
                double? CreditAmount = ent.Database.SqlQuery<double>(@"select IsNull(Sum(Amount),0) as CreditAmount from UserWallet where TransactionType='cr' and UserId=" + UserId).FirstOrDefault();
                double? totalAmt = CreditAmount - DebitAmt;
                model.Amount = totalAmt;
                return Ok(model);
            }
            else
            {
                rm.Message = "Invalid Attempt";
                rm.Status = 0;
                return Ok(rm);
            }
        }

        [HttpPost]
        public IHttpActionResult DeductValue(WalletDTO model)
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
            var domain = Mapper.Map<UserWallet>(model);
            domain.TransactionType = "dr";
            ent.UserWallets.Add(domain);
            ent.SaveChanges();
            rm.Status = 1;
            rm.Message = "Success";
            return Ok(rm);
        }


        [HttpPost, Route("api/Wallet/AddWalletMoney")]
        public IHttpActionResult AddWalletMoney(UserWallet model)
        {           
            try
            {
                if (model.UserId != null)
                {
                    var emp = ent.Patients.FirstOrDefault(a => a.Id == model.UserId);
                    if(emp.walletAmount==null)
                    {
                        emp.walletAmount = 0;
                    }
                    emp.walletAmount = model.walletAmount + emp.walletAmount;
                    ent.SaveChanges();
                    return Ok("Add Money SuccessFully");
                }
                else
                {
                    return Ok("Please enter the amount");
                }
            }
            catch
            {
                return BadRequest("Server Error");
            }
        }

        [HttpPost, Route("api/Wallet/UpdateWalletMoney")]
        public IHttpActionResult UpdateWalletMoney(UserWallet model)
        {
            try
            {
                if (model.walletAmount != null)
                {
                    var emp = ent.Patients.FirstOrDefault(a => a.Id == model.UserId);
                    if (emp.walletAmount >= model.walletAmount)
                    {
                        emp.walletAmount = emp.walletAmount - model.walletAmount;
                        ent.SaveChanges();
                        return Ok("Wallet Amount Update SuccessFully");
                    }
                    else
                    {
                        return BadRequest("Please check wallet amount");
                    }
                }
                else
                {
                    return BadRequest("Please enter the amount");
                }
            }
            catch
            {
                return BadRequest("Server Error");
            }
        }

        [HttpGet, Route("api/Wallet/ListWalletMoney/{UserId}")]
        public IHttpActionResult ListWalletMoney(int UserId)
        {
            try
            {
                var result = ent.Patients.Where(x => x.Id == UserId).Select(x => new { x.Id, x.walletAmount }).ToList();
                if (result != null)
                {
                    return Ok(new { result, status = 200, message = "Customer Wallet Balance" });
                }
                else
                {
                    return NotFound();
                }
            }
            catch
            {
                return BadRequest("Server Error");
            }
        }
    }
}
