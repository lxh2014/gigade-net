using BLL.gigade.Common;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
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
    public class ReceiveStatusFromTCatMgr
    {
        private ScheduleServiceMgr _secheduleServiceMgr;

        static IDBAccess _accessMySql;
        static string mySqlConnectionString = string.Empty;
        public ReceiveStatusFromTCatMgr(string connectionString)
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
                string ftpServerIP = string.Empty;
                string filename = string.Empty;
                string ftpUserID = string.Empty;
                string ftpPassword = string.Empty;
                string localPath_1 = string.Empty;
                string localPath_2 = string.Empty;

                //獲取該排程參數
                List<ScheduleConfigQuery> store_config = new List<ScheduleConfigQuery>();
                ScheduleConfigQuery query_config = new ScheduleConfigQuery();
                query_config.schedule_code = schedule_code;
                _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
                store_config = _secheduleServiceMgr.GetScheduleConfig(query_config);

                #region FTP參數賦值
                foreach (ScheduleConfigQuery item in store_config)
                {
                    if (item.parameterCode.Equals("FtpSite"))
                    {
                        ftpServerIP = item.value;
                    }
                    else if (item.parameterCode.Equals("ftpuser"))
                    {
                        ftpUserID = item.value;
                    }
                    else if (item.parameterCode.Equals("ftppassword"))
                    {
                        ftpPassword = item.value;
                    }
                    else if (item.parameterCode.Equals("localPath_1"))//下載的文件保存路徑（本地）
                    {
                        localPath_1 = item.value;
                    }
                    else if (item.parameterCode.Equals("localPath_2"))//下載的文件保存路徑（本地）
                    {
                        localPath_2 = item.value;
                    } 
                }             
                #endregion

                bool localPath1Bool = true; 
                try
                {
                    if (!Directory.Exists(localPath_1))
                    {
                        Directory.CreateDirectory(localPath_1);
                    }
                }
                catch(Exception ex)
                {
                    localPath1Bool = false;
                    string str1 = " 參數 localPath_1 有問題，創建路徑（保存下載文件）失敗，失敗的原因：" + ex.Message;
                    SendMail(schedule_code, str1);
                    throw new Exception(ex.Message);
                }
              

                #region FTP下載
                /// <summary>
                /// FTP下載
                /// </summary>
                string fileContentStr = string.Empty;
                bool result = false;
                if (localPath1Bool)
                {
                    try
                    {
                        string filePath = localPath_1;//下載保存文件路徑
                        result = DownloadFTP(filePath, ftpServerIP, ftpUserID, ftpPassword);
                    }
                    catch (Exception ex)
                    {
                        int index = ex.Message.LastIndexOf("-->");
                        int subStrLeng = index + 3;
                        string errorMessage = ex.Message.Substring(subStrLeng, ex.Message.Length - subStrLeng);
                        string str = "sod文件下載失敗，失敗的原因：" + errorMessage;
                        SendMail(schedule_code, str);
                        throw new Exception(ex.Message);
                    }
                }
               
               

                if (result)//下載成功后，插入數據，轉移文件，發送郵件
                {               
                    string[] filenames = Directory.GetFiles(localPath_1, "*.sod");


                    #region 循環讀取文件，插入數據
                    /// <summary>
                    /// 循環讀取文件，插入數據
                    /// </summary>
                    foreach (string file in filenames)
                    {
                        string localFilePath_1 = file;
                        if (File.Exists(localFilePath_1))
                        {
                            FileStream fs = new FileStream(localFilePath_1, FileMode.Open, FileAccess.Read);//创建写入文件                        
                            StreamReader readSr = new StreamReader(fs, Encoding.GetEncoding("big5"));

                            fileContentStr = readSr.ReadToEnd();//开始讀取值
                            readSr.Close();
                            fs.Close();
                        }
                        string[] array1 = fileContentStr.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);//數據數組
                        for (int i = 0; i < array1.Length; i++)//循環插入數據
                        {
                            string[] array2 = new string[] { };
                            array2 = array1[i].Split(new string[] { "|" }, StringSplitOptions.None);//欄位數組

                            string year = array2[3].ToString().Substring(0, 4);
                            string month = array2[3].ToString().Substring(4, 2);
                            string day = array2[3].ToString().Substring(6, 2);
                            string hour = array2[3].ToString().Substring(8, 2);
                            string minute = array2[3].ToString().Substring(10, 2);
                            string second = array2[3].ToString().Substring(12, 2);
                            string delivery_status_time = year + "-" + month + "-" + day + " " + hour + ":" + minute + ":" + second;

                            StringBuilder InsertSql = new StringBuilder("");
                            StringBuilder selectSql = new StringBuilder("");
                            selectSql.AppendFormat(@"select order_id  from logistics_tcat_sod where delivery_number='{0}' and order_id='{1}' 
                                            and delivery_status_time='{2}' and status_id='{3}'", array2[0], array2[1], array2[3], array2[5]);

                            DataTable selDT = _accessMySql.getDataTable(selectSql.ToString());
                            if (selDT.Rows.Count == 0)//如果數據已經存在，不插入
                            {
                                InsertSql.AppendFormat(@" INSERT INTO logistics_tcat_sod (delivery_number,order_id,station_name,delivery_status_time,customer_id,status_id,
                                   status_note,specification,create_date) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}'); ",
                                      array2[0], array2[1], array2[2], delivery_status_time, array2[4], array2[5], array2[6], array2[7], DateTime.Now.ToString("yy-MM-dd HH:mm:ss"));
                                _accessMySql.execCommand(InsertSql.ToString());
                            }
                        }
                    } 
                    #endregion


                    #region 循環轉移文件
                    /// <summary>
                    /// 循環轉移文件
                    /// </summary>
                    if (filenames.Length > 0)
                    {
                        bool localPath2Bool = true;
                        foreach (string file in filenames)
                        {
                            int index = file.LastIndexOfAny(new char[] { '/', '\\' });
                            int subStrLeng = index + 1;
                            string localFilePath_1 = file;
                            string localFilePath_2 = localPath_2 + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + file.Substring(subStrLeng, file.Length - subStrLeng);

                            
                            try
                            {
                                //轉移文件
                                if (!Directory.Exists(localPath_2 + "\\" + DateTime.Now.ToString("yyyyMMdd")))
                                {
                                    Directory.CreateDirectory(localPath_2 + "\\" + DateTime.Now.ToString("yyyyMMdd"));
                                }
                                if (File.Exists(localFilePath_1))
                                {
                                    File.Move(localFilePath_1, localFilePath_2);
                                }
                                //if (!File.Exists(localFilePath_2))//本地文件（处理过的文件保存）存在
                                //{
                                //    FileStream fs2 = new FileStream(localFilePath_2, FileMode.Create, FileAccess.Write);//创建写入文件 
                                //    FileStream fs1 = new FileStream(localFilePath_1, FileMode.Open);
                                //    StreamReader sr = new StreamReader(fs1, Encoding.GetEncoding("big5"));
                                //    StreamWriter sw = new StreamWriter(fs2, Encoding.GetEncoding("big5"));
                                //    sw.Write(sr.ReadToEnd());//开始写入值
                                //    sw.Close();
                                //    fs1.Close();
                                //}
                                //else
                                //{
                                //    FileStream fs2 = new FileStream(localFilePath_2, FileMode.Truncate, FileAccess.Write);//创建写入文件 
                                //    FileStream fs1 = new FileStream(localFilePath_1, FileMode.Open);
                                //    StreamReader sr = new StreamReader(fs1, Encoding.GetEncoding("big5"));
                                //    StreamWriter sw = new StreamWriter(fs2, Encoding.GetEncoding("big5"));//Big5
                                //    sw.Write(sr.ReadToEnd());//开始写入值
                                //    sw.Close();
                                //    fs1.Close();
                                //}
                            }
                            catch (Exception ex)
                            {
                                localPath2Bool = false;
                                string str1 = " sod文件下載成功，數據庫更新成功。" + "但是該文件在本地保存失敗，失敗的原因：" + ex.Message;                                
                                SendMail(schedule_code, str1);
                                throw new Exception(ex.Message);
                            }
                        }

                        if (localPath2Bool)
                        {
                            //所有操作都執行成功
                            string downLoadSuccess = "sod文件下載成功，所有操作都執行成功";
                            SendMail(schedule_code, downLoadSuccess);
                        }                       
                    }
                    else
                    {
                        string downLoadNull = "沒有要下載的sod文件";
                        SendMail(schedule_code, downLoadNull);
                    }
                     
                    #endregion                  
                }
                #endregion

            }
            catch (Exception ex)
            {
                throw new Exception("ReceiveStatusFromTCatMgr-->Start-->" + ex.Message);
            }
            return true;
        }

        private bool UploadFTP(string ftpServerIP, string filePath, string ftpUserID, string ftpPassword)
        {

            FileInfo fileInf = new FileInfo(filePath);
            string uri = string.Format("{0}/{1}", ftpServerIP, fileInf.Name);
            FtpWebRequest reqFTP;
            // 根据uri创建FtpWebRequest对象 
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            // ftp用户名和密码
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            // 默认为true，连接不会被关闭
            // 在一个命令之后被执行
            reqFTP.KeepAlive = false;
            // 指定执行什么命令
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            // 指定数据传输类型
            reqFTP.UseBinary = true;
            // 上传文件时通知服务器文件的大小
            reqFTP.ContentLength = fileInf.Length;
            // 缓冲大小设置为2kb
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            // 打开一个文件流 (System.IO.FileStream) 去读上传的文件
            FileStream fs = fileInf.OpenRead();
            try
            {
                // 把上传的文件写入流
                Stream strm = reqFTP.GetRequestStream();
                // 每次读文件流的2kb
                contentLen = fs.Read(buff, 0, buffLength);
                // 流内容没有结束
                while (contentLen != 0)
                {
                    // 把内容从file stream 写入 upload stream
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                // 关闭两个流
                strm.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                //return false;
                throw new Exception("ReceiveStatusFromTCatMgr-->UploadFTP-->" + ex.Message);
            }
            return true;
        }

        private bool DownloadFTP(string filePath, string ftpServerIP, string ftpUserID, string ftpPassword)
        {
            FtpWebRequest reqFTP;
            try
            {
                BLL.gigade.Common.FTP FTP = new Common.FTP(ftpServerIP, ftpUserID, ftpPassword);
                List<string> fileList = FTP.GetFileList();
                foreach (string item in fileList)
                {
                    string uri = string.Format("{0}/{1}", ftpServerIP, item);
                    FileStream outputStream = new FileStream(filePath + "\\" + item, FileMode.Create);                   
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));// 根据uri创建FtpWebRequest对象 
                    reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                    reqFTP.UseBinary = true;
                    reqFTP.KeepAlive = false;
                    reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    Stream ftpStream = response.GetResponseStream();
                    long cl = response.ContentLength;
                    int bufferSize = 2048;
                    int readCount;
                    byte[] buffer = new byte[bufferSize];
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        outputStream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                    }
                    ftpStream.Close();
                    outputStream.Close();
                    response.Close();
                }
                              
            }
            catch (Exception ex)
            {
                throw new Exception("ReceiveStatusFromTCatMgr-->DownloadFTP-->" + ex.Message);
            }
            return true;
        }

        public void SendMail(string schedule_code, string info)
        {
            MailModel mailModel = new MailModel();
            mailModel.MysqlConnectionString = mySqlConnectionString;

            string GroupCode = string.Empty;
            string MailTitle = string.Empty;
            string MailBody = string.Empty;
            bool IsSeparate = true;
            bool IsDisplyName = true;

            try
            {
                //獲取該排程參數
                List<ScheduleConfigQuery> store_config = new List<ScheduleConfigQuery>();
                ScheduleConfigQuery query_config = new ScheduleConfigQuery();
                query_config.schedule_code = schedule_code;
                _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
                store_config = _secheduleServiceMgr.GetScheduleConfig(query_config);

                #region mailModel賦值
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
                }
                MailBody = info;
                #endregion

                MailHelper mail = new MailHelper(mailModel);
                mail.SendToGroup(GroupCode, MailTitle, MailBody + " ", IsSeparate, IsDisplyName);
            }
            catch (Exception ex)
            {
                throw new Exception("SendOrderInfoToBlackCatFTPMgr-->SendMail-->" + ex.Message);
            }
        }
    }
}
