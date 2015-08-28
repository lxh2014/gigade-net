#region 文件信息
/* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
* All rights reserved. 
*  
* 文件名称：OrderExpectDeliverQuery.cs 
* 摘   要： 查詢預購單信息所用到的字段
*  
* 当前版本：1.0 
* 作   者：shuangshuang0420j 
* 完成日期：2014/10/21 13:24:47 
* 
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class OrderExpectDeliverQuery : OrderExpectDeliver
    {
        public string note_order { get; set; }
        public string product_name { get; set; }
        public uint detail_status { get; set; }
        public DateTime date_one { get; set; }
        public DateTime date_two { get; set; }
        public DateTime stime { get; set; }
        public uint item_id { get; set; }
        public string order_name { get; set; }
        public string delivery_name { get; set; }
        public string delivery_mobile { get; set; }
        public uint delivery_zip { get; set; }
        public string delivery_address { get; set; }
        public uint buy_num { get; set; }
        public uint single_money { get; set; }
        public uint deduct_account { get; set; }
        public UInt64 sum { get; set; }
        public int query_status { get; set; }
        public string d_status_name { get; set; }
        public string order_address { set; get; }
        public uint deduct_bonus { get; set; }
        public string zip { set; get; }
       
        public OrderExpectDeliverQuery()
        {
            note_order = string.Empty;
            product_name = string.Empty;
            detail_status = 0;
            date_one = DateTime.MinValue;
            date_two = DateTime.MinValue;
            stime = DateTime.MinValue;
            item_id = 0;
            order_name = string.Empty;
            delivery_name = string.Empty;
            delivery_mobile = string.Empty;
            delivery_zip = 0;
            delivery_address = string.Empty;
            buy_num = 0;
            single_money = 0;
            deduct_account = 0;
            sum = 0;
            deduct_bonus = 0;
            order_address = string.Empty;
            query_status = -1;//查詢狀態時所用，-1表所有狀態
            d_status_name = string.Empty;
            zip = string.Empty;
        }
    }
}
