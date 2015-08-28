/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IChannelOrderImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/22 13:37:54 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    interface IChannelOrderImplMgr
    {
        string Save(BLL.gigade.Model.ChannelOrder channelOrder);
        List<BLL.gigade.Model.ChannelOrder> Query(BLL.gigade.Model.ChannelOrder query);
        int Delete(int orderId);
    }
}
