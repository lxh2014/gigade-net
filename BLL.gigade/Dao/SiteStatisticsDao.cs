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
    public class SiteStatisticsDao
    {
        private IDBAccess _access;
        public SiteStatisticsDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        public DataTable GetSiteStatisticsList(SiteStatistics query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            totalCount = 0;
            sql.AppendFormat(@"SELECT ss_id,ss_show_num,ss_click_num,ss_click_through,ss_cost,ss_budget,");
            sql.AppendFormat(@" ss_effect_num,ss_rank,ss_date,ss_code,ss_create_time,ss_create_user,");
            sql.AppendFormat(@" ss_modify_time,ss_modify_user ");
            sqlwhere.AppendFormat(@" FROM site_statistics");
            sqlwhere.AppendFormat(@" WHERE 1=1 ");
            if (query.ss_id != 0)
            {
                sqlwhere.AppendFormat(@" AND ss_id='{0}' ", query.ss_id);
            }
            if (!string.IsNullOrEmpty(query.ss_code))
            {
                sqlwhere.AppendFormat(@" AND ss_code='{0}' ", query.ss_code);
            }
            if (query.ss_date != DateTime.MinValue)
            {
                sqlwhere.AppendFormat(@" AND ss_date ='{0}' ", query.ss_date);
            }
            if (query.sss_date != DateTime.MinValue && query.ess_date != DateTime.MinValue)
            {
                sqlwhere.AppendFormat(@" AND ss_date>='{0}' AND ss_date<='{1}' ", query.sss_date, query.ess_date);
            }
            sql.Append(sqlwhere.ToString());
            sql.AppendFormat(@" ORDER BY ss_id DESC ");
            try
            {
                if (query.IsPage)
                {
                    sql.AppendFormat(@" LIMIT {0},{1};", query.Start, query.Limit);
                    DataTable dt = _access.getDataTable("SELECT ss_id " + sqlwhere.ToString());
                    totalCount = dt.Rows.Count;
                }
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SiteStatisticsDao-->GetSiteStatisticsList" + ex.Message + sql.ToString(), ex);
            }
        }
        public int Insert(SiteStatistics ss)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendLine(@"INSERT INTO site_statistics (ss_show_num,ss_click_num,ss_click_through,ss_cost,");
                sql.AppendLine(@"ss_budget,ss_effect_num,ss_rank,ss_date,");
                sql.AppendLine(@"ss_code,ss_create_time,ss_create_user,");
                sql.AppendLine(@" ss_modify_time,ss_modify_user ) VALUES(");
                sql.AppendFormat(@" '{0}','{1}','{2}','{3}',", ss.ss_show_num, ss.ss_click_num, ss.ss_click_through, ss.ss_cost);
                sql.AppendFormat(@" '{0}','{1}','{2}','{3}',", ss.ss_budget, ss.ss_effect_num, ss.ss_rank, ss.ss_date.ToString("yyyy-MM-dd"));
                sql.AppendFormat(@" '{0}','{1}','{2}',", ss.ss_code, ss.ss_create_time.ToString("yyyy-MM-dd HH:mm:ss"), ss.ss_create_user);
                sql.AppendFormat(@" '{0}','{1}');", ss.ss_modify_time.ToString("yyyy-MM-dd HH:mm:ss"), ss.ss_modify_user);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SiteStatisticsDao-->Insert" + ex.Message + sql.ToString(), ex);
            }
        }
        public int Update(SiteStatistics ss)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"UPDATE site_statistics SET ss_show_num='{0}',ss_click_num='{1}',",ss.ss_show_num,ss.ss_click_num);
                sql.AppendFormat(@"ss_click_through='{0}',ss_cost='{1}', ",ss.ss_click_through,ss.ss_cost);
                sql.AppendFormat(@"ss_budget='{0}',ss_effect_num='{1}',ss_rank='{2}',ss_date='{3}',", ss.ss_budget, ss.ss_effect_num, ss.ss_rank, ss.ss_date.ToString("yyyy-MM-dd"));
                sql.AppendFormat(@"ss_code='{0}',",ss.ss_code);
                sql.AppendFormat(@" ss_modify_time='{0}',ss_modify_user='{1}' ", ss.ss_modify_time.ToString("yyyy-MM-dd HH:mm:ss"), ss.ss_modify_user);
                sql.AppendFormat(@" WHERE 1=1 ");
                if(ss.ss_id!=0)
                {
                    sql.AppendFormat(@" AND ss_id='{0}';", ss.ss_id);
                }
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SiteStatisticsDao-->Insert" + ex.Message + sql.ToString(), ex);
            }
        }
        public int Delete(SiteStatistics ss)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"DELETE FROM site_statistics WHERE ss_id='{0}';",ss.ss_id);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SiteStatisticsDao-->Delete" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
