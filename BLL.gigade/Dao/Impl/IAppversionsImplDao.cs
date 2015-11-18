using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IAppversionsImplDao
    {
        //查詢方法
        List<Model.AppversionsQuery> GetAppversionsList(Model.AppversionsQuery appsions, out int totalCount);
        //通過ID刪除方法
        int DeleteAppversionsById(string rowid);
        //增加方法
        int AddAppversions(Model.AppversionsQuery appsions);

        int EditAppversions(Model.AppversionsQuery appsions);
        int UpdateAppversionsActive(int id, int status);
    }
}
