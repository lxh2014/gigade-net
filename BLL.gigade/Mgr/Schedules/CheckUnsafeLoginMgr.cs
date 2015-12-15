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
    public class CheckUnsafeLoginMgr
    {
        private ScheduleServiceMgr _secheduleServiceMgr;

        static IDBAccess _accessMySql;
        static string mySqlConnectionString = string.Empty;
        public CheckUnsafeLoginMgr(string connectionString)
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
                bool IsSeparate = true;
                bool IsDisplyName = true;

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
                    //else if (item.parameterCode.Equals("MailBody"))
                    //{
                    //    MailBody = item.value;
                    //}
                    //else if (item.parameterCode.Equals("IsSeparate"))
                    //{
                    //    if (item.value.ToString().Trim().ToLower() == "false")
                    //    {
                    //        IsSeparate = false;
                    //    }
                    //    else if (item.value.ToString().Trim().ToLower() == "true")
                    //    {
                    //        IsSeparate = true;
                    //    }
                    //}
                    //else if (item.parameterCode.Equals("IsDisplyName"))
                    //{
                    //    if (item.value.ToString().Trim().ToLower() == "false")
                    //    {
                    //        IsDisplyName = false;
                    //    }
                    //    else if (item.value.ToString().Trim().ToLower() == "true")
                    //    {
                    //        IsDisplyName = true;
                    //    }
                    //}
                }
                #endregion
                ///獲取用戶登陸信息
                long login_start = 0;
                long login_end = 0;
                int errorCount = 0;             
                login_start = CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy/MM/dd HH:00:00"));
                login_end = CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy/MM/dd HH:59:59"));
                errorCount = 2;//登錄錯誤次數
                string start_time = CommonFunction.GetNetTime(login_start).ToString("yyyy/MM/dd HH:mm:ss");
                string end_time = CommonFunction.GetNetTime(login_end).ToString("yyyy/MM/dd HH:mm:ss");

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@" select login_ipfrom,login_mail,t.parameterName as login_error_from,count(login_type) as total
from user_login_attempts ula
left join (select parameterName,parameterCode from t_parametersrc where parameterType ='user_login_type') t on ula.login_type=t.parameterCode
where login_createdate>='{0}' and login_createdate<='{1}' 
         and ula.login_ipfrom in (select login_ipfrom from user_login_attempts where login_createdate>='{0}' and login_createdate<='{1}' 
                                    GROUP BY login_ipfrom HAVING count(login_ipfrom)>='{2}')  
GROUP BY login_ipfrom,login_type,login_mail ;", login_start, login_end, errorCount);
                DataTable _dt = _accessMySql.getDataTable(sql.ToString());
                DataTable _newDt = new DataTable();
                                                       
                DataRow dr;
                _newDt.Columns.Add("異常IP", typeof(string));
                _newDt.Columns.Add("登入帳號", typeof(string));
                _newDt.Columns.Add("失敗次數", typeof(string));
                _newDt.Columns.Add("錯誤登入來源", typeof(string));          
                _newDt.Columns.Add("登入日期區間", typeof(string));

                if (_dt.Rows.Count > 0)
                {
                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        dr = _newDt.NewRow();
                        dr["異常IP"] = _dt.Rows[i]["login_ipfrom"];
                        dr["登入帳號"] = _dt.Rows[i]["login_mail"];
                        dr["失敗次數"] = _dt.Rows[i]["total"];
                        dr["錯誤登入來源"] = _dt.Rows[i]["login_error_from"];
                        dr["登入日期區間"] = start_time + " ~ " + end_time;
                        
                        _newDt.Rows.Add(dr);
                    }
                }

                if (_newDt.Rows.Count > 0)
                {
                    MailBody = GetHtmlByDataTable(_newDt);
                    //MailBody = "<br/><font size=\"4\">   在 " + "<font color=\"#FF0000\" >" + start_time + " ~ " + end_time + "</font>" + " 的一個小時里，用戶登錄異常記錄如下：</font><br/><p/>" + GetHtmlByDataTable(_newDt);
                    MailHelper mail = new MailHelper(mailModel);
                   // mail.SendToGroup(GroupCode, MailTitle, MailBody + " ", IsSeparate, IsDisplyName);
                    mail.SendToGroup(GroupCode, MailTitle, MailBody + " ", false, true);
                }
                else
                {
                    //MailBody = "<br/><p><font size=\"4\">    在 <font color=\"#FF0000\" >" + start_time + " ~ " + end_time + "</font> 的一個小時里，沒有用戶登錄異常的記錄!</font><p/>";

                }
                
            }
            catch (Exception ex)
            {
                throw new Exception("CheckUnsafeLoginMgr-->Start-->" + ex.Message);
            }
            return true;
        }
        static string GetHtmlByDataTable(DataTable _dt)
        {
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("<table  cellpadding=3 cellspacing=1  border=1 style=\"border-collapse: collapse\">");
            sbHtml.Append("<tr  style=\"text-align: center; COLOR: #0076C8; BACKGROUND-COLOR: #F4FAFF; font-weight: bold\">");
            string[] str = { "style=\"background-color:#dda29a;\"", "style=\"background-color:#d98722;\"", "style=\"background-color:#cfbd2d;\"", "style=\"background-color:#cbd12c;\"", "style=\"background-color:#91ca15;\"", "style=\"background-color:#6dc71e;\"", "style=\"background-color:#25b25c;\"", "style=\"background-color:#13a7a2;\"" };
            string aligns = "align=\"right\"";

            
            for (int i = 0; i < _dt.Columns.Count; i++)
            {
                sbHtml.Append("<th ");
                sbHtml.Append(str[i]);
                sbHtml.Append(" >");
                sbHtml.Append(_dt.Columns[i].ColumnName);
                sbHtml.Append("</th>");
            }
            sbHtml.Append("</tr>");
            for (int i = 0; i < _dt.Rows.Count; i++)//行
            {
                //sbHtml.Append("<tr>");
                //sbHtml.Append("<td ");                
                //sbHtml.Append(" >");
                //sbHtml.Append(i + 1);
                //sbHtml.Append("</td>");
                for (int j = 0; j < _dt.Columns.Count; j++)
                {
                    sbHtml.Append("<td ");
                    sbHtml.Append(aligns);
                    sbHtml.Append(" >");
                    sbHtml.Append(_dt.Rows[i][j]);
                    sbHtml.Append("</td>");
                }
                sbHtml.Append("</tr>");
            }
            sbHtml.Append("</table>");
            return sbHtml.ToString();

        }
        //static string GetHtmlByDataTable(DataTable _dtmyMonth)
        //{
        //    System.Text.StringBuilder sbHtml = new System.Text.StringBuilder();
        //    //sbHtml.Append("<table  cellpadding=3 cellspacing=1  border=2 style=\"border:white solid #ccc; \">");
        //    sbHtml.Append("<table cellpadding=2 cellspacing=2  style='border: #d1d1d1 solid;border-width:1 0 1 0'>");//style='border-collapse: collapse '
        //    sbHtml.Append("<tr style=\"text-align: center; COLOR: black; BACKGROUND-COLOR: #99ccff; font-weight: bold\">");//B3D4FF

        //    string aligns = "align=\"center\"";
        //    string color = "style=\"background-color:#eeeeee;\"";//單數行的樣式f0f0f0 dcb5ff  e0e0e0 ffeedd

        //    sbHtml.Append("<th ");
        //    sbHtml.Append(" >");
        //    sbHtml.Append("行號");
        //    sbHtml.Append("</th>");

        //    //插入列頭
        //    for (int i = 0; i < _dtmyMonth.Columns.Count; i++)
        //    {
        //        sbHtml.Append("<th ");
        //        sbHtml.Append(" >");
        //        sbHtml.Append(_dtmyMonth.Columns[i].ColumnName);
        //        sbHtml.Append("</th>");
        //    }
        //    sbHtml.Append("</tr>");
        //    //插入數據，單數行設置背景色
        //    for (int i = 0; i < _dtmyMonth.Rows.Count; i++)//行
        //    {
        //        sbHtml.Append("<tr>");

        //        sbHtml.Append("<td ");
        //        if (i % 2 == 0)
        //        {
        //            sbHtml.Append(aligns + color);
        //        }
        //        else
        //        {
        //            sbHtml.Append(aligns);
        //        }

        //        sbHtml.Append(" >");
        //        sbHtml.Append(i + 1);
        //        sbHtml.Append("</td>");
        //        for (int j = 0; j < _dtmyMonth.Columns.Count; j++)
        //        {
        //            sbHtml.Append("<td ");
        //            if (i % 2 == 0)
        //            {
        //                sbHtml.Append(aligns + color);
        //            }
        //            else
        //            {
        //                sbHtml.Append(aligns);
        //            }
        //            sbHtml.Append(" >");
        //            sbHtml.Append(_dtmyMonth.Rows[i][j]);
        //            sbHtml.Append("</td>");
        //        }
        //        sbHtml.Append("</tr>");
        //    }
        //    sbHtml.Append("</table>");
        //    return sbHtml.ToString();

        //}       
    }
}
