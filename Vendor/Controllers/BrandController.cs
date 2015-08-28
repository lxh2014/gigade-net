using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Model;
using BLL.gigade.Mgr;
using System.Configuration;
using Vendor.CustomHandleError;

namespace Vendor.Controllers
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
        public string QueryBrand()
        {
            VendorBrand vb = new VendorBrand();
            vb.Vendor_Id = Convert.ToUInt32((Session["vendor"] as BLL.gigade.Model.Vendor).vendor_id);
            vbMgr = new VendorBrandMgr(connectionString);
            //edit by Castle
            //date：2014/07/18
            //要求不限制
            //vb.Brand_Status = 1;
            string json = vbMgr.QueryBrand(vb);
            return json;
        }

        #region //
        //[CustomHandleError]
        //[OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Client)]
        //public string QueryBrand()
        //{
        //    string json = string.Empty;

        //    string strVendorId = Request.QueryString["vendor_id"];

        //    VendorBrand vb = new VendorBrand();
        //    uint vendor_id;
        //    if (uint.TryParse(strVendorId, out vendor_id))
        //    {
        //        vb.Vendor_Id = vendor_id;
        //    }

        //    vbMgr = new VendorBrandMgr(connectionString);
        //    json = vbMgr.QueryBrand(vb);
        //    return json;
        //} 
        #endregion
    }
}
