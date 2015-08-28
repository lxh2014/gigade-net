using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    public interface IAseldImplDao  
    {
        List<Model.Query.OrderMasterQuery> GetOrderMasterList(Model.Query.OrderMasterQuery oderMaster, out int totalCount);
        List<Model.Query.OrderMasterQuery> GetAllOrderDetail(Model.Query.OrderMasterQuery oderMaster);

        DataTable  ExportDeliveryStatement(int counts,int types);//匯出大出貨單
        DataTable ExportAseldMessage(AseldQuery ase);//匯出撿貨表by料位元
        string Insert(Aseld m);
        DataTable SelOrderDetail(string id, string fre, int radioselect);

        int InsertSql(string sql);
        DataTable GetOrderProductInformation(AseldQuery ase);
        string UpdTicker(string id);
        List<AseldQuery> GetAseldList(Aseld ase);
        string UpdAseld(Aseld a);
        int SelCom(Aseld a);
        int SelComA(Aseld a);
        int SelComC(Aseld m);

        int SelectCount(Aseld m);
        string AddIwsRecord(AseldQuery a);
        //DataTable GetProdExt(AseldQuery ase);
        int DecisionBulkPicking(AseldQuery ase, int commodity_type);

        int UpdScaned(Aseld a);

        DataTable GetNComJobDetail(string jobNumbers);
        DataTable GetNComJobSimple();

        int UpdateScnd(Aseld ase);
        int Updwust(Aseld a);
        void ConsoleAseldBeforeInsert(int detail_id);
        string Getfreight(string ord_id);
    }
}
