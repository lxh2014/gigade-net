using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    interface IVGroupCallerImplDao
    {
        /// <summary>
        /// 查詢
        /// </summary>
        /// <returns></returns>
        List<groupCaller> QueryCallidById(groupCaller gc);

        /// <summary>
        /// 保存
        /// </summary>
        int Save(groupCaller gc);

        /// <summary>
        /// 刪除
        /// </summary>
        int Delete(groupCaller gc);
    }
}
