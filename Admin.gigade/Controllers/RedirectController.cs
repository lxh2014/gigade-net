using BLL.gigade.Common;
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
using System.Text;
using System.Configuration;
using System.IO;
using gigadeExcel.Comment;
using System.Text.RegularExpressions;


namespace Admin.gigade.Controllers
{
    public class RedirectController : Controller
    {
        //
        // GET: /Redirect/
        private static DataTable DTExcels = new DataTable();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        private string LinkPath = ConfigurationManager.AppSettings["phpWeb_host"];
        private IRedirectGroupImplMgr _rdGroupMgr;
        private IRedirectImplMgr _redirectMgr;
        private IRedirectClickImplMgr _rcMgr;
        private ISerialImplMgr _serialMgr;
        private IVipUserGroupImplMgr _vipGroupMgr = new VipUserGroupMgr(mySqlConnectionString);

        private IParametersrcImplMgr _parametersrc;
        #region View視圖
        public ActionResult Group()
        {
            return View();
        }
        public ActionResult CounterAllView()
        {
            ViewBag.group_id = Request.Params["group_id"];
            ViewBag.group_name = Request.Params["group_name"];
            return View();
        }
        public ActionResult Count()
        {
            ViewBag.group_id = Request.Params["group_id"];
            ViewBag.group_name = Request.Params["group_name"];
            return View();
        }
        public ActionResult RedirectList()
        {
            ViewBag.group_id = Request.Params["group_id"];
            int group_id = 0;
            if (!string.IsNullOrEmpty(Request.Params["group_id"]))
            {
                group_id = int.Parse(Request.Params["group_id"]);
            }
            _rdGroupMgr = new RedirectGroupMgr(mySqlConnectionString);
            //獲得群組名稱
            ViewBag.group_name = _rdGroupMgr.GetGroupName(group_id);
            ViewBag.LinkAdress = LinkPath;
            ViewBag.BaseAddress = ConfigurationManager.AppSettings["webDavBaseAddress"];
            ViewBag.path = ConfigurationManager.AppSettings["webDavImage"];
            return View();
        }
        public ActionResult CounterView()
        {
            ViewBag.redirect_id = Request.Params["redirect_id"];
            ViewBag.group_id = Request.Params["group_id"];
            int group_id = 0;
            if (!string.IsNullOrEmpty(Request.Params["group_id"]))
            {
                group_id = int.Parse(Request.Params["group_id"]);
            }
            _rdGroupMgr = new RedirectGroupMgr(mySqlConnectionString);
            //獲得群組名稱
            ViewBag.group_name= _rdGroupMgr.GetGroupName(group_id);
            return View();
        }
        public ActionResult GroupImport()//連接管理 群組管理 匯入
        {
            ViewBag.group_id = Convert.ToInt32(Request.Params["group_id"]);
            return View();
        }
        public ActionResult RedirectView()
        {
            return View();
        }
        #endregion

        #region 群組列表頁
        public HttpResponseBase RedirectGroupList()
        {
            List<RedirectGroupQuery> rgli = new List<RedirectGroupQuery>();
            string json = string.Empty;
            try
            {
                RedirectGroup RGModel = new RedirectGroup();
                RGModel.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                RGModel.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                _rdGroupMgr = new RedirectGroupMgr(mySqlConnectionString);
                RGModel.group_name = Request.Params["search_content"];
                int totalCount = 0;
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                rgli = _rdGroupMgr.QueryAll(RGModel, out totalCount);
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(rgli, Formatting.Indented, timeConverter) + "}";

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
        #region 群組報表匯出
        public void RedirectClickExport()
        {
            string newCsvName = string.Empty;
            string json = string.Empty;
            _rdGroupMgr = new RedirectGroupMgr(mySqlConnectionString);
            DataTable dt = new DataTable();
            DataTable dtCsv = new DataTable();
            string newExcelName = string.Empty;
            try
            {
                _redirectMgr = new RedirectMgr(mySqlConnectionString);
                RedirectQuery query = new RedirectQuery();
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = Convert.ToUInt32(Request.Params["group_id"].ToString());
                }
                int totalCount = 0;
                query.IsPage = false;
                dt = _redirectMgr.GetRedirectList(query, out totalCount);
                dtCsv = dt.DefaultView.ToTable(false, new string[] { "group_id", "group_name", "redirect_url", "redirect_total", "status" });
                string[] colname = { "群組編號", "群組名稱", "目的連結", "點閱次數", "狀態" };
                string filename = "group_click_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                newExcelName = Server.MapPath(excelPath) + filename;
                if (System.IO.File.Exists(newExcelName))
                {
                    System.IO.File.SetAttributes(newExcelName, FileAttributes.Normal);
                    System.IO.File.Delete(newExcelName);
                }
                StringWriter sw = ExcelHelperXhf.SetCsvFromData(dt, filename);
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(filename));
                Response.ContentType = "application/ms-excel";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(sw);
                Response.End();
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "false";
            }
        }
        #endregion
        #region 保存群組編輯或新增
        public HttpResponseBase SaveGroup()
        {
            string json = string.Empty;
            try
            {
                RedirectGroup rgModel = new RedirectGroup();
                _rdGroupMgr = new RedirectGroupMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    rgModel.group_id = Convert.ToUInt32(Request.Params["group_id"].ToString());
                    rgModel.group_name = Request.Params["group_name"].ToString();
                    rgModel.group_updatedate = Convert.ToUInt32(CommonFunction.GetPHPTime());
                    if (!string.IsNullOrEmpty(Request.Params["group_type"].ToString()))
                    {
                        rgModel.group_type = Request.Params["group_type"].ToString();
                    }
                    rgModel.group_ipfrom = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList.FirstOrDefault().ToString();
                    if (_rdGroupMgr.Update(rgModel) > 0)
                    {
                        json = "{success:true}";
                    }
                }
                else
                {
                    rgModel.group_name = Request.Params["group_name"].ToString();
                    if (!string.IsNullOrEmpty(Request.Params["group_type"].ToString()))
                    {
                        rgModel.group_type = Request.Params["group_type"].ToString();
                    }
                    rgModel.group_createdate = Convert.ToUInt32(CommonFunction.GetPHPTime());
                    rgModel.group_updatedate = rgModel.group_createdate;
                    rgModel.group_ipfrom = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList.FirstOrDefault().ToString();
                    if (_rdGroupMgr.Save(rgModel) > 0)
                    {
                        json = "{success:true}";
                    }
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
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region  匯出群組csv檔案
        public void GroupExportCSV()
        {
            string newCsvName = string.Empty;
            string json = string.Empty;
            _rdGroupMgr = new RedirectGroupMgr(mySqlConnectionString);
            DataTable dt = new DataTable();
            DataTable dtCsv = new DataTable();
            string newExcelName = string.Empty;
            try
            {
                _redirectMgr = new RedirectMgr(mySqlConnectionString);
                RedirectQuery query = new RedirectQuery();
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = Convert.ToUInt32(Request.Params["group_id"].ToString());
                }
                #region 歷史
                //RedirectGroup rgModel = new RedirectGroup();
                //rgModel.group_id = Convert.ToUInt32(Request.Params["group_id"].ToString());
                //List<Redirect> rli = _rdGroupMgr.QueryRedirectAll(rgModel.group_id);
                //dt.Columns.Add("日期");
                //// Array rename = rli.GroupBy(m => m.redirect_name).Select(m => m.Key).ToArray();
                //// Array reID = rli.GroupBy(m => m.redirect_id).Select(m => m.Key).ToArray();
                //string[] reArray = null;
                //foreach (var li in rli)
                //{
                //    dt.Columns.Add(li.redirect_name);
                //    reArray[li.redirect_id] = li.redirect_name;
                //}
                //// 查詢點率次數
                //List<RedirectClick> reCli = _rdGroupMgr.QueryRedirectClictAll(null);
                //uint minClick = Convert.ToUInt32(reCli.Min(m => m.click_id).ToString());
                //if (minClick > 3000123123)
                //{
                //    minClick = 3000123123;
                //}
                //string[][] reclickArray = null;
                //foreach (var cli in reCli)
                //{
                //    //minClick = minClick > cli.click_id ? cli.click_id : minClick;
                //    int redid = (int)cli.redirect_id;
                //    string d = cli.click_year + "-" + cli.click_month.ToString().Substring(0, 2) + "-" + cli.click_day.ToString().Substring(0, 2);

                //    if (reclickArray['d'][redid] != null)
                //    {
                //        reclickArray['d'][redid] = reclickArray['a'][redid] + cli.click_total;
                //    }
                //    else
                //    {
                //        reclickArray['d'][redid] = cli.click_total.ToString();
                //    }
                //}
                //// int count = 0;
                #endregion
                int totalCount = 0;
                query.IsPage = false;
                dt = _redirectMgr.GetRedirectList(query, out totalCount);
                dtCsv = dt.DefaultView.ToTable(false, new string[] { "group_id", "group_name", "redirect_url", "redirect_total", "redirect_status" });
                for (int i = 0; i < dtCsv.Rows.Count; i++)
                {
                    switch (dtCsv.Rows[i]["redirect_status"].ToString())
                    {
                        case "1":
                            dtCsv.Rows[i]["invoice_status"] = "正常";
                            break;
                        case "2":
                            dtCsv.Rows[i]["invoice_status"] = "停用";
                            break;
                        default:
                            break;
                    }
                    }
                string[] colname = { "群組編號", "群組名稱", "目的連結", "點閱次數", "狀態" };
                string filename = "group_click_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                newExcelName = Server.MapPath(excelPath) + filename;
                if (System.IO.File.Exists(newExcelName))
                    {
                    //設置文件的屬性，以防刪除文件的時候因為文件的屬性造成無法刪除
                    System.IO.File.SetAttributes(newExcelName, FileAttributes.Normal);
                    System.IO.File.Delete(newExcelName);
                }
                StringWriter sw = ExcelHelperXhf.SetCsvFromData(dt, filename);
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(filename));
                Response.ContentType = "application/ms-excel";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(sw);
                Response.End();
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "false";
            }
        }
        #endregion
        public HttpResponseBase GetCountdate()
        {
            string json = string.Empty;
            try
            {
                DateTime Today = DateTime.Now;
                json = "{success:true,data:[";
                for (int i = 0; i < 12; i++)
                {
                    DateTime DateTemp = Today.AddMonths(-i);
                    json += "{";
                    json += string.Format("\"Value\":\"{0}\"", DateTemp.Year + "/" + DateTemp.Month);
                    json += "}";
                }
                json += "]}";
                json = json.ToString().Replace("}{", "},{");
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
        #region 群組列表報表button
        public HttpResponseBase GetDailyGrid()
        {
            string json = string.Empty;
            try
            {
                //  string selectdate = Request.Params["selectDate"].ToString();
                string selectdate = "2015/2";
                DateTime getMonthDaily = Convert.ToDateTime(Convert.ToDateTime(selectdate).ToString("yyyy-MM-dd 23:59:59"));
                int year = getMonthDaily.Year;
                int month = getMonthDaily.Month;
                RedirectClickQuery RcQuery = new RedirectClickQuery();
                RcQuery.startdate = Convert.ToInt32(year.ToString("0000") + month.ToString("00") + getMonthDaily.Day.ToString("00") + "00");
                RcQuery.enddate = Convert.ToInt32(year.ToString("0000") + getMonthDaily.AddMonths(1).Month.ToString("00") + getMonthDaily.Day.ToString("00") + "00");
                int dayNum = (getMonthDaily.AddMonths(1) - getMonthDaily).Days;
                uint group_id = Convert.ToUInt32(Request.Params["group_id"].ToString());
                _rdGroupMgr = new RedirectGroupMgr(mySqlConnectionString);
                List<Redirect> rgli = _rdGroupMgr.QueryRedirectAll(group_id);
                if (rgli.Count != 0)
                {
                    foreach (var item in rgli)
                    {
                        if (string.IsNullOrEmpty(RcQuery.redirectstr))
                        {
                            RcQuery.redirectstr += item.redirect_id;
                        }
                        else
                        {
                            RcQuery.redirectstr += "," + item.redirect_id;
                        }
                    }
                }
                List<RedirectClick> reCli = _rdGroupMgr.QueryRedirectClictAll(RcQuery);
                uint[] redayArray = new uint[dayNum];
                uint[] reweekArray = new UInt32[7];
                uint[] rehourArray = new UInt32[24];
                StringBuilder dayJson = new StringBuilder();
                //dayJson.Append("{\"day\":[");
                StringBuilder weekJson = new StringBuilder();
                weekJson.Append("{\"week\":[");
                StringBuilder hourJson = new StringBuilder();
                hourJson.Append("{\"hour\":[");
                foreach (var item in reCli)
                {
                    redayArray[item.click_day - 1] += item.click_total;
                    reweekArray[item.click_week] += item.click_total;
                    rehourArray[item.click_hour] += item.click_total;
                }
                DateTime Wday = new DateTime();
                for (int i = 1; i < redayArray.Length; i++)
                {
                    int wd = 0;
                    Wday = Convert.ToDateTime(year.ToString("0000") + "/" + month.ToString("00") + "/" + i.ToString("00"));
                    if (Wday.DayOfWeek == DayOfWeek.Sunday || Wday.DayOfWeek == DayOfWeek.Saturday)
                    {
                        wd = 1;
                    }
                    dayJson.Append("{");
                    dayJson.Append(string.Format("\"datetime\":\"{0}\",\"click_total\":\"{1}\",\"holiday\":\"{2}\"", Wday.ToShortDateString(), redayArray[i - 1], wd));
                    dayJson.Append("}");
                }
                //dayJson.Append("]}");

                for (int i = 0; i < reweekArray.Length; i++)
                {
                    weekJson.Append("{");
                    weekJson.Append(string.Format("\"datetime\":\"{0}\",\"click_total\":\"{1}\"", i, reweekArray[i]));
                    weekJson.Append("}");
                }
                weekJson.Append("]}");
                //weekJson.ToString().Replace("}{", "},{"); 
                for (int i = 0; i < rehourArray.Length; i++)
                {
                    hourJson.Append("{");
                    hourJson.Append(string.Format("\"datetime\":\"{0}\",\"click_total\":\"{1}\"", i, rehourArray[i]));
                    hourJson.Append("}");
                }
                hourJson.Append("]}");
                //hourJson.ToString().Replace("}{", "},{");
                //json = "{success:true,data:[" + (dayJson.ToString() + weekJson.ToString() + hourJson.ToString()).Replace("}{", "},{") + "]}";
                json = "{success:true,data:[" + (dayJson.ToString()).Replace("}{", "},{") + "]}";
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
        public HttpResponseBase GetGroupName()
        {
            string json = string.Empty;
            int group_id = 0;
            string group_name = string.Empty;
            try
            {
                _rdGroupMgr = new RedirectGroupMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    group_id = Convert.ToInt32(Request.Params["group_id"]);
                }
                group_name = _rdGroupMgr.GetGroupName(group_id);
                json = "{success:true,data:'" + group_name + "'}";
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

        #region 鏈接列表頁 
        public HttpResponseBase GetRedirectList()
        {
            DataTable dt = new DataTable();
            List<RedirectQuery> list = new List<RedirectQuery>();
            string json = string.Empty;
            try
            {
                RedirectQuery query = new RedirectQuery();
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = uint.Parse(Request.Params["group_id"]);
                }
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                _redirectMgr = new RedirectMgr(mySqlConnectionString);
                int totalCount = 0;
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
               // dt = _redirectMgr.GetRedirectList(query, out totalCount);
                list = _redirectMgr.GetRedirect(query, out totalCount);
                foreach (var item in list)
                {
                    item.sredirect_createdate = CommonFunction.GetNetTime(item.redirect_createdate);
                    item.sredirect_updatedate = CommonFunction.GetNetTime(item.redirect_updatedate);
                    query.redirect_id = item.redirect_id;
                    query.selsum = 0;
                    item.user = _redirectMgr.GetSum(query);
                    query.selsum = 1;
                    item.order = _redirectMgr.GetSum(query);
                }
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(list, Formatting.Indented, timeConverter) + "}";
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
        #region 獲取會員群組
        public HttpResponseBase GetVipGroup()
        {
            List<VipUserGroup> stores = new List<VipUserGroup>();
            string json = string.Empty;
            try
            {
                int totalCount = 0;
                stores = _vipGroupMgr.GetAllUserGroup();
                VipUserGroup Dmodel = new VipUserGroup();
                Dmodel.group_id = 0;
                Dmodel.group_name = "不綁定";
                stores.Insert(0, Dmodel);//使會員群組的第一行為不分
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
        #region 保存連結編輯或新增
        public HttpResponseBase SaveRedirect()
        {
            string json = string.Empty;
            try
            {
                Redirect redirect = new Redirect();
                _redirectMgr = new RedirectMgr(mySqlConnectionString);
                _serialMgr = new SerialMgr(mySqlConnectionString);
                redirect.redirect_name = Request.Params["redirect_name"].ToString();
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    redirect.group_id = uint.Parse(Request.Params["group_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["user_group_id"]))
                {
                    redirect.user_group_id = int.Parse(Request.Params["user_group_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["redirect_status"]))
                {
                    redirect.redirect_status = uint.Parse(Request.Params["redirect_status"]);
                }
                redirect.redirect_url = Request.Params["redirect_url"];
                redirect.redirect_note = Request.Params["redirect_note"];

                redirect.redirect_ipfrom = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList.FirstOrDefault().ToString();
                if (!string.IsNullOrEmpty(Request.Params["redirect_id"]))
                {
                    redirect.redirect_id = uint.Parse(Request.Params["redirect_id"]);
                    redirect.redirect_updatedate = Convert.ToUInt32(CommonFunction.GetPHPTime());
                    if (_redirectMgr.Update(redirect) > 0)
                    {
                        json = "{success:true}";
                    }
                }
                else
                {
                    redirect.redirect_id = uint.Parse((_serialMgr.GetSerialById(4).Serial_Value + 1).ToString());
                    redirect.redirect_createdate = Convert.ToUInt32(CommonFunction.GetPHPTime());
                    redirect.redirect_updatedate = redirect.redirect_createdate;
                    if (_redirectMgr.Save(redirect) > 0)
                    {
                        Serial serial = new Serial();
                        serial.Serial_id = 4;
                        serial.Serial_Value = redirect.redirect_id;
                        _serialMgr.Update(serial);
                        json = "{success:true}";
                    }
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
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 連結管理報表button
        public HttpResponseBase GetRedirectClickCount()
        {
            _rcMgr = new RedirectClickMgr(mySqlConnectionString);
            RedirectClickQuery query = new RedirectClickQuery();
            if (!string.IsNullOrEmpty(Request.Params["selectDate"]))
            {
                string date = Request.Params["selectDate"];
                query.click_year = uint.Parse(date.Substring(0, 4));
                query.click_month = uint.Parse(date.Substring(5, 2));
            }
            if (!string.IsNullOrEmpty(Request.Params["redirect_id"]))
            {
                query.redirect_id = uint.Parse(Request.Params["redirect_id"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["group_id"]))
            {
                query.redirect_id_groupID = uint.Parse(Request.Params["group_id"]);
            }
            DataColumn coll;
            DataColumn col;
            DataTable days = new DataTable();
            coll = new DataColumn("datetype", typeof(string));
            col = new DataColumn("totalcount", typeof(string));
            days.Columns.Add(coll);
            days.Columns.Add(col);
            DataTable weeks = new DataTable();
            coll = new DataColumn("datetype", typeof(string));
            col = new DataColumn("totalcount", typeof(string));
            weeks.Columns.Add(coll);
            weeks.Columns.Add(col);
            DataTable hours = new DataTable();
            coll = new DataColumn("datetype", typeof(string));
            col = new DataColumn("totalcount", typeof(string));
            hours.Columns.Add(coll);
            hours.Columns.Add(col);
            //獲取指定時間的月的總天數
            for (int i = 1; i <= DateTime.DaysInMonth(int.Parse(query.click_year.ToString()), int.Parse(query.click_month.ToString())); i++)
            {
                DataRow dr = days.NewRow();
                DateTime tday = new DateTime(int.Parse(query.click_year.ToString()), int.Parse(query.click_month.ToString()), i);
                if (tday.DayOfWeek == DayOfWeek.Saturday || tday.DayOfWeek == DayOfWeek.Sunday)
                {
                    dr[0] = tday.ToString("yyyy年MM月dd日*");
                }
                else
                {
                    dr[0] = tday.ToString("yyyy年MM月dd日");
                }
                dr[1] = 0;
                days.Rows.Add(dr);
            }
            for (int i = 0; i < 7; i++)
            {
                DataRow dr = weeks.NewRow();
                dr[0] = i;
                dr[1] = 0;
                weeks.Rows.Add(dr);
            }
            for (int i = 0; i < 24; i++)
            {
                DataRow dr = hours.NewRow();
                dr[0] = i;
                dr[1] = 0;
                hours.Rows.Add(dr);
            }
            List<RedirectClickQuery> store = new List<RedirectClickQuery>();
            store = _rcMgr.GetRedirectClick(query);
            string json = string.Empty;
            try
            {
                foreach (var item in store)
                {
                    int dayindex = int.Parse(item.click_day.ToString()) - 1;
                    days.Rows[dayindex][1] = uint.Parse(days.Rows[dayindex][1].ToString()) + item.click_total;
                    int weekindex = int.Parse(item.click_week.ToString());
                    weeks.Rows[weekindex][1] = uint.Parse(weeks.Rows[weekindex][1].ToString()) + item.click_total;
                    int hourindex = int.Parse(item.click_hour.ToString());
                    hours.Rows[hourindex][1] = uint.Parse(hours.Rows[hourindex][1].ToString()) + item.click_total;

                }
                json = "{success:true,daydata:" + JsonConvert.SerializeObject(days, Formatting.Indented) + ",weekdata:" + JsonConvert.SerializeObject(weeks, Formatting.Indented) + ",hourdata:" + JsonConvert.SerializeObject(hours, Formatting.Indented) + "}";//返回json數據
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
        #region 連接點管理匯入
        public HttpResponseBase RedirectInto()
        {
            string json = string.Empty;
            DTExcels.Clear();
            DTExcels.Columns.Clear();
            string newName = string.Empty;
            DTExcels.Columns.Add("連結名稱", typeof(String));
            DTExcels.Columns.Add("目的連結", typeof(String));
            DTExcels.Columns.Add("連結狀態", typeof(String));//1表示正常2表示停用 其他都為正常
            int count = 0;//總匯入數
            int errorcount = 0;
            StringBuilder strsql = new StringBuilder();
            try
            {
                if (Request.Files["ImportFileMsg"] != null && Request.Files["ImportFileMsg"].ContentLength > 0)
                {
                    HttpPostedFileBase excelFile = Request.Files["ImportFileMsg"];
                    newName = Server.MapPath(excelPath) + excelFile.FileName;
                    excelFile.SaveAs(newName);
                    DataTable dt = new DataTable();
                    dt = CsvHelper.ReadCsvToDataTable(newName, true);
                    _redirectMgr = new RedirectMgr(mySqlConnectionString);
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            RedirectQuery rd = new RedirectQuery();
                            rd.group_id = Convert.ToUInt32(Request.Params["group_id"]);//得到群組
                            try
                            {
                                int linkstatus = 0;
                                if (!int.TryParse(dr[2].ToString(), out linkstatus))
                                {
                                    linkstatus = 1;
                                }
                                if (linkstatus == 2)
                                {
                                    linkstatus = 2;
                                    rd.redirect_name = dr[0].ToString();//連接名稱
                                    rd.redirect_url = dr[1].ToString();//目的連接
                                    rd.redirect_status = Convert.ToUInt32(linkstatus);//連接狀態
                                }
                                else
                                {
                                    linkstatus = 1;
                                    rd.redirect_name = dr[0].ToString();
                                    rd.redirect_url = dr[1].ToString();
                                    rd.redirect_status = Convert.ToUInt32(linkstatus);
                                }
                                rd.redirect_createdate = Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                                rd.redirect_updatedate = Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                                rd.redirect_ipfrom = CommonFunction.GetClientIP();
                                //獲取Serial裡面的redirect_id
                                _serialMgr = new SerialMgr(mySqlConnectionString);
                                rd.redirect_id = uint.Parse((_serialMgr.GetSerialById(4).Serial_Value + 1).ToString());
                            }
                            catch (Exception ex)
                            {//不是正常數據時候直接帶1(正常)
                                int linkstatus = 1;
                                rd.redirect_name = dr[0].ToString();
                                rd.redirect_url = dr[1].ToString();
                                rd.redirect_status = Convert.ToUInt32(linkstatus);
                            }
                            int result = 0;//如果獲取到的連結名稱和url為空就不新增
                            if (!string.IsNullOrEmpty(rd.redirect_name) && !string.IsNullOrEmpty(rd.redirect_url))//&& Regex.IsMatch(rd.redirect_url,'')
                            {
                                
                                result = _redirectMgr.EnterInotRedirect(rd);
                            }
                            if (result > 0)
                            {//插入成功一條并修改serial表數據
                                count++;
                                Serial serial = new Serial();
                                serial.Serial_id = 4;
                                serial.Serial_Value = rd.redirect_id;
                                _serialMgr.Update(serial);
                            }
                            else
                            {
                                DataRow drs = DTExcels.NewRow();
                                drs[0] = dr[0].ToString();
                                drs[1] = dr[1].ToString();
                                drs[2] = dr[2].ToString();
                                DTExcels.Rows.Add(drs);
                                errorcount++;//插入失敗一條
                            }
                            json = "{success:true,count:" + count + ",errorcount:" + errorcount + "}";
                        }
                    }
                    else
                    {
                        json = "{success:true,count:" + 0 + ",errorcount:" + 0 + "}";
                    }
                }
            }
            catch (Exception ex)
            {
                newName = string.Empty;
                DTExcels.Clear();
                DTExcels.Columns.Clear();
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:" + "" + "}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        //excel
        public HttpResponseBase RedirectUploadExcel()
        {
            string newName = string.Empty;
            string json = string.Empty;
            _redirectMgr = new RedirectMgr(mySqlConnectionString);
            try
            {
                DTExcels.Clear();
                DTExcels.Columns.Clear();
                DTExcels.Columns.Add("連結名稱", typeof(String));
                DTExcels.Columns.Add("目的連結", typeof(String));
                DTExcels.Columns.Add("連結狀態", typeof(String));//1表示正常2表示停用 其他都為正常
                int count = 0;//總匯入數
                int errorcount = 0;//數據異常個數
                int create_user = (Session["caller"] as Caller).user_id;
                if (Request.Files["ImportExcelFile"] != null && Request.Files["ImportExcelFile"].ContentLength > 0)
                {
                    HttpPostedFileBase excelFile = Request.Files["ImportExcelFile"];
                    newName = Server.MapPath(excelPath) + excelFile.FileName;
                    excelFile.SaveAs(newName);
                    DataTable dt = new DataTable();
                    NPOI4ExcelHelper helper = new NPOI4ExcelHelper(newName);
                    dt = helper.SheetData();
                    if (dt.Rows.Count > 0 && !string.IsNullOrEmpty(dt.Rows[0][0].ToString()) && !string.IsNullOrEmpty(dt.Rows[0][1].ToString()))
                    {
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (!string.IsNullOrEmpty(dr[0].ToString()) && !string.IsNullOrEmpty(dr[1].ToString()))
                                {
                                    #region 匯入數據
                                    RedirectQuery rd = new RedirectQuery();
                                    rd.group_id = Convert.ToUInt32(Request.Params["group_id"]);//得到群組
                                    try
                                    {
                                        int linkstatus = 0;
                                        if (!int.TryParse(dr[2].ToString(), out linkstatus))
                                        {
                                            linkstatus = 1;
                                        }
                                        if (linkstatus == 2)
                                        {
                                            linkstatus = 2;
                                            rd.redirect_name = dr[0].ToString();//連接名稱
                                            rd.redirect_url = dr[1].ToString();//目的連接
                                            rd.redirect_status = Convert.ToUInt32(linkstatus);//連接狀態
                                        }
                                        else
                                        {
                                            linkstatus = 1;
                                            rd.redirect_name = dr[0].ToString();
                                            rd.redirect_url = dr[1].ToString();
                                            rd.redirect_status = Convert.ToUInt32(linkstatus);
                                        }
                                        rd.redirect_createdate = Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                                        rd.redirect_updatedate = Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                                        rd.redirect_ipfrom = CommonFunction.GetClientIP();
                                        //獲取Serial裡面的redirect_id
                                        _serialMgr = new SerialMgr(mySqlConnectionString);
                                        rd.redirect_id = uint.Parse((_serialMgr.GetSerialById(4).Serial_Value + 1).ToString());
                                    }
                                    catch (Exception ex)
                                    {//不是正常數據時候直接帶1(正常)
                                        int linkstatus = 1;
                                        rd.redirect_name = dr[0].ToString();
                                        rd.redirect_url = dr[1].ToString();
                                        rd.redirect_status = Convert.ToUInt32(linkstatus);
                                    }
                                    int result = 0;//如果獲取到的連結名稱和url為空就不新增
                                    if (!string.IsNullOrEmpty(rd.redirect_name) && !string.IsNullOrEmpty(rd.redirect_url))//&& Regex.IsMatch(rd.redirect_url,'')
                                    {

                                        result = _redirectMgr.EnterInotRedirect(rd);
                                    }
                                    if (result > 0)
                                    {//插入成功一條并修改serial表數據
                                        count++;
                                        Serial serial = new Serial();
                                        serial.Serial_id = 4;
                                        serial.Serial_Value = rd.redirect_id;
                                        _serialMgr.Update(serial);
                                    }
                                    else
                                    {
                                        DataRow drs = DTExcels.NewRow();
                                        drs[0] = dr[0].ToString();
                                        drs[1] = dr[1].ToString();
                                        drs[2] = dr[2].ToString();
                                        DTExcels.Rows.Add(drs);
                                        errorcount++;//插入失敗一條
                                    }
                                    #endregion
                                }
                                json = "{success:true,count:" + count + ",errorcount:" + errorcount + "}";
                            }
                    }                
                    else
                    {
                        json = "{success:true,total:" + 0 + ",error:" + 0 + ",entercount:" + 0 + "}";
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:" + "" + "}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 連接點管理匯出
        public void RedirectExport()
        {
            string json = string.Empty;
            StringBuilder sb = new StringBuilder();
            string jsonStr = String.Empty;
            try
            {
                _redirectMgr = new RedirectMgr(mySqlConnectionString);
                RedirectQuery query = new RedirectQuery();
                DataTable dt = new DataTable();
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = uint.Parse(Request.Params["group_id"]);
                }
                dt = _redirectMgr.GetRedirectListCSV(query);
                DataTable dtCsv = new DataTable();
                string newExcelName = string.Empty;
                dtCsv.Columns.Add("群組", typeof(String));
                dtCsv.Columns.Add("編號", typeof(String));
                dtCsv.Columns.Add("連結名稱", typeof(String));
                dtCsv.Columns.Add("目的連結", typeof(String));
                dtCsv.Columns.Add("點閱次數", typeof(String)); 
                dtCsv.Columns.Add("狀態", typeof(String));
                dtCsv.Columns.Add("統計連結位置", typeof(String));
                //string[] colname = { "群組", "編號", "連結名稱", "目的連結", "點閱次數", "狀態", "統計連結位址" };
                string newName = string.Empty;
                string fileName = "連接點管理匯出" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                newName = Server.MapPath(excelPath) + fileName;
                foreach (DataRow dr in dt.Rows)
                {
                    dr["status_name"] = (dr["status"].ToString() == "1" ? "正常" : "停用");
                    dr["redirect"] = LinkPath+"/public/link.php?r=" + dr["redirect_id"].ToString();
                }
                dt.Columns.Remove("status");
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow drr = dtCsv.NewRow();
                    drr[0] = dr["group_name"];
                    drr[1] = dr["redirect_id"];
                    drr[2] = dr["redirect_name"];
                    drr[3] = dr["redirect_url"];
                    drr[4] = dr["redirect_total"];
                    drr[5] = dr["status_name"].ToString();
                    drr[6] = dr["redirect"];
                    dtCsv.Rows.Add(drr);
                }
                if (System.IO.File.Exists(newName))
                {
                    //設置文件的屬性，以防刪除文件的時候因為文件的屬性造成無法刪除
                    System.IO.File.SetAttributes(newName, FileAttributes.Normal);
                    System.IO.File.Delete(newName);
                }
                //CsvHelper.ExportDataTableToCsv(dt, newName, colname, true);

                StringWriter sw = ExcelHelperXhf.SetCsvFromData(dtCsv, "連結管理.csv");
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + "連結管理_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv");
                Response.ContentType = "application/ms-excel";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(sw);
                Response.End();
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:[]}";
            }
        }
        #endregion
        #region 未匯入的信息
        public void Updownmessage()
        {
            string json = string.Empty;
            try
            {
                StringWriter sw = ExcelHelperXhf.SetCsvFromData(DTExcels, "未匯入數據.csv");
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + "未匯入數據_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv");
                Response.ContentType = "application/ms-excel";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(sw);
                Response.End();
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:" + "" + "}";
            }
        }
        public void UpdownmessageExcel()
        {
            string json = string.Empty;
            try
            {
                string fileName = "未匯入數據_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                MemoryStream ms = ExcelHelperXhf.ExportDT(DTExcels, "未匯入數據");
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());
                //StringWriter sw = ExcelHelperXhf.ExportDT(DTExcels, "未匯入數據.xls");
                //Response.Clear();
                //Response.AddHeader("Content-Disposition", "attachment; filename=" + "未匯入數據_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
                //Response.ContentType = "application/ms-excel";
                //Response.ContentEncoding = Encoding.Default;
                //Response.Write(sw);
                //Response.End();
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:" + "" + "}";
            }
        }
        #endregion
        #region 匯出csv模板
        public void IplasUpdownTemplate()
        {
            string newCsvName = string.Empty;
            DataTable dt = new DataTable(); 
            try
            {
                dt.Columns.Add("連結名稱", typeof(String));
                dt.Columns.Add("目的連結", typeof(String));
                dt.Columns.Add("連結狀態", typeof(String));
                string fileName = "連結管理匯入模板_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                newCsvName = Server.MapPath(excelPath) + fileName;
                if (System.IO.File.Exists(newCsvName))
                {
                    System.IO.File.SetAttributes(newCsvName, FileAttributes.Normal);
                    System.IO.File.Delete(newCsvName);
                }
                StringWriter sw = ExcelHelperXhf.SetCsvFromData(dt, "連結管理模板.csv");
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + "連結管理模板_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv");
                Response.ContentType = "application/text";
                Response.ContentEncoding = Encoding.UTF8;
                Response.Write(sw);
                Response.End();
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
        }
        //Excel
        public void RedirectUpdownTemplate()
        {
            string json = string.Empty;
            DataTable dtTemplateExcel = new DataTable();
            try
            {
                dtTemplateExcel.Columns.Add("連結名稱", typeof(String));
                dtTemplateExcel.Columns.Add("目的連結", typeof(String));
                dtTemplateExcel.Columns.Add("連結狀態", typeof(String));
                DataRow newRow = dtTemplateExcel.NewRow();
                dtTemplateExcel.Rows.Add(newRow);
                string fileName = "連結管理匯入模板_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                MemoryStream ms = ExcelHelperXhf.ExportDT(dtTemplateExcel, "");//"條碼維護匯入模板_" + DateTime.Now.ToString("yyyyMMddHHmmss")
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:" + "" + "}";
            }
        }
        #endregion

        public HttpResponseBase GetGroupType()
        {
            List<Parametersrc> stores = new List<Parametersrc>();
            string json = string.Empty;
            _parametersrc = new ParameterMgr(mySqlConnectionString);
            try
            {
                stores = _parametersrc.GetElementType("group_type");
                //Parametersrc Dmodel = new Parametersrc();
                //Dmodel.ParameterCode = "0";
                //Dmodel.parameterName = "--請選擇--";
                //stores.Insert(0, Dmodel);//使會員群組的第一行為不分
                json = "{success:true,data:" + JsonConvert.SerializeObject(stores) + "}";//返回json數據
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

    }
}
