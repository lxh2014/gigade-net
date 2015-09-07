
#region 文件信息
/* 
* Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：SecretInfoController.cs      
* 摘 要：                                                                               
* 資安密碼管理
* 当前版本：v1.1                                                                 
* 作 者： shuangshuang0420j                                           
* 完成日期：2015/4/3 
* 修改歷史： 
 *     2015/04/28 16:06:04 mengjuan0826j 修改方法 GetUserInfo
*        
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Data;
using BLL.gigade.Common;
using System.Net;

namespace Admin.gigade.Controllers
{
    public class SecretInfoController : Controller
    {
        //
        // GET: /SecretInfo/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private SecretInfoLogMgr _secretLogMgr;
        private SecretAccountSetMgr sasMgr;
        private IManageUserImplMgr _muMgr;
        private UserLoginAttemptsMgr ulaMgr;
        private VipUserGroupMgr _vipUserGroup;
        private IBrowseDataImplMgr _IBrowseDataMgr; 
        private ZipMgr zMgr;

        #region 視圖
        /// <summary>
        /// 
        /// 機敏資料查詢記錄
        /// </summary>
        /// <returns></returns>
        public ActionResult Secret_Log()
        {
            return View();
        }
        /// <summary>
        /// 密碼重置
        /// </summary>
        /// <returns></returns>
        public ActionResult SecretSet()
        {
            return View();
        }
        #endregion

        #region 機敏資料查詢記錄
        [CustomHandleError]
        public HttpResponseBase GetSecretInfoLog()
        {
            List<SecretInfoLog> store = new List<SecretInfoLog>();
            string json = string.Empty;
            int totalCount = 0;
            try
            {
                _secretLogMgr = new SecretInfoLogMgr(mySqlConnectionString);

                SecretInfoLog query = new SecretInfoLog();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");

                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["user_id"]))
                {
                    query.user_id = uint.Parse(Request.Params["user_id"]);
                }
                query.user_email = Request.Params["login_mail"];
                query.ipfrom = Request.Params["login_ipfrom"];
                if (!string.IsNullOrEmpty(Request.Params["start_date"]))
                {
                    query.date_one = Convert.ToDateTime(Request.Params["start_date"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["end"]))
                {
                    query.date_two = Convert.ToDateTime(Request.Params["end"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["sumtotal"]))
                {
                    query.sumtotal = int.Parse(Request.Params["sumtotal"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["is_count"]))
                {
                    int is_count = int.Parse(Request.Params["is_count"]);
                    if (is_count == 1)
                    {
                        if (!string.IsNullOrEmpty(Request.Params["ismail"]))
                        {
                            query.ismail = int.Parse(Request.Params["ismail"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["countClass"]))
                        {
                            query.countClass = int.Parse(Request.Params["countClass"]);
                        }
                    }
                    else
                    {
                        query.ismail = -1;
                        query.countClass = -1;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["type"]))
                {
                    query.type = int.Parse(Request.Params["type"]);
                }


                DataTable DT = _secretLogMgr.GetSecretInfoLog(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(DT, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 獲取用戶查詢記錄
        public HttpResponseBase GetSecretLog()
        {
            string json = string.Empty;
            try
            {
                _secretLogMgr = new SecretInfoLogMgr(mySqlConnectionString);
                SecretInfoLog query = new SecretInfoLog();
                query.user_id = Convert.ToUInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                query.ipfrom = CommonFunction.GetIP4Address(Request.UserHostAddress.ToString());
                List<SecretInfoLog> store = _secretLogMgr.GetSecretInfoLog(query);//first是輸入密碼最近的
                if (store.Count != 0)
                {
                    DateTime dtNow = DateTime.Now.AddMinutes(-5);
                    if (dtNow.CompareTo(Convert.ToDateTime(store[0].input_pwd_date)) >= 0)
                    {
                        json = "{success:true,data:true}";//超出密保時間，需輸入密碼
                    }
                    else
                    {
                        json = "{success:true,data:false}";//未超出密保時間,則不需輸入密碼
                    }
                }
                else
                {
                    json = "{success:true,data:true}";//超出密保時間，需輸入密碼
                }


            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:true}";//異常
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        #endregion

        #region 保存機敏資料查詢記錄
        /// <summary>
        /// 誰在什麼時候通過哪個頁面訪問了哪筆機敏資料
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SaveSecretLog()
        {

            string json = "{success:false,isconti:false,ispower:false,pwd_status:\"" + 0 + "\"}";
            try
            {
                _secretLogMgr = new SecretInfoLogMgr(mySqlConnectionString);

                SecretInfoLog query = new SecretInfoLog();
                //誰
                query.user_id = Convert.ToUInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                query.ipfrom = CommonFunction.GetIP4Address(Request.UserHostAddress.ToString());
                //在哪個時候
                query.createdate = DateTime.Now;
                //訪問了哪個頁面
                if (!string.IsNullOrEmpty(Request.Params["urlRecord"]))
                {
                    query.url = Request.Params["urlRecord"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["secretType"]))
                {
                    query.type = Convert.ToInt32(Request.Params["secretType"].ToString());
                }
                //哪筆機敏資料
                if (!string.IsNullOrEmpty(Request.Params["ralatedId"]))
                {
                    query.related_id = Convert.ToInt32(Request.Params["ralatedId"].ToString());
                }
                sasMgr = new SecretAccountSetMgr(mySqlConnectionString);
                SecretAccountSet querysas = new SecretAccountSet();
                querysas.user_id = query.user_id;
                querysas.ipfrom = query.ipfrom;
                querysas.status = -1;
                List<SecretAccountSet> store = sasMgr.GetSecretSetList(querysas);//獲得用戶的密保信息
                if (store.Count > 0)//該賬號具有機敏權限
                {
                    if ((store[0].secret_count < store[0].secret_limit) && store[0].status == 1)//該賬號查詢次數未達極限
                    {
                        if (_secretLogMgr.InsertSecretInfoLog(query) > 0)//查詢記錄保存成功
                        {
                            store[0].secret_count = store[0].secret_count + 1;
                            store[0].updatedate = DateTime.Now;
                            sasMgr.Update(store[0]);
                            //判斷是否具有權限
                            json = "{success:true,isconti:true,ispower:true,pwd_status:\"" + store[0].pwd_status + "\"}";//正常進行
                        }
                    }
                    else if ((store[0].secret_count >= store[0].secret_limit) && store[0].status == 1)//極限值訪問
                    {
                        store[0].status = 0;
                        store[0].updatedate = DateTime.Now;
                        sasMgr.Update(store[0]);
                        //判斷是否具有權限
                        json = "{success:true,isconti:false,ispower:true,pwd_status:\"" + store[0].pwd_status + "\"}";//已達極限
                    }
                    else if ((store[0].secret_count < store[0].secret_limit) && store[0].status == 0)
                    {//達極限
                        json = "{success:true,isconti:false,ispower:false,pwd_status:\"" + store[0].pwd_status + "\"}";//沒有賬號

                    }
                    else
                    {
                        json = "{success:true,isconti:false,ispower:true,pwd_status:\"" + store[0].pwd_status + "\"}";//已達極限
                    }
                }
                else
                {
                    json = "{success:true,isconti:false,ispower:false,pwd_status:\"" + 0 + "\"}";//沒有賬號
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
        #endregion

        #region 密碼重置功能
        public HttpResponseBase GetSecretSetList()
        {
            string json = string.Empty;
            int totalCount = 0;
            uint result = 0;
            try
            {
                SecretAccountSetQuery query = new SecretAccountSetQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["search_content"]))
                {
                    if (uint.TryParse(Request.Params["search_content"], out result))
                    {
                        query.user_id = result;
                    }
                    else
                    {
                        query.user_username = Request.Params["search_content"];
                    }
                }
                //判斷user_id  和ipfrom是否同時存在該賬號 若存在 則提示不能添加
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.id = int.Parse(Request.Params["id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ipfrom"]))
                {
                    query.ipfrom = Request.Params["ipfrom"];
                }
                if (!string.IsNullOrEmpty(Request.Params["ispage"]))
                {
                    query.IsPage = false;
                }
                sasMgr = new SecretAccountSetMgr(mySqlConnectionString);
                DataTable dt = sasMgr.GetSecretSetList(query, out totalCount);
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
        public HttpResponseBase SaveSecretSet()
        {
            string json = string.Empty;
            SecretAccountSet sas = new SecretAccountSet();
            try
            {
                sasMgr = new SecretAccountSetMgr(mySqlConnectionString);
                SecretAccountSetQuery sasq = new SecretAccountSetQuery();
                sasq.IsPage = false;
                bool issame = false;
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    sas.id = int.Parse(Request.Params["id"]);
                    sasq.id = sas.id;
                }
                SecretAccountSet sasModel = sasMgr.Select(sasq);
                if (!string.IsNullOrEmpty(Request.Params["user_id"]))
                {
                    sas.user_id = uint.Parse(Request.Params["user_id"]);
                }
                string opassword = Request.Params["osecret_password"];
                string npassword = Request.Params["nsecret_password"];
                string password = string.Empty;
                string oldpwd = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["secret_limit"]))
                {
                    sas.secret_limit = Convert.ToInt32(Request.Params["secret_limit"]);
                }

                if (sasModel != null)
                {
                    sas.pwd_status = Convert.ToInt32(sasModel.pwd_status);
                }
                sas.updatedate = sas.createdate;
                //新密碼
                if (!string.IsNullOrEmpty(npassword))
                {
                    HashEncrypt hmd5 = new HashEncrypt();
                    password = hmd5.SHA256Encrypt(npassword);
                    sas.secret_pwd = password;
                    sas.pwd_status = 0;
                }
                if (string.IsNullOrEmpty(Request.Params["reset"]))
                {
                    //舊密碼
                    if (!string.IsNullOrEmpty(opassword))
                    {
                        HashEncrypt hmd5 = new HashEncrypt();
                        oldpwd = hmd5.SHA256Encrypt(opassword);
                    }
                    if (sasModel != null)
                    {
                        if (oldpwd == sasModel.secret_pwd)
                        {
                            issame = true;
                        }
                    }
                    IPAddress ip = new IPAddress(0);
                    if (IPAddress.TryParse(Request.Params["ipfrom"], out ip))
                    {
                        sas.ipfrom = ip.ToString();
                        if (!string.IsNullOrEmpty(Request.Params["id"]))
                        {
                            if (issame || Request.Params["nsecret_password"] == "")
                            {

                                if (sasMgr.Update(sas) > 0)
                                {
                                    json = "{success:true,msg:'修改成功!'}";
                                }
                                else
                                {
                                    json = "{success:false,msg:'修改失敗!'}";
                                }
                            }
                            else
                            {
                                json = "{success:false,msg:'原始密碼輸入錯誤!'}";
                            }
                        }
                        else
                        {
                            sas.secret_count = 0;
                            sas.user_login_attempts = 0;
                            sas.createdate = DateTime.Now;
                            sas.status = 0;
                            sas.pwd_status = 0;
                            if (sasMgr.SelectByUserIP(sas) == null)
                            {
                                if (sasMgr.Insert(sas) > 0)
                                {
                                    json = "{success:true,msg:'保存成功!'}";
                                }
                                else
                                {
                                    json = "{success:false,msg:'保存失敗!'}";
                                }
                            }
                            else
                            {
                                json = "{success:false,msg:'相同的用戶和IP不能重複添加!'}";
                            }
                        }
                    }
                    else
                    {
                        json = "{success:false,msg:'请输入正确的IP地址!'}";
                    }
                }
                else
                {
                    sas.pwd_status = 0;
                    if (sasMgr.Update(sas) > 0)
                    {
                        json = "{success:true}";
                    }
                    else
                    {
                        json = "{success:false}";
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

        public HttpResponseBase GetManagerUser()
        {
            string json = string.Empty;
            ManageUser mu = new ManageUser();
            ManageUserQuery muq = new ManageUserQuery();
            int totalCount = 0;
            List<ManageUser> store = new List<ManageUser>();
            List<ManageUserQuery> storeq = new List<ManageUserQuery>();
            try
            {
                _muMgr = new ManageUserMgr(mySqlConnectionString);
                //判斷輸入密碼是否和登入密碼一樣
                if (!string.IsNullOrEmpty(Request.Params["user_id"]))
                {
                    mu.user_id = uint.Parse(Request.Params["user_id"]);
                    store = _muMgr.GetManageUser(mu);
                    if (store.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(Request.Params["secret_password"]))
                        {
                            HashEncrypt hmd5 = new HashEncrypt();
                            if (hmd5.SHA256Encrypt(Request.Params["secret_password"]) == store[0].user_password)
                            {
                                json = "{success:false}";
                            }
                            else
                            {
                                json = "{success:true}";
                            }
                        }

                    }
                    else
                    {
                        json = "{success:false}";
                    }
                }
                //獲取管理用戶下拉列表
                else
                {
                    muq.IsPage = false;
                    muq.user_username = Request.Params["user_name"];
                    muq.user_status = 1;
                    storeq = _muMgr.GetNameMail(muq, out totalCount);
                    json = "{success:true,data:" + JsonConvert.SerializeObject(storeq, Formatting.Indented) + "}";//返回json數據
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

        #region 機敏資料登入驗證
        public HttpResponseBase SecretLogin()
        {

            string json = string.Empty;
            try
            {
                SecretAccountSet query = new SecretAccountSet();
                sasMgr = new SecretAccountSetMgr(mySqlConnectionString);
                _secretLogMgr = new SecretInfoLogMgr(mySqlConnectionString);
                query.user_id = Convert.ToUInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                query.ipfrom = CommonFunction.GetIP4Address(Request.UserHostAddress.ToString());
                query.status = 1;
                List<SecretAccountSet> store = sasMgr.GetSecretSetList(query);//獲得用戶的密保信息
                if (store.Count != 0)//該用戶有機敏權限
                {
                    if (!string.IsNullOrEmpty(Request.Params["password"]))
                    {
                        HashEncrypt hmd5 = new HashEncrypt();
                        if (store[0].secret_pwd != hmd5.SHA256Encrypt(Request.Params["oldpassword"]) && Request.Params["oldpassword"].ToString() != "" && store[0].pwd_status == 0)
                        {
                            ulaMgr = new UserLoginAttemptsMgr(mySqlConnectionString);
                            UserLoginAttempts ula = new UserLoginAttempts();
                            ula.login_mail = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_email;
                            ula.login_ipfrom = query.ipfrom;
                            ula.login_type = 4;
                            ulaMgr.Insert(ula);
                            SecretAccountSet sas = new SecretAccountSet();
                            store[0].user_login_attempts += 1;
                            store[0].updatedate = DateTime.Now;
                            sasMgr.LoginError(store[0]);
                            int count = 5 - store[0].user_login_attempts;//還有count次登入機會
                            json = "{success:true,error:5,count:" + count + "}";//返回json數據0：密碼錯誤
                        }
                        else
                        {
                            if ((store[0].secret_pwd == hmd5.SHA256Encrypt(Request.Params["password"]) && Request.Params["oldpassword"].ToString() == "") || store[0].pwd_status == 0)//密碼驗證正確
                            {
                                if (store[0].secret_count != 0 || store[0].user_login_attempts != 0 || store[0].pwd_status == 0)
                                {
                                    if (store[0].user_login_attempts != 0)
                                    {
                                        store[0].user_login_attempts = 0;
                                    }
                                    if (store[0].secret_count > 1)
                                    {
                                        store[0].secret_count = 1;
                                    }
                                    if (store[0].pwd_status == 0)
                                    {
                                        store[0].pwd_status = 1;
                                        store[0].secret_pwd = hmd5.SHA256Encrypt(Request.Params["password"]);
                                    }
                                    store[0].updatedate = DateTime.Now;
                                    sasMgr.Update(store[0]);//清空賬戶錯誤預警信息
                                }
                                //獲取最新的一條數據
                                SecretInfoLog info = _secretLogMgr.GetMaxCreateLog(new SecretInfoLog { user_id = query.user_id, ipfrom = query.ipfrom }).FirstOrDefault();
                                if (info.input_pwd_date == DateTime.MinValue)//該條數據是否已經記錄驗證時間，沒有則修改,有則新增
                                {
                                    info.input_pwd_date = DateTime.Now;
                                    _secretLogMgr.UpdateSecretInfoLog(info);
                                }
                                else
                                {
                                    info.input_pwd_date = DateTime.Now;
                                    _secretLogMgr.InsertSecretInfoLog(info);
                                }


                                json = "{success:true,error:0}";//返回json數據

                            }
                            else
                            {//密碼錯誤向
                                ulaMgr = new UserLoginAttemptsMgr(mySqlConnectionString);
                                UserLoginAttempts ula = new UserLoginAttempts();
                                ula.login_mail = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_email;
                                ula.login_ipfrom = query.ipfrom;
                                ula.login_type = 4;
                                ulaMgr.Insert(ula);
                                SecretAccountSet sas = new SecretAccountSet();
                                store[0].user_login_attempts += 1;
                                store[0].updatedate = DateTime.Now;
                                sasMgr.LoginError(store[0]);
                                int count = 5 - store[0].user_login_attempts;//還有count次登入機會
                                if (store[0].secret_pwd != hmd5.SHA256Encrypt(Request.Params["oldpassword"]) && Request.Params["oldpassword"].ToString() != "" && store[0].pwd_status == 0)
                                {
                                    json = "{success:true,error:1,count:" + count + "}";//返回json數據0：密碼錯誤
                                }
                                else
                                {

                                    json = "{success:true,error:1,count:" + count + "}";//返回json數據0：密碼錯誤
                                }
                            }
                        }
                    }
                    else
                    {
                        json = "{success:true,error:3}";//返回json數據，後台未獲取到輸入的密碼 
                    }

                }
                else
                {
                    json = "{success:true,error:2}";//返回json數據1:用戶未註冊資安權限或被鎖定
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

        #region 根據id獲取主表信息GetUserInfo
        [CustomHandleError]
        public HttpResponseBase GetUserInfo()
        {
            string json = "{success:false}";
            try
            {
                int type = 0; int related_id = 0; string urlType = string.Empty; int totalCount = 0;
                int info_id = 0; string info_type = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["relatedID"]))
                {
                    related_id = Convert.ToInt32(Request.Params["relatedID"].ToString());//表主鍵欄位
                }
                if (!string.IsNullOrEmpty(Request.Params["type"]))
                {
                    type = Convert.ToInt32(Request.Params["type"].ToString());//secret_type,表某一模塊
                }
                if (!string.IsNullOrEmpty(Request.Params["urlType"]))//url地址
                {
                    urlType = Request.Params["urlType"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["info_id"]))
                {
                    info_id = Convert.ToInt32(Request.Params["info_id"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["info_type"]))
                {
                    info_type = Request.Params["info_type"].ToString();
                }
                switch (info_type)//1.會員查詢頁面2.訂單內容3.簡訊查詢4.聯絡客服列表
                {
                    case "users":
                        UsersMgr _usermgr = new UsersMgr(mySqlConnectionString);
                        Users u = new Users();
                        Users uModel = _usermgr.GetUser(new Users { user_id = Convert.ToUInt32(info_id) }).FirstOrDefault();
                        json = "{success:true,\"user_id\":\"" + uModel.user_id + "\",\"user_name\":\"" + uModel.user_name + "\",\"user_email\":\"" + uModel.user_email + "\",\"user_mobile\":\"" + uModel.user_mobile + "\",\"user_phone\":\"" + uModel.user_phone + "\",\"user_adress\":\"" + uModel.user_address + "\"}";

                        this.Response.Clear();
                        this.Response.Write(json);
                        this.Response.End();
                        return this.Response;
                    case "trial_share":
                        TrialRecordMgr _recordMgr = new TrialRecordMgr(mySqlConnectionString);
                        TrialShare tModel = _recordMgr.GetTrialShare(new TrialShare { share_id = related_id, user_id = info_id });
                        json = "{success:true,\"user_id\":\"" + tModel.user_id + "\",\"user_name\":\"" + tModel.user_name + "\"}";
                        break;
                    case "manager_user":
                        ManageUserMgr _muMgr = new ManageUserMgr(mySqlConnectionString);
                        ManageUser muModel = _muMgr.GetManageUser(new ManageUser { user_id = Convert.ToUInt32(info_id) }).FirstOrDefault();
                        json = "{success:true,\"user_id\":\"" + muModel.user_id + "\",\"user_name\":\"" + muModel.user_username + "\",\"user_email\":\"" + muModel.user_email + "\"}";
                        break;
                    case "vip_user":
                        VipUserMgr vipusersMgr = new VipUserMgr(mySqlConnectionString);
                        VipUser model = vipusersMgr.GetSingleByID(related_id);
                        json = "{success:true,\"user_id\":\"" + model.user_id + "\",\"user_email\":\"" + model.user_email + "\"}";
                        break;
                    case "edm_group_email":
                        EdmGroupEmailMgr edmgroupmailMgr = new EdmGroupEmailMgr(mySqlConnectionString);
                        EdmGroupEmailQuery egemodel = edmgroupmailMgr.GetModel(new EdmGroupEmail { email_id = Convert.ToUInt32(related_id), group_id = Convert.ToUInt32(info_id) }).FirstOrDefault();
                        json = "{success:true,\"user_id\":\"" + egemodel.email_id + "\",\"user_name\":\"" + egemodel.email_name + "\",\"user_email\":\"" + egemodel.email_address + "\"}";
                        break;
                    case "edm_test":
                        EdmTestMgr edmtestMgr = new EdmTestMgr(mySqlConnectionString);
                        EdmTestQuery etmodel = edmtestMgr.GetModel(new EdmTestQuery { email_id = Convert.ToUInt32(related_id) }).FirstOrDefault();
                        json = "{success:true,\"user_id\":\"" + etmodel.email_id + "\",\"user_name\":\"" + etmodel.test_username + "\",\"user_email\":\"" + etmodel.email_address + "\"}";
                        break;    
                    case "order_master":
                        OrderMasterMgr omMgr = new OrderMasterMgr(mySqlConnectionString);
                        zMgr = new ZipMgr(mySqlConnectionString);
                        DataTable dt = omMgr.GetOrderidAndName(related_id);
                        json = "{success:true,\"order_id\":\"" + dt.Rows[0][0] + "\",\"order_name\":\"" + dt.Rows[0][1] + "\",\"order_phone\":\"" + dt.Rows[0][2] + "\",\"order_mobile\":\"" + dt.Rows[0][3] + "\",\"order_address\":\"" + zMgr.Getaddress(int.Parse(dt.Rows[0][9].ToString())) + dt.Rows[0][4] + "\",\"delivery_name\":\"" + dt.Rows[0][5] + "\",\"delivery_phone\":\"" + dt.Rows[0][6] + "\",\"delivery_mobile\":\"" + dt.Rows[0][7] + "\",\"delivery_address\":\"" + zMgr.Getaddress(int.Parse(dt.Rows[0][10].ToString())) + dt.Rows[0][8] + "\"}";
                        break;
                    case "order_master1":
                        OrderMasterMgr omMgr1 = new OrderMasterMgr(mySqlConnectionString);
                        DataTable dt1 = omMgr1.GetOrderidAndName(related_id);
                        json = "{success:true,\"order_id\":\"" + dt1.Rows[0][0] + "\",\"order_name\":\"" + dt1.Rows[0][1] + "\"}";
                        break;
                    case "order_payment_hitrust":
                        TabShowMgr OphMgr = new TabShowMgr(mySqlConnectionString);
                        DataTable store = new DataTable();
                        store = OphMgr.GetOderHitrustDT(related_id);
                        json = "{success:true,\"id\":\"" + store.Rows[0]["id"] + "\",\"pan\":\"" + store.Rows[0]["pan"] + "\",\"bankname\":\"" + store.Rows[0]["bankname"] + "\"}";
                        break;
                    //case "send_mail":
                    //    SendMailMgr sendmailMgr = new SendMailMgr(mySqlConnectionString);
                    //    SendMail sendModel = sendmailMgr.GetModel(new SendMail { id = related_id });
                    //    json = "{success:true,\"sender_address\":\"" + sendModel.sender_address + "\",\"sender_name\":\"" + sendModel.sender_name + "\",\"send_type\":\"" + sendModel.send_type
                    //        + "\",\"recipient\":\"" + sendModel.recipient + "\",\"recipient_name\":\"" + sendModel.recipient_name
                    //        + "\"}";
                    //    break;
                }
                switch (type)//1.會員查詢頁面2.訂單內容3.簡訊查詢4.聯絡客服列表
                {
                    case 1:
                        if (urlType == "/Member/RecommendMember")//推薦會員中推薦者的信息
                        {
                            UserRecommendMgr _userrecommendMgr = new UserRecommendMgr(mySqlConnectionString);
                            DataTable _urdtno = _userrecommendMgr.getUserInfo(related_id);
                            if (_urdtno.Rows.Count > 0)
                            {
                                json = "{success:true,\"user_id\":\"" + "" + "\",\"user_name\":\"" + "" + "\",\"user_email\":\"" + "" + "\",\"user_phone\":\"" + "" + "\",\"user_adress\":\"" + "" + "\",\"ur_name\":\"" + _urdtno.Rows[0]["name"] + "\",\"ur_mail\":\"" + _urdtno.Rows[0]["mail"] + "\",\"no_ur_name\":\"" + _urdtno.Rows[0]["user_name"] + "\"}";
                            }
                        }
                        else if (urlType == "/Member/UserLoginLog")//會員登入記錄
                        {
                            UserLoginLogMgr _userloginlog = new UserLoginLogMgr(mySqlConnectionString);
                            DataTable _dtull = _userloginlog.GetUserInfo(related_id);
                            if (_dtull.Rows.Count > 0)
                            {
                                json = "{success:true,\"user_id\":\"" + _dtull.Rows[0]["user_id"] + "\",\"user_name\":\"" + _dtull.Rows[0]["user_name"] + "\",\"user_email\":\"" + _dtull.Rows[0]["user_email"] + "\",\"user_phone\":\"" + _dtull.Rows[0]["user_phone"] + "\",\"user_adress\":\"" + _dtull.Rows[0]["user_address"] + "\"}";
                            }
                        }
                        else
                        {
                            UsersListMgr _userMgr = new UsersListMgr(mySqlConnectionString);
                            BLL.gigade.Model.Custom.Users _user = _userMgr.getModel(related_id);
                            if (_user != null)
                            {
                                json = "{success:true,\"user_id\":\"" + _user.user_id + "\",\"user_name\":\"" + _user.user_name + "\",\"user_email\":\"" + _user.user_email + "\",\"user_phone\":\"" + _user.user_phone + "\",\"user_adress\":\"" + _user.user_address + "\"}";
                            }
                        }
                        break;
                    case 2:
                        OrderQuestionMgr _IOrderQuesMgr = new OrderQuestionMgr(mySqlConnectionString);
                        DataTable _dtques = _IOrderQuesMgr.GetUserInfo(related_id);
                        if (_dtques.Rows.Count > 0)
                        {
                            json = "{success:true,\"user_id\":\"" + _dtques.Rows[0]["user_id"] + "\",\"user_name\":\"" + _dtques.Rows[0]["user_name"] + "\",\"user_email\":\"" + _dtques.Rows[0]["user_email"] + "\",\"user_phone\":\"" + _dtques.Rows[0]["user_phone"] + "\",\"user_adress\":\"" + "" + "\",\"order_id\":\"" + _dtques.Rows[0]["order_id"] + "\"}";
                        }
                        break;
                    case 3:
                        SmsMgr _ISmsMgr = new SmsMgr(mySqlConnectionString);

                        SmsQuery SmsStore = _ISmsMgr.GetSmsList(new SmsQuery { id = related_id }, out totalCount).FirstOrDefault();
                        if (SmsStore != null)
                        {
                            json = "{success:true,\"user_id\":\"" + "" + "\",\"user_name\":\"" + "" + "\",\"user_email\":\"" + "" + "\",\"user_phone\":\"" + SmsStore.mobile + "\",\"user_adress\":\"" + "" + "\"}";
                        }
                        break;
                    case 4:
                        ContactUsQuestionMgr _ctactMgr = new ContactUsQuestionMgr(mySqlConnectionString);
                        DataTable _dt = _ctactMgr.GetUserInfo(related_id);
                        if (_dt.Rows.Count > 0)
                        {
                            json = "{success:true,\"user_id\":\"" + _dt.Rows[0]["user_id"] + "\",\"user_name\":\"" + _dt.Rows[0]["user_name"] + "\",\"user_email\":\"" + _dt.Rows[0]["user_email"] + "\",\"user_phone\":\"" + _dt.Rows[0]["user_phone"] + "\",\"user_adress\":\"" + "" + "\"}";
                        }
                        break;
                    case 7://供應商詳情
                        VendorQuery _dtven = null;
                        if (urlType == "/Vendor/VendorBrandList")
                        {
                            VendorBrandSetMgr _IvendorBrandSet = new VendorBrandSetMgr(mySqlConnectionString);
                            VendorBrandSetQuery query = _IvendorBrandSet.GetModelById(related_id);
                            VendorMgr _vendorMgr = new VendorMgr(mySqlConnectionString);

                            _dtven = _vendorMgr.Query(new VendorQuery { vendor_id = query.vendor_id, IsPage = false }, ref totalCount).FirstOrDefault();
                            if (_dtven != null)
                            {
                                json = "{success:true,\"user_id\":\"" + "" + "\",\"user_name\":\"" + _dtven.vendor_name_full + "\",\"user_email\":\"" + "" + "\",\"user_phone\":\"" + "" + "\",\"user_adress\":\"" + "" + "\",\"simple_name\":\"" + _dtven.vendor_name_simple + "\"}";
                            }
                        }
                        else if (urlType == "/Vendor/VendorLoginList")
                        {
                            VendorLoginListMgr _Ivendorloginlist = new VendorLoginListMgr(mySqlConnectionString);

                            VendorLoginQuery query = _Ivendorloginlist.Query(new VendorLoginQuery { login_id = Convert.ToUInt32(related_id), IsPage = false }, out totalCount).FirstOrDefault();
                            VendorMgr _vendorMgr = new VendorMgr(mySqlConnectionString);

                            _dtven = _vendorMgr.Query(new VendorQuery { vendor_id = query.vendor_id, IsPage = false }, ref totalCount).FirstOrDefault();
                            if (_dtven != null)
                            {
                                json = "{success:true,\"user_id\":\"" + _dtven.vendor_code + "\",\"user_name\":\"" + _dtven.vendor_name_full + "\",\"user_email\":\"" + "" + "\",\"user_phone\":\"" + "" + "\",\"user_adress\":\"" + "" + "\",\"simple_name\":\"" + _dtven.vendor_name_simple + "\"}";
                            }
                        }
                        else
                        {
                            VendorMgr _vendorMgr = new VendorMgr(mySqlConnectionString);

                            _dtven = _vendorMgr.Query(new VendorQuery { vendor_id = Convert.ToUInt32(related_id), IsPage = false }, ref totalCount).FirstOrDefault();
                            if (_dtven != null)
                            {
                                json = "{success:true,\"user_id\":\"" + _dtven.vendor_code + "\",\"user_name\":\"" + _dtven.vendor_name_full + "\",\"user_email\":\"" + _dtven.vendor_email + "\",\"user_phone\":\"" + "" + "\",\"user_adress\":\"" + _dtven.vendor_company_address + "\",\"simple_name\":\"" + _dtven.vendor_name_simple + "\"}";
                            }

                        }

                        break;
                    case 9:
                        MailUserMgr _IMailUserMgr = new MailUserMgr(mySqlConnectionString);
                        DataTable _dtmu = _IMailUserMgr.GetUserInfo(related_id);
                        if (_dtmu.Rows.Count > 0)
                        {
                            json = "{success:true,\"user_id\":\"" + _dtmu.Rows[0]["user_id"] + "\",\"user_name\":\"" + _dtmu.Rows[0]["user_name"] + "\",\"user_email\":\"" + _dtmu.Rows[0]["user_email"] + "\",\"user_phone\":\"" + _dtmu.Rows[0]["user_phone"] + "\",\"user_adress\":\"" + _dtmu.Rows[0]["user_address"] + "\"}";
                        }
                        break;
                    case 10:
                        PaperAnswerMgr _paperAnswerMgr = new PaperAnswerMgr(mySqlConnectionString);
                        PaperAnswer store = _paperAnswerMgr.GetPaperAnswerList(new PaperAnswer { answerID = related_id, IsPage = false }, out totalCount).FirstOrDefault();
                        if (store != null)
                        {
                            json = "{success:true,\"user_id\":\"" + store.userid + "\",\"user_name\":\"" + "" + "\",\"user_email\":\"" + store.userMail + "\",\"user_phone\":\"" + "" + "\",\"user_adress\":\"" + "" + "\"}";
                        }
                        break;
                    case 14://會員等級歷程
                        UserLevelLogMgr _userLevelLog = new UserLevelLogMgr(mySqlConnectionString);
                        //   UserLevelLogQuery store

                        UserLevelLogQuery levelStore = _userLevelLog.GetUserLevelLogList(new UserLevelLogQuery { user_id = Convert.ToUInt32(related_id), IsPage = false, isSecret=false}, out totalCount).FirstOrDefault();
                        if (levelStore != null)
                        {
                            json = "{success:true,\"user_id\":\"" + levelStore.user_id + "\",\"user_name\":\"" + levelStore.user_name + "\",\"user_email\":\"" + levelStore.user_email + "\"}";
                        }
                        break;
                    case 18://企業會員管理
                        _vipUserGroup = new VipUserGroupMgr(mySqlConnectionString);
                    VipUserGroupQuery vipStore = _vipUserGroup.GetVipUserGList(new VipUserGroupQuery { group_id = Convert.ToUInt32(related_id), IsPage = false,isSecret=false }, out totalCount).FirstOrDefault();
                    if (vipStore != null)
                        {
                            json = "{success:true,\"group_committe_chairman\":\"" + vipStore.group_committe_chairman + "\",\"group_committe_phone\":\"" + vipStore.group_committe_phone + "\",\"group_committe_mail\":\"" + vipStore.group_committe_mail + "\"}";
                        }
                        break;
                    case 19://商品點擊查詢
                        _IBrowseDataMgr = new BrowseDataMgr(mySqlConnectionString);
                        DataTable _dtBrowse = _IBrowseDataMgr.GetBrowseDataList(new BrowseDataQuery { id = related_id, IsPage = false, isSecret = false }, out totalCount);
                        string user_name = (_dtBrowse.Rows[0]["user_name"].ToString());
                        string user_id = (_dtBrowse.Rows[0]["user_id"].ToString());
                        json = "{success:true,\"user_name\":\"" + user_name + "\",\"user_id\":\"" + user_id + "\"}";
                        break;
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


        #region  對賬號解鎖重設 + HttpResponseBase UnlockAndReset()
        /// <summary>
        /// 對賬號解鎖重設
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult UnlockAndReset()
        {
            string jsonStr = string.Empty;
            try
            {
                sasMgr = new SecretAccountSetMgr(mySqlConnectionString);
                _muMgr = new ManageUserMgr(mySqlConnectionString);
                int id = Convert.ToInt32(Request.Params["id"]);
                int activeValue = Convert.ToInt32(Request.Params["active"]);
                SecretAccountSet sas = new SecretAccountSet();
                sas.id = id;
                SecretAccountSet oldsas = sasMgr.Select(sas);//獲得用戶的密保信息
                if (oldsas.secret_limit == oldsas.secret_count && oldsas.secret_limit != 0)
                {
                    sas.status = 0;
                }
                else
                {
                    sas.status = 1;
                }
                sas.pwd_status = oldsas.pwd_status;
                sas.user_login_attempts = 0;
                if (sasMgr.Update(sas) > 0)
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

        #region 更改活動使用狀態 + HttpResponseBase UpdateActive()
        /// <summary>
        /// 更改活動使用狀態
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult UpdateActive()
        {
            string jsonStr = string.Empty;
            try
            {
                sasMgr = new SecretAccountSetMgr(mySqlConnectionString);
                int id = Convert.ToInt32(Request.Params["id"]);
                int activeValue = Convert.ToInt32(Request.Params["active"]);
                SecretAccountSet model = new SecretAccountSet();
                model.id = id;
                model.status = activeValue;
                model.updatedate = DateTime.Now;
                model.pwd_status = sasMgr.Select(new SecretAccountSet { id = model.id }).pwd_status;
                if (sasMgr.Update(model) > 0)
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

        #region  對訪問次數 + HttpResponseBase UpdateCount()
        /// <summary>
        /// 對賬號解鎖重設
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult UpdateCount()
        {
            string jsonStr = string.Empty;
            try
            {
                sasMgr = new SecretAccountSetMgr(mySqlConnectionString);
                _muMgr = new ManageUserMgr(mySqlConnectionString);
                int id = Convert.ToInt32(Request.Params["id"]);
                int activeValue = Convert.ToInt32(Request.Params["active"]);

                SecretAccountSet sas = new SecretAccountSet();
                sas.id = id;
                SecretAccountSet oldsas = sasMgr.Select(sas);//獲得用戶的密保信息
                if (oldsas.user_login_attempts == 5)
                {
                    sas.status = 0;
                }
                else
                {
                    sas.status = 1;
                }
                sas.pwd_status = oldsas.pwd_status;
                sas.secret_count = 0;
                if (sasMgr.Update(sas) > 0)
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
    }
}
