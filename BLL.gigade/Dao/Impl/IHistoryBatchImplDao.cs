/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IHistoryBatchImplDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2014/2/6 9:39:44 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    interface IHistoryBatchImplDao
    {
        string Save(Model.HistoryBatch historyBatch);
        bool BatchExists(Model.HistoryBatch historyBatch);
        List<Model.HistoryBatch> QueryToday(int itemType);
        Model.HistoryBatch Query(Model.HistoryBatch historyBatch);
    }
}
