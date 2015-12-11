using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using System.Text.RegularExpressions;
using System.Timers;
using log4net;
using Admin.gigade.CustomError;
using BLL.gigade.Common;
using GigadeApi.Framework.ViewModels.SuppliersAccount;

namespace Admin.gigade.Controllers
{
    [HandleError]
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        static ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        private ICallerImplMgr callerMgr = new CallerMgr(connectionString);

        [CustomHandleError]
        public ActionResult Index()
        {
            HttpCookie cookies = Request.Cookies["UserInfo"];
            if (cookies != null)
            {
                ViewBag.LoginEmail = cookies["email"];
            }
            else
            {
                ViewBag.LoginEmail = null;
            }
            if (Session["caller"] != null)
            {
                return RedirectToAction("", "home");
            }
            else
            {
                ViewBag.challenge_id = callerMgr.Add_Challenge();
                ViewBag.challenge_key = callerMgr.Get_Challenge_Key(ViewBag.challenge_id);
                return View();
            }
        }

        #region 管理員登錄
        /// <summary>
        /// 管理員登錄
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        [HttpPost]
        public ActionResult Login()
        {
            object notice = new object();
            if (string.IsNullOrEmpty(Request.Params["txtEmail"]))
            {
                notice = "{result:'Notice',msg:'" + Resources.Login.NOTICE_NO_EMAIL + "'}";
                ViewBag.notice = notice;
                return View("Index");
            }

            string IsRemember = Request.Params["chkRememberEmail"] != null ? Request.Params["chkRememberEmail"] : "false";
            string email = Request.Params["txtEmail"].Trim();
            string passwd = Request.Params["hid_password"].Trim();
            string challenge_id = Request.Params["challenge_id"];
            int CookieExpireTime = 10;
            ViewBag.LoginEmail = null;
            ICallerImplMgr callerMgr = new CallerMgr(connectionString);
            Caller caller = null;
            UserLoginAttemptsMgr ulaMgr = new UserLoginAttemptsMgr(connectionString);

            //記錄/清空cookie

            BLL.gigade.Common.CommonFunction.Cookie_Set("UserInfo", "email", email, IsRemember, CookieExpireTime);


            if (!Regex.IsMatch(email, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
            {
                notice = "{result:'Notice',msg:'" + Resources.Login.NOTICE_EMAIL_FORMAT_ERROR + "'}";
                ViewBag.notice = notice;
                if (IsRemember == "true")
                {
                    ViewBag.LoginEmail = email;
                }

                return View("Index");
            }

            if (passwd == "")
            {
                notice = "{result:'Notice',msg:'" + Resources.Login.NOTICE_NO_PASSWD + "'}";
                ViewBag.notice = notice;
                if (IsRemember == "true")
                {
                    ViewBag.LoginEmail = email;
                }
                return View("Index");
            }

            try
            {
                caller = callerMgr.Login(email);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }


            if (caller == null)
            {
                notice = "{result:'Error',msg:'" + Resources.Login.ERROR_EMAIL_PASSWD_ERROR + "'}";
                ViewBag.notice = notice;
                if (IsRemember == "true")
                {
                    ViewBag.LoginEmail = email;
                }
                UserLoginAttempts ula = new UserLoginAttempts();
                ula.login_mail = email;
                ula.login_ipfrom = CommonFunction.GetIP4Address(Request.UserHostAddress.ToString());
                ula.login_type = 3;
                ulaMgr.Insert(ula);
                return View("Index");
            }
            else
            {
                if (caller.user_status == 2)
                {
                    notice = "{result:'Error',msg:'" + Resources.Login.NOTICE_EMAIL_STOP + "'}";

                    if (IsRemember == "true")
                    {
                        ViewBag.LoginEmail = email;
                    }
                    UserLoginAttempts ula = new UserLoginAttempts();
                    ula.login_mail = email;
                    ula.login_ipfrom = CommonFunction.GetIP4Address(Request.UserHostAddress.ToString());
                    ula.login_type = 3;
                    ulaMgr.Insert(ula);
                    ViewBag.notice = notice;
                    return View("Index");
                }

                if (caller.user_status == 3)
                {
                    notice = "{result:'Error',msg:'" + Resources.Login.NOTICE_EMAIL_DELETE + "'}";

                    if (IsRemember == "true")
                    {
                        ViewBag.LoginEmail = email;
                    }

                    UserLoginAttempts ula = new UserLoginAttempts();
                    ula.login_mail = caller.user_email;
                    ula.login_ipfrom = CommonFunction.GetIP4Address(Request.UserHostAddress.ToString());
                    ula.login_type = 3;
                    ulaMgr.Insert(ula);
                    ViewBag.notice = notice;
                    return View("Index");
                }

                string challenge_key = "";

                try
                {
                    challenge_key = callerMgr.Get_Challenge_Key(challenge_id);
                    callerMgr.Kill_Challenge_Id(challenge_id);
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                }

                BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                string newpasswd = hash.SHA256Encrypt(caller.user_password + challenge_key);


                if (passwd != newpasswd)
                {
                    try
                    {
                        callerMgr.Add_Login_Attempts(caller.user_id);

                    }
                    catch (Exception ex)
                    {
                        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                        log.Error(logMessage);
                    }

                    caller.user_login_attempts++;
                    string tempStr = string.Format(Resources.Login.ERROR_PASSWD_ERROR_TIMES, caller.user_login_attempts, 5);//後台登入改為5次 edit by shuangshuang0420j 201504101555 from hill

                    notice = "{result:'Error',msg:'" + tempStr + "'}";
                    ViewBag.notice = notice;

                    ViewBag.challenge_id = callerMgr.Add_Challenge();
                    ViewBag.challenge_key = callerMgr.Get_Challenge_Key(ViewBag.challenge_id);
                    //後台登入改為5次并計入UserLoginAttempts表 edit by shuangshuang0420j 201504101555 from hill
                    UserLoginAttempts ula = new UserLoginAttempts();
                    ula.login_mail = caller.user_email;
                    ula.login_ipfrom = CommonFunction.GetIP4Address(Request.UserHostAddress.ToString());
                    ula.login_type = 3;
                    ulaMgr.Insert(ula);
                    if (caller.user_login_attempts >= 5)//後台登入改為5次 edit by shuangshuang0420j 201504101555 from hill
                    {
                        try
                        {
                            callerMgr.Modify_User_Status(caller.user_id, 2);
                        }
                        catch (Exception ex)
                        {
                            Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                            logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                            logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            log.Error(logMessage);
                        }

                    }


                    if (IsRemember == "true")
                    {
                        ViewBag.LoginEmail = email;
                    }

                    return View("Index");
                }

                if (caller.user_status == 0)
                {
                    notice = "{result:'Notice',msg:'" + Resources.Login.NOTICE_FIRST_LOGIN + "'}";
                    ViewBag.notice = notice;
                    ViewBag.isFirst = 1;
                    ViewBag.uid = caller.user_id;
                    ViewBag.email = caller.user_email;
                    return View("ChangePasswd");
                }

                try
                {
                    //添加登錄記錄
                    callerMgr.Add_Manage_Login(caller.user_id);

                    //修改登入數據
                    callerMgr.Modify_User_Login_Data(caller.user_id);
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                }

                caller.user_password = "";

                try
                {
                    string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//XML的設置
                    string path = Server.MapPath(xmlPath);
                    SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
                    string APIServer = _siteConfigMgr.GetConfigByName("APIServer").Value;


                    GigadeApiRequest request = new GigadeApiRequest(APIServer);

                    var result = request.Request<SuppliersLoginViewModel, SuppliersLoginResult>("api/admin/account/login",
                         new SuppliersLoginViewModel() { user_email = email, user_password = newpasswd, user_halfToken = challenge_key, login_ipfrom = CommonFunction.GetIP4Address(Request.UserHostAddress.ToString()) });
                    var back = result.result;
                    Session["AccessToken"] = back.userToken.user_token;
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                }
                

                Session["caller"] = caller;
                return Redirect("../home");

            }

        }

        #endregion

        #region 忘記密碼
        /// <summary>
        /// 忘記密碼
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public ActionResult Forget()
        {
            return View();
        }
        #endregion

        #region 處理忘記密碼
        /// <summary>
        /// 處理忘記密碼
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        [HttpPost]
        public ActionResult DoForget()
        {
            object notice = new object();
            if (!string.IsNullOrEmpty(Request.Params["txtEmail"]))
            {
                string _email = Request.Params["txtEmail"].Trim().ToLower();
                if (!Regex.IsMatch(_email, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
                {
                    notice = "{result:'Notice',msg:'" + Resources.Login.NOTICE_EMAIL_FORMAT_ERROR + "'}";
                    ViewBag.notice = notice;
                    return View("Forget");
                }

                Caller caller = null;
                ICallerImplMgr callerMgr = new CallerMgr(connectionString);
                UserLoginAttemptsMgr ulaMgr = new UserLoginAttemptsMgr(connectionString);
                try
                {
                    caller = callerMgr.Login(_email);
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                }

                if (caller == null)
                {
                    notice = "{result:'Error',msg:'" + Resources.Login.ERROR_EMIAL_NOT_MATCH + "'}";
                    ViewBag.notice = notice;
                    return View("Forget");
                }

                if (caller.user_status >= 2)
                {
                    notice = "{result:'Error',msg:'" + Resources.Login.ERROR_EMAIL_STATUS_INACTIVE + "'}";
                    ViewBag.notice = notice;
                    UserLoginAttempts ula = new UserLoginAttempts();
                    ula.login_mail = caller.user_email;
                    ula.login_ipfrom = CommonFunction.GetIP4Address(Request.UserHostAddress.ToString());
                    ula.login_type = 3;
                    ulaMgr.Insert(ula);
                    return View("Forget");
                }

                string sUser_Confirm_Code = BLL.gigade.Common.CommonFunction.Generate_Rand_String(8);

                BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();

                try
                {
                    callerMgr.Modify_User_Confirm_Code(caller.user_id, hash.SHA256Encrypt(sUser_Confirm_Code));
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                }



                ///////////////////////
                //發郵件
                ///////////////////////

                //return Redirect("/Login/ChangePasswd?uid=" + caller.user_id + "&code=" + sUser_Confirm_Code);

                ViewBag.ConfirmSend = "請檢查您的E-mail信箱，以取得密碼的相關資訊!";
                return View("NoticeShow");

            }
            else
            {
                notice = "{result:'Notice',msg:'登錄信箱不能為空!'}";
                ViewBag.notice = notice;
                return View("Forget");
            }
        }
        #endregion

        #region 顯示提示信息
        /// <summary>
        /// 顯示提示信息
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public ActionResult NoticeShow()
        {
            return View();
        }
        #endregion

        #region 修改密碼
        /// <summary>
        /// 修改密碼
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        [HttpPost]
        public ActionResult ChangePasswd()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["uid"]) && !string.IsNullOrEmpty(Request.QueryString["code"]))
            {
                BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                int nGet_User_Id = int.Parse(Request.QueryString["uid"].Trim());
                string sGet_Confirm_Code = Request.QueryString["code"];
                ICallerImplMgr callerMgr = new CallerMgr(connectionString);
                Caller caller = null;
                try
                {
                    caller = callerMgr.GetUserById(nGet_User_Id);
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                }

                string sDB_Confrim_Code = caller.user_confirm_code;

                if (sDB_Confrim_Code == "")
                {
                    ViewBag.ConfirmSend = Resources.Login.ERROR_PASSWD_ACTIVATION;
                    return View("NoticeShow");
                }

                if (sDB_Confrim_Code != hash.SHA256Encrypt(sGet_Confirm_Code))
                {
                    ViewBag.ConfirmSend = Resources.Login.ERROR_PASSWD_ACTIVATION;
                    return View("NoticeShow");
                }

                ViewBag.uid = nGet_User_Id;
                ViewBag.code = sGet_Confirm_Code;
                ViewBag.email = caller.user_email;
                return View();
            }
            else
            {
                ViewBag.ConfirmSend = Resources.Login.ERROR_PASSWD_ACTIVATION;
                return View("NoticeShow");
            }

        }
        #endregion

        #region 處理修改密碼
        /// <summary>
        /// 處理修改密碼
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        [HttpPost]
        public ActionResult Change()
        {
            ICallerImplMgr mgr = null;
            Caller caller = null;
            object notice = new object();
            int nUserId = 0;
            string sPasswd1;
            string sPasswd2 = "";
            string sCode;
            if (!string.IsNullOrEmpty(Request.Params["uid"]) && !string.IsNullOrEmpty(Request.Params["code"]) && Request.Params["hid_isFirst"] != "1")
            {
                nUserId = int.Parse(Request.Params["uid"]);
                sCode = Request.Params["code"];
                sPasswd1 = Request.Params["passwd1"];
                sPasswd2 = Request.Params["passwd2"];
                mgr = new CallerMgr(connectionString);

                try
                {
                    caller = mgr.GetUserById(nUserId);
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                }

                BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();

                string sDB_Confirm_Code = caller.user_confirm_code;
                if (sDB_Confirm_Code == "")
                {
                    ViewBag.ConfirmSend = Resources.Login.ERROR_PASSWD_ACTIVATION;
                    return View("NoticeShow");
                }
                else if (sDB_Confirm_Code != hash.SHA256Encrypt(sCode))
                {
                    ViewBag.ConfirmSend = Resources.Login.ERROR_PASSWD_ACTIVATION;
                    return View("NoticeShow");
                }
                else if (sPasswd1 == "" || sPasswd2 == "")
                {
                    notice = "{result:'Notice',msg:'" + Resources.Login.NOTICE_NO_PASSWD + "'}";
                    ViewBag.notice = notice;
                    return View("ChangePasswd");
                }
                else if (sPasswd1 != sPasswd2)
                {
                    notice = "{result:'Notice',msg:'" + Resources.Login.NOTICE_PASSWD_MISMATCH + "'}";
                    ViewBag.notice = notice;
                    return View("ChangePasswd");
                }
            }
            else if (Request.Params["hid_isFirst"] == "1" && !string.IsNullOrEmpty(Request.Params["uid"]))
            {
                nUserId = int.Parse(Request.Params["uid"]);
                sPasswd1 = Request.Params["passwd1"];
                sPasswd2 = Request.Params["passwd2"];
                mgr = new CallerMgr(connectionString);

                if (sPasswd1 == "" || sPasswd2 == "")
                {
                    notice = "{result:'Notice',msg:'" + Resources.Login.NOTICE_NO_PASSWD + "'}";
                    ViewBag.notice = notice;
                    return View("ChangePasswd");
                }
                else if (sPasswd1 != sPasswd2)
                {
                    notice = "{result:'Notice',msg:'" + Resources.Login.NOTICE_PASSWD_MISMATCH + "'}";
                    ViewBag.notice = notice;
                    return View("ChangePasswd");
                }

                //修改用戶狀態
                try
                {
                    mgr.Modify_User_Status(nUserId, 1);
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                }

            }
            else
            {
                ViewBag.ConfirmSend = Resources.Login.ERROR_ACCESS_LIMIT;
                return View("NoticeShow");
            }

            try
            {
                mgr.Modify_User_Password(nUserId, sPasswd2);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }

            notice = "{result:'Notice',msg:'" + Resources.Login.NOTICE_PASSWD_CHANGE_OK + "'}";
            ViewBag.notice = notice;
            return View("LoginAgain");
        }
        #endregion

        #region 重新登入
        /// <summary>
        /// 重新登入
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public ActionResult LoginAgain()
        {
            Session.Remove("caller");
            return Redirect("../Login");
        }
        #endregion

        #region 登出
        [CustomHandleError]
        public ActionResult Logout()
        {
            Session.Remove("caller");
            return Redirect("/Login");
        }
        #endregion

        //#region 登录日志
        ////登录日志
        //private void SaveLogInLoge(int type, string CallId)
        //{
        //    ILogInLogeImplMgr logInLogeMgr = new LogInLogeMgr(connectionString);
        //    try
        //    {
        //        LogInLoge logInLoge = new LogInLoge() { Type = type };
        //        logInLoge.Callid = CallId;

        //        ClientIpAdderss cIpAddress = new ClientIpAdderss(this.HttpContext);
        //        logInLoge.IP = cIpAddress.GetIpAddress();
        //        logInLogeMgr.Save(logInLoge);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message));
        //    }
        //}

        //#endregion
    }
}
