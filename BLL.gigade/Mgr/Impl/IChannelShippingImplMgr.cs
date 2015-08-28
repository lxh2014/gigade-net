/*
* 文件名稱 :IChannelShippingImplMgr.cs
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
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Mgr.Impl
{
    public interface IChannelShippingImplMgr
    {
        /// <summary>
        /// 查詢
        /// </summary>
        /// <returns>返回Json串</returns>
        string Query(string strChannelId);

        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="channelShipping"></param>
        /// <returns>返回實體集合</returns>
        List<ChannelShipping> Query(ChannelShipping channelShipping);

        /// <summary>
        /// 查詢指定外站的物流模式
        /// </summary>
        /// <param name="strChannelId"></param>
        /// <returns></returns>
        string QueryCarry(string strChannelId);

        DataTable Query(int strChannelId, int shippingcarrior);

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="chc"></param>
        /// <returns></returns>
        int Save(ChannelShipping chs);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="chc"></param>
        /// <returns></returns>
        int Edit(ChannelShipping chs, int shippingCarrior,string shipco_content);

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="ChannelId"></param>
        /// <param name="ShippingCarrior"></param>
        /// <returns></returns>
        int Delete(int ChannelId, int ShippingCarrior);
    }
}
