using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using BLL.gigade.Model.Query;
using Newtonsoft.Json.Converters;
using BLL.gigade.Common;
using Newtonsoft.Json;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System.IO;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Text;
using System.Net;
namespace Admin.gigade.Controllers
{
    public class MemberController : Controller
    {

        static string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//郵件服務器的設置
        string FromName = ConfigurationManager.AppSettings["FromNameGigade"];//發件人姓名
        string EmailTile = ConfigurationManager.AppSettings["EmailTileGigade"];//郵件標題
        static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IUserRecommendIMgr _userrecommendMgr;
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//"http://192.168.71.159:8080"

        string defaultImg = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server) + "/product/nopic_50.jpg";
        private IUserLoginLogImplMgr _userloginlog;
        private IUserEdmImplMgr _edmMgr = null;
        private IUsersListImplMgr _uslmpgr;
        private ISiteConfigImplMgr siteConfigMgr;
        private IUserIOImplMgr userioMgr;
        private IUsersImplMgr usersMgr;
        //private NPOI4ExcelHelper excelHelper;
        private ZipMgr zMgr;
        private IUsersImplMgr _usmpgr;
        private ICallerImplMgr mgr;
        //private IUsersListImplMgr _usltmpgr;
        private ParameterMgr _paraMgr;

        private IVipUserGroupImplMgr _userGroupMgr;
        private ISerialImplMgr _ISerImplMgr;
        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);//ftp用戶名
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);//ftp密碼
        string promoPath = ConfigurationManager.AppSettings["promoPath"];//圖片地址
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//ftp地址
        private string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];//aimg.gigade100.com
        private IConfigImplMgr _configMgr;
        private ShippingVoucherMgr ShippingVoucherMgr;
        private VipUserMgr _vipuserMgr;


        #region 會員免運劵發放功能
        public ActionResult ShippingVoucher()
        {
            return View();
        }

        public HttpResponseBase GetShippingVoucher()
        {
            string json = string.Empty;
            try
            {
                ShippingVoucherQuery query = new ShippingVoucherQuery();
                query.Start = string.IsNullOrEmpty(Request.Params["start"]) ? 0 : int.Parse(Request.Params["start"]);
                query.Limit = string.IsNullOrEmpty(Request.Params["limit"]) ? 25 : int.Parse(Request.Params["limit"]);
                query.sv_state = string.IsNullOrEmpty(Request.Params["state"]) ? 0 : int.Parse(Request.Params["state"]);
                System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^\d+$");
                string oid = Request.Params["oid"].Trim();
                if (oid != "")
                {
                    if (rex.IsMatch(oid))
                    {
                        query.order_id = int.Parse(oid);
                    }
                    else
                    {
                        query.order_id = -1;
                    }
                }
                query.user_id = query.order_id;
                query.user_name = Request.Params["username"].Trim();
                DateTime datetime;
                if (DateTime.TryParse(Request.Params["time_start"], out datetime))
                {
                    query.created_start = DateTime.Parse(datetime.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if (DateTime.TryParse(Request.Params["time_end"], out datetime))
                {
                    query.created_end = DateTime.Parse(datetime.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                int totalCount = 0;
                ShippingVoucherMgr = new ShippingVoucherMgr(mySqlConnectionString);
                List<ShippingVoucherQuery> list = ShippingVoucherMgr.GetList(query, out totalCount);
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

        #region View
        public ActionResult MemberIndex()
        {
            return View();
        }
        public ActionResult UserLoginLog()
        {
            return View();
        }
        /// <summary>
        /// 用戶列表
        /// </summary>
        /// <returns></returns>
        public ActionResult UserList()
        {
            return View();
        }
        public ActionResult UserSyncToEdm()
        {
            return View();
        }
        /// <summary>
        /// 會員推薦
        /// </summary>
        /// <returns></returns>
        public ActionResult RecommendMember()
        {
            return View();
        }
        /// <summary>
        /// 會員資料探勘
        /// </summary>
        /// <returns></returns>
        public ActionResult Export()
        {
            if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
            }
            return View();
        }

        public ActionResult UserPhoneAdd()
        {
            return View();
        }

        /// <summary>
        /// 會員購買記錄排行
        /// </summary>
        /// <returns></returns>
        public ActionResult VipList()
        {
            return View();
        }

        #region 會員查詢列表信息+ActionResult Index()
        public ActionResult UsersListIndex()
        {
            ViewBag.UserEmail = Request.QueryString["UserEmail"] ?? "";//獲取會員email
            ViewBag.UserMobile = Request.QueryString["UserMobile"] ?? "";//獲取會員email
            return View();
        }
        #endregion

        #region 購物金信息+ActionResult BonusSearch()
        public ActionResult BonusSearch()
        {
            ViewBag.user_id = Convert.ToInt32(Request.Params["uid"]);
            ViewBag.bonus_type = Convert.ToInt32(Request.Params["bonus_type"]);
            return View();
        }
        #endregion
        /// <summary>
        /// 群組列表
        /// </summary>
        /// <returns></returns>
        public ActionResult VipUserGroupList()
        {
            string user_email = (Session["caller"] as Caller).user_email;
            int user_id = (Session["caller"] as Caller).user_id;
            _configMgr = new ConfigMgr(mySqlConnectionString);
            List<ConfigQuery> store = _configMgr.Query("member_group", 7);//獲取7個會員群組管理人員
            ViewBag.valetservice = 0;
            ViewBag.modifyonly = 0;
            foreach (var item in store)
            {
                if (item.email == user_email)
                {
                    ViewBag.valetservice = 1;
                }
            }
            if (user_id == 85 || user_id == 96)
            {
                ViewBag.modifyonly = 1;
            }
            return View();
        }
        /// <summary>
        /// vip名單列表
        /// </summary>
        /// <returns></returns>
        public ActionResult VipUserList()
        {
            ViewBag.group_id = Request.QueryString["id"].ToString();
            return View();
        }
        /// <summary>
        /// vip名單匯入
        /// </summary>
        /// <returns></returns>
        public ActionResult VipUserImport()
        {
            ViewBag.group_id = Request.QueryString["id"].ToString();
            ViewBag.group_name = Request.QueryString["name"].ToString();
            if (!string.IsNullOrEmpty(Request.QueryString["check"]))
            {
                ViewBag.check_iden = Request.QueryString["check"];
            }
            ViewBag.care = "1.匯入CSV檔案，必需為逗點分隔欄位。<br/>2.CSV檔案，一列一筆資料，若資料有異常，系統主動略過不處理。<br/>3. CSV檔案，包含一個欄位，如下：<br/>欄位一：電子信箱位址。<br/>其它欄位：皆略過不處理。<br/>4.若此郵件群組中已有相同信箱時，系統不重覆匯入。<br/>5. 匯入CSV檔案大小預設 2MB，若超出系統限制時，請自行分割檔案後，再重新匯入。<br/>6.匯入處理速度，每秒約 100 筆資料，若名單過大，請自行切割檔案後分批匯入，以減少錯誤及影響系統效能。";
            return View();
        }

        #endregion

        #region 推薦會員列表頁 HttpResponseBase Recommendlist()
        [CustomHandleError]
        public HttpResponseBase Recommendlist()
        {
            List<UserRecommendQuery> store = new List<UserRecommendQuery>();
            string json = string.Empty;
            try
            {
                UserRecommendQuery query = new UserRecommendQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");

                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ddlSel"]))
                {
                    query.expired = Convert.ToInt32(Request.Params["ddlSel"]);
                }
                if (Request.Params["ddlSel"] != "")
                {
                    query.ddlstore = Int32.Parse(Request.Params["ddlSel"]);
                }
                if (Request.Params["ddlCon"] != null && Request.Params["ddlCon"] != "")
                {
                    query.con = Request.Params["ddlCon"];
                }
                if (Request.Params["start_date"] != null && Request.Params["start_date"] != "")
                {
                    query.startdate = Convert.ToDateTime(Request.Params["start_date"]);
                }
                if (Request.Params["end_date"] != null && Request.Params["end_date"] != "")
                {
                    query.enddate = Convert.ToDateTime(Request.Params["end_date"]);
                }
                if (Request.Params["recommend"] != null && Request.Params["recommend"] != "")
                {
                    query.recommend_user_id = Int32.Parse(Request.Params["recommend"]);
                }
                _userrecommendMgr = new UserRecommendMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _userrecommendMgr.QueryAll(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                foreach (var item in store)
                {

                    if (!string.IsNullOrEmpty(item.name))
                    {
                        item.name = item.name.Substring(0, 1) + "**";
                    }
                    if (!string.IsNullOrEmpty(item.usname))
                    {
                        item.usname = item.usname.Substring(0, 1) + "**";
                    }
                    if (!string.IsNullOrEmpty(item.user_name))
                    {
                        item.user_name = item.user_name.Substring(0, 1) + "**";
                    }
                    if (!string.IsNullOrEmpty(item.mail))
                    {
                        item.mail = item.mail.Split('@')[0] + "@***";
                    }
                    if (!string.IsNullOrEmpty(item.user_email))
                    {
                        item.user_email = item.user_email.Split('@')[0] + "@***";
                    }
                    if (!string.IsNullOrEmpty(item.user_mobile))
                    {
                        if (item.user_phone.ToString().Length > 3)
                        {
                            item.user_phone = item.user_phone.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.user_phone = item.user_phone + "***";
                        }
                    }
                    if (!string.IsNullOrEmpty(item.user_mobile))
                    {
                        if (item.user_mobile.ToString().Length > 3)
                        {
                            item.user_mobile = item.user_mobile.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.user_mobile = item.user_mobile + "***";
                        }
                    }
                    if (item.user_address.ToString().Length > 3)
                    {
                        item.user_address = item.user_address.Substring(0, 3) + "***";
                    }
                    else
                    {
                        item.user_address = item.user_address + "***";
                    }
                    item.user_id = Convert.ToInt32(item.suser_id);
                    item.Iuser_reg_date = CommonFunction.GetNetTime(item.user_reg_date);
                    item.suser_reg_date = CommonFunction.GetNetTime(item.user_reg_date);
                    //  item.birthday = item.user_birthday_year.ToString() + "/" + item.user_birthday_month.ToString() + "/" + item.user_birthday_day.ToString();

                }
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + ",'msg':'rec',}";//返回json數據
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

        #region 會員資料探勘保存導入文件+HttpResponseBase ExportCsv()

        [CustomHandleError]
        [HttpPost]
        public HttpResponseBase ExportCsv()
        {
            string filename = string.Empty;
            string json = "{success:false}";//json字符串
            userioMgr = new UserIOMgr(mySqlConnectionString);
            usersMgr = new UsersMgr(mySqlConnectionString);
            siteConfigMgr = new SiteConfigMgr(Server.MapPath(xmlPath));
            try
            {
                if (Request.Files["ImportCsvFile"] != null && Request.Files["ImportCsvFile"].ContentLength > 0)//判斷文件是否為空
                {
                    HttpPostedFileBase excelFile = Request.Files["ImportCsvFile"];//獲取文件流

                    FileManagement fileManagement = new FileManagement();//實例化 FileManagement

                    string fileLastName = excelFile.FileName.Substring((excelFile.FileName).LastIndexOf('.')).ToLower().Trim();
                    if (fileLastName.Equals(".csv"))
                    {

                        string newExcelName = Server.MapPath(excelPath) + "user_io" + fileManagement.NewFileName(excelFile.FileName);//處理文件名，獲取新的文件名

                        System.Data.DataTable _dt = new DataTable();

                        excelFile.SaveAs(newExcelName);//上傳文件
                        //excelHelper = new NPOI4ExcelHelper(newExcelName);
                        _dt = CsvHelper.ReadCsvToDataTable(newExcelName, true);//獲取csv里的數據
                        Regex num = new System.Text.RegularExpressions.Regex("^[0-9]+$");

                        string sqlAdition = "";
                        for (int i = 0; i < _dt.Rows.Count; i++)
                        {
                            if (_dt.Rows[i][0].ToString() != "")
                            {
                                if (!num.IsMatch(_dt.Rows[i][0].ToString()))//判斷是否匹配正則表達式
                                {
                                    System.IO.File.Delete(newExcelName);//刪除上傳過的excle
                                    int rows = i + 2;
                                    int cloumns = 1;
                                    json = "{success:false,msg:\"" + "第" + rows + "條第" + cloumns + "列數據錯誤" + "\"}";
                                    this.Response.Clear();
                                    this.Response.Write(json);
                                    this.Response.End();
                                    return this.Response;
                                }
                                else
                                {
                                    if (i == 0)
                                    {
                                        sqlAdition = _dt.Rows[i][0].ToString();
                                    }
                                    else
                                    {
                                        sqlAdition += "," + Convert.ToUInt32(_dt.Rows[i][0].ToString());//獲取id，以，隔開id
                                    }
                                }

                            }
                        }

                        DataTable dtexcel = userioMgr.GetExcelTable(sqlAdition);//從數據庫里獲取csv里的id的數據

                        #region 讀取會員資料
                        json = string.Empty;
                        if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
                        {
                            System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
                        }
                        try
                        {
                            string[] colname = new string[dtexcel.Columns.Count];
                            filename = "user_io" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
                            string newExcelfilename = Server.MapPath(excelPath) + filename;
                            for (int i = 0; i < dtexcel.Columns.Count; i++)
                            {
                                colname[i] = dtexcel.Columns[i].ColumnName;
                            }
                            if (System.IO.File.Exists(newExcelfilename))
                            {
                                System.IO.File.Delete(newExcelfilename);
                            }
                            excelFile.SaveAs(newExcelfilename);//上傳文件
                            CsvHelper.ExportDataTableToCsv(dtexcel, newExcelfilename, colname, true);


                            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                            timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                            json = "true";

                        }
                        catch (Exception ex)
                        {
                            Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                            logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                            logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            log.Error(logMessage);
                            json = "false,";
                        }

                        #endregion

                        #region ///更改前的內容
                        //string[] colname = new string[dtexcel.Columns.Count];//定義一個數組
                        //for (int i = 0; i < dtexcel.Columns.Count; i++)
                        //{
                        //    colname[i] = dtexcel.Columns[i].ColumnName;//獲取列名
                        //}
                        //CsvHelper.ExportDataTableToCsv(dtexcel, newExcelName, colname, true);

                        ////打開導出的Excel 文件
                        //System.Diagnostics.Process Excelopen = new System.Diagnostics.Process();
                        //Excelopen.StartInfo.FileName = newExcelName;
                        //Excelopen.Start();
                        #endregion

                        json = "{success:true,msg:\"" + filename + "\"}";
                    }
                    else
                    {
                        json = "{success:false,msg:\"" + "請匯入CSV格式檔案" + "\"}";
                        this.Response.Clear();
                        this.Response.Write(json);
                        this.Response.End();
                        return this.Response;

                    }
                }

                else
                {
                    json = "{success:false,msg:\"" + "請匯入檔案" + "\"}";
                }
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

        #region 新增電話會員 +HttpResponseBase SavePhone(); +string QueryCity() ; +string QueryZip()
        #region 保存 +HttpResponseBase SavePhone()
        [CustomHandleError]
        public HttpResponseBase SavePhone()
        {
            string jsonStr = string.Empty;
            UserQuery user = new UserQuery();
            HashEncrypt hmd5 = new HashEncrypt();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["name"]))
                {
                    user.user_name = Request.Params["name"].ToString();
                }
                else
                {
                    user.user_name = "";
                }
                if (!string.IsNullOrEmpty(Request.Params["tel"]))
                {
                    user.user_mobile = Request.Params["tel"].ToString();
                    if (user.user_mobile.Length < 10 || user.user_mobile.Substring(0, 2).ToString() != "09")
                    {
                        for (int i = user.user_mobile.Length; i < 10; i++)
                        {
                            user.user_mobile = "0" + user.user_mobile;
                        }
                    }
                }
                else
                {
                    user.user_mobile = "";
                }
                user.user_email = user.user_mobile + "@user.gigade.com.tw";
                #region 獲取生日的年月日
                try
                {
                    DateTime birth = Convert.ToDateTime(Request.Params["birth"].ToString());
                    user.user_birthday_year = Convert.ToUInt32(birth.Year);
                    user.user_birthday_month = Convert.ToUInt32(birth.Month);
                    user.user_birthday_day = Convert.ToUInt32(birth.Day);
                }
                catch (Exception)
                {
                    user.user_birthday_year = 1970;
                    user.user_birthday_month = 0;
                    user.user_birthday_day = 0;
                }
                #endregion
                #region 密碼
                user.user_password = "g" + user.user_birthday_year;
                if (user.user_birthday_month.ToString().Length == 1)
                {
                    user.user_password += "0" + user.user_birthday_month;
                }
                else
                {
                    user.user_password += user.user_birthday_month;
                }
                if (user.user_birthday_day.ToString().Length == 1)
                {
                    user.user_password += "0" + user.user_birthday_day;
                }
                else
                {
                    user.user_password += user.user_birthday_day;
                }
                user.user_password = hmd5.SHA256Encrypt(user.user_password);
                #endregion
                if (!string.IsNullOrEmpty(Request.Params["zip"]))
                {
                    user.user_zip = Convert.ToUInt32(Request.Params["zip"].ToString());
                }
                else
                {
                    user.user_zip = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["address"]))
                {
                    user.user_address = Request.Params["address"].ToString();
                }
                else
                {
                    user.user_address = "";
                }
                if (!string.IsNullOrEmpty(Request.Params["IsAcceptAd"]))
                {
                    if (Request.Params["IsAcceptAd"].ToString() == "on")
                    {
                        user.send_sms_ad = true;
                    }
                }
                else
                {
                    user.send_sms_ad = false;
                }
                if (!string.IsNullOrEmpty(Request.Params["Remark"]))
                {
                    user.adm_note = Request.Params["Remark"].ToString();
                }
                else
                {
                    user.adm_note = "";
                }

                user.ip = Request.UserHostAddress;
                user.file_name = "UserPhone.chtml";

                user.created = DateTime.Now;
                user.kuser_id = Convert.ToUInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                mgr = new CallerMgr(mySqlConnectionString);
                Caller caller = new Caller();
                caller = mgr.GetUserById(Convert.ToInt32(user.kuser_id));
                user.kuser_name = caller.user_username;

                user.content = "user_email:" + user.user_email + ",user_mobile:" + user.user_mobile + ",user_birthday_year" + user.user_birthday_year + ",user_birthday_month" + user.user_birthday_month + ",user_birthday_day" + user.user_birthday_day + ",user_zip" + user.user_zip + ",user_address" + user.user_address + ",send_sms_ad" + user.send_sms_ad + ",adm_note" + user.adm_note;

                user.user_status = 1;
                user.user_source = "電話會員";
                user.user_login_attempts = 0;
                user.user_reg_date = Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                user.user_updatedate = Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                user.user_type = 2;

                _usmpgr = new UsersMgr(mySqlConnectionString);//實現方法
                if (_usmpgr.QueryByUserMobile(user.user_mobile).Rows.Count == 0)
                {
                    _usmpgr = new UsersMgr(mySqlConnectionString);
                    if (_usmpgr.SaveUserPhone(user) > 0)
                    {
                        jsonStr = "{success:true,msg:1}";
                    }
                    else
                    {
                        jsonStr = "{success:false,msg:2 }";
                    }
                }
                else
                {
                    jsonStr = "{success:false,msg:3 }";
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
        #endregion
        #region 獲取區域地址  +string QueryCity() ; +string QueryZip()
        [CustomHandleError]
        public string QueryCity()
        {
            zMgr = new ZipMgr(mySqlConnectionString);
            string json = string.Empty;
            try
            {
                json = zMgr.QueryMiddle(Request.Form["topValue"] ?? "");
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "[]";
            }
            return json;
        }

        [CustomHandleError]
        public string QueryZip()
        {
            zMgr = new ZipMgr(mySqlConnectionString);
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["topValue"]))
                {
                    json = zMgr.QuerySmall(Request.Form["topValue"].ToString(), Request.Form["topText"].ToString());
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "[]";
            }
            return json;
        }
        #endregion
        #endregion

        #region 同步會員至電子報 +HttpResponseBase UserSyncToEdmList()
        /// <summary>
        /// 同步會員至電子報
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase UserSyncToEdmList()
        {
            string json = String.Empty;
            try
            {
                _edmMgr = new UserEdmMgr(mySqlConnectionString);
                json = _edmMgr.UpdateEdm();
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

        #region 會員登入記錄+HttpResponseBase UserLoginLogList()
        public HttpResponseBase UserLoginLogList()
        {
            /********************************************************/
            List<UsersLoginQuery> stores = new List<UsersLoginQuery>();

            string json = string.Empty;
            try
            {
                UsersLoginQuery query = new UsersLoginQuery();

                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量

                if (!string.IsNullOrEmpty(Request.Params["myuser_id"]))
                {
                    query.user_id = Convert.ToUInt32(Request.Params["myuser_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["timestart"]))
                {
                    query.serchstart = Convert.ToDateTime(Request.Params["timestart"]);
                    query.serchstart = Convert.ToDateTime(query.serchstart.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if (!string.IsNullOrEmpty(Request.Params["timeend"]))
                {
                    query.serchend = Convert.ToDateTime(Request.Params["timeend"]);
                    query.serchend = Convert.ToDateTime(query.serchend.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                _userloginlog = new UserLoginLogMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _userloginlog.Query(query, out totalCount);
                foreach (var item in stores)
                {
                    if (!string.IsNullOrEmpty(item.username))
                    {
                        item.username = item.username.Substring(0, 1) + "**";
                    }
                    item.slogin_createdate = CommonFunction.GetNetTime(item.login_createdate);

                }
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

        #region 獲取會員購買記錄列表 + HttpResponseBase GetVipList()
        /// <summary>
        /// 獲取會員購買記錄列表
        /// create by shuangshuang0420j 20140923 17:44
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase GetVipList()
        {
            /********************************************************/
            List<UserVipListQuery> stores = new List<UserVipListQuery>();

            string json = string.Empty;
            try
            {
                UserVipListQuery query = new UserVipListQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量

                if (!string.IsNullOrEmpty(Request.Params["dateOne"]))
                {
                    query.create_dateOne = (uint)CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["dateOne"]).ToString("yyyy-MM-dd HH:mm:ss"));

                }
                if (!string.IsNullOrEmpty(Request.Params["dateTwo"]))
                {
                    query.create_dateTwo = (uint)CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["dateTwo"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if (!string.IsNullOrEmpty(Request.Params["user_id"]))
                {
                    query.user_id = uint.Parse(Request.Params["user_id"]); 
                }

                usersMgr = new UsersMgr(mySqlConnectionString);
                int totalCount = 0;


                stores = usersMgr.GetVipList(query, ref totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                // timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                DataTable _dt = new DataTable();
                foreach (var item in stores)
                {
                    if (!string.IsNullOrEmpty(item.user_name))
                    {
                        item.user_name = item.user_name.Substring(0, 1) + "**";
                    }
                    //item.sum_bonus = item.normal_deduct_bonus + item.low_deduct_bonus;
                    //item.sum_amount = item.normal_product + item.low_product;
                    //獲取客單價的上限
                    decimal s = item.sum_amount / item.cou;
                    int sint = Convert.ToInt32(s);
                    item.aver_amount = s > sint ? sint + 1 : sint;                  
                    //獲取時間
                    item.reg_date = CommonFunction.GetNetTime(item.user_reg_date);
                    item.create_date = CommonFunction.GetNetTime(item.order_createdate);
                    item.birthday = item.user_birthday_year.ToString() + "/" + item.user_birthday_month.ToString() + "/" + item.user_birthday_day.ToString();
                    item.mytype = item.user_type == 1 ? "網絡會員" : "電話會員";
                    item.vip = "N";
                    _dt = usersMgr.IsVipUserId(item.user_id);
                    if (_dt.Rows.Count != 0)
                    {
                        item.vip = "Y";
                    }
                }
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

        #region 會員購買記錄排行匯出csv
        public HttpResponseBase ExportVipListCsv()
        {
            string json = string.Empty;
            UserVipListQuery query = new UserVipListQuery();
            List<UserVipListQuery> stores = new List<UserVipListQuery>();
            try
            {              
                if (!string.IsNullOrEmpty(Request.Params["dateOne"]))
                {
                    query.create_dateOne = (uint)CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["dateOne"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if (!string.IsNullOrEmpty(Request.Params["dateTwo"]))
                {
                    query.create_dateTwo = (uint)CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["dateTwo"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if (!string.IsNullOrEmpty(Request.Params["user_id"]))
                {
                    query.user_id = uint.Parse(Request.Params["user_id"]);
                }

                int totalCount = 0;
                query.IsPage = false;
                zMgr = new ZipMgr(mySqlConnectionString);
                usersMgr = new UsersMgr(mySqlConnectionString);
                stores = usersMgr.ExportVipListCsv(query,ref totalCount );
                DataTable _vipdt = usersMgr.IsVipUserId(0);
                DataTable newDt = new DataTable();
                newDt.Columns.Add("user_id", typeof(string));
                newDt.Columns.Add("user_status", typeof(string));
                newDt.Columns.Add("user_name", typeof(string));
                newDt.Columns.Add("user_gender", typeof(string));
                newDt.Columns.Add("VIP", typeof(string));
                // newDt.Columns.Add("user_email", typeof(string));
                newDt.Columns.Add("age", typeof(string));
                newDt.Columns.Add("user_birthday_month", typeof(string));
                // newDt.Columns.Add("user_address", typeof(string));
                newDt.Columns.Add("user_reg_date", typeof(string));//需要轉
                newDt.Columns.Add("order_createdate", typeof(string));//需要轉
                newDt.Columns.Add("last_time", typeof(string));//需要轉
                newDt.Columns.Add("sum_amount", typeof(string));
                newDt.Columns.Add("cou", typeof(string));
                newDt.Columns.Add("aver_amount", typeof(string));//計算得出
                newDt.Columns.Add("sum_bonus", typeof(string));
                newDt.Columns.Add("normal_product", typeof(string));
                newDt.Columns.Add("freight_normal", typeof(string));
                newDt.Columns.Add("low_product", typeof(string));
                newDt.Columns.Add("freight_low", typeof(string));
                newDt.Columns.Add("ct", typeof(string));
                newDt.Columns.Add("HG", typeof(string));
                newDt.Columns.Add("ht", typeof(string));
                newDt.Columns.Add("ml_code", typeof(string));
                //newDt.Columns.Add("order_product_subtotal", typeof(string));
                for (int i = 0; i < stores.Count; i++)
                {
                    DataRow newRow = newDt.NewRow();
                    newRow["user_id"] = stores[i].user_id;
                    //newRow["user_status"] = stores[i].user_status;
                    switch (stores[i].user_status)
                    {
                        case 0: newRow["user_status"] = "未啟用";
                                break;
                        case 1: newRow["user_status"] = "已啟用";
                                break;
                        case 2: newRow["user_status"] = "停用";
                                break;
                        case 5: newRow["user_status"] = "簡易會員";
                                break;
                    }
                    newRow["user_name"] = stores[i].user_name;
                    newRow["user_gender"] = stores[i].user_gender == 0 ? "小姐" : "先生";
                    newRow["VIP"] = "N";
                    for (int j = 0; j < _vipdt.Rows.Count; j++)
                    {
                        if (_vipdt.Rows[j]["user_id"] != null)
                        {
                            if (stores[i].user_id.ToString() == _vipdt.Rows[j]["user_id"].ToString())
                            {
                                newRow["VIP"] = "Y";
                                break;
                            }
                        }
                    }
                    //newRow["user_email"] = stores[i].user_email;
                    newRow["age"] = DateTime.Now.Year - stores[i].user_birthday_year;
                    newRow["user_birthday_month"] = stores[i].user_birthday_month;
                    //if (zMgr.QueryCityAndZip(stores[i].user_zip.ToString()) != null)
                    //{
                    //    newRow["user_address"] = zMgr.QueryCityAndZip(stores[i].user_zip.ToString()).middle + " " + zMgr.QueryCityAndZip(stores[i].user_zip.ToString()).small;
                    //}
                    //else
                    //{
                    //    newRow["user_address"] = "";
                    //}
                    newRow["user_reg_date"] = CommonFunction.DateTimeToString(CommonFunction.GetNetTime(stores[i].user_reg_date));
                    newRow["order_createdate"] = CommonFunction.DateTimeToString(CommonFunction.GetNetTime(stores[i].order_createdate));
                    newRow["last_time"] = newRow["order_createdate"];
                    //if (stores[i].last_time == Convert.ToUInt32(CommonFunction.GetPHPTime("1970-1-1 8:00")))
                    //{
                    //    newRow["last_time"] = "";
                    //}
                    //else
                    //{
                    //    newRow["last_time"] = CommonFunction.DateTimeToString(CommonFunction.GetNetTime(stores[i].last_time));
                    //}
                    newRow["sum_amount"] = stores[i].sum_amount;
                    newRow["cou"] = stores[i].cou;
                    decimal s = stores[i].sum_amount / stores[i].cou;
                    int sint = Convert.ToInt32(s);
                    newRow["aver_amount"] = s > sint ? sint + 1 : sint;
                    newRow["sum_bonus"] = stores[i].sum_bonus;
                    newRow["normal_product"] = stores[i].normal_product;
                    newRow["freight_normal"] = stores[i].freight_normal;
                    newRow["low_product"] = stores[i].low_product;
                    newRow["freight_low"] = stores[i].freight_low;
                    newRow["ct"] = stores[i].ct;
                    newRow["HG"] = stores[i].ct;
                    newRow["ht"] = stores[i].ht;
                    newRow["ml_code"] = stores[i].ml_code;
                    //newRow["order_product_subtotal"] = stores[i].order_product_subtotal;
                    newDt.Rows.Add(newRow);
                }
                // string[] columnName = { "編號", "會員狀態", "姓名", "性別", "VIP", "電子郵件", "年齡", "生日月份", "居住區", "註冊時間", "最近歸檔日", "最近購買日", "購買金額", "購買次數", "客單價", "購物金使用", "常溫商品總額", "常溫商品運費", "低溫商品總額", "低溫商品運費", "中信折抵", "HG折抵", "台新折抵" };
                string[] columnName = { "會員編號", "會員狀態", "姓名", "性別", "VIP", "年齡", "生日月份", "註冊時間", "最近歸檔日", "最近購買日", "購買金額", "購買次數", "客單價", "購物金使用", "常溫商品總額", "常溫商品運費", "低溫商品總額", "低溫商品運費", "中信折抵", "HG折抵", "台新折抵", "會員等級"};//, "近期累積金額" };

                string fileName = "Vip_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                string newFileName = Server.MapPath(excelPath) + fileName;
                json = "{success:'true',filename:'" + fileName + "'}";
                CsvHelper.ExportDataTableToCsv(newDt, newFileName, columnName, true);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:'false'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #endregion

        #region  會員群組管理
        /*獲取用戶群組列表*/
        #region 獲取用戶群組列表 + HttpResponseBase GetVipUserGroupList()
        /// <summary> 
        /// 獲取用戶群組列表
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase GetVipUserGroupList()
        {
            /********************************************************/
            List<VipUserGroupQuery> stores = new List<VipUserGroupQuery>();
            string json = string.Empty;
            try
            {
                VipUserGroupQuery query = new VipUserGroupQuery();
                int totalCount = 0;
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                _userGroupMgr = new VipUserGroupMgr(mySqlConnectionString);
                string group_id_or_group_name = Request.Params["group_id_or_group_name"];
                string gName = string.Empty;
                string gNameSubString = string.Empty;
                char[] specialChar = { '[', '_', '%' };
                int n = 0;
                //if (!string.IsNullOrEmpty(Request.Params["dateOne"]))
                //{
                //    query.create_dateOne = (uint)CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["dateOne"]).ToString("yyyy-MM-dd 00:00:00"));

                //}
                //if (!string.IsNullOrEmpty(Request.Params["dateTwo"]))
                //{
                //    query.create_dateTwo = (uint)CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["dateTwo"]).ToString("yyyy-MM-dd 23:59:59"));
                //}

                //用於判斷是查詢條件是群組編號/群組名稱， 
                // by zhaozhi0623j，2015/09/22
                if (!string.IsNullOrEmpty(group_id_or_group_name))
                {
                    uint result = 0;
                    if (uint.TryParse(group_id_or_group_name, out result))
                    {
                        query.group_id = result;
                    }
                    //查詢條件為群組名稱時，判斷字符串中是否含有"%","_","["  如有為其前方添加轉意符號"\",便於查詢
                    else
                    {
                        query.group_name = group_id_or_group_name;
                        gName = group_id_or_group_name;
                        n = gName.IndexOfAny(specialChar);
                        if (n >= 0)
                        {
                            gNameSubString = gName.Substring(n, gName.Length - n);
                            query.group_name = gName.Replace(gNameSubString, "\\" + gNameSubString);
                        }
                    }
                }
                System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
                string ip = string.Empty;
                if (addlist.Length > 0)
                {
                    ip = addlist[0].ToString();
                }
                stores = _userGroupMgr.QueryAll(query, out totalCount);
                foreach (var item in stores)
                {

                    if (item.image_name != "")
                    {
                        item.image_name = imgServerPath + promoPath + item.image_name;
                    }
                    else
                    {
                        item.image_name = defaultImg;
                    }
                    item.screatedate = CommonFunction.GetNetTime(item.createdate).ToString("yyyy/MM/dd");
                    item.list = _userGroupMgr.GetVuserCount(item);
                    item.ip = ip;
                }
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
        /*編輯 新增和修改功能 */
        #region 新增和修改+ public HttpResponseBase SaveVipUserGroup()
        [CustomHandleError]
        //  [HttpPost]
        public HttpResponseBase SaveVipUserGroup()
        {
            NameValueCollection param = Request.Params;
            VipUserGroup userGroup = new VipUserGroup();
            string json = string.Empty;
            bool size = true;
            Serial serial = new Serial();
            _userGroupMgr = new VipUserGroupMgr(mySqlConnectionString);
            _ISerImplMgr = new SerialMgr(mySqlConnectionString);
            serial = _ISerImplMgr.GetSerialById(72);
            string path = Server.MapPath(xmlPath);
            SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
            SiteConfig extention_config = _siteConfigMgr.GetConfigByName("PIC_Extention_Format");
            SiteConfig minValue_config = _siteConfigMgr.GetConfigByName("PIC_Length_MinValue");
            SiteConfig maxValue_config = _siteConfigMgr.GetConfigByName("PIC_Length_MaxValue");
            SiteConfig admin_userName = _siteConfigMgr.GetConfigByName("ADMIN_USERNAME");
            SiteConfig admin_passwd = _siteConfigMgr.GetConfigByName("ADMIN_PASSWD");
            //擴展名、最小值、最大值
            string extention = extention_config.Value == "" ? extention_config.DefaultValue : extention_config.Value;
            string minValue = minValue_config.Value == "" ? minValue_config.DefaultValue : minValue_config.Value;
            string maxValue = maxValue_config.Value == "" ? maxValue_config.DefaultValue : maxValue_config.Value;
            string localPromoPath = imgLocalPath + promoPath;//圖片存儲地址
            Random rand = new Random();
            int newRand = rand.Next(1000, 9999);

            string NewName = string.Empty;//當前文件名
            string fileExtention = string.Empty;//當前文件的擴展名
            string NewFileName = string.Empty;
            FileManagement fileLoad = new FileManagement();
            try
            {
                #region 新增
                if (String.IsNullOrEmpty(param["group_id"]))
                {
                    if (!string.IsNullOrEmpty(Request.Form["group_name"]))
                    {
                        userGroup.group_name = Request.Form["group_name"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Form["tax_id"]))
                    {
                        userGroup.tax_id = Request.Form["tax_id"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Form["eng_name"]))
                    {
                        userGroup.eng_name = Request.Form["eng_name"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Form["gift_bonus"]))
                    {
                        userGroup.gift_bonus = Convert.ToUInt32(Request.Form["gift_bonus"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Form["group_category"]))
                    {
                        userGroup.group_category = Convert.ToUInt32(Request.Form["group_category"]);
                    }

                    siteConfigMgr = new SiteConfigMgr(Server.MapPath(xmlPath));
                    try
                    {
                        if (Request.Files["image_name"] != null && Request.Files["image_name"].ContentLength > 0)
                        {
                            HttpPostedFileBase file = Request.Files["image_name"];
                            if (file.ContentLength > int.Parse(minValue) * 1024 && file.ContentLength < int.Parse(maxValue) * 1024)
                            {
                                NewName = Path.GetFileName(file.FileName);
                                bool result = false;
                                //獲得文件的後綴名
                                fileExtention = NewName.Substring(NewName.LastIndexOf(".")).ToLower();
                                //新的文件名是隨機數字
                                BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                                NewFileName = hash.Md5Encrypt(newRand.ToString(), "32") + fileExtention;
                                NewName = NewFileName;
                                string ServerPath = string.Empty;
                                //判斷目錄是否存在，不存在則創建
                                string[] mapPath = new string[1];
                                mapPath[0] = promoPath.Substring(1, promoPath.Length - 2);
                                string jian = localPromoPath.Substring(0, localPromoPath.Length - promoPath.Length + 1);
                                CreateFolder(localPromoPath.Substring(0, localPromoPath.Length - promoPath.Length + 1), mapPath);
                                NewFileName = localPromoPath + NewFileName;//絕對路徑
                                ServerPath = Server.MapPath(imgLocalServerPath + promoPath);
                                string ErrorMsg = string.Empty;
                                //上傳
                                result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                                if (result)
                                {
                                    userGroup.image_name = NewName;
                                }

                            }
                            else
                            {
                                size = false;
                            }
                        }

                    }

                    catch (Exception)
                    {
                        userGroup.image_name = string.Empty;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["check_iden"]))
                    {
                        userGroup.check_iden = int.Parse(Request.Params["check_iden"]);
                    }
                    userGroup.createdate = (uint)CommonFunction.GetPHPTime(DateTime.Now.ToString());
                    userGroup.group_id = uint.Parse((serial.Serial_Value + 1).ToString());
                    if (_userGroupMgr.Insert(userGroup) > 0)
                    {
                        serial.Serial_Value = serial.Serial_Value + 1;/*所在操作表的列增加*/
                        _ISerImplMgr.Update(serial);/*修改所在的表的列對應的值*/
                        if (size)
                        {
                            json = "{success:true,msg:\"" + "" + "\"}";
                        }
                        else
                        {
                            json = "{success:true,msg:\"" + " 文件大小應大於1KB小於100KB！" + "\"}";
                        }
                    }
                    else
                    {
                        json = "{success:false,msg:\"" + "新增失敗!" + "\"}";
                    }

                }
                #endregion
                #region 編輯
                else
                {
                    _userGroupMgr = new VipUserGroupMgr(mySqlConnectionString);
                    userGroup.group_id = Convert.ToUInt32(param["group_id"]);
                    VipUserGroup oldUserGroup = _userGroupMgr.GetModelById(userGroup.group_id);
                    if (!string.IsNullOrEmpty(Request.Form["group_name"]))
                    {
                        userGroup.group_name = Request.Form["group_name"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Form["tax_id"]))
                    {
                        userGroup.tax_id = Request.Form["tax_id"].ToString();
                    }

                    if (!string.IsNullOrEmpty(Request.Form["eng_name"]))
                    {
                        userGroup.eng_name = Request.Form["eng_name"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Form["gift_bonus"]))
                    {
                        userGroup.gift_bonus = Convert.ToUInt32(Request.Form["gift_bonus"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Form["group_category"]))
                    {
                        userGroup.group_category = Convert.ToUInt32(Request.Form["group_category"]);
                    }
                    try
                    {
                        //如果圖片沒有改變
                        if (Request.Form["image_name"] == oldUserGroup.image_name)
                        {
                            userGroup.image_name = Request.Form["image_name"];
                        }
                        else
                        {
                            //圖片改變了
                            if (Request.Files["image_name"] != null && Request.Files["image_name"].ContentLength > 0)
                            {
                                HttpPostedFileBase file = Request.Files["image_name"];
                                if (file.ContentLength > int.Parse(minValue) * 1024 && file.ContentLength < int.Parse(maxValue) * 1024)
                                {
                                    NewName = Path.GetFileName(file.FileName);
                                    bool result = false;

                                    //獲得文件的後綴名
                                    fileExtention = NewName.Substring(NewName.LastIndexOf(".")).ToLower();
                                    //新的文件名是隨機數字
                                    BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                                    NewFileName = hash.Md5Encrypt(newRand.ToString(), "32") + fileExtention;
                                    NewName = NewFileName;
                                    string ServerPath = string.Empty;
                                    //判斷目錄是否存在，不存在則創建
                                    string[] mapPath = new string[1];
                                    mapPath[0] = promoPath.Substring(1, promoPath.Length - 2);
                                    string jian = localPromoPath.Substring(0, localPromoPath.Length - promoPath.Length + 1);
                                    CreateFolder(localPromoPath.Substring(0, localPromoPath.Length - promoPath.Length + 1), mapPath);
                                    //  returnName += promoPath + NewFileName;
                                    NewFileName = localPromoPath + NewFileName;//絕對路徑
                                    ServerPath = Server.MapPath(imgLocalServerPath + promoPath);
                                    string ErrorMsg = string.Empty;
                                    //上傳之前刪除已有的圖片
                                    string oldFileName = oldUserGroup.image_name;
                                    //FTP ftp = new FTP(localPromoPath, ftpuser, ftppwd);
                                    //List<string> tem = ftp.GetFileList();
                                    //上傳
                                    result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                                    if (result)
                                    {
                                        userGroup.image_name = NewName;

                                        if (System.IO.File.Exists(ServerPath + oldFileName))
                                        {
                                            FTP ftps = new FTP(localPromoPath + oldFileName, ftpuser, ftppwd);
                                            if (System.IO.File.Exists(localPromoPath + oldFileName))
                                            {
                                                ftps.DeleteFile(localPromoPath + oldFileName);//刪除ftp:71.159上的舊圖片
                                            }
                                            DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
                                        }
                                    }
                                    else
                                    {
                                        userGroup.image_name = oldUserGroup.image_name;
                                    }
                                }
                                else
                                {
                                    size = false;
                                    userGroup.image_name = oldUserGroup.image_name;
                                }
                            }
                        }
                    }

                    catch (Exception)
                    {
                        userGroup.image_name = oldUserGroup.image_name;
                    }
                    try
                    {
                        userGroup.check_iden = int.Parse(Request.Params["check_iden"]);
                    }
                    catch (Exception)
                    {
                        userGroup.check_iden = 0;
                    }
                    if (_userGroupMgr.Update(userGroup) > 0)
                    {
                        if (size)
                        {
                            json = "{success:true,msg:\"" + "" + "\"}";
                        }
                        else
                        {
                            json = "{success:true,msg:\"" + " 文件大小應大於1KB小於100KB！" + "\"}";
                        }
                    }
                    else
                    {
                        json = "{success:false,msg:\"" + "修改失敗!" + "\"}";
                    }

                }
                #endregion
            }
            catch (Exception ex)
            {
                json = "{success:false,msg:\"" + "異常" + "\"}";
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
        /*獲取用戶列表*/
        #region 獲取群組下的用戶列表 + HttpResponseBase GetVipUserList()
        /// <summary>
        /// 獲取用戶列表
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase GetVipUserList()
        {
            List<VipUserQuery> stores = new List<VipUserQuery>();
            string json = string.Empty;
            try
            {
                int totalCount = 0;
                VipUserQuery query = new VipUserQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                if (!string.IsNullOrEmpty(Request.QueryString["groupid"]))
                {
                    query.group_id = Convert.ToUInt32(Request.QueryString["groupid"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["serchs"]))
                {
                    query.serchtype = Request.Params["serchs"];
                }
                query.content = Request.Params["serchcontent"];
                _userGroupMgr = new VipUserGroupMgr(mySqlConnectionString);
                stores = _userGroupMgr.GetVipUserList(query, out totalCount);
                foreach (var item in stores)
                {
                    if (!string.IsNullOrEmpty(item.createdate.ToString()))
                    {
                        item.screatedate = CommonFunction.GetNetTime(item.createdate).ToString("yyyy/MM/dd");
                    }
                    else
                    {
                        item.screatedate = CommonFunction.GetNetTime(0).ToString("yyyy/MM/dd");
                    }
                    if (!string.IsNullOrEmpty(item.user_reg_date.ToString()))
                    {
                        item.reg_date = CommonFunction.GetNetTime(item.user_reg_date);
                    }
                    else
                    {
                        item.reg_date = CommonFunction.GetNetTime(0);
                    }
                }
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

        /// <summary>
        /// 通過郵箱獲取該用戶的信息
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetUserName()
        {
            UserQuery user = new UserQuery();
            List<UserQuery> userList = new List<UserQuery>();
            string user_email = "";
            uint group_id = 0;
            if (!string.IsNullOrEmpty(Request.Params["Email"]))
            {
                user_email = Request.Params["Email"];
            }
            if (!string.IsNullOrEmpty(Request.Params["group_id"]))
            {
                group_id = uint.Parse(Request.Params["group_id"]);
            }


            _usmpgr = new UsersMgr(mySqlConnectionString);
            string jsonStr = string.Empty;
            try
            {

                userList = _usmpgr.GetUserByEmail(user_email, group_id);
                if (userList.Count() > 0)//查詢到會員
                {
                    jsonStr = "{success:true,msg:\"" + 99 + "\"}";//該用戶已在此群組中
                }
                else
                {
                    userList = _usmpgr.GetUserByEmail(user_email, 0);
                    if (userList.Count() > 0)
                    {
                        jsonStr = "{success:true,msg:\"" + 100 + "\",user_id:'" + userList[0].user_id + "',user_name:'" + userList[0].user_name + "'}";//返回json數據
                    }
                    else
                    {
                        jsonStr = "{success:true,msg:\"" + 98 + "\"}";//此用戶不存在
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
        /// <summary>
        /// 新增會員至群組
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SaveVipUser()
        {
            string jsonStr = "";
            VipUserQuery vip = new VipUserQuery();
            if (!string.IsNullOrEmpty(Request.Params["usermail"]))
            {
                vip.user_email = Request.Params["usermail"];
            }
            if (!string.IsNullOrEmpty(Request.Params["user_id"]))
            {
                vip.user_id = uint.Parse(Request.Params["user_id"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["group_id"]))
            {
                vip.group_id = uint.Parse(Request.Params["group_id"]);
            }

            vip.create_id = uint.Parse((Session["caller"] as Caller).user_id.ToString());
            vip.update_id = uint.Parse((Session["caller"] as Caller).user_id.ToString());
            vip.createdate = uint.Parse(CommonFunction.GetPHPTime().ToString());
            vip.updatedate = vip.createdate;
            try
            {
                _vipuserMgr = new VipUserMgr(mySqlConnectionString);
                if (_vipuserMgr.AddVipUser(vip) > 0)
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
        /// <summary>
        /// 刪除會員至群組
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase DeleVipUser()
        {
            string jsonStr = "";
            VipUserQuery vip = new VipUserQuery();
            if (!string.IsNullOrEmpty(Request.Params["vid"]))
            {
                vip.v_id = uint.Parse(Request.Params["vid"]);
            }

            try
            {
                _vipuserMgr = new VipUserMgr(mySqlConnectionString);
                if (_vipuserMgr.DeleVipUser(vip) > 0)
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
        /*修改會員狀態，啟用或者禁用 */
        #region 修改會員狀態，啟用或者禁用
        public JsonResult UpdateUserState()
        {
            int id = Convert.ToInt32(Request.Params["id"]);
            int activeValue = Convert.ToInt32(Request.Params["active"]);

            _userGroupMgr = new VipUserGroupMgr(mySqlConnectionString);
            if (_userGroupMgr.UpdateUserState(activeValue, id) > 0)
            {
                return Json(new { success = "true", msg = "" });
            }
            else
            {
                return Json(new { success = "false", msg = "" });
            }
        }

        #endregion
        /*匯入 */
        #region 匯入文件+ public HttpResponseBase ImportCsv()
        [CustomHandleError]
        [HttpPost]
        public HttpResponseBase ImportCsv()
        {
            string json = "";
            string errorMsg = "";
            string repeatMsg = "";
            _userGroupMgr = new VipUserGroupMgr(mySqlConnectionString);
            string sqlwhere = string.Empty;
            VipUser query = new VipUser();

            int add = 0;
            int repeat = 0;
            int error = 0;
            string newCSVName = string.Empty;
            string check_iden = string.Empty;
            List<string> emp_id = new List<string>();
            if (!string.IsNullOrEmpty(Request.Params["group_id"]))
            {
                query.group_id = Convert.ToUInt32(Request.Params["group_id"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["check_iden"]))
            {
                check_iden = Request.Params["check_iden"];
            }

            siteConfigMgr = new SiteConfigMgr(Server.MapPath(xmlPath));
            try
            {
                if (Request.Files["ImportCsvFile"] != null && Request.Files["ImportCsvFile"].ContentLength > 0)
                {
                    //一些文件的方法，包括获取文件的类型、长度、保存
                    HttpPostedFileBase excelFile = Request.Files["ImportCsvFile"];
                    //自定义的类，其中包括一些文件的处理方法
                    FileManagement fileManagement = new FileManagement();

                    //文件重命名，然後設置存儲的路徑
                    newCSVName = Server.MapPath(excelPath) + "user_io" + fileManagement.NewFileName(excelFile.FileName);//處理文件名，獲取新的文件名
                    System.Data.DataTable _dt = new DataTable();
                    excelFile.SaveAs(newCSVName);//
                    _dt = CsvHelper.ReadCsvToDataTable(newCSVName, true);
                    //匯入員工編號
                    DataTable btopEmp = _userGroupMgr.BtobEmp(query.group_id.ToString());
                    for (int k = 0; k < btopEmp.Rows.Count; k++)
                    {
                        if (!string.IsNullOrEmpty(btopEmp.Rows[k]["emp_id"].ToString()))
                        {
                            emp_id.Add(btopEmp.Rows[k]["emp_id"].ToString());
                        }
                    }

                    //郵箱的格式
                    Regex num = new System.Text.RegularExpressions.Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        //只讀第一列,其餘列忽略 
                        for (int j = 0; j < 1; j++)
                        {
                            //匯入員工編號
                            if (!string.IsNullOrEmpty(check_iden))
                            {
                                int u1 = _userGroupMgr.UpdateEmp(query.group_id.ToString(), _dt.Rows[i][j].ToString(), 1);
                                if (emp_id.Contains(_dt.Rows[i][j].ToString()))
                                {
                                    int u2 = _userGroupMgr.UpdateEmp(query.group_id.ToString(), _dt.Rows[i][j].ToString(), 2);
                                    json = "{success:true,msg:\"" + "上傳成功！" + "\"}";
                                }
                                else
                                {
                                    int h = _userGroupMgr.InsertEmp(query.group_id.ToString(), _dt.Rows[i][j].ToString());
                                    json = "{success:true,msg:\"" + "上傳成功！" + "\"}";
                                }
                            }
                            //匯入郵箱
                            else
                            {
                                //郵箱的格式驗證
                                if (!num.IsMatch(_dt.Rows[i][j].ToString()))
                                {
                                    errorMsg += "<br/>第" + (i + 1) + "條第" + (j + 1) + "列數據錯誤";
                                    error++;
                                }
                                else
                                {
                                    sqlwhere = _dt.Rows[i][j].ToString();
                                    DataTable dtUser = _userGroupMgr.GetUser(sqlwhere);
                                    //判斷郵箱是否是存在于users表中
                                    if (dtUser.Rows.Count > 0)
                                    {
                                        query.user_email = _dt.Rows[i][j].ToString();
                                        DataTable VipUserExist = _userGroupMgr.GetVipUser(query);
                                        //根據郵箱和group_id來判斷vip_user表中是否存在此數據，若不存在，就添加，若存在，就不添加
                                        if (VipUserExist.Rows.Count == 0)
                                        {
                                            query.vuser_email = _dt.Rows[i][j].ToString();
                                            query.createdate = (uint)CommonFunction.GetPHPTime(DateTime.Now.ToString());
                                            if (!string.IsNullOrEmpty(dtUser.Rows[0][0].ToString()))
                                            {
                                                query.User_Id = Convert.ToUInt32(dtUser.Rows[0][0]);
                                            }
                                            int k = _userGroupMgr.InsertVipUser(query);
                                            if (k > 0)
                                            {
                                                add++;
                                            }
                                        }
                                        else
                                        {
                                            //json = "{success:false,msg:\"" + "第" + i + 1 + "條第" + j + "列數據重複" + "\"}";
                                            repeatMsg += "<br/>第" + (i + 1) + "條第" + (j + 1) + "列數據重複";
                                            repeat++;
                                        }
                                    }
                                    else
                                    {
                                        //json = "{success:false,msg:\"" + "第" + i + 1 + "條第" + j + "列數據錯誤" + "\"}";
                                        errorMsg += "<br/>第" + (i + 1) + "條第" + (j + 1) + "列數據錯誤";
                                        error++;
                                    }


                                }
                            }
                        }
                    }

                    //string[] colname = new string[_dt.Columns.Count];
                    //for (int i = 0; i < _dt.Columns.Count; i++)
                    //{
                    //    colname[i] = _dt.Columns[i].ColumnName;
                    //}
                    //CsvHelper.ExportDataTableToCsv(_dt, newExcelName, colname, true);


                    json = "{success:true,msg:\"" + errorMsg + repeatMsg + "<br/>" + "上傳成功！<br/>新增：" + add + "<br/>重複：" + repeat + "<br/>錯誤：" + error + "<br/>總計：" + _dt.Rows.Count + "\"}";

                }

                else
                {
                    json = "{success:false,msg:\"" + "請匯入檔案" + "\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                //出現異常后,刪除上傳的文件
                if (System.IO.File.Exists(newCSVName))
                {
                    //設置文件的屬性，以防刪除文件的時候因為文件的屬性造成無法刪除
                    System.IO.File.SetAttributes(newCSVName, FileAttributes.Normal);
                    System.IO.File.Delete(newCSVName);
                }
                json = "{success:false,msg:\"" + errorMsg + repeatMsg + "上傳失敗！" + "\"}";
            }

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 刪除本地上傳的圖片
        /// <summary>
        /// 刪除本地上傳的圖片
        /// </summary>

        public void DeletePicFile(string imageName)
        {
            if (System.IO.File.Exists(imageName))
            {
                System.IO.File.Delete(imageName);
            }
        }
        #endregion
        #region 創建ftp文件夾
        /// <summary>
        /// 創建文件夾
        /// </summary>
        /// <param name="path">路徑</param>
        /// <param name="Mappath">文件名</param>
        public void CreateFolder(string path, string[] Mappath)
        {
            FTP ftp = null;
            try
            {
                string fullPath = path;
                foreach (string s in Mappath)
                {
                    ftp = new FTP(fullPath.Substring(0, fullPath.Length - 1), ftpuser, ftppwd);
                    fullPath += s;

                    if (!ftp.DirectoryExist(s.Replace("/", "")))
                    {
                        ftp = new FTP(fullPath.Substring(0, fullPath.Length - 1), ftpuser, ftppwd);
                        ftp.MakeDirectory();
                    }

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

        #region UserList,會員搜索
        #region 會員管理 會員列表 +HttpResponseBase UsersList()
        public HttpResponseBase UsersList()
        {
            List<UsersListQuery> stores = new List<UsersListQuery>();
            List<SiteConfig> configs = new List<SiteConfig>();
            SiteConfig con = new SiteConfig();
            string json = string.Empty;
            try
            {
                string path = Server.MapPath(xmlPath);
                if (System.IO.File.Exists(path))
                {
                    siteConfigMgr = new SiteConfigMgr(path);
                    configs = siteConfigMgr.Query();
                }
                foreach (SiteConfig site in configs)
                {
                    if (site.Name == "DoMain_Name")
                    {
                        con = site;
                        break;
                    }
                }
                UsersListQuery query = new UsersListQuery();

                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量

                //todo:分页汇出会员信息，由于不能确定是按分页汇出还是汇出全部会员信息，暂且保留汇出全部会员信息
                //start = query.Start;
                //limit = query.Limit;
                if (!string.IsNullOrEmpty(Request.Params["serchs"]))
                {
                    query.serchtype = Request.Params["serchs"];
                }
                if (!string.IsNullOrEmpty(Request.Params["timestart"]))
                {

                    query.serchstart = Convert.ToDateTime(Request.Params["timestart"]);
                    query.serchstart = Convert.ToDateTime(query.serchstart.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if (!string.IsNullOrEmpty(Request.Params["timeend"]))
                {
                    query.serchend = Convert.ToDateTime(Request.Params["timeend"]);
                    query.serchend = Convert.ToDateTime(query.serchend.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                query.content = Request.Params["serchcontent"];
                query.types = Request.Params["bonus_type"];
                query.checks = Request.Params["checkbox1"];
                if (!string.IsNullOrEmpty(Request.Params["relation_id"]))//待回覆
                {
                    query.user_id = Convert.ToUInt32(Request.Params["relation_id"]);
                }
                _uslmpgr = new UsersListMgr(mySqlConnectionString);
                _paraMgr = new ParameterMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _uslmpgr.Query(query, out totalCount);
                Parametersrc pa = new Parametersrc();
                foreach (var item in stores)
                {
                    string[] url = con.Value.Split('/');
                    item.user_url = "http://" + url[0] + "/ecservice_jump.php";//?uid=" + item.user_id;
                    if (Convert.ToBoolean(Request.Params["isSecret"]))
                    {
                        if (!string.IsNullOrEmpty(item.user_name))
                        {
                            item.user_name = item.user_name.Substring(0, 1) + "**";
                        }
                        item.user_email = item.user_email.Split('@')[0] + "@***";
                        if (!string.IsNullOrEmpty(item.user_mobile))
                        {
                            if (item.user_phone.ToString().Length > 3)
                            {
                                item.user_phone = item.user_phone.Substring(0, 3) + "***";
                            }
                            else
                            {
                                item.user_phone = item.user_phone + "***";
                            }
                        }
                        if (!string.IsNullOrEmpty(item.user_mobile))
                        {
                            if (item.user_mobile.ToString().Length > 3)
                            {
                                item.user_mobile = item.user_mobile.Substring(0, 3) + "***";
                            }
                            else
                            {
                                item.user_mobile = item.user_mobile + "***";
                            }
                        }
                        if (item.user_address.ToString().Length > 3)
                        {
                            item.user_address = item.user_address.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.user_address = item.user_address + "***";
                        }
                    }
                    //獲取時間
                    item.reg_date = CommonFunction.GetNetTime(item.user_reg_date);
                    item.sfirst_time = CommonFunction.GetNetTime(item.first_time);
                    item.slast_time = CommonFunction.GetNetTime(item.last_time);
                    item.sbe4_last_time = CommonFunction.GetNetTime(item.be4_last_time);

                    pa = _paraMgr.QueryUsed(new Parametersrc { Used = 1, ParameterCode = item.user_level.ToString(), ParameterType = "UserLevel" }).FirstOrDefault();
                    if (pa != null)
                    {
                        item.userLevel = pa.parameterName;
                    }
                    #region 購物金欄位修改 add by yafeng0715j 20150924
                    BonusMasterMgr bmMgr = new BonusMasterMgr(mySqlConnectionString);
                    BonusMasterQuery bmQuery = new BonusMasterQuery();
                    bmQuery.user_id = item.user_id;
                    bmQuery.bonus_type = 1;
                    DataTable table = bmMgr.GetBonusMasterList(bmQuery);
                    uint master_total = 0;
                    int master_balance = 0;
                    if (table.Rows[0][0].ToString() != "")
                    {
                        master_total = Convert.ToUInt32(table.Rows[0][0]);
                        master_balance = Convert.ToInt32(table.Rows[0][1]);
                    }
                    item.bonus_type = 1;
                    item.bonus_typename = string.Format("購物金(剩餘{0}/總{1})", master_balance, master_total);

                    master_total = 0;
                    master_balance = 0;
                    bmQuery.bonus_type = 2;
                    table = bmMgr.GetBonusMasterList(bmQuery);
                    if (table.Rows[0][0].ToString() != "")
                    {
                        master_total = Convert.ToUInt32(table.Rows[0][0]);
                        master_balance = Convert.ToInt32(table.Rows[0][1]);
                    }
                    item.bonus_type1 = 2;
                    item.bonus_typenamequan = string.Format("抵用券(剩餘{0}/總{1})", master_balance, master_total);
                    #endregion

                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",start:" + query.Start + ",limit:" + query.Limit + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據

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
        public HttpResponseBase GetUserLife()
        {

            string json = string.Empty;
            try
            {

                uint user_id = 0;
                if (!string.IsNullOrEmpty(Request.Params["user_id"]))
                {
                    user_id = Convert.ToUInt32(Request.Params["user_id"]);
                }
                UserLifeMgr _userLifeMgr = new UserLifeMgr(mySqlConnectionString);
                List<UserLife> stores = _userLifeMgr.GetUserLife(user_id);
                UserLifeCustom model = new UserLifeCustom();
                if (stores.Count > 0)
                {
                    model.user_id = stores.FirstOrDefault().user_id;
                    var alist = stores.Find(m => m.info_type == "cancel_edm_time");
                    var blist = stores.Find(m => m.info_type == "cancel_info_time");
                    var clist = stores.Find(m => m.info_type == "disable_time");
                    var dlist = stores.Find(m => m.info_type == "user_marriage");
                    var elist = stores.Find(m => m.info_type == "child_num");
                    var flist = stores.Find(m => m.info_type == "vegetarian_type");
                    var glist = stores.Find(m => m.info_type == "like_fivespice");
                    var hlist = stores.Find(m => m.info_type == "like_contact");
                    var ilist = stores.Find(m => m.info_type == "like_time");
                    var jlist = stores.Find(m => m.info_type == "work_type");
                    var klist = stores.Find(m => m.info_type == "user_educated");
                    var llist = stores.Find(m => m.info_type == "user_salary");
                    var mlist = stores.Find(m => m.info_type == "user_religion");
                    var nlist = stores.Find(m => m.info_type == "user_constellation");

                    if (alist != null)
                    {
                        model.cancel_edm_date = CommonFunction.GetNetTime(Convert.ToInt32(alist.info_code)).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    if (blist != null)
                    {
                        model.cancel_info_date = CommonFunction.GetNetTime(Convert.ToInt32(blist.info_code)).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    if (clist != null)
                    {
                        model.disable_date = CommonFunction.GetNetTime(Convert.ToInt32(clist.info_code)).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    if (dlist != null)
                    {
                        model.user_marriage = Convert.ToInt32(dlist.info_code);
                    }
                    if (elist != null)
                    {
                        model.child_num = Convert.ToInt32(elist.info_code);
                    }
                    if (flist != null)
                    {
                        model.vegetarian_type = Convert.ToInt32(flist.info_code);
                    }
                    if (glist != null)
                    {
                        model.like_fivespice = Convert.ToInt32(glist.info_code);
                    }
                    if (hlist != null)
                    {
                        model.like_contact = hlist.info_code;
                    }
                    if (ilist != null)
                    {
                        model.like_time = Convert.ToInt32(ilist.info_code);
                    }
                    if (jlist != null)
                    {
                        model.work_type = Convert.ToInt32(jlist.info_code);
                    }
                    if (klist != null)
                    {
                        model.user_educated = Convert.ToInt32(klist.info_code);
                    }
                    if (llist != null)
                    {
                        model.user_salary = Convert.ToInt32(llist.info_code);
                    }
                    if (mlist != null)
                    {
                        model.user_religion = Convert.ToInt32(mlist.info_code);
                    }
                    if (nlist != null)
                    {
                        model.user_constellation = Convert.ToInt32(nlist.info_code);
                    }
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                json = "{success:true,data:" + JsonConvert.SerializeObject(model, Formatting.Indented, timeConverter) + "}";//返回json數據

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,error:\"" + BLL.gigade.Common.CommonFunction.MySqlException(ex) + "\"}";

            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 會員管理 會員列表編輯+HttpResponseBase SaveUsersList()
        public HttpResponseBase SaveUsersList()
        {
            string json = string.Empty;
            try
            {
                #region 獲取會員基本信息
                //獲取會員基本信息
                UsersListQuery user = new UsersListQuery();
                if (!string.IsNullOrEmpty(Request.Params["user_id"]))
                {
                    user.user_id = Convert.ToUInt32(Request.Params["user_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["user_name"]))
                {
                    user.user_name = Request.Params["user_name"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["user_gender"]))
                {
                    user.user_gender = Convert.ToUInt32(Request.Params["user_gender"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["user_phone"]))
                {
                    user.user_phone = Request.Params["user_phone"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["user_mobile"]))
                {
                    user.user_mobile = Request.Params["user_mobile"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["my_birthday"]))
                {
                    DateTime birth = Convert.ToDateTime(Request.Params["my_birthday"].ToString());
                    user.user_birthday_year = Convert.ToUInt32(birth.Year);
                    user.user_birthday_month = Convert.ToUInt32(birth.Month);
                    user.user_birthday_day = Convert.ToUInt32(birth.Day);
                }
                else
                {
                    user.user_birthday_year = 1970;
                    user.user_birthday_month = 0;
                    user.user_birthday_day = 0;
                }
                if (user.user_id == 0 && !string.IsNullOrEmpty(Request.Params["user_password_add"]))
                {
                    HashEncrypt hmd5 = new HashEncrypt();
                    user.user_password = hmd5.SHA256Encrypt(Request.Params["user_password_add"].Trim());
                }
                else if (!string.IsNullOrEmpty(Request.Params["user_password_edit"]))
                {
                    HashEncrypt hmd5 = new HashEncrypt();
                    user.user_password = hmd5.SHA256Encrypt(Request.Params["user_password_edit"].Trim());
                }
                if (Request.Params["send_sms_ad"].ToString() == "on")
                {
                    user.send_sms_ad = true;
                }
                else
                {
                    user.send_sms_ad = false;
                }
                if (!string.IsNullOrEmpty(Request.Params["admNote"]))
                {
                    user.adm_note = Request.Params["admNote"].ToString();
                }
                user.user_zip = Convert.ToUInt32(Request.Params["user_zip"]);
                user.user_address = Request.Params["user_address"].ToString();
                if (!string.IsNullOrEmpty(Request.Params["paper_invoice"]))
                {
                    if (Request.Params["paper_invoice"].ToString() == "on")
                    {
                        user.paper_invoice = true;
                    }
                    else
                    {
                        user.paper_invoice = false;
                    }
                }
                #endregion
                #region 獲取會員生活屬性
                List<UserLife> userInfoList = new List<UserLife>();
                UserLife uModel = new UserLife();
                uModel.user_id = user.user_id;
                uModel.kdate = (uint)CommonFunction.GetPHPTime();
                uModel.kuser = (Session["caller"] as Caller).user_id;
                user.update_user = uModel.kuser;
                if (Request.Params["user_marriage"] == "1")
                {
                    UserLife model = new UserLife();
                    model.user_id = uModel.user_id;
                    model.kdate = uModel.kdate;
                    model.kuser = uModel.kuser;
                    model.info_type = "user_marriage";
                    model.info_name = "是否結婚";
                    model.info_code = "1";
                    userInfoList.Add(model);

                }
                if (!string.IsNullOrEmpty(Request.Params["child_num"]) && Convert.ToInt32(Request.Params["child_num"]) != 0)
                {
                    UserLife model = new UserLife();
                    model.user_id = uModel.user_id;
                    model.kdate = uModel.kdate;
                    model.kuser = uModel.kuser;
                    model.info_type = "child_num";
                    model.info_name = "孩子個數";
                    model.info_code = Request.Params["child_num"];
                    userInfoList.Add(model);
                }
                if (!string.IsNullOrEmpty(Request.Params["vegetarian_type"]) && Convert.ToInt32(Request.Params["vegetarian_type"]) != 0)
                {
                    UserLife model = new UserLife();
                    model.user_id = uModel.user_id;
                    model.kdate = uModel.kdate;
                    model.kuser = uModel.kuser;
                    model.info_type = "vegetarian_type";
                    model.info_name = "是否吃素";
                    model.info_code = Request.Params["vegetarian_type"];
                    userInfoList.Add(model);
                }
                if (Request.Params["like_fivespice"] == "on")
                {
                    UserLife model = new UserLife();
                    model.user_id = uModel.user_id;
                    model.kdate = uModel.kdate;
                    model.kuser = uModel.kuser;
                    model.info_type = "like_fivespice";
                    model.info_name = "是否吃五辛";
                    model.info_code = "1";
                    userInfoList.Add(model);
                }
                string contact = string.Empty;
                if (Request.Params["contact1"] == "on")
                {
                    contact += "1,";

                }
                if (Request.Params["contact2"] == "on")
                {
                    contact += "2,";

                }
                if (Request.Params["contact3"] == "on")
                {
                    contact += "3,";

                }
                contact = contact.TrimEnd(',');
                if (!string.IsNullOrEmpty(contact))
                {
                    UserLife model = new UserLife();
                    model.user_id = uModel.user_id;
                    model.kdate = uModel.kdate;
                    model.kuser = uModel.kuser;
                    model.info_type = "like_contact";
                    model.info_name = "方便聯繫方式";
                    model.info_code = contact;
                    userInfoList.Add(model);
                }
                if (!string.IsNullOrEmpty(Request.Params["like_time"]) && Convert.ToInt32(Request.Params["like_time"]) != 0)
                {
                    UserLife model = new UserLife();
                    model.user_id = uModel.user_id;
                    model.kdate = uModel.kdate;
                    model.kuser = uModel.kuser;
                    model.info_type = "like_time";
                    model.info_name = "方便聯繫時間";
                    model.info_code = Request.Params["like_time"];
                    userInfoList.Add(model);
                }
                if (!string.IsNullOrEmpty(Request.Params["user_salary"]) && Convert.ToInt32(Request.Params["user_salary"]) != 0)
                {
                    UserLife model = new UserLife();
                    model.user_id = uModel.user_id;
                    model.kdate = uModel.kdate;
                    model.kuser = uModel.kuser;
                    model.info_type = "user_salary";
                    model.info_name = "年薪";
                    model.info_code = Request.Params["user_salary"];
                    userInfoList.Add(model);
                }
                if (!string.IsNullOrEmpty(Request.Params["work_type"]) && Convert.ToInt32(Request.Params["work_type"]) != 0)
                {
                    UserLife model = new UserLife();
                    model.user_id = uModel.user_id;
                    model.kdate = uModel.kdate;
                    model.kuser = uModel.kuser;
                    model.info_type = "work_type";
                    model.info_name = "職業";
                    model.info_code = Request.Params["work_type"];
                    userInfoList.Add(model);
                }
                if (!string.IsNullOrEmpty(Request.Params["user_educated"]) && Convert.ToInt32(Request.Params["user_educated"]) != 0)
                {
                    UserLife model = new UserLife();
                    model.user_id = uModel.user_id;
                    model.kdate = uModel.kdate;
                    model.kuser = uModel.kuser;
                    model.info_type = "user_educated";
                    model.info_name = "教育";
                    model.info_code = Request.Params["user_educated"];
                    userInfoList.Add(model);
                }
                if (!string.IsNullOrEmpty(Request.Params["user_religion"]) && Convert.ToInt32(Request.Params["user_religion"]) != 0)
                {
                    UserLife model = new UserLife();
                    model.user_id = uModel.user_id;
                    model.kdate = uModel.kdate;
                    model.kuser = uModel.kuser;
                    model.info_type = "user_religion";
                    model.info_name = "宗教信仰";
                    model.info_code = Request.Params["user_religion"];
                    userInfoList.Add(model);
                }
                if (!string.IsNullOrEmpty(Request.Params["user_constellation"]) && Convert.ToInt32(Request.Params["user_constellation"]) != 0)
                {
                    UserLife model = new UserLife();
                    model.user_id = uModel.user_id;
                    model.kdate = uModel.kdate;
                    model.kuser = uModel.kuser;
                    model.info_type = "user_constellation";
                    model.info_name = "星座";
                    model.info_code = Request.Params["user_constellation"];
                    userInfoList.Add(model);
                }
                #endregion
                _uslmpgr = new UsersListMgr(mySqlConnectionString);
                if (_uslmpgr.SaveUserList(user, userInfoList))
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
                json = "{success:false,error:\"" + BLL.gigade.Common.CommonFunction.MySqlException(ex) + "\"}";

            }
            this.Response.Clear();
            this.Response.Write(json.ToString());
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 會員管理 會員列表->購物金查詢+HttpResponseBase BonusSearchList()
        public HttpResponseBase BonusTypeList()
        {
            DataTable table = new DataTable();
            string json = string.Empty;
            try
            {
                BonusTypeMgr bonusTypeMgr = new BonusTypeMgr(mySqlConnectionString);
                table = bonusTypeMgr.GetBonusTypeList();
                DataRow row = table.NewRow();
                row[0] = "0";
                row[1] = "全部";
                table.Rows.InsertAt(row, 0);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                json = "{data:" + JsonConvert.SerializeObject(table, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        public HttpResponseBase BonusSearchList()
        {
            List<BonusMasterQuery> stores = new List<BonusMasterQuery>();

            string json = string.Empty;
            try
            {
                BonusMasterQuery query = new BonusMasterQuery();
                UInt32 uint32 = 0;
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                if (UInt32.TryParse(Request.Params["uid"], out uint32))
                {
                    query.user_id = uint32;
                }
                bool status = true;
                if (bool.TryParse(Request.Params["use"], out status))
                {
                    query.use = status;
                }
                if (bool.TryParse(Request.Params["using"], out status))
                {
                    query.useing = status;
                }
                if (bool.TryParse(Request.Params["used"], out status))
                {
                    query.used = status;
                }
                if (bool.TryParse(Request.Params["usings"], out status))
                {
                    query.useings = status;
                }
                if (bool.TryParse(Request.Params["useds"], out status))
                {
                    query.useds = status;
                }
                if (!string.IsNullOrEmpty(Request.Params["userNameMail"]))
                {
                    query.user_name = Request.Params["userNameMail"];
                    query.user_email = query.user_name;
                }
                DateTime dt;
                if (DateTime.TryParse(Request.Params["timestart"], out dt))
                {
                    query.smaster_start =Convert.ToDateTime(dt.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if (DateTime.TryParse(Request.Params["timeend"], out dt))
                {
                    query.smaster_end = Convert.ToDateTime(dt.ToString("yyyy-MM-dd HH:mm:ss")); ;
                }

                if (UInt32.TryParse(Request.Params["bonus_type"], out uint32))
                {
                    query.bonus_type = uint32;
                }
                if (UInt32.TryParse(Request.Params["type_id"], out uint32))
                {
                    query.type_id = uint32;
                }
                _uslmpgr = new UsersListMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _uslmpgr.bQuery(query, out totalCount);
                foreach (var item in stores)
                {
                    //獲取時間
                    item.smaster_start = CommonFunction.GetNetTime(item.master_start);
                    item.smaster_end = CommonFunction.GetNetTime(item.master_end);
                    item.smaster_createtime = CommonFunction.GetNetTime(item.master_createdate);
                    item.now_time = Convert.ToInt32(CommonFunction.GetPHPTime());
                    if (!string.IsNullOrEmpty(item.user_name))
                    {
                        item.user_name = item.user_name.Substring(0, 1) + "**";
                    }
                    item.user_email = item.user_email.Split('@')[0] + "@***";
                }
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

        #region 會員管理 會員列表->發送郵件+bool sendemail()
        public HttpResponseBase sendemail()
        {
            string path = Server.MapPath(xmlPath);
            SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
            SiteConfig Mail_From = _siteConfigMgr.GetConfigByName("Mail_From");
            SiteConfig Mail_Host = _siteConfigMgr.GetConfigByName("Mail_Host");
            SiteConfig Mail_Port = _siteConfigMgr.GetConfigByName("Mail_Port");
            SiteConfig Mail_UserName = _siteConfigMgr.GetConfigByName("Mail_UserName");
            SiteConfig Mail_UserPasswd = _siteConfigMgr.GetConfigByName("Mail_UserPasswd");
            string EmailFrom = Mail_From.Value;//發件人郵箱
            string SmtpHost = Mail_Host.Value;//smtp服务器
            string SmtpPort = Mail_Port.Value;//smtp服务器端口
            string EmailUserName = Mail_UserName.Value;//郵箱登陸名
            string EmailPassWord = Mail_UserPasswd.Value;//郵箱登陸密碼
            string json = string.Empty;//返回json數據
            int id = Convert.ToInt32(Request.Params["id"]);
            _uslmpgr = new UsersListMgr(mySqlConnectionString);

            BLL.gigade.Model.Custom.Users stores = new BLL.gigade.Model.Custom.Users();
            try
            {
                BLL.gigade.Model.Custom.Users query = new BLL.gigade.Model.Custom.Users();
                query.user_id = Convert.ToUInt32(id);
                stores = _uslmpgr.getModel(id);
                if (stores.user_actkey == "")
                {
                    stores.user_actkey = CommonFunction.Generate_Rand_String(8);
                }
                else
                {
                    stores.user_actkey = stores.user_actkey;
                }
                _uslmpgr.UpdateUser(stores); //更新user數據庫

                BLL.gigade.Model.Custom.Users urs = new BLL.gigade.Model.Custom.Users();
                urs = _uslmpgr.getModel(id);
                //發送郵件
                DateTime dt = DateTime.Now;//寄送時間
                // stores.user_id;會員編號
                //store.user_name;會員姓名
                //store.user_email;寄送地址
                //store.actkey 認證編號

                // bool state = sendemail("shiwei0620j@hz-mail.eamc.com.tw", "12345", "Gigade郵件", "www.gigade100.com/member_join_ok.php?uid=" + stores.user_id + "&act_key=" + stores.user_actkey, urs.user_email);
                FileStream fs = new FileStream(Server.MapPath("../ImportUserIOExcel/MemberValidate.html"), FileMode.OpenOrCreate, FileAccess.Read);
                StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                string strTemp = sr.ReadToEnd();
                sr.Close();
                fs.Close();
                strTemp = strTemp.Replace("{{$content$}}", "<a " + "href=" + "\"" + "www.gigade100.com/member_join_ok.php?uid=" + stores.user_id + "&act_key=" + stores.user_actkey + "\"" + ">" + "www.gigade100.com/member_join_ok.php?uid=" + stores.user_id + "&act_key=" + stores.user_actkey + "</a>");
                bool state = sendmail(EmailFrom, FromName, urs.user_email, urs.user_name, EmailTile, strTemp, "", SmtpHost, Convert.ToInt32(SmtpPort), EmailUserName, EmailPassWord);
                string time = CommonFunction.DateTimeToString(DateTime.Now);
                if (state == true)
                {
                    json = "{success:true,sendtime:'" + time + "',userid:'" + stores.user_id + "',username:'" + stores.user_name + "',sendto:'" + stores.user_email + "',authenid:'" + stores.user_actkey + "'}";
                }
                else
                {
                    json = "{success:false,sendtime:'" + time + "',userid:'" + stores.user_id + "',username:'" + stores.user_name + "',sendto:'" + stores.user_email + "',authenid:'" + stores.user_actkey + "'}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            this.Response.Clear();
            this.Response.Write(json.ToString());
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 會員管理 會員列表->修改購物金信息+HttpResponseBase updateuser_master()
        public HttpResponseBase updateuser_master()
        {
            string json = string.Empty;
            BonusMasterQuery bmq = new BonusMasterQuery();
            try
            {
                _uslmpgr = new UsersListMgr(mySqlConnectionString);
                bmq.master_id = Convert.ToUInt32(Request.Params["master_id"]);
                bmq.user_id = Convert.ToUInt32(Request.Params["user_id"]);
                bmq.master_total = Convert.ToUInt32(Request.Params["master_total"]);
                bmq.master_note = Convert.ToString(Request.Params["master_note"]);
                int already_use_bonus = 0;
                if (!string.IsNullOrEmpty(Request.Params["already_use_bonus"]))
                {
                    already_use_bonus = int.Parse(Request.Params["already_use_bonus"]);
                }


                bmq.master_balance = int.Parse(bmq.master_total.ToString()) - already_use_bonus;
                bmq.master_end = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["smaster_end"]));
                bmq.master_start = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["smaster_start"]));
                _uslmpgr.updateuser_master(bmq);
                json = "{success:true}";//返回json數據
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

        #region 會員管理 會員列表->匯出檔案+ HttpResponseBase UserslistExport()
        public HttpResponseBase UserslistExport()
        {
            List<UsersListQuery> stores = new List<UsersListQuery>();
            UsersListQuery query = new UsersListQuery();

            //todo:分页汇出会员信息，由于不能确定是按分页汇出还是汇出全部会员信息，暂且保留汇出全部会员信息

            //query.Start = start;//用於分頁的變量
            //query.Limit = limit;//用於分頁的變量
            if (!string.IsNullOrEmpty(Request.Params["serchs"]))
            {
                query.serchtype = Request.Params["serchs"];
            }
            if (!string.IsNullOrEmpty(Request.Params["timestart"]))
            {

                query.serchstart = Convert.ToDateTime(Request.Params["timestart"]);
                query.serchstart = Convert.ToDateTime(query.serchstart.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (!string.IsNullOrEmpty(Request.Params["timestart"]))
            {
                query.serchend = Convert.ToDateTime(Request.Params["timeend"]);
                query.serchend = Convert.ToDateTime(query.serchend.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            query.content = Request.Params["serchcontent"];
            query.types = Request.Params["bonus_type"];
            query.checks = Request.Params["checkbox1"];


            string json = string.Empty;
            if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
            }
            DataTable dtHZ = new DataTable();

            string newExcelName = string.Empty;
            dtHZ.Columns.Add("編號", typeof(String));
            //dtHZ.Columns.Add("信箱", typeof(String));
            dtHZ.Columns.Add("姓名", typeof(String));
            dtHZ.Columns.Add("性別", typeof(String));
            dtHZ.Columns.Add("生日", typeof(String));
            dtHZ.Columns.Add("等級", typeof(String));
            //dtHZ.Columns.Add("行動電話", typeof(String));
            // dtHZ.Columns.Add("聯絡電話", typeof(String));
            // dtHZ.Columns.Add("郵遞區號", typeof(String));
            //dtHZ.Columns.Add("地址", typeof(String));
            dtHZ.Columns.Add("註冊日期", typeof(String));

            dtHZ.Columns.Add("來源", typeof(String));
            dtHZ.Columns.Add("公司", typeof(String));
            dtHZ.Columns.Add("站內連結群組", typeof(String));
            dtHZ.Columns.Add("站內連結", typeof(String));
            dtHZ.Columns.Add("購物金發放", typeof(String));

            dtHZ.Columns.Add("購物金使用", typeof(String));
            dtHZ.Columns.Add("首購時間", typeof(String));
            dtHZ.Columns.Add("上次購買時間", typeof(String));
            dtHZ.Columns.Add("上上次購買時間", typeof(String));
            dtHZ.Columns.Add("初次登入IP", typeof(String));
            try
            {
                _uslmpgr = new UsersListMgr(mySqlConnectionString);
                stores = _uslmpgr.Export(query);
                string userId = " ";/*获取会员的ID成为字符串，进入数据库查询*/

                foreach (var item in stores)
                {
                    userId += item.user_id + ",";
                    //獲取時間
                    item.suser_reg_date = CommonFunction.GetNetTime(item.user_reg_date);
                    item.sfirst_time = CommonFunction.GetNetTime(item.first_time);
                    item.slast_time = CommonFunction.GetNetTime(item.last_time);
                    item.sbe4_last_time = CommonFunction.GetNetTime(item.be4_last_time);
                }
                userId = userId.Remove(userId.Length - 1);

                DataTable dtBonusTotal = _uslmpgr.GetBonusTotal(query.serchstart, query.serchend, userId);/*购物金发放*/
                DataTable dtRecordTotal = _uslmpgr.GetRecordTotal(query.serchstart, query.serchend, userId);/*购物金使用*/
                DataTable dtZipCode = _uslmpgr.GetZipCode();
                foreach (var item in stores)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = item.user_id;
                    //dr[1] = item.user_email;
                    dr[1] = ((char)9).ToString() + item.user_name.Replace(',', '，');
                    if (item.user_gender == 0)
                    {
                        dr[2] = "小姐";
                    }
                    else if (item.user_gender == 1)
                    {
                        dr[2] = "先生";
                    }
                    dr[3] = item.birthday;//生日
                    dr[4] = item.ml_code;
                    dr[5] = item.suser_reg_date;

                    dr[6] = ((char)9).ToString() + item.user_source.Replace(',', '，');//來源
                    dr[7] = ((char)9).ToString() + item.user_company_id.Replace(',', '，');//外網來源
                    dr[8] = item.group_name;//站内链接群组
                    dr[9] = item.redirect_name;//站内链接群组
                    dr[10] = 0;//item.master_total;//购物金发放
                    dr[11] = 0;//item.master_balance;//购物金使用
                    #region 把购物金的信息导入dr表中;
                    DataRow[] rBonusTotal = dtBonusTotal.Select("user_id='" + item.user_id + "'");
                    if (rBonusTotal.Length > 0)
                    {
                        dr[10] = rBonusTotal[0]["total_in"];//購物金發放
                    }
                    DataRow[] rRecordTotal = dtRecordTotal.Select("user_id='" + item.user_id + "'");
                    if (rRecordTotal.Length > 0)
                    {
                        dr[11] = rRecordTotal[0]["total_out"];//購物金使用
                    }
                    #endregion
                    dr[12] = CommonFunction.DateTimeToString(item.sfirst_time) == "1970-01-01 08:00:00" ? "N/A" : CommonFunction.DateTimeToString(item.sfirst_time);//首購時間
                    dr[13] = CommonFunction.DateTimeToString(item.slast_time) == "1970-01-01 08:00:00" ? "N/A" : CommonFunction.DateTimeToString(item.slast_time);//上次購買時間
                    dr[14] = CommonFunction.DateTimeToString(item.sbe4_last_time) == "1970-01-01 08:00:00" ? "N/A" : CommonFunction.DateTimeToString(item.sbe4_last_time);//上上此購買時間
                    dr[15] = item.master_ipfrom;//初次登陸IP
                    dtHZ.Rows.Add(dr);
                }
                string[] colname = new string[dtHZ.Columns.Count];
                string filename = "user_list_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
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


                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,fileName:\'" + filename + "\'}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "false";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }
        #endregion

        //#region 會員管理 會員列表->修改用戶email+ HttpResponseBase UpdateEmail()
        //public HttpResponseBase UpdateEmail()
        //{
        //    string json = string.Empty;
        //    try
        //    {
        //        BLL.gigade.Model.Users query = new BLL.gigade.Model.Users();
        //        if (!string.IsNullOrEmpty(Request.Params["userid"]))
        //        {
        //            query.user_id = (ulong)Convert.ToInt32(Request.Params["userid"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["useremail"]))
        //        {
        //            query.user_email = Request.Params["useremail"];
        //        }
        //        _usmpgr = new UsersMgr(mySqlConnectionString);
        //        int i = _usmpgr.UpdateEmail(query);
        //        if (i > 0)
        //        {
        //            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
        //            timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        //            json = "{success:true}";
        //        }
        //        else
        //        {
        //            json = "{success:false}";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "false";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json);
        //    this.Response.End();
        //    return this.Response;

        //}
        //#endregion
        #endregion
        #region C#发送邮件函数
        /// <summary>
        /// C#发送邮件函数
        /// </summary>
        /// <param name="from">发送者邮箱</param>
        /// <param name="fromer">发送人</param>
        /// <param name="sto">接受者邮箱</param>
        /// <param name="toer">收件人</param>
        /// <param name="Subject">主题</param>
        /// <param name="Body">内容</param>
        /// <param name="file">附件地址</param>
        /// <param name="SMTPHost">smtp服务器</param>
        /// <param name="SMTPuser">邮箱</param>
        /// <param name="SMTPpass">密码</param>
        /// <returns></returns>
        public bool sendmail(string sfrom, string sfromer, string sto, string stoer, string sSubject, string sBody, string sfile, string sSMTPHost, int sSMTPPort, string sSMTPuser, string sSMTPpass)
        {
            ////设置from和to地址
            MailAddress from = new MailAddress(sfrom, sfromer);
            MailAddress to = new MailAddress(sto, stoer);

            ////创建一个MailMessage对象
            MailMessage oMail = new MailMessage(from, to);
            //// 添加附件
            if (sfile != "")
            {
                oMail.Attachments.Add(new Attachment(sfile));
            }
            ////邮件标题
            oMail.Subject = sSubject;
            ////邮件内容
            //oMail.Body = sBody;
            AlternateView htmlBody = AlternateView.CreateAlternateViewFromString(sBody, null, "text/html");
            oMail.AlternateViews.Add(htmlBody);
            ////邮件格式
            oMail.IsBodyHtml = true;
            ////邮件采用的编码
            oMail.BodyEncoding = System.Text.Encoding.GetEncoding("GB2312");
            ////设置邮件的优先级为高
            oMail.Priority = MailPriority.Normal;
            ////发送邮件
            SmtpClient client = new SmtpClient();
            ////client.UseDefaultCredentials = false;
            client.Host = sSMTPHost;
            client.Port = sSMTPPort;
            client.Credentials = new NetworkCredential(sSMTPuser, sSMTPpass);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            try
            {
                client.Send(oMail);
                return true;
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return false;
            }
            finally
            {
                ////释放资源
                oMail.Dispose();
            }

        }
        #endregion
        #region 會員查詢禁用button
        [CustomHandleError]
        public HttpResponseBase UserCancel()
        {
            string json = string.Empty;
            _uslmpgr = new UsersListMgr(mySqlConnectionString);
            UsersListQuery u = new UsersListQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["rowID"]))
                {
                    u.user_id = uint.Parse(Request.Params["rowID"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["email"]))
                {
                    u.user_email = Request.Params["email"];
                }
                u.update_user = (Session["caller"] as Caller).user_id;
                if (_uslmpgr.UserCancel(u) > 0)
                {
                    json = "{success:true}";
                }
                else
                {
                    json = "{success:false}";
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

        #endregion
    }
}
