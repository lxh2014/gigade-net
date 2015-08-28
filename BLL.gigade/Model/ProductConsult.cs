using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductConsult : PageBase
    {
        public int consult_id { get; set; }
        public uint product_id { get; set; }
        public int user_id { get; set; }
        public string consult_info { get; set; }
        public int consult_type { get; set; }
        public string consult_answer { get; set; }
        public int is_sendEmail { get; set; }
        public DateTime create_date { get; set; }
        public DateTime answer_date { get; set; }
        public int answer_user { get; set; }
        public int status { get; set; }
        public string consult_url { get; set; }
        public string product_url { get; set; }
        public int item_id { get; set; }
        public int answer_status { get; set; }
        public string delay_reason { get; set; }
        public ProductConsult()
        {
            consult_id = 0;
            product_id = 0;
            user_id = 0;
            consult_info = string.Empty;
            consult_type = 0;
            consult_answer = string.Empty;
            is_sendEmail = 0;
            create_date = DateTime.MinValue;
            answer_date = DateTime.MinValue;
            answer_user = 0;
            status = 0;
            consult_url = string.Empty;
            product_url = string.Empty;
            item_id = 0;
            answer_status = 0;
            delay_reason = string.Empty;
        }
    }

}
