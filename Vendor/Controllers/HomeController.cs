using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vendor.CustomHandleError;
using BLL.gigade.Model;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using log4net;
using System.Configuration;
using BLL.gigade.Common;
namespace Vendor.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        static ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        private IVendorImplMgr _vendorImp;
        [CustomHandleError]
        public ActionResult Index()
        {
            //string vendorid = Request.QueryString["vendor_id"];
            //string key = Request.QueryString["key"];
            //string keysec = string.Empty;
            //if (!string.IsNullOrEmpty(vendorid) && !string.IsNullOrEmpty(key))
            //{
            //    int vendor_id = Convert.ToInt32(vendorid);
            //    _vendorImp = new VendorMgr(connectionString);
            //    string str = _vendorImp.GetLoginId(vendor_id);
            //    HashEncrypt hmd5 = new HashEncrypt();
            //    string mdlogin_id = hmd5.Md5Encrypt(str, "MD5");
            //    keysec = hmd5.Md5Encrypt(mdlogin_id + str, "MD5");
            //}
            //else
            //{
            //    return Redirect("/Error");
            //}
            //if (key != keysec)
            //{
            //    return Redirect("/Error");
            //}
            //else
            //{
            //    BLL.gigade.Model.Vendor vendor = new BLL.gigade.Model.Vendor();
            //    BLL.gigade.Model.Vendor ven = new BLL.gigade.Model.Vendor();
            //    vendor.vendor_id =Convert.ToUInt32( vendorid);
            //    _vendorImp = new VendorMgr(connectionString);
            //    ven = _vendorImp.GetSingle(vendor);
            //    Session["vendor"] = ven;
            //    Session["lgnName"] = ven.vendor_name_simple;
            //    return View();
            //}
            return View();
        }
        [CustomHandleError]
        public ActionResult Top()
        {
            return View();
        }

    }
}
