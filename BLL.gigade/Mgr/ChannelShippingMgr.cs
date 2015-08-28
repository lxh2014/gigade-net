/*
* 文件名稱 :ChannelShippingMgr.cs
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
using BLL.gigade.Model;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Mgr
{
    public class ChannelShippingMgr:IChannelShippingImplMgr
    {
        private IChannelShippingImplDao _chsDao;

        public ChannelShippingMgr(string connectionString)
        {
            _chsDao = new ChannelShippingDao(connectionString);
        }

        #region IChannelShippingImplMgr 成员

        public string Query(string strChannelId)
        {
            try
            {
                List<ChannelShipping> chsResult = _chsDao.Query(new ChannelShipping { channel_id = Convert.ToInt32(strChannelId) });
                StringBuilder stb = new StringBuilder();

                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                foreach (ChannelShipping chs in chsResult)
                {
                    stb.Append("{");
                    //stb.Append(string.Format("\"channel_id\":\"{0}\",\"shipping_carrior\":\"{1}\",\"shipping_carrior_content\":\"{1}\",\"shipping_type\":\"{2}\",\"shipping_type_content\":\"{2}\",\"threshold\":\"{3}\",\"fee\":\"{4}\",\"return_fee\":\"{5}\",\"ship_logistics\":\"{6}\",\"ship_logistics_content\":\"{6}\"", chs.channel_id, chs.shipping_carrior, chs.shipping_type, chs.threshold, chs.fee, chs.return_fee, chs.ship_logistics));
                    stb.Append(string.Format("\"channel_id\":\"{0}\",\"shipping_carrior\":\"{1}\",\"shipping_carrior_content\":\"{1}\",\"n_threshold\":\"{2}\",\"l_threshold\":\"{3}\",\"n_fee\":\"{4}\",\"l_fee\":\"{5}\",\"n_return_fee\":\"{6}\",\"l_return_fee\":\"{7}\",\"shipco\":\"{8}\",\"retrieve_mode\":\"{9}\",\"shipco_content\":\"{8}\"", chs.channel_id, chs.shipping_carrior, chs.n_threshold, chs.l_threshold, chs.n_fee, chs.l_fee, chs.n_return_fee, chs.l_return_fee, chs.shipco, chs.retrieve_mode));
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelShippingMgr-->Query(string strChannelId)-->" + ex.Message, ex);
            }
        }

        public List<ChannelShipping> Query(ChannelShipping channelShipping)
        {
            try
            {
                return _chsDao.Query(channelShipping);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelShippingMgr-->Query(ChannelShipping channelShipping)-->" + ex.Message, ex);
            }
            
        }


        public string QueryCarry(string strChannelId)
        {
            try
            {
                List<ChannelShippingCustom> chsResult = _chsDao.QueryCarry(strChannelId);
                StringBuilder stb = new StringBuilder();
                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                foreach (ChannelShippingCustom chs in chsResult)
                {
                    stb.Append("{");
                    stb.Append(string.Format("\"channel_id\":\"{0}\",\"shipping_carrior\":\"{1}\",\"shipco\":\"{2}\",\"sort\":\"{3}\",\"retrieve_mode\":\"{4}\"", chs.channel_id, chs.shipping_carrior, chs.shipco, chs.sort, chs.retrieve_mode));
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelShippingMgr-->QueryCarry-->" + ex.Message, ex);
            }

        }

        public int Save(ChannelShipping chs)
        {
            
            try
            {
                return _chsDao.Save(chs);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelShippingMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
        }

        public int Edit(ChannelShipping chs, int shippingCarrior,string shipco_content)
        {
            
            try
            {
                return _chsDao.Edit(chs, shippingCarrior, shipco_content);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelShippingMgr-->Edit-->" + ex.Message, ex);
            }
        }

        public int Delete(int ChannelId, int ShippingCarrior)
        {
            
            try
            {
                return _chsDao.Delete(ChannelId, ShippingCarrior);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelShippingMgr-->Delete-->" + ex.Message, ex);
            }
        }

        public System.Data.DataTable Query(int strChannelId, int shippingcarrior)
        {
            
            try
            {
                return _chsDao.Query(strChannelId, shippingcarrior);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelShippingMgr-->Query(int strChannelId, int shippingcarrior)-->" + ex.Message, ex);
            }
        }

        #endregion

        public List<ChannelShipping> QueryByChannelId(ChannelShipping channelShipping)
        {
            try
            {
                return _chsDao.Query(channelShipping);
            }
            catch (Exception ex)
            {
                throw new Exception("ChannelShippingMgr-->QueryByChannelId-->" + ex.Message, ex);
            }
        }
    }
}
