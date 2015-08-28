using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
   public interface IEmsImplMgr
    {
       List<EmsGoalQuery> GetEmsGoalList(EmsGoalQuery query, out int totalCount);
       List<EmsGoalQuery> GetDepartmentStore();
       string SaveEmsGoal(EmsGoalQuery query);
       List<EmsActualQuery> GetEmsActualList(EmsActualQuery query, out int totalCount);
       int VerifyData(EmsGoalQuery query);
       string EditEmsGoal(EmsGoalQuery query);
       string EditEmsActual(EmsActualQuery query);
       string SaveEmsActual(EmsActualQuery query);
       int VerifyActualData(EmsActualQuery query);
       //int insertPreDate(EmsActualQuery query);
    }
}
