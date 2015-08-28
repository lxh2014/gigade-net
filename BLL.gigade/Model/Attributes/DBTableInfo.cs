/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：DBTableInfo 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/22 13:59:18 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Attributes
{
    public class DBTableInfo:Attribute
    {
        public string DBName { get; set; }
        public string PK_Col { get; set; }

        public DBTableInfo(string _dbName)
        {
            this.DBName = _dbName;
        }
    }
}
