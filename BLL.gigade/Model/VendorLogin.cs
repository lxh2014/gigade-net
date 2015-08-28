#region 文件信息
/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：VendorLogin.cs 
 * 摘   要： 
 *      供應商管理-->供應商登入記錄
 * 当前版本：v1.1 
 * 作   者： changjian0408j
 * 完成日期：2014/10/7
 */
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class VendorLogin:PageBase
    {
        public uint login_id { get; set; }
        public uint vendor_id { get; set; }
        public string login_ipfrom { get; set; }
        public uint login_createdate { get; set; }
        public VendorLogin()
        {
            login_ipfrom = string.Empty;
            login_createdate = 0;
        }
    }
    
}
