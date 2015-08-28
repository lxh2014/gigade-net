/*
* 文件名稱 :UserOrdersSubtotalQuery.cs
* 文件功能描述 :對UserOrdersSubtotal表的擴充
* 版權宣告 :鄭州分公司
* 開發人員 : 文博
* 版本資訊 : 1.0
* 日期 : 2015-7-28
* 修改人員 :無
* 版本資訊 : 1.0
* 日期 : 2015-7-28
* 修改備註 :無
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class UserOrdersSubtotalQuery : UserOrdersSubtotal
    {
        public double startMoney { get; set; }
        public double endMoney { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public int searchType { get; set; }
        public string searchKey { get; set; }
       
        public string user_name { get; set; }
        public UserOrdersSubtotalQuery()
        {
            this.searchKey = string.Empty;
            this.user_name = string.Empty;
            this.searchType = 0;
            this.startMoney = 0;
            this.endMoney = 0;
            this.startTime = DateTime.MinValue;
            this.endTime = DateTime.MinValue;
        }
    }
}
