/*
* 文件名稱 :IChannelImplMgr.cs
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

namespace BLL.gigade.Mgr.Impl
{
    public interface IChannelImplMgr
    {
        /// <summary>
        /// 查詢
        /// </summary>
        /// <returns></returns>
        string Query(int status = 0);

        List<Channel> QueryList(int status = 0);

        /// <summary>
        /// 按條件查詢
        /// </summary>
        /// <param name="strSel"></param>
        /// <returns></returns>
        string Query(string strSel);

        /// <summary>
        /// 按條件查詢并分頁
        /// </summary>
        /// <param name="strSel">查詢串</param>
        /// <param name="startPage"></param>
        /// <param name="endPage"></param>
        /// <param name="totalPage"></param>
        /// <returns></returns>
        string DataPager(string strSel, int startPage, int endPage);

        /// <summary>
        /// 查詢合作外站（非Gigade外站）
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        string QueryCooperationSite(int status = 0);

        string QueryOther(int channelid);


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

      
    }
}
