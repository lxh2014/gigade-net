/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：TableHistoryItemQuery 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2014/2/5 14:56:43 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class TableHistoryItemQuery : TableHistoryItem
    {
        public string table_name { get; set; }
        public string pk_name { get; set; }
        public string pk_value { get; set; }
        public string batchno { get; set; }

        public TableHistoryItemQuery()
        {
            table_name = string.Empty;
            pk_name = string.Empty;
            pk_value = string.Empty;
            batchno = string.Empty;
        }
    }
}
