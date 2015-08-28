/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ChannelOrderMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/22 13:38:19 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class ChannelOrderMgr:IChannelOrderImplMgr
    {
        private IChannelOrderImplDao _channelOrderDao;
        public ChannelOrderMgr(string connectionStr)
        {
            _channelOrderDao = new ChannelOrderDao(connectionStr);
        }

        public string Save(BLL.gigade.Model.ChannelOrder channelOrder)
        {
            return _channelOrderDao.Save(channelOrder);
        }
        public List<BLL.gigade.Model.ChannelOrder> Query(BLL.gigade.Model.ChannelOrder query)
        {
            try
            {
                return _channelOrderDao.Query(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelOrderMgr-->Query-->" + ex.Message, ex);
            }
            
        }
        public int Delete(int orderId)
        {
            try
            {
                return _channelOrderDao.Delete(orderId);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelOrderMgr-->Delete-->" + ex.Message, ex);
            }
            
        }
    }
}
