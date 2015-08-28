using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface IEmsImplDao
    {
        List<EmsGoalQuery> GetEmsGoalList(EmsGoalQuery query,out int totalCount);
        List<EmsGoalQuery> GetDepartmentStore();
        int SaveEmsGoal(EmsGoalQuery query);
        List<EmsActualQuery> GetEmsActualList(EmsActualQuery query, out int totalCount);
        int VerifyData(EmsGoalQuery query);
        int EditEmsGoal(EmsGoalQuery query);
     //   int EditEmsActual(EmsActualQuery query);
        int SaveEmsActual(EmsActualQuery query);
        int VerifyActualData(EmsActualQuery query);
   //     int insertPreDate(EmsActualQuery query);
        int IsExist(EmsActualQuery query);
        string insertSql(EmsActualQuery query);
    }
}
