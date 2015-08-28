/*
* 文件名稱 :Fgroup.cs
* 文件功能描述 :群組
* 版權宣告 :
* 開發人員 : 天娥璇子
* 版本資訊 : 1.0
* 日期 : 2013/08/02
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Fgroup:PageBase
    {
        public int rowid { get; set; }
        public string groupName { get; set; }
        public string groupCode { get; set; }
        public Int64 callid { get; set; }
        public string remark { get; set; }
        public string kuser { get; set; }
        public DateTime kdate { get; set; }

        public Fgroup()
        {
            rowid = 0;
            groupName = string.Empty;
            groupCode = string.Empty;
            callid = 0;
            remark = string.Empty;
            kuser = string.Empty;
            kdate = DateTime.MinValue;
        }
    }
}
