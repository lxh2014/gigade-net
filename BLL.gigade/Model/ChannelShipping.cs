/*
* 文件名稱 :ChannelShipping.cs
* 文件功能描述 :外站運費設定
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
    public class ChannelShipping:PageBase
    {
        /*
        public int channel_id { get; set; }
        public int shipping_carrior { get; set; }
        public int shipping_carrior_content { get; set; }
        public int threshold { get; set; }
        public int fee { get; set; }
        public int return_fee { get; set; }
        public int shipping_type { get; set; }
        public int ship_logistics { get; set; }
        public string shipping_type_content { get; set; }
        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }

        public ChannelShipping()
        { 
            channel_id = 0;
            shipping_carrior = 0;
            shipping_carrior_content = 0;
            threshold = 0;
            ship_logistics = 0;
            fee = 0;
            return_fee = 0;
            shipping_type = 0;
            shipping_type_content = string.Empty;
            createdate = DateTime.MinValue;
            updatedate = DateTime.MinValue;
        }*/

        public int channel_id { get; set; }
        public int shipping_carrior { get; set; }
        public int shipping_carrior_content { get; set; }
        public string shipco { get; set; }
        public int n_threshold { get; set; }
        public int l_threshold { get; set; }
        public int n_fee { get; set; }
        public int l_fee { get; set; }
        public int n_return_fee { get; set; }
        public int l_return_fee { get; set; }
        public int retrieve_mode { get; set; }

        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }

        public ChannelShipping()
        {
            channel_id = 0;
            shipping_carrior = 0;
            shipping_carrior_content = 0;
            n_threshold = 0;
            l_threshold = 0;
            shipco = "";
            n_fee = 0;
            l_fee = 0;
            n_return_fee = 0;
            l_return_fee = 0;
            retrieve_mode = 0;

            createdate = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")); //DateTime.MinValue;
            updatedate = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));//DateTime.MinValue;
        }
    }
}
