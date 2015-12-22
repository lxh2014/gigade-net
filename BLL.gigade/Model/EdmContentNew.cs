using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EdmContentNew : PageBase
    {
        public int content_id { get; set; }//EDM內容代碼，主索引鍵
        public int group_id { get; set; }// EDM群組代碼，決定該EDM屬於哪一個EDM群組
        public string subject { get; set; }//EDM郵件標題
        public int template_id { get; set; }//所選郵件範本代碼
        public string template_data { get; set; } // 範本程式產生的相關資料，未來產生郵件內容時，範本程式會需要讀回去的資料
        public int importance { get; set; }//郵件重要性
        public int sender_id { get; set; }//EDM寄件人代碼
        public DateTime content_createdate { get; set; }//EDM內容建立時間
        public DateTime content_updatedate { get; set; }//EDM內容修改時間
        public int content_create_userid { get; set; }//EDM內容建立者
        public int content_update_userid { get; set; }//EDM內容修改者


        public Int64 count { get; set; }
        public DateTime date { get; set; }

        public string sender_name { get; set; }
        public string sender_email { get; set; }

        public string edit_url { get; set; }
        public string content_url { get; set; }

        public string editor1 { get; set; }
        public string editor2 { get; set; }
        public string split_str { get; set; }

        public int pm { get; set; }
        public string edm_pm { get; set; }

        public int active { get; set; }

        public  UInt64 static_template { get; set; }

        public string user_username_create { get; set; }
        public string user_username_update { get; set; }
        public EdmContentNew()
        {
            content_id = 0;
            group_id = 0;
            subject = string.Empty;
            template_id = 0;
            template_data = string.Empty;
            importance = 0;
            sender_id = 0;
            content_createdate = DateTime.Now;
            content_updatedate = DateTime.Now;
            content_create_userid = 0;
            content_update_userid = 0;
            date = DateTime.MinValue;
            count = 0;
            sender_email = string.Empty;
            sender_name = string.Empty;
            editor1 = string.Empty;
            editor2 = string.Empty;
            split_str = string.Empty;
            pm = 0;
            edm_pm = string.Empty;
            active = 0;
            static_template = 1;
            user_username_create = string.Empty;
            user_username_update = string.Empty;
        }
    }
}
