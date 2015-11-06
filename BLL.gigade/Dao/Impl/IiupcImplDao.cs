using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    public interface IiupcImplDao
    {
        string IsExist(Iupc iupc);
        int Insert(Iupc iupc);
        int Delete(Iupc iupc);
        int Update(Iupc iupc);
        List<IupcQuery> GetIupcList(IupcQuery iupc, out int totalCount);
        DataTable upcid(Iupc m);
        int Yesornoexist(int i, string j);
        int ExcelImportIupc(string condition);
        List<IupcQuery> GetIupcExportList(IupcQuery iupc);//獲取匯出列表
        int upc_num(int m);//查詢這個商品是否有國際碼
        string Getupc(string item_id, string type);
        List<IupcQuery> GetIupcByItemID(IupcQuery query);
        List<IupcQuery> GetIupcByType(IupcQuery query);
    }
}
