using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Temp;
using gigadeExcel.Comment;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BLL.gigade.Mgr
{
    public class ProductOrderTempMgr : IProductOrderTempImplMgr
    {
        private Dao.Impl.IProductOrderTempImplDao _potDao;
        private string conStr = "";
        public ProductOrderTempMgr(string connectionString)
        {
            _potDao = new Dao.ProductOrderTempDao(connectionString);
            conStr = connectionString;
        }

        public List<ProductOrderTemp> QuerySingle(ProductOrderTemp pot)
        {
            try
            {
                return _potDao.QuerySingle(pot);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductOrderTempMgr-->QuerySingle" + ex, ex);
            }
        }

        public List<ProductOrderTemp> QueryParent(ProductOrderTemp pot)
        {
            try
            {
                return _potDao.QueryParent(pot);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductOrderTempMgr-->QueryParent" + ex, ex);
            }
        }

        public List<ProductOrderTemp> QueryChild(DateTime dt, string parentIds)
        {
            try
            {
                return _potDao.QueryChild(dt, parentIds);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductOrderTempMgr-->QueryChild" + ex, ex);
            }       
        }


        public MemoryStream OutToExcel(string fileName,ProductOrderTemp pot)
        {
            //XDocument xml = XDocument.Load(fileName);//加载xml 
            //Dictionary<string, string> columns = xml.Elements().Elements().ToDictionary(p => p.Attribute("key").Value, p => p.Value);//將xml轉換成Dictionary
            List<ProductOrderTemp> productOrderTempItems = new List<ProductOrderTemp>();//組合商品
            List<ProductOrderTemp> productSingleTempItems =new List<ProductOrderTemp>();//單一商品
            List<ProductOrderTemp> childOrderTempItems = new List<ProductOrderTemp>();//組合商品中子商品
            List<ProductOrderTemp> mainOrderTempItems = new List<ProductOrderTemp>();//組合商品與子商品的集合
            DateTime dt = Convert.ToDateTime("2014/10/1 00:00:01");
            string parentIds = "";
            productSingleTempItems = QuerySingle(pot);
            productOrderTempItems = QueryParent(pot); ///查詢父商品
            if (productOrderTempItems != null && productOrderTempItems.Count > 0)
            {
                foreach (ProductOrderTemp p in productOrderTempItems)
                {
                    parentIds += p.product_id + ",";
                }
                parentIds = parentIds.Remove(parentIds.ToString().Length - 1, 1);//拼接父商品product_id
                childOrderTempItems = QueryChild(dt, parentIds); //查詢所有的子商品
                foreach (ProductOrderTemp p in productOrderTempItems)
                {
                    mainOrderTempItems.Add(p);
                    List<ProductOrderTemp> list = childOrderTempItems.FindAll(m => m.parent_id == p.product_id);
                    mainOrderTempItems.AddRange(list);
                }//查找父商品的子商品

                var dic = new Dictionary<string,List<ProductOrderTemp>>();
                dic.Add("單一商品",productSingleTempItems);
                dic.Add("組合商品",mainOrderTempItems);
                return ExcelHelperXhf.ExportExcel<ProductOrderTemp>(dic, fileName, null);
                //return ExportExcelByOrder(productSingleTempItems,mainOrderTempItems, columns);
            }
            else 
            {
                return null;
            }
            throw new Exception("unaccepted exportFlag!!!");
        }


        
    }
}
