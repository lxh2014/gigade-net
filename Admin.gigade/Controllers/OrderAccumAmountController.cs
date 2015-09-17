using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.gigade.Controllers
{
    public class OrderAccumAmountController : Controller
    {
        //
        // GET: /OrderAccumAmount/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IOrderAccumAmountImplMgr _IOrderAccumAmount;
        #region 視圖
       
        public ActionResult OrderAccumAmountView()
        {
            return View();
        }
        #endregion
        #region 會員累積金額
        /// <summary>
        /// 會員累計金額列表頁
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetOrderAccumAmountList()
        {
            string jsonStr = string.Empty;
            int tranInt = 0;
            try
            {
                OrderAccumAmountQuery query = new OrderAccumAmountQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                 query.event_status = -1;
                if (int.Parse(Request.Params["event_status"])!=-1)
                {
                    query.event_status = int.Parse(Request.Params["event_status"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["event_search"]))
                {
                    if (int.TryParse(Request.Params["event_search"], out tranInt))
                    {
                        query.event_id = int.Parse(Request.Params["event_search"].ToString());
                    }
                    else
                    {
                        query.event_name = Request.Params["event_search"];
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["date"]))
                {
                    query.dateCondition = Convert.ToInt32(Request.Params["date"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["TimeStart"]))
                {
                    query.event_start_time = Convert.ToDateTime(Convert.ToDateTime(Request.Params["TimeStart"].ToString()).ToString("yyyy-MM-dd 00:00:00"));
                }
                if (!string.IsNullOrEmpty(Request.Params["TimeEnd"]))
                {
                    query.event_end_time = Convert.ToDateTime(Convert.ToDateTime(Request.Params["TimeEnd"].ToString()).ToString("yyyy-MM-dd 23:59:59"));
                }
                int totalCount = 0;
                _IOrderAccumAmount = new OrderAccumAmountMgr(mySqlConnectionString);

                DataTable _dt = _IOrderAccumAmount.GetOrderAccumAmountTable(query, out totalCount);

                //foreach (DataRow item in _dt.Rows)
                //{
                //    string s=item["event_desc_start"].ToString().Substring(0,8);
                //    if (item["event_desc_start"].ToString().Substring(0, 8) == "0000-000-00")
                //    { 
                    
                //    }
                //}
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                jsonStr = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        /// <summary>
        ///會員累積金額新增與編輯
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SaveOrderAccumAmount()
        {

            string json = string.Empty;
            try
            {
                OrderAccumAmountQuery query = new OrderAccumAmountQuery();
                if (!string.IsNullOrEmpty(Request.Params["event_id"]))
                {
                    query.event_id = Convert.ToInt32(Request.Params["event_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["event_name"]))
                {
                    query.event_name = Request.Params["event_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["event_desc"]))
                {
                    query.event_desc = Request.Params["event_desc"];
                }
                if (!string.IsNullOrEmpty(Request.Params["event_start_time"]))
                {
                    query.event_start_time =DateTime.Parse(Request.Params["event_start_time"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["event_end_time"]))
                {
                    query.event_end_time = DateTime.Parse(Request.Params["event_end_time"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["event_desc_start"]))
                {
                    query.event_desc_start = DateTime.Parse(Request.Params["event_desc_start"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["event_desc_end"]))
                {
                    query.event_desc_end = DateTime.Parse(Request.Params["event_desc_end"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["accum_amount"]))
                {
                    query.accum_amount = Convert.ToInt32(Request.Params["accum_amount"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["event_status"]))
                {
                    query.event_status = Convert.ToInt32(Request.Params["event_status"]);
                }
                query.event_create_user = (Session["caller"] as Caller).user_id;
                query.event_update_user = (Session["caller"] as Caller).user_id;
                query.event_create_time = DateTime.Now;
                query.event_update_time = query.event_create_time;
                _IOrderAccumAmount = new OrderAccumAmountMgr(mySqlConnectionString);
                int result = 0;
                if (query.event_id != 0)//編輯
                {
                    result = _IOrderAccumAmount.UPOrderAccumAmount(query);
                }
                else //新增
                {
                    result = _IOrderAccumAmount.AddOrderAccumAmount(query);
                }
               
                if (result > 0)
                {
                    json = "{\"success\":\"true\",\"msg\":\"保存成功!\"}";
                }
                else
                {
                    json = "{\"success\":\"false\",\"msg\":\"保存失敗!\"}";
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{\"success\":\"false\",\"msg\":\"參數出錯!\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        /// <summary>
        /// 會員累積金額修改狀態
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateActiveQuestion()
        {
            try
            {
                OrderAccumAmountQuery query = new OrderAccumAmountQuery();
                if (!string.IsNullOrEmpty(Request.Params["event_id"].ToString()))
                {
                    query.event_id = Convert.ToInt32(Request.Params["event_id"].ToString());
                }
                query.event_status = Convert.ToInt32(Request.Params["event_status"] ?? "0");
                _IOrderAccumAmount = new OrderAccumAmountMgr(mySqlConnectionString);



                query.event_update_user = (Session["caller"] as Caller).user_id;
                query.event_update_time= DateTime.Now;
                if (_IOrderAccumAmount.UpdateActive(query) > 0)
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
        /// <summary>
        /// 會員累積金額刪除
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase DeleteOrderAccumAmount()
        {
            OrderAccumAmountQuery query = new OrderAccumAmountQuery();
            string json = string.Empty;
            try
            {
                string Row_id = "";
                if (!string.IsNullOrEmpty(Request.Params["rowId"]))
                {
                    Row_id = Request.Params["rowId"];
                    query.event_id_in = Row_id.TrimEnd(',');
                }

                _IOrderAccumAmount = new OrderAccumAmountMgr(mySqlConnectionString);

                int result = _IOrderAccumAmount.DelOrderAccumAmount(query);
                if (result > 0)
                {

                    json = "{success:true,msg:\"" + result + "\"}";
                }
                else
                {
                    json = "{success:false,msg:\"" + result + "\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'0'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }
        #endregion
    }
}
