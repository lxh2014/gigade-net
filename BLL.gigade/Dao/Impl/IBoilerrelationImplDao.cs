using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface IBoilerrelationImplDao
    {
        int GetintoBoilerrelation(DataRow[] dr, out int tatal);//匯入數據

        List<boilerrelationQuery> QueryBoilerRelationAll(boilerrelationQuery boilQuery, out int totalCount);
    }
}
