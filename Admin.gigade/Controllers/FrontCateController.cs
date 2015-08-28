using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using BLL.gigade.Mgr;
using BLL.gigade.Model;
using BLL.gigade.Mgr.Impl;
using System.Configuration;
using Newtonsoft.Json;
using BLL.gigade.Model.Custom;


namespace Admin.gigade.Controllers
{
    public class FrontCateController : Controller
    {
        //
        // GET: /FrontCate/

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        //private IProductImplMgr _productMgr;
        //private IProductTempImplMgr _productTempMgr;
        //private IProductCategoryImplMgr _procateMgr;
        //private IProductCategorySetImplMgr _categorySetMgr;
        //private IProductCategorySetTempImplMgr _categoryTempSetMgr;
        public ActionResult Index()
        {
            return View();
        }

    }
}
