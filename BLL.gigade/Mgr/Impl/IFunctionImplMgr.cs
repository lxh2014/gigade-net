/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IFunctionImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/1 16:18:14 
 * 
 */

using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IFunctionImplMgr
    {
        int Save(Model.Function function);
        int Update(Model.Function function);
        int Delete(int rowId);

        List<Model.Function> Query(Model.Function query,int controlType=1,int groupId = 1);
        Model.Function QueryFunction(string childCode, string parentCode);

        /// <summary>
        /// 根據funcitonId獲取擁有該功能的user集合
        /// </summary>
        /// <returns>(rowId,callid,groupName)的集合</returns>
        ///add by wwei0216w 2015/7/1
        List<FunctionCustom> GetUserById(Function fun);
        MemoryStream ExcelOut(Function fun, string functionName);
    }
}
