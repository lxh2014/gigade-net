using BLL.gigade.Common;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

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
                    //else if (item.parameterCode.Equals("MailBody"))
                    //{
                    //    MailBody = item.value;
                    //}
                    //else if (item.parameterCode.Equals("sumDays"))
                    //{
                    //    sumDays = item.value;
                    //}
                    //else if (item.parameterCode.Equals("periodDays"))
                    //{
                    //    periodDays = item.value;
                    //}
                    //else if (item.parameterCode.Equals("NOSuggestCountMsg"))
                    //{
                    //    NOSuggestCountMsg = item.value;
                    //}
                }
                #endregion

                MailHelper mail = new MailHelper(mailModel);

                OrderMasterQuery query = new OrderMasterQuery();
                query.Money_Collect_Date = int.Parse(CommonFunction.GetPHPTime(DateTime.Now.AddHours(-1).ToString("yyyy/MM/dd HH:mm:ss")).ToString());

//                long time = CommonFunction.GetPHPTime(DateTime.Now.AddHours(-1).ToString("yyyy/MM/dd HH:mm:ss"));
//                string sql = string.Format(@"select order_id ,money_collect_date, order_amount from order_master where order_amount>(select parameterName 
//                from t_parametersrc where parameterType='auto_paramer' and parameterCode='order_amount_limit' )  and money_collect_date>'{0}' order by money_collect_date desc;", time);
                _orderMaster = new OrderMasterMgr(mySqlConnectionString);

                DataTable _dtCheck = new DataTable();
                _dtCheck = _orderMaster.GetCheckOrderAmount(query);
                if (_dtCheck.Rows.Count > 0)
                {
                    DataTable _newDt = new DataTable();
                    _newDt.Columns.Add("訂單編號", typeof(String));
                    _newDt.Columns.Add("付款時間", typeof(String));
                    _newDt.Columns.Add("付款總金額", typeof(String));
                    for (int i = 0; i < _dtCheck.Rows.Count; i++)
                    {
                        DataRow dr = _newDt.NewRow();
                        dr[0] = _dtCheck.Rows[i]["order_id"];
                        dr[1] = CommonFunction.GetNetTime(Convert.ToInt32(_dtCheck.Rows[i]["money_collect_date"])).ToString("yyyy/MM/dd HH:mm:ss");
                        dr[2] = GetString(_dtCheck.Rows[i]["order_amount"].ToString());
                        _newDt.Rows.Add(dr);
                    }
                    strbody.AppendFormat(GetHtmlByDataTable(_newDt));
                }
                else
                {
                    strbody.AppendFormat("付款金額訂單不存在");
                }
                strbody.AppendFormat("<br/>");
               // int totalcount = Convert.ToInt32(_accessMySql.getDataTable("select parameterName from t_parametersrc where parameterType='auto_paramer' and parameterCode='order_count_limit';").Rows[0][0]);
                DataTable _newErrorDt = new DataTable();
                _newErrorDt.Columns.Add("訂購人", typeof(String));
                _newErrorDt.Columns.Add("郵箱", typeof(String));
                _newErrorDt.Columns.Add("IP地址", typeof(String));
                _newErrorDt.Columns.Add("訂單編號", typeof(String));
                _newErrorDt.Columns.Add("訂購時間", typeof(String));
//                string strsql = string.Format(@"SELECT user_id,odcount 
//                from (
//                SELECT count(1) as odcount,user_id 
//                FROM 
//                (select order_amount,user_id
//                from 
//                order_master where order_amount>
//                (select parameterName from t_parametersrc where parameterType='auto_paramer' and parameterCode='order_amount_limit' ) and money_collect_date>'{0}' ) 
//                as new_table 
//                GROUP BY user_id) as new_tabletwo 
//                WHERE odcount>'{1}';", time, totalcount);
                DataTable _dtUsers = _orderMaster.GetUsersOrderAmount(query);
                if (_dtUsers.Rows.Count > 0)
                {
                    for (int i = 0; i < _dtUsers.Rows.Count; i++)
                    {
                        query.user_id = Convert.ToUInt32(_dtUsers.Rows[i]["user_id"]);
//                        string newsqlstr = string.Format(@"SELECT us.user_name,us.user_email,om.order_ipfrom,om.order_id,FROM_UNIXTIME(om.money_collect_date)as new_time 
//FROM order_master om LEFT JOIN users us on us.user_id=om.user_id  
//WHERE om.money_collect_date>'{0}' and om.user_id='{1}' and om.order_amount>(select parameterName from t_parametersrc where parameterType='auto_paramer' and parameterCode='order_amount_limit' );", time, _dtUsers.Rows[i]["user_id"]);
                        DataTable _dtResult = _orderMaster.GetCheckOrderAmount(query);
                        for (int j = 0; j < _dtResult.Rows.Count; j++)
                        {
                            DataRow drtwo = _newErrorDt.NewRow();
                            drtwo[0] = _dtResult.Rows[j]["user_name"];
                            drtwo[1] = _dtResult.Rows[j]["user_email"];
                            drtwo[2] = _dtResult.Rows[j]["order_ipfrom"];
                            drtwo[3] = _dtResult.Rows[j]["order_id"];
                            drtwo[4] = _dtResult.Rows[j]["new_time"];
                            _newErrorDt.Rows.Add(drtwo);
                        }
                    }
                    strbody.AppendFormat(GetHtmlByDataTable(_newErrorDt));
                }
                else
                {
                    strbody.AppendFormat("異常訂單不存在");
                }
                mail.SendToGroup(GroupCode, MailTitle, strbody.ToString(), true, true);//發送郵件給群組
                result= true;
            }
            catch (Exception ex)
            {
                throw new Exception("CheckOrderAmount-->Start-->" + ex.Message);
            }
            return result;
        }




        static string GetHtmlByDataTable(DataTable _dtmyMonth)
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
        static string GetString(string name)
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

    }
}
