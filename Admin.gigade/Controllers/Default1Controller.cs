using BLL.gigade.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.gigade.Controllers
{
    public class Default1Controller : Controller
    {
        //
        // GET: /Default1/

        public ActionResult Index()
        {
            string a = EncryptComputer.EncryptDecryptTextByApi("1");
            string b = EncryptComputer.EncryptDecryptTextByApi("EAAAAFQBc0fDqZLg63ultKK44dikzbUxn+HK5W1gmB4TpbJF",false);

            Response.Write(a+"<br/>");
            Response.Write(b);
            return View();
        }

    }
}
