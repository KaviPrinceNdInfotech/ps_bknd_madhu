using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalPortal.Controllers
{
    public class UnsuccessfulAttemptController : Controller
    {
        // GET: UnsuccessfulAttempt
        public ActionResult Unsuccess()
        {
            return View();
        }
    }
}