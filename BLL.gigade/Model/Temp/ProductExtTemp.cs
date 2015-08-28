using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Temp
{
    public class ProductExtTemp
    {
        public int item_id { get; set; }
        public string pend_del { get; set; }//等待删除中(Ｙ/Ｎ)
        public int cde_dt_shp { get; set; }//允出天数
        public string pwy_dte_ctl { get; set; }//有效期控制的商品(Ｙ/Ｎ)
        public int cde_dt_incr { get; set; }//保存期限(天数)
        public int cde_dt_var { get; set; }//允收天数
        public string hzd_ind { get; set; }//易损坏的等级
        public int cse_wid { get; set; }//外箱的宽度(cm)
        public int cse_wgt { get; set; }//外箱的重量(kg)
        public string cse_unit { get; set; }//外箱单位(ＯＭ)
        public int cse_len { get; set; }//外箱的长度(cm)
        public int cse_hgt { get; set; }//外箱的高度(cm)
        public int unit_ship_cse { get; set; }//商品的ＯＰ
        public int inner_pack_wid { get; set; }//内装的宽度(cm)
        public int inner_pack_wgt { get; set; }//内装的重量(kg)
        public int inner_pack_unit { get; set; }//内装单位(ＯＰ)
        public int inner_pack_len { get; set; }//内装的长度(cm)
        public int inner_pack_hgt { get; set; }//内装的高度(cm)

        public ProductExtTemp()
        {
            item_id = 0;
            pend_del = string.Empty;
            cde_dt_shp = 0;
            pwy_dte_ctl = string.Empty;
            cde_dt_incr = 0;
            cde_dt_var = 0;
            hzd_ind = string.Empty;
            cse_wid = 0;
            cse_wgt = 0;
            cse_unit = string.Empty;
            cse_len = 0;
            cse_hgt = 0;
            unit_ship_cse = 0;
            inner_pack_wid = 0;
            inner_pack_wgt= 0;
            inner_pack_unit= 0;
            inner_pack_len= 0;
            inner_pack_hgt = 0;
        }
    }
}
