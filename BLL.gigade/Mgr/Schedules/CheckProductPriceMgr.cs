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
    public class CheckProductPriceMgr
    {
        private ScheduleServiceMgr _secheduleServiceMgr;

        static IDBAccess _access;
        static string mySqlConnectionString = string.Empty;
        public CheckProductPriceMgr(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
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
                int HourNum = 0;

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
                    else if (item.parameterCode.Equals("HourNum"))
                    {
                        HourNum = int.Parse(item.value);
                    }
                    
                }
                string MailBody = string.Empty;
                //獲取history修改中的itemid
                string items=string.Empty;
                if (HourNum > 0)
                {
                    items = GetHistoryItemid(HourNum);
                }
                else
                {
                    items = "ALL";
                }
                DataTable _dt = GetDataTable(items);
                DataTable _newdt = new DataTable();
                _newdt.Columns.Add("商品名稱", typeof(String));
                _newdt.Columns.Add("商品編號", typeof(String));
                _newdt.Columns.Add("商品細項編號", typeof(String));
                _newdt.Columns.Add("商品規格", typeof(String));
                _newdt.Columns.Add("異常原因", typeof(String));
                _newdt.Columns.Add("item_money", typeof(String));
                _newdt.Columns.Add("item_cost", typeof(String));
                _newdt.Columns.Add("event_money", typeof(String));
                _newdt.Columns.Add("event_cost", typeof(String));
                _newdt.Columns.Add("event_start", typeof(String));
                _newdt.Columns.Add("event_end", typeof(String));
                //
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    DataRow newRow = _newdt.NewRow();
                    newRow[0] = _dt.Rows[i]["product_name"];
                    newRow[1] = _dt.Rows[i]["product_id"];
                    newRow[2] = _dt.Rows[i]["item_id"];
                    newRow[3] = _dt.Rows[i]["prod_sz"];

                    int item_money = _dt.Rows[i]["item_money"] == DBNull.Value ? 0 : Convert.ToInt32(_dt.Rows[i]["item_money"]);
                    int item_cost = _dt.Rows[i]["item_cost"] == DBNull.Value ? 0 : Convert.ToInt32(_dt.Rows[i]["item_cost"]);
                    int event_money = _dt.Rows[i]["event_money"] == DBNull.Value ? 0 : Convert.ToInt32(_dt.Rows[i]["event_money"]);
                    int event_cost = _dt.Rows[i]["event_cost"] == DBNull.Value ? 0 : Convert.ToInt32(_dt.Rows[i]["event_cost"]);
                    int event_start = _dt.Rows[i]["event_start"] == DBNull.Value ? 0 : Convert.ToInt32(_dt.Rows[i]["event_start"]);
                    int event_end = _dt.Rows[i]["event_end"] == DBNull.Value ? 0 : Convert.ToInt32(_dt.Rows[i]["event_end"]);
                    newRow[5] = item_money;
                    newRow[6] = item_cost;
                    newRow[7] = event_money;
                    newRow[8] = event_cost;
                    newRow[9] = event_start;
                    newRow[10] = event_end;
                    if(item_money==0)
                    {
                        newRow[4] = "商品價格為零";
                    }
                    else if(item_money>item_cost)
                    {
                        newRow[4] = "商品價格大於成本價";
                    }
                    else if(event_money==0 && event_end > CommonFunction.GetPHPTime())
                    {
                        newRow[4] = "商品活動價格為零";
                    }
                    else if(event_money>event_cost && event_end > CommonFunction.GetPHPTime())
                    {
                        newRow[4] = "商品活動價格大於商品活動成本價格";
                    }
                    _newdt.Rows.Add(newRow);

                }
                if (_newdt.Rows.Count > 0)
                {
                    MailBody = GetHtmlByDataTable(_newdt);
                    /////////////////
                    _secheduleServiceMgr.SendMail(mailModel, GroupCode, MailTitle, MailBody, false, true);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("CheckProductPriceMgr-->Start-->" + ex.Message);
            }
            return true;
        }
        private string GetHistoryItemid(int HourNum)
        {

            DateTime kdate = DateTime.Now.AddHours( - HourNum);
            StringBuilder sql = new StringBuilder();
            string items = string.Empty;
            try
            {
                sql.AppendFormat(@"SELECT ip.item_id FROM t_history_batch thb
LEFT JOIN t_table_history tth ON tth.batchno = thb.batchno
LEFT JOIN item_price ip ON tth.PK_value = ip.item_price_id
WHERE 
table_name ='{0}'", "item_price");

                if (HourNum > 0)
                {
                    sql.AppendFormat(@" and thb.kdate >= '{0}'", CommonFunction.DateTimeToString(kdate));
                }
                DataTable _dt = _access.getDataTable(sql.ToString());

                for (int i = 0; i < _dt.Rows.Count;i++ )
                {
                    if (!string.IsNullOrEmpty(_dt.Rows[i]["item_id"].ToString().Trim()))
                    {
                        items = items + _dt.Rows[i]["item_id"] + ",";
                    }
                }

                sql.Clear();
                sql.AppendFormat(@"SELECT pi.item_id FROM t_history_batch thb
LEFT JOIN t_table_history tth ON tth.batchno = thb.batchno
LEFT JOIN price_master pm ON tth.PK_value = pm.price_master_id
LEFT JOIN product_item pi ON pi.product_id = pm.product_id
WHERE 
table_name ='{0}'", "price_master");

                if (HourNum > 0)
                {
                    sql.AppendFormat(@" and thb.kdate >= '{0}'", CommonFunction.DateTimeToString(kdate));
                }
                _dt = _access.getDataTable(sql.ToString());
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(_dt.Rows[i]["item_id"].ToString().Trim()))
                    {
                        items = items + _dt.Rows[i]["item_id"] + ",";
                    }
                }

                if (!string.IsNullOrEmpty(items))
                {
                    items = items.Substring(0, items.Length - 1);
                }
                else
                {
                    items="''";
                }
                return items;
            }
            catch (Exception ex)
            {

                throw new Exception("CheckProductPriceMgr-->GetHistoryItemid-->" + ex.Message, ex);
            }
        }
        private DataTable GetDataTable(string items)
        {
           
            StringBuilder sql_danyi = new StringBuilder();
            StringBuilder sql_guding = new StringBuilder();
            try
            {
                sql_danyi.AppendFormat(@"
SELECT 
vpo.product_name,vpo.product_id,vpo.item_id,concat(IFNULL(ps1.spec_name,''),IFNULL(ps2.spec_name,'')) as prod_sz ,ip.item_money,ip.item_cost,ip.event_money,ip.event_cost, pm.event_start,pm.event_end

FROM v_product_onsale vpo

LEFT JOIN product ON vpo.product_id = product.product_id

LEFT JOIN product_item pi ON vpo.item_id = pi.item_id
LEFT JOIN product_spec ps1 ON pi.spec_id_1 = ps1.spec_id
LEFT JOIN product_spec ps2 ON pi.spec_id_2 = ps2.spec_id

LEFT JOIN item_price ip ON pi.item_id = ip.item_id

LEFT JOIN price_master pm ON pm.price_master_id = ip.price_master_id

WHERE 1=1 

and product.combination=1

AND ( ip.item_money=0 

or ip.item_money>ip.item_cost 

or ( pm.event_end>'{0}' and ip.event_money = 0 )

or ( pm.event_end>'{0}' and ip.event_money > ip.event_cost )

)", CommonFunction.GetPHPTime());

                if (items != "ALL")
                {
                    sql_danyi.AppendFormat(@" and vpo.item_id in ({0}) ", items);
                }
                DataTable dt_danyi = _access.getDataTable(sql_danyi.ToString());
                
                //固定組合商品
                sql_guding.AppendFormat(@"
SELECT 
vpo.product_name,vpo.product_id,vpo.item_id,concat(IFNULL(ps1.spec_name,''),IFNULL(ps2.spec_name,'')) as prod_sz ,
pm.price as item_money,pm.cost as item_cost,pm.event_price as event_money,pm.event_cost as event_cost,pm.event_start,pm.event_end

FROM v_product_onsale vpo

LEFT JOIN product ON vpo.product_id = product.product_id

LEFT JOIN product_item pi ON vpo.item_id = pi.item_id
LEFT JOIN product_spec ps1 ON pi.spec_id_1 = ps1.spec_id
LEFT JOIN product_spec ps2 ON pi.spec_id_2 = ps2.spec_id

LEFT JOIN price_master pm ON pm.price_master_id = vpo.product_id

WHERE 1=1 

and product.combination=2

AND ( pm.price = 0 

or pm.price>pm.cost 

or ( pm.event_end>'{0}' and pm.event_price = 0 )

or ( pm.event_end>'{0}' and pm.event_price > pm.event_cost )

)", CommonFunction.GetPHPTime());

                if (items != "ALL")
                {
                    sql_guding.AppendFormat(@" and vpo.item_id in ({0}) ", items);
                }
                DataTable dt_guding = _access.getDataTable(sql_guding.ToString());
                for (int i = 0; i < dt_guding.Rows.Count; i++)
                {
                    dt_danyi.Rows.Add(dt_guding.Rows[i]);
                }
                return dt_danyi;

            }
            catch (Exception ex)
            {
                throw new Exception("CheckProductPriceMgr-->GetDataTable-->" + ex.Message);
            }
        }

        static string GetHtmlByDataTable(DataTable _dtmyMonth)
        {
            try
            {
                System.Text.StringBuilder sbHtml = new System.Text.StringBuilder();
                sbHtml.Append("<table  cellpadding=3 cellspacing=1  border=1 style=\"border-collapse: collapse\">");
                sbHtml.Append("<tr  style=\"text-align: center; COLOR: #0076C8; BACKGROUND-COLOR: #F4FAFF; font-weight: bold\">");
                string[] str = { "style=\"background-color:#dda29a;\"", "style=\"background-color:#d98722;\"", "style=\"background-color:#cfbd2d;\"", "style=\"background-color:#cbd12c;\"", "style=\"background-color:#91ca15;\"", "style=\"background-color:#6dc71e;\"", "style=\"background-color:#25b25c;\"", "style=\"background-color:#13a7a2;\"" };
                string aligns = "align=\"right\"";
                for (int i = 0; i < _dtmyMonth.Columns.Count; i++)
                {
                    sbHtml.Append("<th ");
                    sbHtml.Append(str[i%str.Length]);
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
            catch (Exception ex)
            {
                throw new Exception("CheckProductPriceMgr-->GetHtmlByDataTable-->" + ex.Message);
            }

        }

    }
}
