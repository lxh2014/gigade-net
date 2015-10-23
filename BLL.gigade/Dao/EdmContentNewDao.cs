using BLL.gigade.Common;
using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class EdmContentNewDao
    {
        private IDBAccess _access;

        public EdmContentNewDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public List<EdmContentNew> GetECNList(EdmContentNew query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            query.Replace4MySQL();
            totalCount = 0;
            try
            {
                sqlCount.AppendFormat("select count(edn.content_id) as countTotal ");
                sql.AppendFormat("select edn.content_id,edn.group_id,`subject`,esl.count,esl.date,edn.sender_id,ms.sender_email,ms.sender_name,edn.importance,edn.template_id,edn.template_data,et.edit_url,et.content_url   ");
                sqlFrom.AppendFormat("from edm_content_new edn LEFT JOIN  (SELECT content_id,COUNT(content_id) as count,MAX(schedule_date) as date from edm_send_log WHERE test_send=0 GROUP BY content_id)  esl ON edn.content_id=esl.content_id LEFT JOIN mail_sender ms on edn.sender_id=ms.sender_id LEFT JOIN edm_template et on et.template_id=edn.template_id ");
                sqlWhere.AppendFormat(" where 1=1 ");
                sqlWhere.AppendFormat(" and edn.content_createdate between '{0}' and '{1}' ", CommonFunction.DateTimeToString(DateTime.Now.AddDays(-5)), CommonFunction.DateTimeToString(DateTime.Now));
                if (query.group_id != 0)
                {
                    sqlWhere.AppendFormat(" and  edn.group_id='{0}'  ", query.group_id);
                }
                if (query.content_id != 0)
                {
                    sqlWhere.AppendFormat(" and  edn.content_id='{0}'  ", query.content_id);
                }
                DataTable _dt = _access.getDataTable(sqlCount.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0][0]);
                }
                sqlWhere.AppendFormat(" order by edn.content_id desc limit {0},{1}; ", query.Start, query.Limit);
                return _access.getDataTableForObj<EdmContentNew>(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetECNList-->" + sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString() + ex.Message, ex);
            }

        }

        public List<MailSender> GetMailSenderStore()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select sender_id,sender_email,sender_name from mail_sender;");
                return _access.getDataTableForObj<MailSender>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetECNList-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public List<EdmGroupNew> GetEdmGroupNewStore()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select group_id,group_name from edm_group_new;");
                return _access.getDataTableForObj<EdmGroupNew>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetEdmGroupNewStore-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public List<EdmTemplate> GetEdmTemplateStore()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select template_id,template_name,edit_url,content_url from edm_template where enabled=1;");
                return _access.getDataTableForObj<EdmTemplate>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetEdmTemplateStore-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public int InsertEdmContentNew(EdmContentNew query)
        {
            query.Replace4MySQL(); StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("insert into edm_content_new(group_id,`subject`,template_id,template_data, ");
                sql.Append("importance,sender_id,content_createdate,content_updatedate,content_create_userid,content_update_userid) ");
                sql.AppendFormat("values('{0}','{1}','{2}','{3}',", query.group_id, query.subject, query.template_id, query.template_data);
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}');", query.importance, query.sender_id, CommonFunction.DateTimeToString(DateTime.Now), CommonFunction.DateTimeToString(DateTime.Now), query.content_create_userid, query.content_update_userid);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->InsertEdmContentNew-->" + ex.Message + ";sql:" + sql.ToString(), ex);
            }
        }

        public int UpdateEdmContentNew(EdmContentNew query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            query.Replace4MySQL();

            try
            {
                sql.AppendFormat("update edm_content_new set group_id='{0}',subject='{1}',template_id='{2}',template_data='{3}',importance='{4}',", query.group_id, query.subject, query.template_id, query.template_data, query.importance);
                sql.AppendFormat(" sender_id='{0}',content_updatedate='{1}',content_update_userid='{2}' where content_id='{3}';", query.sender_id, CommonFunction.DateTimeToString(DateTime.Now), query.content_update_userid, query.content_id);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->InsertEdmContentNew-->" + ex.Message + ";sql:" + sql.ToString(), ex);
            }
        }

        public DataTable InsertEdmSendLog(EdmSendLog query)
        {
            StringBuilder sql = new StringBuilder();
            query.Replace4MySQL();

            try
            {
                sql.Append("insert into edm_send_log (content_id,test_send,receiver_count,schedule_date,expire_date,createdate,create_userid)values(");
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}');select @@identity", query.content_id, query.test_send, query.receiver_count, CommonFunction.DateTimeToString(query.schedule_date), CommonFunction.DateTimeToString(query.expire_date), CommonFunction.DateTimeToString(query.createdate), query.create_userid);

                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->InsertEdmSendLog-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string InsertEmailRequest(MailRequest query)
        {
            query.Replace4MySQL();

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("insert into  mail_request(priority,user_id,sender_address, ");
                sql.Append(" sender_name,receiver_address,receiver_name,subject,  ");
                sql.Append(" body, importance,schedule_date,valid_until_date, ");
                sql.Append(" retry_count,last_sent,next_send,max_retry,sent_log,request_createdate,request_updatedate,success_action,fail_action) values( ");
                sql.AppendFormat("'{0}','{1}','{2}',", query.priority, query.user_id, query.sender_address);
                sql.AppendFormat("'{0}','{1}','{2}','{3}',", query.sender_name, query.receiver_address, query.receiver_name, query.subject);
                sql.AppendFormat("'{0}','{1}','{2}','{3}',", query.body, query.importance, CommonFunction.DateTimeToString(query.schedule_date), CommonFunction.DateTimeToString(query.valid_until_date));
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}',NOW(),'{6}','{7}');", query.retry_count, CommonFunction.DateTimeToString(query.last_sent), CommonFunction.DateTimeToString(query.next_send), query.max_retry, query.sent_log, CommonFunction.DateTimeToString(DateTime.Now), query.success_action, query.fail_action);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->InsertEmailRequest-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public DataTable GetOuterCustomer(int group_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select  oc.customer_email ,oc.customer_id  from outer_customer oc LEFT JOIN outer_edm_subscription oes on oes.customer_id=oc.customer_id where oes.group_id='{0}';", group_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetOuterCustomer-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public DataTable GetInnerCustomer(int group_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select  u.user_email,es.user_id from edm_subscription es LEFT JOIN users u on es.user_id=u.user_id   where group_id='{0}';", group_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetOuterCustomer-->" + sql.ToString() + ex.Message, ex);
            }
        }

        #region 統計資料
        //發信名單統計
        public DataTable FXMD(EdmTrace query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            int totalCount = 0;
            try
            {

                sqlCount.Append("select count(et.content_id) as totalCount ");
                sql.AppendFormat(" SELECT et.success,ete.`name`,ete.email, '' as 'pic',et.count,ml.request_createdate  as 'request_createdate',et.first_traceback,et.last_traceback    ", query.content_id);
                sqlFrom.Append(" from edm_trace et LEFT JOIN edm_trace_email ete ON et.email_id=ete.email_id  LEFT JOIN mail_log ml ON et.content_id=ml.content_id AND ete.email=ml.receiver_address ");
                sqlWhere.AppendFormat(" WHERE et.content_id='{0}'   ", query.content_id);
                DataTable _dt = _access.getDataTable(sqlCount.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0][0]);
                }
                sqlWhere.AppendFormat(" limit {0},{1} ; ", query.Start, query.Limit);
                return _access.getDataTable(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->FXMD-->" + sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString() + ex.Message, ex);
            }
        }
        //開信名單下載
        public DataTable KXMD(int content_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" SELECT et.success,ete.`name`,ete.email,ete.email_id, et.count,ml.request_createdate  as 'request_createdate',et.first_traceback,et.last_traceback   from edm_trace et LEFT JOIN edm_trace_email ete ON et.email_id=ete.email_id  LEFT JOIN mail_log ml ON et.content_id=ml.content_id AND ete.email=ml.receiver_address  WHERE et.content_id='{0}' and et.count>0 and et.success=1;", content_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->KXMD-->" + sql.ToString() + ex.Message, ex);
            }
        }
        //未開信名單下載
        public DataTable WKXMD(int content_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT et.success,ete.`name`,ete.email,ete.email_id, et.count,ml.request_createdate  as 'request_createdate',et.first_traceback,et.last_traceback   from edm_trace et LEFT JOIN edm_trace_email ete ON et.email_id=ete.email_id  LEFT JOIN mail_log ml ON et.content_id=ml.content_id AND ete.email=ml.receiver_address  WHERE et.content_id='{0}' and et.count=0 and et.success=0;", content_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->WKXMD-->" + sql.ToString() + ex.Message, ex);
            }
        }
        //發信成功人數
        public int GetSendMailSCount(int content_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT COUNT(content_id) FROM edm_trace
WHERE content_id='{0}' AND edm_trace.success=1;", content_id);
                return int.Parse(_access.getDataTable(sql.ToString()).Rows[0][0].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetSendMailSCount-->" + sql.ToString() + ex.Message, ex);
            }
        }
        //發信失敗人數
        public int GetSendMailFCount(int content_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT COUNT(content_id) FROM edm_trace
WHERE content_id='{0}' AND edm_trace.success=0;", content_id);
                return int.Parse(_access.getDataTable(sql.ToString()).Rows[0][0].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetSendMailFCount-->" + sql.ToString() + ex.Message, ex);
            }
        }
        //總開信人數
        public int GetSendMailCount(int content_id)
        {
            StringBuilder sql = new StringBuilder();
            int result = 0;
            try
            {
                sql.AppendFormat(@"SELECT count(edm_trace.content_id) FROM edm_trace 
WHERE content_id='{0}' AND edm_trace.count>0;", content_id);
                DataTable _dt = _access.getDataTable(sql.ToString());
                if (int.TryParse(_dt.Rows[0][0].ToString(), out result))
                {
                    result = Convert.ToInt32(_dt.Rows[0][0].ToString());
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetSendMailCount-->" + sql.ToString() + ex.Message, ex);
            }
        }
        //總開信ci數
        public int GetSendCount(int content_id)
        {
            StringBuilder sql = new StringBuilder();
            int result = 0;
            try
            {
                sql.AppendFormat(@"SELECT SUM(edm_trace.count) FROM edm_trace 
WHERE content_id='{0}' AND edm_trace.count>0;", content_id);
                DataTable _dt = _access.getDataTable(sql.ToString());
                if (int.TryParse(_dt.Rows[0][0].ToString(), out result))
                {
                    result = Convert.ToInt32(_dt.Rows[0][0].ToString());
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetSendCount-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        public DataTable InsertEdmTraceEmail(EdmTraceEmail query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" insert into edm_trace_email (`email`,`name`)values('{0}','{1}');select @@identity;", query.email, query.name);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetOuterCustomer-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string InsertEdmTrace(EdmTrace query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" insert into edm_trace (log_id,content_id,email_id,count,success) values('{0}','{1}','{2}','{3}','{4}');", query.log_id, query.content_id, query.email_id, query.count, query.success);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->InsertEdmTrace-->" + sql.ToString() + ex.Message, ex);
            }
        }

        /// <summary>
        ///電子報統計報表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable EdmTrace(EdmTrace query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            int totalCount = 0;
            try
            {
                sqlCount.Append(" select count(et.log_id)  as totalCount  ");
                sql.Append("select * from(SELECT DATE_FORMAT(etl.trace_day,'%Y-%m-%d') as 'trace_day',count(etl.log_id) as 'openPerson',SUM(etl.trace_count) as 'openCount'   ");
                sqlFrom.Append("from edm_trace et LEFT JOIN edm_trace_log etl ON et.content_id=etl.content_id AND et.email_id=etl.email_id AND et.log_id=etl.log_id  ");
                sqlWhere.AppendFormat(" where  et.content_id='{0}'  and et.first_traceback>0   GROUP BY DATE_FORMAT(etl.trace_day,'%Y-%m-%d')  ", query.content_id);
                DataTable _dt = _access.getDataTable(sqlCount.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0][0]);
                }
                sqlWhere.AppendFormat("  ) e where e.openPerson>0 limit {0},{1};  ", query.Start, query.Limit);
                return _access.getDataTable(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("EdmContentNewDao-->EdmTrace-->" + sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString() + ex.Message, ex);
            }
        }

    }
}
