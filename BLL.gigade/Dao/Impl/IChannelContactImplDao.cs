/*
* 文件名稱 :IChannelContactImplDao.cs
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

namespace BLL.gigade.Dao.Impl
{
    interface IChannelContactImplDao
    {
        /// <summary>
        /// 查詢
        /// </summary>
        /// <returns></returns>
        List<ChannelContact> Query(string strChannelId);

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
