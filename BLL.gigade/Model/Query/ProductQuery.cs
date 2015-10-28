
/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：ProductQuery 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：shuangshuang0420j 
 * 完成日期：2014/10/20 13:48:16 
 *  
 * chaojie1124j 于2014/11/18添加六個字段vendor_name_simple，upc_id，category_name,loc_id,prod_sz,prod_qty。用於完成功能<無主料位商品報表>
 *  mengjuan0826j 于2014/11/19添加字段siteStr用於查詢站台下面的類別商品
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ProductQuery : Product
    {
        public bool isjoincate { get; set; }//判斷是否要鏈接categoryset表
        public uint category_id { get; set; }
        public string brandArry { get; set; }
        public string categoryArry { get; set; }
        public string siteStr { get; set; }//站台商品
        public string vendor_name_simple { get; set; }//廠商名稱
        public string upc_id { get; set; }//商品條碼
        public string category_name { get; set; }//商品分類
        public string loc_id { set; get; }//主料位
        public string prod_sz { set; get; }//商品規格，，500ml  
        public int product_qty { set; get; }//商品庫存
        public uint item_id { set; get; }//商品庫存
        public int inner_pack_len{get;set;}
        public int inner_pack_wid{get;set;}
        public int inner_pack_hgt{get;set;}
        public string Product_Id_In { get; set; }
        public string searchcontent { get; set; }
        public int this_product_state { get; set; }
        public int vendor_status { get; set; }//供應商狀態 add by jiaohe0625j
        public int brand_status { get; set; }//品牌狀態 add by jiaohe0625j

        public string create_username { get; set; }//存儲建立人姓名 add by shuangshuang0420j 20150513 13:37
        public DateTime time_start { get; set; }//存儲查詢開始時間 add by shuangshuang0420j 20150513 13:37
        public DateTime time_end { get; set; }//存儲查詢結束時間 add by shuangshuang0420j 20150513 13:37

        public int item_stock { set; get; }//前台庫存量chaojie1124j 2015/10/27
        public string iinvd_stock { set; get; }//後台庫存量chaojie1124j 2015/10/27
        public string product_status_string { set; get; }//商品狀態chaojie1124j 2015/10/27
        public int product_freight { set; get; }//溫層delivery_freight_set_mapping
        public string loc_id2 { set; get; }//走道範圍，料位範圍chaojie1124j 2015/10/27
        public ProductQuery()
        {
            isjoincate = false;
            category_id = 0;
            brandArry = string.Empty;
            categoryArry = string.Empty;
            siteStr = string.Empty;
            vendor_name_simple = string.Empty;
            upc_id = string.Empty;
            category_name = string.Empty;//站台商品
            loc_id = string.Empty;
            prod_sz = string.Empty;
            product_qty =0;
            item_id = 0;
            inner_pack_len=0;
            inner_pack_wid=0;
            inner_pack_hgt=0;
            Product_Id_In = string.Empty;
            searchcontent = string.Empty;
            this_product_state = -1;//-1表示所有狀態

            create_username = string.Empty;
            time_start =DateTime.MinValue ;
            time_end = DateTime.MinValue;

            vendor_status = 0;//供應商狀態
            brand_status = 0;//品牌狀態
            item_stock = 0;
            iinvd_stock = "0";
            product_status_string = string.Empty;
            product_freight = 0;
            loc_id2 = string.Empty;
        }
    }
}
