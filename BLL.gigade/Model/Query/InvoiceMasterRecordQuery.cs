using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class InvoiceMasterRecordQuery : InvoiceMasterRecord
    {
        public DateTime open_date { set; get; }//開立時間
        public string sqlwhere { set; get; }//開立發票列表查詢條件
        public InvoiceMasterRecordQuery()
        {
            open_date = DateTime.MinValue;
            sqlwhere = string.Empty;
        }
    }
}
