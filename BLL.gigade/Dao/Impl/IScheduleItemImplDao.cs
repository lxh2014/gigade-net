using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IScheduleItemImplDao
    {
        string Save(ScheduleItem si);
        List<ScheduleItemCustom> Query(ScheduleItem si);
        string Update(ScheduleItem si);
        string Delete(int schedule, string ids);
        List<ScheduleItemCustom> QueryByCondition(ScheduleItem si);
    }
}
