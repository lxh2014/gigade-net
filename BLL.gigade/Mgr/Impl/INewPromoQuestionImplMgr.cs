using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface INewPromoQuestionImplMgr
    {
        DataTable GetPromoQuestionList(NewPromoQuestionQuery query, out int totalCount);
        //int SaveQuestionOne(NewPromoQuestionQuery query);
        int InsertNewPromoQuestion(NewPromoQuestionQuery query);
        int DeleteQuestion(string row_id);
        int UpdateNewPromoQuestion(NewPromoQuestionQuery query);
        List<NewPromoQuestionQuery> GetPromoQuestionList(NewPromoQuestionQuery query);
        int UpdateActive(NewPromoQuestionQuery query);
        //NewPromoQuestionQuery GetModel(NewPromoQuestionQuery query);
    }
}
