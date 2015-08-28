/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ImportOrdersLog 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/9/14 9:15:17 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ImportOrdersLog : PageBase
    {
        public int RId { get; set; }
        public int Channel_Id { get; set; }
        public int TCount { get; set; }
        public int Success_Count { get; set; }
        public string File_Name { get; set; }
        public DateTime Import_Date { get; set; }
        public string Exec_Name { get; set; }

        public ImportOrdersLog()
        {
            RId = 0;
            Channel_Id = 0;
            TCount = 0;
            Success_Count = 0;
            File_Name = string.Empty;
            Import_Date = DateTime.MinValue;
            Exec_Name = string.Empty;
        }
    }
}
