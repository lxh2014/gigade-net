using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using DBAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace BLL.gigade.Mgr.Schedules
{
    public class CheckIPAddressMgr
    {
        static IDBAccess _accessMySql;
        static string mySqlConnectionString = string.Empty;
        public CheckIPAddressMgr(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            mySqlConnectionString = connectionString;
        }

        public bool Start(string schedule_code)
        {
            string b = @"{'code':0,'data':{'country':'\u4e2d\u56fd','country_id':'CN','area':'\u534e\u4e2d','area_id':'400000','region':'\u6cb3\u5357\u7701','region_id':'410000','city':'\u90d1\u5dde\u5e02','city_id':'410100','county':'','county_id':'-1','isp':'\u8054\u901a','isp_id':'100026','ip':'115.60.75.15'}}";
            string source = "\u534e\u4e2d\u4e2d\u56fd";
            //string ip = "60.249.127.62";
            string cd = source;
            //GetMessageByIP(ip);

            DataTable userOtherLogin = GetUserLoginDataTable();
            string mailBody = GetHtmlByDataTable(userOtherLogin);
            CodeData codeData = JsonConvert.DeserializeObject<CodeData>(b);
            IPMessage ipMessage = JsonConvert.DeserializeObject<IPMessage>(codeData.data.ToString());

            MailModel mailModel = new MailModel();
            mailModel.MysqlConnectionString = mySqlConnectionString;

            string GroupCode = string.Empty;
            string MailTitle = string.Empty;
            //獲取該排程參數
            List<ScheduleConfigQuery> store_config = new List<ScheduleConfigQuery>();
            ScheduleConfigQuery query_config = new ScheduleConfigQuery();
            query_config.schedule_code = schedule_code;
            ScheduleServiceMgr _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
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
            }

            MailHelper mail = new MailHelper(mailModel);
            if (userOtherLogin.Rows.Count == 0)
            {
                return mail.SendToGroup(GroupCode, MailTitle,mailBody, false, true);
            }
            return true;
        }

        private static DataTable GetUserLoginDataTable()
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, mySqlConnectionString);
            DataTable userLogin = _accessMySql.getDataTable(string.Format("select login_id,user_id,login_ipfrom, FROM_UNIXTIME(login_createdate) login_createdate from users_login where FROM_UNIXTIME(login_createdate)>'{0}' ", CommonFunction.DateTimeToString(DateTime.Now.AddHours(-1))));
            DataTable othenCityUser = new DataTable();
            othenCityUser.Columns.Add("會員編號", typeof(string));
            othenCityUser.Columns.Add("IP", typeof(string));
            othenCityUser.Columns.Add("登錄地區", typeof(string));
            othenCityUser.Columns.Add("登錄時間", typeof(string));


            foreach (DataRow drUser in userLogin.Rows)
            {
                System.Threading.Thread.Sleep(1000);
                DataRow newRow = othenCityUser.NewRow();
                newRow["會員編號"] = drUser["user_id"];
                newRow["IP"] = drUser["login_ipfrom"];
                string ip = drUser["login_ipfrom"].ToString();
                IPMessage ipMessage = new IPMessage();
                try
                {
                    ipMessage = GetMessageByIP(ip);
                    newRow["登錄地區"] = ipMessage.country.ToString() + ipMessage.city;
                    if (ipMessage.country_id.ToString() == "TW")
                    {
                        continue;
                    }
                }
                catch (Exception)
                {

                    newRow["登錄地區"] = "無資訊";
                }

                newRow["登錄時間"] = drUser["login_createdate"];

                othenCityUser.Rows.Add(newRow);
            }
            return othenCityUser;
        }
        private static IPMessage GetMessageByIP(string ip)
        {
            string errorMesage = String.Empty;
            IPMessage ipMessage = new IPMessage();
            try
            {
                string url = "http://ip.taobao.com/service/getIpInfo.php?ip=";
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url + ip);
                httpRequest.Timeout = 10000;
                httpRequest.Method = "GET";
                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                StreamReader sr = new StreamReader(httpResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                string result = sr.ReadToEnd();
                result = result.Replace("\r", "").Replace("\n", "").Replace("\t", "");
                //helper.SendToGroup(GroupCode, "rgf", json, true, true);
                int status = (int)httpResponse.StatusCode;
                //ErrorLogHelper loghelper = new ErrorLogHelper("請求ip地址信息出錯" + result);
                CodeData codeData = JsonConvert.DeserializeObject<CodeData>(result);
                ipMessage = JsonConvert.DeserializeObject<IPMessage>(codeData.data.ToString());
                sr.Close();
                return ipMessage;
            }
            catch (Exception ex)
            {
                errorMesage += ex.Message;
                //ErrorLogHelper loghelper = new ErrorLogHelper("請求ip地址信息出錯"+ex.ToString());
                Console.WriteLine(ex);
                return ipMessage;
            }
        }
        public static string GetHtmlByDataTable(DataTable _dtmyMonth)
        {
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("<table  cellpadding=3 cellspacing=1  border=1 style=\"border-collapse: collapse\">");
            sbHtml.Append("<tr  style=\"text-align: center; COLOR: #0076C8; BACKGROUND-COLOR: #F4FAFF; font-weight: bold\">");
            string[] str = { "style=\"background-color:#dda29a;\"", "style=\"background-color:#d98722;\"", "style=\"background-color:#cfbd2d;\"", "style=\"background-color:#cbd12c;\"", "style=\"background-color:#91ca15;\"", "style=\"background-color:#6dc71e;\"", "style=\"background-color:#25b25c;\"", "style=\"background-color:#13a7a2;\"", "style=\"background-color:#13b7a2;\"" };
            string aligns = "align=\"right\"";
            string alignsleft = "align=\"left\"";
            for (int i = 0; i < _dtmyMonth.Columns.Count; i++)
            {
                sbHtml.Append("<th ");
                sbHtml.Append(str[i]);
                sbHtml.Append(" >");
                sbHtml.Append(_dtmyMonth.Columns[i].ColumnName);
                sbHtml.Append("</th>");
            }
            sbHtml.Append("</tr>");
            for (int i = 0; i < _dtmyMonth.Rows.Count; i++)//行
            {
                sbHtml.Append("<tr>");
                for (int j = 0; j < _dtmyMonth.Columns.Count; j++)
                {
                    sbHtml.Append("<td ");
                    if (j == 0)
                    {
                        sbHtml.Append(alignsleft);
                    }
                    else
                    {
                        sbHtml.Append(aligns);
                    }
                    sbHtml.Append(" >");
                    sbHtml.Append(_dtmyMonth.Rows[i][j]);
                    sbHtml.Append("</td>");
                }
                sbHtml.Append("</tr>");
            }
            sbHtml.Append("</table>");
            return sbHtml.ToString();

        }
    }
    #region Model
    public class IPMessage
    {

        public object country { set; get; }
        public object country_id { set; get; }
        public object area { set; get; }
        public object area_id { set; get; }
        public object region { set; get; }
        public object region_id { set; get; }
        public object city { set; get; }
        public object city_id { set; get; }
        public object county { set; get; }
        public object county_id_id { set; get; }
        public object isp { set; get; }
        public object isp_id { set; get; }
        public object ip { set; get; }


    }

    public class CodeData
    {
        public object code { set; get; }
        public object data { set; get; }
    }
    #endregion

}
