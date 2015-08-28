using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class OrderImportFactory
    {
        public delegate string MySqlConnStr();

        public static event MySqlConnStr GetMySqlConnStr; 

        public static IOrderImport InitOrderImport(int ChannelId)
        {

            IOrderImport orderImport = null;
            ChannelMgr channelMgr = new ChannelMgr(GetMySqlConnStr());
            Channel channel=channelMgr.QueryC(" and channel_id='" + ChannelId + "'").FirstOrDefault();
            if (channel != null)
            {
                switch (channel.model_in)
                {
                    case "1":
                        orderImport = new OrderImportZero();
                        break;
                    case "2":
                        orderImport = new OrderImportGigade();
                        break;
                    case "3":
                        orderImport = new OrderImportPayEasy();  //當類型類PayEasy訂單的時候實例化該類型
                        break;
                }
                if (orderImport != null)
                {
                    orderImport.MySqlConnStr = GetMySqlConnStr();
                    orderImport.CurChannel = channel;
                }
            }
            return orderImport;
        }
    }
}
