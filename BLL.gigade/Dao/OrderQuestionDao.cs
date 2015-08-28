using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using System.Data;

namespace BLL.gigade.Dao
{
    public class OrderQuestionDao : IOrderQuestionIDao
    {
        private IDBAccess _access;
        public OrderQuestionDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
        }
        public List<OrderQuestionQuery> GetOrderQuestionList(OrderQuestionQuery o, out int totalCount)
        {

            StringBuilder sel1 = new StringBuilder();
            StringBuilder sbwhere = new StringBuilder();
            try
            {//tp.parameterName as question_type_name,tp1.parameterName as question_status_name,
                sel1.Append(@"SELECT DISTINCT o.question_id,o.order_id,o.question_username,o.question_phone,o.question_email,
o.question_type,o.question_status,
o.question_createdate,o.question_reply,o.question_reply_time,o.question_content,o.question_ipfrom,o.question_file,
t.response_createdate ");
                sbwhere.Append(" FROM order_question as o  left join order_response t ON o.question_id = t.question_id ");
                //sbjoin.Append(" LEFT JOIN (SELECT * from t_parametersrc where parameterType='problem_category') tp ON o.question_type = tp.parameterCode ");
                //sbjoin.Append(" LEFT JOIN (SELECT * from t_parametersrc where parameterType='Question_Status') tp1 ON o.question_status = tp1.parameterCode ");
                sbwhere.Append(" WHERE 1=1 ");
                #region 查詢條件
                if (!string.IsNullOrEmpty(o.selcontent))//所有問題列表
                {
                    switch (o.ddlSel)
                    {
                        case 1:
                            sbwhere.AppendFormat(" AND o.question_id = '{0}' ", o.selcontent);
                            break;
                        case 2:
                            sbwhere.AppendFormat(" AND o.order_id = '{0}' ", o.selcontent);
                            break;
                        case 3:
                            sbwhere.AppendFormat(" AND o.question_username like N'%{0}%' ", o.selcontent);
                            break;
                        case 4:
                            sbwhere.AppendFormat(" AND o.question_email like N'%{0}%' ", o.selcontent);
                            break;
                        default:
                            break;
                    }
                }
                switch (o.ddtSel)
                {
                    case 1:
                        if (o.time_start > 0 && o.time_end > 0)
                        {
                            sbwhere.AppendFormat(" AND o.question_createdate between '{0}' and '{1}' ", o.time_start, o.time_end);
                        }

                        break;
                    case 2:
                        if (o.time_start > 0 && o.time_end > 0)
                        {
                            sbwhere.AppendFormat(" AND t.response_createdate between '{0}' and '{1}' ", o.time_start, o.time_end);
                        }
                        break;
                    default:
                        break;
                }
                if (o.question_type > 0)
                {
                    sbwhere.AppendFormat(" AND o.question_type = '{0}' ", o.question_type);
                }
                if (o.ddlstatus != "-1" && o.ddlstatus != null)
                {
                    sbwhere.AppendFormat(" AND o.question_status = '{0}' ", o.ddlstatus);
                }
                if (o.question_id != 0)
                {
                    sbwhere.AppendFormat(" AND o.question_id = '{0}' ", o.question_id);
                }
                #endregion
                totalCount = 0;

                if (o.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(" select count(o.question_id) as totalCount " + sbwhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sbwhere.AppendFormat(" order by o.question_id desc limit {0},{1} ", o.Start, o.Limit);
                }
                sel1.Append(sbwhere.ToString());
                return _access.getDataTableForObj<OrderQuestionQuery>(sel1.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderQuestionDao-->GetOrderQuestionList-->" + ex.Message + sel1.ToString(), ex);
            }
        }

        /// <summary>
        /// 根據條件查詢問題及回覆列表信息
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <param name="totalCount">查到數據總條件</param>
        /// <returns>問題及回覆列表</returns>
        public DataTable GetList(OrderQuestionQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlColumn = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder("SELECT count(1) as totalCount ");
            StringBuilder sqlTable = new StringBuilder();
            StringBuilder sqlCondition = new StringBuilder();

            sqlColumn.Append("SELECT oq.question_content,ore.response_type,FROM_UNIXTIME(oq.question_createdate) as question_createdate,ore.response_content,FROM_UNIXTIME(ore.response_createdate) as response_createdate,mu.user_username ");
            sqlTable.Append(" FROM order_question oq ");
            sqlTable.Append(" LEFT JOIN order_response ore ON oq.question_id=ore.question_id ");
            sqlTable.Append(" LEFT JOIN manage_user mu ON ore.user_id=mu.user_id ");
            sqlCondition.Append(" WHERE 1=1 ");
            if (query.order_id != 0)
            {
                sqlCondition.AppendFormat(" AND oq.order_id={0} ", query.order_id);
            }
            if (query.question_id != 0)
            {
                sqlCondition.AppendFormat(" AND oq.question_id={0} ", query.question_id);
            }
            totalCount = 0;
            try
            {
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable(sqlCount.ToString() + sqlTable.ToString() + sqlCondition.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sqlCondition.AppendFormat(" ORDER BY oq.question_createdate,ore.response_createdate DESC limit {0},{1};", query.Start, query.Limit);
                }
                sql.Append(sqlColumn).Append(sqlTable).Append(sqlCondition);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BrowseDataDao-->GetBrowseDataList -->" + ex.Message + sql.ToString(), ex);
            }
        }

        public DataTable GetOrderQuestionExcel(OrderQuestionQuery o)
        {

            StringBuilder sel1 = new StringBuilder();

            try
            {//tp1.parameterName as question_status_name,tp.parameterName as question_type_name,
                sel1.Append(@"SELECT  DISTINCT o.question_id,o.order_id,o.question_username,o.question_type,o.question_status,");
                sel1.Append(" o.question_createdate,t.response_createdate,o.question_reply,o.question_reply_time,o.question_content,t.response_content,");
                sel1.Append("   o.question_ipfrom,o.question_file FROM order_question as o ");

                sel1.Append(" left join order_response t ON	o.question_id = t.question_id ");
                //sel1.Append(" LEFT JOIN (SELECT * from t_parametersrc where parameterType='problem_category') tp ON o.question_type = tp.parameterCode ");
                //sel1.Append(" LEFT JOIN (SELECT * from t_parametersrc where parameterType='Question_Status') tp1 ON o.question_status = tp1.parameterCode ");
                sel1.Append(" WHERE 1=1 ");
                if (!string.IsNullOrEmpty(o.selcontent))//所有問題列表
                {
                    switch (o.ddlSel)
                    {
                        case 1:
                            sel1.AppendFormat(" AND o.question_id = '{0}' ", o.selcontent);
                            break;
                        case 2:
                            sel1.AppendFormat(" AND o.order_id = '{0}' ", o.selcontent);
                            break;
                        case 3:
                            sel1.AppendFormat(" AND o.question_username like N'%{0}%' ", o.selcontent);
                            break;
                        case 4:
                            sel1.AppendFormat(" AND o.question_email like N'%{0}%' ", o.selcontent);
                            break;
                        default:
                            break;
                    }
                }
                switch (o.ddtSel)
                {
                    case 1:
                        if (o.time_start > 0 && o.time_end > 0)
                        {
                            sel1.AppendFormat(" AND o.question_createdate between '{0}' and '{1}' ", o.time_start, o.time_end);
                        }

                        break;
                    case 2:
                        if (o.time_start > 0 && o.time_end > 0)
                        {
                            sel1.AppendFormat(" AND o.response_createdate between '{0}' and '{1}' ", o.time_start, o.time_end);
                        }
                        break;
                    default:
                        break;
                }
                if (o.question_type > 0)
                {
                    sel1.AppendFormat(" AND o.question_type = '{0}' ", o.question_type);
                }
                if (o.ddlstatus != "-1")
                {
                    sel1.AppendFormat(" AND o.question_status = '{0}' ", o.ddlstatus);
                }
                sel1.AppendFormat(" order by o.question_id desc ");

                return _access.getDataTable(sel1.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderQuestionDao-->GetOrderQuestionExcel-->" + ex.Message + sel1.ToString(), ex);
            }
        }

        public List<Parametersrc> GetDDL()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("select parameterType,parameterCode,parameterName from t_parametersrc where parameterType='problem_category' and parameterCode<>'0' ;");
                return _access.getDataTableForObj<Parametersrc>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderQuestionDao-->GetDDL-->" + ex.Message + sb, ex);
            }
        }

        /// <summary>
        /// 更新訂單問題狀態
        /// </summary>
        /// <param name="query">更新條件</param>
        /// <returns></returns>
        public void UpdateQuestionStatus(OrderQuestionQuery query)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("UPDATE order_question SET question_status='{0}' WHERE question_id={1}", query.question_status, query.question_id);
            try
            {
                _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderQuestionDao-->UpdateQuestionStatus -->" + ex.Message + sql.ToString(), ex);
            }
        }

        /// <summary>
        /// 新增訂單問題
        /// </summary>
        /// <param name="query">新增的數據</param>
        /// <returns></returns>
        public int InsertOrderQuestion(OrderQuestion query)
        {
            SerialDao serialDao = new SerialDao("");
            StringBuilder sql = new StringBuilder();
            string sql_question_id = serialDao.Update(34);
            int question_id = Convert.ToInt32(_access.getDataTable(sql_question_id).Rows[0][0]);
            sql.AppendFormat("INSERT INTO order_question (question_id,order_id,question_username,question_phone,question_email,question_type,question_reply,");
            sql.AppendFormat("question_reply_time,question_status,question_content,question_ipfrom,question_createdate) ");
            sql.AppendFormat(" VALUES({0},{1},'{2}','{3}'", question_id, query.order_id, query.question_username, query.question_phone);
            sql.AppendFormat(",'{0}',{1},'{2}',{3}", query.question_email, query.question_type, query.question_reply, query.question_reply_time);
            sql.AppendFormat(",{0},'{1}','{2}',{3}", query.question_status, query.question_content, query.question_ipfrom, query.question_createdate);
            sql.Append(")");
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderQuestionDao-->InsertOrderQuestion -->" + ex.Message + sql.ToString(), ex);
            }
        }

        public DataTable GetUserInfo(int rowID)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select users.user_id,c.order_id,c.question_email as user_email,question_username as user_name,question_phone as user_phone");
                sql.Append(" from order_question c");
                sql.Append(" left join users on users.user_email=question_email and users.user_name=question_username");
                sql.AppendFormat(" where c.question_id='{0}'", rowID);

                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderQuestionDao.GetUserInfo-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
