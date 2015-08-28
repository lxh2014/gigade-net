using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class MemberLevel:PageBase
    {
        public int rowID { get; set; }
        public string ml_code { get; set; }
        public string ml_name { get; set; }
        public int ml_seq { get; set; }
        public int ml_minimal_amount { get; set; }
        public int ml_max_amount { get; set; }
        public int ml_month_seniority { get; set; }
        public int ml_last_purchase { get; set; }
        public int ml_minpurchase_times { get; set; }
        public int ml_birthday_voucher { get; set; }
        public int ml_shipping_voucher { get; set; }
        public SByte ml_status { get; set; }
        public DateTime k_date { get; set; }
        public int k_user { get; set; }
        public DateTime m_date { get; set; }
        public int m_user { get; set; }
   
        public MemberLevel()
        {
            rowID = 0;
            ml_code = string.Empty;
            ml_name = string.Empty;
            ml_seq = 0;
            ml_minimal_amount = 0;
            ml_month_seniority = 0;
            ml_last_purchase = 0;
            ml_minpurchase_times = 0;
            ml_birthday_voucher = 0;
            ml_status = 0;
            k_date = DateTime.MinValue;
            k_user = 0;
            k_date = DateTime.MinValue;
            m_user = 0;
            ml_max_amount = 0;
        }

    }
}
