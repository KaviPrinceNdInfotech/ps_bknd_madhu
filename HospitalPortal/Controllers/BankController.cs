using AutoMapper;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Repositories;
using HospitalPortal.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    public class BankController : Controller
    {
        DbEntities ent = new DbEntities();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(BankController));
        [HttpGet]
        public ActionResult UpdateBank(int id)
        {
            var model = new UpdateBank();
            var data = ent.BankDetails.Where(a => a.Login_Id == id).ToList();
            if (data.Count() == 0)
            {
                return View(model);
            }
            else
            {
                model.AccountNo = data.FirstOrDefault().AccountNo;
                model.BranchAddress = data.FirstOrDefault().BranchAddress;
                model.BranchName = data.FirstOrDefault().BranchName;
                model.IFSCCode = data.FirstOrDefault().IFSCCode;
                model.Id = data.FirstOrDefault().Id;
                model.Login_Id = data.FirstOrDefault().Login_Id;
                model.HolderName = data.FirstOrDefault().HolderName;
                //model.CancelCheque = data.FirstOrDefault().CancelCheque;
                model.isverfied = data.FirstOrDefault().isverified;
                return View(model);
            }

        }

        [HttpPost]
        public ActionResult UpdateBank(UpdateBank model)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage));
                TempData["msg"] = message;
                return RedirectToAction("UpdateBank", new { id = model.Id });
            }
            //ModelState.Remove("CancelCheque");
            var data = ent.BankDetails.Where(a => a.Login_Id == model.Id).ToList();
            var domain = Mapper.Map<BankDetail>(model);
            //if (model.CancelChequeFile != null)
            //{
            //    var verf = FileOperation.UploadImage(model.CancelChequeFile, "Images");
            //    if (verf == "not allowed")
            //    {
            //        TempData["msg"] = "Only png,jpg,jpeg files are allowed as Cancel Cheque document";
            //        return View(model);
            //    }
            //    model.CancelCheque = verf;
            //}
            if (data.Count() == 0)
            {
                domain.isverified = true;
                //domain.CancelCheque = model.CancelCheque;
                domain.Login_Id = model.Id;
                ent.BankDetails.Add(domain);
            }
            else
            {
                string q = @"update BankDetails set AccountNo='" + model.AccountNo + "',BranchAddress ='" + model.BranchAddress + "',BranchName='" + model.BranchName + "', IFSCCode='" + model.IFSCCode + "',HolderName= '" + model.HolderName + "', isverified=1 where Login_Id=" + model.Id + "";
                ent.Database.ExecuteSqlCommand(q);
            }
            TempData["msg"] = "SuccessFully Updated";
            ent.SaveChanges();
            return RedirectToAction("UpdateBank", new { id = model.Id });
        }


        public ActionResult SendOtp(string MobileNo)
        {
            string msg;
            int Otp = new Random().Next(1000, 9999);
            string m = "Your OTP " + Otp;
            Message.SendSms("mobileNumbe", m);
            HttpCookie OTP = new HttpCookie("OTP");
            OTP.Value = Convert.ToString(Otp);
            //StudentCookies.Expires = DateTime.Now.AddHours(1);
            Response.SetCookie(OTP);
            msg = "ok";
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerifyOtp(string otpNo)
        {

            string msg;
            string otpValue = Request.Cookies["OTP"].Value;
            if (string.IsNullOrEmpty(otpValue))
            {
                msg = "wrong";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }

            if (otpNo == otpValue)
            {
                //delete cookie
                HttpCookie gCookie = new HttpCookie("OTP");
                gCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(gCookie);
                msg = "true";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            else
            {
                msg = "false";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }

        }
    }
}
