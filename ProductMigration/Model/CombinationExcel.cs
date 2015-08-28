/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：CombinationExcel 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2014/1/14 9:31:37 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Common;

namespace ProductMigration.Model
{
    public class CombinationExcel
    {
        public string msg { get; set; }
        public string brand_name { get; set; }
        public string product_id { get; set; }
        public string temp_id { get; set; }
        public string is_exist { get; set; }
        public string site { get; set; }
        public string product_name { get; set; }
        public string combination { get; set; }
        public string new_old { get; set; }
        public string formula { get; set; }
        public string number { get; set; }
        public string buylimit { get; set; }
        public string status { get; set; }
        public string ignore_stock { get; set; }
        public string shortage { get; set; }
        public string cate_id { get; set; }
        public string display { get; set; }
        public string service_fee { get; set; }
        public string cost { get; set; }
        public string price { get; set; }

        public CombinationExcel()
        {
            msg = string.Empty;
            brand_name = string.Empty;
            product_id = string.Empty;
            temp_id = string.Empty;
            is_exist = string.Empty;
            site = string.Empty;
            product_name = string.Empty;
            combination = string.Empty;
            new_old = string.Empty;
            formula = string.Empty;
            number = string.Empty;
            buylimit = string.Empty;
            status = string.Empty;
            ignore_stock = string.Empty;
            shortage = string.Empty;
            cate_id = string.Empty;
            display = string.Empty;
            service_fee = string.Empty;
            cost = string.Empty;
            price = string.Empty;
        }

        public void Validate()
        {
            if (string.IsNullOrEmpty(brand_name))
            {
                msg = "品牌名稱不為空";
                return;
            }
            if (string.IsNullOrEmpty(product_id))
            {
                msg = "商品編號不為空";
                return;
            }
            if (string.IsNullOrEmpty(site))
            {
                msg = "站臺不為空";
                return;
            }
            if (string.IsNullOrEmpty(new_old))
            {
                msg = "原料編號新舊不能為空";
                return;
            }
            if (string.IsNullOrEmpty(formula))
            {
                msg = "原料編號不為空";
                return;
            }
            if (combination.Trim() != "299" && string.IsNullOrEmpty(number))
            {
                msg = "商品數量不為空";
                return;
            }
            if (combination.Trim() != "2" && combination.Trim() != "299" && string.IsNullOrEmpty(buylimit))
            {
                msg = "僅限每種一單位不為空";
                return;
            }
            if (!string.IsNullOrEmpty(service_fee) && !DataCheck.IsNumeric(service_fee))
            {
                msg = "寄倉費格式錯誤";
                return;
            }
            if (!string.IsNullOrEmpty(cost) && !DataCheck.IsNumeric(cost))
            {
                msg = "成本格式錯誤";
                return;
            }
            if (!string.IsNullOrEmpty(price) && !DataCheck.IsNumeric(price))
            {
                msg = "售價格式錯誤";
                return;
            }
        }
    }
}
