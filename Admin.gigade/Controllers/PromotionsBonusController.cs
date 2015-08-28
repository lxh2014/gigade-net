/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsBonusController.cs
* 摘 要：
* 序號兌換
* 当前版本：v1.1
* 作 者：dongya0410j    
* 完成日期：2014/6/20 
* 修改歷史:
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：dongya0410j
*         v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using BLL.gigade.Model.Query;
using Newtonsoft.Json.Converters; 
using Newtonsoft.Json;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Mgr;
using BLL.gigade.Model;
using BLL.gigade.Common;
using BLL.gigade.Dao;
using System.Data;
using System.Configuration;
using gigadeExcel.Comment;
using System.IO;
using System.Text;

namespace Admin.gigade.Controllers
{
    public class PromotionsBonusController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IPromotionsBonusImplMgr _promBnus;
        private IPromotionsBonusSerialImplMgr _promBnusSeral;
        private IPromotionsBonusSerialHistoryImplMgr _promBnusSeralHitory;
        private PromotionsBonusSerialDao _pbsd;
        private static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        #region 序號兌換 视图 +ActionResult Index()
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PromotionsBonusSerialHistoryList()
        {
            ViewBag.SerialId = Convert.ToInt32(Request.Params["id"]);
            return View();
        }
        public ActionResult PromotionsBonusSerialList()
        {
            ViewBag.SerialId = Convert.ToInt32(Request.Params["id"]);
            return View();
        }
        #endregion

        #region 序號兌換 查詢出序號兌換+HttpResponseBase PromotionsBonuslist()
        /// <summary>
        /// 查詢出序號兌換列表
        /// </summary>
        /// <returns>序號兌換列表</returns>
        public HttpResponseBase PromotionsBonuslist()
        {
            List<PromotionsBonusQuery> stores = new List<PromotionsBonusQuery>();
            _promBnus = new PromotionsBonusMgr(mySqlConnectionString);
            string json = string.Empty;
            try
            {
                PromotionsBonusQuery query = new PromotionsBonusQuery();
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
                stores = _promBnus.Query(query, out totalCount);
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

        #region 序號兌換 刪除某行數據 +HttpResponseBase Delete()
        /// <summary>
        /// 刪除數據
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public HttpResponseBase Delete()
        {
            string jsonStr = String.Empty;
            _promBnus = new PromotionsBonusMgr(mySqlConnectionString);
            PromotionsBonus query = new PromotionsBonus();
            if (!String.IsNullOrEmpty(Request.Params["rowid"]))
            {
                try
                {
                    foreach (string rid in Request.Params["rowid"].ToString().Split('|'))
                    {
                        if (!string.IsNullOrEmpty(rid))
                        {
                            query.id = Convert.ToInt32(rid);
                            query.muser=int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                            query.modified = DateTime.Now;
                            _promBnus.Delete(query);
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

        #region  序號兌換 保存前臺新增或者修改的數據 HttpResponseBase Save()
        /// <summary>
        /// 保存前臺新增或者修改的數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase Save()
        {
            PromotionsBonus model = new PromotionsBonus();
            model.muser=int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
            //判斷是否能夠獲取到rowid
            if (String.IsNullOrEmpty(Request.Params["rowid"]))  //新增
            {
                model.modified = DateTime.Now;
                model.created = DateTime.Now;
                model.kuser = model.muser;
                //todo:對model進行賦值
                try
                {
                    model.amount = Convert.ToInt32(Request.Params["amount"]);
                }
                catch (Exception)
                {
                    model.amount = 0;
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
                else if (Request.Params["condition_id"].ToString() != "")
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
                    model.end = Convert.ToDateTime(Request.Params["end"]);
                }
                catch (Exception)
                {
                    model.end = DateTime.Now;
                }

                try
                {
                    model.start = Convert.ToDateTime(Request.Params["startbegin"]);
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
                    model.type = Convert.ToInt32(Request.Params["bonus_type"]);
                }
                catch (Exception)
                {
                    model.type = 0;
                }
                try
                {
                    if (Convert.ToInt32(Request.Params["hmcfsy"]) == 1)
                    {
                        model.repeat = true;
                    }
                    else
                    {
                        model.repeat = false;
                    }
                }
                catch (Exception)
                {
                    model.repeat = false;
                }
                try
                {
                    if (Convert.ToInt32(Request.Params["sydzxh"]) == 1)
                    {
                        model.multiple = true;
                    }
                    else
                    {
                        model.multiple = false;
                    }
                }
                catch (Exception)
                {
                    model.multiple = false;
                }
                //添加使用期限
                int day;
                if (Int32.TryParse(Request.Params["days"], out day))
                {
                    model.days = day;
                }
                return InsertPromotionsBonus(model);//如果獲取不到則進行新增

            }
            else    //修改
            {
                _promBnus = new PromotionsBonusMgr(mySqlConnectionString);
                model.id = Convert.ToInt32(Request.Params["rowid"]);
                PromotionsBonus PB = _promBnus.GetModel(model.id);
                model.modified = DateTime.Now;
                try
                {
                    model.amount = Convert.ToInt32(Request.Params["amount"]);
                }
                catch (Exception)
                {
                    model.amount = PB.amount;
                }
                try
                {
                    model.type = Convert.ToInt32(Request.Params["bonus_type"]);
                }
                catch (Exception)
                {
                    model.type = PB.type;
                }
                if (Request.Params["group_id"].ToString() != "")
                {
                    try//group_id
                    {
                        model.group_id = Convert.ToInt32(Request.Params["group_id"].ToString());
                    }
                    catch (Exception)
                    {
                        model.group_id = PB.group_id;
                    }
                    model.condition_id = 0;
                }
                else if (Request.Params["condition_id"].ToString() != "")
                {
                    try//condition_id
                    {
                        model.condition_id = Convert.ToInt32(Request.Params["condition_id"].ToString());
                    }
                    catch (Exception)
                    {
                        model.condition_id = PB.condition_id;
                    }
                    model.group_id = 0;
                }
                try
                {
                    model.end = Convert.ToDateTime(Request.Params["end"]);
                }
                catch (Exception)
                {
                    model.end = PB.end;
                }
                try
                {
                    model.start = Convert.ToDateTime(Request.Params["startbegin"]);
                }
                catch (Exception)
                {
                    model.start = PB.start;
                }
                try
                {
                    model.name = Request.Params["name"];
                }
                catch (Exception)
                {
                    model.name = PB.name;
                }
                try
                {
                    if ("1" == Request.Params["hmcfsy"].ToString())
                    {
                        model.repeat = true;
                    }
                    else
                    {
                        model.repeat = false;
                    }
                }
                catch (Exception)
                {
                    model.repeat = PB.repeat;
                }
                try
                {
                    if ("1" == Request.Params["sydzxh"].ToString())
                    {
                        model.multiple = true;
                    }
                    else
                    {
                        model.multiple = false;
                    }
                }
                catch (Exception)
                {
                    model.multiple = PB.multiple;
                }
                model.active = false;
                int day;
                if (Int32.TryParse(Request.Params["days"], out day))
                {
                    model.days = day;
                }
                //todo:對model進行賦值
                return UpdatePromotionsBonus(model);//如果可以獲取到rowid則進行修改
            }
        }
        #endregion

        #region 序號兌換 新增功能 InsertPromotionsBonus +HttpResponseBase InsertPromotionsBonus(PromotionsBonus model)
        /// <summary>
        /// 新增功能 PromotionsBonus
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected HttpResponseBase InsertPromotionsBonus(PromotionsBonus model)
        {
            string json = string.Empty;
            try
            {
                _promBnus = new PromotionsBonusMgr(mySqlConnectionString);
                _promBnus.Save(model);
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

        #region 序號兌換 修改功能 PromotionsBonus +HttpResponseBase UpdatePromotionsAmountGift(PromotionsBonus model)
        /// <summary>
        /// 修改功能 PromotionsAmountFare
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected HttpResponseBase UpdatePromotionsBonus(PromotionsBonus model)
        {
            string json = string.Empty;
            try
            {
                model.muser = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                model.modified = DateTime.Now;
                //listUser是准备转换的对象
                _promBnus.Update(model);
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

        #region 序號兌換-檢視序號 +HttpResponseBase PromotionsBonusSerialLists()
        [CustomHandleError]
        public HttpResponseBase PromotionsBonusSerialLists()
        {
            List<PromotionsBonusSerial> stores = new List<PromotionsBonusSerial>();
            List<PromotionsBonusSerial> mystores = new List<PromotionsBonusSerial>();
            _promBnusSeral = new PromotionsBonusSerialMgr(mySqlConnectionString);
            string json = string.Empty;
            try
            {
                int id = Convert.ToInt32(Request.Params["ids"]);
                stores = _promBnusSeral.QueryById(id);
                int i;
                int j = 1;
                for (i = 0; i < stores.Count; i++)
                {
                    PromotionsBonusSerial query = new PromotionsBonusSerial();
                    query.id = stores[i].id;
                    query.active = stores[i].active;
                    query.promotion_id = stores[i].promotion_id;
                    query.serial = stores[i].serial;
                    query.myid = j++;
                    mystores.Add(query);
                }
                json = "{success:true,data:" + JsonConvert.SerializeObject(mystores) + "}";//返回json數據

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 序號兌換-檢視記錄 + HttpResponseBase PromotionsBonusSerialHistoryLists()
        [CustomHandleError]
        public HttpResponseBase PromotionsBonusSerialHistoryLists()
        {
            List<PromotionsBonusSerialHistoryQuery> stores = new List<PromotionsBonusSerialHistoryQuery>();
            _promBnusSeralHitory = new PromotionsBonusSerialHistoryMgr(mySqlConnectionString);
            PromotionsBonusSerialHistoryQuery query = new PromotionsBonusSerialHistoryQuery();
            string json = string.Empty;
            try
            {
                int id = Convert.ToInt32(Request.Params["ids"]);
                stores = _promBnusSeralHitory.QueryById(id);

                foreach (var item in stores)
                {
                    item.user_email = item.user_email.Split('@')[0] + "@***";
                }

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,items:" + JsonConvert.SerializeObject(stores, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 序號兌換-檢視序號-插入數據+HttpResponseBase InsertPromotionsBonusSerial()
        public HttpResponseBase InsertPromotionsBonusSerial()
        {
            string json = string.Empty;
            try
            {
                _pbsd = new PromotionsBonusSerialDao(mySqlConnectionString);
                _promBnusSeral = new PromotionsBonusSerialMgr(mySqlConnectionString);
                PromotionsBonusSerial pbs = new PromotionsBonusSerial();
                int couts = Convert.ToInt32(Request.Params["xhsl"]);
                int lenghts = Convert.ToInt32(Request.Params["xhcd"]);
                int id = Convert.ToInt32(Request.Params["ids"]);
                Random rd = new Random();
                for (int i = 0; i < couts; i++)
                {
                    string serials = CommonFunction.Getserials(lenghts, rd);
                    List<PromotionsBonusSerial> ls = new List<PromotionsBonusSerial>();
                    ls = _pbsd.YesOrNoExist(serials);
                    if (ls.Single().serial == serials)
                    {
                        continue;
                    }
                    else
                    {
                        int count = _promBnusSeral.Save(serials, id);
                        if (count > 0)
                        {
                            continue;
                        }
                    }
                }
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

        #region 序號兌換 更改活動使用狀態+JsonResult UpdateActive()
        /// <summary>
        /// 更改活動使用狀態
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult UpdateActive()
        {
            string currentUser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
            string muser = string.Empty;
            int activeValue = Convert.ToInt32(Request.Params["active"]);
            if (!string.IsNullOrEmpty(Request.Params["muser"]))
            {
                muser = (Request.Params["muser"]);
            }
            if (currentUser == muser && activeValue == 1)
            {
                return Json(new { success = "stop" });
            }
            _promBnus = new PromotionsBonusMgr(mySqlConnectionString);
            int id = Convert.ToInt32(Request.Params["id"]);
            PromotionsBonus model = new PromotionsBonus();
            model = _promBnus.GetModel(id);
            model.active = Convert.ToBoolean(activeValue);
            model.muser =int.Parse(currentUser);
            model.modified = DateTime.Now;
            if (_promBnus.UpdateActive(model) > 0)
            {
                return Json(new { success = "true", msg = "" });
            }
            else
            {
                return Json(new { success = "false", msg = "" });
            }
        }
        #endregion


        #region 序列號匯出
        public HttpResponseBase PromotionsBonusSerialListsExport()
        {
            List<PromotionsBonusSerial> stores = new List<PromotionsBonusSerial>();
            List<PromotionsBonusSerial> mystores = new List<PromotionsBonusSerial>();
            string json = "false";
            _promBnusSeral = new PromotionsBonusSerialMgr(mySqlConnectionString);
            int id = 0;
            if (!string.IsNullOrEmpty(Request.Params["ids"]))
            {
                id = Convert.ToInt32(Request.Params["ids"]);
            }
            stores = _promBnusSeral.QueryById(id);
            int j = 1;
            for (int i = 0; i < stores.Count; i++)
            {
                PromotionsBonusSerial query = new PromotionsBonusSerial();
                query.id = stores[i].id;
                query.active = stores[i].active;
                query.promotion_id = stores[i].promotion_id;
                query.serial = stores[i].serial;
                query.myid = j++;
                mystores.Add(query);
            }
            DataTable dtHZ = new DataTable();
            string newExcelName = string.Empty;
            dtHZ.Columns.Add("編號", typeof(string));
            dtHZ.Columns.Add("序號", typeof(string));
            dtHZ.Columns.Add("ACTIVE", typeof(string));
            try
            {
                foreach (PromotionsBonusSerial dr_v in mystores)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = dr_v.myid.ToString();
                    dr[1] = dr_v.serial.ToString();
                    dr[2] = dr_v.active.ToString();
                    dtHZ.Rows.Add(dr);
                }

                string[] colname = new string[dtHZ.Columns.Count];
                string filename = "promotions_bonus_serial.csv";
                newExcelName = Server.MapPath(excelPath) + filename;
                for (int i = 0; i < dtHZ.Columns.Count; i++)
                {
                    colname[i] = dtHZ.Columns[i].ColumnName;
                }

                if (System.IO.File.Exists(newExcelName))
                {
                    System.IO.File.Delete(newExcelName);
                }

                CsvHelper.ExportDataTableToCsv(dtHZ, newExcelName, colname, true);
                json = "true";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);

            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

    }
}
