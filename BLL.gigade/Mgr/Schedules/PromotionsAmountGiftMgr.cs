using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Mgr.Schedules
{
    public class PromotionsAmountGiftMgr
    {
        private ScheduleServiceMgr _secheduleServiceMgr;
        static string mySqlConnectionString;//獲取鏈接字符串
        static IDBAccess _accessMySql;
        public PromotionsAmountGiftMgr(string connectionString)
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
            PromotionsAmountGiftDao giftDao = new PromotionsAmountGiftDao(mySqlConnectionString);
            try
            {
                DataTable dtProd = giftDao.GetProdStock();
                StringBuilder str = new StringBuilder();
                if (dtProd.Rows.Count > 0)
                {
                    str.Append("<style>.wid100{width:100px;text-align: right;} td{padding:2px;}</style>");
                    str.AppendFormat("當前贈品庫存狀態");
                    str.Append("<table  cellspacing='1' cellpadding='3' border='1' style='border-collapse: collapse'>");
                    str.AppendFormat("<tr style='text-align: center; background-color: #f4faff; color: #0076c8; font-weight: bold;'><th>活動編號</th><th  style='background-color: #db7093;'>活動名稱</th><th  style='background-color: #EEEE00;' >商品編號</th><th style='background-color: #b4eeb4;' >商品細項編號</th><th style='background-color:#AEEEEE'>庫存數量</th></tr>");


                    foreach (DataRow row in dtProd.Rows)
                    {
                        str.AppendFormat("<tr><td style='width:100px;text-align: center;'>{0}</td><td>{1}</td><td class='wid100'>{2}</td><td class='wid100'>{3}</td><td class='wid100'>{4}</td></tr>", row["id"], row["event_name"], row["product_id"], row["item_id"], GetString(row["kucun"].ToString()));
                    }

                    str.AppendFormat("</table>");
                }
                else
                {
                    str.Append("<span style='color:red;'><b>沒有適合條件的數據<b></span>");
                }

                List<ScheduleConfigQuery> store_config = new List<ScheduleConfigQuery>();
                ScheduleConfigQuery query_config = new ScheduleConfigQuery();
                query_config.schedule_code = schedule_code;
                _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
                store_config = _secheduleServiceMgr.GetScheduleConfig(query_config);
                string GroupCode = string.Empty;
                string EmailTile = string.Empty;
                MailModel mailModel = new MailModel();
                mailModel.MysqlConnectionString = mySqlConnectionString;
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
                        EmailTile = item.value;
                    }
                }
                MailHelper mHelper = new MailHelper(mailModel);
                if (mHelper.SendToGroup(GroupCode, EmailTile, str.ToString(), false, true))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountGiftMgr-->Start-->" + ex.Message);
            }
        }

        public static string GetString(string name)
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


    public class PromotionsAmountGiftDao
    {
        private IDBAccess _access;
        private string connStr;
        public PromotionsAmountGiftDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }

        public DataTable GetProdStock()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select * from ( ");
                sql.Append("(SELECT pag.id, pag.`name` 'event_name', p.product_id,pi.item_id,pi.item_stock as 'kucun' FROM product p ");
                sql.Append("LEFT JOIN product_item pi USING(product_id) ");
                sql.Append("LEFT JOIN promotions_amount_gift pag on pag.gift_id=p.product_id ");
                sql.Append("where gift_id>10000  and  pag.bonus_type=0 and pag.active=1 and pag.`start`<=NOW() and pag.`end`>=NOW() ) UNION ");
                sql.Append("(SELECT pag.id, pag.`name` 'event_name',p.product_id,pi.item_id, pi.item_stock as 'kucun' FROM product p ");
                sql.Append("LEFT JOIN product_item pi USING(product_id) ");
                sql.Append("LEFT JOIN promotions_amount_gift pag on pag.gift_id=pi.item_id ");
                sql.Append("where gift_id>100000 and  pag.bonus_type=0  and pag.active=1 and pag.`start`<=NOW() and pag.`end`>=NOW()) ");
                sql.Append(" )result GROUP BY item_id;");
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("贈品庫存設定:PromotionsAmountGiftDao/GetProdStock" + ex.ToString());
            }
        }

    }
}
