using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using MySql.Data.MySqlClient;

namespace BLL.gigade.Dao
{
    public class NewsContentDao : INewsContentImplDao
    {
        private IDBAccess _accessMySql;
        private string connStr;
        public NewsContentDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }

        public List<NewsContentQuery> GetNewsList(NewsContentQuery store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            StringBuilder sqlLimit = new StringBuilder();
            totalCount = 0;
            try
            {
                sqlCount.Append("select count(*) as totalCount ");
                sql.Append("select nc.news_id ,nc.user_id,nc.news_title,nc.news_sort,nc.news_status,nc.news_show_start,nc.news_show_end,mu.user_username, nc.news_content,nc.news_ipfrom,nc.news_createdate,nc.news_updatedate ");
                sqlFrom.Append("from news_content nc LEFT JOIN manage_user mu on nc.user_id=mu.user_id  ");
                sqlWhere.Append("  where 1=1  ");
                #region 狀態
                if (!string.IsNullOrEmpty(store.searchCon))
                {
                    if (store.searchCon == "0")
                    {
                        sqlWhere.AppendFormat(" and nc.news_status=0  ");
                    }
                    else if (store.searchCon == "1")
                    {
                        sqlWhere.AppendFormat(" and nc.news_status=1 ");
                    }
                    else if (store.searchCon == "2")
                    {
                        sqlWhere.AppendFormat(" and  nc.news_status=2  ");
                    }
                    else if (store.searchCon == "3")
                    {
                        sqlWhere.AppendFormat(" and  nc.news_status=3 ");
                    }

                }
                #endregion
                #region 標題
                if (!string.IsNullOrEmpty(store.search_con))
                {
                    sqlWhere.AppendFormat(" and nc.news_title like '%{0}%'  ", store.search_con);
                }
                #endregion
                #region 日期條件
                if (!string.IsNullOrEmpty(store.date))
                {
                    if (store.start_time != DateTime.MinValue && store.end_time != DateTime.MinValue)
                    {
                        if (store.date == "0")
                        {
                            sqlWhere.AppendFormat(" and  nc.news_show_start >={0} and nc.news_show_end<={1} ", CommonFunction.GetPHPTime(store.start_time.ToString()), CommonFunction.GetPHPTime(store.end_time.ToString()));
                        }
                        else if (store.date == "1")
                        {
                            sqlWhere.AppendFormat(" and  nc.news_show_start >={0} and nc.news_show_start<={1} ", CommonFunction.GetPHPTime(store.start_time.ToString()), CommonFunction.GetPHPTime(store.end_time.ToString()));
                        }
                        else if (store.date == "2")
                        {
                            sqlWhere.AppendFormat(" and  nc.news_show_end>={0} and nc.news_show_end<={1} ", CommonFunction.GetPHPTime(store.start_time.ToString()), CommonFunction.GetPHPTime(store.end_time.ToString()));
                        }
                    }
                }
                #endregion
                if (store.IsPage)
                {
                    DataTable _dt = _accessMySql.getDataTable(sqlCount.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                }
                sqlLimit.AppendFormat(" ORDER BY nc.news_show_start DESC,nc.news_sort DESC,nc.news_id desc  limit {0},{1};", store.Start, store.Limit);
                sql.AppendFormat(sqlFrom.ToString() + sqlWhere.ToString() + sqlLimit.ToString());
                return _accessMySql.getDataTableForObj<NewsContentQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("NewsContentDao-->GetNewsList-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public NewsContentQuery OldQuery(uint newsId)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select nc.news_id ,nc.user_id,nc.news_title,nc.news_sort,nc.news_status,nc.news_show_start,nc.news_show_end,mu.user_username, nc.news_content,nc.news_ipfrom,nc.news_createdate,nc.news_updatedate ");
                sql.Append("from news_content nc LEFT JOIN manage_user mu on nc.user_id=mu.user_id  ");
                sql.AppendFormat("  where nc.news_id={0};", newsId);
                return _accessMySql.getSinggleObj<NewsContentQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("NewsContentDao-->OldQuery-->" + ex.Message + sql.ToString(), ex);
            }
        }

        #region 得到加1后的serial_value
        /// <summary>
        /// 得到加1后的serial_value
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public int GetSerialValue(int serialId)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select serial_value from serial where serial_id={0};", serialId);
                DataTable _dt = _accessMySql.getDataTable(sql.ToString());
                return Convert.ToInt32(_dt.Rows[0]["serial_value"]) + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentDao-->GetSerialValue-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion
        #region 更新serial_value
        public string UpdateSerialVal(int serialValue, int serialId)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("update serial set serial_value={0}  where serial_id={1};", serialValue, serialId);
            return sql.ToString();
        }
        #endregion

        public int NewsContentSave(NewsContentQuery store)
        {
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            StringBuilder sql = new StringBuilder();
            int re = 0;
            try
            {
                #region 新增
                if (store.news_id == 0)//新增
                {
                    if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                    {
                        store.serialvalue = GetSerialValue(20);//news_content
                        store.log_value = GetSerialValue(21);//news_log
                        mySqlConn.Open();
                        mySqlCmd.Connection = mySqlConn;
                        mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                        mySqlCmd.CommandType = System.Data.CommandType.Text;
                        mySqlCmd.CommandText = UpdateSerialVal(store.serialvalue, 20);
                        mySqlCmd.CommandText += InsertNewsContent(store);
                        mySqlCmd.CommandText += UpdateSerialVal(store.log_value, 21);
                        mySqlCmd.CommandText += AddNewsLog(store);
                        re = mySqlCmd.ExecuteNonQuery();
                        mySqlCmd.Transaction.Commit();
                    }
                }
                #endregion
                #region 編輯
                else//編輯
                {
                    store.Replace4MySQL();
                    if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                    {
                        store.serialvalue = GetSerialValue(20);//news_content
                        store.log_value = GetSerialValue(21);//news_log
                        mySqlConn.Open();
                        mySqlCmd.Connection = mySqlConn;
                        mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                        mySqlCmd.CommandType = System.Data.CommandType.Text;
                        mySqlCmd.CommandText = UpdateNewsContent(store);
                        mySqlCmd.CommandText += UpdateSerialVal(store.log_value, 21);
                        mySqlCmd.CommandText += AddNewsLog(store);
                        re = mySqlCmd.ExecuteNonQuery();
                        mySqlCmd.Transaction.Commit();
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("NewsContentDao-->NewsContentSave-->" + mySqlCmd.ToString() + ex.Message, ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }

            return re;
        }
        public string InsertNewsContent(NewsContentQuery store)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into news_content (news_id,user_id,news_title,news_content,news_sort,");
            sql.Append("news_status,news_show_start,news_show_end,");
            sql.Append("news_createdate,news_updatedate,");
            sql.Append("news_ipfrom)");
            sql.AppendFormat("values({0},{1},'{2}','{3}',{4},", store.serialvalue, store.user_id, store.news_title, store.news_content, store.news_sort);
            sql.AppendFormat("{0},{1},{2}, ", store.news_status, CommonFunction.GetPHPTime(store.s_news_show_start.ToString()), CommonFunction.GetPHPTime(store.s_news_show_end.ToString()));
            sql.AppendFormat("{0},{1},", CommonFunction.GetPHPTime(store.s_news_createdate.ToString()), CommonFunction.GetPHPTime(store.s_news_updatedate.ToString()));
            sql.AppendFormat("'{0}');", store.news_ipfrom);
            return sql.ToString();
        }

        public string UpdateNewsContent(NewsContentQuery store)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("set sql_safe_updates=0;update news_content set news_title='{0}',news_content='{1}',news_sort={2},", store.news_title, store.news_content, store.news_sort);
            sql.AppendFormat("news_status={0},news_show_start={1},news_show_end={2},", store.news_status, CommonFunction.GetPHPTime(store.s_news_show_start.ToString()), CommonFunction.GetPHPTime(store.s_news_show_end.ToString()));
            sql.AppendFormat("news_updatedate={0},news_ipfrom='{1}'", CommonFunction.GetPHPTime(store.s_news_updatedate.ToString()), store.news_ipfrom);
            sql.AppendFormat(" where news_id={0};set sql_safe_updates=1;", store.news_id);
            return sql.ToString();
        }

        public string AddNewsLog(NewsContentQuery store)
        {
            StringBuilder sql = new StringBuilder();

            if (store.news_id == 0)
            {
                sql.AppendFormat("insert into news_log (log_id,news_id,user_id,log_description,log_ipfrom,log_createdate)");
                sql.AppendFormat("values({0},{1},{2},'{3}','{4}',{5});", store.log_value, store.serialvalue, store.user_id, store.log_description, store.news_ipfrom, CommonFunction.GetPHPTime(DateTime.Now.ToString()));
            }
            else
            {
                sql.AppendFormat("insert into news_log (log_id,news_id,user_id,log_description,log_ipfrom,log_createdate)");
                sql.AppendFormat("values({0},{1},{2},'{3}','{4}',{5});", store.log_value, store.news_id, store.user_id, store.log_description, store.news_ipfrom, CommonFunction.GetPHPTime(DateTime.Now.ToString()));
            }
            return sql.ToString();
        }

        public List<NewsContentQuery> GetNewContent()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select * from news_content ");
                return _accessMySql.getDataTableForObj<NewsContentQuery>(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("NewsContentDao-->GetNewContent-->" + sql.ToString() + ex.Message, ex);
            }
        }


        public List<NewsLogQuery> GetNewsLogList(NewsLogQuery store, out int totalCount)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder strWhere = new StringBuilder();
            try
            {
                strSql.AppendFormat("select el.*,mu.user_username as user_name from news_log el left join manage_user mu on el.user_id=mu.user_id  ");
                sqlCount.AppendFormat("select count( *) as  totalCount from news_log el left join manage_user mu on el.user_id=mu.user_id ");
                strWhere.AppendFormat(" where 1=1 and  el.news_id={0}", store.news_id);
                totalCount = 0;
                if (store.IsPage)
                {
                    DataTable _dt = _accessMySql.getDataTable(sqlCount.ToString() + strWhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                }
                strSql.AppendFormat(strWhere.ToString());
                strSql.AppendFormat(" limit {0},{1};", store.Start, store.Limit);
                return _accessMySql.getDataTableForObj<NewsLogQuery>(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("NewsContentDao-->GetNewsLogList-->"+strSql.ToString() + ex.Message, ex);
            }
        }
    }
}
