using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface IOrderQuestionIDao
    {
        List<OrderQuestionQuery> GetOrderQuestionList(OrderQuestionQuery o, out int totalCount);
        DataTable GetOrderQuestionExcel(OrderQuestionQuery o);
        System.Data.DataTable GetList(OrderQuestionQuery query, out int totalCount);
        List<Parametersrc> GetDDL();

        /// <summary>
        /// 更新訂單問題狀態
        /// </summary>
        /// <param name="query">更新條件</param>
        void UpdateQuestionStatus(OrderQuestionQuery query);

        int InsertOrderQuestion(OrderQuestion query);//新增訂單問題
        DataTable GetUserInfo(int rowID);
    }
}
