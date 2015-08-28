using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
namespace Admin.gigade.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        [CustomHandleError]
        public ActionResult Index()
        {
  
            if (Session["caller"]==null)
            {
                return Redirect("/Login");
            }
            else
            {
                return View();
            }
        }

        [CustomHandleError]
        public ActionResult Top()
        {
            return View();
        }
    }
}
