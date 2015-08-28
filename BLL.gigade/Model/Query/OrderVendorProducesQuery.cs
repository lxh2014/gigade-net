using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public class OrderVendorProducesQuery:OrderDetail
    {
        public string pic_patch { get; set; }
        public uint slave_status { get; set; }
        public DateTime slave_date_delivery { get; set; }
        public uint order_id { get; set; }
        public uint user_id { get; set; }
        public DateTime order_createdate { get; set; }
        public string order_name { get; set; }
        public string order_mobile { get; set; }
        public string note_order { get; set; }
        public uint order_payment { get; set; }
        public string payment { get; set; }
        public string delivery_name { get; set; }
        public uint delivery_gender { get; set; }
        public uint delivery_zip { get; set; }
        public string delivery_address { get; set; }
        public string vendor_name_simple { get; set; }
        public DateTime order_date_pay { get; set;}
        public DateTime money_collect_date { get; set; }
        public string product_manage { get; set; }
        public string sqlwhere { get; set; }
        public string selecttype { get; set; }//查詢條件
        public string searchcon { get; set; }//查詢內容
        public string date_type { get; set; }//日期查詢條件
        public DateTime dateStart { get; set; }
        public DateTime dateEnd { get; set; }
        public string slave { get; set; }//日期查詢條件
        public uint SingleMoney { get; set; }
        public string product_freight_set_in { set; get; }
        public OrderVendorProducesQuery()
        {
            pic_patch = string.Empty;
            slave_status = 0;
            slave_date_delivery = DateTime.Now;
            order_id = 0;
            user_id = 0;
            order_createdate = DateTime.Now;
            order_name = string.Empty;
            order_mobile = string.Empty;
            note_order = string.Empty;
            order_payment = 0;
            payment = string.Empty;
            delivery_name = string.Empty;
            delivery_gender = 0;
            delivery_zip = 0;
            delivery_address = string.Empty;
            vendor_name_simple = string.Empty;
            order_date_pay = DateTime.Now;
            money_collect_date= DateTime.MinValue;
            product_manage = string.Empty;
            sqlwhere = string.Empty;
            SingleMoney = 0;
            product_freight_set_in = string.Empty;
        }
    }
}
