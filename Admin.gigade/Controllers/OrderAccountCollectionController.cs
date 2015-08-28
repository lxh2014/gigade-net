using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Mgr;
using BLL.gigade.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Admin.gigade.Controllers
{
    public class OrderAccountCollectionController : Controller
    {
        //
        // GET: /OrderAccountCollection/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private OrderAccountCollectionMgr _OrderAccCollectMgr;
        public HttpResponseBase GetOrderAccountCollectionList()
        {
            string jsonStr = string.Empty;

            try
            {
                OrderAccountCollection query = new OrderAccountCollection();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Params["searchcontent"]))
                {
                    query.order_id = uint.Parse(Request.Params["searchcontent"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                int totalCount = 0;
                _OrderAccCollectMgr = new OrderAccountCollectionMgr(mySqlConnectionString);

                DataTable _dt = _OrderAccCollectMgr.GetOrderAccountCollectionList(query, out totalCount);

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

        public HttpResponseBase SaveOrEdit()
        {
            string jsonStr = String.Empty;
            int result = 0;
            try
            {
                OrderAccountCollection orderCollection = new OrderAccountCollection();
                _OrderAccCollectMgr = new OrderAccountCollectionMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["row_id"]))//表示為編輯
                {
                    orderCollection.row_id = Convert.ToInt32(Request.Params["row_id"]);
                }
                orderCollection.order_id = Convert.ToUInt32(Request.Params["order_id"]);
                if (!string.IsNullOrEmpty(Request.Params["account_collection_time"]))
                {
                    orderCollection.account_collection_time = Convert.ToDateTime(Request.Params["account_collection_time"]);
                } if (!string.IsNullOrEmpty(Request.Params["account_collection_money"]))
                {
                    orderCollection.account_collection_money = Convert.ToInt32(Request.Params["account_collection_money"]);
                }

                //orderCollection.amount_account_collection_money = Convert.ToInt32(Request.Params["amount_account_collection_money"]);
                //orderCollection.amount_invoice = Convert.ToInt32(Request.Params["amount_invoice"]);
                if (!string.IsNullOrEmpty(Request.Params["poundage"]))
                {
                    orderCollection.poundage = Convert.ToInt32(Request.Params["poundage"]);
                } if (!string.IsNullOrEmpty(Request.Params["return_collection_time"]))
                {
                    orderCollection.return_collection_time = Convert.ToDateTime(Request.Params["return_collection_time"]);
                } if (!string.IsNullOrEmpty(Request.Params["return_collection_money"]))
                {
                    orderCollection.return_collection_money = Convert.ToInt32(Request.Params["return_collection_money"]);
                } if (!string.IsNullOrEmpty(Request.Params["return_poundage"]))
                {
                    orderCollection.return_poundage = Convert.ToInt32(Request.Params["return_poundage"]);
                }
                orderCollection.remark = Request.Params["remark"];

                result = _OrderAccCollectMgr.SaveOrEdit(orderCollection);
                if (result > 0)
                {
                    jsonStr = "{success:true}";
                }
                else
                {
                    jsonStr = "{success:false}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }


        public HttpResponseBase Delete()
        {
            string jsonStr = String.Empty;
            int result = 0;
            string str_row_id = string.Empty;
            try
            {
                OrderAccountCollection orderCollection = new OrderAccountCollection();
                _OrderAccCollectMgr = new OrderAccountCollectionMgr(mySqlConnectionString);
                str_row_id = Request.Params["rid"];
                str_row_id = str_row_id.Substring(0, str_row_id.LastIndexOf(','));
                result = _OrderAccCollectMgr.delete(str_row_id);
                if (result > 0)
                {
                    jsonStr = "{success:true}";
                }
                else
                {
                    jsonStr = "{success:false}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase YesOrNoOrderId()
        {
            string jsonStr = String.Empty;
            int result = 0;
            int results = 0;
            string order_id = string.Empty;
            try
            {
                OrderAccountCollection orderCollection = new OrderAccountCollection();
                _OrderAccCollectMgr = new OrderAccountCollectionMgr(mySqlConnectionString);
                order_id = Request.Params["order_id"];

                result = _OrderAccCollectMgr.YesOrNoOrderId(order_id);
                if (result > 0)
                {
                    results = _OrderAccCollectMgr.IncludeOrderId(order_id);
                    if (results > 0)//表示表中已經存在
                    {
                        jsonStr = "{success:true,msg:1}";
                    }
                    else
                    {
                        jsonStr = "{success:true,msg:0}";
                    }
                }
                else
                {
                    jsonStr = "{success:false}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }

    }
}
