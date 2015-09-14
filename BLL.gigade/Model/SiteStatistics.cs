using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class SiteStatistics : PageBase
    {
        public int ss_id { get; set; }
        public int ss_show_num { get; set; }//曝光數
        public int ss_click_num { get; set; }//點擊數
        public float ss_click_through { get; set; }//點擊率
        public float ss_cost { get; set; }//費用
        //public float ss_budget { get; set; }//預算
        //public int ss_effect_num { get; set; }//有效點擊數
        //public float ss_rank { get; set; }//平均排名
        public int ss_newuser_number { get; set; }
        public int ss_converted_newuser { get; set; }
        public int ss_sum_order_amount { get; set; }
        public DateTime ss_date { get; set; }
        public string ss_code { get; set; }
        public DateTime ss_create_time { get; set; }
        public int ss_create_user { get; set; }
        public DateTime ss_modify_time { get; set; }
        public int ss_modify_user { get; set; }

        public DateTime sss_date { get; set; }
        public DateTime ess_date { get; set; }
        public SiteStatistics()
        {
            ss_id = 0;
            ss_show_num = 0;
            ss_click_num = 0;
            ss_click_through = 0;
            ss_sum_order_amount = 0;
            ss_converted_newuser = 0;
            ss_newuser_number = 0;
            ss_date = DateTime.MinValue;
            ss_code = string.Empty;
            ss_create_time = DateTime.MinValue;
            ss_create_user = 0;
            ss_modify_time = DateTime.MinValue;
            ss_modify_user = 0;
            sss_date = DateTime.MinValue;
            ess_date = DateTime.MinValue;
        }
    }
}
