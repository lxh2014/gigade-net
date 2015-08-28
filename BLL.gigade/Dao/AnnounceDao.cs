using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
using DBAccess;
using MySql.Data.MySqlClient;

namespace BLL.gigade.Dao
{
    public class AnnounceDao : IAnnounceImplDao
    {
        private IDBAccess _accessMySql;
        private string connStr;
        public AnnounceDao(string connectionstring)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }

        public List<AnnounceQuery> GetAnnounce()
        {
            StringBuilder sql = new StringBuilder();

            try
            {
                sql.Append("SELECT *  FROM announce ORDER BY announce_id DESC ");
                return _accessMySql.getDataTableForObj<AnnounceQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AnnounceDao-->GetAnnounce-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public AnnounceQuery GetAnnounce(AnnounceQuery query)
        {
            StringBuilder sql = new StringBuilder();

            try
            {
                sql.Append("SELECT *  FROM announce where 1=1");
                if (query.announce_id != 0)
                {
                    sql.AppendFormat(" and announce_id='{0}' ", query.announce_id);
                }
                sql.Append(" ORDER BY announce_id DESC");

                return _accessMySql.getSinggleObj<AnnounceQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AnnounceDao-->GetAnnounce-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public List<AnnounceQuery> GetAnnounceList(AnnounceQuery store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            totalCount = 0;
            try
            {
                sql.Append("select a.announce_id ,a.title,a.content,a.sort,a.status,a.type,t_type.parameterName as type_name,");
                sql.Append(" a.creator,u1.user_username as c_name,FROM_UNIXTIME(a.create_time) as create_date,u2.user_username as u_name, ");
                sql.Append("  a.modifier,FROM_UNIXTIME(a.modify_time) as modify_date ");
                sqlCondi.Append(" from announce a LEFT JOIN manage_user u1 on a.creator=u1.user_id  ");
                sqlCondi.Append("left join manage_user u2 on a.modifier=u2.user_id  ");
                sqlCondi.Append("left join t_parametersrc t_type on t_type.parameterType='announce_type' and t_type.parameterCode=a.type");
                sqlCondi.Append("  where 1=1 ");
                sqlCount.Append(" select count(a.announce_id) as totalCount ");
                if (store.type != 0)
                {
                    sqlCondi.AppendFormat(" and a.type='{0}'", store.type);
                }
                if (store.con_status != -1)
                {
                    sqlCondi.AppendFormat(" and a.status='{0}'", store.con_status);
                }
                if (!string.IsNullOrEmpty(store.key))
                {
                    sqlCondi.AppendFormat(" and ( a.title like N'%{0}%' or a.content like N'%{0}%' )", store.key);
                }
                if (store.IsPage)
                {
                    sqlCount.Append(sqlCondi.ToString());
                    DataTable _dt = _accessMySql.getDataTable(sqlCount.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                }
                sqlCondi.AppendFormat("ORDER BY a.announce_id  DESC limit {0},{1};", store.Start, store.Limit);

                return _accessMySql.getDataTableForObj<AnnounceQuery>(sql.Append(sqlCondi).ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AnnounceDao-->GetAnnounceList-->" + ex.Message + sql.ToString() + sqlCount.ToString(), ex);
            }
        }

        public int AnnounceSave(AnnounceQuery store)
        {
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            StringBuilder sql = new StringBuilder();
            int re = 0;
            store.Replace4MySQL();
            try
            {

                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                SerialDao _serialDao = new SerialDao(connStr);
                #region 新增
                if (store.announce_id == 0)//新增
                {
                    Serial sQuery = new Serial();
                    store.announce_id = Convert.ToUInt32(_serialDao.GetSerialById(65).Serial_Value + 1); // GetSerialValue(20);
                    mySqlCmd.CommandText = _serialDao.UpdateAutoIncreament(new Serial { Serial_id = 65, Serial_Value = store.announce_id });
                    mySqlCmd.CommandText += InsertAnnounce(store);
                    re = mySqlCmd.ExecuteNonQuery();
                }
                else//編輯
                {
                    mySqlCmd.CommandText = UpdateAnnounce(store);
                    re = mySqlCmd.ExecuteNonQuery();
                }
                #endregion
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("AnnounceDao-->AnnounceSave-->" + mySqlCmd.ToString() + ex.Message, ex);
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

        public string InsertAnnounce(AnnounceQuery store)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into announce (announce_id,title,content,sort,status,");
            sql.Append("type,creator,create_time,modifier,modify_time)");
            sql.AppendFormat("values('{0}','{1}','{2}','{3}',{4},", store.announce_id, store.title, store.content, store.sort, store.status);
            sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}'); ", store.type, store.creator, CommonFunction.GetPHPTime(store.create_date.ToString()), store.modifier, CommonFunction.GetPHPTime(store.modify_date.ToString()));

            return sql.ToString();
        }

        public string UpdateAnnounce(AnnounceQuery store)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("set sql_safe_updates=0;update announce set title='{0}',content='{1}',sort='{2}',", store.title, store.content, store.sort);
            sql.AppendFormat("status={0},type='{1}',", store.status, store.type);
            sql.AppendFormat("modifier='{0}',modify_time='{1}'", store.modifier, CommonFunction.GetPHPTime(store.modify_date.ToString()));
            sql.AppendFormat(" where announce_id={0};set sql_safe_updates=1;", store.announce_id);
            return sql.ToString();
        }


    }
}
