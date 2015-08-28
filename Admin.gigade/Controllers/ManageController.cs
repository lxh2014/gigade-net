using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using BLL.gigade.Common;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Admin.gigade.Controllers
{
    public class ManageController : Controller
    {
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//郵件服務器的設置
        string FromName = ConfigurationManager.AppSettings["UserName"];//發件人姓名
        string EmailTile = ConfigurationManager.AppSettings["UserEmailTile"];//郵件標題
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IManageUserImplMgr _manageuserMgr;
        private ManageLoginMgr _managelogionMgr;
        private IParametersrcImplMgr _paraMgr;
        HashEncrypt hmd5 = new HashEncrypt();
        //
        // GET: /Manage/

        #region View
        public ActionResult ManageUser()
        {
            return View();
        }
        public ActionResult ManageLogin()
        {
            return View();
        }

        #endregion

        #region Manage_User

        #region 賬號列表頁
        [CustomHandleError]
        public HttpResponseBase GetManageUserList()
        {
            List<ManageUserQuery> store = new List<ManageUserQuery>();
            string json = string.Empty;
            int totalCount = 0;
            try
            {
                _manageuserMgr = new ManageUserMgr(mySqlConnectionString);
                ManageUserQuery query = new ManageUserQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["search_status"]))
                {
                    query.search_status = Request.Params["search_status"];
                }
                if (!string.IsNullOrEmpty(Request.Params["s_mail"]))
                {
                    query.user_email = Request.Params["s_mail"];
                }
                if (!string.IsNullOrEmpty(Request.Params["s_name"]))
                {
                    query.user_username = Request.Params["s_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["login_sum"]))
                {
                    query.login_sum = Request.Params["login_sum"];
                }
                if (!string.IsNullOrEmpty(Request.Params["relation_id"]))//待回覆
                {
                    query.userid = Request.Params["relation_id"];
                }
                store = _manageuserMgr.GetManageUserList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                foreach (var item in store)
                {
                    if (Convert.ToBoolean(Request.Params["isSecret"]))
                    {
                        item.user_email = item.user_email.Split('@')[0] + "@***";
                    }
                    item.lastlogin = CommonFunction.GetNetTime(item.user_last_login);
                    item.creattime = CommonFunction.GetNetTime(item.user_createdate);
                    item.updtime = CommonFunction.GetNetTime(item.user_updatedate);
                }
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 賬號新增/編輯
        public HttpResponseBase SaveManageUser()
        {
            string json = string.Empty;
            DataTable dt = new DataTable();
            bool isupdate = false;
            string password;
            try
            {
                #region 發送email設置
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
                #endregion
                _manageuserMgr = new ManageUserMgr(mySqlConnectionString);
                ManageUserQuery store = new ManageUserQuery();
                ManageUserQuery query = new ManageUserQuery();
                if (!string.IsNullOrEmpty(Request.Params["user_id"]))
                {//如果是編輯獲取該id數據
                    int totalCount = 0;
                    query.IsPage = false;
                    query.user_id = uint.Parse(Request.Params["user_id"]);
                    query.userid = Request.Params["user_id"];
                    query.search_status = "-1";
                    store = _manageuserMgr.GetManageUserList(query, out totalCount).FirstOrDefault();
                    isupdate = true;
                }
                if (!string.IsNullOrEmpty(Request.Params["user_username"]))
                {
                    query.user_username = Request.Params["user_username"];
                }
                if (!string.IsNullOrEmpty(Request.Params["user_email"]))
                {
                    query.user_email = Request.Params["user_email"];
                    if (store != null)
                    {
                        if (store.user_email == query.user_email)
                        {//如果編輯沒有變email就空值
                            query.user_email = string.Empty;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["erp_id"]))
                {
                    query.erp_id = Request.Params["erp_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["user_status"]))
                {
                    query.user_status = uint.Parse(Request.Params["user_status"]);
                }
                else
                {
                    query.user_status = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["manage"]))
                {
                    query.manage = uint.Parse(Request.Params["manage"]);
                }
                else
                {
                    query.manage = 0;
                }
                Random rd = new Random();
                password = CommonFunction.Getserials(8, rd);
                query.user_password = hmd5.SHA256Encrypt(password);
                query.user_lastvisit = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
                query.user_last_login = query.user_lastvisit;
                query.user_createdate = query.user_lastvisit;
                query.user_updatedate = query.user_lastvisit;
                if (_manageuserMgr.CheckEmail(query) > 0 && !string.IsNullOrEmpty(query.user_email))
                {//判斷新增編輯過得email數據庫是否有重複
                    json = "{success:true,msg:2}";
                }
                else
                {
                    if (isupdate)
                    {
                        #region 編輯
                        if (query.user_status == 3)
                        {
                            query.user_delete_email = query.user_email;
                            Random re = new Random();
                            query.user_email = DateTime.Now.ToString("yyyyMMddhhmmss") + hmd5.SHA256Encrypt(CommonFunction.Getdeleteemail(32, re));
                            if (_manageuserMgr.ManageUserUpd(query) > 0)
                            {
                                json = "{success:true,msg:1}";
                            }
                            else
                            {
                                json = "{success:false,msg:4}";
                            }
                        }
                        else
                        {
                            if (_manageuserMgr.ManageUserUpd(query) > 0)
                            {
                                json = "{success:true,msg:1}";
                            }
                            else
                            {
                                json = "{success:false,msg:4}";
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region 新增
                        if (_manageuserMgr.ManageUserAdd(query) > 0)
                        {
                            FileStream fs = new FileStream(Server.MapPath("../ImportUserIOExcel/901.html"), FileMode.OpenOrCreate, FileAccess.Read);
                            StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                            string strTemp = sr.ReadToEnd();
                            sr.Close();
                            fs.Close();
                            _paraMgr = new ParameterMgr(mySqlConnectionString);
                            string linkurl = string.Empty;
                            Parametersrc paModel = _paraMgr.QueryUsed(new Parametersrc { ParameterType = "admin_link_url" }).FirstOrDefault();
                            if (paModel != null)
                            {
                                linkurl = paModel.ParameterCode;
                            }


                            strTemp = strTemp.Replace("{{$s_user_username$}}", query.user_username);
                            strTemp = strTemp.Replace("{{$u_admin_url$}}", linkurl);
                            strTemp = strTemp.Replace("{{$s_email$}}", query.user_email);
                            strTemp = strTemp.Replace("{{$s_password$}}", password);
                            if (CommonFunction.sendmail(EmailFrom, FromName, query.user_email, query.user_name, EmailTile, strTemp, "", SmtpHost, Convert.ToInt32(SmtpPort), EmailUserName, EmailPassWord))
                            {
                                json = "{success:true,msg:1}";
                            }
                            else
                            {
                                json = "{success:true,msg:3}";
                            }
                        }
                        else
                        {
                            json = "{success:false,msg:4}";
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'操作失敗!'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 賬號變更密碼
        public HttpResponseBase Updatepassword()
        {
            string json = string.Empty;
            try
            {
                _manageuserMgr = new ManageUserMgr(mySqlConnectionString);
                ManageUserQuery query = new ManageUserQuery();
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.user_id = uint.Parse(Request.Params["id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["user_password"]))
                {
                    query.user_password = hmd5.SHA256Encrypt(Request.Params["user_password"]);
                }
                query.user_updatedate = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
                if (_manageuserMgr.UpdPassword(query) > 0)
                {
                    json = "{success:true,msg:'修改成功!'}";
                }
                else
                {
                    json = "{success:false,msg:'修改失敗!'}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'操作失敗!'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 賬號啟用
        public HttpResponseBase UpdateStatus()
        {
            string json = string.Empty;
            try
            {
                _manageuserMgr = new ManageUserMgr(mySqlConnectionString);
                ManageUserQuery query = new ManageUserQuery();
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.user_id = uint.Parse(Request.Params["id"]);
                }
                query.user_status = 1;
                query.user_updatedate = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
                if (_manageuserMgr.UpdStatus(query) > 0)
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
                json = "{success:false,msg:'操作失敗!'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        #endregion

        #endregion

        #region Manage_Login 處理
        public HttpResponseBase GetManageLoginList()
        {
            string json = string.Empty;
            int totalCount = 0;
            List<ManageLoginQuery> list = new List<ManageLoginQuery>();
            _managelogionMgr = new ManageLoginMgr(mySqlConnectionString);
            ManageLoginQuery query = new ManageLoginQuery();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
            try
            {

                query.login_start = CommonFunction.GetPHPTime(Request.Params["time_start"]);
                query.login_end = CommonFunction.GetPHPTime(Request.Params["time_end"]);
                if (!String.IsNullOrEmpty(Request.Params["LoginSearch"]))//登錄人
                {
                    query.user_name = Request.Params["LoginSearch"];
                }
                if (!String.IsNullOrEmpty(Request.Params["IPSearch"]))//IP
                {

                    query.login_ipfrom = Request.Params["IPSearch"];
                }

                list = _managelogionMgr.GetManageLoginList(query, out totalCount);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                //listUser是准备转换的对象
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
    }
}
