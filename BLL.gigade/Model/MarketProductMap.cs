using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class MarketProductMap : PageBase
    {
        public int map_id { set; get; }//關係編號
        public int product_category_id { set; get; }//吉甲地類別編號
        public int market_category_id { set; get; }//美安類別編號
        public int kuser { get; set; }//創建人
        public int muser { get; set; }//修改人
        public DateTime created { set; get; }//創建時間
        public DateTime modified { get; set; }//修改時間

        public MarketProductMap()
        {
            map_id = 0;
            product_category_id = 0;
            market_category_id = 0;
            kuser = 0;
            muser = 0;
            created = DateTime.MinValue;
            modified = DateTime.MinValue;
        }
    }
}
