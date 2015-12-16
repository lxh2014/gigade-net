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
        public DataTable GetVipUserInfo(RecommendedOutPra rop, string sheetname)
        {
            try
            {
                //獲得會員
                DataTable dtVipUser = _iRecommendedExcleImplDao.GetVipUserInfo(rop);
                return dtVipUser;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedExcleMgr-->GetVipUserInfo" + ex.Message, ex);
            }
        }
        //導出商品
        public DataTable GetProductInfo(RecommendedOutPra rop, string sheetname)
        {
            try
            {
                //獲得商品
                DataTable dtProduct = _iRecommendedExcleImplDao.GetProductInfo(rop);
                return dtProduct;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedExcleMgr-->GetProductInfo" + ex.Message, ex);
            }
        }
        //導出訂單
        public DataTable GetOrderInfo(RecommendedOutPra rop, string sheetname)
        {
            try
            {
                //獲得訂單
                DataTable dtOrder = _iRecommendedExcleImplDao.GetOrderInfo(rop);
                return dtOrder;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedExcleMgr-->GetOrderInfo" + ex.Message, ex);
            }
        }
        //導出訂單內容
        public DataTable GetOrderDetailInfo(RecommendedOutPra rop, string sheetname)
        {
            try
            {
                //獲得訂單內容
                DataTable dtOrderDetail = _iRecommendedExcleImplDao.GetOrderDetailInfo(rop);
                return dtOrderDetail;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedExcleMgr-->GetOrderDetailInfo" + ex.Message, ex);
            }
        }
        //導出類別
        public DataTable GetCategoryInfo(RecommendedOutPra rop, string sheetname)
        {
            try
            {
                //獲得類別
                DataTable dtCategory = _iRecommendedExcleImplDao.GetCategoryInfo(rop);
                return dtCategory;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedExcleMgr-->GetCategoryInfo" + ex.Message, ex);
            }
        }
        //導出品牌
        public DataTable GetBrandInfo(RecommendedOutPra rop, string sheetname)
        {
            //獲得品牌
            try
            {
                DataTable dtBrand = _iRecommendedExcleImplDao.GetBrandInfo(rop);
                return dtBrand;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedExcleMgr-->GetBrandInfo" + ex.Message, ex);
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
        public StringBuilder GetThisProductInfo(int start_product_id, int end_product_id)
        {
            //獲得品牌
            StringBuilder str = new StringBuilder();
            try
            {
                //获取到所有商品的基本的信息
                DataTable dtProduct = _iRecommendedExcleImplDao.GetThisProductInfo(start_product_id, end_product_id);
                //获取到所有商品和品牌信息
                DataTable dtBrand = _iRecommendedExcleImplDao.GetAllBrandByProductId();
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
                    #region 根据product_id和brand_id获取到cid内的值
                    DataRow[] _dtNew = dtBrand.Select("product_id=" + dtProduct.Rows[i]["product_id"]);
                    string tcidstr = string.Empty;
                    foreach (DataRow row in _dtNew)  // 将查询的结果添加到dt中;
                    {
                        if (tcidstr.Length > 0)
                        {
                            tcidstr = tcidstr + "," + row[1].ToString() + "-" + row[2].ToString();//0表示product_id1表示category_id 2表示brand_id
                        }
                        else
                        {
                            tcidstr = row[1].ToString() + "-" + row[2].ToString();
                        }
                    }
                    #endregion
                    str.AppendLine(@"<cid>" + tcidstr + "</cid>");//類別id
                    str.AppendLine(@"<pubdate>" + dtProduct.Rows[i]["crate_time"] + "</pubdate>");//上架时间   
                    string status = string.Empty;
                    if (dtProduct.Rows[i]["product_status"].ToString() == "5")
                    {
                        status = "1";
                    }
                    else
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
                        strurl = @"<![CDATA[http://" + "www.gigade100.com/newweb" + "/stuff/product_stuff.php?pid=" + dtProduct.Rows[i]["product_id"] + "&view=" + DateTime.Now.ToString("yyyyMMdd") + "]]>";
                    }
                    else
                    {
                        strurl = @"<![CDATA[http://" + "www.gigade100.com/newweb" + "/product.php?pid=" + dtProduct.Rows[i]["product_id"] + "&view=" + DateTime.Now.ToString("yyyyMMdd") + "]]>";//商品預覽
                    }
                    str.AppendLine(@"<url>" + strurl + "</url>");
                    str.AppendLine(@"<imgurl>" + "<![CDATA[" + picPath + "/product/" + dtProduct.Rows[i]["product_image"] + "]]>" + "</imgurl>");//图片
                    str.AppendLine(@"<adforbid>0</adforbid>");
                    str.AppendLine(@"</item>");
                }
                str.AppendLine(@"</feeds>");

                return str;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedExcleMgr-->GetThisProductInfo" + ex.Message + str.ToString(), ex);
            }
        }
        #endregion

        #region 获取到食用品館類別樹含品牌
        public DataTable GetVendorCategoryMsg()
        {
            try
            {
                StringBuilder str = new StringBuilder();
                List<CategoryItem> lscm = new List<CategoryItem>();
                CategoryItem foodItem = new CategoryItem()//食品館
                {
                    Id = "1162",
                    PId = "0",
                    Name = "食品館",
                    Depth = 1
                };
                lscm.Add(foodItem);

                //導出XML
                List<CategoryItem> lssw = _iRecommendedExcleImplDao.GetVendorCategoryMsg(foodItem, lscm);


                CategoryItem stuffItem = new CategoryItem()//用品館
                       {
                           Id = "1239",
                           PId = "0",
                           Name = "用品館",
                           Depth = 1
                       };
                lssw.Add(stuffItem);

                List<CategoryItem> lsswtwo = _iRecommendedExcleImplDao.GetVendorCategoryMsg(stuffItem, lscm);

                DataTable result = new DataTable();
                result.Columns.Add("category_name", typeof(String));
                result.Columns.Add("category_id", typeof(String));
                result.Columns.Add("parent_id", typeof(String));
                result.Columns.Add("level", typeof(String));

                foreach (var ls in lsswtwo)
                {
                    DataRow drfood = result.NewRow();
                    drfood[0] = ls.Name;
                    drfood[1] = ls.Id;
                    drfood[2] = ls.PId;
                    drfood[3] = ls.Depth;
                    result.Rows.Add(drfood);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedExcleMgr-->GetVendorCategoryMsg" + ex.Message, ex);
            }
        }

        #endregion
    }
}
