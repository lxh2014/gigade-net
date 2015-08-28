using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text.RegularExpressions;
using BLL.gigade.Common;

namespace Admin.gigade.Controllers
{
    public class ProductHistoryController : Controller
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];

        private ITableHistoryImplMgr _tableHistoryMgr;
        private ITableHistoryItemImplMgr _tableHistoryItemMgr;

        //
        // GET: /ProductHistory/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 獲取表明
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public ActionResult GetTableName()
        {
            string json = string.Empty;
            try
            {
                _tableHistoryMgr = new TableHistoryMgr(connectionString);
                return Json(_tableHistoryMgr.QueryTableName().Select(m => new { m.table_name }));
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return new EmptyResult();
            }
        }


        /// <summary>
        /// 歷史記錄列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public ActionResult GetHistory(TableHistory th)
        {
            List<TableHistoryCustom> list = new List<TableHistoryCustom>();
            IsoDateTimeConverter iso = new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            try
            {
                int total = 0;
                var Start = int.Parse(Request.Form["start"] ?? "0");
                if (Request["limit"] != null)
                {
                    var Limit = Convert.ToInt32(Request["limit"]);
                }
                _tableHistoryMgr = new TableHistoryMgr(connectionString);
                list =  _tableHistoryMgr.GetHistoryByCondition(th, out total);
                return Content("{succes:true,totalCount:" + total + ",item:" + JsonConvert.SerializeObject(list, Formatting.None, iso) + "}");
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return new EmptyResult();
            }
        }


        /// <summary>
        /// 歷史記錄詳情
        /// </summary>
        /// <param name="batchno"></param>
        /// <returns></returns>
        public ActionResult HistoryDetails(string batchno)
        {
            _tableHistoryItemMgr = new TableHistoryItemMgr(connectionString);
            var items = _tableHistoryItemMgr.GetHistoryInfoByConditon(new TableHistory { batchno = batchno });
            /*匹配十位整數正則表達式          
            判斷是否是時間戳格式,是的話轉換為時間格式
            不是的話不按原先值返回 add by wwei0216w 2015/7/8*/
            Regex reg = new Regex("^\\d{10}$");
            foreach (var a in items)
            {
                foreach (var b in a.historyItem)
                {
                    if (reg.IsMatch(b.col_value) && reg.IsMatch(b.old_value))
                    {
                        try
                        {
                            b.col_value = CommonFunction.GetNetTime(Convert.ToInt64(b.col_value)).ToString("yyyy/MM/dd HH:mm:ss");
                            b.old_value = CommonFunction.GetNetTime(Convert.ToInt64(b.old_value)).ToString("yyyy/MM/dd HH:mm:ss");
                        }
                        catch { }
                    }
                }

            }
            return Json(items);
        }


        /// <summary>
        /// 商品細項&價格管理記錄
        /// </summary>
        /// <returns></returns>
        public ActionResult ParticularsHistory()
        {
            return View();
        }


        /// <summary>
        /// 查詢商品細項歷史記錄
        /// </summary>
        /// <returns></returns>
        public ActionResult GetParticularsHistory(string productID_5, string productID_6, string time_start, string time_end, int brand_id = 0)
        {
            int[] id_5 = (from i in productID_5.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) select int.Parse(i)).ToArray();
            int[] id_6 = (from i in productID_6.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) select int.Parse(i)).ToArray();
            string productIds = string.Join(",", id_5);
            string itemIds = string.Join(",", id_6);
            Int64 timeStartPHP = time_start == "" ? 0 : Convert.ToInt64(time_start);
            Int64 timeEndPHP = time_end == "" ? 0 : Convert.ToInt64(time_end);
            try
            {
                IProductExtImplMgr _prodcutExt = new ProductExtMgr(connectionString);
                return Json(_prodcutExt.QueryHistoryInfo(timeStartPHP, timeEndPHP, brand_id, itemIds, productIds));
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return new EmptyResult();
            }
        }


        /// <summary>
        /// 查詢商品價格歷史記錄
        /// </summary>
        /// <returns></returns>
        //public ActionResult GetPricesHistory(string productID_5, string productID_6, string time_start, string time_end)
        //{
        //    int[] id_5 = (from i in productID_5.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) select int.Parse(i)).ToArray();
        //    int[] id_6 = (from i in productID_6.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) select int.Parse(i)).ToArray();
        //    try
        //    {
        //        IProductExtImplMgr _prodcutExt = new ProductExtMgr(connectionString);
        //        return Json(_prodcutExt.QueryHistoryInfo(new ProductExtCustom { Product_id = Convert.ToUInt32(id_5), Item_id = Convert.ToUInt32(id_6) }));
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        return new EmptyResult();
        //    }
        //}


        /// <summary>
        /// 歷史記錄匯出方法
        /// </summary>
        /// <returns></returns>
        public ActionResult HistoryExcel(string productID_5, string productID_6, string time_start, string time_end, int brand_id = 0)
        {
            try
            {
                string xmlPath = "../XML/ProductItemHistory.xml";

                IProductExtImplMgr _productExtMgr = new ProductExtMgr(connectionString);
                ProductExtCustom pe = new ProductExtCustom { Product_id = 12306 };

                int[] id_5 = (from i in productID_5.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) select int.Parse(i)).ToArray();
                int[] id_6 = (from i in productID_6.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) select int.Parse(i)).ToArray();
                string productIds = string.Join(",", id_5);
                string itemIds = string.Join(",", id_6);

                MemoryStream ms = _productExtMgr.OutToExcel(Server.MapPath(xmlPath), Convert.ToInt64(time_start), Convert.ToInt64(time_end), brand_id, itemIds, productIds);
                if (ms == null)
                {
                    return new EmptyResult();
                }
                return File(ms.ToArray(), "application/-excel", DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls");
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return new EmptyResult();
            }
        }



    }
}
