using HospitalPortal.Models.APIModels;
using HospitalPortal.Models.DomainModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace HospitalPortal.Controllers
{
    public class TestListController : Controller
    {
        DbEntities ent = new DbEntities();
        // GET: TestList
        public ActionResult Index(int id, DateTime? AppointmentDate = null)
        {
            //Call The Model
            var model = new PaymentHistroyForDosctor();
            // Use Http Client to Initialize HttpClient Class
            using (var client = new HttpClient())
            {
                //Call BaseAddress of API
                client.BaseAddress = new Uri("http://localhost:55405/api/");
                //Call WebApi Address With  Controller Name and ActionName + Parameters (If Any)
                var response = client.GetAsync("DoctorApi/payment?Id="+id+"&Date="+ AppointmentDate);
                response.Wait();
                //Store the Response Result
                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    //Read the Content
                    var readTask = result.Content.ReadAsAsync<List<ListPayment>>();
                    //Serialise the JSON Object
                    string jsonstring = JsonConvert.SerializeObject(readTask);
                    readTask.Wait();
                    //Store the Result into IEnumerable List
                    model.PaymentHistory = readTask.Result;
                }
                else
                {
                    TempData["msg"] = "No Record";
                }
            }
                return View(model);
        }
    }
}