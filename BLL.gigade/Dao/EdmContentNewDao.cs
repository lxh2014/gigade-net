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
                sql.AppendFormat("select edn.content_id,edn.group_id,`subject`,esl.count,esl.date,edn.sender_id,ms.sender_email,ms.sender_name,edn.importance,edn.template_id,edn.template_data,'' as 'template_data_send', et.edit_url,et.content_url , edn.pm,  para.parameterName 'edm_pm'  ");
                sqlFrom.AppendFormat("from edm_content_new edn LEFT JOIN  (SELECT content_id,COUNT(content_id) as count,MAX(schedule_date) as date from edm_send_log WHERE test_send=0 GROUP BY content_id)  esl ON edn.content_id=esl.content_id LEFT JOIN mail_sender ms on edn.sender_id=ms.sender_id LEFT JOIN edm_template et on et.template_id=edn.template_id ");
                sqlFrom.Append(" left join (select  parameterCode,parameterName from t_parametersrc where parameterType='edm_pm_name' and used=1) para on edn.pm=para.parameterCode    ");
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
                sql.Append("select group_id,group_name from edm_group_new where enabled=1 order by  is_member_edm  desc, sort_order  ;");
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
                sql.Append("select template_id,template_name,edit_url,content_url from edm_template where enabled=1 order by  template_id asc;");
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
                sql.Append("importance,sender_id,content_createdate,content_updatedate,content_create_userid,content_update_userid,pm) ");
                sql.AppendFormat("values('{0}','{1}','{2}','{3}',", query.group_id, query.subject, query.template_id, query.template_data);
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}');", query.importance, query.sender_id, CommonFunction.DateTimeToString(DateTime.Now), CommonFunction.DateTimeToString(DateTime.Now), query.content_create_userid, query.content_update_userid,query.pm);
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
                sql.AppendFormat("update edm_content_new set group_id='{0}',subject='{1}',template_id='{2}',template_data='{3}',importance='{4}',pm='{5}', ", query.group_id, query.subject, query.template_id, query.template_data, query.importance,query.pm);
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
                sql.AppendFormat("'{0}','{1}','{2}','{3}',", query.bodyData, query.importance, CommonFunction.DateTimeToString(query.schedule_date), CommonFunction.DateTimeToString(query.valid_until_date));
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
                sql.AppendFormat(" SELECT et.success,ete.`name`,ete.email,et.count,et.send_date,et.first_traceback,et.last_traceback    ", query.content_id);
                sqlFrom.Append(" from edm_trace et LEFT JOIN edm_trace_email ete ON et.email_id=ete.email_id ");
                sqlWhere.AppendFormat(" WHERE et.content_id='{0}'  and et.log_id='{1}'  ", query.content_id,query.log_id);
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
        public DataTable KXMD(int content_id,int log_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" SELECT et.success,ete.`name`,ete.email,ete.email_id, et.count,et.send_date,et.first_traceback,et.last_traceback   from edm_trace et LEFT JOIN edm_trace_email ete ON et.email_id=ete.email_id  WHERE et.content_id='{0}'  and et.log_id='{1}'   and et.count>0 and et.success=1;", content_id, log_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->KXMD-->" + sql.ToString() + ex.Message, ex);
            }
        }
        //未開信名單下載
        public DataTable WKXMD(int content_id,int log_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT et.success,ete.`name`,ete.email,ete.email_id, et.count,et.send_date  from edm_trace et LEFT JOIN edm_trace_email ete ON et.email_id=ete.email_id  WHERE et.content_id='{0}'  and et.log_id='{1}' and et.count=0 and et.success=1;", content_id,log_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->WKXMD-->" + sql.ToString() + ex.Message, ex);
            }
        }
        //發信成功人數
        public int GetSendMailSCount(int content_id, int log_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT COUNT(content_id) FROM edm_trace
WHERE content_id='{0}' and log_id='{1}'  AND edm_trace.success=1;", content_id, log_id);
                return int.Parse(_access.getDataTable(sql.ToString()).Rows[0][0].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetSendMailSCount-->" + sql.ToString() + ex.Message, ex);
            }
        }
        //發信失敗人數
        public int GetSendMailFCount(int content_id, int log_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT COUNT(content_id) FROM edm_trace
WHERE content_id='{0}' and log_id='{1}' AND edm_trace.success=0;", content_id, log_id);
                return int.Parse(_access.getDataTable(sql.ToString()).Rows[0][0].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetSendMailFCount-->" + sql.ToString() + ex.Message, ex);
            }
        }
        //總開信人數
        public int GetSendMailCount(int content_id,int log_id)
        {
            StringBuilder sql = new StringBuilder();
            int result = 0;
            try
            {
                sql.AppendFormat(@"SELECT count(edm_trace.content_id) FROM edm_trace 
WHERE content_id='{0}'  and log_id='{1}'  AND edm_trace.count>0;", content_id, log_id);
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
        public int GetSendCount(int content_id, int log_id)
        {
            StringBuilder sql = new StringBuilder();
            int result = 0;
            try
            {
                sql.AppendFormat(@"SELECT SUM(edm_trace.count) FROM edm_trace 
WHERE content_id='{0}'  and log_id='{1}'   AND edm_trace.count>0;", content_id, log_id);
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
                sql.AppendFormat(" insert into edm_trace (log_id,content_id,email_id,first_traceback,last_traceback,count,success) values('{0}','{1}','{2}',NOW(),NOW(),'{3}','{4}');", query.log_id, query.content_id, query.email_id, query.count, query.success);
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
                sqlWhere.AppendFormat(" where  et.content_id='{0}'  and et.first_traceback>0  and et.log_id='{1}'  GROUP BY DATE_FORMAT(etl.trace_day,'%Y-%m-%d')  ", query.content_id,query.log_id);
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

        public DataTable CreatedateAndLogId(int content_id)
        {
            DataTable _dt = new DataTable();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select log_id,createdate from edm_send_log where test_send=0  and  content_id='{0}' order by createdate desc ;",content_id);
                _dt = _access.getDataTable(sql.ToString());
                return _dt;
            }
            catch (Exception ex)
            {

                throw new Exception("EdmContentNewDao-->EdmTrace-->" + sql.ToString()+ex.Message, ex);
            }
        }

        public DataTable GetScheduleDate(int content_id, int log_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select schedule_date from edm_send_log where content_id='{0}' and log_id='{1}';",content_id,log_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetScheduleDate-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public DataTable GetPraraData(int parameterCode )
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("SELECT parameterName from t_parametersrc WHERE parameterType='edm_type' and parameterCode='{0}';", parameterCode);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetPraraData-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public DataTable GetEditUrl(int template_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select edit_url  from edm_template where template_id='{0}';",template_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetEditUrl-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public DataTable GetContentUrl(int template_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select content_url  from edm_template where template_id='{0}';", template_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetContentUrl-->" + sql.ToString() + ex.Message, ex);
            }
        }


        public DataTable GetHtml(EdmContentNew query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select  template_data from edm_content_new where content_id='{0}' and template_id='{1}';", query.content_id, query.template_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetHtml-->" + sql.ToString() + ex.Message, ex);
            }
        }

        #region 寄信排成
        //清除過期信件
        public int ValidUntilDate()
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sql1 = new StringBuilder();
            List<MailRequest> MR = new List<MailRequest>();
            try
            {
                sql1.AppendFormat("SELECT request_id,priority,user_id,sender_address,sender_name,receiver_address,receiver_name,`subject`,importance,schedule_date,valid_until_date,retry_count,last_sent,sent_log,request_createdate,request_updatedate from mail_request where valid_until_date<'{0}'  ;", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                MR = _access.getDataTableForObj<MailRequest>(sql1.ToString());
                sql.Append(InsertLog(MR, "3"));
                if (sql.Length > 0)
                {
                    return _access.execCommand(sql.ToString());
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleServiceDao-->SchedulePeriodDelete-->" + sql.ToString() + ex.Message);
            }
        }
        //清除重複過多次數的信件
        public int MaxRetry()
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sql1 = new StringBuilder();
            List<MailRequest> MR = new List<MailRequest>();
            try
            {
                sql1.AppendFormat("SELECT request_id,priority,user_id,sender_address,sender_name,receiver_address,receiver_name,`subject`,importance,schedule_date,valid_until_date,retry_count,last_sent,sent_log,request_createdate,request_updatedate from mail_request where retry_count<>0 and retry_count >= max_retry;");
                MR = _access.getDataTableForObj<MailRequest>(sql1.ToString());
                sql.Append(InsertLog(MR, "2"));
                if (sql.Length > 0)
                {
                    return _access.execCommand(sql.ToString());
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleServiceDao-->SchedulePeriodDelete-->" + sql.ToString() + ex.Message);
            }
        }
        //
        public bool SendEMail(MailHelper mail)
        {
            DataTable dt = new DataTable();
            StringBuilder sql = new StringBuilder();
            StringBuilder sql1 = new StringBuilder();
            StringBuilder sql2 = new StringBuilder();
            List<MailRequest> MR = new List<MailRequest>();

            //MailHelper mail = new MailHelper();
            try
            {
                sql1.AppendFormat("SELECT request_id,priority,user_id,sender_address,sender_name,receiver_address,receiver_name,`subject`,importance,schedule_date,valid_until_date,retry_count,last_sent,sent_log,request_createdate,request_updatedate,body,success_action,fail_action from mail_request where schedule_date<'{0}'   order by next_send,priority,valid_until_date;", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                MR = _access.getDataTableForObj<MailRequest>(sql1.ToString());
                int next_time = int.Parse(_access.getDataTable("SELECT parameterName from t_parametersrc where parameterType='edm_type' AND parameterCode='4';").Rows[0][0].ToString());
                sql2.Append("SELECT email_address from email_block_list;");
                dt = _access.getDataTable(sql2.ToString());
                foreach (var item in MR)
                {
                    bool black = true;
                    //擋信名單排除
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (item.receiver_address.ToString() == dt.Rows[i][0].ToString())
                        {
                            //刪除擋信名單的數據
                            sql.Append(InsertLog(item, "4"));
                            black = false;
                            if (sql.Length > 0)
                            {
                                _access.execCommand(sql.ToString());
                                sql.Clear();
                            }
                        }
                    }
                    if (black)
                    {//是不是擋信名單的email
                        try
                        {
                            if (mail.SendMailAction(item.receiver_address.ToString(), item.subject.ToString(), item.body.ToString(), item.sender_address, item.sender_name))
                            {
                                sql.Append(item.success_action);
                                //發送成功刪除原數據新增log
                                sql.Append(InsertLog(item, "1"));
                            }
                            else
                            {
                                //發送失敗更新數據
                                sql.Append(item.fail_action);
                                sql.AppendFormat("update mail_request set retry_count ='{1}',next_send='{2}',sent_log='{3}' where request_id='{0}' ;", item.request_id, item.retry_count + 1, DateTime.Now.AddMinutes(next_time), "not errow massage");
                                //sql.Append(item.fail_action + ";");
                            }
                            if (sql.Length > 0)
                            {
                                _access.execCommand(sql.ToString());
                                sql.Clear();
                            }
                        }
                        catch (Exception ex)
                        {
                            item.sent_log = ex.ToString();
                            item.Replace4MySQL();
                            sql.Append(item.fail_action);
                            //發送失敗更新數據
                            sql.AppendFormat("update mail_request set retry_count ='{1}',next_send='{2}',sent_log='{3}' where request_id='{0}' ;", item.request_id, item.retry_count + 1, DateTime.Now.AddMinutes(next_time).ToString("yyyy-MM-dd HH:mm:ss"), item.sent_log);
                            _access.execCommand(sql.ToString());
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleServiceDao-->SchedulePeriodDelete-->" + sql.ToString() + ex.Message);
            }
        }
        // 刪除mailrequest 新增log
        public string InsertLog(List<MailRequest> q, string mail_result)
        {
            StringBuilder sb = new StringBuilder();

            string id = "";
            try
            {
                if (q.Count > 0)
                {
                    foreach (var m in q)
                    {
                        m.Replace4MySQL();
                        sb.AppendFormat("insert into mail_log (priority,user_id,send_address,sender_name,receiver_address,receiver_name,subject,importance,schedule_date,valid_until_date,retry_count,last_sent,sent_log,send_result,request_createdate,request_updatedate,log_createdate) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}',NOW(),NOW(),NOW());", m.priority, m.user_id, m.sender_address, m.sender_name, m.receiver_address, m.receiver_name, m.subject, m.importance, CommonFunction.DateTimeToString(m.schedule_date), CommonFunction.DateTimeToString(m.valid_until_date), m.retry_count, CommonFunction.DateTimeToString(m.last_sent), m.sent_log, mail_result);
                        id += m.request_id + ",";
                    }
                }

                if (id.Length > 1)
                {
                    id = id.Substring(0, id.Length - 1);
                    sb.AppendFormat("Delete from mail_request where request_id in ({0});", id);
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleServiceDao-->InsertLog1-->" + sb.ToString() + ex.Message);
            }
        }

        public string InsertLog(MailRequest m, string mail_result)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("insert into mail_log (priority,user_id,send_address,sender_name,receiver_address,receiver_name,subject,importance,schedule_date,valid_until_date,retry_count,last_sent,sent_log,send_result,request_createdate,request_updatedate,log_createdate) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}',NOW(),NOW(),NOW());", m.priority, m.user_id, m.sender_address, m.sender_name, m.receiver_address, m.receiver_name, m.subject, m.importance, CommonFunction.DateTimeToString(m.schedule_date), CommonFunction.DateTimeToString(m.valid_until_date), m.retry_count, CommonFunction.DateTimeToString(m.last_sent), m.sent_log, mail_result);
                sb.AppendFormat("Delete from mail_request where request_id in ({0});", m.request_id);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleServiceDao-->InsertLog2-->" + sb.ToString() + ex.Message);
            }
        }

        #endregion


        public DataTable GetParaStore(string paraType)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select  parameterType,parameterCode,parameterName from t_parametersrc where parameterType='{0}' and used=1;", paraType);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetParaStore-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public DataTable GetContentUrlByContentId(int content_id)
        {
            DataTable _dt = new DataTable();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select et.content_url  from edm_content_new ecn LEFT JOIN edm_template et on ecn.template_id=et.template_id where ecn.content_id='{0}';",content_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetContentUrlByContentId-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public DataTable GetContentIDAndUrl(int group_id)
        {
            DataTable _dt = new DataTable();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select edm_template.content_url, edm_content_new.content_id,edm_content_new.template_id,edm_content_new.template_data  from edm_group_new inner join edm_content_new	on edm_group_new.group_id=edm_content_new.group_id inner join edm_send_log on edm_send_log.content_id=edm_content_new.content_id inner join edm_template on edm_content_new.template_id=edm_template.template_id where edm_group_new.group_id='{0}' order by edm_send_log.createdate desc limit 1;", group_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetContentIDAndUrl-->" + sql.ToString() + ex.Message, ex);
            }
        }


        public DataTable AdvanceTemplate()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select template_id from edm_template where template_name='預設'; ");
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->AdvanceTemplate-->" + sql.ToString() + ex.Message, ex);
            }
        }
    }
}
