using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class IwmsRrecord : PageBase
    {
        public int row_id { set; get; }
        public int order_id { get; set; }
        public int detail_id { get; set; }
        /// <summary>
        /// 檢貨數量
        /// </summary>
        public int act_pick_qty { get; set; }
        public DateTime made_dt { get; set; }
        public DateTime cde_dt { get; set; }
        public int status { get; set; }
        public DateTime create_date { get; set; }
        public int create_user_id { get; set; }
        /// <summary>
        /// 保存天數
        /// </summary>
        public int cde_dt_incr { get; set; }
        /// <summary>
        /// 允出天數
        /// </summary>
        public int cde_dt_shp { get; set; }
        public IwmsRrecord()
        {
            row_id = 0;
            order_id = 0;
            detail_id = 0;
            act_pick_qty = 0;
            made_dt = DateTime.Now;
            cde_dt = DateTime.Now;
            create_date = DateTime.Now;
            status = 0;
            create_user_id = 0;
            cde_dt_incr = 0;
            cde_dt_shp = 0;
        }
    }
}
