using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using DBAccess;
using BLL.gigade.Dao.Impl;
using System.Data;
using BLL.gigade.Common;
namespace BLL.gigade.Dao
{
    public class ErrorLogDao:IErrorLogImplDao
    {
        IDBAccess _access;
        public ErrorLogDao(string connectionStr)
        {
            _access = DBAccess.DBFactory.getDBAccess(DBType.MySql,connectionStr);
        }
        public List<ErrorLog> QueryErrorLog(string startDate, string endDate, int startPage, int endPage, out int totalCount, string level)
        {
            StringBuilder stb = new StringBuilder();
            string sql = "",tempStr = "1=1";
            if (string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate) && string.IsNullOrEmpty(level))
            {
                tempStr = "1=1";
            }
            else
            {
                if (!string.IsNullOrEmpty(startDate))
                {
                    tempStr += string.Format(" and log_date >= '{0}'",CommonFunction.DateTimeToString(Convert.ToDateTime(startDate)));
                }

                if (!string.IsNullOrEmpty(endDate))
                {
                    tempStr += string.Format(" and log_date <= '{0}'", CommonFunction.DateTimeToString(Convert.ToDateTime(endDate)));
                }
                //添加 級別 的查詢條件  edit by zhuoqin0830w 2015/02/05
                if (!string.IsNullOrEmpty(level))
                {
                    tempStr += string.Format(" AND level = '{0}'", level);
                }
            }

            stb.Append(string.Format("select * from t_errorlog where {0} order by log_date desc limit {1},{2}", tempStr, startPage, endPage));

            sql = string.Format("select * from t_errorlog where {0}",tempStr);

            totalCount = _access.getDataTable(sql).Rows.Count;

            return _access.getDataTableForObj<ErrorLog>(stb.ToString());
        }

        #region 獲取 級別 下拉框的值  + GetLevel()  add by zhuoqin0830w 2015/02/04
        /// <summary>
        /// 獲取 級別 下拉框的值
        /// </summary>
        /// <returns></returns>
        public List<ErrorLog> GetLevel()
        {
            return _access.getDataTableForObj<ErrorLog>("SELECT level FROM t_errorlog GROUP BY level");
        }
        #endregion
    }
}
