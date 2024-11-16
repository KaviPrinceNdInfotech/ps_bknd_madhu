using DocumentFormat.OpenXml.Wordprocessing;
using HospitalPortal.Models.DomainModels;
using HospitalPortal.Models.ViewModels;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    public class AmbulancePaymentController : Controller
    {
        DbEntities ent = new DbEntities();

        public ActionResult Show(int? Id, DateTime? sdate, DateTime? edate)
        {
            var model = new AmbulanceList();
            if(sdate!=null && edate!=null)
            {
                var doctor1 = @"SELECT distinct v.Id AS VehicleId,v.VehicleNumber,d.DriverId,d.DriverName,ISNULL(v.VehicleName, 'NA') AS VehicleName
    FROM DriverLocation trm
    JOIN Driver d ON d.Id = trm.Driver_Id
    JOIN Vehicle v ON v.Id = d.Vehicle_Id   
	JOIN Patient p on p.Id=trm.PatientId     
    WHERE trm.IsPay = 'Y' AND trm.EntryDate BETWEEN @sdate AND @edate and trm.RideComplete=1 group by v.Id,v.VehicleNumber, d.DriverName,v.VehicleName,d.DriverId";
                var data1 = ent.Database.SqlQuery<AmbulanceReport>(doctor1, new SqlParameter("sdate", sdate),
             new SqlParameter("edate", edate)).ToList();
                model.Ambulance = data1;
                return View(model);
            }
            else
            {
                var doctor = @"SELECT distinct v.Id AS VehicleId,v.VehicleNumber,d.DriverId,d.DriverName,ISNULL(v.VehicleName, 'NA') AS VehicleName
    FROM DriverLocation trm
    JOIN Driver d ON d.Id = trm.Driver_Id
    JOIN Vehicle v ON v.Id = d.Vehicle_Id   
	JOIN Patient p on p.Id=trm.PatientId     
    WHERE trm.IsPay = 'Y' AND trm.EntryDate BETWEEN DATEADD(DD, -7, GETDATE()) AND GETDATE() and trm.RideComplete=1 group by v.Id,v.VehicleNumber, d.DriverName,v.VehicleName,d.DriverId";
                var data = ent.Database.SqlQuery<AmbulanceReport>(doctor).ToList();
                model.Ambulance = data;
               
            }
            return View(model);

        }

        public ActionResult ViewDetails(int id, DateTime? sdate, DateTime? edate)
        {
             var model = new AmbulanceList();
            var vehicle = @"select * from Vehicle join Driver on Driver.Id = Vehicle.Driver_Id where Vehicle.Id=" + id;
            var mek = ent.Database.SqlQuery<AmbulanceList>(vehicle).ToList();
            model.VehicleName = mek.FirstOrDefault().VehicleName;
            model.VehicleNumber = mek.FirstOrDefault().VehicleNumber;
            model.DriverName = mek.FirstOrDefault().DriverName;
            if (sdate != null && edate != null)
            {
                var doct = @"SELECT trm.id,p.PatientName,p.PatientRegNo,CONCAT(DAY(dp.PaymentDate), ' ',UPPER(FORMAT(trm.PaymentDate, 'MMM')), ' ', YEAR(trm.PaymentDate),' ',FORMAT(trm.PaymentDate, 'hh:mm tt')) AS PaymentDate,v.VehicleNumber,ISNULL(v.VehicleName, 'NA') AS VehicleName,trm.TotalPrice as TotalPrice,
trm.ToatlDistance AS Distance,d.DriverName,v.Id AS VehicleId,trm.start_Lat,trm.start_Long,trm.end_Lat,trm.end_Long FROM DriverLocation trm
JOIN Driver d ON d.Id = trm.Driver_Id
JOIN Vehicle v ON v.Id = d.Vehicle_Id
JOIN Patient p ON p.Id = trm.PatientId
WHERE v.Id=" + id+" and trm.IsPay = 'Y' AND CONVERT(DATETIME, trm.PaymentDate, 103) BETWEEN @sdate AND @edate) SELECT id, PatientName, PaymentDate, PatientRegNo, VehicleNumber, VehicleName, Amount, Distance, DriverName, VehicleId, start_Lat, start_Long, end_Lat, end_Long FROM RankedResults WHERE RowNum = 1;";
               // var doctor1 = ent.Database.SqlQuery<AmbulanceReport>(doct).ToList();
				var doctor1 = ent.Database.SqlQuery<AmbulanceReport>(doct,
			new SqlParameter("sdate", sdate),
			new SqlParameter("edate", edate)).ToList();
                TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                model.Ambulance = doctor1;
            }
            else
            { 
                var doctor1 = @"SELECT trm.id,p.PatientName,p.PatientRegNo,CONCAT(DAY(trm.PaymentDate), ' ',UPPER(FORMAT(trm.PaymentDate, 'MMM')), ' ', YEAR(trm.PaymentDate),' ',FORMAT(trm.PaymentDate, 'hh:mm tt')) AS PaymentDate,v.VehicleNumber,ISNULL(v.VehicleName, 'NA') AS VehicleName,trm.TotalPrice as TotalPrice,
trm.ToatlDistance AS Distance,d.DriverName,v.Id AS VehicleId,trm.start_Lat,trm.start_Long,trm.end_Lat,trm.end_Long FROM DriverLocation trm
JOIN Driver d ON d.Id = trm.Driver_Id
JOIN Vehicle v ON v.Id = d.Vehicle_Id
JOIN Patient p ON p.Id = trm.PatientId
WHERE v.Id=" + id+" and trm.IsPay = 'Y'";
                var doctorList = ent.Database.SqlQuery<AmbulanceReport>(doctor1).ToList();
                model.Ambulance = doctorList;
                TempData["msg"] = "No data available.";
            } 
            return View(model);
        }

        public ActionResult Driver( int? id, int? pageNumber, DateTime? RequestDate, string name = null)
        {
            double commision = ent.Database.SqlQuery<double>(@"select Commission from CommissionMaster where IsDeleted=0 and Name='Ambulance'").FirstOrDefault();
            var model = new AmbulanceList();
            if (RequestDate != null)
            {
                DateTime dateCriteria = RequestDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
trm.Amount, trm.Distance, d.DriverName,
trm.PickUp_Place, trm.Drop_Place,
v.Id as VehicleId
from TravelRecordMaster trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1 and d.Id = "+id+" and trm.RequestDate between '" + RequestDate + "' and '" + date + "'";
                var data1 = ent.Database.SqlQuery<AmbulanceReport>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Commission = commision;
                    int total = data1.Count;
                    pageNumber = (int?)pageNumber ?? 1;
                    int pageSize = 10;
                    decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                    model.TotalPages = (int)noOfPages;
                    model.PageNumber = (int)pageNumber;
                    data1 = data1.OrderBy(a => a.VehicleId).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                    if (name != null)
                    {
                        data1 = data1.Where(a => a.VehicleNumber.ToLower().Contains(name)).ToList();
                    }
                    model.Ambulance = data1;
                    return View(model);
                }
            }
            var doctor = @"select p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
trm.Amount, trm.Distance, d.DriverName,
trm.PickUp_Place, trm.Drop_Place,
v.Id as VehicleId
from TravelRecordMaster trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1 and d.Id = " + id + " and trm.RequestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE()";
            var data = ent.Database.SqlQuery<AmbulanceReport>(doctor).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record Of Current Week";
                return View(model);
            }
            else
            {
                ViewBag.Commission = commision;
                int total = data.Count;
                pageNumber = (int?)pageNumber ?? 1;
                int pageSize = 10;
                decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                model.TotalPages = (int)noOfPages;
                model.PageNumber = (int)pageNumber;
                data = data.OrderBy(a => a.VehicleId).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                model.Ambulance = data;
                return View(model);
            }
        }

        public ActionResult DriverTDS(int? id, int? pageNumber, DateTime? RequestDate, string name = null)
        {
            double commision = ent.Database.SqlQuery<double>(@"select Amount from TDSMaster where IsDeleted=0 and Name='Vehicle'").FirstOrDefault();
            var model = new AmbulanceList();
            if (RequestDate != null)
            {
                DateTime dateCriteria = RequestDate.Value.AddDays(-7);
                string date = dateCriteria.ToString("dd/MM/yyyy");
                var qry1 = @"select p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
trm.Amount, trm.Distance, d.DriverName,
trm.PickUp_Place, trm.Drop_Place,
v.Id as VehicleId
from TravelRecordMaster trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1 and d.Id = " + id + " and trm.RequestDate between '" + RequestDate + "' and '" + date + "'";
                var data1 = ent.Database.SqlQuery<AmbulanceReport>(qry1).ToList();
                if (data1.Count() == 0)
                {
                    TempData["msg"] = "Your Selected Date Doesn't Contain any Information.";
                }
                else
                {
                    ViewBag.Commission = commision;
                    int total = data1.Count;
                    pageNumber = (int?)pageNumber ?? 1;
                    int pageSize = 10;
                    decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                    model.TotalPages = (int)noOfPages;
                    model.PageNumber = (int)pageNumber;
                    data1 = data1.OrderBy(a => a.VehicleId).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                    if (name != null)
                    {
                        data1 = data1.Where(a => a.VehicleNumber.ToLower().Contains(name)).ToList();
                    }
                    model.Ambulance = data1;
                    return View(model);
                }
            }
            var doctor = @"select p.PatientName, v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
trm.Amount, trm.Distance, d.DriverName,
trm.PickUp_Place, trm.Drop_Place,
v.Id as VehicleId
from TravelRecordMaster trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1 and d.Id = " + id + " and trm.RequestDate between DateAdd(DD,-7,GETDATE() ) and GETDATE()";
            var data = ent.Database.SqlQuery<AmbulanceReport>(doctor).ToList();
            if (data.Count() == 0)
            {
                TempData["msg"] = "No Record Of Current Week";
                return View(model);
            }
            else
            {
                ViewBag.Commission = commision;
                int total = data.Count;
                pageNumber = (int?)pageNumber ?? 1;
                int pageSize = 10;
                decimal noOfPages = Math.Ceiling((decimal)total / pageSize);
                model.TotalPages = (int)noOfPages;
                model.PageNumber = (int)pageNumber;
                data = data.OrderBy(a => a.VehicleId).Skip(pageSize * ((int)pageNumber - 1)).Take(pageSize).ToList();
                model.Ambulance = data;
                return View(model);
            }
        }

        public ActionResult DriverReport()
        {
            return View();
        }

        private int GetDriverId()
        {
            int loginId = Convert.ToInt32(User.Identity.Name);
            int DriverId = ent.Database.SqlQuery<int>("select Id from Driver where AdminLogin_Id=" + loginId).FirstOrDefault();
            return DriverId;
        }

        public ActionResult Daily(string term)
        {
            int id = GetDriverId();
            var model = new AmbulanceList();
            string q = @"selectselect v.VehicleNumber, IsNull(v.VehicleName,'NA') as VehicleName, 
v.Id as VehicleId
from TravelRecordMaster trm 
join Driver d on d.Id = trm.Driver_Id
join Vehicle v on v.Id = trm.Vehicle_Id
join Patient p on p.Id = trm.Patient_Id
where trm.IsDriveCompleted = 1 and v.Id = "+id+ " and Convert(Date,trm.RequestDate) = (GetDate()) group by v.VehicleNumber, v.VehicleName, v.Id";
            var data = ent.Database.SqlQuery<AmbulanceReport>(q).ToList();
            if(data.Count()  == 0)
            {
                TempData["msg"] = "No Current Record Present";
                return View(model);
            }
            model.Ambulance = data;
            return View(model);
        }

        //public void DownloadAmbulanceExcelForBank()
        //{
        //    string query = "SELECT distinct v.Id AS VehicleId, d.DriverId as UniqueId, d.DriverName as Name, d.EmailId, sum(dl.Amount) as Amount, bd.AccountNo, bd.IFSCCode FROM DriverLocation dl JOIN Driver d ON d.Id = dl.Driver_Id JOIN Vehicle v ON v.Id = d.Vehicle_Id LEFT JOIN BankDetails as bd ON bd.Login_Id=d.AdminLogin_Id WHERE dl.IsPay = 'Y' AND dl.EntryDate BETWEEN DATEADD(DD, -7, GETDATE()) AND GETDATE() and dl.IsPayoutPaid=1 GROUP BY v.Id, d.DriverName, d.DriverId, d.EmailId, bd.AccountNo, bd.IFSCCode";
        //    List<DetailsForBank> employeeDetails = ent.Database.SqlQuery<DetailsForBank>(query, Array.Empty<object>()).ToList();
        //    ExcelPackage Ep = new ExcelPackage();
        //    ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
        //    Sheet.Cells["A1"].Value = "Payment Method";
        //    Sheet.Cells["B1"].Value = "Payment Amount";
        //    Sheet.Cells["C1"].Value = "Activation Date";
        //    Sheet.Cells["D1"].Value = "Beneficiary Name";
        //    Sheet.Cells["E1"].Value = "Account No in text";
        //    Sheet.Cells["F1"].Value = "Email";
        //    Sheet.Cells["G1"].Value = "Email Body";
        //    Sheet.Cells["H1"].Value = "Debit Account No";
        //    Sheet.Cells["I1"].Value = "CRN No";
        //    Sheet.Cells["J1"].Value = "Receiver IFSC Code";
        //    Sheet.Cells["K1"].Value = "Receiver Account";
        //    Sheet.Cells["L1"].Value = "Remarks";
        //    Sheet.Cells["M1"].Value = "Phone No";
        //    int row = 2;
        //    CRNGenerator crnGenerator = new CRNGenerator();
        //    foreach (DetailsForBank item in employeeDetails)
        //    {
        //        string dvrId = item.UniqueId;
        //        long sdds = Convert.ToInt64(item.AccountNo);
        //        string crn = crnGenerator.GenerateCRN(dvrId);
        //        Sheet.Cells[$"A{row}"].Value = "N";
        //        Sheet.Cells[$"B{row}"].Value = item.Amount;
        //        Sheet.Cells[$"C{row}"].Value = DateTime.Now;
        //        Sheet.Cells[$"C{row}"].Style.Numberformat.Format = "yyyy-MM-dd";
        //        Sheet.Cells[$"D{row}"].Value = item.Name;
        //        Sheet.Cells[$"E{row}"].Value = NumberToWords(sdds);
        //        Sheet.Cells[$"F{row}"].Value = item.EmailId;
        //        Sheet.Cells[$"G{row}"].Value = "This is your payment report";
        //        Sheet.Cells[$"H{row}"].Value = "55443333322222(fix)";
        //        Sheet.Cells[$"I{row}"].Value = crn;
        //        Sheet.Cells[$"J{row}"].Value = item.IFSCCode;
        //        Sheet.Cells[$"K{row}"].Value = item.AccountNo;
        //        Sheet.Cells[$"L{row}"].Value = "Enter your remark";
        //        Sheet.Cells[$"M{row}"].Value = "9090907867(admin)";
        //        row++;
        //    }
        //    Sheet.Cells["A:AZ"].AutoFitColumns();
        //    base.Response.Clear();
        //    base.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    base.Response.AddHeader("content-disposition", "attachment; filename=Report.xlsx");
        //    base.Response.BinaryWrite(Ep.GetAsByteArray());
        //    base.Response.End();
        //} 
        static string NumberToWords(long number)
        {
            if (number == 0)
                return "zero";

            int val;
            long next, num_digits;
            long[] a = new long[19]; // Maximum number of digits in a long is 19

            // words for every digits from 0 to 9
            string[] digits_words = {
        "zero",
        "one",
        "two",
        "three",
        "four",
        "five",
        "six",
        "seven",
        "eight",
        "nine"
    };

            string words = "";

            val = 0;
            next = 0;
            num_digits = 0;

            while (number > 0)
            {
                next = number % 10;
                a[val] = next;
                val++;
                num_digits++;
                number = number / 10;
            }

            for (val = (int)(num_digits - 1); val >= 0; val--)
            {
                words += digits_words[a[val]] + " ";
            }

            return words.Trim(); // Trim any trailing whitespace
        } 
        public class CRNGenerator
        {
            private int sequenceNumber = 1;

            public string GenerateCRN(string dvrId)
            {
                string month = DateTime.Now.ToString("MMM");
                string year = DateTime.Now.ToString("yyyy");
                string formattedSequence = sequenceNumber.ToString().PadLeft(4, '0');
                sequenceNumber++;
                return $"{dvrId}/{month}/{year}/{formattedSequence}";
            }
        }

        public string NumberToText(int number)
        {
            Dictionary<int, string> numberTextMap = new Dictionary<int, string>
            {
              {0, "zero"},
              {1, "one"},
              {2, "two"},
              {3, "three"},
              {4, "four"},
              {5, "five"},
              {6, "six"},
              {7, "seven"},
              {8, "eight"},
              {9, "nine"},
            };

            if (numberTextMap.ContainsKey(number))
            {
                return numberTextMap[number];
            }
            else
            {
                return number.ToString(); // Return original number if not found in the map
            }
        }
        public class AmbulanceList
        {
            public DateTime? RequestDate { get; set; }
			public DateTime startdate { get; set; }
			public DateTime enddate { get; set; }
			public int TotalPages { get; set; }
            public int VehicleId { get; set; }
            public int PageNumber { get; set; }
            public string VehicleNumber { get; set; }
            public string VehicleName { get; set; }
            public string DriverName { get; set; }

            public IEnumerable<AmbulanceReport> Ambulance { get; set; }
        }

        public class AmbulanceReport
        {
            public int Id { get; set; }
            public int VehicleId { get; set; }
            public int TotalPrice { get; set; }

            public string DriverId { get; set; }
            public string VehicleNumber { get; set; }
            public string VehicleName { get; set; }
            public decimal Amount { get; set; }
			public double Amountwithrazorpaycomm { get; set; }
			public int Distance { get; set; }
            public string PatientName { get; set; }
            public string PatientRegNo { get; set; }
            public string PaymentDate { get; set; }
            public DateTime EntryDate { get; set; }
            public string DriverName { get; set; }  
            public double end_Lat { get; set; }
            public double end_Long { get; set; }
            public double start_Lat { get; set; }
            public double start_Long { get; set; }
            //CODE FOR LAT LONG TO LOCATION 
            public string PickUp_Place
            {
                get { return getlocation(start_Lat.ToString(), start_Long.ToString()); }
            }
            public string Drop_Place
            {
                get { return getlocation(end_Lat.ToString(), end_Long.ToString()); }
            }
             
            private string getlocation(string latitude, string longitude)
            {

                string url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + latitude + "," + longitude + "&key=AIzaSyBrbWFXlOYpaq51wteSyFS2UjdMPOWBlQw";

                // Make the HTTP request.
                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 10000;

                // Get the response.
                WebResponse response = request.GetResponse();
                string responseText = new StreamReader(response.GetResponseStream()).ReadToEnd();

                // Parse the response JSON.
                var json = JsonConvert.DeserializeObject<dynamic>(responseText);

                // Get the location from the JSON.
                var location = json.results[0].formatted_address;
                return location;
            }

            //END CODE FOR LAT LONG TO LOCATION 
        }

        public class DetailsForBank
        {
            public int Id { get; set; }
            public string BeneficiaryName { get; set; }
            public string UniqueId { get; set; }
            public string EmailId { get; set; }
            public string AccountNo { get; set; }
            public string MobileNumber { get; set; }
            public string IFSCCode { get; set; }
            public string CRN { get; set; }
            public double Amount { get; set; }
            public double TotalFee { get; set; }
        }

    }
}