using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class OrderBrandProducesQuery : Model.Custom.OrderDetails
    {
        public DateTime order_createdates { get; set; }
        public DateTime order_date_pays { get; set; }
        public DateTime slave_date_deliverys { get; set; }
        public uint user_id { get; set; }
        public string order_name { get; set; }
        public string order_mobile { get; set; }
        public uint order_payment { get; set; }
        public string delivery_name { get; set; }
        public uint delivery_gender { get; set; }
        public uint delivery_zip { get; set; }
        public string delivery_address { get; set; }
        public string note_order { get; set; }
        public uint order_date_pay { get; set; }
        public uint order_createdate { get; set; }
        public uint order_id { get; set; }
        public uint slave_status { get; set; }
        public uint slave_date_delivery { get; set; }
        public string vendor_name_simple { get; set; }
        public string brand_name { get; set; }
        public uint brand_id { get; set; }
        public uint product_id { get; set; }
        public uint user_reg_date { get; set; }
        public uint user_birthday_year { get; set; }
        public uint user_birthday_month { get; set; }
        public uint user_birthday_day { get; set; }
        public string user_email { get; set; }
        public string delivery_mobile { get; set; }
        public string delivery_phone { get; set; }
        public string note_admin { get; set; }
        public string events { get; set; }
        public string user_birthday { get; set; }
        public string delivery_genders { get; set; }
        public string delivery_zips { get; set; }
        public DateTime user_reg_dates { get; set; }
        public string payments { get; set; }
        public string states { get; set; }
        public uint SingleMoney { get; set; }
        //查詢條件
        public uint product_manage { get; set; }
        public int channel { get; set; }
        public string selecttype { get; set; }
        public string searchcon { get; set; }
        public string date_type { get; set; }
        public DateTime dateOne { get; set; }
        public DateTime dateTwo { get; set; }
        public string bid { get; set; }
        public string slave { get; set; }

        public OrderBrandProducesQuery()
        {
            user_id = 0;
            order_name = string.Empty;
            order_mobile = string.Empty;
            order_payment = 0;
            delivery_name = string.Empty;
            delivery_gender = 0;
            delivery_zip = 0;
            delivery_address = string.Empty;
            note_order = string.Empty;
            order_date_pay = 0;
            order_createdate = 0;
            //order_id = 0;
            slave_status = 0;
            slave_date_delivery = 0;
            vendor_name_simple = string.Empty;
            brand_name = string.Empty;
           
          
            brand_id = 0;
            product_id = 0;
            order_createdates = DateTime.MinValue;
            order_date_pays = DateTime.MinValue;
            slave_date_deliverys = DateTime.MinValue;
            SingleMoney = 0;


        }
    }
}
