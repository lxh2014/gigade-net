#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：SingleProductPriceTemp.cs
* 摘 要：
* * 供應商組合商品編輯獲取臨時表單一商品信息
* 当前版本：v1.0
* 作 者： mengjuan0826j
* 完成日期：2014/09/02  供應商組合商品編輯獲取臨時表單一商品信息
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class SingleProductPriceTemp
    {
        public string product_id { get; set; }
        public string product_name { get; set; }
        public string prod_sz { get; set; }
        public uint price { get; set; }
        public int s_must_buy { get; set; }
        public int g_must_buy { get; set; }
        public SingleProductPriceTemp()
        {
            product_id = string.Empty;
            product_name = "";
            price = 0;
            s_must_buy = 0;
            g_must_buy = 0;
            prod_sz = string.Empty;
        }
    }
}