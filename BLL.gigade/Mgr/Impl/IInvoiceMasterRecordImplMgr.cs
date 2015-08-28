using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IInvoiceMasterRecordImplMgr
    {
        DataTable GetInvoiceList(InvoiceMasterRecordQuery imrq, out int totalCount);
        DataTable ExportTXT(InvoiceMasterRecordQuery imrq);
        DataTable ExportExlF(InvoiceMasterRecordQuery imrq);
        DataTable ExportExlS(string order_id);
        //DataTable ExportExlT(uint order_id);
        //DataTable GetMangerUser();
        //DataTable QueryTXT(InvoiceList ilist);
        DataTable InvoicePrint(uint invoice_id);
        int Update(InvoiceMasterRecord imr);
        List<Zip> GetZipAddress(Zip zip);
        DataTable GetParametersrc(string parameterType, string key);

        /// <summary>
        /// 更新定單狀態以便開立發票
        /// </summary>
        /// <param name="order_id">定單編號</param>
        /// <param name="freight_normal">常溫</param>
        /// <param name="freight_low">低溫</param>
        /// <param name="status">操作狀態</param>
        /// <returns>是否操作成功</returns>
        bool ModifyOrderInvoice(int order_id, int freight_normal, int freight_low, string status);
    }
}