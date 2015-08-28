/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：HistoryBatch 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2014/2/6 9:38:00 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class HistoryBatch
    {
        public string batchno { get; set; }
        public int functionid { get; set; }
        public string kuser { get; set; }
        public DateTime kdate { get; set; }
        /// <summary>
        /// 商品分割之後的商品ID reason 用來在移動中排序商品的
        /// </summary>
        public string product_rowid { get; set; }
        public string channel_name_full { get; set; }
        public string channel_detail_id { get; set; }
        public string product_name { get; set; }

        public HistoryBatch()
        {
            batchno = string.Empty;
            functionid = 0;
            kuser = string.Empty;
            kdate = DateTime.MinValue;
            product_rowid = "0";
            channel_name_full = string.Empty;
            channel_detail_id = string.Empty;
            product_name = string.Empty;
        }
    }
}
