using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    interface IInvoiceMasterRecordImplDao
    {
        DataTable GetInvoiceList(InvoiceMasterRecordQuery imrq, out int totalCount);
        DataTable ExportTXT(InvoiceMasterRecordQuery imrq);
        DataTable ExportExlF(InvoiceMasterRecordQuery imrq);
        //DataTable ExportExlS(string order_id);
        DataTable ExportExlS(string order_id);
        //DataTable GetMangerUser();
        //DataTable QueryTXT(InvoiceList ilist);
        DataTable InvoicePrint(uint invoice_id);
        int Update(InvoiceMasterRecord imr);
        List<Zip> GetZipAddress(Zip zip);
        DataTable GetParametersrc(string parameterType, string key);
    }
}
