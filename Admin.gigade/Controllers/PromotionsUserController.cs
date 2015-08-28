#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsUserController.cs
* 摘 要：
* 會員條件設定controller
* 当前版本：v1.1
* 作 者： mengjuan0826j
* 完成日期：2014/6/20 
* 修改歷史：
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：mengjuan0826j 
*         v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋 
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Specialized;
using BLL.gigade.Model;
using BLL.gigade.Common;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Mgr;
using Newtonsoft.Json;
using Admin.gigade.CustomError;
using Newtonsoft.Json.Converters;

namespace Admin.gigade.Controllers
{
    public class PromotionsUserController : Controller
    {
        //
        // GET: /PromotionsUser/

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string MySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IUserConditionImplMgr _ucMgr = new UserConditionMgr(MySqlConnectionString);
        public ActionResult Index()
        { 
            return View();
        }
        #region 獲取數據 會員條件設定新增或編輯 HttpResponseBase SavePromotionsUser()
        /// <summary>
        ///獲取數據  會員條件設定新增或編輯
        /// </summary>
        /// <returns>跳轉到執行方法</returns>
        public HttpResponseBase SavePromotionsUser()
        {
            NameValueCollection param = Request.Params;
            UserCondition uc = new UserCondition();
            uc.condition_name = param["condition_name"];
            if (param["condition_id"] == "0" || param["condition_id"] == "")
            {
                if (!string.IsNullOrEmpty(Request.Params["reg_start"]))
                {
                    uc.reg_start = Convert.ToInt32(CommonFunction.GetPHPTime(param["reg_start"].ToString()));
                }
                else
                {
                    uc.reg_start = Convert.ToInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                }
                if (!string.IsNullOrEmpty(Request.Params["reg_end"]))
                {
                    uc.reg_end = Convert.ToInt32(CommonFunction.GetPHPTime(param["reg_end"].ToString()));
                }
                else
                {
                    uc.reg_end = Convert.ToInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                }
                if (!string.IsNullOrEmpty(Request.Params["reg_interval"]))
                {
                    uc.reg_interval = Convert.ToInt32(param["reg_interval"]);
                }
                else
                {
                    uc.reg_interval = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["buy_times_min"]))
                {
                    uc.buy_times_min = Convert.ToInt32(param["buy_times_min"]);
                }
                else
                {
                    uc.buy_times_min = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["buy_times_max"]))
                {
                    uc.buy_times_max = Convert.ToInt32(param["buy_times_max"]);
                }
                else
                {
                    uc.buy_times_max = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["buy_amount_min"]))
                {
                    uc.buy_amount_min = Convert.ToInt32(param["buy_amount_min"]);
                }
                else
                {
                    uc.buy_amount_min = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["buy_amount_max"]))
                {
                    uc.buy_amount_max = Convert.ToInt32(param["buy_amount_max"]);
                }
                else
                {
                    uc.buy_amount_max = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["last_time_start"]))
                {
                    uc.last_time_start = Convert.ToInt32(CommonFunction.GetPHPTime(param["last_time_start"].ToString()));
                }
                else
                {
                    uc.last_time_start = Convert.ToInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                }
                if (!string.IsNullOrEmpty(Request.Params["last_time_end"]))
                {
                    uc.last_time_end = Convert.ToInt32(CommonFunction.GetPHPTime(param["last_time_end"].ToString()));
                }
                else
                {
                    uc.last_time_end = Convert.ToInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                }
                if (!string.IsNullOrEmpty(Request.Params["last_time_interval"]))
                {
                    uc.last_time_interval = Convert.ToInt32(param["last_time_interval"]);
                }
                else
                {
                    uc.last_time_interval = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["join_channel"]))
                {
                    uc.join_channel = Convert.ToInt32(param["join_channel"]);
                }
                else
                {
                    uc.join_channel = 0;
                }
                uc.status = 1;
                return AddPromotionsUser(uc);
            }
            else
            {
                uc.condition_id = Convert.ToInt32(param["condition_id"].ToString());
                UserCondition OldUc = _ucMgr.Select(uc);
                if (!string.IsNullOrEmpty(Request.Params["reg_start"]))
                {
                    uc.reg_start = Convert.ToInt32(CommonFunction.GetPHPTime(param["reg_start"].ToString()));
                }
                else
                {
                    uc.reg_start = OldUc.reg_start;
                }
                if (!string.IsNullOrEmpty(Request.Params["reg_end"]))
                {
                    uc.reg_end = Convert.ToInt32(CommonFunction.GetPHPTime(param["reg_end"].ToString()));
                }
                else
                {
                    uc.reg_end = OldUc.reg_end;
                }
                if (!string.IsNullOrEmpty(Request.Params["reg_interval"]))
                {
                    uc.reg_interval = Convert.ToInt32(param["reg_interval"]);
                }
                else
                {
                    uc.reg_interval = OldUc.reg_interval;
                }
                if (!string.IsNullOrEmpty(Request.Params["buy_times_min"]))
                {
                    uc.buy_times_min = Convert.ToInt32(param["buy_times_min"]);
                }
                else
                {
                    uc.buy_times_min = OldUc.buy_times_min;
                }
                if (!string.IsNullOrEmpty(Request.Params["buy_times_max"]))
                {
                    uc.buy_times_max = Convert.ToInt32(param["buy_times_max"]);
                }
                else
                {
                    uc.buy_times_max = OldUc.buy_times_max;
                }
                if (!string.IsNullOrEmpty(Request.Params["buy_amount_min"]))
                {
                    uc.buy_amount_min = Convert.ToInt32(param["buy_amount_min"]);
                }
                else
                {
                    uc.buy_amount_min = OldUc.buy_amount_min;
                }
                if (!string.IsNullOrEmpty(Request.Params["buy_amount_max"]))
                {
                    uc.buy_amount_max = Convert.ToInt32(param["buy_amount_max"]);
                }
                else
                {
                    uc.buy_amount_max = OldUc.buy_amount_max;
                }
                if (!string.IsNullOrEmpty(Request.Params["last_time_start"]))
                {
                    uc.last_time_start = Convert.ToInt32(CommonFunction.GetPHPTime(param["last_time_start"].ToString()));
                }
                else
                {
                    uc.last_time_start = OldUc.last_time_start;
                }
                if (!string.IsNullOrEmpty(Request.Params["last_time_end"]))
                {
                    uc.last_time_end = Convert.ToInt32(CommonFunction.GetPHPTime(param["last_time_end"].ToString()));
                }
                else
                {
                    uc.last_time_end = OldUc.last_time_end;
                }
                if (!string.IsNullOrEmpty(Request.Params["last_time_interval"]))
                {
                    uc.last_time_interval = Convert.ToInt32(param["last_time_interval"]);
                }
                else
                {
                    uc.last_time_interval = OldUc.last_time_interval;
                }
                if (!string.IsNullOrEmpty(Request.Params["join_channel"].ToString()))
                {
                    int channelId = 0;
                    if (int.TryParse(Request.Params["join_channel"].ToString(), out channelId))
                    {
                        uc.join_channel = Convert.ToInt32(param["join_channel"]);
                    }
                    else
                    {
                        uc.join_channel = OldUc.join_channel;
                    }
                }
                else
                {
                    uc.join_channel = OldUc.join_channel;
                }
                uc.status = 1;
                return UpdatePromotionsUser(uc);
            }

        }
        #endregion

        #region  會員條件設定新增 HttpResponseBase AddPromotionsUser(UserCondition uc)
        /// <summary>
        /// 會員條件設定新增
        /// </summary>
        /// <param name="uc">UserCondition uc對象</param>
        /// <returns>執行結果 json數組獲取condition_id</returns>
        public HttpResponseBase AddPromotionsUser(UserCondition uc)
        {
            string json = string.Empty;
            try
            {
                System.Data.DataTable query = _ucMgr.Add(uc);
                if (query != null)
                {
                    json = "{success:true,id:" + query.Rows[0]["condition_id"] + ",data:" + JsonConvert.SerializeObject(query) + "}";
                }
                else
                {
                    json = "{success:false}";
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
        #endregion

        #region  會員條件設定編輯 HttpResponseBase UpdatePromotionsUser( UserCondition uc )
        /// <summary>
        /// 會員條件設定編輯
        /// </summary>
        /// <param name="uc">UserCondition uc對象</param>
        /// <returns>執行結果</returns>
        public HttpResponseBase UpdatePromotionsUser(UserCondition uc)
        {
            string json = string.Empty;
            try
            {
                if (_ucMgr.Update(uc) > 0)
                {
                    json = "{success:true}";
                }
                else
                {
                    json = "{success:false}";
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
        #endregion

        #region 獲取condition_id 對應的數據 + HttpResponseBase UserConInfo()
        /// <summary>
        /// 獲取會員條件設定對應的值
        /// </summary>
        /// <returns>json數組數據</returns>
        [CustomHandleError]
        public HttpResponseBase UserConInfo()
        {
            UserCondition uc = new UserCondition();
            string json = string.Empty;

            if (Request.Params["condition_id"] != "" && Request.Params["condition_id"] != "0")
            {
                if (!string.IsNullOrEmpty(Request.Params["condition_id"]))
                {
                    uc.condition_id = Convert.ToInt32(Request.Params["condition_id"]);
                }
                else
                {
                    uc.condition_id = 0;
                }
                UserCondition ucResult = new UserCondition();
                ucResult = _ucMgr.GetModelById(uc);

                ucResult.reg_startDateTime = CommonFunction.GetNetTime(ucResult.reg_start);
                ucResult.reg_endDateTime = CommonFunction.GetNetTime(ucResult.reg_end);
                ucResult.last_time_startDateTime = CommonFunction.GetNetTime(ucResult.last_time_start);
                ucResult.last_time_endDateTime = CommonFunction.GetNetTime(ucResult.last_time_end);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                //listUser是准备转换的对象
                //json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據
                try
                {

                    if (ucResult != null)
                    {

                        json = "{success:true,data:[" + JsonConvert.SerializeObject(ucResult, Formatting.Indented, timeConverter) + "]}";
                    }
                    else
                    {

                        json = "{success:false}";

                    }
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
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

    }
}
