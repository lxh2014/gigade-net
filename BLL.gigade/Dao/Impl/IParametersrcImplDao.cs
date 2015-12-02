using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    interface IParametersrcImplDao
    {
        /// <summary>
        /// 根據參數類型查找參數
        /// </summary>
        /// <param name="para">參數</param>
        /// <returns></returns>
        List<Parametersrc> Query(Parametersrc para);
        /// <summary>
        /// 查询topValue为零的参数
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        List<Parametersrc> QueryForTopValue(Parametersrc para);
        string Save(Parametersrc p);
        bool Save(List<Parametersrc> saveList);
        string Update(Parametersrc para);
        bool Update(List<Parametersrc> updateList);
        string DeleteByType(Parametersrc p);
        List<Parametersrc> QueryType(string NotIn);
        DataTable QueryProperty(Parametersrc Pquery, Parametersrc Cquery);
        List<Parametersrc> GetParameterByCode(string code); //edit by wangwei2014/10/09
        List<Parametersrc> GetParametersrcList(Parametersrc store, out int totalCount);
        int ParametersrcSave(Parametersrc para);
        int UpdateUsed(Parametersrc store);
        List<Parametersrc> PayforType(string parameterType);
        List<Parametersrc> QuerySinggleByID(Parametersrc para);
        List<Parametersrc> GetElementType(string types);

        List<Parametersrc> GetAllKindType(string types);//根據不同類型獲取到各種類型中的值 通用
        //庫調列表
        List<Parametersrc> GetIialgParametersrcList(Parametersrc store, out int totalCount);
        int Delete(Parametersrc m);
        List<Parametersrc> GetParameter(Parametersrc p);
        int InsertTP(Parametersrc p);
        DataTable GetParametercode(Parametersrc p);
        int UpdTP(Parametersrc p);
        string GetOrderStatus(int pc);//得到物流狀體
        List<Parametersrc> QueryParametersrcByTypes(params string[] types);
        DataTable GetTP(Parametersrc p);
        string Getmail(string p);
        List<Parametersrc> ReturnParametersrcList();
        List<Parametersrc> GetKindTypeByStatus(string types);
        List<Parametersrc> SearchParameters(params string[] types);
    }
}
