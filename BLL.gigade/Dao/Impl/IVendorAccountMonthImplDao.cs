using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Model.Custom;
namespace BLL.gigade.Dao.Impl
{
    public interface IVendorAccountMonthImplDao
    {
        List<VendorAccountMonthQuery> GetVendorAccountMonthList(VendorAccountMonthQuery store, out int totalCount);
        List<VendorAccountDetailQuery> GetVendorAccountMonthDetailList(VendorAccountDetailQuery store, out int totalCount);
        List<VendorAccountCustom> VendorAccountDetailExport(VendorAccountDetailQuery query);
        VendorQuery GetVendorInfoByCon(VendorQuery query);
        DataTable GetVendorAccountMonthZongZhang(VendorAccountMonthQuery query);
        DataTable GetFreightMoney(VendorAccountDetailQuery query, out int tempFreightDelivery_Normal, out int tempFreightDelivery_Low);
        DataTable BatchOrderDetail(VendorAccountDetailQuery query);
        DataTable VendorAccountCountExport(VendorAccountMonthQuery query);
        DataTable VendorAccountInfoExport(VendorAccountMonthQuery query);
        DataTable GetVendorAccountMonthInfo(VendorAccountMonthQuery query);
        DataTable GetTaxMoney(VendorAccountDetailQuery query);
    }
}
