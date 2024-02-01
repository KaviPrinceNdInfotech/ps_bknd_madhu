using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace HospitalPortal.BL
{
    public class NearestMedicine
    {

        DbEntities ent = new DbEntities();

        public NearestChemist GetNearsestMedicalStore(double lat, double lng)
        {
            string query = @"execute sp_GetNearestMedicalShop @userLat,@userLng";
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@userLat", lat);
            prms[1] = new SqlParameter("@userLng", lng);
            var data = ent.Database.SqlQuery<NearestChemist>(query, prms).FirstOrDefault();
            return data;
        }
    }
}