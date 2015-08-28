/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ISerialImplDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/23 9:41:23 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    interface ISerialImplDao
    {
        Serial GetSerialById(int serialId);
        int Update(Serial serial);
        string Update(int seriaId);

        string UpdateAutoIncreament(Serial serial);
        int Insert(Serial serial);
    }
}
