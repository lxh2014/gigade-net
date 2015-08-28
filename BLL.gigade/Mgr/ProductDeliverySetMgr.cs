using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Common;
using gigadeExcel.Comment;
using BLL.gigade.Model.Custom;
using System.IO;


namespace BLL.gigade.Mgr
{
    public class ProductDeliverySetMgr : IProductDeliverySetImplMgr
    {
        private string connectionString;
        private IProductDeliverySetImplDao ds;
        private IProductImplDao _productDao;
        public ProductDeliverySetMgr(string connectionString)
        {
            this.connectionString=connectionString;
            ds = new ProductDeliverySetDao(connectionString);
        }

        /// <summary>
        /// 新增插入信息
        /// </summary>
        /// <param name="sc">一個ProductDeliverySet對象</param>
        /// <returns>受影響的行數</returns>
        public bool Save(List<ProductDeliverySet> pds,int delProdId)
        {
            try
            {
                return ds.Save(pds, delProdId) > 0;
            }
            catch (Exception ex)
            {

                throw new Exception("ProductDeliverySetMgr-->Save-->" + ex.Message, ex);
            }
        }

        public List<ProdDeliverySetImport> Save(string filePath,out string resultPath,ProductDeliverySet deliverySet)
        {
            try
            {
                _productDao = new ProductDao(connectionString);
                List<ProdDeliverySetImport> productAll = ExcelHelperXhf.ReadExcel<ProdDeliverySetImport>(filePath);
                //查詢已經存在的
                List<ProductDeliverySet> existProds = ds.Query(productAll.Select(p => uint.Parse(p.ProductId)).Distinct().ToArray(), deliverySet);

                foreach (var prod in productAll)
                {
                    if (existProds.Find(p => p.Product_id == uint.Parse(prod.ProductId)) != null)
                        prod.Status = 2;//2為存在
                }
                
                List<Product> products = _productDao.Query((from p in productAll where p.Status == 0 select uint.Parse(p.ProductId)).Distinct().ToArray());

                foreach (var prod in productAll.FindAll(p => p.Status == 0))
                {
                    var findProd = products.Find(p => p.Product_Id == uint.Parse(prod.ProductId));
                    if (findProd == null)
                    {
                        prod.Status = 4;//該商品不存在
                    }
                    else if (!findProd.CheckdStoreFreight())
                    {
                        prod.Status = 3;//3為不符合条件
                    }
                }

                //符合條件的
                var fitPords = (from p in productAll
                               where p.Status == 0
                               select new ProductDeliverySet
                                      {
                                          Product_id = int.Parse(p.ProductId),
                                          Freight_big_area = deliverySet.Freight_big_area,
                                          Freight_type = deliverySet.Freight_type
                                      }).Distinct();
                if (fitPords.Count()>0&&Save(fitPords.ToList(), 0))
                {
                    productAll.FindAll(p=>p.Status==0).ForEach(p => p.Status = 1);//1保存成功
                }

                #region 將結果保存到excel
                Dictionary<string, string> columns = new Dictionary<string, string>();
                columns.Add("ProductId", "商品ID");
                columns.Add("BrandName", "品牌名稱");
                columns.Add("ProductName", "商品名稱");
                columns.Add("Msg", "結果");
                
                MemoryStream ms = ExcelHelperXhf.ExportExcel(productAll, columns);
                resultPath=Path.GetDirectoryName(filePath)+"\\result.xls";
                FileStream fs = new FileStream(resultPath, FileMode.Create);
                byte[] data = ms.ToArray();
                fs.Write(data, 0, data.Length);
                fs.Dispose(); 
                #endregion

                return productAll;
            }
            catch (Exception ex)
            {

                throw new Exception("ProductDeliverySetMgr-->Save-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 根據上傳的excel批量刪除配送模式
        /// </summary>
        /// <param name="filePath">excel路徑</param>
        /// <param name="deliverySet">要刪除的配送模式</param>
        /// <returns></returns>
        public bool Delete(string filePath,ProductDeliverySet deliverySet)
        {
            try
            {
                _productDao = new ProductDao(connectionString);
                List<ProdDeliverySetImport> productAll = ExcelHelperXhf.ReadExcel<ProdDeliverySetImport>(filePath);
                return Delete(deliverySet, productAll.Select(p => uint.Parse(p.ProductId)).ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDeliverySetMgr-->Delete-->" + ex.Message, ex);
            }
        }


        public MemoryStream ExportProdDeliverySet(ProductDeliverySet query)
        {
            try
            {
                Dictionary<string, string> columns = new Dictionary<string, string>();
                //edit by zhuoqin0830w  2015/04/24  使頁眉顯示為英文字符
                columns.Add("ProductId", "ProductId");
                columns.Add("BrandName", "BrandName");
                columns.Add("ProductName", "ProductName");
                columns.Add("FreightTypeName", "FreightTypeName");
                List<ProductDeliverySetCustom> items = ds.QueryProductDeliveryByCondition(query);
                //edit by zhuoqin0830w  2015/04/24  通過遍歷循環查出的數據進行賦值
                List<ProdDeliverySetImport> list = new List<ProdDeliverySetImport>()
            {
                new ProdDeliverySetImport{
                 ProductId = "商品編號",
                 BrandName = "品牌",
                 ProductName = "商品名稱",
                 FreightTypeName = "物流配送模式"
                }
            };
                foreach (var item in items)
                {
                    list.Add(new ProdDeliverySetImport
                    {
                        ProductId = item.Product_id.ToString(),
                        BrandName = item.Brand_name,
                        ProductName = item.Product_name,
                        FreightTypeName = item.Freight_type_name,
                    });
                }
                return ExcelHelperXhf.ExportExcel(list, columns);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDeliverySetMgr-->ExportProdDeliverySet-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查詢符合條件的ProductDeliverySet集合
        /// </summary>
        /// <param name="pds">查詢的條件</param>
        /// <returns>符合條件的集合</returns>
        public List<ProductDeliverySet> QueryByProductId(int productId)
        {
            try
            {

                return ds.QueryByProductId(productId);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDeliverySetMgr-->QueryByProductId-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 刪除物流配送模式
        /// </summary>
        /// <param name="productId">商品id</param>
        /// <returns></returns>
        public bool Delete(ProductDeliverySet prodDeliSet,params uint[] productIds)
        {
            try
            {
                return ds.Delete(prodDeliSet, productIds) >= 0;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDeliverySetMgr-->Delete-->" + ex.Message, ex);
            }
        }
    }
}
