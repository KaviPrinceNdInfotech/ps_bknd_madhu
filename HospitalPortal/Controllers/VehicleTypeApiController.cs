using AutoMapper;
using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Repositories;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using static HospitalPortal.Controllers.AmbulancePaymentController;
using static HospitalPortal.Controllers.VehicleTypeApiController;

namespace HospitalPortal.Controllers
{
    public class VehicleTypeApiController : ApiController
    {
        DbEntities ent = new DbEntities();
        ReturnMessage rm = new ReturnMessage();
        CommonRepository repos = new CommonRepository();

        [System.Web.Http.HttpGet]
        public IHttpActionResult VehicleCategory(string Type)
        {
            var model = new VehicelCategory();
            if(Type == "normal")
            {
                string q1 = @"select * from MainCategory where IsDeleted = 0 and [Type] = '" + Type + "' order by CategoryName asc";
                var data = ent.Database.SqlQuery<VehicleList>(q1).ToList();
                if (data.Count() == 0)
                {
                    model.Status = 0;
                    model.Message = "Failue";
                    return Ok(model);
                }
                model.Status = 1;
                model.Message = "Success";
                model.Vehicle = data;
            }
            else if(Type == "funeral")
            {
                string q2 = @"select * from MainCategory where IsDeleted = 0 and [Type] = '"+Type+"'  order by CategoryName asc";
                var data1 = ent.Database.SqlQuery<VehicleList>(q2).ToList();
                if (data1.Count() == 0)
                {
                    model.Status = 0;
                    model.Message = "Failue";
                    return Ok(model);
                }
                model.Status = 1;
                model.Message = "Success";
                model.Vehicle = data1;

            }
            else
            {
                string q2 = @"select * from MainCategory where IsDeleted = 0 and [Type] = '" + Type + "'  order by CategoryName asc";
                var data2 = ent.Database.SqlQuery<VehicleList>(q2).ToList();
                if(data2.Count() == 0)
                {
                    model.Status = 0;
                    model.Message = "Failue";
                    return Ok(model);
                }
                model.Status = 1;
                model.Message = "Success";
                model.Vehicle = data2;
            }
            return Ok(model);
        }

        [System.Web.Http.HttpGet]
        public IHttpActionResult Vehicles(int Id)
        {
            var model = new VehicleRecord();
            var qry = @"exec dbo.spVehicleList @CategoryId ="+Id;
            var data = ent.Database.SqlQuery<VehicleDetails>(qry).ToList();
            model.VehicleDetails = data;
            return Ok(model);
        }

        //[22-04-2023]

        [System.Web.Http.HttpGet]
        public IHttpActionResult AmbulanceTypeList()
        {
            var model = new AmbulanceRecord();
            var qry = @"select * from AmbulanceType";
            var data = ent.Database.SqlQuery<AmbulanceT>(qry).ToList();
            model.AmbulanceT = data;
            return Ok(model);
        }


        [System.Web.Http.HttpGet]
        public IHttpActionResult Vehicle(int id)
        {
            var model = new VehicelCategory();
            var query = @"select Mcy.id, Mcy.CategoryName from   MainCategory as Mcy With(nolock)
Inner join AmbulanceType as amt with(nolock) on Mcy.AmbulanceType_id=amt.id
where Mcy.IsDeleted=0 and Mcy.AmbulanceType_id= " + id;
            var data = ent.Database.SqlQuery<VehicleList>(query).ToList();
            model.Vehicle = data;
            return Ok(model);
        }

        [System.Web.Http.HttpGet]
        public IHttpActionResult VehicleType(int id)
        {
            var model = new VehicleRecords();
            var query = @" select vt.id,vt.VehicleTypeName from  VehicleType as vt with(nolock)
 Inner join MainCategory as mcy with(nolock) on mcy.id=vt.Category_Id
 where vt.IsDeleted=0 and vt.Category_Id= " + id;
            var data = ent.Database.SqlQuery<VehicleDetailes>(query).ToList();
            model.VehicleDetailes = data;
            return Ok(model);
        }



        public class VehicleRecords
        {
            public IEnumerable<VehicleDetailes> VehicleDetailes { get; set; }
        }

        public class VehicleDetailes
        {
            public int Id { get; set; }
            public string VehicleTypeName { get; set; }

        }

        public class AmbulanceRecord
        {
            public IEnumerable<AmbulanceT> AmbulanceT { get; set; }
        }
        public class AmbulanceT
        {
            public int id { get; set; }
            public string AmbulanceType { get; set; }
        }

        public class VehicelCategory
        {
            public string Message { get; set; }
            public int Status { get; set; }

            public IEnumerable<VehicleList> Vehicle { get; set; }
        }

        public class VehicleList
        {
            public int Id { get; set; }

            public string CategoryName { get; set; }
        }

        public class VehicleRecord
        {
            public IEnumerable<VehicleDetails> VehicleDetails { get; set; }
        }

        public class VehicleDetails
        {
            public int Category_Id { get; set; }
            public int Id { get; set; }
            public string VehicleTypeName { get; set; }
            public bool IsDeleted { get; set; }
            public Nullable<double> _0_5 { get; set; }
            public Nullable<double> _6_10 { get; set; }
            public Nullable<double> _11_20 { get; set; }
            public Nullable<double> _21_40 { get; set; }
            public Nullable<double> _41_60 { get; set; }
            public Nullable<double> _61_80 { get; set; }
            public Nullable<double> _81_100 { get; set; }
            public Nullable<double> _100_150 { get; set; }
            public Nullable<double> _151_200 { get; set; }
            public Nullable<double> _201_250 { get; set; }
            public Nullable<double> _251_300 { get; set; }
            public Nullable<double> _301_350 { get; set; }
            public Nullable<double> _351_400 { get; set; }
            public Nullable<double> _401_450 { get; set; }
            public Nullable<double> _451_500 { get; set; }
            public Nullable<double> _500 { get; set; }
        }
             
    }
}
