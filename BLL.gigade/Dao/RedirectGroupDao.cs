using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class RedirectGroupDao : IRedirectGroupImplDao
    {
        private IDBAccess _access;
        public RedirectGroupDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
        }
        public List<RedirectGroupQuery> QueryAll(RedirectGroup query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                StringBuilder sqlConi = new StringBuilder();
                sqlConi.Append(" SELECT group_id,group_name,group_createdate, FROM_UNIXTIME(group_createdate) createdate,group_updatedate,FROM_UNIXTIME(group_updatedate) updatedate,group_ipfrom,group_type,t.parameterName ");
                sql.Append(" FROM redirect_group left join t_parametersrc t on group_type = t.parameterCode AND t.parameterType='group_type' WHERE 1=1 ");
                if (query.group_id != 0)
                {
                    sql.AppendFormat(" AND  group_id={0}", query.group_id);
                }
                if(!string.IsNullOrEmpty(query.group_name))
                {
                    sql.AppendFormat(" AND group_name LIKE '%{0}%' ", query.group_name);
                }
                if (!string.IsNullOrEmpty(query.group_type))
                {
                    sql.AppendFormat(" AND group_type ='{0}' ", query.group_type);
                }
                sql.Append(" ORDER BY group_name ");

                totalCount = 0;

                if (query.IsPage)
                {
                    StringBuilder consql = new StringBuilder("SELECT count(group_id) ");
                    DataTable dt = _access.getDataTable(consql.ToString() + sql.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0][0].ToString());
                    }
                    sql.AppendFormat(" LIMIT {0},{1};", query.Start, query.Limit);

                }
                return _access.getDataTableForObj<RedirectGroupQuery>(sqlConi.ToString() + sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectGroupDao-->QueryAll-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string Save(RedirectGroup query)
        {
            StringBuilder sql = new StringBuilder("");
            try
            {
                query.Replace4MySQL();
                sql.Append(" INSERT INTO redirect_group(group_id,group_name,group_createdate,group_updatedate,group_ipfrom,group_type) ");
                sql.AppendFormat(" VALUES ('{0}','{1}','{2}','{3}','{4}','{5}')", query.group_id, query.group_name, query.group_createdate, query.group_updatedate, query.group_ipfrom,query.group_type);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectGroupDao-->Save-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int Update(RedirectGroup query)
        {
            StringBuilder sql = new StringBuilder("");
            try
            {
                query.Replace4MySQL();
                sql.AppendFormat("UPDATE redirect_group  set ");
                sql.AppendFormat(" group_name='{0}',group_updatedate='{1}',group_ipfrom='{2}',group_type='{3}' ", query.group_name, query.group_updatedate, query.group_ipfrom,query.group_type);
                sql.AppendFormat(" WHERE group_id='{0}' ;", query.group_id);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectGroupDao-->Update-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public List<Redirect> QueryRedirectAll(uint group_id)
        {
            StringBuilder sql = new StringBuilder("");
            try
            {
                sql.Append(" SELECT	redirect_id,redirect_name FROM redirect ");
                sql.AppendFormat(" WHERE group_id = '{0}'", group_id);
                sql.Append(" ORDER BY redirect_id ASC ;");
                return _access.getDataTableForObj<Redirect>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectGroupDao-->QueryRedirectAll-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public List<RedirectClick> QueryRedirectClictAll(RedirectClickQuery rcModel)
        {
            StringBuilder sql = new StringBuilder("");
            try
            {
                sql.Append(" SELECT	redirect_id,click_id,click_year,click_month,click_week,click_day,click_hour,click_total FROM redirect_click ");
                sql.Append("where 1=1 ");
                if (rcModel.startdate != 0)
                {
                    sql.AppendFormat(" AND  click_id >={0}", rcModel.startdate);
                }
                if (rcModel.enddate != 0)
                {
                    sql.AppendFormat(" AND click_id <{0}", rcModel.enddate);
                }
                if (!string.IsNullOrEmpty(rcModel.redirectstr))
                {
                    sql.AppendFormat(" AND  redirect_id in ({0})", rcModel.redirectstr);
                }
                sql.Append(" ORDER BY click_id ASC ;");
                return _access.getDataTableForObj<RedirectClick>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectGroupDao-->QueryRedirectClictAll-->" + ex.Message + sql.ToString(), ex);
            }
        }


        public string GetGroupName(int group_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT group_name FROM redirect_group where group_id ='{0}';", group_id);
                DataTable dt = _access.getDataTable(sql.ToString());
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["group_name"].ToString();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectGroupDao-->GetGroupName-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
