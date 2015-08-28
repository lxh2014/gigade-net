using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class RedirectDao : IRedirectImplDao
    {
        private IDBAccess _access;
        public RedirectDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
        }
        #region MyRegion
        /// <summary>
        /// 連接管理=>群組列表：連接點管理匯出
        /// </summary>
        /// <param name="query">綁定擴展實體進行查詢</param>
        /// <param name="totalcount">可以進行分頁</param>
        /// <param name="str">擴展的sql語句</param>
        /// <returns>List<RedirectQuery>集合</returns>
        public DataTable GetRedirectList(RedirectQuery query, out int totalcount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlfield = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            StringBuilder sqlgroup = new StringBuilder();
            try
            {
                sqlfield.AppendFormat(@" SELECT	(select group_name from redirect_group where group_id = '{0}') as group_name, ", query.group_id);
                sqlfield.AppendFormat(@" redirect_id,group_id,redirect_name,redirect_url,redirect_status as 'status', redirect_total,");
                sqlfield.AppendFormat(@" FROM_UNIXTIME(redirect_createdate) AS redirect_createdate,FROM_UNIXTIME(redirect_updatedate) AS redirect_updatedate,redirect_ipfrom");
                sqlfrom.AppendFormat(@" from redirect ");
                sqlwhere.AppendFormat(" where 1=1 and group_id ='{0}' ", query.group_id);
                sql.Append(sqlfield.ToString() + sqlfrom.ToString() + sqlwhere.ToString());
                sqlgroup.AppendFormat(@" ORDER BY redirect_id DESC ,redirect_status ASC,redirect_name ASC ");
                totalcount = 0;
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable("select redirect_id " + sqlfrom.ToString() + sqlwhere.ToString());
                    totalcount = _dt.Rows.Count;
                    sqlgroup.AppendFormat(@" limit {0},{1}", query.Start, query.Limit);
                }
                sql.Append(sqlgroup.ToString());
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectDao.GetRedirectList-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public List<RedirectQuery> GetRedirect(RedirectQuery query, out int totalcount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlfield = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            StringBuilder sqlgroup = new StringBuilder();
            try
            {
                sqlfield.AppendFormat(@" SELECT	(select group_name from redirect_group where group_id = '{0}') as group_name, ", query.group_id);
                sqlfield.AppendLine(@"redirect_id,group_id,user_group_id,redirect_name,redirect_url,redirect_status,redirect_total,");
                sqlfield.AppendLine(@"redirect_note,redirect_createdate,redirect_updatedate,redirect_ipfrom");
                sqlfrom.AppendLine(@" FROM redirect where 1=1");
                if (query.group_id > 0)
                {
                    sqlwhere.AppendFormat("  and group_id ='{0}' ", query.group_id);
                }
                sql.Append(sqlfield.ToString() + sqlfrom.ToString() + sqlwhere.ToString());
                sqlgroup.AppendFormat(@" ORDER BY redirect_id DESC ,redirect_status ASC,redirect_name ASC ");
                totalcount = 0;
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable("select Count(redirect_id) as 'sum' " + sqlfrom.ToString() + sqlwhere.ToString());
                    totalcount = int.Parse(_dt.Rows[0]["sum"].ToString());
                    sqlgroup.AppendFormat(@" limit {0},{1}", query.Start, query.Limit);
                }
                sql.Append(sqlgroup.ToString());
                return _access.getDataTableForObj<RedirectQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectDao.GetRedirectList-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        public string Save(Redirect query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.Append(" INSERT INTO redirect(redirect_id,group_id,user_group_id,redirect_name,redirect_url,");
                sql.Append("  redirect_status,redirect_total, redirect_note,redirect_createdate,redirect_updatedate,redirect_ipfrom ) ");
                sql.AppendFormat(" VALUES ('{0}','{1}','{2}','{3}','{4}',", query.redirect_id, query.group_id, query.user_group_id, query.redirect_name, query.redirect_url);
                sql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}','{5}')", query.redirect_status, query.redirect_total, query.redirect_note, query.redirect_createdate, query.redirect_updatedate, query.redirect_ipfrom);

                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectDao-->Save-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int Update(Redirect query)
        {
            StringBuilder sql = new StringBuilder("");
            try
            {
                query.Replace4MySQL();
                sql.AppendFormat("UPDATE redirect  set ");
                sql.AppendFormat(" group_id='{0}',user_group_id='{1}',redirect_name='{2}',", query.group_id, query.user_group_id, query.redirect_name);
                sql.AppendFormat(" redirect_url='{0}',redirect_status='{1}',", query.redirect_url, query.redirect_status);
                sql.AppendFormat(" redirect_note='{0}',redirect_updatedate='{1}',redirect_ipfrom='{2}'", query.redirect_note, query.redirect_updatedate, query.redirect_ipfrom);
                sql.AppendFormat(" WHERE redirect_id='{0}' ;", query.redirect_id);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectDao-->Update-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int EnterInotRedirect(RedirectQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.Append(" INSERT INTO redirect(redirect_id,group_id,user_group_id,redirect_name,redirect_url,");
                sql.Append("  redirect_status,redirect_total, redirect_note,redirect_createdate,redirect_updatedate,redirect_ipfrom ) ");
                sql.AppendFormat(" VALUES ('{0}','{1}','{2}','{3}','{4}',", query.redirect_id, query.group_id, query.user_group_id, query.redirect_name, query.redirect_url);
                sql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}','{5}')", query.redirect_status, query.redirect_total, query.redirect_note, query.redirect_createdate, query.redirect_updatedate, query.redirect_ipfrom);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectDao-->Save-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string GetSum(RedirectQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select count(r.redirect_id) as Sum from redirect r ");
                if (query.selsum == 0)
                {
                    sql.Append("LEFT JOIN users u ON r.redirect_id = u.source_trace ");
                }
                else if (query.selsum == 1)
                {
                    sql.Append("LEFT JOIN order_master u ON r.redirect_id = u.source_trace ");
                }
                sql.AppendFormat(" where r.redirect_id='{0}'; ", query.redirect_id);

                return _access.getDataTable(sql.ToString()).Rows[0]["Sum"].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectDao.GetUserSum-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public DataTable GetRedirectListCSV(RedirectQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlfield = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            StringBuilder sqlgroup = new StringBuilder();
            try
            {
                sqlfield.AppendFormat(@" SELECT	(select group_name from redirect_group where group_id = '{0}') as group_name, ", query.group_id);
                sqlfield.Append(@" redirect_id,redirect_name,redirect_url,redirect_total,redirect_status as status,'' as status_name,'' as redirect ");
                sqlfrom.Append(@" from redirect ");
                sqlwhere.AppendFormat(" where 1=1 and group_id ='{0}' ", query.group_id);
                sql.Append(sqlfield.ToString() + sqlfrom.ToString() + sqlwhere.ToString());
                sqlgroup.AppendFormat(@" ORDER BY redirect_id DESC ,redirect_status ASC,redirect_name ASC ");
                sql.Append(sqlgroup.ToString());
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectDao.GetRedirectListCSV-->" + ex.Message + sql.ToString(), ex);
            }
        }

    }
}
