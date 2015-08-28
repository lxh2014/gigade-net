/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：TableHistoryItem 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/22 11:38:48 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class TableHistoryItem:PageBase
    {
        public int rowid { get; set; }
        public int tablehistoryid { get; set; }
        public string col_name { get; set; }
        public string col_chsname { get; set; }
        public string col_value { get; set; }
        public string old_value { get; set; }
        public int type { get; set; }

        public TableHistoryItem()
        {
            rowid = 0;
            tablehistoryid = 0;
            col_name = string.Empty;
            col_chsname = string.Empty;
            col_value = string.Empty;
            old_value = string.Empty;
            type = 0;
        }
    }
}
