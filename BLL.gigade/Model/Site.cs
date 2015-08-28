/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：Site 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/25 10:49:35 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Site : PageBase
    {
        public uint Site_Id { get; set; }//流水號，預設1為吉甲地
        public string Site_Name { get; set; }//站台名稱
        public string Domain { get; set; }
        public uint Cart_Delivery { get; set; }
        public int Online_User { get; set; }//站台人數
        public int Max_User { get; set; }//最大人數
        public string Page_Location { get; set; }//站台首頁
        public int Site_Status { get; set; }
        public DateTime Site_Createdate { set; get; }
        public DateTime Site_Updatedate { set; get; }
        public int Create_Userid { set; get; }
        public int Update_Userid { set; get; }

        public Site()
        {
            Site_Id = 0;
            Site_Name = string.Empty;
            Domain = string.Empty;
            Cart_Delivery = 1;
            Online_User = 0;
            Max_User = 0;
            Page_Location = string.Empty;
            Site_Status = 0;
            Site_Createdate = DateTime.Now;
            Site_Updatedate = DateTime.Now;
            Create_Userid = 0;
            Update_Userid = 0;


        }
    }
}
