#region 文件信息
/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：VendorLoginQuery.cs 
 * 摘   要： 
 *       供應商管理-->供應商登入記錄
 * 当前版本：v1.1 
 * 作   者： changjian0408j
 * 完成日期：2014/10/7
 */

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public class VendorLoginQuery:VendorLogin
    {public string username { get; set; }
        public DateTime serchstart { get; set; }
        public DateTime serchend { get; set; }
        public DateTime slogin_createdate { get; set; }
        public string vendor_code { get; set; }
        public VendorLoginQuery()
        {
            username = string.Empty;
            serchstart = DateTime.MinValue;
            serchend = DateTime.MinValue;
            slogin_createdate = DateTime.MinValue;
            vendor_code = string.Empty;
        }
    }
}
