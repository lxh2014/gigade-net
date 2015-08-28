using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Text.RegularExpressions;
using System.Timers;
using log4net;
using BLL.gigade.Common;
using Vendor.CustomHandleError;
namespace Vendor.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        static ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        private ICallerImplMgr callerMgr = new CallerMgr(connectionString);
        private IVendorImplMgr _vendorImp;

        [CustomHandleError]
        public ActionResult Index()
        {
            HttpCookie cookies = Request.Cookies["vendor"];
            if (cookies != null)
            {
                ViewBag.LoginEmail = cookies["email"];
            }
            else
            {
                ViewBag.LoginEmail = null;
            }

            if (Session["vendor"] != null)
            {
                return RedirectToAction("", "home");
            }
            else
            {
                string vendorid = Request.QueryString["vendor_id"];
                string key = Request.QueryString["key"];
                string keysec = string.Empty;
                if (!string.IsNullOrEmpty(vendorid) && !string.IsNullOrEmpty(key))
                {
                    int vendor_id = Convert.ToInt32(vendorid);
                    _vendorImp = new VendorMgr(connectionString);
                    string str = _vendorImp.GetLoginId(vendor_id);
                    HashEncrypt hmd5 = new HashEncrypt();
                    string mdlogin_id = hmd5.Md5Encrypt(str, "MD5");
                    keysec = hmd5.Md5Encrypt(mdlogin_id + str, "MD5");
                    if (key.ToString().Trim() != keysec.ToString().Trim())
                    {
                        return View();
                    }
                    else
                    {
                        BLL.gigade.Model.Vendor vendor = new BLL.gigade.Model.Vendor();
                        BLL.gigade.Model.Vendor ven = new BLL.gigade.Model.Vendor();
                        vendor.vendor_id = Convert.ToUInt32(vendorid);
                        _vendorImp = new VendorMgr(connectionString);
                        ven = _vendorImp.GetSingle(vendor);
                        ven.vendor_password = "";
                        Session["vendor"] = ven;
                        Session["lgnName"] = ven.vendor_name_simple;
                        return RedirectToAction("", "home");
                    }
                }
                else
                {
                    return View();
                }

            }
        }

        #region 管理員登錄
        /// <summary>
        /// 輸出圖形驗證碼
        /// </summary>
        public void Code()
        {
            Session["code"] = CreateCode.RndNum(4);
            if (Session["code"]!=null)
            {
                CreateCode.CreateCheckCodeImage(Session["code"].ToString());
            }  
        }
        /// <summary>
        /// 管理員登錄
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        [HttpPost]
        public ActionResult Login()
        {
            string code = string.Empty;
            string challenge_id = string.Empty;
            string challenge_key = string.Empty;
          
            object notice = new object();
            if (string.IsNullOrEmpty(Request.Params["txtEmail"]))
            {
                notice = "{result:'Notice',msg:'" + Resources.Login.NOTICE_NO_EMAIL + "'}";
                ViewBag.notice = notice;
                return View("Index");
            }

            string IsRemember = Request.Params["chkRememberEmail"] != null ? Request.Params["chkRememberEmail"] : "false";
            string email = Request.Params["txtEmail"].Trim();
            string passwd = Request.Params["passwd"].Trim();
            int CookieExpireTime = 10;
            ViewBag.LoginEmail = null;
            _vendorImp = new VendorMgr(connectionString);
            //Caller caller = null;
            BLL.gigade.Model.Vendor vendor = null;
            //記錄/清空cookie

            BLL.gigade.Common.CommonFunction.Cookie_Set("vendor", "email", email, IsRemember, CookieExpireTime);

            UserLoginAttemptsMgr ulaMgr = new UserLoginAttemptsMgr(connectionString);

            //if (!Regex.IsMatch(email, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
            if (!Regex.IsMatch(email, @"[\w|-]+@[-|\w]*[-|\.|\w]*\.\w"))
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
            if (!string.IsNullOrEmpty(Request.Params["CheckCode"]))
            {
                code = Request.Params["CheckCode"].ToString().Trim();
            }
            if (Session["code"] != null)
            {
                if (Session["code"].ToString() != code)
                {
                    string message = "{result:'Notice',msg:'驗證碼輸入錯誤'}";
                    ViewBag.notice = message;
                    ViewBag.Email = Request.Params["txtEmail"];
                    ViewBag.Password = Request.Params["passwd"];
                    return View("Index");
                }
            }
            try
            {
                BLL.gigade.Model.Vendor query = new BLL.gigade.Model.Vendor();
                query.vendor_email = email;
                vendor = _vendorImp.Login(query);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }


            if (vendor == null)
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
                ula.login_type = 5;
                ulaMgr.Insert(ula);
                return View("Index");
            }
            else
            {
                if (vendor.vendor_status == 2)
                {
                    notice = "{result:'Error',msg:'" + Resources.Login.NOTICE_EMAIL_STOP + "'}";
                    ViewBag.notice = notice;
                    if (IsRemember == "true")
                    {
                        ViewBag.LoginEmail = email;
                    }
                    UserLoginAttempts ula = new UserLoginAttempts();
                    ula.login_mail = email;
                    ula.login_ipfrom = CommonFunction.GetIP4Address(Request.UserHostAddress.ToString());
                    ula.login_type = 5;
                    ulaMgr.Insert(ula);
                    ViewBag.Email = Request.Params["txtEmail"];
                    return View("Index");
                }

                //if (caller.user_status == 3)
                //{
                //    notice = "{result:'Error',msg:'" + Resources.Login.NOTICE_EMAIL_DELETE + "'}";
                //    ViewBag.notice = notice;
                //    if (IsRemember == "true")
                //    {
                //        ViewBag.LoginEmail = email;
                //    }
                //    return View("Index");
                //}


                try
                {
                    challenge_id = callerMgr.Add_Challenge();
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
                string inputpasswd = hash.SHA256Encrypt(hash.SHA256Encrypt(passwd) + challenge_key);
                string newpasswd = hash.SHA256Encrypt(vendor.vendor_password+ challenge_key);

                if (inputpasswd != newpasswd)
                {
                    try
                    {
                        _vendorImp.Add_Login_Attempts(Convert.ToInt32(vendor.vendor_id));
                        //callerMgr.Add_Login_Attempts(caller.user_id);
                    }
                    catch (Exception ex)
                    {
                        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                        log.Error(logMessage);
                    }

                    vendor.vendor_login_attempts++;
                    string tempStr = string.Format(Resources.Login.ERROR_PASSWD_ERROR_TIMES, vendor.vendor_login_attempts, 6);
                    notice = "{result:'Error',msg:'" + tempStr + "'}";
                    UserLoginAttempts ula = new UserLoginAttempts();
                    ula.login_mail = email;
                    ula.login_ipfrom = CommonFunction.GetIP4Address(Request.UserHostAddress.ToString());
                    ula.login_type = 5;
                    ulaMgr.Insert(ula);
                    ViewBag.notice = notice;
                    ViewBag.Email = Request.Params["txtEmail"];
                    ViewBag.challenge_id = callerMgr.Add_Challenge();
                    ViewBag.challenge_key = callerMgr.Get_Challenge_Key(ViewBag.challenge_id);

                    if (vendor.vendor_login_attempts >= 6)
                    {
                        try
                        {
                            _vendorImp.Modify_Vendor_Status(Convert.ToInt32(vendor.vendor_id), 2);
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

                //if (caller.user_status == 0)
                //{
                //    notice = "{result:'Notice',msg:'" + Resources.Login.NOTICE_FIRST_LOGIN + "'}";
                //    ViewBag.notice = notice;
                //    ViewBag.isFirst = 1;
                //    ViewBag.uid = caller.user_id;
                //    ViewBag.email = caller.user_email;
                //    return View("ChangePasswd");
                //}

                //try
                //{
                //    //添加登錄記錄
                //    callerMgr.Add_Manage_Login(caller.user_id);

                //    //修改登入數據
                //    callerMgr.Modify_User_Login_Data(caller.user_id);
                //}
                //catch (Exception ex)
                //{
                //    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                //    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                //    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                //    log.Error(logMessage);
                //}

                //caller.user_password = "";

                //Session["caller"] = caller;
                vendor.vendor_password = "";
                Session["vendor"] = vendor;
                Session["lgnName"] = vendor.vendor_name_simple;
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

                _vendorImp = new VendorMgr(connectionString);
                //Caller caller = null;
                BLL.gigade.Model.Vendor vendor = null;
                ICallerImplMgr callerMgr = new CallerMgr(connectionString);
                BLL.gigade.Model.Vendor query = new BLL.gigade.Model.Vendor();
                try
                {
                    query.vendor_email = _email;
                    vendor = _vendorImp.Login(query);
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                }

                if (vendor == null)
                {
                    notice = "{result:'Error',msg:'" + Resources.Login.ERROR_EMIAL_NOT_MATCH + "'}";
                    ViewBag.notice = notice;
                    return View("Forget");
                }

                if (vendor.vendor_status >= 2)
                {
                    notice = "{result:'Error',msg:'" + Resources.Login.ERROR_EMAIL_STATUS_INACTIVE + "'}";
                    ViewBag.notice = notice;
                    return View("Forget");
                }

                string sUser_Confirm_Code = BLL.gigade.Common.CommonFunction.Generate_Rand_String(8);

                BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();

                try
                {
                    _vendorImp.Modify_Vendor_Confirm_Code(Convert.ToInt32(vendor.vendor_id), hash.SHA256Encrypt(sUser_Confirm_Code));
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

        #region 重新登入
        /// <summary>
        /// 重新登入
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public ActionResult LoginAgain()
        {
            Session.Remove("vendor");
            return Redirect("../Login");
        }
        #endregion

        #region 登出
        [CustomHandleError]
        public ActionResult Logout()
        {
            Session.Remove("vendor");
            return Redirect("/Login");
        }
        #endregion
    }
}
