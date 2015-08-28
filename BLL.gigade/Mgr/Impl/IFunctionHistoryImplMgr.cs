using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IFunctionHistoryImplMgr
    {
        /// <summary>
        /// 保存操作的歷史記錄
        /// </summary>
        /// <param name="fh">一個FunctionHistory對象</param>
        /// <returns>true or false</returns>
        bool Save(FunctionHistory fh);

        /// <summary>
        /// 查詢相關功能的操作信息
        /// </summary>
        /// <param name="function_id">功能id</param>
        /// <returns>FunctionHistory集合</returns>
        List<FunctionHistoryCustom> Query(int function_id, int start, int limit, string condition, DateTime timeStart, DateTime timeEnd, out int total);
    }
}
