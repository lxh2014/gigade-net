using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade.Mgr.Impl
{
    public interface IParametersrcImplMgr
    {
        string Query(string strParaType, int used = 1);
        string QueryByCode(string strParaCode, int used = 1);
        List<Parametersrc> QueryUsed(Parametersrc para);
        string Save(Parametersrc p);
        bool Save(List<Parametersrc> saveList);
        bool Update(List<Parametersrc> updateList);
        List<Parametersrc> QueryType(string NotIn);
        DataTable QueryProperty(Parametersrc Pquery, Parametersrc Cquery);
        List<Parametersrc> GetParameterByCode(string code); //add by wangwei0216w 2014/10/9
        int ParametersrcSave(Parametersrc para);
        List<Parametersrc> GetParametersrcList(Parametersrc store, out int totalCount);
        int UpdateUsed(Parametersrc store);
        List<Parametersrc> PayforType(string parameterType);
        List<Parametersrc> QuerySinggleByID(Parametersrc para);
        List<Parametersrc> GetElementType(string types);
        List<Parametersrc> QueryForTopValue(Parametersrc para);
        List<Parametersrc> GetAllKindType(string types);//根據不同類型獲取到各種類型中的值 通用
        //庫調
        List<Parametersrc> GetIialgParametersrcList(Parametersrc store, out int totalCount);
        int Delete(Parametersrc m);
        List<Parametersrc> GetParameter(Parametersrc p);
        int InsertTP(Parametersrc p);
        DataTable GetParametercode(Parametersrc p);
        int UpdTP(Parametersrc p);
        string GetOrderStatus(int pc);//得到物流狀體
        DataTable GetTP(Parametersrc p);
        string Getmail(string p);
        List<Parametersrc> ReturnParametersrcList();
        List<Parametersrc> GetKindTypeByStatus(string types);
    }
}
