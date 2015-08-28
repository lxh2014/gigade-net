using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface ICbjobDetailImplDao
    {
        List<CbjobDetailQuery> GetMessage(CbjobDetailQuery cbjobQuery, out int totalCount);

        int DeleteCbjobmessage(CbjobDetailQuery cbjobQuery);//改變cbjob表狀態

        int UpdateCbjobMaster(CbjobDetailQuery cbjobQuery);

        int UpdateCbjobstaid(CbjobDetailQuery cbjobQuery);//蓋帳

        int FupanComplete(CbjobDetailQuery cbjobQuery);//復盤完成

        string insertsql(CbjobDetail cb);
        int InsertSql(string sql);
    }
}
