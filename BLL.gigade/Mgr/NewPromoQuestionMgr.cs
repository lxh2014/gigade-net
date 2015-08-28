using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Mgr
{
    public class NewPromoQuestionMgr : INewPromoQuestionImplMgr
    {
        private INewPromoQuestionImplDao _INewsPromoQuestionDao;
        private IDBAccess _access;
        private string connStr;
        public NewPromoQuestionMgr(string connectionString)
        {
            _INewsPromoQuestionDao = new NewPromoQuestionDao(connectionString);
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }

        public DataTable GetPromoQuestionList(NewPromoQuestionQuery query, out int totalCount)
        {
            try
            {
                return _INewsPromoQuestionDao.GetPromoQuestionList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoQuestionMgr-->GetPromoQuestionList" + ex.Message, ex);
            }
        }

        public List<NewPromoQuestionQuery> GetPromoQuestionList(NewPromoQuestionQuery query)
        {
            try
            {
                return _INewsPromoQuestionDao.GetPromoQuestionList(query);
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoQuestionMgr-->GetPromoQuestionList" + ex.Message, ex);
            }
        }



        public int InsertNewPromoQuestion(NewPromoQuestionQuery query)
        {
            try
            {
                int row_id = 0;
                DataTable dt = _access.getDataTable(_INewsPromoQuestionDao.GetMaxRowId());
                if (dt.Rows.Count > 0)
                {
                    row_id = Convert.ToInt32(dt.Rows[0]["row_id"].ToString());
                }
                query.row_id = row_id + 1;
                query.event_id = BLL.gigade.Common.CommonFunction.GetEventId("F1", query.row_id.ToString());
                return _access.execCommand(_INewsPromoQuestionDao.InsertNewPromoQuestion(query));
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoQuestionMgr-->InsertNewPromoQuestion" + ex.Message, ex);
            }
        }
        public int UpdateNewPromoQuestion(NewPromoQuestionQuery query)
        {
            try
            {
                return _access.execCommand(_INewsPromoQuestionDao.UpdateNewPromoQuestion(query));
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoQuestionMgr-->UpdateNewPromoQuestion" + ex.Message, ex);
            }
        }
        public int DeleteQuestion(string row_id)
        {
            try
            {
                return _INewsPromoQuestionDao.DeleteNewPromoQuestion(row_id);
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoQuestionMgr-->DeleteQuestion" + ex.Message, ex);
            }
        }
        public int UpdateActive(NewPromoQuestionQuery query)
        {
            try
            {
                return _access.execCommand(_INewsPromoQuestionDao.UpdateActive(query));
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoQuestionMgr-->UpdateActive" + ex.Message, ex);
            }
        }
    }
}
