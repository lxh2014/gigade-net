

#region 文件信息
/* 
* Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：VendorBrandController.cs      
* 摘 要：                                                                               
* 獲取該供應商下的所有品牌
* 当前版本：v1.0                                                                
* 作 者： shuangshuang0420j                                           
* 完成日期：
* 
*/

#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using BLL.gigade.Model;
using BLL.gigade.Mgr;
using System.Configuration;


namespace Vendor.Controllers
{
    public class VendorBrandController : Controller
    {
        //
        // GET: /Brand/

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        private VendorBrandMgr vbMgr;
        public ActionResult Index()
        {
            return View();
        }


        [OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Client)]
        public string QueryBrand()
        {
            string json = string.Empty;
            try
            {
                BLL.gigade.Model.Vendor vendorModel=(BLL.gigade.Model.Vendor)Session["vendor"];
                VendorBrand vb = new VendorBrand();
                vb.Vendor_Id = vendorModel.vendor_id;//獲取該供應商下的所有品牌，暫時寫死
                vbMgr = new VendorBrandMgr(connectionString);

                json = vbMgr.QueryBrand(vb);

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return json;
        }


    }
}
