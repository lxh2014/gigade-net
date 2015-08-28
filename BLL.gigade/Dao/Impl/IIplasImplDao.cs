using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;
namespace BLL.gigade.Dao.Impl
{
    public interface IIplasImplDao
    {
        List<IplasQuery> GetIplas(IplasQuery m, out int totalCount);
        int InsertIplasList(Iplas m);
        int UpIplas(Iplas m);
        string IsTrue(Iplas m);
        List<Iplas> GetIplasCount(Iplas m);
        int GetLocCount(Iloc loc);
        DataTable Getprodbyupc(string prodid);
        List<IplasQuery> Export(IplasQuery m);
        //List<ProductQueryForExcel> NoIlocReportList(ProductQuery query, out int totalCount);
        DataTable NoIlocReportList(ProductQuery query);
        List<Vendor> VendorQueryAll(Vendor query, string AddSql=null);
        DataTable GetIlocReportList(ProductQuery query, out int totalCount);
        int GetIinvdItemId(IinvdQuery vd);
        int DeleteIplasById(IplasQuery plas);
        int GetIplasid(IplasQuery plas);
        DataTable ExportMessage(IplasQuery m);
        List<IplasQuery> GetIplasExportList(IplasQuery iplas);//獲取匯出列表

        int YesOrNoExist(int item_id);
        int YesOrNoLocIdExsit(string loc_id);

        int ExcelImportIplas(string condition);
        int YesOrNoLocIdExsit(int item_id, string loc_id);
        Iplas getplas(Iplas query);
        string Getlocid(string loc_id);
    }
}
