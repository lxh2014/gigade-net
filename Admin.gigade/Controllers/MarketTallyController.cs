using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
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
        IIialgImplMgr _iagMgr;
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

        //RF直接庫調
        public HttpResponseBase RFKT()
        {
            string json = string.Empty;
            IialgQuery q = new IialgQuery();
            uint id = 0; DateTime dt = new DateTime(); int sun = 0;
            try
            {
                if (uint.TryParse(Request.Params["item_id"].ToString(), out id))
                {//商品id
                    q.item_id = id;
                }
                if (DateTime.TryParse(Request.Params["made_date"].ToString(), out dt))
                {//商品製造日期
                    q.made_dt = dt;
                }
                if (int.TryParse(Request.Params["prod_qty"].ToString(), out sun))
                {//商品原有數量
                    q.qty_o = sun;
                }
                if (int.TryParse(Request.Params["pnum"].ToString(), out sun))
                {//商品撿貨數量
                    q.pnum = sun;
                }
                if (!string.IsNullOrEmpty(Request.Params["loc_id"].ToString()))
                {//商品撿貨數量
                    q.loc_id = Request.Params["loc_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["order_id"]))
                {
                    q.order_id = Request.Params["order_id"];
                }
                q.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                //進行庫調
                _iagMgr = new IialgMgr(mySqlConnectionString);
                if (q.loc_id == "YY999999")
                {
                    json = "{success:false}";
                }
                else
                {
                    int result = _iagMgr.addIialgIstock(q);
                    if (result == 2)
                    {
                        json = "{success:true,msg:2}";
                    }
                    if (result == 100)
                    {
                        json = "{success:true,msg:100}";
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

        #region 輸入實際揀貨量后操作--寄倉+調度
        [CustomHandleError]
        public HttpResponseBase GETMarkTallyWD()
        {
            StringBuilder sb = new StringBuilder();
            string json = String.Empty;
            AseldQuery m = new AseldQuery();
            List<AseldQuery> list = new List<AseldQuery>();
            _iasdMgr = new AseldMgr(mySqlConnectionString);
            int flag = 2;
            int try1 = 0;
            try
            {
                m.seld_id = Int32.Parse(Request.Params["seld_id"]);//aseld的流水號
                m.commodity_type = Request.Params["commodity_type"];//獲取寄倉2和調度3
                m.ord_qty = Int32.Parse(Request.Params["ord_qty"]);//需要訂貨數量
                if (Int32.TryParse(Request.Params["act_pick_qty"], out try1))
                {
                    m.act_pick_qty = Int32.Parse(Request.Params["act_pick_qty"]);
                }
                else
                {
                    m.act_pick_qty = 0;
                }
                m.item_id = uint.Parse(Request.Params["item_id"]);
                m.out_qty = Int32.Parse(Request.Params["out_qty"]) - m.act_pick_qty;//缺貨數量
                m.act_pick_qty = m.ord_qty - m.out_qty;
                m.complete_dtim = DateTime.Now;
                m.assg_id = Request.Params["assg_id"];
                m.ord_id = Int32.Parse(Request.Params["ord_id"]);
                m.ordd_id = int.Parse(Request.Params["ordd_id"]);//商品細項編號。操作iwms_record需要
                m.change_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                m.deliver_code = Request.Params["deliver_id"];
                m.deliver_id = int.Parse(m.deliver_code.Substring(1, m.deliver_code.Length - 1).ToString());
                if (m.out_qty == 0)
                {//揀完了,判斷缺貨數量是否為0
                    m.wust_id = "COM";
                }
                else
                {//沒拿夠貨物
                    m.wust_id = "SKP";
                }
                sb.Append(_iasdMgr.UpdAseld(m));
                if (m.commodity_type == "2")
                {
                    #region 寄倉--對庫存進行操作
                    Dictionary<string, string> dickuCun = new Dictionary<string, string>();
                    if (Int32.TryParse(Request.Params["act_pick_qty"], out try1))
                    {
                        if (!string.IsNullOrEmpty(Request.Params["pickRowId"]))
                        {
                            string[] iinvd = Request.Params["pickRowId"].Split(',');
                            string[] pick = Request.Params["pickInfo"].Split(',');
                            for (int i = 0; i < iinvd.Length; i++)
                            {
                                if (!dickuCun.Keys.Contains(iinvd[i]))
                                {
                                    dickuCun.Add(iinvd[i], pick[i]);
                                }
                                else
                                {
                                    dickuCun[iinvd[i]] = pick[i];
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(_iasdMgr.updgry(m, dickuCun)))
                    {
                        sb.Append(_iasdMgr.updgry(m, dickuCun));
                    }
                    if (!string.IsNullOrEmpty(sb.ToString()))
                    {
                        _iasdMgr.InsertSql(sb.ToString());//執行SQL語句裡面有事物處理
                    }
                    int ord = 1;
                    int can = 0;
                    #region  判斷項目狀態
                    if (_iasdMgr.SelCom(m) == 0)
                    {
                        ord = 0;//訂單揀貨完成，可以封箱
                    }
                    if (_iasdMgr.SelComA(m) == 0)
                    {
                        flag = 0;//項目訂單揀貨完成
                    }
                    if (ord == 0)
                    {//有沒有臨時取消的商品
                        if (_iasdMgr.SelComC(m) > 0)
                        {
                            can = 1;
                        }
                    }
                    #endregion
                    json = "{success:true,qty:'" + m.out_qty + "',flag:'" + flag + "',ord:'" + ord + "',can:'" + can + "'}";//返回json數據  
                    //qty 該物品是否缺貨，如果為零揀貨完成，否則彈框提示缺貨數量。
                    //over：0表示該訂單已經揀貨完畢，如果qty為零則提示該訂單可以封箱，qty不為零則提示該訂單還缺物品的數量。不為零則不提示任何信息。
                    #endregion
                }
                else if (m.commodity_type == "3")
                {
                    #region 調度--對庫存進行操作
                    m.change_user = int.Parse((Session["caller"] as Caller).user_id.ToString());//操作iwms_record 需要插入create_uaer_id。对aseld中的change_user未做任何改变
                    m.act_pick_qty = Int32.Parse(Request.Params["act_pick_qty"]);//下一步插入檢貨記錄表，每檢一次記錄一次，實際撿貨量以傳過來的值為標準
                    if (_iasdMgr.getTime(m).Rows.Count > 0)
                    {//獲取到有效期控管商品的保質期
                        m.cde_dt_incr = int.Parse(_iasdMgr.getTime(m).Rows[0]["cde_dt_incr"].ToString());
                        m.cde_dt_shp = int.Parse(_iasdMgr.getTime(m).Rows[0]["cde_dt_shp"].ToString());
                    }
                    if (!string.IsNullOrEmpty(Request.Params["cde_dt"]))
                    {//獲取有效日期算出製造日期
                        m.cde_dt = DateTime.Parse(Request.Params["cde_dt"]);
                        if (m.cde_dt_incr > 0)
                        {
                            m.made_dt = m.cde_dt.AddDays(-m.cde_dt_incr);
                        }
                        else
                        {
                            m.made_dt = DateTime.Now;
                        }
                    }
                    else if (!string.IsNullOrEmpty(Request.Params["made_dt"]))
                    {//獲取製造日期獲取有效日期
                        m.made_dt = DateTime.Parse(Request.Params["made_dt"]);
                        if (m.cde_dt_incr > 0)
                        {
                            m.cde_dt = m.made_dt.AddDays(m.cde_dt_incr);
                        }
                        else
                        {
                            m.cde_dt = DateTime.Now;
                        }
                    }
                    else
                    {//不是有效期控管
                        m.made_dt = DateTime.Now;
                        m.cde_dt = DateTime.Now;
                    }
                    if (m.act_pick_qty > 0)
                    {
                        sb.Append(_iasdMgr.AddIwsRecord(m));
                    }
                    //m.act_pick_qty = m.ord_qty - m.out_qty;
                    _iasdMgr.InsertSql(sb.ToString());//執行SQL語句裡面有事物處理
                    int result = _iasdMgr.DecisionBulkPicking(m, 3);//判斷調度是否檢完，是否檢夠，是否可以裝箱

                    json = "{success:true,msg:'" + result + "'" + "}";//返回json數據  
                    #endregion
                }
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
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

    }
}
