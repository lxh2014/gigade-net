/*
* 文件名稱 :ConsumeStatisticDao.cs
* 文件功能描述 :訪問數據庫,獲得會員消費金額統計信息
* 版權宣告 :鄭州分公司
* 開發人員 : 文博
* 版本資訊 : 1.0
* 日期 : 2015-7-28
* 修改人員 :無
* 版本資訊 : 1.0
* 日期 : 2015-7-28
* 修改備註 :無
 */
using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class ConsumeStatisticDao
    {
        IDBAccess _accessMySql;
        public ConsumeStatisticDao(string connectionstring)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionstring);
        }
        #region 獲取會員消費統計列表數據 + GetUserOrdersSubtotal()
        public List<UserOrdersSubtotalQuery> GetUserOrdersSubtotal(UserOrdersSubtotalQuery query, out int totalCount)
        {
            totalCount = 0;
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            sql.Append("SELECT uos.row_id, u.user_id,u.user_name,uos.year,uos.month,uos.order_product_subtotal,uos.normal_product_subtotal,uos.low_product_subtotal,uos.buy_count,uos.last_buy_time,uos.buy_avg,uos.note,uos.create_datetime ");
            sqlCount.Append("select count(u.user_id) as totalCount ");
            sqlCondi.Append(" from users as u  LEFT JOIN  user_orders_subtotal as uos   on u.user_id=uos.user_id  WHERE 1=1 ");
            try
            {
                if (query.startTime!=DateTime.MinValue)
                {
                    sqlCondi.AppendFormat(" and uos.create_datetime BETWEEN '{0}' and '{1}'", CommonFunction.DateTimeToString(query.startTime), CommonFunction.DateTimeToString(query.endTime));
                }
                if (query.startMoney != 0)
                {
                    sqlCondi.AppendFormat(" and uos.order_product_subtotal >= {0}",query.startMoney);//BETWEEN {0} and {1}", query.startMoney, query.endMoney);
                }
                if (query.endMoney != 0)
                {
                    sqlCondi.AppendFormat(" and uos.order_product_subtotal <= {0}",query.endMoney);//BETWEEN {0} and {1}", query.startMoney, query.endMoney);
                }
                if (!string.IsNullOrEmpty(query.searchKey))
                {
                   
                    if (query.searchType == 1)
                    {
                        sqlCondi.AppendFormat(" and u.user_id = {0} ", query.searchKey);
                    }
                    else if (query.searchType == 2)
                    {
                        sqlCondi.AppendFormat(" and u.user_name like N'%{0}%' ", query.searchKey);
                    }
                }

                if (query.IsPage)
                {
                    try
                    {
                        sqlCount.Append(sqlCondi.ToString());
                        DataTable dt = _accessMySql.getDataTable(sqlCount.ToString());
                        if (dt.Rows.Count != 0)
                        {
                            totalCount = Convert.ToInt32(dt.Rows[0]["totalCount"].ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("ConsumeStatisticDao-->GetUserOrdersSubtotal: " + ex.Message + "sqlCount:" + sqlCount.ToString());
                    }
                    sqlCondi.AppendFormat(" limit {0},{1};", query.Start, query.Limit);
                }
                sql.Append(sqlCondi.ToString());
                return _accessMySql.getDataTableForObj<UserOrdersSubtotalQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ConsumeStatisticDao-->GetUserOrdersSubtotal: " + ex.Message + "sql:" + sql.ToString());
            }
        }
        #endregion
    }
}
