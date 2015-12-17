using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Mgr;
using System.Configuration;
using Newtonsoft.Json;
using BLL.gigade.Model;
using System.Text;
using Newtonsoft.Json.Converters;
namespace Admin.gigade.Controllers
{
    [HandleError]
    public class SystemController : Controller
    {
        // 
        // GET: /System/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        private IParametersrcImplMgr _Iparametersrc;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ErrorLog()
        {
            return View();
        }

        public ActionResult MailCfg()
        {
            return View();
        }
        public ActionResult ParametersrcIndex()
        {
            return View();
        }

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase QueryErrorLog()
        {
            string jsonStr = "{success:false}";

            string startDate = Request.Form["startDate"] ?? "";
            string endDate = Request.Form["endDate"] ?? "";
            //添加 級別 的查詢條件  add by zhuoqin0830w 2015/02/04
            string level = Request.Form["level"] ?? "";

            if (startDate.Equals("1970-01-01 08:00:00"))
            {
                startDate = "";
            }

            if (endDate.Equals("1970-01-01 08:00:00"))
            {
                endDate = "";
            }
            int startPage = Convert.ToInt32(Request.Form["start"] ?? "0");
            int endPage = Convert.ToInt32(Request.Form["limit"] ?? "20");

            int totalCount = 0;
            try
            {
                IErrorLogImplMgr _errorMgr = new ErrorLogMgr(connectionString);
                List<ErrorLog> errorList = new List<ErrorLog>();
                //添加 級別 的查詢條件  edit by zhuoqin0830w 2015/02/05
                errorList = _errorMgr.QueryErrorLog(startDate, endDate, startPage, endPage, out totalCount, level);
                jsonStr = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(errorList) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }

        /// <summary>
        /// 通知設定查詢
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase mailSetQuery()
        {
            string result = "{success:false}";

            try
            {
                if (!string.IsNullOrEmpty(Request.Params["paraType"]))
                {
                    string paratype = Request.Params["paraType"];
                    StringBuilder stb = new StringBuilder();
                    string tempStr = string.Empty;
                    IParametersrcImplMgr _paraMgr = new BLL.gigade.Mgr.ParameterMgr(connectionString);
                    List<Parametersrc> paraList = _paraMgr.QueryUsed(new Parametersrc { ParameterType = paratype });

                    if (paraList.Count > 2)//為了防止就版本2014/05/23 之前 沒有Sendtime 參數 edit by hufeng0813w 2014/05/23
                    {
                        Parametersrc paraSwitch = paraList.Where(m => m.parameterName == "switch").FirstOrDefault();
                        Parametersrc paraSendTo = paraList.Where(m => m.parameterName == "sendTo").FirstOrDefault();
                        Parametersrc paraSendTime = paraList.Where(m => m.parameterName == "sendTime").FirstOrDefault();

                        switch (paraList[0].ParameterType)
                        {
                            case "warn_stock":
                            case "warn_product":
                            case "warn_productMap": stb.AppendFormat("switchId:'{0}',switch:'{1}',sendToId:'{2}',sendTo:'{3}',sendTimeId:'{4}',sendTime:'{5}'", paraSwitch.Rowid, paraSwitch.ParameterCode, paraSendTo.Rowid, paraSendTo.ParameterCode.Replace("\n", "\\n"), paraSendTime.Rowid, paraSendTime.ParameterCode.Replace("\n", "\\n")); break;
                            default:
                                break;
                        }
                        tempStr = "{" + stb.ToString() + "}";

                    }
                    else
                    {
                        tempStr = "null";
                    }
                    result = "{success:true,data:" + tempStr + "}";
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }


            this.Response.Clear();
            this.Response.Write(result);
            this.Response.End();
            return this.Response;
        }

        /// <summary>
        /// 通知設定保存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase mailSetSave()
        {
            string result = "{success:false}";

            try
            {
                if (!string.IsNullOrEmpty(Request.Params["jsonSave"]))
                {
                    string json = Request.Params["jsonSave"];
                    System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                    List<Parametersrc> paraList = js.Deserialize<List<Parametersrc>>(json);
                    IParametersrcImplMgr _paraMgr = new BLL.gigade.Mgr.ParameterMgr(connectionString);

                    if (paraList.Count() > 0)
                    {
                        if (paraList[0].Rowid != 0)         //更新
                        {
                            List<Parametersrc> updateList = new List<Parametersrc>();
                            paraList.ForEach(m =>
                            {
                                Parametersrc query = _paraMgr.QueryUsed(new Parametersrc { Rowid = m.Rowid }).FirstOrDefault();
                                query.ParameterCode = m.ParameterCode;
                                updateList.Add(query);
                            });
                            if (_paraMgr.Update(updateList))
                            {
                                result = "{success:true}";
                            }
                        }
                        else                                //保存
                        {
                            string c_kuser = (Session["caller"] as Caller).user_email;
                            paraList.ForEach(m => m.Kuser = c_kuser);
                            if (_paraMgr.Save(paraList))
                            {
                                result = "{success:true}";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }


            this.Response.Clear();
            this.Response.Write(result);
            this.Response.End();
            return this.Response;
        }

        #region 參數表新增修改+HttpResponseBase ParametersrcSave()
        /// <summary>
        /// 參數表新增修改
        /// 2014/10/20號zhejiangj新增
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase ParametersrcSave()
        {
            string result = "{success:false}";
            try
            {
                Parametersrc para = new Parametersrc();
                _Iparametersrc = new ParameterMgr(connectionString);
                if (string.IsNullOrEmpty(Request.Params["rowid"]))
                {
                    if (!string.IsNullOrEmpty(Request.Params["parameterType"]))
                    {
                        para.ParameterType = Request.Params["parameterType"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Params["parameterProperty"]))
                    {
                        para.ParameterProperty = Request.Params["parameterProperty"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Params["Code"]))
                    {
                        para.ParameterCode = Request.Params["Code"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Params["parameterName"]))
                    {
                        para.parameterName = Request.Params["parameterName"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Params["Explain"]))
                    {
                        para.remark = Request.Params["Explain"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Params["sort"]))
                    {
                        para.Sort = Convert.ToInt32(Request.Params["sort"].ToString());
                    }
                    if (!string.IsNullOrEmpty(Request.Params["topValue"]))
                    {
                        para.TopValue = Request.Params["topValue"].ToString();
                    }
                    para.Kdate = DateTime.Now;
                    para.Kuser = (Session["caller"] as Caller).user_email.ToString();
                    para.Used = 1;
                    int i = _Iparametersrc.ParametersrcSave(para);
                    if (i > 0)
                    {
                        result = "{success:true}";
                    }
                }
                else
                {
                    para.Rowid = Convert.ToInt32(Request.Params["rowid"]);
                    if (!string.IsNullOrEmpty(Request.Params["parameterType"]))
                    {
                        para.ParameterType = Request.Params["parameterType"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Params["parameterProperty"]))
                    {
                        para.ParameterProperty = Request.Params["parameterProperty"].ToString();
                    }

                    if (!string.IsNullOrEmpty(Request.Params["Code"]))
                    {
                        para.ParameterCode = Request.Params["Code"].ToString();
                    }

                    if (!string.IsNullOrEmpty(Request.Params["parameterName"]))
                    {
                        para.parameterName = Request.Params["parameterName"].ToString();
                    }

                    if (!string.IsNullOrEmpty(Request.Params["Explain"]))
                    {
                        para.remark = Request.Params["Explain"].ToString();
                    }

                    if (!string.IsNullOrEmpty(Request.Params["sort"]))
                    {
                        para.Sort = Convert.ToInt32(Request.Params["sort"].ToString());
                    }

                    if (!string.IsNullOrEmpty(Request.Params["topValue"]))
                    {
                        para.TopValue = Request.Params["topValue"].ToString();
                    }

                    para.Kdate = DateTime.Now;
                    para.Kuser = (Session["caller"] as Caller).user_email.ToString();
                    para.Used = 1;
                    int i = _Iparametersrc.ParametersrcSave(para);
                    if (i > 0)
                    {
                        result = "{success:true}";
                    }
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }


            this.Response.Clear();
            this.Response.Write(result);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 參數表列表頁顯示+HttpResponseBase ParametersrcList()
        public HttpResponseBase ParametersrcList()
        {
            List<Parametersrc> stores = new List<Parametersrc>();

            string json = string.Empty;
            try
            {
                Parametersrc query = new Parametersrc();
                if (!string.IsNullOrEmpty(Request.Params["serchcontent"]))
                {

                    query.ParameterType = Request.Params["serchcontent"];
                }
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量

                _Iparametersrc = new ParameterMgr(connectionString);
                int totalCount = 0;
                stores = _Iparametersrc.GetParametersrcList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";
                //返回json數據

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

        #region  更改狀態 + UpdateUsed
        public JsonResult UpdateUsed()
        {
            string json = string.Empty;
            try
            {
                Parametersrc query = new Parametersrc();
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.Rowid = Convert.ToInt32(Request.Params["id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["active"]))
                {
                    query.Used = Convert.ToInt32(Request.Params["active"]);
                }

                _Iparametersrc = new ParameterMgr(connectionString);
                if (_Iparametersrc.UpdateUsed(query) > 0)
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

        #region 獲取 ErrorLog 中所有的 Level  + GetLevel()  add by zhuoqin0830w 2015/02/04
        /// <summary>
        /// 獲取 ErrorLog 中所有的 Level
        /// </summary>
        /// <returns></returns>
        public JsonResult GetLevel()
        {
            JsonResult json = null;
            try
            {
                IErrorLogImplMgr _errorMgr = new ErrorLogMgr(connectionString);
                List<ErrorLog> errorList = new List<ErrorLog>();
                errorList = _errorMgr.GetLevel();
                var result = from f in errorList select new { Level = f.Level };
                if (result == null) { json = Json("[]"); }
                else { json = Json(result); }
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
        #endregion
    }
}
