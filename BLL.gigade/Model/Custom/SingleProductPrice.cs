﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class SingleProductPrice
    {
        public uint product_id { get; set; }
        public string product_name { get; set; }
        public string prod_sz { get; set; }
        public int price { get; set; }////edit by wwei0216 棄用的 product_item表使用的是無符號類型,新使用的price_master表中用的是整型,所以改為int
        public int s_must_buy { get; set; }
        public int g_must_buy { get; set; }
        public SingleProductPrice()
        {
            product_id = 0;
            product_name = "";
            prod_sz = string.Empty;
            price = 0;
            s_must_buy = 0;
            g_must_buy = 0;
        }
    }

    public class SelfSingleProductPrice
    {
        public uint product_id { get; set; }
        public string product_name { get; set; }
        public string prod_sz { get; set; }
        public int price { get; set; }
        public int s_must_buy { get; set; }
        public int g_must_buy { get; set; }
        public SelfSingleProductPrice()
        {
            product_id = 0;
            product_name = "";
            prod_sz = string.Empty;
            price = 0;
            s_must_buy = 0;
            g_must_buy = 0;
        }
    }
}