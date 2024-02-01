using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using HospitalPortal.Utilities;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.BL
{
    public class Job : IJob
    {
        DbEntities ent = new DbEntities();
        public void Execute(IJobExecutionContext contect)
        {
            var Insauracnce = @"select * from Vehicle where DATEDIFF(DAY, GETDATE(), InsurranceDate) = 7 and Vendor_Id Is Not null";
            var Validity = @"select * from Vehicle where DATEDIFF(DAY, GETDATE(), Validity) = 7 and Vendor_Id Is Not null";
            var PollutionDate = @"select * from Vehicle where DATEDIFF(DAY, GETDATE(), PollutionDate) = 7) = 7 and Vendor_Id Is Not null";
            var FitnessCertificateValidity = @"select * from Vehicle where DATEDIFF(DAY, GETDATE(), FitnessCertificateValidity) = 7) = 7 and Vendor_Id Is Not null";
            if (Insauracnce != null)
            {
                //SendMsgInsauracnce();
            }
            if (Validity != null)
            {
                //SendMsgValidity();
            }
            if (PollutionDate != null)
            {
                //SendMsgPollutionDate();
            }
            if (FitnessCertificateValidity != null)
            {
                //SendMsgFitnessCertificateValidity();
            }

        }

        private void SendMsgInsauracnce()
        {
            var model = new VendorData();
            var query = @"select Vendor.MobileNumber, Vehicle.VehicleNumber, Vehicle.InsurranceDate from Vehicle join Vendor on Vehicle.Vendor_Id = Vendor.Id  where DATEDIFF(DAY, GETDATE(), Validity) = 7 and Vendor_Id Is Not null";
            var data = ent.Database.SqlQuery<VendorMsgList>(query).ToList();
            model.VendorMsgList = data;
            foreach (var item in data)
            {
                DateTime? dt = data.FirstOrDefault().InsurranceDate;
                var date = dt.ToString();
                string msg = "The Insurance is Going to Expire of Vehicle Number " + data.FirstOrDefault().VehicleNumber+" on "+data.FirstOrDefault().InsurranceDate.Value.Date+"";
                Message.SendSms(item.MobileNumber, msg);
                Message.SendSms("7838521030", msg);
            }
        }

        private void SendMsgValidity()
        {
            var model = new VendorData();
            var query = @"select Vendor.MobileNumber, Vehicle.VehicleNumber, Vehicle.Validity from Vehicle join Vendor on Vehicle.Vendor_Id = Vendor.Id  where DATEDIFF(DAY, GETDATE(), Validity) = 7 and Vendor_Id Is Not null";
            var data = ent.Database.SqlQuery<VendorMsgList>(query).ToList();
            model.VendorMsgList = data;
            foreach (var item in data)
            {
                string msg = "The Validity is Going to Expire of Vehicle Number " + data.FirstOrDefault().VehicleNumber + " on " + data.FirstOrDefault().Validity.Value.Date + "";
                Message.SendSms(item.MobileNumber, msg);
                Message.SendSms("7838521030", msg);
            }
        }

        private void SendMsgPollutionDate()
        {
            var model = new VendorData();
            var query = @"select Vendor.MobileNumber, Vehicle.VehicleNumber, Vehicle.PollutionDate from Vehicle join Vendor on Vehicle.Vendor_Id = Vendor.Id  where DATEDIFF(DAY, GETDATE(), Validity) = 7 and Vendor_Id Is Not null";
            var data = ent.Database.SqlQuery<VendorMsgList>(query).ToList();
            model.VendorMsgList = data;
            foreach (var item in data)
            {
                string msg = "The Pollution Date is Going to Expire of Vehicle Number " + data.FirstOrDefault().VehicleNumber + " on " + data.FirstOrDefault().Validity.Value.Date + "";
                Message.SendSms(item.MobileNumber, msg);
                Message.SendSms("7838521030", msg);
            }
        }

        private void SendMsgFitnessCertificateValidity()
        {
            var model = new VendorData();
            var query = @"select Vendor.MobileNumber, Vehicle.VehicleNumber, Vehicle.FitnessCertificateValidity from Vehicle join Vendor on Vehicle.Vendor_Id = Vendor.Id  where DATEDIFF(DAY, GETDATE(), Validity) = 7 and Vendor_Id Is Not null";
            var data = ent.Database.SqlQuery<VendorMsgList>(query).ToList();
            model.VendorMsgList = data;
            foreach (var item in data)
            {
                string msg = "The Fitness Certificate is Going to Expire of Vehicle Number " + data.FirstOrDefault().VehicleNumber + " on " + data.FirstOrDefault().Validity.Value.Date + "";
                Message.SendSms(item.MobileNumber, msg);
                Message.SendSms("7838521030", msg);
            }
        }

    }
}