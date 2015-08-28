using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
      public class SiteModel : PageBase
    {
        public uint site_id { get; set; }//流水號，預設1為吉甲地
        public string site_name { get; set; }//站台名稱
        public string domain { get; set; }
        public uint cart_delivery { get; set; }
        public int online_user { get; set; }//站台人數
        public int max_user { get; set; }//最大人數
        public string page_location { get; set; }//站台首頁
        public int site_status { get; set; }
        public DateTime site_createdate { set; get; }
        public DateTime site_updatedate { set; get; }
        public int create_userid { set; get; }
        public int update_userid { set; get; }

        public SiteModel()
        {
            site_id = 1;
            site_name = string.Empty;
            domain = string.Empty;
            cart_delivery = 0;
            online_user = 0;
            max_user = 0;
            page_location = string.Empty;
            site_status = 0;
            site_createdate = DateTime.Now;
            site_updatedate = DateTime.Now;
            create_userid = 0;
            update_userid = 0;


        }
    }
}
