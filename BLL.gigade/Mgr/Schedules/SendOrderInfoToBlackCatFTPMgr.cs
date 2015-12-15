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
using System.Web;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr.Schedules
{
    public class SendOrderInfoToBlackCatFTPMgr
    {
        private ScheduleServiceMgr _secheduleServiceMgr;
        private LogisticsTcatEodDao logisticsTcatEodDao;
        static IDBAccess _accessMySql;
        static string mySqlConnectionString = string.Empty;
        public SendOrderInfoToBlackCatFTPMgr(string connectionString)
        {
            logisticsTcatEodDao = new LogisticsTcatEodDao(connectionString);
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
                string newfilename = string.Empty;
                string ftpUserID = string.Empty;
                string ftpPassword = string.Empty;
                string localPath_1 = string.Empty;
                string localPath_2 = "c:\\EOD_TCAT";

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
                    else if (item.parameterCode.Equals("localPath_1"))//文件生成保存路徑（本地）
                    {
                        localPath_1 = item.value;
                    }
                    else if (item.parameterCode.Equals("localPath_2"))//文件上傳成功轉移路徑（本地）
                    {
                        localPath_2 = item.value;
                    }
                }
                newfilename = ftpUserID + DateTime.Now.ToString("MMddHH") + ".eod";//要上傳的文件的名稱
                //string localFilePath_1 = "F:\\" + newfilename; //本地文件路徑
                string localFilePath_1 = localPath_1 + "\\" + newfilename; //本地文件路徑,要上傳的文件路徑
                string localFilePath_2 = localPath_2 + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + newfilename; //本地文件路徑
                #endregion



                
                StringBuilder sb = new StringBuilder();
                DataTable lteDT = logisticsTcatEodDao.GetOrderInfoForTcat();
                              
                //有需要上傳的數據

                bool localPath1Bool = true; 
                if (lteDT.Rows.Count > 0)
                {
                    #region 要上傳的文件生成
                    /// <summary>
                    /// 生成eod文件
                    /// </summary>
                    /// <returns></returns>

                    try
                    {                          
                        string[] eodArrary = new string[lteDT.Rows.Count];
                        StringBuilder fileDataSb = new StringBuilder();

                        for (int i = 0; i < lteDT.Rows.Count; i++)
                        {
                            DataRow lteRow = lteDT.Rows[i];
                            for (int j = 0; j < lteDT.Columns.Count; j++)
                            {
                                if (j == 21)
                                {
                                    string deliveryDateStr = Convert.ToDateTime(lteRow[j]).ToString("yyyyMMddHHmmss");
                                    sb.AppendFormat("{0}|", deliveryDateStr);
                                }
                                else
                                {
                                    sb.AppendFormat("{0}|", lteRow[j].ToString());
                                }                          
                            }
                            eodArrary[i] = sb.ToString().Substring(0, sb.ToString().Length - 1);
                            sb.Clear();
                            if (i == 0)
                            {
                                fileDataSb.Append(eodArrary[i]);
                            }
                            else
                            {
                                fileDataSb.Append("\r\n" + eodArrary[i]);
                            }
                        }
                        if (!Directory.Exists(localPath_1))
                        {                          
                            Directory.CreateDirectory(localPath_1);
                        }
                        if (!File.Exists(localFilePath_1))//本地文件（要上傳的文件）存在
                        {
                            FileStream fs1 = new FileStream(localFilePath_1, FileMode.Create, FileAccess.Write);//创建写入文件 
                            StreamWriter sw = new StreamWriter(fs1, Encoding.GetEncoding("big5"));
                            sw.Write(fileDataSb.ToString());//开始写入值
                            sw.Close();
                            fs1.Close();
                        }
                        else
                        {
                            FileStream fs = new FileStream(localFilePath_1, FileMode.Truncate, FileAccess.Write);
                            StreamWriter sr = new StreamWriter(fs, Encoding.GetEncoding("big5"));//GetEncoding("Utf8")
                            sr.Write(fileDataSb.ToString());//开始写入值
                            sr.Close();
                            fs.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        localPath1Bool = false;
                        string str = "eod文件生成失敗，失敗的原因：" + ex.Message;
                        SendMail(schedule_code, str);
                        throw new Exception(ex.Message);
                    }
                    

                    #endregion


                    #region FTP上傳
                    /// <summary>
                    /// FTP上傳
                    /// </summary>
                    bool result = false;
                    if (localPath1Bool)
                    {
                        try
                        {
                            string filePath = localFilePath_1;//本地的文件路徑
                            result = UploadFTP(ftpServerIP, filePath, ftpUserID, ftpPassword);
                        }
                        catch (Exception ex)
                        {
                            int index = ex.Message.LastIndexOf("-->");
                            //int index1 = ex.Message.LastIndexOfAny(new char[] {'>'});
                            int subStrLeng = index + 3;
                            string errorMessage = ex.Message.Substring(subStrLeng, ex.Message.Length - subStrLeng);
                            string str = newfilename + " 文件上傳失敗，失敗的原因：" + errorMessage;
                            SendMail(schedule_code, str);
                            throw new Exception(ex.Message);
                        }
                    }
                                     
                
                    if (result)//上傳成功，更新 upload_time為當前時間，轉移文件，發送郵件
                    {

                        logisticsTcatEodDao.UpdateUploadTime();//更新 upload_time為當前時間

                        bool localPath2Bool = true;
                        try
                        {
                             //要上傳的文件的名稱                           
                            localFilePath_2 = localPath_2 + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + ftpUserID + DateTime.Now.ToString("MMddHHmmss") + ".eod";
                            if (!Directory.Exists(localPath_2 + "\\" + DateTime.Now.ToString("yyyyMMdd")))
                            {
                                Directory.CreateDirectory(localPath_2 + "\\" + DateTime.Now.ToString("yyyyMMdd"));
                            }                           
                            if (File.Exists(localFilePath_1))
                            {
                                File.Move(localFilePath_1, localFilePath_2);
                            }
                            //if (!File.Exists(localFilePath_2))//本地文件（要上傳的文件）存在
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
                            string str1 =  newfilename + " 文件上傳成功。"+"數據庫upload_time欄位更新成功，但是該文件在本地保存失敗，失敗的原因：" + ex.Message;
                            SendMail(schedule_code, str1);
                            throw new Exception(ex.Message);
                        }

                        if (localPath2Bool)
                        {
                            string str = newfilename + " 文件上傳成功，全部操作執行成功";//全部執行成功
                            SendMail(schedule_code, str);
                        }
                        
                     
                    }                                                 
                    #endregion
                   
                }
                else//沒有需要上傳的數據，發送郵件
                {
                    string str = "沒有要上傳的資料";
                    SendMail(schedule_code, str);
                }                        
            }
            catch (Exception ex)
            {               
                throw new Exception("SendOrderInfoToBlackCatFTPMgr-->Start-->" + ex.Message);
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
                throw new Exception("SendOrderInfoToBlackCatFTPMgr-->UploadFTP-->" + ex.Message);
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
            bool IsSeparate = false;
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
