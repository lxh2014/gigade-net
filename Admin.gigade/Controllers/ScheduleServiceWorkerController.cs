using BLL.gigade.Mgr.Schedules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Common;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Mgr;
using System.Data;
using System.Text;
using BLL.gigade.Mgr.Impl;

namespace Admin.gigade.Controllers
{
    //排程控制器 add by yafeng0715j 201510231013
    public class ScheduleServiceWorkerController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        public EdmContentNewMgr _edmcontentMgr;
        public IProductItemImplMgr _productItemMgr;
        BLL.gigade.Mgr.ProductItemMgr productItemMgr = new BLL.gigade.Mgr.ProductItemMgr(mySqlConnectionString);
        IParametersrcImplMgr _paraMgr;
        public ActionResult Index()
        {
            return View();
        }
        #region 黑貓物流狀態抓取排程 add by yafeng0715j 20151019AM

        public bool DeliverStatus()
        {
            DateTime startTime = DateTime.Now;
            if (string.IsNullOrEmpty(Request.Params["schedule_code"]))
            {
                return false;
            }
            try
            {
                DeliverStatusMgr dsMgr = new DeliverStatusMgr(mySqlConnectionString);
                dsMgr.Start(Request.Params["schedule_code"], startTime);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return true;
        }

        #endregion

        #region 寄信排程
        public bool SendEMail()
        {
            string json = string.Empty;

            try
            {
                #region
                string schedule_code = "send mail";//Request.Params["schedule_code"].ToString();
                MailModel mailModel = new MailModel();
                mailModel.MysqlConnectionString = mySqlConnectionString;
                string GroupCode = string.Empty;
                string MailTitle = string.Empty;
                string MailBody = string.Empty;
                bool IsSeparate = false;
                bool IsDisplyName = true;
                ScheduleServiceMgr _secheduleServiceMgr;
                List<MailRequest> MR = new List<MailRequest>();
                MailRequest model = new MailRequest();
                List<ScheduleConfigQuery> store_config = new List<ScheduleConfigQuery>();
                ScheduleConfigQuery query_config = new ScheduleConfigQuery();

                query_config.schedule_code = schedule_code;
                _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
                store_config = _secheduleServiceMgr.GetScheduleConfig(query_config);
                foreach (ScheduleConfigQuery item in store_config)
                {
                    if (item.parameterCode.Equals("MailFromAddress"))
                    {
                        mailModel.MailFromAddress = item.value;
                    }
                    else if (item.parameterCode.Equals("MailHost"))
                    {
                        mailModel.MailHost = item.value;
                    }
                    else if (item.parameterCode.Equals("MailPort"))
                    {
                        mailModel.MailPort = item.value;
                    }
                    else if (item.parameterCode.Equals("MailFromUser"))
                    {
                        mailModel.MailFromUser = item.value;
                    }
                    else if (item.parameterCode.Equals("EmailPassWord"))
                    {
                        mailModel.MailFormPwd = item.value;
                    }
                    else if (item.parameterCode.Equals("GroupCode"))
                    {
                        GroupCode = item.value;
                    }
                    else if (item.parameterCode.Equals("MailTitle"))
                    {
                        MailTitle = item.value;
                    }
                    else if (item.parameterCode.Equals("MailTitle"))
                    {
                        MailTitle = item.value;
                    }
                    else if (item.parameterCode.Equals("MailBody"))
                    {
                        MailBody = item.value;
                    }
                    else if (item.parameterCode.Equals("IsSeparate"))
                    {
                        if (item.value.ToString().Trim().ToLower() == "false")
                        {
                            IsSeparate = false;
                        }
                        else if (item.value.ToString().Trim().ToLower() == "true")
                        {
                            IsSeparate = true;
                        }
                    }
                    else if (item.parameterCode.Equals("IsDisplyName"))
                    {
                        if (item.value.ToString().Trim().ToLower() == "false")
                        {
                            IsDisplyName = false;
                        }
                        else if (item.value.ToString().Trim().ToLower() == "true")
                        {
                            IsDisplyName = true;
                        }
                    }
                    else if (!string.IsNullOrEmpty(Request.Params["IsDisplyName"]))
                    {
                        if (Request.Params["IsDisplyName"].ToString().Trim().ToLower() == "false")
                        {
                            IsSeparate = false;
                        }
                        else if (Request.Params["IsDisplyName"].ToString().Trim().ToLower() == "true")
                        {
                            IsSeparate = true;
                        }
                    }
                }
                MailHelper mail = new MailHelper(mailModel);
                _edmcontentMgr = new EdmContentNewMgr(mySqlConnectionString);
                _edmcontentMgr.SendEMail(mail);

                #endregion
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false}";
            }
            return true;
        }
        #endregion

        #region 期望到貨日調整記錄通知排程 add by zhaozhi0623j 20151118PM

        public bool DeliveryChangeLogSendMailSchedule()
        {
            bool result = false;     
            if (string.IsNullOrEmpty(Request.Params["schedule_code"]))
            {
                return false;
            }
            try
            {            
                IDeliverChangeLogImplMgr dclMgr = new DeliverChangeLogMgr(mySqlConnectionString);
                result = dclMgr.Start(Request.Params["schedule_code"]);
            }
            catch (Exception ex)
            {
                //throw new Exception("ScheduleServiceWorkerController-->DeliveryChangeLogSendMailSchedule-->" + ex.Message, ex);
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return result;
        }
        #endregion

        #region 用戶登陸日誌排程
        public bool UserLoginLog()
        {
            bool result = false;
            if (string.IsNullOrEmpty(Request.Params["schedule_code"]))
            {
                return false;
            }
            try
            {
                BLL.gigade.Mgr.Schedules.UserLoginLogMgr UserLoginLog = new BLL.gigade.Mgr.Schedules.UserLoginLogMgr(mySqlConnectionString);
                result = UserLoginLog.Start(Request.Params["schedule_code"]);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return result;
        } 
        #endregion

        #region 設置config文件排程
        public bool setconfigxml()
        {
            bool result = false;
            if (string.IsNullOrEmpty(Request.Params["schedule_code"]))
            {
                return false;
            }
            try
            {
                SetConfigXmlMgr setconfigxml = new SetConfigXmlMgr(mySqlConnectionString);
                result = setconfigxml.Start(Request.Params["schedule_code"]);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return result;
        } 
        #endregion


        #region 商品建議採購量排程
        public bool GetSugestEMail()
        {
            bool result = false;
            if (string.IsNullOrEmpty(Request.Params["schedule_code"]))
            {
                return false;
            }
            try
            {
                BLL.gigade.Mgr.Schedules.ProductPurchaseMgr PurchaseMgr = new BLL.gigade.Mgr.Schedules.ProductPurchaseMgr(mySqlConnectionString);
                result = PurchaseMgr.Start(Request.Params["schedule_code"]);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }

            return result;
        }
       
        #endregion

        #region 用戶異常登錄提醒排程
        public bool CheckUnsafeLogin()
        {
            bool result = false;
            if (string.IsNullOrEmpty(Request.Params["schedule_code"]))
            {
                return false;
            }
            try
            {
                BLL.gigade.Mgr.Schedules.CheckUnsafeLoginMgr CheckUnsafeLoginMgr  = new BLL.gigade.Mgr.Schedules.CheckUnsafeLoginMgr(mySqlConnectionString);
                result = CheckUnsafeLoginMgr.Start(Request.Params["schedule_code"]);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return result;
        } 
        #endregion

        #region 拋出訂單資料給黑貓FTP add by zhaozhi0623j 20151127PM
        /// <summary>
        /// 拋出訂單資料給黑貓FTP
        /// </summary>
        /// <returns></returns>
        public bool SendOrderInfoToBlackCatFTP() 
        {
            bool result = false;
            if (string.IsNullOrEmpty(Request.Params["schedule_code"]))
            {
                return false;
            }
            try
            {
                BLL.gigade.Mgr.Schedules.SendOrderInfoToBlackCatFTPMgr SendOrderInfoToBlackCatFTPMgr = new BLL.gigade.Mgr.Schedules.SendOrderInfoToBlackCatFTPMgr(mySqlConnectionString);
                result = SendOrderInfoToBlackCatFTPMgr.Start(Request.Params["schedule_code"]);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return result;
        }

        #endregion

        #region 接收黑貓拋回之狀態
         /// <summary>
        /// 接收黑貓拋回之狀態//add by zhaozhi0623j 20151201PM
        /// </summary>
        /// <returns></returns>
        public bool ReceiveStatusFromTCat() 
        {
            bool result = false;
            if (string.IsNullOrEmpty(Request.Params["schedule_code"]))
            {
                return false;
            }
            try
            {
                BLL.gigade.Mgr.Schedules.ReceiveStatusFromTCatMgr ReceiveStatusFromTCatMgr = new BLL.gigade.Mgr.Schedules.ReceiveStatusFromTCatMgr(mySqlConnectionString);
                result = ReceiveStatusFromTCatMgr.Start(Request.Params["schedule_code"]);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return result;
        }
        #endregion  
     
        #region 贈品庫存設定
        public bool PromotionsAmountGift()
        {
            bool result = false;
            if (string.IsNullOrEmpty(Request.Params["schedule_code"]))
            {
                return false;
            }
            try
            {
                BLL.gigade.Mgr.Schedules.PromotionsAmountGiftMgr promotionsAmountGiftMgr = new BLL.gigade.Mgr.Schedules.PromotionsAmountGiftMgr(mySqlConnectionString);
                result = promotionsAmountGiftMgr.Start(Request.Params["schedule_code"]);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return result;
        }
        #endregion

        #region 檢查異常訂單排程
        public bool GetCheckOrderAmountEMail()
        {
            bool result = false;
            if (string.IsNullOrEmpty(Request.Params["schedule_code"]))
            {
                return false;
            }
            try
            {
                BLL.gigade.Mgr.Schedules.CheckOrderAmountMgr OrderAmountMgr = new BLL.gigade.Mgr.Schedules.CheckOrderAmountMgr(mySqlConnectionString);
                result = OrderAmountMgr.Start(Request.Params["schedule_code"]);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }

            return result;
        }
        #endregion

        #region 檢查異地IP登錄
        public bool CheckIPAddress()
        {
            if (string.IsNullOrEmpty(Request.Params["schedule_code"]))
            {
                return false;
            }
            try
            {
                CheckIPAddressMgr dsMgr = new CheckIPAddressMgr(mySqlConnectionString);
                return dsMgr.Start(Request.Params["schedule_code"]);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return true;
        }
        #endregion

        #region 檢查Url結果發送郵件
        public bool CheckUrlResult()
        {
            bool result = false;
            if (string.IsNullOrEmpty(Request.Params["schedule_code"]))
            {
                return false;
            }
            try
            {
                BLL.gigade.Mgr.Schedules.CheckUrlResultMgr CheckUrlResultMgr = new BLL.gigade.Mgr.Schedules.CheckUrlResultMgr(mySqlConnectionString);
                result = CheckUrlResultMgr.Start(Request.Params["schedule_code"]);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return result;
        }
        #endregion

        #region 更新已存在登錄地區的IP地址
        public bool UpdateLoginAddress()
        {
            if (string.IsNullOrEmpty(Request.Params["schedule_code"]))
            {
                return false;
            }
            try
            {
                UpdateLoginAddressMgr dsMgr = new UpdateLoginAddressMgr(mySqlConnectionString);
                return dsMgr.Start(Request.Params["schedule_code"]);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return true;
        }
        #endregion

        #region 中獎發票同步
        /// <summary>
        /// 中獎發票同步排程
        /// </summary>
        /// <returns></returns>
        public bool WinningInvoiceSynchronism()
        {
            bool result = false;
            if (string.IsNullOrEmpty(Request.Params["schedule_code"]))
            {
                return false;
            }
            try
            {
                BLL.gigade.Mgr.Schedules.WinningInvoiceSynchronismMgr WinningInvoiceSynchronismMgr = new BLL.gigade.Mgr.Schedules.WinningInvoiceSynchronismMgr(mySqlConnectionString);
                result = WinningInvoiceSynchronismMgr.Start(Request.Params["schedule_code"]);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return result;
        }     
        #endregion
    }
}
