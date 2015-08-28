using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public  class EmsDepRelation:PageBase
    {
        public int relation_id { get; set; }
        public int relation_type { get; set; }
        public int relation_order_count { get; set; }
        public int relation_order_cost { get; set; }
        public int relation_dep { get; set; }
        public DateTime update_time { get; set; }
        public DateTime create_time { get; set; }
        public int relation_create_type { get; set; }
        public int create_user { get; set; }
        public int update_user { get; set; }
        public int relation_year { get; set; }
        public int relation_month { get; set; }
        public int relation_day { get; set; }

        public string dep_code { get; set; }
        public string dep_name { get; set; }
        public string user_username { get; set; }
        public int datatype { get; set; }
        public int searchdate { get; set; }
        public DateTime date { get; set; }
        public string emsdep { get; set; }
        public int value { get; set; }
        public int re_type { get; set; }
        public DateTime predate { get; set; }
        public string dep_code_insert { get; set; }
        public int relation_type_insert { get; set; }
        public int insert_day { get; set; }
        public EmsDepRelation()
        {
            relation_id = 0;
            relation_type = 0;
            relation_order_count = 0;
            relation_order_cost = 0;
            relation_dep = 0;
            update_time = DateTime.MinValue;
            create_time = DateTime.MinValue;
            relation_create_type =2;
            create_user = 2;
            update_user = 0;
            relation_year = 0;
            relation_month = 0;
            relation_day = 0;
            dep_name = string.Empty;
            dep_code = string.Empty;
            user_username = string.Empty;
            datatype =0;
            searchdate = 0;
            date = DateTime.MinValue;
            emsdep = string.Empty;
            value = 0;
            re_type = 0;
            predate = DateTime.MinValue;
            dep_code_insert = string.Empty;
            relation_type_insert = 0;
        }
    }
}
