/*
* 文件名稱 :ChannelContact.cs
* 文件功能描述 :外站聯絡人
* 版權宣告 :
* 開發人員 : 天娥璇子
* 版本資訊 : 1.0
* 日期 : 2013/08/19
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ChannelContact:PageBase
    {
        public int rid { get; set; }
        public int channel_id { get; set; }
        public string contact_type { get; set; }
        public string contact_name { get; set; }
        public string contact_phone1 { get; set; }
        public string contact_phone2 { get; set; }
        public string contact_mobile { get; set; }
        public string contact_email { get; set; }
        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }

        public ChannelContact()
        { 
            rid = 0;
            channel_id = 0;
            contact_type = string.Empty;
            contact_name = string.Empty;
            contact_phone1 = string.Empty;
            contact_phone2 = string.Empty;
            contact_mobile = string.Empty;
            contact_email = string.Empty;
            createdate = DateTime.MinValue;
            updatedate = DateTime.MinValue;
        }
    }
}
