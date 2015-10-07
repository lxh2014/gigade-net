using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class TFunction:PageBase
    {
        public int rowid { get; set; }
        public int functionType { get; set; }
        public string functionGroup { get; set; }
        public string functionName { get; set; }
        public string functionCode { get; set; }
        public string iconCls { get; set; }
        public int isEdit { get; set; }
        public string remark { get; set; }
        public string kuser { get; set; }
        public DateTime kdate { get; set; }
        public int topValue { get; set; }
        public TFunction() 
        {
            rowid = 0;
            functionType = 0;
            functionGroup = string.Empty;
            functionName = string.Empty;
            functionCode = string.Empty;
            iconCls = string.Empty;
            isEdit = 0;
            remark = string.Empty;
            kuser = string.Empty;
            kdate = DateTime.MinValue;
            topValue = 0;
        }
    }
}
