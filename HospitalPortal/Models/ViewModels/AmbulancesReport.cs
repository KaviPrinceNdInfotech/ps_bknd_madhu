using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace HospitalPortal.Models.ViewModels
{
    public class AmbulancesReport
    {
        public DateTime? sdate { get; set; }
        public DateTime? edate { get; set; }
        public double? PayAmt { get; set;}
        public IEnumerable<Ambulance> Ambulance { get; set; }
    }


    public class Ambulance
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int Driver_Id { get; set; }
        public string VehicleNumber { get; set; }
        public string VehicleName { get; set; }
        public decimal? Amount { get; set; }
        public double? Amountwithrazorpaycomm { get; set; }
        public int? Distance { get; set; }
        public string PatientName { get; set; }
        public string DriverId { get; set; }
        public string DriverName { get; set; }
        public string PickUp_Place { get; set; }
        public string Drop_Place { get; set; }
        public double end_Lat { get; set; }
        public double end_Long { get; set; }
        public double start_Lat { get; set; }
        public double start_Long { get; set; }

        //CODE FOR LAT LONG TO LOCATION 
        public string ReverseStartLatLong_To_Location
        {
            get { return getlocation(start_Lat.ToString(), start_Long.ToString()); }
        }
        public string ReverseEndLatLong_To_Location
        {
            get { return getlocation(end_Lat.ToString(), end_Long.ToString()); }
        }
        public string Duration
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

}