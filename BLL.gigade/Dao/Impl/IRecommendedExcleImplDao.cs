using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Dao.Impl
{
    public interface IRecommendedExcleImplDao
    {
        //獲得會員信息
        DataTable GetVipUserInfo(RecommendedOutPra rop);
        //獲得商品信息
        DataTable GetProductInfo(RecommendedOutPra rop);
        //獲得訂單信息
        DataTable GetOrderInfo(RecommendedOutPra rop);
        //獲得訂單內容信息
        DataTable GetOrderDetailInfo(RecommendedOutPra rop);
        //獲得類別信息
        DataTable GetCategoryInfo(RecommendedOutPra rop);
        //獲得品牌信息
        DataTable GetBrandInfo(RecommendedOutPra rop);
        //獲得訂單信息最小年份
        DataTable GetOrderInfoByMinYear();
        //獲取商品信息
        DataTable GetThisProductInfo();

        List<CategoryItem> GetVendorCategoryMsg(CategoryItem pcb,List<CategoryItem> lscm);

        DataTable GetVendorBrandMsg(CategoryItem pcb);
    }
}
