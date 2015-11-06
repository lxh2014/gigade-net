using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    //吉甲地推薦系統匯出 Mgr接口 guodong1130w 2015/10/9
    public interface IRecommendedExcleImplMgr
    {
        //導出會員
        List<MemoryStream> GetVipUserInfo(RecommendedOutPra rop, string sheetname);
        //導出商品
        List<MemoryStream> GetProductInfo(RecommendedOutPra rop, string sheetname);
        //導出訂單
        List<MemoryStream> GetOrderInfo(RecommendedOutPra rop, string sheetname);
        //導出訂單內容
        List<MemoryStream> GetOrderDetailInfo(RecommendedOutPra rop, string sheetname);
        //導出類別
        List<MemoryStream> GetCategoryInfo(RecommendedOutPra rop, string sheetname);
        //導出品牌
        List<MemoryStream> GetBrandInfo(RecommendedOutPra rop, string sheetname);
        //構造Excle大於60000條數據處理
        List<MemoryStream> BuildRecommendedExcleOutBigInfo(DataTable dt, string NameListStr);
        //構造Excle
        List<MemoryStream> BuildRecommendedExcleOut(DataTable dt, string NameListStr);
        //導出product信息
        StringWriter GetThisProductInfo();

        StringWriter GetVendorCategoryMsg();
    }
}
