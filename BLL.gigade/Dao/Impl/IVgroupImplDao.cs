/*
* 文件名稱 :IVgroupImplDao.cs
* 文件功能描述 :供應商群組
* 版權宣告 :
* 開發人員 : 天娥璇子
* 版本資訊 : 1.0
* 日期 : 2013/08/02
* 修改人員 :shiwei0620j
* 版本資訊 : 
* 日期 : 2014/08/18
* 修改備註 : 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    interface IVgroupImplDao
    {
        /// <summary>
        /// 查詢
        /// </summary>
        /// <returns></returns>
        List<Vgroup> QueryAll();

        List<ManageUser> QueryCallid();

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="fg"></param>
        /// <returns></returns>
        int Save(Vgroup vg);

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="fg"></param>
        /// <returns></returns>
        int Delete(Vgroup vg);
    }
}
