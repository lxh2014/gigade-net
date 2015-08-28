using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using BLL.gigade.Model;
using BLL.gigade.Mgr;
using System.Configuration;

namespace Admin.gigade.Controllers
{
    public class BrandController : Controller
    {
        //
        // GET: /Brand/
        private VendorBrandMgr vbMgr;
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];

        public ActionResult Index()
        {
            return View();
        }

        [CustomHandleError]
        [OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Client)]
        public string QueryBrand(int hideOffGrade = 0)
        {
            VendorBrand vb = new VendorBrand();
            vbMgr = new VendorBrandMgr(connectionString);
            //edit by Castle
            //date：2014/07/18
            //要求不限制
            //vb.Brand_Status = 1;
            string json = vbMgr.QueryBrand(vb, hideOffGrade);
            return json;
        }

    }
}
