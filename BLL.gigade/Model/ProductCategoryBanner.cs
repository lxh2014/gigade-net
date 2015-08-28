#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：ProductCategoryBanner.cs
* 摘 要：
* 專區商品類別設置
* 当前版本：v1.0
* 作 者： shuangshuang0420j
* 完成日期：2014/12/30 
* 修改歷史：
*         
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductCategoryBanner : PageBase
    {
        public int row_id { get; set; }
        public uint banner_cateid { get; set; }//專區類別id
        public uint category_id { get; set; }
        public uint category_father_id { get; set; }
        public string category_name { get; set; }
        public uint category_sort { get; set; }//排序
        public uint category_display { get; set; }//是否顯示 1：顯示 0：隱藏
        public uint category_link_mode { get; set; }//連接方式 1：原視窗 2：新視窗
        public uint createdate { get; set; }
        public uint updatedate { get; set; }
        public string create_ipfrom { get; set; }//創建ip
        public int status { get; set; }//狀態 1：啟用 0：禁用
        public ProductCategoryBanner()
        {
            row_id = 0;
            banner_cateid = 0;
            category_id = 0;
            category_father_id = 0;
            category_name = string.Empty;
            category_sort = 0;
            category_display = 0;
            category_link_mode = 0;
            createdate = 0;
            updatedate = 0;
            create_ipfrom = string.Empty;
            status = 0;


        }
    }
}
