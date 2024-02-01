using AutoMapper;
using HospitalPortal.BL;
using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.RequestModel;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Repositories;
using HospitalPortal.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using static HospitalPortal.Models.ViewModels.ChemistDTO;

namespace HospitalPortal.Controllers
{
    public class ChemistApiController : ApiController
    {
        DbEntities ent = new DbEntities();
        returnMessage rm = new returnMessage();
        GenerateBookingId bk = new GenerateBookingId();
        CommonRepository repos = new CommonRepository();
        ILog log = LogManager.GetLogger(typeof(PatientApiController));

        //============chemisthistory 04/05/23===========


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/ChemistApi/chemisthistory")]
        public IHttpActionResult chemisthistory(int Id)
        {
            var data = new Chemist();
            string qry = @"select cp.Id,ch.ChemistName,ch.MobileNumber,cm.cityname,sm.StateName,ch.location,cp.Amount from ChemistPayOut as cp with(nolock) left join Chemist as ch with(nolock) on ch.Id = cp.Chemist_Id left join citymaster as cm with(nolock) on cm.id=ch.CityMaster_Id left join statemaster as sm with(nolock) on sm.id=ch.stateMaster_Id where cp.Chemist_Id="+ Id + "";
            var chmi1 = ent.Database.SqlQuery<chemistdetail>(qry).ToList();
            return Ok(new { chmi1 });
        }


        //===========paymenthistory 05/05/23=================

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/ChemistApi/paymenthistory")]

        public IHttpActionResult paymenthistory(int Id)
        {
            var data = new Chemist();
            string qry = @"select cp.Id, ch.ChemistName,Bd.BranchName,cp.Amount,cp.id as PaymentId,cp.PaymentDate from ChemistPayOut as cp left join Chemist as ch on ch.id=cp.Chemist_Id left join BankDetails as Bd on bd.id=cp.BankdetailsId where cp.Chemist_Id="+ Id + " order by cp.Id desc";
            var pay = ent.Database.SqlQuery<paymentdetail>(qry).ToList();
            return Ok(new { pay });

        }

        //===========chemistprofiledetail 05/05/23=================

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/ChemistApi/chemistprofiledetail")]
        public IHttpActionResult chemistprofiledetail(int id)
        {
            var data = new Chemist();
            string qry = @"select ch.ChemistName,ch.EmailId,ch.MobileNumber,ch.location,sm.StateName,cm.cityname,ch.PinCode from chemist as ch with(nolock)
left join citymaster as cm with(nolock) on cm.id=ch.CityMaster_Id
left join statemaster as sm with(nolock) on sm.id=ch.stateMaster_Id where ch.id=" + id + "";
            var data1 = ent.Database.SqlQuery<chemistpro_detail>(qry).FirstOrDefault();
            return Ok(data1);
        }

        //===========payouthistory 05/05/23=================//

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/ChemistApi/payouthistory")]

        public IHttpActionResult payouthistory(int Id)
        {

            var data = new Chemist();
            string qry = @"select cp.id, ch.ChemistName,cp.Amount,cp.PaymentDate from ChemistPayOut as cp left join Chemist as ch on ch.id=cp.Chemist_Id where cp.Chemist_Id="+ Id + " order by cp.Id desc";
            var payout = ent.Database.SqlQuery<payoutdetail>(qry).ToList();
            return Ok(new { payout });

        }

        //===========ChemistUpdateProfile 06/05/23=================

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/ChemistApi/ManageProfile")]
        public IHttpActionResult ManageProfile(ChemistUpdateProfile model)
        {
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
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
                if (model.ChemistName == "")
                {
                    data.ChemistName = data.ChemistName;
                }
                else
                {
                    data.ChemistName = model.ChemistName;
                }

                //if (model.LicenceImage == null)
                //{
                //    rm.Message = "Licence Image File Picture can not be null";

                //    return Ok(rm);
                //}
                if (model.LicenceImageBase64 != null)
                {
                    var img = FileOperation.UploadFileWithBase64("Images", model.LicenceImage, model.LicenceImageBase64, allowedExtensions);
                    if (img == "not allowed")
                    {
                        rm.Message = "Only png,jpg,jpeg,pdf files are allowed.";

                        return Ok(rm);
                    }
                    model.LicenceImage = img;
                    data.ChemistName = model.ChemistName;
                    data.ShopName = model.ShopName;
                    data.StateMaster_Id = model.StateMaster_Id;
                    data.CityMaster_Id = model.CityMaster_Id;
                    data.Location = model.Location;
                    data.GSTNumber = model.GSTNumber;
                    data.LicenceNumber = model.LicenceNumber;
                    data.LicenceImage = model.LicenceImage;
                    ent.SaveChanges();
                    rm.Status = 1;
                    rm.Message = " Chemist profile updated Successfully .";
                }
                else
                {


                    data.ChemistName = model.ChemistName;
                    data.ShopName = model.ShopName;
                    data.StateMaster_Id = model.StateMaster_Id;
                    data.CityMaster_Id = model.CityMaster_Id;
                    data.Location = model.Location;
                    data.GSTNumber = model.GSTNumber;
                    data.LicenceNumber = model.LicenceNumber;
                    data.LicenceImage = model.LicenceImage;
                    ent.SaveChanges();
                    rm.Status = 1;
                    rm.Message = " Chemist profile updated Successfully .";
                }

            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);
        }
        ///////////////////////////UpdateProfilebyChemist////////////////////////////////

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/ChemistApi/UpdateProfilebyChemist")]
        public IHttpActionResult UpdateProfilebyChemist(ChemistPro_bnkUpdate model)
        {

            try
            {
                var data = ent.Chemists.Where(a => a.Id == model.Id).FirstOrDefault();
                data.ShopName = model.ShopName;
                data.MobileNumber = model.MobileNumber;
                data.StateMaster_Id = model.StateMaster_Id;
                data.CityMaster_Id = model.CityMaster_Id;
                data.Location = model.Location;
                data.PinCode = model.PinCode;
                
                ent.SaveChanges();
                rm.Status = 1;
                rm.Message = "Profile has updated.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                rm.Status = 0;
                rm.Message = "Internal server error";
            }
            return Ok(rm);

        }

        //=====================CHEMIST ABOUT==================//

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/ChemistApi/ChemistAbout")]

        public IHttpActionResult ChemistAbout(int Id)
        {
            string qry = @"select Id,About from Chemist where IsDeleted=0 and Id=" + Id + "";
            var ChemistAbout = ent.Database.SqlQuery<Ch_About>(qry).FirstOrDefault();
            return Ok( ChemistAbout );
        }
    }

}
