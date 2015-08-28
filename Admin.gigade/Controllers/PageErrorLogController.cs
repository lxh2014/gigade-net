using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Mgr;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Admin.gigade.Controllers
{
    public class PageErrorLogController : Controller
    {
        //
        // GET: /PageErrorLog/

        private static readonly string SqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        PageErrorLogMgr pelMgr = new PageErrorLogMgr(SqlConnectionString);
     
        /// <summary>
        /// 總行數
        /// </summary>
        int totalCount =0;

        [OutputCache(Duration = 3600, VaryByParam = "paraType", Location = System.Web.UI.OutputCacheLocation.Client)]
        public string QueryPara()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["paraType"]))
                {
                    json = pelMgr.QueryPara(Request.QueryString["paraType"].ToString());
                }
               
            }
            catch (Exception)
            {
                json = "[]";
            }
            return json;
        }


        /// <summary>
        /// 錯誤記錄列表頁
        /// </summary>
        /// <returns></returns>
        public ActionResult PageList()
        {
             return View();
        }
        /// <summary>
        /// 獲取列表數據
        /// </summary>
        /// <returns></returns>
        public  HttpResponseBase  GetList()
        {

            List<PageErrorLogQuery> stores = new List<PageErrorLogQuery>();
            string json = string.Empty;
            try
            {
                PageErrorLogQuery query = new PageErrorLogQuery();
                query.Start = Convert.ToInt32(Request.Params["Start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Params["Limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["Limit"]);
                }

                query.searchType = Convert.ToInt32(Request.Params["errorType"]);
                query.searchKey = Request.Params["searchKey"];
                query.startT = Request.Params["startT"];
                query.endT = Request.Params["endT"];
                ///獲取滿足條件的數據
                stores = pelMgr.GetData(query,out  totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception)
            {
                json = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
       
    }
}
