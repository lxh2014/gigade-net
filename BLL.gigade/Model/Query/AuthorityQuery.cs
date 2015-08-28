/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：AuthorityQuery 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/2 16:39:13 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class AuthorityQuery
    {
        public int RowId { get; set; }
        public int Type { get; set; }
        public string CallId { get; set; }
        public int GroupId { get; set; }

        public AuthorityQuery()
        {
            RowId = 0;
            Type = 0;
            CallId = string.Empty;
            GroupId = 0;
        }
    }
}
