using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface ILogInLogeImplDao
    {
        /// <summary>
        /// 系統登錄記錄查詢
        /// </summary>
        /// <param name="logInLogeQuery">登錄記錄實體</param>
        /// <param name="totalCount">查詢結果總條數</param>
        /// <returns>記錄列表</returns>
        List<Model.Query.LogInLogeQuery> QueryList(LogInLogeQuery logInLogeQuery, out int totalCount);
    }
}
