/*
* 文件名稱 :ConsumeStatisticController.cs
* 文件功能描述 :會員消費金額統計控制器
* 版權宣告 :鄭州分公司
* 開發人員 : 文博
* 版本資訊 : 1.0
* 日期 : 2015-7-28
* 修改人員 :無
* 版本資訊 : 1.0
* 日期 : 2015-7-28
* 修改備註 :無
 */
using BLL.gigade.Mgr;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.gigade.Controllers
{
    public class ConsumeStatisticController : Controller
    {
        //
        // GET: /ConsumeStatistic/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string SqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        ConsumeStatisticMgr csMgr;
        /// <summary>
        /// 會員消費統計列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ConsumeStatisticList()
        {
            return View();
        }
        #region 獲取會員消費統計列表數據 +GetUserOrdersSubtotalList()
        public HttpResponseBase GetUserOrdersSubtotalList()
        {
            int totalCount = 0;
            List<UserOrdersSubtotalQuery> stores = new List<UserOrdersSubtotalQuery>();
            string json = string.Empty;
            try
            {
                csMgr = new ConsumeStatisticMgr(SqlConnectionString);
                UserOrdersSubtotalQuery query = new UserOrdersSubtotalQuery();
                query.Start = Convert.ToInt32(Request.Params["Start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Params["Limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["Limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["startMoney"]))
                {
                    query.startMoney = Convert.ToDouble(Request.Params["startMoney"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["endMoney"]))
                {
                    query.endMoney = Convert.ToDouble(Request.Params["endMoney"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["startTime"]))
                {
                    query.startTime = Convert.ToDateTime(Request.Params["startTime"]);
                    query.startTime = Convert.ToDateTime(query.startTime.ToString("yyyy-MM-dd 00:00:00"));
                }
                if (!string.IsNullOrEmpty(Request.Params["endTime"]))
                {
                    query.endTime = Convert.ToDateTime(Request.Params["endTime"]);
                    query.endTime = Convert.ToDateTime(query.endTime.ToString("yyyy-MM-dd 23:59:59"));
                }
                if (!string.IsNullOrEmpty(Request.Params["searchType"]))
                {
                    query.searchType = Convert.ToInt32(Request.Params["searchType"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["searchKey"]))
                {
                    query.searchKey = Request.Params["searchKey"];
                }
                stores = csMgr.GetUserOrdersSubtotal(query, out totalCount);
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

    }
}
