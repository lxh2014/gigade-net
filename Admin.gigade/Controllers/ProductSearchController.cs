using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Mgr;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Admin.gigade.Controllers
{
    public class ProductSearchController : Controller
    {
        //
        // GET: /ProductSearch/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string SqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        //private static readonly string sphinxHost = GetParameterCode(sphinxHost);
        //private static readonly string sphinxPort = System.Configuration.ConfigurationManager.AppSettings["sphinxPort"].ToString();
        ProductSearchMgr searchMgr = null;
        SphinxExcludeMgr seMgr = null;
        public ActionResult ProductSearchList()
        {
            return View();
        }
        
        #region 搜索列表
        public HttpResponseBase GetList()
        {
            int totalCount = 0;
            List<ProductSearchQuery> stores = new List<ProductSearchQuery>();
            string json = string.Empty;
            try
            {
                searchMgr = new ProductSearchMgr(SqlConnectionString, "sphinxHost", "sphinxPort");
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false}";
            }
            try
            {
                ProductSearchQuery query = new ProductSearchQuery();
                query.Start = Convert.ToInt32(Request.Params["Start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Params["Limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["Limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["flag"]))
                {
                    query.flag = Request.Params["flag"];
                }
                if (!string.IsNullOrEmpty(Request.Params["searchKey"]))
                {
                    query.searchKey = Request.Params["searchKey"];
                }
                stores = searchMgr.GetProductSearchList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 排除食安關鍵字 + RemoveSystemKeyWord()
        public HttpResponseBase RemoveSystemKeyWord()
        {
            string json = string.Empty;
            ///返回的状态
            int state = 0;
            List<SphinxKeywordQuery> stores = new List<SphinxKeywordQuery>();
            SphinxExcludeQuery query = new SphinxExcludeQuery();
            seMgr = new SphinxExcludeMgr(SqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["Product_Id"]))
                {
                  query.product_id=Convert.ToInt32( Request.Params["Product_Id"]);
                }
                query.kdate = DateTime.Now;
                state = seMgr.InsertModel(query);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,state:" + state + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }
        #endregion

    }

}
