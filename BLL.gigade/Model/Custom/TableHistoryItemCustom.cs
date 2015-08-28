using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class TableHistoryItemCustom 
    {
        public string table_name { get; set; }
        public string functionname { get; set; }
        public string pk_name { get; set; }
        public string pk_value { get; set; }
        public string batchno { get; set; }
        public List<TableHistoryItem> historyItem { set; get; }

        public TableHistoryItemCustom()
        {
            table_name =string.Empty;
            functionname = string.Empty;
            pk_name =string.Empty;
            pk_value =string.Empty;
            batchno =string.Empty;
            historyItem = new List<TableHistoryItem>();
        }
    }
}
