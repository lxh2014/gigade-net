using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    /// <summary>
    ///  吉甲地推薦系統匯出參數實體類 guodong1130w 2015/10/9
    /// </summary>
    public class RecommendedOutPra : PageBase
    {
        public string outType {get;set;}
        public string outTime { get; set; }
        public string nowYear { get; set; }
        public string nowMonth { get; set; }
        public RecommendedOutPra()
        {
            outType = string.Empty;
            outTime = string.Empty;
            nowYear = string.Empty;
            nowMonth = string.Empty;
        }
    }
}
