using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class OrderQuestionMgr : IOrderQuestionIMgr
    {
        private IOrderQuestionIDao _oqdao;
        private string connStr;
        public OrderQuestionMgr(string connectionString)
        {
            _oqdao = new OrderQuestionDao(connectionString);
            connStr = connectionString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<OrderQuestionQuery> GetOrderQuestionList(OrderQuestionQuery query, out int totalCount)
        {
            try
            {
                List<OrderQuestionQuery> _list = _oqdao.GetOrderQuestionList(query, out totalCount);
                if (_list.Count > 0)
                {
                    ParametersrcDao _parameterDao = new Dao.ParametersrcDao(connStr);
                    List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("Question_Status", "problem_category");
                    foreach (OrderQuestionQuery dr in _list)
                    {
                        //求參數
                        var alist = parameterList.Find(m => m.ParameterType == "Question_Status" && m.ParameterCode == dr.question_status.ToString());
                        var clist = parameterList.Find(m => m.ParameterType == "problem_category" && m.ParameterCode == dr.question_type.ToString());
                        if (alist != null)
                        {
                            dr.question_status_name = alist.parameterName;
                        }
                        if (clist != null)
                        {
                            dr.question_type_name = clist.parameterName;
                        }
                    }
                }
                return _list;

            }
            catch (Exception ex)
            {
                throw new Exception("OrderQuestionMgr-->GetOrderQuestionList-->" + ex.Message, ex);
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
            try
            {
                return _oqdao.GetList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderQuestionMgr-->GetList-->" + ex.Message, ex);
            }
        }

        public DataTable GetOrderQuestionExcel(OrderQuestionQuery o)
        {
            try
            {
                DataTable _list = _oqdao.GetOrderQuestionExcel(o);
                if (_list.Rows.Count > 0)
                {
                    ParametersrcDao _parameterDao = new Dao.ParametersrcDao(connStr);
                    List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("Question_Status", "problem_category");
                    _list.Columns.Add("question_status_name");
                    _list.Columns.Add("question_type_name");
                    foreach (DataRow dr in _list.Rows)
                    {
                        //求參數
                        var alist = parameterList.Find(m => m.ParameterType == "Question_Status" && m.ParameterCode == dr["question_status"].ToString());
                        var clist = parameterList.Find(m => m.ParameterType == "problem_category" && m.ParameterCode == dr["question_type"].ToString());
                        if (alist != null)
                        {
                            dr["question_status_name"] = alist.parameterName;
                        }
                        if (clist != null)
                        {
                            dr["question_type_name"] = clist.parameterName;
                        }
                    }
                }
                return _list;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderQuestionMgr-->GetOrderQuestionExcel-->" + ex.Message, ex);
            }
        }
        public List<Parametersrc> GetDDL()
        {
            try
            {
                return _oqdao.GetDDL();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderQuestionMgr-->GetDDL-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 更新訂單問題狀態
        /// </summary>
        /// <param name="query">更新條件</param>
        public void UpdateQuestionStatus(OrderQuestionQuery query)
        {
            try
            {
                _oqdao.UpdateQuestionStatus(query);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderQuestionMgr-->UpdateQuestionStatus-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 新增訂單問題
        /// </summary>
        /// <param name="query">新增的數據</param>
        /// <returns></returns>
        public int InsertOrderQuestion(OrderQuestion query)
        {
            try
            {
                return _oqdao.InsertOrderQuestion(query);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderQuestionMgr-->InsertOrderQuestion-->" + ex.Message, ex);
            }
        }

        public DataTable GetUserInfo(int rowID)
        {
            try
            {
                return _oqdao.GetUserInfo(rowID);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderQuestionMgr-->GetUserInfo-->" + ex.Message, ex);
            }
        }
    }
}