using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    class Edm_Template:PageBase
    {
        public int template_id { get; set; }//EDM範本代碼
        public string template_name { get; set; }//EDM範本名稱
        public string edit_url { get; set; }//EDM編輯者，選擇該範本後，用來給編輯者提供該範本相關資料的網頁
        public string content_url { get; set; }//最終用來產出EDM內容的網頁，會被程式呼叫，以便取得EDM郵件內容。產出的內容會用來寫入到mail_request的body欄位
        public int enabled { get; set; }//設定該範本是否啟用，不啟用的時候，EDM編輯者在EDM新增修改畫面就選不到該範本
        public DateTime template_createdate { get; set; }//範本建立時間
        public DateTime template_updatedate { get; set; }//範本更新時間
        public int template_create_userid { set; get; }//範本建立者的使用者代碼
        public int template_update_userid { set; get; }//範本更新者的使用者代碼

        public Edm_Template() 
        {
            template_id = 0;
            template_name = string.Empty;
            edit_url = string.Empty;
            content_url = string.Empty;
            enabled = 0;
            template_createdate = DateTime.Now;
            template_updatedate = DateTime.Now;
            template_update_userid = 0;
            template_create_userid = 0;
        }
    }
}
