/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ChannelOrderDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/22 13:36:03 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    public class ChannelOrderDao : IChannelOrderImplDao
    {
        private IDBAccess _dbAccess;
        public ChannelOrderDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        public string Save(ChannelOrder channelOrder)
        {
            channelOrder.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("insert into channel_order(`channel_id`,`order_id`,`channel_detail_id`,`store_dispatch_file`,`dispatch_seq`,`createtime`");
            strSql.AppendFormat(",`ordertime`,`latest_deliver_date`)values({0},'{1}','{2}'", channelOrder.Channel_Id, channelOrder.Order_Id, channelOrder.Channel_Detail_Id);
            strSql.AppendFormat(",'{0}','{1}',", channelOrder.Store_Dispatch_File, channelOrder.Dispatch_Seq);
            strSql.Append(channelOrder.Createtime == DateTime.MinValue ? "null," : "'" + channelOrder.Createtime.ToString("yyyy/MM/dd HH:mm:ss") + "',");
            strSql.Append(channelOrder.Ordertime == DateTime.MinValue ? "null," : "'" + channelOrder.Ordertime.ToString("yyyy/MM/dd HH:mm:ss") + "',");
            strSql.Append(channelOrder.Latest_Deliver_Date == DateTime.MinValue ? "null)" : "'" + channelOrder.Latest_Deliver_Date.ToString("yyyy/MM/dd HH:mm:ss") + "')");
            return strSql.ToString();
        }
        public List<ChannelOrder> Query(ChannelOrder query)
        {
            query.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("select serial_number,channel_id,order_id,channel_detail_id,store_dispatch_file,dispatch_seq,createtime,ordertime,latest_deliver_date from channel_order where 1=1 ");
            if (!string.IsNullOrEmpty(query.Order_Id))
            {
                strSql.AppendFormat(" and order_id='{0}'", query.Order_Id);
            }
            if (query.Channel_Id != 0)
            {
                strSql.AppendFormat(" and channel_id={0}", query.Channel_Id);
            }
            if (!string.IsNullOrEmpty(query.Channel_Detail_Id))
            {
                strSql.AppendFormat(" and channel_detail_id = '{0}'", query.Channel_Detail_Id);
            }
            return _dbAccess.getDataTableForObj<ChannelOrder>(strSql.ToString());
        }
        public int Delete(int orderId)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;delete from channel_order where order_id='" + orderId + "';set sql_safe_updates=1");
            return _dbAccess.execCommand(strSql.ToString());
        }
    }
}
