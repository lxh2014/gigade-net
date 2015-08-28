using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class OrderDetailCustom:OrderDetail
    {
        //add by wwei0216w
        public int arrive_days { get; set; }//運達天數(修正值)
        public int deliver_days { get; set; }//供應商出貨天數
        public uint product_id { get; set; }//商品id
        public uint order_date_pay { get; set; }//付款日期
        public int item_stock { get; set; }//庫存
        public int parentId { get; set; }//父商品id

        public OrderDetailCustom()
        {
            arrive_days = 0;
            deliver_days = 0;
            product_id = 0;
            order_date_pay = 0;
            item_stock = 0;
            parentId = 0;
        }
    }
}
