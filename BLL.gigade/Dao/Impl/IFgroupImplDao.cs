/*
* 文件名稱 :IFgroupImplDao.cs
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
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    interface IFgroupImplDao
    {
        /// <summary>
        /// 查詢
        /// </summary>
        /// <returns></returns>
        List<Fgroup> QueryAll();

        List<Fgroup> Query(string callid, string groupCode);

        List<ManageUser> QueryCallid();

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

        DataTable GetFgroupList();

        DataTable GetUsersByGroupId(int groupid);

        DataTable GetAuthorityByGroupId(int groupid);
        Fgroup GetSingle(Fgroup model);//add by mengjuan0826j 獲取單個model  根據groupname和groupcode 
    }
}
