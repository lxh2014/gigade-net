using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface ICalendarImplMgr
    {
        /// <summary>
        /// 無條件查詢所有行事歷控件信息
        /// </summary>
        List<Calendar> Query();

        /// <summary>
        /// 保存
        /// </summary>
        bool Save(Calendar c);

        /// <summary>
        /// 修改
        /// </summary
        bool Update(Calendar c);

        /// <summary>
        /// 刪除
        /// </summary>
        bool Delete(Calendar c);


        /// <summary>
        /// 根據條件查詢行事歷控件信息
        /// </summary>
        /// add by wwei0216w 2015/5/25
        List<Calendar> GetCalendarInfo(Calendar c);
    }
}
