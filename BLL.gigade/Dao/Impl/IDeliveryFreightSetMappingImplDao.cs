/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IDeliveryFreightSetMappingImplDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/21 16:57:01 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    interface IDeliveryFreightSetMappingImplDao
    {
        BLL.gigade.Model.DeliveryFreightSetMapping GetDeliveryFreightSetMapping(BLL.gigade.Model.DeliveryFreightSetMapping query);
    }
}
