
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class SecretInfoLog : PageBase
    { 
        public int log_id { get; set; }
        public uint user_id { get; set; }
        public DateTime createdate { get; set; }
        public string ipfrom { get; set; }
        public string url { get; set; }
        public DateTime input_pwd_date { get; set; }
        public int type { get; set; }//關聯參數表 1：查詢會員頁面 2:訂單內容 3：簡訊查詢 4：聯絡客服列表
        public int related_id { get; set; }


        public string user_username { get; set; }
        public DateTime date_one { get; set; }
        public DateTime date_two { get; set; }
        public string user_email { get; set; }
        public int sumtotal { get; set; }
        public int ismail { get; set; }
        public int countClass { get; set; }
        public SecretInfoLog()
        {
            log_id = 0;
            user_id = 0;
            createdate = DateTime.MinValue;
            ipfrom = string.Empty;
            url = string.Empty;
            type = 0;
            related_id = 0;
            user_username = string.Empty;
            date_one = DateTime.MinValue;
            date_two = DateTime.MinValue;
            user_email = string.Empty;
            sumtotal = 0;
            ismail = 0;
            countClass = 0;
        }

    }
}
