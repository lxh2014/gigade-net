using Admin.gigade.CustomError;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using BLL.gigade.Model.Query;
using System.Data;
using System.Drawing;
using System.Text;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;
using System.Configuration;
using BLL.gigade.Common;
using gigadeExcel.Comment;
using BLL.gigade.Model.Custom;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.pdf;
using BLL.gigade.Dao;

namespace Admin.gigade.Controllers
{
    public class WareHouseController : Controller
    {
        //
        // GET: /WareHouse/        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        private static DataTable DTExcel = new DataTable();
        private static DataTable DTIupcExcel = new DataTable();
        private static DataTable DTIplasExcel = new DataTable();
        private static DataTable DTIplasEnterExcel = new DataTable();
        private static DataTable DTIlocExcel = new DataTable();
        IIlocImplMgr _IlocMgr;
        IIplasImplMgr _IiplasMgr;
        IiupcImplMgr _IiupcMgr;
        IinvdImplMgr _iinvd;
        IPalletMoveImplMgr _ipalet;
        IAseldImplMgr _iasdMgr;
        IAseldMasterImplMgr _aseldmasterMgr;
        IParametersrcImplMgr _psrcMgr;
        IParametersrcImplMgr _paraMgr;
        IIialgImplMgr _iagMgr;
        ICbjobDetailImplMgr _cbjobMgr;
        ICbjobMasterImplMgr _cbMasterMgr;
        IlocChangeDetailMgr _ilocDetailMger;
        IIialgImplMgr _iialgMgr;
        private IParametersrcImplMgr _ptersrc;
        IIwmsRrecordMgr _IIwmsRrecordMgr;
        IstockChangeMgr _istockMgr;
        IIpoImplMgr _ipoMgr;
        IIpodImplMgr _ipodMgr;
        public ProductItemMgr productitemMgr;
        private IVendorImplMgr _vendorMgr;
        IProductItemImplMgr _proditemMgr;
        IParametersrcImplMgr _IparasrcMgr;
        #region Views
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 料位管理
        /// </summary>
        /// <returns>料位管理視圖</returns>
        public ActionResult Iloc()
        {
            return View();
        }
        /// <summary>
        /// 商品主料位管理
        /// </summary>
        /// <returns></returns>
        public ActionResult IPlas()
        {
            return View();
        }
        /// <summary>
        /// 商品主料位管理
        /// </summary>
        /// <returns></returns>
        public ActionResult IinvdIndex()
        {
            return View();
        }
        /// <summary>
        /// 條碼維護
        /// </summary>
        /// <returns></returns>
        public ActionResult Iupc()
        {
            return View();
        }
        /// <summary>
        /// 補貨到主料位
        /// </summary>
        /// <returns></returns>
        public ActionResult PalletMove()
        {
            return View();
        }
        //
        public ActionResult AseldIndex()
        {
            return View();
        }
        /// <summary>
        /// 理貨員工作
        /// </summary>
        /// <returns></returns>
        public ActionResult MarkTally()
        {
            return View();
        }
        /// <summary>
        /// 理貨--寄倉流程 Warehouse District
        /// </summary>
        /// <returns></returns>
        public ActionResult MarkTallyWD()
        {
            ViewBag.number = Request.Params["number"];
            return View();
        }
        public ActionResult Iialg()
        {
            return View();
        }
        /// <summary>
        /// 理貨--調度流程 transit Warehouse
        /// </summary>
        /// <returns></returns>
        public ActionResult MarkTallyTW()
        {
            ViewBag.number = Request.Params["number"];
            ViewBag.freight_set = Request.Params["freight_set"];
            return View();
        }

        /// <summary>
        /// 補貨建議報表（副料位到主料位）
        /// </summary>
        /// <returns></returns>
        public ActionResult PalletSuggest()
        {

            return View();
        }
        /// <summary>
        /// 无主料位报表
        /// </summary>
        /// <returns></returns>
        public ActionResult NoIlocReport()
        {
            return View();
        }
        /// <summary>
        /// 主料位摘除表
        /// </summary>
        /// <returns></returns>
        public ActionResult IlocReport()
        {
            return View();
        }

        public ActionResult IlocExport()
        {
            return View();
        }
        /// <summary>
        /// 無條碼商品表匯出
        /// </summary>
        /// <returns></returns>
        public ActionResult NoiupcExport()
        {
            return View();
        }
        /// <summary>
        /// 即期/過期品報表
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult PastProductExport()
        {
            return View();
        }
        /// <summary>
        /// 大出貨報表
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliveryStatement()
        {
            return View();
        }
        /// <summary>
        /// 未完成理貨工作明細
        /// </summary>
        /// <returns></returns>
        public ActionResult UnfinishedJobExport()
        {
            return View();

        }
        /// <summary>
        /// 庫存鎖匯出
        /// </summary>
        /// <returns></returns>
        public ActionResult KucunLock()
        {
            return View();
        }
        /// <summary>
        /// 撿貨表by料位元
        /// </summary>
        /// <returns></returns>
        public ActionResult AseldExport()
        {
            return View();
        }
        /// <summary>
        /// 庫存調整
        /// </summary>
        /// <returns></returns>
        public ActionResult KucunTiaozheng()
        {
            return View();
        }
        /// <summary>
        /// 庫存匯入調整
        /// </summary>
        /// <returns></returns>
        public ActionResult KucunhuiruTiaozheng()
        {
            return View();
        }
        /// <summary>
        /// 參數設定
        /// </summary>
        /// <returns></returns>
        public ActionResult IialgParameter()
        {
            return View();
        }

        /// <summary>
        /// 盤點報表
        /// </summary>
        /// <returns></returns>
        public ActionResult CountBook()
        {
            return View();
        }
        /// <summary>
        /// 盤點差異報表
        /// </summary>
        /// <returns></returns>
        public ActionResult DifCountBook()
        {
            return View();
        }
        /// <summary>
        /// 盤點差異報表OBK
        /// </summary>
        /// <returns></returns>
        public ActionResult CountBookOBK()
        {
            return View();
        }
        /// <summary>
        /// 盤點管理
        /// </summary>
        /// <returns></returns>
        public ActionResult checkmanage()
        {
            return View();
        }
        /// <summary>
        /// 料位移動查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult IlocChangeDetail()
        {
            return View();
        }
        //料位移動記錄確認
        public ActionResult IlocChangeDetailLink()
        {
            ViewBag.Icd_Id = Request.Params["icd_id"];
            return View();
        }

        //idiff_count_book 即時差異報表
        public ActionResult idiffcountbook()
        {
            return View();
        }
        /// <summary>
        /// RF理貨記錄列表頁
        /// </summary>
        /// <returns></returns>
        public ActionResult IwmsRecord()
        {
            return View();
        }
        /// <summary>
        /// 料位帳卡列表頁
        /// </summary>
        /// <returns></returns>
        public ActionResult IstockChange()
        {
            return View();
        }
        /// <summary>
        /// 採購單，標頭
        /// </summary>
        /// <returns></returns>
        public ActionResult Ipo()
        {
            return View();
        }
        public ActionResult Ipod()
        {
            ViewBag.Ipo_poid = Request.Params["po_id"];
            return View();
        }
        //採購單驗收
        public ActionResult Check()
        {
            return View();
        }
        //等待料位報表
        public ActionResult WaitLiaoWei()
        {
            return View();
        }
        #endregion

        #region 料位管理模塊

        #region 料位維護 iloc

        #region 料位列表頁
        public HttpResponseBase GetIlocList()
        {
            string json = string.Empty;
            IlocQuery iloc = new IlocQuery();
            iloc.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            iloc.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            iloc.lcat_id = Request.Params["Ilocid_type"];
            if (!string.IsNullOrEmpty(Request.Params["search_type"]))//model中默認為F
            {
                iloc.lsta_id = Request.Params["search_type"];
            }
            else
            {
                iloc.lsta_id = string.Empty;
            }
            if (!string.IsNullOrEmpty(Request.Params["searchcontent"].Trim()))
            {
                iloc.loc_id = Request.Params["searchcontent"].ToString().ToUpper();
            }
            DateTime time;
            if (DateTime.TryParse(Request.Params["starttime"].ToString(), out time))
            {
                iloc.starttime = time;
            }
            if (DateTime.TryParse(Request.Params["endtime"].ToString(), out time))
            {
                iloc.endtime = time.AddDays(1);
            }
            try
            {
                List<IlocQuery> store = new List<IlocQuery>();
                _IlocMgr = new IlocMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _IlocMgr.GetIocList(iloc, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 料位新增
        [CustomHandleError]
        public HttpResponseBase InestIloc()
        {
            string jsonStr = String.Empty;
            try
            {
                Iloc m = new Iloc();
                _IlocMgr = new IlocMgr(mySqlConnectionString);
                m.dc_id = Int32.Parse(Request.Params["dc_id"].ToString());
                m.whse_id = Int32.Parse(Request.Params["whse_id"].ToString());
                m.loc_id = Request.Params["loc_id"].ToString().ToUpper();
                m.llts_id = Request.Params["llts_id"].ToString();
                m.ldes_id = Request.Params["ldes_id"].ToString();
                m.lcat_id = Request.Params["lcat_id"];
                if (m.lcat_id == "S")//表示如果是主料位
                {
                    if (!string.IsNullOrEmpty(Request.Params["sel_stk_pos"]))
                    {
                        m.sel_stk_pos = Int32.Parse(Request.Params["sel_stk_pos"]);
                    }
                    else
                    {
                        m.sel_stk_pos = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["sel_pos_hgt"]))
                    {
                        m.sel_pos_hgt = Int32.Parse(Request.Params["sel_pos_hgt"]);
                    }
                    else
                    {
                        m.sel_pos_hgt = 0;
                    }
                    m.rsv_stk_pos = 0;
                    m.rsv_pos_hgt = 0;
                }
                else if (m.lcat_id == "R")//表示如果是副料位
                {
                    if (!string.IsNullOrEmpty(Request.Params["rsv_stk_pos"]))
                    {
                        m.rsv_stk_pos = Int32.Parse(Request.Params["rsv_stk_pos"]);
                    }
                    else
                    {
                        m.rsv_stk_pos = 0;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["rsv_pos_hgt"]))
                    {
                        m.rsv_pos_hgt = Int32.Parse(Request.Params["rsv_pos_hgt"]);
                    }
                    else
                    {
                        m.rsv_pos_hgt = 0;
                    }
                    m.sel_stk_pos = 0;
                    m.sel_pos_hgt = 0;
                }
                m.loc_status = 1;
                m.stk_lmt = Int32.Parse(Request.Params["stk_lmt"]);
                m.stk_pos_wid = Int32.Parse(Request.Params["stk_pos_wid"]);
                m.lev = Int32.Parse(Request.Params["lev"]);
                m.lhnd_id = Request.Params["lhnd_id"];
                m.stk_pos_dep = Int32.Parse(Request.Params["stk_pos_dep"]);
                m.comingle_allow = Request.Params["comingle_allow"];
                //m.ldsp_id = Request.Params["ldsp_id"];
                m.ldsp_id = string.Empty;
                if (String.IsNullOrEmpty(Request.Params["row_id"]))
                {
                    m.create_dtim = DateTime.Now;       //創建時間
                    m.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    m.change_dtim = DateTime.Now;   //編輯時間
                    m.change_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;

                    if (_IlocMgr.IlocInsert(m) > 0)
                    {
                        jsonStr = "{success:true}";
                    }
                    else
                    {
                        jsonStr = "{success:false}";
                    }
                }
                else
                {
                    m.row_id = Convert.ToInt32(Request.Params["row_id"]);
                    m.change_dtim = DateTime.Now;
                    m.change_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    if (_IlocMgr.IlocEdit(m) > 0)
                    {
                        jsonStr = "{success:true,msg:0}";//返回json數據
                    }
                    else
                    {
                        jsonStr = "{success:false,msg:0}";//返回json數據
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
        // 判斷料號是否重複
        public HttpResponseBase GetLoc_id()
        {
            string json = string.Empty;
            Iloc loc = new Iloc();
            int result = 0;
            try
            {
                _IlocMgr = new IlocMgr(mySqlConnectionString);
                if (String.IsNullOrEmpty(Request.Params["row_id"]))//如果是新增
                {
                    loc.loc_id = Request.Params["id"].ToString().ToUpper();
                    result = _IlocMgr.GetLoc_id(loc);
                }
                else
                {
                    loc.loc_id = Request.Params["id"].ToUpper();
                    int row_id = Convert.ToInt32(Request.Params["row_id"]);
                    string loc_id = _IlocMgr.GetLoc_idByRow_id(row_id);
                    if (loc_id == loc.loc_id)
                    {
                        result = 0;//當編輯沒有改變時,則表示此料位沒有重複
                    }
                    else
                    {
                        result = _IlocMgr.GetLoc_id(loc);
                    }
                }

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,data:" + result + "}";//返回json數據
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
        #region 料位刪除
        public HttpResponseBase DeleteLocid()
        {
            string jsonStr = String.Empty;
            _IlocMgr = new IlocMgr(mySqlConnectionString);
            Iloc loc = new Iloc();
            try
            {
                string str = Request.Params["row_id"];//獲取類型
                str = str.Substring(0, str.LastIndexOf(","));
                loc.loc_id = str;//這個是row拼接的結果
                loc.change_dtim = DateTime.Now;
                loc.change_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                int j = _IlocMgr.DeleteLocidByIloc(loc); //更改iloc表中的狀態
                if (j > 0)
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
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 更新表Iloc的狀態
        public JsonResult UpdateIlocActive()
        {
            string jsonStr = string.Empty;
            try
            {
                _IlocMgr = new IlocMgr(mySqlConnectionString);
                Iloc loc = new Iloc();
                int id = Convert.ToInt32(Request.Params["id"]);
                string active = Request.Params["active"];
                if (active == "F")
                {
                    loc.lsta_id = "H";
                }
                else if (active == "H")
                {
                    loc.lsta_id = "F";
                }
                loc.row_id = id;
                loc.change_user = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                loc.change_dtim = DateTime.Now;

                if (_IlocMgr.UpdateIlocLock(loc) > 0)
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
        #endregion
        #region 料位維護匯出
        public void IlocExcelList()
        {
            string json = string.Empty;
            IlocQuery ilocQuery = new IlocQuery();
            DataTable dtExcel = new DataTable();
            try
            {
                ilocQuery.lcat_id = Request.Params["Ilocid_type"];
                if (!string.IsNullOrEmpty(Request.Params["search_type"]) && Request.Params["search_type"] != "null")//model中默認為F
                {
                    ilocQuery.lsta_id = Request.Params["search_type"];
                }
                else
                {
                    ilocQuery.lsta_id = string.Empty;
                }
                if (!string.IsNullOrEmpty(Request.Params["searchcontent"].Trim()))
                {
                    ilocQuery.loc_id = Request.Params["searchcontent"].ToString().ToUpper();
                }
                DateTime time;
                if (DateTime.TryParse(Request.Params["starttime"].ToString(), out time))
                {
                    ilocQuery.starttime = time;
                }
                if (DateTime.TryParse(Request.Params["endtime"].ToString(), out time))
                {
                    ilocQuery.endtime = time;
                }
                List<IlocQuery> store = new List<IlocQuery>();
                _IlocMgr = new IlocMgr(mySqlConnectionString);
                store = _IlocMgr.GetIlocExportList(ilocQuery);
                dtExcel.Columns.Add("料位編號", typeof(String));
                dtExcel.Columns.Add("Hash料位", typeof(String));
                dtExcel.Columns.Add("修改人員", typeof(String));
                dtExcel.Columns.Add("修改時間", typeof(String));
                dtExcel.Columns.Add("料位類型", typeof(String));
                dtExcel.Columns.Add("料位狀態", typeof(String));
                for (int i = 0; i < store.Count; i++)
                {
                    DataRow newRow = dtExcel.NewRow();
                    newRow[0] = store[i].loc_id.ToString();
                    newRow[1] = store[i].hash_loc_id.ToString();
                    newRow[2] = store[i].change_users.ToString();
                    newRow[3] = store[i].change_dtim.ToString();
                    newRow[4] = store[i].lcat_id.ToString();
                    newRow[5] = store[i].lsta_id.ToString();
                    dtExcel.Rows.Add(newRow);
                }
                if (dtExcel.Rows.Count > 0)
                {
                    string fileName = "料位維護_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtExcel, "料位維護_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
                else
                {
                    Response.Write("匯出數據不存在");
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
        }
        #endregion
        #region 料位維護-匯入功能
        public HttpResponseBase IlocUploadExcel()
        {
            string newName = string.Empty;
            string json = string.Empty;
            List<IlocQuery> store = new List<IlocQuery>();
            HashEncrypt hashpt = new HashEncrypt();
            try
            {

                DTIlocExcel.Clear();
                DTIlocExcel.Columns.Clear();
                DTIlocExcel.Columns.Add("料位編號", typeof(String));
                DTIlocExcel.Columns.Add("料位類型", typeof(String));
                DTIlocExcel.Columns.Add("所在層數");
                DTIlocExcel.Columns.Add("不能匯入的原因", typeof(String));
                int result = 0;
                int count = 0;//總匯入數
                int entercount = 0;//插入失敗個數
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
                    if (dt.Rows.Count > 0)
                    {
                        _IlocMgr = new IlocMgr(mySqlConnectionString);
                        _IiplasMgr = new IplasMgr(mySqlConnectionString);

                        int i = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            StringBuilder strsql = new StringBuilder();
                            Iloc ic = new BLL.gigade.Model.Iloc();
                            i++;
                            try
                            {
                                if (!string.IsNullOrEmpty(dr[0].ToString()) && Regex.IsMatch(dr[0].ToString(), @"^[A-Z]{2}\d{3}[A-Z]\d{2}$") && !string.IsNullOrEmpty(dr[1].ToString()) && (dr[1].ToString() == "S" || dr[1].ToString() == "R"))
                                {
                                    int loc_id_exsit = _IiplasMgr.YesOrNoLocIdExsit(dr[0].ToString());//判斷料位是否存在
                                    ic.loc_id = dr[0].ToString();
                                    ic.lsta_id = "F";
                                    ic.lcat_id = dr[1].ToString();
                                    ic.create_dtim = DateTime.Now;
                                    ic.change_dtim = DateTime.Now;
                                    ic.create_user = create_user;
                                    ic.change_user = create_user;
                                    ic.loc_status = 1;
                                    ic.lev = GetIntByString(dr[2].ToString());
                                    if (loc_id_exsit > 0)
                                    {
                                        DataRow drtwo = DTIlocExcel.NewRow();
                                        drtwo[0] = dr[0].ToString();
                                        drtwo[1] = dr[1].ToString();
                                        drtwo[2] = dr[2].ToString();
                                        drtwo[3] = "該料位已存在";
                                        DTIlocExcel.Rows.Add(drtwo);
                                        errorcount++;
                                        continue;
                                    }
                                    else//料位不存在
                                    {
                                        string has = hashpt.Md5Encrypt(ic.loc_id, "16");

                                        strsql.AppendFormat(@"insert into iloc(dc_id,whse_id,loc_id,llts_id,bkfill_loc,ldes_id,
                                             ldim_id,x_coord,y_coord,z_coord,bkfill_x_coord,bkfill_y_coord,
                                             bkfill_z_coord,lsta_id,sel_stk_pos,sel_seq_loc,sel_pos_hgt,rsv_stk_pos,
                                             rsv_pos_hgt,stk_lmt,stk_pos_wid,lev,lhnd_id,ldsp_id,
                                             create_user,create_dtim,comingle_allow,change_user,change_dtim,lcat_id,
                                             space_remain,max_loc_wgt,loc_status,stk_pos_dep,hash_loc_id
                    ) values ('{0}','{1}','{2}','{3}','{4}','{5}',
                              '{6}','{7}','{8}','{9}','{10}','{11}',
                              '{12}','{13}','{14}','{15}','{16}','{17}',
                              '{18}','{19}','{20}','{21}','{22}','{23}',
                              '{24}','{25}','{26}','{27}','{28}','{29}',
                              '{30}','{31}','{32}','{33}','{34}');",
                             ic.dc_id, ic.whse_id, ic.loc_id, ic.llts_id, ic.bkfill_loc, ic.ldes_id,
                             ic.ldim_id, ic.x_coord, ic.y_coord, ic.z_coord, ic.bkfill_x_coord, ic.bkfill_y_coord,
                             ic.bkfill_z_coord, ic.lsta_id, ic.sel_stk_pos, ic.sel_seq_loc, ic.sel_pos_hgt, ic.rsv_stk_pos,
                             ic.rsv_pos_hgt, ic.stk_lmt, ic.stk_pos_wid, ic.lev, ic.lhnd_id, ic.ldsp_id,
                             ic.create_user, BLL.gigade.Common.CommonFunction.DateTimeToString(ic.create_dtim), ic.comingle_allow, ic.change_user, BLL.gigade.Common.CommonFunction.DateTimeToString(ic.change_dtim), ic.lcat_id,
                             ic.space_remain, ic.max_loc_wgt, ic.loc_status, ic.stk_pos_dep, has
                             );
                                        result = _IlocMgr.SaveBySql(strsql.ToString());
                                        if (result > 0)
                                        {
                                            count++;
                                            continue;
                                        }
                                        else
                                        {
                                            DataRow drtwo = DTIlocExcel.NewRow();
                                            drtwo[0] = dr[0].ToString();
                                            drtwo[1] = dr[1].ToString();
                                            drtwo[2] = dr[2].ToString();
                                            drtwo[3] = "料位插入數據庫時失敗";
                                            DTIlocExcel.Rows.Add(drtwo);
                                            entercount++;
                                            continue;
                                        }
                                    }
                                }
                                else
                                {
                                    DataRow drtwo = DTIlocExcel.NewRow();
                                    drtwo[0] = dr[0].ToString();
                                    drtwo[1] = dr[1].ToString();
                                    drtwo[2] = dr[2].ToString();
                                    drtwo[3] = "料位編號或者料位類型或者所在層數不符合格式";
                                    DTIlocExcel.Rows.Add(drtwo);
                                    errorcount++;
                                    continue;
                                }
                            }
                            catch
                            {
                                DataRow drtwo = DTIlocExcel.NewRow();
                                drtwo[0] = dr[0].ToString();
                                drtwo[1] = dr[1].ToString();
                                drtwo[2] = dr[2].ToString();
                                drtwo[3] = "數據異常";
                                DTIlocExcel.Rows.Add(drtwo);
                                errorcount++;
                                continue;
                            }
                        }
                        if (count > 0)
                        {
                            json = "{success:true,total:" + count + ",error:" + errorcount + ",entercount:" + entercount + "}";
                        }
                        else
                        {
                            json = "{success:true,total:" + 0 + ",error:" + errorcount + ",entercount:" + entercount + "}";
                        }
                    }
                    else
                    {
                        json = "{success:true,total:" + 0 + ",error:" + 0 + ",entercount" + 0 + "}";
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
        public int GetIntByString(string str)
        {
            try
            {
                Dictionary<string, int> dty = new Dictionary<string, int>();
                dty.Add("A", 1);
                dty.Add("B", 2);
                dty.Add("C", 3);
                dty.Add("D", 4);
                dty.Add("E", 5);
                dty.Add("F", 6);
                dty.Add("G", 7);
                dty.Add("H", 8);
                dty.Add("I", 9);
                dty.Add("J", 10);
                dty.Add("K", 11);
                dty.Add("L", 12);
                dty.Add("M", 13);
                dty.Add("N", 14);
                dty.Add("O", 15);
                return dty[str.ToUpper()];
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return 1;
            }
        }
        public void IlocUpdownmessage()
        {
            string json = string.Empty;
            try
            {
                string fileName = "料位維護_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                MemoryStream ms = ExcelHelperXhf.ExportDT(DTIlocExcel, "");
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
        public void IlocUpdownTemplate()
        {
            string json = string.Empty;
            DataTable dtTemplateExcel = new DataTable();
            try
            {
                dtTemplateExcel.Columns.Add("料位編號", typeof(String));
                dtTemplateExcel.Columns.Add("料位類型", typeof(String));
                dtTemplateExcel.Columns.Add("所在層數", typeof(String));
                DataRow newRow = dtTemplateExcel.NewRow();
                dtTemplateExcel.Rows.Add(newRow);
                string fileName = "料位維護匯入模板_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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

        #region 一鍵Hash  HashAll
        public HttpResponseBase HashAll()
        {
            string jsonStr = String.Empty;
            try
            {
                Iloc m = new Iloc();
                _IlocMgr = new IlocMgr(mySqlConnectionString);

                if (_IlocMgr.HashAll() > 0)
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
        #endregion
        #endregion

        #region 條碼維護
        //條碼列表頁
        public HttpResponseBase GetIupcList()
        {
            string json = string.Empty;

            IupcQuery iupc = new IupcQuery();
            iupc.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            iupc.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            string content = string.Empty;
            if (!string.IsNullOrEmpty(Request.Params["searchcontent"]))
            {
                content = Request.Params["searchcontent"].Replace('，', ',').Replace('|', ',').Replace(' ', ',');//.Replace(' ',',')
                string[] list = content.Split(',');
                string test = "^[0-9]*$";
                int count = 0;//實現最後一個不加,
                for (int i = 0; i < list.Length; i++)
                {
                    if (!string.IsNullOrEmpty(list[i]))
                    {
                        if (Regex.IsMatch(list[i], test))
                        {
                            count = count + 1;
                            if (count == 1)
                            {
                                iupc.searchcontent = list[i];
                            }
                            else
                            {
                                iupc.searchcontent = iupc.searchcontent + "," + list[i];
                            }
                        }
                        else
                        {
                            iupc.searchcontentstring = iupc.searchcontentstring + list[i] + ",";
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(Request.Params["time_start"]))
            {
                iupc.create_time_start = DateTime.Parse(Request.Params["time_start"]).ToString("yyyy/MM/dd 00:00:00");
            }
            if (!string.IsNullOrEmpty(Request.Params["time_end"]))
            {
                iupc.create_time_end = DateTime.Parse(Request.Params["time_end"]).ToString("yyyy/MM/dd 23:59:59");
            }
            try
            {
                List<IupcQuery> store = new List<IupcQuery>();
                _IiupcMgr = new IupcMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _IiupcMgr.GetIupcList(iupc, out  totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                List<Parametersrc> stores = new List<Parametersrc>();
                _ptersrc = new ParameterMgr(mySqlConnectionString);
                stores = _ptersrc.GetElementType("iupc_type");
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                foreach (var item in store)
                {
                    item.product_name += GetProductSpec(item.item_id.ToString());
                    for (int i = 0; i < stores.Count; i++)
                    {
                        if (item.upc_type_flg == stores[i].ParameterCode)
                        {
                            item.upc_type_flg_string = stores[i].parameterName;
                        }
                    }
                }
                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        //條碼新增修改
        public HttpResponseBase SaveIupc()
        {
            string json = string.Empty;
            try
            {
                Iupc iupc = new Iupc();
                iupc.item_id = uint.Parse(Request.Params["item_id"]);
                iupc.upc_id = Request.Params["upc_id"];
                if (!string.IsNullOrEmpty(Request.Params["upc_type_flg"]))
                {
                    iupc.upc_type_flg = Request.Params["upc_type_flg"];
                }

                if (!string.IsNullOrEmpty(Request.Params["row_id"]))
                {
                    iupc.row_id = int.Parse(Request.Params["row_id"]);
                }
                else
                {
                    iupc.row_id = 0;
                }
                _IiupcMgr = new IupcMgr(mySqlConnectionString);
                string result = _IiupcMgr.IsExist(iupc);
                if (result == "0")
                {
                    //新增
                    if (string.IsNullOrEmpty(Request.Params["row_id"]))
                    {
                        iupc.create_user = (Session["caller"] as Caller).user_id;
                        iupc.create_dtim = DateTime.Now;
                        _IiupcMgr.Insert(iupc);
                        json = "{success:true,msg:\"" + "新增成功！" + "\"}";
                    }
                    //修改
                    else
                    {
                        iupc.row_id = int.Parse(Request.Params["row_id"]);
                        _IiupcMgr.Update(iupc);
                        json = "{success:true,msg:\"" + "修改成功！" + "\"}";
                    }
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
                json = "{success:false,msg:\"" + "" + "\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        //條碼刪除
        public HttpResponseBase IupcDelete()
        {
            string jsonStr = String.Empty;
            Iupc iupc = new Iupc();
            _IiupcMgr = new IupcMgr(mySqlConnectionString);
            if (!String.IsNullOrEmpty(Request.Params["rowid"]))
            {
                try
                {
                    foreach (string rid in Request.Params["rowid"].ToString().Split('|'))
                    {
                        if (!string.IsNullOrEmpty(rid))
                        {
                            iupc.row_id = Convert.ToInt32(rid);
                            _IiupcMgr.Delete(iupc);
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
        public HttpResponseBase GetIupcType()
        {
            List<Parametersrc> stores = new List<Parametersrc>();
            _ptersrc = new ParameterMgr(mySqlConnectionString);
            string type = string.Empty;
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["Type"]))
                {
                    type = Request.Params["Type"];
                }
                stores = _ptersrc.GetElementType(type);

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
        #region 條碼維護-匯入功能
        /// <summary>
        /// 料位匯入,通過Excel
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase UploadExcel()
        {
            string newName = string.Empty;
            string json = string.Empty;
            List<IupcQuery> store = new List<IupcQuery>();
            _IparasrcMgr = new ParameterMgr(mySqlConnectionString);
            StringBuilder codeType1 = new StringBuilder();
            string codeTypeStr1 = string.Empty;
            try
            {

                List<BLL.gigade.Model.Parametersrc> codeTypeList = _IparasrcMgr.GetElementType("iupc_type");
                foreach (var codeTypeModel in codeTypeList)
                {
                    codeType1.AppendFormat("{0}:{1}, ", codeTypeModel.ParameterCode, codeTypeModel.parameterName);
                }
                codeTypeStr1 = codeType1.ToString().Substring(0, codeType1.Length - 2);

                DTIupcExcel.Clear();
                DTIupcExcel.Columns.Clear();

                DTIupcExcel.Columns.Add("商品細項編號", typeof(String));
                DTIupcExcel.Columns.Add("條碼編號", typeof(String));
                DTIupcExcel.Columns.Add("條碼類型（" + codeTypeStr1 + "）", typeof(String));
                DTIupcExcel.Columns.Add("不能匯入的原因", typeof(String));
                DTIupcExcel.Columns.Add("匯入失敗數據的行號", typeof(String));

                DataTable DTIupcImportSucceed = new DataTable();
                DTIupcImportSucceed.Columns.Add("商品細項編號", typeof(String));
                DTIupcImportSucceed.Columns.Add("條碼編號", typeof(String));
                DTIupcImportSucceed.Columns.Add("條碼類型", typeof(String));
                DTIupcImportSucceed.Columns.Add("行號", typeof(String));

                int result = 0;
                int count = 0;//總匯入數
                int errorCount = 0;//異常數據數量
                int create_user = (Session["caller"] as Caller).user_id;
                int itemIdNotExistCount = 0;//商品細項編號不存在數量
                //int iupcTypeNotExistCount = 0;//條碼類型不存在數量
                int repeatCount = 0;//商品條碼重複數量
                StringBuilder strsql = new StringBuilder();
                if (Request.Files["ImportExcelFile"] != null && Request.Files["ImportExcelFile"].ContentLength > 0)
                {
                    HttpPostedFileBase excelFile = Request.Files["ImportExcelFile"];
                    //FileManagement fileManagement = new FileManagement();
                    newName = Server.MapPath(excelPath) + excelFile.FileName;
                    excelFile.SaveAs(newName);
                    DataTable dt = new DataTable();
                    NPOI4ExcelHelper helper = new NPOI4ExcelHelper(newName);
                    dt = helper.SheetData();

                    if (dt.Rows.Count > 0)
                    {
                        _IiupcMgr = new IupcMgr(mySqlConnectionString);
                        //_IparasrcMgr = new ParameterMgr(mySqlConnectionString);
                        #region 循環Excel的數據

                        //List<BLL.gigade.Model.Parametersrc> codeTypeList = _IparasrcMgr.GetElementType("iupc_type");

                        int i = 0;
                        for (int k = 0; k < dt.Rows.Count; k++)
                        {
                            i++;
                            try
                            {
                                bool b0 = string.IsNullOrEmpty(dt.Rows[k][0].ToString().Trim());
                                bool b1 = string.IsNullOrEmpty(dt.Rows[k][1].ToString().Trim());
                                bool b2 = string.IsNullOrEmpty(dt.Rows[k][2].ToString().Trim());

                                if (b0 || b1 || b2)//如果數據有一個欄位為空
                                {
                                    DataRow drtwo = DTIupcExcel.NewRow();
                                    drtwo[0] = dt.Rows[k][0].ToString();
                                    drtwo[1] = " " + dt.Rows[k][1].ToString().Trim();
                                    drtwo[2] = dt.Rows[k][2].ToString();
                                    drtwo[3] = "這條數據有欄位為空，請確認";
                                    drtwo[4] = k + 2;//匯入失敗數據的行號,Excel表行號
                                    DTIupcExcel.Rows.Add(drtwo);
                                    errorCount++;
                                    continue;
                                }
                                if (dt.Rows[k][1].ToString().Trim().Length <= 25)
                                {

                                    int a = Convert.ToInt32(dt.Rows[k][0]);//商品細項編號
                                    string b = dt.Rows[k][1].ToString().Trim();//條碼編號
                                    int c = Convert.ToInt32(dt.Rows[k][2]);//條碼類型
                                    int flag = _IiupcMgr.Yesornoexist(a, b);

                                    if (flag == 1)//等於1表示商品細項表里面沒有此商品細項編號
                                    {
                                        DataRow drtwo = DTIupcExcel.NewRow();
                                        drtwo[0] = dt.Rows[k][0].ToString();
                                        drtwo[1] = " " + dt.Rows[k][1].ToString().Trim();
                                        drtwo[2] = dt.Rows[k][2].ToString();
                                        drtwo[3] = "在數據庫商品表中，不存在此商品細項編號";
                                        drtwo[4] = k + 2;//匯入失敗數據的行號,Excel表行號
                                        DTIupcExcel.Rows.Add(drtwo);
                                        itemIdNotExistCount++;
                                        continue;
                                    }

                                    if (flag == 2)//等於2表示條碼表裡面已存在此條碼
                                    {
                                        DataRow drtwo = DTIupcExcel.NewRow();
                                        drtwo[0] = dt.Rows[k][0].ToString();
                                        drtwo[1] = " " + dt.Rows[k][1].ToString().Trim();
                                        drtwo[2] = dt.Rows[k][2].ToString();
                                        drtwo[3] = "在數據庫中，該條碼已經存在";
                                        drtwo[4] = k + 2;
                                        DTIupcExcel.Rows.Add(drtwo);
                                        repeatCount++;
                                        continue;
                                    }

                                    if (flag == 0)//當存在此商品細項編號並且該條碼不存在時進行添加數據
                                    {

                                        bool xunhuan = true;
                                        for (int j = 0; j < i - 1; j++)
                                        {
                                            if (dt.Rows[j][1].ToString() == dt.Rows[k][1].ToString())//如果匯入的Excel條碼重複
                                            {
                                                xunhuan = false;
                                                DataRow drtwo = DTIupcExcel.NewRow();
                                                drtwo[0] = dt.Rows[k][0].ToString();
                                                drtwo[1] = " " + dt.Rows[k][1].ToString().Trim();
                                                drtwo[2] = dt.Rows[k][2].ToString();
                                                drtwo[3] = "該商品條碼與此表中(行號： " + (j + 2) + " )的商品細項編號:" + dt.Rows[j][0].ToString() + "的條碼重複";
                                                drtwo[4] = k + 2;
                                                DTIupcExcel.Rows.Add(drtwo);
                                                repeatCount++;
                                                break;
                                            }
                                        }
                                        if (xunhuan)
                                        {
                                            string codeTypeStr = string.Empty;
                                            StringBuilder codeType = new StringBuilder();
                                            bool haveCodeType = false;
                                            foreach (var codeTypeModel in codeTypeList)
                                            {
                                                if (Convert.ToString(dt.Rows[k][2]).Trim() == codeTypeModel.ParameterCode)
                                                {
                                                    haveCodeType = true;
                                                }
                                                codeType.AppendFormat("{0}:{1}, ", codeTypeModel.ParameterCode, codeTypeModel.parameterName);
                                            }
                                            codeTypeStr = codeType.ToString().Substring(0, codeType.Length - 2);
                                            if (!haveCodeType)//此條碼類型是否在參數表t_parameterSrc(parameterType="iupc_type")中存在
                                            {
                                                DataRow drtwo = DTIupcExcel.NewRow();
                                                drtwo[0] = dt.Rows[k][0].ToString();
                                                drtwo[1] = " " + dt.Rows[k][1].ToString().Trim();
                                                drtwo[2] = dt.Rows[k][2].ToString();
                                                drtwo[3] = "在數據庫參數表中，此條碼類型不存在(" + codeTypeStr + ")";
                                                drtwo[4] = k + 2;
                                                DTIupcExcel.Rows.Add(drtwo);
                                                errorCount++;
                                                continue;
                                            }
                                            //如果條碼類為 1 時，判斷該商品是否在Iupc表中已經存在國際條碼
                                            if (_IiupcMgr.upc_num(Convert.ToInt32(dt.Rows[k][0])) > 0 && dt.Rows[k][2].ToString() == "1")
                                            {
                                                DataRow drtwo = DTIupcExcel.NewRow();
                                                drtwo[0] = dt.Rows[k][0].ToString();
                                                drtwo[1] = " " + dt.Rows[k][1].ToString().Trim();
                                                drtwo[2] = dt.Rows[k][2].ToString();
                                                drtwo[3] = "在數據庫中，該商品已經存在國際條碼";
                                                drtwo[4] = k + 2;
                                                DTIupcExcel.Rows.Add(drtwo);
                                                repeatCount++;
                                                continue;
                                            }
                                            else
                                            {
                                                bool skip = false;
                                                for (int index = 0; index < DTIupcImportSucceed.Rows.Count; index++)
                                                {
                                                    bool m1 = dt.Rows[k][0].ToString().Trim() == DTIupcImportSucceed.Rows[index][0].ToString().Trim();
                                                    bool m2 = dt.Rows[k][2].ToString().Trim() == "1";
                                                    bool m3 = DTIupcImportSucceed.Rows[index][2].ToString().Trim() == "1";

                                                    if (m1 && m2 && m3)//在已經成功匯入的數據中，判斷該商品是否存在國際條碼 
                                                    {
                                                        skip = true;
                                                        DataRow drtwo1 = DTIupcExcel.NewRow();
                                                        drtwo1[0] = dt.Rows[k][0].ToString();
                                                        drtwo1[1] = " " + dt.Rows[k][1].ToString().Trim();
                                                        drtwo1[2] = dt.Rows[k][2].ToString();
                                                        drtwo1[3] = "在已經成功匯入的數據中(行號： " + DTIupcImportSucceed.Rows[index][3].ToString() + "),該商品已經存在國際條碼";
                                                        drtwo1[4] = k + 2;
                                                        DTIupcExcel.Rows.Add(drtwo1);
                                                        repeatCount++;
                                                        break;
                                                    }
                                                }
                                                if (skip)
                                                {
                                                    continue;
                                                }
                                                DataRow drtwo = DTIupcImportSucceed.NewRow();
                                                drtwo[0] = dt.Rows[k][0].ToString();
                                                drtwo[1] = " " + dt.Rows[k][1].ToString().Trim();
                                                drtwo[2] = dt.Rows[k][2].ToString();
                                                drtwo[3] = k + 2;
                                                DTIupcImportSucceed.Rows.Add(drtwo);


                                                count++;
                                                string dataTimeNow = CommonFunction.DateTimeToString(DateTime.Now);
                                                strsql.AppendFormat(@"insert into iupc(upc_id,item_id,suppr_upc,lst_ship_dte,lst_rct_dte,create_dtim,create_user,upc_type_flg)
                    values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}');", b, a, "", dataTimeNow, dataTimeNow, dataTimeNow, create_user, c);
                                                continue;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    DataRow drtwo = DTIupcExcel.NewRow();
                                    drtwo[0] = dt.Rows[k][0].ToString();
                                    drtwo[1] = " " + dt.Rows[k][1].ToString().Trim();
                                    drtwo[2] = dt.Rows[k][2].ToString();
                                    drtwo[3] = "條碼不符合格式(0-25位)";
                                    drtwo[4] = k + 2;
                                    DTIupcExcel.Rows.Add(drtwo);
                                    errorCount++;
                                    continue;
                                }
                            }
                            catch
                            {
                                DataRow drtwo = DTIupcExcel.NewRow();
                                drtwo[0] = dt.Rows[k][0].ToString();
                                drtwo[1] = " " + dt.Rows[k][1].ToString().Trim();
                                drtwo[2] = dt.Rows[k][2].ToString();
                                drtwo[3] = "數據異常";
                                drtwo[4] = k + 2;
                                DTIupcExcel.Rows.Add(drtwo);
                                errorCount++;
                                continue;
                            }
                        }
                        #region 註釋的代碼foreach
                        //                        foreach (DataRow dr in dt.Rows)
                        //                        {
                        //                            i++;
                        //                            try
                        //                            {
                        //                                if (!string.IsNullOrEmpty(dr[1].ToString()) && dr[1].ToString().Length >= 8 && dr[1].ToString().Length <= 25)
                        //                                {

                        //                                    int a = Convert.ToInt32(dr[0]);//商品細項編號
                        //                                    string b = dr[1].ToString();//條碼編號
                        //                                    int c = Convert.ToInt32(dr[2]);//條碼類型
                        //                                    int flag = _IiupcMgr.Yesornoexist(a, b);

                        //                                    if (flag == 1)//等於1表示商品細項表里面沒有此商品細項編號
                        //                                    {
                        //                                        DataRow drtwo = DTIupcExcel.NewRow();
                        //                                        drtwo[0] = dr[0].ToString();
                        //                                        drtwo[1] = " " + dr[1].ToString();
                        //                                        drtwo[2] = dr[2].ToString();
                        //                                        drtwo[3] = "商品表不存在此商品細項編號";
                        //                                        DTIupcExcel.Rows.Add(drtwo);
                        //                                        bucunzaicount++;
                        //                                        continue;
                        //                                    }

                        //                                    if (flag == 2)//等於2表示條碼表裡面已存在此條碼
                        //                                    {
                        //                                        DataRow drtwo = DTIupcExcel.NewRow();
                        //                                        drtwo[0] = dr[0].ToString();
                        //                                        drtwo[1] = " " + dr[1].ToString();
                        //                                        drtwo[2] = dr[2].ToString();
                        //                                        drtwo[3] = "該條碼已經存在";
                        //                                        DTIupcExcel.Rows.Add(drtwo);
                        //                                        chongfucount++;
                        //                                        continue;
                        //                                    }

                        //                                    if (flag == 0)//當存在此商品細項編號並且該條碼不存在時進行添加數據
                        //                                    {

                        //                                        bool xunhuan = true;
                        //                                        for (int j = 0; j < i - 1; j++)
                        //                                        {
                        //                                            if (dt.Rows[j][1].ToString() == dr[1].ToString())//如果匯入的Excel條碼重複
                        //                                            {
                        //                                                xunhuan = false;
                        //                                                DataRow drtwo = DTIupcExcel.NewRow();
                        //                                                drtwo[0] = dr[0].ToString();
                        //                                                drtwo[1] = " " + dr[1].ToString();
                        //                                                drtwo[2] = dr[2].ToString();
                        //                                                drtwo[3] = "該商品條碼與在此表中的商品細項編號:" + dt.Rows[j][0].ToString() + "條碼重複";
                        //                                                DTIupcExcel.Rows.Add(drtwo);
                        //                                                chongfucount++;
                        //                                                break;
                        //                                            }
                        //                                        }
                        //                                        if (xunhuan)
                        //                                        {
                        //                                            string codeTypeStr = string.Empty;
                        //                                            StringBuilder codeType = new StringBuilder();
                        //                                            bool haveCodeType = false;
                        //                                            foreach (var codeTypeModel in codeTypeList)
                        //                                            {
                        //                                                if (Convert.ToString(dr[2]) == codeTypeModel.ParameterCode)
                        //                                                {
                        //                                                    haveCodeType = true;                                                  
                        //                                                }
                        //                                                codeType.AppendFormat("{0}:{1}, ", codeTypeModel.ParameterCode, codeTypeModel.parameterName);
                        //                                            }
                        //                                            codeTypeStr = codeType.ToString().Substring(0, codeType.Length - 2);
                        //                                            if (!haveCodeType)
                        //                                            {
                        //                                                DataRow drtwo = DTIupcExcel.NewRow();
                        //                                                drtwo[0] = dr[0].ToString();
                        //                                                drtwo[1] = " " + dr[1].ToString();
                        //                                                drtwo[2] = dr[2].ToString();
                        //                                                drtwo[3] = "此條碼類型不存在(" + codeTypeStr + ")";
                        //                                                DTIupcExcel.Rows.Add(drtwo);
                        //                                                errorcount++;
                        //                                                continue;
                        //                                            }

                        //                                            if (_IiupcMgr.upc_num(Convert.ToInt32(dr[0])) > 0)
                        //                                            {
                        //                                                DataRow drtwo = DTIupcExcel.NewRow();
                        //                                                drtwo[0] = dr[0].ToString();
                        //                                                drtwo[1] = " " + dr[1].ToString();
                        //                                                drtwo[2] = dr[2].ToString();
                        //                                                drtwo[3] = "該商品已經存在國際條碼";
                        //                                                DTIupcExcel.Rows.Add(drtwo);
                        //                                                chongfucount++;
                        //                                                break;
                        //                                            }
                        //                                            else
                        //                                            {
                        //                                                count++;
                        //                                                strsql.AppendFormat(@"insert into iupc(upc_id,item_id,create_dtim,create_user,upc_type_flg)
                        //                    values('{0}','{1}','{2}','{3}','{4}');", b, a, CommonFunction.DateTimeToString(DateTime.Now), create_user,c);//默認匯入 的是國際條碼
                        //                                                continue;
                        //                                            }
                        //                                        }
                        //                                    }
                        //                                }
                        //                                else
                        //                                {
                        //                                    DataRow drtwo = DTIupcExcel.NewRow();
                        //                                    drtwo[0] = dr[0].ToString();
                        //                                    drtwo[1] = " " + dr[1].ToString();
                        //                                    drtwo[2] = dr[2].ToString();
                        //                                    drtwo[3] = "條碼不符合格式";
                        //                                    DTIupcExcel.Rows.Add(drtwo);
                        //                                    errorcount++;
                        //                                    continue;
                        //                                }
                        //                            }
                        //                            catch
                        //                            {
                        //                                DataRow drtwo = DTIupcExcel.NewRow();
                        //                                drtwo[0] = dr[0].ToString();
                        //                                drtwo[1] = " " + dr[1].ToString();
                        //                                drtwo[2] = dr[2].ToString();
                        //                                drtwo[3] = "數據異常";
                        //                                DTIupcExcel.Rows.Add(drtwo);
                        //                                errorcount++;
                        //                                continue;
                        //                            }
                        // 
                        #endregion                        }
                        #endregion
                        if (strsql.ToString().Trim() != "")
                        {
                            result = _IiupcMgr.ExcelImportIupc(strsql.ToString());
                            if (result > 0)
                            {
                                json = "{success:true,total:" + count + ",error:" + errorCount + ",repeat:" + repeatCount + ",NoItem:" + itemIdNotExistCount + "}";
                            }
                            else
                            {
                                json = "{success:false}";
                            }
                        }
                        else
                        {
                            json = "{success:true,total:" + 0 + ",error:" + errorCount + ",repeat:" + repeatCount + ",NoItem:" + itemIdNotExistCount + "}";
                        }
                    }
                    else
                    {
                        json = "{success:true,total:" + 0 + ",error:" + 0 + ",repeat:" + 0 + ",NoItem:" + 0 + ",NoType:" + 0 + "}";
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false }";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        /// <summary>
        /// 顯示匯入失敗的數據
        /// </summary>
        public void Updownmessage()
        {
            string json = string.Empty;
            try
            {
                string fileName = "IupcImportErrorMsg" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                MemoryStream ms = ExcelHelperXhf.ExportDT(DTIupcExcel, "");
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());
                //StringWriter sw = ExcelHelperXhf.SetCsvFromData(DTIupcExcel, "目前不符合的數據.csv");
                //Response.Clear();
                //Response.AddHeader("Content-Disposition", "attachment; filename=" + "目前不符合的數據_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv");
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
        public void UpdownTemplate()
        {
            string json = string.Empty;
            DataTable dtTemplateExcel = new DataTable();
            _IparasrcMgr = new ParameterMgr(mySqlConnectionString);
            StringBuilder codeType = new StringBuilder();
            string codeTypeStr = string.Empty;
            try
            {
                List<BLL.gigade.Model.Parametersrc> codeTypeList = _IparasrcMgr.GetElementType("iupc_type");
                foreach (var codeTypeModel in codeTypeList)
                {
                    codeType.AppendFormat("{0}:{1}, ", codeTypeModel.ParameterCode, codeTypeModel.parameterName);
                }
                codeTypeStr = codeType.ToString().Substring(0, codeType.Length - 2);

                dtTemplateExcel.Columns.Add("商品細項編號", typeof(String));
                dtTemplateExcel.Columns.Add("條碼編號", typeof(String));
                dtTemplateExcel.Columns.Add("條碼類型(" + codeTypeStr + ")", typeof(String));
                DataRow newRow = dtTemplateExcel.NewRow();
                dtTemplateExcel.Rows.Add(newRow);
                string fileName = "BarCodeVindicateImportModel_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";//條碼維護匯入模板
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

        #region 條碼維護匯出
        //條碼維護匯出
        public void ReportManagementExcelList()
        {
            string json = string.Empty;
            IupcQuery iupc = new IupcQuery();
            DataTable dtExcel = new DataTable();
            try
            {
                string content = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["searchcontent"]))
                {
                    content = Request.Params["searchcontent"].Replace('，', ',').Replace('|', ',').Replace(' ', ',');//.Replace(' ',',')
                    string[] list = content.Split(',');
                    string test = "^[0-9]*$";
                    int count = 0;//實現最後一個不加,
                    for (int i = 0; i < list.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(list[i]))
                        {
                            if (Regex.IsMatch(list[i], test))
                            {
                                count = count + 1;
                                if (count == 1)
                                {
                                    iupc.searchcontent = list[i];
                                }
                                else
                                {
                                    iupc.searchcontent = iupc.searchcontent + "," + list[i];
                                }
                            }
                            else
                            {
                                iupc.searchcontentstring = iupc.searchcontentstring + list[i] + ",";
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["time_start"]))
                {
                    iupc.create_time_start = DateTime.Parse(Request.Params["time_start"]).ToString("yyyy/MM/dd 00:00:00");
                }
                if (!string.IsNullOrEmpty(Request.Params["time_end"]))
                {
                    iupc.create_time_end = DateTime.Parse(Request.Params["time_end"]).ToString("yyyy/MM/dd 23:59:59");
                }
                List<IupcQuery> store = new List<IupcQuery>();
                _IiupcMgr = new IupcMgr(mySqlConnectionString);
                store = _IiupcMgr.GetIupcExportList(iupc);
                dtExcel.Columns.Add("商品編號", typeof(String));
                dtExcel.Columns.Add("商品名稱", typeof(String));
                dtExcel.Columns.Add("條碼編號", typeof(String));
                dtExcel.Columns.Add("條碼類型", typeof(String));
                dtExcel.Columns.Add("創建人", typeof(String));
                dtExcel.Columns.Add("創建時間", typeof(String));
                for (int i = 0; i < store.Count; i++)
                {
                    DataRow newRow = dtExcel.NewRow();
                    newRow[0] = store[i].item_id.ToString();
                    store[i].product_name += GetProductSpec(store[i].item_id.ToString());
                    newRow[1] = store[i].product_name.ToString();
                    newRow[2] = " " + store[i].upc_id.ToString();
                    //if (!string.IsNullOrEmpty(store[i].upc_type_flg))
                    //{
                    //    string upc_type = store[i].upc_type_flg;
                    //    if (upc_type.Equals("1"))
                    //    {
                    //        newRow[3] = "國際條碼";
                    //    }
                    //    if (upc_type.Equals("2"))
                    //    {
                    //        newRow[3] = "吉甲地店內碼";
                    //    }
                    //    if (upc_type.Equals("3"))
                    //    {
                    //        newRow[3] = "供應商店內碼";
                    //    }
                    //}
                    //else { newRow[3] = ""; }
                    newRow[3] = store[i].parametername.ToString();
                    newRow[4] = store[i].create_users.ToString();
                    newRow[5] = store[i].create_dtim.ToString();
                    dtExcel.Rows.Add(newRow);
                }
                if (dtExcel.Rows.Count > 0)
                {
                    string fileName = "條碼維護_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtExcel, "條碼維護_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
                else
                {
                    Response.Write("匯出數據不存在");
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
        }
        #endregion

        #endregion

        #region 商品指定主料位

        #region 商品指定主料位列表頁
        public HttpResponseBase GetIPlasList()
        {
            string json = string.Empty;

            IplasQuery iplas = new IplasQuery();
            iplas.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            iplas.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            //iplas.searchcontent = Request.Params["searchcontent"];

            try
            {
                #region 修改之前


                //if (!string.IsNullOrEmpty(Request.Params["searchcontent"]))
                //{
                //    iplas.searchcontent = Request.Params["searchcontent"].ToString().ToUpper();//輸入的內容
                //    if (iplas.searchcontent.Length > 10)
                //    {
                //        _iinvd = new IinvdMgr(mySqlConnectionString);
                //        DataTable dt = new DataTable();
                //        dt = _iinvd.Getprodubybar(Request.Params["searchcontent"].ToString());
                //        if (dt.Rows.Count > 0)
                //        {
                //            iplas.searchcontent = dt.Rows[0]["item_id"].ToString();
                //        }
                //    }
                //}
                #endregion
                #region 修改之後
                string content = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["search_type"]))
                {
                    iplas.serch_type = int.Parse(Request.Params["search_type"]);
                    if (!string.IsNullOrEmpty(Request.Params["searchcontent"]) && Request.Params["searchcontent"].Trim().Length > 0)//有查詢內容就不管時間
                    {

                        switch (iplas.serch_type)
                        {
                            case 1:
                            case 2:
                                iplas.searchcontent = Request.Params["searchcontent"].Trim();
                                break;
                            case 3:
                                #region 之後的更改
                                content = Request.Params["searchcontent"].Replace('，', ',').Replace('|', ',').Replace(' ', ',');//.Replace(' ',',')
                                string[] list = content.Split(',');
                                string test = "^[0-9]*$";
                                int count = 0;//實現最後一個不加,
                                for (int i = 0; i < list.Length; i++)
                                {
                                    if (!string.IsNullOrEmpty(list[i]))
                                    {
                                        if (Regex.IsMatch(list[i], test))
                                        {
                                            count = count + 1;
                                            if (count == 1)
                                            {
                                                iplas.searchcontent = list[i];
                                            }
                                            else
                                            {
                                                iplas.searchcontent = iplas.searchcontent + "," + list[i];
                                            }
                                        }
                                        else
                                        {
                                            iplas.searchcontent = iplas.searchcontent + list[i] + ",";
                                        }
                                    }
                                }

                                #endregion
                                break;
                            default:
                                break;
                        }


                    }

                }
                DateTime time;
                if (DateTime.TryParse(Request.Params["starttime"].ToString(), out time))
                {
                    iplas.starttime = DateTime.Parse(time.ToString("yyyy-MM-dd 00:00:00"));
                }
                if (DateTime.TryParse(Request.Params["endtime"].ToString(), out time))
                {
                    iplas.endtime = DateTime.Parse(time.ToString("yyyy-MM-dd 23:59:59"));
                }

                #endregion
                //DateTime time;
                //if (DateTime.TryParse(Request.Params["starttime"].ToString(), out time))
                //{
                //    iplas.starttime = time;
                //}
                //if (DateTime.TryParse(Request.Params["endtime"].ToString(), out time))
                //{
                //    iplas.endtime = time;
                //}
                List<IplasQuery> store = new List<IplasQuery>();
                _IiplasMgr = new IplasMgr(mySqlConnectionString);
                //  _IiplasMgr.UpIplas(iplas);
                int totalCount = 0;
                store = _IiplasMgr.GetIplas(iplas, out  totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                foreach (var item in store)
                {
                    item.product_name += GetProductSpec(item.item_id.ToString());
                }
                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 商品指定主料位新增、編輯和刪除
        [CustomHandleError]
        public HttpResponseBase GetIPlasEdit()
        {
            string json = string.Empty;
            IplasQuery iplas = new IplasQuery();
            Iloc iloc = new Iloc();
            Iupc upc = new Iupc();
            _IiplasMgr = new IplasMgr(mySqlConnectionString);
            _IlocMgr = new IlocMgr(mySqlConnectionString);
            _ipalet = new PalletMoveMgr(mySqlConnectionString);
            try
            {
                if (string.IsNullOrEmpty(Request.Params["plas_id"]))//首先考慮添加情況
                {
                    string itemid = Request.Params["item_id"];
                    DataTable dt = _ipalet.GetProdInfo(itemid);
                    if (dt.Rows.Count > 0)
                    {
                        iplas.item_id = uint.Parse(dt.Rows[0]["item_id"].ToString());
                    }
                    else
                    {
                        iplas.item_id = 0;
                    }
                    if (_IiplasMgr.IsTrue(iplas) == "false")
                    {
                        json = "{success:false,msg:\"" + "商品編號不存在" + "\"}";
                        this.Response.Clear();
                        this.Response.Write(json);
                        this.Response.End();
                        return this.Response;
                    }
                    if (_IiplasMgr.GetIplasid(iplas) > 0)
                    {
                        json = "{success:false,msg:\"" + "此商品已存在主料位" + "\"}";
                        this.Response.Clear();
                        this.Response.Write(json);
                        this.Response.End();
                        return this.Response;
                    }
                    iloc.loc_id = Request.Params["loc_id"].ToString().ToUpper();
                    if (_IiplasMgr.GetLocCount(iloc) <= 0)
                    {
                        json = "{success:false,msg:\"" + "非主料位或主料位已鎖定" + "\"}";
                        this.Response.Clear();
                        this.Response.Write(json);
                        this.Response.End();
                        return this.Response;
                    }
                    iplas.loc_id = Request.Params["loc_id"].ToString().ToUpper();
                    if (_IiplasMgr.GetIplasCount(iplas).Count > 0)//主料位重複
                    {
                        json = "{success:false,msg:\"" + "主料位重複" + "\"}";
                        this.Response.Clear();
                        this.Response.Write(json);
                        this.Response.End();
                        return this.Response;
                    }
                    iplas.loc_stor_cse_cap = int.Parse(Request.Params["loc_stor_cse_cap"]);
                    iplas.create_user = (Session["caller"] as Caller).user_id;
                    iplas.create_dtim = DateTime.Now;
                    iplas.change_user = (Session["caller"] as Caller).user_id;
                    iplas.change_dtim = DateTime.Now;
                    _IiplasMgr.InsertIplas(iplas);//判斷主料位和商品編號沒有問題,插入Iplas表的同時.操作iloc表,設置其為已指派料位
                    json = "{success:true}";
                }
                else//編輯
                {
                    iplas.plas_id = int.Parse(Request.Params["plas_id"]);
                    upc.upc_id = Request.Params["upcid"];
                    iplas.item_id = uint.Parse(Request.Params["item_id"]);
                    iplas.loc_id = Request.Params["loc_id"].ToString().ToUpper();
                    iloc.loc_id = iplas.loc_id;
                    if (_IiplasMgr.GetLocCount(iloc) <= 0)
                    {
                        json = "{success:false,msg:\"" + "非主料位或主料位已鎖定" + "\"}";
                        this.Response.Clear();
                        this.Response.Write(json);
                        this.Response.End();
                        return this.Response;
                    }
                    if (_IiplasMgr.GetIplasCount(iplas).Count > 0)//主料位重複
                    {
                        json = "{success:false,msg:\"" + "該主料位不可用!" + "\"}";
                        this.Response.Clear();
                        this.Response.Write(json);
                        this.Response.End();
                        return this.Response;
                    }
                    iplas.loc_stor_cse_cap = int.Parse(Request.Params["loc_stor_cse_cap"]);
                    iplas.change_user = (Session["caller"] as Caller).user_id;
                    iplas.change_dtim = DateTime.Now;
                    _IiplasMgr.UpIplas(iplas);
                    json = "{success:true}";
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

        //條碼轉換為商品編號
        public string Getprodbyupc(string jsonStr)
        {
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["item_id"]))
                {
                    string prodid = Request.Params["item_id"];
                    if (prodid.Length == 6)
                    {
                        jsonStr = prodid;
                    }
                    else
                    {
                        _IiplasMgr = new IplasMgr(mySqlConnectionString);
                        DataTable dt = _IiplasMgr.Getprodbyupc(prodid);
                        if (dt.Rows.Count > 0)
                        {
                            jsonStr = dt.Rows[0]["item_id"].ToString();
                        }
                        else
                        {
                            jsonStr = string.Empty;
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
            return jsonStr;
        }
        //刪除沒有商品佔據的主料位
        public HttpResponseBase DeleteIplasById()
        {
            string jsonStr = String.Empty;
            _IlocMgr = new IlocMgr(mySqlConnectionString);
            _IiplasMgr = new IplasMgr(mySqlConnectionString);
            Iloc loc = new Iloc();
            IplasQuery plasQuery = new IplasQuery();
            IinvdQuery nvdQery = new IinvdQuery();
            try
            {
                string str = Request.Params["loc_id"].ToString().ToUpper();//獲取類型
                str = str.Substring(0, str.LastIndexOf(","));
                int sum = 0;
                string[] strs = str.Split(',');
                for (int i = 0; i < strs.Length; i++)
                {
                    nvdQery.loc_id = strs[i];
                    sum = sum + _IiplasMgr.GetIinvdItemId(nvdQery);
                }
                if (sum > 0)
                {
                    jsonStr = "{success:true,sum:" + sum + "}";//大於0表示算選包含庫存量
                }
                else
                {
                    int counts = 0;
                    for (int i = 0; i < strs.Length; i++)
                    {
                        plasQuery.loc_id = strs[i];
                        counts = counts + _IiplasMgr.DeleteIplasById(plasQuery);
                    }

                    if (counts > 0)
                    {
                        jsonStr = "{success:true,count:" + counts + "}";
                    }
                    else
                    {
                        jsonStr = "{success:false}";
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
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }

        #endregion

        #region 商品指定主料位-匯入功能

        public HttpResponseBase IplasUploadExcel()
        {
            string newName = string.Empty;
            string json = string.Empty;
            List<IplasQuery> store = new List<IplasQuery>();
            try
            {

                DTIplasExcel.Clear();
                DTIplasExcel.Columns.Clear();

                DTIplasExcel.Columns.Add("商品細項編號", typeof(String));
                DTIplasExcel.Columns.Add("主料位", typeof(String));
                DTIplasExcel.Columns.Add("不能匯入的原因", typeof(String));
                int result = 0;
                int count = 0;//總匯入數
                int errorcount = 0;//數據異常個數
                int comtentcount = 0;//內容不符合格式
                int create_user = (Session["caller"] as Caller).user_id;
                int item_idcount = 0;//商品細項編號不存在
                int item_id_have_locid = 0;//商品細項編號已存在主料位
                int locid_lock = 0;//商品料位已經被鎖定
                StringBuilder strsql = new StringBuilder();
                if (Request.Files["ImportExcelFile"] != null && Request.Files["ImportExcelFile"].ContentLength > 0)
                {
                    HttpPostedFileBase excelFile = Request.Files["ImportExcelFile"];
                    //FileManagement fileManagement = new FileManagement();
                    newName = Server.MapPath(excelPath) + excelFile.FileName;
                    excelFile.SaveAs(newName);
                    DataTable dt = new DataTable();
                    NPOI4ExcelHelper helper = new NPOI4ExcelHelper(newName);
                    dt = helper.SheetData();
                    if (dt.Rows.Count > 0)
                    {
                        _IiplasMgr = new IplasMgr(mySqlConnectionString);
                        #region 循環Excel的數據
                        Iloc ic = new BLL.gigade.Model.Iloc();
                        Iplas ips = new Iplas();
                        int i = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            i++;
                            try
                            {
                                if (!string.IsNullOrEmpty(dr[0].ToString()) && Regex.IsMatch(dr[0].ToString(), @"^\d{6}$") && !string.IsNullOrEmpty(dr[1].ToString()) && Regex.IsMatch(dr[1].ToString(), @"^[A-Z]{2}\d{3}[A-Z]\d{2}$"))
                                {
                                    ic.loc_id = dr[1].ToString();
                                    ic.lsta_id = "F";
                                    ic.lcat_id = "S";
                                    ic.create_dtim = DateTime.Now;
                                    ic.change_dtim = DateTime.Now;
                                    ic.create_user = create_user;
                                    ic.change_user = create_user;
                                    ic.loc_status = 1;
                                    ips.item_id = Convert.ToUInt32(dr[0]);
                                    ips.loc_id = dr[1].ToString();
                                    ips.change_user = create_user;
                                    ips.create_user = create_user;
                                    ips.create_dtim = DateTime.Now;
                                    ips.change_dtim = DateTime.Now;
                                    //根據商品編號查看是否存在主料位
                                    int item_id_exsit = _IiplasMgr.YesOrNoExist(Convert.ToInt32(dr[0]));//檢查item_id是否存在主料位
                                    int loc_id_exsit = _IiplasMgr.YesOrNoLocIdExsit(dr[1].ToString());//判斷料位是否存在
                                    int loc_id_lock = _IiplasMgr.GetLocCount(ic);//判斷料位是否鎖定
                                    if (_IiplasMgr.IsTrue(ips) == "false")//首先判斷item_id是否存在
                                    {
                                        DataRow drtwo = DTIplasExcel.NewRow();
                                        drtwo[0] = dr[0].ToString();
                                        drtwo[1] = dr[1].ToString();
                                        drtwo[2] = "商品細項編號不存在";
                                        DTIplasExcel.Rows.Add(drtwo);
                                        errorcount++;
                                        item_idcount++;
                                        continue;
                                    }
                                    else//如果存在item_id
                                    {
                                        if (item_id_exsit > 0)//表示已經存在主料位
                                        {
                                            DataRow drtwo = DTIplasExcel.NewRow();
                                            drtwo[0] = dr[0].ToString();
                                            drtwo[1] = dr[1].ToString();
                                            drtwo[2] = "商品細項編號已存在主料位";
                                            DTIplasExcel.Rows.Add(drtwo);
                                            errorcount++;
                                            item_id_have_locid++;
                                            continue;
                                        }
                                        #region
                                        if (loc_id_exsit > 0)//如果料位存在
                                        {
                                            if (loc_id_lock > 0)//如果料位存在並且沒有被鎖定
                                            {

                                                strsql.AppendFormat("Insert into iplas (dc_id,whse_id,loc_id,change_dtim,change_user,create_dtim,create_user,lcus_id,luis_id,item_id,prdd_id,loc_rpln_lvl_uoi,loc_stor_cse_cap,ptwy_anch,flthru_anch,pwy_loc_cntl) Values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}');", ips.dc_id, ips.whse_id, ips.loc_id.ToString().ToUpper(), CommonFunction.DateTimeToString(ips.change_dtim), ips.change_user, CommonFunction.DateTimeToString(ips.create_dtim), ips.create_user, ips.lcus_id, ips.luis_id, ips.item_id, ips.prdd_id, ips.loc_rpln_lvl_uoi, ips.loc_stor_cse_cap, ips.ptwy_anch, ips.flthru_anch, ips.pwy_loc_cntl);//插入數據到表iplas表中
                                                strsql.AppendFormat(@" set sql_safe_updates = 0; update iloc set lsta_id='{0}',change_user='{1}',change_dtim='{2}' where loc_id='{3}';set sql_safe_updates = 1; ", "A", ips.change_user, BLL.gigade.Common.CommonFunction.DateTimeToString(ips.change_dtim), ips.loc_id.ToString().ToUpper());
                                                count++;
                                            }
                                            else
                                            {
                                                DataRow drtwo = DTIplasExcel.NewRow();
                                                drtwo[0] = dr[0].ToString();
                                                drtwo[1] = dr[1].ToString();
                                                drtwo[2] = "主料位已經被鎖定";
                                                DTIplasExcel.Rows.Add(drtwo);
                                                errorcount++;
                                                locid_lock++;
                                                continue;
                                            }
                                        }
                                        else//料位不存在
                                        {
                                            strsql.AppendFormat(@"insert into iloc(dc_id,whse_id,loc_id,llts_id,bkfill_loc,ldes_id,
                                             ldim_id,x_coord,y_coord,z_coord,bkfill_x_coord,bkfill_y_coord,
                                             bkfill_z_coord,lsta_id,sel_stk_pos,sel_seq_loc,sel_pos_hgt,rsv_stk_pos,
                                             rsv_pos_hgt,stk_lmt,stk_pos_wid,lev,lhnd_id,ldsp_id,
                                             create_user,create_dtim,comingle_allow,change_user,change_dtim,lcat_id,
                                             space_remain,max_loc_wgt,loc_status,stk_pos_dep
                    ) values ('{0}','{1}','{2}','{3}','{4}','{5}',
                              '{6}','{7}','{8}','{9}','{10}','{11}',
                              '{12}','{13}','{14}','{15}','{16}','{17}',
                              '{18}','{19}','{20}','{21}','{22}','{23}',
                              '{24}','{25}','{26}','{27}','{28}','{29}',
                              '{30}','{31}','{32}','{33}');",
                                 ic.dc_id, ic.whse_id, ic.loc_id, ic.llts_id, ic.bkfill_loc, ic.ldes_id,
                                 ic.ldim_id, ic.x_coord, ic.y_coord, ic.z_coord, ic.bkfill_x_coord, ic.bkfill_y_coord,
                                 ic.bkfill_z_coord, ic.lsta_id, ic.sel_stk_pos, ic.sel_seq_loc, ic.sel_pos_hgt, ic.rsv_stk_pos,
                                 ic.rsv_pos_hgt, ic.stk_lmt, ic.stk_pos_wid, ic.lev, ic.lhnd_id, ic.ldsp_id,
                                 ic.create_user, BLL.gigade.Common.CommonFunction.DateTimeToString(ic.create_dtim), ic.comingle_allow, ic.change_user, BLL.gigade.Common.CommonFunction.DateTimeToString(ic.change_dtim), ic.lcat_id,
                                 ic.space_remain, ic.max_loc_wgt, ic.loc_status, ic.stk_pos_dep
                                 );
                                            strsql.AppendFormat("Insert into iplas (dc_id,whse_id,loc_id,change_dtim,change_user,create_dtim,create_user,lcus_id,luis_id,item_id,prdd_id,loc_rpln_lvl_uoi,loc_stor_cse_cap,ptwy_anch,flthru_anch,pwy_loc_cntl) Values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}');", ips.dc_id, ips.whse_id, ips.loc_id.ToString().ToUpper(), CommonFunction.DateTimeToString(ips.change_dtim), ips.change_user, CommonFunction.DateTimeToString(ips.create_dtim), ips.create_user, ips.lcus_id, ips.luis_id, ips.item_id, ips.prdd_id, ips.loc_rpln_lvl_uoi, ips.loc_stor_cse_cap, ips.ptwy_anch, ips.flthru_anch, ips.pwy_loc_cntl);//插入數據到表iplas表中
                                            strsql.AppendFormat(@" set sql_safe_updates = 0; update iloc set lsta_id='{0}',change_user='{1}',change_dtim='{2}' where loc_id='{3}';set sql_safe_updates = 1; ", "A", ips.change_user, BLL.gigade.Common.CommonFunction.DateTimeToString(ips.change_dtim), ips.loc_id.ToString().ToUpper());
                                            count++;
                                        }
                                        #endregion
                                    }
                                }
                                else
                                {
                                    DataRow drtwo = DTIplasExcel.NewRow();
                                    drtwo[0] = dr[0].ToString();
                                    drtwo[1] = dr[1].ToString();
                                    drtwo[2] = "商品細項編號或者主料位不符合格式";
                                    DTIplasExcel.Rows.Add(drtwo);
                                    errorcount++;
                                    comtentcount++;
                                    continue;
                                }
                            }
                            catch
                            {
                                DataRow drtwo = DTIplasExcel.NewRow();
                                drtwo[0] = dr[0].ToString();
                                drtwo[1] = dr[1].ToString();
                                drtwo[2] = "數據異常";
                                DTIplasExcel.Rows.Add(drtwo);
                                errorcount++;
                                continue;
                            }
                        }
                        #endregion
                        if (strsql.ToString().Trim() != "")
                        {
                            result = _IiplasMgr.ExcelImportIplas(strsql.ToString());
                            if (result > 0)
                            {
                                json = "{success:true,total:" + count + ",error:" + errorcount + ",item_idcount:" + item_idcount + ",item_id_have_locid:" + item_id_have_locid + ",comtentcount:" + comtentcount + ",locid_lock:" + locid_lock + "}";
                            }
                            else
                            {
                                json = "{success:false}";
                            }
                        }
                        else
                        {
                            json = "{success:true,total:" + 0 + ",error:" + errorcount + ",item_idcount:" + item_idcount + ",item_id_have_locid:" + item_id_have_locid + ",comtentcount:" + comtentcount + ",locid_lock:" + locid_lock + "}";
                        }
                    }
                    else
                    {
                        json = "{success:true,total:" + 0 + ",error:" + errorcount + ",item_idcount:" + item_idcount + ",item_id_have_locid:" + item_id_have_locid + ",comtentcount:" + comtentcount + ",locid_lock:" + locid_lock + "}";
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

        public void IplasUpdownmessage()
        {
            string json = string.Empty;
            try
            {
                string fileName = "商品指定主料位_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                MemoryStream ms = ExcelHelperXhf.ExportDT(DTIplasExcel, "");
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
        public void IplasUpdownTemplate()
        {
            string json = string.Empty;
            DataTable dtTemplateExcel = new DataTable();
            try
            {
                dtTemplateExcel.Columns.Add("商品細項編號", typeof(String));
                dtTemplateExcel.Columns.Add("主料位", typeof(String));
                DataRow newRow = dtTemplateExcel.NewRow();
                dtTemplateExcel.Rows.Add(newRow);
                string fileName = "商品指定主料位匯入模板_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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
        #region 商品指定主料位匯出
        public void IplasExcelList()
        {
            string json = string.Empty;
            IplasQuery ilpsQuery = new IplasQuery();
            DataTable dtExcel = new DataTable();
            try
            {
                string content = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["search_type"]))
                {
                    ilpsQuery.serch_type = int.Parse(Request.Params["search_type"]);
                    if (!string.IsNullOrEmpty(Request.Params["searchcontent"]) && Request.Params["searchcontent"].Trim().Length > 0)//有查詢內容就不管時間
                    {

                        switch (ilpsQuery.serch_type)
                        {
                            case 1:
                            case 2:
                                ilpsQuery.searchcontent = Request.Params["searchcontent"].Trim();
                                break;
                            case 3:
                                #region 之後的更改
                                content = Request.Params["searchcontent"].Replace('，', ',').Replace('|', ',').Replace(' ', ',');//.Replace(' ',',')
                                string[] list = content.Split(',');
                                string test = "^[0-9]*$";
                                int count = 0;//實現最後一個不加,
                                for (int i = 0; i < list.Length; i++)
                                {
                                    if (!string.IsNullOrEmpty(list[i]))
                                    {
                                        if (Regex.IsMatch(list[i], test))
                                        {
                                            count = count + 1;
                                            if (count == 1)
                                            {
                                                ilpsQuery.searchcontent = list[i];
                                            }
                                            else
                                            {
                                                ilpsQuery.searchcontent = ilpsQuery.searchcontent + "," + list[i];
                                            }
                                        }
                                        else
                                        {
                                            ilpsQuery.searchcontent = ilpsQuery.searchcontent + list[i] + ",";
                                        }
                                    }
                                }

                                #endregion
                                break;
                            default:
                                break;
                        }


                    }


                }
                DateTime time;
                if (DateTime.TryParse(Request.Params["starttime"].ToString(), out time))
                {
                    ilpsQuery.starttime = DateTime.Parse(time.ToString("yyyy-MM-dd 00:00:00"));
                }
                if (DateTime.TryParse(Request.Params["endtime"].ToString(), out time))
                {
                    ilpsQuery.endtime = DateTime.Parse(time.ToString("yyyy-MM-dd 23:59:59"));
                }
                List<IplasQuery> store = new List<IplasQuery>();
                _IiplasMgr = new IplasMgr(mySqlConnectionString);
                store = _IiplasMgr.GetIplasExportList(ilpsQuery);
                dtExcel.Columns.Add("商品編號", typeof(String));
                dtExcel.Columns.Add("商品名稱", typeof(String));
                dtExcel.Columns.Add("料位編號", typeof(String));
                dtExcel.Columns.Add("料位容量", typeof(String));
                dtExcel.Columns.Add("是否買斷", typeof(String));
                dtExcel.Columns.Add("創建人", typeof(String));
                dtExcel.Columns.Add("創建時間", typeof(String));
                for (int i = 0; i < store.Count; i++)
                {
                    DataRow newRow = dtExcel.NewRow();
                    newRow[0] = store[i].item_id.ToString();
                    newRow[1] = store[i].product_name.ToString();
                    newRow[2] = store[i].loc_id.ToString();
                    newRow[3] = store[i].loc_stor_cse_cap.ToString();
                    newRow[4] = store[i].prepaid.ToString() == "0" ? "否" : "是";
                    newRow[5] = store[i].create_users.ToString();
                    newRow[6] = store[i].create_dtim.ToString();
                    dtExcel.Rows.Add(newRow);
                }
                if (dtExcel.Rows.Count > 0)
                {
                    string fileName = "商品指定主料位_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtExcel, "商品指定主料位_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
                else
                {
                    Response.Write("匯出數據不存在");
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
        }
        #endregion









        //商品指定主料位-互搬
        public HttpResponseBase IplasUploadExcelEnter()
        {
            string newName = string.Empty;
            string json = string.Empty;
            List<IplasQuery> store = new List<IplasQuery>();
            try
            {

                DTIplasEnterExcel.Clear();
                DTIplasEnterExcel.Columns.Clear();

                DTIplasEnterExcel.Columns.Add("商品細項編號", typeof(String));
                DTIplasEnterExcel.Columns.Add("原料位", typeof(String));
                DTIplasEnterExcel.Columns.Add("新料位", typeof(String));
                DTIplasEnterExcel.Columns.Add("不能搬移的原因", typeof(String));
                int result = 0;
                int count = 0;//總匯入數
                int errorcount = 0;//數據異常個數
                int comtentcount = 0;//內容不符合格式
                int create_user = (Session["caller"] as Caller).user_id;
                int item_idcount = 0;//商品細項編號不存在
                int item_id_have_locid = 0;//商品細項編號已存在主料位
                int locid_lock = 0;//商品料位已經被鎖定
                StringBuilder strsql = new StringBuilder();
                if (Request.Files["IplasImportExcelFile"] != null && Request.Files["IplasImportExcelFile"].ContentLength > 0)
                {
                    HttpPostedFileBase excelFile = Request.Files["IplasImportExcelFile"];
                    //FileManagement fileManagement = new FileManagement();
                    newName = Server.MapPath(excelPath) + excelFile.FileName;
                    excelFile.SaveAs(newName);
                    DataTable dt = new DataTable();
                    NPOI4ExcelHelper helper = new NPOI4ExcelHelper(newName);
                    dt = helper.SheetData();

                    if (dt.Rows.Count > 0)
                    {
                        _IiplasMgr = new IplasMgr(mySqlConnectionString);



                        #region 測試
                        #region 循環Excel的數據
                        Iloc ic = new BLL.gigade.Model.Iloc();
                        Iplas ips = new Iplas();
                        int i = 0;
                        if (dt.Columns.Count < 3)
                        {
                            DataRow drtwo = DTIplasEnterExcel.NewRow();
                            drtwo[0] = "這個是商品細項編號";
                            drtwo[1] = "這個是商品原料位";
                            drtwo[2] = "這個是商品新料位";
                            drtwo[3] = "請匯入足夠的列數";
                            DTIplasEnterExcel.Rows.Add(drtwo);
                            errorcount++;
                        }
                        else
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (string.IsNullOrEmpty(dr[0].ToString()) && string.IsNullOrEmpty(dr[1].ToString()) && string.IsNullOrEmpty(dr[2].ToString()))
                                {
                                    continue;
                                }

                                i++;
                                try
                                {
                                    if (!string.IsNullOrEmpty(dr[0].ToString()) && Regex.IsMatch(dr[0].ToString(), @"^\d{6}$") && !string.IsNullOrEmpty(dr[1].ToString()) && Regex.IsMatch(dr[1].ToString(), @"^[A-Z]{2}\d{3}[A-Z]\d{2}$") && !string.IsNullOrEmpty(dr[2].ToString()) && Regex.IsMatch(dr[2].ToString(), @"^[A-Z]{2}\d{3}[A-Z]\d{2}$"))
                                    {
                                        ic.loc_id = dr[2].ToString();
                                        //ic.lsta_id = "F";
                                        //ic.lcat_id = "S";
                                        //ic.create_dtim = DateTime.Now;
                                        //ic.change_dtim = DateTime.Now;
                                        //ic.create_user = 2;
                                        //ic.change_user = 2;
                                        //ic.loc_status = 1;
                                        ips.item_id = Convert.ToUInt32(dr[0]);
                                        ips.loc_id = dr[1].ToString();


                                        //根據商品編號查看是否存在主料位
                                        int item_id_exsit = _IiplasMgr.YesOrNoExist(Convert.ToInt32(dr[0]));//檢查item_id是否存在
                                        int loc_id_exsit = _IiplasMgr.YesOrNoLocIdExsit(dr[1].ToString());//判斷原料位是否存在
                                        int item_loc_id = _IiplasMgr.YesOrNoLocIdExsit(Convert.ToInt32(dr[0]), dr[1].ToString());//判斷原料位是否為該商品的主料位
                                        int New_loc_id_exsit = _IiplasMgr.YesOrNoLocIdExsit(dr[2].ToString());//判斷新料位是否存在
                                        int loc_id_lock = _IiplasMgr.GetLocCount(ic);//判斷新料位是否鎖定

                                        if (_IiplasMgr.IsTrue(ips) == "false")//首先判斷item_id是否存在
                                        {
                                            DataRow drtwo = DTIplasEnterExcel.NewRow();
                                            drtwo[0] = dr[0].ToString();
                                            drtwo[1] = dr[1].ToString();
                                            drtwo[2] = dr[2].ToString();
                                            drtwo[3] = "商品細項編號不存在";
                                            DTIplasEnterExcel.Rows.Add(drtwo);
                                            errorcount++;
                                            item_idcount++;
                                            continue;
                                        }
                                        else//如果存在item_id
                                        {
                                            if (loc_id_exsit <= 0)//表示原料為不存在料位//------------------------------
                                            {
                                                DataRow drtwo = DTIplasEnterExcel.NewRow();
                                                drtwo[0] = dr[0].ToString();
                                                drtwo[1] = dr[1].ToString();
                                                drtwo[2] = dr[2].ToString();
                                                drtwo[3] = "商品原料位不存在";
                                                DTIplasEnterExcel.Rows.Add(drtwo);
                                                errorcount++;
                                                locid_lock++;
                                                continue;
                                            }
                                            if (item_loc_id <= 0)//表示原料位不是此商品的原料位//------------------------------
                                            {
                                                DataRow drtwo = DTIplasEnterExcel.NewRow();
                                                drtwo[0] = dr[0].ToString();
                                                drtwo[1] = dr[1].ToString();
                                                drtwo[2] = dr[2].ToString();
                                                drtwo[3] = "原料位不是此商品的料位";
                                                DTIplasEnterExcel.Rows.Add(drtwo);
                                                errorcount++;
                                                locid_lock++;
                                                continue;
                                            }
                                            if (New_loc_id_exsit <= 0)//表示新料位不存在//------------------------------
                                            {
                                                DataRow drtwo = DTIplasEnterExcel.NewRow();
                                                drtwo[0] = dr[0].ToString();
                                                drtwo[1] = dr[1].ToString();
                                                drtwo[2] = dr[2].ToString();
                                                drtwo[3] = "新料位不存在";
                                                DTIplasEnterExcel.Rows.Add(drtwo);
                                                errorcount++;
                                                item_id_have_locid++;
                                                continue;
                                            }
                                            else
                                            {
                                                if (loc_id_lock > 0)//如果新料位存在並且沒有被鎖定--plas新增，iloc更改狀態
                                                {

                                                    ips = _IiplasMgr.getplas(ips);
                                                    ips.loc_id = dr[2].ToString();
                                                    ips.change_user = (Session["caller"] as Caller).user_id;
                                                    ips.change_dtim = DateTime.Now;
                                                    ips.item_id = Convert.ToUInt32(dr[0]);
                                                    if (_IiplasMgr.UpIplas(ips) > 0)
                                                    {
                                                        strsql.Append(ips.loc_id);

                                                        //strsql.AppendFormat("Insert into iplas (dc_id,whse_id,loc_id,change_dtim,change_user,create_dtim,create_user,lcus_id,luis_id,item_id,prdd_id,loc_rpln_lvl_uoi,loc_stor_cse_cap,ptwy_anch,flthru_anch,pwy_loc_cntl) Values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}');", ips.dc_id, ips.whse_id, ips.loc_id.ToString().ToUpper(), CommonFunction.DateTimeToString(ips.change_dtim), ips.change_user, CommonFunction.DateTimeToString(ips.create_dtim), ips.create_user, ips.lcus_id, ips.luis_id, ips.item_id, ips.prdd_id, ips.loc_rpln_lvl_uoi, ips.loc_stor_cse_cap, ips.ptwy_anch, ips.flthru_anch, ips.pwy_loc_cntl);//插入數據到表iplas表中
                                                        //strsql.AppendFormat(@" set sql_safe_updates = 0; update iloc set lsta_id='{0}',change_user='{1}',change_dtim='{2}' where loc_id='{3}';set sql_safe_updates = 1; ", "A", ips.change_user, BLL.gigade.Common.CommonFunction.DateTimeToString(ips.change_dtim), ips.loc_id.ToString().ToUpper());
                                                        count++;
                                                        result++;
                                                    }
                                                    else
                                                    {
                                                        DataRow drtwo = DTIplasEnterExcel.NewRow();
                                                        drtwo[0] = dr[0].ToString();
                                                        drtwo[1] = dr[1].ToString();
                                                        drtwo[2] = dr[2].ToString();
                                                        drtwo[3] = "未知原因導入失敗";
                                                        DTIplasEnterExcel.Rows.Add(drtwo);
                                                        errorcount++;
                                                        continue;
                                                    }
                                                }
                                                else
                                                {
                                                    DataRow drtwo = DTIplasEnterExcel.NewRow();
                                                    drtwo[0] = dr[0].ToString();
                                                    drtwo[1] = dr[1].ToString();
                                                    drtwo[2] = dr[2].ToString();
                                                    drtwo[3] = "新料位已經被鎖定或非主料位";
                                                    DTIplasEnterExcel.Rows.Add(drtwo);
                                                    errorcount++;
                                                    item_id_have_locid++;
                                                    continue;
                                                }
                                            }


                                        }
                                    }
                                    else
                                    {
                                        DataRow drtwo = DTIplasEnterExcel.NewRow();
                                        drtwo[0] = dr[0].ToString();
                                        drtwo[1] = dr[1].ToString();
                                        drtwo[2] = dr[2].ToString();
                                        drtwo[3] = "商品細項編號或者料位不符合格式";
                                        DTIplasEnterExcel.Rows.Add(drtwo);
                                        errorcount++;
                                        comtentcount++;
                                        continue;
                                    }
                                }
                                catch
                                {
                                    DataRow drtwo = DTIplasEnterExcel.NewRow();
                                    drtwo[0] = dr[0].ToString();
                                    drtwo[1] = dr[1].ToString();
                                    drtwo[2] = dr[2].ToString();
                                    drtwo[3] = "數據異常";
                                    DTIplasEnterExcel.Rows.Add(drtwo);
                                    errorcount++;
                                    continue;
                                }
                            }
                        }
                        #endregion


                        #endregion

                        if (strsql.ToString().Trim() != "")
                        {
                            //result = _IiplasMgr.ExcelImportIplas(strsql.ToString());
                            if (result > 0)
                            {
                                json = "{success:true,total:" + count + ",error:" + errorcount + ",item_idcount:" + item_idcount + ",item_id_have_locid:" + item_id_have_locid + ",comtentcount:" + comtentcount + ",locid_lock:" + locid_lock + "}";
                            }
                            else
                            {
                                json = "{success:false}";
                            }
                        }
                        else
                        {
                            json = "{success:true,total:" + 0 + ",error:" + errorcount + ",item_idcount:" + item_idcount + ",item_id_have_locid:" + item_id_have_locid + ",comtentcount:" + comtentcount + ",locid_lock:" + locid_lock + "}";
                        }
                    }
                    else
                    {
                        json = "{success:true,total:" + 0 + ",error:" + errorcount + ",item_idcount:" + item_idcount + ",item_id_have_locid:" + item_id_have_locid + ",comtentcount:" + comtentcount + ",locid_lock:" + locid_lock + "}";
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

        public void IplasEntermessage()
        {
            string json = string.Empty;
            try
            {
                string fileName = "料位轉移異常數據_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                MemoryStream ms = ExcelHelperXhf.ExportDT(DTIplasEnterExcel, "");
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
        //料位互搬模板
        public void IplasEnterTemplate()
        {
            string json = string.Empty;
            DataTable dtTemplateExcel = new DataTable();
            try
            {
                dtTemplateExcel.Columns.Add("商品細項編號", typeof(String));
                dtTemplateExcel.Columns.Add("原料位", typeof(String));
                dtTemplateExcel.Columns.Add("新料位", typeof(String));
                DataRow newRow = dtTemplateExcel.NewRow();
                dtTemplateExcel.Rows.Add(newRow);
                string fileName = "料位轉移匯入模板_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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

        #region 收貨上架
        // 收貨上架列表頁
        public HttpResponseBase GetIinvdList()
        {
            string json = string.Empty;
            IinvdQuery iivd = new IinvdQuery();
            iivd.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            iivd.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            string content = string.Empty;
            //變更的時候記得把匯出也修改了獲取條件是同時的
            if (!string.IsNullOrEmpty(Request.Params["search_type"]))
            {
                iivd.serch_type = int.Parse(Request.Params["search_type"]);
                if (!string.IsNullOrEmpty(Request.Params["searchcontent"]) && Request.Params["searchcontent"].Trim().Length > 0)//有查詢內容就不管時間
                {

                    switch (iivd.serch_type)
                    {
                        case 1:
                        case 2:
                            iivd.serchcontent = Request.Params["searchcontent"].Trim();
                            break;
                        case 3:
                            #region 之後的更改
                            content = Request.Params["searchcontent"].Replace('，', ',').Replace('|', ',').Replace(' ', ',');//.Replace(' ',',')
                            string[] list = content.Split(',');
                            string test = "^[0-9]*$";
                            int count = 0;//實現最後一個不加,
                            for (int i = 0; i < list.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(list[i]))
                                {
                                    if (Regex.IsMatch(list[i], test))
                                    {
                                        count = count + 1;
                                        if (count == 1)
                                        {
                                            iivd.serchcontent = list[i];
                                        }
                                        else
                                        {
                                            iivd.serchcontent = iivd.serchcontent + "," + list[i];
                                        }
                                    }
                                    else
                                    {
                                        iivd.serchcontent = iivd.serchcontent + list[i] + ",";
                                    }
                                }
                            }

                            #endregion
                            break;
                        default:
                            break;
                    }
                }
            }
            DateTime time;
            if (DateTime.TryParse(Request.Params["starttime"].ToString(), out time))
            {
                iivd.starttime = DateTime.Parse(time.ToString("yyyy-MM-dd 00:00:00"));
            }
            if (DateTime.TryParse(Request.Params["endtime"].ToString(), out time))
            {
                iivd.endtime = DateTime.Parse(time.ToString("yyyy-MM-dd 23:59:59"));
            }
            try
            {
                List<IinvdQuery> store = new List<IinvdQuery>();
                _iinvd = new IinvdMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _iinvd.GetIinvdList(iivd, out  totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                foreach (var item in store)
                {
                    item.product_name += GetProductSpec(item.item_id.ToString());
                }
                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        // 收貨上架新增
        [CustomHandleError]
        public HttpResponseBase InsertIinvd()
        {
            string jsonStr = String.Empty;
            Int64 aaa = 0;  //無用變數
            uint p = 0;  //無用變數
            try
            {
                Iinvd m = new Iinvd();
                IialgQuery ia = new IialgQuery();
                Iupc iu = new Iupc();
                ProductItem proitem = new ProductItem();
                Caller call = new Caller();
                IstockChangeQuery stock = new IstockChangeQuery();
                call = (System.Web.HttpContext.Current.Session["caller"] as Caller);
                string path = "";
                _iinvd = new IinvdMgr(mySqlConnectionString);
                _iagMgr = new IialgMgr(mySqlConnectionString);
                _IiupcMgr = new IupcMgr(mySqlConnectionString);
                #region 獲取數據往
                if (Int64.TryParse(Request.Params["item_id"].ToString(), out aaa))
                {
                    if (uint.TryParse(Request.Params["item_id"].ToString(), out p))
                    {
                        m.item_id = uint.Parse(Request.Params["item_id"].ToString());
                    }
                    if (Request.Params["item_id"].ToString().Length > 6)
                    {
                        m.item_id = uint.Parse(_iinvd.Getprodubybar(Request.Params["item_id"].ToString()).Rows[0]["item_id"].ToString());
                    }
                }
                else
                {
                    if (Request.Params["item_id"].ToString().Length > 6)
                    {
                        m.item_id = uint.Parse(_iinvd.Getprodubybar(Request.Params["item_id"].ToString()).Rows[0]["item_id"].ToString());
                    }
                }

                m.dc_id = 1;
                m.whse_id = 1;
                m.prod_qty = Int32.Parse(Request.Params["prod_qty"].ToString());//數量
                DateTime today = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
                DateTime dtime;
                if (DateTime.TryParse(Request.Params["startTime"].ToString(), out dtime))
                {//用戶填寫創建時間算出有效日期
                    DateTime start = DateTime.Parse(Request.Params["startTime"].ToString());
                    m.made_date = start;
                    if (uint.TryParse(Request.Params["cde_dt_incr"].ToString(), out p))
                    {
                        m.cde_dt = start.AddDays(Int32.Parse(Request.Params["cde_dt_incr"].ToString()));
                    }
                    else
                    {
                        m.cde_dt = DateTime.Now;
                    }
                }
                else
                {
                    if (DateTime.TryParse(Request.Params["cde_dt"].ToString(), out dtime))
                    {//用戶填寫有效日期算出製造日期
                        m.cde_dt = DateTime.Parse(Request.Params["cde_dt"].ToString());//有效時間 
                        if (uint.TryParse(Request.Params["cde_dt_incr"].ToString(), out p))
                        {
                            m.made_date = m.cde_dt.AddDays(-Int32.Parse(Request.Params["cde_dt_incr"].ToString()));
                        }
                        else
                        {
                            m.made_date = today;
                        }
                    }
                    else
                    {
                        m.cde_dt = today;
                        m.made_date = today;
                    }
                }
                m.cde_dt = DateTime.Parse(m.cde_dt.ToShortDateString());
                m.made_date = DateTime.Parse(m.made_date.ToShortDateString());
                m.plas_loc_id = Request.Params["plas_loc_id"].ToString().ToUpper();//上架料位
                string loc_id = Request.Params["loc_id"].ToString().ToUpper();
                m.change_dtim = DateTime.Now;//編輯時間
                m.receipt_dtim = DateTime.Now;//收貨時間
                m.create_user = (Session["caller"] as Caller).user_id;
                #endregion
                #region 獲取數據添加打iialg
                ia.loc_id = m.plas_loc_id.ToString().ToUpper();
                ia.item_id = m.item_id;
                stock.sc_trans_type = 0;
                if (!string.IsNullOrEmpty(Request.Params["iarc_id"].ToString()))
                {
                    ia.iarc_id = Request.Params["iarc_id"].ToString();
                }
                else
                {
                    ia.iarc_id = "PC";
                    stock.sc_trans_type = 1;//收貨上架
                }
                //if (ia.iarc_id == "DR" || ia.iarc_id == "KR")
                //{
                //    type = 2;//RF理貨
                //}

                ia.create_dtim = DateTime.Now;
                ia.create_user = m.create_user;
                ia.doc_no = "P" + DateTime.Now.ToString("yyyyMMddHHmmss");
                if (!string.IsNullOrEmpty(Request.Params["doc_num"]))
                {
                    ia.doc_no = Request.Params["doc_num"];
                    stock.sc_trans_id = ia.doc_no;//交易單號
                }
                if (!string.IsNullOrEmpty(Request.Params["Po_num"]))
                {
                    ia.po_id = Request.Params["Po_num"];
                    stock.sc_cd_id = ia.po_id;//前置單號
                }
                if (!string.IsNullOrEmpty(Request.Params["remark"]))
                {
                    ia.remarks = Request.Params["remark"];
                    stock.sc_note = ia.remarks;//備註 
                }
                ia.made_dt = m.made_date;
                ia.cde_dt = m.cde_dt;
                #endregion

                #region 獲取店內條碼-=添加條碼

                if (!string.IsNullOrEmpty(Request.Params["vendor_id"].ToString()))
                {
                    iu.upc_id = CommonFunction.GetUpc(m.item_id.ToString(), Request.Params["vendor_id"].ToString(), m.cde_dt.ToString("yyMMdd"));
                }
                iu.item_id = m.item_id;
                iu.upc_type_flg = "2";//店內碼
                iu.create_user = m.create_user;
                string result = _IiupcMgr.IsExist(iu);//是否有重複的條碼
                if (result == "0")
                {
                    if (_IiupcMgr.Insert(iu) < 1)
                    {
                        jsonStr = "{success:true,msg:2}";
                    }
                }
                #endregion

                #region 新增/編輯
                #region 庫存調整的時候，商品庫存也要調整
                _proditemMgr = new ProductItemMgr(mySqlConnectionString);
                int item_stock = m.prod_qty;
                proitem.Item_Stock = item_stock;
                proitem.Item_Id = m.item_id;
                #endregion
                if (_iinvd.IsUpd(m, stock) > 0)
                {//編輯             
                    ia.qty_o = _iinvd.Selnum(m);
                    ia.adj_qty = m.prod_qty;

                    m.prod_qty = ia.qty_o + m.prod_qty;
                    if (m.prod_qty >= 0)
                    {
                        if (_iinvd.Upd(m) > 0)
                        {
                            if (Request.Params["iialg"].ToString() == "Y")
                            {// 
                                if (ia.iarc_id != "PC" && ia.iarc_id != "NE")//------------庫存調整的時候商品庫存也更改，收貨上架的時候不更改,RF理貨的時候也是不更改
                                {
                                    path = "/WareHouse/KutiaoAddorReduce";
                                    _proditemMgr.UpdateItemStock(proitem, path, call);
                                }
                                if (_iagMgr.insertiialg(ia) > 0)
                                {
                                    jsonStr = "{success:true,msg:0}";//更新成功
                                }
                                else
                                {
                                    jsonStr = "{success:false,msg:1}";//更新失敗
                                }
                            }
                            else
                            {
                                jsonStr = "{success:true,msg:0}";//更新成功
                            }
                        }
                        else
                        {
                            jsonStr = "{success:false,msg:1}";//更新失敗
                        }
                    }
                    else
                    {
                        jsonStr = "{success:false,msg:1}";//庫存為負數
                    }
                }
                else
                {//新增
                    m.ista_id = "A";
                    m.create_dtim = DateTime.Now;       //創建時間
                    if (_iinvd.Insert(m) > 0)
                    {
                        _IlocMgr = new IlocMgr(mySqlConnectionString);
                        Iloc loc = new BLL.gigade.Model.Iloc();
                        loc.change_user = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                        loc.change_dtim = DateTime.Now;
                        loc.loc_id = m.plas_loc_id.ToString().ToUpper();
                        if (loc_id.Trim() != m.plas_loc_id.Trim())//判斷如果是主料位不需要進行多餘的操作
                        {
                            if (_IlocMgr.SetIlocUsed(loc) > 0)
                            {
                                if (Request.Params["iialg"].ToString() == "Y")
                                {
                                    if (ia.iarc_id != "PC" && ia.iarc_id != "NE")//------------庫存調整的時候商品庫存也更改，收貨上架的時候不更改,RF理貨的時候也是不更改
                                    {
                                        path = "/WareHouse/KutiaoAddorReduce";
                                        _proditemMgr.UpdateItemStock(proitem, path, call);
                                    }
                                    ia.qty_o = 0;
                                    ia.adj_qty = m.prod_qty;
                                    if (_iagMgr.insertiialg(ia) > 0)
                                    {
                                        jsonStr = "{success:true,msg:0}";//更新成功
                                    }
                                    else
                                    {
                                        jsonStr = "{success:false,msg:1}";//更新失敗
                                    }
                                }
                                else
                                {
                                    jsonStr = "{success:true,msg:0}";//新增成功
                                }
                            }
                            else
                            {
                                jsonStr = "{success:false,msg:1}";//新增失敗
                            }
                        }
                        else
                        {
                            if (Request.Params["iialg"].ToString() == "Y")
                            {
                                if (ia.iarc_id != "PC" && ia.iarc_id != "NE")//------------庫存調整的時候商品庫存也更改，收貨上架的時候不更改,RF理貨的時候也是不更改
                                {
                                    path = "/WareHouse/KutiaoAddorReduce";
                                    _proditemMgr.UpdateItemStock(proitem, path, call);
                                }
                                ia.qty_o = 0;
                                ia.adj_qty = m.prod_qty;
                                if (_iagMgr.insertiialg(ia) > 0)
                                {
                                    jsonStr = "{success:true,msg:0}";//更新成功
                                }
                                else
                                {
                                    jsonStr = "{success:false,msg:1}";//更新失敗
                                }
                            }
                            else
                            {
                                jsonStr = "{success:true,msg:0}";//新增成功
                            }
                        }
                    }
                    else
                    {
                        jsonStr = "{success:false,msg:1}";
                    }
                }
                #endregion
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
        // 變更數量--丟棄
        public HttpResponseBase UpdProdqty()
        {
            Iinvd m = new Iinvd();
            IinvdLog il = new IinvdLog();
            _iinvd = new IinvdMgr(mySqlConnectionString);
            string jsonStr = String.Empty;
            StringBuilder sb = new StringBuilder();
            _iasdMgr = new AseldMgr(mySqlConnectionString);
            try
            {
                il.nvd_id = Int32.Parse(Request.Params["row_id"].ToString());
                il.change_num = Int32.Parse(Request.Params["change_num"].ToString());
                il.from_num = Int32.Parse(Request.Params["from_num"].ToString());
                il.create_user = (Session["caller"] as Caller).user_id;
                il.create_date = DateTime.Now;

                m.row_id = il.nvd_id;
                m.prod_qty = il.change_num + il.from_num;
                m.change_dtim = DateTime.Now;
                m.change_user = (Session["caller"] as Caller).user_id;
                if (m.prod_qty >= 0)
                {
                    sb.Append(_iinvd.UpdProdqty(m));
                    sb.Append(_iinvd.InsertIinvdLog(il));
                    _iasdMgr.InsertSql(sb.ToString());//執行SQL語句裡面有事物處理
                    jsonStr = "{success:true,msg:0}";
                }
                else
                {
                    jsonStr = "{success:true,msg:1}";
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
        // 用商品編號獲取品名
        public HttpResponseBase Getprodbyid()
        {
            string jsonStr = String.Empty;
            try
            {
                int id;
                _iinvd = new IinvdMgr(mySqlConnectionString);
                DataTable dt = new DataTable();
                //if (int.TryParse(Request.Params["id"].ToString(), out id) && Request.Params["id"].ToString().Length == 6)
                //{//獲取商品編號
                //    dt = _iinvd.Getprodu(int.Parse(Request.Params["id"].ToString()));
                //}
                //else
                //{//獲取條碼
                    dt = _iinvd.Getprodubybar(Request.Params["id"].ToString());
               // }
                if (dt.Rows.Count > 0)
                {//pwy_dte_ctl是否是有效期控管的商品，cde_dt_shp：允出天數，cde_dt_var：允收天數,cde_dt_incr 保存天數
                    string spec = string.Empty;
                    if (!string.IsNullOrEmpty(dt.Rows[0]["spec_name"].ToString()))
                    {
                        spec += dt.Rows[0]["spec_name"].ToString();
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[0]["spec_name1"].ToString()))
                    {
                        if (spec.Length > 0)
                        {
                            spec += ",";
                        }
                        spec += dt.Rows[0]["spec_name1"].ToString();
                    }
                    if (spec.Length > 0)
                    {
                        spec = "(" + spec + ")";
                    }
                    jsonStr = "{success:true,msg:\"" + dt.Rows[0]["product_name"] + spec + "\",locid:'" + dt.Rows[0]["loc_id"].ToString().ToUpper() + "',item_id:'" + dt.Rows[0]["item_id"] + "',day:'" + dt.Rows[0]["cde_dt_var"] + "',cde_dt_shp:'" + dt.Rows[0]["cde_dt_shp"] + "',pwy_dte_ctl:'" + dt.Rows[0]["pwy_dte_ctl"] + "',cde_dt_var:'" + dt.Rows[0]["cde_dt_var"] + "',cde_dt_incr:'" + dt.Rows[0]["cde_dt_incr"] + "',vendor_id:'" + dt.Rows[0]["vendor_id"] + "'}";//返回json數據
                }
                else
                {
                    jsonStr = "{success:false,msg:0}";//返回json數據
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
        //判斷上架料位是否存在和被佔用
        public HttpResponseBase Islocid()
        {
            string jsonStr = String.Empty;
            try
            {
                string id = Request.Params["plas_loc_id"].ToString().ToString().ToUpper();//料位 
                string zid = Request.Params["loc_id"].ToString().ToString().ToUpper();//主料位
                string prod_id = Request.Params["prod_id"].ToString();//商品編號
                _iinvd = new IinvdMgr(mySqlConnectionString);
                int msg = _iinvd.Islocid(id, zid, prod_id);

                jsonStr = "{success:true,msg:" + msg + "}";//返回json數據

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
        // 更新表Iinvd庫存鎖的狀態
        public JsonResult UpdateIinvdActive()
        {
            string jsonStr = string.Empty;
            try
            {
                _iinvd = new IinvdMgr(mySqlConnectionString);
                Iinvd nvd = new Iinvd();
                IialgQuery q = new IialgQuery();
                _iagMgr = new IialgMgr(mySqlConnectionString);

                int id = Convert.ToInt32(Request.Params["id"]);
                string active = Request.Params["active"];
                string lock_id = Request.Params["lock_id"];
                if (!string.IsNullOrEmpty(Request.Params["po_id"].ToString()))
                {
                    q.po_id = Request.Params["po_id"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["remarks"].ToString()))
                {
                    q.remarks = Request.Params["remarks"].ToString();
                }
                if (active == "H")
                {
                    nvd.ista_id = "A";
                    nvd.qity_id = 0;
                }
                else if (active == "A")
                {
                    nvd.qity_id = Convert.ToInt32(lock_id);
                    nvd.ista_id = "H";
                }
                nvd.row_id = id;
                nvd.change_user = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                nvd.change_dtim = DateTime.Now;
                if (_iinvd.UpdateIinvdLock(nvd, q) > 0)
                {
                    //加鎖成功往iialg插入一條數據;解鎖不需要記錄
                    if (active == "A")
                    {
                        Iinvd store = _iinvd.GetIinvd(nvd).FirstOrDefault();
                        q.loc_id = store.plas_loc_id;
                        q.item_id = store.item_id;
                        q.iarc_id = "KS";
                        q.qty_o = store.prod_qty;
                        q.type = 1;
                        q.adj_qty = -store.prod_qty;
                        q.create_dtim = DateTime.Now;
                        q.create_user = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                        q.made_dt = store.made_date;
                        q.cde_dt = store.cde_dt;
                        if (_iagMgr.insertiialg(q) > 0)
                        {
                            Caller call = new Caller();
                            call = (System.Web.HttpContext.Current.Session["caller"] as Caller);
                            ProductItem proitem = new ProductItem();
                            _proditemMgr = new ProductItemMgr(mySqlConnectionString);
                            int item_stock = store.prod_qty;
                            proitem.Item_Stock = -item_stock;
                            proitem.Item_Id = store.item_id;
                            string path = "/WareHouse/KutiaoAddorReduce";
                            _proditemMgr.UpdateItemStock(proitem, path, call);
                            return Json(new { success = "true" });
                        }
                        else
                        {

                            return Json(new { success = "false" });
                        }
                    }
                    else
                    {
                        Iinvd store = _iinvd.GetIinvd(nvd).FirstOrDefault();
                        q.loc_id = store.plas_loc_id;
                        q.item_id = store.item_id;
                        q.iarc_id = "KS";
                        q.qty_o = 0;
                        q.type = 1;
                        q.adj_qty = store.prod_qty;
                        q.create_dtim = DateTime.Now;
                        q.create_user = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                        q.made_dt = store.made_date;
                        q.cde_dt = store.cde_dt;
                        if (_iagMgr.insertiialg(q) > 0)
                        {
                            Caller call = new Caller();
                            call = (System.Web.HttpContext.Current.Session["caller"] as Caller);
                            ProductItem proitem = new ProductItem();
                            _proditemMgr = new ProductItemMgr(mySqlConnectionString);
                            int item_stock = store.prod_qty;
                            proitem.Item_Stock = item_stock;
                            proitem.Item_Id = store.item_id;
                            string path = "/WareHouse/KutiaoAddorReduce";
                            _proditemMgr.UpdateItemStock(proitem, path, call);
                            return Json(new { success = "true" });
                        }
                        else
                        {

                            return Json(new { success = "false" });
                        }
                    }
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
        // 收貨上架庫存鎖
        public HttpResponseBase GetWhyLock()
        {
            string json = string.Empty;
            string types = "loc_lock_msg";
            try
            {
                List<Parametersrc> store = new List<Parametersrc>();
                _psrcMgr = new ParameterMgr(mySqlConnectionString);
                store = _psrcMgr.GetAllKindType(types);

                json = "{success:true,'msg':'user',data:" + JsonConvert.SerializeObject(store) + "}";//返回json數據
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
        //收貨上架匯出
        public void IinvdExcelList()
        {
            string json = string.Empty;
            IinvdQuery iivd = new IinvdQuery();
            DataTable dtIinvdExcel = new DataTable();
            DateTime dtime = DateTime.Now;
            _IiupcMgr = new IupcMgr(mySqlConnectionString);
            try
            {
                iivd.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                iivd.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                string content = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["search_type"]))
                {
                    iivd.serch_type = int.Parse(Request.Params["search_type"]);
                    if (!string.IsNullOrEmpty(Request.Params["searchcontent"]) && Request.Params["searchcontent"].Trim().Length > 0)
                    {

                        switch (iivd.serch_type)
                        {
                            case 1:
                            case 2:
                                iivd.serchcontent = Request.Params["searchcontent"].Trim();
                                break;
                            case 3:
                                #region 之後的更改
                                content = Request.Params["searchcontent"].Replace('，', ',').Replace('|', ',').Replace(' ', ',');//.Replace(' ',',')
                                string[] list = content.Split(',');
                                string test = "^[0-9]*$";
                                int count = 0;//實現最後一個不加,
                                for (int i = 0; i < list.Length; i++)
                                {
                                    if (!string.IsNullOrEmpty(list[i]))
                                    {
                                        if (Regex.IsMatch(list[i], test))
                                        {
                                            count = count + 1;
                                            if (count == 1)
                                            {
                                                iivd.serchcontent = list[i];
                                            }
                                            else
                                            {
                                                iivd.serchcontent = iivd.serchcontent + "," + list[i];
                                            }
                                        }
                                        else
                                        {
                                            iivd.serchcontent = iivd.serchcontent + list[i] + ",";
                                        }
                                    }
                                }
                                #endregion
                                break;
                            default:
                                break;
                        }
                    }
                }
                DateTime time;
                if (DateTime.TryParse(Request.Params["starttime"].ToString(), out time))
                {
                    iivd.starttime = DateTime.Parse(time.ToString("yyyy-MM-dd 00:00:00"));
                }
                if (DateTime.TryParse(Request.Params["endtime"].ToString(), out time))
                {
                    iivd.endtime = DateTime.Parse(time.ToString("yyyy-MM-dd 23:59:59"));
                }
                List<IinvdQuery> store = new List<IinvdQuery>();
                _iinvd = new IinvdMgr(mySqlConnectionString);
                store = _iinvd.GetIinvdExprotList(iivd);
                #region 列名
                dtIinvdExcel.Columns.Add("商品編號", typeof(String));
                dtIinvdExcel.Columns.Add("商品名稱", typeof(String));
                dtIinvdExcel.Columns.Add("數量", typeof(String));
                dtIinvdExcel.Columns.Add("有效日期", typeof(String));
                dtIinvdExcel.Columns.Add("上架料位", typeof(String));
                dtIinvdExcel.Columns.Add("主料位", typeof(String));
                dtIinvdExcel.Columns.Add("允收天數", typeof(String));
                dtIinvdExcel.Columns.Add("建立日期", typeof(String));
                dtIinvdExcel.Columns.Add("建立人員", typeof(String));
                dtIinvdExcel.Columns.Add("鎖定狀態", typeof(String));
                dtIinvdExcel.Columns.Add("庫鎖原因", typeof(String));
                dtIinvdExcel.Columns.Add("店內碼", typeof(String));
                dtIinvdExcel.Columns.Add("國際碼", typeof(String));
                dtIinvdExcel.Columns.Add("庫鎖備註", typeof(String));
                #endregion
                for (int i = 0; i < store.Count; i++)
                {
                    string upc = "";
                    Iupc iu = new Iupc();
                    DataRow newRow = dtIinvdExcel.NewRow();
                    newRow[0] = store[i].item_id.ToString();
                    #region 添加店內碼
                    if (DateTime.TryParse(store[i].cde_dt.ToString(), out dtime))
                    {
                        upc = CommonFunction.GetUpc(store[i].item_id.ToString(), store[i].vendor_id.ToString(), dtime.ToString("yyMMdd"));
                    }
                    else
                    {
                        upc = CommonFunction.GetUpc(store[i].item_id.ToString(), store[i].vendor_id.ToString(), dtime.ToString("yyMMdd"));
                    }
                    iu.upc_id = upc;
                    iu.item_id = uint.Parse(store[i].item_id.ToString());
                    iu.upc_type_flg = "2";//店內碼
                    iu.create_user = (Session["caller"] as Caller).user_id;
                    string result = _IiupcMgr.IsExist(iu);//是否有重複的條碼
                    if (result == "0")
                    {
                        _IiupcMgr.Insert(iu);
                    }
                    #endregion
                    newRow[1] = store[i].product_name.ToString();
                    newRow[2] = store[i].prod_qty.ToString();
                    newRow[3] = store[i].cde_dt.ToString();
                    newRow[4] = store[i].plas_loc_id.ToString();
                    newRow[5] = store[i].loc_id.ToString();
                    newRow[6] = store[i].cde_dt_var.ToString();
                    newRow[7] = store[i].create_dtim.ToString();
                    newRow[8] = store[i].user_name.ToString();
                    newRow[9] = store[i].ista_id.ToString();
                    newRow[10] = store[i].qity_name.ToString();
                    newRow[11] = " " + upc;
                    newRow[12] = " " + store[i].upc_id.ToString();
                    iivd.item_id = uint.Parse(store[i].item_id.ToString());
                    iivd.plas_loc_id = store[i].plas_loc_id.ToString();
                    iivd.made_date = store[i].made_date;
                    if (store[i].ista_id.ToString() == "H")
                    {
                        newRow[13] = _iinvd.remark(iivd);
                    }
                    else
                    {
                        newRow[13] = "";
                    }
                    dtIinvdExcel.Rows.Add(newRow);
                }
                if (dtIinvdExcel.Rows.Count > 0)
                {
                    string fileName = "收貨上架_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtIinvdExcel, "收貨上架_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
                else
                {
                    Response.Write("匯出數據不存在");
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
        }

        //判斷某個料位的商品是否被鎖定
        public HttpResponseBase GetSearchStock()
        {
            string json = string.Empty;
            int islock = 0;
            _iinvd = new IinvdMgr(mySqlConnectionString);
            IinvdQuery query = new IinvdQuery();

            if (!string.IsNullOrEmpty(Request.Params["loc_id"]))
            {
                query.plas_loc_id = Request.Params["loc_id"];
            }
            if (!string.IsNullOrEmpty(Request.Params["item_id"]))
            {
                query.item_id = uint.Parse(Request.Params["item_id"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["cde_date"]) && Request.Params["cde_date"] != "null")
            {
                query.cde_dt = DateTime.Parse(Request.Params["cde_date"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["made_date"]) && Request.Params["made_date"] != "null")
            {
                query.made_date = DateTime.Parse(Request.Params["made_date"]);
            }
            query.ista_id = "H";
            //{
            //    plas_loc_id = Request.Params["plas_loc_id"],
            //    item_id = uint.Parse(Request.Params["item_id"]),
            //    ista_id = "H",
            //    made_date = string.IsNullOrEmpty(Request.Params["made_date"]) ? DateTime.MinValue : DateTime.Parse(Request.Params["made_date"]),
            //    cde_dt = string.IsNullOrEmpty(Request.Params["cde_dt"]) ? DateTime.MinValue : DateTime.Parse(Request.Params["cde_dt"])
            //};
            try
            {
                if (!string.IsNullOrEmpty(query.plas_loc_id))
                {
                    if (query.made_date == query.cde_dt)
                    {
                        query.cde_dt = query.made_date = DateTime.Now;
                    }
                    List<IinvdQuery> listIinvdQuery = _iinvd.GetSearchIinvd(query);



                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                    //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                    //timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                    timeConverter.DateTimeFormat = "yyyy-MM-dd";
                    if (listIinvdQuery.Count > 0)
                    {
                        islock = 1;
                    }
                    //實際能檢的庫存listIinvdQuery.Count
                    json = "{success:true,msg:\"" + islock + "\"}";
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

        #region 補貨到主料位

        /// <summary>
        /// 用商品編號或者條碼編號獲取商品名稱  並且判斷是否存在主料位 如果不存在用寄倉YY999999或者ZZ999999
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase GetProdInfo()
        {
            string jsonStr = String.Empty;
            try
            {
                string pid = Request.Params["id"].ToString();
                DataTable dt = new DataTable();
                _ipalet = new PalletMoveMgr(mySqlConnectionString);
                dt = _ipalet.GetProdInfo(pid);
                if (dt.Rows.Count > 0)
                {
                    jsonStr = "{success:true,msg:\"" + dt.Rows[0]["product_name"] + "\",locname:'" + dt.Rows[0]["loc_id"] + "',yunsongtype:'" + dt.Rows[0]["product_mode"] + "',vendor_id:'" + dt.Rows[0]["vendor_id"] + "'}";//返回json數據
                }
                else
                {
                    jsonStr = "{success:false,msg:0}";//返回json數據
                }
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

        public HttpResponseBase GetProductInfoByLocId()
        {
            string jsonStr = String.Empty;
            try
            {
                string loc_id = Request.Params["loc_id"].ToString();
                DataTable dt = new DataTable();
                _ipalet = new PalletMoveMgr(mySqlConnectionString);
                dt = _ipalet.GetProductMsgByLocId(loc_id);
                if (dt.Rows.Count > 0)
                {
                    jsonStr = "{success:true,msg:\"" + dt.Rows[0]["product_name"] + "\",item_id:'" + dt.Rows[0]["item_id"] + "',yunsongtype:'" + dt.Rows[0]["product_mode"] + "'}";//返回json數據
                }
                else
                {
                    jsonStr = "{success:false,msg:0}";//返回json數據
                }
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

        #region 左邊料位，副料位
        public HttpResponseBase GetFPalletList()
        {
            string json = string.Empty;
            IinvdQuery Iinvd = new IinvdQuery();

            if (!string.IsNullOrEmpty(Request.Params["prod_id"]))//查詢商品编号
            {
                if (Request.Params["prod_id"].Length == 6)
                {
                    Iinvd.item_id = UInt32.Parse(Request.Params["prod_id"]);
                }
                else
                {
                    Iinvd.upc_id = Request.Params["prod_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["sloc_id"]))
                {
                    Iinvd.plas_loc_id = Request.Params["sloc_id"].ToString().Trim().ToUpper();
                }
            }
            try
            {
                List<IinvdQuery> store = new List<IinvdQuery>();
                _ipalet = new PalletMoveMgr(mySqlConnectionString);
                store = _ipalet.GetPalletList(Iinvd);//查询的是副料位
                foreach (var item in store)
                {
                    item.cde_dt_make = item.made_date;
                    //if (item.pwy_dte_ctl == "N")
                    //{
                    //    item.cde_dt_make = DateTime.Now;
                    //}
                    //else
                    //{
                    //    //item.cde_dt_make = item.cde_dt.AddDays(-item.cde_dt_incr);
                    //    item.cde_dt_make = item.made_date;
                    //}
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        #region 右邊料位，主或副料位+GetSPalletList()
        public HttpResponseBase GetSPalletList()
        {
            string json = string.Empty;

            IinvdQuery Iinvd = new IinvdQuery();

            if (!string.IsNullOrEmpty(Request.Params["prod_id"]))//查詢商品编号
            {
                if (Request.Params["prod_id"].Length == 6)
                {
                    Iinvd.item_id = uint.Parse(Request.Params["prod_id"]);
                }
                else
                {
                    Iinvd.upc_id = Request.Params["prod_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["eloc_id"]))
                {
                    Iinvd.plas_loc_id = Request.Params["eloc_id"].ToString().Trim().ToUpper();
                }
            }
            try
            {
                List<IinvdQuery> store = new List<IinvdQuery>();
                _ipalet = new PalletMoveMgr(mySqlConnectionString);
                store = _ipalet.GetPalletList(Iinvd);
                foreach (var item in store)
                {
                    item.cde_dt_make = item.made_date;
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        #region 修改副料位的時間+ UpPalletTime()

        public JsonResult UpPalletTime()
        {
            try
            {
                int row_id = int.Parse(Request.Params["row_id"]);
                string cde_dt = Request.Params["cde_dt"];
                IinvdQuery invd = new IinvdQuery();
                invd.row_id = row_id;
                invd.cde_dt = DateTime.Parse(cde_dt);
                invd.made_date = invd.cde_dt.AddDays(-invd.cde_dt_incr);
                _ipalet = new PalletMoveMgr(mySqlConnectionString);
                if (_ipalet.UpPalletTime(invd) > 0)
                {
                    return Json(new { success = "true" });//修改时间
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

        #endregion

        #region 把副料位信息補到主料位
        public HttpResponseBase SavePallet()
        {
            string json = string.Empty;
            string result = string.Empty;
            int userId = (Session["caller"] as Caller).user_id;
            try
            {
                string arr = Request.Params["num"];
                IinvdQuery Iinvd = new IinvdQuery();
                _IlocMgr = new IlocMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["prod_id"]))//查詢商品编号
                {
                    if (Request.Params["prod_id"].Length == 6)
                    {
                        Iinvd.item_id = uint.Parse(Request.Params["prod_id"]);
                    }
                    else
                    {
                        Iinvd.upc_id = Request.Params["prod_id"];
                    }

                    if (!string.IsNullOrEmpty(Request.Params["sloc_id"]))
                    {
                        if (Request.Params["sloc_id"].Trim().Length > 8)//大於8表示不是正常的料位,判斷是否為hash料位
                        {
                            Iinvd.plas_loc_id = _IlocMgr.GetLocidByHash(Request.Params["sloc_id"].Trim().ToUpper());
                        }
                        else
                        {
                            Iinvd.plas_loc_id = Request.Params["sloc_id"].ToString().Trim().ToUpper();
                        }
                    }
                }
                string eloc_id = string.Empty;
                if (Request.Params["eloc_id"].Trim().ToUpper().Length > 8)
                {
                    eloc_id = _IlocMgr.GetLocidByHash(Request.Params["eloc_id"].Trim().ToUpper());
                }
                else
                {
                    eloc_id = Request.Params["eloc_id"].Trim().ToUpper();
                }

                //int[] num = Array.ConvertAll<string, int>(nums, delegate(string s) { return int.Parse(s); });
                _ipalet = new PalletMoveMgr(mySqlConnectionString);
                result = _ipalet.UpPallet(Iinvd, eloc_id, arr, userId);
                json = "{success:true,msg:\"" + result + "\"}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:\"" + result + "\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        #endregion

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
        #endregion

        #endregion

        #region 生成理货单
        //理貨列表
        public HttpResponseBase GetAseldList()
        {
            string json = string.Empty;
            OrderMasterQuery oderMaster = new OrderMasterQuery();
            oderMaster.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            oderMaster.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["type_id"]))//判斷id是不是存在.也就是說是否選擇了運送方式.當id不為0時表示選擇了運送方式.
                {

                    List<OrderMasterQuery> store = new List<OrderMasterQuery>();
                    List<OrderMasterQuery> storetwo = new List<OrderMasterQuery>();
                    _iasdMgr = new AseldMgr(mySqlConnectionString);
                    int totalCount = 0;
                    oderMaster.export_id = Convert.ToInt32(Request.Params["type_id"]);
                    if (!string.IsNullOrEmpty(Request.Params["deliver_type"]))
                    {
                        oderMaster.deliver_type = Convert.ToInt32(Request.Params["deliver_type"]);
                    }
                    string i = Request.Params["beforeradio"];
                    if (i == "false" || i == "true")
                    {
                        if (i == "false")//有調度
                        {
                            oderMaster.jdtype = 2;
                        }
                        else if (i == "true")//表示無調度
                        {
                            oderMaster.jdtype = 1;
                        }
                    }
                    else
                    {
                        oderMaster.jdtype = Convert.ToInt32(Request.Params["radio"]);//1 表示沒有調度 2 表示有調度
                    }
                    store = _iasdMgr.GetOrderMasterList(oderMaster, out  totalCount);
                    foreach (var item in store)
                    {
                        oderMaster.deliver_id = item.deliver_id;
                        storetwo = _iasdMgr.GetAllOrderDetail(oderMaster);
                        int count = 0;//寄倉品類數量
                        int dcount = 0;//調度品類數量
                        foreach (var items in storetwo)
                        {
                            //參數表 1 自出 2 寄倉 3 調度
                            if (items.product_mode == 2)
                            {
                                count = count + 1;
                                item.count = count;
                            }
                            else if (items.product_mode == 3)
                            {
                                dcount = dcount + 1;
                                item.dcount = dcount;
                            }
                        }
                        //oderMaster.Order_Id = item.Order_Id;
                        //storetwo = _iasdMgr.GetAllOrderDetail(oderMaster);
                        //int count = 0;//寄倉品類數量
                        //int dcount = 0;//調度品類數量
                        //foreach (var items in storetwo)
                        //{
                        //    //參數表 1 自出 2 寄倉 3 調度
                        //    if (items.product_mode == 2)
                        //    {
                        //        count = count + 1;
                        //        item.count = count;
                        //    }
                        //    else if (items.product_mode == 3)
                        //    {
                        //        dcount = dcount + 1;
                        //        item.dcount = dcount;
                        //    }
                        //}
                    }
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                    //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                    timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                    json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
                }
                else
                {
                    json = "{success:false,totalCount:0,data:[]}";//返回json數據
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
        //生成理货单
        public HttpResponseBase CreateTallyList()
        {
            string json = String.Empty;
            Aseld m = new Aseld();
            AseldMaster am = new AseldMaster();
            StringBuilder sql = new StringBuilder();
            List<AseldQuery> list = new List<AseldQuery>();
            _iasdMgr = new AseldMgr(mySqlConnectionString);
            _aseldmasterMgr = new AseldMasterMgr(mySqlConnectionString);

            //string order_id = Request.Params["order_id"];
            string deliver_id = Request.Params["deliver_id"];
            string fre = Request.Params["type_id"];
            int radioselect = Convert.ToInt32(Request.Params["radio"]);

            //string ticket = "";
            if (deliver_id != "" && fre != "")
            {
                DateTime dt = DateTime.Now;
                string type_id = "N";
                if (fre != "2" && fre == "92")
                {
                    type_id = "F";
                }
                string assg = type_id + dt.ToString("yyyyMMddHHmmss");
                try
                {
                    if (!string.IsNullOrEmpty(deliver_id))
                    {
                        deliver_id = deliver_id.Substring(0, deliver_id.Length - 1).ToString();
                        DataTable selDT = _iasdMgr.SelOrderDetail(deliver_id, fre, radioselect);

                        foreach (DataRow r in selDT.Rows)
                        {
                            m.ordd_id = Convert.ToInt32(r["detail_id"]);
                            _iasdMgr.ConsoleAseldBeforeInsert(Convert.ToInt32(r["detail_id"]));
                            //ticket = ticket + item.ticket_id.ToString() + ',';

                            m.deliver_id = Convert.ToInt32(r["deliver_id"]);

                            m.deliver_code = CreateDeliverCode(r["deliver_id"].ToString());

                            m.ord_id = Int32.Parse(r["order_id"].ToString());//order_id
                            m.ordd_id = Int32.Parse(r["detail_id"].ToString());//od.detail_id
                            m.cust_id = r["user_id"].ToString();//om.user_id
                            m.hzd_ind = r["item_id"].ToString(); //od.item_id
                            m.item_id = Convert.ToUInt32(r["item_id"]);//od.item_id
                            m.assg_id = assg;
                            m.prod_qty = Int32.Parse(r["buy_num"].ToString());
                            if (r["sel_loc"].ToString() == "YY999999")
                            {
                                m.sel_loc = null;
                            }
                            else
                            {
                                m.sel_loc = r["sel_loc"].ToString();// i.loc_id
                            }
                            m.curr_pal_no = int.Parse(r["order_id"].ToString() + "1");//om.order_id
                            m.wust_id = "AVL";
                            m.description = Convert.ToString(r["description"]);//od.product_name
                            m.prod_sz = r["prod_sz"].ToString();//od.product_spec_name
                            m.cust_name = r["cust_name"].ToString();//om.delivery_name
                            m.invc_id = int.Parse(r["order_id"].ToString());//om.order_id
                            m.commodity_type = r["product_mode"].ToString();//2寄倉或者3調度
                            //if (m.commodity_type == "3")
                            //{
                            //    m.sel_loc = "ZZ999999";
                            //}
                            if (Convert.ToInt32(r["item_mode"]) != 0 && Convert.ToInt32(r["parent_num"]) != 0)
                            {
                                m.out_qty = m.prod_qty * Int32.Parse(r["parent_num"].ToString());
                                m.ord_qty = m.out_qty;
                            }
                            else
                            {
                                m.out_qty = m.prod_qty;
                                m.ord_qty = m.out_qty;
                            }
                            m.upc_id = r["upc_id"].ToString();//iu.upc_id
                            m.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                            sql.Append(_iasdMgr.Insert(m));//add aseld sql
                        }
                        am.assg_id = assg;
                        am.create_time = dt;
                        am.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                        sql.AppendFormat(_aseldmasterMgr.Insert(am));//add aseld sql
                        sql.AppendFormat(_iasdMgr.UpdTicker(deliver_id));//upd deliver_id 

                        int msg = _iasdMgr.InsertSql(sql.ToString());
                        if (msg > 0)
                        {
                            json = "{success:true,assg:'" + assg + "'}";
                        }
                        else
                        {
                            json = "{success:false,msg:0}";
                        }
                    }
                    else
                    {
                        json = "{success:false,msg:0}";
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
            }
            this.Response.Clear();
            this.Response.Write(json.ToString());
            this.Response.End();
            return this.Response;
        }
        // 創建deliver_code
        public string CreateDeliverCode(string deliverCode)
        {
            StringBuilder newDeliverCode = new StringBuilder();
            newDeliverCode.Append("D");
            for (int i = 0; i < 8 - deliverCode.Length; i++)
            {
                newDeliverCode.Append("0");
            }
            newDeliverCode.Append(deliverCode);
            return newDeliverCode.ToString();
        }
        // 物流方式下拉框
        public HttpResponseBase GetAllkindType()
        {
            string json = string.Empty;
            string types = "Deliver_Store";
            try
            {
                List<Parametersrc> store = new List<Parametersrc>();
                _psrcMgr = new ParameterMgr(mySqlConnectionString);
                store = _psrcMgr.GetAllKindType(types);

                json = "{success:true,data:" + JsonConvert.SerializeObject(store) + "}";//返回json數據
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

        #region RF理貨--寄倉 and 調度商品
        #region RF理貨判斷調度商品的運送方式
        public HttpResponseBase Getfreight()
        {
            string json = string.Empty;
            Aseld q = new Aseld();
            string fre;
            try
            {
                _iasdMgr = new AseldMgr(mySqlConnectionString);
                if (!String.IsNullOrEmpty(Request.Params["number"]))//如果是新增
                {
                    q.ord_id = Convert.ToInt32(Request.Params["number"]);
                }
                fre = _iasdMgr.Getfreight(q.ord_id.ToString());
                if (!string.IsNullOrEmpty(fre))
                {
                    fre = fre.Substring(0, 1);
                }
                json = "{success:true,fre:'" + fre + "'}";//返回json數據
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
        #region RF理貨判斷走寄倉還是調度
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
                if (!String.IsNullOrEmpty(Request.Params["number"]))//如果是新增
                {
                    aseMaster.assg_id = Request.Params["number"];
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
        //通過訂單判斷在表中是否存在
        public HttpResponseBase GetAseldOrdCount()
        {
            string json = string.Empty;

            Aseld ase = new Aseld();
            int count = 0;
            try
            {
                _iasdMgr = new AseldMgr(mySqlConnectionString);
                if (!String.IsNullOrEmpty(Request.Params["number"]))//如果是新增
                {
                    ase.ord_id = Convert.ToInt32(Request.Params["number"]);
                }

                ase.change_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                ase.change_dtim = DateTime.Now;
                count = _iasdMgr.SelectCount(ase);
                if (count > 0)
                {
                    _iasdMgr.UpdateScnd(ase);
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
        #endregion
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

        #region 理貨員工作--寄倉流程
        //理貨員工作--寄倉--獲取商品數據
        public HttpResponseBase JudgeAssg()
        {//判斷寄倉或者調度
            string json = String.Empty;
            string id = Request.Params["assg_id"];
            Aseld m = new Aseld();
            List<AseldQuery> list = new List<AseldQuery>();
            _iasdMgr = new AseldMgr(mySqlConnectionString);
            try
            {
                if (id.Length > 9)
                {//獲取寄倉信息
                    m.assg_id = id;
                    list = _iasdMgr.GetAseldList(m);
                    foreach (var item in list)
                    {
                        m.seld_id = item.seld_id;
                    }
                    m.wust_id = "BSY";
                    m.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    _iasdMgr.Updwust(m);
                }
                json = "{success:true,data:" + JsonConvert.SerializeObject(list, Formatting.Indented) + "}";//返回json數據              
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
        public HttpResponseBase GetStockByProductId()
        {
            string json = string.Empty;
            int totalCount = 0;
            int islock = 0;
            _iinvd = new IinvdMgr(mySqlConnectionString);
            IinvdQuery query = new IinvdQuery()
            {
                plas_loc_id = Request.Params["loc_id"],
                ista_id = "A"
            };
            try
            {
                if (!string.IsNullOrEmpty(query.plas_loc_id))
                {
                    List<IinvdQuery> listIinvdQuery = _iinvd.GetIinvdList(query, out totalCount);

                    //處理顯示有效期控管的有效期
                    #region
                    //foreach (var item in listIinvdQuery)
                    //{
                    //    if (item.pwy_dte_ctl == "Y")
                    //    {
                    //        if (!string.IsNullOrEmpty(item.cde_dt_incr.ToString()))
                    //        {
                    //            item.cde_dt = item.made_date.AddDays(item.cde_dt_incr);
                    //        }
                    //    }
                    //}
                    #endregion

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
                        json = "{success:true,islock:'" + islock + "',totalCount:" + listIinvdQuery.Count + ",data:" + JsonConvert.SerializeObject(listIinvdQuery, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        //寄倉-庫存-輸入檢貨量
        public HttpResponseBase GetSum()
        {
            string json = string.Empty;
            _iinvd = new IinvdMgr(mySqlConnectionString);
            Iinvd i = new Iinvd();
            int S = 0;//主料位庫存
            int R = 0;//輔料位庫存
            uint item;
            try
            {
                if (Request.Params["lcat_id"].ToString() == "S")
                {
                    if (uint.TryParse(Request.Params["item_id"].ToString(), out item))
                    {
                        i.item_id = item;
                        S = _iinvd.sum(i, "S");
                        R = _iinvd.sum(i, "R");
                        json = "{success:true,S:'" + S + "',R:'" + R + "'}";//返回json數據
                    }
                }
                else if (Request.Params["lcat_id"].ToString() == "R")
                {//查詢輔料位該日期的庫存
                    if (uint.TryParse(Request.Params["item_id"].ToString(), out item))
                    {
                        i.item_id = item;
                        i.made_date = DateTime.Parse(Request.Params["made_date"].ToString());
                        R = _iinvd.sum(i, "R");
                        json = "{success:true,S:'" + S + "',R:'" + R + "'}";//返回json數據
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
            _proditemMgr = new ProductItemMgr(mySqlConnectionString);
            ProductItem Proitems = new ProductItem();
            try
            {
                if (uint.TryParse(Request.Params["item_id"].ToString(), out id))
                {//商品id
                    q.item_id = id;
                    Proitems.Item_Id = id;
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
                Caller call = new Caller();
                call = (System.Web.HttpContext.Current.Session["caller"] as Caller);
                string path = "/WareHouse/KutiaoAddorReduce";
                if (q.loc_id == "YY999999")
                {
                    json = "{success:false}";
                }
                else
                {
                    Proitems.Item_Stock = q.pnum - q.qty_o;
                    int result = _iagMgr.addIialgIstock(q);
                    if (result == 2)
                    {
                        json = "{success:true,msg:2}";
                    }
                    if (result == 100)
                    {
                        _proditemMgr.UpdateItemStock(Proitems, path, call);
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

        #endregion
        #region 理貨員工作——調度流程
        /// <summary>
        /// 撿貨——調度流程頁面加載數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetMarkTallyTW()
        {
            string json = string.Empty;
            string ord_id = Request.Params["ord_id"];
            string type = Request.Params["type"];
            AseldQuery ase = new AseldQuery();
            Aseld m = new Aseld();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["ord_id"]))
                {
                    ase.ord_id = int.Parse(Request.Params["ord_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["freight_set"]))
                {
                    ase.freight_set = Request.Params["freight_set"].Substring(0, 1);
                }
                //if (!string.IsNullOrEmpty(Request.Params["deliver_code"]))
                //{
                //    ase.deliver_code = Request.Params["deliver_code"].Substring(0, 1);
                //}
                if (!string.IsNullOrEmpty(Request.Params["item_id"]))
                {
                    if (Request.Params["item_id"].Length == 6)
                    {
                        ase.item_id = uint.Parse(Request.Params["item_id"]);
                    }
                    else
                    {
                        ase.upc_id = Request.Params["item_id"];
                    }
                }
                _iasdMgr = new AseldMgr(mySqlConnectionString);
                DataTable dt = _iasdMgr.GetOrderProductInformation(ase);
                //0代表加載工作代號
                if (type == "0")//141980010
                {
                    if (dt.Rows.Count > 0)
                    {
                        json = "\"" + "assg_id" + "\"" + ":" + "\"" + dt.Rows[0]["assg_id"].ToString() + "\"";
                        json += ",\r\n" + "\"" + "deliver_code" + "\"" + ":" + "\"" + dt.Rows[0]["deliver_code"].ToString() + "\"";
                    }
                }
                else
                {
                    if (dt.Rows.Count > 0)
                    {
                        //json = "\"" + "assg_id" + "\"" + ":" + "\"" + dr["assg_id"].ToString() + "\"";
                        if (!string.IsNullOrEmpty(dt.Rows[0]["prod_sz"].ToString()))
                        {
                            json = "\"" + "product_name" + "\"" + ":" + "\"" + dt.Rows[0]["description"].ToString() + " (" + dt.Rows[0]["prod_sz"].ToString() + ")\"";
                        }
                        else
                        {
                            json = "\"" + "product_name" + "\"" + ":" + "\"" + dt.Rows[0]["description"].ToString() + " " + dt.Rows[0]["prod_sz"].ToString() + "\"";
                        }
                        //json = "\"" + "product_name" + "\"" + ":" + "\"" + dt.Rows[0]["description"].ToString() + " " + dt.Rows[0]["prod_sz"].ToString() + "\"";
                        json += ",\r\n" + "\"" + "seld_id" + "\"" + ":" + "\"" + dt.Rows[0]["seld_id"].ToString() + "\"";//row_id.唯一的
                        json += ",\r\n" + "\"" + "ord_qty" + "\"" + ":" + "\"" + dt.Rows[0]["ord_qty"].ToString() + "\"";//訂貨量
                        json += ",\r\n" + "\"" + "out_qty" + "\"" + ":" + "\"" + dt.Rows[0]["out_qty"].ToString() + "\"";//缺貨量
                        json += ",\r\n" + "\"" + "ordd_id" + "\"" + ":" + "\"" + dt.Rows[0]["ordd_id"].ToString() + "\"";
                        json += ",\r\n" + "\"" + "deliver_id" + "\"" + ":" + "\"" + dt.Rows[0]["deliver_id"].ToString() + "\"";
                        json += ",\r\n" + "\"" + "deliver_code" + "\"" + ":" + "\"" + dt.Rows[0]["deliver_code"].ToString() + "\"";
                        json += ",\r\n" + "\"" + "note_order" + "\"" + ":" + "\"" + dt.Rows[0]["note_order"].ToString() + "\"";
                        if (!string.IsNullOrEmpty(dt.Rows[0]["cde_dt_shp"].ToString()))
                        {
                            json += ",\r\n" + "\"" + "cde_dt_shp" + "\"" + ":" + "\"" + dt.Rows[0]["cde_dt_shp"].ToString() + "\"";//商品的允出天數

                        }
                        else
                        {
                            json += ",\r\n" + "\"" + "cde_dt_shp" + "\"" + ":" + "\"" + "999999" + "\"";//商品的允出天數
                        }
                        if (!string.IsNullOrEmpty(dt.Rows[0]["pwy_dte_ctl"].ToString()))
                        {
                            json += ",\r\n" + "\"" + "pwy_dte_ctl" + "\"" + ":" + "\"" + dt.Rows[0]["pwy_dte_ctl"].ToString() + "\"";//有效期控管
                        }
                        else
                        {
                            json += ",\r\n" + "\"" + "pwy_dte_ctl" + "\"" + ":" + "\"" + "N" + "\"";//有效期控管
                        }
                        #region 查詢商品信息同時把wust_id更改為BSY
                        m.wust_id = "BSY";
                        m.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                        if (!string.IsNullOrEmpty(dt.Rows[0]["seld_id"].ToString()))
                        {
                            m.seld_id = int.Parse(dt.Rows[0]["seld_id"].ToString());
                        }
                        _iasdMgr.Updwust(m);
                        #endregion
                    }
                    else
                    {//判斷條碼輸入的是否正確
                        DataTable data = new DataTable();
                        Iupc i = new Iupc();
                        _IiupcMgr = new IupcMgr(mySqlConnectionString);
                        if (!string.IsNullOrEmpty(ase.upc_id))
                        {
                            i.upc_id = ase.upc_id;
                            data = _IiupcMgr.upcid(i);
                            if (data.Rows.Count > 0)
                            {
                                json = "\"" + "item_id" + "\"" + ":" + "\"" + data.Rows[0]["item_id"].ToString() + "\"";
                            }
                        }
                    }
                }
                json = "{success:true,data:\r\n  {\r\n" + json + "\r\n} \r\n" + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,data:\r\n  {\r\n" + "" + "\r\n} \r\n" + "}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 理貨員工作--寄倉--根據item_id 查找upc_id
        public HttpResponseBase Judgeupcid()
        {
            string jsonStr = "";
            _IiupcMgr = new IupcMgr(mySqlConnectionString);
            Iupc m = new Iupc();
            DataTable dt = new DataTable();
            uint id = 0;
            try
            {
                if (uint.TryParse(Request.Params["item_id"].ToString(), out id))
                {
                    m.item_id = id;
                    m.upc_id = Request.Params["upc_id"].ToString();
                    if (m.upc_id.ToString() == m.item_id.ToString())
                    {//如果輸入商品編號也可以通過
                        jsonStr = "{success:true}";
                    }
                    else
                    {//獲取條碼
                        if (!string.IsNullOrEmpty(m.upc_id))
                        {
                            dt = _IiupcMgr.upcid(m);
                            if (dt.Rows.Count == 0)
                            {
                                jsonStr = "{success:false,msg:2}";
                            }
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                if (dt.Rows[i]["item_id"].ToString() == id.ToString())
                                {
                                    jsonStr = "{success:true}";
                                }
                                else
                                {
                                    jsonStr = "{success:false,msg:1,itemid:" + dt.Rows[i]["item_id"].ToString() + "}";
                                }
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
        #region RF理貨記錄列表頁
        public HttpResponseBase GetIwmsRecord()
        {
            string json = string.Empty;
            int totalCount = 0;
            try
            {
                List<IwmsRrecordQuery> list = new List<IwmsRrecordQuery>();
                IwmsRrecordQuery query = new IwmsRrecordQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["oid"]))
                {
                    System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^\d+$");
                    string id = Request.Params["oid"].ToString().Trim();
                    if (rex.IsMatch(id))
                    {

                        query.item_id = uint.Parse(id);

                        if (!string.IsNullOrEmpty(Request.Params["productname"]))
                        {
                            query.product_name = Request.Params["productname"].ToString().Trim();
                        }
                        DateTime time;
                        if (DateTime.TryParse(Request.Params["time_start"], out time))
                        {
                            query.starttime = DateTime.Parse(time.ToString("yyyy-MM-dd 00:00:00"));
                        }
                        if (DateTime.TryParse(Request.Params["time_end"], out time))
                        {
                            query.endtime = DateTime.Parse(time.ToString("yyyy-MM-dd 23:59:59"));
                        }
                        if (!string.IsNullOrEmpty(Request.Params["dateSel"]))
                        {
                            query.datetype = Request.Params["dateSel"].ToString();
                        }
                        if (!string.IsNullOrEmpty(Request.Params["username"]))
                        {
                            query.user_username = Request.Params["username"].ToString().Trim();
                        }

                        _IIwmsRrecordMgr = new IwmsRrecordMgr(mySqlConnectionString);
                        list = _IIwmsRrecordMgr.GetIwmsRrecordList(query, out totalCount);
                        IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                        //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                        timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                        json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(list, Formatting.Indented, timeConverter) + "}";//返回json數據
                    }
                    else
                    {
                        json = "{success:true}";
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.Params["productname"]))
                    {
                        query.product_name = Request.Params["productname"].ToString().Trim();
                    }
                    DateTime time;
                    if (DateTime.TryParse(Request.Params["time_start"], out time))
                    {
                        query.starttime = DateTime.Parse(time.ToString("yyyy-MM-dd 00:00:00"));
                    }
                    if (DateTime.TryParse(Request.Params["time_end"], out time))
                    {
                        query.endtime = DateTime.Parse(time.ToString("yyyy-MM-dd 23:59:59"));
                    }
                    if (!string.IsNullOrEmpty(Request.Params["dateSel"]))
                    {
                        query.datetype = Request.Params["dateSel"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Params["username"]))
                    {
                        query.user_username = Request.Params["username"].ToString().Trim();
                    }

                    _IIwmsRrecordMgr = new IwmsRrecordMgr(mySqlConnectionString);
                    list = _IIwmsRrecordMgr.GetIwmsRrecordList(query, out totalCount);
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                    //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                    timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                    json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(list, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #endregion

        #region 參數設定
        //獲取數據
        public HttpResponseBase GetTPList()
        {
            string json = string.Empty;
            Parametersrc p = new Parametersrc();
            _paraMgr = new ParameterMgr(mySqlConnectionString);
            List<Parametersrc> list = new List<Parametersrc>();
            p.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            p.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            p.remark = Request.Params["searchcontent"];
            try
            {
                //p.ParameterType="wms_parameter";
                int totalCount = 0;
                list = _paraMgr.GetIialgParametersrcList(p, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(list, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        //新增
        public HttpResponseBase InsertTP()
        {
            string jsonStr = String.Empty;
            Parametersrc p = new Parametersrc();
            _paraMgr = new ParameterMgr(mySqlConnectionString);
            int id;
            try
            {
                if (int.TryParse(Request.Params["ParameterType"].ToString(), out id))
                {//新增子參數
                    p.TopValue = id.ToString();
                    //p.ParameterCode = Request.Params["parameterCode"].ToString();
                    p.ParameterType = Request.Params["Parameter_type"].ToString();
                    p.parameterName = Request.Params["parameterName"].ToString();
                }
                else
                {//新增父參數
                    p.ParameterType = "wms_parameter";
                    p.parameterName = Request.Params["ParameterType"].ToString();
                }
                p.ParameterCode = Request.Params["parameterCode"].ToString();
                if (_paraMgr.GetParametercode(p).Rows.Count == 0)
                {//同一類別的code沒有重複則可以新增                    
                    p.remark = Request.Params["remark"].ToString();
                    p.Kdate = DateTime.Now;
                    p.Kuser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                    p.Used = 1;
                    p.Sort = 0;
                    if (_paraMgr.InsertTP(p) > 0)
                    {
                        jsonStr = "{success:true,msg:0}";
                    }
                    else
                    {
                        jsonStr = "{success:true,msg:1}";//新增失敗
                    }
                }
                else
                {
                    jsonStr = "{success:true,msg:2}";//code重複
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
        //編輯
        public HttpResponseBase UpdTP()
        {
            string jsonStr = String.Empty;
            Parametersrc p = new Parametersrc();
            _paraMgr = new ParameterMgr(mySqlConnectionString);
            int id;
            try
            {
                if (int.TryParse(Request.Params["rowid"].ToString(), out id))
                {//新增子參數
                    p.Rowid = id;
                    if (!string.IsNullOrEmpty(Request.Params["parameterName"]))
                    {
                        p.parameterName = Request.Params["parameterName"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Params["parameterCode"]))
                    {
                        p.ParameterCode = Request.Params["parameterCode"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Params["ParameterType"]))
                    {
                        p.ParameterType = Request.Params["ParameterType"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Params["remark"]))
                    {
                        p.remark = Request.Params["remark"].ToString();
                    }
                    if (_paraMgr.UpdTP(p) > 0)
                    {
                        jsonStr = "{success:true,msg:0}";
                    }
                    else
                    {
                        jsonStr = "{success:true,msg:1}";
                    }
                }
                else
                {
                    jsonStr = "{success:true,msg:1}";
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
        //DeleteTp
        public HttpResponseBase DeleteTp()
        {
            string jsonStr = String.Empty;
            _paraMgr = new ParameterMgr(mySqlConnectionString);
            Parametersrc p = new Parametersrc();

            if (!String.IsNullOrEmpty(Request.Params["rowid"]))
            {
                try
                {
                    foreach (string rid in Request.Params["rowid"].ToString().Split('|'))
                    {
                        if (!string.IsNullOrEmpty(rid))
                        {
                            p.Rowid = Convert.ToInt32(rid);
                            _paraMgr.Delete(p);
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
        //新增時判斷父子類
        public HttpResponseBase GetParameter()
        {
            string json = string.Empty;
            Parametersrc p = new Parametersrc();
            _paraMgr = new ParameterMgr(mySqlConnectionString);
            List<Parametersrc> list = new List<Parametersrc>();
            try
            {
                if (Request.Params["pn"].ToString() == "2")
                {
                    if (!string.IsNullOrEmpty(Request.Params["parameterName"].ToString()))
                    {
                        p.parameterName = Request.Params["parameterName"].ToString();
                    }
                }
                p.ParameterType = "wms_parameter";
                list = _paraMgr.GetParameter(p);
                json = "{success:true,data:" + JsonConvert.SerializeObject(list, Formatting.Indented) + "}";//返回json數據
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

        #region 料位移動
        #region 料位移動查詢
        public HttpResponseBase GetIlocChangeDetailList()
        {
            string json = string.Empty;
            IlocChangeDetailQuery ilocDetailQuery = new IlocChangeDetailQuery();
            ilocDetailQuery.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            ilocDetailQuery.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            if (!string.IsNullOrEmpty(Request.Params["productids"]))
            {
                ilocDetailQuery.productids = Request.Params["productids"].Replace('，', ',').Replace('|', ',').Replace(' ', ',');
            }

            if (!string.IsNullOrEmpty(Request.Params["oldilocid"]))//model中默認為F
            {
                ilocDetailQuery.icd_old_loc_id = Request.Params["oldilocid"];
            }

            if (!string.IsNullOrEmpty(Request.Params["newilocid"]))
            {
                ilocDetailQuery.icd_new_loc_id = Request.Params["newilocid"].ToString();
            }
            DateTime time;
            if (DateTime.TryParse(Request.Params["start_time"].ToString(), out time))
            {
                ilocDetailQuery.start_time = DateTime.Parse(time.ToString("yyyy-MM-dd 00:00:00"));
            }
            if (DateTime.TryParse(Request.Params["end_time"].ToString(), out time))
            {
                ilocDetailQuery.end_time = DateTime.Parse(time.ToString("yyyy-MM-dd 23:59:59"));
            }
            if (!string.IsNullOrEmpty(Request.Params["startloc"]))
            {
                ilocDetailQuery.startloc = Request.Params["startloc"].ToUpper();
            }
            if (!string.IsNullOrEmpty(Request.Params["endloc"]))
            {
                ilocDetailQuery.endloc = Request.Params["endloc"].ToUpper() + "Z";
            }
            if (!string.IsNullOrEmpty(Request.Params["icd_status"]) && Request.Params["icd_status"] != "全部")
            {
                ilocDetailQuery.icd_status = Request.Params["icd_status"];
            }
            try
            {
                List<IlocChangeDetailQuery> store = new List<IlocChangeDetailQuery>();
                _ilocDetailMger = new IlocChangeDetailMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _ilocDetailMger.GetIlocChangeDetailList(ilocDetailQuery, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                IinvdMgr iinvdMgr = new IinvdMgr(mySqlConnectionString);
                foreach (var item in store)
                {
                    Iinvd invd = new Iinvd();
                    invd.item_id = item.icd_item_id;
                    item.prod_qty = iinvdMgr.SumProd_qty(invd);
                    if (item.prepaid == 0)
                    {
                        item.prepa_name = "否";
                    }
                    else
                    {
                        item.prepa_name = "是";
                    }
                    if (item.icd_item_id > 0)
                    {
                        item.product_sz = GetProductSpec(item.icd_item_id.ToString());
                    }

                    //if (item.pwy_dte_ctl == "Y")
                    //{
                    //    if (item.made_date.AddDays(item.cde_dt_incr) < DateTime.Now)
                    //    {
                    //        item.isgq = "是";
                    //    }
                    //    else if (item.made_date.AddDays(item.cde_dt_incr - item.cde_dt_shp) < DateTime.Now)
                    //    {
                    //        item.isjq = "是";
                    //    }
                    //    else
                    //    {
                    //        item.isgq = "否";
                    //        item.isjq = "否";
                    //    }
                    //}
                    //else
                    //{
                    //    item.isgq = "-";
                    //    item.isjq = "-";
                    //}
                }
                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        #region 料位移動記錄
        public HttpResponseBase MaterialHandling()
        {//判斷寄倉或者調度
            string json = String.Empty;
            string id = Request.Params["icd"];
            IlocChangeDetailQuery ilocDetailQuery = new IlocChangeDetailQuery();
            ilocDetailQuery.icd_id = int.Parse(id);
            DataTable _dt = new DataTable();
            _ilocDetailMger = new IlocChangeDetailMgr(mySqlConnectionString);
            _dt = _ilocDetailMger.GetIlocChangeDetailExcelList(ilocDetailQuery);
            foreach (DataRow item in _dt.Rows)
            {
                item["product_sz"] = GetProductSpec(item["icd_item_id"].ToString());
                // item.product_sz = GetProductSpec(item.icd_item_id.ToString());
            }
            try
            {
                if (_dt.Rows.Count > 0)
                {//獲取寄倉信息
                    json = "{success:true,data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented) + "}";//返回json數據            
                }
                json = "{success:true,data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented) + "}";//返回json數據              
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

        #endregion

        #region 料位移動查詢匯出
        public void IlocChangeDetailExcelList()
        {
            string json = string.Empty;
            IlocChangeDetailQuery ilocDetailQuery = new IlocChangeDetailQuery();
            DataTable dtExcel = new DataTable();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["productids"]))
                {
                    ilocDetailQuery.productids = Request.Params["productids"].Replace('，', ',').Replace('|', ',').Replace(' ', ',');
                }

                if (!string.IsNullOrEmpty(Request.Params["oldilocid"]))//model中默認為F
                {
                    ilocDetailQuery.icd_old_loc_id = Request.Params["oldilocid"];
                }

                if (!string.IsNullOrEmpty(Request.Params["newilocid"]))
                {
                    ilocDetailQuery.icd_new_loc_id = Request.Params["newilocid"].ToString();
                }
                DateTime time;

                if (DateTime.TryParse(Request.Params["start_time"].ToString(), out time))
                {
                    ilocDetailQuery.start_time = DateTime.Parse(time.ToString("yyyy-MM-dd 00:00:00"));
                }
                if (DateTime.TryParse(Request.Params["end_time"].ToString(), out time))
                {
                    ilocDetailQuery.end_time = DateTime.Parse(time.ToString("yyyy-MM-dd 23:59:59"));
                }
                if (!string.IsNullOrEmpty(Request.Params["startloc"]))
                {
                    ilocDetailQuery.startloc = Request.Params["startloc"].ToUpper();
                }
                if (!string.IsNullOrEmpty(Request.Params["endloc"]))
                {
                    ilocDetailQuery.endloc = Request.Params["endloc"].ToUpper() + "Z";
                }
                if (!string.IsNullOrEmpty(Request.Params["icd_status"]) && Request.Params["icd_status"] != "全部")
                {
                    ilocDetailQuery.icd_status = Request.Params["icd_status"];
                }
                DataTable store = new DataTable();
                _ilocDetailMger = new IlocChangeDetailMgr(mySqlConnectionString);
                store = _ilocDetailMger.GetIlocChangeDetailExcelList(ilocDetailQuery);
                dtExcel.Columns.Add("商品編號", typeof(String));
                dtExcel.Columns.Add("商品名稱", typeof(String));
                dtExcel.Columns.Add("規格", typeof(String));
                dtExcel.Columns.Add("是否買斷", typeof(String));
                dtExcel.Columns.Add("原料位編號", typeof(String));
                dtExcel.Columns.Add("新料位編號", typeof(String));
                // dtExcel.Columns.Add("製造日期", typeof(String));
                //dtExcel.Columns.Add("保存期限", typeof(String));
                dtExcel.Columns.Add("搬移日期", typeof(String));
                dtExcel.Columns.Add("庫存", typeof(String));
                dtExcel.Columns.Add("是否有效期控管", typeof(String));
                //dtExcel.Columns.Add("是否即期", typeof(String));
                //dtExcel.Columns.Add("是否過期", typeof(String));
                dtExcel.Columns.Add("允收天數", typeof(String));
                dtExcel.Columns.Add("允出天數", typeof(String));
                dtExcel.Columns.Add("搬移狀態", typeof(String));
                IinvdMgr iinvdMgr = new IinvdMgr(mySqlConnectionString);

                List<Parametersrc> stores = new List<Parametersrc>();
                _ptersrc = new ParameterMgr(mySqlConnectionString);
                stores = _ptersrc.GetElementType("icd_status");
                for (int i = 0; i < store.Rows.Count; i++)
                {
                    Iinvd invd = new Iinvd();
                    invd.item_id = uint.Parse(store.Rows[i]["icd_item_id"].ToString());
                    DataRow newRow = dtExcel.NewRow();
                    newRow[0] = store.Rows[i]["icd_item_id"].ToString();
                    newRow[1] = store.Rows[i]["product_name"].ToString();
                    newRow[2] = GetProductSpec(store.Rows[i]["icd_item_id"].ToString());
                    newRow[3] = store.Rows[i]["prepaid"].ToString() == "1" ? "是" : "否";
                    newRow[4] = store.Rows[i]["icd_old_loc_id"].ToString();
                    newRow[5] = store.Rows[i]["icd_new_loc_id"].ToString();
                    // newRow[6] = store.Rows[i]["made_date"].ToString();
                    newRow[6] = store.Rows[i]["icd_create_time"].ToString();
                    //   newRow[8] = store.Rows[i]["cde_dt"].ToString();
                    newRow[7] = iinvdMgr.SumProd_qty(invd);//store.Rows[i]["prod_qty"].ToString();
                    newRow[8] = store.Rows[i]["pwy_dte_ctl"].ToString();
                    //if (store.Rows[i]["pwy_dte_ctl"].ToString() == "Y")
                    //{
                    //    DateTime made = DateTime.Parse(store.Rows[i]["made_date"].ToString());
                    //    int incr = int.Parse(store.Rows[i]["cde_dt_incr"].ToString());
                    //    int shp = int.Parse(store.Rows[i]["cde_dt_shp"].ToString());
                    //    if (made.AddDays(incr) < DateTime.Now)
                    //    {
                    //        newRow[9] = "是";
                    //    }
                    //    else if (made.AddDays(incr - shp) < DateTime.Now)
                    //    {
                    //        newRow[10] = "是";
                    //    }
                    //    else
                    //    {
                    //        newRow[10] = "否";
                    //        newRow[9] = "否";
                    //    }
                    //}
                    //else
                    //{
                    //    newRow[9] = "";
                    //    newRow[10] = "";
                    //}
                    newRow[9] = store.Rows[i]["cde_dt_var"].ToString();
                    newRow[10] = store.Rows[i]["cde_dt_shp"].ToString();

                    for (int a = 0; a < stores.Count; a++)
                    {
                        if (stores[a].ParameterCode == store.Rows[i]["icd_status"].ToString())
                        {
                            newRow[11] = stores[a].parameterName;
                        }
                    }
                    dtExcel.Rows.Add(newRow);
                }
                if (dtExcel.Rows.Count > 0)
                {
                    string fileName = "料位移動查詢_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtExcel, "料位移動查詢_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
                else
                {
                    Response.Write("匯出數據不存在");
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
        }
        #endregion


        //料位移動之搬移
        public HttpResponseBase GetIlocChangeDetailEdit()
        {
            string json = string.Empty;
            IlocChangeDetailQuery ilocDetailQuery = new IlocChangeDetailQuery();
            _ilocDetailMger = new IlocChangeDetailMgr(mySqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["icd_id_In"]))//
                {
                    ilocDetailQuery.icd_id_In = Request.Params["icd_id_In"];
                    ilocDetailQuery.icd_id_In = ilocDetailQuery.icd_id_In.TrimEnd(',');

                }
                ilocDetailQuery.icd_modify_time = DateTime.Now;
                ilocDetailQuery.icd_modify_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                if (_ilocDetailMger.UpdateIcdStatus(ilocDetailQuery) > 0)
                {
                    json = "{success:true}";//返回json數據
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

        #region 料位帳卡
        /// <summary>
        /// 匯出料位帳卡Excel
        /// </summary>
        public void IstockChangeExcelList()
        {
            try
            {
                int totalCount = 0;
                List<IstockChangeQuery> list = new List<IstockChangeQuery>();
                IstockChangeQuery query = new IstockChangeQuery();
                if (!string.IsNullOrEmpty(Request.Params["oid"]))
                {
                    string id = Request.Params["oid"].ToString().Trim();

                    System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^\d{6}$");

                    if (rex.IsMatch(id))
                    {
                        query.item_id = uint.Parse(id);
                    }
                    else
                    {
                        query.item_upc = Request.Params["oid"].ToString().Trim();//料位和條碼不再通過長度來判斷了
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["start_time"]) && Request.Params["start_time"] != "1970-01-01")//
                {
                    query.starttime = DateTime.Parse(DateTime.Parse(Request.Params["start_time"]).ToString("yyyy-MM-dd 00:00:00"));
                }

                if (!string.IsNullOrEmpty(Request.Params["end_time"]) && Request.Params["end_time"] != "1970-01-01")
                {
                    query.endtime = DateTime.Parse(DateTime.Parse(Request.Params["end_time"]).ToString("yyyy-MM-dd 23:59:59"));

                }
                query.IsPage = false;
                _istockMgr = new IstockChangeMgr(mySqlConnectionString);
                list = _istockMgr.GetIstockChangeList(query, out totalCount);
                DataTable dtExcel = new DataTable();
                dtExcel.Columns.Add("交易類型", typeof(String));
                dtExcel.Columns.Add("商品細項編號", typeof(String));
                dtExcel.Columns.Add("商品名稱", typeof(String));
                dtExcel.Columns.Add("商品規格", typeof(String));
                dtExcel.Columns.Add("交易單號", typeof(String));
                dtExcel.Columns.Add("前置單號", typeof(String));
                dtExcel.Columns.Add("交易數量", typeof(String));
                dtExcel.Columns.Add("結餘數量", typeof(String));
                dtExcel.Columns.Add("創建日期", typeof(String));
                dtExcel.Columns.Add("管理員", typeof(String));
                dtExcel.Columns.Add("帳卡原因", typeof(String));
                dtExcel.Columns.Add("備註", typeof(String));
                for (int i = 0; i < list.Count; i++)
                {
                    DataRow dtrow = dtExcel.NewRow();
                    dtrow[0] = list[i].typename;
                    dtrow[1] = " " + list[i].item_id.ToString();
                    dtrow[2] = list[i].product_name;
                    dtrow[3] = list[i].specname;
                    dtrow[4] = " " + list[i].sc_trans_id;
                    dtrow[5] = " " + list[i].sc_cd_id.ToString();
                    dtrow[6] = list[i].sc_num_chg.ToString();
                    dtrow[7] = list[i].sc_num_new.ToString();
                    dtrow[8] = list[i].sc_time.ToString();
                    dtrow[9] = list[i].manager;
                    dtrow[10] = list[i].istockwhy;
                    dtrow[11] = list[i].sc_note;
                    dtExcel.Rows.Add(dtrow);
                }
                if (dtExcel.Rows.Count > 0)
                {
                    string fileName = "料位賬卡查詢_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    string title = list[0].item_id.ToString() + "_" + list[0].product_name + "_" + GetProductSpec(list[0].item_id.ToString()) + "_前期期末庫存:" + list[0].sc_num_new;
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtExcel, title);
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
                else
                {
                    Response.Write("匯出數據不存在");
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
        }
        //料位帳卡列表頁
        public HttpResponseBase GetIstockChange()
        {
            string json = string.Empty;
            int totalCount = 0;
            try
            {
                List<IstockChangeQuery> list = new List<IstockChangeQuery>();
                IstockChangeQuery query = new IstockChangeQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["oid"]))
                {
                    string id = Request.Params["oid"].ToString().Trim();
                   
                    System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^\d{6}$");
                    
                    if (rex.IsMatch(id))
                    {
                        query.item_id = uint.Parse(id);
                    }
                    else
                    {
                        query.item_upc = Request.Params["oid"].ToString().Trim();//料位和條碼不再通過長度來判斷了
                    }
                }
                DateTime time;
                if (DateTime.TryParse(Request.Params["time_start"], out time))
                {
                    query.starttime = DateTime.Parse(time.ToString("yyyy-MM-dd 00:00:00"));
                }
                if (DateTime.TryParse(Request.Params["time_end"], out time))
                {
                    query.endtime = DateTime.Parse(time.ToString("yyyy-MM-dd 23:59:59"));
                }

                _istockMgr = new IstockChangeMgr(mySqlConnectionString);
                list = _istockMgr.GetIstockChangeList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(list, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region GetIdiffCountBookList 獲取Idiff_count_book的信息
        public HttpResponseBase GetIdiffCountBookList()
        {
            string json = string.Empty;
            IdiffcountbookQuery idiffQuery = new IdiffcountbookQuery();
            idiffQuery.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            idiffQuery.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            if (!string.IsNullOrEmpty(Request.Params["s_job_id"].Trim()))
            {
                idiffQuery.cb_jobid = Request.Params["s_job_id"].Trim();
            }
            if (!string.IsNullOrEmpty(Request.Params["s_item_id"].Trim()))
            {
                idiffQuery.item_id = Convert.ToInt32(Request.Params["s_item_id"].Trim());
            }
            if (!string.IsNullOrEmpty(Request.Params["s_loc_id"].Trim()))
            {
                idiffQuery.loc_id = Request.Params["s_loc_id"].Trim();
            }
            try
            {
                DataTable _dtstore = new DataTable();
                IdiffcountbookMgr idiffMgr = new IdiffcountbookMgr(mySqlConnectionString);
                int totalCount = 0;
                _dtstore = idiffMgr.GetIdiffCountBookList(idiffQuery, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                IinvdMgr iinvdMgr = new IinvdMgr(mySqlConnectionString);

                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(_dtstore, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #endregion

        #region 料位報表模塊

        #region 補貨建議報表
        public void OutPalletSuggest()
        {
            IinvdQuery model = new IinvdQuery();
            _iinvd = new IinvdMgr(mySqlConnectionString);
            _ipalet = new PalletMoveMgr(mySqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["item_id"]))
                {
                    string itemid = Request.Params["item_id"];
                    DataTable dt = _ipalet.GetProdInfo(itemid);
                    if (dt.Rows.Count > 0)
                    {
                        model.item_id = uint.Parse(dt.Rows[0]["item_id"].ToString());
                    }
                    else
                    {
                        model.item_id = 0;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["locid"]))
                {
                    model.loc_id = Request.Params["locid"].ToUpper();
                }
                if (!string.IsNullOrEmpty(Request.Params["startIloc"]))
                {
                    model.startIloc = Request.Params["startIloc"].ToUpper();
                }
                if (!string.IsNullOrEmpty(Request.Params["endIloc"]))
                {
                    model.endIloc = Request.Params["endIloc"] + "Z";
                    model.endIloc = model.endIloc.ToUpper();
                }
                if (!string.IsNullOrEmpty(Request.Params["number"]))
                {
                    model.sums = int.Parse(Request.Params["number"]);
                }
                if (Request.Params["Auto"] == "true")
                {
                    model.auto = 1;
                }
                DataTable dtHZ=_iinvd.ExportExcel(model);
                if(dtHZ.Rows.Count>0)
                {
                    string fileName = "補貨建議報表_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, "補貨建議報表(FIFO;副料位到主料位_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ")");
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
                else
                {
                    Response.Clear();
                    this.Response.Write("無數據存在<br/>");
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
        }
        #endregion
        #region 即期品/過期品預告表
        public void PastProductExportlist()
        {
            string json = string.Empty;
            string fileName = "";
            string fileNametwo = "";
            IinvdQuery invd = new IinvdQuery();
            try
            {
                _iinvd = new IinvdMgr(mySqlConnectionString);
                _IiupcMgr = new IupcMgr(mySqlConnectionString);
                invd.notimeortimeout = Convert.ToInt32(Request.Params["time_type"]);
                if (!string.IsNullOrEmpty(Request.Params["startIloc"]))//model中默認為F
                {
                    invd.startIloc = Request.Params["startIloc"].ToUpper();
                }
                else
                {
                    invd.startIloc = string.Empty;
                }
                if (!string.IsNullOrEmpty(Request.Params["endIloc"]))
                {
                    invd.endIloc = Request.Params["endIloc"] + "Z";
                    invd.endIloc = invd.endIloc.ToUpper();
                }
                else
                {
                    invd.endIloc = string.Empty;
                }
                if (!string.IsNullOrEmpty(Request.Params["startDay"]))
                {
                    invd.startDay = Convert.ToInt32(Request.Params["startDay"]);
                }
                else
                {
                    invd.startDay = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["endDay"]))
                {
                    invd.endDay = Convert.ToInt32(Request.Params["endDay"]);
                }
                else
                {
                    invd.endDay = 0;
                }
                int yugao = 0;
                if (int.TryParse(Request.Params["yugaoDay"], out yugao))
                {
                    invd.yugaoDay = yugao;
                }
                else
                {
                    invd.yugaoDay = 0;
                }
                if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
                }
                DataTable dtHZ = new DataTable();
                string newExcelName = string.Empty;
                dtHZ.Columns.Add("即過", typeof(String));
                dtHZ.Columns.Add("料位", typeof(String));
                dtHZ.Columns.Add("屬性", typeof(String));
                dtHZ.Columns.Add("品號", typeof(String));
                dtHZ.Columns.Add("數量", typeof(String));
                dtHZ.Columns.Add("品名", typeof(String));
                dtHZ.Columns.Add("規格", typeof(String));
                dtHZ.Columns.Add("條碼", typeof(String));
                dtHZ.Columns.Add("允出天數", typeof(String));
                dtHZ.Columns.Add("有效日期", typeof(String));
                dtHZ.Columns.Add("是否買斷", typeof(String));
                dtHZ.Columns.Add("溫層", typeof(String));
                dtHZ.Columns.Add("即期日期", typeof(String));
                dtHZ.Columns.Add("庫存鎖", typeof(String));

                DataTable dt = new DataTable();

                dt = _iinvd.PastProductExportExcel(invd);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dtHZ.NewRow();
                    if (invd.notimeortimeout == 2)//(Convert.ToDateTime(dt.Rows[i]["cde_dt"]) <= DateTime.Now.AddDays(invd.yugaoDay))//過期有效期<=今天
                    {
                        dr[0] = "過期";
                        fileName = "過期品";
                        fileNametwo = "過期品";
                    }
                    else if (invd.notimeortimeout == 1)//(Convert.ToDateTime(dt.Rows[i]["cde_dt"]) <= DateTime.Now.AddDays(int.Parse(dt.Rows[i]["cde_dt_shp"].ToString() + invd.yugaoDay)))
                    {
                        dr[0] = "即期";
                        fileName = "即期品";
                        fileNametwo = "即期品";
                    }
                    else
                    {
                        dr[0] = "錯誤";
                    }
                    dr[1] = dt.Rows[i]["plas_loc_id"];
                    dr[2] = dt.Rows[i]["lcat_id"];
                    dr[3] = dt.Rows[i]["item_id"];
                    dr[4] = dt.Rows[i]["prod_qty"];
                    dr[5] = dt.Rows[i]["product_name"];
                    dr[6] = GetProductSpec(dt.Rows[i]["item_id"].ToString());
                    dr[7] = " " + _IiupcMgr.Getupc(dt.Rows[i]["item_id"].ToString(), "0");
                    dr[8] = dt.Rows[i]["cde_dt_shp"];
                    dr[9] = Convert.ToDateTime(dt.Rows[i]["cde_dt"]).ToString("yyyy-MM-dd");
                    dr[10] = dt.Rows[i]["prepaid"].ToString() == "1" ? "是" : "否";
                    if (dt.Rows[i]["product_freight_set"].ToString() == "1")
                    {
                        dr[11] = "常溫";
                    }
                    else if (dt.Rows[i]["product_freight_set"].ToString() == "2")
                    {
                        dr[11] = "冷凍";
                    }
                    else
                    {
                        dr[11] = "";
                    }
                    dr[12] = DateTime.Parse(dt.Rows[i]["cde_dt"].ToString()).AddDays(-int.Parse(dt.Rows[i]["cde_dt_shp"].ToString())).ToString("yyyy-MM-dd");
                    dr[13] = dt.Rows[i]["ista_id"];
                    dtHZ.Rows.Add(dr);
                }

                if (System.IO.File.Exists(newExcelName))
                {
                    System.IO.File.Delete(newExcelName);
                }
                fileName += "預告報表預告天數:" + invd.yugaoDay + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                fileNametwo += " 預告報表 預告天數:" + invd.yugaoDay + " _" + DateTime.Now.ToString("yyyyMMddHHmm");
                MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, fileNametwo);
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());
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
        #region 無條碼商品表
        public void ExportInoiupc()
        {
            string json = string.Empty;
            IplasQuery m = new IplasQuery();
            if (!string.IsNullOrEmpty(Request.Params["startIloc"]))
            {//料位開始
                m.startloc = Request.Params["startIloc"].ToUpper();
            }
            if (!string.IsNullOrEmpty(Request.Params["endIloc"]))
            {
                m.endloc = Request.Params["endIloc"] + "Z";
                m.endloc = m.endloc.ToUpper();
            }
            if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
            }
            DataTable dtHZ = new DataTable();
            string newExcelName = string.Empty;
            dtHZ.Columns.Add("料位", typeof(String));
            dtHZ.Columns.Add("屬性", typeof(String));
            dtHZ.Columns.Add("品號", typeof(String));
            dtHZ.Columns.Add("數量", typeof(String));
            dtHZ.Columns.Add("品名", typeof(String));
            dtHZ.Columns.Add("規格", typeof(String));
            dtHZ.Columns.Add("條碼", typeof(String));
            dtHZ.Columns.Add("有效日期", typeof(String));
            try
            {
                //List<IplasQuery> store = new List<IplasQuery>();
                DataTable _dt = new DataTable();
                _IiplasMgr = new IplasMgr(mySqlConnectionString);

                //store = _IiplasMgr.Export(m);
                _dt = _IiplasMgr.ExportMessage(m);
                foreach (DataRow Drow in _dt.Rows)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = Drow["loc_id"];
                    dr[1] = Drow["lcat_id"];
                    dr[2] = Drow["item_id"];
                    dr[3] = Drow["prod_qtys"];
                    dr[4] = Drow["product_name"];
                    dr[5] = Drow["prod_sz"];
                    dr[6] = " " + Drow["upc_id"];
                    if (Drow["cde_dt"].ToString() != null && Drow["cde_dt"].ToString().Trim() != "")
                    {
                        dr[7] = DateTime.Parse(Drow["cde_dt"].ToString()).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        dr[7] = "";
                    }
                    dtHZ.Rows.Add(dr);
                }
                string fileName = DateTime.Now.ToString("無條碼商品報表_yyyyMMddHHmm") + ".xls";
                MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, "無條碼商品報表_" + DateTime.Now.ToString("yyyyMMddHHmm"));
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());
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
        #region 主料位商品摘除報表
        // 廠商下拉列表
        public HttpResponseBase GetVendorName()
        {
            _IiplasMgr = new IplasMgr(mySqlConnectionString);
            List<Vendor> stores = new List<Vendor>();
            string json = string.Empty;
            try
            {
                Vendor query = new Vendor();
                stores = _IiplasMgr.VendorQueryAll(query);

                query.vendor_name_simple = "所有廠商名稱";
                stores.Insert(0, query);
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
        public void GetIlocReportList()
        {
            string json = string.Empty;
            string fileName = "主料位商品摘除報表_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            ProductQuery prod = new ProductQuery();
            prod.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            prod.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["search"]))
                {
                    prod.vendor_name_simple = Request.Params["search"];
                }
                _IiplasMgr = new IplasMgr(mySqlConnectionString);
                int totalCount = 0;
                DataTable store = _IiplasMgr.GetIlocReportList(prod, out  totalCount);
                DataTable dtHZ = new DataTable();
                string newExcelName = string.Empty;
                dtHZ.Columns.Add("主料位", typeof(String));
                dtHZ.Columns.Add("商品編號", typeof(string));
                dtHZ.Columns.Add("品號", typeof(String));
                dtHZ.Columns.Add("品名", typeof(String));
                dtHZ.Columns.Add("規格", typeof(String));
                dtHZ.Columns.Add("廠商", typeof(String));
                dtHZ.Columns.Add("條碼", typeof(String));
                dtHZ.Columns.Add("商品分類", typeof(String));
                dtHZ.Columns.Add("庫存", typeof(String));
                foreach (DataRow item in store.Rows)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = item["loc_id"];
                    dr[1] = item["product_id"];
                    dr[2] = item["item_id"];
                    dr[3] = item["Product_Name"];
                    dr[4] = item["prod_sz"];
                    dr[5] = item["vendor_name_simple"];
                    dr[6] = " " + item["upc_id"];
                    dr[7] = item["parameterName"];
                    dr[8] = item["product_qty"];
                    dtHZ.Rows.Add(dr);
                }
                MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, "主料位商品摘除報表" + "_" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());
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
        #region 大出貨報表
        public void ExportDeliveryStatement()
        {
            string json = string.Empty;
            IlocQuery iloc = new IlocQuery();
            if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
            }
            DataTable dtHZ = new DataTable();
            string counts = Request.Params["counts"];
            string searchtype = Request.Params["searchtype"];//常溫或者冷凍
            string newExcelName = string.Empty;
            dtHZ.Columns.Add("主料位", typeof(String));
            dtHZ.Columns.Add("溫層", typeof(String));
            dtHZ.Columns.Add("品號", typeof(String));
            dtHZ.Columns.Add("品名", typeof(String));
            dtHZ.Columns.Add("規格", typeof(String));
            dtHZ.Columns.Add("廠商", typeof(String));
            dtHZ.Columns.Add("數量", typeof(String));
            dtHZ.Columns.Add("寄倉/調度", typeof(String));
            try
            {
                DataTable dt = new DataTable();
                _iasdMgr = new AseldMgr(mySqlConnectionString);
                dt = _iasdMgr.ExportDeliveryStatement(Convert.ToInt32(counts), Convert.ToInt32(searchtype));

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = dt.Rows[i]["loc_id"];

                    if (Convert.ToInt32(dt.Rows[i]["export_id"]) == 2)
                    {
                        dr[1] = "常溫";
                    }
                    else if (Convert.ToInt32(dt.Rows[i]["export_id"]) == 92)
                    {
                        dr[1] = "冷凍";
                    }
                    else
                    {
                        dr[1] = "";
                    }
                    dr[2] = dt.Rows[i]["item_id"];
                    dr[3] = dt.Rows[i]["product_name"];

                    dr[4] = dt.Rows[i]["prod_sz"];
                    dr[5] = dt.Rows[i]["vendor_name_full"];

                    dr[6] = dt.Rows[i]["buy_num"];
                    if (dt.Rows[i]["product_mode"].ToString() == "2")
                    {
                        dr[7] = "寄倉";
                    }
                    else if (dt.Rows[i]["product_mode"].ToString() == "3")
                    {
                        dr[7] = "調度";
                    }
                    else
                    {
                        dr[7] = "";
                    }
                    dtHZ.Rows.Add(dr);
                }
                if (System.IO.File.Exists(newExcelName))
                {
                    System.IO.File.Delete(newExcelName);
                }
                string fileName = DateTime.Now.ToString("大出貨報表_yyyyMMddHHmm") + ".xls";
                MemoryStream ms = new MemoryStream();
                if (searchtype == "2")
                {
                    ms = ExcelHelperXhf.ExportDT(dtHZ, "常溫~大出貨報表_" + DateTime.Now.ToString("yyyyMMddHHmm"));
                }
                else
                {
                    ms = ExcelHelperXhf.ExportDT(dtHZ, "冷凍~大出貨報表_" + DateTime.Now.ToString("yyyyMMddHHmm"));
                }
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());
                //CsvHelper.ExportDataTableToCsv(dtHZ, newExcelName, colname, true);
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
        #region 可用副料位／主料位報表
        public void ExportIlocList()
        {
            string json = string.Empty;
            IlocQuery iloc = new IlocQuery();
            iloc.lcat_id = Request.Params["Ilocid_type"];
            if (!string.IsNullOrEmpty(Request.Params["startIloc"]))//model中默認為F
            {
                iloc.startiloc = Request.Params["startIloc"].ToUpper();
            }
            else
            {
                iloc.startiloc = string.Empty;
            }
            if (!string.IsNullOrEmpty(Request.Params["endIloc"]))
            {
                iloc.endiloc = Request.Params["endIloc"] + "Z";
                iloc.endiloc = iloc.endiloc.ToUpper();
            }
            else
            {
                iloc.endiloc = string.Empty;
            }

            if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
            }
            DataTable dtHZ = new DataTable();

            string newExcelName = string.Empty;
            dtHZ.Columns.Add("料位", typeof(String));
            dtHZ.Columns.Add("屬性", typeof(String));
            dtHZ.Columns.Add("狀態", typeof(String));
            dtHZ.Columns.Add("長", typeof(String));
            dtHZ.Columns.Add("寬", typeof(String));
            dtHZ.Columns.Add("主高", typeof(String));
            dtHZ.Columns.Add("副高", typeof(String));
            dtHZ.Columns.Add("類型", typeof(String));
            try
            {
                List<IlocQuery> store = new List<IlocQuery>();
                _IlocMgr = new IlocMgr(mySqlConnectionString);
                store = _IlocMgr.Export(iloc);

                foreach (var item in store)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = item.loc_id;
                    dr[1] = item.lcat_id;
                    dr[2] = item.lsta_id;

                    dr[3] = item.stk_pos_dep;
                    dr[4] = item.stk_pos_wid;

                    dr[5] = item.sel_pos_hgt;
                    dr[6] = item.rsv_pos_hgt;
                    dr[7] = item.ldes_id;
                    dtHZ.Rows.Add(dr);
                }
                if (System.IO.File.Exists(newExcelName))
                {
                    System.IO.File.Delete(newExcelName);
                }

                string fileName = "匯出可用主料位/副料位報表_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, "匯出可用主料位/副料位報表_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());
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
        #region 無主料位商品報表
        public void NoIlocReportList()
        {
            string json = string.Empty;
            string fileName = "無主料位商品報表_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            ProductQuery prod = new ProductQuery();
            prod.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            prod.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["search"]))
                {
                    prod.vendor_name_simple = Request.Params["search"];
                }
                DataTable dt = new DataTable();
                _IiplasMgr = new IplasMgr(mySqlConnectionString);
                _IiupcMgr = new IupcMgr(mySqlConnectionString);
                dt = _IiplasMgr.NoIlocReportList(prod);
                DataTable dtHZ = new DataTable();
                string newExcelName = string.Empty;
                dtHZ.Columns.Add("主料位", typeof(String));
                dtHZ.Columns.Add("商品品號", typeof(String));
                dtHZ.Columns.Add("品號", typeof(String));
                dtHZ.Columns.Add("品名", typeof(String));
                dtHZ.Columns.Add("規格", typeof(String));
                dtHZ.Columns.Add("廠商", typeof(String));
                dtHZ.Columns.Add("條碼", typeof(String));
                dtHZ.Columns.Add("內箱長", typeof(String));
                dtHZ.Columns.Add("內箱寬", typeof(String));
                dtHZ.Columns.Add("內箱高", typeof(String));
                //dtHZ.Columns.Add("商品分類", typeof(String));
                // pe.inner_pack_len,pe.inner_pack_wid,pe.inner_pack_hgt
                foreach (DataRow item in dt.Rows)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = item["loc_id"];
                    dr[1] = item["Product_Id"];
                    dr[2] = item["item_id"];
                    dr[3] = item["Product_Name"];
                    dr[4] = GetProductSpec(item["item_id"].ToString());
                    dr[5] = item["vendor_name_simple"];
                    if (string.IsNullOrEmpty(_IiupcMgr.Getupc(item["item_id"].ToString(), "0")))
                    {
                        dr[6] = "未維護";
                    }
                    else
                    {
                        dr[6] = " " + _IiupcMgr.Getupc(item["item_id"].ToString(), "0");
                    }
                    if (string.IsNullOrEmpty(item["inner_pack_len"].ToString()))
                    {
                        dr[7] = "未維護";
                    }
                    else
                    {
                        dr[7] = item["inner_pack_len"];
                    }
                    if (string.IsNullOrEmpty(item["inner_pack_wid"].ToString()))
                    {
                        dr[8] = "未維護";
                    }
                    else
                    {
                        dr[8] = item["inner_pack_wid"];
                    }
                    if (string.IsNullOrEmpty(item["inner_pack_hgt"].ToString()))
                    {
                        dr[9] = "未維護";
                    }
                    else
                    {
                        dr[9] = item["inner_pack_hgt"];
                    }
                    dtHZ.Rows.Add(dr);
                }
                MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, "無主料位商品報表(寄倉品)" + "_" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());
                #region CSV
                //string[] colname = new string[dtHZ.Columns.Count];
                //string filename = "NoIplas.csv";
                //newExcelName = Server.MapPath(excelPath) + filename;
                //for (int i = 0; i < dtHZ.Columns.Count; i++)
                //{
                //    colname[i] = dtHZ.Columns[i].ColumnName;
                //}
                //if (System.IO.File.Exists(newExcelName))
                //{
                //    System.IO.File.Delete(newExcelName);
                //}
                //CsvHelper.ExportDataTableToCsv(dtHZ, newExcelName, colname, true);
                //IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                #endregion
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
        #region 缺貨明細～未完成理貨工作報表
        public void OutUndoneJobExl()
        {
            string type = string.Empty;
            AseldQuery asd = new AseldQuery();
            DataTable dt = new DataTable();
            StringBuilder sbJobsNumber = new StringBuilder();
            _iasdMgr = new AseldMgr(mySqlConnectionString);
            try
            {
                //選擇製作總表，還是產生明細報表
                if (Request.Params["radio1"] == "true")
                {
                    type = "0";
                }
                if (Request.Params["radio2"] == "true")
                {
                    type = "1";
                    if (!string.IsNullOrEmpty(Request.Params["assg_id"]))
                    {
                        //asd.assg_id = Request["assg_id"].Replace("，",",");
                        string str = Request["assg_id"].Replace("，", ",");
                        string[] strs = str.Split(',');
                        foreach (string item in strs)
                        {
                            asd.assg_id = item;
                            sbJobsNumber.AppendFormat("'{0}',", asd.assg_id);
                        }
                    }
                }
                string jobNumbers = sbJobsNumber.ToString().TrimEnd(',');
                string fileName = string.Empty;
                MemoryStream ms = new MemoryStream();
                if (type == "0")
                {
                    fileName = "缺貨總報表_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    ms = ExcelHelperXhf.ExportDT(_iasdMgr.GetDetailOrSimple(type, jobNumbers), "總表~未完成理貨工作_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                }
                else if (type == "1")
                {
                    fileName = "缺貨明細報表_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    ms = ExcelHelperXhf.ExportDT(_iasdMgr.GetDetailOrSimple(type, jobNumbers), "缺貨明細~未完成理貨工作_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                }

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
        #endregion
        #region 庫存鎖報表
        public void ExportKucunLockList()
        {
            string json = string.Empty;
            IinvdQuery m = new IinvdQuery();
            if (!string.IsNullOrEmpty(Request.Params["startIloc"]))
            {//料位開始
                m.startIloc = Request.Params["startIloc"].ToUpper();
            }
            if (!string.IsNullOrEmpty(Request.Params["endIloc"]))
            {
                m.endIloc = Request.Params["endIloc"] + "Z";
                m.endIloc = m.endIloc.ToUpper();
            }
            _iinvd = new IinvdMgr(mySqlConnectionString);
            if (!String.IsNullOrEmpty(Request.Params["item_id"]))//料位和條碼不再通過長度來判斷了
            {
                ////if (Request.Params["item_id"].ToString().Length >= 8)
                ////{
                    DataTable dt = new DataTable();
                    dt = _iinvd.Getprodubybar(Request.Params["item_id"].ToString());
                    if (dt.Rows.Count > 0)
                    {
                        m.item_id = Convert.ToUInt32(dt.Rows[0]["item_id"].ToString());
                    }
                ////}
                    else
                    {
                        int itemid = 0;
                    }

                //m.upc_id
            }
            if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
            }
            DataTable dtHZ = new DataTable();
            string newExcelName = string.Empty;
            dtHZ.Columns.Add("料位", typeof(String));
            dtHZ.Columns.Add("數量", typeof(String));
            dtHZ.Columns.Add("商品細項編號", typeof(String));
            dtHZ.Columns.Add("品名", typeof(String));
            dtHZ.Columns.Add("規格", typeof(String));
            dtHZ.Columns.Add("有效期/FIFO", typeof(String));
            dtHZ.Columns.Add("是否買斷", typeof(String));
            dtHZ.Columns.Add("條碼", typeof(String));
            dtHZ.Columns.Add("庫鎖原因", typeof(String));
            dtHZ.Columns.Add("庫鎖備註", typeof(String));
            try
            {
                List<IinvdQuery> store = new List<IinvdQuery>();
                _iinvd = new IinvdMgr(mySqlConnectionString);
                _IiupcMgr = new IupcMgr(mySqlConnectionString);
                store = _iinvd.KucunExport(m);
                foreach (var item in store)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = item.plas_loc_id;
                    dr[1] = item.prod_qty;
                    dr[2] = item.item_id;
                    dr[3] = item.product_name;
                    dr[4] = GetProductSpec(item.item_id.ToString());
                    dr[5] = item.cde_dt.ToString("yyyy-MM-dd");
                    dr[6] = item.prepaid == 0 ? "否" : "是";
                    dr[7] = " " + _IiupcMgr.Getupc(item.item_id.ToString(), "0");
                    dr[8] = item.parameterName;
                    m.item_id = uint.Parse(item.item_id.ToString());
                    m.plas_loc_id = item.plas_loc_id.ToString();
                    m.made_date = item.made_date;
                    if (item.ista_id.ToString() == "H")
                    {
                        dr[9] = _iinvd.remark(m);
                    }
                    else
                    {
                        dr[9] = "";
                    }
                    dtHZ.Rows.Add(dr);
                }
                string fileName = DateTime.Now.ToString("庫存鎖住管理報表_yyyyMMddHHmmss") + ".xls";
                MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, "庫存鎖住管理報表_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());
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
        #region 撿貨表by料位元報表
        public void ExportAseldMessage()
        {
            string json = string.Empty;
            AseldQuery m = new AseldQuery();
            if (!string.IsNullOrEmpty(Request.Params["job_id"]))
            {//料位開始
                m.assg_id = Request.Params["job_id"];
            }
            if (!string.IsNullOrEmpty(Request.Params["aseld_type"]))
            {
                m.commodity_type = Request.Params["aseld_type"];
            }
            if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
            }
            DataTable dtHZ = new DataTable();
            string newExcelName = string.Empty;
            dtHZ.Columns.Add("出貨單號", typeof(String));
            dtHZ.Columns.Add("訂單編號", typeof(String));
            dtHZ.Columns.Add("料位", typeof(String));
            dtHZ.Columns.Add("數量", typeof(String));
            dtHZ.Columns.Add("已撿", typeof(String));
            dtHZ.Columns.Add("缺", typeof(String));
            dtHZ.Columns.Add("品號", typeof(String));
            dtHZ.Columns.Add("條碼", typeof(String));
            dtHZ.Columns.Add("商品名稱", typeof(String));
            dtHZ.Columns.Add("狀態", typeof(String));
            dtHZ.Columns.Add("規格", typeof(String));
            try
            {
                DataTable dt = new DataTable();
                _iasdMgr = new AseldMgr(mySqlConnectionString);
                _IiupcMgr = new IupcMgr(mySqlConnectionString);
                dt = _iasdMgr.ExportAseldMessage(m);
                foreach (DataRow item in dt.Rows)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = item["deliver_code"];
                    dr[1] = item["ord_id"];
                    //dr[1] = item["sel_loc"];
                    if (string.IsNullOrEmpty(item["loc_id"].ToString()))
                    {
                        switch (item["commodity_type"].ToString())
                        {
                            case "2": dr[2] = "YY999999"; break;
                            case "3": dr[2] = "ZZ999999"; break;
                        }
                    }
                    else
                    {
                        dr[2] = item["loc_id"];
                    }
                    dr[3] = item["out_qty"];
                    dr[4] = string.Empty;
                    dr[5] = string.Empty;
                    dr[6] = item["item_id"];
                    dr[7] = " " + _IiupcMgr.Getupc(item["item_id"].ToString(), "0");
                    dr[8] = item["product_name"];
                    dr[9] = item["wust_id"];
                    dr[10] = item["prod_sz"];
                    dtHZ.Rows.Add(dr);
                }
                string str = "撿貨報表by料位";
                if (!string.IsNullOrEmpty(m.assg_id))
                {
                    if (m.assg_id.IndexOf('N') == 0)
                    {
                        str = m.assg_id + "(常溫/";
                    }
                    else if (m.assg_id.IndexOf('N') != 0 && m.assg_id.IndexOf('F') == 0)
                    {
                        str = m.assg_id + "(冷凍/";
                    }
                }
                if (m.commodity_type == "2")
                {
                    str = str + "寄倉)";
                }
                else if (m.commodity_type == "3")
                {
                    str = str + "調度)";
                }
                string fileName = str + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xls";
                MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, str + "_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());
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
        #region 料位盤點報表
        public HttpResponseBase GetCountBook1()
        {
            // DataSet.Tables["XX"].Columns["xx"].ColumnName = "NewColumnName";
            string json = string.Empty;
            IinvdQuery m = new IinvdQuery();
            CbjobMaster cm = new CbjobMaster();
            CbjobDetail cd = new CbjobDetail();
            IinvdMgr iinvdMgr = new IinvdMgr(mySqlConnectionString);
            _IiupcMgr = new IupcMgr(mySqlConnectionString);
            try
            {
                #region 條件
                if (!string.IsNullOrEmpty(Request.Params["startIloc"]))
                {//料位開始
                    m.startIloc = Request.Params["startIloc"].ToUpper();
                }
                if (!string.IsNullOrEmpty(Request.Params["endIloc"]))
                {
                    m.endIloc = Request.Params["endIloc"] + "Z";
                    m.endIloc = m.endIloc.ToUpper();
                }
                if (!string.IsNullOrEmpty(Request.Params["level"]))//層數
                {//層數選擇
                    m.lot_no = Request.Params["level"].ToString().ToUpper();
                }
                if (!string.IsNullOrEmpty(Request.Params["sort"]))//排序
                {//排序方式
                    m.Sort = Request.Params["sort"];
                    if (m.Sort == "0" && !string.IsNullOrEmpty(Request.Params["firstsd"]))
                    {
                        m.Firstsd = Request.Params["firstsd"];
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["vender"]))
                {//vender
                    m.vender = Request.Params["vender"].ToString().ToUpper();
                }
                #endregion
                DataTable dt = iinvdMgr.CountBook(m);
                if (dt.Rows.Count > 0)
                {
                    //查出數據循環插入數據
                    StringBuilder sb = new StringBuilder();
                    _cbjobMgr = new CbjobDetailMgr(mySqlConnectionString);
                    _cbMasterMgr = new CbjobMasterMgr(mySqlConnectionString);
                    cm.cbjob_id = "PC" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    cm.create_datetime = DateTime.Now;
                    cm.sta_id = "CNT";
                    cm.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    sb.Append(_cbMasterMgr.Insertsql(cm));//往CbjobMaster插入數據
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        cd.cb_jobid = cm.cbjob_id;
                        cd.cb_newid = i + 1;
                        cd.chang_user = cm.create_user;
                        cd.change_datetime = DateTime.Now;
                        cd.create_datetime = DateTime.Now;
                        cd.create_user = cm.create_user;
                        cd.status = 1;
                        if (!string.IsNullOrEmpty(dt.Rows[i]["row_id"].ToString()))
                        {
                            cd.iinvd_id = int.Parse(dt.Rows[i]["row_id"].ToString());
                            sb.AppendFormat("set sql_safe_updates = 0; UPDATE iinvd set st_qty=prod_qty where row_id='{0}'; set sql_safe_updates = 1;", cd.iinvd_id);
                        }
                        else
                        {
                            cd.iinvd_id = 0;
                        }
                        sb.Append(_cbjobMgr.insertsql(cd));
                    }
                    _cbjobMgr.InsertSql(sb.ToString());

                    m.cb_jobid = cm.cbjob_id;
                    dt = iinvdMgr.CountBook(m);
                    #region 列名
                    DataTable dtCountBook = new DataTable();
                    dtCountBook.Columns.Add("編號", typeof(String));
                    dtCountBook.Columns.Add("料位", typeof(String));
                    dtCountBook.Columns.Add("狀態", typeof(String));
                    dtCountBook.Columns.Add("商品細項編號", typeof(String));
                    dtCountBook.Columns.Add("庫存", typeof(String));
                    dtCountBook.Columns.Add("成本", typeof(String));
                    dtCountBook.Columns.Add("商品名稱", typeof(String));
                    dtCountBook.Columns.Add("商品規格", typeof(String));
                    dtCountBook.Columns.Add("是否買斷", typeof(String));
                    dtCountBook.Columns.Add("製造日期", typeof(String));
                    dtCountBook.Columns.Add("保存期限", typeof(String));
                    dtCountBook.Columns.Add("有效日期", typeof(String));
                    dtCountBook.Columns.Add("是否有效期控制", typeof(String));
                    dtCountBook.Columns.Add("是否即期", typeof(String));
                    dtCountBook.Columns.Add("是否過期", typeof(String));
                    dtCountBook.Columns.Add("允收天數", typeof(String));
                    dtCountBook.Columns.Add("允出天數", typeof(String));
                    dtCountBook.Columns.Add("國際條碼", typeof(String));
                    dtCountBook.Columns.Add("最新店內碼", typeof(String));
                    #endregion
                    #region 讀取iinvd裡面的數據
                    int bh = 1;
                    foreach (DataRow item in dt.Rows)
                    {
                        DataRow dr = dtCountBook.NewRow();
                        if (!string.IsNullOrEmpty(item["cb_newid"].ToString()))
                        {
                            dr[0] = item["cb_newid"];
                        }
                        else
                        {
                            dr[0] = bh;
                        }
                        dr[1] = item["loc_id"];
                        dr[2] = item["lsta_id"];
                        if (!string.IsNullOrEmpty(item["item_id"].ToString()))
                        {
                            dr[3] = item["item_id"];
                        }
                        if (!string.IsNullOrEmpty(item["prod_qty"].ToString()))
                        {
                            dr[4] = item["prod_qty"];
                        }
                        else
                        {
                            dr[4] = "0";
                        }
                        if (!string.IsNullOrEmpty(item["product_id"].ToString()))
                        {
                            dr[5] = iinvdMgr.Getcost(item["product_id"].ToString());
                        }
                        dr[6] = item["product_name"];
                        dr[7] = GetProductSpec(item["item_id"].ToString());
                        dr[8] = item["prepaid"].ToString() == "0" ? "否" : "是";
                        DateTime md;
                        if (DateTime.TryParse(item["made_date"].ToString(), out md))
                        {
                            dr[9] = DateTime.Parse(item["made_date"].ToString()).ToString("yyyy/MM/dd");
                        }
                        dr[10] = item["cde_dt_incr"];
                        DateTime cdate;
                        if (DateTime.TryParse(item["cde_dt"].ToString(), out cdate))
                        {
                            dr[11] = cdate.ToString("yyyy/MM/dd");
                        }
                        int shp = 0;
                        if (item["pwy_dte_ctl"].ToString() == "Y" && Int32.TryParse(item["cde_dt_shp"].ToString(), out shp))//表示是有效期控管的商品
                        {
                            dr[12] = "Y";
                            if (cdate.AddDays(-shp) <= DateTime.Now && cdate >= DateTime.Now)
                            {
                                dr[13] = "Y";
                            }
                            else
                            {
                                dr[13] = "N";
                            }
                            if (cdate < DateTime.Now)
                            {
                                dr[14] = "Y";
                            }
                            else
                            {
                                dr[14] = "N";
                            }
                            dr[15] = item["cde_dt_shp"];//允出天數
                            dr[16] = item["cde_dt_var"];
                        }
                        else if (item["pwy_dte_ctl"].ToString() == "N")
                        {
                            dr[12] = "N";
                            dr[13] = "";
                            dr[14] = "";
                            dr[15] = 0;//允出天數
                            dr[16] = 0;
                        }
                        else
                        {
                            dr[12] = "";
                            dr[13] = "";
                            dr[14] = "";
                            dr[15] = 0;//允出天數
                            dr[16] = 0;
                        }
                        dr[17] = " " + _IiupcMgr.Getupc(item["item_id"].ToString(), "1");
                        dr[18] = " " + _IiupcMgr.Getupc(item["item_id"].ToString(), "2");
                        dtCountBook.Rows.Add(dr);
                        bh++;
                    }
                    #endregion
                    #region iinvd沒有的料位信息從iplas表查出
                    DataTable dti = iinvdMgr.Getloc();
                    foreach (DataRow item in dti.Rows)
                    {
                        DataRow dr = dtCountBook.NewRow();
                        m.loc_id = item["loc_id"].ToString();
                        DataTable dt1 = iinvdMgr.GetIplasCountBook(m);
                        foreach (DataRow item1 in dt1.Rows)
                        {
                            dr[0] = bh;
                            dr[1] = item["loc_id"];
                            dr[2] = item1["lsta_id"];
                            if (!string.IsNullOrEmpty(item1["item_id"].ToString()))
                            {
                                dr[3] = item1["item_id"];
                            }
                            if (!string.IsNullOrEmpty(item1["prod_qty"].ToString()))
                            {
                                dr[4] = item1["prod_qty"];
                            }
                            else
                            {
                                dr[4] = "0";
                            }
                            if (!string.IsNullOrEmpty(item1["product_id"].ToString()))
                            {
                                dr[5] = iinvdMgr.Getcost(item1["product_id"].ToString());
                            }
                            else
                            {
                                dr[5] = "0";
                            }
                            dr[6] = item1["product_name"];
                            dr[7] = GetProductSpec(item1["item_id"].ToString());
                            dr[8] = item1["prepaid"].ToString() == "0" ? "否" : "是";
                            dr[9] = "";
                            dr[10] = item1["cde_dt_incr"];
                            dr[11] = "";
                            if (item1["pwy_dte_ctl"].ToString() == "Y")//表示是有效期控管的商品
                            {
                                dr[12] = "Y";
                                dr[13] = "N";
                                dr[14] = "N";
                                dr[15] = item1["cde_dt_shp"];//允出天數
                                dr[16] = item1["cde_dt_var"];
                            }
                            else if (item1["pwy_dte_ctl"].ToString() == "N")
                            {
                                dr[12] = "N";
                                dr[13] = "";
                                dr[14] = "";
                                dr[15] = 0;//允出天數
                                dr[16] = 0;
                            }
                            else
                            {
                                dr[12] = "";
                                dr[13] = "";
                                dr[14] = "";
                                dr[15] = 0;//允出天數
                                dr[16] = 0;
                            }
                            dr[17] = " " + _IiupcMgr.Getupc(item1["item_id"].ToString(), "1");
                            dr[18] = " " + _IiupcMgr.Getupc(item1["item_id"].ToString(), "2");
                            dtCountBook.Rows.Add(dr);
                            bh++;
                        }
                    }
                    #endregion
                    string fileName = "盤點簿" + cm.cbjob_id.Substring(2, 14) + ".xls";
                    String str = "盤點簿報表-" + cm.cbjob_id;
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtCountBook, str);
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());

                }
                else
                {
                    Response.Clear();
                    this.Response.Write("沒有數據<br/>");
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return this.Response;
        }

        public HttpResponseBase GetCountBook()
        {
            // DataSet.Tables["XX"].Columns["xx"].ColumnName = "NewColumnName";
            string json = string.Empty;
            IinvdQuery m = new IinvdQuery();
            CbjobMaster cm = new CbjobMaster();
            CbjobDetail cd = new CbjobDetail();
            IinvdMgr iinvdMgr = new IinvdMgr(mySqlConnectionString);
            _IiupcMgr = new IupcMgr(mySqlConnectionString);
            string cbjob_id = "PC" + DateTime.Now.ToString("yyyyMMddHHmmss");
            try
            {
                #region 條件
                if (!string.IsNullOrEmpty(Request.Params["startIloc"]))
                {//料位開始
                    m.startIloc = Request.Params["startIloc"].ToUpper();
                }
                if (!string.IsNullOrEmpty(Request.Params["endIloc"]))
                {
                    m.endIloc = Request.Params["endIloc"] + "Z";
                    m.endIloc = m.endIloc.ToUpper();
                }
                if (!string.IsNullOrEmpty(Request.Params["level"]))//層數
                {//層數選擇
                    m.lot_no = Request.Params["level"].ToString().ToUpper();
                }
                if (!string.IsNullOrEmpty(Request.Params["sort"]))//排序
                {//排序方式
                    m.Sort = Request.Params["sort"];
                    if (m.Sort == "0" && !string.IsNullOrEmpty(Request.Params["firstsd"]))
                    {
                        m.Firstsd = Request.Params["firstsd"];
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["vender"]))
                {//vender
                    m.vender = Request.Params["vender"].ToString().ToUpper();
                }
                if (!string.IsNullOrEmpty(Request.Params["prepaid"]))
                {
                    m.prepaid = int.Parse(Request.Params["prepaid"]);
                }
                #endregion
                DataTable dt = iinvdMgr.getproduct(m);
                #region 列名
                DataTable dtCountBook = new DataTable();
                dtCountBook.Columns.Add("編號", typeof(String));
                dtCountBook.Columns.Add("料位", typeof(String));
                dtCountBook.Columns.Add("狀態", typeof(String));
                dtCountBook.Columns.Add("商品細項編號", typeof(String));
                dtCountBook.Columns.Add("庫存", typeof(String));
                dtCountBook.Columns.Add("成本", typeof(String));
                dtCountBook.Columns.Add("商品名稱", typeof(String));
                dtCountBook.Columns.Add("商品規格", typeof(String));
                dtCountBook.Columns.Add("是否買斷", typeof(String));
                dtCountBook.Columns.Add("製造日期", typeof(String));
                dtCountBook.Columns.Add("保存期限", typeof(String));
                dtCountBook.Columns.Add("有效日期", typeof(String));
                dtCountBook.Columns.Add("是否有效期控制", typeof(String));
                dtCountBook.Columns.Add("是否即期", typeof(String));
                dtCountBook.Columns.Add("是否過期", typeof(String));
                dtCountBook.Columns.Add("允出天數", typeof(String));
                dtCountBook.Columns.Add("允收天數", typeof(String));
                dtCountBook.Columns.Add("國際條碼", typeof(String));
                dtCountBook.Columns.Add("最新店內碼", typeof(String));
                #endregion

                if (dt.Rows.Count > 0)
                {
                    #region 往cbjob_master裡面插入一條數據
                    StringBuilder sb = new StringBuilder();
                    _cbjobMgr = new CbjobDetailMgr(mySqlConnectionString);
                    _cbMasterMgr = new CbjobMasterMgr(mySqlConnectionString);
                    cm.cbjob_id = cbjob_id;
                    cm.create_datetime = DateTime.Now;
                    cm.sta_id = "CNT";
                    cm.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    sb.Append(_cbMasterMgr.Insertsql(cm));
                    #endregion
                    #region 修改iinvd數據,往cbjob_detail循環插入數據
                    int a = 0;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(dt.Rows[i]["row_id"].ToString()))
                        {
                            cd.cb_jobid = cm.cbjob_id;
                            cd.cb_newid = a + 1;
                            cd.chang_user = cm.create_user;
                            cd.change_datetime = DateTime.Now;
                            cd.create_datetime = DateTime.Now;
                            cd.create_user = cm.create_user;
                            cd.status = 1;
                            cd.iinvd_id = int.Parse(dt.Rows[i]["row_id"].ToString());
                            sb.AppendFormat("set sql_safe_updates = 0; UPDATE iinvd set st_qty=prod_qty where row_id='{0}'; set sql_safe_updates = 1;", cd.iinvd_id);
                            sb.Append(_cbjobMgr.insertsql(cd));
                            a++;
                        }
                    }
                    _cbjobMgr.InsertSql(sb.ToString());
                    #endregion
                    m.cb_jobid = cm.cbjob_id;//賦值給m
                    //dt = iinvdMgr.CountBook(m);
                    int bh = 1;
                    string loc = "";
                    foreach (DataRow item2 in dt.Rows)
                    {

                        if (item2["loc_id"].ToString() != loc)
                        {
                            if (!string.IsNullOrEmpty(item2["row_id"].ToString()))
                            {
                                #region 讀取iinvd裡面的數據
                                m.loc_id = item2["loc_id"].ToString();
                                DataTable Invdt = iinvdMgr.GetIinvdCountBook(m);
                                foreach (DataRow item in Invdt.Rows)
                                {
                                    DataRow dr = dtCountBook.NewRow();
                                    dr[0] = bh;
                                    dr[1] = item["loc_id"];
                                    dr[2] = item["lsta_id"];
                                    if (!string.IsNullOrEmpty(item["item_id"].ToString()))
                                    {
                                        dr[3] = item["item_id"];
                                    }
                                    if (!string.IsNullOrEmpty(item["prod_qty"].ToString()))
                                    {
                                        dr[4] = item["prod_qty"];
                                    }
                                    else
                                    {
                                        dr[4] = "0";
                                    }
                                    if (!string.IsNullOrEmpty(item["product_id"].ToString()))
                                    {
                                        dr[5] = iinvdMgr.Getcost(item["product_id"].ToString());
                                    }
                                    dr[6] = item["product_name"];
                                    dr[7] = GetProductSpec(item["item_id"].ToString());
                                    dr[8] = item["prepaid"].ToString() == "0" ? "否" : "是";
                                    DateTime md;
                                    if (DateTime.TryParse(item["made_date"].ToString(), out md))
                                    {
                                        dr[9] = DateTime.Parse(item["made_date"].ToString()).ToString("yyyy/MM/dd");
                                    }
                                    dr[10] = item["cde_dt_incr"];
                                    DateTime cdate;
                                    if (DateTime.TryParse(item["cde_dt"].ToString(), out cdate))
                                    {
                                        dr[11] = cdate.ToString("yyyy/MM/dd");
                                    }
                                    int shp = 0;
                                    if (item["pwy_dte_ctl"].ToString() == "Y" && Int32.TryParse(item["cde_dt_shp"].ToString(), out shp))//表示是有效期控管的商品
                                    {
                                        dr[12] = "Y";
                                        if (cdate.AddDays(-shp) <= DateTime.Now && cdate >= DateTime.Now)
                                        {
                                            dr[13] = "Y";
                                        }
                                        else
                                        {
                                            dr[13] = "N";
                                        }
                                        if (cdate < DateTime.Now)
                                        {
                                            dr[14] = "Y";
                                        }
                                        else
                                        {
                                            dr[14] = "N";
                                        }
                                        dr[15] = item["cde_dt_shp"];//允出天數
                                        dr[16] = item["cde_dt_var"];
                                    }
                                    else if (item["pwy_dte_ctl"].ToString() == "N")
                                    {
                                        dr[12] = "N";
                                        dr[13] = "";
                                        dr[14] = "";
                                        dr[15] = 0;//允出天數
                                        dr[16] = 0;
                                    }
                                    else
                                    {
                                        dr[12] = "";
                                        dr[13] = "";
                                        dr[14] = "";
                                        dr[15] = 0;//允出天數
                                        dr[16] = 0;
                                    }
                                    dr[17] = " " + _IiupcMgr.Getupc(item["item_id"].ToString(), "1");
                                    dr[18] = " " + _IiupcMgr.Getupc(item["item_id"].ToString(), "2");
                                    dtCountBook.Rows.Add(dr);
                                    bh++;
                                }
                                #endregion
                            }
                            else
                            {
                                #region iinvd沒有的料位信息從iplas表查出
                                DataRow dr = dtCountBook.NewRow();
                                m.loc_id = item2["loc_id"].ToString();
                                DataTable dt1 = iinvdMgr.GetIplasCountBook(m);
                                foreach (DataRow item1 in dt1.Rows)
                                {
                                    dr[0] = bh;
                                    dr[1] = item2["loc_id"];
                                    dr[2] = item1["lsta_id"];
                                    if (!string.IsNullOrEmpty(item1["item_id"].ToString()))
                                    {
                                        dr[3] = item1["item_id"];
                                    }
                                    if (!string.IsNullOrEmpty(item1["prod_qty"].ToString()))
                                    {
                                        dr[4] = item1["prod_qty"];
                                    }
                                    else
                                    {
                                        dr[4] = "0";
                                    }
                                    if (!string.IsNullOrEmpty(item1["product_id"].ToString()))
                                    {
                                        dr[5] = iinvdMgr.Getcost(item1["product_id"].ToString());
                                    }
                                    else
                                    {
                                        dr[5] = "0";
                                    }
                                    dr[6] = item1["product_name"];
                                    dr[7] = GetProductSpec(item1["item_id"].ToString());
                                    dr[8] = item1["prepaid"].ToString() == "0" ? "否" : "是";
                                    dr[9] = "";
                                    dr[10] = item1["cde_dt_incr"];
                                    dr[11] = "";
                                    if (item1["pwy_dte_ctl"].ToString() == "Y")//表示是有效期控管的商品
                                    {
                                        dr[12] = "Y";
                                        dr[13] = "N";
                                        dr[14] = "N";
                                        dr[15] = item1["cde_dt_shp"];//允出天數
                                        dr[16] = item1["cde_dt_var"];
                                    }
                                    else if (item1["pwy_dte_ctl"].ToString() == "N")
                                    {
                                        dr[12] = "N";
                                        dr[13] = "";
                                        dr[14] = "";
                                        dr[15] = 0;//允出天數
                                        dr[16] = 0;
                                    }
                                    else
                                    {
                                        dr[12] = "";
                                        dr[13] = "";
                                        dr[14] = "";
                                        dr[15] = 0;//允出天數
                                        dr[16] = 0;
                                    }
                                    dr[17] = " " + _IiupcMgr.Getupc(item1["item_id"].ToString(), "1");
                                    dr[18] = " " + _IiupcMgr.Getupc(item1["item_id"].ToString(), "2");
                                    dtCountBook.Rows.Add(dr);
                                    bh++;
                                }
                                #endregion
                            }
                            loc = item2["loc_id"].ToString();
                        }
                    }
                    string fileName = "盤點簿" + cm.cbjob_id.Substring(2, 14) + ".xls";
                    String str = "盤點簿報表-" + cm.cbjob_id;
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtCountBook, str);
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
                else
                {
                    Response.Clear();
                    this.Response.Write("沒有數據<br/>");
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return this.Response;
        }


        #endregion
        #region 料位盤點差異匯出
        public HttpResponseBase GetDifCountBook()
        {
            string json = string.Empty;
            IinvdQuery m = new IinvdQuery();
            CbjobMaster cm = new CbjobMaster();
            CbjobDetail cd = new CbjobDetail();
            try
            {
                #region 條件
                if (!string.IsNullOrEmpty(Request.Params["startIloc"]))
                {//料位開始
                    m.startIloc = Request.Params["startIloc"].ToUpper();
                }
                if (!string.IsNullOrEmpty(Request.Params["endIloc"]))
                {
                    m.endIloc = Request.Params["endIloc"] + "Z";
                    m.endIloc = m.endIloc.ToUpper();
                }
                if (!string.IsNullOrEmpty(Request.Params["startcost"]))//金額
                {
                    m.startcost = int.Parse(Request.Params["startcost"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["endcost"]))//金額
                {
                    m.endcost = int.Parse(Request.Params["endcost"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["startsum"]))//數量
                {
                    m.startsum = int.Parse(Request.Params["startsum"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["endsum"]))//數量
                {
                    m.endsum = int.Parse(Request.Params["endsum"]);
                }
                #endregion

                _iinvd = new IinvdMgr(mySqlConnectionString);
                DataTable dt = _iinvd.DifCountBook(m);
                if (dt.Rows.Count > 0)
                {
                    #region 生成報表
                    DataTable dtCountBook = new DataTable();
                    dtCountBook.Columns.Add("編號", typeof(String));
                    dtCountBook.Columns.Add("料位", typeof(String));
                    dtCountBook.Columns.Add("狀態", typeof(String));
                    dtCountBook.Columns.Add("商品細項編號", typeof(String));
                    dtCountBook.Columns.Add("成本", typeof(String));
                    dtCountBook.Columns.Add("商品名稱", typeof(String));
                    dtCountBook.Columns.Add("商品規格", typeof(String));
                    dtCountBook.Columns.Add("製造日期", typeof(String));
                    dtCountBook.Columns.Add("有效日期", typeof(String));
                    dtCountBook.Columns.Add("原有數量", typeof(String));
                    dtCountBook.Columns.Add("現有數量", typeof(String));
                    dtCountBook.Columns.Add("差異數量(%)", typeof(String));
                    dtCountBook.Columns.Add("差異金額", typeof(String));
                    foreach (DataRow item in dt.Rows)
                    {
                        DataRow dr = dtCountBook.NewRow();
                        dr[0] = item["row_id"];
                        dr[1] = item["loc_id"];
                        dr[2] = item["lsta_id"];
                        dr[3] = item["item_id"];
                        dr[4] = item["cost"];
                        dr[5] = item["product_name"];
                        dr[6] = GetProductSpec(item["item_id"].ToString());
                        dr[7] = DateTime.Parse(item["made_date"].ToString()).ToString("yyyy/MM/dd");
                        dr[8] = DateTime.Parse(item["cde_dt"].ToString()).ToString("yyyy/MM/dd");
                        dr[9] = item["prod_qty"];
                        dr[10] = item["st_qty"];
                        dr[11] = item["qty"];
                        dr[12] = item["money"];
                        dtCountBook.Rows.Add(dr);
                    }
                    #endregion
                    string fileName = "盤點差異報表" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xls";
                    String str = "盤點差異報表" + DateTime.Now.ToString("_yyyyMMddHHmmss");
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtCountBook, str);
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
                else
                {
                    Response.Clear();
                    this.Response.Write("該範圍內沒有差異數據<br/>");
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return this.Response;
        }
        #endregion
        #region 料位盤點差異匯出OBK
        public HttpResponseBase GetCountBookOBK()
        {
            string json = string.Empty;
            IinvdQuery m = new IinvdQuery();
            CbjobMaster cm = new CbjobMaster();
            CbjobDetail cd = new CbjobDetail();
            try
            {
                #region 條件
                if (!string.IsNullOrEmpty(Request.Params["startIloc"]))
                {//料位開始
                    m.startIloc = Request.Params["startIloc"].ToUpper();
                }
                if (!string.IsNullOrEmpty(Request.Params["endIloc"]))
                {
                    m.endIloc = Request.Params["endIloc"] + "Z";
                    m.endIloc = m.endIloc.ToUpper();
                }
                if (!string.IsNullOrEmpty(Request.Params["startcost"]))//金額
                {
                    m.startcost = int.Parse(Request.Params["startcost"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["endcost"]))//金額
                {
                    m.endcost = int.Parse(Request.Params["endcost"].ToString());
                }
                #endregion

                _iinvd = new IinvdMgr(mySqlConnectionString);
                DataTable dt = _iinvd.CountBookOBK(m);
                if (dt.Rows.Count > 0)
                {
                    #region 生成報表
                    DataTable dtCountBook = new DataTable();
                    dtCountBook.Columns.Add("編號", typeof(String));
                    dtCountBook.Columns.Add("料位", typeof(String));
                    dtCountBook.Columns.Add("狀態", typeof(String));
                    dtCountBook.Columns.Add("商品細項編號", typeof(String));
                    dtCountBook.Columns.Add("成本", typeof(String));
                    dtCountBook.Columns.Add("商品名稱", typeof(String));
                    dtCountBook.Columns.Add("商品規格", typeof(String));
                    dtCountBook.Columns.Add("製造日期", typeof(String));
                    dtCountBook.Columns.Add("有效日期", typeof(String));
                    dtCountBook.Columns.Add("原有數量", typeof(String));
                    dtCountBook.Columns.Add("現有數量", typeof(String));
                    dtCountBook.Columns.Add("差異金額", typeof(String));
                    foreach (DataRow item in dt.Rows)
                    {
                        DataRow dr = dtCountBook.NewRow();
                        dr[0] = item["row_id"];
                        dr[1] = item["loc_id"];
                        dr[2] = item["lsta_id"];
                        dr[3] = item["item_id"];
                        dr[4] = item["cost"];
                        dr[5] = item["product_name"];
                        dr[6] = GetProductSpec(item["item_id"].ToString());
                        dr[7] = DateTime.Parse(item["made_date"].ToString()).ToString("yyyy/MM/dd");
                        dr[8] = DateTime.Parse(item["cde_dt"].ToString()).ToString("yyyy/MM/dd");
                        dr[9] = item["prod_qty"];
                        dr[10] = item["st_qty"];
                        dr[11] = item["money"];
                        dtCountBook.Rows.Add(dr);
                    }
                    #endregion
                    string fileName = "盤點差異報表OBK" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xls";
                    String str = "盤點差異報表OBK" + DateTime.Now.ToString("_yyyyMMddHHmmss");
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtCountBook, str);
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
                else
                {
                    Response.Clear();
                    this.Response.Write("該範圍內沒有差異數據<br/>");
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return this.Response;
        }
        #endregion

        #endregion

        #region 庫存管理
        #region 庫存調整列表頁
        public HttpResponseBase GeKuCunList()
        {
            string json = string.Empty;
            IinvdQuery Iinvd = new IinvdQuery();

            if (!string.IsNullOrEmpty(Request.Params["prod_id"]))//查詢商品编号
            {
                if (Request.Params["prod_id"].Length == 6)
                {
                    Iinvd.item_id = UInt32.Parse(Request.Params["prod_id"]);
                }
                else
                {
                    Iinvd.upc_id = Request.Params["prod_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["sloc_id"]))
                {
                    Iinvd.plas_loc_id = Request.Params["sloc_id"].ToString().Trim().ToUpper();
                }
                Iinvd.ista_id = "A";
            }
            try
            {
                List<IinvdQuery> store = new List<IinvdQuery>();
                _ipalet = new PalletMoveMgr(mySqlConnectionString);
                store = _ipalet.GetPalletList(Iinvd);//查询的是副料位
                foreach (var item in store)
                {
                    item.cde_dt_make = item.made_date;
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 庫存調整
        public HttpResponseBase KutiaoAddorReduce()
        {
            string jsonStr = String.Empty;
            _iinvd = new IinvdMgr(mySqlConnectionString);
            _iagMgr = new IialgMgr(mySqlConnectionString);
            Iinvd invd = new Iinvd();
            iialg iag = new iialg();
            IstockChange Icg = new IstockChange();
            ProductItem Proitems = new ProductItem();
            int results = 0;
            try
            {
                invd.row_id = Convert.ToInt32(Request.Params["row_id"]);//行號碼
                int resultcount = 0;
                int kucuncount = Convert.ToInt32(Request.Params["benginnumber"]);//庫存數量
                int tiaozhengcount = Convert.ToInt32(Request.Params["changenumber"]);
                int kucuntype = Convert.ToInt32(Request.Params["kutiaotype"]);
                if (!string.IsNullOrEmpty(Request.Params["item_id"]))//商品細項編號
                {
                    Icg.item_id = Convert.ToUInt32(Request.Params["item_id"]);
                    Proitems.Item_Id = Icg.item_id;
                }
                int oldsumcount = _iinvd.GetProqtyByItemid(Convert.ToInt32(Icg.item_id));//總庫存
                string iarc_id = "";
                if (!string.IsNullOrEmpty(Request.Params["iarcid"]))
                {
                    iarc_id = Request.Params["iarcid"];//庫調原因
                }

                #region 庫存調整的時候，商品庫存也要調整
                _proditemMgr = new ProductItemMgr(mySqlConnectionString);
                int item_stock = 0;
                #endregion
                if (kucuntype == 1)//表示選擇了加
                {
                    resultcount = kucuncount + tiaozhengcount;
                    item_stock = tiaozhengcount;
                }
                else//表示選擇了減號
                {
                    resultcount = kucuncount - tiaozhengcount;
                    item_stock = -tiaozhengcount;
                }
                Proitems.Item_Stock = item_stock;
                invd.prod_qty = resultcount;//此時為更改后的庫存
                invd.change_dtim = DateTime.Now;
                invd.change_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                Icg.sc_trans_type = 2;
                Icg.sc_num_old = oldsumcount;
                Icg.sc_time = DateTime.Now;
                Icg.sc_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;

                if (!string.IsNullOrEmpty(Request.Params["po_id"]))
                {
                    Icg.sc_cd_id = Request.Params["po_id"];//採購單編號
                }
                if (!string.IsNullOrEmpty(Request.Params["doc_no"]))
                {
                    Icg.sc_trans_id = Request.Params["doc_no"];//庫調單號
                }
                if (!string.IsNullOrEmpty(Request.Params["remarks"]))
                {
                    Icg.sc_note = Request.Params["remarks"];//備註
                }
                _istockMgr = new IstockChangeMgr(mySqlConnectionString);

                int j = _iinvd.kucunTiaozheng(invd); //更改iloc表中的狀態並且在iialg表中插入數據
                string path = "/WareHouse/KutiaoAddorReduce";
                Caller call = new Caller();
                call = (System.Web.HttpContext.Current.Session["caller"] as Caller);
                int k = 0;
                if (iarc_id == "NE" || iarc_id == "RF")//庫存調整-不改動前台庫存
                {
                    k = 1;
                }
                else
                {
                    k = _proditemMgr.UpdateItemStock(Proitems, path, call);
                }
                int newsumcount = _iinvd.GetProqtyByItemid(Convert.ToInt32(Icg.item_id));//總庫存

                Icg.sc_num_chg = newsumcount - oldsumcount;
                Icg.sc_num_new = newsumcount;
                Icg.sc_istock_why = 2;
                if (oldsumcount != newsumcount)
                {
                    results = _istockMgr.insert(Icg);
                }
                else
                {
                    results = 1;
                }
                if (j > 0 && results > 0 && k > 0)
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
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }

        public void KTPrintPDF()
        {
            PdfHelper pdf = new PdfHelper();
            List<string> pdfList = new List<string>();
            float[] arrColWidth = new float[] { 60, 60, 100, 60, 50, 30, 60, 60, 60, 60 };
            int index = 0;
            string newFileName = string.Empty;
            string newName = string.Empty;
            string json = string.Empty;
            IialgQuery q = new IialgQuery();
            if (!string.IsNullOrEmpty(Request.Params["KT_NO"].Trim().ToUpper()))//by zhaozhi0623j add 20151006
            {
                q.doc_no = Request.Params["KT_NO"].Trim().ToUpper();
            }
            try
            {
                List<IialgQuery> store = new List<IialgQuery>();
                _iagMgr = new IialgMgr(mySqlConnectionString);
                int totalCount = 0;
                q.IsPage = false;
                store = _iagMgr.GetIialgList(q, out totalCount);
                int rid = 0;
                DataTable _dtBody = new DataTable();

                if (store.Count > 0)
                {
                    _dtBody.Columns.Add("商品細項編號", typeof(string));
                    _dtBody.Columns.Add("主料位", typeof(string));
                    _dtBody.Columns.Add("商品名稱", typeof(string));
                    _dtBody.Columns.Add("規格", typeof(string));
                    _dtBody.Columns.Add("調整原因", typeof(string));
                    _dtBody.Columns.Add("數量", typeof(string));
                    _dtBody.Columns.Add("调整料位", typeof(string));
                    _dtBody.Columns.Add("有效日期", typeof(string));
                    _dtBody.Columns.Add("前置單號", typeof(string));
                    _dtBody.Columns.Add("備註", typeof(string));
                    for (int i = 0; i < store.Count; i++)
                    {
                        store[i].id = rid++;
                        store[i].qty = store[i].qty_o + store[i].adj_qty;
                        DataRow newRow = _dtBody.NewRow();
                        newRow["商品細項編號"] = store[i].item_id;
                        newRow["主料位"] = store[i].loc_id;
                        newRow["商品名稱"] = store[i].product_name;
                        newRow["規格"] = string.IsNullOrEmpty(store[i].prod_sz) ? " " : store[i].prod_sz;
                        newRow["調整原因"] = string.IsNullOrEmpty(store[i].iarc_id) ? " " : store[i].iarc_id;
                        newRow["數量"] = store[i].adj_qty;
                        newRow["调整料位"] = store[i].loc_R;
                        newRow["有效日期"] = store[i].cde_dt.ToString("yyyy-MM-dd").Substring(0, 10) == "0001-01-01" ? " " : store[i].cde_dt.ToString("yyyy-MM-dd").Substring(0, 10);
                        newRow["前置單號"] = string.IsNullOrEmpty(store[i].po_id) ? " " : store[i].po_id;
                        newRow["備註"] = string.IsNullOrEmpty(store[i].remarks) ? " " : store[i].remarks;

                        _dtBody.Rows.Add(newRow);
                    }
                }
                string UsingName = " ";
                String UsingTime = " ";
                if (store.Count > 0)
                {
                    UsingName = store[0].name;
                    UsingTime = store[0].create_dtim.ToString("yyyy/MM/dd");
                }
                BaseFont bf = BaseFont.CreateFont("C:\\WINDOWS\\Fonts\\simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font fontChinese = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.UNDERLINE, iTextSharp.text.BaseColor.RED);
                iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
                string filename = "庫存調整" + DateTime.Now.ToString("yyyyMMddHHmmss");
                Document document = new Document(PageSize.A4.Rotate());
                string newPDFName = Server.MapPath(excelPath) + filename;
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(newPDFName, FileMode.Create));
                document.Open();

                #region 庫存調整單頭

                PdfPTable ptable = new PdfPTable(10);


                ptable.WidthPercentage = 100;//表格寬度
                ptable.SetTotalWidth(arrColWidth);
                PdfPCell cell = new PdfPCell();

                cell = new PdfPCell(new Phrase("執行人員:" + UsingName, new iTextSharp.text.Font(bf, 10)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.Colspan = 2;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 10)));
                cell.VerticalAlignment = Element.ALIGN_RIGHT;//字體水平居右
                cell.Colspan = 8;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("執行日期:" + UsingTime, new iTextSharp.text.Font(bf, 10)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.Colspan = 2;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 10)));
                cell.VerticalAlignment = Element.ALIGN_RIGHT;//字體水平居右
                cell.Colspan = 8;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(bf, 4)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.Colspan = 10;
                //cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                //cell.DisableBorderSide(4);
                //cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 15)));
                cell.VerticalAlignment = Element.ALIGN_CENTER;//字體水平居左
                cell.Colspan = 3;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                //cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("  庫存調整單", new iTextSharp.text.Font(bf, 15)));
                cell.VerticalAlignment = Element.ALIGN_CENTER;//字體水平居左
                cell.Colspan = 7;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                // cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 10)));
                cell.VerticalAlignment = Element.ALIGN_CENTER;//字體水平居左
                cell.Colspan = 3;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                // cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("單號" + ":" + q.doc_no, new iTextSharp.text.Font(bf, 10)));// ipoStore[a].po_type_desc
                cell.VerticalAlignment = Element.ALIGN_CENTER;//字體水平居左
                cell.Colspan = 7;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                // cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(bf, 4)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.Colspan = 10;
                cell.DisableBorderSide(1);
                //cell.DisableBorderSide(2);
                // cell.DisableBorderSide(4);
                //cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("商品細項編號", new iTextSharp.text.Font(bf, 10)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左

                //cell.DisableBorderSide(1);
                // cell.DisableBorderSide(2);
                //cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("主料位", new iTextSharp.text.Font(bf, 10)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左

                //cell.DisableBorderSide(1);
                // cell.DisableBorderSide(2);
                //cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("商品名稱", new iTextSharp.text.Font(bf, 10)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左

                //cell.DisableBorderSide(1);
                // cell.DisableBorderSide(2);
                //cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("規格", new iTextSharp.text.Font(bf, 10)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左

                //cell.DisableBorderSide(1);
                // cell.DisableBorderSide(2);
                //cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("調整原因", new iTextSharp.text.Font(bf, 10)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左

                //cell.DisableBorderSide(1);
                // cell.DisableBorderSide(2);
                //cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("數量", new iTextSharp.text.Font(bf, 10)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左

                //cell.DisableBorderSide(1);
                //cell.DisableBorderSide(2);
                //cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("调整料位", new iTextSharp.text.Font(bf, 10)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左

                //cell.DisableBorderSide(1);
                // cell.DisableBorderSide(2);
                //cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("有效日期", new iTextSharp.text.Font(bf, 10)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左

                //cell.DisableBorderSide(1);
                // cell.DisableBorderSide(2);
                //cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("前置單號", new iTextSharp.text.Font(bf, 10)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左

                //cell.DisableBorderSide(1);
                // cell.DisableBorderSide(2);
                //cell.DisableBorderSide(4);
                //cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("備註", new iTextSharp.text.Font(bf, 10)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左

                //cell.DisableBorderSide(1);
                // cell.DisableBorderSide(2);
                //cell.DisableBorderSide(4);
                //cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                //cell.UseAscender = true;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;//字體垂直居中
                //cell.VerticalAlignment = Element.ALIGN_MIDDLE;//字體水平居中
                //cell.BorderWidth = 0.1f;
                //cell.BorderColor = new BaseColor(0, 0, 0);

                #endregion

                #region 庫存調整單尾

                PdfPTable ptablefoot = new PdfPTable(10);


                ptablefoot.WidthPercentage = 100;//表格寬度
                ptablefoot.SetTotalWidth(arrColWidth);
                PdfPCell footcell = new PdfPCell();
                footcell.UseAscender = true;
                footcell.HorizontalAlignment = Element.ALIGN_CENTER;//字體垂直居中
                footcell.VerticalAlignment = Element.ALIGN_MIDDLE;//字體水平居中
                footcell.BorderWidth = 0.1f;
                footcell.BorderColor = new BaseColor(0, 0, 0);
                footcell = new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(bf, 15)));
                footcell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                //footcell.HorizontalAlignment = Element.ALIGN_LEFT;//水平居右
                footcell.Colspan = 10;
                footcell.DisableBorderSide(1);
                footcell.DisableBorderSide(2);
                footcell.DisableBorderSide(4);
                footcell.DisableBorderSide(8);
                ptablefoot.AddCell(footcell);


                footcell = new PdfPCell(new Phrase("印表日期:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), new iTextSharp.text.Font(bf, 10)));
                footcell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居右
                // footcell.HorizontalAlignment = Element.ALIGN_LEFT;//水平居右
                footcell.Colspan = 2;
                footcell.DisableBorderSide(1);
                footcell.DisableBorderSide(2);
                footcell.DisableBorderSide(4);
                footcell.DisableBorderSide(8);
                ptablefoot.AddCell(footcell);
                footcell = new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(bf, 10)));
                footcell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居右
                // footcell.HorizontalAlignment = Element.ALIGN_LEFT;//水平居右
                footcell.Colspan = 1;
                footcell.DisableBorderSide(1);
                footcell.DisableBorderSide(2);
                footcell.DisableBorderSide(4);
                footcell.DisableBorderSide(8);
                ptablefoot.AddCell(footcell);

                footcell = new PdfPCell(new Phrase("印表人:" + (System.Web.HttpContext.Current.Session["caller"] as Caller).user_username, new iTextSharp.text.Font(bf, 10)));
                footcell.VerticalAlignment = Element.ALIGN_RIGHT;//水平居右
                footcell.Colspan = 2;
                footcell.DisableBorderSide(1);
                footcell.DisableBorderSide(2);
                footcell.DisableBorderSide(4);
                footcell.DisableBorderSide(8);
                ptablefoot.AddCell(footcell);
                footcell = new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(bf, 10)));
                footcell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居右
                // footcell.HorizontalAlignment = Element.ALIGN_LEFT;//水平居右
                footcell.Colspan = 3;
                footcell.DisableBorderSide(1);
                footcell.DisableBorderSide(2);
                footcell.DisableBorderSide(4);
                footcell.DisableBorderSide(8);
                ptablefoot.AddCell(footcell);

                footcell = new PdfPCell(new Phrase("主管簽核:__________________", new iTextSharp.text.Font(bf, 10)));
                footcell.VerticalAlignment = Element.ALIGN_RIGHT;//字體水平居右
                footcell.Colspan = 2;
                footcell.DisableBorderSide(1);
                footcell.DisableBorderSide(2);
                footcell.DisableBorderSide(4);
                footcell.DisableBorderSide(8);
                ptablefoot.AddCell(footcell);


                #endregion
                if (store.Count == 0)
                {
                    document = new Document(PageSize.A4.Rotate());
                    if (!document.IsOpen())
                    {
                        document.Open();
                    }
                    cell = new PdfPCell(new Phrase(" ", font));
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_CENTER;//字體水平居左
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);

                    cell = new PdfPCell(new Phrase("此庫調單庫調數據不存在!", font));
                    cell.Colspan = 7;
                    cell.DisableBorderSide(4);
                    cell.VerticalAlignment = Element.ALIGN_CENTER;//字體水平居左
                    ptable.AddCell(cell);


                    // document.Add(ptable);
                    //document.Add(ptablefoot); 
                    newFileName = newPDFName + "_part" + index++ + "." + "pdf";
                    pdf.ExportDataTableToPDF(_dtBody, false, newFileName, arrColWidth, ptable, ptablefoot, "", "", 10, uint.Parse(store.Count.ToString()));/*第一7是列，第二個是行*/
                    pdfList.Add(newFileName);

                }
                else
                {
                    newFileName = newPDFName + "_part" + index++ + "." + "pdf";

                    pdf.ExportDataTableToPDF(_dtBody, false, newFileName, arrColWidth, ptable, ptablefoot, "", "", 10, uint.Parse(store.Count.ToString()));/*第一7是列，第二個是行*/
                    pdfList.Add(newFileName);

                }


                //newFileName = newPDFName + "_part" + index++ + "." + "pdf";
                //pdf.ExportDataTableToPDF(newFileName, ptable, "", "");
                //pdfList.Add(newFileName);

                //document.Add(ptable);
                //document.NewPage();
                newFileName = newPDFName + "." + "pdf";
                pdf.MergePDF(pdfList, newFileName);

                Response.Clear();
                Response.Charset = "gb2312";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.AddHeader("Content-Disposition", "attach-ment;filename=" + filename + ".pdf");
                Response.WriteFile(newFileName);

            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion
        #region 獲取庫調編號
        public HttpResponseBase Getkutiaonumber()
        {
            string jsonStr = string.Empty;
            try
            {
                string kutiaonumber = "K" + DateTime.Now.ToString("yyyyMMddHHmmss");
                jsonStr = "{success:true,msg:'" + kutiaonumber + "'}";
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
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 獲取庫調原因
        public HttpResponseBase Getkutiaowhy()
        {
            string json = string.Empty;
            string types = "adj_code";
            try
            {
                List<Parametersrc> store = new List<Parametersrc>();
                _psrcMgr = new ParameterMgr(mySqlConnectionString);
                store = _psrcMgr.GetKindTypeByStatus(types);
                json = "{success:true,'msg':'user',data:" + JsonConvert.SerializeObject(store) + "}";//返回json數據
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

        public HttpResponseBase InsertIialg()
        {
            string json = string.Empty;
            IialgQuery iagQuery = new IialgQuery();
            Iinvd invd = new Iinvd();
            int result = 0;
            try
            {
                invd.row_id = Convert.ToInt32(Request.Params["row_id"]);//行號碼 
                _iinvd = new IinvdMgr(mySqlConnectionString);
                DataTable dt = _iinvd.GetRowMsg(invd);//首先根據row_id 獲取到製造日期和有效日期
                iagQuery.made_dt = Convert.ToDateTime(dt.Rows[0]["made_date"]);//製造日期
                iagQuery.cde_dt = Convert.ToDateTime(dt.Rows[0]["cde_dt"]);//有效日期
                if (!string.IsNullOrEmpty(Request.Params["item_id"]))//商品細項編號
                {
                    iagQuery.item_id = Convert.ToUInt32(Request.Params["item_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["po_id"]))
                {
                    iagQuery.po_id = Request.Params["po_id"];//採購單編號
                }
                if (!string.IsNullOrEmpty(Request.Params["iarc_id"]))
                {
                    iagQuery.iarc_id = Request.Params["iarc_id"];//庫調原因
                }
                if (!string.IsNullOrEmpty(Request.Params["ktloc_id"]))
                {
                    iagQuery.loc_id = Request.Params["ktloc_id"].ToUpper();//料位編號
                }
                if (!string.IsNullOrEmpty(Request.Params["doc_no"]))
                {
                    iagQuery.doc_no = Request.Params["doc_no"];//庫調單號
                }
                if (!string.IsNullOrEmpty(Request.Params["remarks"]))
                {
                    iagQuery.remarks = Request.Params["remarks"];//庫調單號
                }
                if (!string.IsNullOrEmpty(Request.Params["made_date"]))//創建時間
                {
                    iagQuery.made_dt = Convert.ToDateTime(Request.Params["made_date"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["end_date"]))//有效日期
                {
                    iagQuery.cde_dt = Convert.ToDateTime(Request.Params["end_date"]);//庫調單號
                }
                int kucuncount = Convert.ToInt32(Request.Params["benginnumber"]);//庫存數量
                int tiaozhengcount = Convert.ToInt32(Request.Params["changenumber"]);//調整數量
                int kucuntype = Convert.ToInt32(Request.Params["kutiaotype"]);//庫存類型
                if (kucuntype == 1)
                {
                    iagQuery.adj_qty = tiaozhengcount; //調整庫存
                }
                else
                {
                    iagQuery.adj_qty = tiaozhengcount * (-1);//調整庫存
                }
                iagQuery.qty_o = kucuncount;//原來庫存
                iagQuery.create_dtim = DateTime.Now;
                iagQuery.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;

                if (!string.IsNullOrEmpty(Request.Params["ktloc_id"]))
                {
                    iagQuery.loc_id = Request.Params["ktloc_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["doc_no"]))
                {
                    iagQuery.doc_no = Request.Params["doc_no"];
                }
                _iagMgr = new IialgMgr(mySqlConnectionString);
                result = _iagMgr.insertiialg(iagQuery);
                if (result > 0)
                {
                    json = "{success:true}";//返回json數據
                }
                else
                {
                    json = "{success:false}";//返回json數據
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
        #region 庫存調整-判斷item_id和loc_id是否對應
        public HttpResponseBase AboutItemidLocid()
        {
            string json = string.Empty;
            Iinvd invd = new Iinvd();
            int result = 0;
            try
            {
                invd.plas_loc_id = Request.Params["tloc_id"].ToUpper();
                invd.item_id = Convert.ToUInt32(Request.Params["titem_id"]);
                _iinvd = new IinvdMgr(mySqlConnectionString);
                result = _iinvd.AboutItemidLocid(invd);
                if (result > 0)
                {
                    json = "{success:true}";//返回json數據
                }
                else
                {
                    json = "{success:true,msg:0}";//返回json數據
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
        #region 插入庫存信息 更改iinvd 和插入iialg表 HttpResponseBase InsertkucunMessage()
        public HttpResponseBase InsertkucunMessage()
        {
            int j = 0;
            string json = string.Empty;//json字符串
            int iialgtotal = 0;
            int iinvdtotal = 0;
            try
            {
                if (Request.Files["ImportFileMsg"] != null && Request.Files["ImportFileMsg"].ContentLength > 0)//判斷文件是否為空
                {
                    HttpPostedFileBase excelFile = Request.Files["ImportFileMsg"];//獲取文件流
                    FileManagement fileManagement = new FileManagement();//實例化 FileManagement
                    //string fileLastName = excelFile.FileName.Substring((excelFile.FileName).LastIndexOf('.')).ToLower().Trim();
                    string fileLastName = excelFile.FileName;
                    string newExcelName = Server.MapPath(excelPath) + "Kucuntiaozheng" + fileManagement.NewFileName(excelFile.FileName);//處理文件名，獲取新的文件名
                    excelFile.SaveAs(newExcelName);//上傳文件
                    DataTable dt = new DataTable();
                    NPOI4ExcelHelper helper = new NPOI4ExcelHelper(newExcelName);
                    dt = helper.SheetData();
                    DataRow[] dr = dt.Select(); //定义一个DataRow数组,读取ds里面所有行
                    int rowsnum = dt.Rows.Count;
                    if (rowsnum == 0)
                    {
                        json = "{success:true,iialgtotal:\"" + 0 + "\",iinvdtotal:\"" + 0 + "\",msg:\"" + "匯入盤點報表中沒有數據" + "\"}";
                    }
                    if (dr[0][0].ToString().Trim() == "編號" && dr[0][1].ToString().Trim() == "料位" && dr[0][2].ToString().Trim() == "料位狀態" && dr[0][11].ToString().Trim() == "有效日期" && dt.Columns.Count == 12)
                    {
                        _iagMgr = new IialgMgr(mySqlConnectionString);
                        j = _iagMgr.HuiruInsertiialg(dr, out iialgtotal, out iinvdtotal);
                        if (j > 0)
                        {
                            json = "{success:true,iialgtotal:\"" + iialgtotal + "\",iinvdtotal:\"" + iinvdtotal + "\",msg:\"" + "匯入盤點報表對照表成功!" + "\"}";
                        }
                        else
                        {
                            json = "{success:true,iialgtotal:\"" + iialgtotal + "\",iinvdtotal:\"" + iinvdtotal + "\",msg:\"" + "匯入數據標準不對,請嚴格按照模板匯入!" + "\"}";
                        }
                    }
                    else
                    {
                        json = "{success:true,iialgtotal:\"" + 0 + "\",iinvdtotal:\"" + 0 + "\",msg:\"" + "匯入盤點報表格式不正確!" + "\"}";
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:\"" + ex.ToString() + "\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 初盤復盤--根據工作單號獲取到列表值
        public HttpResponseBase GetMessage()
        {
            string json = string.Empty;

            CbjobDetailQuery cbjQuery = new CbjobDetailQuery();
            cbjQuery.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            cbjQuery.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            if (!string.IsNullOrEmpty(Request.Params["searchcontent"].Trim()))
            {
                cbjQuery.searchcontent = Request.Params["searchcontent"];
            }
            if (!string.IsNullOrEmpty(Request.Params["newid"].Trim()))
            {
                cbjQuery.cb_newid = Convert.ToInt32(Request.Params["newid"].Trim());
            }
            try
            {
                List<CbjobDetailQuery> store = new List<CbjobDetailQuery>();
                _cbjobMgr = new CbjobDetailMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _cbjobMgr.GetMessage(cbjQuery, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 初盤復盤--刪除某個工作編號
        public HttpResponseBase DeleteCbjobmessage()
        {
            int result = 0;
            string json = string.Empty;
            CbjobDetailQuery cbjobQuery = new CbjobDetailQuery();
            if (!string.IsNullOrEmpty(Request.Params["searchcontent"].Trim()))
            {
                cbjobQuery.cb_jobid = Request.Params["searchcontent"].Trim();
            }
            try
            {
                _cbjobMgr = new CbjobDetailMgr(mySqlConnectionString);
                result = _cbjobMgr.DeleteCbjobmessage(cbjobQuery);
                if (result == -2)
                {
                    json = "{success:true,msg:-2}";
                }
                else if (result == -1)
                {
                    json = "{success:true,msg:-1}";
                }
                else if (result > 0)
                {

                    json = "{success:true,msg:1}";//返回json數據

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
        #region 初盤復盤---248
        public HttpResponseBase UpdateCbjobMaster()
        {
            int result = 0;
            string json = string.Empty;
            CbjobDetailQuery cbjobQuery = new CbjobDetailQuery();
            if (!string.IsNullOrEmpty(Request.Params["gzbh"].Trim()))
            {

                cbjobQuery.cb_jobid = Request.Params["gzbh"].Trim();//獲取工作編號
                cbjobQuery.create_datetime = DateTime.Now;       //創建時間
                cbjobQuery.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
            }
            try
            {
                _cbjobMgr = new CbjobDetailMgr(mySqlConnectionString);
                result = _cbjobMgr.UpdateCbjobMaster(cbjobQuery);
                if (result > 0)
                {

                    json = "{success:true,msg:1}";//返回json數據

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
        #region 初盤復盤--蓋帳按鈕--插入idiff_count_book表中即時差異報表/把COM的狀態改為END
        public HttpResponseBase UpdateCbjobstaid()
        {
            int result = 0;
            int resulttwo = 0;
            string json = string.Empty;
            CbjobDetailQuery cbjobQuery = new CbjobDetailQuery();
            if (!string.IsNullOrEmpty(Request.Params["jobnumber"].Trim()))
            {
                cbjobQuery.cb_jobid = Request.Params["jobnumber"].Trim();//獲取工作編號
            }
            DateTime time = DateTime.MinValue;
            if (DateTime.TryParse(Request.Params["starttime"].ToString(), out time))
            {
                cbjobQuery.StartDate = time;
            }
            if (DateTime.TryParse(Request.Params["endtime"].ToString(), out time))
            {
                cbjobQuery.EndDate = time.AddDays(1);
            }
            cbjobQuery.create_datetime = DateTime.Now;       //修改時間
            cbjobQuery.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;//修改用戶
            try
            {
                _cbjobMgr = new CbjobDetailMgr(mySqlConnectionString);
                _istockMgr = new IstockChangeMgr(mySqlConnectionString);
                resulttwo = _istockMgr.insertistocksome(cbjobQuery);
                result = _cbjobMgr.UpdateCbjobstaid(cbjobQuery);

                if (result > 0)
                {
                    if (resulttwo > 0)
                    {
                        json = "{success:true,msg:'" + result + "'}";//返回json數據
                    }
                    else
                    {
                        json = "{success:false,msg:'" + result + "'}";//返回json數據
                    }
                }
                else if (result == -1)
                {
                    json = "{success:true,msg:-1}";//返回json數據
                }
                else if (result == -2)
                {
                    json = "{success:true,msg:-2}";//返回json數據
                }
                else if (result == -3)
                {
                    json = "{success:true,msg:-3}";//返回json數據
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
        #region 復盤完成+HttpResponseBase FupanComplete()
        public HttpResponseBase FupanComplete()
        {
            int result = 0;
            string json = string.Empty;
            CbjobDetailQuery cbjobQuery = new CbjobDetailQuery();
            if (!string.IsNullOrEmpty(Request.Params["jobnumber"].Trim()))
            {
                cbjobQuery.cb_jobid = Request.Params["jobnumber"].Trim();//獲取工作編號
            }
            cbjobQuery.create_datetime = DateTime.Now;       //創建時間
            cbjobQuery.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
            try
            {
                _cbjobMgr = new CbjobDetailMgr(mySqlConnectionString);
                result = _cbjobMgr.FupanComplete(cbjobQuery);
                if (result == -1)
                {
                    json = "{success:true,msg:-1}";//返回json數據
                }
                else if (result == -2)
                {
                    json = "{success:true,msg:-2}";//返回json數據
                }
                else if (result == -3)
                {
                    json = "{success:true,msg:-3}";//返回json數據
                }
                else if (result > 0)
                {

                    json = "{success:true,msg:1}";//返回json數據
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
        #region 初盤復盤--改變iinvd的st_qty庫存量
        public HttpResponseBase Updateiinvdstqty()
        {
            int result = 0;
            string json = string.Empty;
            Iinvd invd = new Iinvd();

            if (!string.IsNullOrEmpty(Request.Params["row_id"].Trim()))
            {
                invd.row_id = Convert.ToInt32(Request.Params["row_id"].Trim());//獲取工作編號
            }
            if (!string.IsNullOrEmpty(Request.Params["stqty"].Trim()))
            {
                invd.st_qty = Convert.ToInt32(Request.Params["stqty"].Trim());//獲取工作編號
            }
            invd.create_dtim = DateTime.Now;       //創建時間
            invd.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
            try
            {
                _iinvd = new IinvdMgr(mySqlConnectionString);
                result = _iinvd.Updateiinvdstqty(invd);
                if (result > 0)
                {
                    json = "{success:true}";//返回json數據
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

        #region 庫調記錄
        //庫調記錄列表
        public HttpResponseBase GetIialgList()
        {
            string json = string.Empty;
            IialgQuery q = new IialgQuery();
            q.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            q.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            uint id;
            int rid = 0;
            if (uint.TryParse(Request.Params["item_id"].ToUpper(), out id))//獲取參數
            {
                q.item_id = id;
            }
            if (!string.IsNullOrEmpty(Request.Params["loc_id"]))
            {
                q.loc_id = Request.Params["loc_id"].ToString().ToUpper();
            }
            if (!string.IsNullOrEmpty(Request.Params["po_id"]))
            {
                q.po_id = Request.Params["po_id"].ToString();
            }
            if (Request.Params["doc_userid"].ToString() != "-1")
            {
                q.doc_userid = int.Parse(Request.Params["doc_userid"]);
            }
            //if (!string.IsNullOrEmpty(Request.Params["iarc_id"]))
            //{
            //    q.iarc_id = Request.Params["iarc_id"].ToString();
            //}
            DateTime time = DateTime.MinValue;
            if (DateTime.TryParse(Request.Params["starttime"].ToString(), out time))
            {
                q.starttime = time;
            }
            if (DateTime.TryParse(Request.Params["endtime"].ToString(), out time))
            {
                q.endtime = time;
            }
            if (!string.IsNullOrEmpty(Request.Params["doc_no"].Trim().ToUpper()))//by zhaozhi0623j add 20151006
            {
                q.doc_no = Request.Params["doc_no"].Trim().ToUpper();
            }
            try
            {
                List<IialgQuery> store = new List<IialgQuery>();
                _iagMgr = new IialgMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _iagMgr.GetIialgList(q, out totalCount);

                foreach (var item in store)
                {
                    item.id = rid++;
                    item.qty = item.qty_o + item.adj_qty;
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        // 庫調匯出
        public void IialgExcel()
        {
            string json = string.Empty;
            IialgQuery q = new IialgQuery();
            try
            {
                uint id;
                if (uint.TryParse(Request.Params["item_id"], out id))//獲取參數
                {
                    q.item_id = id;
                }
                if (!string.IsNullOrEmpty(Request.Params["loc_id"]))
                {
                    q.loc_id = Request.Params["loc_id"].ToString().ToUpper();
                }
                if (!string.IsNullOrEmpty(Request.Params["po_id"]))
                {
                    q.po_id = Request.Params["po_id"].ToString();
                }
                //if (!string.IsNullOrEmpty(Request.Params["iarc_id"]))
                //{
                //    q.iarc_id = Request.Params["iarc_id"].ToString();
                //}
                if (Request.Params["doc_userid"].ToString() != "-1")//by zhaozhi0623j add 20151006
                {
                    q.doc_userid = int.Parse(Request.Params["doc_userid"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["doc_no"].Trim().ToUpper()))//by zhaozhi0623j add 20151006
                {
                    q.doc_no = Request.Params["doc_no"].Trim().ToUpper();
                }
                DateTime time = DateTime.MinValue;
                if (DateTime.TryParse(Request.Params["starttime"].ToString(), out time))
                {
                    q.starttime = time;
                }
                if (DateTime.TryParse(Request.Params["endtime"].ToString(), out time))
                {
                    q.endtime = time;
                }
                if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
                }

                DataTable dtHZ = new DataTable();
                string newExcelName = string.Empty;
                #region 表頭
                dtHZ.Columns.Add("編號", typeof(String));
                dtHZ.Columns.Add("庫存調整單號", typeof(String));
                dtHZ.Columns.Add("主料位", typeof(String));
                dtHZ.Columns.Add("調整料位", typeof(String));
                dtHZ.Columns.Add("商品細項編號", typeof(String));
                dtHZ.Columns.Add("商品名稱", typeof(String));
                dtHZ.Columns.Add("商品規格", typeof(String));
                dtHZ.Columns.Add("製造日期", typeof(String));
                dtHZ.Columns.Add("有效日期", typeof(String));
                dtHZ.Columns.Add("新製造日期", typeof(String));
                dtHZ.Columns.Add("新有效日期", typeof(String));
                dtHZ.Columns.Add("調整前數量", typeof(String));
                dtHZ.Columns.Add("調整數量", typeof(String));
                dtHZ.Columns.Add("調整后數量", typeof(String));
                dtHZ.Columns.Add("調整原因", typeof(String));
                dtHZ.Columns.Add("調整日期", typeof(String));
                dtHZ.Columns.Add("調整人員", typeof(String));
                dtHZ.Columns.Add("前置單號", typeof(String));
                dtHZ.Columns.Add("備註欄", typeof(String));
                #endregion

                List<IialgQuery> store = new List<IialgQuery>();
                _iagMgr = new IialgMgr(mySqlConnectionString);
                q.IsPage = false;
                store = _iagMgr.GetExportIialgList(q);
                int i = 1;
                foreach (var item in store)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = i++;
                    dr[1] = item.doc_no;
                    dr[2] = item.loc_id;
                    dr[3] = item.loc_R;
                    dr[4] = item.item_id;
                    dr[5] = item.product_name;
                    dr[6] = item.prod_sz;
                    dr[7] = item.made_dt;
                    dr[8] = item.cde_dt;

                    if (item.c_made_dt > DateTime.MinValue)
                    {
                        dr[9] = item.c_made_dt;
                    }
                    else
                    {
                        dr[9] = "";
                    }
                    if (item.c_cde_dt > DateTime.MinValue)
                    {
                        dr[10] = item.c_cde_dt;
                    }
                    else
                    {
                        dr[10] = "";
                    }
                    dr[11] = item.qty_o;
                    dr[12] = item.adj_qty;
                    dr[13] = item.qty_o + item.adj_qty;
                    dr[14] = item.iarc_id;
                    dr[15] = item.create_dtim;
                    dr[16] = item.name;
                    dr[17] = item.po_id;
                    dr[18] = item.remarks;
                    dtHZ.Rows.Add(dr);
                }
                if (System.IO.File.Exists(newExcelName))
                {
                    System.IO.File.Delete(newExcelName);
                }
                string fileName = "庫存調整歷史記錄_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                string fileNametwo = "庫存調整歷史記錄_" + DateTime.Now.ToString("yyyyMMddHHmm");
                MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, fileNametwo);
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());
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
        public HttpResponseBase GetkutiaoUser() //by zhaozhi0623j add 庫調人員列表
        {

            string json = string.Empty;
            try
            {
                _iagMgr = new IialgMgr(mySqlConnectionString);
                List<ManageUser> store = new List<ManageUser>();


                store = _iagMgr.GetkutiaoUser();
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }
        #endregion

        #endregion

        #region 貌似是棄用的方法!沒有找到調用的地方-如果是該方法的創建者請給我說下這個到底用不用--jialei
        //public HttpResponseBase SaveWhyLock()
        //{
        //    string json = string.Empty;
        //    try
        //    {
        //        Iinvd Iinvd = new Iinvd();
        //        _iinvd = new IinvdMgr(mySqlConnectionString);

        //        json = "{success:true}";
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:false}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json);
        //    this.Response.End();
        //    return this.Response;
        //}
        //public HttpResponseBase GetAseldListByJobidorOrderid()
        //{
        //    string json = string.Empty;
        //    AseldQuery asequery = new AseldQuery();
        //    try
        //    {
        //        List<AseldQuery> store = new List<AseldQuery>();
        //        _iasdMgr = new AseldMgr(mySqlConnectionString);
        //        store = _iasdMgr.GetAseldList(asequery);
        //        IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
        //        //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
        //        timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        //        json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:false,totalCount:0,data:[]}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json);
        //    this.Response.End();
        //    return this.Response;
        //}
        #endregion

        #region 公共方法
        //商品顯示加規格
        public string GetProductSpec(string id)
        {
            DataTable dt = new DataTable();
            _iinvd = new IinvdMgr(mySqlConnectionString);
            int item_id = 0;
            string spec = string.Empty;
            if (int.TryParse(id, out item_id) && id.Length == 6)
            {//獲取商品編號
                dt = _iinvd.Getprodu(item_id);
            }
            if (dt.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dt.Rows[0]["spec_name"].ToString()))
                {
                    spec += dt.Rows[0]["spec_name"].ToString();
                }
                if (!string.IsNullOrEmpty(dt.Rows[0]["spec_name1"].ToString()))
                {
                    if (spec.Length > 0)
                    {
                        spec += ",";
                    }
                    spec += dt.Rows[0]["spec_name1"].ToString();
                }
                if (spec.Length > 0)
                {
                    spec = "(" + spec + ")";
                }
                return spec;
            }
            else
            {
                return spec;
            }
        }


        public HttpResponseBase GetGroupUsers()
        {
            string json = string.Empty;
            string types = "picking";
            try
            {
                List<ManageUser> store = new List<ManageUser>();
                _IIwmsRrecordMgr = new IwmsRrecordMgr(mySqlConnectionString);
                store = _IIwmsRrecordMgr.GetUserslist(types);
                json = "{success:true,'msg':'user',data:" + JsonConvert.SerializeObject(store) + "}";//返回json數據
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

        #region 料位管理->採購單
        #region 採購單單頭
        /// <summary>
        /// 採購單單頭列表頁
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetIpoList()
        {
            string json = string.Empty;
            IpoQuery ipo = new IpoQuery();
            ipo.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            ipo.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            string content = string.Empty;
            if (!string.IsNullOrEmpty(Request.Params["Poid"]))
            {
                ipo.po_id = Request.Params["Poid"];
            }
            if (!string.IsNullOrEmpty(Request.Params["Potype"]))
            {
                ipo.po_type = Request.Params["Potype"];
            }
            if (!string.IsNullOrEmpty(Request.Params["start_time"]))
            {
                ipo.start_time = Convert.ToDateTime(Request.Params["start_time"].ToString());
            }
            if (!string.IsNullOrEmpty(Request.Params["end_time"]))
            {
                ipo.end_time = Convert.ToDateTime(Request.Params["end_time"].ToString());
            }
            if (!string.IsNullOrEmpty(Request.Params["freight"]))
            {
                ipo.freight = Convert.ToInt32(Request.Params["freight"].ToString());
            }
            //變更的時候記得把匯出也修改了獲取條件是同時的
            try
            {
                List<IpoQuery> store = new List<IpoQuery>();
                _ipoMgr = new IpoMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _ipoMgr.GetIpoList(ipo, out  totalCount);
                //if (!string.IsNullOrEmpty(Request.Params["freight"]))
                //{
                //    if (Request.Params["freight"].ToString() != "0")
                //    {
                //        totalCount = 0;
                //        _ipodMgr = new IpodMgr(mySqlConnectionString);
                //        List<IpoQuery> newstore = new List<IpoQuery>();
                //        foreach (IpoQuery item in store)
                //        {
                //            if (!string.IsNullOrEmpty(item.po_id))
                //            {
                //                if (_ipodMgr.GetIpodfreight(item.po_id, Convert.ToInt32(Request.Params["freight"].ToString())))
                //                {
                //                    newstore.Add(item);
                //                    totalCount++;
                //                }
                //            }
                //        }
                //        store = newstore;
                //    }
                //}

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式    ,totalCount:" + totalCount + " 
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        /// <summary>
        /// 保存採購單單頭
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase IpoAddOrEdit()
        {
            string jsonStr = String.Empty;
            try
            {
                IpoQuery m = new IpoQuery();
                _ipoMgr = new IpoMgr(mySqlConnectionString);
                m.po_id = Request.Params["po_id"];
                m.vend_id = Request.Params["vend_id"];
                m.buyer = Request.Params["buyer"];
                m.po_type = Request.Params["po_type"];
                m.po_type_desc = Request.Params["po_type_desc"];
                m.sched_rcpt_dt = Convert.ToDateTime(Request.Params["sched_rcpt_dt"]);
                m.msg1 = Request.Params["msg1"];
                m.msg2 = Request.Params["msg2"];
                m.msg3 = Request.Params["msg3"];
                if (String.IsNullOrEmpty(Request.Params["row_id"]))
                {
                    m.create_dtim = DateTime.Now; //創建時間
                    m.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    m.change_dtim = DateTime.Now;   //編輯時間
                    m.change_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    m.status = 1;
                    if (_ipoMgr.SelectIpoCountByIpo(m.po_id) > 0)
                    {
                        jsonStr = "{success:true,msg:\"" + "此採購單已存在!" + "\"}";
                    }
                    else
                    {
                        if (_ipoMgr.AddIpo(m) > 0)
                        {
                            jsonStr = "{success:true}";
                        }
                        else
                        {
                            jsonStr = "{success:false}";
                        }
                    }
                }
                else
                {
                    m.row_id = Convert.ToInt32(Request.Params["row_id"]);
                    m.change_dtim = DateTime.Now;
                    m.change_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    if (_ipoMgr.UpdateIpo(m) > 0)
                    {
                        jsonStr = "{success:true}";//返回json數據
                    }
                    else
                    {
                        jsonStr = "{success:false}";//返回json數據
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
        /// <summary>
        /// 刪除採購單單頭
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase DeleteIpo()
        {
            IpoQuery query = new IpoQuery();
            string json = string.Empty;
            try
            {
                string Row_id = "";
                if (!string.IsNullOrEmpty(Request.Params["rowId"]))
                {
                    Row_id = Request.Params["rowId"];
                    Row_id = Row_id.TrimEnd(',');
                    query.row_ids = Row_id;
                }

                _ipoMgr = new IpoMgr(mySqlConnectionString);

                int result = _ipoMgr.DeletIpo(query);
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
        /// <summary>
        /// 查詢當前
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SelectIpoCountByIpo()
        {
            string jsonStr = String.Empty;
            try
            {
                string str = Request.Params["po_id"];
                _ipoMgr = new IpoMgr(mySqlConnectionString);
                int count = _ipoMgr.SelectIpoCountByIpo(str);
                jsonStr = "{success:true,count:" + count + "}";//返回json數據
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

        #region 採購單單身
        /// <summary>
        /// 列表頁
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetIpodList()
        {
            string json = string.Empty;
            IpodQuery ipod = new IpodQuery();
            ipod.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            ipod.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            string content = string.Empty;
            //變更的時候記得把匯出也修改了獲取條件是同時的
            if (!string.IsNullOrEmpty(Request.Params["ipod"]))
            {
                ipod.po_id = Request.Params["ipod"];
            }

            try
            {
                List<IpodQuery> store = new List<IpodQuery>();
                _ipodMgr = new IpodMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _ipodMgr.GetIpodList(ipod, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        /// <summary>
        /// 保存採購單單身
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SaveIpod()
        {
            string json = string.Empty;
            try
            {
                IpodQuery ipod = new IpodQuery();
                if (!string.IsNullOrEmpty(Request.Params["po_id"]))
                {
                    ipod.po_id = Request.Params["po_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["prod_id"]))
                {
                    ipod.prod_id = Request.Params["prod_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["plst_id"]))
                {
                    ipod.plst_id = Request.Params["plst_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["qty_ord"]))
                {
                    ipod.qty_ord = int.Parse(Request.Params["qty_ord"]); ;
                }
                if (!string.IsNullOrEmpty(Request.Params["pwy_dt"]))
                {
                    int type = int.Parse(Request.Params["pwy_dt"]);
                    if (type != 0)
                    {
                        ipod.pwy_dte_ctl = "Y";
                        if (!string.IsNullOrEmpty(Request.Params["cde_dt_incr"]))
                        {
                            ipod.cde_dt_incr = int.Parse(Request.Params["cde_dt_incr"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["cde_dt_var"]))
                        {
                            ipod.cde_dt_var = int.Parse(Request.Params["cde_dt_var"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["cde_dt_shp"]))
                        {
                            ipod.cde_dt_shp = int.Parse(Request.Params["cde_dt_shp"]);
                        }
                    }
                    else
                    {
                        ipod.pwy_dte_ctl = "N";
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["qty_damaged"]))
                {
                    ipod.qty_damaged = int.Parse(Request.Params["qty_damaged"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["qty_claimed"]))
                {
                    ipod.qty_claimed = int.Parse(Request.Params["qty_claimed"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["bkord"]))
                {
                    int type = int.Parse(Request.Params["bkord"]);
                    if (type != 0)
                    {
                        ipod.bkord_allow = "Y";
                    }
                    else
                    {
                        ipod.bkord_allow = "N";
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["promo_invs_flg"]))
                {
                    ipod.promo_invs_flg = Request.Params["promo_invs_flg"];
                }
                if (!string.IsNullOrEmpty(Request.Params["req_cost"]))
                {
                    ipod.req_cost = double.Parse(Request.Params["req_cost"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["off_invoice"]))
                {
                    ipod.off_invoice = double.Parse(Request.Params["off_invoice"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["freight_price"]))
                {
                    ipod.freight_price = int.Parse(Request.Params["freight_price"]);
                }
                _ipodMgr = new IpodMgr(mySqlConnectionString);
                ipod.new_cost = ipod.req_cost * ipod.off_invoice / 100;

                if (!string.IsNullOrEmpty(Request.Params["row_id"]))
                {
                    ipod.row_id = int.Parse(Request.Params["row_id"]);
                    ipod.pod_id = _ipodMgr.GetPodID(ipod);
                    ipod.change_user = (Session["caller"] as Caller).user_id;
                    ipod.change_dtim = DateTime.Now;
                    _ipodMgr.UpdateIpod(ipod);
                    json = "{success:true,msg:\"" + "保存成功！" + "\"}";
                }
                else
                {
                    ipod.row_id = 0;
                    ipod.pod_id = _ipodMgr.GetPodID(ipod);
                    ipod.create_user = (Session["caller"] as Caller).user_id;
                    ipod.change_user = ipod.create_user;
                    ipod.create_dtim = DateTime.Now;
                    ipod.change_dtim = ipod.create_dtim;
                    _ipodMgr.AddIpod(ipod);
                    json = "{success:true,msg:\"" + "新增成功！" + "\"}";

                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:\"" + "操作失敗" + "\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        /// <summary>
        /// 獲取驗收採購單單身數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetIpodCheck()
        {
            string json = string.Empty;
            IpodQuery query = new IpodQuery();
            if (!string.IsNullOrEmpty(Request.Params["ipod"]))
            {
                query.po_id = Request.Params["ipod"];
            }
            try
            {
                List<IpodQuery> ipodStore = new List<IpodQuery>();
                _ipodMgr = new IpodMgr(mySqlConnectionString);
                ipodStore = _ipodMgr.GetIpodListExprot(query);
                for (int i = 0; i < ipodStore.Count; i++)//通過運送方式保存到字典里
                {
                    ipodStore[i].spec = GetProductSpec(ipodStore[i].prod_id.ToString());
                    ipodStore[i].plst_id = ipodStore[i].plst_id.ToString() == "F" ? "已驗收" : "未驗收";
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                json = "{success:true,data:" + JsonConvert.SerializeObject(ipodStore, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }
        /// <summary>
        /// 驗收採購單單身
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase UpdateIpodCheck()
        {

            IpodQuery query = new IpodQuery();
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["row_id"]))
                {
                    query.row_id = Convert.ToInt32(Request.Params["row_id"].ToString());
                    query.change_user = (Session["caller"] as Caller).user_id;
                    query.user_email = (Session["caller"] as Caller).user_email;
                    query.change_dtim = DateTime.Now;
                }
                if (!string.IsNullOrEmpty(Request.Params["qty_damaged"]))
                {
                    query.qty_damaged = Convert.ToInt32(Request.Params["qty_damaged"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["qty_claimed"]))
                {
                    query.qty_claimed = Convert.ToInt32(Request.Params["qty_claimed"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["item_stock"]))
                {
                    query.item_stock = Convert.ToInt32(Request.Params["item_stock"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["plst_id"]))
                {
                    query.plst_id = Request.Params["plst_id"].ToString();
                }

                _ipodMgr = new IpodMgr(mySqlConnectionString);

                bool result = _ipodMgr.UpdateIpodCheck(query);
                if (result)
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
        /// <summary>
        /// 刪除採購單單身
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase DeleteIpod()
        {
            IpodQuery query = new IpodQuery();
            string json = string.Empty;
            try
            {
                string Row_id = "";
                if (!string.IsNullOrEmpty(Request.Params["rowId"]))
                {
                    Row_id = Request.Params["rowId"];
                    Row_id = Row_id.TrimEnd(',');
                    query.row_ids = Row_id;
                }

                _ipodMgr = new IpodMgr(mySqlConnectionString);

                int result = _ipodMgr.DeletIpod(query);
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
        /// <summary>
        /// 標註品項庫存用途
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetPromoInvsFlgList()
        {
            List<Parametersrc> stores = new List<Parametersrc>();
            _ptersrc = new ParameterMgr(mySqlConnectionString);
            string type = string.Empty;
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["Type"]))
                {
                    type = Request.Params["Type"];
                }
                stores = _ptersrc.GetElementType(type);

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
        /// <summary>
        /// 每個品項的收貨狀態
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetPlstIdList()
        {
            List<Parametersrc> stores = new List<Parametersrc>();
            _ptersrc = new ParameterMgr(mySqlConnectionString);
            string type = string.Empty;
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["Type"]))
                {
                    type = Request.Params["Type"];
                }
                stores = _ptersrc.GetElementType(type);

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
        #endregion

        #region 料位管理->採購單 需要的方法
        /// <summary>
        /// 讀取多個Excel 其中循環的item對應的就是多個sheet
        /// </summary>
        public void ReadUpload()
        {
            string newName = string.Empty;
            string json = string.Empty;

            try
            {
                if (Request.Files["IplasImportExcelFile"] != null && Request.Files["IplasImportExcelFile"].ContentLength > 0)
                {
                    #region MyRegion


                    HttpPostedFileBase excelFile = Request.Files["IplasImportExcelFile"];
                    //FileManagement fileManagement = new FileManagement();
                    newName = Server.MapPath(excelPath) + excelFile.FileName;
                    excelFile.SaveAs(newName);
                    ExcelHelperXhf helper = new ExcelHelperXhf();
                    DataSet ds = helper.GetExcelModel(newName);
                    DataTable _dt = new DataTable();
                    foreach (DataTable item in ds.Tables)
                    {
                        _dt = item;
                    }
                    #endregion


                }
            }
            catch (Exception ex)
            {


            }
        }
        /// <summary>
        /// 匯出pdf文檔
        /// </summary>
        //public void WritePdf()
        //{
        //    string newName = string.Empty;
        //    string json = string.Empty;
        //    IpodQuery ipod = new IpodQuery();
        //    IpoQuery ipo = new IpoQuery();

        //    if (!string.IsNullOrEmpty(Request.Params["Poid"]))
        //    {
        //        ipo.po_id = Request.Params["Poid"];
        //    }
        //    if (!string.IsNullOrEmpty(Request.Params["Potype"]))
        //    {
        //        ipo.po_type = Request.Params["Potype"];
        //    }
        //    List<IpodQuery> ipodStore = new List<IpodQuery>();
        //    List<IpoQuery> ipoStore = new List<IpoQuery>();
        //    _ipoMgr = new IpoMgr(mySqlConnectionString);
        //    int totalCount = 0;
        //    ipo.IsPage = false;
        //    ipoStore = _ipoMgr.GetIpoList(ipo, out  totalCount);
        //    try
        //    {
        //        #region 採購單匯出

        //        BaseFont bf = BaseFont.CreateFont("C:\\WINDOWS\\Fonts\\simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        //        iTextSharp.text.Font fontChinese = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.UNDERLINE, iTextSharp.text.BaseColor.RED);
        //        iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
        //        string filename = "採購單"+ DateTime.Now.ToString("yyyyMMddHHmmss") +".pdf";
        //        Document document = new Document(PageSize.A4, (float)5, (float)5, (float)20, (float)0.5);
        //        string newPDFName = Server.MapPath(excelPath) + filename;
        //        PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(newPDFName, FileMode.Create));
        //        document.Open();
        //        //運送方式

        //           _paraMgr = new ParameterMgr(mySqlConnectionString);
        //        List<Parametersrc> parameterStore = new List<Parametersrc>();


        //        parameterStore = _paraMgr.GetElementType("product_freight");

        //        for (int a = 0; a < ipoStore.Count; a++)//循環單頭
        //        {
        //            //GetIpodListExprot
        //            _ipodMgr = new IpodMgr(mySqlConnectionString);
        //            ipod = new IpodQuery();
        //            ipod.po_id = ipoStore[a].po_id;
        //            ipodStore = new List<IpodQuery>();
        //            ipodStore = _ipodMgr.GetIpodListExprot(ipod);
        //            Dictionary<int, List<IpodQuery>> product_freight_set_mapping = new Dictionary<int, List<IpodQuery>>();
        //            #region 通過運送方式把採購單分開--一張採購單，分成常溫，冷凍等採購單


        //            for (int i = 0; i < ipodStore.Count; i++)//通過運送方式保存到字典里
        //            {
        //                ipodStore[i].spec = GetProductSpec(ipodStore[i].item_id.ToString());
        //                IupcQuery upc = new IupcQuery();
        //                upc.item_id = ipodStore[i].item_id;
        //                List<IupcQuery> upcStore = new List<IupcQuery>();
        //                _IiupcMgr = new IupcMgr(mySqlConnectionString);
        //                upcStore = _IiupcMgr.GetIupcByItemID(upc);
        //                if (upcStore.Count > 0)
        //                {
        //                    ipodStore[i].upc_id = upcStore[0].upc_id;
        //                }
        //                int freiset = ipodStore[i].product_freight_set;
        //                if (!product_freight_set_mapping.Keys.Contains(freiset))
        //                {
        //                    List<IpodQuery> s = new List<IpodQuery>();
        //                    product_freight_set_mapping.Add(freiset, s);
        //                }
        //                product_freight_set_mapping[freiset].Add(ipodStore[i]);
        //            }
        //            #endregion

        //            #region 針對匯出一個而無商品的pdf


        //            if (ipodStore.Count == 0)
        //            {

        //                #region 獲取供應商信息

        //                Vendor vendor = new Vendor();
        //                _vendorMgr = new VendorMgr(mySqlConnectionString);

        //                vendor.erp_id = ipoStore[a].vend_id;
        //                vendor = _vendorMgr.GetSingle(vendor);
        //                #endregion
        //                #region 採購單標題


        //                PdfContentByte cb = writer.DirectContent;
        //                cb.BeginText();
        //                cb.SetFontAndSize(bf, 15);
        //                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "吉甲地好市集股份有限公司", 220, 790, 0);
        //                font = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑
        //                cb.SetFontAndSize(bf, 12);
        //                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "採購單" + "-" + ipoStore[a].po_type_desc, 280, 770, 0);
        //                cb.SetFontAndSize(bf, 8);
        //                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "公司電話：", 60, 760, 0);
        //                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "公司傳真：", 470, 760, 0);
        //                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "製造日期：" + DateTime.Now.ToString("yyyy/MM/dd"), 60, 750, 0);
        //                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "頁", 510, 750, 0);
        //                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "次", 530, 750, 0);
        //                #endregion

        //                PdfPTable ptable = new PdfPTable(6);

        //                ptable.WidthPercentage = 150;//表格寬度
        //                font = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
        //                ptable.SetTotalWidth(new float[] { 82, 50, 100, 90, 110, 71 });
        //                PdfPCell cell = new PdfPCell();
        //                cell.UseAscender = true;
        //                cell.HorizontalAlignment = Element.ALIGN_CENTER;//字體垂直居中
        //                cell.VerticalAlignment = Element.ALIGN_MIDDLE;//字體水平居中
        //                cell.BorderWidth = 0.1f;
        //                cell.BorderColor = new BaseColor(0, 0, 0);

        //                #region 上部分


        //                cell = new PdfPCell(new Phrase("採購單別:" + ipoStore[a].po_type, font));
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("交易幣別:" + "世界貨幣", font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("匯率:" + "浮動", font));
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("運輸方式:" , font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);


        //                cell = new PdfPCell(new Phrase("商品是新品么？:", font));//新品
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("所在層:", font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("採購單(" + ipoStore[a].po_type_desc + ")", font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("預約到貨日期:", font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);



        //                cell = new PdfPCell(new Phrase("採購單號:" + ipoStore[a].po_id, font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("課稅別:", font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("營業稅率:", font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("價格條件:", font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);



        //                cell = new PdfPCell(new Phrase("單據日期:" + DateTime.Now.ToString("yyyy/MM/dd"), font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("採購人員:" + ipoStore[a].buyer, font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase((System.Web.HttpContext.Current.Session["caller"] as Caller).user_username, font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("廠別代號:", font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("gigade(讀取)", font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);



        //                cell = new PdfPCell(new Phrase("廠商代號:" + ipoStore[a].vend_id, font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("", font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("付款條件(讀取)", font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("月結N天(讀取):", font));


        //                cell.Colspan = 3;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("廠商全名(讀取):" , font));
        //                cell.Colspan = 4;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("備註:", font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);


        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("廠商地址:", font));
        //                cell.Colspan = 6;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);


        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase(" ", font));
        //                cell.Colspan = 6;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                ptable.AddCell(cell);


        //                cell = new PdfPCell(new Phrase("聯絡人(讀取):", font));
        //                cell.Colspan = 2;

        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("廠商電話:" , font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("廠商傳真:" , font));


        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("送貨地址(讀取):", font));



        //                cell.Colspan = 6;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase(" ", font));




        //                cell.Colspan = 6;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("預計送貨日期(讀取):", font));




        //                cell.Colspan = 6;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("配送聯絡人(讀取):", font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("配送電話(讀取):", font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("配送傳真(讀取):", font));


        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("處理備註:", font));


        //                cell.Colspan = 6;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("運送備註:", font));
        //                cell.Colspan = 6;
        //                cell.DisableBorderSide(1);

        //                ptable.AddCell(cell);
        //                #endregion

        //                ptable.WriteSelectedRows(0, -1, 46, 740, writer.DirectContent);//顯示的開始行，結束航(-1為所有)x坐標，y坐標
        //                PdfPTable nulltable = new PdfPTable(2);
        //                nulltable.SetWidths(new int[] { 20, 20 });
        //                nulltable.DefaultCell.DisableBorderSide(1);
        //                nulltable.DefaultCell.DisableBorderSide(2);
        //                nulltable.DefaultCell.DisableBorderSide(4);
        //                nulltable.DefaultCell.DisableBorderSide(8);
        //                nulltable.AddCell("");
        //                nulltable.AddCell("");
        //                nulltable.SpacingAfter = 292;
        //                document.Add(nulltable);
        //                ptable = new PdfPTable(6);
        //                ptable.WidthPercentage = 86;//表格寬度
        //                font = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
        //                ptable.SetTotalWidth(new float[] { 90, 130, 50, 50, 60, 120 });
        //                cell = new PdfPCell(new Phrase("此採購單商品不存在!", font));


        //                cell.Colspan = 6;

        //                ptable.AddCell(cell);
        //                cb.EndText();
        //                document.Add(ptable);
        //                document.NewPage();


        //            }
        //            #endregion

        //            foreach (int key in product_freight_set_mapping.Keys)
        //            {
        //                #region 取出運送方式
        //                string procduct_freight = "";
        //                for (int i = 0; i < parameterStore.Count; i++)
        //                {
        //                    if (key.ToString() == parameterStore[i].ParameterCode)
        //                    {
        //                        procduct_freight = parameterStore[i].parameterName;
        //                    }
        //                }
        //                #endregion

        //                #region 獲取供應商信息

        //               Vendor vendor = new Vendor();
        //               _vendorMgr = new VendorMgr(mySqlConnectionString);

        //               vendor.erp_id = ipoStore[a].vend_id;
        //               vendor = _vendorMgr.GetSingle(vendor);
        //                #endregion
        //                #region 採購單標題


        //                PdfContentByte cb = writer.DirectContent;
        //                cb.BeginText();
        //                cb.SetFontAndSize(bf, 15);
        //                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "吉甲地好市集股份有限公司", 220, 790, 0);
        //                font = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑
        //                cb.SetFontAndSize(bf, 12);
        //                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "採購單" + "-" + ipoStore[a].po_type_desc, 280, 770, 0);
        //                cb.SetFontAndSize(bf, 8);
        //                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "公司電話：", 60, 760, 0);
        //                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "公司傳真：", 470, 760, 0);
        //                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "製造日期：" + DateTime.Now.ToString("yyyy/MM/dd"), 60, 750, 0);
        //                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "頁", 510, 750, 0);
        //                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "次", 530, 750, 0);
        //               #endregion

        //                PdfPTable ptable = new PdfPTable(6);

        //                ptable.WidthPercentage = 150;//表格寬度
        //                font = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
        //                ptable.SetTotalWidth(new float[] { 82, 50, 100, 90, 110, 71 });
        //                PdfPCell cell = new PdfPCell();
        //                cell.UseAscender = true;
        //                cell.HorizontalAlignment = Element.ALIGN_CENTER;//字體垂直居中
        //                cell.VerticalAlignment = Element.ALIGN_MIDDLE;//字體水平居中
        //                cell.BorderWidth = 0.1f;
        //                cell.BorderColor = new BaseColor(0, 0, 0);


        //                #region 上部分


        //                cell = new PdfPCell(new Phrase("採購單別:" + ipoStore[a].po_type, font));
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("交易幣別:" + "世界貨幣", font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("匯率:" + "浮動", font));
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("運輸方式:" + procduct_freight, font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);


        //                cell = new PdfPCell(new Phrase("商品是新品么？:", font));//新品
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("所在層:", font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("採購單(" + ipoStore[a].po_type_desc + ")", font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("預約到貨日期:", font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);



        //                cell = new PdfPCell(new Phrase("採購單號:" + ipoStore[a].po_id, font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("課稅別:", font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("營業稅率:", font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("價格條件:", font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);



        //                cell = new PdfPCell(new Phrase("單據日期:" + DateTime.Now.ToString("yyyy/MM/dd"), font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("採購人員:" + ipoStore[a].buyer, font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase((System.Web.HttpContext.Current.Session["caller"] as Caller).user_username, font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("廠別代號:" , font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("gigade(讀取)", font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);



        //                cell = new PdfPCell(new Phrase("廠商代號:" + ipoStore[a].vend_id, font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase(vendor == null ? "暫無此信息" : vendor.vendor_name_simple, font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("付款條件(讀取)", font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("月結N天(讀取):", font));


        //                cell.Colspan = 3;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase(vendor == null ? "廠商全名(讀取):暫無此信息" :"廠商全名:"+ vendor.vendor_name_full, font));
        //                cell.Colspan = 4;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("備註:", font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);


        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase(vendor == null ? "廠商地址:暫無此信息" : "廠商地址:"+vendor.company_address, font));
        //                cell.Colspan = 6;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);


        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase(" ", font));
        //                cell.Colspan = 6;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                ptable.AddCell(cell);


        //                cell = new PdfPCell(new Phrase("聯絡人(讀取):", font));
        //                cell.Colspan = 2;

        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase(vendor == null?"廠商電話:暫無此信息" :"廠商電話:"+ vendor.company_phone, font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase(vendor == null? "廠商傳真:暫無此信息" :"廠商傳真:"+ vendor.company_fax, font));


        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("送貨地址(讀取):", font));



        //                cell.Colspan = 6;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase(" ", font));




        //                cell.Colspan = 6;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("預計送貨日期(讀取):", font));




        //                cell.Colspan = 6;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("配送聯絡人(讀取):", font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("配送電話(讀取):", font));
        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("配送傳真(讀取):", font));


        //                cell.Colspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("處理備註:", font));


        //                cell.Colspan = 6;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("運送備註:", font));
        //                cell.Colspan = 6;
        //                cell.DisableBorderSide(1);

        //                ptable.AddCell(cell);
        //                #endregion


        //                ptable.WriteSelectedRows(0, -1, 46, 740, writer.DirectContent);//顯示的開始行，結束航(-1為所有)x坐標，y坐標
        //                PdfPTable nulltable = new PdfPTable(2);
        //                nulltable.SetWidths(new int[] { 20, 20 });
        //                nulltable.DefaultCell.DisableBorderSide(1);
        //                nulltable.DefaultCell.DisableBorderSide(2);
        //                nulltable.DefaultCell.DisableBorderSide(4);
        //                nulltable.DefaultCell.DisableBorderSide(8);
        //                nulltable.AddCell("");
        //                nulltable.AddCell("");
        //                nulltable.SpacingAfter = 292;
        //                document.Add(nulltable);
        //                ptable = new PdfPTable(6);
        //                ptable.WidthPercentage = 86;//表格寬度
        //                font = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
        //                ptable.SetTotalWidth(new float[] { 90, 130, 50, 50, 60, 120 });
        //                #region 下面表格頭部

        //                cell = new PdfPCell(new Phrase("條碼", font));
        //                cell.Rowspan = 2;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("品號", font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("採購數量", font));
        //                cell.HorizontalAlignment = Element.ALIGN_RIGHT;//.setHorizontalAlignment(Element.ALIGN_CENTER);
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("允收天數", font));
        //                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("製造日期", font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("備註", font));
        //                cell.Rowspan = 3;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);




        //                cell = new PdfPCell(new Phrase("品名", font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("允收數量", font));
        //                cell.HorizontalAlignment = Element.ALIGN_RIGHT;//.setHorizontalAlignment(Element.ALIGN_CENTER);
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("允出天數", font));
        //                cell.HorizontalAlignment = Element.ALIGN_RIGHT;//.setHorizontalAlignment(Element.ALIGN_CENTER);
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(2);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("有效日期", font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(4);
        //                cell.Rowspan = 2;
        //                ptable.AddCell(cell);



        //                cell = new PdfPCell(new Phrase("料位", font));
        //                cell.DisableBorderSide(1);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("規格", font));
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("不允收數量", font));
        //                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("有效期天數", font));
        //                cell.HorizontalAlignment = Element.ALIGN_RIGHT;//.setHorizontalAlignment(Element.ALIGN_CENTER);
        //                cell.DisableBorderSide(1);
        //                cell.DisableBorderSide(4);
        //                ptable.AddCell(cell);


        //                #endregion
        //                _ipodMgr = new IpodMgr(mySqlConnectionString);
        //                ipod = new IpodQuery();
        //                ipod.po_id = ipoStore[a].po_id;
        //                ipod.IsPage = false;
        //                ipodStore = new List<IpodQuery>();
        //                ipodStore = _ipodMgr.GetIpodList(ipod, out totalCount);


        //                List<IpodQuery> Ipodleibie = new List<IpodQuery>();
        //                Ipodleibie.AddRange(product_freight_set_mapping[key]);

        //                #region 循環讀取數據填入表格


        //                for (int i = 0; i < Ipodleibie.Count; i++)
        //                {
        //                    //string sResult = "";
        //                    //if (ipodStore[i].pod_id.ToString().Length < 4)
        //                    //{
        //                    //    for (int n = 0; n < 4 - (ipodStore[i].pod_id.ToString().Length); n++)
        //                    //    {
        //                    //        sResult += "0";
        //                    //    }

        //                    //}
        //                    cell = new PdfPCell(new Phrase(Ipodleibie[i].upc_id, font));//條碼
        //                    cell.Rowspan = 2;
        //                    cell.DisableBorderSide(1);
        //                    cell.DisableBorderSide(2);
        //                    ptable.AddCell(cell);
        //                    cell = new PdfPCell(new Phrase(Ipodleibie[i].Erp_Id.ToString(), font));//品號
        //                    cell.DisableBorderSide(1);
        //                    cell.DisableBorderSide(2);
        //                    cell.DisableBorderSide(4);
        //                    ptable.AddCell(cell);
        //                    cell = new PdfPCell(new Phrase(Ipodleibie[i].qty_ord.ToString(), font));//採購數量qty_ord
        //                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;//.setHorizontalAlignment(Element.ALIGN_CENTER);
        //                    cell.DisableBorderSide(1);
        //                    cell.DisableBorderSide(2);
        //                    cell.DisableBorderSide(4);
        //                    ptable.AddCell(cell);
        //                    cell = new PdfPCell(new Phrase(Ipodleibie[i].cde_dt_var.ToString(), font));//允收天數cde_dt_var
        //                    cell.DisableBorderSide(1);
        //                    cell.DisableBorderSide(2);
        //                    cell.DisableBorderSide(4);
        //                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;//.setHorizontalAlignment(Element.ALIGN_CENTER);
        //                    ptable.AddCell(cell);
        //                    cell = new PdfPCell(new Phrase("", font));//製造日期
        //                    cell.DisableBorderSide(1);
        //                    cell.DisableBorderSide(2);
        //                    cell.DisableBorderSide(4);
        //                    ptable.AddCell(cell);
        //                    cell = new PdfPCell(new Phrase("", font));//備註
        //                    cell.Rowspan = 3;
        //                    cell.DisableBorderSide(1);
        //                    cell.DisableBorderSide(4);
        //                    ptable.AddCell(cell);




        //                    cell = new PdfPCell(new Phrase(Ipodleibie[i].product_name, font));//品名
        //                    cell.DisableBorderSide(1);
        //                    cell.DisableBorderSide(2);
        //                    cell.DisableBorderSide(4);
        //                    ptable.AddCell(cell);
        //                    cell = new PdfPCell(new Phrase(Ipodleibie[i].qty_claimed.ToString(), font));//允收數量
        //                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;//.setHorizontalAlignment(Element.ALIGN_CENTER);
        //                    cell.DisableBorderSide(1);
        //                    cell.DisableBorderSide(2);
        //                    cell.DisableBorderSide(4);
        //                    ptable.AddCell(cell);
        //                    cell = new PdfPCell(new Phrase(Ipodleibie[i].cde_dt_shp.ToString(), font));//允出天數
        //                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;//.setHorizontalAlignment(Element.ALIGN_CENTER);
        //                    cell.DisableBorderSide(1);
        //                    cell.DisableBorderSide(2);
        //                    cell.DisableBorderSide(4);
        //                    ptable.AddCell(cell);
        //                    cell = new PdfPCell(new Phrase("", font));//有效日期
        //                    cell.DisableBorderSide(1);
        //                    cell.DisableBorderSide(4);
        //                    cell.Rowspan = 2;
        //                    ptable.AddCell(cell);



        //                    cell = new PdfPCell(new Phrase(Ipodleibie[i].loc_id, font));//料位
        //                    cell.DisableBorderSide(1);
        //                    ptable.AddCell(cell);
        //                    cell = new PdfPCell(new Phrase(Ipodleibie[i].spec, font));//規格
        //                    cell.DisableBorderSide(1);
        //                    cell.DisableBorderSide(4);
        //                    ptable.AddCell(cell);
        //                    cell = new PdfPCell(new Phrase(Ipodleibie[i].qty_damaged.ToString(), font));//不允收數量
        //                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //                    cell.DisableBorderSide(1);
        //                    cell.DisableBorderSide(4);
        //                    ptable.AddCell(cell);
        //                    cell = new PdfPCell(new Phrase(Ipodleibie[i].cde_dt_incr.ToString(), font));//有效期天數
        //                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //                    cell.DisableBorderSide(1);
        //                    cell.DisableBorderSide(4);
        //                    ptable.AddCell(cell);
        //                }
        //                #endregion

        //                //cell = new PdfPCell(new Phrase(" 數量合計:" + 5, font));
        //                //cell.Colspan = 2;

        //                //cell.DisableBorderSide(1);

        //                //cell.DisableBorderSide(8);
        //                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //                //ptable.AddCell(cell);
        //                //cell = new PdfPCell(new Phrase(" 採購金額:", font));
        //                //cell.Colspan = 2;
        //                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //                //cell.DisableBorderSide(1);

        //                //cell.DisableBorderSide(4);
        //                //cell.DisableBorderSide(8);
        //                //ptable.AddCell(cell);
        //                //cell = new PdfPCell(new Phrase(" 稅額:", font));
        //                //cell.Colspan = 2;
        //                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //                //cell.DisableBorderSide(1);

        //                //cell.DisableBorderSide(4);
        //                //ptable.AddCell(cell);
        //                //cell = new PdfPCell(new Phrase(" 金額合計:", font));
        //                //cell.Colspan = 2;

        //                //cell.DisableBorderSide(1);
        //                //cell.DisableBorderSide(4);

        //                //ptable.AddCell(cell);
        //                //Sumtable.AddCell(ptable);

        //                cb.EndText();
        //                // Sumtable.SpacingAfter = 0;
        //                ptable.SpacingAfter = 250;
        //                //Sumtable.WriteSelectedRows(0, -1, 60, 740, writer.DirectContent);//顯示的開始行，結束航(-1為所有)x坐標，y坐標
        //                document.Add(ptable);
        //                document.NewPage();


        //            }






        //    }


        //        document.Close();
        //        writer.Resume();

        //        Response.Clear();
        //        Response.Charset = "gb2312";
        //        Response.ContentEncoding = System.Text.Encoding.UTF8;
        //        Response.AddHeader("Content-Disposition", "attach-ment;filename=" + filename);
        //        Response.WriteFile(newPDFName);
        //        //}
        //        #endregion


        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        //cb.EndText();
        //        //writer.Resume();
        //        //Response.Clear();

        //    }
        //}
        public void WritePdf()
        {
            PdfHelper pdf = new PdfHelper();
            List<string> pdfList = new List<string>();
            //float[] arrColWidth_pftable = new float[] { 30,100, 80, 60, 60, 60, 60 };
            float[] arrColWidth = new float[] { 30, 100, 80, 60, 60, 60, 60 };
            int index = 0;
            string newFileName = string.Empty;
            string newName = string.Empty;
            string json = string.Empty;
            IpodQuery ipod = new IpodQuery();
            IpoQuery ipo = new IpoQuery();

            if (!string.IsNullOrEmpty(Request.Params["Poid"]))
            {
                ipo.po_id = Request.Params["Poid"];
            }
            if (!string.IsNullOrEmpty(Request.Params["Potype"]))
            {
                ipo.po_type = Request.Params["Potype"];
            }
            if (!string.IsNullOrEmpty(Request.Params["start_time"]))
            {
                ipo.start_time = Convert.ToDateTime(Request.Params["start_time"].ToString());
            }
            if (!string.IsNullOrEmpty(Request.Params["end_time"]))
            {
                ipo.end_time = Convert.ToDateTime(Request.Params["end_time"].ToString());
            }
            if (!string.IsNullOrEmpty(Request.Params["freight"]))
            {
                ipo.freight = Convert.ToInt32(Request.Params["freight"].ToString());
            }
            List<IpodQuery> ipodStore = new List<IpodQuery>();
            List<IpoQuery> ipoStore = new List<IpoQuery>();
            _ipoMgr = new IpoMgr(mySqlConnectionString);
            int totalCount = 0;
            ipo.IsPage = false;
            ipoStore = _ipoMgr.GetIpoList(ipo, out  totalCount);
            //if (!string.IsNullOrEmpty(Request.Params["freight"]))
            //{
            //    if (Request.Params["freight"].ToString() != "0")
            //    {
            //        _ipodMgr = new IpodMgr(mySqlConnectionString);
            //        List<IpoQuery> newstore = new List<IpoQuery>();
            //        foreach (IpoQuery item in ipoStore)
            //        {
            //            if (!string.IsNullOrEmpty(item.po_id))
            //            {
            //                if (_ipodMgr.GetIpodfreight(item.po_id, Convert.ToInt32(Request.Params["freight"].ToString())))
            //                {
            //                    newstore.Add(item);
            //                }
            //            }
            //        }
            //        ipoStore = newstore;
            //    }
            //}
            try
            {
                #region 採購單匯出

                BaseFont bf = BaseFont.CreateFont("C:\\WINDOWS\\Fonts\\simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font fontChinese = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.UNDERLINE, iTextSharp.text.BaseColor.RED);
                iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
                string filename = "採購單" + DateTime.Now.ToString("yyyyMMddHHmmss");
                Document document = new Document(PageSize.A4);
                string newPDFName = Server.MapPath(excelPath) + filename;
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(newPDFName, FileMode.Create));
                document.Open();
                //運送方式

                _paraMgr = new ParameterMgr(mySqlConnectionString);
                List<Parametersrc> parameterStore = new List<Parametersrc>();


                parameterStore = _paraMgr.GetElementType("product_freight");
                if (ipoStore.Count == 0)
                {

                }
                for (int a = 0; a < ipoStore.Count; a++)//循環單頭
                {
                    _ipodMgr = new IpodMgr(mySqlConnectionString);
                    ipod = new IpodQuery();
                    ipod.po_id = ipoStore[a].po_id;
                    ipodStore = new List<IpodQuery>();
                    ipodStore = _ipodMgr.GetIpodListExprot(ipod);
                    Dictionary<int, List<IpodQuery>> product_freight_set_mapping = new Dictionary<int, List<IpodQuery>>();
                    #region 通過運送方式把採購單分開--一張採購單，分成常溫，冷凍等採購單


                    for (int i = 0; i < ipodStore.Count; i++)//通過運送方式保存到字典里
                    {
                        ipodStore[i].spec = GetProductSpec(ipodStore[i].prod_id.ToString());//--------取值出錯了item_id-----------
                        IupcQuery upc = new IupcQuery();
                        _IiupcMgr = new IupcMgr(mySqlConnectionString);

                        upc.item_id = uint.Parse(ipodStore[i].prod_id);//--------取值出錯了item_id-----------
                        //獲取國際條碼
                        List<IupcQuery> upcInternationalStore = new List<IupcQuery>();
                        upc.upc_type_flg = "1";
                        upcInternationalStore = _IiupcMgr.GetIupcByType(upc);
                        //獲取店內條碼
                        List<IupcQuery> upcShopStore = new List<IupcQuery>();
                        upc.upc_type_flg = "3";
                        upcShopStore = _IiupcMgr.GetIupcByType(upc);
                        if (upcInternationalStore.Count > 0)
                        {
                            ipodStore[i].upc_id_international = upcInternationalStore[0].upc_id;
                        }
                        if (upcShopStore.Count > 0)
                        {
                            ipodStore[i].upc_id_shop = upcShopStore[0].upc_id;
                        }
                        int freiset = ipodStore[i].product_freight_set;
                        if (!product_freight_set_mapping.Keys.Contains(freiset))
                        {
                            List<IpodQuery> s = new List<IpodQuery>();
                            product_freight_set_mapping.Add(freiset, s);
                        }
                        product_freight_set_mapping[freiset].Add(ipodStore[i]);
                    }
                    #endregion

                    #region 針對匯出一個而無商品的pdf
                    if (ipodStore.Count == 0)
                    {

                        #region 獲取供應商信息

                        Vendor vendor = new Vendor();
                        _vendorMgr = new VendorMgr(mySqlConnectionString);

                        vendor.erp_id = ipoStore[a].vend_id;
                        vendor = _vendorMgr.GetSingle(vendor);
                        #endregion
                        #region 採購單標題

                        PdfPTable ptable = new PdfPTable(7);


                        ptable.WidthPercentage = 100;//表格寬度
                        font = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
                        ptable.SetTotalWidth(arrColWidth);
                        PdfPCell cell = new PdfPCell();

                        cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 15)));
                        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("    吉甲地好市集股份有限公司", new iTextSharp.text.Font(bf, 15)));
                        cell.VerticalAlignment = Element.ALIGN_CENTER;//字體水平居左
                        cell.Colspan = 5;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 12)));
                        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                        cell.Colspan = 3;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("採購單" + "-" + ipoStore[a].po_type_desc, new iTextSharp.text.Font(bf, 12)));
                        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                        cell.Colspan = 4;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("公司電話：", new iTextSharp.text.Font(bf, 8)));
                        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                        cell.Colspan = 6;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("公司傳真：", new iTextSharp.text.Font(bf, 8)));
                        cell.VerticalAlignment = Element.ALIGN_RIGHT;//字體水平居右
                        cell.Colspan = 1;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("製造日期：" + DateTime.Now.ToString("yyyy/MM/dd"), new iTextSharp.text.Font(bf, 8)));
                        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                        cell.Colspan = 3;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);

                        cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 8)));
                        cell.VerticalAlignment = Element.ALIGN_RIGHT;//字體水平居右
                        cell.Colspan = 4;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);

                        cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 8)));
                        cell.VerticalAlignment = Element.ALIGN_RIGHT;//字體水平居右
                        cell.Colspan = 7;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);

                        cell.UseAscender = true;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;//字體垂直居中
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;//字體水平居中
                        cell.BorderWidth = 0.1f;
                        cell.BorderColor = new BaseColor(0, 0, 0);

                        #endregion
                        #region 上部分


                        cell = new PdfPCell(new Phrase("採購單別:" + ipoStore[a].po_type, font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("交易幣別:" + "世界貨幣", font));
                        cell.Colspan = 1;
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("匯率:" + "浮動", font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("運輸方式:", font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        ptable.AddCell(cell);


                        cell = new PdfPCell(new Phrase("商品是新品么？:", font));//新品
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("所在層:", font));
                        cell.Colspan = 1;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("採購單(" + ipoStore[a].po_type_desc + ")", font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("預約到貨日期:", font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        ptable.AddCell(cell);



                        cell = new PdfPCell(new Phrase("採購單號:" + ipoStore[a].po_id, font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("課稅別:", font));
                        cell.Colspan = 1;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("營業稅率:", font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("價格條件:", font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        ptable.AddCell(cell);



                        cell = new PdfPCell(new Phrase("單據日期:" + DateTime.Now.ToString("yyyy/MM/dd"), font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("採購人員:" + ipoStore[a].buyer, font));
                        cell.Colspan = 1;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase((System.Web.HttpContext.Current.Session["caller"] as Caller).user_username, font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("廠別代號:", font));
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("gigade(讀取)", font));
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        ptable.AddCell(cell);



                        cell = new PdfPCell(new Phrase("廠商代號:" + ipoStore[a].vend_id, font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("", font));
                        cell.Colspan = 1;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("付款條件(讀取)", font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("月結N天(讀取):", font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        ptable.AddCell(cell);

                        cell = new PdfPCell(new Phrase("廠商全名(讀取):", font));
                        cell.Colspan = 5;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("備註:", font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);


                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("廠商地址:", font));
                        cell.Colspan = 7;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);


                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase(" ", font));
                        cell.Colspan = 7;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);


                        cell = new PdfPCell(new Phrase("聯絡人(讀取):", font));
                        cell.Colspan = 2;

                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("廠商電話:", font));
                        cell.Colspan = 1;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("廠商傳真:", font));
                        cell.Colspan = 4;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        ptable.AddCell(cell);

                        cell = new PdfPCell(new Phrase("送貨地址(讀取):", font));
                        cell.Colspan = 7;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);

                        cell = new PdfPCell(new Phrase(" ", font));
                        cell.Colspan = 7;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);

                        cell = new PdfPCell(new Phrase("預計送貨日期(讀取):", font));
                        cell.Colspan = 7;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);

                        cell = new PdfPCell(new Phrase("配送聯絡人(讀取):", font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("配送電話(讀取):", font));
                        cell.Colspan = 1;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);

                        cell = new PdfPCell(new Phrase("配送傳真(讀取):", font));
                        cell.Colspan = 4;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        ptable.AddCell(cell);

                        cell = new PdfPCell(new Phrase("處理備註:", font));
                        cell.Colspan = 7;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("運送備註:", font));
                        cell.Colspan = 7;
                        cell.DisableBorderSide(1);

                        ptable.AddCell(cell);
                        #endregion

                        cell = new PdfPCell(new Phrase("此採購單商品不存在!", font));
                        cell.Colspan = 7;

                        ptable.AddCell(cell);

                        newFileName = newPDFName + "_part" + index++ + "." + "pdf";
                        pdf.ExportDataTableToPDF(newFileName, ptable, "", "");
                        pdfList.Add(newFileName);

                        document.Add(ptable);
                        document.NewPage();


                    }
                    #endregion

                    foreach (int key in product_freight_set_mapping.Keys)
                    {
                        #region 取出運送方式
                        string procduct_freight = "";
                        for (int i = 0; i < parameterStore.Count; i++)
                        {
                            if (key.ToString() == parameterStore[i].ParameterCode)
                            {
                                procduct_freight = parameterStore[i].parameterName;
                            }
                        }
                        #endregion

                        #region 獲取供應商信息

                        Vendor vendor = new Vendor();
                        _vendorMgr = new VendorMgr(mySqlConnectionString);

                        vendor.erp_id = ipoStore[a].vend_id;
                        vendor = _vendorMgr.GetSingle(vendor);
                        #endregion

                        #region 採購單標題


                        PdfPTable ptable = new PdfPTable(7);


                        ptable.WidthPercentage = 100;//表格寬度
                        font = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
                        ptable.SetTotalWidth(arrColWidth);
                        PdfPCell cell = new PdfPCell();

                        cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 15)));
                        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("      吉甲地好市集股份有限公司", new iTextSharp.text.Font(bf, 15)));
                        cell.VerticalAlignment = Element.ALIGN_CENTER;//字體水平居左
                        cell.Colspan = 5;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 12)));
                        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                        cell.Colspan = 3;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("    採購單" + "-" + ipoStore[a].po_type_desc, new iTextSharp.text.Font(bf, 12)));
                        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                        cell.Colspan = 4;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("公司電話：", new iTextSharp.text.Font(bf, 8)));
                        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                        cell.Colspan = 6;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("公司傳真：", new iTextSharp.text.Font(bf, 8)));
                        cell.VerticalAlignment = Element.ALIGN_RIGHT;//字體水平居右
                        cell.Colspan = 1;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("製造日期：" + DateTime.Now.ToString("yyyy/MM/dd"), new iTextSharp.text.Font(bf, 8)));
                        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                        cell.Colspan = 3;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);

                        cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 8)));
                        cell.VerticalAlignment = Element.ALIGN_RIGHT;//字體水平居右
                        cell.Colspan = 4;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);

                        cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 8)));
                        cell.VerticalAlignment = Element.ALIGN_RIGHT;//字體水平居右
                        cell.Colspan = 7;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);

                        cell.UseAscender = true;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;//字體垂直居中
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;//字體水平居中
                        cell.BorderWidth = 0.1f;
                        cell.BorderColor = new BaseColor(0, 0, 0);
                        #endregion

                        #region 上部分


                        cell = new PdfPCell(new Phrase("採購單別:" + ipoStore[a].po_type, font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("交易幣別:" + "世界貨幣", font));
                        cell.Colspan = 1;
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("匯率:" + "浮動", font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        if (procduct_freight != "常溫" && procduct_freight != "冷凍")
                        {
                            ;
                        }
                        cell = new PdfPCell(new Phrase("運輸方式:" + procduct_freight, font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        ptable.AddCell(cell);


                        cell = new PdfPCell(new Phrase("商品是新品么？:", font));//新品
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("所在層:", font));
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("採購單(" + ipoStore[a].po_type_desc + ")", font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("預約到貨日期:", font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        ptable.AddCell(cell);



                        cell = new PdfPCell(new Phrase("採購單號:" + ipoStore[a].po_id, font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("課稅別:", font));
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("營業稅率:", font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("價格條件:", font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        ptable.AddCell(cell);



                        cell = new PdfPCell(new Phrase("單據日期:" + DateTime.Now.ToString("yyyy/MM/dd"), font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("採購人員:", font));
                        //cell = new PdfPCell(new Phrase("採購人員:" + ipoStore[a].buyer, font));
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase((System.Web.HttpContext.Current.Session["caller"] as Caller).user_username, font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("廠別代號:", font));
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("gigade(讀取)", font));
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        ptable.AddCell(cell);



                        cell = new PdfPCell(new Phrase("廠商代號:" + ipoStore[a].vend_id, font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase(vendor == null ? "暫無此信息" : vendor.vendor_name_simple, font));
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("付款條件(讀取)", font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("月結N天(讀取):", font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        ptable.AddCell(cell);

                        cell = new PdfPCell(new Phrase(vendor == null ? "廠商全名(讀取):暫無此信息" : "廠商全名:" + vendor.vendor_name_full, font));
                        cell.Colspan = 5;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("備註:", font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);


                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase(vendor == null ? "廠商地址:暫無此信息" : "廠商地址:" + vendor.company_address, font));
                        cell.Colspan = 7;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);


                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase(" ", font));
                        cell.Colspan = 7;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);


                        cell = new PdfPCell(new Phrase("聯絡人(讀取):", font));
                        cell.Colspan = 2;

                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase(vendor == null ? "廠商電話:暫無此信息" : "廠商電話:" + vendor.company_phone, font));
                        cell.Colspan = 1;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase(vendor == null ? "廠商傳真:暫無此信息" : "廠商傳真:" + vendor.company_fax, font));


                        cell.Colspan = 4;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        ptable.AddCell(cell);

                        cell = new PdfPCell(new Phrase("送貨地址(讀取):", font));
                        cell.Colspan = 7;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);

                        cell = new PdfPCell(new Phrase(" ", font));
                        cell.Colspan = 7;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);

                        cell = new PdfPCell(new Phrase("預計送貨日期(讀取):", font));
                        cell.Colspan = 7;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("配送聯絡人(讀取):", font));
                        cell.Colspan = 2;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("配送電話(讀取):", font));
                        cell.Colspan = 1;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        cell.DisableBorderSide(8);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("配送傳真(讀取):", font));


                        cell.Colspan = 4;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        cell.DisableBorderSide(4);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("處理備註:", font));


                        cell.Colspan = 7;
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("運送備註:", font));
                        cell.Colspan = 7;
                        cell.DisableBorderSide(1);

                        ptable.AddCell(cell);
                        #endregion
                        #region 下面表格頭部
                        cell = new PdfPCell(new Phrase("序號", font));
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //cell.DisableBorderSide(2);
                        cell.Rowspan = 3;
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("國際條碼", font));
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("品號", font));
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("採購數量", font));
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("允收天數", font));
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("製造日期", font));
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("備註", font));
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);



                        cell = new PdfPCell(new Phrase("供應商店內碼", font));
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("品名", font));
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("允收數量", font));
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("允出天數", font));
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("有效日期", font));
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("", font));
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("料位", font));
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("規格", font));
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                        ptable.AddCell(cell);

                        cell = new PdfPCell(new Phrase("不允收數量", font));
                        cell.DisableBorderSide(1);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("有效期天數", font));
                        cell.DisableBorderSide(1);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("", font));
                        cell.DisableBorderSide(1);
                        ptable.AddCell(cell);
                        cell = new PdfPCell(new Phrase("", font));
                        cell.DisableBorderSide(1);
                        ptable.AddCell(cell);

                        #endregion

                        _ipodMgr = new IpodMgr(mySqlConnectionString);
                        ipod = new IpodQuery();
                        ipod.po_id = ipoStore[a].po_id;
                        ipod.IsPage = false;
                        ipodStore = new List<IpodQuery>();
                        ipodStore = _ipodMgr.GetIpodList(ipod, out totalCount);

                        List<IpodQuery> Ipodleibie = new List<IpodQuery>();
                        Ipodleibie.AddRange(product_freight_set_mapping[key]);

                        #region 循環讀取數據填入表格
                        DataTable Ipod_dt = new DataTable();
                        Ipod_dt.Columns.Add("序號", typeof(string));
                        Ipod_dt.Columns.Add("國際條碼", typeof(string));
                        Ipod_dt.Columns.Add("品號", typeof(string));
                        Ipod_dt.Columns.Add("採購數量", typeof(string));
                        Ipod_dt.Columns.Add("允收天數", typeof(string));
                        Ipod_dt.Columns.Add("製造日期", typeof(string));
                        Ipod_dt.Columns.Add("備註", typeof(string));
                        Ipod_dt.Columns.Add("Empty_1", typeof(string));
                        Ipod_dt.Columns.Add("供應商店內碼", typeof(string));
                        Ipod_dt.Columns.Add("品名", typeof(string));
                        Ipod_dt.Columns.Add("允收數量", typeof(string));
                        Ipod_dt.Columns.Add("允出天數", typeof(string));
                        Ipod_dt.Columns.Add("有效日期", typeof(string));
                        Ipod_dt.Columns.Add("Empty_3", typeof(string));
                        Ipod_dt.Columns.Add("Empty_4", typeof(string));
                        Ipod_dt.Columns.Add("料位", typeof(string));
                        Ipod_dt.Columns.Add("規格", typeof(string));
                        Ipod_dt.Columns.Add("不允收數量", typeof(string));
                        Ipod_dt.Columns.Add("有效期天數", typeof(string));
                        Ipod_dt.Columns.Add("Empty_5", typeof(string));
                        Ipod_dt.Columns.Add("Empty_6", typeof(string));

                        for (int i = 0; i < Ipodleibie.Count; i++)
                        {
                            DataRow newRow = Ipod_dt.NewRow();
                            newRow["國際條碼"] = Ipodleibie[i].upc_id_international;
                            newRow["品號"] = Ipodleibie[i].Erp_Id.ToString();
                            newRow["採購數量"] = Ipodleibie[i].qty_ord.ToString();
                            newRow["允收天數"] = Ipodleibie[i].cde_dt_var.ToString();
                            newRow["製造日期"] = "";
                            newRow["備註"] = "";
                            newRow["Empty_1"] = (i + 1).ToString(); //序號
                            newRow["供應商店內碼"] = Ipodleibie[i].upc_id_shop;
                            newRow["品名"] = Ipodleibie[i].product_name;
                            newRow["允收數量"] = Ipodleibie[i].qty_claimed.ToString();
                            newRow["允出天數"] = Ipodleibie[i].cde_dt_shp.ToString();
                            newRow["有效日期"] = "";
                            newRow["Empty_3"] = "";
                            newRow["Empty_4"] = "";
                            newRow["料位"] = Ipodleibie[i].loc_id;
                            newRow["規格"] = Ipodleibie[i].spec;
                            newRow["不允收數量"] = Ipodleibie[i].qty_damaged.ToString();
                            newRow["有效期天數"] = Ipodleibie[i].cde_dt_incr.ToString();
                            newRow["Empty_5"] = "";
                            newRow["Empty_6"] = "";
                            Ipod_dt.Rows.Add(newRow);
                        }

                        #endregion
                        ////////
                        newFileName = newPDFName + "_part" + index++ + "." + "pdf";

                        pdf.ExportDataTableToPDF(Ipod_dt, false, newFileName, arrColWidth, ptable, "", "", 7, 7);/*第一7是列，第二個是行*/
                        pdfList.Add(newFileName);

                    }






                }
                newFileName = newPDFName + "." + "pdf";
                pdf.MergePDF(pdfList, newFileName);

                Response.Clear();
                Response.Charset = "gb2312";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.AddHeader("Content-Disposition", "attach-ment;filename=" + filename + ".pdf");
                Response.WriteFile(newFileName);
                //}
                #endregion
            }
            catch (Exception ex)
            {
                //cb.EndText();
                //writer.Resume();
                //Response.Clear();

            }
        }

        #region 採購單驗收不符報表
        public ActionResult IpodList()
        {
            return View();

        }
        public void WriteExcel()
        {
            try
            {
                IpodQuery ipod = new IpodQuery();
                List<IpodQuery> ipoStore = new List<IpodQuery>();
                if (!string.IsNullOrEmpty(Request.Params["freight"]))
                {
                    ipod.product_freight_set = int.Parse(Request.Params["freight"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["updateuser"]))
                {
                    ipod.change_user = int.Parse(Request.Params["updateuser"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["erp_id"]))
                {
                    ipod.Erp_Id = Request.Params["erp_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["Potype"]))
                {
                    ipod.po_type = Request.Params["Potype"];//類別
                }
                if (Request.Params["vendor_id"] != "null")
                {
                    ipod.vendor_id = uint.Parse(Request.Params["vendor_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["vendor_name_full"]))
                {
                    string vendorName = Request.Params["vendor_name_full"].ToString();
                    int index1 = vendorName.IndexOf('%');
                    int index2 = vendorName.IndexOf('_');
                    if (index1 != -1)
                    {
                        string start = vendorName.Substring(0, index1);
                        string end = vendorName.Substring(index1 + 1);
                        vendorName = start + "/" + "%" + end;
                    }
                    if (index2 != -1)
                    {
                        string start = vendorName.Substring(0, index2);
                        string end = vendorName.Substring(index2 + 1);
                        vendorName = start + "/" + "_" + end;
                    }
                    ipod.vendor_name_full = vendorName;
                }
                if (!string.IsNullOrEmpty(Request.Params["check"]))
                {
                    bool check = true;
                    if (bool.TryParse(Request.Params["check"], out check))
                    {
                        ipod.Check = check;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["product_name"]))
                {
                    string product_name = Request.Params["product_name"].ToString();
                    int index1 = product_name.IndexOf('%');
                    int index2 = product_name.IndexOf('_');
                    if (index1 != -1)
                    {
                        string start = product_name.Substring(0, index1);
                        string end = product_name.Substring(index1 + 1);
                        product_name = start + "/" + "%" + end;
                    }
                    if (index2 != -1)
                    {
                        string start = product_name.Substring(0, index2);
                        string end = product_name.Substring(index2 + 1);
                        product_name = start + "/" + "_" + end;
                    }
                    ipod.product_name = product_name;
                }
                if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                {
                    uint product_id = 0;
                    if (uint.TryParse(Request.Params["product_id"], out product_id))
                    {
                        ipod.product_id = product_id;
                    }
                }
                DateTime date;
                if (!string.IsNullOrEmpty(Request.Params["start_time"]))
                {
                    if (DateTime.TryParse(Request.Params["start_time"].ToString(), out date))
                    {
                        ipod.start_time = date;
                    }

                }
                if (!string.IsNullOrEmpty(Request.Params["end_time"]))
                {
                    if (DateTime.TryParse(Request.Params["end_time"].ToString(), out date))
                    {
                        ipod.end_time = Convert.ToDateTime(date.ToString("yyyy-MM-dd 23:59:59"));
                    }
                }
                ipod.IsPage = false;
                _ipodMgr = new IpodMgr(mySqlConnectionString);
                int totalCount = 0;
                ipoStore = _ipodMgr.GetIpodListNo(ipod, out totalCount);
                DataTable _newDt = new DataTable();
                _newDt.Columns.Add("採購單別", typeof(string));
                _newDt.Columns.Add("採購單號", typeof(string));
                _newDt.Columns.Add("供應商編號", typeof(string));
                _newDt.Columns.Add("供應商名稱", typeof(string));
                _newDt.Columns.Add("品號", typeof(string));
                _newDt.Columns.Add("商品編號", typeof(string));
                _newDt.Columns.Add("商品細項編號", typeof(string));
                _newDt.Columns.Add("商品名稱", typeof(string));
                _newDt.Columns.Add("溫層", typeof(string));
                _newDt.Columns.Add("規格", typeof(string));
                _newDt.Columns.Add("採購數量", typeof(string));
                _newDt.Columns.Add("允收數量", typeof(string));
                _newDt.Columns.Add("不允收量", typeof(string));
                _newDt.Columns.Add("創建時間", typeof(string));
                _newDt.Columns.Add("創建人", typeof(string));
                _newDt.Columns.Add("異動時間", typeof(string));
                _newDt.Columns.Add("異動人", typeof(string));



                for (int i = 0; i < ipoStore.Count; i++)
                {
                    DataRow newRow = _newDt.NewRow();
                    newRow["採購單別"] = ipoStore[i].parameterName;
                    newRow["採購單號"] = ipoStore[i].po_id + " ";
                    newRow["品號"] = ipoStore[i].Erp_Id + " ";
                    newRow["商品名稱"] = ipoStore[i].product_name;
                    newRow["供應商名稱"] = ipoStore[i].vendor_name_full;
                    newRow["規格"] = ipoStore[i].spec;
                    newRow["採購數量"] = ipoStore[i].qty_ord;
                    newRow["允收數量"] = ipoStore[i].qty_claimed;
                    newRow["不允收量"] = ipoStore[i].qty_damaged;
                    newRow["商品細項編號"] = ipoStore[i].item_id;
                    newRow["商品編號"] = ipoStore[i].productid;
                    newRow["供應商編號"] = ipoStore[i].vendor_id;
                    newRow["創建時間"] = ipoStore[i].create_dtim.ToString("yyyy-MM-dd HH:mm:ss");
                    newRow["創建人"] = ipoStore[i].create_username;
                    newRow["異動時間"] = ipoStore[i].change_dtim.ToString("yyyy-MM-dd HH:mm:ss");
                    newRow["異動人"] = ipoStore[i].change_username;
                    newRow["溫層"] = ipoStore[i].product_freight_set == 1 ? "常溫" : "冷凍";
                    _newDt.Rows.Add(newRow);
                }
                string fileName = string.Empty;
                if (ipod.Check)
                {
                    fileName = "採購單驗收不符報表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                }
                else
                {
                    fileName = "採購單驗收報表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                }
                MemoryStream ms = ExcelHelperXhf.ExportDT(_newDt, "");
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

        public HttpResponseBase GetIpodListNo()
        {
            IpodQuery ipod = new IpodQuery();
            string json = string.Empty;
            ipod.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            ipod.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            if (!string.IsNullOrEmpty(Request.Params["freight"]))
            {
                ipod.product_freight_set = int.Parse(Request.Params["freight"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["updateuser"]))
            {
                ipod.change_user = int.Parse(Request.Params["updateuser"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["erp_id"]))
            {
                ipod.Erp_Id = Request.Params["erp_id"];
            }
            if (!string.IsNullOrEmpty(Request.Params["Potype"]))
            {
                ipod.po_type = Request.Params["Potype"];//類別
            }
            if (!string.IsNullOrEmpty(Request.Params["vendor_id"]))
            {
                UInt64 vendorid = 0;
                if (UInt64.TryParse(Request.Params["vendor_id"], out vendorid))
                {
                    ipod.vendor_id = vendorid;
                }
            }
            if (!string.IsNullOrEmpty(Request.Params["vendor_name_full"]))
            {
                string vendorName = Request.Params["vendor_name_full"].ToString();
                int index1 = vendorName.IndexOf('%');
                int index2 = vendorName.IndexOf('_');
                if (index1 != -1)
                {
                    string start = vendorName.Substring(0, index1);
                    string end = vendorName.Substring(index1 + 1);
                    vendorName = start + "/" + "%" + end;
                }
                if (index2 != -1)
                {
                    string start = vendorName.Substring(0, index2);
                    string end = vendorName.Substring(index2 + 1);
                    vendorName = start + "/" + "_" + end;
                }
                ipod.vendor_name_full = vendorName;
            }
            if (!string.IsNullOrEmpty(Request.Params["check"]))
            {
                bool check = true;
                if (bool.TryParse(Request.Params["check"], out check))
                {
                    ipod.Check = check;
                }
            }
            if (!string.IsNullOrEmpty(Request.Params["product_name"]))
            {

                string product_name = Request.Params["product_name"].ToString();
                int index1 = product_name.IndexOf('%');
                int index2 = product_name.IndexOf('_');
                if (index1 != -1)
                {
                    string start = product_name.Substring(0, index1);
                    string end = product_name.Substring(index1 + 1);
                    product_name = start + "/" + "%" + end;
                }
                if (index2 != -1)
                {
                    string start = product_name.Substring(0, index2);
                    string end = product_name.Substring(index2 + 1);
                    product_name = start + "/" + "_" + end;
                }
                ipod.product_name = product_name;
            }
            if (!string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                uint product_id = 0;
                if (uint.TryParse(Request.Params["product_id"], out product_id))
                {
                    ipod.product_id = product_id;
                }
            }
            if (!string.IsNullOrEmpty(Request.Params["start_time"]))
            {
                ipod.start_time = Convert.ToDateTime(Request.Params["start_time"].ToString());
            }
            if (!string.IsNullOrEmpty(Request.Params["end_time"]))
            {
                ipod.end_time = Convert.ToDateTime(Convert.ToDateTime(Request.Params["end_time"].ToString()).ToString("yyyy-MM-dd 23:59:59"));
            }
            try
            {
                List<IpodQuery> store = new List<IpodQuery>();
                _ipodMgr = new IpodMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _ipodMgr.GetIpodListNo(ipod, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        public HttpResponseBase GetUpdateUsersList()
        {
            string json = string.Empty;
            try
            {
                FgroupMySqlDao fdao = new FgroupMySqlDao(mySqlConnectionString);
                DataTable dt = fdao.GetFgroupLists();
                DataRow row = dt.NewRow();
                row[0] = "0";
                row[1] = "全部";
                dt.Rows.InsertAt(row, 0);
                json = "{data:" + JsonConvert.SerializeObject(dt, Formatting.Indented) + "}";//返回json數據
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


        #endregion

        #endregion

        // 等待料位報表
        public HttpResponseBase GetWaitLiaoWeiList()// createTime 2015/10/19 by yachao1120j
        {
            string json = string.Empty;
            int totalcount = 0;
            ProductItemQuery query = new ProductItemQuery();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
            productitemMgr = new ProductItemMgr(mySqlConnectionString);

            if (!string.IsNullOrEmpty(Request.Params["product_mode"]))
            {
                query.product_mode = Convert.ToInt32(Request.Params["product_mode"]);//出貨方式
            }
            if (!string.IsNullOrEmpty(Request.Params["freight"]))
            {
                query.product_freight_set = Convert.ToUInt32(Request.Params["freight"]);//溫層
            }
            //if (!string.IsNullOrEmpty(Request.Params["product_status"]))
            //{
            //    query.product_status = Convert.ToUInt32(Request.Params["product_status"]);//商品状态
            //}
            if (!string.IsNullOrEmpty(Request.Params["start_time"]))//開始時間
            {
                //query.start_time = Convert.ToDateTime(Request.Params["start_time"]).ToString("yyyy-MM-dd 00:00:00");
                query.start_time = (int)CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["start_time"]).ToString("yyyy-MM-dd 00:00:00"));
            }
            if (!string.IsNullOrEmpty(Request.Params["end_time"]))//結束時間
            {
                //query.end_time = Convert.ToDateTime(Request.Params["end_time"]).ToString("yyyy-MM-dd 23:59:59");
                query.end_time = (int)CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["end_time"]).ToString("yyyy-MM-dd 23:59:59"));
            }
            List<ProductItemQuery> list = productitemMgr.GetWaitLiaoWeiList(query, out totalcount);
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            json = "{success:true,totalCount:" + totalcount + ",data:" + JsonConvert.SerializeObject(list, Formatting.Indented, timeConverter) + "}";
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return Response;

        }

        //匯出 等待料位報表
        public void ExportCSV() // createTime 2015/10/21 by yachao1120j
        {
            ProductItemQuery query = new ProductItemQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["product_mode"]))
                {
                    query.product_mode = Convert.ToInt32(Request.Params["product_mode"]);//出貨方式
                }
                if (!string.IsNullOrEmpty(Request.Params["freight"]))
                {
                    query.product_freight_set = Convert.ToUInt32(Request.Params["freight"]);//溫層
                }
                //if (!string.IsNullOrEmpty(Request.Params["product_status"]))
                //{
                //    query.product_status = Convert.ToUInt32(Request.Params["product_status"]);//商品状态
                //}
                if (!string.IsNullOrEmpty(Request.Params["start_time"]))//開始時間
                {
                    //query.start_time = Convert.ToDateTime(Request.Params["start_time"]).ToString("yyyy-MM-dd 00:00:00");
                    query.start_time = (int)CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["start_time"]).ToString("yyyy-MM-dd 00:00:00"));
                }
                if (!string.IsNullOrEmpty(Request.Params["end_time"]))//結束時間
                {
                    //query.end_time = Convert.ToDateTime(Request.Params["end_time"]).ToString("yyyy-MM-dd 23:59:59");
                    query.end_time = (int)CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["end_time"]).ToString("yyyy-MM-dd 23:59:59"));
                }
                DataTable dtHZ = new DataTable();
                int totalcount = 0;
                query.IsPage = false;
                string newExcelName = string.Empty;
                dtHZ.Columns.Add("料位編號", typeof(String));
                dtHZ.Columns.Add("商品細項編號", typeof(String));
                dtHZ.Columns.Add("商品名稱", typeof(String));
                dtHZ.Columns.Add("商品規格", typeof(String));
                dtHZ.Columns.Add("商品類型", typeof(String));
                dtHZ.Columns.Add("分類--大類", typeof(String));
                dtHZ.Columns.Add("分類--小類", typeof(String));
                dtHZ.Columns.Add("商品狀態", typeof(String));
                dtHZ.Columns.Add("出貨方式", typeof(String));
                dtHZ.Columns.Add("溫層", typeof(String));
                dtHZ.Columns.Add("商品建立日期", typeof(String));
                dtHZ.Columns.Add("商品上架時間", typeof(String));
                dtHZ.Columns.Add("採購單單號", typeof(String));
                productitemMgr = new ProductItemMgr(mySqlConnectionString);
                List<ProductItemQuery> list = productitemMgr.GetWaitLiaoWeiList(query, out totalcount);
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        DataRow dr = dtHZ.NewRow();
                        dr[0] = list[i].plas_id_string;
                        dr[1] = list[i].item_id;
                        dr[2] = list[i].product_name;
                        dr[3] = list[i].product_spec;
                        dr[4] = list[i].combination_string;
                        dr[5] = list[i].product_fenlei_dalei;
                        dr[6] = list[i].product_fenlei_xiaolei;
                        dr[7] = list[i].product_status_string;
                        dr[8] = list[i].product_mode_string;
                        dr[9] = list[i].product_freight_set_string;
                        dr[10] = list[i].product_createdate_string;
                        dr[11] = list[i].product_start_string;
                        dr[12] = list[i].po_id;
                        dtHZ.Rows.Add(dr);
                    }
                    string fileName = "等待料位報表匯出_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, "");
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
                else
                {
                    Response.Clear();
                    this.Response.Write("無數據存在<br/>");
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);

            }
        }
        #region  待撿貨商品報表 add by yafeng0715j 201510260934

        public ActionResult Aseld()
        {
            return Index();
        }
        public HttpResponseBase AseldList()
        {
            string json = string.Empty;
            int totalcount = 0;
            AseldQuery query = new AseldQuery();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
            IAseldImplMgr aseldMgr = new AseldMgr(mySqlConnectionString);

            if (!string.IsNullOrEmpty(Request.Params["assg_id"]))
            {
                query.assg_id = Request.Params["assg_id"];
            }
            DateTime date = DateTime.MinValue;
            if (!string.IsNullOrEmpty(Request.Params["start_time"]))//開始時間
            {
                if (DateTime.TryParse(Request.Params["start_time"], out date))
                {
                    query.start_dtim = Convert.ToDateTime(date.ToString("yyyy-MM-dd 00:00:00"));
                }
            }
            if (!string.IsNullOrEmpty(Request.Params["end_time"]))//結束時間
            {
                if (DateTime.TryParse(Request.Params["end_time"], out date))
                {
                    query.change_dtim = Convert.ToDateTime(date.ToString("yyyy-MM-dd 23:59:59"));
                }
            }
            DataTable table = aseldMgr.GetAseldTable(query, out totalcount);
            _IiupcMgr = new IupcMgr(mySqlConnectionString);
            IupcQuery queryIupc = new IupcQuery();
            string upc_id = string.Empty;
            List<IupcQuery> list = new List<IupcQuery>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                //string lcat_id = table.Rows[i]["lcat_id"].ToString();
                //if (lcat_id != "S")
                //{
                //    string parameterName = table.Rows[i]["parameterName"].ToString();
                //    if (parameterName == "寄倉")
                //    {
                //        table.Rows[i]["loc_id"] = "YY999999";
                //    }
                //    else if (parameterName == "調度")
                //    {
                //        table.Rows[i]["loc_id"] = "ZZ999999";
                //    }
                //}
                uint item_id = uint.Parse(table.Rows[i]["item_id"].ToString());
                queryIupc.item_id = item_id;
                queryIupc.upc_type_flg = "1";
                list = _IiupcMgr.GetIupcByType(queryIupc);
                int j = 0;
                if (list.Count > 0)
                {
                    table.Rows[i]["upc_id"] = list[0].upc_id;
                    j++;
                }
                else
                {
                    queryIupc.upc_type_flg = "3";
                    list = _IiupcMgr.GetIupcByType(queryIupc);
                    if (list.Count > 0)
                    {
                        table.Rows[i]["upc_id"] = list[0].upc_id;
                        j++;
                    }
                    else
                    {
                        queryIupc.upc_type_flg = "2";
                        list = _IiupcMgr.GetIupcByType(queryIupc);
                        if (list.Count > 0)
                        {
                            table.Rows[i]["upc_id"] = list[0].upc_id;
                            j++;
                        }
                    }
                }
                if(j==0)
                {
                    table.Rows[i]["upc_id"] ="";
                }
            }
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            json = "{success:true,totalCount:" + totalcount + ",data:" + JsonConvert.SerializeObject(table, Formatting.Indented, timeConverter) + "}";
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return Response;
        }
        //public static DataTable AseldPDF(DataTable aseldTable)
        //{
        //    DataTable table = new DataTable();
        //    table.Columns.Add("商品編號", typeof(string));
        //    table.Columns.Add("商品名稱", typeof(string));
        //    table.Columns.Add("細項編號", typeof(string));
        //    table.Columns.Add("規格", typeof(string));
        //    table.Columns.Add("待檢貨量", typeof(string));
        //    table.Columns.Add("已檢貨量", typeof(string));
        //    table.Columns.Add("創建時間", typeof(string));
        //    for (int i = 0; i < aseldTable.Rows.Count; i++)
        //    {
        //        DataRow row = table.NewRow();
        //        row["商品編號"] = aseldTable.Rows[i]["product_id"];
        //        row["商品名稱"] = aseldTable.Rows[i]["product_name"];
        //        row["細項編號"] = aseldTable.Rows[i]["item_id"];
        //        row["規格"] = aseldTable.Rows[i]["spec"];
        //        row["待檢貨量"] = aseldTable.Rows[i]["out_qty"];
        //        row["已檢貨量"] = aseldTable.Rows[i]["act_pick_qty"];
        //        row["創建時間"] = aseldTable.Rows[i]["create_dtim"];
        //        table.Rows.Add(row);
        //    }
        //    return table;
        //}
        //public string MakePDF(DataTable aseldTable, string assg_id, string user_username, string newPDFName, int index)
        //{
        //    int columnNum = 10;
        //    string dateNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //    PdfHelper pdf = new PdfHelper();
        //    float[] arrColWidth = new float[] { 150, 60, 60, 30, 35, 35, 45, 45, 60, 35 };
        //    Document document = new Document(PageSize.A4);
        //    BaseFont bf = BaseFont.CreateFont("C:\\WINDOWS\\Fonts\\simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        //    iTextSharp.text.Font fontChinese = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.UNDERLINE, iTextSharp.text.BaseColor.RED);
        //    iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));//黑  

        //    string newfilename = "";
        //    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(newPDFName + index, FileMode.Create));
        //    document.Open();

        //    PdfPTable ptable = new PdfPTable(columnNum);
        //    ptable.WidthPercentage = 100;//表格寬度
        //    font = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
        //    ptable.SetTotalWidth(arrColWidth);
        //    PdfPCell cell = new PdfPCell();
        //    int pagesize = 1;
        //    if (aseldTable.Rows.Count > 35)
        //    {
        //        if (aseldTable.Rows.Count % 35 == 0)
        //        {
        //            pagesize = aseldTable.Rows.Count / 35;
        //        }
        //        else
        //        {
        //            pagesize = aseldTable.Rows.Count / 35 + 1;
        //        }
        //    }
        //    for (int j = 0; j < pagesize; j++)
        //    {
        //        #region 表頭
        //        cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 12)));
        //        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //        cell.Colspan = columnNum;
        //        cell.DisableBorderSide(1);
        //        cell.DisableBorderSide(2);
        //        cell.DisableBorderSide(4);
        //        cell.DisableBorderSide(8);
        //        ptable.AddCell(cell);

        //        cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 12)));
        //        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //        cell.Colspan = 2;
        //        cell.DisableBorderSide(1);
        //        cell.DisableBorderSide(2);
        //        cell.DisableBorderSide(4);
        //        cell.DisableBorderSide(8);
        //        ptable.AddCell(cell);

        //        cell = new PdfPCell(new Phrase("待撿貨商品報表", new iTextSharp.text.Font(bf, 12)));
        //        cell.VerticalAlignment = Element.ALIGN_LEFT;
        //        cell.Colspan = 4;
        //        cell.DisableBorderSide(1);
        //        cell.DisableBorderSide(2);
        //        cell.DisableBorderSide(4);
        //        cell.DisableBorderSide(8);
        //        ptable.AddCell(cell);

        //        cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 12)));
        //        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //        cell.Colspan = 3;
        //        cell.DisableBorderSide(1);
        //        cell.DisableBorderSide(2);
        //        cell.DisableBorderSide(4);
        //        cell.DisableBorderSide(8);
        //        ptable.AddCell(cell);

        //        cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 12)));
        //        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //        cell.Colspan = columnNum;
        //        cell.DisableBorderSide(1);
        //        cell.DisableBorderSide(2);
        //        cell.DisableBorderSide(4);
        //        cell.DisableBorderSide(8);
        //        ptable.AddCell(cell);

        //        cell = new PdfPCell(new Phrase("印表人：" + user_username, new iTextSharp.text.Font(bf, 8)));
        //        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //        cell.Colspan = 2;
        //        cell.DisableBorderSide(1);
        //        cell.DisableBorderSide(2);
        //        cell.DisableBorderSide(4);
        //        cell.DisableBorderSide(8);
        //        ptable.AddCell(cell);
        //        cell = new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(bf, 8)));
        //        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //        cell.Colspan = 5;
        //        cell.DisableBorderSide(1);
        //        cell.DisableBorderSide(2);
        //        cell.DisableBorderSide(4);
        //        cell.DisableBorderSide(8);
        //        ptable.AddCell(cell);

        //        cell = new PdfPCell(new Phrase("印表時間：" + dateNow, new iTextSharp.text.Font(bf, 8)));
        //        cell.VerticalAlignment = Element.ALIGN_RIGHT;//字體水平居右
        //        cell.Colspan = 3;
        //        cell.DisableBorderSide(1);
        //        cell.DisableBorderSide(2);
        //        cell.DisableBorderSide(4);
        //        cell.DisableBorderSide(8);
        //        ptable.AddCell(cell);

        //        cell = new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(bf, 1)));
        //        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //        cell.Colspan = columnNum;
        //        cell.DisableBorderSide(1);
        //        cell.DisableBorderSide(2);
        //        cell.DisableBorderSide(4);
        //        cell.DisableBorderSide(8);
        //        ptable.AddCell(cell);
        //        #endregion

        //        cell = new PdfPCell(new Phrase("                                       工作代號:" + assg_id, new iTextSharp.text.Font(bf, 10)));
        //        cell.VerticalAlignment = Element.ALIGN_CENTER;
        //        cell.Colspan = columnNum;
        //        cell.DisableBorderSide(2);
        //        ptable.AddCell(cell);

        //        //cell = new PdfPCell(new Phrase("商品編號", new iTextSharp.text.Font(bf, 8)));
        //        //cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //        //cell.DisableBorderSide(8);
        //        //ptable.AddCell(cell);

        //        cell = new PdfPCell(new Phrase("商品名稱", new iTextSharp.text.Font(bf, 8)));
        //        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //        cell.DisableBorderSide(8);
        //        ptable.AddCell(cell);

        //        cell = new PdfPCell(new Phrase("條碼", new iTextSharp.text.Font(bf, 8)));
        //        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //        cell.DisableBorderSide(8);
        //        ptable.AddCell(cell);

        //        cell = new PdfPCell(new Phrase("規格", new iTextSharp.text.Font(bf, 8)));
        //        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //        cell.DisableBorderSide(8);
        //        ptable.AddCell(cell);

        //        cell = new PdfPCell(new Phrase("訂貨量", new iTextSharp.text.Font(bf, 8)));
        //        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //        cell.DisableBorderSide(8);
        //        ptable.AddCell(cell);

        //        cell = new PdfPCell(new Phrase("已檢貨量", new iTextSharp.text.Font(bf, 8)));
        //        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //        cell.DisableBorderSide(8);
        //        ptable.AddCell(cell);

        //        cell = new PdfPCell(new Phrase("待檢貨量", new iTextSharp.text.Font(bf, 8)));
        //        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //        cell.DisableBorderSide(8);
        //        ptable.AddCell(cell);

        //        cell = new PdfPCell(new Phrase("本次檢貨量", new iTextSharp.text.Font(bf, 8)));
        //        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //        cell.DisableBorderSide(8);
        //        ptable.AddCell(cell);

        //        cell = new PdfPCell(new Phrase("料位編號", new iTextSharp.text.Font(bf, 8)));
        //        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //        cell.DisableBorderSide(8);
        //        ptable.AddCell(cell);

        //        cell = new PdfPCell(new Phrase("創建時間", new iTextSharp.text.Font(bf, 8)));
        //        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //        ptable.AddCell(cell);

        //        cell = new PdfPCell(new Phrase("備註", new iTextSharp.text.Font(bf, 8)));
        //        cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //        ptable.AddCell(cell);

        //        if (0 < aseldTable.Rows.Count)
        //        {
        //            int k = 0;
        //            if (aseldTable.Rows.Count > 35)
        //            {
        //                k = (j + 1) * 35;
        //                if (j != 0)
        //                {
        //                    if ((aseldTable.Rows.Count - j * 35) < 35)
        //                    {
        //                        k = aseldTable.Rows.Count;
        //                    }
        //                    else
        //                    {
        //                        k = (j + 1) * 35;
        //                    }
        //                }
        //            }
        //            else if (0 < aseldTable.Rows.Count)
        //            {
        //                k = aseldTable.Rows.Count;
        //            }
        //            _IiupcMgr = new IupcMgr(mySqlConnectionString);
        //            IupcQuery query = new IupcQuery();
        //            for (int i = j * 35; i < k; i++)
        //            {
        //                //cell = new PdfPCell(new Phrase(aseldTable.Rows[i]["product_id"].ToString(), new iTextSharp.text.Font(bf, 8)));
        //                //cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //                //cell.DisableBorderSide(8);
        //                //ptable.AddCell(cell);

        //                cell = new PdfPCell(new Phrase(aseldTable.Rows[i]["product_name"].ToString(), new iTextSharp.text.Font(bf, 8)));
        //                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);

        //                string upc_id = string.Empty;
        //                List<IupcQuery> list = new List<IupcQuery>();
        //                if (!string.IsNullOrEmpty(aseldTable.Rows[i]["item_id"].ToString()))
        //                {
        //                    uint item_id = uint.Parse(aseldTable.Rows[i]["item_id"].ToString());
        //                    query.item_id = item_id;
        //                    query.upc_type_flg = "1";
        //                    list = _IiupcMgr.GetIupcByType(query);
        //                    if (list.Count > 0)
        //                    {
        //                        upc_id = list[0].upc_id;
        //                    }
        //                    else
        //                    {
        //                        query.upc_type_flg = "3";
        //                        list = _IiupcMgr.GetIupcByType(query);
        //                        if (list.Count > 0)
        //                        {
        //                            upc_id = list[0].upc_id;
        //                        }
        //                        else
        //                        {
        //                            query.upc_type_flg = "2";
        //                            list = _IiupcMgr.GetIupcByType(query);
        //                            if (list.Count > 0)
        //                            {
        //                                upc_id = list[0].upc_id;
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    upc_id = " ";
        //                }
        //                cell = new PdfPCell(new Phrase(upc_id, new iTextSharp.text.Font(bf, 8)));
        //                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);

        //                cell = new PdfPCell(new Phrase(aseldTable.Rows[i]["spec"].ToString(), new iTextSharp.text.Font(bf, 8)));
        //                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);

        //                cell = new PdfPCell(new Phrase(aseldTable.Rows[i]["ord_qty"].ToString(), new iTextSharp.text.Font(bf, 8)));
        //                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);

        //                cell = new PdfPCell(new Phrase(aseldTable.Rows[i]["act_pick_qty"].ToString(), new iTextSharp.text.Font(bf, 8)));
        //                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);

        //                cell = new PdfPCell(new Phrase(aseldTable.Rows[i]["out_qty"].ToString(), new iTextSharp.text.Font(bf, 8)));
        //                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);

        //                cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 8)));
        //                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);

        //                string loc_id = aseldTable.Rows[i]["loc_id"].ToString();
        //                string lcat_id = aseldTable.Rows[i]["lcat_id"].ToString();
        //                if (lcat_id != "S")
        //                {
        //                    string parameterName = aseldTable.Rows[i]["parameterName"].ToString();
        //                    if (parameterName == "寄倉")
        //                    {
        //                        loc_id = "YY999999";
        //                    }
        //                    else if (parameterName == "調度")
        //                    {
        //                        loc_id = "ZZ999999";
        //                    }
        //                }
        //                cell = new PdfPCell(new Phrase(loc_id, new iTextSharp.text.Font(bf, 8)));
        //                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //                cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);

        //                string dateStr = string.Empty;
        //                DateTime dateCreate = DateTime.MinValue;
        //                if (DateTime.TryParse(aseldTable.Rows[i]["create_dtim"].ToString(), out dateCreate))
        //                {
        //                    dateStr = dateCreate.ToString("yyyy-MM-dd HH:mm:ss");
        //                }
        //                cell = new PdfPCell(new Phrase(dateStr, new iTextSharp.text.Font(bf, 8)));
        //                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //                //cell.DisableBorderSide(8);
        //                ptable.AddCell(cell);

        //                cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 8)));
        //                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //                ptable.AddCell(cell);
        //            }
        //        }
        //        else
        //        {
        //            cell = new PdfPCell(new Phrase("此工作代號無數據", new iTextSharp.text.Font(bf, 12)));
        //            cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
        //            cell.Colspan = columnNum;
        //            cell.DisableBorderSide(1);
        //            cell.DisableBorderSide(2);
        //            cell.DisableBorderSide(4);
        //            cell.DisableBorderSide(8);
        //            ptable.AddCell(cell);
        //        }
        //    }
        //    newfilename = newPDFName + "_part" + index + "." + "pdf";
        //    // pdf.ExportDataTableToPDF(aseldTable, false, newfilename, arrColWidth, ptable, comTable, "", "", 7, uint.Parse(table.Rows.Count.ToString()));/*第一7是列，第二個是行*/
        //    pdf.ExportDataTableToPDF(newfilename, ptable, "", "");

        //    return newfilename;
        //}
        //public void AseldPDF()
        //{
        //    string user_username = (Session["caller"] as Caller).user_username;
        //    DataTable aseldTable = new DataTable();
        //    DataTable assg_idTable = new DataTable();
        //    AseldQuery ase_query = new AseldQuery();
        //    ase_query.IsPage = false;
        //    ase_query.assg_id = string.Empty;
        //    ase_query.start_dtim = DateTime.MinValue;
        //    ase_query.change_dtim = DateTime.MinValue;
        //    int total = 0;

        //    PdfHelper pdf = new PdfHelper();
        //    List<string> pdfList = new List<string>();
        //    string newfilename = string.Empty;
        //    string filename = "待撿貨商品報表" + DateTime.Now.ToString("yyyyMMddHHmmss");
        //    string newPDFName = Server.MapPath(excelPath) + filename;
        //    int index = 0;
        //    int serchWhr = 0;

        //    if (!string.IsNullOrEmpty(Request.Params["assg_id"]))
        //    {
        //        ase_query.assg_id = Request.Params["assg_id"];
        //        serchWhr++;
        //    }
        //    DateTime date = DateTime.MinValue;
        //    if (Request.Params["start_time"] != "null" && Request.Params["end_time"] != "null")
        //    {
        //        if (DateTime.TryParse(Request.Params["start_time"], out date))
        //        {
        //            ase_query.start_dtim = Convert.ToDateTime(date.ToString("yyyy-MM-dd 00:00:00"));
        //        }
        //        if (DateTime.TryParse(Request.Params["end_time"], out date))
        //        {
        //            ase_query.change_dtim = Convert.ToDateTime(date.ToString("yyyy-MM-dd 23:59:59"));
        //        }
        //        serchWhr++;
        //    }
        //    IAseldImplMgr aseldMgr = new AseldMgr(mySqlConnectionString);

        //    DataTable _dtBody = new DataTable();
        //    _dtBody.Columns.Add("商品名稱", typeof(string));
        //    _dtBody.Columns.Add("條碼", typeof(string));
        //    _dtBody.Columns.Add("規格", typeof(string));
        //    _dtBody.Columns.Add("訂貨量", typeof(string));
        //    _dtBody.Columns.Add("已檢貨量", typeof(string));
        //    _dtBody.Columns.Add("待檢貨量", typeof(string));
        //    _dtBody.Columns.Add("本次檢貨量", typeof(string));
        //    _dtBody.Columns.Add("檢貨料位編號", typeof(string));
        //    _dtBody.Columns.Add("檢貨庫存", typeof(string));
        //    _dtBody.Columns.Add("製造日期", typeof(string));
        //    _dtBody.Columns.Add("有效日期", typeof(string));
        //    _dtBody.Columns.Add("創建時間", typeof(string));
        //    _dtBody.Columns.Add("備註", typeof(string));

        //    if (ase_query.assg_id != string.Empty)
        //    {
        //        aseldTable = aseldMgr.GetAseldTable(ase_query, out total);
        //        #region 新增功能


        //        if (aseldTable.Rows.Count > 0)
        //        {
        //            _iinvd = new IinvdMgr(mySqlConnectionString);//  GetSearchIinvd
        //            _IiupcMgr = new IupcMgr(mySqlConnectionString);
        //            foreach (DataRow rows in aseldTable.Rows)
        //            {
        //                IinvdQuery IinvdQuery = new IinvdQuery();
        //                IinvdQuery.item_id = uint.Parse(rows["item_id"].ToString());
        //                IinvdQuery.ista_id = "A";
        //                List<IinvdQuery> Store = new List<IinvdQuery>();
        //                Store = _iinvd.GetPlasIinvd(IinvdQuery);
        //                int P_num = string.IsNullOrEmpty(rows["out_qty"].ToString()) ? 0 : int.Parse(rows["out_qty"].ToString()); /*要檢貨的數量*/
        //                string upc_id = string.Empty;
        //                #region 取條碼

        //                List<IupcQuery> list = new List<IupcQuery>();
        //                IupcQuery iupc_query = new IupcQuery();
        //                if (!string.IsNullOrEmpty(rows["item_id"].ToString()))
        //                {
        //                    uint item_id = uint.Parse(rows["item_id"].ToString());
        //                    iupc_query.item_id = item_id;
        //                    iupc_query.upc_type_flg = "1";
        //                    list = _IiupcMgr.GetIupcByType(iupc_query);
        //                    if (list.Count > 0)
        //                    {
        //                        upc_id = list[0].upc_id;
        //                    }
        //                    else
        //                    {
        //                        iupc_query.upc_type_flg = "3";
        //                        list = _IiupcMgr.GetIupcByType(iupc_query);
        //                        if (list.Count > 0)
        //                        {
        //                            upc_id = list[0].upc_id;
        //                        }
        //                        else
        //                        {
        //                            iupc_query.upc_type_flg = "2";
        //                            list = _IiupcMgr.GetIupcByType(iupc_query);
        //                            if (list.Count > 0)
        //                            {
        //                                upc_id = list[0].upc_id;
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    upc_id = " ";
        //                }
        //                #endregion

        //                if (Store.Count > 0)
        //                {
        //                    for (int i = 0; i < Store.Count; i++)
        //                    {
        //                        DataRow row = _dtBody.NewRow();
        //                        if (Store[i].prod_qty > P_num)
        //                        {
        //                            row["商品名稱"] = rows["product_name"];
        //                            row["條碼"] = upc_id;
        //                            row["規格"] = rows["spec"];
        //                            row["訂貨量"] = rows["ord_qty"];
        //                            row["已檢貨量"] = rows["act_pick_qty"];
        //                            row["待檢貨量"] = rows["out_qty"];
        //                            row["本次檢貨量"] = " ";
        //                            row["檢貨料位編號"] = Store[i].plas_loc_id;
        //                            row["檢貨庫存"] = P_num;
        //                            row["製造日期"] = Store[i].made_date;
        //                            row["有效日期"] = Store[i].cde_dt;
        //                            row["創建時間"] = rows["create_dtim"];
        //                            row["備註"] = " ";
        //                            _dtBody.Rows.Add(row);
        //                            break;
        //                        }
        //                        else
        //                        {
        //                            row["商品名稱"] = rows["product_name"];
        //                            row["條碼"] = upc_id;
        //                            row["規格"] = rows["spec"];
        //                            row["訂貨量"] = rows["ord_qty"];
        //                            row["已檢貨量"] = rows["act_pick_qty"];
        //                            row["待檢貨量"] = rows["out_qty"];
        //                            row["本次檢貨量"] = " ";
        //                            row["檢貨料位編號"] = Store[i].plas_loc_id;
        //                            row["檢貨庫存"] = Store[i].prod_qty;
        //                            row["製造日期"] = Store[i].made_date;
        //                            row["有效日期"] = Store[i].cde_dt;
        //                            row["創建時間"] = rows["create_dtim"];
        //                            row["備註"] = " ";
        //                            _dtBody.Rows.Add(row);
        //                            P_num -= Store[i].prod_qty;
        //                        }

        //                    }
        //                    // _dtBody.Rows.Add(row);
        //                }
        //                else
        //                {
        //                    DataRow row = _dtBody.NewRow();
        //                    row["商品名稱"] = rows["product_name"];
        //                    row["條碼"] = upc_id;
        //                    row["規格"] = rows["spec"];
        //                    row["訂貨量"] = rows["ord_qty"];
        //                    row["已檢貨量"] = rows["act_pick_qty"];
        //                    row["待檢貨量"] = rows["out_qty"];
        //                    row["本次檢貨量"] = " ";
        //                    row["檢貨料位編號"] = " ";
        //                    row["檢貨庫存"] = 0;
        //                    row["製造日期"] = " ";
        //                    row["有效日期"] = " ";
        //                    row["創建時間"] = rows["create_dtim"];
        //                    row["備註"] = " ";
        //                    _dtBody.Rows.Add(row);
        //                }


        //            }
        //        }
        //        #endregion

        //        pdfList.Add(MakePDF(aseldTable, ase_query.assg_id, user_username, newPDFName, index++));
        //    }
        //    else if (ase_query.start_dtim != DateTime.MinValue && ase_query.change_dtim != DateTime.MinValue || serchWhr == 0)
        //    {
        //        assg_idTable = aseldMgr.GetAseldTablePDF(ase_query);
        //        for (int a = 0; a < assg_idTable.Rows.Count; a++)
        //        {
        //            ase_query.assg_id = assg_idTable.Rows[a]["assg_id"].ToString();
        //            aseldTable = aseldMgr.GetAseldTable(ase_query, out total);
        //            #region 新增功能


        //            if (aseldTable.Rows.Count > 0)
        //            {
        //                _iinvd = new IinvdMgr(mySqlConnectionString);//  GetSearchIinvd
        //                _IiupcMgr = new IupcMgr(mySqlConnectionString);
        //                foreach (DataRow rows in aseldTable.Rows)
        //                {
        //                    IinvdQuery IinvdQuery = new IinvdQuery();
        //                    IinvdQuery.item_id = uint.Parse(rows["item_id"].ToString());
        //                    IinvdQuery.ista_id = "A";
        //                    List<IinvdQuery> Store = new List<IinvdQuery>();
        //                    Store = _iinvd.GetPlasIinvd(IinvdQuery);
        //                    int P_num = string.IsNullOrEmpty(rows["out_qty"].ToString()) ? 0 : int.Parse(rows["out_qty"].ToString()); /*要檢貨的數量*/
        //                    string upc_id = string.Empty;
        //                    #region 取條碼

        //                    List<IupcQuery> list = new List<IupcQuery>();
        //                    IupcQuery iupc_query = new IupcQuery();
        //                    if (!string.IsNullOrEmpty(rows["item_id"].ToString()))
        //                    {
        //                        uint item_id = uint.Parse(rows["item_id"].ToString());
        //                        iupc_query.item_id = item_id;
        //                        iupc_query.upc_type_flg = "1";
        //                        list = _IiupcMgr.GetIupcByType(iupc_query);
        //                        if (list.Count > 0)
        //                        {
        //                            upc_id = list[0].upc_id;
        //                        }
        //                        else
        //                        {
        //                            iupc_query.upc_type_flg = "3";
        //                            list = _IiupcMgr.GetIupcByType(iupc_query);
        //                            if (list.Count > 0)
        //                            {
        //                                upc_id = list[0].upc_id;
        //                            }
        //                            else
        //                            {
        //                                iupc_query.upc_type_flg = "2";
        //                                list = _IiupcMgr.GetIupcByType(iupc_query);
        //                                if (list.Count > 0)
        //                                {
        //                                    upc_id = list[0].upc_id;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        upc_id = " ";
        //                    }
        //                    #endregion

        //                    if (Store.Count > 0)
        //                    {
        //                        for (int i = 0; i < Store.Count; i++)
        //                        {
        //                            DataRow row = _dtBody.NewRow();
        //                            if (Store[i].prod_qty > P_num)
        //                            {
        //                                row["商品名稱"] = rows["product_name"];
        //                                row["條碼"] = upc_id;
        //                                row["規格"] = rows["spec"];
        //                                row["訂貨量"] = rows["ord_qty"];
        //                                row["已檢貨量"] = rows["act_pick_qty"];
        //                                row["待檢貨量"] = rows["out_qty"];
        //                                row["本次檢貨量"] = " ";
        //                                row["檢貨料位編號"] = Store[i].plas_loc_id;
        //                                row["檢貨庫存"] = P_num;
        //                                row["製造日期"] = Store[i].made_date;
        //                                row["有效日期"] = Store[i].cde_dt;
        //                                row["創建時間"] = rows["create_dtim"];
        //                                row["備註"] = " ";
        //                                _dtBody.Rows.Add(row);
        //                                break;
        //                            }
        //                            else
        //                            {
        //                                row["商品名稱"] = rows["product_name"];
        //                                row["條碼"] = upc_id;
        //                                row["規格"] = rows["spec"];
        //                                row["訂貨量"] = rows["ord_qty"];
        //                                row["已檢貨量"] = rows["act_pick_qty"];
        //                                row["待檢貨量"] = rows["out_qty"];
        //                                row["本次檢貨量"] = " ";
        //                                row["檢貨料位編號"] = Store[i].plas_loc_id;
        //                                row["檢貨庫存"] = Store[i].prod_qty;
        //                                row["製造日期"] = Store[i].made_date;
        //                                row["有效日期"] = Store[i].cde_dt;
        //                                row["創建時間"] = rows["create_dtim"];
        //                                row["備註"] = " ";
        //                                _dtBody.Rows.Add(row);
        //                                P_num -= Store[i].prod_qty;
        //                            }

        //                        }
        //                        // _dtBody.Rows.Add(row);
        //                    }
        //                    else
        //                    {
        //                        DataRow row = _dtBody.NewRow();
        //                        row["商品名稱"] = rows["product_name"];
        //                        row["條碼"] = upc_id;
        //                        row["規格"] = rows["spec"];
        //                        row["訂貨量"] = rows["ord_qty"];
        //                        row["已檢貨量"] = rows["act_pick_qty"];
        //                        row["待檢貨量"] = rows["out_qty"];
        //                        row["本次檢貨量"] = " ";
        //                        row["檢貨料位編號"] = " ";
        //                        row["檢貨庫存"] = 0;
        //                        row["製造日期"] = " ";
        //                        row["有效日期"] = " ";
        //                        row["創建時間"] = rows["create_dtim"];
        //                        row["備註"] = " ";
        //                        _dtBody.Rows.Add(row);
        //                    }


        //                }
        //            }
        //            #endregion

        //            pdfList.Add(MakePDF(aseldTable, ase_query.assg_id, user_username, newPDFName, index++));
        //        }
        //    }



        //    newfilename = newPDFName + "." + "pdf";
        //    pdf.MergePDF(pdfList, newfilename);

        //    Response.Clear();
        //    Response.Charset = "gb2312";
        //    Response.ContentEncoding = System.Text.Encoding.UTF8;
        //    Response.AddHeader("Content-Disposition", "attach-ment;filename=" + filename + ".pdf");
        //    Response.WriteFile(newfilename);

        //}

        public void AseldPDFS()
        {
            PdfHelper pdf = new PdfHelper();
            List<string> pdfList = new List<string>();
            float[] arrColWidth = new float[] { 150, 60, 35,  45, 45,50, 50, 50, 45, 55, 35 };
            int index = 0;
            string newFileName = string.Empty;
            string newName = string.Empty;
            string json = string.Empty;
            BaseFont bf = BaseFont.CreateFont("C:\\WINDOWS\\Fonts\\simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font fontChinese = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.UNDERLINE, iTextSharp.text.BaseColor.RED);
            iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
            string filename = "待撿貨商品報表" + DateTime.Now.ToString("yyyyMMddHHmmss");
            Document document = new Document(PageSize.A4.Rotate());
            string newPDFName = Server.MapPath(excelPath) + filename;
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(newPDFName, FileMode.Create));
            document.Open();


            string user_username = (Session["caller"] as Caller).user_username;
            DataTable aseldTable = new DataTable();
            DataTable assg_idTable = new DataTable();
            AseldQuery ase_query = new AseldQuery();
            ase_query.IsPage = false;
            ase_query.assg_id = string.Empty;
            ase_query.start_dtim = DateTime.MinValue;
            ase_query.change_dtim = DateTime.MinValue;
            int total = 0;

            //PdfHelper pdf = new PdfHelper();
            //List<string> pdfList = new List<string>();
            //string newfilename = string.Empty;
            //string filename = "待撿貨商品報表" + DateTime.Now.ToString("yyyyMMddHHmmss");
            //string newPDFName = Server.MapPath(excelPath) + filename;
            //int index = 0;
            int serchWhr = 0;

            if (!string.IsNullOrEmpty(Request.Params["assg_id"]))
            {
                ase_query.assg_id = Request.Params["assg_id"];
                serchWhr++;
            }
            DateTime date = DateTime.MinValue;
            if (Request.Params["start_time"] != "null" && Request.Params["end_time"] != "null")
            {
                if (DateTime.TryParse(Request.Params["start_time"], out date))
                {
                    ase_query.start_dtim = Convert.ToDateTime(date.ToString("yyyy-MM-dd 00:00:00"));
                }
                if (DateTime.TryParse(Request.Params["end_time"], out date))
                {
                    ase_query.change_dtim = Convert.ToDateTime(date.ToString("yyyy-MM-dd 23:59:59"));
                }
                serchWhr++;
            }
            IAseldImplMgr aseldMgr = new AseldMgr(mySqlConnectionString);

            DataTable _dtBody = new DataTable();
            _dtBody.Columns.Add("商品名稱", typeof(string));
            _dtBody.Columns.Add("條碼", typeof(string));
           // _dtBody.Columns.Add("規格", typeof(string));
            _dtBody.Columns.Add("訂貨量", typeof(string));
            _dtBody.Columns.Add("已檢貨量", typeof(string));
            _dtBody.Columns.Add("待檢貨量", typeof(string));
            _dtBody.Columns.Add("料位編號", typeof(string));
            _dtBody.Columns.Add("製造日期", typeof(string)); 
            _dtBody.Columns.Add("有效日期", typeof(string));
            
           // _dtBody.Columns.Add("檢貨料位編號", typeof(string));
            _dtBody.Columns.Add("檢貨庫存", typeof(string)); 
            _dtBody.Columns.Add("本次檢貨量", typeof(string));
           
           // _dtBody.Columns.Add("創建時間", typeof(string));
            _dtBody.Columns.Add("備註", typeof(string));
            PdfPTable ptablefoot = new PdfPTable(14);
            #region MyRegion



            #region 數據行
            if (ase_query.assg_id != string.Empty)
            {
                _dtBody.Rows.Clear();
                aseldTable = aseldMgr.GetAseldTable(ase_query, out total);
                #region 標頭
                #region 表頭
                PdfPTable ptable = new PdfPTable(11);


                ptable.WidthPercentage = 100;//表格寬度
                ptable.SetTotalWidth(arrColWidth);
                PdfPCell cell = new PdfPCell();
                cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.Colspan = 11;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.Colspan = 3;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("待撿貨商品報表", new iTextSharp.text.Font(bf, 18)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 5;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.Colspan = 3;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.Colspan = 11;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("印表人：" + user_username, new iTextSharp.text.Font(bf, 8)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.Colspan = 2;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(bf, 8)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.Colspan = 6;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("印表時間：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), new iTextSharp.text.Font(bf, 8)));
                cell.VerticalAlignment = Element.ALIGN_RIGHT;//字體水平居右
                cell.Colspan = 3;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(bf, 8)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.Colspan = 11;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                #endregion
                cell = new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(bf, 8)));
                cell.VerticalAlignment = Element.ALIGN_RIGHT;//字體水平居右
                cell.Colspan = 3;
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("工作代號:" + ase_query.assg_id, new iTextSharp.text.Font(bf, 15)));
                cell.VerticalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 5;
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(bf, 8)));
                cell.VerticalAlignment = Element.ALIGN_RIGHT;//字體水平居右
                cell.Colspan = 3;
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("商品名稱", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("條碼", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                //cell = new PdfPCell(new Phrase("規格", new iTextSharp.text.Font(bf, 8)));
                //cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                //cell.DisableBorderSide(8);
                //ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("訂貨量", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("已檢貨量", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("待檢貨量", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("料位編碼", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("製造日期", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                //cell = new PdfPCell(new Phrase("檢貨料位編號", new iTextSharp.text.Font(bf, 8)));
                //cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                //cell.DisableBorderSide(8);
                //ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("有效日期", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("檢貨庫存", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("本次檢貨量", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                //cell = new PdfPCell(new Phrase("創建時間", new iTextSharp.text.Font(bf, 8)));
                //cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                //ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("備註", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                ptable.AddCell(cell);
                #endregion
                #region 新增功能


                if (aseldTable.Rows.Count > 0)
                {
                    _iinvd = new IinvdMgr(mySqlConnectionString);//  GetSearchIinvd
                    _IiupcMgr = new IupcMgr(mySqlConnectionString);
                    foreach (DataRow rows in aseldTable.Rows)
                    {
                        IinvdQuery IinvdQuery = new IinvdQuery();
                        IinvdQuery.item_id = uint.Parse(rows["item_id"].ToString());
                        IinvdQuery.ista_id = "A";
                        List<IinvdQuery> Store = new List<IinvdQuery>();
                        Store = _iinvd.GetPlasIinvd(IinvdQuery);
                        int P_num = string.IsNullOrEmpty(rows["out_qty"].ToString()) ? 0 : int.Parse(rows["out_qty"].ToString()); /*要檢貨的數量*/
                        string upc_id = string.Empty;
                        #region 取條碼

                        List<IupcQuery> list = new List<IupcQuery>();
                        IupcQuery iupc_query = new IupcQuery();
                        if (!string.IsNullOrEmpty(rows["item_id"].ToString()))
                        {
                            uint item_id = uint.Parse(rows["item_id"].ToString());
                            iupc_query.item_id = item_id;
                            iupc_query.upc_type_flg = "1";
                            list = _IiupcMgr.GetIupcByType(iupc_query);
                            if (list.Count > 0)
                            {
                                upc_id = list[0].upc_id;
                            }
                            else
                            {
                                iupc_query.upc_type_flg = "3";
                                list = _IiupcMgr.GetIupcByType(iupc_query);
                                if (list.Count > 0)
                                {
                                    upc_id = list[0].upc_id;
                                }
                                else
                                {
                                    iupc_query.upc_type_flg = "2";
                                    list = _IiupcMgr.GetIupcByType(iupc_query);
                                    if (list.Count > 0)
                                    {
                                        upc_id = list[0].upc_id;
                                    }
                                }
                            }
                        }
                        else
                        {
                            upc_id = " ";
                        }
                        #endregion

                        if (Store.Count > 0)
                        {
                            for (int i = 0; i < Store.Count; i++)
                            {
                                DataRow row = _dtBody.NewRow();
                                if (Store[i].prod_qty > P_num)
                                {
                                    row["商品名稱"] = rows["product_name"] + rows["spec"].ToString();
                                    row["條碼"] = upc_id;
                                   // row["規格"] = rows["spec"];
                                    row["訂貨量"] = rows["ord_qty"];
                                    row["已檢貨量"] = rows["act_pick_qty"];
                                    row["待檢貨量"] = rows["out_qty"];
                                    row["料位編號"] = rows["loc_id"]; 
                                    row["製造日期"] = string.IsNullOrEmpty(Store[i].made_date.ToString()) ? " " : Store[i].made_date.ToString("yyyy/MM/dd");
                                    row["有效日期"] = string.IsNullOrEmpty(Store[i].cde_dt.ToString()) ? " " : Store[i].cde_dt.ToString("yyyy/MM/dd");
                                    row["檢貨庫存"] = P_num;
                                    row["本次檢貨量"] = " ";
                                    row["備註"] = " ";
                                   // row["檢貨料位編號"] = Store[i].plas_loc_id;
                                   
                                   
                                  //  row["創建時間"] = rows["create_dtim"];
                                  
                                    _dtBody.Rows.Add(row);
                                    break;
                                }
                                else
                                {
                                    row["商品名稱"] = rows["product_name"] + rows["spec"].ToString();
                                    row["條碼"] = upc_id;
                                    //row["規格"] = rows["spec"];
                                    row["訂貨量"] = rows["ord_qty"];
                                    row["已檢貨量"] = rows["act_pick_qty"];
                                    row["待檢貨量"] = rows["out_qty"];
                                    row["料位編號"] = rows["loc_id"];
                                    row["製造日期"] = string.IsNullOrEmpty(Store[i].made_date.ToString()) ? " " : Store[i].made_date.ToString("yyyy/MM/dd");
                                    row["有效日期"] = string.IsNullOrEmpty(Store[i].cde_dt.ToString()) ? " " : Store[i].cde_dt.ToString("yyyy/MM/dd");
                                    row["檢貨庫存"] = Store[i].prod_qty;
                                    row["本次檢貨量"] = " ";
                                   
                                    //row["檢貨料位編號"] = Store[i].plas_loc_id;
                                  //  row["創建時間"] = rows["create_dtim"];
                                    row["備註"] = " ";
                                    _dtBody.Rows.Add(row);
                                    P_num -= Store[i].prod_qty;
                                    if (P_num == 0)
                                        break;
                                }

                            }
                            // _dtBody.Rows.Add(row);
                        }
                        else
                        {
                            DataRow row = _dtBody.NewRow();
                            row["商品名稱"] = rows["product_name"] + rows["spec"].ToString();
                            row["條碼"] = upc_id;
                           // row["規格"] = rows["spec"];
                            row["訂貨量"] = rows["ord_qty"];
                            row["已檢貨量"] = rows["act_pick_qty"];
                            row["待檢貨量"] = rows["out_qty"];
                            row["本次檢貨量"] = " ";
                            row["料位編號"] = rows["loc_id"];
                            //row["檢貨料位編號"] = " ";
                            row["檢貨庫存"] = 0;
                            row["製造日期"] = " ";
                            row["有效日期"] = " ";
                         //   row["創建時間"] = rows["create_dtim"];
                            row["備註"] = " ";
                            _dtBody.Rows.Add(row);
                        }


                    }
                }
                #endregion

                //  pdfList.Add(MakePDF(aseldTable, ase_query.assg_id, user_username, newPDFName, index++));
                newFileName = newPDFName + "_part" + index++ + "." + "pdf";
                pdf.ExportDataTableToPDF(_dtBody, false, newFileName, arrColWidth, ptable, ptablefoot, "", "", 11, uint.Parse(_dtBody.Rows.Count.ToString()));/*第一7是列，第二個是行*/
                pdfList.Add(newFileName);
            }
            else if (ase_query.start_dtim != DateTime.MinValue && ase_query.change_dtim != DateTime.MinValue || serchWhr == 0)
            {
                assg_idTable = aseldMgr.GetAseldTablePDF(ase_query);
                for (int a = 0; a < assg_idTable.Rows.Count; a++)
                {
                    ase_query.assg_id = assg_idTable.Rows[a]["assg_id"].ToString();
                    aseldTable = aseldMgr.GetAseldTable(ase_query, out total);
                    _dtBody.Rows.Clear();
                    #region 標頭
                    #region 表頭
                    PdfPTable ptable = new PdfPTable(11);


                    ptable.WidthPercentage = 100;//表格寬度
                    ptable.SetTotalWidth(arrColWidth);
                    PdfPCell cell = new PdfPCell();
                    cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 12)));
                    cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                    cell.Colspan = 11;
                    cell.DisableBorderSide(1);
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(4);
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);

                    cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 12)));
                    cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                    cell.Colspan = 3;
                    cell.DisableBorderSide(1);
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(4);
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);

                    cell = new PdfPCell(new Phrase("待撿貨商品報表", new iTextSharp.text.Font(bf, 18)));
                    cell.VerticalAlignment = Element.ALIGN_LEFT;
                    cell.Colspan = 5;
                    cell.DisableBorderSide(1);
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(4);
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);

                    cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 12)));
                    cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                    cell.Colspan = 3;
                    cell.DisableBorderSide(1);
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(4);
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);

                    cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 12)));
                    cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                    cell.Colspan = 11;
                    cell.DisableBorderSide(1);
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(4);
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);

                    cell = new PdfPCell(new Phrase("印表人：" + user_username, new iTextSharp.text.Font(bf, 8)));
                    cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                    cell.Colspan = 2;
                    cell.DisableBorderSide(1);
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(4);
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);
                    cell = new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(bf, 8)));
                    cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                    cell.Colspan = 6;
                    cell.DisableBorderSide(1);
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(4);
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);

                    cell = new PdfPCell(new Phrase("印表時間：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), new iTextSharp.text.Font(bf, 8)));
                    cell.VerticalAlignment = Element.ALIGN_RIGHT;//字體水平居右
                    cell.Colspan = 3;
                    cell.DisableBorderSide(1);
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(4);
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(bf, 8)));
                    cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                    cell.Colspan = 11;
                    cell.DisableBorderSide(1);
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(4);
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);
                    #endregion
                    cell = new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(bf, 8)));
                    cell.VerticalAlignment = Element.ALIGN_RIGHT;//字體水平居右
                    cell.Colspan = 3;
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);
                    cell = new PdfPCell(new Phrase("工作代號:" + ase_query.assg_id, new iTextSharp.text.Font(bf, 15)));
                    cell.VerticalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 5;
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(4);
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);
                    cell = new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(bf, 8)));
                    cell.VerticalAlignment = Element.ALIGN_RIGHT;//字體水平居右
                    cell.Colspan = 3;
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(4);
                    ptable.AddCell(cell);
                    cell = new PdfPCell(new Phrase("商品名稱", new iTextSharp.text.Font(bf, 12)));
                    cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);

                    cell = new PdfPCell(new Phrase("條碼", new iTextSharp.text.Font(bf, 12)));
                    cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);

                    //cell = new PdfPCell(new Phrase("規格", new iTextSharp.text.Font(bf, 8)));
                    //cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                    //cell.DisableBorderSide(8);
                    //ptable.AddCell(cell);

                    cell = new PdfPCell(new Phrase("訂貨量", new iTextSharp.text.Font(bf, 12)));
                    cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);

                    cell = new PdfPCell(new Phrase("已檢貨量", new iTextSharp.text.Font(bf, 12)));
                    cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);

                    cell = new PdfPCell(new Phrase("待檢貨量", new iTextSharp.text.Font(bf, 12)));
                    cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);
                    cell = new PdfPCell(new Phrase("料位編碼", new iTextSharp.text.Font(bf, 12)));
                    cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);
                    cell = new PdfPCell(new Phrase("製造日期", new iTextSharp.text.Font(bf, 12)));
                    cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);
                    //cell = new PdfPCell(new Phrase("檢貨料位編號", new iTextSharp.text.Font(bf, 8)));
                    //cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                    //cell.DisableBorderSide(8);
                    //ptable.AddCell(cell);
                    cell = new PdfPCell(new Phrase("有效日期", new iTextSharp.text.Font(bf, 12)));
                    cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);
                    cell = new PdfPCell(new Phrase("檢貨庫存", new iTextSharp.text.Font(bf, 12)));
                    cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);

                    cell = new PdfPCell(new Phrase("本次檢貨量", new iTextSharp.text.Font(bf, 12)));
                    cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);

                    //cell = new PdfPCell(new Phrase("創建時間", new iTextSharp.text.Font(bf, 8)));
                    //cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                    //ptable.AddCell(cell);

                    cell = new PdfPCell(new Phrase("備註", new iTextSharp.text.Font(bf, 12)));
                    cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                    ptable.AddCell(cell);
                    #endregion


                    #region 新增功能


                    if (aseldTable.Rows.Count > 0)
                    {
                        _iinvd = new IinvdMgr(mySqlConnectionString);//  GetSearchIinvd
                        _IiupcMgr = new IupcMgr(mySqlConnectionString);
                        foreach (DataRow rows in aseldTable.Rows)
                        {
                            IinvdQuery IinvdQuery = new IinvdQuery();
                            IinvdQuery.item_id = uint.Parse(rows["item_id"].ToString());
                            IinvdQuery.ista_id = "A";
                            List<IinvdQuery> Store = new List<IinvdQuery>();
                            Store = _iinvd.GetPlasIinvd(IinvdQuery);
                            int P_num = string.IsNullOrEmpty(rows["out_qty"].ToString()) ? 0 : int.Parse(rows["out_qty"].ToString()); /*要檢貨的數量*/
                            string upc_id = string.Empty;
                            #region 取條碼

                            List<IupcQuery> list = new List<IupcQuery>();
                            IupcQuery iupc_query = new IupcQuery();
                            if (!string.IsNullOrEmpty(rows["item_id"].ToString()))
                            {
                                uint item_id = uint.Parse(rows["item_id"].ToString());
                                iupc_query.item_id = item_id;
                                iupc_query.upc_type_flg = "1";
                                list = _IiupcMgr.GetIupcByType(iupc_query);
                                if (list.Count > 0)
                                {
                                    upc_id = list[0].upc_id;
                                }
                                else
                                {
                                    iupc_query.upc_type_flg = "3";
                                    list = _IiupcMgr.GetIupcByType(iupc_query);
                                    if (list.Count > 0)
                                    {
                                        upc_id = list[0].upc_id;
                                    }
                                    else
                                    {
                                        iupc_query.upc_type_flg = "2";
                                        list = _IiupcMgr.GetIupcByType(iupc_query);
                                        if (list.Count > 0)
                                        {
                                            upc_id = list[0].upc_id;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                upc_id = " ";
                            }
                            #endregion

                            if (Store.Count > 0)
                            {
                                for (int i = 0; i < Store.Count; i++)
                                {
                                    DataRow row = _dtBody.NewRow();
                                    if (Store[i].prod_qty > P_num)
                                    {
                                        row["商品名稱"] = rows["product_name"] + rows["spec"].ToString();
                                        row["條碼"] = upc_id;
                                        // row["規格"] = rows["spec"];
                                        row["訂貨量"] = rows["ord_qty"];
                                        row["已檢貨量"] = rows["act_pick_qty"];
                                        row["待檢貨量"] = rows["out_qty"];
                                        row["料位編號"] = rows["loc_id"];
                                        row["製造日期"] = string.IsNullOrEmpty(Store[i].made_date.ToString()) ? " " : Store[i].made_date.ToString("yyyy/MM/dd");
                                        row["有效日期"] = string.IsNullOrEmpty(Store[i].cde_dt.ToString()) ? " " : Store[i].cde_dt.ToString("yyyy/MM/dd");
                                        row["檢貨庫存"] = P_num;
                                        row["本次檢貨量"] = " ";
                                        row["備註"] = " ";
                                        _dtBody.Rows.Add(row);
                                        break;
                                    }
                                    else
                                    {
                                        row["商品名稱"] = rows["product_name"] + rows["spec"].ToString();
                                        row["條碼"] = upc_id;
                                        //row["規格"] = rows["spec"];
                                        row["訂貨量"] = rows["ord_qty"];
                                        row["已檢貨量"] = rows["act_pick_qty"];
                                        row["待檢貨量"] = rows["out_qty"];
                                        row["料位編號"] = rows["loc_id"];
                                        row["製造日期"] = string.IsNullOrEmpty(Store[i].made_date.ToString()) ? " " : Store[i].made_date.ToString("yyyy/MM/dd");
                                        row["有效日期"] = string.IsNullOrEmpty(Store[i].cde_dt.ToString()) ? " " : Store[i].cde_dt.ToString("yyyy/MM/dd");
                                        row["檢貨庫存"] = Store[i].prod_qty;
                                        row["本次檢貨量"] = " ";

                                        //row["檢貨料位編號"] = Store[i].plas_loc_id;
                                        //  row["創建時間"] = rows["create_dtim"];
                                        row["備註"] = " ";
                                        _dtBody.Rows.Add(row);
                                        P_num -= Store[i].prod_qty;
                                        if (P_num == 0)
                                            break;
                                    }

                                }
                                // _dtBody.Rows.Add(row);
                            }
                            else
                            {
                                DataRow row = _dtBody.NewRow();
                                row["商品名稱"] = rows["product_name"] + rows["spec"].ToString();
                                row["條碼"] = upc_id;
                               // row["規格"] = rows["spec"];
                                row["訂貨量"] = rows["ord_qty"];
                                row["已檢貨量"] = rows["act_pick_qty"];
                                row["待檢貨量"] = rows["out_qty"];
                                row["本次檢貨量"] = " ";
                                //row["檢貨料位編號"] = " ";
                                row["檢貨庫存"] = 0;
                                row["製造日期"] = " ";
                                row["有效日期"] = " ";
                              //  row["創建時間"] = rows["create_dtim"];
                                row["備註"] = " ";
                                _dtBody.Rows.Add(row);
                            }


                        }
                    }
                    #endregion

                    //  pdfList.Add(MakePDF(aseldTable, ase_query.assg_id, user_username, newPDFName, index++));
                    newFileName = newPDFName + "_part" + index++ + "." + "pdf";
                    pdf.ExportDataTableToPDF(_dtBody, false, newFileName, arrColWidth, ptable, ptablefoot, "", "", 11, uint.Parse(_dtBody.Rows.Count.ToString()));/*第一7是列，第二個是行*/
                    pdfList.Add(newFileName);
                }
            }
            #endregion

            #endregion
           
          
            if (_dtBody.Rows.Count == 0)
            {
                #region 標頭
                #region 表頭
                PdfPTable ptable = new PdfPTable(11);


                ptable.WidthPercentage = 100;//表格寬度
                ptable.SetTotalWidth(arrColWidth);
                PdfPCell cell = new PdfPCell();
                cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.Colspan = 11;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.Colspan = 3;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("待撿貨商品報表", new iTextSharp.text.Font(bf, 18)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 5;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.Colspan = 3;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.Colspan = 11;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("印表人：" + user_username, new iTextSharp.text.Font(bf, 8)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.Colspan = 2;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(bf, 8)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.Colspan = 6;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("印表時間：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), new iTextSharp.text.Font(bf, 8)));
                cell.VerticalAlignment = Element.ALIGN_RIGHT;//字體水平居右
                cell.Colspan = 3;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(bf, 8)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.Colspan = 11;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                #endregion
                cell = new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(bf, 8)));
                cell.VerticalAlignment = Element.ALIGN_RIGHT;//字體水平居右
                cell.Colspan = 3;
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("工作代號:" + ase_query.assg_id, new iTextSharp.text.Font(bf, 15)));
                cell.VerticalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 5;
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(bf, 8)));
                cell.VerticalAlignment = Element.ALIGN_RIGHT;//字體水平居右
                cell.Colspan = 3;
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("商品名稱", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("條碼", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                //cell = new PdfPCell(new Phrase("規格", new iTextSharp.text.Font(bf, 8)));
                //cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                //cell.DisableBorderSide(8);
                //ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("訂貨量", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("已檢貨量", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("待檢貨量", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("料位編碼", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("製造日期", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                //cell = new PdfPCell(new Phrase("檢貨料位編號", new iTextSharp.text.Font(bf, 8)));
                //cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                //cell.DisableBorderSide(8);
                //ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("有效日期", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);
                cell = new PdfPCell(new Phrase("檢貨庫存", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("本次檢貨量", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                //cell = new PdfPCell(new Phrase("創建時間", new iTextSharp.text.Font(bf, 8)));
                //cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                //ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("備註", new iTextSharp.text.Font(bf, 12)));
                cell.VerticalAlignment = Element.ALIGN_LEFT;//字體水平居左
                ptable.AddCell(cell);
                #endregion
                document = new Document(PageSize.A4.Rotate());
                if (!document.IsOpen())
                {
                    document.Open();
                }
                cell = new PdfPCell(new Phrase(" ", font));
                cell.Colspan = 5;
                cell.VerticalAlignment = Element.ALIGN_CENTER;//字體水平居左
                cell.DisableBorderSide(8);
                ptable.AddCell(cell);

                cell = new PdfPCell(new Phrase("此工作代號無數據!", font));
                cell.Colspan = 9;
                cell.DisableBorderSide(4);
                cell.VerticalAlignment = Element.ALIGN_CENTER;//字體水平居左
                ptable.AddCell(cell);


                // document.Add(ptable);
                //document.Add(ptablefoot); 
                newFileName = newPDFName + "_part" + index++ + "." + "pdf";
                pdf.ExportDataTableToPDF(_dtBody, false, newFileName, arrColWidth, ptable, ptablefoot, "", "", 11, uint.Parse(_dtBody.Rows.Count.ToString()));/*第一7是列，第二個是行*/
                pdfList.Add(newFileName);

            }
            //else
            //{
            //    newFileName = newPDFName + "_part" + index++ + "." + "pdf";

            //    pdf.ExportDataTableToPDF(_dtBody, false, newFileName, arrColWidth, ptable, ptablefoot, "", "", 11, uint.Parse(_dtBody.Rows.Count.ToString()));/*第一7是列，第二個是行*/
            //    pdfList.Add(newFileName);

            //}

            newFileName = newPDFName + "." + "pdf";
            pdf.MergePDF(pdfList, newFileName);

            Response.Clear();
            Response.Charset = "gb2312";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attach-ment;filename=" + filename + ".pdf");
            Response.WriteFile(newFileName);

        }
        #endregion
    }
}
