/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IFunctionGroupImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/2 16:36:51 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IFunctionGroupImplMgr
    {
        int Save(Model.FunctionGroup functionGroup);
        bool Save(int[] funIds, int groupId, string kuser);
        int Delete(int RowId);
        List<Model.Function> CallerAuthorityQuery(Model.Query.AuthorityQuery query);
        List<Model.Function> GroupAuthorityQuery(Model.Query.AuthorityQuery query ,int controlType = 1);

        //add by wwei0216w 2015/1/6
        /// <summary>
        /// 根據用戶類型獲得對應的可編輯權限集合
        /// </summary>
        /// <param name="query">查詢類型的條件對象</param>
        /// <returns>符合條件的功能集合</returns>
        List<Model.Function> GetEditFunctionByGroup(Model.Query.AuthorityQuery query);

        //add bywwei 0216w 2015/1/6
        /// <summary>
        /// 更新編輯權限
        /// </summary>
        /// <param name="functionIds">需要更新的權限functionIds集合</param>
        /// <returns>true or false</returns>
        bool UpdateEditFunction(string[] rowId, int groupId);
    }
}
