/*
* 文件名稱 :ChannelShippingDao.cs
* 文件功能描述 :外站運費設定
* 版權宣告 :
* 開發人員 : 天娥璇子
* 版本資訊 : 1.0
* 日期 : 2013/08/19
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using System.Data;
using System.Data.SqlClient;
using BLL.gigade.Model.Custom;
namespace BLL.gigade.Dao
{
    class ChannelShippingDao : IChannelShippingImplDao
    {
        private IDBAccess _accessMySql;
        string strSql = string.Empty;

        public ChannelShippingDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        #region IChannelShippingImplDao 成员

        public List<ChannelShipping> Query(ChannelShipping channelShipping)
        {
            channelShipping.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("select channel_id,shipping_carrior,shipco,n_threshold,n_fee,n_return_fee,l_threshold,l_fee,");
            strSql.Append("l_return_fee,createdate,updatedate,retrieve_mode from channel_shipping where 1=1 ");

            if (channelShipping.channel_id != 0)
            {
                strSql.AppendFormat(" and channel_id={0}", channelShipping.channel_id);
            }
            if (!string.IsNullOrEmpty(channelShipping.shipco))
            {
                strSql.AppendFormat(" and shipco='{0}'", channelShipping.shipco);
            }
            if (channelShipping.shipping_carrior != 0)
            {
                strSql.AppendFormat(" and shipping_carrior={0}",channelShipping.shipping_carrior);
            }

            return _accessMySql.getDataTableForObj<ChannelShipping>(strSql.ToString());
        }

        public List<ChannelShippingCustom> QueryCarry(string strChannelId)
        {
            strSql = string.Format(@"select distinct channel_id,shipping_carrior,shipco,p.sort,retrieve_mode from channel_shipping c inner join 
                                    (select parametercode,parametername,sort from t_parametersrc where parametertype='deliver_store') p on c.shipping_carrior = p.parametercode 
                                                 where channel_id ={0} ", strChannelId);

            return _accessMySql.getDataTableForObj<ChannelShippingCustom>(strSql);
        }

        public int Save(ChannelShipping chs)
        {
            chs.Replace4MySQL();
            strSql = string.Format(@"insert into channel_shipping(channel_id,shipping_carrior,n_threshold,l_threshold,n_fee,l_fee,n_return_fee,l_return_fee,shipco,retrieve_mode,createdate,updatedate) 
                values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')", chs.channel_id, chs.shipping_carrior, chs.n_threshold, chs.l_threshold, chs.n_fee, chs.l_fee, chs.n_return_fee, chs.l_return_fee, chs.shipco, chs.retrieve_mode, chs.createdate.ToString("yyyy/MM/dd hh:mm:ss"), chs.updatedate.ToString("yyyy/MM/dd hh:mm:ss"));

            return _accessMySql.execCommand(strSql);
        }

        public int Edit(ChannelShipping chs, int shippingCarrior,string shipco_content)
        {
            chs.Replace4MySQL();
            shipco_content = Common.CommonFunction.StringTransfer(shipco_content);
            strSql = string.Format(@"update channel_shipping set shipping_carrior='{0}',n_threshold='{1}',l_threshold='{2}',n_fee='{3}',l_fee='{4}',n_return_fee='{5}',l_return_fee='{6}',shipco='{7}',retrieve_mode='{10}',updatedate = '{11}'
                where channel_id='{8}' and shipping_carrior='{9}' and shipco='{12}'", chs.shipping_carrior, chs.n_threshold, chs.l_threshold, chs.n_fee, chs.l_fee, chs.n_return_fee, chs.l_return_fee,
                chs.shipco, chs.channel_id, shippingCarrior, chs.retrieve_mode, chs.updatedate.ToString("yyyy/MM/dd hh:mm:ss"), shipco_content);

            return _accessMySql.execCommand(strSql);
        }

        public int Delete(int channelId, int shippingCarrior)
        {
            strSql = string.Format(@"delete from channel_shipping where channel_id='{0}' and shipping_carrior='{1}'", channelId, shippingCarrior);
            return _accessMySql.execCommand(strSql);
        }

        public DataTable Query(int strChannelId, int shippingcarrior)
        {
            strSql = string.Format(@"select channel_id from channel_shipping where channel_id='{0}' and shipping_carrior='{1}'", strChannelId, shippingcarrior);
            return _accessMySql.getDataTable(strSql);
        }

        #endregion
    }
}
