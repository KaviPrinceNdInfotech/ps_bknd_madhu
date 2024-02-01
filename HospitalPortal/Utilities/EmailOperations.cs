
using HospitalPortal.Models.DomainModels;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;

namespace HospitalPortal.Utilities
{
    public class EmailOperations
    {
         public static bool SendEmail(string recipeint, string subject, string msg, bool IsBodyHtml)
          {
            try
            {
                string sender = ConfigurationManager.AppSettings["smtpUser"];
                string password = ConfigurationManager.AppSettings["smtpPass"];
                MailMessage message = new MailMessage();
                message.From = new MailAddress(sender);
                message.To.Add(recipeint);
                message.Subject = subject;
                message.Body = msg;
                message.IsBodyHtml = IsBodyHtml;
                SmtpClient client = new SmtpClient();
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(sender, password);
                client.EnableSsl = true;
                client.Send(message);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public static bool SendEmail1(string recipeint, string subject, string msg, bool IsBodyHtml)
        {
            try
            {
                string sender = ConfigurationManager.AppSettings["smtpUser"];
                string password = ConfigurationManager.AppSettings["smtpPass"];
                MailMessage message = new MailMessage();
                message.From = new MailAddress(sender);
                message.To.Add(recipeint);
                message.Subject = subject;
                message.Body = msg;
                message.IsBodyHtml = IsBodyHtml;
                SmtpClient client = new SmtpClient();
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(sender, password);
                client.EnableSsl = true;
                client.Send(message);
                return true;
            }
            catch
            {
                return false;
            }

        }

        // new
        public static int SendEmainew(EmailEF emailef)
        {
            try
            {
                string sender = "pswellnes@gmail.com";
                string password = "maorkxgquanancri";
                MailMessage message = new MailMessage();
                message.From = new MailAddress(sender);
                message.To.Add(emailef.EmailAddress);
                message.Subject = emailef.Subject;
                message.Body = emailef.Message;
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient();
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(sender, password);
                client.EnableSsl = true;
                client.Send(message);

                return 1;
            }
            catch (Exception ex)
            {

                return 0;
            }
        }
        
        public static int SendEmainewpdf(EmailEF emailef)
        {

            try
            {
                //  byte[] pdfBytes = GeneratePdf();

                string sender = "pswellnes@gmail.com";
                string password = "maorkxgquanancri";
                MailMessage message = new MailMessage();
                message.From = new MailAddress(sender);
                message.To.Add(emailef.EmailAddress);
                message.Subject = emailef.Subject;
                message.Body = emailef.Message;
                System.Net.Mail.Attachment attachment;
                // attachment = new System.Net.Mail.Attachment("http://localhost:55405/DoctorRegistration/MedicinePdf");
                //string fileUrl = $"http://localhost:55405/DoctorRegistration/MedicinePdf?id={emailef.id}";
                string fileUrl = "http://test.pswellness.in/DoctorRegistration/MedicinePdf";

                // Download the file from the URL.
                using (HttpClient client1 = new HttpClient())
                {
                    var response = client1.GetAsync(fileUrl).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        byte[] fileBytes = response.Content.ReadAsByteArrayAsync().Result;

                        // Create an Attachment using the downloaded file bytes.
                        attachment = new Attachment(new MemoryStream(fileBytes), "your.pdf");
                        message.Attachments.Add(attachment);
                    }
                    else
                    {
                        // Handle the case where the URL request was not successful.
                        return 0;
                    }
                }
                //attachment = new System.Net.Mail.Attachment(MedicinePdf());
                message.Attachments.Add(attachment);
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient();
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(sender, password);
                client.EnableSsl = true;
                client.Send(message);

                return 1;
            }
            catch (Exception ex)
            {

                return 0;
            }
        }


        public class EmailEF
        {
            public string EmailAddress { get; set; }
            public string Subject { get; set; }
            public string Message { get; set; }
            public int? id { get; set; }
        }
    }
}