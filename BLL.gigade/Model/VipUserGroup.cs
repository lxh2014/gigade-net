using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model
{
    //vip_user_group
    [DBTableInfo("vip_user_group")]
    public class VipUserGroup : PageBase
    {
        public uint group_id { get; set; }
        public string group_name { get; set; }
        public string domain { get; set; }
        public string tax_id { get; set; }

        public string group_code { get; set; }
        public int group_capital { get; set; }
        public int group_emp_number { get; set; }
        public string group_emp_age { get; set; }
        public int group_emp_gender { get; set; }
        public string group_benefit_type { get; set; }
        public string group_benefit_desc { get; set; }
        public int group_subsidiary { get; set; }
        public string group_hq_name { get; set; }
        public string group_hq_code { get; set; }
        public string group_committe_name { get; set; }
        public string group_committe_code { get; set; }
        public string group_committe_chairman { get; set; }
        public string group_committe_phone { get; set; }
        public string group_committe_mail { get; set; }
        public string group_committe_promotion { get; set; }
        public string group_committe_desc { get; set; }


        public string image_name { get; set; }
        public uint gift_bonus { get; set; }
        public uint createdate { get; set; }
        public uint group_category { get; set; }
        public int bonus_rate { get; set; }
        public int bonus_expire_day { get; set; }
        public string eng_name { get; set; }
        public int check_iden { get; set; }
        public uint site_id { get; set; }



        public int group_status { get; set; }
        public string file_name { get; set; }
        public int k_user { get; set; }
        public DateTime k_date { get; set; }
        public int m_user { get; set; }
        public DateTime m_date { get; set; }

        public VipUserGroup()
        {
            group_id = 0;
            group_name = string.Empty;
            domain = string.Empty;
            tax_id = string.Empty;

            group_code = string.Empty;
            group_capital = 0;
            group_emp_number = 0;
            group_emp_age = string.Empty;
            group_emp_gender = 0;
            group_benefit_type = string.Empty;
            group_benefit_desc = string.Empty;
            group_subsidiary = 0;
            group_hq_name = string.Empty;
            group_hq_code = string.Empty;
            group_committe_name = string.Empty;
            group_committe_code = string.Empty;
            group_committe_chairman = string.Empty;
            group_committe_phone = string.Empty;
            group_committe_mail = string.Empty;
            group_committe_promotion = string.Empty;
            group_committe_desc = string.Empty;

            image_name = string.Empty;
            gift_bonus = 0;
            createdate = 0;
            group_category = 0;
            bonus_rate = 1;
            bonus_expire_day = 90;
            eng_name = string.Empty;
            check_iden = 0;
            site_id = 0;

            group_status = 0;
            file_name = string.Empty;
            k_user = 0;
            k_date = DateTime.MinValue;
            m_user = 0;
            m_date = DateTime.MinValue;
        }
    }
}