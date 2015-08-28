using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Data;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Common;
using System.IO;
using gigadeExcel.Comment;
using System.Text.RegularExpressions;

namespace Admin.gigade.Controllers
{
    public class ProductClickController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private ProductClickMgr pcMgr;
        public ActionResult Index()
        {
            return View();
        }

        public HttpResponseBase GetProductClickList()
        {
            string json = string.Empty;
            try
            {
                ProductClickQuery query = new ProductClickQuery();
                pcMgr = new ProductClickMgr(mySqlConnectionString);
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["product_status"]))
                {
                    query.product_status = uint.Parse(Request.Params["product_status"]);
                }
                //支持空格，中英文逗號隔開
                if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                {
                    //query.pids = Request.Params["product_id"].Replace(" ", ",");
                    query.pids=Regex.Replace(Request.Params["product_id"].Trim(), "(\\s+)|(，)|(\\,)", ",");
                    //query.pids = Request.Params["product_id"].Replace("|", ",");
                }
                if (!string.IsNullOrEmpty(Request.Params["brand_id"]))
                {
                    query.brand_id = uint.Parse(Request.Params["brand_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["prod_classify"]))
                {
                    query.prod_classify = uint.Parse(Request.Params["prod_classify"]);
                }
                //if (!string.IsNullOrEmpty(Request.Params["startdate"]))
                //{
                //    query.sclick_year = int.Parse(Request.Params["startdate"].Substring(0, 4));
                //    query.sclick_month = int.Parse(Request.Params["startdate"].Substring(5, 2));
                //    query.sclick_day = int.Parse(Request.Params["startdate"].Substring(8, 2));
                //}
                //if (!string.IsNullOrEmpty(Request.Params["enddate"]))
                //{
                //    query.eclick_year = int.Parse(Request.Params["enddate"].Substring(0, 4));
                //    query.eclick_month = int.Parse(Request.Params["enddate"].Substring(5, 2));
                //    query.eclick_day = int.Parse(Request.Params["enddate"].Substring(8, 2));
                //}
                switch (Request.Params["type"])
                {
                    case "b":
                        break;
                    case "y":
                        query.click_year = 1;
                        break;
                    case "m":
                        query.click_month = 1;
                        break;
                    case "d":
                        query.click_day = 1;
                        break;
                }
                if (!string.IsNullOrEmpty(Request.Params["startdate"]))
                {
                    query.sclick_id = uint.Parse(Request.Params["startdate"].Substring(0, 10).Replace("-", "") + "00");
                }
                if (!string.IsNullOrEmpty(Request.Params["enddate"]))
                {
                    query.eclick_id = uint.Parse(Request.Params["enddate"].Substring(0, 10).Replace("-", "") + "23");
                }
                int totalCount = 0;
                DataTable dt = pcMgr.GetProductClickList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(dt, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        public void OutProductClickExcel()
        {
            string json = string.Empty;
            try
            {
                ProductClickQuery query = new ProductClickQuery();
                pcMgr = new ProductClickMgr(mySqlConnectionString);
                query.IsPage = false;
                if (Request.Params["product_status"] != "null" && !string.IsNullOrEmpty(Request.Params["product_status"]))
                {
                    query.product_status = uint.Parse(Request.Params["product_status"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                {
                    //支持空格，中英文逗號隔開
                    if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                    {
                        query.pids = Regex.Replace(Request.Params["product_id"].Trim(), "(\\s+)|(，)|(\\,)", ",");
                    }
                }
                if (Request.Params["brand_id"] != "null" && !string.IsNullOrEmpty(Request.Params["brand_id"]))
                {
                    query.brand_id = uint.Parse(Request.Params["brand_id"]);
                }
                if (Request.Params["prod_classify"] != "null" && !string.IsNullOrEmpty(Request.Params["prod_classify"]))
                {
                    query.prod_classify = uint.Parse(Request.Params["prod_classify"]);
                }
                //if (!string.IsNullOrEmpty(Request.Params["startdate"]))
                //{
                //    query.sclick_year = int.Parse(Request.Params["startdate"].Substring(0, 4));
                //    query.sclick_month = int.Parse(Request.Params["startdate"].Substring(5, 2));
                //    query.sclick_day = int.Parse(Request.Params["startdate"].Substring(8, 2));
                //}
                //if (!string.IsNullOrEmpty(Request.Params["enddate"]))
                //{
                //    query.eclick_year = int.Parse(Request.Params["enddate"].Substring(0, 4));
                //    query.eclick_month = int.Parse(Request.Params["enddate"].Substring(5, 2));
                //    query.eclick_day = int.Parse(Request.Params["enddate"].Substring(8, 2));
                //}
                switch (Request.Params["type"])
                {
                    case "b":
                        break;
                    case "y":
                        query.click_year = 1;
                        break;
                    case "m":
                        query.click_month = 1;
                        break;
                    case "d":
                        query.click_day = 1;
                        break;
                }
                if (!string.IsNullOrEmpty(Request.Params["startdate"]))
                {
                    query.sclick_id = uint.Parse(Request.Params["startdate"].Substring(0, 10).Replace("-", "") + "00");
                }
                if (!string.IsNullOrEmpty(Request.Params["enddate"]))
                {
                    query.eclick_id = uint.Parse(Request.Params["enddate"].Substring(0, 10).Replace("-", "") + "23");
                }
                int totalCount = 0;
                DataTable dt = pcMgr.GetProductClickList(query, out totalCount);
                dt.Columns["product_id"].ColumnName = "商品編號";
                dt.Columns["product_name"].ColumnName = "商品名稱";
                dt.Columns["brand_name"].ColumnName = "品牌名稱";
                dt.Columns["prod_classify"].ColumnName = "商品館別";
                dt.Columns["click_year"].ColumnName = "年";
                dt.Columns["click_month"].ColumnName = "月";
                dt.Columns["click_day"].ColumnName = "日";
                dt.Columns["click_total"].ColumnName = "點擊次數"; 
                switch (Request.Params["type"])
                {
                    case "b":
                        break;
                    case "y":
                        dt.Columns.Remove("月");
                        dt.Columns.Remove("日");
                        break;
                    case "m":
                        dt.Columns.Remove("年");
                        dt.Columns.Remove("日");
                        break;
                    case "d":
                        dt.Columns.Remove("年");
                        dt.Columns.Remove("月");
                        break;
                }               
                //dt.DefaultView.ToTable(false, new string[] { "product_id", "product_name", "brand_name_simple", "prod_classify" });              
                string fileName = "商品點擊次數統計_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                MemoryStream ms = ExcelHelperXhf.ExportDT(dt, "");
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
        }
    }
}
