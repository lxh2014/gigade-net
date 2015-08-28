#region 文件信息
/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：PromotionsAccumulateRateController.cs 
 * 摘   要： 
 *      點數累積
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
using Admin.gigade.CustomError;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using BLL.gigade.Model.Query;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Mgr;
using BLL.gigade.Model;

namespace Admin.gigade.Controllers
{
    public class PromotionsAccumulateRateController : Controller
    {
        //
        // GET: /PromotionsAccumulateRate/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IPromotionsAccumulateRateImplMgr _padAccRateMgr;
        //private IPaymentTypeImplMgr _paytyMgr;
        private IParametersrcImplMgr _parasrcMgr;

        private IUserConditionImplMgr _ucMgr = new UserConditionMgr(mySqlConnectionString);

        public ActionResult Index()
        {
            return View();
        }

        #region 點數累積 列表頁 +HttpResponseBase PromotionsAccumulateRatelist()
        [CustomHandleError]
        public HttpResponseBase PromotionsAccumulateRatelist()
        {
            List<PromotionsAccumulateRateQuery> stores = new List<PromotionsAccumulateRateQuery>();
            _padAccRateMgr = new PromotionsAccumulateRateMgr(mySqlConnectionString);
            _parasrcMgr = new ParameterMgr(mySqlConnectionString);
            string json = string.Empty;
            try
            {
                PromotionsAccumulateRateQuery query = new PromotionsAccumulateRateQuery();
                query.Start = Convert.ToInt32(Request["Start"] ?? "0");
                if (!string.IsNullOrEmpty(Request["Limit"]))
                {
                    query.Limit = Convert.ToInt32(Request["Limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ddlSel"]))
                {
                    query.expired = Convert.ToInt32(Request.Params["ddlSel"]);
                }
                int totalCount = 0;
                stores = _padAccRateMgr.AllMessage(query, ref totalCount);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                foreach (var item in stores)
                {

                    if (!string.IsNullOrEmpty(item.payment_type_rid))
                    {
                        string[] arryPayment = item.payment_type_rid.Split(',');//將payment_code轉化為payment_name
                        for (int i = 0; i < arryPayment.Length; i++)
                        {
                            if (arryPayment[i] != "0" && arryPayment[i] != "")
                            {
                                var _alist = _parasrcMgr.QueryUsed(new Parametersrc { ParameterType = "payment", Used = 1, ParameterCode = arryPayment[i] });
                                if (_alist.Count > 0)
                                {
                                    string nameStr = _alist.FirstOrDefault().parameterName;
                                    if (!string.IsNullOrEmpty(nameStr))
                                    {
                                        item.payment_name += nameStr + ",";
                                    }
                                }
                            }

                        }
                        item.payment_name = item.payment_name.TrimEnd(',');
                    }
                }
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

        #region 點數累積 刪除 +HttpResponseBase Delete()
        public HttpResponseBase Delete()
        {
            string jsonStr = String.Empty;
            _padAccRateMgr = new PromotionsAccumulateRateMgr(mySqlConnectionString);
            PromotionsAccumulateRate query = new PromotionsAccumulateRate();
            if (!String.IsNullOrEmpty(Request.Params["rowid"]))
            {
                try
                {
                    foreach (string rid in Request.Params["rowid"].ToString().Split('|'))
                    {
                        if (!string.IsNullOrEmpty(rid))
                        {
                            query.id = Convert.ToInt32(rid);
                            query.muser =(System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                            query.modified = DateTime.Now;
                            _padAccRateMgr.Delete(query);
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
                    jsonStr = "{success:false}";
                }
            }

            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 點數累積 獲取付款類型 +HttpResponseBase getfkfs()  已廢除
        //public HttpResponseBase getfkfs()
        //{
        //    List<PaymentType> fklx = new List<PaymentType>();
        //    List<PaymentType> nfklx = new List<PaymentType>();
        //    string resultStr = "";
        //    try
        //    {
        //        _paytyMgr = new PaymentTypeMgr(mySqlConnectionString);
        //        fklx = _paytyMgr.Myfkfs();
        //        PaymentType pt1 = new PaymentType();
        //        pt1.payment_name = "不分";
        //        pt1.payment_code = "0";
        //        nfklx.Add(pt1);

        //        for (int i = 0; i < fklx.Count; i++)
        //        {
        //            PaymentType pt = new PaymentType();
        //            pt.payment_name = fklx[i].payment_name;
        //            pt.payment_code = fklx[i].payment_code;
        //            nfklx.Add(pt);
        //        }

        //        resultStr = JsonConvert.SerializeObject(nfklx);

        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //    }

        //this.Response.Clear();
        //this.Response.Write(resultStr);
        //this.Response.End();
        //return this.Response;
        //}
        #endregion

        #region 點數累積 判斷保存或者編輯 +HttpResponseBase Save()
        public HttpResponseBase Save()
        {
            PromotionsAccumulateRate model = new PromotionsAccumulateRate();
            model.muser=(System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
            model.modified = DateTime.Now;
            string jsonStr = String.Empty;
            //判斷是否能夠獲取到rowid
            if (String.IsNullOrEmpty(Request.Params["rowid"]))
            {
                //todo:對model進行賦值  
                model.kuser = model.muser;
                model.created = model.modified;
                #region try catch 應用
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
                    if (Convert.ToInt32(Request.Params["bonus_type"]) == 1)
                    {
                        model.bonus_type = 1;
                    }
                    else
                    {
                        model.bonus_type = 0;
                    }
                }
                catch
                {
                    model.bonus_type = 0;
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
                    model.start = Convert.ToDateTime(Request.Params["newstart"]);
                }
                catch (Exception)
                {
                    model.start = DateTime.Now;
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
                    model.point = Convert.ToInt32(Request.Params["points"]);
                }
                catch (Exception)
                {
                    model.point = 0;
                }
                try
                {
                    model.payment_type_rid = Request.Params["payment_id"];
                }
                catch (Exception)
                {
                    model.payment_type_rid = "";
                }
                #endregion
                return InsertPromotionsAccumulateRate(model);//如果獲取不到則進行新增

            }
            else
            {
                _padAccRateMgr = new PromotionsAccumulateRateMgr(mySqlConnectionString);
                model.id = Convert.ToInt32(Request.Params["rowid"].ToString());
                PromotionsAccumulateRate PAR = _padAccRateMgr.GetModel(model.id);
                #region try catch應用
                try
                {
                    model.amount = Convert.ToInt32(Request.Params["amount"]);
                }
                catch (Exception)
                {
                    model.amount = PAR.amount;
                }

                try
                {
                    model.bonus_type = Convert.ToInt32(Request.Params["bonus_type"]);
                }
                catch (Exception)
                {
                    model.bonus_type = PAR.bonus_type;
                }

                try
                {
                    model.end = Convert.ToDateTime(Request.Params["end"]);
                }
                catch (Exception)
                {
                    model.end = PAR.end;
                }


                try
                {
                    model.start = Convert.ToDateTime(Request.Params["newstart"]);
                }
                catch (Exception)
                {
                    model.start = PAR.start;
                }
                try
                {
                    model.name = Request.Params["name"];
                }
                catch (Exception)
                {
                    model.name = PAR.name;
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
                    model.point = Convert.ToInt32(Request.Params["points"]);
                }
                catch (Exception)
                {
                    model.point = PAR.point;
                }
                try
                {
                    model.payment_type_rid = Request.Params["payment_id"];
                }
                catch (Exception)
                {
                    model.payment_type_rid = PAR.payment_type_rid;
                }

                //try
                //{

                //    model.group_id = Convert.ToInt32(Request.Params["group_id"]);

                //}
                //catch (Exception)
                //{
                //    model.group_id = PAR.group_id;
                //}

                //try
                //{
                //    if (Convert.ToInt32(Request.Params["condition_id"]) != 0)
                //    {
                //        model.condition_id = Convert.ToInt32(Request.Params["condition_id"]);
                //        //model.group_id = 0;
                //    }
                //    else
                //    {
                //        model.condition_id = 0;
                //    }
                //}
                //catch (Exception)
                //{
                //    model.condition_id = PAR.condition_id;
                //}
                if (Request.Params["group_id"].ToString() != "")
                {
                    try//group_id
                    {
                        model.group_id = Convert.ToInt32(Request.Params["group_id"].ToString());
                    }
                    catch (Exception)
                    {
                        model.group_id = PAR.group_id;
                    }

                    if (Request.Params["condition_id"].ToString() != "" && Request.Params["condition_id"].ToString() != "0")
                    {
                        UserCondition uc = new UserCondition();
                        uc.condition_id = Convert.ToInt32(Request.Params["condition_id"]);
                        if (_ucMgr.Delete(uc) > 0)
                        {
                            jsonStr = "{success:true}";
                            model.condition_id = 0;
                        }
                        else
                        {
                            jsonStr = "{success:false,msg:'user_condition刪除出錯！'}";
                            this.Response.Clear();
                            this.Response.Write(jsonStr.ToString());
                            this.Response.End();
                            return this.Response;
                        }
                    }
                }
                else if (Request.Params["condition_id"].ToString() != "" && Request.Params["condition_id"].ToString() != "0")
                {
                    try//condition_id
                    {
                        model.condition_id = Convert.ToInt32(Request.Params["condition_id"].ToString());
                    }
                    catch (Exception)
                    {
                        model.condition_id = PAR.condition_id;
                    }
                    model.group_id = 0;
                }
                #endregion
                model.active = false;
                //todo:對model進行賦值
                return UpdatePromotionsAccumulateRate(model);//如果可以獲取到rowid則進行修改
            }
        }
        #endregion
        #region 點數累積 保存  +HttpResponseBase InsertPromotionsAccumulateRate(PromotionsAccumulateRate model)
        /// <summary>
        /// 新增功能 PromotionsAccumulateRate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected HttpResponseBase InsertPromotionsAccumulateRate(PromotionsAccumulateRate model)
        {

            string json = string.Empty;
            try
            {
                _padAccRateMgr = new PromotionsAccumulateRateMgr(mySqlConnectionString);
                //listUser是准备转换的对象
                _padAccRateMgr.Save(model);
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
        #region 點數累積 編輯 +PromotionsAccumulateRate +HttpResponseBase UpdatePromotionsAmountGift(PromotionsAccumulateRate model)
        /// <summary>
        /// 修改功能 PromotionsAmountFare
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected HttpResponseBase UpdatePromotionsAccumulateRate(PromotionsAccumulateRate model)
        {
            string json = string.Empty;
            try
            {
                _padAccRateMgr.Update(model);
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

        #region 點數累積 更改活動狀態 +JsonResult UpdateActive()
        /// <summary>
        /// 更改活動使用狀態
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult UpdateActive()
        {
            string currentUser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
            string muser = Request.Params["muser"];
            int activeValue = Convert.ToInt32(Request.Params["active"]);
            if (currentUser == muser && activeValue == 1)
            {
                return Json(new { success = "stop" });
            }
            _padAccRateMgr = new PromotionsAccumulateRateMgr(mySqlConnectionString);
            int id = Convert.ToInt32(Request.Params["id"]);
            PromotionsAccumulateRate model = new PromotionsAccumulateRate();
            model.active = Convert.ToBoolean(activeValue);
            model.modified = DateTime.Now;
            model.muser = int.Parse(currentUser);
            model.id = id;
            if (_padAccRateMgr.UpdateActive(model) > 0)
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
