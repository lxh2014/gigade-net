using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Users : PageBase
    {
        public UInt64 user_id { get; set; }
        public string user_email { get; set; }
        public string user_new_email { get; set; }
        public int user_status { get; set; }
        public string user_password { get; set; }
        public string user_newpasswd { get; set; }
        public string user_name { get; set; }
        public int user_gender { get; set; }
        public int user_birthday_year { get; set; }
        public int user_birthday_month { get; set; }
        public int user_birthday_day { get; set; }
        public string user_mobile { get; set; }
        public string user_phone { get; set; }
        public int user_zip { get; set; }
        public string user_address { get; set; }
        public int user_login_attempts { get; set; }
        public string user_actkey { get; set; }
        public int user_reg_date { get; set; }
        public int user_updatedate { get; set; }
        public string user_old_password { get; set; }
        public string user_company_id { get; set; }
        public string user_source { get; set; }
        public string user_fb_id { get; set; }
        public int user_country { get; set; }
        public int user_ref_user_id { get; set; }
        public string user_province { get; set; }
        public string user_city { get; set; }
        public int source_trace { get; set; }
        public int user_type { get; set; }
        public bool send_sms_ad { get; set; }
        public string adm_note { get; set; }
        public uint user_level { get; set; }
        

        public Users()
        {
            user_id = 0;
            user_email = string.Empty;
            user_new_email = string.Empty;
            user_status = 0;
            user_password = string.Empty;
            user_newpasswd = string.Empty;
            user_name = string.Empty;
            user_gender = 0;
            user_birthday_year = 0;
            user_birthday_month = 0;
            user_birthday_day = 0;
            user_mobile = string.Empty;
            user_phone = string.Empty;
            user_zip = 0;
            user_address = string.Empty;
            user_login_attempts = 0;
            user_actkey = string.Empty;
            user_reg_date = 0;
            user_updatedate = 0;
            user_old_password = string.Empty;
            user_company_id = string.Empty;
            user_source = string.Empty;
            user_fb_id = string.Empty;
            user_country = 0;
            user_ref_user_id = 0;
            user_province = string.Empty;
            user_city = string.Empty;
            source_trace = 0;
            user_type = 0;
            send_sms_ad = false;
            adm_note = string.Empty;
            user_level = 0;
        }
    }
}
