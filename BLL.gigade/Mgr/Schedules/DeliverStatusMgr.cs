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
            int updateNumber = 0;
            if (table.Rows.Count > 0)
            {
                DeliveryInfo Model;
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Model = GetWebContent(table.Rows[i]["delivery_code"].ToString());
                    if (Model.Status == "順利送達")
                    {
                        updateNumber++;
                        DeliverStatus dsmodel = new DeliverStatus();

                        dsmodel.freight_type = 11;//物流配送模式
                        dsmodel.Logistics_providers = 1;//物流商
                        dsmodel.deliver_id = int.Parse(table.Rows[i]["deliver_id"].ToString());
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
            MailHelper mail = new MailHelper(mailModel);
            return mail.SendToGroup(GroupCode, MailTitle, "物流狀態抓取排程執行成功,<br/>更新" + updateNumber + "個物流單<br/>共耗時" + Second + "秒", true, true);
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
        public int SelectDeliverStatus(long deliver_id)
        {
            string sql = string.Format("select count(deliver_id) from deliver_status where deliver_id='{0}' and state='99';", deliver_id);
            int count = int.Parse(_accessMySql.getDataTable(sql).Rows[0][0].ToString());
            return count;
        }
    }
}
