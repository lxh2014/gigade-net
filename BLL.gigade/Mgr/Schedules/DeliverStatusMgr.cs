using BLL.gigade.Common;
using BLL.gigade.Dao;
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
    public class DeliverStatusMgr
    {
        static IDBAccess _accessMySql;
        static string mySqlConnectionString = string.Empty;
        static string Url = "http://www.t-cat.com.tw/Inquire/Trace.aspx?no=";
        public DeliverStatusMgr(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            mySqlConnectionString = connectionString;
        }
        /// <summary>
        /// 根據物流單號獲取物流狀態
        /// </summary>
        /// <param name="delivery_code">物流單號</param>
        /// <returns>DeliveryInfo對象</returns>
        static DeliveryInfo GetWebContent(string delivery_code)
        {
            string strResult = "";
            DeliveryInfo model = new DeliveryInfo();
            model.delivery_code = delivery_code;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + delivery_code);
            request.Timeout = 30000;//设置连接超时时间 
            request.Headers.Set("Pragma", "no-cache");
            request.ContentType = "text/xml; charset=gb2312";
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1";
            request.ContentType = "application/x-www-form-urlencoded";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream streamReceive = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(streamReceive, Encoding.UTF8);
            strResult = streamReader.ReadToEnd();

            #region 抓取狀態
            string strCheckStatus = "<font color='red'>";
            int indexStatus = strResult.IndexOf(strCheckStatus);
            if (indexStatus != -1)
            {
                string status = strResult.Substring(indexStatus + strCheckStatus.Length, 4);
                if (status.Contains("<"))
                {
                    model.Status = status.Substring(0, 3);
                }
                else
                {
                    model.Status = status;
                }
            }
            #endregion

            #region 抓取創建時間
            string strCheckTime = "<div align='center'>            <span class='bl12'>";
            int indexCreateTime = strResult.IndexOf(strCheckTime);

            if (indexCreateTime != -1)
            {
                string date = strResult.Substring(indexCreateTime + strCheckTime.Length, 10);
                string time = strResult.Substring(indexCreateTime + strCheckTime.Length + 15, 5);
                DateTime dt = Convert.ToDateTime(date + " " + time);
                model.CreateTime = dt;
            }
            #endregion

            #region 抓取營業所
            string strCheckName = "<span class='bl12'><a class='text4' href='Foothold_Detail.aspx";
            int indexName = strResult.IndexOf(strCheckName);

            if (indexName != -1)
            {
                string name = strResult.Substring(indexName + strCheckName.Length + 9, 20);
                int indexSign = name.IndexOf('<');
                if (indexSign != -1)
                {
                    model.DeliveryName = name.Substring(0, indexSign);
                }
                else
                {
                    model.DeliveryName = name;
                }
            }
            #endregion

            return model;
        }

        public bool Start(string schedule_code,DateTime startTime)
        {

            DeliverStatusDao dsDao = new DeliverStatusDao(mySqlConnectionString);
           // DateTime startTime = DateTime.Now;
            string HourNum = string.Empty;
            MailModel mailModel = new MailModel();
            mailModel.MysqlConnectionString = mySqlConnectionString;

            string GroupCode = string.Empty;
            string MailTitle = string.Empty;
            string MailBody = string.Empty;
            //bool IsSeparate = false;
            //bool IsDisplyName = true;
            //獲取該排程參數
            List<ScheduleConfigQuery> store_config = new List<ScheduleConfigQuery>();
            ScheduleConfigQuery query_config = new ScheduleConfigQuery();
            query_config.schedule_code = schedule_code;
            ScheduleServiceMgr _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
            store_config = _secheduleServiceMgr.GetScheduleConfig(query_config);
            foreach (ScheduleConfigQuery item in store_config)
            {
                if (item.parameterCode.Equals("HourNum"))
                {
                    HourNum = item.value;
                }
                else if (item.parameterCode.Equals("MailFromAddress"))
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
                else if (item.parameterCode.Equals("MailFormPwd"))
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
            }
            IDeliverMasterImplMgr DeliverMaster = new DeliverMasterMgr(mySqlConnectionString);
            DataTable table = DeliverMaster.GetDeliverMaster(HourNum);
            int delNumber = table.Rows.Count;
            int updateNumber = 0;
            if (delNumber > 0)
            {
                DeliveryInfo Model;
                LogisticsTcatSodDao LTSDao=new LogisticsTcatSodDao(mySqlConnectionString);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    table.Rows[i]["gettime"] = "";
                    Model =LTSDao.GetLogisticsTcatSod(table.Rows[i]["delivery_code"].ToString());
                    if (Model.Status == "順利送達")
                    {
                        table.Rows[i]["gettime"] = Model.CreateTime;
                        updateNumber++;
                        DeliverStatus dsmodel = new DeliverStatus();

                        dsmodel.freight_type = 11;//物流配送模式
                        dsmodel.Logistics_providers = 1;//物流商
                        dsmodel.deliver_id = Int64.Parse(table.Rows[i]["deliver_id"].ToString());
                        dsmodel.state = 99;
                        dsmodel.settime = Model.CreateTime;
                        dsmodel.endtime = Model.CreateTime;
                        dsDao.InsertDeliverStatus(dsmodel);
                    }
                }
            }
            DateTime endTime = DateTime.Now;
            TimeSpan ts = endTime - startTime;
            double Second = ts.TotalSeconds;
            int num1 = 0;//期望到貨日<運達時間的物流單個數
            int num2 = 0;//期望到貨日>今天的物流單個數
            MailHelper mail = new MailHelper(mailModel);
            StringBuilder sbmailBody = new StringBuilder();
            string tablestr = GetHtmlByDataTable(GetSendTable(table), out num1, out num2);
            sbmailBody.Append("物流狀態抓取排程執行成功<br/>");
            sbmailBody.Append("更新<span style=\"font-size:large;\">" + delNumber + "</span>個出貨單<br/>");
            sbmailBody.Append("更新<span style=\"font-size:large;\">" + updateNumber + "</span>個物流單<br/>");
            sbmailBody.Append("期望到貨日小於運達時間的物流單個數:<span style=\"font-size:large;\">" + num1 + "</span><br/>");
            sbmailBody.Append("期望到貨日小於今天的物流單個數:<span style=\"font-size:large;\">" + num2 + "</span><br/>");
            sbmailBody.Append("共耗時<span style=\"font-size:large;\">" + Second + "</span>秒<br/>");
            sbmailBody.Append("更新出貨單詳情如下:<br/><br/>");
            sbmailBody.Append(tablestr);
            return mail.SendToGroup(GroupCode, MailTitle,sbmailBody.ToString(), false, true);
        }

        public static string GetHtmlByDataTable(DataTable _dtmyMonth,out int num1,out int num2) 
        {
            num1 = 0;
            num2 = 0;
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("<table  cellpadding=3 cellspacing=1  border=1 style=\"border-collapse: collapse\">");
            sbHtml.Append("<tr  style=\"text-align: center; COLOR: #0076C8; BACKGROUND-COLOR: #F4FAFF; font-weight: bold\">");
            string[] str = { "style=\"background-color:#dda29a;\"", "style=\"background-color:#d98722;\"", "style=\"background-color:#cfbd2d;\"", "style=\"background-color:#cbd12c;\"", "style=\"background-color:#91ca15;\"", "style=\"background-color:#6dc71e;\"", "style=\"background-color:#25b25c;\"", "style=\"background-color:#13a7a2;\"", "style=\"background-color:#13b7a2;\"", "style=\"background-color:#d48952;\"" };
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
                DateTime planTime = DateTime.MinValue;
                DateTime getTime = DateTime.MinValue;
                DateTime.TryParse(_dtmyMonth.Rows[i]["期望到貨日"].ToString(), out planTime);
                DateTime.TryParse(_dtmyMonth.Rows[i]["運達時間"].ToString(), out getTime);

                if (planTime.Date<getTime.Date)
                {
                    sbHtml.Append("<tr style=\"background-color:#E68688;\" >");
                    num1++;
                }
                else
                {
                    if (planTime.Date< DateTime.Now.Date)
                    {
                        sbHtml.Append("<tr style=\"background-color:#ffda44;\" >");
                        num2++;
                    }
                    else
                    {
                        sbHtml.Append("<tr>");
                    }
                }
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

        public static DataTable GetSendTable(DataTable table)
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("訂單編號", typeof(String));
            dataTable.Columns.Add("出貨單編號", typeof(String));
            dataTable.Columns.Add("物流單號", typeof(String));
            dataTable.Columns.Add("物流商", typeof(String));
            dataTable.Columns.Add("出貨單位", typeof(String));
            dataTable.Columns.Add("出貨單建立時間", typeof(String));
            dataTable.Columns.Add("出貨單壓單時間", typeof(String));
            dataTable.Columns.Add("預計到貨日", typeof(String));
            dataTable.Columns.Add("期望到貨日", typeof(String));
            dataTable.Columns.Add("運達時間", typeof(String));

            if (table.Rows.Count > 0)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DataRow row = dataTable.NewRow();
                    row["訂單編號"] = table.Rows[i]["order_id"];
                    row["出貨單編號"] = table.Rows[i]["deliver_id"];
                    row["物流單號"] = table.Rows[i]["delivery_code"];
                    row["物流商"] = table.Rows[i]["delivery_store"];
                    row["出貨單位"] = table.Rows[i]["vendor_name_full"];
                    row["出貨單建立時間"] = Convert.ToDateTime(table.Rows[i]["created"]).ToString("yyyy-MM-dd HH:mm:ss");
                    row["出貨單壓單時間"] = Convert.ToDateTime(table.Rows[i]["delivery_date"]).ToString("yyyy-MM-dd HH:mm:ss");
                    row["預計到貨日"] = Convert.ToDateTime(table.Rows[i]["deliver_org_days"]).ToString("yyyy-MM-dd");
                    row["期望到貨日"] = Convert.ToDateTime(table.Rows[i]["expect_arrive_date"]).ToString("yyyy-MM-dd");
                    try
                    {
                        row["運達時間"] = Convert.ToDateTime(table.Rows[i]["gettime"]).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    catch (Exception)
                    {

                        row["運達時間"] = "";
                    }
                    dataTable.Rows.Add(row);
                }
            }
            return dataTable;
        }
    }
    public class DeliveryInfo
    {
        public string delivery_code { get; set; }
        public string Status { get; set; }
        public DateTime CreateTime { get; set; }//資料登入時間
        public string DeliveryName { get; set; }//營業所名稱

        public DeliveryInfo()
        {
            delivery_code = string.Empty;
            Status = string.Empty;
            CreateTime = DateTime.MinValue;
            DeliveryName = string.Empty;
        }
    }

    public class DeliverStatus
    {
        public int id { get; set; }
        public Int64 deliver_id { get; set; }
        public int state { get; set; }//商品送達狀態
        public DateTime settime { get; set; }
        public DateTime endtime { get; set; }//商品最後處理時間
        public int freight_type { get; set; }//物流配送模式
        public int Logistics_providers { get; set; }//物流商

        public DeliverStatus()
        {
            id = 0;
            deliver_id = 0;
            state = 0;
            settime = DateTime.MinValue;
            endtime = DateTime.MinValue;
            freight_type = 0;
            Logistics_providers = 0;
        }
    }

    class DeliverStatusDao
    {
        static IDBAccess _accessMySql;
        public DeliverStatusDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        public int InsertDeliverStatus(DeliverStatus dsmodel)
        {
            string sqlDS = string.Format("insert into deliver_status(deliver_id,state,settime,endtime,freight_type,Logistics_providers) values({0},{1},'{2}','{3}',{4},{5})", dsmodel.deliver_id, dsmodel.state,
                               CommonFunction.DateTimeToString(dsmodel.settime), CommonFunction.DateTimeToString(dsmodel.endtime), dsmodel.freight_type, dsmodel.Logistics_providers);
            int row = _accessMySql.execCommand(sqlDS);
            return row;
        }
    }
}
