using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vendor.Controllers
{
    public class VendorStockController : Controller
    {
        //
        // GET: /VendorStock/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IProductImplMgr _IProductMgr;
        private ProductMgr productMgr;
        public ActionResult VendorProductList()
        {
            BLL.gigade.Model.Vendor vendorModel = (BLL.gigade.Model.Vendor)Session["vendor"];
            VendorBrand vb = new VendorBrand();
            ViewBag.vendor_id = vendorModel.vendor_id;
            return View();
        }

        #region 獲取到某個供應商頁面信息
        public HttpResponseBase GetVendorProductList()
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            ProductQuery query = new ProductQuery();
            int totalCount = 0;
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                query.Vendor_Id = Convert.ToUInt32(Request.Params["vendor_id"]);
                query.searchcontent = Request.Params["searchcontent"].Replace('，',',').Replace('|',',');
                query.this_product_state = Convert.ToInt32(Request.Params["product_state"]);//產品狀態
                _IProductMgr = new ProductMgr(mySqlConnectionString);
                _dt = _IProductMgr.GetVendorProductList(query, out  totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented, timeConverter) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json.ToString());
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 獲取商品規格信息
        public HttpResponseBase GetVendorProductSpec()
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            ProductQuery query = new ProductQuery();
            int totalCount = 0;
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                query.Product_Id = Convert.ToUInt32(Request.Params["product_id"]);
                _IProductMgr = new ProductMgr(mySqlConnectionString);
                _dt = _IProductMgr.GetVendorProductSpec(query, out  totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented, timeConverter) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json.ToString());
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 更改庫存數量
        public JsonResult UpdateStock()
        {
            string jsonStr = string.Empty;
            try
            {  
                int newstock = 0;
                int item_id = 0;
                int oldstock = 0;
                int vendor_id = 0;
                if (!string.IsNullOrEmpty(Request.Params["newstock"]))
                {
                     newstock = Convert.ToInt32(Request.Params["newstock"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["item_id"]))
                {
                     item_id = Convert.ToInt32(Request.Params["item_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["oldstock"]))
                {
                     oldstock = Convert.ToInt32(Request.Params["oldstock"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["vendor_id"]))
                {
                     vendor_id = Convert.ToInt32(Request.Params["vendor_id"]);
                }
                _IProductMgr = new ProductMgr(mySqlConnectionString);

                if (_IProductMgr.UpdateStock(newstock, oldstock, item_id, vendor_id) == true)
                {
                    return Json(new { success = "true" });
                }
                else
                {
                    return Json(new { success = "false" });
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json(new { success = "false" });
            }

        }
        #endregion

        #region 更改規格顯示或者隱藏
        public JsonResult UpdateStatus()
        {
            string jsonStr = string.Empty;
            try
            {  
                int spec_id=0;
                int spec_status=0;
                if (!string.IsNullOrEmpty(Request.Params["spec_id"]))
                {
                    spec_id = Convert.ToInt32(Request.Params["spec_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["spec_status"]))
                {
                    spec_status = Convert.ToInt32(Request.Params["spec_status"]);
                }
                _IProductMgr = new ProductMgr(mySqlConnectionString);

                if (_IProductMgr.UpdateStatus(spec_id, spec_status) == true)
                {
                    return Json(new { success = "true" });
                }
                else
                {
                    return Json(new { success = "false" });
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json(new { success = "false" });
            }

        }
        #endregion

        public JsonResult UpdateShortage()
        {
            int product_id = Convert.ToInt32(Request.Params["product_id"] ?? "0");
                int shortage = Convert.ToInt32(Request.Params["shortage"] ?? "0");
            productMgr=new ProductMgr(mySqlConnectionString);
            try
            {
                if (productMgr.UpdateShortage(product_id, shortage) > 0)
                {
                    return Json(new { success = "true" });
                }
                else
                {
                    return Json(new { success = "false" });
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json(new { success = "false" });
            }
        }
    }
}
