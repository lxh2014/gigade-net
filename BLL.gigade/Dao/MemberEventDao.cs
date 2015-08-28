using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Common;
using System.Data;
using MySql.Data.MySqlClient;
namespace BLL.gigade.Dao
{
    public class MemberEventDao
    {
        private IDBAccess _access;
        private string connStr;
        public MemberEventDao(string connectionString)
        {
            this.connStr = connectionString;
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        #region 會員活動列表查詢+List<MemberEventQuery> Query(MemberEventQuery query, out int totalCount)
        /// <summary>
        /// 會員活動列表查詢
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<MemberEventQuery> Query(MemberEventQuery query, out int totalCount)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder strCondition = new StringBuilder();
            StringBuilder strSelect = new StringBuilder();
            StringBuilder strTemp = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                totalCount = 0;
                strSelect.AppendFormat(@"select me.rowID,me.me_name,me.me_desc,me.me_startdate,me.me_enddate,me.et_id,me.me_birthday,me.event_id, me.me_big_banner,me.me_banner_link, ");
                strSelect.AppendFormat(@"me.me_bonus_onetime,me.ml_code,me.me_status,me.k_date,me.k_user,");
                strSelect.AppendFormat(@"me.m_date,me.m_user,et.et_name,et.et_date_parameter,et.et_starttime,et.et_endtime  ");
                strCondition.AppendFormat(@" from member_event me  ");
                strCondition.AppendFormat(@" left join event_type et on me.et_id=et.et_id ");
                strCondition.AppendFormat(@" where 1=1 ");
                if (query.rowID != 0)
                {
                    strCondition.AppendFormat(@" and me.rowID='{0}' ",query.rowID);
                }
                if (!string.IsNullOrEmpty(query.ml_name))
                {
                    strCondition.AppendFormat(@" AND (me.rowID LIKE  N'%{0}%' OR me.me_name LIKE '%{0}%' OR me.me_desc LIKE  N'%{0}%') ", query.ml_name);
                }
                if (!string.IsNullOrEmpty(query.timestart))
                {
                    strCondition.AppendFormat(" and me.k_date>='{0}' ",query.timestart);
                }
                if (!string.IsNullOrEmpty(query.timestart))
                {
                    strCondition.AppendFormat(" and me.k_date<='{0}' ", query.timeend);
                }
                if (query.IsPage)
                {
                    strSql.AppendFormat(@" select count(rowID) as totalCount ");
                    strSql.AppendFormat(strCondition.ToString());
                    System.Data.DataTable _dt = _access.getDataTable(strSql.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    strSql.AppendFormat(";");
                    strCondition.AppendFormat(@" order by rowID desc ");
                }
                strTemp.AppendFormat(strSelect.ToString());
                strTemp.AppendFormat(strCondition.ToString());
                strTemp.AppendFormat(@" limit {0},{1}", query.Start, query.Limit);
                strTemp.AppendFormat(";");
                strSql.AppendFormat(strTemp.ToString());
                return _access.getDataTableForObj<MemberEventQuery>(strTemp.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("MemberEventDao-->Query-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 會員活動新增+int MemberEventSave(MemberEventQuery query)
        /// <summary>
        /// 會員活動新增編輯
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int MemberEventSave(MemberEventQuery meq)
        {
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            StringBuilder sql = new StringBuilder();
            meq.Replace4MySQL();
            int i = 0;
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }

                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                //新增
                if (meq.rowID == 0)
                {
                    //獲得自增列下一個ID
                    sql.AppendFormat(@"SELECT AUTO_INCREMENT FROM information_schema.TABLES WHERE  TABLE_NAME = 'event_type';");
                    mySqlCmd.CommandText = sql.ToString();
                    object ob = mySqlCmd.ExecuteScalar();
                    if (ob != null)
                    {
                        meq.et_id = int.Parse(ob.ToString());
                    }
                    sql.AppendFormat(@"INSERT INTO member_event (me_name,me_desc,me_startdate,me_enddate,");
                    sql.AppendFormat(@" et_id,me_birthday,event_id,me_bonus_onetime,ml_code,me_status,");
                    sql.AppendFormat(@"  me_big_banner,me_banner_link,k_date,k_user,m_date,m_user) ");
                    sql.AppendFormat(@" VALUES('{0}','{1}','{2}','{3}',", meq.me_name, meq.me_desc, meq.me_startdate.ToString("yyyy-MM-dd"), meq.me_enddate.ToString("yyyy-MM-dd"));
                    sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}','{5}',", meq.et_id, meq.me_birthday, meq.event_id, meq.me_bonus_onetime, meq.ml_code, meq.me_status);
                    sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}','{5}'); ",meq.me_big_banner,meq.me_banner_link, meq.k_date.ToString("yyyy-MM-dd HH:mm:ss"), meq.k_user, meq.m_date.ToString("yyyy-MM-dd HH:mm:ss"), meq.m_user);
                    sql.AppendFormat(@"INSERT INTO event_type (et_name,et_date_parameter,et_starttime,et_endtime) ");
                    sql.AppendFormat(@"VALUES('{0}','{1}',", meq.et_name, meq.et_date_parameter);
                    sql.AppendFormat(@" '{0}','{1}');", meq.et_starttime, meq.et_endtime);

                }
                //編輯
                else
                {
                    sql.AppendFormat(@"UPDATE member_event SET me_name='{0}',me_desc='{1}',", meq.me_name, meq.me_desc);
                    sql.AppendFormat(@"me_startdate='{0}',me_enddate='{1}',", meq.me_startdate.ToString("yyyy-MM-dd"), meq.me_enddate.ToString("yyyy-MM-dd"));
                    sql.AppendFormat(@"me_birthday='{0}',event_id='{1}',", meq.me_birthday, meq.event_id);
                    sql.AppendFormat(@"me_bonus_onetime='{0}',ml_code='{1}',me_big_banner='{2}',me_banner_link='{3}',", meq.me_bonus_onetime, meq.ml_code, meq.me_big_banner,meq.me_banner_link);
                    sql.AppendFormat(@"m_date='{0}',m_user='{1}',me_status='{2}' ", meq.m_date.ToString("yyyy-MM-dd HH:mm:ss"), meq.m_user,0);
                    sql.AppendFormat(@" WHERE rowID={0};", meq.rowID);
                    sql.AppendFormat(@"UPDATE event_type SET et_name='{0}',et_date_parameter='{1}',", meq.et_name, meq.et_date_parameter);
                    sql.AppendFormat(@"et_starttime='{0}',et_endtime='{1}' ", meq.et_starttime, meq.et_endtime);
                    sql.AppendFormat(@" WHERE et_id='{0}';", meq.et_id);
                }

                mySqlCmd.CommandText = sql.ToString();
                i = mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("MemberEventDao-->MemberEventSave-->" + ex.Message + sql.ToString(), ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
            return i;
        }
        #endregion

        /// <summary>
        /// 更新狀態
        /// </summary>
        /// <param name="meq"></param>
        /// <returns></returns>
        public int UpdateState(MemberEventQuery meq)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"UPDATE member_event SET me_status='{0}',m_date='{1}' ", meq.me_status, meq.m_date.ToString("yyyy-MM-dd HH:mm:ss"));
            sql.AppendFormat(@" ,m_user='{0}' WHERE rowID='{1}'; ", meq.m_user, meq.rowID);
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("MemberEventDao-->UpdateState" + ex.Message + sql.ToString(), ex);
            }
        }
        /// <summary>
        /// 查詢會員等級列表
        /// </summary>
        /// <param name="ml"></param>
        /// <returns></returns>
        public DataTable GetMemberLevelList(MemberLevel ml)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"SELECT ml_code,ml_name FROM member_level WHERE 1=1 ");
            if (ml.ml_status != 0)
            {
                sql.AppendFormat(@" AND ml_status=1; ");
            }
            try
            {
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("MemberEventDao-->GetMemberLevelList" + ex.Message + sql.ToString(), ex);
            }

        }
        ///// <summary>
        ///// 會員級別保存
        ///// </summary>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //public int EventGroupSave(MemberEventQuery query)
        //{
        //    StringBuilder strSql = new StringBuilder();
        //    try
        //    {
        //        if (query.eg_id == 0)
        //        {
        //            strSql.AppendFormat("insert into event_group (ml_id)values('{0}');select @@identity;", query.ml_id);
        //        }
        //        else
        //        {
        //            strSql.AppendFormat("update event_group set ml_id='{0}'where eg_id={1};select @@identity;", query.ml_id, query.eg_id);
        //        }
        //        DataTable dt = _access.getDataTable(strSql.ToString());
        //        return Convert.ToInt32(dt.Rows[0][0]);

        //    }
        //    catch (Exception ex)
        //    {

        //        throw new Exception("MemberEventDao-->EventGroupSave-->" + ex.Message, ex);
        //    }
        //}
        ///// <summary>
        ///// 群組名稱
        ///// </summary>
        ///// <param name="query"></param>
        ///// <returns></returns>
        // public string GetMlName(MemberEventQuery query)
        // {
        //     StringBuilder strSql = new StringBuilder();
        //     try
        //     {
        //         strSql.AppendFormat("SELECT group_concat( LEFT(ml_name,1)) as ml_name FROM member_level where rowID in ({0})", query.ml_id);
        //         DataTable dt = _access.getDataTable(strSql.ToString());
        //         return dt.Rows[0]["ml_name"].ToString();
        //     }
        //     catch (Exception ex)
        //     {
        //         throw new Exception("MemberEventDao-->GetMlName-->" + ex.Message, ex);
        //     }
        // }


        #region 會員活動判斷數據不能重複
        public DataTable InsertRepeat(MemberEventQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select  me.rowID   from member_event me where me.event_id='{0}' and ml_code='{1}';",query.event_id,query.ml_code);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("MemberEventDao-->InsertRepeat" + ex.Message + sql.ToString(), ex);
            }
        }

        public DataTable UpdateRepeat(MemberEventQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select me.rowID   from member_event me where me.event_id='{0}' and ml_code='{1}' and rowID!='{2}';",query.event_id,query.ml_code,query.rowID);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("MemberEventDao-->UpdateRepeat" + ex.Message + sql.ToString(), ex);
            }
        }

        public DataTable IsGetEventID(string event_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select `start`,`end` from new_promo_present where event_id='{0}' and status=1;",event_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("MemberEventDao-->IsGetEventID" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
    }
}
