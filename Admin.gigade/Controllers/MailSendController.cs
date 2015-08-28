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
    public class MailSendController : Controller
    {
        //
        // GET: /MailSend/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string SqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        MailSendMgr mailMgr = new MailSendMgr(SqlConnectionString);
        /// <summary>
        /// 總行數
        /// </summary>
        int totalCount = 0;


        public ActionResult List()
        {
            return View();
        }

        #region 獲取列表數據
        /// <summary>
        /// 獲取列表數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetList()
        {
            List<MailSendQuery> stores = new List<MailSendQuery>();
            string json = string.Empty;
            try
            {
                MailSendQuery query = new MailSendQuery();
                query.Start = Convert.ToInt32(Request.Params["Start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Params["Limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["Limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["startT"]))
                {
                    query.startT = Request.Params["startT"];
                }
                if (!string.IsNullOrEmpty(Request.Params["endT"]))
                {
                    query.endT = Request.Params["endT"];
                }
                if (!string.IsNullOrEmpty(Request.Params["search"]))
                {
                    query.search = Request.Params["search"];
                }
                stores = mailMgr.GetData(query, out totalCount);
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
