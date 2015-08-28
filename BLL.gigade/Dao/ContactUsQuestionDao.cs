using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class ContactUsQuestionDao : IContactUsQuestionImplDao
    {
        private IDBAccess _access;
        private string connStr;
        public ContactUsQuestionDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }
        #region 查詢
        public System.Data.DataTable GetContactUsQuestionList(Model.Query.ContactUsQuestionQuery query, out int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlconditon = new StringBuilder();
            try
            {
                #region 查詢條件
                if (query.question_id != 0)
                {
                    sqlconditon.AppendFormat(" and cution.question_id='{0}'", query.question_id);
                }
                if (!string.IsNullOrEmpty(query.searchcontent.Trim()))//搜索條件
                {
                    switch (query.search_type)//搜索內容
                    {
                        case 1:
                            sqlconditon.AppendFormat(" and cution.question_username LIKE N'%{0}%' ", query.searchcontent);
                            break;
                        case 2:
                            sqlconditon.AppendFormat(" and cution.question_email LIKE N'%{0}%' ", query.searchcontent);
                            break;
                        case 3:
                            sqlconditon.AppendFormat(" and cution.question_phone LIKE N'%{0}%' ", query.searchcontent);
                            break;
                        default:
                            sqlconditon.AppendFormat("");
                            break;
                    }
                }

                if (query.datestart != DateTime.MinValue && query.dateend != DateTime.MinValue)
                {
                    sqlconditon.AppendFormat(" and cution.question_createdate between '{0}' and '{1}' ", Common.CommonFunction.GetPHPTime(Common.CommonFunction.DateTimeToString(query.datestart)), Common.CommonFunction.GetPHPTime(Common.CommonFunction.DateTimeToString(query.dateend)));
                }

                switch (query.question_type)//搜索內容
                {
                    case 1:
                        sqlconditon.Append(" and cution.question_type =1 ");
                        break;
                    case 2:
                        sqlconditon.Append(" and cution.question_type=2 ");
                        break;
                    case 3:
                        sqlconditon.Append(" and cution.question_type=3 ");
                        break;
                    case 4:
                        sqlconditon.Append(" and cution.question_type=4 ");
                        break;
                }
                if (query.question_status == 3)
                {
                    sqlconditon.AppendFormat(" and cution.question_status=0 ");//待回復
                }
                else if (query.question_status == 4)
                {
                    sqlconditon.AppendFormat(" and cution.question_status=1 ");//已回復
                }
                else if (query.question_status == 2)
                {
                    sqlconditon.AppendFormat(" and cution.question_status=2 ");//已處理
                }

                #endregion
                sql.Append(@"SELECT question_id,question_type,question_company,question_username,users.user_id,question_email,question_phone,question_status,question_content,question_ipfrom,question_problem,
question_reply,question_reply_time, concat(DATE(FROM_UNIXTIME(question_createdate)),' ',TIME(FROM_UNIXTIME(question_createdate))) as question_createdate");

                sqlFrom.Append(@" FROM	contact_us_question cution
LEFT JOIN  users on users.user_email=cution.question_email and users.user_name=cution.question_username");
                //LEFT JOIN (SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='Question_Status')as qstp on qstp.parameterCode=cution.question_status
                //LEFT JOIN (SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='Question_Type')as qttp on qttp.parameterCode=cution.question_type
                //LEFT JOIN (SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType='problem_category') as pctp on pctp.parameterCode=cution.question_problem where 1=1 ");
                if (sqlconditon.Length != 0)
                {
                    sqlFrom.Append(" WHERE " + sqlconditon.ToString().TrimStart().Remove(0, 3));
                }
                sqlFrom.Append(" ORDER BY question_id DESC ");
                totalCount = 0;

                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(" SELECT count(question_id) as totalCount " + sqlFrom.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }

                    sqlFrom.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                sql.Append(sqlFrom.ToString());
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ContactUsQuestionDao.GetContactUsQuestionList-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public System.Data.DataTable GetContactUsQuestionExcelList(Model.Query.ContactUsQuestionQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlconditon = new StringBuilder();
            try
            {
                #region 條件
                if (!string.IsNullOrEmpty(query.searchcontent.Trim()))//搜索條件
                {
                    switch (query.search_type)//搜索內容
                    {
                        case 1:
                            sqlconditon.AppendFormat(" and cution.question_username LIKE '%{0}%' ", query.searchcontent);
                            break;
                        case 2:
                            sqlconditon.AppendFormat(" and cution.question_email LIKE '%{0}%' ", query.searchcontent);
                            break;
                        case 3:
                            sqlconditon.AppendFormat(" and cution.question_phone LIKE '%{0}%' ", query.searchcontent);
                            break;
                        default:
                            sqlconditon.AppendFormat(" ");
                            break;
                    }
                }
                if (query.datestart != DateTime.MinValue && query.dateend != DateTime.MinValue)
                {
                    sqlconditon.AppendFormat(" and cution.question_createdate between '{0}' and '{1}' ", Common.CommonFunction.GetPHPTime(Common.CommonFunction.DateTimeToString(query.datestart)), Common.CommonFunction.GetPHPTime(Common.CommonFunction.DateTimeToString(query.dateend)));
                }


                switch (query.question_type)//搜索內容
                {
                    case 1:
                        sqlconditon.AppendFormat(" and cution.question_type =1 ");
                        break;
                    case 2:
                        sqlconditon.AppendFormat(" and cution.question_type=2 ");
                        break;
                    case 3:
                        sqlconditon.AppendFormat(" and cution.question_type=3 ");
                        break;
                    case 4:
                        sqlconditon.AppendFormat(" and cution.question_type=4 ");
                        break;
                }

                if (query.question_status == 3)
                {
                    sqlconditon.AppendFormat(" and cution.question_status=0 ");
                }
                else if (query.question_status == 4)
                {
                    sqlconditon.AppendFormat(" and cution.question_status=1 ");
                }
                else if (query.question_status == 2)
                {
                    sqlconditon.AppendFormat(" and cution.question_status=2 ");
                }

                #endregion


                sql.AppendFormat(@"SELECT  cution.question_id,cution.question_type,cution.question_company,cution.question_status,cution.question_username,cution.question_content,conse.response_content,
                                    cution.question_problem, question_reply, question_reply_time,
                                    concat(DATE(FROM_UNIXTIME(question_createdate)),' ',TIME(FROM_UNIXTIME(question_createdate))) as question_createdate,
                                    CONCAT(DATE(FROM_UNIXTIME(response_createdate)),' ',TIME(FROM_UNIXTIME(response_createdate))) as response_createdate		
                                    FROM	contact_us_question cution
                                    LEFT JOIN contact_us_response conse on conse.question_id=cution.question_id 
                                     ");
                //  LEFT JOIN (SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc
                // WHERE parameterType='Question_Status')as qstp on qstp.parameterCode=cution.question_status
                //LEFT JOIN (SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc 
                //WHERE parameterType='Question_Type')as qttp on qttp.parameterCode=cution.question_type
                //LEFT JOIN (SELECT rowid,parameterType,parameterCode,parameterName,remark FROM t_parametersrc
                // WHERE parameterType='problem_category') as pctp on pctp.parameterCode=cution.question_problem 
                if (sqlconditon.Length != 0)
                {
                    sql.Append(" WHERE " + sqlconditon.ToString().TrimStart().Remove(0, 3));
                }
                sql.Append(" ORDER BY question_id DESC ");
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ContactUsQuestionDao.GetContactUsQuestionExcelList-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public DataTable GetUserInfo(int rowID)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select users.user_id,c.question_email as user_email,question_username as user_name,question_phone as user_phone");
                sql.Append(" from contact_us_question c");
                sql.Append(" left join users on users.user_email=question_email and users.user_name=question_username");
                sql.AppendFormat(" where c.question_id='{0}'", rowID);

                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ContactUsQuestionDao.GetUserInfo-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 新增客服問題保存
        /// <summary>
        ///新增客服問題保存
        /// </summary>
        /// <param name="query">要保存的信息</param>
        /// <returns></returns>
        public int Save(ContactUsQuestion query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" insert into contact_us_question(question_id,question_language,question_type,question_company,question_username,
                                    question_email,question_phone,question_status,question_content,question_createdate,question_ipfrom,question_problem,question_reply,question_reply_time)values
                                    ('{0}','{1}','{2}','{3}','{4}',
                                    '{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}')",
                         query.question_id, query.question_language, query.question_type, query.question_company, query.question_username,
                         query.question_email, query.question_phone, query.question_status, query.question_content, query.question_createdate, query.question_ipfrom, query.question_problem, query.question_reply, query.question_reply_time);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ContactUsQuestionDao.Save-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion


        //public int Update(ContactUsQuestion query)
        //{
        //    StringBuilder sql = new StringBuilder();
        //    try
        //    {
        //        sql.AppendFormat(@" ");
        //        return _access.execCommand(sql.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("ContactUsQuestionDao.Update-->" + ex.Message + sql.ToString(), ex);
        //    }
        //}

        public int GetMaxQuestionId()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" SELECT max(question_id) FROM contact_us_question ");
                DataTable _dt = _access.getDataTable(sql.ToString());
                return Convert.ToInt32(_dt.Rows[0][0]) + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("ContactUsQuestionDao.GetMaxQuestionId-->" + ex.Message + sql.ToString(), ex);
            }
        }


        public string UpdateSql(ContactUsQuestion query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"UPDATE contact_us_question SET question_status ='{0}' WHERE	question_id ='{1}';", query.question_status, query.question_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ContactUsQuestionDao.UpdateSql-->" + ex.Message + sql.ToString(), ex);
            }
        }

        #region 聯絡客服列表狀態更改
        /// <summary>
        /// 聯絡客服列表狀態更改
        /// </summary>
        /// <param name="sql">要執行的sql語句</param>
        /// <returns></returns>
        public int UpdateActive(string sql)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(sql);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ContactUsQuestionDao.UpdateActive-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
    }
}
