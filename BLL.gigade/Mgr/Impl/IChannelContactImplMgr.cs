/*
* 文件名稱 :IChannelContactImplMgr.cs
* 文件功能描述 :外站聯絡人
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

namespace BLL.gigade.Mgr.Impl
{
    public interface IChannelContactImplMgr
    {
        /// <summary>
        /// 查詢
        /// </summary>
        /// <returns></returns>
        string Query(string strChannelId);

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="chc"></param>
        /// <returns></returns>
        int Save(ChannelContact chc);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="chc"></param>
        /// <returns></returns>
        int Edit(ChannelContact chc);

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="chc"></param>
        /// <returns></returns>
        int Delete(int rid);
    }
}
