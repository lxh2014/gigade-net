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
using BLL.gigade.Dao;
using Newtonsoft.Json;

namespace BLL.gigade.Mgr.Schedules
{
    public class WinningInvoiceSynchronismMgr
    {
        private ScheduleServiceMgr _secheduleServiceMgr;

        static IDBAccess _accessMySql;
        static string mySqlConnectionString = string.Empty;
        public WinningInvoiceSynchronismMgr(string connectionString)
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
            MailModel mailModel = new MailModel();
            mailModel.MysqlConnectionString = mySqlConnectionString;

            string GroupCode = string.Empty;
            string MailTitle = string.Empty;
            string MailBody = string.Empty;
            bool IsSeparate = false;
            bool IsDisplyName = true;
            string requestUrl = string.Empty;
            try
            {               
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
                    else if (item.parameterCode.Equals("MailTitle"))
                    {
                        MailTitle = item.value;
                    }
                    else if (item.parameterCode.Equals("requestUrl"))
                    {
                        requestUrl = item.value;
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

                int result = 0;
                int resultmes = 0;
                
                StringBuilder str = new StringBuilder();
                StringBuilder sqlstrall = new StringBuilder();
                InvoiceWinningNumberDao invoiceWinningNumberDao = new InvoiceWinningNumberDao(mySqlConnectionString);


                #region 獲取網頁里的數據，轉化成json字符串
                string urlone = "https://www.einvoice.nat.gov.tw/PB2CAPIVAN/invapp/InvApp?version=0.2&action=QryWinningList&invTerm=";
                string urltwo = "&appID=EINV5201502271601";
                string dateone = string.Empty;
                if (DateTime.Now.Month >= 10)
                {
                    if (DateTime.Now.Month == 11)
                    {
                        dateone = (DateTime.Now.Year - 1911).ToString() + (DateTime.Now.Month - 1);
                    }
                    else
                    {
                        dateone = (DateTime.Now.Year - 1911).ToString() + DateTime.Now.Month;
                    }
                }
                else if (DateTime.Now.Month == 1)
                {
                    dateone = (DateTime.Now.AddMonths(-1).Year - 1911).ToString() + "12";
                }
                else
                {
                    if (DateTime.Now.Month % 2 == 1)
                    {
                        dateone = (DateTime.Now.Year - 1911).ToString() + "0" + (DateTime.Now.Month - 1);
                    }
                    else
                    {
                        dateone = (DateTime.Now.Year - 1911).ToString() + "0" + DateTime.Now.Month;
                    }
                }
                string url = string.Empty;
                if (requestUrl.Trim() == string.Empty)
                {
                    url = urlone + dateone + urltwo;
                }
                else
                {
                    url = requestUrl;
                }                              

                string json = GetPage(url, "utf-8");
                ResultClass rc = new ResultClass();               
                #endregion

                MailHelper mail = new MailHelper(mailModel);
                int year = 0;
                int month = 0;
                string[,] strarray = new string[,]{};
                bool isHaveInfo = false;
                try//判斷如果出錯發郵件
                {
                    rc = JsonConvert.DeserializeObject<ResultClass>(json); 
                    year = Convert.ToInt32(rc.invoYm.Substring(0, 3));
                    month = Convert.ToInt32(rc.invoYm.Substring(3, 2));


                    strarray = new string[20,2] { { "superPrizeNo", rc.superPrizeNo }, { "spcPrizeNo", rc.spcPrizeNo }, { "spcPrizeNo2", rc.spcPrizeNo2 }, 
                                                  { "spcPrizeNo3", rc.spcPrizeNo3 }, { "firstPrizeNo1", rc.firstPrizeNo1 }, { "firstPrizeNo2", rc.firstPrizeNo2 }, 
                                                  { "firstPrizeNo3", rc.firstPrizeNo3 }, { "firstPrizeNo4", rc.firstPrizeNo4 }, { "firstPrizeNo5", rc.firstPrizeNo5 },
                                                  { "firstPrizeNo6", rc.firstPrizeNo6 }, { "firstPrizeNo7", rc.firstPrizeNo7 }, { "firstPrizeNo8", rc.firstPrizeNo8 }, 
                                                  { "firstPrizeNo9", rc.firstPrizeNo9 }, { "firstPrizeNo10", rc.firstPrizeNo10 }, { "sixthPrizeNo1", rc.sixthPrizeNo1 },
                                                  { "sixthPrizeNo2", rc.sixthPrizeNo2 }, { "sixthPrizeNo3", rc.sixthPrizeNo3 }, { "sixthPrizeNo4", rc.sixthPrizeNo4 }, 
                                                  { "sixthPrizeNo5", rc.sixthPrizeNo5 }, { "sixthPrizeNo6", rc.sixthPrizeNo6 } };
                    isHaveInfo = true;
                }
                catch (Exception ex)
                {
                    MailBody = "<p/><a href=" + url + ">" + url + "</a>" + "　該鏈接中未讀到數據!";                     
                    mail.SendToGroup(GroupCode, MailTitle, MailBody + " ", IsSeparate, IsDisplyName);
                    //throw new Exception(ex.Message);
                }
                if (isHaveInfo)//如果從鏈接中讀到數據
                {
                    for (int i = 0; i < strarray.GetLength(0); i++)
                    {
                        sqlstrall.AppendFormat(invoiceWinningNumberDao.ReturnInsertSql(year, month, strarray[i, 0].ToString(), strarray[i, 1].ToString()));
                    }
                    if (!string.IsNullOrEmpty(sqlstrall.ToString()))
                    {
                        result = invoiceWinningNumberDao.ResultOfExeInsertSql(sqlstrall.ToString());
                    }
                    else
                    {
                        resultmes = 1;//設置其大於0
                    }
                    if (result > 0)
                    {
                        MailBody = "<p/>執行成功";
                        mail.SendToGroup(GroupCode, MailTitle, MailBody + " ", IsSeparate, IsDisplyName);
                    }
                    else
                    {
                        if (resultmes > 0)
                        {
                            MailBody = "<p/>數據都已經存在";
                            mail.SendToGroup(GroupCode, MailTitle, MailBody + " ", IsSeparate, IsDisplyName);
                        }                       
                    }
                }             
            }
            catch (Exception ex)
            {
                MailHelper mail = new MailHelper(mailModel);
                MailBody = "<p/>執行失敗";
                mail.SendToGroup(GroupCode, MailTitle, MailBody + " ", IsSeparate, IsDisplyName);  
                throw new Exception("WinningInvoiceSynchronismMgr-->Start-->" + ex.Message);
            }
            return true;
        }

        #region 獲取到某個網頁下面的數據 + static string GetPage(string m_uri, string encode)
        static string GetPage(string m_uri, string encode)
        {
            WebResponse response = null;
            Stream stream = null;
            StreamReader reader = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(m_uri);
                request.Timeout = 300000;
                response = request.GetResponse();
                if (!request.HaveResponse)
                {
                    response.Close();
                    return null;
                }
                stream = response.GetResponseStream();
                Encoding encoding;
                string strEncoding = encode.ToLower();
                if (strEncoding == "utf-8")
                    encoding = Encoding.UTF8;
                else if (strEncoding == "utf-7")
                    encoding = Encoding.UTF7;
                else if (strEncoding == "unicode")
                    encoding = Encoding.Unicode;
                else
                    encoding = Encoding.Default;
                reader = new StreamReader(stream, encoding);
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (response != null) response.Close();
            }
        }
        #endregion

        public struct ResultClass
        {
            public string fifthPrizeAmt { get; set; }
            public string firstPrizeAmt { get; set; }
            public string firstPrizeNo1 { get; set; }
            public string firstPrizeNo10 { get; set; }
            public string firstPrizeNo2 { get; set; }
            public string firstPrizeNo3 { get; set; }
            public string firstPrizeNo4 { get; set; }
            public string firstPrizeNo5 { get; set; }
            public string firstPrizeNo6 { get; set; }
            public string firstPrizeNo7 { get; set; }
            public string firstPrizeNo8 { get; set; }
            public string firstPrizeNo9 { get; set; }
            public string fourthPrizeAmt { get; set; }
            public string invoYm { get; set; }
            public string secondPrizeAmt { get; set; }
            public string sixthPrizeAmt { get; set; }
            public string sixthPrizeNo1 { get; set; }
            public string sixthPrizeNo2 { get; set; }
            public string sixthPrizeNo3 { get; set; }
            public string sixthPrizeNo4 { get; set; }
            public string sixthPrizeNo5 { get; set; }
            public string sixthPrizeNo6 { get; set; }
            public string spcPrizeAmt { get; set; }
            public string spcPrizeNo { get; set; }
            public string spcPrizeNo2 { get; set; }
            public string spcPrizeNo3 { get; set; }
            public string superPrizeAmt { get; set; }
            public string superPrizeNo { get; set; }
            public string thirdPrizeAmt { get; set; }
            public TimeStamp timestamp { get; set; }
            public string updatedate { get; set; }
            public string v { get; set; }
            public string code { get; set; }
            public string msg { get; set; }
        }

        public struct TimeStamp
        {
            public int date { get; set; }
            public int day { get; set; }
            public int hours { get; set; }
            public int minutes { get; set; }
            public int month { get; set; }
            public int seconds { get; set; }
            public string time { get; set; }
            public string timezoneOffset { get; set; }
            public int year { get; set; }
        }

    }
}
