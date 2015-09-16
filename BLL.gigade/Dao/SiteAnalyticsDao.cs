using BLL.gigade.Common;
using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class SiteAnalyticsDao
    {
        private IDBAccess _accessMySql;
        private string connStr;
        public SiteAnalyticsDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }

        public List<SiteAnalytics> GetSiteAnalyticsList(SiteAnalytics query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            totalCount = 0;
            try
            {
                sql.Append(" select saly.sa_id,saly.sa_date,saly.sa_session, saly.sa_create_time,saly.sa_user,saly.sa_create_user,mu.user_username as 's_sa_create_user',mu1.user_username as sa_modify_username, ");
                sql.Append("sa_pageviews,sa_pages_session,sa_bounce_rate,sa_avg_session_duration,sa_modify_time ");
                sqlFrom.Append(" from site_analytics saly LEFT JOIN manage_user mu on mu.user_id=saly.sa_create_user   ");
                sqlFrom.Append("left join manage_user mu1 on mu1.user_id=saly.sa_modify_user");
                sqlWhere.Append(" where 1=1   ");
                if (query.search_con != 0)
                {
                    sqlWhere.AppendFormat(" and  saly.sa_date='{0}' ", query.s_sa_date);
                }
                if (query.IsPage)
                {
                    DataTable _dt = _accessMySql.getDataTable("select count(saly.sa_id) as totalCount " + sqlFrom.ToString() + sqlWhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                }
                sqlWhere.AppendFormat(" order by sa_date desc limit {0},{1}; ", query.Start, query.Limit);

                return _accessMySql.getDataTableForObj<SiteAnalytics>(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SiteAnalyticsDao-->GetSiteAnalyticsList-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public bool ImportExcelToDt(SiteAnalytics query, DataTable _dt)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder insertSql = new StringBuilder();
            StringBuilder selectSql = new StringBuilder();
            StringBuilder updateSql = new StringBuilder();
            try
            {
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    selectSql = new StringBuilder();

                    if (_dt.Rows[i][0].ToString() != "" && _dt.Rows[i][1].ToString() != "" && _dt.Rows[i][2].ToString() != "")
                    {

                        string sa_date = Convert.ToDateTime(_dt.Rows[i][0]).ToString("yyyy-MM-dd");
                        selectSql.AppendFormat(" select sa_id from site_analytics where sa_date='{0}'", sa_date);
                        DataTable _selectDt = _accessMySql.getDataTable(selectSql.ToString());
                        if (_selectDt.Rows.Count > 0)//有此條數據，更新
                        {
                            updateSql.AppendFormat(" update site_analytics set sa_date='{0}',sa_work_stage='{1}',sa_user='{2}',sa_create_time='{3}'  where sa_id='{4}';", _dt.Rows[i][0], _dt.Rows[i][1].ToString().Replace(',', ' ').Replace(" ", ""), _dt.Rows[i][2].ToString().Replace(',', ' ').Replace(" ", ""), CommonFunction.DateTimeToString(DateTime.Now), _selectDt.Rows[0][0]);
                        }
                        else//新增
                        {
                            insertSql.Append("insert into site_analytics(sa_date,sa_work_stage,sa_user,sa_create_time,sa_create_user) values( ");
                            insertSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}');", sa_date, _dt.Rows[i][1].ToString().Replace(',', ' ').Replace(" ", ""), _dt.Rows[i][2].ToString().Replace(',', ' ').Replace(" ", ""), CommonFunction.DateTimeToString(DateTime.Now), query.sa_create_user);
                        }
                    }

                }
                sql.Append(updateSql.ToString() + insertSql.ToString());
                if (sql.ToString() != "")
                {
                    if (_accessMySql.execCommand(sql.ToString()) > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("SiteAnalyticsDao-->ImportExcelToDt-->" + ex.Message + sql.ToString(), ex);
            }

        }

        public DataTable SiteAnalyticsDt(SiteAnalytics query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            try
            {
                sql.Append("  select CASE saly.sa_date WHEN '0000-00-00' THEN '0001-01-01' ELSE sa_date  END  AS sa_date,saly.sa_session,saly.sa_user,sa_avg_session_duration,sa_bounce_rate,sa_pages_session,sa_pageviews from site_analytics saly ");
                sqlWhere.Append(" where 1=1 ");
                if (query.search_con != 0)
                {
                    sqlWhere.AppendFormat(" and  saly.sa_date ='{0}' ", query.s_sa_date);
                }
                sqlWhere.Append(" order by sa_date asc ");
                return _accessMySql.getDataTable(sql.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SiteAnalyticsDao-->SiteAnalyticsDt-->" + ex.Message + sql.ToString(), ex);
            }
        }


        public int IsExist(SiteAnalytics query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                //string sa_date = Convert.ToDateTime(_dt.Rows[i][0]).ToString("yyyy-MM-dd");
                sql.AppendFormat(" select sa_id from site_analytics where sa_date='{0}'", query.s_sa_date);
                DataTable _dt = _accessMySql.getDataTable(sql.ToString());
                //return
                if (_dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(_dt.Rows[0][0]);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SiteAnalyticsDao-->IsExist-->" + ex.Message + sql.ToString(), ex);
            }

        }

        public string UpdateSNA(SiteAnalytics query)
        { 
            StringBuilder updateSql = new StringBuilder();
            try
            {
                updateSql.AppendFormat(" update site_analytics set sa_date='{0}',sa_session='{1}',sa_user='{2}',sa_modify_time='{3}',", query.s_sa_date, query.sa_session, query.sa_user, CommonFunction.DateTimeToString(query.sa_modify_time));
                updateSql.AppendFormat("sa_pageviews='{0}',sa_pages_session='{1}',sa_bounce_rate='{2}',sa_avg_session_duration='{3}',sa_modify_user='{5}' where sa_id='{4}';", query.sa_pageviews, query.sa_pages_session, query.sa_bounce_rate, query.sa_avg_session_duration,query.sa_id,query.sa_modify_user);
                return updateSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("SiteAnalyticsDao-->UpdateSNA-->" + ex.Message + updateSql.ToString(), ex);
            }
        }

        public string InsertSNA(SiteAnalytics query)
        {
            StringBuilder insertSql = new StringBuilder();
            try
            {
                insertSql.Append("insert into site_analytics(sa_date,sa_session,sa_user,sa_create_time,sa_create_user,sa_pageviews,sa_pages_session,sa_bounce_rate,sa_avg_session_duration,sa_modify_time,sa_modify_user) values( ");
                insertSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}');", query.s_sa_date, query.sa_session, query.sa_user,
                    CommonFunction.DateTimeToString(query.sa_create_time), query.sa_create_user, query.sa_pageviews, query.sa_pages_session, query.sa_bounce_rate, query.sa_avg_session_duration,
                    Common.CommonFunction.DateTimeToString(query.sa_modify_time),query.sa_modify_user );
                return insertSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("SiteAnalyticsDao-->InsertSNA-->" + ex.Message + insertSql.ToString(), ex);
            }
        }

        public bool ExecSql(ArrayList arrList)
        {
            try
            {
                MySqlDao myDao = new MySqlDao(connStr);
                return myDao.ExcuteSqls(arrList);
            }
            catch (Exception ex)
            {
                throw new Exception(" EmsDao-->ExecSql--> " + arrList + ex.Message, ex);
            }
        }

        public int InsertSiteAnalytics(SiteAnalytics query)
        {
            string sql = string.Empty;
            try
            {
                sql =InsertSNA(query);
                return _accessMySql.execCommand(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("SiteAnalyticsDao-->InsertSiteAnalytics-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public int UpdateSiteAnalytics(SiteAnalytics query)
        {
            string sql = string.Empty;
            try
            {
                sql = UpdateSNA(query);
                return _accessMySql.execCommand(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("SiteAnalyticsDao-->UpdateSiteAnalytics-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public int DeleteSiteAnalytics(SiteAnalytics query)
        {
            string sql = string.Empty;
            try
            {
                sql = string.Format("delete from site_analytics where sa_id in({0})", query.sa_ids);
                return _accessMySql.execCommand(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("SiteAnalyticsDao-->UpdateSiteAnalytics-->" + ex.Message + sql.ToString(), ex); 
            }
        }
    }
}
