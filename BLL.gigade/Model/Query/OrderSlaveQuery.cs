using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/**
 * chaojie_zz1124j 添加三個字段delivery_store,estimated_arrival_period,holiday_deliver，用於實現供應商後台>訂單管理>供應商自行出貨列表
 */
namespace BLL.gigade.Model.Query
{
    public class OrderSlaveQuery:OrderSlave
    {
        public string order_name { set; get; }
        public string order_mobile{set;get;}
        public string delivery_name{set;get;}
        public string delivery_mobile{set;get;}
        public string note_order{set;get;}
        public string order_createdate{set;get;}
        public uint order_payment{set;get;}
        public string vendor_name_simple{set;get;}
        public uint dispatch{set;get;}
        public uint order_id { set; get; }
        public string pay_time { set; get; }
        public string status { set; get; }
        public uint vendor_id { set; get; }
        public DateTime order_date_pay { set; get; }
        public string code { set; get; }
        public uint delivery_zip { set; get; }
        public string delivery_address { set; get; }
        public string delivery { set; get; }

        public int delivery_store { set; get; }
        public int estimated_arrival_period { set; get; }
        public uint holiday_deliver { set; get; }


        /// <summary>
        /// 從吉甲地後臺登錄到供應商後臺所需的加密字符串
        /// </summary>
        public string key { get; set; }
        public OrderSlaveQuery()
        {
            order_name = string.Empty;
            order_mobile = string.Empty;
            delivery_name = string.Empty;
            delivery_mobile = string.Empty;
            note_order = string.Empty;
            order_createdate = string.Empty;
            order_payment = 0;
            vendor_name_simple = string.Empty;
            dispatch = 0;
            order_id = 0;
            pay_time = string.Empty;
            status = string.Empty;
            vendor_id = 0;
            order_date_pay = DateTime.Now;
            code = string.Empty;
            delivery_zip = 0;
            delivery_address = string.Empty;
            delivery_store = 0;
            estimated_arrival_period = 0;
            holiday_deliver = 0;
            delivery = string.Empty;
        }

    }
}
