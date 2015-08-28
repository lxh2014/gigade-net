using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class ProductClickDao
    {
        private IDBAccess _access;
        public ProductClickDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        public DataTable GetProductClickList(ProductClickQuery query, out int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcondi = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            totalCount = 0;
            string total = string.Empty;
            if (query.click_year != 1 && query.click_month != 1 && query.click_day != 1)
            {
                total = ",click_total ";
            }
            else
            {
                total = ",SUM(pc.click_total) AS click_total ";
            }
            sql.AppendFormat(@"SELECT pc.product_id,p.product_name,vb.brand_name,p.prod_classify,");
            //sql.AppendFormat(@"pc.click_hour,pc.click_week,");
            sql.AppendFormat(@"pc.click_year,pc.click_month,pc.click_day ");
            sql.Append(total);
            sqlwhere.AppendFormat(@" FROM product_click pc ");
            sqlwhere.AppendFormat(@" LEFT JOIN product p  ON pc.product_id=p.product_id");
            sqlwhere.AppendFormat(@" LEFT JOIN vendor_brand vb ON p.brand_id=vb.brand_id");
            if (!string.IsNullOrEmpty(query.pids))
            {
                sqlcondi.AppendFormat(@" AND pc.product_id IN({0})", query.pids);
            }
            if (query.product_status != 999)
            {
                sqlcondi.AppendFormat(@" AND p.product_status='{0}'", query.product_status);
            }
            if (query.prod_classify != 0)
            {
                sqlcondi.AppendFormat(@" AND p.prod_classify='{0}'", query.prod_classify);
            }
            if (query.brand_id != 0)
            {
                sqlcondi.AppendFormat(@" AND p.brand_id='{0}'", query.brand_id);
            }
            if (query.sclick_id != 0)
            {
                sqlcondi.AppendFormat(@" AND  pc.click_id>='{0}' ", query.sclick_id);
            }
            if (query.eclick_id != 0)
            {
                sqlcondi.AppendFormat(@" AND  pc.click_id<='{0}' ", query.eclick_id);
            }
            if (sqlcondi.Length != 0)
            {
                sqlwhere.Append(" WHERE ");
                sqlwhere.Append(sqlcondi.ToString().TrimStart().Remove(0, 3));
            }
            //選擇的是按年統計
            if (query.click_year == 1)
            {
                sqlwhere.AppendFormat(@"GROUP BY pc.product_id");
                sqlwhere.AppendFormat(@", pc.click_year ");
            }
            else if (query.click_month == 1) //選擇的是按月統計
            {
                sqlwhere.AppendFormat(@"GROUP BY pc.product_id");
                sqlwhere.AppendFormat(@", pc.click_month ");
            }
            else if (query.click_day == 1)//選擇的是按天統計
            {
                sqlwhere.AppendFormat(@"GROUP BY pc.product_id");
                sqlwhere.AppendFormat(@", pc.click_day ");
            }

            sql.Append(sqlwhere.ToString());
            try
            {
                if (query.IsPage)
                {
                    sql.AppendFormat(@" LIMIT {0},{1};", query.Start, query.Limit);
                    DataTable dt = _access.getDataTable("SELECT count( pc.product_id) totalCount " + sqlwhere.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0]["totalCount"]);
                    }
                }
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductClickDao-->GetProductClickList" + ex.Message + sql.ToString(), ex);
            }

        }
    }
}
