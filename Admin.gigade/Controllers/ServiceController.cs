using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using BLL.gigade.Common;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using gigadeExcel.Comment;
using BLL.gigade.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;
using System.Net.Mail;
using System.Net; 

namespace Admin.gigade.Controllers
{
    public class ServiceController : Controller
    {
        //
        // GET: /Service/
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//郵件服務器的設置
        string FromName = ConfigurationManager.AppSettings["ContactFromName"];//發件人姓名
        string EmailTile = ConfigurationManager.AppSettings["ContactEmailTile"];//郵件標題
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IArrivalNoticeImplMgr _IArrivalNoticeMgr;
        private ISmsLogImplMgr _IsmsLogMgr;

        private ISmsImplMgr _ISmsMgr;
        private IOrderQuestionIMgr _IOrderQuesMgr;
        private IOrderResponseIMgr _IOrderResponseMgr;
        static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        private IContactUsQuestionImplMgr _ctactMgr;
        private IContactUsResponseImplMgr _ctactrsponseMgr;

        UsersListMgr _usmgr = new UsersListMgr(mySqlConnectionString);

        #region View
        public ActionResult Index()
        {
            return View();
        }
        //客戶問題頁面
        public ActionResult OrderQuestion()
        {
            return View();
        }
        //新增訂單問題頁面
        public ActionResult OrderQuestionAdd()
        {
            ViewBag.OrderId = Request.QueryString["Order_Id"] ?? "";//獲取付款單號
            return View();
        }
        //補貨追蹤列表
        public ActionResult ArrivalNotice()
        {
            return View();
        }

        /// <summary>
        /// 簡訊查詢列表頁
        /// </summary>
        /// <returns></returns>
        public ActionResult SmsSearchIndex()
        {
            if (!string.IsNullOrEmpty(Request.Params["SMSID"]))
            {
                ViewBag.sms_id = Request.Params["SMSID"];
            }
            if (!string.IsNullOrEmpty(Request.Params["StartTime"]))
            {
                ViewBag.start_time = Request.Params["StartTime"];
            }
            else
            {
                ViewBag.start_time = 0;
            }
            return View();
        }
        //簡訊發送記錄頁面
        public ActionResult SendSmsRecord()
        {
            if (!string.IsNullOrEmpty(Request.Params["SMSID"]))
            {
                ViewBag.sms_id = Request.Params["SMSID"];
            }
            if (!string.IsNullOrEmpty(Request.Params["StartTime"]))
            {
                ViewBag.start_time = Request.Params["StartTime"];
            }
            else
            {
                ViewBag.start_time = 0;
            }
            return View();
        }
        /// <summary>
        /// 回覆訂單問題和意見
        /// </summary>
        /// <returns>回覆訂單問題和意見視圖</returns>
        public ActionResult OrderQuestionResponse()
        {
            string question_id = Request.Params["question_id"];
            string order_id = Request.Params["order_id"];
            string status = Request.Params["status"];
            ViewBag.question_id = question_id;
            ViewBag.order_id = order_id;
            ViewBag.status = status;
            return View();
        }
        #endregion

        public ActionResult ContactUsQuestionResponse()
        {
            return View();
        }
        public ActionResult ContactRecord()
        {
            ViewBag.question_id = Request.Params["qid"];
            return View();
        }

        #region 訂單問題列表頁
        [CustomHandleError]
        public HttpResponseBase OrderQuestionlist()
        {
            List<OrderQuestionQuery> store = new List<OrderQuestionQuery>();
            string json = string.Empty;
            int totalCount = 0;
            _IOrderQuesMgr = new OrderQuestionMgr(mySqlConnectionString);
            try
            {
                OrderQuestionQuery query = new OrderQuestionQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ddlSel"]))
                {
                    query.ddlSel = Convert.ToInt32(Request.Params["ddlSel"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["selcontent"]))
                {
                    query.selcontent = Request.Params["selcontent"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["ddtSel"]))
                {
                    query.ddtSel = Convert.ToInt32(Request.Params["ddtSel"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["time_start"]))
                {
                    query.question_createdates = Convert.ToDateTime(Request.Params["time_start"]);
                    query.time_start = Convert.ToUInt32(CommonFunction.GetPHPTime(query.question_createdates.ToString()));
                }
                if (!string.IsNullOrEmpty(Request.Params["time_end"]))
                {
                    query.response_createdates = Convert.ToDateTime(Request.Params["time_end"]);
                    query.time_end = Convert.ToUInt32(CommonFunction.GetPHPTime(query.response_createdates.ToString()));
                }
                if (!string.IsNullOrEmpty(Request.Params["ddrSel"]))
                {
                    query.question_type = Convert.ToUInt32(Request.Params["ddrSel"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ddlstatus"]))
                {
                    query.ddlstatus = Request.Params["ddlstatus"];
                }
                else
                {
                    query.ddlstatus = "-1";
                }
                if (!string.IsNullOrEmpty(Request.Params["relation_id"]))//待回覆
                {
                    query.question_id = Convert.ToUInt32(Request.Params["relation_id"]);
                }
                store = _IOrderQuesMgr.GetOrderQuestionList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                foreach (var item in store)
                {
                    if (Convert.ToBoolean(Request.Params["isSecret"]))
                    {
                        if (!string.IsNullOrEmpty(item.question_username))
                        {
                            item.question_username = item.question_username.Substring(0, 1) + "**";
                        }
                        item.question_email = item.question_email.Split('@')[0] + "@***";
                        if (item.question_phone.ToString().Length > 3)
                        {
                            item.question_phone = item.question_phone.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.question_phone = item.question_phone + "***";
                        }
                    }
                    item.question_createdates = CommonFunction.GetNetTime(item.question_createdate);
                    item.response_createdates = CommonFunction.GetNetTime(item.response_createdate);
                }
                //listUser是准备转换的对象
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 訂單列表狀態更改
        public JsonResult UpdateOrderQuestionActive()
        {
            string json = string.Empty;
            OrderQuestionQuery query = new OrderQuestionQuery();
            _IOrderQuesMgr = new OrderQuestionMgr(mySqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.question_id = Convert.ToUInt32(Request.Params["id"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["active"]))
                {
                    query.question_status = Convert.ToUInt32(Request.Params["active"]);
                }
                if (query.question_id != 0)
                {
                    _IOrderQuesMgr.UpdateQuestionStatus(query);
                    return Json(new { success = "true", msg = "" });
                }
                else
                {
                    return Json(new { success = "false", msg = "" });
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json(new { success = "false", msg = "" });
            }
        }
        #endregion

        #region 簡訊查詢列表頁
        public HttpResponseBase SmsList()
        {
            string jsonStr = string.Empty;

            try
            {
                List<SmsQuery> SmsStore = new List<SmsQuery>();
                SmsQuery query = new SmsQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["datequery"]))
                {
                    int type = int.Parse(Request.Params["datequery"]);
                    if (type == 1)
                    {
                        query.send = 1;
                        query.modified_time = DateTime.Now.ToString();
                    }
                    else if (type == 0)
                    {
                        query.created_time = DateTime.Now.ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Params["time_start"]))
                    {
                        query.StartTime = DateTime.Parse(Request.Params["time_start"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["time_end"]))
                    {
                        query.EndTime = DateTime.Parse(Request.Params["time_end"]);
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["searchcontent"]))
                {
                    query.content = Request.Params["searchcontent"];
                }
                if (!string.IsNullOrEmpty(Request.Params["send"]))
                {
                    query.send = Convert.ToInt32(Request.Params["send"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["trustsend"]))
                {
                    int trustsend = 0;
                    if (int.TryParse(Request.Params["trustsend"].ToString(), out trustsend))
                    {
                        if (trustsend == 1)
                        {
                            query.trust_send = "1";
                        }
                        else if (trustsend == 0)
                        {
                            query.trust_send = "0,2";
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["relation_id"]))//待回覆
                {
                    query.id = Convert.ToInt32(Request.Params["relation_id"]);
                }
                int totalCount = 0;
                _ISmsMgr = new SmsMgr(mySqlConnectionString);
                SmsStore = _ISmsMgr.GetSmsList(query, out totalCount);
                foreach (var item in SmsStore)
                {
                    if (Convert.ToBoolean(Request.Params["isSecret"]))
                    {
                        if (!string.IsNullOrEmpty(item.mobile))
                        {
                            if (item.mobile.ToString().Length > 3)
                            {
                                item.mobile = item.mobile.Substring(0, 3) + "***";
                            }
                            else
                            {
                                item.mobile = item.mobile.ToString() + "***";
                            }
                        }
                    }
                    item.created_time = "";
                    if (item.created > DateTime.Parse("1970-01-01 08:00:00"))
                    {
                        item.created_time = item.created.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    item.estimated_time = "";
                    if (item.estimated_send_time > DateTime.Parse("1970-01-01 08:00:00"))
                    {
                        item.estimated_time = item.estimated_send_time.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    item.modified_time = "";
                    if (item.modified > DateTime.Parse("1970-01-01 08:00:00"))
                    {
                        item.modified_time = item.modified.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                jsonStr = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(SmsStore, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false,msg:0}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase updateSms()
        {
            string jsonStr = String.Empty;
            int result = 0;
            SmsQuery query = new SmsQuery();

            try
            {
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.id = int.Parse(Request.Params["id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["content"]))
                {
                    query.content = Request.Params["content"];
                }
                if (Convert.ToBoolean(Request.Params["state1"]) == true)
                {
                    query.send = 0;//1表示已經回覆
                }
                else if (Convert.ToBoolean(Request.Params["state2"]) == true)
                {
                    query.send = 3;//1表示已經回覆
                }
                _ISmsMgr = new SmsMgr(mySqlConnectionString);
                result = _ISmsMgr.updateSms(query);
                if (result > 0)
                {
                    jsonStr = "{success:true}";
                }
                else
                {
                    jsonStr = "{success:false}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }


        #endregion

        #region 補貨追蹤
        #region 補貨追蹤列表頁+HttpResponseBase GetArrivalNoticeList
        public HttpResponseBase GetArrivalNoticeList()
        {
            List<ArrivalNoticeQuery> store = new List<ArrivalNoticeQuery>();
            ArrivalNoticeQuery query = new ArrivalNoticeQuery();
            int totalCount = 0;
            string json = string.Empty;
            try
            {

                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["condition"]))
                {
                    query.condition = Convert.ToInt32(Request.Params["condition"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["searchCon"]))
                {
                    query.searchCon = Request.Params["searchCon"].ToString().Trim();
                }
                if (!string.IsNullOrEmpty(Request.Params["status"]))
                {
                    query.status = Convert.ToInt32(Request.Params["status"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["searchNum"]))
                {
                    query.item_stock = Convert.ToInt32(Request.Params["searchNum"]);
                }
                _IArrivalNoticeMgr = new ArrivalNoticeMgr(mySqlConnectionString);
                store = _IArrivalNoticeMgr.ArrivalNoticeList(query, out totalCount);
                foreach (var item in store)
                {
                    if (!string.IsNullOrEmpty(item.user_name.ToString()))
                    {
                        item.user_name = item.user_name.Substring(0, 1) + "**";
                    }
                    item.user_email = item.user_email.Split('@')[0] + "@***";

                    item.s_create_time = CommonFunction.GetNetTime(item.create_time);
                    if (item.coming_time != 0)
                    {
                        item.s_coming_time = CommonFunction.DateTimeToString(CommonFunction.GetNetTime(item.coming_time));
                    }
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";
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
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 修改通知狀態 +HttpResponseBase IgnoreNotice()
        public HttpResponseBase IgnoreNotice()
        {
            List<ArrivalNoticeQuery> list = new List<ArrivalNoticeQuery>();
            ArrivalNoticeQuery query = null;
            string json = string.Empty;
            MailHelper mailHelper = new MailHelper();
            string mailBody;

            try
            {
                if (!string.IsNullOrEmpty(Request.Params["rowID"]))
                {
                    string select_id = Request.Params["rowID"];
                    if (select_id.IndexOf("∑") != -1)
                    {
                        foreach (string id in select_id.Split('∑'))
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                string[] data = id.Split(',');

                                query = new ArrivalNoticeQuery();
                                query.status = Convert.ToInt32(Request.Params["type"]);
                                query.id = Convert.ToUInt32(data[0]);
                                query.user_email = _usmgr.getModel(Convert.ToInt32(data[1].ToString())).user_email;
                                query.product_name = data[2];
                                list.Add(query);
                            }
                        }
                    }
                }
                _IArrivalNoticeMgr = new ArrivalNoticeMgr(mySqlConnectionString);
                if (_IArrivalNoticeMgr.IgnoreNotice(list))
                {
                    FileStream fs = new FileStream(Server.MapPath("../ImportUserIOExcel/ArrivalNotice.html"), FileMode.OpenOrCreate, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                    string strTemp = sr.ReadToEnd();
                    for (int i = 0; i < list.Count; i++)
                    {
                        mailBody = "您關注補貨的商品" + "【" + list[i].product_name + "】" + "已到貨。  ";
                        strTemp = strTemp.Replace("{{$serviceAnwser$}}", mailBody);
                        try
                        {
                            mailHelper.SendMailAction(list[i].user_email, "吉甲地市集補貨通知信", strTemp);
                        }
                        catch (Exception ex)
                        {
                            json = "{success:true,msg:1}";
                        }
                    }
                    json = "{success:true,msg:0}";
                }
                else
                {
                    json = "{failure:true}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{failure:true}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 不再補貨或預計捕獲
        public HttpResponseBase NoOrComing()
        {
            List<ArrivalNoticeQuery> list = new List<ArrivalNoticeQuery>();
            ArrivalNoticeQuery query = null;
            string json = string.Empty;
            string mailBody;
            MailHelper mailHelper = new MailHelper();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["rowID"]))
                {
                    string select_id = Request.Params["rowID"];
                    if (select_id.IndexOf("∑") != -1)
                    {
                        foreach (string id in select_id.Split('∑'))
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                query = new ArrivalNoticeQuery();
                                string[] data = id.Split(',');
                                query.status = Convert.ToInt32(Request.Params["type"]);
                                query.id = Convert.ToUInt32(data[0]);
                                query.user_email = _usmgr.getModel(Convert.ToInt32(data[1].ToString())).user_email;
                                query.product_name = data[2];
                                if (!string.IsNullOrEmpty(Request.Params["coming_time"]))
                                {
                                    query.coming_time = Convert.ToInt32(CommonFunction.GetPHPTime(Request.Params["coming_time"].ToString()));
                                }
                                if (!string.IsNullOrEmpty(Request.Params["recommend"]))
                                {
                                    query.recommend = Request.Params["recommend"].ToString();
                                }
                                list.Add(query);
                            }
                        }
                    }
                }
                _IArrivalNoticeMgr = new ArrivalNoticeMgr(mySqlConnectionString);
                if (_IArrivalNoticeMgr.IgnoreNotice(list))
                {
                    FileStream fs = new FileStream(Server.MapPath("../ImportUserIOExcel/ArrivalNotice.html"), FileMode.OpenOrCreate, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                    string strTemp = sr.ReadToEnd();
                    for (int i = 0; i < list.Count; i++)
                    {

                        #region 不再補貨
                        if (list[i].status == 2)
                        {
                            if (!string.IsNullOrEmpty(list[i].recommend))
                            {
                                mailBody = "您關注補貨的商品" + "【" + list[i].product_name + "】" + "已不再進貨/販售，建議您購買我們推薦的類似商品：" + list[i].recommend;
                            }
                            else
                            {
                                mailBody = "您關注補貨的商品" + "【" + list[i].product_name + "】" + "已不再進貨/販售。  ";
                            }
                            strTemp = strTemp.Replace("{{$serviceAnwser$}}", mailBody);
                        }
                        #endregion

                        #region 預計補貨
                        if (list[i].status == 3)
                        {
                            mailBody = "您關注補貨的商品" + "【" + list[i].product_name + "】" + "將預計在" + CommonFunction.DateTimeToString(CommonFunction.GetNetTime(list[i].coming_time)) + "補貨，敬請及早購買，以免向隅。  ";
                            strTemp = strTemp.Replace("{{$serviceAnwser$}}", mailBody);
                        }
                        #endregion
                        try
                        {
                            mailHelper.SendMailAction(list[i].user_email, "吉甲地市集補貨通知信", strTemp);
                        }
                        catch (Exception ex)
                        {
                            json = "{success:true,msg:1}";
                        }
                    }

                    json = "{success:true,msg:0}";
                }
                else
                {
                    json = "{failure:true}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{failure:true}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #endregion

        #region 簡訊發送記錄列表
        public HttpResponseBase SendSmsRecordList()
        {
            _IsmsLogMgr = new SmsLogMgr(mySqlConnectionString);
            List<SmsLogQuery> store = new List<SmsLogQuery>();
            SmsLogQuery query = new SmsLogQuery();
            string json = string.Empty;
            int totalCount = 0;
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["provider"]))
                {
                    query.provider = int.Parse(Request.Params["provider"]);
                }
                query.success = -1;
                if (!string.IsNullOrEmpty(Request.Params["success"]))
                {
                    query.success = int.Parse(Request.Params["success"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["SmsId"]))
                {
                    query.sms_id = int.Parse(Request.Params["SmsId"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["start_time"]))
                {
                    query.created = DateTime.Parse(Request.Params["start_time"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["end_time"]))
                {
                    query.modified = DateTime.Parse(Request.Params["end_time"]);
                }

                store = _IsmsLogMgr.GetSmsLog(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 獲取回覆記錄列表
        /// <summary>
        /// 獲取回覆記錄列表
        /// </summary>
        /// <returns>回覆記錄json格式數據</returns>
        public HttpResponseBase GetOrderQuestionResponseList()
        {
            string json = string.Empty;
            DataTable dtQuestionResponseList = new DataTable();
            int totalCount = 0;
            OrderQuestionQuery query = new OrderQuestionQuery();
            string question_id = Request.Params["question_id"];
            if (!string.IsNullOrEmpty(question_id))
            {
                query.question_id = Convert.ToUInt32(question_id);
            }
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");

            _IOrderQuesMgr = new OrderQuestionMgr(mySqlConnectionString);

            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
            timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            try
            {
                dtQuestionResponseList = _IOrderQuesMgr.GetList(query, out totalCount);
                if (dtQuestionResponseList.Rows.Count > 0)
                {
                    #region //
                    //for (int i = 0; i < dtQuestionResponseList.Rows.Count; i++)
                    //{
                    //    string user_id = dtQuestionResponseList.Rows[i]["user_id"].ToString();
                    //    string product_id = dtQuestionResponseList.Rows[i]["product_id"].ToString();
                    //    string item_id = string.Empty;
                    //    List<ProductItem> listProductItem = _IProductItemMgr.GetProductItemByID(Convert.ToInt32(product_id));
                    //    if (listProductItem.Count > 0)
                    //    {
                    //        item_id = listProductItem[0].Item_Id.ToString();
                    //    }
                    //    OrderMasterQuery orderMasterQuery = new OrderMasterQuery
                    //    {
                    //        User_Id = Convert.ToUInt32(dtQuestionResponseList.Rows[i]["user_id"]),
                    //        Item_Id = Convert.ToInt32(item_id)
                    //    };
                    //    dtQuestionResponseList.Rows[i]["buyCount"] = _IOrderMasterMgr.GetBuyCount(orderMasterQuery);
                    //} 
                    #endregion
                    json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(dtQuestionResponseList, Formatting.Indented, timeConverter) + "}";
                }
                else
                {
                    json = "{success:false,msg:0}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:0}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 聯絡客服列表
        public HttpResponseBase GetContactUsQuestionList()
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            ContactUsQuestionQuery cuQuery = new ContactUsQuestionQuery();
            cuQuery.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            cuQuery.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            try
            {

                cuQuery.search_type = Convert.ToInt32(Request.Params["search_type"]);
                cuQuery.searchcontent = Request.Params["searchcontent"];
                if (!string.IsNullOrEmpty(Request.Params["dateStart"]))
                {
                    cuQuery.datestart = Convert.ToDateTime(Request.Params["dateStart"]);//建立時間
                }
                if (!string.IsNullOrEmpty(Request.Params["dateEnd"]))
                {
                    cuQuery.dateend = Convert.ToDateTime(Request.Params["dateEnd"]);
                }
                cuQuery.question_type = Convert.ToUInt32(Request.Params["qusetion_type"]);
                if (Convert.ToBoolean(Request.Params["radio2"]) == true)//待回覆
                {
                    cuQuery.question_status = 3;
                }
                else if (Convert.ToBoolean(Request.Params["radio3"]) == true)//已回覆
                {
                    cuQuery.question_status = 4;
                }
                else if (Convert.ToBoolean(Request.Params["radio4"]) == true)//已處理
                {
                    cuQuery.question_status = 2;
                }
                if (!string.IsNullOrEmpty(Request.Params["relation_id"]))
                {
                    cuQuery.question_id = Convert.ToUInt32(Request.Params["relation_id"]);
                }
                if (Convert.ToBoolean(Request.Params["isSecret"]))
                {
                    cuQuery.isSecret = true;
                }
                int totalCount = 0;
                _ctactMgr = new ContactUsQuestionMgr(mySqlConnectionString);
                _dt = _ctactMgr.GetContactUsQuestionList(cuQuery, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented, timeConverter) + "}";//返回json數據
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
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 回覆記錄
        public HttpResponseBase GetRecordList()
        {
            string json = string.Empty;
            DataTable _dt = new System.Data.DataTable();
            ContactUsResponse cuResponse = new ContactUsResponse();
            cuResponse.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            cuResponse.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量

            string startDate = Request.Params["startDate"] ?? "";
            string endDate = Request.Params["endDate"] ?? "";
            string reply_user = Request.Params["reply_user"] ?? "";

            if (startDate.Equals("1970-01-01"))
            {
                startDate = "";
            }
            else
            {
                startDate = Convert.ToDateTime(startDate).ToString("yyyy-MM-dd 00:00:00");
            }
            if (endDate.Equals("1970-01-01"))
            {
                endDate = "";
            }
            else
            {
                endDate = Convert.ToDateTime(endDate).ToString("yyyy-MM-dd 23:59:59");
            }
            try
            {
                cuResponse.question_id = Convert.ToUInt32(Request.Params["qid"]);
                _ctactrsponseMgr = new ContactUsResponseMgr(mySqlConnectionString);
                int totalCount = 0;
                _dt = _ctactrsponseMgr.GetRecordList(cuResponse, startDate, endDate, reply_user, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented, timeConverter) + "}";//返回json數據
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
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 匯出客服列表信息
        public void GetContactUsQuestionExcelList()
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            DataTable dtHZ = new DataTable();
            ContactUsQuestionQuery cuQuery = new ContactUsQuestionQuery();
            try
            {
                string newExcelName = string.Empty;
                dtHZ.Columns.Add("問題流水號", typeof(String));
                dtHZ.Columns.Add("公司", typeof(String));
                dtHZ.Columns.Add("姓名", typeof(String));
                //dtHZ.Columns.Add("電話", typeof(String));
                // dtHZ.Columns.Add("電子信箱", typeof(String));

                dtHZ.Columns.Add("問題類型", typeof(String));
                dtHZ.Columns.Add("問題分類", typeof(String));
                dtHZ.Columns.Add("狀態", typeof(String));
                dtHZ.Columns.Add("問題內容", typeof(String));
                dtHZ.Columns.Add("回覆內容", typeof(String));

                dtHZ.Columns.Add("建立日期", typeof(String));
                dtHZ.Columns.Add("回覆日期", typeof(String));

                _ctactMgr = new ContactUsQuestionMgr(mySqlConnectionString);
                cuQuery.search_type = Convert.ToInt32(Request.Params["search_type"]);
                cuQuery.searchcontent = Request.Params["searchcontent"];
                //  cuQuery.date_type = Convert.ToInt32(Request.Params["date_type"]);
                if (!string.IsNullOrEmpty(Request.Params["dateStart"]) && Request.Params["dateStart"]!="0")
                {
                    cuQuery.datestart = Convert.ToDateTime(Request.Params["dateStart"]);//建立時間
                }
                if (!string.IsNullOrEmpty(Request.Params["dateEnd"]) && Request.Params["dateEnd"] != "0")
                {
                    cuQuery.dateend = Convert.ToDateTime(Request.Params["dateEnd"]);
                }
                cuQuery.question_type = Convert.ToUInt32(Request.Params["qusetion_type"]);
                if (Convert.ToBoolean(Request.Params["radio2"]) == true)//待回覆
                {
                    cuQuery.question_status = 3;
                }
                else if (Convert.ToBoolean(Request.Params["radio3"]) == true)//已回覆
                {
                    cuQuery.question_status = 4;
                }
                else if (Convert.ToBoolean(Request.Params["radio4"]) == true)
                {
                    cuQuery.question_status = 2;
                }
                _dt = _ctactMgr.GetContactUsQuestionExcelList(cuQuery);
                if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
                }
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = _dt.Rows[i]["question_id"];
                    dr[1] = _dt.Rows[i]["question_company"];
                    dr[2] = _dt.Rows[i]["question_username"];
                    //dr[3] = _dt.Rows[i]["question_phone"];
                    //dr[4] = _dt.Rows[i]["question_email"];

                    dr[3] = _dt.Rows[i]["question_type_name"];
                    dr[4] = _dt.Rows[i]["question_problem_name"];
                    dr[5] = _dt.Rows[i]["question_status_name"];
                    dr[6] = _dt.Rows[i]["question_content"];


                    dr[8] = _dt.Rows[i]["question_createdate"];
                    if (_dt.Rows[i]["question_status"].ToString() == "0")
                    {
                        dr[9] = "~";
                        dr[7] = "";
                    }
                    else
                    {
                        dr[9] = _dt.Rows[i]["response_createdate"];
                        dr[7] = _dt.Rows[i]["response_content"];
                    }
                    dtHZ.Rows.Add(dr);
                }
                if (dtHZ.Rows.Count > 0)
                {
                    string fileName = DateTime.Now.ToString("聯絡客服列表_yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, "聯絡客服列表_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
                else
                {
                    Response.Write("匯出數據不存在");
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,totalCount:0,data:[]}";
            }
        }
        #endregion

        #region 客服列表添加
        public HttpResponseBase Save()
        {
            string jsonStr = String.Empty;
            int result = 0;
            ContactUsQuestion cusTion = new ContactUsQuestion();
            try
            {
                _ctactMgr = new ContactUsQuestionMgr(mySqlConnectionString);
                cusTion.question_id = Convert.ToUInt32(_ctactMgr.GetMaxQuestionId());
                cusTion.question_language = 1;
                if (Convert.ToUInt32(Request.Params["type_id"]) == 0)
                {
                    cusTion.question_type = 1;
                }
                else
                {
                    cusTion.question_type = Convert.ToUInt32(Request.Params["type_id"]);
                }
                cusTion.question_company = Request.Params["company_id"];
                // cusTion.question_username = Request.Params["linkpeople"];
                cusTion.question_email = Request.Params["email_id"];
                cusTion.question_phone = Request.Params["linkphone"];
                cusTion.question_problem = 9;
                cusTion.question_status = 0;
                cusTion.question_content = Request.Params["content"];
                //電話回復方式
                bool phone1 = false;
                bool phone2 = false;
                bool phone3 = false;
                if (!string.IsNullOrEmpty(Request.Params["phone1"]))
                {
                    phone1 = Convert.ToBoolean(Request.Params["phone1"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["phone2"]))
                {
                    phone2 = Convert.ToBoolean(Request.Params["phone2"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["phone3"]))
                {
                    phone3 = Convert.ToBoolean(Request.Params["phone3"]);
                }
                //回複方式
                bool reply1 = false;
                bool reply2 = false;
                bool reply3 = false;
                if (!string.IsNullOrEmpty(Request.Params["reply1"]))
                {
                    reply1 = Convert.ToBoolean(Request.Params["reply1"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["reply2"]))
                {
                    reply2 = Convert.ToBoolean(Request.Params["reply2"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["reply3"]))
                {
                    reply3 = Convert.ToBoolean(Request.Params["reply3"]);
                }
                if (reply1)
                {
                    cusTion.question_reply = cusTion.question_reply + "1|";
                    if (reply2)
                    {
                        cusTion.question_reply = cusTion.question_reply + "1|";
                        if (reply3)
                        {
                            if (phone1)
                            {
                                cusTion.question_reply_time = 1;
                            }
                            else if (phone2)
                            {
                                cusTion.question_reply_time = 2;
                            }
                            else if (phone3)
                            {
                                cusTion.question_reply_time = 3;
                            }
                            else
                            {
                                cusTion.question_reply_time = 4;
                            }
                            cusTion.question_reply = cusTion.question_reply + "1";
                        }
                        else
                        {
                            cusTion.question_reply_time = 0;
                            cusTion.question_reply = cusTion.question_reply + "0";
                        }
                    }
                    else
                    {
                        cusTion.question_reply = cusTion.question_reply + "0|";
                        if (reply3)
                        {
                            if (phone1)
                            {
                                cusTion.question_reply_time = 1;
                            }
                            else if (phone2)
                            {
                                cusTion.question_reply_time = 2;
                            }
                            else if (phone3)
                            {
                                cusTion.question_reply_time = 3;
                            }
                            else
                            {
                                cusTion.question_reply_time = 4;
                            }
                            cusTion.question_reply = cusTion.question_reply + "1";
                        }
                        else
                        {
                            cusTion.question_reply_time = 0;
                            cusTion.question_reply = cusTion.question_reply + "0";
                        }
                    }
                }
                else
                {
                    cusTion.question_reply = cusTion.question_reply + "0|";
                    if (reply2)
                    {
                        cusTion.question_reply = cusTion.question_reply + "1|";
                        if (reply3)
                        {
                            if (phone1)
                            {
                                cusTion.question_reply_time = 1;
                            }
                            else if (phone2)
                            {
                                cusTion.question_reply_time = 2;
                            }
                            else if (phone3)
                            {
                                cusTion.question_reply_time = 3;
                            }
                            else
                            {
                                cusTion.question_reply_time = 4;
                            }
                            cusTion.question_reply = cusTion.question_reply + "1";
                        }
                        else
                        {
                            cusTion.question_reply_time = 0;
                            cusTion.question_reply = cusTion.question_reply + "0";
                        }
                    }
                    else
                    {
                        cusTion.question_reply = cusTion.question_reply + "0|";
                        if (reply3)
                        {//當電話被選擇時在判斷在什麼時間段
                            if (phone1)
                            {
                                cusTion.question_reply_time = 1;
                            }
                            else if (phone2)
                            {
                                cusTion.question_reply_time = 2;
                            }
                            else if (phone3)
                            {
                                cusTion.question_reply_time = 3;
                            }
                            else
                            {
                                cusTion.question_reply_time = 4;
                            }
                            cusTion.question_reply = cusTion.question_reply + "1";
                        }
                        else
                        {
                            cusTion.question_reply_time = 0;
                            cusTion.question_reply = cusTion.question_reply + "0";
                        }
                    }
                }
                //if (!reply1 && !reply2 && !reply3)
                //{
                //    cusTion.question_reply_time = 4;
                //}
                if (!string.IsNullOrEmpty(Request.Params["linkpeople"]))
                {
                    string info = Request.Params["linkpeople"].ToString();
                    if (info.Contains(cusTion.question_email))
                    {
                        cusTion.question_username = info.Substring(0, info.Length - cusTion.question_email.Length - 2);
                    }
                    else
                    {
                        cusTion.question_username = Request.Params["linkpeople"];
                    }
                }
                cusTion.question_createdate = Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                // cusTion.question_ipfrom = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList.FirstOrDefault().ToString();
                cusTion.question_ipfrom = CommonFunction.GetIP4Address(Request.UserHostAddress.ToString());
                result = _ctactMgr.Save(cusTion);
                if (result > 0)
                {
                    jsonStr = "{success:true}";
                }
                else
                {
                    jsonStr = "{success:false}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }
        [CustomHandleError]
        [HttpPost]
        public JsonResult GetUserInfo()
        {
            try
            {
                UsersMgr _userMgr = new UsersMgr(mySqlConnectionString);
                BLL.gigade.Model.Custom.Users userQuery = new BLL.gigade.Model.Custom.Users();
                if (!string.IsNullOrEmpty(Request.Params["name"].ToString()))
                {
                    userQuery.user_name = Request.Params["name"].ToString();

                    List<UserQuery> store = _userMgr.Query(userQuery);

                    return Json(store);
                }
                else
                {
                    return Json("[]");
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json("[]");
            }

        }

        #endregion

        #region 客服列表發送郵件
        public HttpResponseBase update()
        {
            string path = Server.MapPath(xmlPath);
            SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
            SiteConfig Mail_From = _siteConfigMgr.GetConfigByName("Mail_From");
            SiteConfig Mail_Host = _siteConfigMgr.GetConfigByName("Mail_Host");
            SiteConfig Mail_Port = _siteConfigMgr.GetConfigByName("Mail_Port");
            SiteConfig Mail_UserName = _siteConfigMgr.GetConfigByName("Mail_UserName");
            SiteConfig Mail_UserPasswd = _siteConfigMgr.GetConfigByName("Mail_UserPasswd");
            string EmailFrom = Mail_From.Value;//發件人郵箱
            string SmtpHost = Mail_Host.Value;//smtp服务器
            string SmtpPort = Mail_Port.Value;//smtp服务器端口
            string EmailUserName = Mail_UserName.Value;//郵箱登陸名
            string EmailPassWord = Mail_UserPasswd.Value;//郵箱登陸密碼
            string jsonStr = String.Empty;
            int result = 0;
            ContactUsQuestion cusTion = new ContactUsQuestion();
            ContactUsResponse cusponse = new ContactUsResponse();
            SmsQuery smsquery = new SmsQuery();
            try
            {
                _ctactMgr = new ContactUsQuestionMgr(mySqlConnectionString);
                _ctactrsponseMgr = new ContactUsResponseMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["question_id"]))
                {
                    cusTion.question_id = Convert.ToUInt32(Request.Params["question_id"]);
                    smsquery.serial_id = Convert.ToInt32(cusTion.question_id);
                }
                if (Convert.ToBoolean(Request.Params["state1"]) == true)
                {
                    cusTion.question_status = 1;//1表示已經回覆
                }
                else if (Convert.ToBoolean(Request.Params["state2"]) == true)
                {
                    cusTion.question_status = 1;//1表示已經回覆
                }
                else if (Convert.ToBoolean(Request.Params["state3"]) == true)
                {
                    cusTion.question_status = 2;//2表示已經回覆
                }
                string updatesql = _ctactMgr.UpdateSql(cusTion);
                string questiontime = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["question_createdate"]))
                {
                    questiontime = Request.Params["question_createdate"];//提問問題時間 
                }
                if (!string.IsNullOrEmpty(Request.Params["question_phone"]))
                {
                    smsquery.mobile = Request.Params["question_phone"];//提問者電話
                }
                if (!string.IsNullOrEmpty(Request.Params["question_content"]))
                {
                    cusTion.question_content = Request.Params["question_content"];//咨詢問題內容
                }
                if (!string.IsNullOrEmpty(Request.Params["question_username"]))
                {
                    cusTion.question_username = Request.Params["question_username"];//提問問題用戶名
                }
                cusponse.response_id = Convert.ToUInt32(_ctactrsponseMgr.GetMaxResponseId());
                if (!string.IsNullOrEmpty(Request.Params["question_id"]))
                {
                    cusponse.question_id = Convert.ToUInt32(Request.Params["question_id"]);//咨詢問題id
                }
                cusponse.user_id = Convert.ToUInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id);
                if (!string.IsNullOrEmpty(Request.Params["content_reply"]))
                {
                    cusponse.response_content = Request.Params["content_reply"].Replace("\\", "\\\\");
                    smsquery.content = Request.Params["content_reply"].ToString().Replace("\\", "\\\\");//保存sms表的數據

                }
                bool reply1 = false;
                bool reply2 = false;
                bool reply3 = false;
                if (!string.IsNullOrEmpty(Request.Params["reply1"]))
                {
                    reply1 = Convert.ToBoolean(Request.Params["reply1"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["reply2"]))
                {
                    reply2 = Convert.ToBoolean(Request.Params["reply2"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["reply3"]))
                {
                    reply3 = Convert.ToBoolean(Request.Params["reply3"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["question_email"]))
                {
                    cusTion.question_email = Request.Params["question_email"].ToString().Trim();
                }
                cusponse.response_createdate = Convert.ToInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                cusponse.response_ipfrom = CommonFunction.GetIP4Address(Request.UserHostAddress.ToString());
                if (cusTion.question_status != 2)
                {
                    //mh.SendToUser(Convert.ToInt32(cusponse.user_id),"郵箱標題","郵箱內容");//發送郵件
                    if (reply1 || !reply3 && !reply2 && !reply1)
                    {//當email被勾選
                        cusponse.response_type = 1;
                        _ctactrsponseMgr.Insert(updatesql, cusponse);
                        FileStream fs = new FileStream(Server.MapPath("../ImportUserIOExcel/ContactUsQuestionResponse.html"), FileMode.OpenOrCreate, FileAccess.Read);
                        StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                        string strTemp = sr.ReadToEnd();
                        sr.Close();
                        fs.Close();
                        strTemp = strTemp.Replace("{{$s_datetime$}}", questiontime);
                        strTemp = strTemp.Replace("{{$s_username$}}", cusTion.question_username);
                        strTemp = strTemp.Replace("{{$consultInfo$}}", cusTion.question_content);
                        strTemp = strTemp.Replace("{{$consultAnwser$}}", cusponse.response_content.Replace("\\\\", "\\"));
                        sendmail(EmailFrom, FromName, cusTion.question_email, cusTion.question_username, EmailTile, strTemp, "", SmtpHost, Convert.ToInt32(SmtpPort), EmailUserName, EmailPassWord);
                    }
                    else if (reply2)
                    {//當簡訊被勾選
                        cusponse.response_id = Convert.ToUInt32(_ctactrsponseMgr.GetMaxResponseId());
                        smsquery.type = 10;//10表示聯絡客服列表保存過去的
                        smsquery.send = 0;
                        smsquery.trust_send = "8";
                        smsquery.created = DateTime.Now;
                        smsquery.modified = DateTime.Now;
                        cusponse.response_type = 2;
                        _ctactrsponseMgr.Insert(updatesql, cusponse);
                        _ISmsMgr = new SmsMgr(mySqlConnectionString);
                        _ISmsMgr.InsertSms(smsquery);
                    }
                    else if (reply3)
                    {//當電話被勾選
                        cusponse.response_id = Convert.ToUInt32(_ctactrsponseMgr.GetMaxResponseId());
                        cusponse.response_type = 3;
                        _ctactrsponseMgr.Insert(updatesql, cusponse);
                    }
                    //if (!reply3 && !reply2 && !reply1)
                    //{
                    //    cusponse.response_id = Convert.ToUInt32(_ctactrsponseMgr.GetMaxResponseId());
                    //    cusponse.response_type = 4;
                    //    _ctactrsponseMgr.Insert(updatesql, cusponse);
                    //}
                    jsonStr = "{success:true}";
                }
                else//等於2時,已處理,不操作其它
                {
                    cusponse.response_id = 4;
                    result = _ctactrsponseMgr.Insert(updatesql, cusponse);
                    if (result > 0)
                    {
                        jsonStr = "{success:true}";
                    }
                    else
                    {
                        jsonStr = "{success:false}";
                    }
                    DateTime today = DateTime.Now;
                    DateTime a = DateTime.Now.AddDays(-5);


                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 聯絡客服列表更改狀態
        public JsonResult UpdateActive()
        {
            string json = string.Empty;
            ContactUsQuestionQuery query = new ContactUsQuestionQuery();
            _ctactMgr = new ContactUsQuestionMgr(mySqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.question_id = Convert.ToUInt32(Request.Params["id"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["active"]))
                {
                    query.question_status = Convert.ToUInt32(Request.Params["active"]);
                }
                if (query.question_id != 0)
                {
                    string sql = _ctactMgr.UpdateSql(query);
                    _ctactMgr.UpdateActive(sql);
                    return Json(new { success = "true", msg = "" });
                }
                else
                {
                    return Json(new { success = "false", msg = "" });
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json(new { success = "false", msg = "" });
            }
        }
        #endregion

        #region 匯出訂單問題列表
        public void OrderQuestionExcel()
        {

            OrderQuestionQuery query = new OrderQuestionQuery();
            _IOrderQuesMgr = new OrderQuestionMgr(mySqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["ddlSel"]))
                {
                    query.ddlSel = Convert.ToInt32(Request.Params["ddlSel"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["selcontent"]))
                {
                    query.selcontent = Request.Params["selcontent"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["ddtSel"]))
                {
                    query.ddtSel = Convert.ToInt32(Request.Params["ddtSel"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["time_start"]) && Request.Params["time_start"] != "null")
                {
                    query.question_createdates = Convert.ToDateTime(Request.Params["time_start"]);
                    query.time_start = Convert.ToUInt32(CommonFunction.GetPHPTime(query.question_createdates.ToString()));
                }
                if (!string.IsNullOrEmpty(Request.Params["time_end"]) && Request.Params["time_end"] != "null")
                {
                    query.response_createdates = Convert.ToDateTime(Request.Params["time_end"]);
                    query.time_end = Convert.ToUInt32(CommonFunction.GetPHPTime(query.response_createdates.ToString()));
                }
                if (!string.IsNullOrEmpty(Request.Params["ddrSel"]))
                {
                    query.question_type = Convert.ToUInt32(Request.Params["ddrSel"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ddlstatus"]))
                {
                    query.ddlstatus = Request.Params["ddlstatus"];
                }
                DataTable dtMain = _IOrderQuesMgr.GetOrderQuestionExcel(query);
                DataTable new_Table = new DataTable();
                #region new_Table表頭
                new_Table.Columns.Add(new DataColumn("問題流水號"));
                new_Table.Columns.Add(new DataColumn("付款單號"));
                new_Table.Columns.Add(new DataColumn("姓名"));
                //new_Table.Columns.Add(new DataColumn("聯絡電話"));
                //new_Table.Columns.Add(new DataColumn("電子信箱"));
                new_Table.Columns.Add(new DataColumn("問題分類"));
                new_Table.Columns.Add(new DataColumn("狀態"));
                new_Table.Columns.Add(new DataColumn("問題內容"));
                new_Table.Columns.Add(new DataColumn("回覆內容"));
                new_Table.Columns.Add(new DataColumn("建立日期"));
                new_Table.Columns.Add(new DataColumn("回覆日期"));
                #endregion
                for (int i = 0; i < dtMain.Rows.Count; i++)
                {
                    DataRow dr1 = new_Table.NewRow();
                    dr1["問題流水號"] = dtMain.Rows[i]["question_id"].ToString();
                    dr1["付款單號"] = dtMain.Rows[i]["order_id"].ToString();
                    dr1["姓名"] = dtMain.Rows[i]["question_username"].ToString();
                    //dr1["聯絡電話"] = dtMain.Rows[i]["question_phone"].ToString();
                    //dr1["電子信箱"] = dtMain.Rows[i]["question_email"].ToString();
                    dr1["問題分類"] = dtMain.Rows[i]["question_type_name"].ToString();
                    dr1["狀態"] = dtMain.Rows[i]["question_status_name"].ToString();
                    dr1["問題內容"] = dtMain.Rows[i]["question_content"].ToString();

                    //CommonFunction.GetNetTime(item.question_createdate);
                    dr1["建立日期"] = CommonFunction.GetNetTime(uint.Parse(dtMain.Rows[i]["question_createdate"].ToString()));
                    if (dtMain.Rows[i]["question_status"].ToString() == "0")
                    {
                        dr1["回覆日期"] = "~";
                        dr1["回覆內容"] = "";
                    }
                    else
                    {
                        dr1["回覆內容"] = dtMain.Rows[i]["response_content"].ToString();
                        if (!string.IsNullOrEmpty(dtMain.Rows[i]["response_createdate"].ToString()))
                        {
                            dr1["回覆日期"] = CommonFunction.GetNetTime(uint.Parse(dtMain.Rows[i]["response_createdate"].ToString()));
                        }
                        else
                        {
                            dr1["回覆日期"] = "~";
                        }
                    }
                    new_Table.Rows.Add(dr1);
                }
                if (new_Table.Rows.Count > 0)
                {
                    string fileName = "訂單問題" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(new_Table, "訂單問題" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
                else
                {
                    Response.Write("匯出數據不存在");
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
        }
        #endregion

        #region 獲取參數
        public HttpResponseBase GetDDL()
        {
            List<Parametersrc> stores = new List<Parametersrc>();
            _IOrderQuesMgr = new OrderQuestionMgr(mySqlConnectionString);
            string json = string.Empty;
            try
            {
                int totalCount = 0;
                stores = _IOrderQuesMgr.GetDDL();
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores) + "}";//返回json數據

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 回復訂單問題保存
        public HttpResponseBase ReplyQuestion()
        {
            string timestr = string.Empty;
            string path = Server.MapPath(xmlPath);
            SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
            SiteConfig Mail_From = _siteConfigMgr.GetConfigByName("Mail_From");
            SiteConfig Mail_Host = _siteConfigMgr.GetConfigByName("Mail_Host");
            SiteConfig Mail_Port = _siteConfigMgr.GetConfigByName("Mail_Port");
            SiteConfig Mail_UserName = _siteConfigMgr.GetConfigByName("Mail_UserName");
            SiteConfig Mail_UserPasswd = _siteConfigMgr.GetConfigByName("Mail_UserPasswd");
            string EmailFrom = Mail_From.Value;//發件人郵箱
            string SmtpHost = Mail_Host.Value;//smtp服务器
            string SmtpPort = Mail_Port.Value;//smtp服务器端口
            string EmailUserName = Mail_UserName.Value;//郵箱登陸名
            string EmailPassWord = Mail_UserPasswd.Value;//郵箱登陸密碼
            string json = string.Empty;
            _IOrderQuesMgr = new OrderQuestionMgr(mySqlConnectionString);
            _IOrderResponseMgr = new OrderResponseMgr(mySqlConnectionString);
            OrderQuestionQuery query = new OrderQuestionQuery();
            OrderResponse ormodel = new OrderResponse();
            SmsQuery smsquery = new SmsQuery();

            //if (!string.IsNullOrEmpty(Request.Params["question_content"]))
            //{
            //    query.question_content = Request.Params["question_content"];

            //}

            //string order_id = Request.Params["OrderId"].ToString();
            //if (!string.IsNullOrEmpty(order_id))
            //{
            //    query.order_id = Convert.ToUInt32(order_id);
            //}
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["question_id"]))
                {
                    query.question_id = Convert.ToUInt32(Request.Params["question_id"]);
                    ormodel.question_id = query.question_id;

                }
                if (!string.IsNullOrEmpty(Request.Params["order_id"]))
                {
                    smsquery.order_id = Convert.ToInt32(Request.Params["order_id"]);
                }
                //if (!string.IsNullOrEmpty(Request.Params["status"]))
                //{
                //    query.question_status = Convert.ToUInt32(Request.Params["status"]);
                //}
                if (!string.IsNullOrEmpty(Request.Params["response_content"]))
                {
                    ormodel.response_content = Request.Params["response_content"].Replace("\\", "\\\\"); ;
                    smsquery.content = ormodel.response_content.Replace("\\", "\\\\"); ;
                }
                if (!string.IsNullOrEmpty(Request.Params["question_createdate"]))
                {
                    timestr = Request.Params["question_createdate"];
                }
                if (!string.IsNullOrEmpty(Request.Params["question_username"]))
                {
                    query.question_username = Request.Params["question_username"];
                }
                if (!string.IsNullOrEmpty(Request.Params["this_question_email"]))
                {
                    query.question_email = Request.Params["this_question_email"];

                }
                if (!string.IsNullOrEmpty(Request.Params["question_phone"]))
                {
                    smsquery.mobile = Request.Params["question_phone"].ToString();
                }

                //回複方式
                bool reply1 = false;
                bool reply2 = false;
                bool reply3 = false;
                if (!string.IsNullOrEmpty(Request.Params["reply_mail"]))
                {
                    reply1 = Convert.ToBoolean(Request.Params["reply_mail"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["reply_sms"]))
                {
                    reply2 = Convert.ToBoolean(Request.Params["reply_sms"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["reply_phone"]))
                {
                    reply3 = Convert.ToBoolean(Request.Params["reply_phone"]);
                }
                //如果選擇結案就更新問題狀態
                //更新order_question表狀態
                query.question_status = 1;
                _IOrderQuesMgr.UpdateQuestionStatus(query);
                //獲取數據往回覆表添加數據
                ormodel.user_id = uint.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                ormodel.response_ipfrom = CommonFunction.GetIP4Address(Request.UserHostAddress.ToString()); ;
                ormodel.response_createdate = Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                //判斷選擇的回複方式
                if (reply1 || !reply1 && !reply2 && !reply3)
                {
                    ormodel.response_type = 1;
                    _IOrderResponseMgr.insert(ormodel);
                    FileStream fs = new FileStream(Server.MapPath("../ImportUserIOExcel/OrderQuestionResponse.html"), FileMode.OpenOrCreate, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                    string strTemp = sr.ReadToEnd();
                    sr.Close();
                    fs.Close();
                    strTemp = strTemp.Replace("{{$s_datetime$}}", timestr);//添加提問時間
                    strTemp = strTemp.Replace("{{$s_username$}}", query.question_username);//添加提問用戶名
                    strTemp = strTemp.Replace("{{$consultInfo$}}", query.question_content);//添加提問內容
                    strTemp = strTemp.Replace("{{$consultAnwser$}}", ormodel.response_content.Replace("\\\\", "\\"));
                    sendmail(EmailFrom, FromName, query.question_email, query.question_username, EmailTile, strTemp, "", SmtpHost, Convert.ToInt32(SmtpPort), EmailUserName, EmailPassWord);
                }
                else if (reply2)
                {
                    ormodel.response_type = 2;
                    _IOrderResponseMgr.insert(ormodel);
                    smsquery.type = 11;//10表示聯絡客服列表保存過去的
                    smsquery.send = 0;
                    smsquery.trust_send = "9";
                    smsquery.created = DateTime.Now;
                    smsquery.modified = DateTime.Now;
                    _ISmsMgr = new SmsMgr(mySqlConnectionString);
                    _ISmsMgr.InsertSms(smsquery);
                }
                else if (reply3)
                {
                    ormodel.response_type = 3;
                    _IOrderResponseMgr.insert(ormodel);
                }
                //if (!reply1 && !reply2 && !reply3)
                //{
                //    ormodel.response_type = 4;
                //    _IOrderResponseMgr.insert(ormodel);
                //}
                json = "{success:true}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:0}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region C#发送邮件函数
        /// <summary>
        /// C#发送邮件函数
        /// </summary>
        /// <param name="from">发送者邮箱</param>
        /// <param name="fromer">发送人</param>
        /// <param name="sto">接受者邮箱</param>
        /// <param name="toer">收件人</param>
        /// <param name="Subject">主题</param>
        /// <param name="Body">内容</param>
        /// <param name="file">附件地址</param>
        /// <param name="SMTPHost">smtp服务器</param>
        /// <param name="SMTPuser">邮箱</param>
        /// <param name="SMTPpass">密码</param>
        /// <returns></returns>
        public bool sendmail(string sfrom, string sfromer, string sto, string stoer, string sSubject, string sBody, string sfile, string sSMTPHost, int sSMTPPort, string sSMTPuser, string sSMTPpass)
        {
            ////设置from和to地址
            MailAddress from = new MailAddress(sfrom, sfromer);
            MailAddress to = new MailAddress(sto, stoer);

            ////创建一个MailMessage对象
            MailMessage oMail = new MailMessage(from, to);
            //// 添加附件
            if (sfile != "")
            {
                oMail.Attachments.Add(new Attachment(sfile));
            }
            ////邮件标题
            oMail.Subject = sSubject;
            ////邮件内容
            //oMail.Body = sBody;
            AlternateView htmlBody = AlternateView.CreateAlternateViewFromString(sBody, null, "text/html");
            oMail.AlternateViews.Add(htmlBody);
            ////邮件格式
            oMail.IsBodyHtml = true;
            ////邮件采用的编码
            oMail.BodyEncoding = System.Text.Encoding.GetEncoding("GB2312");
            ////设置邮件的优先级为高
            oMail.Priority = MailPriority.Normal;
            ////发送邮件
            SmtpClient client = new SmtpClient();
            ////client.UseDefaultCredentials = false;
            client.Host = sSMTPHost;
            client.Port = sSMTPPort;
            client.Credentials = new NetworkCredential(sSMTPuser, sSMTPpass);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            try
            {
                client.Send(oMail);
                return true;
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return false;
            }
            finally
            {
                ////释放资源
                oMail.Dispose();
            }

        }
        #endregion

        #region 新增訂單問題

        #region 問題類型store
        public HttpResponseBase GetQuestionTypeList()
        {
            IParametersrcImplMgr parametersrcMgr = new BLL.gigade.Mgr.ParameterMgr(mySqlConnectionString);
            List<Parametersrc> parametersrclist = new List<Parametersrc>();
            string json = string.Empty;
            try
            {
                string type = "problem_category";
                parametersrclist = parametersrcMgr.GetElementType(type);
                json = "{success:true,data:" + JsonConvert.SerializeObject(parametersrclist) + "}";//返回json數據
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

        #region 新增訂單問題保存
        public HttpResponseBase OrderQuestionSave()
        {
            OrderQuestion query = new OrderQuestion();
            _IOrderQuesMgr = new OrderQuestionMgr(mySqlConnectionString);
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["orderid"]))
                {
                    query.order_id = uint.Parse(Request.Params["orderid"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["questiontype"]))
                {
                    query.question_type = uint.Parse(Request.Params["questiontype"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["name"]))
                {
                    query.question_username = Request.Params["name"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["email_id"]))
                {
                    query.question_email = Request.Params["email_id"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["linkphone"]))
                {
                    query.question_phone = Request.Params["linkphone"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["content"]))
                {
                    query.question_content = Request.Params["content"].ToString();
                }
                //回復方式
                bool reply1 = false;
                bool reply2 = false;
                bool reply3 = false;
                if (!string.IsNullOrEmpty(Request.Params["reply1"]))
                {
                    reply1 = Convert.ToBoolean(Request.Params["reply1"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["reply2"]))
                {
                    reply2 = Convert.ToBoolean(Request.Params["reply2"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["reply3"]))
                {
                    reply3 = Convert.ToBoolean(Request.Params["reply3"]);
                }
                if (reply3)
                {
                    if (!string.IsNullOrEmpty(Request.Params["ddlstatus"]))
                    {
                        query.question_reply_time = int.Parse(Request.Params["ddlstatus"]);
                    }
                    else
                    {
                        query.question_reply_time = 4;
                    }
                }
                else
                {
                    query.question_reply_time = 0;
                }
                if (reply1)
                {
                    query.question_reply = query.question_reply + "1|";
                    if (reply2)
                    {
                        query.question_reply = query.question_reply + "1|";
                        if (reply3)
                        {
                            query.question_reply = query.question_reply + "1";
                        }
                        else
                        {
                            query.question_reply = query.question_reply + "0";
                        }
                    }
                    else
                    {
                        query.question_reply = query.question_reply + "0|";
                        if (reply3)
                        {
                            query.question_reply = query.question_reply + "1";
                        }
                        else
                        {
                            query.question_reply = query.question_reply + "0";
                        }
                    }
                }
                else
                {
                    query.question_reply = query.question_reply + "0|";
                    if (reply2)
                    {
                        query.question_reply = query.question_reply + "1|";
                        if (reply3)
                        {
                            query.question_reply = query.question_reply + "1";
                        }
                        else
                        {
                            query.question_reply = query.question_reply + "0";
                        }
                    }
                    else
                    {
                        query.question_reply = query.question_reply + "0|";
                        if (reply3)
                        {//當電話被選擇時在判斷在什麼時間段
                            query.question_reply = query.question_reply + "1";
                        }
                        else
                        {
                            query.question_reply = query.question_reply + "0";
                        }
                    }
                }
                //System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
                //query.question_ipfrom = addlist[0].ToString();
                query.question_ipfrom = CommonFunction.GetIP4Address(Request.UserHostAddress.ToString());
                query.question_status = 0;
                DateTime createdate = DateTime.Now;
                query.question_createdate = uint.Parse(CommonFunction.GetPHPTime(createdate.ToString()).ToString());
                _IOrderQuesMgr.InsertOrderQuestion(query);
                json = "{success:true}";//返回json數據
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

        #endregion
    }
}
