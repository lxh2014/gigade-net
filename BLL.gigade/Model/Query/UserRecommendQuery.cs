using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class UserRecommendQuery : UserRecommend
    {
        //public string user_name { get; set; }
       
        public int expired { get; set; }
        public int ddlstore { get; set; }
        public string con { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        //public uint user_birthday_month { get; set; }
        //public uint user_birthday_day { get; set; }
        //public uint user_birthday_year { get; set; }
        //public string birthday { get; set; }
        public DateTime suser_reg_date { get; set; }
        public uint suser_id { get; set; }
        #region 新增的
        public string user_password { get; set; }
        public string user_name { get; set; }
        public int user_gender { get; set; }
        public uint user_birthday_year { get; set; }
        public uint user_birthday_month { get; set; }
        public uint user_birthday_day { get; set; }
        public string birthday { get; set; }
        public string user_mobile { get; set; }
        public string user_phone { get; set; }
        public uint user_zip { get; set; }
        public string user_address { get; set; }
        public DateTime Iuser_reg_date { get; set; }
        public uint user_reg_date { get; set; }
        public uint user_country { get; set; }
        public string user_province { get; set; }
        public string user_city { get; set; }
        public sbyte user_type { get; set; }
        public bool send_sms_ad { get; set; }
        public bool paper_invoice { get; set; }
        public string adm_note { get; set; }
        public string mytype { get; set; }
        public string user_email { get; set; }
        public string usname { get; set; }

        #endregion
        public UserRecommendQuery()
        {
            //user_name = string.Empty;
            Start = 0;
            Limit = 0;
            expired = 0;
            ddlstore = 0;
            //user_birthday_month = 0;
            //user_birthday_day = 0;
            //user_birthday_year = 0;
            //birthday = string.Empty;
            con = string.Empty;
            startdate = DateTime.Parse("2010/01/01");
            enddate = DateTime.Now;
            suser_reg_date = DateTime.MinValue;
            suser_id = 0;
            #region 新增的
            user_password = string.Empty;
            user_name = string.Empty;
            user_gender = 0;
            user_birthday_year = 0;
            user_birthday_month = 0;
            user_birthday_day = 0;
            birthday = string.Empty;
            user_mobile = string.Empty;
            user_phone = string.Empty;
            user_zip = 0;
            user_address = string.Empty;
            Iuser_reg_date = DateTime.Parse("2010/01/01");
            user_reg_date = 0;
            user_country = 0;
            user_province = string.Empty;
            user_city = string.Empty;
            user_type = 0;
            send_sms_ad = false;
            paper_invoice = false;
            adm_note = string.Empty;
            mytype = string.Empty;
            usname = string.Empty;
            #endregion
        }
    }
}
