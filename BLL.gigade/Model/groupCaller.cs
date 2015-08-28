/*
* 文件名稱 :groupCaller.cs
* 文件功能描述 :群組人員
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
    public class groupCaller : PageBase
    {
        public int rowid { get; set; }
        public int groupId { get; set; }
        public string callid { get; set; }

        public groupCaller()
        {
            rowid = 0;
            groupId = 0;
            callid = string.Empty;
        }
    }
}
