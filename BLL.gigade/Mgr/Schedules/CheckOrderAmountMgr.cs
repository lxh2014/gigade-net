using BLL.gigade.Common;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Common;

namespace BLL.gigade.Mgr.Schedules
{
    public class CheckOrderAmountMgr
    {
         private ScheduleServiceMgr _secheduleServiceMgr;
        static IDBAccess _accessMySql;
        static string mySqlConnectionString = string.Empty;
        IOrderMasterImplMgr _orderMaster;
        public CheckOrderAmountMgr(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            mySqlConnectionString = connectionString;
        }
        public bool Start(string schedule_code)
        {
            bool result = false;
            try
            {
                if (string.IsNullOrEmpty(schedule_code))
                {
                    return result;
                }
                string GroupCode = string.Empty;
                string MailTitle = string.Empty;

                StringBuilder strbody = new StringBuilder();
                MailModel mailModel = new MailModel();
                mailModel.MysqlConnectionString = mySqlConnectionString;
                //獲取該排程參數
                List<ScheduleConfigQuery> store_config = new List<ScheduleConfigQuery>();
                ScheduleConfigQuery query_config = new ScheduleConfigQuery();
                query_config.schedule_code = schedule_code;
                _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
                store_config = _secheduleServiceMgr.GetScheduleConfig(query_config);
                #region FTP參數賦值
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
                
                }
                #endregion

                MailHelper mail = new MailHelper(mailModel);

                OrderMasterQuery query = new OrderMasterQuery();
                query.order_date_pay_endTime = DateTime.Now;
                query.order_date_pay_startTime = query.order_date_pay_endTime.AddHours(-1);
                StringBuilder sbMailBody = new StringBuilder();
                try
                {
                      bool isSHow = false;
                      string html =GetBigAmountMailBody(query,out isSHow);
                      if (isSHow)
                      {
                          sbMailBody.AppendLine(html);
                      }
                }
                catch (Exception ex)
                {
                    throw new Exception("CheckOrderAmountMgr-->GetBigAmountMailBody-->" + "大金額訂單排程:" + ex.Message);
                }
                try
                {
                     bool isSHow = false;
                     string html = GetBigOrderNumbersMailBody(query, out isSHow);
                     if (isSHow)
                     {
                         sbMailBody.AppendLine(html);
                     }
                }
                catch (Exception ex)
                {
                    throw new Exception("CheckOrderAmountMgr-->GetBigOrderNumbersMailBody-->"+"一小時多次付款客戶異常訂單付款通知:"+ ex.Message);
                }
                try
                {
                    bool isSHow = false;
                    string html = FirstBuyEmail(query, out isSHow);
                    if (isSHow)
                    {
                        sbMailBody.AppendLine(html);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("CheckOrderAmountMgr-->OtherTWPay-->" + "首購超過5000訂單付款檢查:" + ex.Message);
                }
                try
                {
                    bool isSHow = false;
                    string html = OtherTWPay(query, out isSHow);
                    if (isSHow)
                    {
                        sbMailBody.AppendLine(html);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("CheckOrderAmountMgr-->OtherTWPay-->" + "異地信用卡付款付款訂單通知:" + ex.Message);
                }

                if (!String.IsNullOrEmpty(sbMailBody.ToString()))
                {
                    mail.SendToGroup(GroupCode, MailTitle, sbMailBody.ToString(), false, true);//發送郵件給群組
                }
                result= true;
            }
            catch (Exception ex)
            {
                throw new Exception("CheckOrderAmount-->Start-->" + ex.Message);
            }
            return result;
        }


        #region 一小時之內數量過多訂單檢查
        public string GetBigOrderNumbersMailBody(OrderMasterQuery query,out bool isShow)
        {
            isShow = false;
            StringBuilder strbody = new StringBuilder();
            strbody.AppendLine("一小時之內數量過多訂單檢查:");
            
            DataTable _newErrorDt = new DataTable();
            _newErrorDt.Columns.Add("訂購人", typeof(String));
            _newErrorDt.Columns.Add("郵箱", typeof(String));
            _newErrorDt.Columns.Add("IP地址", typeof(String));
            _newErrorDt.Columns.Add("訂單編號", typeof(String));
            _newErrorDt.Columns.Add("訂購時間", typeof(String));
            _orderMaster = new OrderMasterMgr(mySqlConnectionString);

            DataTable _dtResult = _orderMaster.GetBigOrderNumbers(query);
            if (_dtResult.Rows.Count > 0)
            {
                for (int j = 0; j < _dtResult.Rows.Count; j++)
                {
                    DataRow drtwo = _newErrorDt.NewRow();
                    drtwo[0] = _dtResult.Rows[j][0];
                    drtwo[1] = _dtResult.Rows[j][1];
                    drtwo[2] = _dtResult.Rows[j][2];
                    drtwo[3] = _dtResult.Rows[j][3];
                    drtwo[4] = _dtResult.Rows[j][4];
                    _newErrorDt.Rows.Add(drtwo);
                }

                isShow = true;
            }
            strbody.AppendFormat(GetHtmlByDataTable(_newErrorDt));
            return strbody.ToString();
        }
        #endregion

        #region 大金額訂單檢查
        public string GetBigAmountMailBody(OrderMasterQuery query,out bool isShow)
        {
            isShow = false;

            StringBuilder strbody = new StringBuilder();
            strbody.AppendLine("大金額訂單檢查");
            _orderMaster = new OrderMasterMgr(mySqlConnectionString);

            DataTable _dt = _orderMaster.GetBigAmount(query);

            if (_dt.Rows.Count > 0)
            {
                DataTable _newDt = new DataTable();
                _newDt.Columns.Add("訂單編號", typeof(String));
                _newDt.Columns.Add("付款時間", typeof(String));
                _newDt.Columns.Add("付款總金額", typeof(String));
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    DataRow dr = _newDt.NewRow();
                    dr[0] = _dt.Rows[i][0];
                    dr[1] = CommonFunction.GetNetTime(Convert.ToInt64(_dt.Rows[i][1])).ToString("yyyy/MM/dd HH:mm:ss");
                    dr[2] = GetString(_dt.Rows[i][2].ToString());
                    _newDt.Rows.Add(dr);
                }
                strbody.AppendFormat(GetHtmlByDataTable(_newDt));
                isShow = true;
            }

            return strbody.ToString();
        }
        #endregion

        #region 首購超過5000檢查
        public string FirstBuyEmail(OrderMasterQuery query,out bool isShow)
        {
            isShow = false;
            StringBuilder strbody = new StringBuilder();
            strbody.AppendLine("首購超過5000訂單檢查");
            _orderMaster = new OrderMasterMgr(mySqlConnectionString);

            DataTable _dt = _orderMaster.GetUsersOrderAmount(query);
            if (_dt.Rows.Count > 0)
            {
                DataTable _newdt = new DataTable();
                _newdt.Columns.Add("訂單編號", typeof(String));
                _newdt.Columns.Add("付款時間", typeof(String));
                _newdt.Columns.Add("付款總金額", typeof(String));
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    DataRow dr = _newdt.NewRow();
                    dr[0] = _dt.Rows[i][0];
                    dr[1] = CommonFunction.GetNetTime(Convert.ToInt64(_dt.Rows[i][1])).ToString("yyyy/MM/dd HH:mm:ss");
                    dr[2] = GetString(_dt.Rows[i][2].ToString());
                    _newdt.Rows.Add(dr);
                }
                strbody.AppendFormat(GetHtmlByDataTable(_newdt));
                isShow = true;
            }
            return strbody.ToString();


        }
        #endregion

        #region 異地付款訂單檢查
        public string OtherTWPay(OrderMasterQuery query,out bool isShow)
        {
            isShow = false;
            StringBuilder strbody = new StringBuilder();
            strbody.AppendLine("異地付款訂單檢查");

            _orderMaster = new OrderMasterMgr(mySqlConnectionString);

            DataTable _dt = _orderMaster.GetOtherTWPay(query);
            if (_dt.Rows.Count > 0)
            {
                DataTable _newdt = new DataTable();
                _newdt.Columns.Add("訂單編號", typeof(String));
                _newdt.Columns.Add("付款總金額", typeof(String));
                _newdt.Columns.Add("付款時間", typeof(String));
                _newdt.Columns.Add("IP", typeof(String));
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    DataRow dr = _newdt.NewRow();
                    dr[0] = _dt.Rows[i][0];
                    dr[1] = _dt.Rows[i][1].ToString();
                    dr[2] = CommonFunction.GetNetTime(Convert.ToInt64(_dt.Rows[i][2])).ToString();
                    dr[3] = _dt.Rows[i][3].ToString();
                    _newdt.Rows.Add(dr);
                }
                strbody.AppendFormat(GetHtmlByDataTable(_newdt));
                isShow = true;
            }
            return strbody.ToString();
        }
        #endregion




        #region DataTable轉Html +string GetHtmlByDataTable(DataTable _dtmyMonth)
        /// <summary>
        /// DataTable轉Html
        /// </summary>
        /// <param name="_dtmyMonth"></param>
        /// <returns></returns>
        public  string GetHtmlByDataTable(DataTable _dtmyMonth)
        {
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("<table  cellpadding=3 cellspacing=1  border=1 style=\"border-collapse: collapse\">");
            sbHtml.Append("<tr  style=\"text-align: center; COLOR: #0076C8; BACKGROUND-COLOR: #F4FAFF; font-weight: bold\">");
            string[] str = { "style=\"background-color:#dda29a;\"", "style=\"background-color:#d98722;\"", "style=\"background-color:#cfbd2d;\"", "style=\"background-color:#cbd12c;\"", "style=\"background-color:#91ca15;\"", "style=\"background-color:#6dc71e;\"", "style=\"background-color:#25b25c;\"", "style=\"background-color:#13a7a2;\"" };
            string aligns = "align=\"right\"";
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
                    sbHtml.Append(aligns);
                    sbHtml.Append(" >");
                    sbHtml.Append(_dtmyMonth.Rows[i][j]);
                    sbHtml.Append("</td>");
                }
                sbHtml.Append("</tr>");
            }
            sbHtml.Append("</table>");
            return sbHtml.ToString();

        }
        #endregion

        #region +string GetString(string name)
        public string GetString(string name)
        {
            string results = Convert.ToDouble(name).ToString("N");
            if (results.IndexOf('.') > 0)
            {
                return results.Substring(0, results.LastIndexOf('.'));
            }
            else
            {
                return results;
            }
        }
        #endregion

    }
}
