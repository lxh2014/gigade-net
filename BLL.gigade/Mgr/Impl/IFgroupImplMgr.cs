/*
* 文件名稱 :IFgroupImplMgr.cs
* 文件功能描述 :群組
* 版權宣告 :
* 開發人員 : 天娥璇子
* 版本資訊 : 1.0
* 日期 : 2013/08/02
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
    public interface IFgroupImplMgr
    {
        /// <summary>
        /// 查詢
        /// </summary>
        /// <returns></returns>
        string QueryAll();

        string QueryCallid();

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="fg"></param>
        /// <returns></returns>
        int Save(Fgroup fg);

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="fg"></param>
        /// <returns></returns>
        int Delete(Fgroup fg);
        /// <summary>
        /// 查詢該用戶是否有編輯庫存之特權
        /// </summary>
        /// <param name="callid">用戶callid</param>
        /// <param name="xmlPath">config.xml路徑</param>
        /// <returns></returns>
        bool QueryStockPrerogative(string callid, string xmlPath);
        Fgroup GetSingle(Fgroup model);//add by mengjuan0826j 獲取單個model  根據groupname和groupcode 
    }
}
