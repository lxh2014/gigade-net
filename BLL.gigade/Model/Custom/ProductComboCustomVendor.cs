#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：ProductComboCustomVendor.cs
* 摘 要：
* * 供應商商品管理 組合商品規格 數據  包括正式表詩句和臨時表該供應商新增過的數據
* 当前版本：v1.0
* 作 者： mengjuan0826j
* 完成日期：2014/08/26   供應商商品管理 組合商品規格 數據  包括正式表詩句和臨時表該供應商新增過的數據
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class ProductComboCustomVendor : BLL.gigade.Model.ProductComboTemp
    {
        public string Product_Name { get; set; }
        public string prod_sz { get; set; }
        public uint brand_id { get; set; }
        public int item_money { get; set; }
        public int event_money { get; set; }
        public string spec_1 { get; set; }
        public string spec_2 { get; set; }
        public uint item_id { get; set; }
        public uint item_price_id { get; set; }
        public uint price_master_id { get; set; }
        public int user_id { get; set; }
        public int user_level { get; set; }
        public int site_id { get; set; }
        public int item_cost { get; set; }
        public int event_cost { get; set; }
        public int price_type { get; set; }
        public int create_channel { get; set; }
        public int temp_status { get; set; }
        public ProductComboCustomVendor()
        {
            Product_Name = string.Empty;
            brand_id = 0;
            item_money = 0;
            event_money = 0;
            spec_1 = string.Empty;
            spec_2 = string.Empty;
            item_id = 0;
            item_price_id = 0;
            price_master_id = 0;
            user_id = 0;
            user_level = 0;
            site_id = 0;
            item_cost = 0;
            event_cost = 0;
            create_channel = 0;
            temp_status = 0;
            prod_sz = string.Empty;
        }
    }
}
