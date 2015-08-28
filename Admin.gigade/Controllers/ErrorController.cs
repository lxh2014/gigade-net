using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.gigade.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            this.Response.Status = "404 Error";
            this.Response.StatusCode = 404;
            return View();
        }

    }
}
