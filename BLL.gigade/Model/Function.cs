/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：Function 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/1 16:08:45 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Function:PageBase
    {
        public int RowId { get; set; }
        public int FunctionType { get; set; }
        public string FunctionGroup { get; set; }
        public string FunctionName { get; set; }
        public string FunctionCode { get; set; }
        public string IconCls { get; set; }
        public string Remark { get; set; }
        public string Kuser { get; set; }
        public DateTime Kdate { get; set; }
        public int TopValue { get; set; }
        public int IsEdit { get; set; } // add by wwei 2014/12/22 是否可編輯的判斷
        public long UEdit { get; set; }//用于查询时查询出使用者是否有修改权限
        public Int64 Count { get; set; }// add by 2015/4/14 

        public Function()
        {
            RowId = 0;
            FunctionType = 0;
            FunctionGroup = string.Empty;
            FunctionName = string.Empty;
            FunctionCode = string.Empty;
            IconCls = string.Empty;
            Remark = string.Empty;
            Kuser = string.Empty;
            Kdate = DateTime.MinValue;
            TopValue = 0;
            IsEdit = 0;
            Count = 0;
        }
    }
}
