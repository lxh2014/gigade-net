using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao
{
    public class VoteEventDao
    {
        private IDBAccess _access;
        public VoteEventDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
        }
        public List<VoteEventQuery> GetVoteEventList(VoteEventQuery query, out int totalcount)
        {
            StringBuilder sqlfield = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            try
            {
                sqlfield.AppendFormat(@" SELECT	event_id,event_name,event_desc,event_banner,event_start, ");
                sqlfield.AppendFormat(@" event_end,word_length,vote_everyone_limit,vote_everyday_limit,number_limit, ");
                sqlfield.AppendFormat(@" present_event_id,create_user,create_time,update_user,update_time,event_status,is_repeat ");
                sqlwhere.AppendFormat(@" from vote_event ");
                sqlwhere.AppendFormat(" where 1=1 ");
                sqlcount.Append("select count(event_id) as totalCount ");
                if (query.event_id != 0)
                {
                    sqlwhere.AppendFormat(@" and (event_id='{0}' or event_name like N'%{0}%' or event_desc like N'%{0}%' ) ", query.event_id);
                }
                if (!string.IsNullOrEmpty(query.event_name))
                {
                    sqlwhere.AppendFormat(@" and (event_name like N'%{0}%' or event_desc like N'%{0}%')", query.event_name);
                }
                
                
                totalcount = 0;
                if (query.IsPage)
                {
                    sqlcount.Append(sqlwhere.ToString());
                    //DataTable _dt = _access.getDataTable("select event_id " + sqlfrom.ToString() + sqlwhere.ToString());
                    DataTable _dt = _access.getDataTable(sqlcount.ToString());
                    totalcount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    sqlwhere.AppendFormat(@" ORDER BY event_id DESC ");
                    sqlwhere.AppendFormat(@" limit {0},{1}", query.Start, query.Limit);
                }
                else {
                    sqlwhere.AppendFormat(@" ORDER BY event_id DESC ");
                }
                sqlfield.Append(sqlwhere.ToString());
                return _access.getDataTableForObj<VoteEventQuery>(sqlfield.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VoteEventDao.GetVoteEventList-->" + ex.Message + sqlfield.ToString() + sqlcount.ToString(), ex);
            }
        }
        public List<VoteEventQuery> GetVoteEventDownList(VoteEventQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendLine(@"SELECT event_id,CONCAT_WS('-',event_id,event_name) AS event_name FROM vote_event");
                sql.AppendFormat(" where 1=1 ");
                if (query.event_id != 0)
                {
                    sql.AppendFormat(@" and event_id='{0}' ", query.event_id);
                }
                //if (!string.IsNullOrEmpty(query.event_name))
                //{
                //    sql.AppendFormat(@" and event_name='{0}' ", query.event_name);
                //}
                if (query.event_status != 0)
                {
                    sql.AppendFormat(@" and event_status='{0}' ", query.event_status);
                }
                return _access.getDataTableForObj<VoteEventQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VoteEventDao.GetVoteEventDownList-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int Save(VoteEvent ve)
        {
            StringBuilder sql = new StringBuilder();
            ve.Replace4MySQL();
            try
            {
                sql.AppendFormat(@" INSERT INTO vote_event(event_name,event_desc,event_banner,event_start,");
                sql.AppendFormat(@"  event_end,word_length,vote_everyone_limit,vote_everyday_limit,number_limit,  ");
                sql.AppendFormat(@" present_event_id,create_user,create_time,update_user,update_time,is_repeat,event_status)");
                sql.AppendFormat(@" VALUES ('{0}','{1}','{2}','{3}',", ve.event_name, ve.event_desc, ve.event_banner, ve.event_start.ToString("yyyy-MM-dd HH:mm;ss"));
                sql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}',", ve.event_end.ToString("yyyy-MM-dd HH:mm;ss"), ve.word_length, ve.vote_everyone_limit, ve.vote_everyday_limit, ve.number_limit);
                sql.AppendFormat(@" '{0}','{1}','{2}','{3}','{4}', ", ve.present_event_id, ve.create_user, ve.create_time.ToString("yyyy-MM-dd HH:mm;ss"), ve.update_user, ve.update_time.ToString("yyyy-MM-dd HH:mm;ss"));
                sql.AppendFormat(@"'{0}','{1}')", ve.is_repeat,ve.event_status);

                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VoteEventDao-->Save-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int Update(VoteEvent ve)
        {
            StringBuilder sql = new StringBuilder();
            ve.Replace4MySQL();
            try
            {
                sql.AppendFormat(@"UPDATE vote_event  set ");
                sql.AppendFormat(@" event_name='{0}',event_desc='{1}',event_banner='{2}',", ve.event_name, ve.event_desc, ve.event_banner);
                sql.AppendFormat(@" event_start='{0}',event_end='{1}',", ve.event_start.ToString("yyyy-MM-dd HH:mm:ss"), ve.event_end.ToString("yyyy-MM-dd HH:mm:ss"));
                sql.AppendFormat(@" word_length='{0}',vote_everyone_limit='{1}',vote_everyday_limit='{2}',", ve.word_length, ve.vote_everyone_limit, ve.vote_everyday_limit);
                sql.AppendFormat(@" number_limit='{0}',present_event_id='{1}',", ve.number_limit, ve.present_event_id);
                sql.AppendFormat(@" update_user='{0}',update_time='{1}',is_repeat='{2}' ", ve.update_user, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ve.is_repeat);
                sql.AppendFormat(" WHERE event_id='{0}' ;", ve.event_id);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VoteEventDao-->Update-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int UpdateState(VoteEvent ve)
        {
            ve.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"UPDATE vote_event SET event_status='{0}',update_user='{1}',", ve.event_status, ve.update_user);
            sql.AppendFormat(@"update_time='{0}' ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sql.AppendFormat(@" where event_id='{0}';", ve.event_id);
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VoteEventDao-->UpdateState" + ex.Message + sql.ToString(), ex);
            }
        }
        public int SelectByEventName(VoteEvent m)
        {
            StringBuilder str = new StringBuilder();
            try
            {
                str.AppendFormat("SELECT event_name,event_id FROM vote_event  where  event_name ='{0}' and event_id<>'{1}'", m.event_name, m.event_id);
                return _access.getDataTable(str.ToString()).Rows.Count;
            }
            catch (Exception ex)
            {
                throw new Exception(" VoteEventDao-->SelectByEventName-->" + ex.Message + "sql:" + str.ToString(), ex);
            }
        }
    }
}