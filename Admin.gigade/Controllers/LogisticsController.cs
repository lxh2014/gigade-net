using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace Admin.gigade.Controllers
{
    public class LogisticsController : Controller
    {
        //
        // GET: /Logistics/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        public BLL.gigade.Model.Query.ShippingCarriorQuery _shippingcarriorModel;
        public IShippingCarriorImplMgr _ishippingcarriorMgr;

        #region view
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LogisticsList()
        {
            return View();
        }
        #endregion

        #region 加載列表
        public HttpResponseBase LoadList()
        {
            string json = string.Empty;
            _ishippingcarriorMgr = new ShippingCarriorMgr(mySqlConnectionString);
            _shippingcarriorModel = new ShippingCarriorQuery();
            int totalCount=0;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["searchcontent"]))
                {
                    _shippingcarriorModel.Delivery_store_id =int.Parse(Request.Params["searchcontent"].ToString());
                }
                _shippingcarriorModel.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                _shippingcarriorModel.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
               DataTable dt=  _ishippingcarriorMgr.GetShippingCarriorList(_shippingcarriorModel,out totalCount);
               json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(dt, Formatting.Indented) + "}";
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

        #region //editcombobox
        public HttpResponseBase GetLogisticsName()
        {
            string json = string.Empty;
            _ishippingcarriorMgr = new ShippingCarriorMgr(mySqlConnectionString);
            Parametersrc _parametersecModel = new Parametersrc();
            try
            {
                string name=string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["ltype"]))
                {
                    name = Request.Params["ltype"].ToString();
                }
                _parametersecModel.ParameterType = "Deliver_Store";
                DataTable dt = _ishippingcarriorMgr.GetLogisticsName(_parametersecModel,name);
                json = "{success:true,data:" + JsonConvert.SerializeObject(dt, Formatting.Indented) + "}";
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

        public HttpResponseBase GetLogisticsArea()
        {
            string json = string.Empty;
            _ishippingcarriorMgr = new ShippingCarriorMgr(mySqlConnectionString);
            Parametersrc _parametersecModel = new Parametersrc();
            try
            {
                string name = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["ltype"]))
                {
                    name = Request.Params["ltype"].ToString();
                }
                _parametersecModel.ParameterType = "freight_big_area";
                DataTable dt = _ishippingcarriorMgr.GetLogisticsName(_parametersecModel,name);
                json = "{success:true,data:" + JsonConvert.SerializeObject(dt, Formatting.Indented) + "}";
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
        public HttpResponseBase GetLogisticsType()
        {
            string json = string.Empty;
            _ishippingcarriorMgr = new ShippingCarriorMgr(mySqlConnectionString);
            Parametersrc _parametersecModel = new Parametersrc();
            try
            {
                string name = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["ltype"]) && Request.Params["ltype"]!="0")
                {
                    name = Request.Params["ltype"].ToString();
                }
                _parametersecModel.ParameterType = "freight_type";
                DataTable dt = _ishippingcarriorMgr.GetLogisticsName(_parametersecModel,name);
                json = "{success:true,data:" + JsonConvert.SerializeObject(dt, Formatting.Indented) + "}";
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

        #region 物流保存
        public HttpResponseBase LogisticsSave()
        {
            string json = string.Empty;
            _ishippingcarriorMgr = new ShippingCarriorMgr(mySqlConnectionString);
            _shippingcarriorModel = new ShippingCarriorQuery();
            try{
                if (!string.IsNullOrEmpty(Request.Params["rid"]))
                {
                    _shippingcarriorModel.Rid = int.Parse(Request.Params["rid"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["dsid"]))
                {
                    _shippingcarriorModel.Delivery_store_id = int.Parse(Request.Params["dsid"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["fbarea"]))
                {
                    _shippingcarriorModel.Freight_big_area = int.Parse(Request.Params["fbarea"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["ftype"]))
                {
                    _shippingcarriorModel.Freight_type = int.Parse(Request.Params["ftype"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["dfset"]))
                {
                    _shippingcarriorModel.Delivery_freight_set = int.Parse(Request.Params["dfset"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["slimit"]))
                {
                    _shippingcarriorModel.Size_limitation = int.Parse(Request.Params["slimit"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["length"]))
                {
                    _shippingcarriorModel.Length = int.Parse(Request.Params["length"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["width"]))
                {
                    _shippingcarriorModel.Width = int.Parse(Request.Params["width"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["height"]))
                {
                    _shippingcarriorModel.Height = int.Parse(Request.Params["height"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["weight"]))
                {
                    _shippingcarriorModel.Weight = int.Parse(Request.Params["weight"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["note"]))
                {
                    _shippingcarriorModel.Note = Request.Params["note"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["pod"]))
                {
                    _shippingcarriorModel.Pod = int.Parse(Request.Params["pod"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["ctype"]))
                {
                    _shippingcarriorModel.Charge_type = int.Parse(Request.Params["ctype"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["sfree"]))
                {
                    _shippingcarriorModel.Shipping_fee = int.Parse(Request.Params["sfree"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["rfree"]))
                {
                    _shippingcarriorModel.Return_fee = int.Parse(Request.Params["rfree"].ToString());
                }
                if (_shippingcarriorModel.Size_limitation == 0)
                {
                    _shippingcarriorModel.Width = 0;
                    _shippingcarriorModel.Height = 0;
                    _shippingcarriorModel.Length = 0;
                    _shippingcarriorModel.Weight = 0;
                }
                if (_shippingcarriorModel.Rid>0)
                {
                    _ishippingcarriorMgr.LogisticsUpdate(_shippingcarriorModel);
                    json = "{success:true,res:1}";
                }
                else
                {
                    _shippingcarriorModel.Active = 1;
                    int count = _ishippingcarriorMgr.LogisticsAddCheck(_shippingcarriorModel);
                    if (count > 0)
                    {
                        json = "{success:true,res:0}";
                    }
                    else
                    {
                        _ishippingcarriorMgr.LogisticsSave(_shippingcarriorModel);
                        json = "{success:true,res:1}";
                    }
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
        #region 列表刪除
        public HttpResponseBase DeleteShippingCarriorById()
        {
            string json = string.Empty;
            _ishippingcarriorMgr = new ShippingCarriorMgr(mySqlConnectionString);
            _shippingcarriorModel = new ShippingCarriorQuery();
            try{
                string rid = Request.Params["rid"];
                string[] rids = rid.Split(',');
                for (int i = 0; i < rids.Length - 1; i++)
                {
                    _shippingcarriorModel.Rid = int.Parse(rids[i].ToString());
                    _ishippingcarriorMgr.Delete(_shippingcarriorModel.Rid.ToString());
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
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 更改狀態
        public JsonResult UpdateActive()
        {
            _ishippingcarriorMgr = new ShippingCarriorMgr(mySqlConnectionString);
            _shippingcarriorModel = new ShippingCarriorQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    _shippingcarriorModel.Rid = int.Parse(Request.Params["id"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["active"]))
                {
                    _shippingcarriorModel.Active = int.Parse(Request.Params["active"].ToString());
                }
               int res= _ishippingcarriorMgr.LogisticsUpdateActive(_shippingcarriorModel);

            if (res > 0)
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
