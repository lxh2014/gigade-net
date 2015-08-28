using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model.Custom
{
    [DBTableInfo("product_ext")]
    public class ProductExtCustom : ProductExt
    {
        public uint Product_id { get; set; }
        public string Product_name { get; set; }
        public bool Pend_del_bool
        {
            get
            {
                return this.Pend_del == "Y" ? true : false;

            }
            set
            {
                this.Pend_del = value ? "Y" : "N";
            }
        }
        public bool Pwy_dte_ctl_bool
        {
            get
            {
                return this.Pwy_dte_ctl =="Y" ? true : false;

            }
            set
            {
                this.Pwy_dte_ctl = value ? "Y" : "N";
            }
        }
        public string Spec_name { get; set; } //add by wwei 0215/4/2
        public string Brand_name { get; set; }//商品品牌 edit by wwei0216w 2015/6/15
        public uint Brand_id { get; set; }
        public string User_mail { get; set; }//創建人edit by wwei0216w 2015/6/15
        public DateTime Product_createdate { get; set; }//創建時間edit by wwei0216w 2015/6/15
        public string Kuser { get; set; }//修改人edit by wwei0216w 2015/6/15
        public DateTime Kdate { get; set; }//修改時間edit by wwei0216w 2015/6/15
        public string Col_name { get; set; }//修改的欄位名稱edit by wwei0216w 2015/6/15
        public DateTime Update_start { get; set; }
        public DateTime Update_end { get; set; }
        public string Incr_old { get; set; }//(保存期限)修改前值
        public string Shp_old { get; set; }//(允出天數)修改前值
        public string Var_old { get; set; }//(允收天數)修改前值
        public string Batchno { get; set; }//修改的批次號
        //public string MessageStr { get; set; }//保存前後記錄的字段
        //public string Old_value { get; set; }//原始值edit by wwei0216w 2015/6/15
        //public string Col_value { get; set; }//修改后的值edit by wwei0216w 2015/6/15
        public string Var_value { get; set; }//(允收天數)修改後值
        public string Shp_value { get; set; }//(允出天數)修改后值
        public string Incr_value { get; set; }//(保存期限)修改后值

        public ProductExtCustom()
        {
            Product_id = 0;
            Brand_id = 0;
            Product_name = string.Empty;
            Spec_name = string.Empty;
            Brand_name = string.Empty;
            User_mail = string.Empty;
            Product_createdate = DateTime.MinValue;
            Kuser = string.Empty;
            Kdate = DateTime.MinValue;
            Col_name = string.Empty;
            Incr_old = "0";
            Shp_old = "0";
            Var_old = "0";
            Batchno = "0";
            Var_value = "0";
            Shp_value = "0";
            Incr_value = "0";
            Update_start = DateTime.MinValue;
            Update_end = DateTime.MaxValue;
        }

        public enum Condition { ProductId = 1, ItemId = 2, BrandId = 3 }
    }
}
