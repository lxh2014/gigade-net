using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        IiupcImplMgr _IiupcMgr;
        IAseldImplMgr _iasdMgr;
        IAseldMasterImplMgr _aseldmasterMgr;
        IIialgImplMgr _iagMgr;
        IProductItemImplMgr _iproductitemMgr;
        IPalletMoveImplMgr _ipalet;
        IIialgImplMgr _iialgMgr;
        MarketTallyMgr _marketTallyMgr;
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
            ViewBag.itemid = Request.Params["itemid"];
            return View();
        }
        public ActionResult AutoMarketTally()
        {
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

        //根據upcid獲取item_id
        public HttpResponseBase GetItemidByUpcid()
        {
            string json = string.Empty;
            _IiupcMgr = new IupcMgr(mySqlConnectionString);
            _iproductitemMgr = new ProductItemMgr(mySqlConnectionString);
            Iupc m = new Iupc();
            DataTable dt = new DataTable();
            bool isUpc = true;
            try
            {
                //獲取條碼
                if (!string.IsNullOrEmpty(Request.Params["upc_id"]))
                {
                    m.upc_id = Request.Params["upc_id"].ToString().Trim();
                    //6位整數時先判斷輸入的是否為item_id
                    Regex reg = new Regex("^\\d{6}$");
                    if (reg.IsMatch(m.upc_id))
                    {
                        ProductItemQuery query = new ProductItemQuery();
                        query.Item_Id = Convert.ToUInt32(m.upc_id);
                        List<ProductItemQuery> store = _iproductitemMgr.GetProductItemByID(query);
                        if (store.Count == 0)
                        {
                            isUpc = true;
                        }
                        else if (store.Count > 1)
                        {
                            isUpc = true;
                        }
                        else
                        {
                            json = "{success:true,msg:1,itemid:" + query.Item_Id + "}";
                            isUpc = false;
                        }
                    }
                    if (isUpc)
                    {
                        dt = _IiupcMgr.upcid(m);
                        if (dt.Rows.Count == 0)
                        {
                            json = "{success:false,msg:0}";
                        }
                        else if (dt.Rows.Count > 1)
                        {
                            json = "{success:false,msg:2}";
                        }
                        else
                        {
                            json = "{success:true,msg:1,itemid:" + dt.Rows[0]["item_id"].ToString() + "}";
                        }
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


        //根據工作代號、細項編號 獲取商品數據
        /// <summary>
        /// 自動理貨 根據工作代號/細項編號/訂單號/出貨單號獲取所有商品信息
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetAllAseldList()
        {//判斷寄倉或者調度
            string json = String.Empty;

            AseldQuery m = new AseldQuery();
            List<AseldQuery> list = new List<AseldQuery>();
            _iasdMgr = new AseldMgr(mySqlConnectionString);
            int totalCount = 0;
            try
            {

                string search_type = Request.Params["search_type"].ToString().Trim();
                if (search_type == "assg_id")
                {
                    m.assg_id = Request.Params["search_con"].ToString().Trim();
                }
                else if (search_type == "item_id")
                {
                    m.item_id = Convert.ToUInt32(Request.Params["search_con"].ToString().Trim());
                }
                else if (search_type == "ord_id")
                {
                    m.ord_id = Convert.ToInt32(Request.Params["search_con"].ToString().Trim());
                }
                else if (search_type == "deliver_code")
                {
                    m.deliver_code = Request.Params["search_con"].ToString().Trim();
                }
                else
                {

                }
                if (!string.IsNullOrEmpty(Request.Params["start_time"]))
                {
                    m.start_time = Convert.ToDateTime(Convert.ToDateTime(Request.Params["start_time"]).ToString("yyyy-MM-dd 00:00:00"));
                }
                if (!string.IsNullOrEmpty(Request.Params["end_time"]))
                {
                    m.end_time = Convert.ToDateTime(Convert.ToDateTime(Request.Params["end_time"]).ToString("yyyy-MM-dd 23:59:59"));
                }
                
                list = _iasdMgr.GetAllAseldList(m, out totalCount);
                foreach (var item in list)
                {
                    m.seld_id = item.seld_id;
                }
                m.wust_id = "BSY";
                m.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                _iasdMgr.Updwust(m);
                json = "{success:true,totalCount:"+ totalCount +",data:" + JsonConvert.SerializeObject(list, Formatting.Indented) + "}";//返回json數據              
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
            this.Response.Write(json.ToString());
            this.Response.End();
            return this.Response;
        }

        //根據工作代號、細項編號 獲取商品數據
        public HttpResponseBase GetAseldListByItemid()
        {//判斷寄倉或者調度
            string json = String.Empty;

            Aseld m = new Aseld();
            List<AseldQuery> list = new List<AseldQuery>();
            _iasdMgr = new AseldMgr(mySqlConnectionString);
            try
            {
                m.assg_id = Request.Params["assg_id"].ToString().Trim();
                if (!string.IsNullOrEmpty(Request.Params["item_id"]))
                {
                    m.item_id = Convert.ToUInt32(Request.Params["item_id"].ToString().Trim());
                }
                list = _iasdMgr.GetAseldListByItemid(m);
                foreach (var item in list)
                {
                    m.seld_id = item.seld_id;
                }
                m.wust_id = "BSY";
                m.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                _iasdMgr.Updwust(m);
                json = "{success:true,totalCount:" + list.Count + ",data:" + JsonConvert.SerializeObject(list, Formatting.Indented) + "}";//返回json數據              
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
        public HttpResponseBase GetStockByItemid()
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
                if (!string.IsNullOrEmpty(Request.Params["item_id"].ToString().Trim()))
                {
                    query.item_id = Convert.ToUInt32(Request.Params["item_id"].ToString().Trim());
                    List<IinvdQuery> listIinvdQuery = _iinvd.GetIinvdListByItemid(query, out totalCount);
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
                        json = JsonConvert.SerializeObject(listIinvdQuery, Formatting.Indented, timeConverter);//返回json數據
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
                    int cancel = 0;
                    int ord_id = 0;
                    #region  判斷項目狀態
                    if (_iasdMgr.SelCom(m) == 0)
                    {
                        ord = 0;
                        ord_id = m.ord_id;
                        //訂單揀貨完成，可以封箱
                    }
                    if (_iasdMgr.SelComA(m) == 0)
                    {
                        flag = 0;//項目訂單揀貨完成
                    }
                    if (ord == 0)
                    {//有沒有臨時取消的商品
                        if (_iasdMgr.SelComC(m) > 0)
                        {
                            cancel = 1; ord_id = m.ord_id;
                        }
                    }
                    #endregion
                    json = "{success:true,qty:'" + m.out_qty + "',flag:'" + flag + "',ord:'" + ord + "',cancel:'" + cancel + "',ord_id:'" + ord_id + "'}";//返回json數據  
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

        //獲取製造日期，有效日期判斷是否日期控管
        public HttpResponseBase JudgeDate()
        {
            string jsonStr = "{success:false}";
            DateTime dt = new DateTime();
            DataTable data = new DataTable();
            _iinvd = new IinvdMgr(mySqlConnectionString);
            int day = 0;
            try
            {
                string dtstring = Request.Params["dtstring"].ToString();
                if (DateTime.TryParse(Request.Params["startTime"].ToString(), out dt))
                {
                    #region 編號獲取數據
                    if (!int.TryParse(Request.Params["item_id"].ToString(), out day))
                    {//獲取條碼
                        data = _iinvd.Getprodubybar(Request.Params["item_id"].ToString());
                    }
                    else
                    {//獲取商品編號
                        data = _iinvd.Getprodu(int.Parse(Request.Params["item_id"].ToString()));
                    }
                    #endregion
                    DateTime dts = DateTime.Parse(Request.Params["startTime"].ToString());

                    if (data.Rows.Count > 0)
                    {//該商品有數據才往下進行
                        if (data.Rows[0]["pwy_dte_ctl"].ToString() == "Y")
                        {//需要日期控管才進行操作]
                            DateTime dte = dts, dtss, dtee;
                            dt = DateTime.Now;
                            if (dtstring == "1" || dtstring == "2")
                            {
                                if (dtstring == "1")
                                {//根據製造日期求出有效期
                                    dte = dts.AddDays(int.Parse(data.Rows[0]["cde_dt_incr"].ToString()));//製造日期+保質期=有效期
                                }
                                if (dtstring == "2")
                                {//根據有效日期求出製造日期
                                    dts = dte.AddDays(-int.Parse(data.Rows[0]["cde_dt_incr"].ToString()));
                                }
                                //
                                if (dts > dt)
                                {
                                    jsonStr = "{success:true,msg:'1'}";
                                }
                                else
                                {
                                    dtss = dts.AddDays(int.Parse(data.Rows[0]["cde_dt_var"].ToString()));//製造時間+允出天數
                                    dtee = dt.AddDays(int.Parse(data.Rows[0]["cde_dt_shp"].ToString()));//今天+允出天數
                                    if (dt > dtss)
                                    {
                                        jsonStr = "{success:true,msg:'2'}";
                                        if (dtee > dte)
                                        {
                                            jsonStr = "{success:true,msg:'3'}";
                                            if (dte < dt)
                                            {
                                                jsonStr = "{success:true,msg:'4',dte:'" + dte.ToShortDateString() + "'}";
                                            }
                                        }
                                    }
                                    else
                                    { //有效期匯出                                       
                                        jsonStr = "{success:true,msg:'5',dts:'" + dts.ToString("yyyy-MM-dd") + "',dte:'" + dte.ToString("yyyy-MM-dd") + "'}";
                                    }
                                }
                            }
                            else
                            {
                                jsonStr = "{success:false}";
                            }
                        }
                        else
                        {
                            if (dts > DateTime.Now)
                            {
                                jsonStr = "{success:true,msg:'1'}";
                            }
                        }
                    }
                    else
                    {
                        if (dts > DateTime.Now)
                        {
                            jsonStr = "{success:true,msg:'1'}";
                        }
                    }
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

        //
        public HttpResponseBase selectproductexttime()
        {
            string json = string.Empty;
            int result = 0;
            List<ProductExt> lsPt = new List<ProductExt>();
            try
            {
                string Item_id = "";
                if (!string.IsNullOrEmpty(Request.Params["item_id"]))//查詢商品编号
                {
                    Item_id = Request.Params["item_id"];
                }
                _ipalet = new PalletMoveMgr(mySqlConnectionString);
                lsPt = _ipalet.selectproductexttime(Item_id);
                foreach (var item in lsPt)
                {
                    result = item.Cde_dt_incr;
                }
                json = "{success:true,msg:\"" + result + "\"}";
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

        #region 貨物轉移時間變動
        public HttpResponseBase aboutmadetime()
        {
            string jsonStr = string.Empty;
            int result = 0;
            DataTable dt = new DataTable();
            try
            {
                int userId = (Session["caller"] as Caller).user_id;
                DateTime nowtimes = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                int type_id = 0;//類型
                int days = 0;
                int row_id = int.Parse(Request.Params["row_id"]);
                string cde_dtormade_dt = Request.Params["cde_dtormade_dt"];

                string y_cde_dtormade_dt = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["y_cde_dtormade_dt"]))
                {
                    y_cde_dtormade_dt = Request.Params["y_cde_dtormade_dt"];//原來的日期
                }
                if (!string.IsNullOrEmpty(Request.Params["type_id"]))
                {
                    type_id = Convert.ToInt32(Request.Params["type_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["datetimeday"]))
                {
                    days = Convert.ToInt32(Request.Params["datetimeday"]);
                }

                IinvdQuery invd = new IinvdQuery();
                IinvdQuery newinvd = new IinvdQuery();
                IialgQuery ialg = new IialgQuery();
                newinvd.change_user = userId;
                newinvd.change_dtim = DateTime.Now;
                if (!string.IsNullOrEmpty(Request.Params["sloc_id"]))
                {
                    invd.plas_loc_id = Convert.ToString(Request.Params["sloc_id"]).ToUpper();
                    ialg.loc_id = invd.plas_loc_id;
                }
                if (!string.IsNullOrEmpty(Request.Params["prod_id"]))
                {
                    invd.item_id = Convert.ToUInt32(Request.Params["prod_id"]);
                    ialg.item_id = invd.item_id;
                }
                if (!string.IsNullOrEmpty(Request.Params["prod_qtys"]))
                {
                    invd.prod_qty = Convert.ToInt32(Request.Params["prod_qtys"]);
                    ialg.qty_o = invd.prod_qty;
                }
                if (!string.IsNullOrEmpty(Request.Params["remarks"]))
                {
                    invd.remarks = Request.Params["remarks"];
                    ialg.remarks = invd.remarks;
                }
                if (!string.IsNullOrEmpty(Request.Params["po_id"]))
                {
                    ialg.po_id = Request.Params["po_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["iarc_id"]))
                {
                    ialg.iarc_id = Request.Params["iarc_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["doc_no"]))
                {
                    ialg.doc_no = Request.Params["doc_no"];
                }
                ialg.create_user = userId;
                invd.change_user = userId;
                ialg.create_dtim = DateTime.Now;
                invd.change_dtim = DateTime.Now;
                invd.row_id = row_id;
                if (type_id == 1)//表示編輯的是製造日期
                {
                    invd.made_date = DateTime.Parse(cde_dtormade_dt);
                    invd.cde_dt = invd.made_date.AddDays(days);//有效日期
                    if (invd.made_date > nowtimes)//已經過期
                    {
                        jsonStr = "{success:true,msg:1}";//1表示有效日期不能小於當前日期
                    }
                    else
                    {
                        _ipalet = new PalletMoveMgr(mySqlConnectionString);
                        _iialgMgr = new IialgMgr(mySqlConnectionString);
                        result = _ipalet.selectcount(invd);
                        #region 往iialg表中插入時間修改記錄
                        ialg.made_dt = DateTime.Parse(y_cde_dtormade_dt);//原來的日期
                        ialg.c_made_dt = DateTime.Parse(cde_dtormade_dt);//改后的日期
                        ialg.cde_dt = DateTime.Parse(y_cde_dtormade_dt).AddDays(days);//原來的有效日期
                        ialg.c_cde_dt = DateTime.Parse(cde_dtormade_dt).AddDays(days);//修改后的有效日期
                        ialg.adj_qty = 0;
                        if (string.IsNullOrEmpty(ialg.iarc_id))
                        {
                            ialg.iarc_id = "PC";
                        }
                        _iialgMgr.insertiialg(ialg);//往iialg中插入數據,用來記錄數據
                        #endregion
                        if (result > 0)//大於0表示裡面存在一樣子的值
                        {
                            dt = _ipalet.selectrow_id(invd);//獲取這個重複的row_id
                            newinvd.row_id = Convert.ToInt32(dt.Rows[0][0]);
                            newinvd.prod_qty = Convert.ToInt32(dt.Rows[0][1]) + invd.prod_qty;

                            if (_ipalet.UpdateordeleteIinvd(invd, newinvd) > 0)
                            {
                                jsonStr = "{success:true,msg:2}";//修改成功
                            }
                            else
                            {
                                jsonStr = "{success:false,msg:3}";//修改失敗
                            }
                        }
                        else
                        {
                            if (_ipalet.updatemadedate(invd) > 0)
                            {
                                jsonStr = "{success:true,msg:2}";//修改成功
                            }
                            else
                            {
                                jsonStr = "{success:false,msg:3}";//修改失敗
                            }
                        }
                    }
                }
                else if (type_id == 2)//表示有效日期
                {
                    invd.cde_dt = DateTime.Parse(cde_dtormade_dt);
                    invd.made_date = invd.cde_dt.AddDays(days * (-1));
                    if (invd.made_date > nowtimes)
                    {
                        jsonStr = "{success:true,msg:1}";//1表示有效日期不能小於當前日期
                    }
                    else
                    {
                        _ipalet = new PalletMoveMgr(mySqlConnectionString);
                        _iialgMgr = new IialgMgr(mySqlConnectionString);
                        result = _ipalet.selectcount(invd);
                        #region 往iialg表中插入時間修改記錄
                        ialg.cde_dt = DateTime.Parse(y_cde_dtormade_dt);//原來的有效日期日期
                        ialg.c_cde_dt = DateTime.Parse(cde_dtormade_dt);//改后的有效日期日期
                        ialg.made_dt = DateTime.Parse(y_cde_dtormade_dt).AddDays(days * (-1));//原來的製造日期
                        ialg.c_made_dt = DateTime.Parse(cde_dtormade_dt).AddDays(days * (-1));//修改后製造日期
                        ialg.adj_qty = 0;
                        if (string.IsNullOrEmpty(ialg.iarc_id))
                        {
                            ialg.iarc_id = "PC";
                        }
                        _iialgMgr.insertiialg(ialg);//往iialg中插入數據,用來記錄數據
                        #endregion
                        if (result > 0)//大於0表示裡面存在一樣子的值
                        {
                            dt = _ipalet.selectrow_id(invd);//獲取這個重複的row_id
                            newinvd.row_id = Convert.ToInt32(dt.Rows[0][0]);
                            newinvd.prod_qty = Convert.ToInt32(dt.Rows[0][1]) + invd.prod_qty;
                            if (_ipalet.UpdateordeleteIinvd(invd, newinvd) > 0)
                            {
                                jsonStr = "{success:true,msg:2}";//修改成功
                            }
                            else
                            {
                                jsonStr = "{success:false,msg:3}";//修改失敗
                            }
                        }
                        else
                        {
                            if (_ipalet.updatemadedate(invd) > 0)
                            {
                                jsonStr = "{success:true,msg:2}";//修改成功
                            }
                            else
                            {
                                jsonStr = "{success:false,msg:3}";//修改失敗
                            }
                        }
                    }
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
        #endregion

        public HttpResponseBase RFAutoMarketTally()
        {
            string json = String.Empty;
            json = "{success:true}";
            try
            {
                _marketTallyMgr = new MarketTallyMgr(mySqlConnectionString);

                string id = Request.Params["id"];
                string[] ids = id.Split(',');
                for (int i = 0; i < ids.Length - 1; i++)
                {
                     int seld_id = int.Parse(ids[i].ToString());
                     bool result = _marketTallyMgr.RFAutoMarketTally(seld_id);
                     if (!result)
                     {
                         json = "{success:false}";
                         break;
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
            this.Response.Write(json.ToString());
            this.Response.End();
            return this.Response;

        }
    }
}
