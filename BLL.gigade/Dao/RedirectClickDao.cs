using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using System.Data;

namespace BLL.gigade.Dao
{
    public class RedirectClickDao : IRedirectClickImplDao
    {
          private IDBAccess _access;
          public RedirectClickDao(string connectionstring)
          {
                _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
          }
          public List<RedirectClickQuery> QueryAllById(RedirectClickQuery query)
          {
              StringBuilder sqlclomn = new StringBuilder();
              StringBuilder sqltable = new StringBuilder();
              StringBuilder sqlcondition = new StringBuilder();
              StringBuilder sqlgroup = new StringBuilder();
              try
              {

                  sqlclomn.Append(@" select count(click_total)sum_click,SUBSTRING(click_id,1,8) as click_id,click_year,click_month,click_day,redirect_id ");//,count(click_total)click_total,
                  sqltable.Append(" from redirect_click ");
                  sqlcondition.Append(" where 1=1 ");
                  if (query.redirect_id_groupID!=0) 
                  {
                      sqlcondition.AppendFormat(" and redirect_id in(SELECT redirect_id from redirect where group_id='{0}') ", query.redirect_id_groupID);
                  }
                  sqlgroup.Append(" GROUP BY redirect_id,click_year,click_month,click_day  ORDER BY click_id ASC ");
             
                  string sql = sqlclomn.ToString() + sqltable.ToString() + sqlcondition.ToString() + sqlgroup.ToString();
                  return _access.getDataTableForObj<RedirectClickQuery>(sql);
              }
              catch (Exception ex)
              {
                  throw new Exception("RedirectClickDao.QueryAllById-->" + ex.Message + sqlclomn.ToString() + sqltable.ToString() + sqlcondition.ToString() + sqlgroup.ToString(), ex);
              }
          }
        /// <summary>
        /// 報表統計
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
          public List<RedirectClickQuery> GetRedirectClick(RedirectClickQuery query)
          {
              string month = query.click_month < 10 ? "0" + query.click_month.ToString() : query.click_month.ToString();
              string start = query.click_year.ToString() + month + "01" + "00";
              int days = DateTime.DaysInMonth(int.Parse(query.click_year.ToString()), int.Parse(query.click_month.ToString()));
              string end = query.click_year.ToString() + month + days + "23";
              string redirect_id = string.Empty;
              StringBuilder sql = new StringBuilder();
              try
              {
                  if (query.redirect_id_groupID != 0)
                  {
                      //根據group_id查出所有的redirect_id
                      sql.AppendFormat(@"SELECT GROUP_CONCAT(redirect_id) FROM redirect WHERE group_id='{0}';", query.redirect_id_groupID);
                      DataTable dt = _access.getDataTable(sql.ToString());
                      if(dt.Rows.Count>0)
                      {
                          //判斷此群組下是否有連結
                          if (!string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
                          {
                              redirect_id = dt.Rows[0][0].ToString();
                          }
                          else
                          {
                              redirect_id = "0";
                          }
                      }
                  }
                  else
                  {
                      redirect_id = query.redirect_id.ToString();
                  }
                  sql.Clear();
                  sql.AppendFormat(@"SELECT click_id,click_day,click_hour,click_week,click_total ");
                  sql.AppendFormat(@" FROM redirect_click");
                  sql.AppendFormat(@" WHERE 1=1 AND redirect_id in ({0}) ", redirect_id);
                  sql.AppendFormat(@" AND click_id>={0} AND click_id<={1} ", start, end);
                  sql.AppendFormat(@" ORDER BY click_id ASC;");
                  return _access.getDataTableForObj<RedirectClickQuery>(sql.ToString());
              }
              catch (Exception ex)
              {
                  throw new Exception("RedirectClickDao.GetRedirectClick-->" + ex.Message + sql.ToString(), ex);
              }
           }
        /// <summary>
        ///連接管理的匯出功能
        /// 查詢一個最小的時間值，為了匯出時從最小時間開始這個方法不適用，但是為了提高程序效率，這個方法也是必須要的。
        /// </summary>
        /// <returns></returns>
          public RedirectClickQuery ReturnMinClick()
          {
              string sql = " select min(click_id) as click_id, click_year,	click_month,click_day from redirect_click ";
              try
              {            
                  return _access.getSinggleObj<RedirectClickQuery>(sql);
              }
              catch (Exception ex )
              {
             throw new Exception("RedirectClickDao.QueryAllById-->" + ex.Message +sql, ex);
              }
          }

    }
}
