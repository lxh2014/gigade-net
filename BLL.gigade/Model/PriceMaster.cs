/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：PriceMaster 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/31 18:57:31 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;
using System.Text.RegularExpressions;

namespace BLL.gigade.Model
{
    [DBTableInfo("price_master")]
    public class PriceMaster : PageBase, ICloneable
    {
        public readonly static char L_KH = '(', R_KH = ')', L_HKH = '〖', R_HKH = '〗';

        public uint price_master_id { get; set; }
        public uint product_id { get; set; }
        public uint site_id { get; set; }
        public uint user_level { get; set; }
        public uint user_id { get; set; }
        public string product_name { get; set; }
        public Single bonus_percent { get; set; }
        public Single default_bonus_percent { get; set; }
        public uint bonus_percent_start { get; set; }
        public uint bonus_percent_end { get; set; }
        public int same_price { get; set; }
        public uint event_start { get; set; }
        public uint event_end { get; set; }
        public uint price_status { get; set; }
        public int price { get; set; }
        public int event_price { get; set; }
        public int child_id { get; set; }
        public uint apply_id { get; set; }
        public int cost { get; set; }
        public int event_cost { get; set; }
        public uint accumulated_bonus { get; set; }
        public int max_price { get; set; }
        public int max_event_price { get; set; }
        public int valid_start { get; set; }
        public int valid_end { get; set; }

        public PriceMaster()
        {
            price_master_id = 0;
            product_id = 0;
            site_id = 0;
            user_level = 0;
            user_id = 0;
            product_name = string.Empty;
            bonus_percent = 0;
            default_bonus_percent = 0;
            same_price = 0;
            event_start = 0;
            event_end = 0;
            price_status = 0;
            price = 0;
            event_price = 0;
            child_id = 0;
            cost = 0;
            event_cost = 0;
            accumulated_bonus = 0;
            max_price = 0;
            max_event_price = 0;
            valid_start = 0;
            valid_end = 0;
        }
        /// <summary>
        /// 商品名称组成是：前缀+商品名+prod_sz+后缀
        /// 该方法是将其分割开来用字符隔开"`LM`"(拆分)
        /// </summary>
        /// <returns></returns>
        public static string Product_Name_Op(string productName, string split = "`LM`")
        {
            string QZ = string.Empty;//前缀
            string PN = string.Empty;//商品名
            string HZ = string.Empty;//后缀
            string SZ = string.Empty;//prod_sz

            if (string.IsNullOrEmpty(productName)) {
                return QZ + split + PN + split + SZ + split + HZ;
            }
            //前缀
            if (productName.IndexOf(L_HKH) == 0 && productName.IndexOf(R_HKH) > 0)
            {
                QZ = productName.Substring(productName.IndexOf(L_HKH) + 1, productName.IndexOf(R_HKH) - 1);
                productName = productName.Substring(productName.IndexOf(R_HKH) + 1);
            }
            //商品名
            if (productName.IndexOf(L_KH) > 0 && productName.IndexOf(R_KH) > 0)
            {
                PN = productName.Substring(0, productName.IndexOf(L_KH));
                productName = productName.Substring( productName.IndexOf(L_KH) );
            }
            else if (productName.IndexOf(L_HKH) > 0 && productName.IndexOf(R_HKH) > 0)
            {
                PN = productName.Substring(0, productName.IndexOf(L_HKH) - 1);
                productName = productName.Substring(productName.IndexOf(L_HKH));
            } else {
                PN = productName;
            }
            //prod_sz
            if (productName.IndexOf(L_KH) >= 0 && productName.IndexOf(R_KH) > 0)
            {
                SZ = productName.Substring(productName.IndexOf(L_KH) + 1, productName.IndexOf(R_KH) - 1); ;
                productName = productName.Substring( productName.IndexOf(R_KH) + 1 );
            }
            //后缀
            if (productName.IndexOf(L_HKH) >= 0 && productName.IndexOf(R_HKH) > 0)
            {
                HZ = productName.Substring(productName.IndexOf(L_HKH) + 1, productName.IndexOf(R_HKH) - 1);
            }
            return QZ + split + PN + split + SZ + split + HZ;
        }
        /// <summary>
        /// 将商品名的不同组成部分组成一个商品名(组成)
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
        public static string Product_Name_FM(string productName, string split = "`LM`")
        {
            string result = string.Empty;
            string[] proName = Regex.Split(productName, split, RegexOptions.IgnoreCase);
            if (proName.Length != 4)
                return "";
            if (proName[0] != "")
            {
                result += L_HKH + proName[0] + R_HKH;
            }
            if (proName[1] != "")
            {
                result += proName[1];
            }
            if (proName[2] != "")
            {
                result += L_KH + proName[2] + R_KH;
            }
            if (proName[3] != "")
            {
                result += L_HKH + proName[3] + R_HKH;
            }
            return result;
        } 

        public static bool CheckProdName(string productName){
            if (productName.IndexOf(L_HKH) >= 0 || productName.IndexOf(R_HKH) >= 0 ||
                productName.IndexOf(L_KH) >= 0 || productName.IndexOf(R_KH) >= 0)
            {
                return false;
            }
            else {
                return true;
            }
        }
        public object Clone()
        {
            return (PriceMaster)this.MemberwiseClone();
        }
    }
}
