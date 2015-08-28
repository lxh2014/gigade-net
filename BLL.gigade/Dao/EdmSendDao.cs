using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class EdmSendDao
    {
        private IDBAccess _access;
        private string connctionString;
        public EdmSendDao(string connctionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connctionString);
            this.connctionString = connctionString;
        }
        public List<EdmSendQuery> GetStatisticsEdmSend(EdmSendQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            totalCount = 1;//IFNULL(SUM(traveluser_totalppl),9)es.content_id = ' 1207 '
            try
            {
                sql.Append(@"SELECT	es.email_id,es.send_status,es.send_datetime,es.open_first,es.open_last,es.open_total,ee.email_name ");
                sqlWhere.Append(" FROM  edm_email ee,edm_send es WHERE es.email_id = ee.email_id  ");
                
                if (!string.IsNullOrEmpty(query.email_name)) //email_name
                {
                    sqlWhere.AppendFormat("  and ee.email_name like '%{0}%'", query.email_name);
                }
                if (query.date == 1) //寄信時間
                {
                    if (!string.IsNullOrEmpty(query.start_time)) //email_name
                    {
                        sqlWhere.AppendFormat("  and send_datetime >='{0}'", CommonFunction.GetPHPTime(query.start_time));
                    }
                    if (!string.IsNullOrEmpty(query.end_time)) //email_name
                    {
                        sqlWhere.AppendFormat(" and send_datetime <='{0}'", CommonFunction.GetPHPTime(query.end_time));
                    }
                }
                if (query.date == 2) //首次開信時間
                {
                    if (!string.IsNullOrEmpty(query.start_time)) //email_name
                    {
                        sqlWhere.AppendFormat("  and open_first >='{0}'", CommonFunction.GetPHPTime(query.start_time));
                    }
                    if (!string.IsNullOrEmpty(query.end_time)) //email_name
                    {
                        sqlWhere.AppendFormat(" and open_first <='{0}'", CommonFunction.GetPHPTime(query.end_time));
                    }
                }
                if (query.date == 3) //最近開信時間
                {
                    if (!string.IsNullOrEmpty(query.start_time)) //email_name
                    {
                        sqlWhere.AppendFormat("  and open_last >='{0}'", CommonFunction.GetPHPTime(query.start_time));
                    }
                    if (!string.IsNullOrEmpty(query.end_time)) //email_name
                    {
                        sqlWhere.AppendFormat(" and open_last <='{0}'", CommonFunction.GetPHPTime(query.end_time));
                    }
                }

                sqlWhere.AppendFormat(" and es.content_id = '{0}' ORDER BY open_total DESC, es.email_id ASC ", query.content_id);
                sqlCount.Append("select count(es.email_id) totalCount "); //
                if (query.IsPage)
                {                 
                    DataTable dt = _access.getDataTable(sqlCount.ToString()+sqlWhere.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0]["totalCount"]);
                    }
                    sqlWhere.AppendFormat(" limit {0},{1} ;", query.Start, query.Limit);
                }
                return _access.getDataTableForObj<EdmSendQuery>(sql.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmSendDao-->GetStatisticsEdmSend-->" + sql.ToString() + ex.Message);
            }
        }
        public List<EdmListQuery> GetStatisticsEdmList(EdmListQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            totalCount = 1;//IFNULL(SUM(traveluser_totalppl),9)es.content_id = ' 1207 '
            try
            {
                sql.Append(@"SELECT statistics_id,total_click ,total_person ");
                sqlWhere.AppendFormat(" from edm_daily_statistics WHERE 1=1 and content_id = '{0}' ", query.content_id);
                sqlCount.Append("select count(statistics_id) totalCount "); //
                if (query.IsPage)
                {
                    System.Data.DataTable dt = _access.getDataTable(sqlCount.ToString() + sqlWhere.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0]["totalCount"]);
                    }
                    sqlWhere.AppendFormat(" limit {0},{1} ;", query.Start, query.Limit);
                }
                return _access.getDataTableForObj<EdmListQuery>(sql.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmSendDao-->GetStatisticsEdmList-->" + sql.ToString() + ex.Message);
            }
        }
        public DataTable EdmSendExportCSV(EdmSendQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(@"SELECT	es.email_id,es.send_status,es.send_datetime,es.open_first,es.open_last,es.open_total,ee.email_name,ee.email_address");
                sql.Append(" FROM  edm_email ee,edm_send es WHERE es.email_id = ee.email_id ");
                sql.AppendFormat(" and es.content_id = '{0}' ORDER BY open_total DESC, es.email_id ASC ", query.content_id);

                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmSendDao->EdmSendExportCSV->" + sql.ToString() + ex.Message, ex);
            }
        }
        public int GetMaxOpen(EdmSendQuery query)
        {
            int result = 0;
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT max(open_total) open_total FROM  edm_send es WHERE content_id = '{0}' ", query.content_id);
                DataTable _dt = _access.getDataTable(sql.ToString());
                if(_dt.Rows.Count>0)
                {
                    result = Convert.ToInt32(_dt.Rows[0]["open_total"].ToString());
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("EdmSendDao-->GetMaxOpen-->" + sql.ToString() + ex.Message,ex);
            }
        }
        public int GetMaxClick(EdmListQuery query,out int sum_total_click, out int sum_total_person)
        {
            int result = 0;
            sum_total_click = 0;
            sum_total_person = 0;
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT MAX(total_click) total_click ,SUM(total_click) sum_total_click,SUM(total_person) sum_total_person  FROM	edm_daily_statistics  WHERE content_id = '{0}' ", query.content_id);
                DataTable _dt = _access.getDataTable(sql.ToString());
                if (_dt.Rows.Count > 0)
                {
                    sum_total_click = Convert.ToInt32(_dt.Rows[0]["sum_total_click"].ToString());
                    sum_total_person = Convert.ToInt32(_dt.Rows[0]["sum_total_person"].ToString());
                    result = Convert.ToInt32(_dt.Rows[0]["total_click"].ToString());
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("EdmSendDao-->GetMaxOpen-->" + sql.ToString() + ex.Message, ex);
            }
        }
        public EdmSendQuery EdmSendLoad(EdmSendQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT	group_id,
					content_status,
					content_email_id,
					content_start,
					content_end,
					content_range,
					content_single_count,
					content_click,
					content_person,
					content_send_success,
					content_send_failed,
					content_from_name,
					content_from_email,
					content_reply_email,
					content_priority,
					content_title,
					content_body,
					content_createdate,
					content_updatedate
				FROM edm_content 
				WHERE content_id = '{0}' ", query.content_id);

                return _access.getSinggleObj<EdmSendQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmSendDao-->EdmSendLoad-->" + sql.ToString() + ex.Message, ex);
            }
        }
        public DataTable GetSendRecordList(EdmSendQuery query,out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            totalCount = 0;
            try
            {
                sql.AppendFormat(@"SELECT ee.send_status,ec.content_id,ec.content_title,ee.open_total,e.email_address,e.email_name,FROM_UNIXTIME(ee.send_datetime) as sendtime,FROM_UNIXTIME(ee.open_first) as firsttime ,FROM_UNIXTIME(ee.open_last) as lasttime 
");
                sqlWhere.AppendFormat(@" FROM edm_send ee LEFT JOIN edm_content ec ON ee.content_id = ec.content_id LEFT JOIN edm_email e on ee.email_id=e.email_id WHERE ee.email_id = {0} ", query.email_id);
                if (!string.IsNullOrEmpty(query.content_title)) //email_name
                {
                    sqlWhere.AppendFormat("  and ec.content_title like '%{0}%'", query.content_title);
                }
                if (query.date == 1) //寄信時間
                {
                    if (!string.IsNullOrEmpty(query.start_time)) //email_name
                    {
                        sqlWhere.AppendFormat("  and ee.send_datetime >='{0}'", CommonFunction.GetPHPTime(query.start_time));
                    }
                    if (!string.IsNullOrEmpty(query.end_time)) //email_name
                    {
                        sqlWhere.AppendFormat(" and ee.send_datetime <='{0}'", CommonFunction.GetPHPTime(query.end_time));
                    }
                }
                if (query.date == 2) //首次開信時間
                {
                    if (!string.IsNullOrEmpty(query.start_time)) //email_name
                    {
                        sqlWhere.AppendFormat("  and ee.open_first >='{0}'", CommonFunction.GetPHPTime(query.start_time));
                    }
                    if (!string.IsNullOrEmpty(query.end_time)) //email_name
                    {
                        sqlWhere.AppendFormat(" and ee.open_first <='{0}'", CommonFunction.GetPHPTime(query.end_time));
                    }
                }
                if (query.date == 3) //最近開信時間
                {
                    if (!string.IsNullOrEmpty(query.start_time)) //email_name
                    {
                        sqlWhere.AppendFormat("  and ee.open_last >='{0}'", CommonFunction.GetPHPTime(query.start_time));
                    }
                    if (!string.IsNullOrEmpty(query.end_time)) //email_name
                    {
                        sqlWhere.AppendFormat(" and ee.open_last <='{0}'", CommonFunction.GetPHPTime(query.end_time));
                    }
                }
                sqlWhere.AppendFormat("ORDER BY ec.content_id DESC");
                sqlCount.Append(@"SELECT count(ec.content_id) totalCount ");
                if (query.IsPage)
                {
                    DataTable dt = _access.getDataTable(sqlCount.ToString()+sqlWhere.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0]["totalCount"]);
                    }
                    sqlWhere.AppendFormat(" limit {0},{1} ;", query.Start, query.Limit);
                }
                return _access.getDataTable(sql.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("EdmSendDao-->GetSendRecordList-->" + sql.ToString() + sqlWhere.ToString() + ex.Message, ex);
            }
        }
    }
}
