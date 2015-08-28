/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：FunctionGroup 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/5 16:02:15 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class FunctionGroup : PageBase
    {
        public int RowId { get; set; }
        public int FunctionId { get; set; }
        public int GroupId { get; set; }
        public DateTime Kdate { get; set; }
        public string Kuser { get; set; }

        public FunctionGroup()
        {
            RowId = 0;
            FunctionId = 0;
            GroupId = 0;
            Kdate = DateTime.MinValue;
            Kuser = string.Empty;
        }
    }
}
