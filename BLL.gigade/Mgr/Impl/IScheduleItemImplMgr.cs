using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IScheduleItemImplMgr
    {
        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="si">ScheduleItem 對象</param>
        /// <returns></returns>
        string Save(ScheduleItem si);

        List<ScheduleItemCustom> Query(ScheduleItem si);

        string Update(ScheduleItem si);

        bool Delete(int schedule, string ids, string item_type, string item_value);

        List<ScheduleItemCustom> QueryByCondition(ScheduleItem si);

        bool UpdateByBacth(List<ScheduleItem> lists);
    }
}
