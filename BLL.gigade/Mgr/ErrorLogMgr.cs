using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
namespace BLL.gigade.Mgr
{
    public class ErrorLogMgr:IErrorLogImplMgr
    {
        IErrorLogImplDao _errorDao;
        public ErrorLogMgr(string connectionStr)
        {
            _errorDao = new ErrorLogDao(connectionStr);
        }

        public List<ErrorLog> QueryErrorLog(string startDate, string endDate, int startPage, int endPage, out int totalCount, string level)
        {
            
            try
            {
                //添加 級別 的查詢條件  edit by zhuoqin0830w 2015/02/05
                return _errorDao.QueryErrorLog(startDate, endDate, startPage, endPage, out totalCount, level);
            }
            catch (Exception ex)
            {
                throw new Exception("ErrorLogMgr-->QueryErrorLog-->" + ex.Message, ex);
            }
        }

        #region 獲取 級別 下拉框的值  + GetLevel()  add by zhuoqin0830w 2015/02/04
        /// <summary>
        /// 獲取 級別 下拉框的值
        /// </summary>
        /// <returns></returns>
        public List<ErrorLog> GetLevel()
        {
            try
            {
                return _errorDao.GetLevel();
            }
            catch (Exception ex)
            {
                throw new Exception("ErrorLogMgr-->QueryErrorLog-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}
