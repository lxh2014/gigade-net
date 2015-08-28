using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    public interface IErrorLogImplDao
    {
        /// <summary>
        /// 查詢錯誤日志信息
        /// </summary>
        /// <param name="startDate">開始日期</param>
        /// <param name="endDate">結束日期</param>
        /// <param name="startPage">起始頁</param>
        /// <param name="endPage">結束頁</param>
        /// <param name="totalCount">記錄總數</param>
        /// <param name="level">級別</param>
        /// <returns></returns>
        List<ErrorLog> QueryErrorLog(string startDate, string endDate, int startPage, int endPage, out int totalCount,string level);

        #region 獲取 級別 下拉框的值  + GetLevel()  add by zhuoqin0830w 2015/02/04
        List<ErrorLog> GetLevel();
        #endregion
    }
}
