/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
* All rights reserved. 
*  
* 文件名称：OrderExpectDeliver.cs 
* 摘   要： 預購單信息
*  
* 当前版本：1.0 
* 作   者：shuangshuang0420j 
* 完成日期：2014/10/21 13:24:47 
* 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class  OrderExpectDeliver : PageBase
    {
        public uint expect_id { get; set; }
        public uint order_id { get; set; }
        public uint slave_id { get; set; }
        public uint detail_id { get; set; }
        public uint status { get; set; }
        public uint store { get; set; }
        public string code { get; set; }
        public uint time { get; set; }
        public string note { get; set; }
        public uint createdate { get; set; }
        public uint updatedate { get; set; }
        public OrderExpectDeliver()
        {
            expect_id = 0;
            order_id = 0;
            slave_id = 0;
            detail_id = 0;
            status = 0;
            store = 0;
            code = string.Empty;
            time = 0;
            note = string.Empty;
            createdate = 0;
            updatedate = 0;
        }
    }
}
