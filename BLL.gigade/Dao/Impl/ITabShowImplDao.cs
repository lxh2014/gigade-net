using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Dao.Impl
{
    public interface ITabShowImplDao
    {
        List<OrderMasterStatusQuery> GetStatus(OrderMasterStatusQuery store, out int totalCount);
        List<OrderDeliverQuery> GetDeliver(OrderDeliverQuery store, out int totalCount);
        List<OrderPaymentAlipay> GetAlipayList(OrderPaymentAlipay store, out int totalCount);
        List<DeliverMasterQuery> GetNewDeliver(DeliverMasterQuery store, out int totalCount);
        List<OrderPaymentUnionPay> GetUnionPayList(OrderPaymentUnionPay store, out int totalCount);
        List<UsersDeductBonus> GetUserDeductBonus(UsersDeductBonus store, out int totalCount);
        List<OrderCancelMasterQuery> GetCancel(OrderCancelMasterQuery store, out int totalCount);
        List<OrderReturnMasterQuery> GetReturn(OrderReturnMasterQuery store, out int totalCount);
        List<OrderMoneyReturnQuery> GetMoney(OrderMoneyReturnQuery store,out int totalCount);
        List<InvoiceMasterRecordQuery> GetInvoiceMasterRecord(InvoiceMasterRecordQuery store, out int totalCount);
        List<OrderQuestionQuery> GetQuestion(OrderQuestionQuery store, out int totalCount);
        List<OrderPaymentCt> GetOrderPaymentCtList(OrderPaymentCt store, out int totalCount);
        List<OrderCancelMsgQuery> GetCancelMsg(OrderCancelMsgQuery store, out int totalCount);
        List<OrderPaymentHncbQuery> QueryOrderHncb(OrderPaymentHncbQuery store, out int totalCount);
        List<OrderPaymentNcccQuery> GetNCCC(OrderPaymentNcccQuery store, out int totalCount);
        List<OrderPaymentHitrustQuery> GetOderHitrust(OrderPaymentHitrustQuery store, out int totalCount);
        List<LogisticsDetailQuery> GetLogistics(LogisticsDetailQuery store, out int totalCount);
        List<OrderReturnContentQuery> GetOrderReturnContentQueryUp(OrderReturnContentQuery store, out int totalCount);
        List<OrderReturnMasterQuery> GetReturnMasterDown(OrderReturnMasterQuery store, out int totalCount);
        DataTable GetOderHitrustDT(int order_id);
        DataTable GetNCCC(int order_id);
    }
}
