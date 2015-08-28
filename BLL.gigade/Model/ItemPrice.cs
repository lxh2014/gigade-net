/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ItemPrice 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/25 10:55:34 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model
{
    [DBTableInfo("item_price")]
    public class ItemPrice : PageBase
    {
        public uint item_price_id { get; set; }
        public uint item_id { get; set; }
        public uint price_master_id { get; set; }
        public uint item_money { get; set; }
        public uint item_cost { get; set; }
        public uint event_money{get;set;}
        public uint event_cost{get;set;}
        public uint apply_id { get; set; }

        public ItemPrice()
        {
            item_price_id = 0;
            item_id = 0;
            price_master_id = 0;
            item_money = 0;
            item_cost = 0;
            event_money = 0;
            event_cost = 0;
            apply_id = 0;
            IsPage = false;
        }
    }   
}
