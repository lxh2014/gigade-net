/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ITableHistoryItemImplDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/22 14:05:46 
 * 
 */

using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    interface ITableHistoryItemImplDao
    {
        List<Model.TableHistoryItem> Query(Model.TableHistoryItem tableHistoryItem);
        string Save(Model.TableHistoryItem tableHistoryItem);
        string UpdateType(Model.TableHistoryItem tableHistoryItem);
        string UpdateType(List<Model.TableHistoryItem> tableHistoryItems, int tablehistoryid);
        List<Model.TableHistoryItem> Query4Batch(Model.Query.TableHistoryItemQuery query);

        /// <summary>
        /// 根據條件獲得歷史記錄信息
        /// </summary>
        /// <param name="th">查詢條件</param>
        /// <returns>符合條件的集合</returns>
        //add by wwei0216w 2015/1/22
        DataTable GetHistoryInfoByCondition(TableHistory th);
    }
}
