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

        public List<EdmContentNew> GetECNList(EdmContentNew query,out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            totalCount = 0;
            try
            {
                sqlCount.AppendFormat("select count(edn.content_id) as countTotal ");
                sql.AppendFormat("select edn.content_id,edn.group_id,`subject`,esl.count,esl.date,edn.sender_id,edn.importance,edn.template_id,edn.template_data  ");
                sqlFrom.AppendFormat("from edm_content_new edn LEFT JOIN  (SELECT content_id,COUNT(content_id) as count,MAX(schedule_date) as date from edm_send_log WHERE test_send=0 GROUP BY content_id)  esl ON edn.content_id=esl.content_id ");
                sqlWhere.AppendFormat(" where 1=1 ");
                DataTable _dt = _access.getDataTable(sqlCount.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
                if (_dt!=null&&_dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0][0]);
                }
                sqlWhere.AppendFormat(" order by edn.content_id desc limit {0},{1}; ",query.Start,query.Limit);
                return _access.getDataTableForObj<EdmContentNew>(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->GetECNList-->" + sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString()+ex.Message,ex);
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
                throw new Exception("EdmContentNewDao-->GetECNList-->" + sql.ToString()+ ex.Message, ex);
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
                throw new Exception("EdmContentNewDao-->GetEdmGroupNewStore-->" + sql.ToString()+ ex.Message, ex);
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
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("insert into edm_content_new(group_id,`subject`,template_id,template_data, ");
                sql.Append("importance,sender_id,content_createdate,content_updatedate,content_create_userid,content_update_userid) ");
                sql.AppendFormat("values('{0}','{1}','{2}','{3}',",query.group_id,query.subject,query.template_id,query.template_data);
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}');", query.importance, query.sender_id, CommonFunction.DateTimeToString(DateTime.Now), CommonFunction.DateTimeToString(DateTime.Now),query.content_create_userid,query.content_update_userid);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->InsertEdmContentNew-->" + sql.ToString()+ex.Message, ex);
            }
        }

        public int UpdateEdmContentNew(EdmContentNew query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update edm_content_new set group_id='{0}',subject='{1}',template_id='{2}',template_data='{3}',importance='{4}',", query.group_id, query.subject, query.template_id, query.template_data, query.importance);
                sql.AppendFormat(" sender_id='{0}',content_updatedate='{1}',content_update_userid='{2}' where content_id='{3}';", query.sender_id, CommonFunction.DateTimeToString(DateTime.Now),query.content_update_userid,query.content_id);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentNewDao-->InsertEdmContentNew-->" + sql.ToString() + ex.Message, ex);
            }
        }
    }
}
