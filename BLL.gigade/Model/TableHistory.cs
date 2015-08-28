/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：TableHistory 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/22 11:38:28 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class TableHistory:PageBase
    {
        public int rowid { get; set; }
        public string table_name { get; set; }
        public int functionid { get; set; }
        public string pk_name { get; set; }
        public string pk_value { get; set; }
        public string batchno { get; set; }

        public TableHistory()
        {
            rowid = 0;
            table_name = string.Empty;
            functionid = 0;
            pk_name = string.Empty;
            pk_value = string.Empty;
            batchno = string.Empty;
        }
        public static string FunctionDescription(string tableName)
        {
            string desc = string.Empty;
            switch (tableName)
            {
                case "product":
                    desc = "商品信息";
                    break;
                case "product_item":
                    desc = "單一商品細項信息";
                    break;
                case "price_master":
                    desc = "商品價格信息";
                    break;
                case "item_price":
                    desc = "單一商品價格細項信息";
                    break;
                default:
                    break;
            }
            return desc;
        }

    }
}
