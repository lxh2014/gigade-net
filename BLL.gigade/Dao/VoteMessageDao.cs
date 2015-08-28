using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class VoteMessageDao
    {
        private IDBAccess _access;
        private string connString;
        public VoteMessageDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connString = connectionString;
        }
        /// <summary>
        ///活動留言列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<VoteMessageQuery> GetVoteMessageList(VoteMessageQuery query, out int totalCount)
        {
            List<VoteMessageQuery> list = new List<VoteMessageQuery>();
            StringBuilder sbSql = new StringBuilder();
            StringBuilder sbSqlCount = new StringBuilder();
            StringBuilder sbSqlCondition = new StringBuilder();

            try
            {
                sbSql.Append("select vm.message_id,vm.article_id,vm.ip,va.article_title,vm.message_status,vm.message_content,vm.create_time,vm.create_user,mu.user_username as create_name,mu2.user_username as update_name, vm.update_time,vm.update_user ");
                sbSqlCondition.Append(" from vote_message vm ");
                sbSqlCondition.Append(" left join manage_user mu on mu.user_id=vm.create_user ");
                sbSqlCondition.Append(" left join manage_user mu2 on mu2.user_id=vm.update_user ");
                sbSqlCondition.Append(" left join vote_article va on va.article_id=vm.article_id ");
                sbSqlCondition.Append(" where 1=1 ");
                sbSqlCount.Append("select count(vm.message_id) as totalCount ");
                if (!string.IsNullOrEmpty(query.article_title))
                {
                    sbSqlCondition.AppendFormat(" and va.article_title like N'%{0}%' ", query.article_title);
                }
                if (query.message_status != -1)
                {
                    sbSqlCondition.AppendFormat(" and vm.message_status ='{0}' ", query.message_status);
                }
                if (query.article_id != 0)
                {
                    sbSqlCondition.AppendFormat(" and vm.article_id ='{0}' ", query.article_id);
                }
                if (!string.IsNullOrEmpty(query.message_content))
                {
                    sbSqlCondition.AppendFormat(" and vm.message_content like N'%{0}%' ", query.message_content);
                }
                if (query.message_id != 0)
                {
                    sbSqlCondition.AppendFormat(" and vm.message_id ='{0}' ", query.message_id);
                }
                totalCount = 0;
                if (query.IsPage)
                {
                    sbSqlCount.Append(sbSqlCondition.ToString());
                    DataTable _dt = _access.getDataTable(sbSqlCount.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                    }
                    sbSqlCondition.Append(" order by vm.message_id desc ");
                    sbSqlCondition.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                }
                else {
                    sbSqlCondition.Append(" order by vm.message_id desc ");
                }
                list = _access.getDataTableForObj<VoteMessageQuery>(sbSql.Append(sbSqlCondition).ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VoteMessageDao-->GetVoteMessageList-->" + ex.Message + sbSql.Append(sbSqlCondition).ToString()+sbSqlCount.ToString(), ex);
            }
            return list;
        }
        /// <summary>
        /// 新增活動留言
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddVoteMessage(VoteMessageQuery model)
        {
            StringBuilder sbSql = new StringBuilder();
            model.Replace4MySQL();
            sbSql.Append("insert into vote_message(article_id,create_time,create_user,update_time,update_user,ip,message_status,message_content)values(");
            sbSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}');", model.article_id,  CommonFunction.DateTimeToString(model.create_time), model.create_user, CommonFunction.DateTimeToString(model.update_time), model.update_user, model.ip, model.message_status, model.message_content);
            sbSql.Append("SELECT @@IDENTITY;");
            try
            {
                DataTable _dt = _access.getDataTable(sbSql.ToString());
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
                throw new Exception("VoteMessageDao-->AddVoteMessage-->" + ex.Message + sbSql.ToString(), ex);
            }
        }
        /// <summary>
        /// 修改活動留言
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateVoteMessage(VoteMessageQuery model)
        {
            int result = 0;
            StringBuilder sbSql = new StringBuilder();
            model.Replace4MySQL();
            sbSql.Append("set sql_safe_updates=0;");
            sbSql.AppendFormat("update vote_message set article_id='{0}',update_time='{1}',update_user='{2}'", model.article_id,  CommonFunction.DateTimeToString(model.update_time), model.update_user);
            sbSql.AppendFormat(",message_content='{0}',ip='{1}' where message_id='{2}';", model.message_content,model.ip,model.message_id);
            sbSql.Append("set sql_safe_updates=1;");
            try
            {
                result = _access.execCommand(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VoteMessageDao-->UpdateVoteMessage-->" + ex.Message + sbSql.ToString(), ex);
            }
            return result;
        }

        /// <summary>
        /// 批量刪除留言
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int DelVoteMessage(VoteMessageQuery query)
        {
            int result = 0;
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("delete from vote_message where message_id in({0});", query.message_id_in);
                result = _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VoteMessageDao.DelVoteMessage-->" + ex.Message + sql.ToString(), ex);
            }
            return result;
        }
        /// <summary>
        /// 更改留言狀態
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int UpVoteMessageStatus(VoteMessageQuery query)
        {
            int result = 0;
            StringBuilder sql = new StringBuilder();
            query.Replace4MySQL();
            try
            {
                sql.Append("set sql_safe_updates=0;");
                sql.AppendFormat("update vote_message set message_status='{0}', update_time='{1}'", query.message_status,CommonFunction.DateTimeToString(query.update_time));
                sql.AppendFormat(" ,update_user='{0}',ip='{1}' where message_id='{2}'; ", query.update_user,query.ip,query.message_id);
                sql.Append("set sql_safe_updates=1;");
                result = _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VoteMessageDao.UpVoteMessageStatus-->" + ex.Message + sql.ToString(), ex);
            }
            return result;
        }
            
        /// <summary>
        /// 通過ID查詢留言信息
        /// </summary>
        /// <param name="message_id"></param>
        /// <returns></returns>
        public VoteMessageQuery GetVoteMessageQueryById(int message_id)
        {
            StringBuilder sbSql = new StringBuilder();
            StringBuilder sbSqlCondition = new StringBuilder();
            try
            {
                sbSql.Append("select vm.message_id,vm.article_id,vm.ip,va.article_title,vm.message_status,vm.message_content,vm.create_time,vm.create_user, vm.update_time,vm.update_user ");
                sbSqlCondition.Append(" from vote_message vm ");
                return _access.getSinggleObj<VoteMessageQuery>(sbSql.Append(sbSqlCondition).ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}