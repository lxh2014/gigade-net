using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/**
 * chaojie1124j添加return_id 字段，實現訂單內容詳細記錄的退款單頁面
*/
namespace BLL.gigade.Model.Query
{
     public class OrderReturnContentQuery : OrderReturnContent
    {
        public string vendor_name_simple { set; get; }
        public string company_fax { set; get; }
        public string company_phone { set; get; }
         public string order_status_str { set; get; }
         public uint return_id { get; set; }
         public OrderReturnContentQuery()
         {
             vendor_name_simple = string.Empty;
            company_phone = string.Empty;
             order_status_str = string.Empty;
             return_id = 0;
         }

    }
}
