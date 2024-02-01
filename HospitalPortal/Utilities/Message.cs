using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace HospitalPortal.Utilities
{
    public class Message
    {

        public static void SendSms(string mobileNumber,string m)
        {
            //Your authentication key
            string authKey = "153995AnO2vKHdOPk59294dd7";
            //Multiple mobiles numbers separated by comma
            //Sender ID,While using route4 sender id should be 6 characters long.
            string senderId = "PSWELL";
            //Your message to send, Add URL encoding here.
            string message = HttpUtility.UrlEncode(m);
            //Prepare you post parameters
            StringBuilder sbPostData = new StringBuilder();
            sbPostData.AppendFormat("authkey={0}", authKey);
            sbPostData.AppendFormat("&mobiles={0}", mobileNumber);
            sbPostData.AppendFormat("&message={0}", message);
            sbPostData.AppendFormat("&sender={0}", senderId);
            sbPostData.AppendFormat("&route={0}", "4");
            try
            {
                //Call Send SMS API
                string sendSMSUri = "https://control.msg91.com/api/sendhttp.php";
                //Create HTTPWebrequest
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sendSMSUri);
                //Prepare and Add URL Encoded data
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] data = encoding.GetBytes(sbPostData.ToString());
                //Specify post method
                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/x-www-form-urlencoded";
                httpWReq.ContentLength = data.Length;
                using (Stream stream = httpWReq.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                //Get the response
                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responseString = reader.ReadToEnd();

                //Close the response
                reader.Close();
                response.Close();
            }
            catch 
            {
               
            }
        }

        public static int SendSmsUserIdPass(string mobileNumber)
        {
            string var = "12345";
           string dltid = "1407169054660166033";
              mobileNumber = "6398161629";
            string m = $"Dear  {var}, Thank You for Joining PS Wellness \r\nNow you Can Login With Your Registered User Id: {var} and Password: {var}";

            string authKey = "153995AnO2vKHdOPk59294dd7";
            //Multiple mobiles numbers separated by comma
            //Sender ID,While using route4 sender id should be 6 characters long.
            string senderId = "CHEIND";
            //Your message to send, Add URL encoding here.
            string message = HttpUtility.UrlEncode(m);

            //Prepare you post parameters
            StringBuilder sbPostData = new StringBuilder();
            sbPostData.AppendFormat("authkey={0}", authKey);
            sbPostData.AppendFormat("&mobiles={0}", mobileNumber);
            sbPostData.AppendFormat("&message={0}", message);
            sbPostData.AppendFormat("&sender={0}", senderId);
            sbPostData.AppendFormat("&DLT_TE_ID={0}", dltid);
            sbPostData.AppendFormat("&route={0}", "4");

            try
            {
                //Call Send SMS API
                string sendSMSUri = "http://api.msg91.com/api/sendhttp.php";
                //Create HTTPWebrequest
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sendSMSUri);
                //Prepare and Add URL Encoded data
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] data = encoding.GetBytes(sbPostData.ToString());
                //Specify post method
                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/x-www-form-urlencoded";
                httpWReq.ContentLength = data.Length;
                using (Stream stream = httpWReq.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                //Get the response
                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responseString = reader.ReadToEnd();

                //Close the response
                reader.Close();
                response.Close();
                return 1;
            }
            catch
            {
                return 0;
            }

            return 1;
        }
    }
}