/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ITablehistoryItemImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/22 15:56:15 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Mgr.Impl
{
    public interface ITableHistoryItemImplMgr
    {
        List<Model.TableHistoryItem> Query(Model.TableHistoryItem tableHistoryItem);
        string Save(Model.TableHistoryItem tableHistoryItem);
        string UpdateType(Model.TableHistoryItem tableHistoryItem);
        string UpdateType(List<Model.TableHistoryItem> tableHistoryItems, int tablehistoryid);
        List<Model.TableHistoryItem> Query4Batch(Model.Query.TableHistoryItemQuery query);

        /// <summary>
        /// 根據批次號獲得該彼此相關記錄
        /// </summary>
        /// <param name="th">條件</param>
        /// <returns>IEnumerable<IGrouping<string,TableHistoryItemCustom>></returns>
        //add by wwei0216w 2015/1/22
        List<TableHistoryItemCustom> GetHistoryInfoByConditon(TableHistory th);
    }
}
