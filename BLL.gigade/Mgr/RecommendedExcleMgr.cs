﻿using System;
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
using System.Configuration;
using BLL.gigade.Model.Custom;

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

        #region 获取到xml的数据流
        public StringWriter GetThisProductInfo()
        {
            //獲得品牌
            DataTable dtProduct = _iRecommendedExcleImplDao.GetThisProductInfo();
            StringWriter sw = new StringWriter();
            StringBuilder str = new StringBuilder();
            string picPath = ConfigurationManager.AppSettings["imgServerPath"];//获取图片显示的路径
            str.AppendLine(@"<?xml version='1.0' encoding='UTF-8'?>");
            str.AppendLine(@"<feeds>");
            str.AppendLine(@"<info>");
            str.AppendLine(@"<format_schema>http://rec.scupio.com/recommendation/info/xml/v1</format_schema>");
            str.AppendLine(@"<date>" + DateTime.Now.ToString() + "</date>");
            str.AppendLine(@"</info>");
            for (int i = 0; i < dtProduct.Rows.Count; i++)
            {
                str.AppendLine(@"<item>");
                str.AppendLine(@"<id>" + dtProduct.Rows[i]["item_id"] + "</id>");//item_id
                str.AppendLine(@"<cid>" + dtProduct.Rows[i]["cate_id"] + "</cid>");//類別id
                str.AppendLine(@"<pubdate>" + dtProduct.Rows[i]["crate_time"] + "</pubdate>");//上架时间   
                string status = string.Empty;
                if (dtProduct.Rows[i]["product_status"].ToString() == "5")
                {
                    status = "1";
                }
                else if (dtProduct.Rows[i]["product_status"].ToString() == "6")
                {
                    status = "0";
                }
                str.AppendLine(@"<status>" + status + "</status>");//1表示上架 0表示下架
                str.AppendLine(@"<sales>" + dtProduct.Rows[i]["event_starts"] + "," + dtProduct.Rows[i]["event_ends"] + "," + dtProduct.Rows[i]["event_price"] + "</sales>");//類別id
                str.AppendLine(@"<name>" + dtProduct.Rows[i]["product_name"] + "</name>");
                str.AppendLine(@"<subtitle>" + dtProduct.Rows[i]["product_alt"] + "</subtitle>");
                str.AppendLine(@"<brief>" + dtProduct.Rows[i]["page_content_1"] + "</brief>");
                str.AppendLine(@"<price>" + dtProduct.Rows[i]["price"] + "</price>");
                str.AppendLine(@"<mprice>" + dtProduct.Rows[i]["cost"] + "</mprice>");
                //根据食品管和用品管的不同,连接也不相同
                string strurl = string.Empty;
                if (dtProduct.Rows[i]["prod_classify"] == "10")
                {
                    strurl = @"<![CDATA[http://" + "www.gigade100.com/newweb" + "/food/product_food.php?pid=" + dtProduct.Rows[i]["product_id"] + "&view=" + DateTime.Now.ToString("yyyyMMdd") + "]]>";
                }
                else if (dtProduct.Rows[i]["prod_classify"] == "20")
                {
                    strurl = @"<![CDATA[http://" + "www.gigade100.com/newweb" + "/stuff/product_stuff.php?pid=" + dtProduct.Rows[i]["product_id"] + "&view=" + DateTime.Now.ToString("yyyyMMdd") + "]]>"; ;
                }
                else
                {
                    strurl = @"<![CDATA[http://" + "www.gigade100.com/newweb" + "/product.php?pid=" + dtProduct.Rows[i]["product_id"] + "&view=" + DateTime.Now.ToString("yyyyMMdd") + "]]>"; ;//商品預覽
                }
                str.AppendLine(@"<url>" + strurl + "</url>");
                str.AppendLine(@"<imgurl>" + "<![CDATA[" + picPath + dtProduct.Rows[i]["product_image"] + "]]>" + "</imgurl>");//图片
                str.AppendLine(@"<adforbid>0</adforbid>");
                str.AppendLine(@"</item>");
            }
            str.AppendLine(@"</feeds>");
            sw.WriteLine(str.ToString());
            sw.Close();
            return sw;
        }
        #endregion

        #region 获取到食用品館類別樹含品牌txt文件信息的数据流

        public string ToTxtString(CategoryItem cm)
        {
            String strPrefix = "";
            for (Int32 i = 1; i < cm.Depth; i++)
            {
                strPrefix += "\t";
            }
            return strPrefix + String.Format("{0}: {1} - {2}", cm.Id, cm.Name, cm.Depth);
        }

        public StringWriter GetVendorCategoryMsg()
        {
            StringBuilder str = new StringBuilder();
            StringWriter sw = new StringWriter();
            List<CategoryItem> lscm = new List<CategoryItem>();
            CategoryItem foodItem = new CategoryItem()//食品館
            {
                Id = "1162",
                Name = "食品館",
                Depth = 1
            };
            lscm.Add(foodItem);

            //導出XML
            List<CategoryItem> lssw = _iRecommendedExcleImplDao.GetVendorCategoryMsg(foodItem, lscm);


            CategoryItem stuffItem = new CategoryItem()//用品館
                   {
                       Id = "1239",
                       Name = "用品館",
                       Depth = 1
                   };
            lssw.Add(stuffItem);

            List<CategoryItem> lsswtwo = _iRecommendedExcleImplDao.GetVendorCategoryMsg(stuffItem, lscm);

            foreach (var ls in lsswtwo)
            {
                str.AppendLine(ToTxtString(ls));
            }
            sw.WriteLine(str.ToString());
            sw.Close();
            return sw;
        }

        #endregion
    }
}
