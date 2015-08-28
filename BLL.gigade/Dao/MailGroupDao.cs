using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using System.Data;
using BLL.gigade.Common;
using MySql.Data.MySqlClient;

namespace BLL.gigade.Dao
{
    class MailGroupDao : IMailGroupImplDao
    {
        private IDBAccess _access;
        private string connStr;
        public MailGroupDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }
        public List<MailGroupQuery> MailGroupList(MailGroupQuery query,out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlLimit = new StringBuilder();
            totalCount = 0;
            try
            {
                sqlCount.Append("select count(mg.row_id) as totalCount from mail_group mg;");
                sql.AppendFormat("select mg.row_id,mg.group_name,mg.remark,mg.`status`,mg.group_code,count(mgm.row_id) as callid ");
                sqlFrom.AppendFormat(" from mail_group mg LEFT  JOIN mail_group_map mgm on mg.row_id=mgm.group_id and mgm.`status`={0} GROUP BY mg.row_id ", query.status);
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable(sqlCount.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"].ToString());
                    }
                    sqlLimit.AppendFormat("order by mg.row_id desc limit {0},{1}; ", query.Start, query.Limit);
                }
                return _access.getDataTableForObj<MailGroupQuery>(sql.ToString() + sqlFrom.ToString() + sqlLimit.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("MapGroupDao-->MailGroupList-->" + sql.ToString()+sqlFrom.ToString() + ex.Message, ex);
            }
        }
        /// <summary>
        /// 有關mail的查詢（此方法可擴充，擴充后請詳細注釋）
        /// 內容：根據群組編號或代碼查詢用戶email地址 add by shuangshuang0420j 2015.02.04
        /// 修改：
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<MailGroupQuery> MailGroupQuery(MailGroupQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(@" select mu.user_mail,mu.user_name");
                sql.Append(@" from mail_group mg");
                sql.Append(@" left join mail_group_map mgm on mgm.group_id=mg.row_id ");
                sql.Append(@" left join mail_user mu on mu.row_id=mgm.user_id ");
                sql.Append(@" where 1=1");
                if (query.row_id != 0)
                {
                    sql.AppendFormat(@" and mg.row_id='{0}'", query.row_id);
                }
                if (!string.IsNullOrEmpty(query.group_code))
                {
                    sql.AppendFormat(@" and mg.group_code='{0}'", query.group_code);
                }
                if (query.status == 1)//查詢啟用狀態的合法群組用戶時必須所有的狀態都為啟用
                {
                    sql.Append(" and mg.`status`=1 and mgm.`status`=1  and mu.`status`=1");
                }
                else if (query.status == 0)//查詢停用的群組時只需群組的狀態為停用即可
                {
                    sql.Append(" and mg.`status`=0");

                }
                return _access.getDataTableForObj<MailGroupQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("MapGroupDao-->MailGroupQuery-->" + sql.ToString() + ex.Message, ex);
            }
        }

        /// <summary>
        /// 新增編輯羣組
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int SaveMailGroup(MailGroupQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {

                if (query.row_id == 0)
                {
                    if (VerifyGroup(query))
                    {
                        sql.Append("insert into mail_group(`group_name`,`remark`,`status`,");
                        sql.Append("`create_time`,`create_user`,`update_time`,");
                        sql.Append("`update_user`,`group_code`) ");
                        sql.AppendFormat("values('{0}','{1}',{2}, ", query.group_name, query.remark, query.status);
                        sql.AppendFormat("'{0}',{1},'{2}',", CommonFunction.DateTimeToString(query.create_time), query.create_user, CommonFunction.DateTimeToString(query.update_time));
                        sql.AppendFormat("{0},'{1}');", query.update_user, query.group_code);
                        return _access.execCommand(sql.ToString());
                    }
                    else
                    {
                        return -1;//羣組名稱或編碼重複
                    }
                }
                else
                {
                    if (VerifyGroup(query))
                    {
                        sql.AppendFormat("update mail_group set group_name='{0}',group_code='{1}',remark='{2}',", query.group_name, query.group_code, query.remark);
                        sql.AppendFormat("update_time='{0}',update_user={1} where row_id={2};", CommonFunction.DateTimeToString(query.update_time), query.update_user, query.row_id);
                        return _access.execCommand(sql.ToString());
                    }
                    else
                    {
                        return -1;//羣組名稱或編碼重複
                    }

                }

            }
            catch (Exception ex)
            {
                throw new Exception("MapGroupDao-->SaveMailGroup-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public bool VerifyGroup(MailGroupQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {


                if (query.row_id == 0)//新增 count=0；
                {
                    sql.AppendFormat("select count(row_id) as num from mail_group where group_name='{0}' or group_code='{1}'; ", query.group_name, query.group_code);
                    DataTable _dt = _access.getDataTable(sql.ToString());
                    if (Convert.ToInt32(_dt.Rows[0]["num"].ToString()) == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else//編輯
                {
                    sql.AppendFormat("select count(row_id) as num from mail_group where (group_name='{0}' or group_code='{1}') and row_id !={2}; ", query.group_name, query.group_code, query.row_id);
                    DataTable _dt = _access.getDataTable(sql.ToString());
                    if (Convert.ToInt32(_dt.Rows[0]["num"].ToString()) == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("MapGroupDao-->VerifyGroup-->" + sql.ToString() + ex.Message, ex);
            }
        }


        /// <summary>
        /// 刪除羣組
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string DeleteMailGroup(MailGroupQuery query)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("delete from mail_group where row_id={0};", query.row_id);
            return sql.ToString();
        }


        public int UpMailGroupStatus(MailGroupQuery query)
        {
            StringBuilder sqlGroup = new StringBuilder();
            StringBuilder sqlMap = new StringBuilder();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            int re = 0;
            try
            {
                sqlGroup.AppendFormat("update mail_group set status={0},update_time='{1}',update_user={2}  where row_id={3};", query.status, CommonFunction.DateTimeToString(query.update_time), query.update_user, query.row_id);
                //sqlMap.AppendFormat("set sql_safe_updates=0; update mail_group_map set status={0} where group_id={1};set sql_safe_updates=1; ", query.status, query.row_id);
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                    mySqlCmd.Connection = mySqlConn;
                    mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                    mySqlCmd.CommandType = System.Data.CommandType.Text;
                    mySqlCmd.CommandText = sqlGroup.ToString();
                //    mySqlCmd.CommandText += sqlMap.ToString();
                    re = mySqlCmd.ExecuteNonQuery();
                    mySqlCmd.Transaction.Commit();
                }
            }

            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("MapGroupDao-->UpMailGroupStatus-->" + mySqlCmd.ToString() + ex.Message, ex);
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


        public List<MailGroupMapQuery> QueryUserById(MailGroupMapQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {

                sql.AppendFormat(@"SELECT mu.user_mail,mu.user_name,mgm.row_id,mgm.user_id,mg.group_name
                        FROM mail_group_map mgm 
                            LEFT join mail_user mu on mgm.user_id=mu.row_id
                        LEFT JOIN mail_group mg ON mg.row_id = mgm.group_id
                        WHERE mgm.group_id={0} AND mgm.`status`={1};", query.group_id, query.status); //add by wwei0216w 2015/4/9 添加 mu.user_mail 查詢
                return _access.getDataTableForObj<MailGroupMapQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("MapGroupDao-->QueryUserById-->"+sql.ToString()+ex.Message,ex);
            }
        }

        /// <summary>
        /// 刪除羣組
        /// </summary>
        /// <param name="group_id"></param>
        /// <returns></returns>
        public int DeleteMailMap(int group_id)
        {
            string sql = string.Empty;

            try
{
                sql = string.Format("delete from mail_group_map where group_id={0};", group_id);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
    {
                throw new Exception("MapGroupDao-->DeleteMailMap-->" + sql.ToString() + ex.Message, ex);
            }
        }

        /// <summary>
        /// 選擇已選的人插入羣組中
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string SaveMailMap(MailGroupMapQuery query)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into mail_group_map (`group_id`,`user_id`,`status`,`create_time`,`create_user`,`update_time`,`update_user`)");
            sql.AppendFormat("values({0},{1},{2},'{3}',{4},'{5}',{6})",query.group_id,query.user_id,query.status,CommonFunction.DateTimeToString(query.create_time),query.create_user,CommonFunction.DateTimeToString(query.update_time),query.update_user);
            return sql.ToString();
        }

        /// <summary>
        /// 查詢用戶狀態
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        public int GetStatus(int user_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select status from mail_user where row_id={0}", user_id);
                DataTable _dt = _access.getDataTable(sql.ToString());
                if (_dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(_dt.Rows[0]["status"]);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("MapGroupDao-->GetStatus-->" + sql.ToString() + ex.Message, ex);
            }
        }
    }
}
