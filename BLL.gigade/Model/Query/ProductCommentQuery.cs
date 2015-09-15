using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ProductCommentQuery : ProductComment
    {
        public int comment_detail_id { get; set; }
        public int comment_type { get; set; }
        public string comment_info { get; set; }
        public int status { get; set; }
        public string user_ip { get; set; }
        public int sender_attitude { get; set; }
        public int logistics_deliver { get; set; }
        public int web_server { get; set; }
        public int seller_server { get; set; }
        public int product_desc { get; set; }
        public int comment_numId { get; set; }
        public string product_name { get; set; }
        public string user_email { get; set; }
        public int beginTime { get; set; }
        public int endTime { get; set; }
        public string brand_name { get; set; }
        public string comment_advice { get; set; }
        public string categoryIds { get; set; }
        public string productIds { get; set; }
        public string parameterCode { get; set; }
        public string parameterName { get; set; }
        public string user_name { get; set; }
        public string comment_answer { get; set; }
        public string old_comment_answer { get; set; }
        public int isReplay { get; set; }
        public int answer_is_show { get; set; }
        public string old_answer_is_show { get; set; }
        public int send_mail { get; set; } //0 發送  1 不發送
        public int reply_time { get; set; }
        public int reply_user { get; set; }
        public string s_reply_time { get; set; }
        public string s_reply_user { get; set; }
        public string commentsel { get; set; } //滿意度
        //異動記錄
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public string change_table { get; set; }
        public ProductCommentQuery()
        {
            comment_detail_id = 0;
            comment_type = -1;
            comment_info = string.Empty;
            status = 0;
            user_ip = string.Empty;
            sender_attitude = 0;
            logistics_deliver = 0;
            web_server = 0;
            seller_server = 0;
            product_desc = 0;
            comment_numId = 0;
            product_name = string.Empty;
            user_email = string.Empty;
            beginTime = 0;
            endTime = 0;
            brand_name = string.Empty;
            comment_advice = string.Empty;
            categoryIds = string.Empty;
            productIds = string.Empty;
            parameterCode = string.Empty;
            parameterName = string.Empty;
            user_email = string.Empty;
            comment_answer = string.Empty;
            old_comment_answer = string.Empty;
            send_mail = 1;  //默認不發送郵件
            isReplay = 0;
            answer_is_show = 0;
            old_answer_is_show = string.Empty;
            reply_time = 0;
            reply_user = 0;
            s_reply_time = string.Empty;
            s_reply_user = string.Empty;
            commentsel = string.Empty;
            start_time = DateTime.MinValue;
            end_time = DateTime.MinValue;
            change_table = string.Empty;
        }
    }
}
