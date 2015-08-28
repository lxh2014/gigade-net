using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class UserLevelLogDao
    {
        private IDBAccess _accessMySql;
        public UserLevelLogDao(string connectionStr)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        public List<UserLevelLogQuery> GetUserLevelLogList(UserLevelLogQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            try
            {
                totalCount = 0;
                sql.Append(" select  ull.rowID,ull.user_id,u.user_name,ull.create_date_time,ull.`year`,ull.`month`,ull.user_order_amount,u.user_email,ml.ml_name as ml_code_old,mll.ml_name as ml_code_new,ull.ml_code_change_type ");
                sqlFrom.Append(" from  user_level_log  ull LEFT JOIN users u on ull.user_id=u.user_id left JOIN member_level ml on ml.ml_code=ull.ml_code_old LEFT JOIN member_level mll on mll.ml_code=ull.ml_code_new ");
                sqlWhere.Append(" where 1=1 ");
                if (query.user_id != 0)
                {
                    sqlWhere.AppendFormat(" and ull.user_id ={0} ", query.user_id);
                }
                if (!string.IsNullOrEmpty(query.key))
                {
                    if (query.searchStatus == 1)
                    {
                        sqlWhere.AppendFormat(" and ull.user_id ={0}", query.key);
                    }
                    else if (query.searchStatus == 2)
                    {
                        sqlWhere.AppendFormat(" and  u.user_name like N'%{0}%' ", query.key);
                    }
                    else if (query.searchStatus == 3)
                    {
                        sqlWhere.AppendFormat("  and u.user_email like N'%{0}%' ", query.key);
                    }
                }
               
                if (!string.IsNullOrEmpty(query.leveltypeid))
                {
                    if (query.leveltype==1)
                    {
                        sqlWhere.AppendFormat("  and ull.ml_code_old ='{0}' ",query.leveltypeid);
                    }
                    else if (query.leveltype == 2)
                    {
                        sqlWhere.AppendFormat("  and ull.ml_code_new ='{0}' ", query.leveltypeid);
                    }
                }
                if (query.IsPage)
                {
                    DataTable _dt = _accessMySql.getDataTable("select count(ull.rowID) as 'totalCount' " + sqlFrom.ToString() + sqlWhere.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0][0]);
                    }
                    sqlWhere.AppendFormat(" order by  ull.create_date_time desc limit {0},{1}; ", query.Start, query.Limit);
                }

                sql.Append(sqlFrom.ToString() + sqlWhere.ToString());
                return _accessMySql.getDataTableForObj<UserLevelLogQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UserLevelLogDao-->GetUserLevelLogList-->" + sql.ToString() + ex.Message, ex);
            }
        }

    }
}
