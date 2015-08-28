/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：Vendor 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/21 16:27:13 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class VendorBrand : PageBase
    {
        public uint Brand_Id { get; set; }
        public uint Vendor_Id { get; set; }
        public string Brand_Name { get; set; }
        public uint Brand_Sort { get; set; }
        public uint Brand_Status { get; set; }
        public string Image_Name { get; set; }
        public uint Image_Status { get; set; }
        public uint Image_Link_Mode { get; set; }
        public string Image_Link_Url { get; set; }
        public string Media_Report_Link_Url { get; set; }
        public string Brand_Msg { get; set; }
        public uint Brand_Msg_Start_Time { get; set; }
        public uint Brand_Msg_End_Time { get; set; }
        public uint Brand_Createdate { get; set; }
        public uint Brand_Updatedate { get; set; }
        public string Brand_Ipfrom { get; set; }
        public uint Cucumber_Brand { get; set; }
        public uint Event { get; set; }
        public string Promotion_Banner_Image { get; set; }
        public string Resume_Image { get; set; }
        public string Promotion_Banner_Image_Link { get; set; }
        public string Resume_Image_Link { get; set; }
        public string Brand_Story_Text { get; set; }//品牌故事
        public int Story_Created { get; set; }//品牌故事創建者
        public DateTime Story_Createdate { get; set; }
        public int Story_Update { get; set; }//品牌故事創建者
        public DateTime Story_Updatedate { get; set; }

        public VendorBrand()
        {
            Brand_Id = 0;
            Vendor_Id = 0;
            Brand_Name = string.Empty;
            Brand_Sort = 0;
            Brand_Status = 0;
            Image_Name = string.Empty;
            Image_Status = 0;
            Image_Link_Mode = 0;
            Image_Link_Url = string.Empty;
            Media_Report_Link_Url = string.Empty;
            Brand_Msg = string.Empty;
            Brand_Msg_Start_Time = 0;
            Brand_Msg_End_Time = 0;
            Brand_Createdate = 0;
            Brand_Updatedate = 0;
            Brand_Ipfrom = string.Empty;
            Cucumber_Brand = 0;
            Event = 0;
            Promotion_Banner_Image = string.Empty;
            Resume_Image = string.Empty;
            Promotion_Banner_Image_Link = string.Empty;
            Resume_Image_Link = string.Empty;
            Brand_Story_Text = string.Empty;
            Story_Created = 0;
            Story_Createdate = DateTime.MinValue;
            Story_Update = 0;
            Story_Updatedate = DateTime.MinValue;
        }
    }
}
