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
using BLL.gigade.Mgr.Impl;

namespace Admin.gigade.Controllers
{
    //排程控制器 add by yafeng0715j 201510231013
    public class ScheduleServiceWorkerController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        public EdmContentNewMgr _edmcontentMgr;
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
    }
}
