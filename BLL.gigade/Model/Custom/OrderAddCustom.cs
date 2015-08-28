using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
namespace BLL.gigade.Model.Custom
{
    public class OrderAddCustom : ProductItem
    {
        public string product_name { get; set; }
        public uint product_freight_set { get; set; }
        public List<ProductSpec> specList1 { get; set; }
        public List<ProductSpec> specList2 { get; set; }
        public uint buynum { get; set; }
        public uint sumprice { get; set; }
        public uint totalPrice { get; set; }
        public string msg { get; set; }
        public string specName { get; set; }
        public uint product_cost { get; set; }//定价
        /// <summary>
        /// 存放此單一商品在組合商品中的必購數量
        /// </summary>
        public int s_must_buy { get; set; }

        public int original_price { get; set; }
        public int parent_id { get; set; }

        //組合商品群組id
        public int group_id { get; set; }

        /// <summary>
        /// 對應於該商品在價格表中的id
        /// </summary>
        public uint price_master_id { get; set; }

        /// <summary>
        /// 價格類型
        /// </summary>
        public int price_type { get; set; }

        /// <summary>
        /// 庫存為0時是否可以販賣
        /// </summary>
        public int ignore_stock { get; set; }

        public string newparent_id { get; set; }

        /// <summary>
        /// 購物金金額   zhuoqin0830w  2015/04/30
        /// </summary>
        public uint deduct_bonus { get; set; }
        /// <summary>
        /// 抵用卷金額  zhuoqin0830w  2015/04/30
        /// </summary>
        public uint deduct_welfare { get; set; }

        /// <summary>
        /// 流水號，預設1為吉甲地  add by zhuoqin0830w 2015/07/03
        /// </summary>
        public uint Site_Id { get; set; }

        /// <summary>
        /// 返還購物金  add by zhuoqin0830w 2015/07/31
        /// </summary>
        public int accumulated_bonus { get; set; }

        public OrderAddCustom()
        {
            product_name = string.Empty;
            product_freight_set = 0;
            specList1 = null;
            specList2 = null;
            buynum = 0;
            sumprice = 0;
            totalPrice = 0;
            msg = string.Empty;
            specName = string.Empty;
            product_cost = 0;
            s_must_buy = 0;
            price_master_id = 0;
            group_id = 0;
            ignore_stock = 0;
            newparent_id = "hfxl";//用於存儲string類型的合作外站的Product_id
            deduct_bonus = 0;//添加 購物金金額 顯示 zhuoqin0830w  2015/04/30
            deduct_welfare = 0;//添加 抵用卷金額 顯示 zhuoqin0830w  2015/04/30
            Site_Id = 0;// 流水號，預設1為吉甲地  add by zhuoqin0830w 2015/07/03
            accumulated_bonus = 0;// 返還購物金  add by zhuoqin0830w 2015/07/31
        }
    }
}