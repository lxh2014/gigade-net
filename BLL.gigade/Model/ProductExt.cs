using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductExt
    {
        public uint Item_id { get; set; }
        public string Pend_del { get; set; }//等待删除中(Ｙ/Ｎ)
        public int Cde_dt_shp { get; set; }//允出天数
        public string Pwy_dte_ctl { get; set;} //有效期控制的商品(Ｙ/Ｎ)
        public int Cde_dt_incr { get; set; }//保存期限(天数)
        public int Cde_dt_var { get; set; }//允收天数
        public string Hzd_ind { get; set; }//易损坏的等级
        public decimal Cse_wid { get; set; }//外箱的宽度(cm)
        public decimal Cse_wgt { get; set; }//外箱的重量(kg)
        public int Cse_unit { get; set; }//外箱单位(ＯＭ)
        public decimal Cse_len { get; set; }//外箱的长度(cm)
        public decimal Cse_hgt { get; set; }//外箱的高度(cm)
        public int Unit_ship_cse { get; set; }//商品的ＯＰ
        public decimal Inner_pack_wid { get; set; }//内装的宽度(cm)
        public decimal Inner_pack_wgt { get; set; }//内装的重量(kg)
        public int Inner_pack_unit { get; set; }//内装单位(ＯＰ)
        public decimal Inner_pack_len { get; set; }//内装的长度(cm)
        public decimal Inner_pack_hgt { get; set; }//内装的高度(cm)

        public ProductExt()
        {
            Item_id = 0;
            Pend_del = string.Empty;
            Cde_dt_shp = 0;
            Pwy_dte_ctl = string.Empty;
            Cde_dt_incr = 0;
            Cde_dt_var = 0;
            Hzd_ind = string.Empty; ;
            Cse_wid = 0;
            Cse_wgt = 0;
            Cse_unit = 0;
            Cse_len = 0;
            Cse_hgt = 0;
            Unit_ship_cse = 0;
            Inner_pack_wid = 0;
            Inner_pack_wgt = 0;
            Inner_pack_unit = 0;
            Inner_pack_len = 0;
            Inner_pack_hgt = 0;
        }
    }
}
