using BLL.gigade.Common;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace BLL.gigade.Mgr.Schedules
{
    public class CheckUrlResultMgr
    {
        private ScheduleServiceMgr _secheduleServiceMgr;
       
        static IDBAccess _accessMySql;
        static string mySqlConnectionString = string.Empty;
        public CheckUrlResultMgr(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            mySqlConnectionString = connectionString;
        }
        public bool Start(string schedule_code)
        {
            if (string.IsNullOrEmpty(schedule_code))
            {
                return false;
            }
            try
            {
                MailModel mailModel = new MailModel();
                mailModel.MysqlConnectionString = mySqlConnectionString;

                string GroupCode = string.Empty;
                string MailTitle = string.Empty;
                string MailBody = string.Empty;
                string requestUrl = string.Empty;
                //bool IsSeparate = true;
                //bool IsDisplyName = true;

                //獲取該排程參數
                List<ScheduleConfigQuery> store_config = new List<ScheduleConfigQuery>();
                ScheduleConfigQuery query_config = new ScheduleConfigQuery();
                query_config.schedule_code = schedule_code;
                _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
                store_config = _secheduleServiceMgr.GetScheduleConfig(query_config);

                #region mailhelp賦值
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
                    else if (item.parameterCode.Equals("requestUrl"))
                    {
                        requestUrl = item.value;
                    }                  
                }
                #endregion

                
                StreamReader sr;                               
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
                httpRequest.Timeout = 10000;
                httpRequest.Method = "GET";
                HttpWebResponse httpResponse;
                try
                {
                     httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                     if (httpResponse.StatusCode == HttpStatusCode.OK)
                     {
                         sr = new StreamReader(httpResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                         string result = sr.ReadToEnd();                         
                         result = result.Trim();
                         if (result == "success")
                         {
                             MailBody = "更新成功";
                             MailHelper mail = new MailHelper(mailModel);
                             mail.SendToGroup(GroupCode, MailTitle, MailBody + " ", false, true);
                         }
                         else if (result == "error")
                         {
                             MailBody = "更新失敗";
                             MailHelper mail = new MailHelper(mailModel);
                             mail.SendToGroup(GroupCode, MailTitle, MailBody + " ", false, true);
                         }
                     }
                     else
                     {
                         MailBody = "更新失敗";
                         MailHelper mail = new MailHelper(mailModel);
                         mail.SendToGroup(GroupCode, MailTitle, MailBody + " ", false, true);
                     }     
                }
                catch (Exception ex)
                {
                    MailBody = "更新失敗";
                    MailHelper mail = new MailHelper(mailModel);
                    mail.SendToGroup(GroupCode, MailTitle, MailBody + " ", false, true);
                    throw new Exception(ex.Message);
                }                                      
            }
            catch (Exception ex)
            {
                throw new Exception("CheckUrlResultMgr-->Start-->" + ex.Message);
            }
            return true;
        }
    }
}
