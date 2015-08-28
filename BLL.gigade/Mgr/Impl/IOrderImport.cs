using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IOrderImport
    {
        BLL.gigade.Model.Channel CurChannel { get; set; }
        string MySqlConnStr { get; set; }
        List<BLL.gigade.Model.OrdersImport> ReadExcel2Page(string filePath, string template, string model_in);
        void ValidateOrders(List<BLL.gigade.Model.OrdersImport> orders);
        int Import2DB(List<BLL.gigade.Model.OrdersImport> all, string pdfFile, string importType, string chkRecord,int site_id,ref int successCount);
    }
}
