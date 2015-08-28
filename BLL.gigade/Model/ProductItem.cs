/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductItem 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/21 9:03:44 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model
{
    //單一商品實體模型
    [DBTableInfo("product_item")]
    public class ProductItem : PageBase
    {
        public uint Item_Id { get; set; }
        public uint Product_Id { get; set; }
        public uint Spec_Id_1 { get; set; }
        public string Spec_Name_1 { get; set; }
        public uint Spec_Id_2 { get; set; }
        public string Spec_Name_2 { get; set; }
        public uint Item_Cost { get; set; }
        public uint Item_Money { get; set; }
        public uint Event_Product_Start { get; set; }
        public uint Event_Product_End { get; set; }
        public uint Event_Item_Cost { get; set; }
        public uint Event_Item_Money { get; set; }
        public int Item_Stock { get; set; }
        public uint Item_Alarm { get; set; }
        public byte Item_Status { get; set; }
        public string Item_Code { get; set; }
        public string Erp_Id { get; set; }//add by xiangwang0413w 2014/06/18 (增加ERP廠商編號)
        public int Export_flag { get; set; }//add by xiangwang0413w 2014/06/18 (增加ERP匯出標志)
        public string Barcode { get; set; }
        public string Remark { get; set; }// add by zhuoqin0830w 2014/02/05 增加備註
        public int Arrive_Days { get; set; } // add by zhuoqin0830w 2014/03/20 增加運達天數

        public ProductItem()
        {
            Item_Id = 0;
            Product_Id = 0; 
            Spec_Name_1 = string.Empty;
            Spec_Id_1 = 0; 
            Spec_Name_2 = string.Empty;
            Spec_Id_2 = 0;
            Item_Cost = 0;
            Item_Money = 0;
            Event_Product_Start = 0;
            Event_Product_End = 0;
            Event_Item_Cost = 0;
            Event_Item_Money = 0;
            Item_Stock = 0;
            Item_Alarm = 0;
            Item_Status = 0;
            Item_Code = string.Empty;
            Erp_Id = string.Empty;//add by xiangwang0413w 2014/06/18 (增加ERP廠商編號)
            Export_flag = 0;//add by xiangwang0413w 2014/06/18 (增加ERP匯出標志)
            Barcode = string.Empty;
            Remark = string.Empty; // add by zhuoqin0830w 2014/02/05 增加備註
            Arrive_Days = 0;// add by zhuoqin0830w 2014/03/20 增加運達天數
        }

        public string GetSpecName() {
            string result = "";
            if (!string.IsNullOrEmpty(Spec_Name_1)) {
                result = Spec_Name_1;
            }
            if (!string.IsNullOrEmpty(Spec_Name_2))
            {
                result +=" + "+ Spec_Name_2;
            }
            return result;
        }

        //庫存預設
        public void  SetDefaultItemStock(Product product)
        {
            if (product.Prepaid == 0 && (product.Product_Mode == 1 || product.Product_Mode == 3))
                this.Item_Stock = 99;
        }

    }
}
