/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：Users 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：dongya0410j 
 * 完成日期：2014/09/22 13:35:21 
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class Users:PageBase
    {
        public uint user_id { get; set; }
        public string user_email { get; set; }
        public string user_new_email { get; set; }
        public uint user_status { get; set; }
        public string user_password { get; set; }
        public string user_newpasswd { get; set; }
        public string user_name { get; set; }
        public uint user_gender { get; set; }
        public uint user_birthday_year { get; set; }
        public uint user_birthday_month { get; set; }
        public uint user_birthday_day { get; set; }
        public string user_mobile { get; set; }
        public string user_phone { get; set; }
        public uint user_zip { get; set; }
        public string user_address { get; set; }
        public uint user_login_attempts { get; set; }
        public string user_actkey { get; set; }
        public uint user_reg_date { get; set; }
        public uint user_updatedate { get; set; }
        public string user_old_password { get; set; }
        public string user_company_id { get; set; }
        public string user_source { get; set; }
        public string user_fb_id { get; set; }
        public uint user_country { get; set; }
        public uint user_ref_user_id { get; set; }
        public string user_province { get; set; }
        public string user_city { get; set; }
        public uint source_trace { get; set; }
        public int user_type { get; set; }
        public bool send_sms_ad { get; set; }
        public string adm_note { get; set; }
        public uint user_level { get; set; }
        public uint buy_times { get; set; }
        public uint buy_amount { get; set; }
        public uint first_time { get; set; }
        public uint last_time { get; set; }
        public uint be4_last_time { get; set; }
        public bool paper_invoice { get; set; }
        public string ml_code { get; set; }
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
            user_birthday_day = 0;
            user_birthday_month = 0;
            user_birthday_year = 0;
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
            user_ref_user_id = 0;
            user_source = string.Empty;
            user_fb_id = string.Empty;
            user_country = 0;
            user_province = null;
            user_city = null;
            source_trace = 0;
            user_type = 1;
            send_sms_ad = false;
            adm_note = null;
            user_level = 1;
            buy_times = 0;
            buy_amount = 0;
            first_time = 0;
            last_time = 0;
            be4_last_time = 0;
            paper_invoice = false;
        }
    }
}
