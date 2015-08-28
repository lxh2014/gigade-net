using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model.Query;
using BLL.gigade.Common;
using BLL.gigade.Model;
using System.Data;
using BLL.gigade.Dao.Impl;
using System.Text.RegularExpressions;
/**
 * chaojie1124j
 */
namespace BLL.gigade.Dao
{
    public class SmsDao : ISmsImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        public SmsDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        #region 事物 添加電話會員信息 記錄到user_history 返回sql語句
        /// <summary>
        /// 用於返回事物所用到的sql語句
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string SaveSms(Sms model)
        {
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" insert into sms (type,order_id,mobile,subject,content,estimated_send_time,send,created,modified) ");
            sb.AppendFormat(" values({0},{1},'{2}','{3}','{4}','{5}','{6}','{7}','{8}');", model.type, model.order_id, model.mobile, model.subject, model.content, CommonFunction.DateTimeToString(model.estimated_send_time), model.send, CommonFunction.DateTimeToString(model.created), CommonFunction.DateTimeToString(model.modified));
            return sb.ToString();
        }
        #endregion

        #region 客服管理=>簡訊查詢
        /// <summary>
        /// 簡訊查詢列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
        public List<SmsQuery> GetSmsList(SmsQuery query, out int totalcount)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {// FROM_UNIXTIME(UNIX_TIMESTAMP(modified)) as modified_time, FROM_UNIXTIME(UNIX_TIMESTAMP(estimated_send_time)) as estimated_time,created_time
                sql.Append(@" select id,order_id,mobile,subject,content,send,trust_send,FROM_UNIXTIME(UNIX_TIMESTAMP(created)) as created ");
                sql.Append(" ,FROM_UNIXTIME(UNIX_TIMESTAMP(estimated_send_time))as estimated_send_time,FROM_UNIXTIME(UNIX_TIMESTAMP(modified)) as modified");
                sqlCondi.Append(" from sms ");
                sqlCondi.Append(" where 1=1 ");
                if (!string.IsNullOrEmpty(AddSqlQuery(query)))
                {
                    sqlCondi.Append(AddSqlQuery(query));
                }
                totalcount = 0;
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable("select count(id) as totalCount " + sqlCondi.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalcount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                    }

                    sqlCondi.Append(" order by id desc ");
                    sqlCondi.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                }

                sql.Append(sqlCondi.ToString());

                return _access.getDataTableForObj<SmsQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SmsDao.GetSmsList-->" + ex.Message + sql.ToString(), ex);
            }

        }
        /// <summary>
        /// 修改狀態
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int updateSms(SmsQuery query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"update  sms set content='{0}',send='{1}'  where id='{2}' ", query.content, query.send, query.id);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SmsDao.updateSms-->" + ex.Message + sql.ToString(), ex);
            }
        }
        /// <summary>
        /// 把要查詢的額外條件根據給擴展類的一些屬性的值來添加sql
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string AddSqlQuery(SmsQuery query)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(query.created_time))
            {
                if (query.StartTime > DateTime.MinValue)
                {
                    sb.AppendFormat(" and created >='{0}' ", query.StartTime.ToString("yyyy-MM-dd 00:00:00"));
                }
                if (query.EndTime > DateTime.MinValue)
                {
                    sb.AppendFormat(" and created <='{0}' ", query.EndTime.ToString("yyyy-MM-dd 23:59:59"));
                }
            }
            else if (!string.IsNullOrEmpty(query.modified_time))
            {
                sb.AppendFormat(" and send='1' ");
                if (query.StartTime > DateTime.MinValue)
                {
                    sb.AppendFormat(" and modified >='{0}' ", query.StartTime.ToString("yyyy-MM-dd 00:00:00"));
                }
                if (query.EndTime > DateTime.MinValue)
                {
                    sb.AppendFormat(" and modified <='{0}' ", query.EndTime.ToString("yyyy-MM-dd 23:59:59"));
                }
            }
            if (!string.IsNullOrEmpty(query.content))
            {
                Regex regex = new System.Text.RegularExpressions.Regex("^[0-9]*$");
                if (regex.IsMatch(query.content))
                {
                    sb.AppendFormat(" and (id='{0}' or order_id='{0}' or mobile='{0}') ", query.content);
                }
                else
                {
                    sb.AppendFormat(" and mobile='{0}' ", query.content);
                }
            }
            if (query.send != -1)
            {
                sb.AppendFormat(" and send='{0}'", query.send);
            }
            if (!string.IsNullOrEmpty(query.trust_send))
            {
                sb.AppendFormat(" and trust_send in ({0})", query.trust_send);
            }
            if (query.id != 0)
            {
                sb.AppendFormat(" and id='{0}'", query.id);
            }
            return sb.ToString();
        }
        #endregion

        #region 客服管理=>聯絡客服列表
        /// <summary>
        /// 新增sms表數據
        /// </summary>
        /// <param name="query">新增信息</param>
        /// <returns></returns>
        public int InsertSms(SmsQuery query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(@"INSERT INTO sms (type,serial_id,order_id,mobile,content,subject,estimated_send_time,send,trust_send,created,modified) ");
                sql.AppendFormat(@" VALUES('{0}','{1}','{2}','{3}','{4}'", query.type, query.serial_id, query.order_id, query.mobile, query.content);
                sql.AppendFormat(@",'{0}','{1}','{2}','{3}'", "", CommonFunction.DateTimeToString(query.estimated_send_time), query.send, query.trust_send);
                sql.AppendFormat(@",'{0}','{1}')", CommonFunction.DateTimeToString(query.created), CommonFunction.DateTimeToString(query.modified));
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SmsDao.InsertSms-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
    }
}


