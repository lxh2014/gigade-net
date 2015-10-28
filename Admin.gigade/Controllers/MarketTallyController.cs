using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Admin.gigade.Controllers
{
    public class MarketTallyController : Controller
    {
        //
        // GET: /MarketTally/
        IinvdImplMgr _iinvd;
        IAseldImplMgr _iasdMgr;
        IAseldMasterImplMgr _aseldmasterMgr;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();


        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MarketTally()
        {
            return View();
        }
        public ActionResult MarketTallyWD()
        {
            ViewBag.number = Request.Params["number"];
            return View();
        }
        public ActionResult MarketTallyWDProduct()
        {
            ViewBag.number = Request.Params["number"];
            ViewBag.upc = Request.Params["upc"];
            return View();
        }

        //通過工作代號判斷在表中是否存在
        public HttpResponseBase GetAseldMasterAssgCount()
        {
            string json = string.Empty;
            Aseld ase = new Aseld();
            AseldMaster aseMaster = new AseldMaster();
            int count = 0;
            try
            {
                _aseldmasterMgr = new AseldMasterMgr(mySqlConnectionString);
                if (!String.IsNullOrEmpty(Request.Params["number"].ToString().Trim()))//如果是新增
                {
                    aseMaster.assg_id = Request.Params["number"].ToString().Trim();
                    ase.assg_id = aseMaster.assg_id;
                }
                ase.change_dtim = DateTime.Now;
                ase.change_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                count = _aseldmasterMgr.SelectCount(aseMaster);
                _iasdMgr = new AseldMgr(mySqlConnectionString);
                if (count > 0)
                {//輸入的項目編號裡面有商品需要揀貨，把aseld裡面的scaned欄位初始化為0（主要在第二次揀貨時）
                    _iasdMgr.UpdScaned(ase);
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,data:" + count + "}";//返回json數據
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

        //根據工作代號、產品條碼 獲取商品數據
        public HttpResponseBase GetAseldByUpc()
        {//判斷寄倉或者調度
            string json = String.Empty;
            
            Aseld m = new Aseld();
            List<AseldQuery> list = new List<AseldQuery>();
            _iasdMgr = new AseldMgr(mySqlConnectionString);
            try
            {

                m.assg_id = Request.Params["assg_id"].ToString().Trim();
                m.upc_id = Request.Params["upc_id"].ToString().Trim();
                list = _iasdMgr.GetAseldListByUpc(m);
                foreach (var item in list)
                {
                    m.seld_id = item.seld_id;
                }
                m.wust_id = "BSY";
                m.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                _iasdMgr.Updwust(m);
                json = "{success:true,totalCount:"+list.Count+",data:" + JsonConvert.SerializeObject(list, Formatting.Indented) + "}";//返回json數據              
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:0}";
            }
            this.Response.Clear();
            this.Response.Write(json.ToString());
            this.Response.End();
            return this.Response;
        }

        //理货员工作--寄仓--庫存信息
        public HttpResponseBase GetStockByUpc()
        {
            string json = string.Empty;
            int totalCount = 0;
            int islock = 0;
            _iinvd = new IinvdMgr(mySqlConnectionString);
            IinvdQuery query = new IinvdQuery()
            {
                ista_id = "A"
            };
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["upc_id"].ToString().Trim()))
                {

                    List<IinvdQuery> listIinvdQuery = _iinvd.GetIinvdListByUpc(query, Request.Params["upc_id"].ToString().Trim(), out totalCount);
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                    //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                    //timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                    timeConverter.DateTimeFormat = "yyyy-MM-dd";
                    if (totalCount > listIinvdQuery.Count)
                    {
                        islock = 1;
                    }
                    //實際能檢的庫存listIinvdQuery.Count
                    if (listIinvdQuery.Count > 0)
                    {
                        json = JsonConvert.SerializeObject(listIinvdQuery, Formatting.Indented, timeConverter) ;//返回json數據
                    }
                    else
                    {
                        IinvdQuery m = new IinvdQuery();
                        m.prod_qty = 0;
                        m.made_date = DateTime.Now;
                        m.cde_dt = DateTime.Now;
                        listIinvdQuery.Add(m);
                        json = "{success:true,islock:'" + islock + "',totalCount:" + listIinvdQuery.Count + ",data:" + JsonConvert.SerializeObject(listIinvdQuery, Formatting.Indented, timeConverter) + "}";//返回json數據
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

    }
}
