using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class ContactUsQuestionMgr : IContactUsQuestionImplMgr
    {
        private Dao.Impl.IContactUsQuestionImplDao _contQuest;
        private Dao.Impl.IParametersrcImplDao _parameterDao;
        private string connStr;
        public ContactUsQuestionMgr(string connectionString)
        {
            _contQuest = new Dao.ContactUsQuestionDao(connectionString);
            connStr = connectionString;
        }


        public System.Data.DataTable GetContactUsQuestionList(Model.Query.ContactUsQuestionQuery query, out int totalCount)
        {
            try
            {
                DataTable _list = _contQuest.GetContactUsQuestionList(query, out totalCount);
                if (_list.Rows.Count > 0)
                {
                    _list.Columns.Add("question_status_name");
                    _list.Columns.Add("question_type_name");
                    _list.Columns.Add("question_problem_name");
                    _parameterDao = new Dao.ParametersrcDao(connStr);
                    List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("Question_Status", "Question_Type", "problem_category");
                    foreach (DataRow dr in _list.Rows)
                    {
                        //求參數
                        var alist = parameterList.Find(m => m.ParameterType == "Question_Status" && m.ParameterCode == dr["question_status"].ToString());
                        var blist = parameterList.Find(m => m.ParameterType == "Question_Type" && m.ParameterCode == dr["question_type"].ToString());
                        var clist = parameterList.Find(m => m.ParameterType == "problem_category" && m.ParameterCode == dr["question_problem"].ToString());
                        if (alist != null)
                        {
                            dr["question_status_name"] = alist.parameterName;
                        }
                        if (blist != null)
                        {
                            dr["question_type_name"] = blist.parameterName;
                        }
                        if (clist != null)
                        {
                            dr["question_problem_name"] = clist.parameterName;
                        }
                        //加密機敏資料
                        if (query.isSecret)
                        {
                            if (!string.IsNullOrEmpty(dr["question_username"].ToString()))
                            {
                                dr["question_username"] = dr["question_username"].ToString().Substring(0, 1) + "**";
                            }
                            dr["question_email"] = dr["question_email"].ToString().Split('@')[0] + "@***";
                            if (dr["question_phone"].ToString().Length > 3)
                            {
                                dr["question_phone"] = dr["question_phone"].ToString().Substring(0, 3) + "***";
                            }
                            else
                            {
                                dr["question_phone"] = dr["question_phone"].ToString() + "***";
                            }
                        }
                    }
                }

                return _list;
            }
            catch (Exception ex)
            {
                throw new Exception("ContactUsQuestionMgr-->GetContactUsQuestionList-->" + ex.Message, ex);
            }
        }


        public System.Data.DataTable GetContactUsQuestionExcelList(Model.Query.ContactUsQuestionQuery query)
        {
            try
            {
                DataTable _list = _contQuest.GetContactUsQuestionExcelList(query);
                if (_list.Rows.Count > 0)
                {
                    _list.Columns.Add("question_status_name");
                    _list.Columns.Add("question_type_name");
                    _list.Columns.Add("question_problem_name");
                    _parameterDao = new Dao.ParametersrcDao(connStr);
                    List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("Question_Status", "Question_Type", "problem_category");
                    foreach (DataRow dr in _list.Rows)
                    {
                        //求參數
                        var alist = parameterList.Find(m => m.ParameterType == "Question_Status" && m.ParameterCode == dr["question_status"].ToString());
                        var blist = parameterList.Find(m => m.ParameterType == "Question_Type" && m.ParameterCode == dr["question_type"].ToString());
                        var clist = parameterList.Find(m => m.ParameterType == "problem_category" && m.ParameterCode == dr["question_problem"].ToString());
                        if (alist != null)
                        {
                            dr["question_status_name"] = alist.parameterName;
                        }
                        if (blist != null)
                        {
                            dr["question_type_name"] = blist.parameterName;
                        }
                        if (clist != null)
                        {
                            dr["question_problem_name"] = clist.parameterName;
                        }
                    }
                }
                return _list;
            }
            catch (Exception ex)
            {
                throw new Exception("ContactUsQuestionMgr-->GetContactUsQuestionExcelList-->" + ex.Message, ex);
            }
        }
        public DataTable GetUserInfo(int rowID)
        {
            try
            {
                return _contQuest.GetUserInfo(rowID);
            }
            catch (Exception ex)
            {
                throw new Exception("ContactUsQuestionMgr-->GetUserInfo-->" + ex.Message, ex);
            }
        }

        public int Save(ContactUsQuestion query)
        {
            try
            {
                return _contQuest.Save(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ContactUsQuestionMgr-->Save-->" + ex.Message, ex);
            }
        }


        //public int Update(ContactUsQuestion query)
        //{
        //    try
        //    {
        //        return _contQuest.Update(query);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("ContactUsQuestionMgr-->Update-->" + ex.Message, ex);
        //    }
        //}

        public int GetMaxQuestionId()
        {
            try
            {
                return _contQuest.GetMaxQuestionId();
            }
            catch (Exception ex)
            {
                throw new Exception("ContactUsQuestionMgr-->GetMaxQuestionId-->" + ex.Message, ex);
            }
        }


        public string UpdateSql(ContactUsQuestion query)
        {
            try
            {
                return _contQuest.UpdateSql(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ContactUsQuestionMgr-->UpdateSql-->" + ex.Message, ex);
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
            try
            {
                return _contQuest.UpdateActive(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("ContactUsQuestionMgr-->UpdateActive-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}
