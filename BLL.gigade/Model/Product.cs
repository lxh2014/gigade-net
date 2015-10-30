/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：Product 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/21 9:19:48 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model
{
    [DBTableInfo("product")]
    public class Product : PageBase
    {
        public static readonly char L_KH = PriceMaster.L_KH, R_KH = PriceMaster.R_KH;

        public uint Product_Id { get; set; }
        public uint Brand_Id { get; set; }
        public string Product_Vendor_Code { get; set; }
        /// <summary>
        /// Prod_Name+Prod_Sz
        /// </summary>
        public string Product_Name { get; set; }
        private string prod_name;
        /// <summary>
        /// 商品名稱
        /// </summary>
        public string Prod_Name
        {
            get { return string.IsNullOrEmpty(prod_name) ? Product_Name.Replace(L_KH + Prod_Sz + R_KH, string.Empty) : prod_name; }
            set { prod_name = value; } 
        }
        /// <summary>
        /// 規格欄位
        /// </summary>
        public string Prod_Sz { get; set; }
        public uint Product_Price_List { get; set; }
        public uint Product_Spec { get; set; }
        public string Spec_Title_1 { get; set; }
        public string Spec_Title_2 { get; set; }
        public uint Product_Freight_Set { get; set; }
        public uint Product_Buy_Limit { get; set; }
        public uint Product_Status { get; set; }
        public bool Product_Hide { get; set; }
        public uint Product_Mode { get; set; }
        public uint Product_Sort { get; set; }
        public uint Product_Start { get; set; }
        public uint Product_End { get; set; }
        public string Page_Content_1 { get; set; }
        public string Page_Content_2 { get; set; }
        public string Page_Content_3 { get; set; }
        public string Product_Keywords { get; set; }
        public uint Product_Recommend { get; set; }
        public string Product_Password { get; set; }
        public uint Product_Total_Click { get; set; }
        public uint Expect_Time { get; set; }
        public string Product_Image { get; set; }
        public uint Product_Createdate { get; set; }
        public uint Product_Updatedate { get; set; }
        public string Product_Ipfrom { get; set; }
        public uint Goods_Area { get; set; }
        public string Goods_Image1 { get; set; }
        public string Goods_Image2 { get; set; }
        public string City { get; set; }
        public uint Bag_Check_Money { get; set; }
        public uint Combination { get; set; }
        public Single Bonus_Percent { get; set; }
        public Single Default_Bonus_Percent { get; set; }
        public uint Bonus_Percent_Start { get; set; }
        public uint Bonus_Percent_End { get; set; }
        public int Tax_Type { get; set; }
        public string Cate_Id { get; set; }
        public uint Fortune_Quota { get; set; }
        public uint Fortune_Freight { get; set; }
        public string product_media { get; set; }
        public int Ignore_Stock { get; set; }
        public int Shortage { get; set; }
        public int stock_alarm { get; set; }
        public int Price_type { get; set; }
        public uint user_id { get; set; }
        public uint show_listprice { get; set; }
        public string expect_msg { get; set; }
        /// <summary>
        /// //建立人，1-後台管理人員，2-供應商
        /// </summary>
        public int Create_Channel { get; set; }
        /// <summary>
        /// 顯示於出貨單中 1:是  0:否
        /// </summary>
        public int Show_In_Deliver { get; set; }
        /// <summary>
        /// 已買斷的商品  0:否  1:是
        /// </summary>
        public int Prepaid { get; set; }
        /// <summary>
        /// 配送系統  0:物流配送, 1:電子郵件, 2:簡訊, 99:系統
        /// </summary>
        public int Process_Type { get; set; }
        /// <summary>
        /// 商品型態  0:實體商品,1:課程,2:旅遊,3:電子票券,91:購物金,92:抵用卷
        /// </summary>
        public int Product_Type { get; set; }
        /// <summary>
        /// 販售狀態  0:正常可顯示,11:有異常可顯示,12:有異常可顯示,13:有異常可顯示,21:有異常不可顯示,22:有異常不可顯示,23:有異常不可顯示,24:有異常不可顯示,25:有異常不可顯示
        /// </summary>
        public int Sale_Status { get; set; }
        /// <summary>
        /// 供應商ID
        /// </summary>
        public uint Vendor_Id { get; set; } 
        /// <summary>
        /// 是否可編輯
        /// </summary>
        public bool IsEdit { get; set; }
        /// <summary>
        /// 館別 食品館:10 用品館:20
        /// </summary>
        public int Prod_Classify { get; set; } // add by wwei02156w 2015/1/13
        /// <summary>
        /// 課程id
        /// </summary>
        public int Course_Id { get; set; }

        //add by zhuoqin0830w  增加5個欄位
        /// <summary>
        /// 供應商出貨天數
        /// </summary>
        public int Deliver_Days { get; set; }
        /// <summary>
        /// 最小採購數量
        /// </summary>
        public int Min_Purchase_Amount { get; set; }
        /// <summary>
        /// 安全存量細數
        /// </summary>
        public double Safe_Stock_Amount{get;set;}
        /// <summary>
        /// 寄倉天數/調度天數
        /// </summary>
        public int Extra_Days { get; set; }

        public string Mobile_Image { get; set; } //手機說明圖檔路徑
        public int Schedule_Id { get; set; }//排程id
        public string Product_alt { get; set; }//商品圖片說明
        public string Desc { get; set; } // add by wwei0216w 2015/4/22 排程文字說明

        public string product_detail_text { get; set; }//商品詳情
        public string pids { get; set; }

        public int detail_created { get; set; }//商品詳情文字 add by shuangshuang0420j 2015/4/13 
        public int detail_update { get; set; }//商品詳情文字 add by shuangshuang0420j 2015/4/13 
        public DateTime detail_updatedate { get; set; }//商品詳情文字 add by shuangshuang0420j 2015/4/13 
        public DateTime detail_createdate { get; set; }//商品詳情文字 add by shuangshuang0420j 2015/4/13

        public int off_grade { get; set; }// add by wwei0216w 2015/7/2 添加`失格`欄位

        public int purchase_in_advance { get; set; }///add by wwei0216w 2015/7/30 是否預購
        public uint purchase_in_advance_start { get; set; }///add by wwei0216w 2015/7/30 預購開始時間
        public uint purchase_in_advance_end { get; set; }///add by wwei0216w 2015/7/30 預購結束時間

        public uint recommedde_jundge { get; set; }
        public uint Recommended_time_end { get; set; }
        public uint Recommended_time_start { get; set; }
        public uint expend_day { get; set; }
        public string months { get; set; }
        public int combotype { get; set; }
        public int outofstock_days_stopselling { get; set; }
        public Product()
        {
            Product_Id = 0;
            Brand_Id = 0;
            Product_Vendor_Code = string.Empty;
            Product_Name = string.Empty;
            Prod_Sz = string.Empty;
            Product_Price_List = 0;
            Product_Spec = 0;
            Spec_Title_1 = string.Empty;
            Spec_Title_2 = string.Empty;
            Product_Freight_Set = 0;
            Product_Buy_Limit = 0;
            Product_Status = 0;
            Product_Hide = false;
            Product_Mode = 0;
            Product_Sort = 0;
            Product_Start = 0;
            Product_End = 0;
            Page_Content_1 = string.Empty;
            Page_Content_2 = string.Empty;
            Page_Content_3 = string.Empty;
            Product_Keywords = string.Empty;
            Product_Recommend = 0;
            Product_Password = string.Empty;
            Product_Total_Click = 0;
            Expect_Time = 0;
            Product_Image = string.Empty;
            Product_Createdate = 0;
            Product_Updatedate = 0;
            Product_Ipfrom = string.Empty;
            Goods_Area = 0;
            Goods_Image1 = string.Empty;
            Goods_Image2 = string.Empty;
            City = string.Empty;
            Bag_Check_Money = 0;
            Combination = 1;
            Bonus_Percent = 0;
            Default_Bonus_Percent = 0;
            Bonus_Percent_Start = 0;
            Bonus_Percent_End = 0;
            Tax_Type = 0;
            Cate_Id = string.Empty;
            Fortune_Quota = 0;
            Fortune_Freight = 0;
            Ignore_Stock = 0;
            Shortage = 0;
            stock_alarm = 0;
            Price_type = 0;
            user_id = 0;
            product_media = string.Empty;
            show_listprice = 0;
            expect_msg = string.Empty;
            Create_Channel = 0;

            Show_In_Deliver = 0;
            Prepaid = 0;
            Process_Type = 0;
            Product_Type = 0;
            Sale_Status = 0;
            Vendor_Id = 0;
            IsEdit = false;
            Prod_Classify = 0;
            Course_Id = 0;

            Deliver_Days = 0;
            Min_Purchase_Amount = 0;
            Safe_Stock_Amount = 0;
            Extra_Days = 0;
            Mobile_Image = string.Empty;
            Schedule_Id = 0;
            Product_alt = string.Empty;
            Desc = string.Empty;
            product_detail_text = string.Empty;
            pids = string.Empty;

            detail_created = 0;
            detail_createdate = DateTime.MinValue;
            detail_update = 0;
            detail_updatedate = DateTime.MinValue;
            off_grade = 0;//0:正常1:失格:-1:組合商品(暫不考慮)
            purchase_in_advance = 0;
            purchase_in_advance_start =0;
            purchase_in_advance_end = 0;
            ///add by wwei0216w 20215/7/30 添加預購3欄位
            ///
            recommedde_jundge = 0;
            Recommended_time_end = 0;
            Recommended_time_start = 0;
            expend_day = 0;
            months = string.Empty;
            combotype = 0;
            outofstock_days_stopselling = 15;
        }

        /// <summary>
        /// 讀取商品名稱:prod_name+prod_sz
        /// </summary>
        /// <returns></returns>
        public string GetProductName()
        {
            if (this.Prod_Sz == "")
            {
                return this.Prod_Name;
            }
            else
            {
                return this.Prod_Name + Product.L_KH + this.Prod_Sz + Product.R_KH;
            }
        }

        /// <summary>
        /// 檢查是否符合設定本島店配的條件
        /// </summary>
        /// <returns></returns>
        public bool CheckdStoreFreight()
        {
            return this.Product_Freight_Set == 1 && this.Product_Mode == 2 /*&& this.Combination == 1*/;
        }

        public static bool CheckProdName(string productName)
        {
           return PriceMaster.CheckProdName(productName);
        }
    }
}
