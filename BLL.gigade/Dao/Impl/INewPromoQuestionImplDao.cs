using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface INewPromoQuestionImplDao
    {
        DataTable GetPromoQuestionList(NewPromoQuestionQuery query, out int totalCount);
        string InsertNewPromoQuestion(NewPromoQuestionQuery store);
        string UpdateNewPromoQuestion(NewPromoQuestionQuery store);
        int DeleteNewPromoQuestion(string row_id);
        List<NewPromoQuestionQuery> GetPromoQuestionList(NewPromoQuestionQuery query);
        string UpdateActive(NewPromoQuestionQuery query);
        string GetMaxRowId();

    }
}
