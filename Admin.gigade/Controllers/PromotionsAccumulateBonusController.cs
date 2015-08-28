
#region 文件信息
/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：PromotionsAccumulateBonusController.cs 
 * 摘   要： 
 *      
 * 当前版本：v1.1 
 * 作   者： dongya0410j
 * 完成日期：2014/6/20
 * 修改歷史：
 *      v1.1修改日期：2014/8/15
 *      v1.1修改人員：dongya0410j
 *      v1.1修改内容：代碼合併
 */
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Model.Query;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using BLL.gigade.Model;

namespace Admin.gigade.Controllers
{ 
    public class PromotionsAccumulateBonusController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IPromotionsAccumulateBonusImplMgr _promAccBnus = new PromotionsAccumulateBonusMgr(mySqlConnectionString);
        private IUserConditionImplMgr _ucMgr = new UserConditionMgr(mySqlConnectionString);
        //
        // GET: /PromotionAccomulateBonus/

        public ActionResult Index()
        {
            return View();
        }

        #region 購物金活動列表 +HttpResponseBase List()
        /// <summary>
        /// 購物金活動列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase List()
        {
            List<PromotionsAccumulateBonusQuery> stores = new List<PromotionsAccumulateBonusQuery>();
            string json = string.Empty;
            try
            {
                PromotionsAccumulateBonusQuery query = new PromotionsAccumulateBonusQuery();
                query.Start = Convert.ToInt32(Request["Start"] ?? "0");
                if (!string.IsNullOrEmpty(Request["Limit"]))
                {
                    query.Limit = Convert.ToInt32(Request["Limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ddlSel"]))
                {
                    query.expired = Convert.ToInt32(Request.Params["ddlSel"]);
                }
                else
                {
                    query.expired = 1;
                }
               
                int totalCount = 0;
                stores = _promAccBnus.Query(query, out totalCount);
              
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
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

        #region 新增或修改數據 +HttpResponseBase SavePromotionsAccumulateBonus()
        /// <summary>
        /// 新增或修改數據
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public HttpResponseBase SavePromotionsAccumulateBonus()
        {
            string json = string.Empty;
            Caller caller = Session["caller"] as Caller;
            string rowid = Request.Form["rowid"];
            string name = Request.Form["name"];
            string group_id = Request.Form["group_id"];
            string startTime = Request.Form["startTime"];
            string end = Request.Form["end"];
            string bonus_rate = Request.Form["bonus_rate"];
            string extra_point = Request.Form["extra_point"];
            string bonus_expire_day = Request.Form["bonus_expire_day"];
            string new_user = Request.Form["new_user"];
            string repeat = Request.Form["repeatPresent"];
            string present_time = Request.Form["present_time"];
            string active = Request.Form["active"];
            string event_desc = Request.Form["event_desc"];
            string event_type = Request.Form["event_type"];
            string condition_id = Request.Form["condition_id"];
            string device = Request.Form["device"];
            string payment_code = Request.Form["payment_code"];
            int kuser = caller.user_id;

            string new_user_date = Request.Form["new_user_date"];

            PromotionsAccumulateBonus model=new PromotionsAccumulateBonus();
            int result = 0; //操作數據庫結果

            if (string.IsNullOrEmpty(rowid))    //新增
            {
                model.name = name;
                model.event_desc = event_desc;
                if (!string.IsNullOrEmpty(group_id) )
                {
                    model.group_id = Convert.ToInt32(group_id);
                    //判斷condition_id是否存在如果存在刪除原有數據
                    if (!string.IsNullOrEmpty(condition_id) && condition_id != "0")
                    {
                        UserCondition uc = new UserCondition();
                        uc.condition_id = Convert.ToInt32(condition_id);
                        _ucMgr.Delete(uc);
                    }
                    model.condition_id = 0;
                }
                else if (!string.IsNullOrEmpty(condition_id))
                {
                    model.condition_id = Convert.ToInt32(condition_id);
                    model.group_id = 0;
                }
                if (!string.IsNullOrEmpty(bonus_rate))
                {
                    model.bonus_rate = Convert.ToUInt32(bonus_rate);
                    model.extra_point = 0;
                }
                else if (!string.IsNullOrEmpty(extra_point))
                {
                    model.extra_point = Convert.ToInt32(extra_point);
                    model.bonus_rate = 0;
                }
                model.bonus_expire_day = Convert.ToInt32(bonus_expire_day);
                if (Convert.ToInt32(repeat) == 0)
                {
                    model.repeat = 0;
                    model.present_time = Convert.ToInt32(present_time);
                }
                else if (Convert.ToInt32(repeat) == 1)
                {
                    model.repeat = 1;
                    model.present_time = 0; //贈送次數清0
                }
                else if (Convert.ToInt32(repeat) == 2)
                {
                    model.repeat = 2;
                    model.present_time = 0; //贈送次數清0
                }
                if (!string.IsNullOrEmpty(Request.Params["payment"]))
                {
                    model.payment_code = Request.Params["payment"].ToString();
                }
                model.startTime = Convert.ToDateTime(startTime);
                model.end = Convert.ToDateTime(end);
                model.created = DateTime.Now;
                model.modified = DateTime.Now;
               
                model.muser = kuser;
                model.kuser = kuser;
                model.active = active == null ? false : true;
               

                try
                {
                    result = _promAccBnus.Save(model);
                    if (result == 1)
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
                    json = "{success:false,msg:'" + Resources.Promotion.ADD_FAIL + "'}";
                }
            }
            else    //修改
            {
                model = _promAccBnus.GetModel(Convert.ToInt32(rowid));
                model.name = name;
                model.event_desc = event_desc;
                if (!string.IsNullOrEmpty(group_id))
                {
                    try
                    {
                        model.group_id = Convert.ToInt32(group_id);
                        //判斷condition_id是否存在如果存在刪除原有數據
                        if (!string.IsNullOrEmpty(condition_id) && condition_id != "0")
                        {
                            UserCondition uc = new UserCondition();
                            uc.condition_id = Convert.ToInt32(condition_id);
                            _ucMgr.Delete(uc);
                        }
                        model.condition_id = 0; //將會員條件清0
                    }
                    catch
                    {
                    }
                }
                else if (!string.IsNullOrEmpty(condition_id))
                {
                    model.condition_id = Convert.ToInt32(condition_id);
                    model.group_id = 0; //將會員群組清0
                }
                if (!string.IsNullOrEmpty(bonus_rate))
                {
                    model.bonus_rate = Convert.ToUInt32(bonus_rate);
                    model.extra_point = 0;
                }
                else if (!string.IsNullOrEmpty(extra_point))
                {
                    model.extra_point = Convert.ToInt32(extra_point);
                    model.bonus_rate = 0;
                }
                model.bonus_expire_day = Convert.ToInt32(bonus_expire_day);
                if (!string.IsNullOrEmpty(new_user))
                {
                    model.new_user = false;
                }
                if (Convert.ToInt32(repeat)==0)
                {
                    model.repeat = 0;
                    model.present_time = 0;

                }
                else if (Convert.ToInt32(repeat)==1)
                {
                    model.repeat = 1;
                    //model.present_time = 0; //贈送次數清0
                    model.present_time = Convert.ToInt32(present_time);
                }
                else if (Convert.ToInt32(repeat)==2)
                {
                    model.repeat = 2;
                    //model.present_time = 0; //贈送次數清0
                    model.present_time = Convert.ToInt32(present_time);
                }
                if (!string.IsNullOrEmpty(Request.Params["payment"]))
                {
                    model.payment_code = Request.Params["payment"].ToString();
                }
                model.startTime = Convert.ToDateTime(startTime);
                model.end = Convert.ToDateTime(end);
                model.active = active == null ? false : true;
                model.modified = DateTime.Now;
                model.muser = kuser;
                try
                {
                    result = _promAccBnus.Update(model);
                    if (result == 1)
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
                    json = "{success:false,msg:'" + Resources.Promotion.SAVE_FAIL + "'}";
                }
            }

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        } 
        #endregion

        #region 刪除購物金活動 +HttpResponseBase DeletePromotionsAccumulateBonus()
        /// <summary>
        /// 刪除購物金活動
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public HttpResponseBase DeletePromotionsAccumulateBonus()
        {
            string json = String.Empty;
            if (!String.IsNullOrEmpty(Request.Params["rowid"]))
            {
                try
                {
                    foreach (string rid in Request.Params["rowid"].ToString().Split('|'))
                    {
                        if (!string.IsNullOrEmpty(rid))
                        {
                            _promAccBnus.Delete(Convert.ToInt32(rid));
                        }
                    }
                    json = "{success:true}";
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    json = "{success:false}";
                }
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        } 
        #endregion

        #region 更改活動使用狀態 +JsonResult UpdateActive()
        /// <summary>
        /// 更改活動使用狀態
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult UpdateActive()
        {
            int id = Convert.ToInt32(Request.Params["id"]);
            string currentUser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
            string kuser = string.Empty;
            int activeValue = Convert.ToInt32(Request.Params["active"]);
            if (!string.IsNullOrEmpty(Request.Params["muser"]))
            {
                kuser = (Request.Params["muser"]);
            }
            if (currentUser == kuser && activeValue == 1)
            {
                return Json(new { success = "stop" });
            }
            PromotionsAccumulateBonus model = _promAccBnus.GetModel(id);
            model.active = Convert.ToBoolean(activeValue);
            model.muser = int.Parse(currentUser);
            model.modified = DateTime.Now;
            if (_promAccBnus.Update(model)>0)
            {
                return Json(new { success = "true", msg = "" });
            }
            else
            {
                return Json(new { success = "false", msg = "" });
            }
        }
        #endregion
    }
}
