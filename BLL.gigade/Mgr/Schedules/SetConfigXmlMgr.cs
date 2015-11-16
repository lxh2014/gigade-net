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
    public class SetConfigXmlMgr
    {
        private ScheduleServiceMgr _secheduleServiceMgr;

        static IDBAccess _accessMySql;
        static string mySqlConnectionString = string.Empty;
        public SetConfigXmlMgr(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            mySqlConnectionString = connectionString;
        }

        public bool Start(string schedule_code)
        {

            string json = string.Empty;
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
                string filepath = string.Empty;
                string ftpuser = string.Empty;
                string ftppassword = string.Empty;

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
                    else if (item.parameterCode.Equals("filepath"))
                    {
                        filepath = item.value;
                    }
                    else if (item.parameterCode.Equals("ftpuser"))
                    {
                        ftpuser = item.value.Trim();
                    }
                    else if (item.parameterCode.Equals("ftppassword"))
                    {
                        ftppassword = item.value.Trim();
                    }
                }
                string mailBody = string.Empty;
                #region  統計目錄內容
                string[] filepaths = filepath.Split(';');
                for (int i = 0; i < filepaths.Length; i++)
                {
                    if (string.IsNullOrEmpty(filepaths[i].Trim()))
                    {
                        continue;
                    }
                    //判斷是否為FTP路徑
                    string aaa = filepaths[i].Trim().ToLower().ToString();
                    if (filepaths[i].Trim().ToLower().ToString().Contains("ftp:"))
                    {
                        if (string.IsNullOrEmpty(ftpuser) || string.IsNullOrEmpty(ftppassword))
                        {
                            mailBody += "<h3>" + (i + 1).ToString() + "." + "目錄 " + filepaths[i] + "需要的FTP賬號或密碼沒有設定！" + "</h3>";
                        }
                        else
                        {
                            mailBody += GetFtpContent(filepaths[i].Trim(), ftpuser, ftppassword,i);
                        }

                    }
                    //判斷路徑是否正確
                    else if (!Directory.Exists(filepaths[i]))
                    {
                        mailBody += "<h3>" + (i + 1).ToString() + "." + "目錄 " + filepaths[i] + "不存在，請修改文件路徑！" + "</h3>";
                    }
                    //判斷路徑下是否有文件
                    else if (Directory.GetFiles(filepaths[i]).Length == 0)
                    {
                        mailBody += "<h3>" + (i + 1).ToString() + "." + "目錄 " + filepaths[i] + "為空目錄！" + "</h3>";
                    }
                    else if (Directory.GetFiles(filepaths[i]).Length > 0)
                    {
                        string Content = "<h3>" + (i + 1).ToString() + "." + "目錄 " + filepaths[i] + " :" + "</h3>";
                        string[] files = Directory.GetFiles(filepaths[i]);
                        for (int j = 0; j < files.Length; j++)
                        {
                            FileInfo info = new FileInfo(files[j]);
                            files[j] = info.Name;
                            Content += "<h5>" + "(" + (j + 1).ToString() + ")" + files[j] + "</h5>";
                        }
                        mailBody += Content;
                    }
                }
                #endregion
                MailHelper mail = new MailHelper(mailModel);
                mail.SendToGroup(GroupCode, MailTitle, mailBody, false, false);//發送郵件


            }
            catch (Exception ex)
            {
                throw new Exception("SetConfigXmlMgr-->Start-->" + ex.Message);
            }
            return true;
        }
        private string GetFtpContent(string url, string ftpuser, string ftppassword,int i)
        {
            string result = string.Empty;
            try
            {
               
                FTP ftp = new FTP(url, ftpuser, ftppassword);
                List<string> files = ftp.GetFileList();

                if (files.Count == 0)
                {
                    result += "<h3>" + (i + 1).ToString() + "." + "目錄 " + url + "為空目錄！" + "</h3>";
                }
                else if (files.Count > 0)
                {
                    result += "<h3>" + (i + 1).ToString() + "." + "目錄 " + url + " :" + "</h3>";
                    for (int j = 0; j < files.Count; j++)
                    {
                        result += "<h5>" + "(" + (j + 1).ToString() + ")" + files[j] + "</h5>";
                    }
                    
                }
            }
            catch (Exception )
            {
                result += "<h3>" + (i + 1).ToString() + "." + "目錄 " + url + "不存在，或者FTP賬號密碼錯誤！" + "</h3>";
            }
            return result;
        }
    }
}
