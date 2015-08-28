using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class PromotionsAmountReduceMemberQuery : PromotionsAmountReduceMember
    {
        public string name { get; set; }
        public int type { get; set; }
        public uint groupid { get; set; }
        public int quantity { get; set; }
        public string user_name { get; set; }
        public string group_name { get; set; }
        public string user_email { get; set; }
        public DateTime screatedate { get; set; }
        public string user_password { get; set; }
        public int user_gender { get; set; }
        public uint user_birthday_year { get; set; }
        public uint user_birthday_month { get; set; }
        public uint user_birthday_day { get; set; }
        public string birthday { get; set; }
        public string user_mobile { get; set; }
        public string user_phone { get; set; }
        public uint user_zip { get; set; }
        public string user_address { get; set; }
        public uint user_reg_date { get; set; }
        public uint user_country { get; set; }
        public string user_province { get; set; }
        public string user_city { get; set; }
        public sbyte user_type { get; set; }
        public bool send_sms_ad { get; set; }
        public string adm_note { get; set; }
        public string mytype { get; set; }
        public DateTime reg_date { set; get; }
        public DateTime suser_reg_date { get; set; }
        public string select_type { get; set; }
        public string search_con { get; set; }
        public int search_date { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public PromotionsAmountReduceMemberQuery()
        {
            groupid = 0;
            name = string.Empty;
            type = 0;
            quantity = 0;
            user_name = string.Empty;
            group_name = string.Empty;
            screatedate = DateTime.MinValue;
            user_password = string.Empty;
            user_email = string.Empty;
            user_gender = 0;
            user_birthday_year = 0;
            user_birthday_month = 0;
            user_birthday_day = 0;
            birthday = string.Empty;
            user_mobile = string.Empty;
            user_phone = string.Empty;
            user_zip = 0;
            user_address = string.Empty;
            user_reg_date = 0;
            user_country = 0;
            user_province = string.Empty;
            user_city = string.Empty;
            user_type = 0;
            send_sms_ad = false;
            adm_note = string.Empty;
            mytype = string.Empty;
            reg_date = DateTime.MinValue;
            suser_reg_date = DateTime.MinValue;
            select_type = string.Empty;
            search_date = 0;
            start_time = DateTime.MinValue;
            end_time = DateTime.MinValue;
        }



    }
}
