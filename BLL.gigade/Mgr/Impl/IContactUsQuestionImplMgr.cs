using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IContactUsQuestionImplMgr
    {
        DataTable GetContactUsQuestionList(ContactUsQuestionQuery query, out int totalCount);
        DataTable GetContactUsQuestionExcelList(ContactUsQuestionQuery query);
        int Save(ContactUsQuestion query);
        //int Update(ContactUsQuestion query);
        int GetMaxQuestionId();
        string UpdateSql(ContactUsQuestion query);
        int UpdateActive(string sql);//更新列表頁狀態
        DataTable GetUserInfo(int rowID);
    }
}
