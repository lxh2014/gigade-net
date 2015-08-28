using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
namespace BLL.gigade.Dao.Impl
{
    public interface IReturnMasterImplDao
    {
        List<OrderReturnUserQuery> GetOrderTempReturnList(OrderReturnUserQuery store, out int totalCount);
        DataTable GetOrderReturnCount(OrderReturnUserQuery store);
        int InsertOrderReturnMaster(OrderReturnUserQuery store);
        int InsertOrderReturnDetail(OrderReturnUserQuery store);
        int InsertOrderMasterStatus(OrderMasterStatus OMS);
        List<OrderReturnUserQuery> OrderMasterQuery(OrderReturnUserQuery store);
        int UpdateOrderMaster(OrderReturnUserQuery store);
        int UpdateOrderDetailStatus(OrderReturnUserQuery store);
        int UpdateTempStatus(OrderReturnUserQuery store);

        DataTable SelOrderMaster(OrderReturnUserQuery store);
        DataTable Seltime(OrderReturnUserQuery store);
        DataTable SelCon(OrderReturnUserQuery store);
        DataTable SelInvoiceid(int invoice_id);
        int Delinvoice(int invoice_id);
        int Updinvoice(OrderReturnUserQuery store);

        List<OrderMaster> Selpay(OrderMaster store);
        DataTable Seldetail(OrderReturnUserQuery store, string status);
        int Insertdb(InvoiceMasterRecord Imr, InvoiceSliveInfo Isi);
        int Updcount(InvoiceAllowanceRecord m);
        DataTable Selmaster(int invoice_id);
        DataTable Selslive(int invoice_id);
        int UpdMaster(int invoice_id);
    }
}
