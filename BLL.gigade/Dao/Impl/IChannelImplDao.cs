/*
* 文件名稱 :IChannelImplDao.cs
* 文件功能描述 :外站資訊表
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
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    interface IChannelImplDao
    {
        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="status">狀態，默認為全部，1：啟用，2:停用</param>
        /// <returns></returns>
        List<Channel> Query(int status = 0);        

        /// <summary>
        /// 按條件查詢
        /// </summary>
        /// <param name="strSel"></param>
        /// <returns></returns>
        List<Channel> Query(string strSel);

        List<Channel> DataPager(string strSel,int startPage,int endPage,ref int totalPage );

        /// <summary>
        /// 查詢合作外站（非Gigade外站）
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        List<Channel> QueryCooperationSite(int status = 0);

        List<Channel> QueryOther(int channelid);

        /// <summary>
        /// 查詢會員是否已存在
        /// </summary>
        /// <param name="strUserID"></param>
        /// <returns></returns>
        DataTable QueryUser(string strUserID);

        /// <summary>
        /// 保存基本資料
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        int Save(Channel ch);

        /// <summary>
        /// 保存其他諮詢
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        int SaveOther(Channel ch);

        /// <summary>
        /// 修改基本資料
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        int Edit(Channel ch);

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        int Delete(Channel ch);

        int GetUserIdByChannelId(int channelId);

        Channel getSingleObj(int id);
        
    }
}
