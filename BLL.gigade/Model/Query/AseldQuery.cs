using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class AseldQuery : Aseld
    {
        public uint buy_num { get; set; }
        public uint parent_num { get; set; }
        public UInt64 order_id { get; set; }
        public uint detail_id { get; set; }
        public uint user_id { get; set; }
        public uint product_mode { get; set; }
        public int ticket_id { get; set; }
        public DateTime cde_dt { get; set; }
        public int cde_dt_shp { get; set; }

        /// <summary>
        /// 保存期限
        /// </summary>
        public int cde_dt_incr { set; get; }//保存期限
        /// <summary>
        /// 製造日期
        /// </summary>
        public DateTime made_dt { set; get; }
        //20150504 add訂單備註信息
        public string note_order { set; get; }
        public string freight_set { set; get; }

        public string hash_loc_id { get; set; }

        public AseldQuery()
        {
            buy_num = 0;
            parent_num = 0;
            order_id = 0;
            detail_id = 0;
            user_id = 0;
            product_mode = 0;
            ticket_id = 0;
            cde_dt_shp = 0;
            cde_dt_incr = 0;
            note_order = string.Empty;
            hash_loc_id = string.Empty;
        }

    }
}
