using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public class EventPromoDiscount:PageBase
    {
       /// <summary>
       /// 抵用編號
       /// </summary>
       public int discount_id{get;set;}
       /// <summary>
       /// 抵用名稱
       /// </summary>
       public string discount_name { get; set; }
       /// <summary>
       /// 滿額金額
       /// </summary>
       public int amount { get; set; }
       /// <summary>
       /// 滿件數量
       /// </summary>
       public int quantity { get; set; }
       /// <summary>
       /// 折扣量
       /// </summary>
       public int discount { get; set; }
       /// <summary>
       /// 活動編號
       /// </summary>
       public string event_id { get; set; }
       /// <summary>
       /// 創建人
       /// </summary>
       public int create_user { get; set; }
       /// <summary>
       /// 創建時間
       /// </summary>
       public DateTime create_time { get; set; }
       /// <summary>
       /// 異動人
       /// </summary>
       public int modify_user { get; set; }
       /// <summary>
       /// 異動時間
       /// </summary>
       public DateTime modify_time { get; set; }

       public EventPromoDiscount()
       {
           discount_id = 0;
           discount_name = string.Empty;
           amount = 0;
           quantity = 0;
           discount = 0;
           event_id = string.Empty;
           create_user = 0;
           create_time = DateTime.MinValue;
           modify_user = 0;
           modify_time = DateTime.MinValue;
       }
    }
}
