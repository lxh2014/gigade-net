#region 文件信息 v1.1
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsDeductRateController.cs
* 摘 要：
* 點數抵用與資料庫交互方法  
* 当前版本：v1.1
* 作 者：dongya0410j    
* 完成日期：2014/6/20 
* 修改歷史:
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：zhejiang0304j 
*         v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋 
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Model.Query;
using BLL.gigade.Mgr;
using Newtonsoft.Json.Converters;
using BLL.gigade.Mgr.Impl;
using Newtonsoft.Json;
using BLL.gigade.Model;

namespace Admin.gigade.Controllers
{
    public class PromotionsDeductRateController : Controller
    {
        //
        // GET: /PromotionsDeductRate/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IPromotionsDeductRateImplMgr _promDRate;
        public ActionResult Index()
        { 
            return View();
        }
        #region 查詢出點數抵用列表頁數據+HttpResponseBase promotionsDeductRatelist()
        /// <summary>
        /// 查詢出點數抵用列表頁數據
        /// </summary>
        /// <returns>HttpResponseBase</returns>
        public HttpResponseBase promotionsDeductRatelist()
        {
            List<PromotionsDeductRateQuery> stores = new List<PromotionsDeductRateQuery>();
            string json = string.Empty;
            try
            {
                PromotionsDeductRateQuery query = new PromotionsDeductRateQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用DI於o分A頁?的o變U量q
                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);//用DI於o分A頁?的o變U量q
                }

                if (!string.IsNullOrEmpty(Request.Params["ddlSel"]))
                {
                    query.expired = Convert.ToInt32(Request.Params["ddlSel"]);
                }
                _promDRate = new PromotionsDeductRateMgr(mySqlConnectionString);//實e現方e法k
                int totalCount = 0;
                stores = _promDRate.Query(query, out totalCount);
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
                json = "{success:true,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 刪除數據+HttpResponseBase Delete()
        /// <summary>
        /// 刪除數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase Delete()
        {
            string jsonStr = String.Empty;
            _promDRate = new PromotionsDeductRateMgr(mySqlConnectionString);
            PromotionsDeductRate query = new PromotionsDeductRate();
            if (!String.IsNullOrEmpty(Request.Params["rowid"]))
            {
                try
                {
                    foreach (string rid in Request.Params["rowid"].ToString().Split('|'))
                    {
                        if (!string.IsNullOrEmpty(rid))
                        {
                            query.id = Convert.ToInt32(rid);
                            _promDRate.Delete(query);
                        }
                    }
                    jsonStr = "{success:true}";
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    jsonStr = "{success:true}";
                }
            }

            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 保存點數抵用新增或者修改的數據+HttpResponseBase Save()
        #region  保存前臺新增或者修改的數據 HttpResponseBase SavePromotionsDeductRate()
        /// <summary>
        /// 保存前臺新增或者修改的數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase Save()
        {
            PromotionsDeductRate model = new PromotionsDeductRate();
            model.muser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
            model.modified = DateTime.Now;
            //判斷是否能夠獲取到rowid
            if (String.IsNullOrEmpty(Request.Params["rowid"]))
            {
                model.created = model.modified;
                model.kuser = model.muser;
                try
                {
                    model.amount = Convert.ToInt32(Request.Params["amount"]);
                }
                catch (Exception)
                {
                    model.amount = 0;
                }

                try
                {
                    model.end = Convert.ToDateTime(Request.Params["end"]);
                }
                catch (Exception)
                {
                    model.end = DateTime.Now;
                }
                if (Request.Params["group_id"].ToString() != "")
                {
                    try//group_id
                    {
                        model.group_id = Convert.ToInt32(Request.Params["group_id"].ToString());
                    }
                    catch (Exception)
                    {
                        model.group_id = 0;
                    }
                    model.condition_id = 0;
                }
                else if (Request.Params["condition_id"].ToString() != "" && Request.Params["condition_id"].ToString() != "0")
                {
                    try//condition_id
                    {
                        model.condition_id = Convert.ToInt32(Request.Params["condition_id"].ToString());
                    }
                    catch (Exception)
                    {
                        model.condition_id = 0;
                    }
                    model.group_id = 0;
                }
                try
                {
                    model.start = Convert.ToDateTime(Request.Params["startdate"]);
                }
                catch (Exception)
                {
                    model.start = DateTime.Now;
                }
                try
                {
                    model.bonus_type = Convert.ToInt32(Request.Params["bonus_type"]);
                }
                catch (Exception)
                {
                    model.bonus_type = 0;
                }
                try
                {
                    model.rate = Convert.ToInt32(Request.Params["rate"]);
                }
                catch (Exception)
                {
                    model.rate = 0;
                }
                try
                {
                    model.name = Request.Params["name"];
                }
                catch (Exception)
                {
                    model.name = "";
                }
                try
                {
                    model.dollar = Convert.ToInt32(Request.Params["dollar"]);
                }
                catch (Exception)
                {
                    model.dollar = 0;
                }

                try
                {
                    model.point = Convert.ToInt32(Request.Params["point"]);
                }
                catch (Exception)
                {
                    model.point = 0;
                }

                //todo:對model進行賦值
                return InsertPromotionsDeductRate(model);//如果獲取不到則進行新增

            }
            else
            {
                _promDRate = new PromotionsDeductRateMgr(mySqlConnectionString);
                model.id = Convert.ToInt32(Request.Params["rowid"].ToString());
                PromotionsDeductRate PDR = _promDRate.GetMOdel(model.id);
                try
                {
                    model.amount = Convert.ToInt32(Request.Params["amount"]);
                }
                catch (Exception)
                {
                    model.amount = PDR.amount;
                }
                try
                {
                    model.end = Convert.ToDateTime(Request.Params["end"]);
                }
                catch (Exception)
                {
                    model.end = PDR.end;
                }

                try
                {
                    model.bonus_type = Convert.ToInt32(Request.Params["bonus_type"]);
                }
                catch (Exception)
                {
                    model.bonus_type = 0;
                }

                if (Request.Params["group_id"].ToString() != "")
                {
                    try//group_id
                    {
                        model.group_id = Convert.ToInt32(Request.Params["group_id"].ToString());
                    }
                    catch (Exception)
                    {
                        model.group_id = PDR.group_id;
                    }
                    model.condition_id = 0;
                }
                else if (Request.Params["condition_id"].ToString() != "" && Request.Params["condition_id"].ToString() != "0")
                {
                    try//condition_id
                    {
                        model.condition_id = Convert.ToInt32(Request.Params["condition_id"].ToString());
                    }
                    catch (Exception)
                    {
                        model.condition_id = PDR.condition_id;
                    }
                    model.group_id = 0;
                }
                try
                {
                    model.start = Convert.ToDateTime(Request.Params["startdate"]);
                }
                catch (Exception)
                {
                    model.start = PDR.start;
                }
                try
                {
                    model.rate = Convert.ToInt32(Request.Params["rate"]);
                }
                catch (Exception)
                {
                    model.rate = PDR.rate;
                }
                try
                {
                    model.name = Request.Params["name"];
                }
                catch (Exception)
                {
                    model.name = PDR.name;
                }
                try
                {
                    model.dollar = Convert.ToInt32(Request.Params["dollar"]);
                }
                catch (Exception)
                {
                    model.dollar = PDR.dollar;
                }

                try
                {
                    model.point = Convert.ToInt32(Request.Params["point"]);
                }
                catch (Exception)
                {
                    model.point = PDR.point;
                }
                model.active = false;

                return UpdatePromotionsDeductRate(model);
            }
        }
        #endregion
        #region 新增功能 InsertPromotionsDeductRate +HttpResponseBase InsertPromotionsDeductRate(PromotionsDeductRate model)
        /// <summary>
        /// 新增功能 PromotionsDeductRate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected HttpResponseBase InsertPromotionsDeductRate(PromotionsDeductRate model)
        {
            string json = string.Empty;
            try
            {
                _promDRate = new PromotionsDeductRateMgr(mySqlConnectionString);
                _promDRate.Save(model);
                //listUser是准备转换的对象
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
        #region 修改功能 PromotionsDeductRate +HttpResponseBase UpdatePromotionsDeductRate(PromotionsDeductRate model)
        /// <summary>
        /// 修改功能 PromotionsAmountFare
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected HttpResponseBase UpdatePromotionsDeductRate(PromotionsDeductRate model)
        {
            string json = string.Empty;
            try
            {
                _promDRate.Update(model);
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

        #region 編輯時獲取到某行的數據+HttpResponseBase GetOneModelPage()
        /// <summary>
        /// 編輯時獲取到某行的數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetOneModelPage()
        {
            string json = string.Empty;
            try
            {
                PromotionsDeductRate PDR = new PromotionsDeductRate();
                _promDRate = new PromotionsDeductRateMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["rowid"]))
                {
                    PDR.id = Convert.ToInt32(Request.Params["rowid"].ToString());
                }
                PDR = _promDRate.GetMOdel(PDR.id);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                json = "{success:true,data:" + JsonConvert.SerializeObject(PDR, Formatting.Indented, timeConverter) + "}";
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

        #region 更改活動使用狀態+JsonResult UpdateActive()
        /// <summary>
        /// 更改活動使用狀態
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult UpdateActive()
        {
            try
            {
                string currentUser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                string muser = Request.Params["muser"];
                int activeValue = Convert.ToInt32(Request.Params["active"]);
                if (currentUser == muser && activeValue == 1)
                {
                    return Json(new { success = "stop" });
                }
                _promDRate = new PromotionsDeductRateMgr(mySqlConnectionString);
                PromotionsDeductRate model = new PromotionsDeductRate();
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                     model.id = Convert.ToInt32(Request.Params["id"]);
                }
                model.active = Convert.ToBoolean(activeValue);
                model.muser = int.Parse(currentUser);
                model.modified = DateTime.Now;
                if (_promDRate.UpdateActive(model) > 0)
                {
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

    }
}
