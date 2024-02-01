using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace HospitalPortal.BL
{
    public class Notification
    {

        public void Message(string deviceId, string title, string body)
        {
            //Get the Credentials from FireBase
            string serverKey = ConfigurationManager.AppSettings["SERVER_API_KEY"];
            string senderId = ConfigurationManager.AppSettings["SENDER_ID"];
            //WebRequest
            WebRequest Transaction = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            Transaction.Method = "post";
            //Server Key from FCM Server
            Transaction.Headers.Add(string.Format("Authorization:key={0}", serverKey));
            Transaction.Headers.Add(string.Format("Sender: id={0}", senderId));
            Transaction.ContentType = "application/json";
            Transaction.UseDefaultCredentials = true;
            Transaction.PreAuthenticate = true;
            Transaction.Credentials = CredentialCache.DefaultCredentials;
            var Notification = new
            {
                to = deviceId,
                priority = "high",
                content_available = true,
                notification = new
                {
                    body = body,
                    title = title,
                    badge = 1
                },
            };
            //Create an JsonObject to Serialize the Notification Message
            var serializer = new JavaScriptSerializer();
            var postbody = serializer.Serialize(Notification);
            //Pass Serialize model into Byte Array
            Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
            //Take the ByteArray Length and pass into Transaction WebRequest
            Transaction.ContentLength = byteArray.Length;
            using (Stream streamData = Transaction.GetRequestStream())
            {
                streamData.Write(byteArray, 0, byteArray.Length);
                using (WebResponse Tresponse = Transaction.GetResponse())
                {

                    using (Stream dataStreamResponse = Tresponse.GetResponseStream())
                    {

                        if (dataStreamResponse != null)
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                string ResponseFromServer = tReader.ReadToEnd();
                                tReader.Close();
                            }
                        dataStreamResponse.Close();
                    }
                    Tresponse.Close();

                }
            }
        }
    }
}