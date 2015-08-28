using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class OrderShowMaster
    {
        public uint order_id { get; set; } //付款單號 
        public int order_payment { get; set; }//付款方式
        public uint order_date_pay { get; set; }//可出貨日期
        public int money_collect_date { get; set; }//帳款實收日期
        public uint order_status { get; set; }//付款單狀態
        public uint order_createdate { get; set; }//付款單成立日期
        public uint order_date_close { get; set; }//付款單歸檔日期
        public int company_write { get; set; }
        public string company_invoice { get; set; }//發票統編
        public string company_title { get; set; }//發票抬頭
        public int channel { get; set; }//賣場
        public DateTime import_time { get; set; }//外站匯入時間
        public string channel_order_id { get; set; }//外站訂單編號
        public int retrieve_mode { get; set; }//取貨方式
        public int delivery_store { get; set; }//物流模式
        public bool billing_checked { get; set; }//對帳
        public uint holiday_deliver { get; set; }
        public string note_order { get; set; }
        public uint order_product_subtotal { get; set; }//商品總金額
        public uint deduct_bonus { get; set; }//購物金抵用金額
        public int accumulated_bonus { get; set; }//購物金發放金額
        public int deduct_happygo { get; set; }//HG抵用點數
        public int accumulated_happygo { get; set; }//HG發放點數
        public uint deduct_welfare { get; set; }//抵用券金額
        public uint order_freight_normal { get; set; }//常溫運費
        public uint order_freight_low { get; set; }//低溫運費
        public uint order_amount { get; set; }//訂單應付金額
        public uint money_cancel { get; set; }//取消金額
        public uint money_return { get; set; }//退貨金額
        public uint user_id { get; set; }
        public string order_name { get; set; }//姓名
        public uint order_gender { get; set; }//性別
        public string order_mobile { get; set; }//手機
        public uint order_zip { get; set; }//帳單地址
        public string order_address { get; set; }//帳單地址
        public int delivery_same { get; set; }//姓名
        public string delivery_name { get; set; }//姓名
        public string delivery_mobile { get; set; }//手機
        public uint delivery_zip { get; set; }//收貨地址
        public string delivery_address { get; set; }//收貨地址
        public string note_admin { get; set; }//管理員備註
        public int estimated_arrival_period { get; set; }
        public int deduct_card_bonus { get; set; }
        public string order_phone { get; set; }
        public uint note_order_modify_time { get; set; }
        public string status_description { get; set; }
        public float deduct_happygo_convert { get; set; }
        public string delivery_phone { get; set; }
        public uint delivery_gender { get; set; }

        public OrderShowMaster()
        {
            order_id = 0;
            order_payment = 0;
            order_date_pay = 0;
            money_collect_date = 0;
            order_status = 0;
            order_createdate = 0;
            order_date_close = 0;
            company_write = 0;
            company_invoice = string.Empty;
            company_title = string.Empty;
            channel = 0;
            import_time = DateTime.MinValue;
            channel_order_id =string.Empty;
            retrieve_mode = 0;
            billing_checked = false;
            holiday_deliver = 0;
            note_order = string.Empty;
            order_product_subtotal = 0;
            deduct_bonus = 0;
            accumulated_bonus = 0;
            deduct_happygo = 0;
            accumulated_happygo = 0;
            deduct_welfare = 0;
            order_freight_normal = 0;
            order_freight_low = 0;
            order_amount = 0;
            money_cancel = 0;
            money_return = 0;
            user_id = 0;
            order_name = string.Empty;
            order_gender = 0;
            order_mobile = string.Empty;
            order_zip = 0;
            order_address = string.Empty;
            delivery_same = 0;
            delivery_name = string.Empty;
            delivery_mobile = string.Empty;
            delivery_zip = 0;
            delivery_address = string.Empty;
            note_admin = string.Empty;
            estimated_arrival_period = 0;
            deduct_card_bonus = 0;
            order_phone = string.Empty;
            note_order_modify_time = 0;
            status_description = string.Empty;
            deduct_happygo_convert = 0;
            delivery_phone = string.Empty;
            delivery_gender = 0;
        }
        
    }
}
