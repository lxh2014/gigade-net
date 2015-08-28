using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IScheduleRelationImplDao
    {
        int Delete(string relateion,int relation_id);
        string Delete(string relation, string relation_id, int schedule_id);//重載
        List<ScheduleRelation> Query(ScheduleRelation sr);
        int Save(ScheduleRelation sr);
        int Update(ScheduleRelation sr);
        List<ScheduleRelation> Query(int schedule_id);
    }
}
