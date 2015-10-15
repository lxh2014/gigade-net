using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using gigadeExcel.Comment;

namespace BLL.gigade.Mgr
{
    //吉甲地推薦系統匯出 Mgr guodong1130w 2015/10/9
    public class RecommendedExcleMgr : IRecommendedExcleImplMgr
    {
        private IRecommendedExcleImplDao _iRecommendedExcleImplDao;
        public RecommendedExcleMgr(string connectionStr)
        {
            _iRecommendedExcleImplDao = new RecommendedExcleDao(connectionStr);
        }

        //導出會員
        public List<MemoryStream> GetVipUserInfo(RecommendedOutPra rop, string sheetname)
        {
            //獲得會員
            DataTable dtVipUser = _iRecommendedExcleImplDao.GetVipUserInfo(rop);
            if (dtVipUser.Rows.Count > 60000)
            {
                return BuildRecommendedExcleOutBigInfo(dtVipUser, sheetname);
            }
            else 
            {
                return BuildRecommendedExcleOut(dtVipUser, sheetname);
            }
        }
        //導出商品
        public List<MemoryStream> GetProductInfo(RecommendedOutPra rop, string sheetname)
        {
            //獲得商品
            DataTable dtProduct = _iRecommendedExcleImplDao.GetProductInfo(rop);
            if (dtProduct.Rows.Count > 60000)
            {
                return BuildRecommendedExcleOutBigInfo(dtProduct, sheetname);
            }
            else
            {
                return BuildRecommendedExcleOut(dtProduct, sheetname);
            }
        }
        //導出訂單
        public List<MemoryStream> GetOrderInfo(RecommendedOutPra rop, string sheetname)
        {
            //獲得訂單
            DataTable dtOrder = _iRecommendedExcleImplDao.GetOrderInfo(rop);
            if (dtOrder.Rows.Count > 60000)
            {
                return BuildRecommendedExcleOutBigInfo(dtOrder, sheetname);
            }
            else
            {
                return BuildRecommendedExcleOut(dtOrder, sheetname);
            }
        }
        //導出訂單內容
        public List<MemoryStream> GetOrderDetailInfo(RecommendedOutPra rop, string sheetname)
        {
            //獲得訂單內容
            DataTable dtOrderDetail = _iRecommendedExcleImplDao.GetOrderDetailInfo(rop);
            if (dtOrderDetail.Rows.Count > 60000)
            {
                return BuildRecommendedExcleOutBigInfo(dtOrderDetail, sheetname);
            }
            else
            {
                return BuildRecommendedExcleOut(dtOrderDetail, sheetname);
            }
        }
        //導出類別
        public List<MemoryStream> GetCategoryInfo(RecommendedOutPra rop, string sheetname)
        {
            //獲得類別
            DataTable dtCategory = _iRecommendedExcleImplDao.GetCategoryInfo(rop);
            if (dtCategory.Rows.Count > 60000)
            {
                return BuildRecommendedExcleOutBigInfo(dtCategory, sheetname);
            }
            else
            {
                return BuildRecommendedExcleOut(dtCategory, sheetname);
            }
        }
        //導出品牌
        public List<MemoryStream> GetBrandInfo(RecommendedOutPra rop, string sheetname)
        {
            //獲得品牌
            DataTable dtBrand = _iRecommendedExcleImplDao.GetBrandInfo(rop);
            if (dtBrand.Rows.Count > 60000)
            {
                return BuildRecommendedExcleOutBigInfo(dtBrand, sheetname);
            }
            else
            {
                return BuildRecommendedExcleOut(dtBrand, sheetname);
            }
        }
        //構造Excle大於60000條數據處理
        public List<MemoryStream> BuildRecommendedExcleOutBigInfo(DataTable dt, string NameListStr)
        {
            List<MemoryStream> list = new List<MemoryStream>();
            List<string> NameList = new List<string>();
            List<DataTable> Elist = new List<DataTable>();
            List<bool> comName = new List<bool>();
            int dtcount = dt.Rows.Count;
            int rowMore = dtcount % 60000;
            int ExcleNum = dtcount / 60000;
            if (rowMore > 0)
            {
                ExcleNum++;
            }
            for (int i = 0; i < ExcleNum; i++)
            {
                DataTable newdt = new DataTable();
                newdt = dt.Clone();
                List<DataRow> lirow = dt.Select().Skip(i * 60000).Take(60000).ToList();
                foreach (DataRow row in lirow)
                {
                    newdt.Rows.Add(row.ItemArray);
                }
                Elist.Add(newdt);
                NameList.Add(NameListStr + i);
                comName.Add(true);
            }
            list.Add(ExcelHelperXhf.ExportDTNoColumnsBySdy(Elist, NameList, comName));
            return list;
        }
        //構造Excle
        public List<MemoryStream> BuildRecommendedExcleOut(DataTable dt, string NameListStr)
        {
            List<MemoryStream> list = new List<MemoryStream>();
            List<string> NameList = new List<string>();
            List<DataTable> Elist = new List<DataTable>();
            List<bool> comName = new List<bool>();
            Elist.Add(dt);
            NameList.Add(NameListStr);
            comName.Add(true);
            list.Add(ExcelHelperXhf.ExportDTNoColumnsBySdy(Elist, NameList, comName));
            return list;
        }
    }
}
