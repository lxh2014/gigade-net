using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class EdmSendQuery:EdmSend
    {
        public uint content_id { get; set; }
        public string s_send_status { get; set; }
        public string s_send_datetime { get; set; }
        public string s_open_first { get; set; }
        public string s_open_last { get; set; }
        public uint image_width { get; set; } //最大開信次數
        //Title
        public uint group_id { get; set; }
        public uint content_status { get; set; }
        public uint content_email_id { get; set; }
        public uint content_start { get; set; }
        public uint content_end { get; set; }
        public uint content_range { get; set; }
        public uint content_single_count { get; set; }
        public uint content_click { get; set; }
        public uint content_person { get; set; }
        public uint content_send_success { get; set; }
        public uint content_send_failed { get; set; }
		public string content_from_name { get; set; }
        public string content_from_email { get; set; }
        public string content_reply_email { get; set; }
        public uint content_priority { get; set; }
        public string content_title { get; set; }
        public string content_body { get; set; }
        public uint content_createdate { get; set; }
        public uint content_updatedate { get; set; }

        public uint content_send { get; set; } //總共發信人數
        public double content_openRate { get; set; }//開信率
        public double content_averageClick { get; set; } //平均開信次數
        public string content_start_s { get; set; }
        public int content_imagewidth_send { get; set; } //總共發信人數圖片長度
        public int content_imagewidth_success { get; set; } //成功發信人數圖片長度
        public int content_imagewidth_failed { get; set; } //失敗發信人數圖片長度
        //
        public string sendtime { get; set; }
        public string firsttime { get; set; }
        public string lasttime { get; set; }
        //查詢條件
        public int date { get; set; } //日期條件
        public string start_time { get; set; }
        public string end_time { get; set; }
        //
        public EdmSendQuery()
        {
            content_id = 0;
            s_send_status = string.Empty;
            s_send_datetime = string.Empty;
            s_open_first = string.Empty;
            s_open_last = string.Empty;
            image_width = 0;
            //Title
            group_id = 0;
            content_status = 0;
            content_email_id = 0;
            content_start = 0; //發送時間
            content_end = 0;
            content_range = 0;
            content_single_count = 0;
            content_click = 0;  //總開信次
            content_person = 0;  //總開信人數
            content_send_success = 0; //發信成功人數
            content_send_failed = 0; //發信失敗人數
		    content_from_name = string.Empty;
            content_from_email = string.Empty;
            content_reply_email = string.Empty;
            content_priority = 0;
            content_title = string.Empty; //郵件主旨
            content_body = string.Empty;
            content_createdate = 0;
            content_updatedate = 0;

            content_start_s = string.Empty; //發送時間
            content_send = 0; //總共發信人數
            content_openRate = 0;//開信率
            content_averageClick = 0; //平均開信次數
            content_imagewidth_send = 0;
            content_imagewidth_success = 0;
            content_imagewidth_failed = 0;
            //
            sendtime = string.Empty;
            firsttime = string.Empty;
            lasttime = string.Empty;
            //查詢條件
            date = 0;
            start_time = string.Empty;
            end_time = string.Empty;
            //
        }
    }
}
