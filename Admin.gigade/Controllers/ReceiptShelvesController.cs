using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Common;
using BLL.gigade.Mgr;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Admin.gigade.Controllers
{
    public class ReceiptShelvesController : Controller
    {
        //
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        IpoNvdMgr _IpoNvdMgr;


        #region View

        // GET: /ReceiptShelves/
        /// <summary>
        /// 收貨上架列表
        /// </summary>
        /// <returns></returns>
        public ActionResult IpoNvdList()
        {
            return View();
        } 


        #endregion


        //採購收穫上架記錄
        public HttpResponseBase GetIpoNvdList()// 獲取period數據
        {
            string json = string.Empty;
            IpoNvdQuery query = new IpoNvdQuery();
            int totalCount = 0;
            try
            {
                List<SchedulePeriodQuery> ipodStore = new List<SchedulePeriodQuery>();
                _IpoNvdMgr = new IpoNvdMgr(mySqlConnectionString);
                List<IpoNvdQuery> store = _IpoNvdMgr.GetIpoNvdList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }
        //生成理货单
        public HttpResponseBase CreateTallyList()
        {
            string json = String.Empty;
            json = "{success:false}";
            IpoNvdQuery query = new IpoNvdQuery();
            string id=string.Empty;
            try 
	        {	        
		        if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    id = Request.Params["id"].ToString();
                    id = id.Substring(0, id.Length - 1).ToString();
                    query.modify_user = (Session["caller"] as Caller).user_id;
                }
                query.work_id = "IN" + DateTime.Now.ToString("yyyyMMddHHmmss");
                if(!string.IsNullOrEmpty(id))
                {
                    _IpoNvdMgr = new IpoNvdMgr(mySqlConnectionString);
                    int result = _IpoNvdMgr.CreateTallyList( query, id);
                    if (result > 0)
                    {
                        json = "{success:true,work_id:\""+query.work_id+"\"}";
                    }
                }
                
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

    }
}
