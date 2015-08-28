/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ISerialImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/23 9:42:59 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface ISerialImplMgr
    {
        Model.Serial GetSerialById(int serialId);
        int Update(BLL.gigade.Model.Serial serial);
        string Update(int seriaId);
        ulong NextSerial(int serialId);
        int Insert(Serial serial);
    }
}
