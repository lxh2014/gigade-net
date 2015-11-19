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
                string EmptyFilesPrompt = string.Empty;
                string ExistFilesPrompt = string.Empty;
                string NotExistDirectory = string.Empty;
                string NotExistFtpDirectory = string.Empty;

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
                    else if (item.parameterCode.Equals("EmptyFilesPrompt"))
                    {
                        EmptyFilesPrompt = item.value.Trim();
                    }
                    else if (item.parameterCode.Equals("ExistFilesPrompt"))
                    {
                        ExistFilesPrompt = item.value.Trim();
                    }
                    else if (item.parameterCode.Equals("NotExistDirectory"))
                    {
                        NotExistDirectory = item.value.Trim();
                    }
                    else if (item.parameterCode.Equals("NotExistFtpDirectory"))
                    {
                        NotExistFtpDirectory = item.value.Trim();
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
                    if (filepaths[i].Trim().ToLower().ToString().Contains("ftp:"))
                    {
                        if (string.IsNullOrEmpty(ftpuser) || string.IsNullOrEmpty(ftppassword))
                        {
                            mailBody += "<h3>" + (i + 1).ToString() + "." + "目錄 " + filepaths[i] + "需要的FTP賬號或密碼沒有設定！" + "</h3>";
                        }
                        else
                        {
                            mailBody += GetFtpContent(filepaths[i].Trim(), ftpuser, ftppassword, i, EmptyFilesPrompt, ExistFilesPrompt, NotExistFtpDirectory);
                        }

                    }
                    //判斷路徑是否正確
                    else if (!Directory.Exists(filepaths[i]))
                    {
                        mailBody += "<h3>" + (i + 1).ToString() + "." + "目錄 " + filepaths[i] + NotExistDirectory + "</h3>";
                    }
                    //判斷路徑下是否有文件
                    else if (Directory.GetFiles(filepaths[i]).Length == 0)
                    {
                        mailBody += "<h3>" + (i + 1).ToString() + "." + "目錄 " + filepaths[i] + EmptyFilesPrompt + "</h3>";
                    }
                    else if (Directory.GetFiles(filepaths[i]).Length > 0)
                    {
                        string Content = "<h3>" + (i + 1).ToString() + "." + "目錄 " + filepaths[i] + ExistFilesPrompt + "</h3>";
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
        private string GetFtpContent(string url, string ftpuser, string ftppassword, int i, string EmptyFilesPrompt, string ExistFilesPrompt, string NotExistFtpDirectory)
        {
            string result = string.Empty;
            try
            {
                List<string> files = GetFileNameList(url, ftpuser, ftppassword);

                if (files.Count == 0)
                {
                    result += "<h3>" + (i + 1).ToString() + "." + "目錄 " + url + EmptyFilesPrompt + "</h3>";
                }
                else if (files.Count > 0)
                {
                    result += "<h3>" + (i + 1).ToString() + "." + "目錄 " + url + ExistFilesPrompt + "</h3>";
                    for (int j = 0; j < files.Count; j++)
                    {
                        result += "<h5>" + "(" + (j + 1).ToString() + ")" + files[j] + "</h5>";
                    }
                    
                }
            }
            catch (Exception )
            {
                result += "<h3>" + (i + 1).ToString() + "." + "目錄 " + url + NotExistFtpDirectory + "</h3>";
            }
            return result;
        }
        public List<string> GetFileNameList(string url, string ftpuser, string ftppassword)
        {
            FtpWebRequest ftpRequest;

            ftpRequest = FtpWebRequest.Create(url) as FtpWebRequest;
            ftpRequest.Credentials = new NetworkCredential(ftpuser, ftppassword);
            ftpRequest.KeepAlive = false;
            ftpRequest.UsePassive = false;
            ftpRequest.UseBinary = true;
            ftpRequest.Timeout = 3000; // 无效   

            List<string> result = new List<string>();
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            FtpWebResponse response = ftpRequest.GetResponse() as FtpWebResponse;
            StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);//中文文件名
            //string line = reader.ReadLine();
            //while (line != null)
            //{
            //    result.Add(line);
            //    line = reader.ReadLine();
            //}
            //return result;
            string Datastring = reader.ReadToEnd();
            List<FileStruct> filesStruct =  GetList(Datastring);
            List<string> filesName = new List<string>();
            foreach (FileStruct item in filesStruct)
            {
                if (!item.IsDirectory)
                {
                    filesName.Add(item.Name);
                }
            }
            return filesName;
        }
        #region 文件结构 FileStruct
        /// <summary>
        /// 文件结构
        /// </summary>
        public class FileStruct
        {
            public string Flags;
            public string Owner;
            public string Group;
            public bool IsDirectory;
            public DateTime CreateTime;
            public string Name;
        }
        #endregion

        #region 获得文件和目录列表 -List<FileStruct> GetList(string datastring)
        /// <summary>
        /// 获得文件和目录列表
        /// </summary>
        /// <param name="datastring">FTP返回的列表字符信息</param>
        private List<FileStruct> GetList(string datastring)
        {
            List<FileStruct> myListArray = new List<FileStruct>();
            string[] dataRecords = datastring.Split('\n');
            //FileListStyle _directoryListStyle = GuessFileListStyle(dataRecords);
            foreach (string s in dataRecords)
            {
                if (s != "")
                {
                    FileStruct f = new FileStruct();
                    f = ParseFileStructFromUnixStyleRecord(s);
                    myListArray.Add(f);
                }
            }

            return myListArray;
        }
        #endregion

        #region 按照一定的规则进行字符串截取 -string _cutSubstringFromStringWithTrim(ref string s, char c, int startIndex)
        /// <summary>
        /// 按照一定的规则进行字符串截取
        /// </summary>
        /// <param name="s">截取的字符串</param>
        /// <param name="c">查找的字符</param>
        /// <param name="startIndex">查找的位置</param>
        private string _cutSubstringFromStringWithTrim(ref string s, char c, int startIndex)
        {
            int pos1 = s.IndexOf(c, startIndex);
            string retString = s.Substring(0, pos1);
            s = (s.Substring(pos1)).Trim();
            return retString;
        }
        #endregion

        #region Unix格式中返回文件信息 -FileStruct ParseFileStructFromUnixStyleRecord(string Record)
        /// <summary>
        /// 从Unix格式中返回文件信息
        /// </summary>
        /// <param name="Record">文件信息</param>
        private FileStruct ParseFileStructFromUnixStyleRecord(string Record)
        {
            FileStruct f = new FileStruct();
            string processstr = Record.Trim();
            f.Flags = processstr.Substring(0, 10);
            f.IsDirectory = (f.Flags[0] == 'd');
            processstr = (processstr.Substring(11)).Trim();
            _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);   //跳过一部分
            f.Owner = _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            f.Group = _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);   //跳过一部分
            string yearOrTime = processstr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
            if (yearOrTime.IndexOf(":") >= 0)  //time
            {
                processstr = processstr.Replace(yearOrTime, DateTime.Now.Year.ToString());
            }
            f.CreateTime = DateTime.Parse(_cutSubstringFromStringWithTrim(ref processstr, ' ', 8));
            f.Name = processstr;   //最后就是名称
            return f;
        }
        #endregion
    }
}
